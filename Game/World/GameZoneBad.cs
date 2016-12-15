using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameZoneBad : GameZone {

    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.bad_out_of_bounds;
    }

    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnDisable() {
        base.OnDisable();
    }
}