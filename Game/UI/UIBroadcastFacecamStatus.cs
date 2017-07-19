#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIBroadcastFacecamStatus : GameObjectBehavior {

    public GameObject objectRecordStatusLight;
    public UILabel labelStatus;

    public void Awake() {

    }

    public void Init() {

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

        if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStart) {

        }
        else if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingStop) {

        }
        else if(broadcastStatus == BroadcastNetworksMessages.broadcastRecordingPlayback) {

        }
        else {

        }

    }
}