using System;
using System.Collections;
using UnityEngine;

using Engine.Events;

public class GameUISceneRoot : GameObjectBehavior {
    
    public LoadSceneAsync loadAsync;
    public UILabel labelProgressTitle = null;
    public UILabel labelProgressMessage = null;
    public UILabel labelProgressPercentage = null;
    public UISlider sliderProgress = null;
    public UISlider sliderProgressItem = null;
    public float currentItemProgress = 0.0f;
    public float currentEasingProgress = 0.0f;
    public float currentProgressItem = 0.0f;
    public float currentProgressItemEasing = 0.0f;
    float progressLevel = 0;
    float progressLevelCount = 0;
    
    public void OnEnable() {
        Messenger<object>.AddListener(ContentMessages.ContentSyncShipContentSuccess, OnContentSyncShipContentSuccess);
        
        Messenger<string,string,float>.AddListener(
                ContentMessages.ContentProgressMessage, 
                OnContentProgressMessageHandler);        
        Messenger<ContentItemStatus>.AddListener("content-item-status", OnContentItemStatus);
    }

    public void OnDisable() {
        Messenger<object>.RemoveListener(ContentMessages.ContentSyncShipContentSuccess, OnContentSyncShipContentSuccess);
    
        Messenger<string,string,float>.RemoveListener(
                ContentMessages.ContentProgressMessage, 
                OnContentProgressMessageHandler);        
        Messenger<ContentItemStatus>.RemoveListener("content-item-status", OnContentItemStatus);
    }
    
    public void OnContentSyncShipContentSuccess(object obj) {
        loadAsync.LoadLevel("GameSceneDynamic");
    }
    
    IEnumerator Start() {

        var glob = GameGlobal.Instance;

        // HACK to prevent game starting in portrait due to Unity bug 

        switch (Input.deviceOrientation) {

        case DeviceOrientation.FaceDown:

        case DeviceOrientation.FaceUp:

        case DeviceOrientation.Portrait:

        case DeviceOrientation.PortraitUpsideDown:

            // None landscape orientation, set it manually

            Screen.orientation = ScreenOrientation.LandscapeLeft;

            // Wait a bit

            yield return new WaitForSeconds(0.5f);

            // Set back to autorotation, it should be alright by now

            Screen.orientation = ScreenOrientation.AutoRotation;

            break;

        }
        
        //Application.LoadLevel("GameUISceneSplash");   
        
        yield return null;

    }
    
    public void OnContentItemStatus(ContentItemStatus contentItemStatus) {
        currentProgressItemEasing = (float)contentItemStatus.itemProgress;
    }
    
    public void SetTitle(string title) {
        UIUtil.SetLabelValue(labelProgressTitle, title);
    }
    
    public void SetDescription(string description) {
        UIUtil.SetLabelValue(labelProgressMessage, CleanupString(description));
    }
            
    public void SetProgress(float progress) {
            
        progress = Mathf.Clamp(progress, .1f, 1f);
        
        float currentProgress = progress;
        
        progressLevel = (float)Contents.Instance.displayState;
        progressLevelCount = (float)Contents.Instance.displayStateCount;
        
        currentProgress = (progress + progressLevel) / progressLevelCount;
                        
        //(0 + 3) / 7
        //(1 + 3) / 7
        
        currentEasingProgress = currentProgress;
        currentProgressItemEasing = progress;
    }
    
    public void OnContentProgressMessageHandler(
        string title, string description, float progress) {
        
        SetTitle(title);
        SetDescription(description);
        SetProgress(progress);
    }
    
    public string ToTitleCase(string str) {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
    
    public string CleanupString(string val) {
        val = val.Replace("-", " ");
        val = val.Replace("_", " ");
        val = ToTitleCase(val);
        return val;
    }
    
    void Update() {         
        
        if (currentEasingProgress > currentItemProgress) {
                
            //currentEasingProgress -= Time.deltaTime;
            
            UIUtil.SetSliderValue(sliderProgress, currentEasingProgress);
        }
        else if (currentEasingProgress < currentItemProgress) {
                
            currentEasingProgress += Time.deltaTime;
            
            UIUtil.SetSliderValue(sliderProgress, currentEasingProgress);           
        }
        
        if (currentProgressItemEasing > currentProgressItem) {
            
            currentProgressItemEasing -= Time.deltaTime;
            
            UIUtil.SetSliderValue(sliderProgressItem, currentProgressItemEasing);
        }
        else if (currentProgressItemEasing < currentProgressItem) {
                
            currentProgressItemEasing += Time.deltaTime;
            
            UIUtil.SetSliderValue(sliderProgressItem, currentProgressItemEasing);   
        }
    }
    
}

