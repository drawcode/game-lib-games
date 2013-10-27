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

    public void Awake() {

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

    public virtual void OnButtonClickEventHandler(string buttonName) {

    }

    public void ShowContent() {

        UIPanelDialogBackground.ShowDefault();

        AnimateInBottom(containerContent);

        UIColors.UpdateColors();
    }

    public void HideContent() {

        AnimateOutBottom(containerContent, 0f, 0f);
    }

    public override void AnimateIn() {
        base.AnimateIn();

        ShowContent();
    }

    public override void AnimateOut() {
        base.AnimateOut();

    }

    public void Update() {

    }
 
}
