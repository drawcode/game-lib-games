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

public class BaseGamePlayerAnimationType {
    public static string walk = "walk";
    public static string run = "run";
    public static string attack = "attack";
    public static string attackAlt = "attack-alt";
    public static string attackRight = "attack-right";
    public static string attackLeft = "attack-left";
    public static string speed = "speed";
    public static string jump = "jump";
    public static string defend = "defend";
    public static string defendAlt = "defend-alt";
    public static string defendRight = "defend-right";
    public static string defendLeft = "defend-left";
    public static string hit = "hit";
    public static string die = "die";
    public static string death = "death";
    public static string skill = "skill";
    public static string magic = "magic";
    public static string use = "use";
    public static string idle = "idle";
    public static string emo = "emo";
    public static string strafe = "strafe";
    public static string strafeLeft = "strafe-left";
    public static string strafeRight = "strafe-right";
    public static string boost = "boost";
    public static string spin = "spin";
}

public class BaseGamePlayerAnimationData {
    public string skill = "emo_0";
    public string jump = "jump_0";
    public string run = "run_0";
    public string walk = "walk_0";
    public string attack = "action_0";
    public string idle = "idle_0";
    public string hit = "hit_0";
    public string death = "death_0";
    public int skillNum = 0;
    public int jumpNum = 0;
    public int idleNum = 0;
    public int attackNum = 0;
    public int runNum = 0;
    public int walkNum = 0;
    public int hitNum = 0;
    public int deathNum = 0;
        
        
    // SKILL
        
    public int SkillCount() {
        return 6;
    }
        
    public string Skill() {
        if(skillNum == 0) {
            skillNum = UnityEngine.Random.Range(1, SkillCount());
        }       
        return Skill(skillNum);
    }
        
    public string Skill(int num) {
        return skill + num;
    }
        
    // IDLE
        
    public int JumpCount() {
        return 3;
    }
        
    public string Jump() {
        if(jumpNum == 0) {
            jumpNum = UnityEngine.Random.Range(1, JumpCount());
        }       
        return Jump(jumpNum);
    }
        
    public string Jump(int num) {
        return jump + num;
    }
        
    // IDLE
        
    public int IdleCount() {
        return 5;
    }
        
    public string Idle() {
        if(idleNum == 0) {
            idleNum = UnityEngine.Random.Range(1, IdleCount());
        }
        return Idle(idleNum);
    }
        
    public string Idle(int num) {
        return idle + num;
    }
        
    // ATTACK
        
    public int AttackCount() {
        return 5;
    }
        
    public string Attack() {
        if(attackNum == 0) {
            attackNum = UnityEngine.Random.Range(1, AttackCount());
        }       
        return Attack(attackNum);
    }
        
    public string Attack(int num) {
        return attack + num;
    }
        
    // RUN
        
    public int RunCount() {
        return 3;
    }
        
    public string Run() {
        if(runNum == 0) {
            runNum = UnityEngine.Random.Range(1, RunCount());
        }       
        return Run(runNum);
    }
        
    public string Run(int num) {
        return run + num;
    }
        
    // WALK
        
    public int WalkCount() {
        return 5;
    }
        
    public string Walk() {
        if(walkNum == 0) {
            walkNum = UnityEngine.Random.Range(1, WalkCount());
        }       
        return Walk(walkNum);
    }
        
    public string Walk(int num) {
        return walk + num;
    }
        
    // HIT
        
    public int HitCount() {
        return 3;
    }
        
    public string Hit() {
        if(hitNum == 0) {
            hitNum = UnityEngine.Random.Range(1, HitCount());
        }       
        return Hit(hitNum);
    }
        
    public string Hit(int num) {
        return hit + num;
    }
                
    // DEATH
        
    public int DeathCount() {
        return 2;
    }
        
    public string Death() {
        if(deathNum == 0) {
            deathNum = UnityEngine.Random.Range(1, DeathCount());
        }       
        return Death(deathNum);
    }
        
    public string Death(int num) {
        return death + num;
    }
}

public class BaseGamePlayerControllerAnimation : MonoBehaviour {

    public float runSpeedScale = 1.2f;
    public float walkSpeedScale = 1.0f;
    public Transform torso;
    public GameObject actor;
    public bool isRunningClampAnimation = false;
    public bool isRunning = false;
    public bool isDead = false;
    public GamePlayerController gamePlayerController;
    public GamePlayerThirdPersonController thirdPersonController;
    public NavMeshAgent navAgent;
    public GamePlayerControllerAnimationType animationType = GamePlayerControllerAnimationType.legacy;
    public AnimationState animationRun;
    public AnimationState animationWalk;
    public AnimationState animationIdle;
    public AnimationState animationHit;
    public AnimationState animationDie;
    public AnimationState animationJump;
    public AnimationState animationAttack;
    public AnimationState animationAttackAlt;
    public AnimationState animationAttackLeft;
    public AnimationState animationAttackRight;
    public Animator animator;
    public Avatar avatar;
    public RuntimeAnimatorController animationController;
 
    public bool isLegacy {
        get {
            if(animationType == GamePlayerControllerAnimationType.legacy) { 
                return true;
            }
            return false;
        }
    }
 
    public bool isMecanim {
        get {
            if(animationType == GamePlayerControllerAnimationType.mecanim) { 
                return true;
            }
            return false;
        }
    }
 
    public virtual void Awake() {
        Init();
    }
 
    public virtual void Init() {
        thirdPersonController = GetComponent<GamePlayerThirdPersonController>();
        navAgent = GetComponent<NavMeshAgent>();
        gamePlayerController = GetComponent<GamePlayerController>();
        Reset();
    }
     
    public virtual void ResetPlayState() {
        isDead = false;
     
        if(!isLegacy) {
            animator.SetFloat(GamePlayerAnimationType.speed, 0f);
            animator.SetFloat(GamePlayerAnimationType.death, 0f);
            animator.SetFloat(GamePlayerAnimationType.strafe, 0f);
            animator.SetFloat(GamePlayerAnimationType.jump, 0f);
            animator.SetFloat(GamePlayerAnimationType.attack, 0f);
            animator.SetFloat(GamePlayerAnimationType.hit, 0f);
        }
    }
 
    public virtual void Reset() {
        if(actor != null) {

            FindAnimatedActor();
                 
            // LEGACY TYPE   
            if(actor.animation != null) {
                         
                // By default loop all animations
                actor.animation.wrapMode = WrapMode.Loop;
                actor.animation.Stop();
             
                if(animationRun == null) {
                    if(actor.animation["run"] != null) {
                        animationRun = actor.animation["run"];
                        animationAttack.layer = 1;
                    }
                }
             
                if(animationWalk == null) {
                    if(actor.animation["walk"] != null) {
                        animationWalk = actor.animation["walk"];
                        animationAttack.layer = 1;
                    }
                }
             
                if(animationIdle == null) {
                    if(actor.animation["idle"] != null) {
                        animationIdle = actor.animation["idle"];
                        animationAttack.layer = 1;
                    }
                }
             
                if(animationHit == null) {
                    if(actor.animation["hit"] != null) {
                        animationHit = actor.animation["hit"];
                        animationAttack.layer = 2;
                    }
                }
             
                if(animationAttack == null) {
                    if(actor.animation["attack"] != null) {
                        animationAttack = actor.animation["attack"];
                        animationAttack.layer = 2;
                    }                    
                }
             
                if(animationAttackAlt == null) {
                    if(actor.animation["attack-alt"] != null) {
                        animationAttackAlt = actor.animation["attack-alt"];
                        animationAttack.layer = 2;
                    }                    
                }
             
                if(animationAttackLeft == null) {
                    if(actor.animation["attack-left"] != null) {
                        animationAttackLeft = actor.animation["attack-left"];
                        animationAttack.layer = 2;
                    }                    
                }
             
                if(animationAttackRight == null) {
                    if(actor.animation["attack-right"] != null) {
                        animationAttackRight = actor.animation["attack-right"];
                        animationAttack.layer = 2;
                    }                    
                }
             
                AnimationStatePlay(animationIdle);
            }
                     
        }
     
        isRunning = true;
        isDead = false;
    }
 
    public virtual void AnimationPlay(Animation ani) {
        if(ani == null) {
            return;
        }
        ani.Play();
    }
 
    public virtual void AnimationCrossFade(Animation ani, Animation aniTo) {
        if(ani == null) {
            return;
        }
     
        if(aniTo == null) {
            return;
        }
     
        if(actor == null) {
            return;
        }
     
        if(actor.animation == null) {
            return;
        }

        if(actor.animation[aniTo.name] != null) {
            ani.CrossFade(aniTo.name);
        }
    }
 
    public virtual void AnimationBlend(Animation ani, Animation aniTo) {
        AnimationBlend(ani, aniTo, .5f, .5f);
    }
 
    public virtual void AnimationBlend(Animation ani, Animation aniTo, float targetWeight) {
        AnimationBlend(ani, aniTo, targetWeight, .5f);
    }
 
    public virtual void AnimationBlend(Animation ani, Animation aniTo, float targetWeight, float fadeLength) {
        if(ani == null) {
            return;
        }
     
        if(aniTo == null) {
            return;
        }
     
        if(actor == null) {
            return;
        }
     
        if(actor.animation == null) {
            return;
        }

        if(actor.animation[aniTo.name] != null) {
            ani.Blend(aniTo.name, targetWeight, fadeLength);
        }
    }
 
    public virtual void AnimationStatePlay(AnimationState ani) {
        if(ani == null) {
            return;
        }
     
        if(actor == null) {
            return;
        }
     
        if(actor.animation == null) {
            return;
        }
     
        actor.animation.Play(ani.name);
    }

    public virtual void AnimationStateCrossFade(AnimationState ani, AnimationState aniTo) {
        if(ani == null) {
            return;
        }
     
        if(aniTo == null) {
            return;
        }
     
        if(actor == null) {
            return;
        }
     
        if(actor.animation == null) {
            return;
        }

        if(actor.animation[aniTo.name] != null) {
            actor.animation.CrossFade(aniTo.name);
        }
    }
 
    public virtual void AnimationStateBlend(AnimationState ani, AnimationState aniTo) {
        AnimationStateBlend(ani, aniTo, .8f, .5f);
    }
 
    public virtual void AnimationStateBlend(AnimationState ani, AnimationState aniTo, float targetWeight) {
        AnimationStateBlend(ani, aniTo, targetWeight, .5f);
    }
 
    public virtual void AnimationStateBlend(AnimationState ani, AnimationState aniTo, float targetWeight, float fadeLength) {
        if(ani == null) {
            return;
        }
     
        if(aniTo == null) {
            return;
        }
     
        if(actor == null) {
            return;
        }
     
        if(actor.animation == null) {
            return;
        }

        if(actor.animation[aniTo.name] != null) {
            actor.animation.Blend(aniTo.name, targetWeight, fadeLength);
        }
    }
 
    public virtual void ResetAnimatedActor(GameObject actorItem) {
        actor = actorItem;
        FindAnimatedActor();
    }
 
    public virtual void FindAnimatedActor() {
        if(actor != null) {                      

            // LEGACY TYPE
            if(actor.animation == null) {
                foreach(Animation anim in actor.GetComponentsInChildren<Animation>()) {
                    actor = anim.gameObject;
                    animationType = GamePlayerControllerAnimationType.legacy;
                    break;
                }
            }
         
            // MECANIM
            if(animator == null) {
                foreach(Animator anim in actor.GetComponentsInChildren<Animator>()) {
                    animator = anim;
                    actor = anim.gameObject;
                    animationType = GamePlayerControllerAnimationType.mecanim;
                    avatar = anim.avatar;
                    animationController = anim.runtimeAnimatorController;
                    break;
                }
            }
        }
    }

    public virtual void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }
     
        if(isDead) {
            return;
        }
             
        if(isRunning) {
         
            //try {
         
            var currentSpeed = 0f;
         
            if(thirdPersonController != null) {
                currentSpeed = thirdPersonController.GetSpeed();
            }
         
            //Debug.Log("currentSpeed:" + currentSpeed);
            //Debug.Log("navAgent:" + navAgent);
         
            if(gamePlayerController != null) {
                if(gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgent
                 || gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgentAttack
                 || gamePlayerController.contextState == GamePlayerContextState.ContextRandom) {
                    if(navAgent != null) {
                        if(navAgent.enabled) {                       
                            //currentSpeed = navAgent.velocity.magnitude + 20;
                         
                            if(navAgent.velocity.magnitude > 0f) {
                                currentSpeed = 15f;
                            }
                            else {
                                currentSpeed = 0;    
                            }
                         
                            if(navAgent.remainingDistance < navAgent.stoppingDistance + 1) {
                                currentSpeed = 0;
                            }
                         
                            if(currentSpeed < navAgent.speed) {
                                //currentSpeed = 0;
                            }
                        }
                    }
                }
            }
         
            float walkSpeed = 5f;
         
            //Debug.Log("currentSpeed:" + currentSpeed);
            if(thirdPersonController != null) {
                walkSpeed = thirdPersonController.walkSpeed;
                //Debug.Log("currentSpeed:" + thirdPersonController.walkSpeed);
            }
         
            FindAnimatedActor();
         
            if(actor == null || (actor.animation == null && animator == null)) {
                return;
            }
         
            if(isLegacy) {
                try {
                    if(actor.animation["run"] != null) {
                        actor.animation["run"].normalizedSpeed = runSpeedScale;
                    }
                    if(actor.animation["walk"] != null) {
                        actor.animation["walk"].normalizedSpeed = walkSpeedScale;
                    }
                }
                catch(System.Exception e) {
                    LogUtil.Log(e);
                }
            }
         
            // Fade in run
            if(currentSpeed > walkSpeed) {
             
                if(isLegacy) {
                    if(actor.animation["run"] != null) {
                        if(actor.animation["run"] != null) {
                            actor.animation["run"].blendMode = AnimationBlendMode.Blend;
                         
                            if(thirdPersonController == null) {                      
                                actor.animation["run"].normalizedSpeed = runSpeedScale;
                                //actor.animation["run"].time = 0f;
                                actor.animation.CrossFade("run", .5f);   
                            }
                            else {                       
                         
                                if(thirdPersonController.verticalInput2 != 0f 
                                 || thirdPersonController.horizontalInput2 != 0f) {
                                 
                                    // if angle between axis is over 120 and less than 240 reverse run
                                    float angleTo = Vector3.Angle(thirdPersonController.movementDirection, thirdPersonController.aimingDirection);
                                 
                                    if(angleTo > 120 && angleTo < 240) {
                                        actor.animation["run"].normalizedSpeed = -runSpeedScale * .9f;                               
                                    }
                                    else {   
                                        actor.animation["run"].normalizedSpeed = runSpeedScale;
                                    }
                                 
                                    //actor.animation["run"].time = actor.animation["run"].length;
                                    actor.animation.Blend("run");    
                                }
                                else {
                                    actor.animation["run"].normalizedSpeed = runSpeedScale;
                                    //actor.animation["run"].time = 0f;
                                    actor.animation.CrossFade("run", .5f);                       
                                }
                            }
                        }
                    }
                    // We fade out jumpland quick otherwise we get sliding feet
                    if(actor.animation["jump"] != null) {
                        actor.animation.CrossFade("jump", 0);
                    }
                }
                else if(isMecanim) {
                    animator.SetFloat("speed", currentSpeed);
                }
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }
         // Fade in walk
            else if(currentSpeed > 0.1) {
             
                if(isLegacy) {
                    if(actor.animation["jump"] != null) {
                        if(actor.animation["jump"] != null) {
                            actor.animation.CrossFade("jump");
                        }
                        // We fade out jumpland realy quick otherwise we get sliding feet
                        actor.animation.Blend("jump", 0);
                    }
                    if(actor.animation["walk"] != null) {
                        if(actor.animation["walk"] != null) {
                            actor.animation["walk"].blendMode = AnimationBlendMode.Blend;
                            if(thirdPersonController.verticalInput2 != 0f 
                             || thirdPersonController.horizontalInput2 != 0f) {
                                // if angle between axis is over 120 and less than 240 reverse run
                                float angleTo = Vector3.Angle(thirdPersonController.movementDirection, thirdPersonController.aimingDirection);
                                                         
                                if(angleTo > 120 && angleTo < 240) {
                                    actor.animation["walk"].normalizedSpeed = -walkSpeedScale * .9f;                             
                                }
                                else {   
                                    actor.animation["walk"].normalizedSpeed = walkSpeedScale;
                                }
                                actor.animation.Blend("walk");   
                            }
                            else {
                                actor.animation["walk"].normalizedSpeed = walkSpeedScale;
                                //actor.animation["run"].time = 0f;
                                actor.animation.CrossFade("walk", .5f);                      
                            }
                         
                         
                            SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
                else if(isMecanim) {
                    if(animator != null) {
                        animator.SetFloat(GamePlayerAnimationType.speed, currentSpeed);
                    }
                }
            }
         // Fade out walk and run
            else {
                if(isLegacy) {
                    AnimateIdle();
                }
                else if(isMecanim) {
                    if(animator != null) {
                        animator.SetFloat(GamePlayerAnimationType.speed, currentSpeed);
                    }
                }
            }
         
            // JUMPING
         
            bool isJumping = false;
            bool isCapeFlying = false;
            bool hasJumpReachedApex = false;
            bool isGroundedWithTimeout = false;
         
            if(thirdPersonController != null) {
                isJumping = thirdPersonController.IsJumping();
                isCapeFlying = thirdPersonController.IsCapeFlying();
                hasJumpReachedApex = thirdPersonController.HasJumpReachedApex();
                isGroundedWithTimeout = thirdPersonController.IsGroundedWithTimeout();
            }
         
            if(isJumping) {
                if(isCapeFlying) {
                    AnimateJump();
                }
                else if(hasJumpReachedApex) {
                    AnimateJump();
                }
                else {
                    AnimateJump();
                }
            }
         // We fell down somewhere
            else if(!isGroundedWithTimeout) {
                //actor.animation.CrossFade("ledgefall", 0.2f);
                //SendMessage("SyncAnimation", "ledgefall", SendMessageOptions.DontRequireReceiver);
            }
         // We are not falling down anymore
            else {
                //actor.animation.Blend("ledgefall", 0.0f, 0.2f);
            }
        }
    }    
 
    // --------------
    // JUMP
 
    public virtual void AnimateJump() {
        if(actor == null) {
            return;
        }
     
        if(isLegacy) {
			
			if(actor.animation != null) {
	            if(actor.animation["jump"] != null) {
	                actor.animation.CrossFade("jump", 0.2f);
	                SendMessage("SyncAnimation", "jump", 
	                 SendMessageOptions.DontRequireReceiver);
	            }
			}
        }
        else if(isMecanim) {
            if(animator != null) {
                PlayOneShotFloat(GamePlayerAnimationType.jump);
            }
        }
     
        //actor.animation.CrossFade("jetpackjump", 0.2f);
        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
        //actor.animation.CrossFade("jumpfall", 0.2f);
        //SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);
    }
 
    public virtual void ResetClampAnimation() {
        isRunningClampAnimation = false; 
    }
 
    public virtual void PauseAnimationUpdate(float duration) {
        Invoke("ResetClampAnimation", duration);
    }

    public virtual void DidLand() {
        if(actor == null) {
            return;
        }

        if(isLegacy) {
            //actor.animation.Play("jumpland");
            //SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
			
			if(actor.animation != null) {
	            if(actor.animation["jump"] != null) {
	                actor.animation.Play("jump");
	                SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
	            }
			}
        }
        else if(isMecanim) {
            if(animator != null) {
                animator.SetFloat(GamePlayerAnimationType.jump, 1f);
            }
        }

    }
 
    // --------------
    // IDLE
 
    public virtual void AnimateIdle() {
        if(actor == null) {
            return;
        }

        if(isLegacy) {
			
			if(actor.animation != null) {
	            if(actor.animation["idle"] != null) {
	                if(actor.animation["idle"] != null
	                 && !isRunningClampAnimation) {
	                    actor.animation.CrossFade("idle");
	                    SendMessage("SyncAnimation", "idle",
	                     SendMessageOptions.DontRequireReceiver);
	                }
	            }
			}
        }
        else if(isMecanim) {
            if(animator != null) {
                ResetPlayState();
            }
        }
    }
 
    // --------------
    // HIT
 
    public virtual void AnimateHit() {       
        if(actor == null) {
            return;
        }

        if(isLegacy) {
            if(actor.animation["hit"] != null) {
                actor.animation.CrossFade("hit", 0.1f);
                SendMessage("SyncAnimation", "hit", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if(isMecanim) {
            if(animator != null) {
                animator.SetFloat(GamePlayerAnimationType.attack, 1f);
            }
        }
    }
 
    // --------------
    // ACTIONS - ATTACK
 
    public virtual void Attack() {       
        DidAttack(); 
    }
 
    public virtual void AttackAlt() {        
        DidAttackAlt();  
    }
 
    public virtual void AttackLeft() {       
        DidAttackLeft(); 
    }
 
    public virtual void AttackRight() {      
        DidAttackRight();    
    }

    public virtual void DidAttack() {        
        DidAttack(GamePlayerAnimationType.attack);
    }
 
    public virtual void DidAttackAlt() {
        DidAttack("attack-alt");
    }
 
    public virtual void DidAttackLeft() {        
        DidAttack("attack-left");
    }
 
    public virtual void DidAttackRight() {       
        DidAttack("attack-right");
    }
 
    public virtual void DidAttack(string animationName) {
     
        if(isDead) {
            return;
        }

        if(isLegacy) {
            if(actor.animation != null) {
                if(actor.animation[animationName] != null) {
                    isRunningClampAnimation = true;
                    PauseAnimationUpdate(.5f);
                    actor.animation.Play(animationName, PlayMode.StopAll);
                    actor.animation.Play("hit", PlayMode.StopAll);
                }
            }
        }
        else {
            if(!isDead) {
               // animator.SetFloat(GamePlayerAnimationType.speed, .8f);
                PlayOneShotFloat(GamePlayerAnimationType.attack);
            }
        }

        /*
        //LogUtil.Log("GamePlayerControllerAnimation:DidAttack:" + animationName);
     
        isRunningClampAnimation = true;
        PauseAnimationUpdate(1f);            
     
        //float currentSpeed = 0f;
        //float walkSpeed = 0f;
     
        if(thirdPersonController != null) {
            //currentSpeed = thirdPersonController.GetSpeed();
            //walkSpeed = thirdPersonController.walkSpeed;
        }
     
        if(actor.animation != null) {
            if(actor.animation[animationName]) {
             
                actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
             
                if(thirdPersonController != null) {
                    if(thirdPersonController.verticalInput2 == 0 && thirdPersonController.horizontalInput2 == 0) {
                        actor.animation.CrossFade(animationName);            
                    }
                    else if(thirdPersonController.verticalInput2 < .5f 
                     && thirdPersonController.horizontalInput2 < .5f
                     && thirdPersonController.verticalInput2 > -.5f
                     && thirdPersonController.horizontalInput2 > -.5f) {
                        actor.animation.Blend(animationName, .8f);           
                    }
                    else {
                        actor.animation.Blend(animationName, .7f); 
                 
                    }
                }
                else {
                    actor.animation.Blend(animationName, .7f); 
             
                }
            }
        }
        */
    }

    // DEFEND


    public virtual void Defend() {
        DidDefend();
    }
 
    public virtual void DefendAlt() {
        DidDefendAlt();
    }

    public virtual void DefendLeft() {
        DidDefendLeft();
    }

    public virtual void DefendRight() {
        DidDefendRight();
    }

    public virtual void DidDefend() {
        DidDefend(GamePlayerAnimationType.defend);
    }
 
    public virtual void DidDefendAlt() {
        DidDefend(GamePlayerAnimationType.defendAlt);
    }
 
    public virtual void DidDefendLeft() {
        DidDefend(GamePlayerAnimationType.defendLeft);
    }
 
    public virtual void DidDefendRight() {
        DidDefend(GamePlayerAnimationType.defendRight);
    }
 
    public virtual void DidDefend(string animationName) {
     
        if(isDead) {
            return;
        }

        if(isLegacy) {
			
			if(actor.animation != null) {
	            if(actor.animation[animationName] != null) {
	                isRunningClampAnimation = true;
	                PauseAnimationUpdate(.5f);
	                actor.animation.Play(animationName, PlayMode.StopAll);
	                actor.animation.Play("hit", PlayMode.StopAll);
	            }
			}
        }
        else {
            if(!isDead) {
                animator.SetFloat(GamePlayerAnimationType.speed, .8f);
                animator.SetFloat("defend", 1f);
            }
        }

        //LogUtil.Log("GamePlayerControllerAnimation:DidAttack:" + animationName);

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

            if(actor.animation != null) {
                if(actor.animation[animationName]) {
    
                    actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
                 
                    if(thirdPersonController != null) {
                        if(thirdPersonController.verticalInput2 == 0 && thirdPersonController.horizontalInput2 == 0) {
                            actor.animation.CrossFade(animationName);
                        }
                        else if(thirdPersonController.verticalInput2 < .5f
                         && thirdPersonController.horizontalInput2 < .5f
                         && thirdPersonController.verticalInput2 > -.5f
                         && thirdPersonController.horizontalInput2 > -.5f) {
                            actor.animation.Blend(animationName, .8f);
                        }
                        else {
                            actor.animation.Blend(animationName, .7f);
                     
                        }
                    }
                    else {
                        actor.animation.Blend(animationName, .7f);
    
                    }
                }
            }
        }
        else {

        }
        */
    }
 
    // --------------
    // ACTIONS - DIE 
 
    public virtual void Die() {  
        DidDie();
    }
 
    public virtual void DidDie() {   
        if(isDead) {
            return;
        }
     
		if(isLegacy) {
			if(actor.animation != null) {
	            if(actor.animation["death"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);  
	                actor.animation.Play("death", PlayMode.StopAll);
	                actor.animation.Play("hit", PlayMode.StopAll);
	            }
			}
        }
        else {
            if(!isDead) {
                animator.SetFloat(GamePlayerAnimationType.speed, 0f);
                animator.SetFloat(GamePlayerAnimationType.death, 1f);
                isDead = true;
            }
        }
    }

    
    // --------------
    // ACTIONS - IDLE    
    
    public virtual void Idle() {
        DidIdle();
    }
    
    public virtual void DidIdle() {
        if(isDead) {
            return;
        }
        
        if(isLegacy) {
			if(actor.animation != null) {
	            if(actor.animation["idle"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);  
	                actor.animation.Play("idle", PlayMode.StopAll);
	            }
			}
        }
        else {
            ResetFloat(GamePlayerAnimationType.attack);
            ResetFloat(GamePlayerAnimationType.hit);
            ResetFloat(GamePlayerAnimationType.strafe);
            ResetFloat(GamePlayerAnimationType.speed);
            ResetFloat(GamePlayerAnimationType.run);
            ResetFloat(GamePlayerAnimationType.walk);

            PlayOneShotFloat(GamePlayerAnimationType.idle);
        }
    }
 
    // --------------
    // ACTIONS - JUMP    
 
    public virtual void Jump() {
        DidJump();
    }
    
    public virtual void DidJump() {
        if(isDead) {
            return;
        }
     
		if(isLegacy) {
			if(actor.animation != null) {
	            if(actor.animation["jump"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);  
	                actor.animation.Play("jump", PlayMode.StopAll);
	            }
			}
        }
        else {
            PlayOneShotFloat(GamePlayerAnimationType.jump);
        }
    }


    // --------------

    // ACTIONS - STRAFE LEFT

    public virtual void StrafeLeft() {
        DidStrafeLeft();
    }

    public virtual void DidStrafeLeft() {
        if(isDead) {
            return;
        }

		if(isLegacy) {
			if(actor.animation != null) {
	            if(actor.animation["strafe_left"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);
	                actor.animation.Play("strafe_left", PlayMode.StopAll);
	            }
			}
        }
        else {
            PlayOneShotFloat(GamePlayerAnimationType.strafe, -1f);
        }
    }

    // --------------

    // ACTIONS - STRAFE RIGHT

    public virtual void StrafeRight() {
        DidStrafeRight();
    }

    public virtual void DidStrafeRight() {
        if(isDead) {
            return;
        }

        if(isLegacy) {
			
			if(actor.animation != null) {
	            if(actor.animation["strafe_right"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);
	                actor.animation.Play("strafe_right", PlayMode.StopAll);
	            }
			}
        }
        else {
            PlayOneShotFloat(GamePlayerAnimationType.strafe);
        }
    }


    // --------------

    // ACTIONS - BOOST

    public virtual void Boost() {
        DidBoost();
    }

    public virtual void DidBoost() {
        if(isDead) {
            return;
        }

        if(isLegacy) {
			
			if(actor.animation != null) {
	            if(actor.animation["boost"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);
	                actor.animation.Play("boost", PlayMode.StopAll);
	            }
			}
        }
        else {
            PlayOneShotFloat(GamePlayerAnimationType.boost);
        }
    }


    // --------------

    // ACTIONS - SPIN

    public virtual void Spin() {
        DidSpin();
    }

    public virtual void DidSpin() {
        if(isDead) {
            return;
        }

        if(isLegacy) {
			
			if(actor.animation != null) {
            	if(actor.animation["spin"] != null) {
	                isRunningClampAnimation = true;
	                //PauseAnimationUpdate(1f);
	                actor.animation.Play("spin", PlayMode.StopAll);
	            }
			}
        }
        else {
            PlayOneShotFloat(GamePlayerAnimationType.spin);
        }
    }
     
    // --------------
    // ACTIONS - SKILL   
 
    public virtual void Skill() {
        DidSkill();
    }
 
    public virtual void DidSkill() {
        if(isDead) {
            return;
        }
     
        isRunningClampAnimation = true;
        PauseAnimationUpdate(1f);    
        //Debug.Log("DidSkill:");
        float currentSpeed = 0f;
        float walkSpeed = 0f;
     
        if(thirdPersonController != null) {
            currentSpeed = thirdPersonController.GetSpeed();
            walkSpeed = thirdPersonController.walkSpeed;
        }
     
        // Fade in run
        if(currentSpeed > walkSpeed) {
            if(actor.animation["skill"] != null) {
                actor.animation.Blend("skill");
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }
        }
     // Fade in walk
        else if(currentSpeed > 0.1) {
            if(actor.animation["skill"] != null) {
                actor.animation.Blend("skill");
                SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
            }
        }
     // Fade out walk and run
        else {
            if(actor.animation["skill"] != null) {
                actor.animation.Play("skill");
                SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
            }
        }
        //SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
    }
 
    public virtual void PlayOneShotBool(string paramName) {
        StartCoroutine(PlayOneShotBoolCo(paramName));
    }
 
    public virtual IEnumerator PlayOneShotBoolCo(string paramName) {
     
        if(!isLegacy) {
            animator.SetBool(paramName, true);
            yield return null;
            animator.SetBool(paramName, false);          
        }
    }

    public virtual void ResetFloat(string paramName) { 
        if(!isLegacy) {
            if(animator != null) {
                animator.SetFloat(paramName, 0.0f);
            }
        }
    }
 
    public virtual void PlayOneShotFloat(string paramName) {
        StartCoroutine(PlayOneShotFloatCo(paramName));
    }
 
    public virtual IEnumerator PlayOneShotFloatCo(string paramName) {
     
        if(!isLegacy) {
            if(animator != null) {
                animator.SetFloat(paramName, 1.0f);
                yield return null;
                animator.SetFloat(paramName, 0.0f);
            }
        }
    }

    public virtual void PlayOneShotFloat(string paramName, float val) {
        StartCoroutine(PlayOneShotFloatCo(paramName, val));
    }
 
    public virtual IEnumerator PlayOneShotFloatCo(string paramName, float val) {
     
        if(!isLegacy) {
            if(animator != null) {
                animator.SetFloat(paramName, val);
                yield return null;
                animator.SetFloat(paramName, 0.0f);
            }
        }
    }
 
     
    // --------------
    // ACTIONS - HIT 
 
    public virtual void ApplyDamage() {
        AnimateHit();
    }
 
    // --------------
    // ACTIONS - EXTRA   
 
    public virtual void ButtStomp() {
        DidButtStomp();
    }

    public virtual void DidButtStomp() {
        //actor.animation.CrossFade("buttstomp", 0.1f);
        //SendMessage("SyncAnimation", "buttstomp", SendMessageOptions.DontRequireReceiver);
        //actor.animation.CrossFadeQueued("jumpland", 0.2f);
    }
 
    public virtual void WallJump() {
        DidWallJump();
    }

    public virtual void DidWallJump() {
        // Wall jump animation is played without fade.
        // We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
        // But we really have to make sure that the animation is in full control so 
        // that we don't do weird blends between 180 degree apart rotations
		
		if(actor.animation != null) {
	        if(actor.animation["walljump"] != null) {
	            actor.animation.Play("walljump");
	            SendMessage("SyncAnimation", "walljump");
	        }
		}
    }
}