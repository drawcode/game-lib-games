using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeObject : GameObjectBehavior {
    
    public string currentCode = "default";
    public GameObject currentObject;

    public virtual void OnEnable() {
        
        Messenger<string>.AddListener(
            ButtonEvents.EVENT_BUTTON_CLICK, 
            OnButtonClickEventHandler);
    }
    
    public virtual void OnDisable() {
        
        Messenger<string>.RemoveListener(
            ButtonEvents.EVENT_BUTTON_CLICK, 
            OnButtonClickEventHandler);
    }

    public virtual void Start() {

    }       
    
    public virtual void OnButtonClickEventHandler(string buttonName) {

    }

    public virtual void Load() {

    }
    
    public virtual void Update() {
       
    }
}
