using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Animation;
using UnityEngine;

public class BaseGamePlayerInventory : GameActor {

    public Dictionary<string, GamePlayerWeapon> weapons = new Dictionary<string, GamePlayerWeapon>();
	public Dictionary<string, GamePlayerItem> items = new Dictionary<string, GamePlayerItem>();
			
	public virtual void Awake() {
		
	}
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();
	}

    /*
	void OnTriggerEnter(Collider collider) {
	
	}
	
	void OnTriggerStay(Collider collider) {
	
	}
	
	void OnTriggerExit(Collider collider) {
	
	}
	
	void OnCollisionEnter(Collision collision) {
		
	}
	
	void OnCollisionStay(Collision collision) {
	
	}
		
	void OnCollisionExit(Collision collision) {
	
	}
 */
	
}

