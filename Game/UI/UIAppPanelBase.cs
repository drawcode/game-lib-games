using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public enum AppViewerUIPanelContext {
    DIRECT,
    BACK,
    NEXT
}

public class UIAppPanelBase : UIPanelBase {

    /*
    public GameObject listGridRoot;
    
    public UIGrid listGrid;
    public UIPanel panelClipped;
    [NonSerialized]
    public int increment = 0;
    public GameObject panelLeftObject;
    public GameObject panelLeftBottomObject;
    public GameObject panelLeftTopObject;
    public GameObject panelRightObject;
    public GameObject panelRightBottomObject;
    public GameObject panelRightTopObject;
    public GameObject panelTopObject;
    public GameObject panelBottomObject;
    public GameObject panelCenterObject;
    public GameObject panelContainer;
    [NonSerialized]
    public float durationShow = .33f;
    [NonSerialized]
    public float durationHide = .33f;
    [NonSerialized]
    public float durationDelayShow = 0f;
    [NonSerialized]
    public float durationDelayHide = .05f;
    [NonSerialized]
    public float leftOpenX = 0f;
    [NonSerialized]
    public float leftClosedX = -2000f;
    [NonSerialized]
    public float rightOpenX = 0f;
    [NonSerialized]
    public float rightClosedX = 2000f;
    [NonSerialized]
    public float bottomOpenY = 0f;
    [NonSerialized]
    public float bottomClosedY = -2000f;
    [NonSerialized]
    public float topOpenY = 0f;
    [NonSerialized]
    public float topClosedY = 2000f;
    */

    public override void Init() {
        base.Init();

        // hide all panels

        HideAllPanelsNow();
    }
}