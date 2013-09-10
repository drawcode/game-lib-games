using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Engine;
using Engine.Animation;
using Engine.Events;
using Engine.UI;
using Engine.Utility;

public class GameSceneDynamic : GameUIScene {
	
	public static GameSceneDynamic Instance;
		
	public float currentTimeBlock = 0.0f;
	public float actionInterval = 1.0f;
			
    public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
				
        Instance = this;
		
		if(GameGlobal.Instance == null) {
			Application.LoadLevel("GameUISceneRoot");
		}
				
	}
		
	void Start() {
		Init();
	}
	
	public override void Init () {
		base.Init ();
		InitEvents();
	}

	void InitEvents() {
		/*
		buttonMeta.SetButton("buttonBack", ref buttonBack, delegate () {
			SceneLoader.LoadSceneGameMain();
		});
		*/
	}

}

