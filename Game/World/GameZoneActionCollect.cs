using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class GameZoneActionCollect : GameZoneAction {
    
    public override void Start() {
        base.Start();
        gameZoneType = GameZoneKeys.action_save;
        actionCode = GameZoneActions.action_save;
    }
    
    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }
    
}