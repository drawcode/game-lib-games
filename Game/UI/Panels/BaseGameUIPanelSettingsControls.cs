using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelSettingsControls : GameUIPanelBase {	
    
    public static GameUIPanelSettingsControls Instance; 
	
    public GameObject listItemPrefab;
		
	public UICheckbox checkboxControlsHandedRight;
	public UICheckbox checkboxControlsHandedLeft;
	
    public UICheckbox checkboxControlsVibrate;
    
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
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		SyncCheckedState();
		loadData();
	}

    public override void OnEnable() {

        //Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);

        Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
    }

    public override void OnDisable() {

        //Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);

        Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
           //
        }
    }
    
    public virtual void OnButtonClickEventHandler(string buttonName) {
        
    }

	public virtual void ChangeCheckedState(UICheckbox box, bool selected) {
		if(box != null) {
			box.isChecked = selected;
		}
	}

    public virtual void SyncCheckedState() {

		bool vibrate = GameProfiles.Current.GetControlVibrate();
		ProfileControlHanded controlHanded = GameProfiles.Current.GetControlHanded();

		if(controlHanded == ProfileControlHanded.LEFT) {
			ChangeCheckedState(checkboxControlsHandedRight, false);
			ChangeCheckedState(checkboxControlsHandedLeft, true);
		}
		else if(controlHanded == ProfileControlHanded.RIGHT) {
			ChangeCheckedState(checkboxControlsHandedRight, true);
			ChangeCheckedState(checkboxControlsHandedLeft, false);
		}

		ChangeCheckedState(checkboxControlsVibrate, vibrate);
	}
	
    public virtual void OnCheckboxChangeEventHandler(string checkboxName, bool selected) {
        //LogUtil.Log("OnCheckboxChangeEventHandler: checkboxName:" + checkboxName + " selected:" + selected );
        
        // Change appstate
        
        if(checkboxName == checkboxControlsHandedRight.name) {
			if(selected) {
				GameProfiles.Current.SetControlHanded(
					ProfileControlHanded.RIGHT);
				ChangeCheckedState(checkboxControlsHandedLeft, false);
			}
        }
        else if(checkboxName == checkboxControlsHandedLeft.name) {
			if(selected) {
				GameProfiles.Current.SetControlHanded(
					ProfileControlHanded.LEFT);
				ChangeCheckedState(checkboxControlsHandedRight, false);
			}
        }
        else if(checkboxName == checkboxControlsVibrate.name) {
			GameProfiles.Current.SetControlVibrate(selected);
        }

		SyncCheckedState();
		GameState.SaveProfile();
		
    }
	
	public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		yield return new WaitForSeconds(1f);
	}
	
}
