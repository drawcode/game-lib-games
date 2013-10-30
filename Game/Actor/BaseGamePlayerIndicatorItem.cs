using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGamePlayerIndicatorItem : MonoBehaviour {

	public string gameIndicatorTypeCode = "color";

    public UILabel labelValue;

    public GameObject containerEffects;
    public GameObject backerObject;
    public GameObject outlineObject;
    
    void Start() {

    }
    
    public void SetLabelValue(string val) {
        UIUtil.SetLabelValue(labelValue, val);
    }
    
    public void SetColorValue(Color color) {
        UIUtil.SetSpriteColor(gameObject, color);
    }

    public void SetColorValueBackground(Color color) {
        UIUtil.SetSpriteColor(backerObject, color);
    }

    public void SetColorValueOutline(Color color) {
        UIUtil.SetSpriteColor(outlineObject, color);
    }
    
    public void SetColorValueEffects(Color color) {
        if(containerEffects != null) {
            containerEffects.SetParticleSystemStartColor(color, true);
        }
    }
}