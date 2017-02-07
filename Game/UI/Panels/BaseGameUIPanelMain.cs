using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class BaseGameUIPanelMain : GameUIPanelBase {

    public static GameUIPanelMain Instance;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIButton buttonPlayerDefaultObject;
    public UIButton buttonPlayerUCFObject;
    public UIButton buttonPlayerBUObject;
#else
    public Button buttonPlayerDefaultObject;
    public Button buttonPlayerUCFObject;
    public Button buttonPlayerBUObject;
#endif
    
    public GameObject listItemPrefab;
    public GameObject listItemSetPrefab;
    public GameObject containerObject;
    public GameObject containerLogoObject;
    public GameObject containerPlayerObject;
    public GameObject containerPlayerObjectUCF;
    public GameObject containerPlayerObjectBU;
    public GameObject containerAppRate;
    public GameObject containerStartObject;
    
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
        //AnimateIn();

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
        
    public override void HandleShow() {
        base.HandleShow();
        
        buttonDisplayState = UIPanelButtonsDisplayState.None;
        characterDisplayState = UIPanelCharacterDisplayState.None;
        backgroundDisplayState = UIPanelBackgroundDisplayState.None;
        
        GameCommunity.HideBroadcastRecordPlayShare();
    }

    public override void HandleHide() {
        base.HandleHide();

        GameCommunity.HideActionAppRate();
        GameCommunity.HideBroadcastRecordPlayShare();
    }

    public override void AnimateIn() {

        backgroundDisplayState = UIPanelBackgroundDisplayState.None;

        base.AnimateIn();

        AnimateStartCharacter();
        Invoke("AnimateInDelayed", 1);
    }

    public override void AnimateOut() {
        base.AnimateOut();

        CancelInvoke("AnimateInDelayed");

        HandleHide();
    }

    public virtual void AnimateInDelayed() {

        GameUIPanelFooter.ShowMain();
        
        GameCommunity.HideBroadcastRecordPlayShare();
        
        GameCommunity.ShowActionAppRate();
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

    public override void OnButtonClickEventHandler(string buttonName) {

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
#if ENABLE_FEATURE_GAME_MODE
            GameUIController.ShowGameMode();   
#endif
        }
    }
}
