using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameZoneAction : GameZone {

    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.action_save;
    }
    
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }
    
}