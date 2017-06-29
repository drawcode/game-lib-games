#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Engine.Events;
using Engine.Utility;

public class UIPanelPause : UIPanelBase {

    public GameObject listItemPrefab;

    public GameObject containerPause;

    /*
#if USE_UI_NGUI_2_7
#endif
#if USE_UI_NGUI_3
    public UIImageButton buttonResume;
    public UIImageButton buttonRestart;
    public UIImageButton buttonQuit;
    public UIImageButton buttonSettingsAudio;
#else
    public Text buttonResume;
    public Text buttonRestart;
    public Text buttonQuit;
    public Text buttonSettingsAudio;
#endif
*/

    public static UIPanelPause Instance;

#if USE_UI_NGUI_2_7
#endif
#if USE_UI_NGUI_3
	public UISlider sliderMusicVolume;
	public UISlider sliderEffectsVolume;	
#else
    public Slider sliderMusicVolume;
    public Slider sliderEffectsVolume;
#endif

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;

        panelTypes.Add(UIPanelBaseTypes.typeDialogHUD);
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

        Messenger<string>.AddListener(GameMessages.gameLevelPause, OnGameLevelPauseHandler);
        Messenger<string>.AddListener(GameMessages.gameLevelResume, OnGameLevelResumeHandler);
        Messenger<string>.AddListener(GameMessages.gameLevelQuit, OnGameLevelQuitHandler);

    }

    public override void OnDisable() {
        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);

        Messenger<string>.RemoveListener(GameMessages.gameLevelPause, OnGameLevelPauseHandler);
        Messenger<string>.RemoveListener(GameMessages.gameLevelResume, OnGameLevelResumeHandler);
        Messenger<string>.RemoveListener(GameMessages.gameLevelQuit, OnGameLevelQuitHandler);
    }

    void OnGameLevelPauseHandler(string levelCode) {
        //TweenUtil.ShowObjectRight(containerPause);
        AnimateIn();
    }

    void OnGameLevelResumeHandler(string levelCode) {
        //TweenUtil.HideObjectRight(containerPause);
        AnimateOut();
    }

    void OnGameLevelQuitHandler(string levelCode) {
        //TweenUtil.HideObjectRight(containerPause);

        AnimateOut();
    }

    public override void OnButtonClickEventHandler(string buttonName) {

    }

    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {

        //LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        bool changeAudio = true;

#if DEV
        if(Application.isEditor) {
            //GameProfiles.Current.SetAudioMusicVolume(GameGlobal.volumeEditor);
            //GameProfiles.Current.SetAudioEffectsVolume(GameGlobal.volumeEditor);
            //changeAudio = false;
        }
#endif

        if(!changeAudio) {
            return;
        }


        if(sliderEffectsVolume != null) {
            if(sliderName == sliderEffectsVolume.name) {
                //GameAudio.SetProfileEffectsVolume(sliderValue);
            }
        }

        if(sliderMusicVolume != null) {
            if(sliderName == sliderMusicVolume.name) {
                //GameAudio.SetProfileAmbienceVolume(sliderValue);
            }
        }
    }

    public static void ShowDefault() {
        if(isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if(isInst) {
            Instance.AnimateOut();
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
            //sliderMusicVolume.ForceUpdate();

            GameAudio.SetProfileEffectsVolume(musicVolume);
        }

        if(sliderEffectsVolume != null) {
            UIUtil.SetSliderValue(sliderEffectsVolume, effectsVolume);
            //sliderEffectsVolume.ForceUpdate();

            GameAudio.SetProfileAmbienceVolume(effectsVolume);
        }
    }

    public void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);

        //UpdateAudioValues();
    }

    public override void AnimateIn() {
        base.AnimateIn();

        TweenUtil.ShowObjectRight(containerPause, TweenCoord.local, true, .5f, 0f);
    }

    public override void AnimateOut() {
        base.AnimateOut();

        TweenUtil.HideObjectRight(containerPause, TweenCoord.local, true, .5f, 0f);
    }
}