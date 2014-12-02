using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGPlayerObject : UIGameRPGObject {

    public GamePlayerController gamePlayerController;

    public override void Start() {
        base.Start();

        if(gamePlayerController == null) {
            gamePlayerController = GameController.CurrentGamePlayerController;
        }
    }
}