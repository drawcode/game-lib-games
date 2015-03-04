using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.UI;
using Engine.Utility;

public class GameUISceneLoader : GameUIScene {
        
    void Awake() {  
        
    }
        
    void Start() {
        Init();
    }
    
    public override void Init() {
        base.Init();
        
        InitEvents();
        
    }
    
    void InitEvents() {
        
    }
}

