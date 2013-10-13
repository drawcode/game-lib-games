using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGObject : MonoBehaviour {

    public double profileValue = 0;
    public double lastValue = 0;
    public double incrementValue = .01;

    public bool useGlobal = false;

    public float lastTime = 0f;

    public UISlider sliderProgress;
    public UILabel labelProgress;
    public UILabel labelValue;

    public virtual void Start() {
        UpdateValue();
    }

    public virtual void UpdateValue() {
        profileValue = 0;
    }

    public virtual void SetLabelValue(double val) {
        UIUtil.SetLabelValue(labelValue, val.ToString("N0"));
    }

    public virtual void SetProgress(double val) {
        SetProgressValue(val);
        SetProgressLabelValue(val);
    }

    public virtual void SetProgressLabelValue(double val) {
        UIUtil.SetLabelValue(labelProgress, val.ToString("P0"));
    }

    public virtual void SetProgressValue(double val) {
        UIUtil.SetSliderValue(sliderProgress, val);
    }

    public virtual void UpdateInterval() {
        if(lastTime > 1f) {
            lastTime = 0f;
            UpdateValue();
        }
    }

    public virtual void HandleUpdate(bool updateIntervalBase) {

        if(updateIntervalBase) {
            lastTime += Time.deltaTime;
            UpdateInterval();
        }

        if(lastValue > profileValue) {
            lastValue -= incrementValue;
        }
        else if(profileValue > lastValue) {
            lastValue += incrementValue;
        }

        if(lastValue < 0) {
            lastValue = 0;
        }

        SetProgress(profileValue);
        SetLabelValue(lastValue);
    }


    public virtual void Update() {
        
        if(GameConfigs.isGameRunning) {
            return;
        }

        HandleUpdate(true);
    }
}