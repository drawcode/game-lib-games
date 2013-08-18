using System;

using UnityEngine;

using Engine.Animation;
using Engine.Events;
using Engine.Utility;

using Engine.Game.Actor;

public class GameLevelItemObject : MonoBehaviour {  
	
	public Vector3 latestPosition;
	
	void Start() {
		latestPosition = transform.position;
	}
	
	void Update(){
		//if(GameDraggableEditor.Instance.grabbed != transform) {
		//	transform.position = latestPosition;
		//}
		//else {
			latestPosition = transform.position;
		//}
	}
	
}