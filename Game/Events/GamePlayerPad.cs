using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine;
using Engine.Events;
using Engine.Utility;

[ExecuteInEditMode]
public class GamePlayerPad : GameObjectBehavior {

    public GameObject containerHighlighted;

    public Color colorHighlight = UIColors.colorOrange();
    
    Color lastColorHighlight = UIColors.colorDark();

    public void Start() {
        UpdateColor();
    }

    public void UpdateColor() {

        if(lastColorHighlight != colorHighlight) {

            lastColorHighlight = colorHighlight;

            if(containerHighlighted != null) {
                UITweenerUtil.ColorToHandler<UISprite>(
                    containerHighlighted, 
                    colorHighlight,
                    1f, 0f);
            }
        }
    }

    public void Update() {

        UpdateColor();
    }
}
