using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIPanelLevels : UIAppPanelBaseList {


    public GameObject listItemPrefab;
    public GameObject listItemSetPrefab;

    public override void Awake() {
        base.Awake();

    }

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();

        LoadData();
    }

    public void LoadData() {
        StartCoroutine(LoadDataCo());
    }

    IEnumerator LoadDataCo() {

        LogUtil.Log("Load GameLevels: LoadDataCo");


        LogUtil.Log("Load GameLevels: LoadDataCo 2");

        if(listGridRoot != null) {
            foreach(Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }

            LogUtil.Log("Load GameLevels: LoadDataCo 3");

            List<GameLevel> levels = GameLevels.Instance.GetAll();//GetByWorldId("world-original");

            LogUtil.Log("Load GameLevels: levels.Count: " + levels.Count);

            double panelCount = Math.Floor((double)(levels.Count / 20));

            LogUtil.Log("Load GameLevels: panelCount: " + panelCount);

            for(int k = 0; k < (int)panelCount; k++) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject itemSet = NGUITools.AddChild(listGridRoot, listItemSetPrefab);
#else
                GameObject itemSet = GameObjectHelper.CreateGameObject(
                    listItemSetPrefab, Vector3.zero, Quaternion.identity, false);
                // NGUITools.AddChild(listGridRoot, listItemPrefab);
                itemSet.transform.parent = listGridRoot.transform;
                itemSet.ResetLocalPosition();
#endif

                itemSet.name = "LevelSet" + k;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                UIGrid listSetGrid = itemSet.transform.Find("LevelContainer").GetComponent<UIGrid>();
#else
                Grid listSetGrid = itemSet.transform.Find("LevelContainer").GetComponent<Grid>();
#endif

                GameObject listSetGridObject = listSetGrid.gameObject;

                for(int y = (k * 20); y < ((20) * (k + 1)); y++) {

                    //GameObject item = NGUITools.AddChild(listSetGridObject, listItemPrefab);
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listSetGridObject, listItemPrefab);
#else
                    GameObject item = GameObjectHelper.CreateGameObject(
                        listItemPrefab, Vector3.zero, Quaternion.identity, false);
                    // NGUITools.AddChild(listGridRoot, listItemPrefab);
                    item.transform.parent = listSetGridObject.transform;
                    item.ResetLocalPosition();
#endif
                    item.name = "LevelItem" + y;

                    UIUtil.UpdateLabelObject(item.transform, "LabelWorld", (y + 1).ToString());

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

                    item.transform.Find("ButtonPlayLevel").GetComponent<UIImageButton>().name
                     = "ButtonPlayLevel$" + (k + 1).ToString() + "-" + (y + 1).ToString(); ///levels[y].name;
#else

                    item.transform.Find("ButtonPlayLevel").GetComponent<Button>().name
                     = "ButtonPlayLevel$" + (k + 1).ToString() + "-" + (y + 1).ToString(); ///levels[y].name;
#endif

                    // TODO find stars/skulls
                    // TODO find play
                    // TODO find lock

                }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                listSetGrid.Reposition();
#else

#endif
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            foreach(UIGrid grid in UnityObjectUtil.FindObjects<UIGrid>()) {
                //yield return new WaitForEndOfFrame();
                grid.Reposition();
                //yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
#endif
            yield return new WaitForEndOfFrame();

        }
    }
}