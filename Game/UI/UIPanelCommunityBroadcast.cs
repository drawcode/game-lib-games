#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityBroadcast : UIPanelCommunityBase {

    public static UIPanelCommunityBroadcast Instance;
    public GameObject panelBroadcastButton;
    public GameObject panelBroadcastRecord;
    public GameObject panelBroadcastRecordPlayShare;
    public GameObject buttonBroadcastReplay;
    public GameObject buttonBroadcastShare;
    public GameObject buttonBroadcastRecordStart;
    public GameObject buttonBroadcastRecordStop;
    public GameObject buttonBroadcastFacecamToggle;
    public GameObject buttonBroadcastOpen;
    public GameObject containerSupported;
    public GameObject containerNotSupported;
    public UICheckbox toggleRecordReplaysLevel;
    bool isEnabled = false;
    bool isSupported = false;
    bool isRecordingSupported = false;
    bool isFacecamSupported = false;

    public override void Awake() {

        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;
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

        //loadData();

        showBroadcastButton();

        UpdateState();

        UpdateBroadcastStatus(BroadcastNetworksMessages.broadcastRecordingStop);

        // hide before any recording has been made

        hideButtonBroadcastShare();
        hideButtonBroadcastReplay();
        hideBroadcastRecordPlayShare();
    }

    public override void Start() {
        Init();
    }

    // EVENTS
 
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnToggleChangedEventHandler);
        
        Messenger<string>.AddListener(
            BroadcastNetworksMessages.broadcastRecordingStatusChanged, 
            OnBroadcastRecordStatusChanged);
        
        Messenger<string>.AddListener(GameMessages.gameLevelStart, OnGameLevelStart);
        Messenger<string>.AddListener(GameMessages.gameLevelEnd, OnGameLevelEnd);
        Messenger<string>.AddListener(GameMessages.gameLevelQuit, OnGameLevelQuit);
                
        Messenger.AddListener(GameMessages.gameResultsStart, OnGameResultsStart);
        Messenger.AddListener(GameMessages.gameResultsEnd, OnGameResultsEnd);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnToggleChangedEventHandler);

        Messenger<string>.RemoveListener(
            BroadcastNetworksMessages.broadcastRecordingStatusChanged, 
            OnBroadcastRecordStatusChanged);
        
        Messenger<string>.RemoveListener(GameMessages.gameLevelStart, OnGameLevelStart);
        Messenger<string>.RemoveListener(GameMessages.gameLevelEnd, OnGameLevelEnd);
        Messenger<string>.RemoveListener(GameMessages.gameLevelQuit, OnGameLevelQuit);
        
        Messenger.RemoveListener(GameMessages.gameResultsStart, OnGameResultsStart);
        Messenger.RemoveListener(GameMessages.gameResultsEnd, OnGameResultsEnd);
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        base.OnButtonClickEventHandler(buttonName);
    }

    public void OnToggleChangedEventHandler(string checkboxName, bool selected) {

        if (UIUtil.IsCheckboxChecked(toggleRecordReplaysLevel, checkboxName)) {
            Debug.Log("OnToggleChangedEventHandler" + " checkboxName:" + checkboxName + " selected:" + selected.ToString());
        
            GameProfiles.Current.SetBroadcastRecordLevels(selected);
            GameState.SaveProfile();        
        }
    }
    
    public void OnBroadcastRecordStatusChanged(string broadcastStatus) {
        
        UpdateBroadcastStatus(broadcastStatus);
    }

    public void OnGameResultsStart() {
        //ShowBroadcastRecordPlayShare();
    }

    public void OnGameResultsEnd() {
        HideBroadcastRecordPlayShare();
    }

    public void OnGameLevelStart(string levelCode) {

        Debug.Log("Broadcast: OnGameLevelStart" + " levelCode:" + levelCode);

        if (BroadcastNetworks.broadcastNetworksEnabled) {

            if (GameProfiles.Current.GetBroadcastRecordLevels()) {
                
                Debug.Log("OnGameLevelStart" + " record levels:" + GameProfiles.Current.GetBroadcastRecordLevels());
                
                if (!BroadcastNetworks.IsRecording()) {
                    BroadcastNetworks.StartRecording();
                }
            }
        }
    }

    public void OnGameLevelEnd(string levelCode) { 
                
        Debug.Log("Broadcast: OnGameLevelEnd" + " levelCode:" + levelCode);

        BroadcastGameLevelFinishDelayed(3.5f);
    }

    public void OnGameLevelQuit(string levelCode) { 
        
        Debug.Log("Broadcast: OnGameLevelQuit" + " levelCode:" + levelCode);
        
        BroadcastGameLevelFinishData(false);
    }

    public void BroadcastGameLevelFinishData(bool showReplayCallout) {
        Debug.Log("Broadcast: BroadcastGameLevelFinishData:Starting");
        
        if (BroadcastNetworks.broadcastNetworksEnabled) {
            
            if (GameProfiles.Current.GetBroadcastRecordLevels()) {
                
                if (BroadcastNetworks.IsRecording()) {

                    Debug.Log("BroadcastGameLevelFinishData" + " record levels:" + 
                              GameProfiles.Current.GetBroadcastRecordLevels());
                    
                    BroadcastNetworks.StopRecording();
                                        
                    if(showReplayCallout) {
                        ShowBroadcastRecordPlayShare();
                    }

                    GamePlayerRuntimeData runtimeData = new GamePlayerRuntimeData();
                    GamePlayerController gamePlayerController = GameController.CurrentGamePlayerController;
                    
                    if (gamePlayerController != null) {
                        runtimeData = gamePlayerController.runtimeData;                        
                    }

                    string word = GameCommunitySocialController.Instance.GetGameAdjective();
                    
                    BroadcastNetworks.SetMetadata("about",
                                                  word + Locos.Get(LocoKeys.social_everyplay_game_results_message));
                    
                    BroadcastNetworks.SetMetadata("game", Locos.Get(LocoKeys.app_display_name));
                    BroadcastNetworks.SetMetadata("level", Locos.Get(LocoKeys.game_type_arcade));
                    BroadcastNetworks.SetMetadata("level_name", Locos.Get(LocoKeys.game_type_arcade_mode));
                    BroadcastNetworks.SetMetadata("url", Locos.Get(LocoKeys.app_web_url));
                    BroadcastNetworks.SetMetadata("score", runtimeData.score.ToString("N0"));
                    BroadcastNetworks.SetMetadata("scores", runtimeData.scores.ToString("N0"));
                    BroadcastNetworks.SetMetadata("coins", runtimeData.coins.ToString("N0"));
                    BroadcastNetworks.SetMetadata("total_score", runtimeData.totalScoreValue.ToString("N0"));
                    
                    Debug.Log("BroadcastGameLevelFinishData" + " runtimeData.scores:" + runtimeData.scores);
                    
                }
            }
        }
    }

    public void BroadcastGameLevelFinishDelayed(float delay) {
        StartCoroutine(BroadcastGameLevelFinishDelayedCo(delay));
    }

    IEnumerator BroadcastGameLevelFinishDelayedCo(float delay) {
        Debug.Log("Broadcast: BroadcastGameLevelFinishDelayedCo" + " delay:" + delay);

        yield return new WaitForSeconds(delay);
        
        BroadcastGameLevelFinishData(true);
    }
    
    public void UpdateBroadcastStatus(string broadcastStatus) {
                
        if (broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStart) {
            HandleUIBroadcastStart();
        }
        else {
            HandleUIBroadcastStop();
        }
    }

    public void HandleUIBroadcastStart() {

        hideButtonBroadcastOpen();
        hideButtonBroadcastShare();
        hideButtonBroadcastReplay();
        hideButtonBroadcastRecordStart();
        showButtonBroadcastRecordStop();
        hideButtonBroadcastFacecamToggle();
    }
    
    public void HandleUIBroadcastStop() {
        
        showButtonBroadcastOpen();
        showButtonBroadcastShare();
        showButtonBroadcastReplay();
        showButtonBroadcastRecordStart();
        hideButtonBroadcastRecordStop();

        if (isFacecamSupported) {
            showButtonBroadcastFacecamToggle();
        }
    }

    //
    
    public static void ShowButtonBroadcastFacecamToggle() {
        if (isInst) {
            Instance.showButtonBroadcastFacecamToggle();
        }
    }
    
    public void showButtonBroadcastFacecamToggle() {
        buttonBroadcastFacecamToggle.Show();
    }
    
    public static void HideButtonBroadcastFacecamToggle() {
        if (isInst) {
            Instance.hideButtonBroadcastFacecamToggle();
        }
    }
    
    public void hideButtonBroadcastFacecamToggle() {
        buttonBroadcastFacecamToggle.Hide();
    }

    //
    
    public static void ShowButtonBroadcastRecordStart() {
        if (isInst) {
            Instance.showButtonBroadcastRecordStart();
        }
    }
    
    public void showButtonBroadcastRecordStart() {
        buttonBroadcastRecordStart.Show();
    }
    
    public static void HideButtonBroadcastRecordStart() {
        if (isInst) {
            Instance.hideButtonBroadcastRecordStart();
        }
    }
    
    public void hideButtonBroadcastRecordStart() {
        buttonBroadcastRecordStart.Hide();
    }
    
    //
    
    public static void ShowButtonBroadcastRecordStop() {
        if (isInst) {
            Instance.showButtonBroadcastRecordStop();
        }
    }
    
    public void showButtonBroadcastRecordStop() {
        buttonBroadcastRecordStop.Show();
    }
    
    public static void HideButtonBroadcastRecordStop() {
        if (isInst) {
            Instance.hideButtonBroadcastRecordStop();
        }
    }
    
    public void hideButtonBroadcastRecordStop() {
        buttonBroadcastRecordStop.Hide();
    }

    //
    
    public static void ShowButtonBroadcastOpen() {
        if (isInst) {
            Instance.showButtonBroadcastOpen();
        }
    }
    
    public void showButtonBroadcastOpen() {
        buttonBroadcastOpen.Show();
    }
    
    public static void HideButtonBroadcastOpen() {
        if (isInst) {
            Instance.hideButtonBroadcastOpen();
        }
    }
    
    public void hideButtonBroadcastOpen() {
        buttonBroadcastOpen.Hide();
    }

    //
    
    public static void ShowButtonBroadcastReplay() {
        if (isInst) {
            Instance.showButtonBroadcastReplay();
        }
    }
    
    public void showButtonBroadcastReplay() {
        buttonBroadcastReplay.Show();
    }
    
    public static void HideButtonBroadcastReplay() {
        if (isInst) {
            Instance.hideButtonBroadcastReplay();
        }
    }
    
    public void hideButtonBroadcastReplay() {
        buttonBroadcastReplay.Hide();
    }

    //

    public static void ShowButtonBroadcastShare() {
        if (isInst) {
            Instance.showButtonBroadcastShare();
        }
    }

    public void showButtonBroadcastShare() {
        buttonBroadcastShare.Show();
    }
    
    public static void HideButtonBroadcastShare() {
        if (isInst) {
            Instance.hideButtonBroadcastShare();
        }
    }

    public void hideButtonBroadcastShare() {
        buttonBroadcastShare.Hide();
    }

    // STATE

    public void UpdateState() {

        isEnabled = AppConfigs.broadcastNetworksEnabled;
        isSupported = BroadcastNetworks.IsSupported();
        isRecordingSupported = BroadcastNetworks.IsRecordingSupported();
        isFacecamSupported = BroadcastNetworks.IsFacecamVideoRecordingSupported();

        hideBroadcastRecordPlayShare();

        ShowContainerNotSupported();

        if (Application.isEditor) {
            isSupported = true;
            isRecordingSupported = true;
            isFacecamSupported = true;
        }
        
        if (isSupported) {           
           
            if (isRecordingSupported) {
                ShowContainerSupported();
            }
        }

        if (isFacecamSupported) {
            showButtonBroadcastFacecamToggle();
        }
        else {
            hideButtonBroadcastFacecamToggle();
        }

        if (toggleRecordReplaysLevel != null) {
            toggleRecordReplaysLevel.isChecked = GameProfiles.Current.GetBroadcastRecordLevels();
        }
    }

    public void ShowContainerSupported() {
        containerSupported.Show();
        containerNotSupported.Hide();        
    }
    
    public void ShowContainerNotSupported() {
        containerSupported.Hide();
        containerNotSupported.Show();
    }

    // SHOW/LOAD

    public void HidePanels() {
        HideBroadcastRecord();
        HideBroadcastButton();
    }

    //
        
    public static void ShowBroadcastButton() {
        if (isInst) {
            Instance.showBroadcastButton();
        }
    }

    public void showBroadcastButton() { 
        HidePanels();
        AnimateInBottom(panelBroadcastButton);
    }
    
    public static void HideBroadcastButton() {
        if (isInst) {
            Instance.hideBroadcastButton();
        }
    }

    public void hideBroadcastButton() {
        AnimateOutBottom(panelBroadcastButton);
    }    
    
    //
    
    public static void ShowBroadcastRecordPlayShare() {
        if (isInst) {
            Instance.showBroadcastRecordPlayShare();
        }
    }
    
    public void showBroadcastRecordPlayShare() {

        if (!isEnabled) {
            return;
        }
        
        if (!isSupported) {
            return;
        }
        
        if (!isRecordingSupported) {
            return;
        }        
        
        if (!GameProfiles.Current.GetBroadcastRecordLevels()) {
            return;
        }

        AnimateInTop(panelBroadcastRecordPlayShare);
    }
    
    public static void HideBroadcastRecordPlayShare() {
        if (isInst) {
            Instance.hideBroadcastRecordPlayShare();
        }
    }
    
    public void hideBroadcastRecordPlayShare() {
        AnimateOutTop(panelBroadcastRecordPlayShare);
    }
    
    //
    
    public static void ShowBroadcastRecord() {
        if (isInst) {
            Instance.showBroadcastRecord();
        }
    }

    public void showBroadcastRecord() {
        HidePanels();
        AnimateInBottom(panelBroadcastRecord);
        UpdateState();
                
        UIPanelCommunityBackground.ShowBackground();
    }
        
    public static void HideBroadcastRecord() {
        if (isInst) {
            Instance.hideBroadcastRecord();
        }
    }
    
    public void hideBroadcastRecord() {
        AnimateOutBottom(panelBroadcastRecord);
    }

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

    public static void LoadData() {
        if (Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {

    }
    
    public static void ShowButton() {
        if (isInst) {
            Instance.showButton();
        }
    }

    public void showButton() {
        ShowBroadcastButton();
        HideBroadcastRecord();
    }

    public static void ShowDialog() {
        if (isInst) {
            Instance.showDialog();
        }
    }
    
    public override void showDialog() {
        base.showDialog();
        ShowBroadcastRecord();
    }

    public static void HideDialog() {
        if (isInst) {
            Instance.hideDialog();
        }
    }
    
    public override void hideDialog() {
        base.hideDialog();        
        
        ShowBroadcastButton();
        HideBroadcastRecordPlayShare();
    }
        
    public static void ShowNone() {
        if (isInst) {
            Instance.showNone();
        }
    }
    
    public void showNone() {
        HideBroadcastButton();
        HideBroadcastRecord();
    }

    public override void AnimateIn() {
        base.AnimateIn();

        UpdateState();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        hideDialog();
    }

    public override void Update() {
        base.Update();
    }
 
}
