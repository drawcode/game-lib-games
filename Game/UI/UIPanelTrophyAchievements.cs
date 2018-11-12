using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIPanelTrophyAchievements : UIAppPanelBaseList {

    public GameObject listItemPrefab;

    public static UIPanelTrophyAchievements Instance;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelPoints;
#else
    public GameObject labelPoints;
#endif

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

        yield return new WaitForSeconds(.5f);

        if(listGridRoot != null) {
            foreach(Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }

            List<GameAchievement> achievements = GameAchievements.Instance.GetAll();

            LogUtil.Log("Load Achievements: achievements.Count: " + achievements.Count);

            int i = 0;

            double totalPoints = 0;

            foreach(GameAchievement achievement in achievements) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
#else
                GameObject item = GameObjectHelper.CreateGameObject(listItemPrefab, Vector3.zero, Quaternion.identity, false);// NGUITools.AddChild(listGridRoot, listItemPrefab);
                item.transform.parent = listGridRoot.transform;
                item.ResetLocalPosition();
#endif

                Transform labelItemName = item.transform.Find("LabelName");
                Transform labelItemDescription = item.transform.Find("LabelDescription");
                Transform labelItemPoints = item.transform.Find("LabelPoints");

                if(labelItemName != null) {
                    UIUtil.SetLabelValue(labelItemName.gameObject, achievement.display_name);
                }

                if(labelItemDescription != null) {
                    UIUtil.SetLabelValue(labelItemDescription.gameObject, achievement.description);
                }

                GameObject iconObject = item.transform.Find("Icon").gameObject;


#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

                UISprite iconSprite = iconObject.GetComponent<UISprite>();
#else
                GameObject iconSprite = null;

                if(iconObject.Has<Image>()) {
                    iconSprite = iconObject.GetComponent<Image>().gameObject;
                }
#endif

                bool completed = GameProfiles.Current.CheckIfAttributeExists(achievement.code);

                if(completed) {
                    completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code);
                }

                if(!completed) {
                    completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code + "_" + achievement.pack_code);
                }

                string points = "";

                if(completed) {
                    double currentPoints = achievement.data.points;
                    totalPoints += currentPoints;
                    points = "+" + currentPoints.ToString();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                    if(iconSprite != null) {                                                
                        iconSprite.alpha = 1f;
                    }
#endif
                }
                else {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                    if(iconSprite != null) {
                        iconSprite.alpha = .33f;
                    }
#endif
                }

                if(labelItemPoints != null) {
                    UIUtil.SetLabelValue(labelItemPoints.gameObject, points);
                }

                // Get trophy icon

                i++;
            }

            if(labelPoints != null) {
                UIUtil.SetLabelValue(labelPoints.gameObject, totalPoints.ToString("N0"));
            }

            yield return new WaitForEndOfFrame();
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();
#endif

        }
    }
}