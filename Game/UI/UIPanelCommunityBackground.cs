#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityBackground : UIPanelBase {

    public static UIPanelCommunityBackground Instance;

    public GameObject panelBackground;

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

    public override void OnButtonClickEventHandler(string buttonName) {

    }
    
    //
    
    public static void ShowBackground() {
        if (isInst) {
            Instance.showBackground();
        }
    }
    
    public void showBackground() { 
        AnimateInBottom(panelBackground);

        //Debug.Log("ShowBackground:");
    }
    
    public static void HideBackground() {
        if (isInst) {
            Instance.hideBackground();
        }
    }
    
    public void hideBackground() {
        AnimateOutBottom(panelBackground);
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

    public static void ShowDialog() {
        if (isInst) {
            Instance.showDialog();
        }
    }
    
    public void showDialog() {
        ShowBackground();
    }
        
    public static void ShowNone() {
        if (isInst) {
            Instance.showNone();
        }
    }
    
    public void showNone() {
        HideBackground();
    }

    public override void AnimateIn() {
        base.AnimateIn();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        ShowNone();
    }

    //public void Update() {
        //base.Update();
    //}
 
}
