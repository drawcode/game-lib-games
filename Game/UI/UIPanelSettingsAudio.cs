using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelSettingsAudio : UIPanelBase {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
	
	public UIImageButton buttonClose;
	
	public static UIPanelSettingsAudio Instance; 		
	
	public UISlider sliderMusicVolume;
	public UISlider sliderEffectsVolume;	
	
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
	
    void OnEnable() {
		Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);

    }
    
    void OnDisable() {
		Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);

    }
	
    void OnButtonClickEventHandler(string buttonName) {
		//Debug.Log("OnButtonClickEventHandler: " + buttonName);					
	}
	
    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {

        //Debug.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        if (sliderName == sliderEffectsVolume.name) {
            GameProfiles.Current.SetAudioEffectsVolume(sliderValue);
			GameAudio.SetEffectsVolume(sliderValue);
			GameState.Instance.SaveProfile();
        }
        else if (sliderName == sliderMusicVolume.name) {
            GameProfiles.Current.SetAudioMusicVolume(sliderValue);
			GameAudio.SetAmbienceVolume(sliderValue);
			GameState.Instance.SaveProfile();
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
	}
	
}
