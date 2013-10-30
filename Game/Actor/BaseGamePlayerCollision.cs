using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseGamePlayerCollision : MonoBehaviour {
	
	public GamePlayerController gamePlayerController;
	private GameObject gamePlayerControllerObject;

    float lastCollision = 0f;
    float intervalCollision = .2f;
	
	public virtual void Start() {
		InvokeRepeating("FindPlayerCollisionParent", 1f, 10);
	}
	
	public virtual void FindPlayerCollisionParent() {
		if(gamePlayerControllerObject == null) {
			gamePlayerControllerObject = gameObject.FindTypeAboveObject<GamePlayerController>();
		}			
		
		if(gamePlayerController == null 
		&& gamePlayerControllerObject != null) {
			gamePlayerController = gamePlayerControllerObject.GetComponent<GamePlayerController>();
            CancelInvoke("FindPlayerCollisionParent");
		}
	}
	
	public virtual void OnCollisionEnter(Collision collision) {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(lastCollision + intervalCollision < Time.time) {
            lastCollision = Time.time;
        }
        else {
            return;
        }

     	if(gamePlayerController != null) {
        	//foreach (ContactPoint contact in collision.contacts) {
				gamePlayerController.HandleCollision(collision);
			//	Debug.Log("contact:" + contact);
			//}
        }
    }
}