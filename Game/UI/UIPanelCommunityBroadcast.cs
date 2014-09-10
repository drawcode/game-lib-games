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

    public GameObject buttonBroadcastReplay;
    public GameObject buttonBroadcastShare;
    public GameObject buttonBroadcastRecordStart;
    public GameObject buttonBroadcastRecordStop;

    public GameObject buttonBroadcastFacecamToggle;
    public GameObject buttonBroadcastOpen;

    public GameObject containerSupported;
    public GameObject containerNotSupported;

    public UICheckbox toggleRecordReplaysLevel;

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
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnToggleChangedEventHandler);

        Messenger<string>.RemoveListener(
            BroadcastNetworksMessages.broadcastRecordingStatusChanged, 
            OnBroadcastRecordStatusChanged);
    }


    public void OnButtonClickEventHandler(string buttonName) {

    }

    public void OnToggleChangedEventHandler(string checkboxName, bool selected) {

        Debug.Log("OnToggleChangedEventHandler" + " checkboxName:" + checkboxName + " selected:" + selected.ToString());
    }    
    
    public void OnBroadcastRecordStatusChanged(string broadcastStatus) {
        
        UpdateBroadcastStatus(broadcastStatus);
    }
    
    public void UpdateBroadcastStatus(string broadcastStatus) {
                
        if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStart) {
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
    }
    
    public void HandleUIBroadcastStop() {
        
        showButtonBroadcastOpen();
        showButtonBroadcastShare();
        showButtonBroadcastReplay();
        showButtonBroadcastRecordStart();
        hideButtonBroadcastRecordStop();

    }
    
    //
    
    public static void ShowButtonBroadcastRecordStart() {
        if(isInst) {
            Instance.showButtonBroadcastRecordStart();
        }
    }
    
    public void showButtonBroadcastRecordStart() {
        buttonBroadcastRecordStart.Show();
    }
    
    public static void HideButtonBroadcastRecordStart() {
        if(isInst) {
            Instance.hideButtonBroadcastRecordStart();
        }
    }
    
    public void hideButtonBroadcastRecordStart() {
        buttonBroadcastRecordStart.Hide();
    }
    
    //
    
    public static void ShowButtonBroadcastRecordStop() {
        if(isInst) {
            Instance.showButtonBroadcastRecordStop();
        }
    }
    
    public void showButtonBroadcastRecordStop() {
        buttonBroadcastRecordStop.Show();
    }
    
    public static void HideButtonBroadcastRecordStop() {
        if(isInst) {
            Instance.hideButtonBroadcastRecordStop();
        }
    }
    
    public void hideButtonBroadcastRecordStop() {
        buttonBroadcastRecordStop.Hide();
    }

    //
    
    public static void ShowButtonBroadcastOpen() {
        if(isInst) {
            Instance.showButtonBroadcastOpen();
        }
    }
    
    public void showButtonBroadcastOpen() {
        buttonBroadcastOpen.Show();
    }
    
    public static void HideButtonBroadcastOpen() {
        if(isInst) {
            Instance.hideButtonBroadcastOpen();
        }
    }
    
    public void hideButtonBroadcastOpen() {
        buttonBroadcastOpen.Hide();
    }

    //
    
    public static void ShowButtonBroadcastReplay() {
        if(isInst) {
            Instance.showButtonBroadcastReplay();
        }
    }
    
    public void showButtonBroadcastReplay() {
        buttonBroadcastReplay.Show();
    }
    
    public static void HideButtonBroadcastReplay() {
        if(isInst) {
            Instance.hideButtonBroadcastReplay();
        }
    }
    
    public void hideButtonBroadcastReplay() {
        buttonBroadcastReplay.Hide();
    }

    //

    public static void ShowButtonBroadcastShare() {
        if(isInst) {
            Instance.showButtonBroadcastShare();
        }
    }

    public void showButtonBroadcastShare() {
        buttonBroadcastShare.Show();
    }
    
    public static void HideButtonBroadcastShare() {
        if(isInst) {
            Instance.hideButtonBroadcastShare();
        }
    }

    public void hideButtonBroadcastShare() {
        buttonBroadcastShare.Hide();
    }

    // STATE

    public void UpdateState() {

        bool isSupported = BroadcastNetworks.IsSupported();
        bool isRecordingSupported = BroadcastNetworks.IsRecordingSupported();
        bool isFacecamSupported = BroadcastNetworks.IsFacecamVideoRecordingSupported();

        ShowContainerNotSupported();

        if(Application.isEditor) {
            isSupported = true;
            isRecordingSupported = true;
            isFacecamSupported = true;
        }
        
        if(isSupported) {           
           
            if(isRecordingSupported) {
                ShowContainerSupported();
            }
        }

        if(isFacecamSupported) {
            ShowButtonFacecam();
        }
        else {
            HideButtonFacecam();
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

    public void ShowButtonFacecam() {
        buttonBroadcastFacecamToggle.Show();
    }
        
    public void HideButtonFacecam() {
        buttonBroadcastFacecamToggle.Hide();
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
    
    public void showDialog() {
        ShowBroadcastRecord();
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

        ShowBroadcastButton();
    }

    public void Update() {
        //base.Update();
    }
 
}
