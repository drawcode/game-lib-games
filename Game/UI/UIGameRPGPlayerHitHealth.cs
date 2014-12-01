using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGPlayerHitHealth : UIGameRPGObject {

    public override void Start() {
        incrementValue = .01;
        profileValue = 1;
        lastValue = 0;
        UpdateValue();
    }

    public override void UpdateValue() {

        if(GameController.CurrentGamePlayerController == null) {
            return;
        }
        
        if(GameController.CurrentGamePlayerController.runtimeData == null) {
            return;
        }

        profileValue = GameController.CurrentGamePlayerController.runtimeData.hitHealthRemaining;
                
    }

    public override void UpdateInterval() {
        if (lastTime > 1f) {
            lastTime = 0f;
            UpdateValue();
        }
    }

    public override void HandleUpdate(bool updateTimeInterval) {

        lastTime += Time.deltaTime;

        if (updateTimeInterval) {
            UpdateInterval();
        }

        base.HandleUpdate(false);
    }

    public override void Update() {

        HandleUpdate(true);

        if (UIGameKeyCodes.isActionPlayerHitAdd) {
            LogUtil.Log("PlayerHitAdd:" + incrementValue);
            GameController.CurrentGamePlayerController.Hit(1);
        }
        else if (UIGameKeyCodes.isActionPlayerHitSubtract) {
            LogUtil.Log("PlayerHitSubtract:" + incrementValue);
            GameController.CurrentGamePlayerController.Hit(-1);
        }
    }
}