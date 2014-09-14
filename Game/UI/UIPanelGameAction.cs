using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public enum GameActionsMode {
    Loading,
    Internal
}

public class UIPanelGameActionMessages {
    public static string actionOverview = "panel-game-action-overview";
    public static string actionPresent = "panel-game-action-present";
    public static string actionSuccess = "panel-game-action-success";
    public static string actionsCycle = "panel-game-action-cycle";
}

public class UIPanelGameAction : UIAppPanelBaseList {
        
    public GameObject containerObject;
    
    public GameObject containerActionControlsDefault;
    
    public GameObject prefabDefault;
    
    public GameObject panelDefault;

    public UIButton buttonBack;
    public UIButton buttonNext;
    public UIButton buttonClose;
    
    public int actionsTotal = 2;
    public int currentActionIndex = 0;
    
    public float currentChangeDelay = 6f;
    
    public GameObject actionsCenterContainer;
    public GameObject actionsTopContainer;
    public GameObject actionsBottomContainer;
    public GameObject actionsTopLeftContainer;
    public GameObject actionsTopRightContainer;
    public GameObject actionsBottomLeftContainer;
    public GameObject actionsBottomRightContainer;
    public GameObject actionsRightContainer;
    public GameObject actionsLeftContainer;

    public UILabel labelCurrentActionStatus;
    
    public GameActionsMode actionsMode = GameActionsMode.Internal;

    public bool hidden = true;
    
    bool deferTap = false;

    public void Awake() {

    }
        
    public override void OnEnable() {
        base.OnEnable();

        //Messenger<DeviceOrientation>.AddListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
        Messenger<float>.AddListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        //Messenger<SwipeGesture>.AddListener(FingerGesturesMessages.OnSwipe, 
        //                                    OnInputSwipe);
        
        //Messenger<TapGesture>.AddListener(FingerGesturesMessages.OnTap, 
        //                                  OnInputTap);
    }
    
    public override void OnDisable() {
        base.OnDisable();

        //Messenger<DeviceOrientation>.RemoveListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
        Messenger<float>.RemoveListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        //Messenger<SwipeGesture>.RemoveListener(FingerGesturesMessages.OnSwipe, 
        //                                    OnInputSwipe);
        //Messenger<TapGesture>.RemoveListener(FingerGesturesMessages.OnTap, 
        //                                     OnInputTap);

    }
    
    public override void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
        
        if(UIUtil.IsButtonClicked(buttonNext, buttonName)) {
            //
            if(isVisible && actionsMode != GameActionsMode.Loading) {
                ShowActionsNext();
            }
        }
        else if(UIUtil.IsButtonClicked(buttonBack, buttonName)) {
            //
            if(isVisible && actionsMode != GameActionsMode.Loading) {
                ShowActionsPrevious();
            }
        }
        else if(UIUtil.IsButtonClicked(buttonClose, buttonName)) {
            AnimateOut();
        }
    }
    
    public override void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();            
        AnimateIn();
        ShowControlsDefault();
        ShowActionsFirst();
        currentChangeDelay = 6f;
    }
    
    public void OnInputSwipe(SwipeGesture gesture) {
        
        //if(!isVisible) {
        //  return;
        //}
        
        if(gesture.Direction == FingerGestures.SwipeDirection.Right
           || gesture.Direction == FingerGestures.SwipeDirection.UpperRightDiagonal
           || gesture.Direction == FingerGestures.SwipeDirection.Down) {
            deferTap = true;
            ShowActionsPrevious();
            
        }
        else if(gesture.Direction == FingerGestures.SwipeDirection.Left
                || gesture.Direction == FingerGestures.SwipeDirection.LowerLeftDiagonal
                || gesture.Direction == FingerGestures.SwipeDirection.Up) {
            deferTap = true;
            ShowActionsNext();
            
        }
    }
    
    public void OnInputTap(TapGesture gesture) {
        
        //if(!isVisible) {
        //  return;
        //}
        if(deferTap) {
            deferTap = false;
            return;
        }
        
        if(gesture.Taps > 0) {
            
            ShowActionsNext();
            
        }
    }
    
    public void HideControlsAll() {
        
        GameObjectHelper.HideObject(containerActionControlsDefault);
    }
    
    public void HideControlsDefault() {
        GameObjectHelper.HideObject(containerActionControlsDefault);
    }
    
    public void ShowControlsDefault() {
        HideControlsAll();      
        GameObjectHelper.ShowObject(containerActionControlsDefault);
        actionsMode = GameActionsMode.Internal;
    }

    /*
    public void ShowControlsLoad() {
        HideControlsDefault();  
        GameObjectHelper.ShowObject(containerActionControlsLoad);
        actionsMode = GameActionsMode.Loading;
    }
    
    public void HideControlsLoad() {
        GameObjectHelper.HideObject(containerActionControlsLoad);
    }
    */
    
    public void LoadData() {
        StartCoroutine(LoadDataCo());
    }
    
    IEnumerator LoadDataCo() {
        
        //HidePanelDefault();
        
        yield return new WaitForSeconds(.5f);
        
        //TakePhoto();
        
        /*
        // Load up list
        ListClear();
        
        // title        
        LoadObjectTitle("Points", "Get to the points! Track your collected points here.", "", AppViewerSectionNames.points);
        
        yield return new WaitForEndOfFrame();
        
        GameObject itemObject = LoadObject(prefabPointsTotal, AppViewerSectionNames.points);
        SetItemLabel(itemObject, "LabelPoints", 
            GameProfileStatistics.Current.GetStatisticValue(
            GameProfileStatisticAttributes.ATT_TOTAL_POINTS).ToString("N0")
            );
        
        LoadObject(prefabPointsAbout, AppViewerSectionNames.points);        
        
        yield return new WaitForEndOfFrame();
        
        ListReposition();
        
        yield return new WaitForEndOfFrame();
        
        ShowPanelDefault();
        */
        
        yield break;
    }
    
    public void ShowActionsFirst() {
        if(actionsMode == GameActionsMode.Loading) {
            ShowActionsRandomNext();
        }
        else {
            ShowAction(0);
        }
    }   
    
    public void ShowActionsRandomNext() {
        if(actionsCenterContainer != null) {
            actionsTotal = actionsCenterContainer.transform.childCount;
            ShowAction(UnityEngine.Random.Range(0, actionsTotal - 1));
        }
    }
    
    public void ShowActionsNext() {
        if(actionsMode == GameActionsMode.Loading) {
            ShowActionsRandomNext();
        }
        else {
            ShowAction(currentActionIndex + 1);
        }
    }
    
    public void ShowActionsPrevious() {
        if(actionsMode == GameActionsMode.Loading) {
            ShowActionsRandomNext();
        }
        else {
            ShowAction(currentActionIndex - 1);   
        }
    }
    
    public void ShowAction(int index) {
        
        ResetChangeTime();
        
        actionsTotal = actionsCenterContainer.transform.childCount;
        
        if(index > actionsTotal - 1) {
            Messenger<string>.Broadcast(UIPanelGameActionMessages.actionsCycle, gameObject.name);
            index = 0;
        }
        
        if(index < 0) {
            index = actionsTotal - 1;
        }
        
        currentActionIndex = index;
        
        HideAllActionContainers();

        if(gameObject.activeSelf && gameObject.activeInHierarchy) {
            StartCoroutine(ShowCurrentActionObjectsCo());
        }
    }
    
    public IEnumerator ShowCurrentActionObjectsCo() {
        yield return new WaitForSeconds(.1f);
        ShowCurrentActionObjects();
    }
    
    public void ShowCurrentActionObjects() {
        
        //AppViewerUIPanelHUD.Instance.inModalAction = true;
        
        //AppViewerUIPanelActionTrackerSearch.HideTrackerDetectObject();
        //AppViewerUIPanelActionTrackerSearch.HideTrackerDetectLabel();
        
        string actionCode = "action-" + (currentActionIndex + 1).ToString(); 
        
        ShowContainer(actionsCenterContainer, actionCode);
        ShowContainer(actionsTopContainer, actionCode);
        ShowContainer(actionsBottomContainer, actionCode);
        ShowContainer(actionsTopLeftContainer, actionCode);
        ShowContainer(actionsTopRightContainer, actionCode);
        ShowContainer(actionsBottomLeftContainer, actionCode);
        ShowContainer(actionsBottomRightContainer, actionCode);
        ShowContainer(actionsRightContainer, actionCode);
        ShowContainer(actionsLeftContainer, actionCode);

        UIUtil.SetLabelValue(
            labelCurrentActionStatus, 
            string.Format("action {0} of {1}", currentActionIndex + 1, actionsTotal));
    }
    
    public void HideAllActionContainers() {
        
        HideContainer(actionsCenterContainer);
        HideContainer(actionsTopContainer);
        HideContainer(actionsBottomContainer);
        HideContainer(actionsTopLeftContainer);
        HideContainer(actionsTopRightContainer);
        HideContainer(actionsBottomLeftContainer);
        HideContainer(actionsBottomRightContainer);
        HideContainer(actionsRightContainer);
        HideContainer(actionsLeftContainer);       
        
        //AppViewerUIPanelActionTrackerSearch.ShowTrackerDetectObject();
        //AppViewerUIPanelActionTrackerSearch.ShowTrackerDetectLabel();
        //AppViewerUIPanelActionTrackerSearch.ShowDefault();
        
        //AppViewerUIPanelHUD.Instance.inModalAction = false;
    }
    
    public void ShowContainer(GameObject container, string actionCode) {
        StartCoroutine(ShowContainerCo(container, actionCode));
    }
    
    IEnumerator ShowContainerCo(GameObject container, string actionCode) {
        
        if(container != null) {
            container.HideChildren(true);
        
            foreach(GameObjectInactive inactive in container.GetComponentsInChildren<GameObjectInactive>(true)) {
                
                if(inactive.name == actionCode ||
                   inactive.name.IndexOf(actionCode + "-") > -1) {
                    inactive.gameObject.Show();
                    
                    var animate = inactive.gameObject.GetComponent<GameObjectBouncy>();
                    if(animate != null) {
                        animate.Animate();
                    }
                    
                    UITweenerUtil.FadeTo(inactive.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, .6f, 1f);
                    //if(actionsMode != GameActionsMode.Loading) {
                    //  if(inactive.name.IndexOf(actionCode + "-Settings") > -1) {
                    //      AppViewerUIPanelHUD.ShowExtraContainer();
                    //  }
                    //  else {
                    //      AppViewerUIPanelHUD.HideExtraContainer();
                    //  }
                    //}
                }
            }   
        }
        
        
        yield break;
    }
    
    public void HideContainer(GameObject container) {
        if(container != null) {
            if(!gameObject.activeSelf || !gameObject.activeInHierarchy 
               || !container.activeSelf || !container.activeInHierarchy) {
                container.HideChildren(true);
            }
            else {
                StartCoroutine(HideContainerCo(container));
            }
        }
    }
    
    IEnumerator HideContainerCo(GameObject container) {
        if(container != null) {
            container.HideChildren(true);
        }
        
        yield return new WaitForSeconds(.1f);
        
        if(container != null) {
            //container.HideChildren(true);
        }
    }
    
    public void ShowPanelDefault() {
        ShowPanelTop(panelDefault);
    }
    
    public void HidePanelDefault() {
        
        HidePanelTop(panelDefault);
    }
    
    public void showDefault() {
        AnimateIn();
        LoadDefault();
        ShowControlsDefault();
    }

    /*
    public void showLoad() {
        AnimateIn();
        LoadDefault();
        //ShowControlsLoad();     
    }
    */
    
    public void LoadDefault() {
        ShowActionsFirst();
        LoadData();
    }
        
    public override void AnimateIn() {
        hidden = false;
        base.AnimateIn();
    }       
    
    public override void AnimateOut() {
        
        hidden = true;
        base.AnimateOut();
        
        HidePanelDefault();
    }
    
    void ResetChangeTime() {        
        currentChangeDelay = 6f;
    }
    
    void Update() {
        currentChangeDelay -= Time.deltaTime;
        if(currentChangeDelay <= 0) {
            if(GameUIController.Instance.uiVisible
               && actionsMode == GameActionsMode.Loading) {
                //ShowActionsRandomNext();
            }
        }
        
        /*
        if(lastUICheck + .8f < Time.realtimeSinceStartup) {
            lastUICheck = Time.realtimeSinceStartup;
            if(Contents.Instance.displayState != ContentSyncDisplayState.SynContentListsCompleted) {
                if(hidden) {
                    AnimateIn();
                }
            }
            else {
                if(!hidden) {
                    AnimateOut();
                }
            }
        }
        */
        
    }
}