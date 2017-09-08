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

        Messenger<string>.AddListener(GameMessages.gameLevelPause, OnGameLevelPauseHandler);
        Messenger<string>.AddListener(GameMessages.gameLevelResume, OnGameLevelResumeHandler);
        Messenger<string>.AddListener(GameMessages.gameLevelQuit, OnGameLevelQuitHandler);
    }

    public override void OnDisable() {
        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(GameMessages.gameLevelPause, OnGameLevelPauseHandler);
        Messenger<string>.RemoveListener(GameMessages.gameLevelResume, OnGameLevelResumeHandler);
        Messenger<string>.RemoveListener(GameMessages.gameLevelQuit, OnGameLevelQuitHandler);
    }

    void OnGameLevelPauseHandler(string levelCode) {
        showDefault();
    }

    void OnGameLevelResumeHandler(string levelCode) {
        hideAll();
    }

    void OnGameLevelQuitHandler(string levelCode) {
        hideAll();
    }

    public override void OnButtonClickEventHandler(string buttonName) {

    }
    
    public static void ShowDefault() {
        if(isInst) {

            Instance.showDefault();
        }
    }

    public void showDefault() {

        ShowCamera();

        AnimateIn();
    }

    public static void HideAll() {
        if(isInst) {

            Instance.hideAll();
        }
    }

    public void hideAll() {

        AnimateOut();

        HideCamera(.5f);

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

        //UpdateAudioValues();
    }

    public override void AnimateIn() {
        base.AnimateIn();

        TweenUtil.ShowObjectRight(containerPause, TweenCoord.local, true, .5f);
    }

    public override void AnimateOut() {
        base.AnimateOut();

        TweenUtil.HideObjectRight(containerPause, TweenCoord.local, true, .5f);
    }
}