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
    //
    public GameObject listItemPrefab;
    //
    public GameWorldsState gameWorldsState = GameWorldsState.selection;
    //
    public UIImageButton buttonGamePlay;
    public UIImageButton buttonClose;
    public UIImageButton buttonWorldNext;
    public UIImageButton buttonWorldPrevious;
    //
    public UILabel labelWorldTitle;
    public UILabel labelWorldDescription;
    //
    public GameObject containerMissions;
    public GameObject containerButtons;
    public GameObject containerButtonNext;
    public GameObject containerButtonPrevious;
    //
    public GameObject containerWorlds;

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
        
        Messenger.AddListener(UIControllerMessages.uiShow, OnUIControllerShowHandler);
        Messenger.AddListener(UIControllerMessages.uiHide, OnUIControllerHideHandler);
        
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
        
        Messenger.RemoveListener(UIControllerMessages.uiShow, OnUIControllerShowHandler);
        Messenger.RemoveListener(UIControllerMessages.uiHide, OnUIControllerHideHandler);
        
        Messenger.RemoveListener(GameWorldsMessages.gameWorldNext, OnGameWorldNext);
        Messenger.RemoveListener(GameWorldsMessages.gameWorldPrevious, OnGameWorldPrevious);
        Messenger.RemoveListener(GameWorldsMessages.gameWorldSelect, OnGameWorldSelect);
    }
    
    public void OnUIControllerShowHandler() {
        if (GameUIPanelSettings.isInst) {
            GameUIPanelWorlds.Instance.ShowWorldsContainer();
        }
    }
    
    public void OnUIControllerHideHandler() {
        if (GameUIPanelSettings.isInst) {
            GameUIPanelWorlds.Instance.HideWorldsContainer();
        }
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

    public virtual void ShowWorldsContainer() {
        containerWorlds.Show();
        ShowPanelBottom(containerWorlds);    
    }
    
    public virtual void HideWorldsContainer() {  
        HidePanelBottom(containerWorlds);
        containerWorlds.HideObjectDelayed(.5f);
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
        
        double scoreTotal = 0;

        foreach (AppContentCollect mission in 
                AppContentCollects.GetMissionsByWorld(GameWorlds.Current.code)) {

            double scoreMission = 0;
            
            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
            item.name = "MissionItem" + i;
            
            UIUtil.UpdateLabelObject(item, "Container/Meta/LabelName", mission.display_name);
            UIUtil.UpdateLabelObject(item, "Container/Meta/LabelDescription", mission.description);
            
            // Update button action
                        
            Transform buttonObject = item.transform.FindChild("Container/Button/ButtonAction");
            if (buttonObject != null) {
                UIImageButton button = buttonObject.gameObject.GetComponent<UIImageButton>();
                if (button != null) {
                    
                    string actionType = "mission";
                    string appContentState = AppContentStates.Current.code;
                    string appState = AppStates.Current.code;
                    string missionCode = mission.code;

                    GameObjectData objData = button.gameObject.Get<GameObjectData>();

                    if (objData == null) {
                        objData = button.gameObject.AddComponent<GameObjectData>();
                    }

                    if (objData != null) {
                        objData.Set(BaseDataObjectKeys.type, actionType);
                        objData.Set(BaseDataObjectKeys.app_content_state, appContentState);
                        objData.Set(BaseDataObjectKeys.app_state, appState);
                        objData.Set(BaseDataObjectKeys.code, missionCode);
                    }

                    button.name = BaseUIButtonNames.buttonGamePlay + 
                        "$" + appContentState + "$" + missionCode;
                }
            }

            // hide actions

            string currentType = "action";
            
            foreach (GameObjectInactive obj in 
                     item.GetComponentsInChildren<GameObjectInactive>(true)) {
                if (obj.type == currentType) {
                    obj.gameObject.Hide();
                }
            }

            // fill in action content and stars
            
            int j = 0;

            foreach (AppContentCollectItem action in mission.GetItemsData()) {
                            
                foreach (GameObjectInactive obj in 
                         item.GetComponentsInChildren<GameObjectInactive>(true)) {

                    if (obj.type == currentType) {

                        string index = (j + 1).ToString();

                        string currentActionItem = currentType + "-" + index;

                        if (obj.code == currentActionItem) {
                            
                            //Debug.Log("action.data.display_name:" + action.data.display_name);

                            obj.gameObject.Show();
                                                        
                            UIUtil.UpdateLabelObject(
                                obj.gameObject, "LabelDescription", action.data.display_name);

                            bool isCompleted = false;
                            double score = 0;

                            // CHECK ACTION COMPLETE/SCORE STATE
                            
                            GameProfileContentCollectItem collectData = 
                                GameProfileModes.Current.GetContentCollectItem(
                                    BaseDataObjectKeys.mission,
                                    GameProfileModes.GetAppContentCollectItemKey(mission.code, action.uid));
                            
                            if(collectData != null) {
                                isCompleted = collectData.complete;
                                score = collectData.points;
                            }

                            if(isCompleted) {                                
                                // figure score                                
                                scoreMission += score;
                            }

                            // STARS LIST
                                                        
                            SetStars(obj.gameObject, isCompleted);

                            // STARS STAR CONTAINER

                            GameObject starObject = null;
                            
                            foreach (GameObjectInactive starItem in 
                                     item.GetComponentsInChildren<GameObjectInactive>(true)) {
                                if(starItem.code == "stars-star-" + index.ToString()) {
                                    starObject = starItem.gameObject;

                                    //Debug.Log("Worlds:loadDataMissions::StarObject Found");
                                }
                            }

                            SetStars(starObject, isCompleted);
                        }
                    }
                }                
                
                j++;
            }                
                
            // fill score
                
            scoreTotal += scoreMission;
            
            Transform scoreObject = item.transform.FindChild("Container/Stars");
            if(scoreObject != null) {
                UIUtil.UpdateLabelObject(
                    scoreObject.gameObject, "LabelScore", scoreMission.ToString("N0"));
            }
            
            i++;
        }
                
        //Transform scoreTotalObject = item.transform.FindChild("Container/ScoreTotal");
        //if(scoreObject != null) {
        //    UIUtil.UpdateLabelObject(
        //        scoreObject.gameObject, "LabelScoreTotal", scoreTotal.ToString("N0"));
        //}

    }

    public void SetStars(GameObject starObject, bool isCompleted) {

        if(starObject == null) {
            return;
        }
        
        if(starObject != null) {
            // find start complete and incomplete
            GameObject starCompleteObject = null;
            GameObject starIncompleteObject = null;
            
            foreach(GameObjectInactive objStarState 
                    in starObject.GetComponentsInChildren<GameObjectInactive>(true)) {
                
                if(objStarState.code == BaseDataObjectKeys.complete) {
                    starCompleteObject = objStarState.gameObject;
                    //Debug.Log("Worlds:loadDataMissions::starCompleteObject Found");
                }
                else if(objStarState.code == BaseDataObjectKeys.incomplete) {
                    starIncompleteObject = objStarState.gameObject;
                    //Debug.Log("Worlds:loadDataMissions::starIncompleteObject Found");
                }
            }
            
            if(starCompleteObject != null
               && starIncompleteObject != null) {
                
                if(isCompleted) {
                    starCompleteObject.Show();
                    starIncompleteObject.Hide();
                                        
                    //Debug.Log("Worlds:loadDataMissions::Set Completed");
                }
                else {
                    starCompleteObject.Hide();
                    starIncompleteObject.Show();   
                    
                    //Debug.Log("Worlds:loadDataMissions::Set Incompleted");
                }                                
            }
        }
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
