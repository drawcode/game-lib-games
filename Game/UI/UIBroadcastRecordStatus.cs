using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIBroadcastRecordStatus : GameObjectBehavior {

    public GameObject objectRecordStatusLight;
    public UILabel labelStatus;

    public void Awake() {
        
    }

    public  void Init() {
        UpdateBroadcastStatus(BroadcastNetworksMessages.broadcastRecordingStop);
    }
    
    public void Start() {
        Init();
    }
    
    public void OnEnable() {
        
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<string>.AddListener(
            BroadcastNetworksMessages.broadcastRecordingStatusChanged, 
            OnBroadcastRecordStatusChanged);
    }
    
    public void OnDisable() {
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<string>.RemoveListener(
            BroadcastNetworksMessages.broadcastRecordingStatusChanged, 
            OnBroadcastRecordStatusChanged);
    }
    
    void OnButtonClickEventHandler(string buttonName) {
        
    }
    
    void OnBroadcastRecordStatusChanged(string broadcastStatus) {
        
        UpdateBroadcastStatus(broadcastStatus);
    }

    public void UpdateBroadcastStatus(string broadcastStatus) {
        
        UIUtil.SetLabelValue(labelStatus, "TAP TO REC");
        
        if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStart) {
            RecordingObjectPingPong();
            
            UIUtil.SetLabelValue(labelStatus, "REC...");
        }
        else if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStop) {            
            RecordingObjectStop();
        }        
        else if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingPlayback) {
            
            RecordingObjectStop();
        }
        else {
            
            RecordingObjectStop();
        }
    }
        
    public virtual void RecordingObjectPingPong() {
        AnimateObjectPingPong(objectRecordStatusLight);
    }
    
    public virtual void RecordingObjectStop() {
        AnimateObjectFadeOut(objectRecordStatusLight);
    }
    
    public virtual void AnimateObjectPingPong(GameObject go) {
        
        if (go != null) {
            
            UITweenerUtil.FadeTo(
                go,
                UITweener.Method.EaseInOut, 
                UITweener.Style.Once, 
                .5f, 0f, 1f);
            UITweenerUtil.FadeTo(
                go,
                UITweener.Method.EaseInOut, 
                UITweener.Style.PingPong, 
                .5f, .5f, .3f);
        }   
    }

    public virtual void AnimateObjectFadeOut(GameObject go) {
        
        if (go != null) {
            UITweenerUtil.FadeTo(
                go,
                UITweener.Method.EaseInOut, 
                UITweener.Style.Once, 
                2f, 0f, 0f);
        }   
    }

}

