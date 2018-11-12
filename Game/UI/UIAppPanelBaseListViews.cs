using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Engine.Events;
using Engine.Utility;

public enum UIAppPanelContentState {
    DEFAULT,
    APP_CONTENT_STATE,
    APP_CONTENT_STATE_DETAIL
}

public class UIAppPanelBaseListViews : UIAppPanelBaseList {

    public GameObject prefabContentDefault;
    public GameObject prefabContentAppContentState;
    public GameObject prefabContentAppContentStateDetail;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    // app state
    public GameObject contentPanelDefault;
    public UIGrid gridDefault;
    public GameObject gridDefaultObject;
    public UIPanel panelClippedDefault;


    // detail on content state, shows info, trackers and list of actions
    public GameObject contentPanelAppContentState;
    public UIGrid gridAppContentState;
    public GameObject gridAppContentStateObject;
    public UIPanel panelClippedAppContentState;

    // app content state
    public GameObject contentPanelAppContentStateDetail;
    public UIGrid gridAppContentStateDetail;
    public GameObject gridAppContentStateDetailObject;
    public UIPanel panelClippedAppContentStateDetail;
#else
    // app state
    public GameObject contentPanelDefault;
    public GameObject gridDefault;
    public GameObject gridDefaultObject;
    public GameObject panelClippedDefault;


    // detail on content state, shows info, trackers and list of actions
    public GameObject contentPanelAppContentState;
    public GameObject gridAppContentState;
    public GameObject gridAppContentStateObject;
    public GameObject panelClippedAppContentState;

    // app content state
    public GameObject contentPanelAppContentStateDetail;
    public GameObject gridAppContentStateDetail;
    public GameObject gridAppContentStateDetailObject;
    public GameObject panelClippedAppContentStateDetail;
#endif
    public UIAppPanelContentState currentState = UIAppPanelContentState.DEFAULT;

    public override void OnEnable() {
        base.OnEnable();
        // Messenger<DeviceOrientation>.AddListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
        //Messenger<float>.AddListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);        
    }

    public override void OnDisable() {
        base.OnDisable();
        // Messenger<DeviceOrientation>.RemoveListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
        //Messenger<float>.RemoveListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);     
    }

    public void ClearLists() {
        ListClear(gridDefaultObject);
        ListClear(gridAppContentStateObject);
        ListClear(gridAppContentStateDetailObject);
    }

    // --- 

    public void ShowPanelDefault() {
        TweenUtil.ShowObjectTop(contentPanelDefault);
    }

    public void HidePanelDefault() {

        TweenUtil.HideObjectTop(contentPanelDefault);
    }

    public void ShowPanelAppContentStates() {
        TweenUtil.ShowObjectRight(contentPanelAppContentState);
    }

    public void HidePanelAppContentStates() {

        TweenUtil.HideObjectRight(contentPanelAppContentState);
    }

    public void ShowPanelAppContentStateDetail() {
        TweenUtil.ShowObjectRight(contentPanelAppContentStateDetail);
    }

    public void HidePanelAppContentStateDetail() {

        TweenUtil.HideObjectRight(contentPanelAppContentStateDetail);
    }

    // --- 

    public void PanelAppContentStatesNext() {
        TweenUtil.HideObjectLeft(contentPanelDefault);
        TweenUtil.ShowObjectRight(contentPanelAppContentState);
        currentState = UIAppPanelContentState.APP_CONTENT_STATE;
        ////AppViewerUIPanelOverlays.Instance.ShowPanelBackButton();
    }

    public void PanelAppContentStatesBack() {
        TweenUtil.HideObjectRight(contentPanelAppContentState);
        TweenUtil.ShowObjectLeft(contentPanelDefault);
        currentState = UIAppPanelContentState.DEFAULT;
        ////AppViewerUIPanelOverlays.Instance.HidePanelBackButton();
    }

    public void PanelAppContentStatesDetailNext() {
        TweenUtil.HideObjectLeft(contentPanelAppContentState);
        TweenUtil.ShowObjectRight(contentPanelAppContentStateDetail);
        currentState = UIAppPanelContentState.APP_CONTENT_STATE_DETAIL;
        ////AppViewerUIPanelOverlays.Instance.ShowPanelBackButton();
    }

    public void PanelAppContentStatesDetailBack() {
        TweenUtil.HideObjectRight(contentPanelAppContentStateDetail);
        TweenUtil.ShowObjectLeft(contentPanelAppContentState);
        currentState = UIAppPanelContentState.APP_CONTENT_STATE;
    }

    public void NavigateBack() {
        //if(GameShooterUIController.Instance.uiVisible) {          
        // go back if not on app state
        //if(currentState == AppViewerUIPanelContentState.APP_CONTENT_STATE_DETAIL) {
        //  PanelAppContentStatesDetailBack();
        //}
        //else if(currentState == AppViewerUIPanelContentState.APP_CONTENT_STATE) {
        //  PanelAppContentStatesBack();
        //}

        //}
    }
}