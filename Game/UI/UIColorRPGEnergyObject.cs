#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIColorRPGEnergyObject : UIColorObject  {

    public override void Awake() {
        SyncColors();
    }
    
    public override void Start() {
        Init();
    }

    public override void Init() {
        SyncColors();
    }

    void OnEnable() {
        Messenger.AddListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }

    void OnDisable() {
        Messenger.RemoveListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }

    void OnColorsUpdateHandler() {
        SyncColors();
    }

    public override void SyncColors() {
        // Set initial color by mode

        if(AppModes.Instance.isAppModeGameTraining) {
            // purple
            UIColors.ColorToPurple(gameObject);
        }
        else if(AppModes.Instance.isAppModeGameChallenge) {
            // blue
            UIColors.ColorToBlue(gameObject);
        }
        else if(AppModes.Instance.isAppModeGameArcade) {
            // green
            UIColors.ColorToOrange(gameObject);
        }
    }
}
