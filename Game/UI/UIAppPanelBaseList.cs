using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



public class UIAppPanelBaseList : UIAppPanelBase {
	
	public GameObject prefabTitle;
	
	public float currentScale = 1f;
		
    public override void OnEnable() {
        base.OnEnable();
        //Messenger<DeviceOrientation>.AddListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
		//Messenger<float>.AddListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);		
    }

    public override void OnDisable() {
        base.OnDisable();
        //Messenger<DeviceOrientation>.RemoveListener(DeviceOrientationMessages.deviceOrientationChange, OnDeviceOrientationChangeHandler);
		//Messenger<float>.RemoveListener(DeviceOrientationMessages.deviceScreenRatioChange, OnDeviceScreenRatioChangeHandler);		
    }
	
	public virtual void OnDeviceOrientationChangeHandler(DeviceOrientation orientationTo) {
		if(orientationTo == DeviceOrientation.LandscapeLeft
			|| orientationTo == DeviceOrientation.LandscapeRight) {
			
			
		}
		else if(orientationTo == DeviceOrientation.Portrait
			|| orientationTo == DeviceOrientation.PortraitUpsideDown) {
			
		}
	}
	
	public virtual void OnDeviceScreenRatioChangeHandler(float scaleTo) {
		ListScale(scaleTo);
	}
	
	public void ListContainerScale(GameObject listObject, float scaleTo) {
		if(listObject != null) {
			Vector3 currentScale = listObject.transform.localScale;
			
			float screenWidth = 640;
			float screenHeight = 960;
			
			scaleTo = Mathf.Clamp(scaleTo/(screenWidth/screenHeight), .5f, 2f);
			
			currentScale = currentScale.WithX(scaleTo).WithY(scaleTo).WithZ(scaleTo);
			
			listObject.transform.localScale = currentScale;
		}
	}
	
	
	public void ListScale(GameObject listObject, float scaleTo) {
		if(listObject != null) {
			Vector3 currentScale = listObject.transform.localScale;
			
			float screenWidth = 640;
			float screenHeight = 960;
			
			scaleTo = Mathf.Clamp(scaleTo/(screenWidth/screenHeight), .5f, 2f);
			
			currentScale = currentScale.WithX(scaleTo).WithY(scaleTo).WithZ(scaleTo);
			
			listObject.transform.localScale = currentScale;
		}
	}
	
	public void PanelScale(UIPanel panel) {		
		if(panelClipped != null) {
			Vector4 range = panelClipped.clipRange;
			range.x = 0f;
			//range.y = 0f;
			range.z = 2500f;
			//range.y = 2500f;
			//range.w = 380f;
			panelClipped.clipRange = range;
		}
	}
	
	public void ListScale(float scaleTo) {
		if(listGridRoot != null) {
			ListScale(listGridRoot, scaleTo);
		}
		
		PanelScale(panelClipped);
		
		ListReposition();
	}
		
	public override void Start() {
		base.Start();		
	}
	
	public override void Init() {
		base.Init();
	}
	
	public void ListClear() {
		ListClear(listGridRoot);
	}
	
	public void ListClear(GameObject listObject) {
		if (listObject != null) {
	    	foreach (Transform item in listObject.transform) {
	        	Destroy(item.gameObject);
	        }
		}
	}
	
	public void ListReposition() {	
		increment = 0;
		if(listGrid != null) {
			RepositionList(listGrid, listGridRoot);			
		}
	}
	
	public void ListReposition(UIGrid grid, GameObject gridObject) {	
		increment = 0;
		if(grid != null) {
			RepositionList(grid, gridObject);			
		}
	}
	
	public void RepositionList(UIGrid grid, GameObject gridObject) {
		if(grid != null) {
			grid.Reposition();			
			if(gridObject.transform.parent != null) {
				UIDraggablePanel[] dragPanels = gridObject.transform.parent.gameObject.GetComponentsInChildren<UIDraggablePanel>();
				if(dragPanels != null) {					
					foreach(UIDraggablePanel panel 
						in dragPanels) {
						panel.ResetPosition();
						break;
					}
				}
			}			
		}
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		ListClear();
	}
	
	public virtual void LoadObjectTitle(GameObject gridObject, string title, string description, string note, string type) {
		LoadObject(gridObject, prefabTitle, "0-title", title, description, note, type);
	}
	
	public virtual void LoadObjectTitle(string title, string description, string note, string type) {
		LoadObject(prefabTitle, "0-title", title, description, note, type);
	}
	
	public virtual void LoadObjectTitle() {
		LoadObject(prefabTitle, "0-title");
	}
			
}
