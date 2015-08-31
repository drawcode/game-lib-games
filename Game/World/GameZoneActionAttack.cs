using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class GameZoneActionAttack : GameZoneActionAsset {
    
    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.action_build;
        actionCode = GameZoneActions.action_build;
    }
    
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }
    
}