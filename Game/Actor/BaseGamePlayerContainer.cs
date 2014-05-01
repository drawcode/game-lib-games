using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;
using Engine.Game;
using Engine.Game.Actor;
using Engine.Game.Controllers;
using Engine.Utility;

public class BaseGamePlayerContainer : GameObjectBehavior {

	//public GamePlayerController gamePlayerController;
	
	public virtual void Start() {
		//if(gamePlayerController) {
		//	gamePlayerController.uuid = UniqueUtil.Instance.currentUniqueId;
		//}
	}
}


