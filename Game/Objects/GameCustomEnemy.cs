using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomEnemy : GameCustomBase {
	
	void Start() {
		//freezeRotation = false;
	}

    public override void OnEnable() {
        base.OnEnable();
    
        //Messenger.AddListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
    }
    
    public override void OnDisable() {
        base.OnDisable();
    
        //Messenger.RemoveListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
    }
}