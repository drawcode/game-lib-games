using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public enum TipsMode {
    Loading,
    Internal
}

public class UIPanelTipsMessages {
    public static string tipsCycle = "panel-tips-cycle";
}

public class UIPanelTips : UIAppPanelBaseList {
        
    public GameObject containerObject;
    
    public GameObject containerTipControlsDefault;
    public GameObject containerTipControlsLoad;
    
    public GameObject prefabDefault;
    
    public GameObject panelDefault;
    
    public GameObject panelHelp;
    
    public UIButton buttonBack;
    public UIButton buttonNext;
    public UIButton buttonClose;
    
    public int tipsTotal = 2;
    public int currentTipIndex = 0;
    
    public float currentChangeDelay = 6f;
    
    public GameObject tipsCenterContainer;
    public GameObject tipsTopContainer;
    public GameObject tipsBottomContainer;
    public GameObject tipsTopLeftContainer;
    public GameObject tipsTopRightContainer;
    public GameObject tipsBottomLeftContainer;
    public GameObject tipsBottomRightContainer;
    public GameObject tipsRightContainer;
    public GameObject tipsLeftContainer;

    public UILabel labelCurrentTipStatus;
    
    public TipsMode tipsMode = TipsMode.Internal;

    public bool hidden = true;
    
    bool deferTap = false;

    public void Awake() {

    }
        
    public override void OnEnable() {
        base.OnEnable();

        //Messenger<DeviceOrientation>.AddListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
        Messenger<float>.AddListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<SwipeGesture>.AddListener(FingerGesturesMessages.OnSwipe, 
                                            OnInputSwipe);
        
        Messenger<TapGesture>.AddListener(FingerGesturesMessages.OnTap, 
                                          OnInputTap);
    }
    
    public override void OnDisable() {
        base.OnDisable();

        //Messenger<DeviceOrientation>.RemoveListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
        Messenger<float>.RemoveListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<SwipeGesture>.RemoveListener(FingerGesturesMessages.OnSwipe, 
                                            OnInputSwipe);
        Messenger<TapGesture>.RemoveListener(FingerGesturesMessages.OnTap, 
                                             OnInputTap);

    }
    
    void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
        
        if(UIUtil.IsButtonClicked(buttonNext, buttonName)) {
            //
            if(isVisible && tipsMode != TipsMode.Loading) {
                ShowTipsNext();
            }
        }
        else if(UIUtil.IsButtonClicked(buttonBack, buttonName)) {
            //
            if(isVisible && tipsMode != TipsMode.Loading) {
                ShowTipsPrevious();
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
        ShowTipsFirst();
        currentChangeDelay = 6f;
    }
    
    public void OnInputSwipe(SwipeGesture gesture) {
        
        if(!isVisible) {
          return;
        }
        
        if(gesture.Direction == FingerGestures.SwipeDirection.Right
           || gesture.Direction == FingerGestures.SwipeDirection.UpperRightDiagonal
           || gesture.Direction == FingerGestures.SwipeDirection.LowerRightDiagonal
           || gesture.Direction == FingerGestures.SwipeDirection.Down) {
            deferTap = true;
            ShowTipsPrevious();
            
        }
        else if(gesture.Direction == FingerGestures.SwipeDirection.Left
                || gesture.Direction == FingerGestures.SwipeDirection.UpperLeftDiagonal
                || gesture.Direction == FingerGestures.SwipeDirection.LowerLeftDiagonal
                || gesture.Direction == FingerGestures.SwipeDirection.Up) {
            deferTap = true;
            ShowTipsNext();
            
        }
    }
    
    public void OnInputTap(TapGesture gesture) {
        StartCoroutine(OnInputTapCo(gesture));
    }

    public IEnumerator OnInputTapCo(TapGesture gesture) {
        yield return new WaitForSeconds(.3f);
        
        if(!isVisible) {
            yield break;
        }
        
        if(GameController.Instance.levelInitializing) {
            ShowTipsFirst();
            yield break;
        }
        
        if(deferTap) {
            deferTap = false;
            yield break;
        }
        
        //if(gesture.Taps > 0) {
        
        ShowTipsNext();
        
        //}
    }
    
    public void HideControlsAll() {
        
        GameObjectHelper.HideObject(containerTipControlsDefault);
        GameObjectHelper.HideObject(containerTipControlsLoad);
    }
    
    public void HideControlsDefault() {
        GameObjectHelper.HideObject(containerTipControlsDefault);
    }
    
    public void ShowControlsDefault() {
        HideControlsAll();      
        GameObjectHelper.ShowObject(containerTipControlsDefault);
        tipsMode = TipsMode.Internal;
    }
    
    public void ShowControlsLoad() {
        HideControlsDefault();  
        GameObjectHelper.ShowObject(containerTipControlsLoad);
        tipsMode = TipsMode.Loading;
    }
    
    public void HideControlsLoad() {
        GameObjectHelper.HideObject(containerTipControlsLoad);
    }
    
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
    
    public void ShowTipsFirst() {
        isVisible = true;

        if(tipsMode == TipsMode.Loading) {
            ShowTipsRandomNext();
        }
        else {
            ShowTip(0);
        }
    }   
    
    public void ShowTipsRandomNext() {
        if(tipsCenterContainer != null) {
            tipsTotal = tipsCenterContainer.transform.childCount;
            ShowTip(UnityEngine.Random.Range(0, tipsTotal - 1));
        }
    }
    
    public void ShowTipsNext() {
        if(tipsMode == TipsMode.Loading) {
            ShowTipsRandomNext();
        }
        else {
            ShowTip(currentTipIndex + 1);
        }
    }
    
    public void ShowTipsPrevious() {
        if(tipsMode == TipsMode.Loading) {
            ShowTipsRandomNext();
        }
        else {
            ShowTip(currentTipIndex - 1);   
        }
    }
    
    public void ShowTip(int index) {
        
        ResetChangeTime();
        
        tipsTotal = tipsCenterContainer.transform.childCount;
        
        if(index > tipsTotal - 1) {
            Messenger<string>.Broadcast(UIPanelTipsMessages.tipsCycle, gameObject.name);
            index = 0;
        }
        
        if(index < 0) {
            index = tipsTotal - 1;
        }
        
        currentTipIndex = index;
        
        HideAllTipContainers();

        if(gameObject.activeSelf && gameObject.activeInHierarchy) {
            StartCoroutine(ShowCurrentTipObjectsCo());
        }
    }
    
    public IEnumerator ShowCurrentTipObjectsCo() {
        yield return new WaitForSeconds(.1f);
        ShowCurrentTipObjects();
    }
    
    public void ShowCurrentTipObjects() {
        
        //AppViewerUIPanelHUD.Instance.inModalAction = true;
        
        //AppViewerUIPanelActionTrackerSearch.HideTrackerDetectObject();
        //AppViewerUIPanelActionTrackerSearch.HideTrackerDetectLabel();
        
        string tipCode = "tip-" + (currentTipIndex + 1).ToString(); 
        
        ShowContainer(tipsCenterContainer, tipCode);
        ShowContainer(tipsTopContainer, tipCode);
        ShowContainer(tipsBottomContainer, tipCode);
        ShowContainer(tipsTopLeftContainer, tipCode);
        ShowContainer(tipsTopRightContainer, tipCode);
        ShowContainer(tipsBottomLeftContainer, tipCode);
        ShowContainer(tipsBottomRightContainer, tipCode);
        ShowContainer(tipsRightContainer, tipCode);
        ShowContainer(tipsLeftContainer, tipCode);

        UIUtil.SetLabelValue(
            labelCurrentTipStatus, 
            string.Format("Tip {0} of {1}", currentTipIndex + 1, tipsTotal));
    }
    
    public void HideAllTipContainers() {
        
        HideContainer(tipsCenterContainer);
        HideContainer(tipsTopContainer);
        HideContainer(tipsBottomContainer);
        HideContainer(tipsTopLeftContainer);
        HideContainer(tipsTopRightContainer);
        HideContainer(tipsBottomLeftContainer);
        HideContainer(tipsBottomRightContainer);
        HideContainer(tipsRightContainer);
        HideContainer(tipsLeftContainer);       
        
        //AppViewerUIPanelActionTrackerSearch.ShowTrackerDetectObject();
        //AppViewerUIPanelActionTrackerSearch.ShowTrackerDetectLabel();
        //AppViewerUIPanelActionTrackerSearch.ShowDefault();
        
        //AppViewerUIPanelHUD.Instance.inModalAction = false;
    }
    
    public void ShowContainer(GameObject container, string tipCode) {
        StartCoroutine(ShowContainerCo(container, tipCode));
    }
    
    IEnumerator ShowContainerCo(GameObject container, string tipCode) {
        
        if(container != null) {
            container.HideChildren(true);
        
            foreach(GameObjectInactive inactive in container.GetComponentsInChildren<GameObjectInactive>(true)) {
                
                if(inactive.name == tipCode ||
                   inactive.name.IndexOf(tipCode + "-") > -1) {
                    inactive.gameObject.Show();
                    
                    var animate = inactive.gameObject.GetComponent<GameObjectBouncy>();
                    if(animate != null) {
                        animate.Animate();
                    }
                    
                    UITweenerUtil.FadeTo(inactive.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, .6f, 1f);
                    //if(tipsMode != TipsMode.Loading) {
                    //  if(inactive.name.IndexOf(tipCode + "-Settings") > -1) {
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
    
    public void showLoad() {
        AnimateIn();
        LoadDefault();
        ShowControlsLoad();     
    }
    
    public void LoadDefault() {
        ShowTipsFirst();
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
               && tipsMode == TipsMode.Loading) {
                //ShowTipsRandomNext();
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