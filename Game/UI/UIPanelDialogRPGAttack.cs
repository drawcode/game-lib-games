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

public class UIPanelDialogRPGAttack : UIPanelBase {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelTitle;
    public UIImageButton buttonBuyRecharge;
    public UIImageButton buttonEarn;
#else
    public Text labelTitle;
    public Button buttonBuyRecharge;
    public Button buttonEarn;
#endif

    public static UIPanelDialogRPGAttack Instance;
    public GameObject containerContent;

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;

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

    public override void OnButtonClickEventHandler(string buttonName) {

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