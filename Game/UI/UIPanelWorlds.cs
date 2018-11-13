using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelWorlds : UIAppPanelBaseList {


    public GameObject listItemPrefab;

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

        LogUtil.Log("Load GameWorlds: LoadDataCo");

        if(listGridRoot != null) {
            foreach(Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }

            List<GameWorld> worlds = GameWorlds.Instance.GetAll();

            LogUtil.Log("Load GameWorlds: worlds.Count: " + worlds.Count);

            int i = 0;

            foreach(GameWorld world in worlds) {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
#else
                GameObject item = GameObjectHelper.CreateGameObject(
                    listItemPrefab, Vector3.zero, Quaternion.identity, false);
                // NGUITools.AddChild(listGridRoot, listItemPrefab);
                item.transform.parent = listGridRoot.transform;
                item.ResetLocalPosition();
#endif

                UIUtil.UpdateLabelObject(item.transform, "LabelWorld", world.name);
                UIUtil.UpdateLabelObject(item.transform, "LabelWorldDisplayName", world.display_name);
                UIUtil.UpdateLabelObject(item.transform, "LabelWorldDescription", world.description);

                //GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
                //item.name = "WorldItem" + world.sort_order;
                //item.transform.Find("LabelWorld").GetComponent<UILabel>().text = world.name;
                //item.transform.Find("LabelWorldDisplayName").GetComponent<UILabel>().text = world.display_name;
                //item.transform.Find("LabelWorldDescription").GetComponent<UILabel>().text = world.description;

                i++;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            //yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
#endif
            yield return new WaitForEndOfFrame();

        }
    }
}