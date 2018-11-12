using System;
using System.Collections;

using UnityEngine;
using Engine.Events;
using UnityEngine.UI;

public class InputEvents : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UIInput currentObj;
#else
    GameObject currentObj;
#endif
    public static string EVENT_ITEM_CLICK = "event-input-item-click";
    public static string EVENT_ITEM_CHANGE = "event-input-item-change";

    void Start() {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        currentObj = GetComponent<UIInput>();
#else
        if(currentObj.Has<Text>()) {
            currentObj = GetComponent<Text>().gameObject;
        }
#endif

        if(currentObj != null) {
            //currentObj.functionName = "OnActivate";
            //currentObj.eventReceiver = gameObject;
        }
    }

    void OnClick() {

        int camIndex = 0;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        camIndex = UICamera.currentTouchID;
#endif

        Messenger<string, int>.Broadcast(InputEvents.EVENT_ITEM_CLICK, transform.name, camIndex);

    }

    void OnActivate(string data) {
        LogUtil.Log("InputEvents:OnActivate: name: " + transform.name + " data:" + data);
        Messenger<string, string>.Broadcast(InputEvents.EVENT_ITEM_CHANGE, transform.name, data);
    }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    void OnInputChanged(UIInput data) {
        LogUtil.Log("InputEvents:OnInputChanged: name: " + transform.name + " data:" + data.text);
        Messenger<string, string>.Broadcast(InputEvents.EVENT_ITEM_CHANGE, transform.name, data.text);
    }
#else
    // TODO Unity UI
    void OnInputChanged(GameObject data) {
        LogUtil.Log("InputEvents:OnInputChanged: name: " + transform.name + " data:" + UIUtil.GetInputValue(data));
        Messenger<string, string>.Broadcast(InputEvents.EVENT_ITEM_CHANGE, transform.name, UIUtil.GetInputValue(data));
    }
#endif

    void OnSubmit(string data) {
        LogUtil.Log("InputEvents:OnSubmit: name: " + transform.name + " data:" + data);
        Messenger<string, string>.Broadcast(InputEvents.EVENT_ITEM_CHANGE, transform.name, data);
    }
}