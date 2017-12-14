using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


// using Engine.Data.Json;
using Engine.Events;
using Engine.Game;
using Engine.Game.Actor;
using Engine.Game.Controllers;
using Engine.Utility;

public class BaseGamePlayerObjectType {

    public static string ball = "ball";
    
}

public class BaseGamePlayerObjectItem : GameObjectBehavior {
    
    public string uuid = "";
    public string prefabName = "game-player-object-ball";
    public Transform currentTarget;
    
    // --------------------------------------------------------------------
    // INIT
    
    public virtual void Awake() {
        
    }
    
    public virtual void Start() {
    }
    
    public virtual void OnEnable() {
        
    }
    
    public virtual void OnDisable() {
        
    }
    
    public virtual void Update() {
        
    }
}

