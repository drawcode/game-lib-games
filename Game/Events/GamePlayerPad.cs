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
    public Color colorHighlight = Color.white;
    Color lastColorHighlight = Color.gray;

    public void Start() {

        lastColorHighlight = UIColors.colorDark;

        UpdateColor();
    }

    public void UpdateColor() {

        if (lastColorHighlight != colorHighlight) {

            lastColorHighlight = colorHighlight;

            if (containerHighlighted != null) {

                TweenUtil.ColorToObject(containerHighlighted, colorHighlight, 1f, 0f);

                //UITweenerUtil.ColorToHandler<UISprite>(
                //    containerHighlighted, 
                //    colorHighlight,
                //    1f, 0f);
            }
        }
    }

    public void Update() {

        UpdateColor();
    }
}
