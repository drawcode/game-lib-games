using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

#if ENABLE_FEATURE_TRAINING

public class BaseGameUIPanelGameModeTrainingModeCollectionSafety : GameUIPanelBase {

    public static GameUIPanelGameModeTrainingModeCollectionSafety Instance;

    public GameObject listItemPrefab;

    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();

        loadData();
    }

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
            //
        }
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        //
    }

    public static void LoadData() {
        if(GameUIPanelGameModeTrainingModeCollectionSafety.Instance != null) {
            GameUIPanelGameModeTrainingModeCollectionSafety.Instance.loadData();
        }
    }

    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);
    }

    public virtual void ClearList() {
        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();
        }
    }

    public override void AnimateIn() {

        base.AnimateIn();

        loadData();
    }

    public override void AnimateOut() {

        base.AnimateOut();

        ClearList();
    }
}
#endif
