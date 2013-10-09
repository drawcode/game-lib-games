using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGEnergy : UIGameRPGObject {

    public override void Start() {
        incrementValue = .1;
        profileValue = 1;
        UpdateValue();
    }

    public override void UpdateValue() {
        if(useGlobal) {
            profileValue = GameProfileRPGs.Current.GetGamePlayerProgressEnergy(1);
        }
        else {
            profileValue = GameProfileCharacters.currentProgress.GetGamePlayerProgressEnergy(1);
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

        if(GameConfigs.isGameRunning) {
            return;
        }
        
        HandleUpdate(true);

        if(UIGameKeyCodes.isActionEnergyAdd) {
            if(useGlobal) {
                GameProfileRPGs.Current.AddGamePlayerProgressEnergy(incrementValue);
            }
            else {
                GameProfileCharacters.currentProgress.AddGamePlayerProgressEnergy(incrementValue);
            }
        }
        else if(UIGameKeyCodes.isActionEnergySubtract) {
            if(useGlobal) {
                GameProfileRPGs.Current.SubtractGamePlayerProgressEnergy(incrementValue);
            }
            else {
                GameProfileCharacters.currentProgress.SubtractGamePlayerProgressEnergy(incrementValue);
            }
        }
    }
}