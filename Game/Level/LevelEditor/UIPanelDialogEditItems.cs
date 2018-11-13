using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIPanelDialogEditItemsFilter {
	public static string all = "all";
	public static string levelAssets = "level-assets";
	public static string levelEnvironment = "level-environments";
	public static string levelEffect = "level-effects";
}

public class UIPanelDialogEditItems : UIAppPanelBaseList {

    public GameObject listItemPrefab;

    public static UIPanelDialogEditItems Instance;

    public string filterType = UIPanelDialogEditItemsFilter.all;

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
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

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();

        LoadData();
    }

    public void LoadData(string levelAssetKey) {
        filterType = levelAssetKey;
        LoadData();
    }

    public void LoadData() {
        StartCoroutine(LoadDataCo());
    }

    IEnumerator LoadDataCo() {

        yield return new WaitForSeconds(.1f);

        if(listGridRoot != null) {
            foreach(Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }

            List<AppContentAsset> assets = AppContentAssets.Instance.GetAll();

            LogUtil.Log("Load AppContentAsset: assets.Count: " + assets.Count);

            int i = 0;

            //int totalPoints = 0;

            foreach(AppContentAsset asset in assets) {

                if(filterType != UIPanelDialogEditItemsFilter.all) {
                    if(asset.key != filterType) {
                        continue;
                    }
                }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
#else
                GameObject item = GameObjectHelper.CreateGameObject(
                    listItemPrefab, Vector3.zero, Quaternion.identity, false);
                // NGUITools.AddChild(listGridRoot, listItemPrefab);

                item.transform.parent = listGridRoot.transform;
                item.ResetLocalPosition();
#endif

                //GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
                item.name = "AssetItem" + i;

                Transform labelItemName = item.transform.Find("LabelName");

                if(labelItemName != null) {
                    UIUtil.SetLabelValue(labelItemName.gameObject, asset.display_name);
                }

                GameObject gameLevelItemObject = 
                    item.transform.Find("GameLevelItemObject").gameObject;

                // clear current items

                foreach(Transform t in gameLevelItemObject.transform) {
                    Destroy(t.gameObject);
                }

                //LogUtil.Log("Load AppContentAsset: gameLevelItemObject.transform: " + gameLevelItemObject.transform.childCount);

                if(GameDraggableEditor.Instance == null) {
                    yield break;
                }
                //
                //LogUtil.Log("Load AppContentAsset: GameDraggableEditor: " + true);

                string assetCode = asset.code;
                if(assetCode.Contains("portal-")) {
                    assetCode = assetCode + "-sm";
                }

                GameObject go = GameDraggableEditor.LoadSpriteUI(
                    gameLevelItemObject, assetCode, Vector3.one);

                gameLevelItemObject.ChangeLayersRecursively("UIEditor");

                //LogUtil.Log("Load AppContentAsset: go: " + go);

                float maxSize = .8f;

                if(go != null) {
                    /*PackedSprite sprite = go.GetComponent<PackedSprite>();
					if(sprite != null) {
						
						float adjust = 1;
						
						if(sprite.height > sprite.width) {
							if(sprite.height > maxSize) {
								adjust = maxSize/sprite.height;
							}
						}
						else {
							if(sprite.width > maxSize) {
								adjust = maxSize/sprite.width;
							}
						}
						
						go.transform.localScale = go.transform.localScale.WithX(adjust).WithY(adjust).WithZ(adjust);
					}
					else {
						*/
                    float adjust = 1;

                    Collider col = go.GetComponent<Collider>();
                    if(col != null) {
                        Bounds bounds = col.bounds;

                        if(bounds.size.x > bounds.size.y) {
                            if(bounds.size.y > maxSize) {
                                adjust = maxSize / bounds.size.x;
                            }
                        }
                        else {
                            if(bounds.size.x > maxSize) {
                                adjust = maxSize / bounds.size.y;
                            }
                        }
                    }
                    adjust = adjust / 2;

                    go.transform.localScale = 
                        go.transform.localScale.WithX(adjust).WithY(adjust).WithZ(adjust);
                    //}

                }

                Transform buttonGameLevelItemObject = item.transform.Find("ButtonGameLevelItemObject");

                if(buttonGameLevelItemObject != null) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                    buttonGameLevelItemObject.GetComponent<UIButton>().name
                            = "ButtonGameLevelItemObject$" + asset.code; ///levels[y].name;
#else
                    if(buttonGameLevelItemObject.gameObject.Has<Button>()) {
                        buttonGameLevelItemObject.GetComponent<Button>().name
                                = "ButtonGameLevelItemObject$" + asset.code; ///levels[y].name;
                    }
#endif
                }

                if(filterType == UIPanelDialogEditItemsFilter.all) {

                }

                i++;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
#endif
            yield return new WaitForEndOfFrame();

        }
    }
}