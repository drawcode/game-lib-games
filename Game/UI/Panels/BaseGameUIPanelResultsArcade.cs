using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelResultsArcade : GameUIPanelResultsBase {
    
    public static GameUIPanelResultsArcade Instance;
    
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
	
	public static void LoadData() {
        if(GameUIPanelResultsArcade.Instance != null) {
            GameUIPanelResultsArcade.Instance.loadData();
		}
	}
	
	public override void loadData() {
        //base.loadData();
	}
	
	public override IEnumerator loadDataCo() {
		yield return new WaitForSeconds(1f);
	}
	
}
