#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelBackgroundColored : MonoBehaviour  {

    public void Awake() {
    
    }
    
    public void Start() {

    }

    public void AnimateBackgroundColor(Color colorTo) {
        gameObject.ColorTo(colorTo);
    }
 
}
