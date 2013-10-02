using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;


public class UIGameRPGCurrency : MonoBehaviour {

    public UILabel labelValue;

    public int profileValue = 0;
    public int lastValue = 0;

    public float lastTime = 0f;

    void Start() {
        UpdateValue();
    }

    void UpdateValue() {
        profileValue = (int)Math.Round(GameProfileRPGs.Current.GetCurrency());
    }

    void SetLabelValue(double val) {
        if(labelValue != null) {
            UIUtil.SetLabelValue(labelValue, val.ToString("N0"));
        }
    }

    void Update() {

        lastTime += Time.deltaTime;

        if(lastTime > 1f) {
            lastTime = 0f;
            UpdateValue();
        }

        if(lastValue > profileValue) {
            lastValue -= 1;
        }
        else if(profileValue > lastValue) {
            lastValue += 1;
        }

        if(lastValue < 0) {
            lastValue = 0;
        }

        SetLabelValue(lastValue);

        if(UIGameKeyCodes.isActionCurrencyAdd) {
            GameProfileRPGs.Current.AddCurrency(100);
        }
        else if(UIGameKeyCodes.isActionCurrencySubtract) {
            GameProfileRPGs.Current.SubtractCurrency(100);
        }
    }
}