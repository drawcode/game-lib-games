#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class UIPanelDialogDisplay : UIPanelBase {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelTitle;
    public UILabel labelDescription;
    public UIImageButton buttonDialogOk;
    public UIImageButton buttonDialogCancel;
    public UIImageButton buttonDialogGo;
    public UIImageButton buttonDialogNext;
#else
    public Text labelTitle;
    public Text labelDescription;
    public Button buttonDialogOk;
    public Button buttonDialogCancel;
    public Button buttonDialogGo;
    public Button buttonDialogNext;
#endif

    public static UIPanelDialogDisplay Instance;

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
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

        Reset();

        loadData();
    }

    public override void Start() {
        Init();
    }

    public void Reset() {

        SetTitle("");
        SetDescription("");

        HideAllButtons();
    }

    public override void OnEnable() {
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

    }

    public override void OnDisable() {
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

    }

    public override void OnButtonClickEventHandler(string buttonName) {
        if(UIUtil.IsButtonClicked(buttonDialogOk, buttonName)) {
            HideAll();
            GameController.GameRunningStateRun();
        }
        else if(UIUtil.IsButtonClicked(buttonDialogGo, buttonName)) {
            HideAll();
            GameController.GameRunningStateRun();
        }
        else if(UIUtil.IsButtonClicked(buttonDialogCancel, buttonName)) {
            HideAll();
            GameController.GameRunningStateRun();
        }
    }

    public static void ShowDefault() {
        if(isInst) {
            Instance.AnimateIn();
            Instance.loadData();
        }
    }

    public static void HideAll() {
        if(isInst) {
            Instance.AnimateOut();
        }
    }

    public static void ShowButtonOk() {
        if(isInst) {
            Instance.showButtonOk();
        }
    }

    public static void ShowButtonCancel() {
        if(isInst) {
            Instance.showButtonCancel();
        }
    }

    public static void ShowButtonGo() {
        if(isInst) {
            Instance.showButtonGo();
        }
    }

    public static void HideButtonOk() {
        if(isInst) {
            Instance.hideButtonOk();
        }
    }

    public static void HideButtonCancel() {
        if(isInst) {
            Instance.hideButtonCancel();
        }
    }

    public static void HideButtonGo() {
        if(isInst) {
            Instance.hideButtonGo();
        }
    }

    public static void HideButtonNext() {
        if(isInst) {
            Instance.hideButtonNext();
        }
    }

    public static void HideAllButtons() {
        if(isInst) {
            Instance.hideAllButtons();
        }
    }

    public void showButtonOk() {
        UIUtil.ShowButton(buttonDialogOk);
    }

    public void showButtonCancel() {
        UIUtil.ShowButton(buttonDialogCancel);
    }

    public void showButtonGo() {
        UIUtil.ShowButton(buttonDialogGo);
    }

    public void showButtonNext() {
        UIUtil.ShowButton(buttonDialogNext);
    }

    public void hideButtonOk() {
        UIUtil.HideButton(buttonDialogOk);
    }

    public void hideButtonCancel() {
        UIUtil.HideButton(buttonDialogCancel);
    }

    public void hideButtonGo() {
        UIUtil.HideButton(buttonDialogGo);
    }

    public void hideButtonNext() {
        UIUtil.HideButton(buttonDialogNext);
    }

    public void hideAllButtons() {
        HideButtonOk();
        HideButtonCancel();
        HideButtonGo();
        HideButtonCancel();
    }

    public static void SetTitle(string titleTo) {
        if(isInst) {
            Instance.setTitle(titleTo);
        }
    }

    public void setTitle(string titleTo) {
        UIUtil.SetLabelValue(labelTitle, titleTo);
    }

    public static void SetDescription(string descriptionTo) {
        if(isInst) {
            Instance.setDescription(descriptionTo);
        }
    }

    public void setDescription(string descriptionTo) {
        UIUtil.SetLabelValue(labelDescription, descriptionTo);
    }

    public static void LoadData() {
        if(Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);

        HideAllButtons();
    }
}