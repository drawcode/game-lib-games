using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class InputEvents : GameObjectBehavior {
        
    UIInput currentObj;
    public static string EVENT_ITEM_CLICK = "event-input-item-click";
    public static string EVENT_ITEM_CHANGE = "event-input-item-change";
        
    void Start() {
        currentObj = GetComponent<UIInput>();
        if (currentObj != null) {
            //currentObj.functionName = "OnActivate";
            //currentObj.eventReceiver = gameObject;
        }
    }
        
    void OnClick() {
        Messenger<string,int>.Broadcast(InputEvents.EVENT_ITEM_CLICK, transform.name, UICamera.currentTouchID);
    }
        
    void OnActivate(string data) {
        LogUtil.Log("InputEvents:OnActivate: name: " + transform.name + " data:" + data);
        Messenger<string, string>.Broadcast(InputEvents.EVENT_ITEM_CHANGE, transform.name, data);
    }
}