using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIBroadcastRecordThumbnail : GameObjectBehavior {
    
    public GameObject placeholderReplayTextureObject;


    public void Awake() {
        
    }

    public  void Init() {
    
        if(placeholderReplayTextureObject != null) {
            placeholderReplayTextureObject.ResizePreservingAspectToScreen(150f, 120f);
        }   
    }
    
    public void Start() {
        Init();
    }

}

