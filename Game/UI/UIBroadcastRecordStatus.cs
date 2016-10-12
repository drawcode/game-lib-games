using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class UIBroadcastRecordStatus : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelStatus;
    public UILabel labelStatusAction;
#else
    public Text labelStatus;
    public Text labelStatusAction;
#endif

    public GameObject objectRecordStatusLight;

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
        
        UIUtil.SetLabelValue(labelStatus, "NOT RECORDING...");
        UIUtil.SetLabelValue(labelStatusAction, "TAP TO START");
        
        if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStart) {
            RecordingObjectPingPong();
            
            UIUtil.SetLabelValue(labelStatus, "RECORDING...");
            UIUtil.SetLabelValue(labelStatusAction, "TAP TO STOP");
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

        AnimateObjectPingPongRecursive(objectRecordStatusLight);
    }
    
    public virtual void RecordingObjectStop() {
        AnimateObjectFadeOutRecursive(objectRecordStatusLight);
    }

    
    public virtual void AnimateObjectPingPongRecursive(GameObject go) {

        AnimateObjectPingPong(go);

        foreach(Transform t in go.transform) {
            AnimateObjectPingPongRecursive(t.gameObject);
        }
    }

    public virtual void AnimateObjectFadeOutRecursive(GameObject go) {
        
        AnimateObjectFadeOut(go);
        
        foreach(Transform t in go.transform) {
            AnimateObjectFadeOutRecursive(t.gameObject);
        }
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
                .5f, .5f, .4f);
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

