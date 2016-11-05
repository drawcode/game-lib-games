using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

#if ENABLE_FEATURE_MODE_CHALLENGE

public class BaseGameUIPanelResultsChallenge : GameUIPanelResultsBase {
    
    public static GameUIPanelResultsChallenge Instance;
    
    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public virtual void Awake() {
    
    }
	
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		loadData();
	}
		 
    public override void OnButtonClickEventHandler(string buttonName) {
        base.OnButtonClickEventHandler(buttonName);

		//Debug.Log("OnButtonClickEventHandler: " + buttonName);
	}

	public override void UpdateDisplay(GamePlayerRuntimeData runtimeData, float timeTotal) {

        base.UpdateDisplay(runtimeData, timeTotal);

	}
		
	public override void loadData() {
        base.loadData();
		StartCoroutine(loadDataCo());
	}
	
	public override IEnumerator loadDataCo() {
        base.loadDataCo();
		yield return new WaitForSeconds(1f);
	}	
}

#endif
