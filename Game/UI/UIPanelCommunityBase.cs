#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityBase : UIPanelBase {

    public override void Awake() {
        base.Awake();
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

#if USE_GAME_LIB_GAMEVERSES
        GameCommunity.HideGameCommunity();

        UIPanelCommunityBackground.ShowBackground();
#endif

        if(GameController.Instance.gameRunningState == GameRunningState.RUNNING) {
            GameController.GameRunningStateOverlay();
        }
    }

    public virtual void hideDialog() {

#if USE_GAME_LIB_GAMEVERSES
        UIPanelCommunityBackground.HideBackground();
#endif

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