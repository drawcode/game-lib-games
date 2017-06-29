using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

#if ENABLE_FEATURE_TRAINING

public class BaseGameUIPanelGameModeTrainingMode : GameUIPanelBase {

    public static GameUIPanelGameModeTrainingMode Instance;

    public GameObject listItemPrefab;

    public UIImageButton buttonGamePlayChoiceQuiz; // quiz
    public UIImageButton buttonGamePlayCollectionSmarts; // concussions,
    public UIImageButton buttonGamePlayCollectionSafety; // equipment

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
        /*
        if(UIUtil.IsButtonClicked(buttonGamePlayChoiceQuiz, buttonName)) {

            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameTrainingChoiceQuiz);
            GameUIController.ShowGameModeTrainingModeChoiceQuiz();
        }
        else if(UIUtil.IsButtonClicked(buttonGamePlayCollectionSmarts, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameTrainingCollectionSmarts);
            GameUIController.ShowGameModeTrainingModeCollectionSmarts();
        }
        else if(UIUtil.IsButtonClicked(buttonGamePlayCollectionSafety, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameTrainingCollectionSafety);
            GameUIController.ShowGameModeTrainingModeCollectionSafety();

        }
        else if(UIUtil.IsButtonClicked(buttonGameEquipmentRoom, buttonName)) {
            GameUIController.ShowEquipment();
        }
        */
    }

    public override void AnimateIn() {
        // base.AnimateIn();

        GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameTrainingChoiceQuiz);
        GameUIController.ShowGameModeTrainingModeChoiceQuiz();
    }

    public static void LoadData() {
        if(GameUIPanelGameModeTrainingMode.Instance != null) {
            GameUIPanelGameModeTrainingMode.Instance.loadData();
        }
    }

    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);
    }
}
#endif