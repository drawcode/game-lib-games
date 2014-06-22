using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelOverlays : GameUIPanelBase {
    
    public static GameUIPanelOverlays Instance;

	public GameObject containerObject;

	public GameObject overlayWhiteRadial;	
	public GameObject overlayBlackSolid;	
	public GameObject overlayWhiteSolid;
    public GameObject overlayWhiteSolidStatic;	
    
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
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		//LoadData();
		//AnimateIn();
	}
	
    public virtual void LoadData() {
		StartCoroutine(LoadDataCo());
	}
	
	IEnumerator LoadDataCo() {
		yield break;
	}	
	
    public virtual void ShowOverlayWhiteFlash() {
		//LogUtil.Log("ShowWhiteFlash");
		
		//DeviceUtil.Vibrate();		
		ShowOverlayWhiteFlashOut();
		HideOverlayWhiteFlashOut();
	}
	
    public virtual void ShowOverlayWhiteFlashOut() {
		//LogUtil.Log("ShowOverlayWhiteFlashOut");		
		//DeviceUtil.Vibrate();		
		ShowOverlayWhite();
	}

    public virtual void HideOverlayWhiteFlashOut() {
		//LogUtil.Log("HideOverlayWhiteFlash");
		HideOverlayWhite(.3f, .3f, 1f, 0f);
	}
	
    public virtual void ShowOverlayWhite() {
		ShowOverlayWhite(.2f, .1f, 0f, 1f);
	}
	
    public virtual void ShowOverlayWhite(float time, float delay, float amountFrom, float amountTo) {
		
		if(overlayWhiteSolid != null) {
			UITweenerUtil.FadeTo(overlayWhiteSolid, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}
	
    public virtual void HideOverlayWhite() {
		HideOverlayWhite(.1f, .2f, 0f, 0f);
	}
	
    public virtual void HideOverlayWhite(float time, float delay, float amountFrom, float amountTo) {		
		if(overlayWhiteSolid != null) {
			UITweenerUtil.FadeTo(overlayWhiteSolid, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}	
	
    public virtual void ShowOverlayWhiteStatic() {
		ShowOverlayWhiteStatic(.5f, 0f, 0f, 1f);
	}
	
    public virtual void ShowOverlayWhiteStatic(float time, float delay, float amountFrom, float amountTo) {
		
		if(overlayWhiteSolidStatic != null) {
			UITweenerUtil.FadeTo(overlayWhiteSolidStatic, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}
	
    public virtual void HideOverlayWhiteStatic() {
		HideOverlayWhiteStatic(1f, 0f, 0f, 0f);
	}
	
    public virtual void HideOverlayWhiteStatic(float time, float delay, float amountFrom, float amountTo) {		
		if(overlayWhiteSolidStatic != null) {
			UITweenerUtil.FadeTo(overlayWhiteSolidStatic, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}	
	
	// White radial
	
    public virtual void ShowOverlayWhiteRadial() {
		ShowOverlayWhite(.2f, .1f, 0f, 1f);
	}
	
    public virtual void ShowOverlayWhiteRadial(float time, float delay, float amountFrom, float amountTo) {
		
		if(overlayWhiteRadial != null) {
			UITweenerUtil.FadeTo(overlayWhiteRadial, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}
	
    public virtual void HideOverlayWhiteRadial() {
		HideOverlayWhiteRadial(.1f, .2f, 0f, 0f);
	}
	
    public virtual void HideOverlayWhiteRadial(float time, float delay, float amountFrom, float amountTo) {		
		if(overlayWhiteRadial != null) {
			UITweenerUtil.FadeTo(overlayWhiteRadial, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}		
	
    public virtual void ShowOverlayBlack() {
		ShowOverlayBlack(.2f, .1f, 0f, 1f);
	}
	
    public virtual void ShowOverlayBlack(float time, float delay, float amountFrom, float amountTo) {
		
		if(overlayBlackSolid != null) {
			UITweenerUtil.FadeTo(overlayBlackSolid, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}
	
    public virtual void HideOverlayBlack() {
		HideOverlayBlack(.1f, .2f, 0f, 0f);
	}
	
    public virtual void HideOverlayBlack(float time, float delay, float amountFrom, float amountTo) {		
		if(overlayBlackSolid != null) {
			UITweenerUtil.FadeTo(overlayBlackSolid, 
				UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
		}
	}	
	
    public virtual void HideAll() {
		HideOverlayWhite(0f, 0f, 0f, 0f);
		HideOverlayWhiteRadial(0f, 0f, 0f, 0f);
		HideOverlayBlack(0f, 0f, 0f, 0f);
		HideOverlayWhiteStatic();
	}
	
	public override void AnimateIn() {
		
		base.AnimateIn();
		
		HideAll();
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		
		HideAll();
	}


    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }
		
		//var ry = 0f;
		//var rx = 0f;
		if(Context.Current.isMobile) {
			//ry =-Input.acceleration.y + Screen.height/2;	
			//rx =-Input.acceleration.x + Screen.width/2;	
		}
		else {
			//ry =-Input.mousePosition.y + Screen.height/2;	
			//rx =-Input.acceleration.x + Screen.width/2;				
		}
	
		if(overlayWhiteRadial != null) {
			//overlayWhiteRadial.transform.Rotate(Vector3.forward * (ry * .005f) * Time.deltaTime); 
		}
		
		if(Application.isEditor) {
			if(Input.GetKeyDown(KeyCode.O)) {
				ShowOverlayWhiteFlashOut();
			}
		}
		
		if(Application.isEditor) {
			if(Input.GetKeyDown(KeyCode.I)) {
				HideOverlayWhiteFlashOut();
			}
		}
		
		if(Application.isEditor) {
			if(Input.GetKeyDown(KeyCode.U)) {
				ShowOverlayWhiteFlash();
			}
		}
	}
	
	/*

    void OnButtonClickEventHandler(string buttonName) {
		LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
		
		if(buttonName == buttonNorahGlowObject.name 
			|| buttonName == buttonNorahStaticObject.name) {
			LogUtil.Log("Norah Clicked: " + buttonName);
		}

    }
	
    void OnListItemClickEventHandler(string listName, string listIndex, bool selected) {
        LogUtil.Log("OnListItemClickEventHandler: listName:" + listName + " listIndex:" + listIndex.ToString() + " selected:" + selected.ToString());

	}

    void OnListItemSelectEventHandler(string listName, string selectName) {
        LogUtil.Log("OnListItemSelectEventHandler: listName:" + listName + " selectName:" + selectName );

        if(listName == "ListState") {

        }
    }

    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {
        LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        // Change appstate

        if(sliderName == "AudioEffectsSlider") {
            //GameProfiles.Current.SetAudioEffectsVolume(sliderValue);
        }
    }
    
    void OnCheckboxChangeEventHandler(string checkboxName, bool selected) {
        LogUtil.Log("OnCheckboxChangeEventHandler: checkboxName:" + checkboxName + " selected:" + selected );
        
        // Change appstate
        
        if(checkboxName == "DeviceModeBestCheckbox") {
            //CameraDevice.Instance.SetFocusMode(
        }
    }
    */
	
}
