using System;

using UnityEngine;


using Engine.Events;
using Engine.Utility;

using Engine.Game.Actor;

public class GameLevelItemObject : GameObjectBehavior {  
	
	public Vector3 latestPosition;
	
	void Start() {
		latestPosition = transform.position;
	}
	
	void Update(){
		//if(GameDraggableEditor.grabbed != transform) {
		//	transform.position = latestPosition;
		//}
		//else {
			latestPosition = transform.position;
		//}
	}
	
}