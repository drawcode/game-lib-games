using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomEnemy : GameCustomBase {
	
	void Start() {
		//freezeRotation = false;
	}
	
	void OnEnable() {
		Messenger.AddListener(GameCustomMessages.customColorsChanged, OnCustomizationColorsChangedHandler);
	}
	
	void OnDisable() {
		Messenger.RemoveListener(GameCustomMessages.customColorsChanged, OnCustomizationColorsChangedHandler);
	}
	
	void OnCustomizationColorsChangedHandler() {
		SetCustomColors(gameObject);
	}
}