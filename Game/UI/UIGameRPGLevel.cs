using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGLevel : UIGameRPGObject {

    public override void Start() {
        incrementValue = 1;
        profileValue = 1;
        UpdateValue();
    }

    public override void UpdateValue() {
        if(useGlobal) {
            profileValue = (int)Math.Round(GameProfileRPGs.Current.GetGamePlayerProgressLevel(1));
        }
        else {
            profileValue = (int)Math.Round(GameProfileCharacters.currentProgress.GetGamePlayerProgressLevel(1));
        }
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

        if(UIGameKeyCodes.isActionLevelAdd) {
            GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressLevel(incrementValue);
        }
        else if(UIGameKeyCodes.isActionLevelSubtract) {
            GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressLevel(-incrementValue);
        }
    }
}