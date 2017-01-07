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

public class UIPanelOverviewMode : UIPanelBase {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    // OVERVIEW

    public UILabel labelOverviewTip;
    public UILabel labelOverviewType;
    public UILabel labelOverviewStatus;
    public UIImageButton buttonOverviewReady;
    public UIImageButton buttonOverviewTutorial;
    public UIImageButton buttonOverviewTips;
    public UIImageButton buttonOverviewMode;
    public UILabel labelOverviewTeamEnemy;
#else
    // OVERVIEW

    public Text labelOverviewTip;
    public Text labelOverviewType;
    public Text labelOverviewStatus;
    public Button buttonOverviewReady;
    public Button buttonOverviewTutorial;
    public Button buttonOverviewTips;
    public Button buttonOverviewMode;
    public Text labelOverviewTeamEnemy;
#endif

    public static UIPanelOverviewMode Instance;
    public GameObject containerOverview;
    public GameObject containerOverviewGameplayTips;
    public GameObject containerTutorial;
    public GameObject containerTips;
    public GameObject containerTipsMode;
    public GameObject containerTipsGameplay;

    //public UIPanelTips tips

    // GLOBAL

    public AppOverviewFlowState flowState = AppOverviewFlowState.Mode;

    public void Awake() {

        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;

        panelTypes.Add(UIPanelBaseTypes.typeDialogHUD);
    }

    public static bool isInst {
        get {
            if (Instance != null) {
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

        Messenger<string>.AddListener(GameMessages.gameInitLevelEnd, OnGameInitLevelEnd);
    }

    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.RemoveListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);

        Messenger<string>.RemoveListener(UIPanelTipsMessages.tipsCycle, OnTipsCycleHandler);

        Messenger<string>.RemoveListener(GameMessages.gameInitLevelEnd, OnGameInitLevelEnd);
    }

    void OnGameInitLevelEnd(string levelCode) {
        ShowTipsObjectMode();
    }

    string lastTipObjectName = "";

    void OnTipsCycleHandler(string objName) {
        if (objName != lastTipObjectName) {
            lastTipObjectName = objName;

            if (flowState == AppOverviewFlowState.GameplayTips) {
                ChangeTipsState(AppOverviewFlowState.Mode);
            }
            else {
                ChangeTipsState(AppOverviewFlowState.GameplayTips);
            }

        }

        GameCustomController.BroadcastCustomSync();
    }

    public override void OnButtonClickEventHandler(string buttonName) {
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
    }

    public void ContentPause() {
        GameController.GameRunningStateContent();
    }

    public void ContentRun() {
        GameController.GameRunningStateRun();
    }

    public void Ready() {

        Messenger.Broadcast(GameMessages.gameLevelPlayerReady);

        HideAll();
    }

    public void ChangeTipsState(AppOverviewFlowState flowStateTo) {
        flowState = flowStateTo;
        UpdateTipsStates();
    }

    public void UpdateTipsStates() {

        if (flowState == AppOverviewFlowState.GameplayTips) {
            ShowTipsObjectGameplay();
        }
        else {
            ShowTipsObjectMode();
        }
    }

    public void ShowTipsObjectGameplay() {
        UIUtil.HideButton(buttonOverviewTips);
        UIUtil.ShowButton(buttonOverviewMode);
        ShowTipsObject("gameplay");
    }

    public void ShowTipsObjectMode() {
        UIUtil.HideButton(buttonOverviewMode);
        UIUtil.ShowButton(buttonOverviewTips);
        string currentAppContentState = AppContentStates.Current.code;
        ShowTipsObject(currentAppContentState);
    }

    public void HideTipsObjects() {
        if (containerTips != null) {
            foreach (UIPanelTips tips in containerTips.GetComponentsInChildren<UIPanelTips>(true)) {
                tips.gameObject.Hide();
                UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .4f, 0f, 0f);
            }
        }
    }

    public void ShowTipsObject(string objName) {

        if (containerTips != null) {

            HideTipsObjects();

            foreach (UIPanelTips tips in containerTips.GetComponentsInChildren<UIPanelTips>(true)) {

                if (!string.IsNullOrEmpty(objName) && tips.name.Contains(objName)) {
                    tips.gameObject.Show();
                    UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);
                    tips.ShowTipsFirst();
                    UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, .6f, 1f);

                }
            }
        }
    }

    public void ShowTutorial() {

        HideStates();

        flowState = AppOverviewFlowState.Tutorial;

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        //LogUtil.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerTutorial);

        ContentPause();

        UIColors.UpdateColors();

    }

    public void HideTutorial() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        ContentRun();
    }

    public void ShowTips() {

        HideStates();

        flowState = AppOverviewFlowState.GameplayTips;

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        //LogUtil.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerOverviewGameplayTips);

        ContentPause();

        UIColors.UpdateColors();

    }

    public void HideTips() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        ContentRun();
    }

    public void OnGameLevelItemsLoadedHandler() {

        //LogUtil.Log("OnGameLevelItemsLoadedHandler");

        if (AppModeTypes.Instance.isAppModeTypeGameChoice) {

            //LogUtil.Log("OnGameLevelItemsLoadedHandler2");
        }
    }

    public void UpdateOverviewWorld() {

        GameWorld gameWorld = GameWorlds.Current;

        if (gameWorld == null) {
            return;
        }

        foreach (GameObjectInactive obj in containerOverview.GetList<GameObjectInactive>()) {

            if (obj.type == BaseDataObjectKeys.overview
                && obj.code == BaseDataObjectKeys.world) {

                foreach (GameObjectInactive world in containerOverview.GetList<GameObjectInactive>()) {
                    if (world.type == BaseDataObjectKeys.world) {
                        TweenUtil.HideObjectBottom(world.gameObject);
                    }
                }

                foreach (GameObjectInactive world in containerOverview.GetList<GameObjectInactive>()) {

                    if (world.code.IsEqualLowercase(gameWorld.code)) {
                        TweenUtil.ShowObjectBottom(world.gameObject);
                    }
                }
            }
        }
    }

    public void ShowOverview() {

        HideStates();

        // Update team display
        //LogUtil.Log("ShowOverview:");

        flowState = AppOverviewFlowState.Mode;

        UIPanelDialogBackground.ShowDefault();

        UpdateOverviewWorld();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        AnimateInBottom(containerOverview);

        GameCustomController.BroadcastCustomSync();

        foreach (GameCustomPlayer customPlayer in gameObject.GetList<GameCustomPlayer>()) {

            if (customPlayer.isActorTypeEnemy) {

                GameTeam team = GameTeams.Current;

                if (team != null) {

                    UIUtil.SetLabelValue(labelOverviewTeamEnemy, team.display_name);

                    GameCustomCharacterData customInfo = new GameCustomCharacterData();
                    customInfo.actorType = GameCustomActorTypes.enemyType;
                    customInfo.presetColorCode = team.data.GetColorPreset().code;
                    customInfo.presetTextureCode = team.data.GetTexturePreset().code;
                    customInfo.type = GameCustomTypes.teamType;
                    customInfo.teamCode = team.code;

                    customPlayer.Load(customInfo);
                }
            }
        }

        ContentPause();

        UIColors.UpdateColors();
    }

    public void HideOverview() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        ContentRun();
    }

    public void ShowCurrentState() {
        ShowOverview();
    }

    public void HideStates() {

        UIPanelDialogBackground.HideAll();

        UIPanelDialogRPGHealth.HideAll();
        UIPanelDialogRPGEnergy.HideAll();

        HideOverview();
        HideTutorial();
        HideTips();
    }

    // SHOW/LOAD

    public static void ShowDefault() {
        if (isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if (isInst) {
            Instance.AnimateOut();
        }
    }

    public void Reset() {
        flowState = AppOverviewFlowState.Mode;
    }

    public static void LoadData() {
        if (Instance != null) {
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

        yield return new WaitForSeconds(1f);
    }

    public override void AnimateIn() {
        base.AnimateIn();

        UIPanelDialogBackground.ShowDefault();

        loadData();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideStates();
    }

    public void Update() {

    }
}