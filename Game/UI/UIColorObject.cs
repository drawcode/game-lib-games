#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIColorObject : MonoBehaviour  {

    public virtual void Awake() {
    
    }
    
    public virtual void Start() {
        Init();
    }

    public virtual void Init() {

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

    public virtual void ColorTo(GameObject inst, Color colorTo) {
        UIColors.AnimateColor(inst, colorTo);
    }

    public virtual void SyncColors() {
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
