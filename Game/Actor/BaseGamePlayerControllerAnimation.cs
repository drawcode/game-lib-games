using System;
using System.Collections;
using System.Collections.Generic;

using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;

using UnityEngine;

public enum GamePlayerControllerAnimationType {
    legacy,
    mecanim
}

public class GameDataPlayType {
    public static string loop = "loop";
    public static string loop_reverse = "loop_reverse";
    public static string once = "once";
    public static string once_reverse = "once_reverse";
}

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
    public NavMeshAgent navAgent;
    public Animator animator;
    public Avatar avatar;
    public RuntimeAnimatorController animationController;
    public GameObject actor;
    public GamePlayerControllerAnimationType animationType = 
        GamePlayerControllerAnimationType.legacy;
    public Dictionary<string,GamePlayerAnimationDataItem> items;
    bool animationsLoaded = false;

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

        if(gamePlayerController != null) {
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

                if(actorAnimation != null) {

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

            if (animationItem.last_update + 5 < Time.time) {
                animationItem.last_update = Time.time;
                code = GetDataAnimation(type);                
            }
            else {
                code = animationItem.code;
            }

        }
        else {
            
            //Debug.LogWarning("GetAnimation:WARNING:" + 
            //    " aniType not found" + " type:" + type + " uid:" + 
            //    gamePlayerController.uniqueId); 
        }

        return code;
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
        animationData.navAgent = GetComponent<NavMeshAgent>();
                
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
        
        if(animationData == null) {
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
        
        if(animationData == null) {
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

    Animation actorAnimation;
    
    public virtual void Update() {
        
        if(!gameObjectTimer.IsTimerPerf(
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
                        
            float currentSpeed = 0f;
            
            if (animationData.thirdPersonController != null) {
                currentSpeed = animationData.thirdPersonController.GetSpeed();
            }
            
            if (animationData.gamePlayerController != null) {
                
                if (animationData.gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgent
                    || animationData.gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgentAttack
                    || animationData.gamePlayerController.contextState == GamePlayerContextState.ContextRandom) {
                                        
                    if (animationData.navAgent != null) {
                        if (animationData.navAgent.enabled) {                       
                            //currentSpeed = navAgent.velocity.magnitude + 20;
                            
                            if (animationData.navAgent.velocity.magnitude > 0f) {
                                currentSpeed = 15f;
                            }
                            else {
                                currentSpeed = 0;    
                            }
                            
                            if (animationData.navAgent.remainingDistance < 
                                animationData.navAgent.stoppingDistance + 1) {
                                currentSpeed = 0;
                            }
                            
                            if (currentSpeed < animationData.navAgent.speed) {
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
                Debug.Log("animationData NULL:" + " uniqueId:" + animationData.gamePlayerController.uniqueId);
                return;
            }

            actorAnimation = animationData.actor.GetComponent<Animation>();
            
            if ((actorAnimation == null && animationData.animator == null)) {
                Debug.Log("animationData NULL:" + " uniqueId:" + animationData.gamePlayerController.uniqueId);
                return;
            }
            
            string currentAnimationRun = animationData.animationCodeRun;
            string currentAnimationWalk = animationData.animationCodeWalk;
            string currentAnimationJump = animationData.animationCodeJump;
            
            
            if (isLegacy) {
                if (animationData.actor != null) {
                    if (actorAnimation != null) {
                        if (actorAnimation[currentAnimationRun] != null) {
                            actorAnimation[currentAnimationRun].normalizedSpeed = animationData.runSpeedScale;
                        }
                        if (actorAnimation[currentAnimationWalk] != null) {
                            actorAnimation[currentAnimationWalk].normalizedSpeed = animationData.walkSpeedScale;
                        }
                    }
                }
            }
            
            // Fade in run
            if (currentSpeed > walkSpeed) {
                
                if (isLegacy) {
                    if (animationData.actor != null) {
                        if (actorAnimation != null) {
                            if (actorAnimation[currentAnimationRun] != null) {
                                if (actorAnimation[currentAnimationRun] != null) {
                                    
                                    actorAnimation[currentAnimationRun].blendMode = AnimationBlendMode.Blend;
                                    
                                    if (animationData.thirdPersonController == null) {                      
                                        actorAnimation[currentAnimationRun].normalizedSpeed = 
                                            animationData.runSpeedScale;
                                        //animationData.actor.animation["run"].time = 0f;
                                        actorAnimation.CrossFade(currentAnimationRun, .5f);   
                                    }
                                    else {                       
                                        
                                        if (animationData.thirdPersonController.verticalInput2 != 0f 
                                            || animationData.thirdPersonController.horizontalInput2 != 0f) {
                                            
                                            // if angle between axis is over 120 and less than 240 reverse run
                                            float angleTo = Vector3.Angle(
                                                animationData.thirdPersonController.movementDirection, 
                                                animationData.thirdPersonController.aimingDirection);
                                            
                                            if (angleTo > 120 && angleTo < 240) {
                                                actorAnimation[currentAnimationRun].normalizedSpeed = 
                                                    -animationData.runSpeedScale * .9f;                               
                                            }
                                            else {   
                                                actorAnimation[currentAnimationRun].normalizedSpeed = 
                                                    animationData.runSpeedScale;
                                            }
                                            
                                            //animationData.actor.animation["run"].time = animationData.actor.animation["run"].length;
                                            actorAnimation.Blend(currentAnimationRun);    
                                        }
                                        else {
                                            actorAnimation[currentAnimationRun].normalizedSpeed = 
                                                animationData.runSpeedScale;
                                            //animationData.actor.animation["run"].time = 0f;
                                            actorAnimation.CrossFade(currentAnimationRun, .5f);                       
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // We fade out jumpland quick otherwise we get sliding feet
                    if (actorAnimation[currentAnimationJump] != null) {
                        actorAnimation.CrossFade(currentAnimationJump, 0);
                    }
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, currentSpeed);
                }
                
                SendMessage("SyncAnimation", GameDataActionKeys.run, SendMessageOptions.DontRequireReceiver);
            }
            // Fade in walk
            else if (currentSpeed > 0.1) {
                
                if (isLegacy) {
                    if (animationData.actor != null) {
                        if (actorAnimation != null) {
                            if (actorAnimation[currentAnimationJump] != null) {
                                if (actorAnimation[currentAnimationJump] != null) {
                                    actorAnimation.CrossFade(currentAnimationJump);
                                }
                                // We fade out jumpland realy quick otherwise we get sliding feet
                                actorAnimation.Blend(currentAnimationJump, 0);
                            }
                            if (actorAnimation[currentAnimationWalk] != null) {

                                if (actorAnimation[currentAnimationWalk] != null) {
                                    actorAnimation[currentAnimationWalk].blendMode = AnimationBlendMode.Blend;

                                    if (animationData.thirdPersonController.verticalInput2 != 0f 
                                        || animationData.thirdPersonController.horizontalInput2 != 0f) {
                                        // if angle between axis is over 120 and less than 240 reverse run
                                        float angleTo = Vector3.Angle(
                                            animationData.thirdPersonController.movementDirection, 
                                            animationData.thirdPersonController.aimingDirection);
                                        
                                        if (angleTo > 120 && angleTo < 240) {
                                            actorAnimation[currentAnimationWalk].normalizedSpeed = -animationData.walkSpeedScale * .9f;                             
                                        }
                                        else {   
                                            actorAnimation[currentAnimationWalk].normalizedSpeed = animationData.walkSpeedScale;
                                        }
                                        actorAnimation.Blend(currentAnimationWalk);   
                                    }
                                    else {
                                        actorAnimation[currentAnimationWalk].normalizedSpeed = animationData.walkSpeedScale;
                                        //animationData.actor.animation["run"].time = 0f;
                                        actorAnimation.CrossFade(currentAnimationWalk, .5f);                      
                                    }
                                    
                                    
                                    SendMessage("SyncAnimation", GameDataActionKeys.walk, SendMessageOptions.DontRequireReceiver);
                                }
                            }
                        }
                    }
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, currentSpeed);
                }
            }
            // Fade out walk and run
            else {
                if (isLegacy) {
                    Idle();
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, currentSpeed);
                }
            }
            
            // JUMPING
            
            bool isJumping = false;
            bool isCapeFlying = false;
            bool hasJumpReachedApex = false;
            bool isGroundedWithTimeout = false;
            
            if (animationData.thirdPersonController != null) {
                isJumping = animationData.thirdPersonController.IsJumping();
                isCapeFlying = animationData.thirdPersonController.IsCapeFlying();
                hasJumpReachedApex = animationData.thirdPersonController.HasJumpReachedApex();
                isGroundedWithTimeout = animationData.thirdPersonController.IsGroundedWithTimeout();
            }
            
            if (isJumping) {
                if (isCapeFlying) {
                    Jump();
                }
                else if (hasJumpReachedApex) {
                    Jump();
                }
                else {
                    Jump();
                }
            }
            // We fell down somewhere
            else if (!isGroundedWithTimeout) {
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

}