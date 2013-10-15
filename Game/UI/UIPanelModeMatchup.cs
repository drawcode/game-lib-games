#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelModeMatchup : UIPanelBase {

	public static UIPanelModeMatchup Instance;

	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
	}
	
	public static bool isInst {
		get {
			if(Instance != null) {
				return true;
			}
			return false;
		}
	}	
	
	public override void Init() {
		base.Init();

		loadData();
	}	
	
	public override void Start() {
		Init();
	}
 
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }
	
    void OnButtonClickEventHandler(string buttonName) {

	}

    public static void ShowDefault() {
        if(isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if(isInst) {
            Instance.AnimateOut();
        }
    }

	public static void LoadData() {
		if(Instance != null) {
			Instance.loadData();
		}
	}

    public void loadData() {
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
        yield return new WaitForSeconds(1f);
    }
	
}
