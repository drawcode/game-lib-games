using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelTrophyStatistics : UIAppPanelBaseList {


    public GameObject listItemPrefab;

    public static UIPanelTrophyStatistics Instance;

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();

        loadData();
        LogUtil.Log("Load GameStatistics: Init");
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

        LogUtil.Log("Load GameStatistics: LoadDataCo");

        if(listGridRoot != null) {
            foreach(Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }

            LogUtil.Log("Load GameStatistics: LoadDataCo 3");

            List<GameStatistic> statistics = GameStatistics.Instance.GetAll();

            LogUtil.Log("Load statistics: statistics.Count: " + statistics.Count);

            int i = 0;

            //int totalPoints = 0;

            foreach(GameStatistic statistic in statistics) {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
#else
                GameObject item = GameObjectHelper.CreateGameObject(
                    listItemPrefab, Vector3.zero, Quaternion.identity, false);
                // NGUITools.AddChild(listGridRoot, listItemPrefab);
                item.transform.parent = listGridRoot.transform;
                item.ResetLocalPosition();
#endif

                item.name = "StatisticItem" + i;


                UIUtil.UpdateLabelObject(item.transform, "LabelName", statistic.display_name);
                UIUtil.UpdateLabelObject(item.transform, "LabelDescription", statistic.description);

                double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
                string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);

                UIUtil.UpdateLabelObject(item.transform, "LabelPoints", displayValue);

                i++;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            listGridRoot.GetComponent<UIGrid>().Reposition();
#endif
            yield return new WaitForEndOfFrame();

        }
    }
}