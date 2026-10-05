using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class CheckboxEvents : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UICheckbox currentObj;
#else
    Engine.UI.UIRef currentObj;
#endif
    public static string EVENT_ITEM_CLICK = "event-checkbox-item-click";
    public static string EVENT_ITEM_CHANGE = "event-checkbox-item-change";

    void Start() {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        currentObj = GetComponent<UICheckbox>();
#else
        // B10: the change half through the UI backend (a uGUI Toggle's onValueChanged under the
        // GameObject backend), raising the same broadcast NGUI's OnActivate SendMessage does.
        currentObj = Engine.UI.UIRef.Of(gameObject);
        UIUtil.SetToggleHandlerChange(currentObj, OnActivate);
#endif

        if (currentObj != null) {
            //currentObj.functionName = "OnActivate";
            //currentObj.eventReceiver = gameObject;
        }
    }

    void OnClick() {

        // See InputEvents: pointer identity now comes from the registered UI backend.
        int camIndex = UIUtil.GetPointerId(gameObject);

        Messenger<string, int>.Broadcast(CheckboxEvents.EVENT_ITEM_CLICK, transform.name, camIndex);
    }

    void OnActivate(bool selected) {
        //LogUtil.Log("CheckboxEvents:OnActivate: name: " + transform.name + " selected:" + selected);
        Messenger<string, bool>.Broadcast(CheckboxEvents.EVENT_ITEM_CHANGE, transform.name, selected);
    }
}
