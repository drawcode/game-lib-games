#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelDialogRPGAttack : UIPanelBase {

    public static UIPanelDialogRPGAttack Instance;

    public GameObject containerContent;

    public UILabel labelTitle;

    public UIImageButton buttonBuyRecharge;
    public UIImageButton buttonEarn;

    public void Awake() {

        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;
        
        panelType = UIPanelBaseTypes.typeDialogHUD;
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
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    public void BroadcastAppContentChoiceItem(AppContentChoiceItem choiceItem) {
        //if(choiceItem == null) {
        //    return;
        //}

        //Messenger<AppContentChoiceItem>.Broadcast(AppContentChoiceMessages.appContentChoiceItem, choiceItem);
    }


    void OnButtonClickEventHandler(string buttonName) {
        if(UIUtil.IsButtonClicked(buttonBuyRecharge, buttonName)) {

            // buy recharge
            Debug.Log("Recharge:");
        }
        else if(UIUtil.IsButtonClicked(buttonEarn, buttonName)) {
            GameUIController.ShowGameModeTrainingModeChoiceQuiz();
        }
    }

    public void ShowCurrentState() {

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

    }

    public static void LoadData() {
        if(Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {

    }

    public override void AnimateIn() {
        base.AnimateIn();

    }

    public override void AnimateOut() {
        base.AnimateOut();

    }

    public void Update() {

    }
 
}
