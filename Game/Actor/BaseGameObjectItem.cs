using System;
using UnityEngine;

using Engine.Events;
using Engine.Utility;

public class BaseGameObjectItem : GameObjectTimerBehavior {
	
	public string uniqueId;	
	
	public float currentTimeBlockLocal = 0.0f;
	public float actionIntervalLocal = 1.0f;	
	
	public virtual void Start() {
		Init();
	}
	
	public virtual void Init() {
		uniqueId = UniqueUtil.CreateUUID4();
	}
	
	public virtual void OnEnable() {

	}
	
	public virtual void OnDisable() {

	}
	
	public virtual void OnInputDown(InputTouchInfo touchInfo) {
		LogUtil.Log("OnInputDown GameActor");		
		
	}
	
	public virtual void OnInputUp(InputTouchInfo touchInfo) {
		//LogUtil.Log("OnInputDown GameActor");
	}
	
	public virtual bool HitObject(GameObject go, InputTouchInfo inputTouchInfo) {
		Ray screenRay = Camera.main.ScreenPointToRay(inputTouchInfo.position3d);
		RaycastHit hit;
		
		if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null)
		{
			if(hit.transform.gameObject == go)
			{
				return true;
			}
		}
		return false;
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
	}	
}

