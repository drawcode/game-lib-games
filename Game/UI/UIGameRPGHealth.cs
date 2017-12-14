using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGHealth : UIGameRPGObject {

    public override void Start() {
        incrementValue = .01;
        profileValue = 1;
        lastValue = 0;
        UpdateValue();
    }

    public override void UpdateValue() {
        //if(useGlobal) {
        //    profileValue = GameProfileRPGs.Current.GetGamePlayerProgressHealth(1);
        //}
        //else {
        profileValue = Math.Round(GameProfileCharacters.currentProgress.GetGamePlayerProgressHealth(1), 2);
        //}
    }

    public override void UpdateInterval() {
        if(lastTime > 3f) {
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

        if(UIGameKeyCodes.isActionHealthAdd) {
            GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(incrementValue);
        }
        else if(UIGameKeyCodes.isActionHealthSubtract) {
            GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(-incrementValue);
        }
    }
}