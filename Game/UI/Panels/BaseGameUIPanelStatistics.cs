using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelStatistics : GameUIPanelBase {

    public static GameUIPanelStatistics Instance;

    public GameObject listItemStatisticPrefab;

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
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
    }

    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        LogUtil.Log("LoadDataCo");

        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();

            yield return new WaitForEndOfFrame();

            loadDataStatistics();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
#endif
            yield return new WaitForEndOfFrame();
        }
    }


    public virtual void loadDataStatistics() {

        LogUtil.Log("Load Statistics:");

        List<GameStatistic> statistics = GameStatistics.Instance.GetAll();

        LogUtil.Log("Load statistics: statistics.Count: " + statistics.Count);

        int i = 0;

        foreach(GameStatistic statistic in statistics) {

            double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
            string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);

            //if(statValue > .1) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listGridRoot, listItemStatisticPrefab);
#else
            GameObject item = GameObjectHelper.CreateGameObject(
                listItemStatisticPrefab, Vector3.zero, Quaternion.identity, false);
            // NGUITools.AddChild(listGridRoot, listItemPrefab);
            item.transform.parent = listGridRoot.transform;
            item.ResetLocalPosition();
#endif

            item.name = "StatisticItem" + i;

            UIUtil.UpdateLabelObject(item.transform, "Container/LabelName", statistic.display_name);
            UIUtil.UpdateLabelObject(item.transform, "Container/LabelDescription", statistic.description);

            UIUtil.UpdateLabelObject(item.transform, "Container/LabelPoints", displayValue);

            i++;
            //}
        }
    }

    public virtual void ClearList() {
        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();
        }
    }

    public override void HandleShow() {
        base.HandleShow();

        buttonDisplayState = UIPanelButtonsDisplayState.GameNetworks;
        characterDisplayState = UIPanelCharacterDisplayState.Character;
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
        adDisplayState = UIPanelAdDisplayState.BannerBottom;
    }

    public override void AnimateIn() {

        base.AnimateIn();

        UIPanelCommunityBroadcast.HideBroadcastRecordPlayShare();

        loadData();
    }

    public override void AnimateOut() {

        base.AnimateOut();

        ClearList();
    }
}