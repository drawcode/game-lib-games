using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayer : GameCustomBase {
		
	void Start() {
		SetCustomColors();
	}
	
	void OnEnable() {
		Messenger.AddListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
	}
	
	void OnDisable() {
		Messenger.RemoveListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
	}
	
	void OnCustomizationColorsPlayerChangedHandler() {
		SetCustomColors(gameObject);
	}
}