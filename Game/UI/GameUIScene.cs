using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.UI;
using Engine.Utility;

public class GameUIScene : GameObjectBehavior {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIButtonMeta buttonMeta = new UIButtonMeta();
#endif

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
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        buttonMeta = new UIButtonMeta();
#endif
    }

    public virtual void LateUpdate() {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        buttonMeta.ResetButtons();
#endif
    }
}