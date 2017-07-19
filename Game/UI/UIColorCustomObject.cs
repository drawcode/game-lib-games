#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIColorCustomObject : UIColorObject {

    public string colorKey = "";
    public float colorAlpha = 1f;

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

    public void SetColorByKey(string key, float alpha) {
        colorKey = key;
        colorAlpha = alpha;
        SyncColors();
    }

    public override void SyncColors() {

        if(!string.IsNullOrEmpty(colorKey)) {

            Color colorTo = GameProfileCharacters.currentCustom.GetCustomColor(colorKey);
            colorTo.a = colorAlpha;
            UIColors.ColorTo(
                gameObject, colorTo);
        }
    }
}