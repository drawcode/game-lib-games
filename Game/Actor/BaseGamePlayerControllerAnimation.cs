using System;
using System.Collections;
using System.Collections.Generic;

using Engine;
using Engine.Data;
using Engine.Game.App;
using Engine.Game.Controllers;
using Engine.Game.Data;
using Engine.Networking;
using Engine.Utility;

using UnityEngine;

public enum GamePlayerControllerAnimationType {
    legacy,
    mecanim
}

//public class GameDataPlayType {
//    public static string loop = "loop";
//    public static string loop_reverse = "loop_reverse";
//    public static string once = "once";
//    public static string once_reverse = "once_reverse";
//}

public class GamePlayerAnimationDataItem : GameDataObject {

    public virtual AnimationState animation_state {
        get {
            return Get<AnimationState>(BaseDataObjectKeys.animation_state);
        }

        set {
            Set<AnimationState>(BaseDataObjectKeys.animation_state, value);
        }
    }

    /*
    public virtual Animation animation {
        get { 
            return Get<Animation>(BaseDataObjectKeys.animation);
        }
        
        set {
            Set<Animation>(BaseDataObjectKeys.animation, value);
        }
    }

    public virtual Animator animator {
        get { 
            return Get<Animator>(BaseDataObjectKeys.animator);
        }
        
        set {
            Set<Animator>(BaseDataObjectKeys.animator, value);
        }
    }
    */

    public virtual GamePlayerControllerAnimationType animation_type {
        get {
            return Get<GamePlayerControllerAnimationType>(BaseDataObjectKeys.animation_type);
        }

        set {
            Set<GamePlayerControllerAnimationType>(BaseDataObjectKeys.animation_type, value);
        }
    }
}

[Serializable]
public class BaseGamePlayerControllerAnimationData {

    public bool initialized = false;
    public float runSpeedScale = 1.2f;
    public float walkSpeedScale = 1.0f;
    public bool isRunningClampAnimation = false;
    public bool isRunning = false;
    public bool isDead = false;
    public GamePlayerController gamePlayerController;
    public UnityEngine.AI.NavMeshAgent navAgent;
    public Animator animator;
    public Avatar avatar;
    public RuntimeAnimatorController animationController;
    public GameObject actor;
    public GamePlayerControllerAnimationType animationType =
        GamePlayerControllerAnimationType.legacy;
    public Dictionary<string, GamePlayerAnimationDataItem> items;
    bool animationsLoaded = false;

    //

    public Animation actorAnimation;

    // The actor object actorAnimation was resolved from. Used to re-resolve on actor swap
    // instead of calling GetComponent every frame.
    public GameObject actorAnimationResolvedFor;

    public bool isJumping;
    public bool isSliding;
    public bool isCapeFlying;
    public bool hasJumpReachedApex;
    public bool isGroundedWithTimeout;

    public float currentSpeed;

    public float angleTo;
    public float walkSpeed;

    public string currentAnimationRun;
    public string currentAnimationWalk;
    public string currentAnimationJump;
    public string currentAnimationSlide;

    //

    public bool isLegacy {
        get {
            if (animationType == GamePlayerControllerAnimationType.legacy) {
                return true;
            }
            return false;
        }
    }

    public bool isMecanim {
        get {
            if (animationType == GamePlayerControllerAnimationType.mecanim) {
                return true;
            }
            return false;
        }
    }

    public GamePlayerThirdPersonController thirdPersonController {
        get {

            if (gamePlayerController == null) {
                return null;
            }

            if (gamePlayerController.controllerData == null) {
                return null;
            }

            if (gamePlayerController.controllerData.thirdPersonController == null) {
                return null;
            }

            return gamePlayerController.controllerData.thirdPersonController;
        }
    }

    // SPEED-FOLLOWING ANIMATION CADENCE
    //
    // A legacy AnimationState's normalizedSpeed is cycles per second, and it was written as a
    // CONSTANT here -- runSpeedScale / walkSpeedScale -- so the leg cycle ran at one fixed rate
    // whatever the actor was actually doing. The controller's speed is not fixed: moveSpeed
    // lerps up from a standstill and swaps target between walkSpeed and trotSpeed after
    // trotAfterSeconds, so the feet slid against the ground whenever the two disagreed.
    //
    // The cadence is now the authored scale times the ratio of real speed to the speed that
    // scale describes, clamped so it can neither freeze nor blur into a scribble.
    public float animationSpeedCycleMin = .45f;
    public float animationSpeedCycleMax = 1.75f;

    // Set per frame by the update: true when currentSpeed is the third person controller's own
    // integrated speed, false when the NavMeshAgent branch has replaced it with its 0-or-15
    // stand-in. Kept for anything outside this repo that reads it.
    public bool speedFromController = false;

    // The cadence inputs, set per frame, and the reason agents animate at a real pace now.
    //
    // currentSpeed CANNOT be used for this. It is the branch selector -- `currentSpeed >
    // walkSpeed` picks run over walk -- and for a NavMeshAgent actor it is deliberately a
    // 0-or-15 stand-in that forces the run branch. Feeding the agent's true velocity into
    // currentSpeed would drop every enemy into the walk clip instead. So the cadence gets its
    // OWN pair of values and the branch selector is left exactly as it was:
    //
    //   speedCycleActual    - the real speed the leg cycle should follow
    //   speedCycleReference - the speed the authored *SpeedScale values describe
    //
    // A reference of 0 means "no continuous speed available", and the cadence falls back to the
    // authored constant.
    public float speedCycleActual = 0f;
    public float speedCycleReference = 0f;

    // The fastest this actor has actually been SEEN to travel, with a slow decay.
    //
    // This exists because NavMeshAgent.speed is a CEILING the agents never come near: measured
    // live, configured speeds of 13-22 against real velocities of 1.9-4.6. Using it as the
    // reference put every ratio (0.03-0.15) far below animationSpeedCycleMin, so every agent
    // clamped to the floor -- which just replaced one constant cadence with a slower constant.
    //
    // A high-water mark is self-calibrating instead: at cruise the actor IS its own maximum, so
    // the cycle sits at 1.0 and sustained movement looks exactly as it does today; when it slows
    // to turn, re-path or close the last metre of its route, the ratio drops and the legs slow
    // with it. The decay lets a one-off velocity spike bleed off instead of pinning the
    // reference high forever, and lets the mark follow an actor whose speed is changed at runtime.
    public float speedCycleObservedMax = 0f;
    public float speedCycleObservedDecay = .995f;

    // The speed the authored *SpeedScale values describe. trotSpeed is what the controller
    // settles at while moving -- walkSpeed only applies for the first trotAfterSeconds -- so
    // anchoring there leaves sustained movement looking as it does today and changes only the
    // ramp in and out of it.
    public float GetSpeedCycleReference() {

        if (thirdPersonController == null) {
            return 0f;
        }

        float reference = thirdPersonController.trotSpeed;

        if (reference <= .01f) {
            reference = thirdPersonController.walkSpeed;
        }

        return reference;
    }

    public float GetSpeedCycleScale() {

        if (speedCycleReference <= .01f) {
            return 1f;
        }

        return Mathf.Clamp(
            speedCycleActual / speedCycleReference,
            animationSpeedCycleMin, animationSpeedCycleMax);
    }

    // properties / helpers

    public string animationCodeIdle {
        get {
            return GetAnimation(GameDataActionKeys.idle);
        }
    }

    public string animationCodeWalk {
        get {
            return GetAnimation(GameDataActionKeys.walk);
        }
    }

    public string animationCodeWalkBack {
        get {
            return GetAnimation(GameDataActionKeys.walk_back);
        }
    }

    public string animationCodeRun {
        get {
            return GetAnimation(GameDataActionKeys.run);
        }
    }

    public string animationCodeRunBack {
        get {
            return GetAnimation(GameDataActionKeys.run_back);
        }
    }

    //

    public string animationCodeSlide {
        get {
            return GetAnimation(GameDataActionKeys.slide);
        }
    }

    //

    public string animationCodeJump {
        get {
            return GetAnimation(GameDataActionKeys.jump);
        }
    }

    //    

    public string animationCodeSkill {
        get {
            return GetAnimation(GameDataActionKeys.skill);
        }
    }

    //    

    public string animationCodeSpin {
        get {
            return GetAnimation(GameDataActionKeys.spin);
        }
    }

    //    

    public string animationCodeBoost {
        get {
            return GetAnimation(GameDataActionKeys.boost);
        }
    }

    //

    public string animationCodeAttack {
        get {
            return GetAnimation(GameDataActionKeys.attack);
        }
    }

    public string animationCodeAttackAlt {
        get {
            return GetAnimation(GameDataActionKeys.attack_alt);
        }
    }

    public string animationCodeAttackFar {
        get {
            return GetAnimation(GameDataActionKeys.attack_far);
        }
    }

    public string animationCodeAttackNear {
        get {
            return GetAnimation(GameDataActionKeys.attack_near);
        }
    }

    public string animationCodeAttackRight {
        get {
            return GetAnimation(GameDataActionKeys.attack_right);
        }
    }

    public string animationCodeAttackLeft {
        get {
            return GetAnimation(GameDataActionKeys.attack_left);
        }
    }

    //

    public string animationCodeDefend {
        get {
            return GetAnimation(GameDataActionKeys.defend);
        }
    }

    public string animationCodeDefendAlt {
        get {
            return GetAnimation(GameDataActionKeys.defend_alt);
        }
    }

    public string animationCodeDefendFar {
        get {
            return GetAnimation(GameDataActionKeys.defend_far);
        }
    }

    public string animationCodeDefendNear {
        get {
            return GetAnimation(GameDataActionKeys.defend_near);
        }
    }

    public string animationCodeDefendRight {
        get {
            return GetAnimation(GameDataActionKeys.defend_right);
        }
    }

    public string animationCodeDefendLeft {
        get {
            return GetAnimation(GameDataActionKeys.defend_left);
        }
    }

    //

    public string animationCodeStrafeRight {
        get {
            return GetAnimation(GameDataActionKeys.strafe_right);
        }
    }

    public string animationCodeStrafeLeft {
        get {
            return GetAnimation(GameDataActionKeys.strafe_left);
        }
    }

    //

    public string animationCodeHit {
        get {
            return GetAnimation(GameDataActionKeys.hit);
        }
    }

    //

    public string animationCodeDeath {
        get {
            return GetAnimation(GameDataActionKeys.death);
        }
    }

    //

    public BaseGamePlayerControllerAnimationData() {
        Reset();
    }

    public void Reset() {
        isDead = false;
        isRunning = true;

        //

        actorAnimation = null;

        isJumping = false;
        isSliding = false;
        isCapeFlying = false;
        hasJumpReachedApex = false;
        isGroundedWithTimeout = false;

        currentSpeed = 0f;
        angleTo = 0;
        walkSpeed = 5f;

        currentAnimationRun = null;
        currentAnimationWalk = null;
        currentAnimationJump = null;
        currentAnimationSlide = null;
    }

    // LOADING/FIND CHARACTER

    public virtual void LoadAnimatedActor() {

        //if (actorItem != null) {

        actor = null;
        animator = null;
        avatar = null;
        animationController = null;
        animationType = GamePlayerControllerAnimationType.legacy;
        animationsLoaded = false;

        if (gamePlayerController != null) {
            actor = gamePlayerController.gamePlayerModelHolderModel;
        }

        FindAnimatedActor();
        //}
    }

    public virtual void FindAnimatedActor() {

        if (actor != null) {

            bool loadAnimations = false;

            // MECANIM
            if (animator == null && !animationsLoaded || !animationsLoaded) {

                foreach (Animator anim in actor.GetComponentsInChildren<Animator>()) {

                    if (anim.runtimeAnimatorController != null
                        && anim.avatar != null) {

                        animator = anim;
                        actor = anim.gameObject;
                        animationType = GamePlayerControllerAnimationType.mecanim;
                        avatar = anim.avatar;
                        animationController = anim.runtimeAnimatorController;

                        loadAnimations = true;
                        animationsLoaded = true;
                    }
                }
            }

            // LEGACY TYPE
            if (!animationsLoaded) {

                foreach (Animation anim in actor.GetComponentsInChildren<Animation>()) {

                    actor = anim.gameObject;
                    animationType = GamePlayerControllerAnimationType.legacy;

                    loadAnimations = true;
                    animationsLoaded = true;
                }
            }

            if (loadAnimations) {
                LoadGamePlayerAnimations();
            }
        }
        else {
            //Debug.LogWarning("FindAnimatedActor:WARNING:" + 
            //    " actor IS NULL" + 
            //    gamePlayerController.uniqueId); 
        }
    }

    // LOADING ANIMATION DATA

    public void LoadGamePlayerAnimationData() {

        if (items == null) {
            items = new Dictionary<string, GamePlayerAnimationDataItem>();
        }
        else {
            items.Clear();
        }

        foreach (GameDataAnimation item in gamePlayerController.gameCharacter.data.animations) {

            GamePlayerAnimationDataItem itemData = new GamePlayerAnimationDataItem();

            itemData.code = GetDataAnimation(item.type);
            itemData.type = item.type;
            itemData.last_update = Time.time;
            itemData.layer = item.layer;
            itemData.play_type = item.play_type;

            items.Set(item.type, itemData);
        }
    }

    public void LoadGamePlayerAnimations() {

        LoadGamePlayerAnimationData();

        if (actor != null) {

            // LEGACY TYPE   
            if (isLegacy) {

                Animation actorAnimation = actor.GetComponent<Animation>();

                if (actorAnimation != null) {

                    actorAnimation.Stop();

                    foreach (GamePlayerAnimationDataItem aniItem in items.Values) {

                        //if (aniItem.animation_state == null) { // || aniItem.animator == null) {

                        if (animationType == GamePlayerControllerAnimationType.mecanim) {
                            aniItem.animation_type = animationType;
                            //aniItem.animator = animator;
                        }
                        else {

                            if (actorAnimation[aniItem.code] != null) {

                                //aniItem.animation = actor.animation;
                                aniItem.animation_state = actorAnimation[aniItem.code];
                                aniItem.animation_state.layer = aniItem.layer;
                                aniItem.animation_type = animationType;

                                if (aniItem.play_type == GameDataPlayType.loop) {
                                    aniItem.animation_state.wrapMode = WrapMode.Loop;
                                }
                                else {
                                    aniItem.animation_state.wrapMode = WrapMode.Once;
                                }
                            }
                        }
                    }
                    //}
                }
            }
        }

        isRunning = true;
        isDead = false;
    }

    public GamePlayerAnimationDataItem GetAnimationData(string key) {

        if (items == null) {
            items = new Dictionary<string, GamePlayerAnimationDataItem>();
        }
        else if (items.ContainsKey(key)) {

            return items.Get(key);
        }
        else {

            //Debug.LogWarning("GetAnimationData:WARNING:" + 
            //    " GamePlayerAnimationDataItem not found" + " key:" + key + " uid:" + 
            //    gamePlayerController.uniqueId);
        }

        return null;
    }

    public string GetAnimation(string type) {

        string code = GameDataActionKeys.idle;

        if (items == null) {
            code = GameDataActionKeys.idle;
        }
        else if (items.ContainsKey(type)) {

            GamePlayerAnimationDataItem animationItem = items.Get<GamePlayerAnimationDataItem>(type);

            code = animationItem.code;

            // The once-a-second re-roll below returns a random variant for ONE call and never
            // stores it. It costs ~309 allocations / 12.6 KB each time -- 94% of everything
            // GamePlayerControllerAnimation.Update allocated (gameplay-tuning iteration 19).
            //
            // It is NOT dead, though: character data lists variants the model does not have (a
            // droid given idle_04), and the play calls return early on a missing clip. For those
            // actors the re-roll is the only thing that ever starts a clip that exists, which
            // then keeps playing. So skip it only when the stored clip is already on the actor --
            // there the re-roll only produced a one-frame cross-fade to another clip and back.
            if (animationItem.last_update + 1f < Time.time
                && !IsClipOnResolvedActor(code)) {

                animationItem.last_update = Time.time;
                code = GetDataAnimation(type);
            }

        }
        else {

            //Debug.LogWarning("GetAnimation:WARNING:" + 
            //    " aniType not found" + " type:" + type + " uid:" + 
            //    gamePlayerController.uniqueId); 
        }

        return code;
    }

    // Legacy only, and only through the Animation Update already resolved for the CURRENT actor:
    // anything else answers false, which keeps GetAnimation's original re-roll behaviour.
    bool IsClipOnResolvedActor(string code) {

        if (!isLegacy
            || actor == null
            || actorAnimation == null
            || actorAnimationResolvedFor != actor
            || string.IsNullOrEmpty(code)) {
            return false;
        }

        return actorAnimation[code] != null;
    }

    public string GetDataAnimation(string type) {

        string code = "idle";

        if (gamePlayerController.gameCharacter != null) {

            GameDataAnimation data =
                gamePlayerController.gameCharacter.data.GetAnimationByType(
                    type);

            if (data != null) {
                code = data.code;
            }
        }

        return code;
    }

    // ANIMATIONS

    public void ResetPlayState() {

        if (isMecanim) {

            if (animator == null) {
                return;
            }

            animator.ResetFloat(GameDataActionKeys.speed);
            animator.ResetFloat(GameDataActionKeys.death);
            animator.ResetFloat(GameDataActionKeys.strafe);
            animator.ResetFloat(GameDataActionKeys.jump);
            animator.ResetFloat(GameDataActionKeys.attack);
            animator.ResetFloat(GameDataActionKeys.hit);
            animator.ResetFloat(GameDataActionKeys.slide);
        }
        else {
            PlayAnimationIdle();
        }
    }

    //

    public virtual void PauseAnimationUpdate(float duration) {
        CoroutineUtil.Start(PauseAnimationUpdateCo(duration));
    }

    public IEnumerator PauseAnimationUpdateCo(float duration) {
        yield return new WaitForSeconds(duration);
        isRunningClampAnimation = false;
    }

    // BLEND PLAY

    public void PlayAnimationBlend(
        string type, float weight, float time,
        AnimationBlendMode blendMode = AnimationBlendMode.Additive) {

        string currentAnimation = GetAnimation(type);

        if (isLegacy) {

            if (actor == null) {
                return;
            }

            Animation actorAnimation = actor.GetComponent<Animation>();

            if (actorAnimation == null) {
                return;
            }

            if (actorAnimation[currentAnimation] == null) {
                return;
            }

            if (isRunningClampAnimation) {
                return;
            }

            actorAnimation[currentAnimation].blendMode = blendMode;
            actorAnimation.Blend(currentAnimation, weight, time);

        }
    }

    // CROSS FADE PLAY

    public void PlayAnimationCrossFade(
        string type, float time, PlayMode playMode) {

        string currentAnimation = GetAnimation(type);

        if (isLegacy) {

            if (actor == null) {
                return;
            }

            Animation actorAnimation = actor.GetComponent<Animation>();

            if (actorAnimation == null) {
                return;
            }

            if (actorAnimation[currentAnimation] == null) {
                return;
            }

            if (isRunningClampAnimation) {
                return;
            }

            actorAnimation.CrossFade(currentAnimation, time, playMode);

        }
    }

    public void AnimationClamp(float time = .5f) {
        isRunningClampAnimation = true;
        PauseAnimationUpdate(time);
    }

    // GENERIC PLAY

    public void PlayAnimation(string type, PlayMode playMode) {

        string currentAnimation = GetAnimation(type);

        if (isLegacy) {

            if (actor == null) {
                return;
            }

            Animation actorAnimation = actor.GetComponent<Animation>();

            if (actorAnimation == null) {
                return;
            }

            if (actorAnimation[currentAnimation] == null) {
                return;
            }

            actorAnimation.Play(currentAnimation, playMode);
        }
    }

    // idle

    public void PlayAnimationIdle() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            PlayAnimationCrossFade(GameDataActionKeys.idle, .5f, PlayMode.StopAll);
        }
        else if (isMecanim) {
            ResetPlayState();

            //animator.PlayOneShotFloat(GameDataActionKeys.idle);
        }
    }

    // jump

    public void PlayAnimationJump() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            PlayAnimationCrossFade(GameDataActionKeys.jump, .2f, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.jump);
        }
    }

    // slide

    public void PlayAnimationSlide() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            PlayAnimationCrossFade(GameDataActionKeys.slide, .2f, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.slide);
        }
    }

    // hit

    public void PlayAnimationHit() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            PlayAnimationCrossFade(GameDataActionKeys.hit, .1f, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.attack, .6f, 0f);
        }
    }

    // DIE

    public void PlayAnimationDeath() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            PlayAnimation(GameDataActionKeys.death, PlayMode.StopAll);
            AnimationClamp(.5f);
            isDead = true;
        }
        else if (isMecanim) {
            ResetPlayState();
            AnimationClamp(.5f);
            animator.SetFloat(GameDataActionKeys.death, 1f);
            isDead = true;
        }
    }

    // ATTACK

    // base

    public void PlayAnimationAttack(string type) {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimationBlend(type, .8f, .5f, AnimationBlendMode.Additive);
            //PlayAnimation(type, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.SetFloat(GameDataActionKeys.speed, .7f);
            animator.PlayOneShotFloat(GameDataActionKeys.attack, .8f, 0f);
        }
    }

    // attack - specific

    public void PlayAnimationAttack() {
        PlayAnimationAttack(GameDataActionKeys.attack);
    }

    public void PlayAnimationAttackAlt() {
        PlayAnimationAttack(GameDataActionKeys.attack_alt);
    }

    public void PlayAnimationAttackFar() {
        PlayAnimationAttack(GameDataActionKeys.attack_far);
    }

    public void PlayAnimationAttackLeft() {
        PlayAnimationAttack(GameDataActionKeys.attack_left);
    }

    public void PlayAnimationAttackRight() {
        PlayAnimationAttack(GameDataActionKeys.attack_right);
    }

    public void PlayAnimationAttackNear() {
        PlayAnimationAttack(GameDataActionKeys.attack_near);
    }

    // DEFEND

    // base

    public void PlayAnimationDefend(string type) {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimationBlend(type, .7f, .5f, AnimationBlendMode.Additive);
        }
        else if (isMecanim) {
            animator.SetFloat(GameDataActionKeys.speed, .7f);
            animator.PlayOneShotFloat(GameDataActionKeys.attack, .8f, 0f);
        }
    }

    // attack - specific

    public void PlayAnimationDefend() {
        PlayAnimationDefend(GameDataActionKeys.defend);
    }

    public void PlayAnimationDefendAlt() {
        PlayAnimationDefend(GameDataActionKeys.defend_alt);
    }

    public void PlayAnimationDefendFar() {
        PlayAnimationDefend(GameDataActionKeys.defend_far);
    }

    public void PlayAnimationDefendLeft() {
        PlayAnimationDefend(GameDataActionKeys.defend_left);
    }

    public void PlayAnimationDefendRight() {
        PlayAnimationDefend(GameDataActionKeys.defend_right);
    }

    public void PlayAnimationDefendNear() {
        PlayAnimationDefend(GameDataActionKeys.defend_near);
    }

    // STRAFE

    // left

    public void PlayAnimationStrafeLeft() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimation(GameDataActionKeys.strafe_left, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.strafe, -1f);
        }
    }

    // right

    public void PlayAnimationStrafeRight() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimation(GameDataActionKeys.strafe_right, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.strafe, 1f);
        }
    }

    // BOOST

    public void PlayAnimationBoost() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimation(GameDataActionKeys.boost, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.boost, 1f);
        }
    }

    // SPIN

    public void PlayAnimationSpin() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimation(GameDataActionKeys.spin, PlayMode.StopSameLayer);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.spin, 1f);
        }
    }

    // SPIN

    public void PlayAnimationSkill() {

        if (isDead) {
            return;
        }

        if (isLegacy) {
            AnimationClamp(.5f);
            PlayAnimationBlend(GameDataActionKeys.skill, .5f, .6f, AnimationBlendMode.Blend);
        }
        else if (isMecanim) {
            animator.PlayOneShotFloat(GameDataActionKeys.skill, 1f);
        }
    }
}

public class BaseGamePlayerControllerAnimation : GameObjectTimerBehavior {

    public GamePlayerControllerAnimationData animationData;

    public bool isLegacy {
        get {
            return animationData.isLegacy;
        }
    }

    public bool isMecanim {
        get {
            return animationData.isMecanim;
        }
    }

    public virtual void Awake() {

    }

    public virtual void Start() {

    }

    public virtual void Init() {

        animationData = new GamePlayerControllerAnimationData();

        animationData.gamePlayerController = GetComponent<GamePlayerController>();
        animationData.navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (animationData.gamePlayerController != null) {
            animationData.gamePlayerController.LoadAnimatedActor();
        }
    }

    public virtual void ResetPlayState() {

        if (animationData == null) {
            return;
        }

        animationData.ResetPlayState();
    }

    public virtual void HandleAnimatorState() {
        ResetPlayState();
    }

    public virtual void Reset() {

        if (animationData == null) {
            return;
        }

        animationData.Reset();

        animationData.ResetPlayState();
    }

    public virtual void LoadAnimatedActor() {

        if (animationData == null) {
            return;
        }

        animationData.LoadAnimatedActor();
    }

    // ATTACK

    public virtual void Attack() {
        Attack(GameDataActionKeys.attack);
    }

    public virtual void AttackAlt() {
        Attack(GameDataActionKeys.attack_alt);
    }

    public virtual void AttackLeft() {
        Attack(GameDataActionKeys.attack_left);
    }

    public virtual void AttackRight() {
        Attack(GameDataActionKeys.attack_right);
    }

    public virtual void AttackNear() {
        Attack(GameDataActionKeys.attack_near);
    }

    public virtual void AttackFar() {
        Attack(GameDataActionKeys.attack_far);
    }

    public virtual void Attack(string animationName) {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationAttack(animationName);

        SendMessage("SyncAnimation",
                    GameDataActionKeys.attack,
                    SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // ACTIONS - DEFEND

    public virtual void Defend() {
        Defend(GameDataActionKeys.defend);
    }

    public virtual void DefendAlt() {
        Defend(GameDataActionKeys.defend_alt);
    }

    public virtual void DefendLeft() {
        Defend(GameDataActionKeys.defend_left);
    }

    public virtual void DefendRight() {
        Defend(GameDataActionKeys.defend_right);
    }

    public virtual void DefendNear() {
        Defend(GameDataActionKeys.defend_near);
    }

    public virtual void DefendFar() {
        Defend(GameDataActionKeys.defend_far);
    }

    public virtual void Defend(string animationName) {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationDefend(animationName);

        SendMessage("SyncAnimation",
                    GameDataActionKeys.defend,
                    SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // ACTIONS - HIT 

    public virtual void Hit() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationHit();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.hit,
                    SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // ACTIONS - DIE 

    public virtual void Die() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationDeath();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.death,
                    SendMessageOptions.DontRequireReceiver);
    }


    // --------------
    // ACTIONS - IDLE    

    public virtual void Idle() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationIdle();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.idle,
                    SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // ACTIONS - JUMP    

    public virtual void Jump() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationJump();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.jump,
                    SendMessageOptions.DontRequireReceiver);

        //animationData.actor.animation.CrossFade("jetpackjump", 0.2f);
        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
        //animationData.actor.animation.CrossFade("jumpfall", 0.2f);
        //SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);

    }

    public virtual void Land() {

        // animationData.actor.animation.Play("jumpland");
        // SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
        // SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // ACTIONS - SLIDE    

    public virtual void Slide() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationSlide();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.slide,
                    SendMessageOptions.DontRequireReceiver);

        //animationData.actor.animation.CrossFade("jetpackjump", 0.2f);
        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
        //animationData.actor.animation.CrossFade("jumpfall", 0.2f);
        //SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);

    }

    // --------------

    // ACTIONS - STRAFE LEFT

    public virtual void StrafeLeft() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationStrafeLeft();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.strafe_left,
                    SendMessageOptions.DontRequireReceiver);
    }

    // --------------

    // ACTIONS - STRAFE RIGHT

    public virtual void StrafeRight() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationStrafeRight();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.strafe_right,
                    SendMessageOptions.DontRequireReceiver);
    }


    // --------------

    // ACTIONS - BOOST

    public virtual void Boost() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationBoost();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.boost,
                    SendMessageOptions.DontRequireReceiver);
    }


    // --------------

    // ACTIONS - SPIN

    public virtual void Spin() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        animationData.PlayAnimationSpin();

        SendMessage("SyncAnimation",
                    GameDataActionKeys.spin,
                    SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // ACTIONS - SKILL   

    public virtual void Skill() {

        if (animationData == null) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        float currentSpeed = 0f;
        float walkSpeed = 0f;

        if (animationData.thirdPersonController != null) {
            currentSpeed = animationData.thirdPersonController.GetSpeed();
            walkSpeed = animationData.thirdPersonController.walkSpeed;
        }

        if (currentSpeed > walkSpeed) {
            animationData.PlayAnimationSkill();
        }
        else if (currentSpeed > 0.1) {
            animationData.PlayAnimationSkill();
        }
        else {
            animationData.PlayAnimationSkill();
        }

        SendMessage("SyncAnimation", GameDataActionKeys.skill, SendMessageOptions.DontRequireReceiver);
    }

    // --------------
    // TOOLS / HELPERS / UTILS 

    public virtual void PlayOneShotBool(string paramName) {
        StartCoroutine(PlayOneShotBoolCo(paramName));
    }

    public virtual IEnumerator PlayOneShotBoolCo(string paramName) {

        if (animationData == null) {
            yield break;
        }

        if (!isLegacy) {

            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetBool(paramName, true);
                yield return null;
                animationData.animator.SetBool(paramName, false);
            }
        }
    }

    public void SetBool(string key, bool val) {

        if (animationData == null) {
            return;
        }

        if (isMecanim) {
            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetBool(key, val);
            }
        }
    }

    public void SetFloat(string key, float val) {

        if (animationData == null) {
            return;
        }

        if (isMecanim) {
            if (animationData.animator != null) {
                animationData.animator.SetFloat(key, val);
            }
        }
    }

    public void PlayOneShotFloat(string key) {

        if (animationData == null) {
            return;
        }

        if (isMecanim) {
            if (animationData.animator != null) {
                animationData.animator.PlayOneShotFloat(key);
            }
        }

    }

    // --------------
    // ACTIONS - HIT 

    public virtual void ApplyDamage() {
        Hit();
    }

    // --------------
    // ACTIONS - EXTRA   

    public virtual void ButtStomp() {
        //animationData.actor.animation.CrossFade("buttstomp", 0.1f);
        //SendMessage("SyncAnimation", "buttstomp", SendMessageOptions.DontRequireReceiver);
        //animationData.actor.animation.CrossFadeQueued("jumpland", 0.2f);
    }

    public virtual void WallJump() {
        // Wall jump animation is played without fade.
        // We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
        // But we really have to make sure that the animation is in full control so 
        // that we don't do weird blends between 180 degree apart rotations

        Animation actorAnimation = animationData.actor.GetComponent<Animation>();

        if (actorAnimation != null) {
            if (actorAnimation["walljump"] != null) {
                actorAnimation.Play("walljump");
                SendMessage("SyncAnimation", GameDataActionKeys.walljump);
            }
        }
    }

    // --------------
    // GAME TICK / UPDATE  

    public virtual void Update() {

        if (!gameObjectTimer.IsTimerPerf(
            GameObjectTimerKeys.gameUpdateAll,
            animationData.gamePlayerController.IsPlayerControlled ? 1f : 2f)) {
            return;
        }

        if (!GameConfigs.isGameRunning || GameConfigs.isUIRunning) {
            return;
        }

        if (animationData.isDead) {
            return;
        }

        if (animationData.isRunning) {

            animationData.currentSpeed = 0f;
            animationData.speedFromController = false;
            animationData.speedCycleActual = 0f;
            animationData.speedCycleReference = 0f;

            if (animationData.thirdPersonController != null) {
                animationData.currentSpeed = animationData.thirdPersonController.GetSpeed();
                animationData.speedFromController = true;

                // Player-side cadence: follow the controller's integrated speed, measured
                // against the speed it settles at while moving.
                animationData.speedCycleActual = animationData.currentSpeed;
                animationData.speedCycleReference = animationData.GetSpeedCycleReference();
            }

            if (animationData.gamePlayerController != null) {

                if (animationData.gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgent
                    || animationData.gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgentAttack
                    || animationData.gamePlayerController.contextState == GamePlayerContextState.ContextRandom) {

                    if (animationData.navAgent != null) {
                        if (animationData.navAgent.enabled) {
                            //currentSpeed = navAgent.velocity.magnitude + 20;

                            animationData.speedFromController = false;

                            // Agent-side cadence. The 0-or-15 stand-in below is kept as the
                            // BRANCH selector, but the leg cycle now follows the agent's real
                            // velocity against its own configured speed -- so an enemy that is
                            // slowed, turning, or closing the last metre of its path animates at
                            // the pace it is actually travelling instead of one fixed rate.
                            // This is what made every bot's run cycle look constant: the branch
                            // above reported "moving at 15" and the cadence had nothing else to
                            // read, so it fell back to the authored constant for every agent.
                            float agentVelocity = animationData.navAgent.velocity.magnitude;

                            animationData.speedCycleObservedMax = Mathf.Max(
                                animationData.speedCycleObservedMax
                                    * animationData.speedCycleObservedDecay,
                                agentVelocity);

                            animationData.speedCycleActual = agentVelocity;
                            animationData.speedCycleReference =
                                animationData.speedCycleObservedMax;

                            if (animationData.navAgent.velocity.magnitude > 0f) {
                                animationData.currentSpeed = 15f;
                            }
                            else {
                                animationData.currentSpeed = 0;
                            }

                            if (animationData.navAgent.remainingDistance <
                                animationData.navAgent.stoppingDistance + 1) {
                                animationData.currentSpeed = 0;
                            }

                            if (animationData.currentSpeed < animationData.navAgent.speed) {
                                //currentSpeed = 0;
                            }
                        }
                    }
                }
            }

            float walkSpeed = 5f;

            //LogUtil.Log("currentSpeed:" + currentSpeed);
            if (animationData.thirdPersonController != null) {
                walkSpeed = animationData.thirdPersonController.walkSpeed;
                //LogUtil.Log("currentSpeed:" + thirdPersonController.walkSpeed);
            }

            if (animationData.actor == null) {
                ////Debug.Log("animationData NULL:" + " uniqueId:" + animationData.gamePlayerController.uniqueId);
                return;
            }

            // Re-resolved only when the ACTOR CHANGED, not every frame. This was an unconditional
            // GetComponent per frame, per actor. It cannot simply be cached once either: the actor
            // model is swapped (customisation, pooled reuse), and the old code's one virtue was
            // that it always matched the current actor. Keying the cache on the actor object keeps
            // that property at one reference comparison a frame.
            // ALSO re-resolve while the cache is still EMPTY on a legacy actor. Keying only on
            // "the actor changed" latched a NULL: an actor's model gets its Animation added
            // asynchronously (the model load / InitControlsCo on spawn), so an actor resolved
            // before its model finished loading stored actorAnimation == null, and because the
            // actor reference never changed again it was never re-read. The early-out below then
            // returned every frame and THAT ACTOR NEVER ANIMATED AGAIN for the rest of the round
            // -- measured live on the player: resolvedFor == actor, actorAnimation == null, while
            // actor.GetComponent<Animation>() returned a real component.
            // A legacy actor is expected to have one, so retrying until it appears is correct and
            // self-limiting; mecanim actors keep the cached-once behaviour and never re-look.
            if (animationData.actorAnimationResolvedFor != animationData.actor
                || (animationData.actorAnimation == null && animationData.isLegacy)) {
                animationData.actorAnimation = animationData.actor.GetComponent<Animation>();
                animationData.actorAnimationResolvedFor = animationData.actor;
            }

            if ((animationData.actorAnimation == null && animationData.animator == null)) {
                ////Debug.Log("animationData NULL:" + " uniqueId:" + animationData.gamePlayerController.uniqueId);
                return;
            }

            animationData.currentAnimationRun = animationData.animationCodeRun;
            animationData.currentAnimationWalk = animationData.animationCodeWalk;
            animationData.currentAnimationJump = animationData.animationCodeJump;
            animationData.currentAnimationSlide = animationData.animationCodeSlide;

            // Cycles per second for this frame: the authored cadence scaled by how fast the
            // actor is really moving. Constant before -- which is what made the run cycle keep
            // one pace while the character sped up and slowed down.
            float speedCycleScale = animationData.GetSpeedCycleScale();
            float runCycleSpeed = animationData.runSpeedScale * speedCycleScale;
            float walkCycleSpeed = animationData.walkSpeedScale * speedCycleScale;

            if (isLegacy) {
                if (animationData.actor != null) {
                    if (animationData.actorAnimation != null) {

                        // Four allocating indexer calls a frame became two. See the note in the
                        // fade-in-run block below -- Animation[string] allocates every time.
                        AnimationState runStateSpeed =
                            animationData.actorAnimation[animationData.currentAnimationRun];
                        AnimationState walkStateSpeed =
                            animationData.actorAnimation[animationData.currentAnimationWalk];

                        if (runStateSpeed != null) {
                            runStateSpeed.normalizedSpeed = runCycleSpeed;
                        }
                        if (walkStateSpeed != null) {
                            walkStateSpeed.normalizedSpeed = walkCycleSpeed;
                        }
                    }
                }
            }

            // Fade in run
            if (animationData.currentSpeed > walkSpeed) {

                if (isLegacy) {

                    if (animationData.actor != null) {

                        if (animationData.actorAnimation != null) {

                            // Animation's string indexer allocates an AnimationState wrapper on
                            // EVERY call. This block used to call it up to five times a frame per
                            // actor -- including the identical lookup twice in a row for a
                            // duplicated null check. Resolve once and reuse.
                            AnimationState runState =
                                animationData.actorAnimation[animationData.currentAnimationRun];

                            if (runState != null) {

                                runState.blendMode = AnimationBlendMode.Blend;

                                if (animationData.thirdPersonController == null) {
                                    runState.normalizedSpeed = runCycleSpeed;
                                    //animationData.actor.animation["run"].time = 0f;
                                    animationData.actorAnimation.CrossFade(animationData.currentAnimationRun, .5f);
                                }
                                else {

                                    if (animationData.thirdPersonController.verticalInput2 != 0f
                                        || animationData.thirdPersonController.horizontalInput2 != 0f) {

                                        // if angle between axis is over 120 and less than 240 reverse run
                                        animationData.angleTo = Vector3.Angle(
                                            animationData.thirdPersonController.movementDirection,
                                            animationData.thirdPersonController.aimingDirection);

                                        if (animationData.angleTo > 120 && animationData.angleTo < 240) {
                                            runState.normalizedSpeed = -runCycleSpeed * .9f;
                                        }
                                        else {
                                            runState.normalizedSpeed = runCycleSpeed;
                                        }

                                        animationData.actorAnimation.Blend(animationData.currentAnimationRun);
                                    }
                                    else {
                                        runState.normalizedSpeed = runCycleSpeed;
                                        //animationData.actor.animation["run"].time = 0f;
                                        animationData.actorAnimation.CrossFade(animationData.currentAnimationRun, .5f);
                                    }
                                }
                            }
                        }
                    }
                    // We fade out jumpland quick otherwise we get sliding feet.
                    //
                    // The two lookups that stood here resolved the jump and slide AnimationStates
                    // only to test them against null -- both bodies are commented out, so they were
                    // two allocating Animation indexer calls per frame doing nothing. They also sat
                    // OUTSIDE the actorAnimation != null guard above, so on a Mecanim-less actor
                    // with only an animator they were a latent NullReferenceException as well.
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, animationData.currentSpeed);
                }

                SyncAnimationMessage(GameDataActionKeys.run);
            }
            // Fade in walk
            else if (animationData.currentSpeed > 0.1) {

                if (isLegacy) {
                    if (animationData.actor != null) {
                        if (animationData.actorAnimation != null) {

                            // Same treatment as the fade-in-run block above: Animation's string
                            // indexer allocates an AnimationState on EVERY call, and this block
                            // made ten of them a frame per actor -- jump twice, slide twice, walk
                            // six times -- three of which were identical lookups repeated purely
                            // for a duplicated null check. Ten is now ONE, with the jump and slide
                            // lookups gone entirely (see below).
                            AnimationState walkState =
                                animationData.actorAnimation[animationData.currentAnimationWalk];

                            // The jump and slide cross-fades that stood here are GONE, matching
                            // the fade-in-run block above (where the same pair was removed as
                            // dead). They were only ever dead by accident: jump/strafe entries
                            // carried no "data_type": "preset", so the entry's own CODE was used
                            // as the clip name, no model had a clip called "animation-jump-bot-1",
                            // the lookup returned null and the bodies never ran.
                            //
                            // Authoring the presets correctly woke them up, and they are wrong:
                            // jump is authored on LAYER 5 and run/walk/idle on LAYER 1, so in
                            // legacy Animation the jump clip MASKS locomotion. Cross-fading it in
                            // (then blending it straight back to 0) on every tick the actor spends
                            // in the walk band left walking actors fighting their own jump clip.
                            // The player's walkSpeed is 24, so that band is most of normal movement.
                            // A real jump still animates -- the JUMPING block below calls Jump().
                            if (walkState != null) {

                                walkState.blendMode = AnimationBlendMode.Blend;

                                // GUARDED, which it was not before. The fade-in-run block above
                                // null-checks thirdPersonController and the JUMPING block below
                                // does too; this one dereferenced it bare, so an actor with a walk
                                // clip but no third-person controller -- any agent -- threw here on
                                // every frame it was walking. Falling through to the else is what
                                // the run block does in the same situation.
                                if (animationData.thirdPersonController != null
                                    && (animationData.thirdPersonController.verticalInput2 != 0f
                                        || animationData.thirdPersonController.horizontalInput2 != 0f)) {

                                    // if angle between axis is over 120 and less than 240 reverse run
                                    animationData.angleTo = Vector3.Angle(
                                        animationData.thirdPersonController.movementDirection,
                                        animationData.thirdPersonController.aimingDirection);

                                    if (animationData.angleTo > 120 && animationData.angleTo < 240) {
                                        walkState.normalizedSpeed = -walkCycleSpeed * .9f;
                                    }
                                    else {
                                        walkState.normalizedSpeed = walkCycleSpeed;
                                    }

                                    animationData.actorAnimation.Blend(animationData.currentAnimationWalk);
                                }
                                else {
                                    walkState.normalizedSpeed = walkCycleSpeed;
                                    //animationData.actor.animation["run"].time = 0f;
                                    animationData.actorAnimation.CrossFade(animationData.currentAnimationWalk, .5f);
                                }

                                SyncAnimationMessage(GameDataActionKeys.walk);
                            }
                        }
                    }
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, animationData.currentSpeed);
                }
            }
            // Fade out walk and run
            else {
                if (isLegacy) {
                    Idle();
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, animationData.currentSpeed);
                }
            }

            // JUMPING

            if (animationData.thirdPersonController != null) {
                animationData.isJumping = animationData.thirdPersonController.IsJumping();
                animationData.isSliding = animationData.thirdPersonController.IsSliding();
                animationData.isCapeFlying = animationData.thirdPersonController.IsCapeFlying();
                animationData.hasJumpReachedApex = animationData.thirdPersonController.HasJumpReachedApex();
                animationData.isGroundedWithTimeout = animationData.thirdPersonController.IsGroundedWithTimeout();
            }

            if (animationData.isJumping) {

                if (animationData.isCapeFlying) {
                    Jump();
                }
                else if (animationData.hasJumpReachedApex) {
                    Jump();
                }
                else {
                    Jump();
                }
            }
            else if (animationData.isSliding) {

                if (animationData.isCapeFlying) {
                    Slide();
                }
                else {
                    Slide();
                }
            }
            // We fell down somewhere
            else if (!animationData.isGroundedWithTimeout) {

                //animationData.actor.animation.CrossFade("ledgefall", 0.2f);
                //SendMessage("SyncAnimation", "ledgefall", SendMessageOptions.DontRequireReceiver);
            }
            // We are not falling down anymore
            else {
                //animationData.actor.animation.Blend("ledgefall", 0.0f, 0.2f);
            }
        }
    }


    // BASE

    /*
        isRunningClampAnimation = true;
        PauseAnimationUpdate(1f);

        //float currentSpeed = 0f;
        //float walkSpeed = 0f;
     
        if(thirdPersonController != null) {
            //currentSpeed = thirdPersonController.GetSpeed();
            //walkSpeed = thirdPersonController.walkSpeed;
        }

        if(isLegacy) {

            if(animationData.actor.animation != null) {
                if(animationData.actor.animation[animationName]) {
    
                    animationData.actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
                 
                    if(thirdPersonController != null) {
                        if(thirdPersonController.verticalInput2 == 0 && thirdPersonController.horizontalInput2 == 0) {
                            animationData.actor.animation.CrossFade(animationName);
                        }
                        else if(thirdPersonController.verticalInput2 < .5f
                         && thirdPersonController.horizontalInput2 < .5f
                         && thirdPersonController.verticalInput2 > -.5f
                         && thirdPersonController.horizontalInput2 > -.5f) {
                            animationData.actor.animation.Blend(animationName, .8f);
                        }
                        else {
                            animationData.actor.animation.Blend(animationName, .7f);
                     
                        }
                    }
                    else {
                        animationData.actor.animation.Blend(animationName, .7f);
    
                    }
                }
            }
        }
        else {

        }
        */

    // SendMessage does a reflection lookup for the handler on EVERY call, and the two run/walk
    // sites below call it once a frame for as long as an actor is moving -- per actor.
    //
    // The receivers ("SyncAnimation") are the networking components: NetworkSyncAnimation and
    // GameNetworkPlayerContainer. Neither is present on a single-player actor, so in the common
    // case every one of those calls was paying the lookup to find nothing. All the OTHER
    // SendMessage("SyncAnimation") sites in this class are event driven -- jump, skill, walljump --
    // and are left as they are; the cost only matters where it repeats per frame.
    //
    // Probed by reflection rather than by referencing the types, because the receivers live in
    // game-lib-engine and game-lib-gameverses and gameverses is behind a compile flag here.
    private bool syncAnimationReceiverChecked = false;
    private System.Action<string> syncAnimationCall = null;

    protected virtual void SyncAnimationMessage(string animationValue) {

        if (!syncAnimationReceiverChecked) {

            syncAnimationReceiverChecked = true;

            MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();

            for (int i = 0; i < behaviours.Length; i++) {

                if (behaviours[i] == null) {
                    continue;
                }

                System.Reflection.MethodInfo method = behaviours[i].GetType().GetMethod(
                    "SyncAnimation",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                    null,
                    new System.Type[] { typeof(string) },
                    null);

                if (method == null) {
                    continue;
                }

                // Bound ONCE into a delegate. MethodInfo.Invoke would have to box the argument
                // into a fresh object[] on every call, which would just trade SendMessage's
                // per-frame cost for a per-frame allocation.
                syncAnimationCall = (System.Action<string>)System.Delegate.CreateDelegate(
                    typeof(System.Action<string>), behaviours[i], method, false);

                if (syncAnimationCall != null) {
                    break;
                }
            }
        }

        // Nothing on this object handles it -- which is what DontRequireReceiver was papering
        // over. Skip the call entirely rather than paying the lookup to find nothing.
        if (syncAnimationCall == null) {
            return;
        }

        syncAnimationCall(animationValue);
    }

}