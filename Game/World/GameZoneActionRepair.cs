using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class GameZoneActionRepair : GameZoneActionAsset {

    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.action_repair;
        actionCode = GameZoneActions.action_repair;
    }

    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnDisable() {
        base.OnDisable();
    }
}