using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

#if ENABLE_FEATURE_CHARACTER_CUSTOMIZE

public class BaseGameUIPanelCustomizeCharacterRPG : GameUIPanelBase {
    
    public static GameUIPanelCustomizeCharacterRPG Instance;

    public Camera cameraCustomize;

    public UICustomizeCharacterRPG customizeCharacterRPG;
    
    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public override void Awake() {
        base.Awake();
    }
 
    public override void Start() {
        Init();
    }
 
    public override void Init() {
        base.Init(); 
     
        //currentColors = GameProfiles.Current.GetCustomColorsRunner();
        //UpdateControls();
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
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
    }
     
    public virtual void UpdateControls() {
     
    }
 
    public static void LoadData() {
        if(GameUIPanelCustomizeCharacterRPG.Instance != null) {
            GameUIPanelCustomizeCharacterRPG.Instance.loadData();
        }
    }
 
    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }
 
    IEnumerator loadDataCo() {       
     
        LogUtil.Log("LoadDataCo");
     
        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();
         
            yield return new WaitForEndOfFrame();
                 
            //loadDataPowerups();
         
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();                
        }

        if(customizeCharacterRPG != null) {
             customizeCharacterRPG.loadData();
        }
    }
     
    public virtual void ClearList() {
        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();
        }
    }
    
    public override void HandleShow() {
        base.HandleShow();
        
        buttonDisplayState = UIPanelButtonsDisplayState.None;
        characterDisplayState = UIPanelCharacterDisplayState.Character;
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
    }
     
    public override void AnimateIn() {
     
        base.AnimateIn();
     
        loadData();
    }
 
    public override void AnimateOut() {
     
        base.AnimateOut();       
        ClearList();
    }
 
    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }

        if(cameraCustomize == null) {
            return;
        }
    }
}

#endif