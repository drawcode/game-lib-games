using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelLoader : GameUIPanelBase {
    
    public static GameUIPanelLoader Instance;

    public GameObject containerObject;
    
    public GameObject charactersObject;
    public GameObject enemiesObject;
    public GameObject logoObject;
    public GameObject loadingObject;
    public UILabel labelLoading;
    public GameObject sliderProgressObject;
    public UISlider sliderProgress;
    
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
    
    public override void OnEnable() {
        
        //Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
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
        
        //Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
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

    }
    
    public override void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();    
        
        //LoadData();
        AnimateIn();
    }
    
    public virtual void LoadData() {
        StartCoroutine(LoadDataCo());
    }
    
    IEnumerator LoadDataCo() {
        yield break;
    }
    
    public virtual void ShowLogo() {
        if(logoObject != null) {
            logoObject.Show();
            //logoObject.FadeTo(5f, 1f, 0f, LoopType.pingPong);
        }
    }
    
    public virtual void HideLogo() {
        if(logoObject != null) {
            //logoObject.FadeTo(5f, 1f, 0f, LoopType.pingPong);
        }
    }
    
    public virtual void ShowCharacters() {
        if(charactersObject != null) {
            charactersObject.Show();
            //charactersObject.FadeTo(5f, 1f, 0f, LoopType.pingPong);
        }
    }
    
    public virtual void HideCharacters() {
        if(charactersObject != null) {
            //charactersObject.FadeTo(5f, 1f, 0f, LoopType.pingPong);
        }
    }
    
    
    public virtual void ShowEnemies() {
        if(enemiesObject != null) {
            enemiesObject.Show();
            //enemiesObject.FadeTo(5f, 1f, 0f, LoopType.pingPong);
        }
    }
    
    public virtual void HideEnemies() {
        if(enemiesObject != null) {
            //enemiesObject.FadeTo(5f, 1f, 0f, LoopType.pingPong);
        }
    }
    
    public override void AnimateIn() {
        
        HideLogo();
        HideEnemies();
        HideCharacters();
        
        base.AnimateIn();
        
        ShowLogo();
        ShowEnemies();
        ShowCharacters();
    }
    
    public override void AnimateOut() {
        
        HideLogo();
        HideEnemies();
        HideCharacters();
        
        base.AnimateOut();
        
    }
    
    /*

    void OnButtonClickEventHandler(string buttonName) {
        LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
        
        if(buttonName == buttonNorahGlowObject.name 
            || buttonName == buttonNorahStaticObject.name) {
            LogUtil.Log("Norah Clicked: " + buttonName);
        }

    }
    
    void OnListItemClickEventHandler(string listName, string listIndex, bool selected) {
        LogUtil.Log("OnListItemClickEventHandler: listName:" + listName + " listIndex:" + listIndex.ToString() + " selected:" + selected.ToString());

    }

    void OnListItemSelectEventHandler(string listName, string selectName) {
        LogUtil.Log("OnListItemSelectEventHandler: listName:" + listName + " selectName:" + selectName );

        if(listName == "ListState") {

        }
    }

    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {
        LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        // Change appstate

        if(sliderName == "AudioEffectsSlider") {
            //GameProfiles.Current.SetAudioEffectsVolume(sliderValue);
        }
    }
    
    void OnCheckboxChangeEventHandler(string checkboxName, bool selected) {
        LogUtil.Log("OnCheckboxChangeEventHandler: checkboxName:" + checkboxName + " selected:" + selected );
        
        // Change appstate
        
        if(checkboxName == "DeviceModeBestCheckbox") {
            //CameraDevice.Instance.SetFocusMode(
        }
    }
    */
    
}