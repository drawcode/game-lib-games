#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIColorsMessages {
    public static string uiColorsUpdate = "ui-colors-update";
}

public class UIColors  {

    public static Color colorRed() {
        return AppColors.GetColor("ui-red");
        //ColorHelper.FromRGB(172, 22, 11);
    }

    public static Color colorLightBlue() {
        return AppColors.GetColor("ui-light-blue");
        //ColorHelper.FromRGB(0, 234, 255);
    }

    public static Color colorBlue() {
        return AppColors.GetColor("ui-blue");
        //ColorHelper.FromRGB(0,109,176);//ColorHelper.FromHex("006DB0");//.FromRGB(0, 234, 255);
    }

    public static Color colorOrange() {
        return AppColors.GetColor("ui-orange");
        //ColorHelper.FromRGB(255, 121, 0);
    }
    public static Color colorYellow() {            
        return AppColors.GetColor("ui-yellow");            
        //ColorHelper.FromRGB(229, 213, 2);
    }

    public static Color colorGreen() {            
        return AppColors.GetColor("ui-green");            
        //ColorHelper.FromRGB(98, 184, 0);
    }

    public static Color colorPurple() {            
        return AppColors.GetColor("ui-purple");            
        //ColorHelper.FromRGB(124, 12, 232);
    }

    public static Color colorDark() {            
        return AppColors.GetColor("ui-dark");            
        //ColorHelper.FromRGB(10, 10, 10);
    }

    public static Color colorLight() {            
        return AppColors.GetColor("ui-light");            
        //ColorHelper.FromRGB(200, 200, 200);
    }

    public static Color colorWhite() {            
        return AppColors.GetColor("ui-white");            
        //ColorHelper.FromRGB(255, 255, 255);
    }

    public static void UpdateColors() {
        Messenger.Broadcast(UIColorsMessages.uiColorsUpdate);
    }

    public static void AnimateColor(GameObject go, Color colorTo) {
        UITweenerUtil.ColorTo(go, UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, colorTo);
    }

    // extensions

    public static void ColorTo(GameObject inst, Color colorTo) {
        AnimateColor(inst, colorTo);
    }

    public static void ColorToGreen(GameObject inst) {
        AnimateColor(inst, colorGreen());
    }

    public static void ColorToRed(GameObject inst) {
        AnimateColor(inst, colorRed());
    }

    public static void ColorToBlue(GameObject inst) {
        AnimateColor(inst, colorBlue());
    }

    public static void ColorToOrange(GameObject inst) {
        AnimateColor(inst, colorOrange());
    }

    public static void ColorToYellow(GameObject inst) {
        AnimateColor(inst, colorYellow());
    }

    public static void ColorToPurple(GameObject inst) {
        AnimateColor(inst, colorPurple());
    }

    public static void ColorToDark(GameObject inst) {
        AnimateColor(inst, colorDark());
    }

    public static void ColorToLight(GameObject inst) {
        AnimateColor(inst, colorLight());
    }

    public static void UpdateColor(GameObject go, Color colorTo) {
        foreach(UIColorModeTypeObject uiColorModeTypeObject in go.GetList<UIColorModeTypeObject>()) {
            uiColorModeTypeObject.ColorTo(uiColorModeTypeObject.gameObject, colorTo);
        }
    }

}
