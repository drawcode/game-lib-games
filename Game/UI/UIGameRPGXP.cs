using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGXP : UIGameRPGObject {

    public override void Start() {
        incrementValue = 1;
        UpdateValue();
    }

    public override void UpdateValue() {
        profileValue = (int)Math.Round(GameProfileRPGs.Current.GetGamePlayerProgressXP(10));
    }

    public override void UpdateInterval() {
        if(lastTime > 1f) {
            lastTime = 0f;
            UpdateValue();
        }
    }

    public override void HandleUpdate(bool updateTimeInterval) {

        lastTime += Time.deltaTime;

        if(updateTimeInterval) {
            UpdateInterval();
        }

        base.HandleUpdate(false);
    }

    public override void Update() {
        HandleUpdate(true);

        if(UIGameKeyCodes.isActionXPAdd) {
            GameProfileRPGs.Current.AddGamePlayerProgressXP(incrementValue * 100);
        }
        else if(UIGameKeyCodes.isActionXPSubtract) {
            GameProfileRPGs.Current.SubtractGamePlayerProgressXP(incrementValue * 100);
        }
    }
}