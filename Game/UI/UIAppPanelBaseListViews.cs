using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum UIAppPanelContentState {
	DEFAULT,
	APP_CONTENT_STATE,
	APP_CONTENT_STATE_DETAIL
}

public class UIAppPanelBaseListViews: UIAppPanelBaseList {
	
	public GameObject prefabContentDefault;
	public GameObject prefabContentAppContentState;
	public GameObject prefabContentAppContentStateDetail;
			 
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
		ShowPanelTop(contentPanelDefault);
	}
	
	public void HidePanelDefault() {
		
		HidePanelTop(contentPanelDefault);
	}
	
	public void ShowPanelAppContentStates() {
		ShowPanelRight(contentPanelAppContentState);
	}
	
	public void HidePanelAppContentStates() {
		
		HidePanelRight(contentPanelAppContentState);
	}
	
	public void ShowPanelAppContentStateDetail() {
		ShowPanelRight(contentPanelAppContentStateDetail);
	}
	
	public void HidePanelAppContentStateDetail() {
		
		HidePanelRight(contentPanelAppContentStateDetail);
	}
		
	// --- 
	
	public void PanelAppContentStatesNext() {		
		HidePanelLeft(contentPanelDefault);
		ShowPanelRight(contentPanelAppContentState);
		currentState = UIAppPanelContentState.APP_CONTENT_STATE;
		////AppViewerUIPanelOverlays.Instance.ShowPanelBackButton();
	}	
	
	public void PanelAppContentStatesBack() {		
		HidePanelRight(contentPanelAppContentState);
		ShowPanelLeft(contentPanelDefault);
		currentState = UIAppPanelContentState.DEFAULT;
		////AppViewerUIPanelOverlays.Instance.HidePanelBackButton();
	}
	
	
	public void PanelAppContentStatesDetailNext() {		
		HidePanelLeft(contentPanelAppContentState);
		ShowPanelRight(contentPanelAppContentStateDetail);
		currentState = UIAppPanelContentState.APP_CONTENT_STATE_DETAIL;
		////AppViewerUIPanelOverlays.Instance.ShowPanelBackButton();
	}	
	
	public void PanelAppContentStatesDetailBack() {		
		HidePanelRight(contentPanelAppContentStateDetail);
		ShowPanelLeft(contentPanelAppContentState);
		currentState = UIAppPanelContentState.APP_CONTENT_STATE;
	}	
	
	public void NavigateBack() {
		//if(GameShooterUIController.Instance.uiVisible) {			
			// go back if not on app state
			//if(currentState == AppViewerUIPanelContentState.APP_CONTENT_STATE_DETAIL) {
			//	PanelAppContentStatesDetailBack();
			//}
			//else if(currentState == AppViewerUIPanelContentState.APP_CONTENT_STATE) {
			//	PanelAppContentStatesBack();
			//}
			
		//}
	}
			
}
