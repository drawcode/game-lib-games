using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelSettingsAudio : GameUIPanelBase {	
    
    public static GameUIPanelSettingsAudio Instance;    
	
    public GameObject listItemPrefab;	
	
	//public UISlider sliderMusicVolume;
    //public UISlider sliderEffectsVolume;
    
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
	
	public override void Init() {
		base.Init();	
		
		loadData();
	}	
	
	public override void Start() {
		Init();
		
        /*
		float effectsVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
		float musicVolume = (float)GameProfiles.Current.GetAudioMusicVolume();
		
		if(sliderMusicVolume != null) {
			sliderMusicVolume.sliderValue = musicVolume;
			sliderMusicVolume.ForceUpdate();
		}
		
		if(sliderEffectsVolume != null) {
			sliderEffectsVolume.sliderValue = effectsVolume;
			sliderEffectsVolume.ForceUpdate();
		}
  */      
	}

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);

        Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);

        Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
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
	
    public override void OnButtonClickEventHandler(string buttonName) {
		LogUtil.Log("OnButtonClickEventHandler: " + buttonName);					
	}
	
    public virtual void OnSliderChangeEventHandler(string sliderName, float sliderValue) {

        //LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        //if (sliderName == sliderEffectsVolume.name) {
        //    GameProfiles.Current.SetAudioEffectsVolume(sliderValue);
		//	GameState.SaveProfile();
        //}
        //else if (sliderName == sliderMusicVolume.name) {
        //    GameProfiles.Current.SetAudioMusicVolume(sliderValue);
		//	GameState.SaveProfile();
        //}
    }
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		yield return new WaitForSeconds(1f);
	}
	
}
