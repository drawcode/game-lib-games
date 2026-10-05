using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class InputEvents : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UIInput currentObj;
#else
    Engine.UI.UIRef currentObj;
#endif
    public static string EVENT_ITEM_CLICK = "event-input-item-click";
    public static string EVENT_ITEM_CHANGE = "event-input-item-change";

    void Start() {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        currentObj = GetComponent<UIInput>();
#else
        // B10: was a Has<Text>() probe on the still-null field (never matched). The change half
        // goes through UIInputChange, raising the same broadcast as OnSubmit; it no-ops until the
        // element's backend implements IUIInputChangeBackend (NGUIBackend deliberately does not).
        currentObj = Engine.UI.UIRef.Of(gameObject);
        Engine.UI.UIInputChange.SetInputHandlerChange(currentObj, OnSubmit);
#endif

        if (currentObj != null) {
            //currentObj.functionName = "OnActivate";
            //currentObj.eventReceiver = gameObject;
        }
    }

    void OnClick() {

        // Pointer identity comes from the registered UI backend, not from UICamera directly:
        // an NGUI widget resolves to UICamera.currentTouchID, a UI Toolkit element to its
        // pointer id. Same value as before under NGUI; no #if needed at the call site.
        int camIndex = UIUtil.GetPointerId(gameObject);

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