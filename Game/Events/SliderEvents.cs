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

    // NGUI's UISlider.Start() ends with Set(rawValue, true) -- it re-announces the slider's own
    // PREFAB-AUTHORED value through the same SendMessage a real drag uses, so a receiver cannot
    // tell the two apart. UISlider.rawValue is serialised as 1f, and downstream UISettingsAudio
    // turns the callback into an eventAudioVolumeChanged broadcast, BaseAudioController commits it
    // through GameAudio.SetProfileAmbienceVolume, and the player's saved volume is gone at the next
    // save. Measured: a plain boot, nobody touching a slider, moved audio-music-volume 0.856 -> 1
    // on disk. The panels DO sync their sliders from the profile, but a second later
    // (UISettingsAudio.loadDataCo waits 1s) -- long after this has already landed.
    //
    // Whether it fires at all depends on undefined Start() order between this component and the
    // UISlider on the same GameObject: the Start() below is what assigns eventReceiver and
    // functionName, so if ours runs first the init callback is delivered, and if it runs second it
    // is dropped. That is why it took the music volume on some boots and the effects volume on
    // others, and why it read as intermittent for three iterations.
    //
    // A real change can only come from input or from a deliberate Set, and neither can reach a
    // slider on the frame it was started -- so the callback that arrives on the start frame is
    // initialisation, and is dropped. A sync FROM the profile in that same frame is dropped too,
    // which is what we want: reading a value out of the profile must never write it back.
    int startedFrame = -1;
    bool changeSeen = false;

    void Start() {

        startedFrame = Time.frameCount;

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

        // See startedFrame above -- this one is NGUI initialising, not the player.
        if (!changeSeen && Time.frameCount == startedFrame) {
            changeSeen = true;
            return;
        }

        changeSeen = true;

        Messenger<string, float>.Broadcast(SliderEvents.EVENT_ITEM_CHANGE, transform.name, changeValue);
    }
}