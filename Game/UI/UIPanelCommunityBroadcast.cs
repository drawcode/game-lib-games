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
    }

    public override void Start() {
        Init();
    }

    // EVENTS
 
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    public void OnButtonClickEventHandler(string buttonName) {

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
    }

    public override void AnimateOut() {
        base.AnimateOut();

        ShowBroadcastButton();
    }

    public void Update() {
        //base.Update();
    }
 
}
