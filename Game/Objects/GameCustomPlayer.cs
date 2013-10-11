using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayer : GameCustomBase {
		
	void Start() {
		SetCustomColors();
	}
	
	public override void OnEnable() {
        base.OnEnable();

		Messenger.AddListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
	}
	
	public override void OnDisable() {
        base.OnDisable();

		Messenger.RemoveListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
	}
	
	void OnCustomizationColorsPlayerChangedHandler() {
        Debug.Log("OnCustomizationColorsPlayerChangedHandler");
		SetCustomColors(gameObject);
	}
}