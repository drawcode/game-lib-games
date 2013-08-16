using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine;
using Engine.Events;
using Engine.Utility;

public class GameTouchInputAxis : MonoBehaviour {
	
	public Camera collisionCamera;
	public Transform pad;// = gameObject.transform.FindChild("Pad");
	public string axisName = "main";
	
	public Vector3 axisInput;
	public Vector3 padPos;
	
	void FindPad() {
		if(pad == null) {
			pad = gameObject.transform.FindChild("Pad");
		}
	}
	
	void PointHitTest(Vector3 point) {
		if(collisionCamera) {
			Ray screenRay = collisionCamera.ScreenPointToRay(point);
			RaycastHit hit;
			if(Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {	
				
				//Debug.Log("hit:" + hit.transform.gameObject.name);
				
				if(hit.transform.gameObject.name == gameObject.name) {		
					
					axisInput.x = (hit.textureCoord.x - .5f) * 2;
					axisInput.y = (hit.textureCoord.y - .5f) * 2;
																			
					Messenger<string, Vector3>.Broadcast("input-axis", "input-axis-" + axisName, axisInput);
					
					if(pad != null) {
						padPos = pad.localPosition;								
						padPos.x = -Mathf.Clamp(axisInput.x * 1.5f, -1.2f, 1.2f);
						padPos.z = -Mathf.Clamp(axisInput.y * 1.5f, -1.2f, 1.2f);
						padPos.y = 0f;
						pad.localPosition = padPos;
					}
				}
			}
			else {
				ResetPad();
			}
		}
	}
	
	void ResetPad() {
		axisInput.x = 0f;
		axisInput.y =  0f;
		
		Messenger<string, Vector3>.Broadcast("input-axis", "input-axis-" + axisName, axisInput);
		
		if(pad != null) {
			Vector3 padPos = pad.localPosition;
			padPos.x = 0;
			padPos.y = 0;
			padPos.z = 0;
			pad.localPosition = padPos;
		}
	}
	
	void Update() {
		if(Input.touchCount > 0) {
			foreach(Touch touch in Input.touches) {
				PointHitTest(touch.position);			
			}			
		}
		else if(Input.GetMouseButton(0)) {
			PointHitTest(Input.mousePosition);
		}
		else {
			ResetPad();
		}
	}
}

