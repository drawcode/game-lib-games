#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public static class UIColors  {

    public static Color colorRed = new Color32(255, 255, 255, 1);
    public static Color colorBlue = new Color32(0, 234, 255, 1);
    public static Color colorOrange = new Color32(255, 121, 0, 1);
    public static Color colorYellow = new Color32(229, 213, 2, 1);
    public static Color colorGreen = new Color32(98, 184, 0, 1);
    public static Color colorPurple = new Color32(255, 255, 255, 1);
    public static Color colorDark = new Color32(200, 200, 200, 1);
    public static Color colorLight = new Color32(10, 10, 10, 1);

    public static void Awake() {
    
    }

    public static void Start() {

    }

    public static void AnimateBackgroundColor(GameObject go, Color colorTo) {
        UITweenerUtil.ColorTo(go, UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, colorTo);
    }

    // extensions

    public static void ColorTo(this GameObject inst, Color colorTo) {
        AnimateBackgroundColor(inst, colorTo);
    }

    public static void ColorToRed(this GameObject inst) {
        AnimateBackgroundColor(inst, colorRed);
    }

    public static void ColorToBlue(this GameObject inst) {
        AnimateBackgroundColor(inst, colorBlue);
    }

    public static void ColorToOrange(this GameObject inst) {
        AnimateBackgroundColor(inst, colorOrange);
    }

    public static void ColorToYellow(this GameObject inst) {
        AnimateBackgroundColor(inst, colorYellow);
    }

    public static void ColorToPurple(this GameObject inst) {
        AnimateBackgroundColor(inst, colorPurple);
    }

    public static void ColorToDark(this GameObject inst) {
        AnimateBackgroundColor(inst, colorDark);
    }

    public static void ColorToLight(this GameObject inst) {
        AnimateBackgroundColor(inst, colorLight);
    }
 
}
