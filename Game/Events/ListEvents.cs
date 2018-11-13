using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class ListEvents : GameObjectBehavior {

    public static string EVENT_ITEM_CLICK = "event-list-item-click";
    public static string EVENT_ITEM_SELECT = "event-list-item-select";
    public static string EVENT_ITEM_SELECT_CLICK = "event-list-item-select-click";

    void Start() {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        UIPopupList popupList = GetComponent<UIPopupList>();
#else

        //GameObject popupList = GetComponent<UIPopupList>();
#endif

        //if(popupList != null) {
        //    //popupList.functionName = "OnSelectionChange";
        //}
    }

    void OnClick() {

        int camIndex = 0;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        camIndex = UICamera.currentTouchID;
#endif

        Messenger<string, int>.Broadcast(ListEvents.EVENT_ITEM_CLICK, transform.name, camIndex);

    }

    void OnSelect(bool selected) {
        Messenger<string, string, bool>.Broadcast(ListEvents.EVENT_ITEM_SELECT, transform.name, "", selected);
    }

    void OnSelectionChange(string selectedName) {
        Messenger<string, string>.Broadcast(ListEvents.EVENT_ITEM_SELECT_CLICK, transform.name, selectedName);
    }
}