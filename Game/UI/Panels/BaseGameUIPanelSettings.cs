using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

#if ENABLE_FEATURE_SETTINGS

public class BaseGameUIPanelSettings : GameUIPanelBase {

    public static GameUIPanelSettings Instance;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIImageButton buttonSettingsAudio;
    public UIImageButton buttonSettingsControls;
    public UIImageButton buttonSettingsProfile;
    public UIImageButton buttonSettingsHelp;
    public UIImageButton buttonSettingsCredits;
#else
    public Button buttonSettingsAudio;
    public Button buttonSettingsControls;
    public Button buttonSettingsProfile;
    public Button buttonSettingsHelp;
    public Button buttonSettingsCredits;
#endif

    public GameObject listItemPrefab;

    public static bool isInst {
        get {
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }

    public virtual void Awake() {

    }

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();

        loadData();
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
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if (className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if (className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if (className == classNameTo) {
            //
        }
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);

        if (UIUtil.IsButtonClicked(buttonSettingsAudio, buttonName)) {

#if ENABLE_FEATURE_SETTINGS_AUDIO
            GameUIController.ShowSettingsAudio();
#endif
        }

#if ENABLE_FEATURE_SETTINGS_CONTROLS
        else if(UIUtil.IsButtonClicked(buttonSettingsControls, buttonName)) {
            GameUIController.ShowSettingsControls();
        }
#endif


#if ENABLE_FEATURE_SETTINGS_PROFILE
        else if(UIUtil.IsButtonClicked(buttonSettingsProfile, buttonName)) {
            GameUIController.ShowSettingsProfile();
        }
#endif

#if ENABLE_FEATURE_SETTINGS_HELP
        else if(UIUtil.IsButtonClicked(buttonSettingsHelp, buttonName)) {
            GameUIController.ShowSettingsHelp();
        }
#endif

#if ENABLE_FEATURE_SETTINGS_CREDITS
        else if(UIUtil.IsButtonClicked(buttonSettingsCredits, buttonName)) {
            GameUIController.ShowSettingsCredits();
        }
#endif
    }

    public override void HandleShow() {
        base.HandleShow();

        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
    }

    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);
    }
}

#endif