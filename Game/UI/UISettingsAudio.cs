#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UISettingsAudio : GameObjectBehavior {
    
    public UISlider sliderMusicVolume;
    public UISlider sliderEffectsVolume;
    
    public void Awake() {
        
    }
    
    public  void Init() {
        
        loadData();
    }
    
    public void Start() {
        Init();
    }
    
    public void OnEnable() {
        
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
    }
    
    public void OnDisable() {
        
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
    }
    
    void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);                  
    }
    
    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {
        
        //LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        if (sliderEffectsVolume != null) {
            if (sliderName == sliderEffectsVolume.name) {
                GameAudio.SetProfileEffectsVolume(sliderValue);
            }
        }
        
        
        if (sliderMusicVolume != null) {
            if (sliderName == sliderMusicVolume.name) {
                GameAudio.SetProfileAmbienceVolume(sliderValue);
            }
        }
    }
    
    public void UpdateAudioValues() {
        float effectsVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        float musicVolume = (float)GameProfiles.Current.GetAudioMusicVolume();
        
        if (sliderMusicVolume != null) {
            sliderMusicVolume.sliderValue = musicVolume;
            sliderMusicVolume.ForceUpdate();
            
            GameAudio.SetProfileAmbienceVolume(musicVolume);
        }
        
        if (sliderEffectsVolume != null) {
            sliderEffectsVolume.sliderValue = effectsVolume;
            sliderEffectsVolume.ForceUpdate();
            
            GameAudio.SetProfileEffectsVolume(effectsVolume);
        }

        if(GameGlobal.appRunState == AppRunState.DEV) {
            
            GameProfiles.Current.SetAudioMusicVolume(GameGlobal.volumeEditorMusic);
            GameProfiles.Current.SetAudioEffectsVolume(GameGlobal.volumeEditorEffects);
            GameAudio.SetEffectsVolume(GameGlobal.volumeEditorEffects);
            GameAudio.SetAmbienceVolume(GameGlobal.volumeEditorMusic);
        }
    }
    
    public void loadData() {
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
        
        yield return new WaitForSeconds(1f);
        
        UpdateAudioValues();
    }
    
}

