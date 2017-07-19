using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.UI;
using Engine.Utility;

public class GameUIScene : GameObjectBehavior {

    public UIButtonMeta buttonMeta = new UIButtonMeta();

    void Start() {
        Init();
    }

    public virtual void Init() {
        if(GameGlobal.Instance == null) {
            Context.Current.ApplicationLoadLevelByName("GameUISceneRoot");
        }

        Reset();
    }

    public virtual void Reset() {
        //SceneLoader.ResetSceneContext();
        buttonMeta = new UIButtonMeta();
    }

    public virtual void LateUpdate() {
        buttonMeta.ResetButtons();
    }
}