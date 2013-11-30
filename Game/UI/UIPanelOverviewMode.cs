#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelOverviewMode : UIPanelBase {

    public static UIPanelOverviewMode Instance;

    public GameObject containerOverview;

    // OVERVIEW

    public UILabel labelOverviewTip;
    public UILabel labelOverviewType;
    public UILabel labelOverviewStatus;

    public UILabel labelOverviewTitle;
    public UILabel labelOverviewBlurb;
    public UILabel labelOverviewBlurb2;
    public UILabel labelOverviewNextSteps;

    public UIImageButton buttonOverviewReady;
    public UIImageButton buttonOverviewTutorial;
    public UIImageButton buttonOverviewTips;

    //public UIPanelTips tips

    // GLOBAL

    public AppModeOverviewFlowState flowState = AppModeOverviewFlowState.AppModeOverview;

	public void Awake() {
		
        if (Instance != null && this != Instance) {
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

		//loadData();
	}	
	
	public override void Start() {
		Init();
	}

    // EVENTS
	
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.AddListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.RemoveListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }

    void OnButtonClickEventHandler(string buttonName) {
        if(UIUtil.IsButtonClicked(buttonOverviewReady, buttonName)) {
            Ready();
        }
        else if(UIUtil.IsButtonClicked(buttonOverviewTutorial, buttonName)) {
            ShowTutorial();
        }
        else if(UIUtil.IsButtonClicked(buttonOverviewTips, buttonName)) {
            ShowTips();
        }
    }
        
    public void ContentPause() {
        GameController.GameRunningStateContent();
    }
    
    public void ContentRun() {
        GameController.GameRunningStateRun();
    }

    public void Ready() {
        HideAll();
    }
    
    public void ShowTutorial() {

    }
    
    public void ShowTips() {

    }

    public void OnGameLevelItemsLoadedHandler() {

        Debug.Log("OnGameLevelItemsLoadedHandler");

        if(AppModeTypes.Instance.isAppModeTypeGameChoice) {

            Debug.Log("OnGameLevelItemsLoadedHandler2");
        }
    }

    public void ShowOverview() {

        //HideStates();

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        //Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerOverview);

        ContentPause();

        //UIColors.UpdateColors();
    }

    public void HideOverview() {

        //Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateOutBottom(containerOverview, 0f, 0f);

        ContentRun();
    }

    public void ShowCurrentState() {        
        UIPanelDialogBackground.ShowDefault();
        ShowOverview();
    }
    
    public void HideStates() {        

        UIPanelDialogBackground.HideAll();

        HideOverview();
    }

    // SHOW/LOAD

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

    public void Reset() {
        flowState = AppModeOverviewFlowState.AppModeOverview;
    }

	public static void LoadData() {
		if(Instance != null) {
			Instance.loadData();
		}
	}

    public void loadData() {
        //Debug.Log("UIPanelModeTypeChoice:loadData");
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
        yield return new WaitForSeconds(1f);

        Reset();

        ShowCurrentState();
    }

    public override void AnimateIn() {
        base.AnimateIn();

        ShowCurrentState();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideStates();
    }

    public void Update() {

    }
	
}
