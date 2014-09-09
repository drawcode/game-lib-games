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
    public GameObject buttonBroadcastBroadcastOpen;

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

        ShowBroadcastButton();

        UpdateState();
    }

    public override void Start() {
        Init();
    }

    // EVENTS
 
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnToggleChangedEventHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnToggleChangedEventHandler);
    }

    public void OnButtonClickEventHandler(string buttonName) {

    }

    public void OnToggleChangedEventHandler(string checkboxName, bool selected) {

        Debug.Log("OnToggleChangedEventHandler" + " checkboxName:" + checkboxName + " selected:" + selected.ToString());
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
