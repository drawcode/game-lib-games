#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;
using Engine.Utility;

public class UIPanelOverlayPrepare : UIPanelBase {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    // OVERVIEW

    public UILabel labelOverviewTip;
    public UILabel labelOverviewType;
    public UILabel labelOverviewStatus;

    public UILabel labelTipTitle;
    public UILabel labelTipDescription;
    public UILabel labelTipType;

    public UIImageButton buttonReady;

    public UIImageButton buttonTipNext;
#else
    // OVERVIEW

    public Text labelOverviewTip;
    public Text labelOverviewType;
    public Text labelOverviewStatus;

    public Text labelTipTitle;
    public Text labelTipDescription;
    public Text labelTipType;

    public Button buttonReady;

    public Button buttonTipNext;
#endif

    public static UIPanelOverlayPrepare Instance;

    public GameObject containerOverview;
    public GameObject containerOverviewGameplayTips;
    public GameObject containerOverviewGeneralTips;
    public GameObject containerTutorial;

    public GameObject containerTips;
    public GameObject containerTipsMode;
    public GameObject containerTipsGameplay;
    public GameObject containerTipsGeneral;

    public GameObject containerLoader;
    public GameObject containerLoaderSpinner;

    public string loadingLevelDisplay = "Loading Level...";

    //public UIPanelTips tips

    // GLOBAL

    public AppOverviewFlowState flowState = AppOverviewFlowState.GeneralTips;

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;
        
        HideCamera();

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
        Ready();
    }

    public override void Start() {
        Init();
    }

    // EVENTS

    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.AddListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);

        Messenger<string>.AddListener(UIPanelTipsMessages.tipsCycle, OnTipsCycleHandler);
    }

    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.RemoveListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);

        Messenger<string>.RemoveListener(UIPanelTipsMessages.tipsCycle, OnTipsCycleHandler);
    }

    string lastTipObjectName = "";

    void OnTipsCycleHandler(string objName) {
        if(objName != lastTipObjectName) {
            lastTipObjectName = objName;

            //if(flowState == AppOverviewFlowState.GameplayTips) {
            //    ChangeTipsState(AppOverviewFlowState.Mode);
            //}
            //else {            
            ChangeTipsState(AppOverviewFlowState.GeneralTips);
            //}
        }
    }

    public override void OnButtonClickEventHandler(string buttonName) {

        if(UIUtil.IsButtonClicked(buttonTipNext, buttonName)) {

            CancelInvoke("ShowOverviewTip");
            ShowOverviewTip();
        }
        /*
        if (UIUtil.IsButtonClicked(buttonOverviewReady, buttonName)) {
            Ready();
        }
        else if (UIUtil.IsButtonClicked(buttonOverviewMode, buttonName)) {
            ChangeTipsState(AppOverviewFlowState.Mode);
        }
        else if (UIUtil.IsButtonClicked(buttonOverviewTutorial, buttonName)) {
            ShowTutorial();
        }
        else if (UIUtil.IsButtonClicked(buttonOverviewTips, buttonName)) {
            ChangeTipsState(AppOverviewFlowState.GameplayTips);
        } 
        */
    }

    public void Ready() {
        //HideAll();
        HideStates();
    }

    public void ChangeTipsState(AppOverviewFlowState flowStateTo) {
        flowState = flowStateTo;
        UpdateTipsStates();
    }

    public void UpdateTipsStates() {

        if(flowState == AppOverviewFlowState.GeneralTips) {
            ShowTipsObjectGeneral();
        }
    }

    public void ShowTipsObjectGameplay() {
        //UIUtil.HideButton(buttonOverviewTips);
        //UIUtil.ShowButton(buttonOverviewMode);
        ShowTipsObject("gameplay");
    }

    public void ShowTipsObjectGeneral() {
        //UIUtil.HideButton(buttonOverviewTips);
        //UIUtil.ShowButton(buttonOverviewMode);
        ShowTipsObject("general");
    }

    public void ShowTipsObjectMode() {
        //UIUtil.HideButton(buttonOverviewMode);
        //UIUtil.ShowButton(buttonOverviewTips);
        //string currentAppContentState = AppContentStates.Current.code;
        //ShowTipsObject(currentAppContentState);
    }

    public void HideTipsObjects() {
        if(containerTips != null) {
            foreach(UIPanelTips tips in containerTips.GetComponentsInChildren<UIPanelTips>(true)) {
                tips.gameObject.Hide();
                                
                TweenUtil.FadeToObject(tips.gameObject, 0f, .4f, 0f);

                //UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .4f, 0f, 0f);
            }
            
            HideCamera();
        }
    }

    public void ShowTipsObject(string objName) {

        if(containerTips != null) {

            HideTipsObjects();

            foreach(UIPanelTips tips in containerTips.GetComponentsInChildren<UIPanelTips>(true)) {

                if(!string.IsNullOrEmpty(objName) && tips.name.Contains(objName)) {
                    tips.gameObject.Show();

                    TweenUtil.FadeToObject(tips.gameObject, 0f, 0f, 0f);
                    //UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);

                    tips.ShowTipsFirst();

                    TweenUtil.FadeToObject(tips.gameObject, 1f, .5f, .6f);
                    //UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, .6f, 1f);

                }
            }

            ShowCamera();
        }
    }

    public void ShowTutorial() {

        ShowCamera();

        HideStates();

        flowState = AppOverviewFlowState.Tutorial;

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        //LogUtil.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerTutorial);

        UIColors.UpdateColors();

    }

    public void HideTutorial() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        HideCamera();
    }

    public void ShowTips() {

        ShowCamera();

        HideStates();

        flowState = AppOverviewFlowState.GeneralTips;

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        //LogUtil.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerOverviewGameplayTips);

        UIColors.UpdateColors();

    }

    public void HideTips() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        HideCamera();
    }

    public void OnGameLevelItemsLoadedHandler() {

        //LogUtil.Log("OnGameLevelItemsLoadedHandler");

        if(AppModeTypes.Instance.isAppModeTypeGameChoice) {

            //LogUtil.Log("OnGameLevelItemsLoadedHandler2");
        }

        UIUtil.SetLabelValue(labelOverviewTip, "READY TO PLAY?");
        ShowButtonPlay();
    }

    public void ShowButtonPlay() {
        if(buttonReady != null) {
            buttonReady.gameObject.Show();
        }

        HideLoaderSpinner();
    }

    public void HideButtonPlay() {
        if(buttonReady != null) {
            buttonReady.gameObject.Hide();
        }

        ShowLoaderSpinner();
    }

    public void ShowLoader() {

        ShowCamera();

        containerLoader.Show();
    }

    public void HideLoader() {

        containerLoader.Hide();

        HideCamera();
    }

    public void ShowLoaderSpinner() {
        
        containerLoaderSpinner.Show();
    }

    public void HideLoaderSpinner() {
        containerLoaderSpinner.Hide();
    }

    public void UpdateOverviewWorld() {

        GameWorld gameWorld = GameWorlds.Current;

        if(gameWorld == null) {
            return;
        }

        foreach(GameObjectInactive obj in containerOverview.GetList<GameObjectInactive>()) {

            if(obj.type == BaseDataObjectKeys.overview
                && obj.code == BaseDataObjectKeys.world) {

                foreach(GameObjectInactive world in containerOverview.GetList<GameObjectInactive>()) {
                    if(world.type == BaseDataObjectKeys.world) {
                        TweenUtil.HideObjectBottom(world.gameObject);
                    }
                }

                foreach(GameObjectInactive world in containerOverview.GetList<GameObjectInactive>()) {

                    if(world.code.IsEqualLowercase(gameWorld.code)) {
                        TweenUtil.ShowObjectBottom(world.gameObject);
                    }
                }
            }
        }
    }


    public void ShowOverview() {
        
        HideStates();

        containerOverview.Show();

        ShowCamera();

        ShowLoaderSpinner();

        UpdateOverviewWorld();

        if(containerLoader.Has<GameObjectImageFill>()) {
            GameObjectImageFill fill = containerLoader.Get<GameObjectImageFill>();
            fill.Reset();
        }

        // Update team display

        //LogUtil.Log("ShowOverview:");

        flowState = AppOverviewFlowState.GeneralTips;

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        AnimateInBottom(containerOverview);

        UIColors.UpdateColors();

        InvokeRepeating("ShowOverviewTip", 0, 15);
    }

    List<AppContentTip> currentTips;
    AppContentTip currentTip;

    public void ShowOverviewTip() {

        if(currentTips == null) {
            currentTips = AppContentTips.Instance.items;
        }

        if(currentTips == null) {
            return;
        }

        if(currentTips.Count == 0) {
            return;
        }

        currentTips.Shuffle();

        currentTip = currentTips[0];

        UIUtil.SetLabelValue(labelTipTitle, currentTip.display_name);
        UIUtil.SetLabelValue(labelTipDescription, currentTip.description);
        UIUtil.SetLabelValue(labelTipType, currentTip.keys[0] + " Tips");
    }

    public void HideOverview() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        containerOverview.Hide();

        CancelInvoke("ShowOverviewTip");

        HideCamera();
    }

    public void ShowCurrentState() {
        ShowOverview();
    }

    public void HideStates() {

        HideButtonPlay();

        UIUtil.SetLabelValue(labelOverviewTip, loadingLevelDisplay);

        UIPanelDialogBackground.HideAll();

        UIPanelDialogRPGHealth.HideAll();
        UIPanelDialogRPGEnergy.HideAll();

        HideOverview();
        HideTutorial();
        HideTips();
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
            Instance.HideStates();
        }
    }

    public void Reset() {
        flowState = AppOverviewFlowState.GeneralTips;
    }

    public static void LoadData() {
        if(Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {
        //LogUtil.Log("UIPanelModeTypeChoice:loadData");
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        Reset();

        ShowCurrentState();

        UpdateTipsStates();

        yield return new WaitForSeconds(.1f);
    }

    public override void AnimateIn() {
        base.AnimateIn();

        loadData();
    }

    public override void AnimateOut() {
        //base.AnimateOut();

        //HideStates();

        //HideCamera();
    }

    public void Update() {

    }
}