using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

public class BaseGamePlayerIndicatorItem : GameObjectBehavior {

    public string gameIndicatorTypeCode = "color";

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelValue;
#else
    public Text labelValue;
#endif
    public GameObject containerEffects;
    public GameObject backerObject;
    public GameObject outlineObject;

    public virtual void Start() {

    }

    public virtual void SetLabelValue(string val) {
        UIUtil.SetLabelValue(labelValue, val);
    }

    public virtual void SetColorValue(Color color) {
        UIUtil.SetSpriteColor(gameObject, color);
    }

    public virtual void SetColorValueBackground(Color color) {
        UIUtil.SetSpriteColor(backerObject, color);
    }

    public virtual void SetColorValueOutline(Color color) {
        UIUtil.SetSpriteColor(outlineObject, color);
    }

    public virtual void SetColorValueEffects(Color color) {
        if (containerEffects != null) {
            containerEffects.SetParticleSystemStartColor(color, true);
        }
    }
}