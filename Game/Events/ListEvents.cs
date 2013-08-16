using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class ListEvents : MonoBehaviour {
	
	public static string EVENT_ITEM_CLICK = "event-list-item-click";
	public static string EVENT_ITEM_SELECT = "event-list-item-select";
	public static string EVENT_ITEM_SELECT_CLICK = "event-list-item-select-click";
	
	void Start() {
		UIPopupList popupList = GetComponent<UIPopupList>();
		if(popupList != null) {
		popupList.functionName = "OnSelectionChange";
		}
	}
	
	void OnClick() {
		Messenger<string,int>.Broadcast(ListEvents.EVENT_ITEM_CLICK, transform.name, UICamera.currentTouchID);
	}
	
	void OnSelect(bool selected) {
		Messenger<string, string, bool>.Broadcast(ListEvents.EVENT_ITEM_SELECT, transform.name, "", selected);
	}
	
	void OnSelectionChange(string selectedName) {
		Messenger<string, string>.Broadcast(ListEvents.EVENT_ITEM_SELECT_CLICK, transform.name, selectedName);
	}
}
