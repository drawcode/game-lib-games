#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelDialogRPGObject : UIPanelBase {

    public GameObject containerContent;

    public UIImageButton buttonBuyRecharge;
    public UIImageButton buttonEarn;
    public UIImageButton buttonResume;

    public UILabel labelTip;
    public UILabel labelTitle;
    public UILabel labelAbout;
    public UILabel labelScore;

    public UISlider sliderValue;

    public virtual void Awake() {        

    }

    public override void Init() {
        base.Init();

        //loadData();
    }

    public override void Start() {
        Init();
    }

    // EVENTS
 
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    public override void OnButtonClickEventHandler(string buttonName) {

    }

    // OVERLAY DIALOG

    public void ContentPause() {
        GameController.GameRunningStateContent();
    }

    public void ContentRun() {
        GameController.GameRunningStateRun();
        //HideStates();
    }

    public void ShowContent() {

        UIPanelDialogBackground.ShowDefault();

        AnimateInBottom(containerContent);

        ContentPause();

        UIColors.UpdateColors();
    }

    public void HideContent() {

        UIPanelDialogBackground.HideAll();

        ContentPause();

        AnimateOutBottom(containerContent, 0f, 0f);
    }

    public override void AnimateIn() {
        base.AnimateIn();

        ShowContent();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideContent();
    }

    public virtual void Update() {

    }
 
}
