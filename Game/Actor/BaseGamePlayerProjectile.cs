using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Animation;
using UnityEngine;

using Engine.Utility;

public class BaseGamePlayerProjectile : GameActor
{
	public Vector3 direction = new Vector3(1f, 1f, 0f);
	public float speed = 100;
	public float liveTime = 5f;
	
	public GameObject projectileObject;
	
	void Awake() {
		Invoke("DestroyMe", liveTime);
	}
	
	public override void Start() {
		Quaternion rotationObject = projectileObject.transform.rotation;
		rotationObject.y = 0;
		rotationObject.x = 0;
		rotationObject.z = 0;
		projectileObject.transform.rotation = rotationObject;
		
	}
	
	public void Launch() {
		if(projectileObject != null) {
			LogUtil.Log("GameProjectile::Launch::" + projectileObject);
			rigidbody.AddForce(direction * speed);
		}
	}
	
	void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }
	}
		
	//void FixedUpdate () {
	//
	//}
	
	void DestroyMe() {
		ObjectPoolManager.destroyPooled(gameObject);
	}
	
}

