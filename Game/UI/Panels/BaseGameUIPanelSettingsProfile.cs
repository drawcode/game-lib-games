using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelSettingsProfile : GameUIPanelBase {	
    
    public static GameUIPanelSettingsProfile Instance;
	
    public GameObject listItemPrefab;
	
	public UIImageButton buttonProfileFacebook;	
	public UIImageButton buttonProfileTwitter;	
	public UIImageButton buttonProfileGameNetwork;	
	
    public UIInput inputProfileName;
    
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

        Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnProfileInputChanged);
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

        Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnProfileInputChanged);
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
	
	public virtual void OnProfileInputChanged(string controlName, string data) {
		
		if(inputProfileName != null && controlName == inputProfileName.name) {
			ChangeUsername(data);
		}
	}
	
    public virtual void ChangeUsername(string username) {
		if(inputProfileName == null) {
			return;
		}
		
		UIUtil.SetInputValue(inputProfileName, username);
		GameProfiles.Current.ChangeUser(username);
		GameProfiles.Current.username = username;
		GameState.SaveProfile();
	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		ChangeUsername(GameProfiles.Current.username);
		
		yield return new WaitForSeconds(1f);
	}
	
}
