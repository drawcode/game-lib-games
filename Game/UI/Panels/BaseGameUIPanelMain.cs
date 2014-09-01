using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelMain : GameUIPanelBase {

    public static GameUIPanelMain Instance;
        
    public GameObject listItemPrefab;
    public GameObject listItemSetPrefab;
    public GameObject containerObject;
    public GameObject containerLogoObject;
    public GameObject containerPlayerObject;
    public GameObject containerPlayerObjectUCF;
    public GameObject containerPlayerObjectBU;
    public GameObject containerAppRate;
    public UIButton buttonPlayerDefaultObject;
    public UIButton buttonPlayerUCFObject;
    public UIButton buttonPlayerBUObject;
    public GameObject containerStartObject;
    public UILabel labelStartObject;
    
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
    
    public override void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();    
        
        //LoadData();
        AnimateIn();

        if (containerAppRate != null) {
            if (Context.Current.isWeb) {
                containerAppRate.Hide();
            }
        }
    }
    
    public void LoadData() {
        StartCoroutine(LoadDataCo());
    }
    
    IEnumerator LoadDataCo() {
        yield break;
    }

    public override void AnimateIn() {

        backgroundDisplayState = GameBackgroundDisplayState.None;

        base.AnimateIn();

        AnimateStartCharacter();
        Invoke("AnimateInDelayed", 1);
    }

    public virtual void AnimateInDelayed() {
        GameUIPanelFooter.ShowMain();
    }
    
    public virtual void AnimateStartCharacter() {
        
        if (containerStartObject != null) {
            UITweenerUtil.FadeTo(containerStartObject,
                UITweener.Method.EaseInOut, UITweener.Style.PingPong, 2f, 0f, .5f);
        }       
        
        /*
        if(buttonPlayerGlowObject != null) {
            UITweenerUtil.FadeTo(buttonPlayerGlowObject.gameObject,
                UITweener.Method.EaseInOut, UITweener.Style.PingPong, 2f, 0f, .1f);
        }
  */      
    }

    public virtual void OnButtonClickEventHandler(string buttonName) {

        bool loadCharacter = false;

        /*
        if (UIUtil.IsButtonClicked(buttonPlayerUCFObject, buttonName)) {

            GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;
            
            // SET CUSTOM VALUES FOR THIS PLAYER
                        
            customItem = GameCustomController.UpdateTexturePresetObject(
                customItem, GameController.CurrentGamePlayerController.gameObject,  
                AppContentAssetTexturePresets.Instance.GetByCode("fiestabowl"));

            customItem = GameCustomController.UpdateColorPresetObject(
                customItem, GameController.CurrentGamePlayerController.gameObject,   
                AppColorPresets.Instance.GetByCode("game-college-ucf-knights"));
                        
            GameCustomController.SaveCustomItem(customItem); 

            loadCharacter = true;
        }
        else if (UIUtil.IsButtonClicked(buttonPlayerBUObject, buttonName)) {
                
            GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;

            // SET CUSTOM VALUES FOR THIS PLAYER
            
            customItem = GameCustomController.UpdateTexturePresetObject(
                customItem, GameController.CurrentGamePlayerController.gameObject,  
                AppContentAssetTexturePresets.Instance.GetByCode("fiestabowl"));
            
            customItem = GameCustomController.UpdateColorPresetObject(
                customItem, GameController.CurrentGamePlayerController.gameObject,  
                AppColorPresets.Instance.GetByCode("game-college-baylor-bears"));

            GameCustomController.SaveCustomItem(customItem);
            
            loadCharacter = true;
        }
        else 
        */

        if (UIUtil.IsButtonClicked(buttonPlayerDefaultObject, buttonName)) {
            loadCharacter = true; 
        }

        if (loadCharacter) {
            LogUtil.Log("Player Clicked: " + buttonName);                 
            
            GameController.LoadCurrentProfileCharacter();
            GameUIController.ShowGameMode();   
        }
    }
}
