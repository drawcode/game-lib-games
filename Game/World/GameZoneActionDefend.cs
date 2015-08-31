using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class GameZoneActionDefend : GameZoneActionAsset {
    
    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.action_defend;
        actionCode = GameZoneActions.action_defend;
    }
    
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }
    
}