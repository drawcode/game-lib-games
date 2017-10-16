using System;
using System.Collections;

using UnityEngine;

using Engine.Events;

public class SliderEvents : GameObjectBehavior {
    
    UISlider currentObj;
    public static string EVENT_ITEM_CLICK = "event-slider-item-click";
    public static string EVENT_ITEM_CHANGE = "event-slider-item-change";
    
    void Start() {
        currentObj = GetComponent<UISlider>();
        if (currentObj != null) {
            currentObj.functionName = "OnSliderChange";
            currentObj.eventReceiver = gameObject;
        }
        
        //LoadData();
    }
    
    /*
    void LoadData() {
        
        string sliderName = transform.name;
        float sliderValue = 1f;
        
        if (sliderName == "AudioEffectsSlider") {
            sliderValue = (float)GameProfiles.Current.GetAudioEffectsVolume();
        }
        else if (sliderName == "AudioMusicSlider") {
            sliderValue = (float)GameProfiles.Current.GetAudioMusicVolume();
        }
        else if (sliderName == "AudioVOSlider") {
            //sliderValue = (float)GameProfiles.Current.GetAudioVOVolume();
        }
        
        if (currentObj != null) {
            currentObj.sliderValue = sliderValue;
        }
    }
    */
    
    void OnClick() {
        Messenger<string,int>.Broadcast(SliderEvents.EVENT_ITEM_CLICK, transform.name, UICamera.currentTouchID);
    }
    
    void OnSliderChange(float changeValue) {
        //LogUtil.Log("SliderEvents:OnSliderChange: name: " + transform.name + " changeValue:" + changeValue);
        Messenger<string, float>.Broadcast(SliderEvents.EVENT_ITEM_CHANGE, transform.name, changeValue);
    }
}
