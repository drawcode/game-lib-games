using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayer : BaseGameCustom {
        
    public override void Start() {
        base.Start();
        //UpdatePlayer();
    }
    
    public override void OnEnable() {
        base.OnEnable();

        Messenger.AddListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
        
        Messenger.AddListener(GameCustomMessages.customCharacterDisplayChanged, OnCustomizationCharacterDisplayChangedHandler);
    }
    
    public override void OnDisable() {
        base.OnDisable();

        Messenger.RemoveListener(GameCustomMessages.customColorsPlayerChanged, OnCustomizationColorsPlayerChangedHandler);
        
        Messenger.RemoveListener(GameCustomMessages.customCharacterDisplayChanged, OnCustomizationCharacterDisplayChangedHandler);
    }
    
    void OnCustomizationColorsPlayerChangedHandler() {
        //// UpdatePlayer();
    }

    void OnCustomizationCharacterDisplayChangedHandler() {
        GameCustomController.UpdateProfileCharacterDisplay(gameObject);
    }
}