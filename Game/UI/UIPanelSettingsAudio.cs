//#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelSettingsAudio : UIPanelBase {


    public GameObject listItemPrefab;

    public static UIPanelSettingsAudio Instance;


#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIImageButton buttonClose;
    public UISlider sliderMusicVolume;
    public UISlider sliderEffectsVolume;
#else
    public GameObject buttonClose;
    public GameObject sliderMusicVolume;
    public GameObject sliderEffectsVolume;
#endif


    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
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

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
    }

    public override void OnDisable() {
        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);					
    }

    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {

        //LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        if(sliderEffectsVolume != null) {
            if(sliderName == sliderEffectsVolume.name) {
                GameAudio.SetProfileEffectsVolume(sliderValue);
            }
        }


        if(sliderMusicVolume != null) {
            if(sliderName == sliderMusicVolume.name) {
                GameAudio.SetProfileAmbienceVolume(sliderValue);
            }
        }
    }

    public static void LoadData() {
        if(Instance != null) {
            Instance.loadData();
        }
    }

    public void UpdateAudioValues() {
        float effectsVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        float musicVolume = (float)GameProfiles.Current.GetAudioMusicVolume();

        if(sliderMusicVolume != null) {

            UIUtil.SetSliderValue(sliderMusicVolume, musicVolume);

            GameAudio.SetProfileAmbienceVolume(musicVolume);
        }

        if(sliderEffectsVolume != null) {

            UIUtil.SetSliderValue(sliderMusicVolume, effectsVolume);

            GameAudio.SetProfileEffectsVolume(effectsVolume);
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