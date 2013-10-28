using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGEnergy : UIGameRPGObject {

    public override void Start() {
        incrementValue = .01;
        profileValue = 1;
        lastValue = 0;
        UpdateValue();
    }

    public override void UpdateValue() {
        profileValue = GameProfileCharacters.currentProgress.GetGamePlayerProgressEnergy(1);
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

        if(UIGameKeyCodes.isActionEnergyAdd) {
            Debug.Log("EnergyAdd:" + incrementValue);
            GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(incrementValue);
        }
        else if(UIGameKeyCodes.isActionEnergySubtract) {
            Debug.Log("EnergySubtract:" + incrementValue);
            GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(-incrementValue);
        }
    }
}