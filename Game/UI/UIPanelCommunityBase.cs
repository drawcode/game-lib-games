#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityBase : UIPanelBase {

    public virtual void Awake() {

    }

    public override void Init() {
        base.Init();

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
        base.OnButtonClickEventHandler(buttonName);
    }

    public virtual void showDialog() {

        GameCommunity.HideGameCommunity();

        UIPanelCommunityBackground.ShowBackground();  

        if(GameController.Instance.gameRunningState == GameRunningState.RUNNING) {
            GameController.GameRunningStateOverlay();
        }
    }

    public virtual void hideDialog() {
        
        UIPanelCommunityBackground.HideBackground();  
        
        if(GameController.Instance.gameRunningState != GameRunningState.RUNNING
           && GameController.Instance.gameState == GameStateGlobal.GameOverlay) {
            GameController.GameRunningStateRun();
        }
    }

    public override void AnimateOut() {
        base.AnimateOut();
    }

    public virtual void Update() {
        //base.Update();
    } 
}
