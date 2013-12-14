using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Animation;
using Engine.Events;
using Engine.UI;
using Engine.Utility;


public class BaseGamePlayerUI : MonoBehaviour {
		
	public float currentTimeBlock = 0.0f;
	public float actionInterval = 5.0f;
	
	public float randomSpeed = 0;
	public float randomStrafe = 0;
	public float randomJump = 0;
	
	public int currentAnimationState = 0;
	
    public float horizontalSpeed = 2.0f;
    public float verticalSpeed = 2.0f;

    public float h = 0;
    public float v = 0;
    public float u = 0;
	
	public virtual void Start() {
		
	}
	
	public virtual void RunAnimations() {
		foreach(Animator anim in gameObject.GetComponentsInChildren<Animator>()) {
			anim.SetFloat("speed", v);
			anim.SetFloat("strafe", h);
			anim.SetFloat("jump", u);
			anim.SetFloat("death", 0);
			anim.SetFloat("hit", 0);
			anim.SetFloat("attack", 0);
			//Avatar avatar = anim.avatar;
			//RuntimeAnimatorController controller = anim.runtimeAnimatorController;

            //Debug.Log("avatar:" + avatar.name);
            //Debug.Log("controller:" + controller.name);
		}
	}
	
    public virtual void Update() {

        //if(Application.isEditor && true == false) {
    	//	if(Input.GetAxis("Horizontal") >= 0f
    	//		|| Input.GetAxis("Vertical") >= 0f) {
    	//
    	//		h = horizontalSpeed * Input.GetAxis("Horizontal");
        //   	v = verticalSpeed * Input.GetAxis("Vertical");
    	//
    	//		RunAnimations();
    	//	}
        //}
		
		currentTimeBlock += Time.deltaTime;		
		
		if(currentTimeBlock > actionInterval) {

            actionInterval = UnityEngine.Random.Range(1, 5);
			currentTimeBlock = 0.0f;
			
			randomSpeed = UnityEngine.Random.Range(.0f, .9f);
			randomStrafe = UnityEngine.Random.Range(.0f, .6f);
			randomJump = UnityEngine.Random.Range(.0f, .7f);
			
			h = randomStrafe;
			v = randomSpeed;
			u = randomJump;
			
			RunAnimations();

            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 1);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 1);
					
		}
	}
}
