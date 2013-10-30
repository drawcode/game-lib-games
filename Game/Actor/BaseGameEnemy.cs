using System;
using UnityEngine;
using Engine.Animation;
using Engine.Events;
using Engine.Utility;

public class BaseGameEnemy : GameActor {
	public override void Start() {
		Init();
	}
	
	public override void Init () {
		base.Init ();
	}
	
	public override void OnInputDown(InputTouchInfo touchInfo) {
		LogUtil.Log("OnInputDown GameEnemy");
	}
}

