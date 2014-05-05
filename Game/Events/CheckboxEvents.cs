using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class CheckboxEvents : GameObjectBehavior {
	
	UICheckbox currentObj;
	
	public static string EVENT_ITEM_CLICK = "event-checkbox-item-click";
	public static string EVENT_ITEM_CHANGE = "event-checkbox-item-change";
	
	void Start() {
		currentObj = GetComponent<UICheckbox>();
		if(currentObj != null) {
			currentObj.functionName = "OnActivate";
			currentObj.eventReceiver = gameObject;
		}
	}
	
	void OnClick() {
		Messenger<string,int>.Broadcast(CheckboxEvents.EVENT_ITEM_CLICK, transform.name, UICamera.currentTouchID);
	}
	
	void OnActivate(bool selected) {
		//LogUtil.Log("CheckboxEvents:OnActivate: name: " + transform.name + " selected:" + selected);
		Messenger<string, bool>.Broadcast(CheckboxEvents.EVENT_ITEM_CHANGE, transform.name, selected);
	}
}
