using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using Engine.Events;

public class CheckboxEvents : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UICheckbox currentObj;
#else
    GameObject currentObj;
#endif
    public static string EVENT_ITEM_CLICK = "event-checkbox-item-click";
    public static string EVENT_ITEM_CHANGE = "event-checkbox-item-change";

    void Start() {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        currentObj = GetComponent<UICheckbox>();
#else
        if(currentObj.Has<Toggle>()) {
            currentObj = GetComponent<Toggle>().gameObject;
        }
#endif

        if (currentObj != null) {
            //currentObj.functionName = "OnActivate";
            //currentObj.eventReceiver = gameObject;
        }
    }

    void OnClick() {

        int camIndex = 0;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        camIndex = UICamera.currentTouchID;
#endif

        Messenger<string, int>.Broadcast(CheckboxEvents.EVENT_ITEM_CLICK, transform.name, camIndex);
    }

    void OnActivate(bool selected) {
        //LogUtil.Log("CheckboxEvents:OnActivate: name: " + transform.name + " selected:" + selected);
        Messenger<string, bool>.Broadcast(CheckboxEvents.EVENT_ITEM_CHANGE, transform.name, selected);
    }
}
