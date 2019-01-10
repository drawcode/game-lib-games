using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine;
using Engine.Networking;
#if USE_GAME_LIB_GAMEVERSES
using Gameverses;
#endif

public class BaseGameUIPanelBase : UIPanelBase {

    public override void Awake() {
        base.Awake();
    }

    public override void Start() {
        base.Start();
    }
}