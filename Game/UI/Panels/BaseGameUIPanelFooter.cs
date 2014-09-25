using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class GameUIPanelFooterButtons {
    public static string gameNetworks = "game-networks";
    public static string character = "character";
    public static string characterLarge = "character-large";
    public static string characterHelp = "character-help";
    public static string characterCustomize = "character-customize";
    public static string statistics = "statistics";
    public static string achievements = "achievements";
    public static string progression = "progression";
    public static string customize = "customize";
}

public class BaseGameUIPanelFooter : GameUIPanelBase {
    
    public static GameUIPanelFooter Instance;      

    public GameObject containerButtons;
			    
    public GameObject containerButtonsSettings;
    public GameObject containerButtonsCustomize;   
    
    public GameObject containerButtonsCharacter;   
    public GameObject containerButtonsCharacterLarge;   
	
    public GameObject containerButtonsGameNetworks;    
    public GameObject containerButtonsProgression;    
    public GameObject containerButtonsCharacterHelp; 
    public GameObject containerButtonsCharacterCustomize;
    
    public GameObject containerButtonsProgressionAchievements;
    public GameObject containerButtonsProgressionStatistics;  

    public bool optionsVisible = false;
    
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
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		loadData();
        
        AnimateIn();   
	}
	
	public override void AnimateIn() {
		
		base.AnimateIn();

        showNone();
	}
    	
	public virtual void AnimateInMain() {
		
		AnimateIn();
		
		showMain();
	}
	
	public virtual void AnimateInInternal() {
		
		AnimateIn();
		
		showFull();
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		
		showNone();
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
	
    public virtual void HideContainerDelayed() {
		StartCoroutine(HideContainerDelayedCo());
	}
	
	IEnumerator HideContainerDelayedCo() {
		yield return new WaitForSeconds(2f);
		//HideOptionsContainer();
    }
    
    public static void ShowFull() {
        if(isInst) {
            Instance.showFull();
        }
    }
	
    public virtual void showFull() {
		showMain();
    }	

    public static void ShowMain() {
        if(isInst) {
            Instance.showMain();
        }
    }
	
    public virtual void showMain() {
        showButtonSettings();
        showButtonCustomize();
	}
	
    public virtual void showNone() {

        hideButtonSettings();
        hideButtonCustomize();

        HideAllButtons();
  	}   

    //

    public virtual void ShowButtons(string code) {
        
        AnimateIn();
                
        foreach(GameObjectShowItem item in 
                containerButtons.GetComponentsInChildren<GameObjectShowItem>(true)) {

            if(item.code == code) {
                HideAllButtons();
                ShowPanelBottom(item.gameObject);
                item.gameObject.ShowObjectDelayed(.7f);
            }
        }
    }

    public virtual void HideAllButtons() {        
        
        foreach(GameObjectShowItem item in 
                containerButtons.GetComponentsInChildren<GameObjectShowItem>(true)) {
            HidePanelBottom(item.gameObject);
            item.gameObject.HideObjectDelayed(.5f);
        }
    }

    // HIDE/SHOW
        
    // BUTTON SETTINGS

    public virtual void showButtonSettings() {

        AnimateInLeftBottom(containerButtonsSettings);
    }
        
    public virtual void hideButtonSettings() {
        
        AnimateOutLeftBottom(containerButtonsSettings);
    }

    // BUTTON CUSTOMIZE
        
    public virtual void showButtonCustomize() {
                
        AnimateInRightBottom(containerButtonsCustomize);
    }
            
    public virtual void hideButtonCustomize() {
                
        AnimateOutRightBottom(containerButtonsCustomize);
    }

    // BUTTON GAME NETWORKS

    public static void ShowButtonGameNetworks() {
        if(isInst) {
            Instance.showButtonGameNetworks();
        }
    }

    public virtual void showButtonGameNetworks() {

        AnimateIn();
        
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) { 
            
            AnimateInRightBottom(containerButtonsGameNetworks);           
            showButtonGameNetworkGameCenter();
        }
        else if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            
            AnimateInRightBottom(containerButtonsGameNetworks);
            showButtonGameNetworkPlayServices();
        }
        else {
            ShowButtonsProgression();
        }
    }
        
    public virtual void hideButtonGameNetworks() {
        
        AnimateOutRightBottom(containerButtonsGameNetworks);
    }

    // BUTTON GAME NETWORKS
        
    public virtual void showButtonGameNetworkGameCenter() {                
        ShowButtonsGameNetwork(GameNetworkType.gameNetworkAppleGameCenter);
    }    
    
    public virtual void showButtonGameNetworkPlayServices() {                
        ShowButtonsGameNetwork(GameNetworkType.gameNetworkGooglePlayServices);
    }

	
    // BUTTONS

    public virtual void ShowButtonsGameNetwork() {
        ShowButtons(GameUIPanelFooterButtons.gameNetworks);
    }

    public virtual void ShowButtonsGameNetwork(string networkTypeTo) {
        if(containerButtonsGameNetworks != null) {

            ShowButtonsGameNetwork();

            foreach(GameObjectInactive item in 
                    containerButtonsGameNetworks.GetComponentsInChildren<GameObjectInactive>(true)) {
                if(item.name == networkTypeTo || item.code == networkTypeTo) {
                    item.gameObject.Show();
                }
                else {
                    item.gameObject.Hide();
                }
            }
        }
    }
    
    public virtual void HideButtonsGameNetworks() {
        if(containerButtonsGameNetworks != null) {
            foreach(GameObjectInactive item in 
                    containerButtonsGameNetworks.GetComponentsInChildren<GameObjectInactive>(true)) {
                item.gameObject.Hide();
            }
        }
    }

    //
          
    public static void ShowButtonsCharacterCustomize() {
        if(isInst) {
            Instance.showButtonsCharacterCustomize();
        }
    }
    
    public virtual void showButtonsCharacterCustomize() {        
        ShowButtons(GameUIPanelFooterButtons.characterCustomize);
    }
    
    //
    
    public static void ShowButtonsCharacter() {
        if(isInst) {
            Instance.showButtonsCharacter();
        }
    }
    
    public virtual void showButtonsCharacter() {        
        ShowButtons(GameUIPanelFooterButtons.character);
    }
    
    //
    
    public static void ShowButtonsCharacterLarge() {
        if(isInst) {
            Instance.showButtonsCharacterLarge();
        }
    }
    
    public virtual void showButtonsCharacterLarge() {        
        ShowButtons(GameUIPanelFooterButtons.characterLarge);
    }

    //
    
    public static void ShowButtonsCharacterHelp() {
        if(isInst) {
            Instance.showButtonsCharacterHelp();
        }
    }

    public virtual void showButtonsCharacterHelp() {
        ShowButtons(GameUIPanelFooterButtons.characterHelp);
    }
    
    public static void ShowButtonsProgression() {
        if(isInst) {
            Instance.showButtonsProgression();
        }
    }
    
    public virtual void showButtonsProgression() {        
        ShowButtons(GameUIPanelFooterButtons.progression);
    }
    
    public static void ShowButtonsStatistics() {
        if(isInst) {
            Instance.showButtonsStatistics();
        }
    }
    
    public virtual void showButtonsStatistics() {        
        ShowButtons(GameUIPanelFooterButtons.statistics);
    }
    
    public static void ShowButtonsAchievements() {
        if(isInst) {
            Instance.showButtonsAchievements();
        }
    }
    
    public virtual void showButtonsAchievements() {        
        ShowButtons(GameUIPanelFooterButtons.achievements);
    }

    //
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		yield return new WaitForSeconds(1f);
	}
    
    /*
    public static void HideOptionsContainer() {
        if(Instance != null) {
            //Instance.hideOptionsContainer();
        }
    }
    
    public void hideOptionsContainer() {
        
        optionsVisible = false;
        
        if(containerOptionsExtra != null) {
            UITweenerUtil.FadeTo(containerOptionsExtra, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .2f, 0f);
        }
        
        if(buttonOptionsSocial != null) {
            UITweenerUtil.FadeTo(buttonOptionsSocial.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .2f, 0f);
        }       
        
        if(buttonOptionsRate != null) {
            UITweenerUtil.FadeTo(buttonOptionsRate.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .2f, 0f);
        }
        
        if(buttonOptionsCredits != null) {
            UITweenerUtil.FadeTo(buttonOptionsCredits.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .3f, 0f);
        }
        
        if(buttonOptionsAudio != null) {
            UITweenerUtil.FadeTo(buttonOptionsAudio.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .3f, 0f);
        }
        
        if(containerOptionsExtraBackground != null) {
            UITweenerUtil.FadeTo(containerOptionsExtraBackground, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .5f, 0f);
        }
    }
    
    
    public static void ShowOptionsContainer() {
        if(Instance != null) {
            Instance.showOptionsContainer();
        }
    }
    
    public void showOptionsContainer() {
        
        //if(!optionsVisible) {
        optionsVisible = true;
        
        if(containerOptionsExtra != null) {
            UITweenerUtil.FadeTo(containerOptionsExtra, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .3f, 1f);
        }
        
        if(buttonOptionsSocial != null) {
            UITweenerUtil.FadeTo(buttonOptionsSocial.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .7f, 1f);
        }       
        
        if(buttonOptionsRate != null) {
            UITweenerUtil.FadeTo(buttonOptionsRate.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .6f, 1f);
        }
        
        if(buttonOptionsCredits != null) {
            UITweenerUtil.FadeTo(buttonOptionsCredits.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .5f, 1f);
        }
        
        if(buttonOptionsAudio != null) {
            UITweenerUtil.FadeTo(buttonOptionsAudio.gameObject, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .4f, 1f);
        }
        
        if(containerOptionsExtraBackground != null) {
            UITweenerUtil.FadeTo(containerOptionsExtraBackground, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, .5f, .3f, .5f);
        }       
        //}
    }
 */
	
}
