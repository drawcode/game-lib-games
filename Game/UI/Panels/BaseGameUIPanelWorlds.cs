using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public enum GameWorldsState {
    selection,
    missions
}

public class GameWorldsMessages {
    public static string gameWorldNext = "game-world-next";
    public static string gameWorldPrevious = "game-world-previous";
    public static string gameWorldSelect = "game-world-select";
}

public class BaseGameUIPanelWorlds : GameUIPanelBase {
    
    public static GameUIPanelWorlds Instance;
    public GameObject listItemPrefab;
    public GameWorldsState gameWorldsState = GameWorldsState.selection;
    public UIImageButton buttonGamePlay;
    public UIImageButton buttonClose;
    public UIImageButton buttonWorldNext;
    public UIImageButton buttonWorldPrevious;
    public UILabel labelWorldTitle;
    public UILabel labelWorldDescription;
    public GameObject containerMissions;
    public GameObject containerButtons;
    public GameObject containerButtonNext;
    public GameObject containerButtonPrevious;
    
    public static bool isInst {
        get {
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {
        
    }
    
    public override void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();    
        
        loadData();

        ChangeState(GameWorldsState.selection);
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
        
        Messenger.AddListener(GameWorldsMessages.gameWorldNext, OnGameWorldNext);
        Messenger.AddListener(GameWorldsMessages.gameWorldPrevious, OnGameWorldPrevious);
        Messenger.AddListener(GameWorldsMessages.gameWorldSelect, OnGameWorldSelect);
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
        
        Messenger.RemoveListener(GameWorldsMessages.gameWorldNext, OnGameWorldNext);
        Messenger.RemoveListener(GameWorldsMessages.gameWorldPrevious, OnGameWorldPrevious);
        Messenger.RemoveListener(GameWorldsMessages.gameWorldSelect, OnGameWorldSelect);

    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if (className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if (className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if (className == classNameTo) {
            //
        }
    }

    public virtual void OnGameWorldNext() {
        ChangeState(GameWorldsState.selection);
    }
    
    public virtual void OnGameWorldPrevious() {
        ChangeState(GameWorldsState.selection);
    }
    
    public virtual void OnGameWorldSelect() {
        ChangeState(GameWorldsState.missions); 

        UpdateMeta();
    }

    public virtual void UpdateMeta() {
        UIUtil.SetLabelValue(labelWorldTitle, GameWorlds.Current.display_name);
        UIUtil.SetLabelValue(labelWorldDescription, GameWorlds.Current.description);
    }

    public virtual void ChangeState(GameWorldsState stateTo) {
        gameWorldsState = stateTo;
        HandleStateChange();
        UpdateMeta();
    }

    public virtual void HandleStateChange() {
        if (gameWorldsState == GameWorldsState.selection) {
            HideSelect();
            ShowButtons();
        }
        else if (gameWorldsState == GameWorldsState.missions) {
            ShowSelect();
            HideButtons();
        }
    }

    public virtual void ShowSelect() {
        ShowPanelBottom(containerMissions);
    }

    public virtual void HideSelect() {        
        HidePanelBottom(containerMissions);
    }
    
    public virtual void ShowButtons() {
        ShowPanelBottom(containerButtons);
    }
    
    public virtual void HideButtons() {        
        HidePanelBottom(containerButtons);
    }
    
    public override void OnButtonClickEventHandler(string buttonName) {     
        if (UIUtil.IsButtonClicked(buttonWorldNext, buttonName)) {            
            Messenger.Broadcast(GameWorldsMessages.gameWorldNext);
        }
        else if (UIUtil.IsButtonClicked(buttonWorldPrevious, buttonName)) {            
            Messenger.Broadcast(GameWorldsMessages.gameWorldPrevious);
        }
        else if (UIUtil.IsButtonClicked(buttonGamePlay, buttonName)) {            
            Messenger.Broadcast(GameWorldsMessages.gameWorldSelect);
        }   
    }
        
    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {      
        
        LogUtil.Log("LoadDataCo");
        
        if (listGridRoot != null) {
            listGridRoot.DestroyChildren();
            
            yield return new WaitForEndOfFrame();
            
            loadDataMissions();
            
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();               
        }
    }
    
    public virtual void loadDataMissions() {

        LogUtil.Log("Load Missions:");
        
        int i = 0;
        
        //int totalPoints = 0;


        foreach (AppContentCollect mission in 
                AppContentCollects.GetMissionsByWorld(GameWorlds.Current.code)) {
            
            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
            item.name = "MissionItem" + i;
            
            UIUtil.UpdateLabelObject(item, "Container/Meta/LabelName", mission.display_name);
            UIUtil.UpdateLabelObject(item, "Container/Meta/LabelDescription", mission.description);
            
            // Update button action
                        
            Transform buttonObject = item.transform.FindChild("Container/Button/ButtonAction");
            if(buttonObject != null) {
                UIImageButton button = buttonObject.gameObject.GetComponent<UIImageButton>();
                if(button != null) {
                    
                    string actionType = "mission";
                    string appContentState = AppContentStates.Current.code;
                    string appState = AppStates.Current.code;
                    string missionCode = mission.code;

                    GameObjectData objData = button.gameObject.Get<GameObjectData>();

                    if(objData == null) {
                        objData = button.gameObject.AddComponent<GameObjectData>();
                    }

                    if(objData != null) {
                        objData.Set(BaseDataObjectKeys.type, actionType);
                        objData.Set(BaseDataObjectKeys.app_content_state, appContentState);
                        objData.Set(BaseDataObjectKeys.app_state, appState);
                        objData.Set(BaseDataObjectKeys.code, missionCode);
                    }

                    button.name = BaseUIButtonNames.buttonGamePlay + 
                        "$" + appContentState + "$" + missionCode;
                }
            }

            string currentType = "action";
            
            foreach (GameObjectInactive obj in item.GetComponentsInChildren<GameObjectInactive>(true)) {
                if (obj.type == currentType) {
                    obj.gameObject.Hide();
                }
            }
            
            int j = 0;

            foreach (AppContentCollectItem action in mission.GetItemsData()) {
                            
                foreach (GameObjectInactive obj in item.GetComponentsInChildren<GameObjectInactive>(true)) {

                    if (obj.type == currentType) {

                        string currentActionItem = currentType + "-" + (j + 1).ToString();

                        if (obj.code == currentActionItem) {
                            
                            Debug.Log("action.data.display_name:" + action.data.display_name);

                            obj.gameObject.Show();
                                                        
                            UIUtil.UpdateLabelObject(
                                obj.gameObject, "LabelDescription", action.data.display_name);
                        }
                    }
                }                
                
                j++;
            
                //GameObject iconObject = item.transform.FindChild("Container/Icon").gameObject;  
                //UISprite iconSprite = iconObject.GetComponent<UISprite>();            
            
                //bool completed = false;
            
                //bool hasValue = GameProfileAchievements.Current.CheckIfAttributeExists(achievement.code);
            
                //if(hasValue) {
                //completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code);
                //}
            
                //if(!hasValue) {
                //completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code + "_" + achievement.pack_code);
                //}
            
                /*
            string points = "";
            
            if(completed) {
                int currentPoints = achievement.points;
                totalPoints += currentPoints;               
                
                if(GameConfigs.useCoinRewardsForAchievements) {
                    currentPoints *= (int)GameConfigs.coinRewardAchievementPoint;  
                }
                
                points = "+" + currentPoints.ToString();
                
                if(iconSprite != null) {
                    iconSprite.alpha = 1f;
                }
                //item.transform.FindChild("Container/ContainerComplete").gameObject.Show(); 
            }   
            else {
                if(iconSprite != null) {
                    iconSprite.alpha = .33f;
                }
                //item.transform.FindChild("Container/ContainerComplete").gameObject.Hide(); 
            }
            */
            
                //item.transform.FindChild("Container/LabelPoints").GetComponent<UILabel>().text = points;                
            
                // Get trophy icon
            
                i++;
            }
        }
        
        //if(labelPoints != null) {
        //  labelPoints.text = totalPoints.ToString("N0");
        //}
    }
    
    public virtual void ClearList() {
        if (listGridRoot != null) {
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
