using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

#if ENABLE_FEATURE_CHARACTER_CUSTOMIZE

public class BaseGameUIPanelCustomizeCharacterColors : GameUIPanelBase {
    
    public static GameUIPanelCustomizeCharacterColors Instance;
    
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

		//UpdateControls();
	}

    public override void OnEnable() {

        Messenger<string>.AddListener(
            ButtonEvents.EVENT_BUTTON_CLICK, 
            OnButtonClickEventHandler);

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

        Messenger<string>.RemoveListener(
            ButtonEvents.EVENT_BUTTON_CLICK, 
            OnButtonClickEventHandler);

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

    public override void OnButtonClickEventHandler(string buttonName) {
    
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


	
	public static void LoadData() {
		if(GameUIPanelCustomizeCharacterColors.Instance != null) {
            GameUIPanelCustomizeCharacterColors.Instance.loadData();
		}

	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
        
        yield return new WaitForEndOfFrame();
        
        // Set currentObject of customizers

        foreach(UICustomizeObject customObject in gameObject.GetList<UICustomizeObject>()) {
            customObject.currentObject = GameController.CurrentGamePlayerController.gameObject;
        }

        Init();
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
	}
	
    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }
	}
			
    public virtual void LateUpdate() {

	}	
	
}

#endif
