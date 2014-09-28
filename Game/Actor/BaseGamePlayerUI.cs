using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Animation;
using Engine.Events;
using Engine.UI;
using Engine.Utility;


public class BaseGamePlayerUI : GameObjectBehavior {
		
	public float currentTimeBlock = 0.0f;
	public float actionInterval = 0.0f;
	
	public float randomSpeed = 0;
	public float randomStrafe = 0;
	public float randomJump = 0;
	
	public int currentAnimationState = 0;
	
    public float horizontalSpeed = 2.0f;
    public float verticalSpeed = 2.0f;

    public float h = 0;
    public float v = 0;
    public float u = 0;

    public bool runOnly = false;
    public bool resetAnimationObject = true;

    GameObject animateObject;

    bool running = false;

    public virtual void Awake() {
        running = false;
    }

    public virtual void OnEnable() {
        running = false;
    }
    
    public virtual void OnDisable() {
        running = false;
    }
	
	public virtual void Start() {        
        running = false;
	}

    public virtual void AnimateRunOnly() {
        foreach(Animator anim in gameObject.GetComponentsInChildren<Animator>()) {
            anim.SetFloat("speed", .9f);
            anim.SetFloat("strafe", .05f);
            anim.SetFloat("jump", .05f);
            anim.SetFloat("death", 0f);
            anim.SetFloat("hit", 0f);
            anim.SetFloat("attack", 0f);
            running = true;
            animateObject = anim.gameObject;
        }
    }
	
	public virtual void RunAnimations() {
		foreach(Animator anim in gameObject.GetComponentsInChildren<Animator>()) {
			anim.SetFloat("speed", v);
			anim.SetFloat("strafe", h);
			anim.SetFloat("jump", u);
			anim.SetFloat("death", 0);
			anim.SetFloat("hit", 0);
            anim.SetFloat("attack", 0);
            animateObject = anim.gameObject;
			//Avatar avatar = anim.avatar;
			//RuntimeAnimatorController controller = anim.runtimeAnimatorController;

            //LogUtil.Log("avatar:" + avatar.name);
            //LogUtil.Log("controller:" + controller.name);
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
        
        if(resetAnimationObject && animateObject != null) {
            
            animateObject.transform.localPosition = 
                Vector3.Lerp(animateObject.transform.localPosition, Vector3.zero, Time.deltaTime);
            
            animateObject.transform.localRotation = 
                Quaternion.Lerp(animateObject.transform.localRotation, Quaternion.identity, Time.deltaTime);
        }

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(runOnly) {

            if(runOnly && !running) {
                AnimateRunOnly();
                running = true;
            }

            return;
        }
		
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

            //transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 1);
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 1);
					
		}
	}
}
