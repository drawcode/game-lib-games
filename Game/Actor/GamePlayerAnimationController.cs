using System;
using System.Collections;
using System.Collections.Generic;

using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;

using UnityEngine;

public enum GamePlayerAnimationControllerType  {
	mecanim,
	legacy
}

public class GamePlayerAnimationController : MonoBehaviour {

	public float runSpeedScale = 1.2f;
	public float walkSpeedScale = 1.0f;
	public Transform torso;
	public GameObject actor;
	
	public GamePlayerAnimationControllerData animationData;
	
	public bool isRunningClampAnimation = false;
	
	public bool isRunning = false;
	public bool isDead = false;
	
	public GamePlayerController gamePlayerController;
	public GamePlayerThirdPersonController thirdPersonController;
	
	public NavMeshAgent navAgent;
	
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
	
	void Awake() {
		Init();
	}
	
	public void Init() {
		thirdPersonController = GetComponent<GamePlayerThirdPersonController>();
		navAgent = GetComponent<NavMeshAgent>();
		gamePlayerController = GetComponent<GamePlayerController>();
		animationData = new GamePlayerAnimationControllerData();
		Reset();
	}
		
	public void ResetPlayState() {	
		isDead = false;
	}
	
	public void Reset() {
		if(actor != null) {
			
			if(actor.animation == null) {
				UnityEngine.Animation[] animatedObjects = actor.GetComponentsInChildren<Animation>();
				foreach(Animation animItem in animatedObjects) {
					actor = animItem.gameObject;
					break;
				}
			}
			
			if(actor.animation != null) {
							// By default loop all animations
				actor.animation.wrapMode = WrapMode.Loop;
	
				actor.animation.Stop();
				
				if(animationRun == null) {
					if(actor.animation[animationData.Run()] != null) {
						animationRun = actor.animation[animationData.Run()];
						animationAttack.layer = 1;
					}
				}
				
				if(animationWalk == null) {
					if(actor.animation[animationData.Walk()] != null) {
						animationWalk = actor.animation[animationData.Walk()];
						animationAttack.layer = 1;
					}
				}
				
				if(animationIdle == null) {
					if(actor.animation[animationData.Idle()] != null) {
						animationIdle = actor.animation[animationData.Idle()];
						animationAttack.layer = 1;
					}
				}
				
				if(animationHit == null) {
					if(actor.animation[animationData.Hit()] != null) {
						animationHit = actor.animation[animationData.Hit()];
						animationAttack.layer = 2;
					}
				}
				
				if(animationAttack == null) {
					if(actor.animation[animationData.Attack()] != null) {
						animationAttack = actor.animation[animationData.Attack()];
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

				isRunning = true;
				isDead = false;
			}
		}
	}
	
	public void AnimationPlay(Animation ani) {
		if(ani == null) {
			return;
		}
		ani.Play();
	}
	
	
	public void AnimationCrossFade(Animation ani, Animation aniTo) {
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
	
	public void AnimationBlend(Animation ani, Animation aniTo) {
		AnimationBlend(ani, aniTo, .5f, .5f);
	}
	
	public void AnimationBlend(Animation ani, Animation aniTo, float targetWeight) {
		AnimationBlend(ani, aniTo, targetWeight, .5f);
	}
	
	public void AnimationBlend(Animation ani, Animation aniTo, float targetWeight, float fadeLength) {
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
	
	
	public void AnimationStatePlay(AnimationState ani) {
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
	
	
	public void AnimationStateCrossFade(AnimationState ani, AnimationState aniTo) {
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
	
	public void AnimationStateBlend(AnimationState ani, AnimationState aniTo) {
		AnimationStateBlend(ani, aniTo, .8f, .5f);
	}
	
	public void AnimationStateBlend(AnimationState ani, AnimationState aniTo, float targetWeight) {
		AnimationStateBlend(ani, aniTo, targetWeight, .5f);
	}
	
	public void AnimationStateBlend(AnimationState ani, AnimationState aniTo, float targetWeight, float fadeLength) {
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
	
	public void ResetAnimatedActor(GameObject actorItem) {
		actor = actorItem;
		FindAnimatedActor();
	}
	
	public void FindAnimatedActor() {
		if(actor != null) {			
			if(actor.animation == null) {
				UnityEngine.Animation[] animatedObjects = actor.GetComponentsInChildren<Animation>();
				foreach(Animation animItem in animatedObjects) {
					actor = animItem.gameObject;
					break;
				}
			}
		}
	}

	void Update() {
		
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
			
			if(actor == null || actor.animation == null) {
				return;
			}
			
			try {
				if(actor.animation[animationData.Run()] != null) {
					actor.animation[animationData.Run()].normalizedSpeed = runSpeedScale;
				}
				if(actor.animation[animationData.Walk()] != null) {
					actor.animation[animationData.Walk()].normalizedSpeed = walkSpeedScale;
				}
			} 
			catch (System.Exception e) {
				LogUtil.Log(e);
			}
			
			// Fade in run
			if(currentSpeed > walkSpeed) {
				
				if(actor.animation[animationData.Run()] != null) {
					if(actor.animation[animationData.Run()] != null) {
						actor.animation[animationData.Run()].blendMode = AnimationBlendMode.Blend;
						
						if(thirdPersonController == null) {						
							actor.animation[animationData.Run()].normalizedSpeed = runSpeedScale;
							//actor.animation[animationData.Run()].time = 0f;
							actor.animation.CrossFade(animationData.Run(), .5f);	
						}
						else {						
						
							if(thirdPersonController.verticalInput2 != 0f 
								|| thirdPersonController.horizontalInput2 != 0f) {
								
								// if angle between axis is over 120 and less than 240 reverse run
								float angleTo = Vector3.Angle(thirdPersonController.movementDirection, thirdPersonController.aimingDirection);
								
								if(angleTo > 120 && angleTo < 240) {
									actor.animation[animationData.Run()].normalizedSpeed = -runSpeedScale * .9f;								
								}
								else {	
									actor.animation[animationData.Run()].normalizedSpeed = runSpeedScale;
								}
								
								//actor.animation[animationData.Run()].time = actor.animation[animationData.Run()].length;
								actor.animation.Blend(animationData.Run());	
							}
							else {
								actor.animation[animationData.Run()].normalizedSpeed = runSpeedScale;
								//actor.animation[animationData.Run()].time = 0f;
								actor.animation.CrossFade(animationData.Run(), .5f);						
							}
						}
					}
				}
				// We fade out jumpland quick otherwise we get sliding feet
				if(actor.animation[animationData.Jump()] != null) {
					//actor.animation.CrossFade(animationData.Jump(), 0);
				}
				SendMessage("SyncAnimation", animationData.Run(), SendMessageOptions.DontRequireReceiver);
			}
			// Fade in walk
			else if(currentSpeed > 0.1) {
				if(actor.animation[animationData.Jump()] != null) {
					//if(actor.animation[animationData.Jump()] != null) {
					//	actor.animation.CrossFade(animationData.Jump());
					//}
					// We fade out jumpland realy quick otherwise we get sliding feet
					//actor.animation.Blend(animationData.Jump(), 0);
				}
				if(actor.animation[animationData.Walk()] != null) {
					if(actor.animation[animationData.Walk()] != null) {
						actor.animation[animationData.Walk()].blendMode = AnimationBlendMode.Blend;
						if(thirdPersonController.verticalInput2 != 0f 
							|| thirdPersonController.horizontalInput2 != 0f) {
							// if angle between axis is over 120 and less than 240 reverse run
							float angleTo = Vector3.Angle(thirdPersonController.movementDirection, thirdPersonController.aimingDirection);
														
							if(angleTo > 120 && angleTo < 240) {
								actor.animation[animationData.Walk()].normalizedSpeed = -walkSpeedScale * .9f;								
							}
							else {	
								actor.animation[animationData.Walk()].normalizedSpeed = walkSpeedScale;
							}
							actor.animation.Blend(animationData.Walk());	
						}
						else {
							actor.animation[animationData.Walk()].normalizedSpeed = walkSpeedScale;
							//actor.animation[animationData.Run()].time = 0f;
							actor.animation.CrossFade(animationData.Walk(), .5f);						
						}
						
						
						SendMessage("SyncAnimation", animationData.Walk(), SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			// Fade out walk and run
			else {
				try {
					if(actor.animation[animationData.Idle()] != null) {
						if(actor.animation[animationData.Idle()] != null
							&& !isRunningClampAnimation) {
							actor.animation.CrossFade(animationData.Idle());
							SendMessage("SyncAnimation", animationData.Idle(), SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				catch (System.Exception e) {
					LogUtil.Log(e);
				}
			}
			
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
			
			if(actor.animation[animationData.Skill()] != null) {
				if(isJumping) {
					if(isCapeFlying) {
						//actor.animation.CrossFade("jetpackjump", 0.2f);
						//SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
						if(actor.animation[animationData.Skill()] != null) {
							actor.animation.CrossFade(animationData.Skill(), 0.2f);
							SendMessage("SyncAnimation", animationData.Skill(), SendMessageOptions.DontRequireReceiver);
						}
					}
					else if(hasJumpReachedApex) {
						//actor.animation.CrossFade("jumpfall", 0.2f);
						//SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);
						if(actor.animation[animationData.Skill()] != null) {
							actor.animation.CrossFade(animationData.Skill(), 0.2f);
							SendMessage("SyncAnimation", animationData.Skill(), SendMessageOptions.DontRequireReceiver);
						}
					}
					else {
						if(actor.animation[animationData.Skill()] != null) {
							actor.animation.CrossFade(animationData.Skill(), 0.2f);
							SendMessage("SyncAnimation", animationData.Skill(), SendMessageOptions.DontRequireReceiver);
						}
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
			
			
			/*
			
			if(actor.animation[animationData.Jump()] != null) {
				if(isJumping) {
					if(isCapeFlying) {
						//actor.animation.CrossFade("jetpackjump", 0.2f);
						//SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
						if(actor.animation[animationData.Jump()] != null) {
							actor.animation.CrossFade(animationData.Jump(), 0.2f);
							SendMessage("SyncAnimation", animationData.Jump(), SendMessageOptions.DontRequireReceiver);
						}
					}
					else if(hasJumpReachedApex) {
						//actor.animation.CrossFade("jumpfall", 0.2f);
						//SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);
						if(actor.animation[animationData.Jump()] != null) {
							actor.animation.CrossFade(animationData.Jump(), 0.2f);
							SendMessage("SyncAnimation", animationData.Jump(), SendMessageOptions.DontRequireReceiver);
						}
					}
					else {
						if(actor.animation[animationData.Jump()] != null) {
							actor.animation.CrossFade(animationData.Jump(), 0.2f);
							SendMessage("SyncAnimation", animationData.Jump(), SendMessageOptions.DontRequireReceiver);
						}
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
			*/
			//}
			//catch (System.Exception ee) {
			//	Debug.Log("e" + ee);
			//}
		}
	}
	
	public void ResetClampAnimation() {
		isRunningClampAnimation = false;	
	}
	
	public void PauseAnimationUpdate(float duration) {
		Invoke("ResetClampAnimation", duration);
	}

	public void DidLand() {
		//actor.animation.Play("jumpland");
		//SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
		if(actor.animation[animationData.Jump()] != null) {
			//actor.animation.Play(animationData.Jump());
			SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void Attack() {		
		DidAttack();	
	}
	
	public void AttackAlt() {		
		DidAttackAlt();	
	}
	
	public void AttackLeft() {		
		DidAttackLeft();	
	}
	
	public void AttackRight() {		
		DidAttackRight();	
	}

	public void DidAttack() {		
		DidAttack(animationData.Attack());
	}
	
	public void DidAttackAlt() {
		DidAttack("attack-alt");
	}
	
	public void DidAttackLeft() {		
		DidAttack("attack-left");
	}	
	
	public void DidAttackRight() {		
		DidAttack("attack-right");
	}
	
	public void DidAttack(string animationName) {
		
		if(isDead) {
			return;
		}
		
		LogUtil.Log("GamePlayerControllerAnimation:DidAttack:" + animationName);
		
		isRunningClampAnimation = true;
		PauseAnimationUpdate(1f);			
		
		//float currentSpeed = 0f;
		//float walkSpeed = 0f;
		
		if(thirdPersonController != null) {
			//currentSpeed = thirdPersonController.GetSpeed();
			//walkSpeed = thirdPersonController.walkSpeed;
		}
		
		if( actor.animation[animationName] ) {
			
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
		/*
		// Fade in run
		if(currentSpeed > walkSpeed) {
			if(actor.animation[animationData.Run()] != null 
				&& actor.animation[animationName] ) {
				actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
				actor.animation.Blend(animationName);
				SendMessage("SyncAnimation", animationData.Run(), 
					SendMessageOptions.DontRequireReceiver);
			}
		}
		// Fade in walk
		else if(currentSpeed > 0.1) {
			if(actor.animation[animationData.Run()] != null 
				&& actor.animation[animationName] ) {
				actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
				actor.animation.Blend(animationName);
				SendMessage("SyncAnimation", animationData.Walk(), 
					SendMessageOptions.DontRequireReceiver);
			}
		}
		// Fade out walk and run
		else {
			if(actor.animation[animationData.Run()] != null 
				&& actor.animation[animationName] ) {
				//actor.animation.Blend(animationName, .6f);
				actor.animation.Play(animationName);
				SendMessage("SyncAnimation", animationData.Idle(), 
					SendMessageOptions.DontRequireReceiver);
			}
		}
		*/
	}	
	
	public void Die() {	
		DidDie();
	}
	
	public void DidDie() {	
		if(isDead) {
			return;
		}
		
		if(actor.animation[animationData.Death()] != null) {
			isRunningClampAnimation = true;
			//PauseAnimationUpdate(1f);	
			actor.animation.Play(animationData.Death(), PlayMode.StopAll);
			actor.animation.Play(animationData.Hit(), PlayMode.StopAll);
		}
	}
	
	public void Jump() {
		DidJump();
	}
	
	public void DidJump() {
	
	}
		
	public void Skill() {
		DidSkill();
	}
	
	public void DidSkill() {
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
			if(actor.animation[animationData.Skill()] != null) {
				actor.animation.Blend(animationData.Skill());
				SendMessage("SyncAnimation", animationData.Run(), SendMessageOptions.DontRequireReceiver);
			}
		}
		// Fade in walk
		else if(currentSpeed > 0.1) {
			if(actor.animation[animationData.Skill()] != null) {
				actor.animation.Blend(animationData.Skill());
				SendMessage("SyncAnimation", animationData.Walk(), SendMessageOptions.DontRequireReceiver);
			}
		}
		// Fade out walk and run
		else {
			if(actor.animation[animationData.Skill()] != null) {
				actor.animation.Play(animationData.Skill());
				SendMessage("SyncAnimation", animationData.Idle(), SendMessageOptions.DontRequireReceiver);
			}
		}
		//SendMessage("SyncAnimation", animationData.Run(), SendMessageOptions.DontRequireReceiver);
	}
	
	public void ButtStomp() {
		DidButtStomp();
	}

	public void DidButtStomp() {
		//actor.animation.CrossFade("buttstomp", 0.1f);
		//SendMessage("SyncAnimation", "buttstomp", SendMessageOptions.DontRequireReceiver);
		//actor.animation.CrossFadeQueued("jumpland", 0.2f);
	}

	public void ApplyDamage() {
		if(actor.animation[animationData.Hit()] != null) {
			actor.animation.CrossFade(animationData.Hit(), 0.1f);
			SendMessage("SyncAnimation", animationData.Hit(), SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void WallJump() {
		DidWallJump();
	}

	public void DidWallJump() {
		// Wall jump animation is played without fade.
		// We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
		// But we really have to make sure that the animation is in full control so 
		// that we don't do weird blends between 180 degree apart rotations
		if(actor.animation["walljump"] != null) {
			actor.animation.Play("walljump");
			SendMessage("SyncAnimation", "walljump");
		}
	}
}