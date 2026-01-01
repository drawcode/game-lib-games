using System;
using UnityEngine;

using Engine.Events;
using Engine.Utility;

using Engine.Game.Actor;

public class BaseGameActor : BaseGameObjectItem {

	/*
	public GameObject actorModelObject;
	
	public bool isMoving = false;
	public bool lastMoving = false;
	public bool isJumping = false;
	public bool lastJumping = false;	
	
	public bool navigating = false;
 */

	public override void Start() {
		Init();
	}

	public override void Init() {
		base.Init();
	}

	public override void OnEnable() {
		base.OnEnable();
		////MessengerObject<InputTouchInfo>.AddListener(EventMessagesType.OnEventInputDown, OnInputDown);
		////MessengerObject<InputTouchInfo>.AddListener(GameMessageType.OnEventInputUp, OnInputUp);
	}

	public override void OnDisable() {
		base.OnEnable();
		////MessengerObject<InputTouchInfo>.RemoveListener(GameMessageType.OnEventInputDown, OnInputDown);
		////MessengerObject<InputTouchInfo>.RemoveListener(GameMessageType.OnEventInputUp, OnInputUp);
	}

	/*
	
	public virtual void CheckAnimationState() {
		if(isJumping != lastJumping) {
			lastJumping = isJumping;
			PlayJump();
			Invoke("CheckJumping", .5f);
		}
		
		if(InputSystem.Instance.IsAnyAxisPressed() || navigating) {
			PlayRun();
		}
		else {
			PlayIdle();
		}
		
		Invoke("CheckMoving", .1f);
		
	}
	
	public virtual void CheckMoving() {
		isMoving = false;
		LogUtil.Log("isMoving:" + isMoving);
	}
	
	public virtual void CheckJumping() {
		isJumping = false;
		lastJumping = false;
		LogUtil.Log("isJumping:" + isJumping);
	}
	
	public virtual void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }
		
		currentTimeBlockLocal += Time.deltaTime;
		if(currentTimeBlockLocal > actionIntervalLocal) {
			currentTimeBlockLocal = 0.0f;
			//CheckAnimationState();
		}
		
		if(Input.GetKeyDown(KeyCode.Space)) {
			//isJumping = true;
		}
		
		//CheckAnimationState();
//
		//if(InputSystem.Instance.IsAnyAxisPressed() || navigating) {
		//	isMoving = true;
		//	CheckAnimationState();
		//}
//
		
		LogUtil.Log("isMoving:" + isMoving);
		
		Vector3 acceleration = Vector3.zero;
        int i = 0;
        while (i < Input.accelerationEventCount) {
            AccelerationEvent accEvent = Input.GetAccelerationEvent(i);
            acceleration += accEvent.acceleration * accEvent.deltaTime;
            ++i;
        }
		LogUtil.Log("acceleration:" + acceleration);
		
	}
	
	public virtual void PlayAnimation(string animationName, PlayMode mode) {
		if(actorModelObject) {
			actorModelObject.animation.CrossFade(animationName, .3f, mode);
		}
	}
	
	public virtual void PlayRun() {
		PlayAnimation("run", PlayMode.StopSameLayer);
	}
			
	public virtual void PlayWalk() {
		PlayAnimation("walk", PlayMode.StopSameLayer);
	}
			
	public virtual void PlayJump() {
		PlayAnimation("jump", PlayMode.StopSameLayer);
	}		
	
	public virtual void PlayIdle() {
		PlayAnimation("idle", PlayMode.StopSameLayer);
	}
 */
}

