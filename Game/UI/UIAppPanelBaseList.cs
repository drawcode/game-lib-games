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
        if (orientationTo == DeviceOrientation.LandscapeLeft
            || orientationTo == DeviceOrientation.LandscapeRight) {
            
            
        }
        else if (orientationTo == DeviceOrientation.Portrait
            || orientationTo == DeviceOrientation.PortraitUpsideDown) {
            
        }
    }
    
    public virtual void OnDeviceScreenRatioChangeHandler(float scaleTo) {
        ListScale(scaleTo);
    }
                
    public override void Start() {
        base.Start();       
    }
    
    public override void Init() {
        base.Init();
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
