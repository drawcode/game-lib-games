using System;
using System.Collections;

using UnityEngine;

using Engine.Events;
using UnityEngine.UI;

public class SliderEvents : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UISlider currentObj;
#else
    GameObject currentObj;
#endif

    public static string EVENT_ITEM_CLICK = "event-slider-item-click";
    public static string EVENT_ITEM_CHANGE = "event-slider-item-change";

    void Start() {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        currentObj = GetComponent<UISlider>();
#else
        if(currentObj.Has<Slider>()) {
            currentObj = GetComponent<Slider>().gameObject;
        }
#endif


        if (currentObj != null) {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            currentObj.functionName = "OnSliderChange";
            currentObj.eventReceiver = gameObject;
#else
            // TODO Unity UI
#endif
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

        // See InputEvents: pointer identity now comes from the registered UI backend.
        int camIndex = UIUtil.GetPointerId(gameObject);

        Messenger<string, int>.Broadcast(SliderEvents.EVENT_ITEM_CLICK, transform.name, camIndex);
    }

    void OnSliderChange(float changeValue) {
        //LogUtil.Log("SliderEvents:OnSliderChange: name: " + transform.name + " changeValue:" + changeValue);
        Messenger<string, float>.Broadcast(SliderEvents.EVENT_ITEM_CHANGE, transform.name, changeValue);
    }
}