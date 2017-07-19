#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine; 

using Engine.Events;

public enum UIColorType {
    colorForeground,
	colorBackground,
	colorForegroundFade,
	colorOverviewTitle,
	colorOverviewText
}

public class UIColorCustomType : GameDataObject {
    public static string colorForeground = "ui-light";
    public static string colorForegroundFade = "ui-light-fade";
    public static string colorBackground = "ui-dark";

    public static string colorOverviewTitle = "ui-overview-title";
    public static string colorOverviewText = "ui-overview-text";

    public UIColorCustomType() {
        this["colorForeground"] = UIColorCustomType.colorForeground;
        this["colorBackground"] = UIColorCustomType.colorBackground;
        this["colorForegroundFade"] = UIColorCustomType.colorForegroundFade;
        this["colorOverviewTitle"] = UIColorCustomType.colorOverviewTitle;
        this["colorOverviewText"] = UIColorCustomType.colorOverviewText;
    }
}

public class UIColorCustomTypeObject : UIColorObject {

    UIColorCustomType colorCustomTypes;
    public UIColorType colorType = UIColorType.colorForeground;
    string colorKey = UIColorCustomType.colorForeground;
    public float colorAlpha = 1f;

    public override void Awake() {

    }

    public override void Start() {
        Init();
    }

    public override void Init() {
        SyncColors();
    }

    void OnEnable() {
        Messenger.AddListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }

    void OnDisable() {
        Messenger.RemoveListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }

    void OnColorsUpdateHandler() {
        SyncColors();
    }

    public void SetColorByKey(string key, float alpha) {
        colorKey = key;
        colorAlpha = alpha;
        SyncColors();
    }

    public override void SyncColors() {

        string colorTypeString = colorType.ToString();

        if(colorCustomTypes == null) {
            colorCustomTypes = new UIColorCustomType();
        }

        if(colorCustomTypes.ContainsKey(colorTypeString)) {

            colorKey = colorCustomTypes.Get<string>(colorType.ToString());

            if(!string.IsNullOrEmpty(colorKey)) {

                AppColor appColor = AppColors.Instance.GetById(colorKey);

                if(appColor != null) {

                    Color colorTo = appColor.GetColor();
                    colorTo.a = colorAlpha;
                    UIColors.ColorTo(
                        gameObject, colorTo);

                    foreach(Transform t in gameObject.transform) {
                        UIColors.ColorTo(
                            t.gameObject, colorTo);
                    }
                }
            }
        }
    }
}