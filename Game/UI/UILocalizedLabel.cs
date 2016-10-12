//#define USE_UI_NGUI_2_7

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class UILocalizedLabel : GameObjectBehavior {

    public string gameLocalizationCode = "";

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelLocalized = null;
#else
    public Text labelLocalized = null;
#endif

    public void Start() {
        FindLabel();
        UpdateContent();
    }

    public void OnEnable() {

        Messenger<string>.AddListener(
            GameLocalizationMessages.gameLocalizationChanged,
            OnGameLocalizationChanged);
    }

    public void OnDisable() {

        Messenger<string>.RemoveListener(
            GameLocalizationMessages.gameLocalizationChanged,
            OnGameLocalizationChanged);
    }

    public void FindLabel() {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

        if(labelLocalized == null) {            
            labelLocalized = gameObject.Get<UILabel>();
        }
#else
        if (labelLocalized == null) {
            labelLocalized = gameObject.Get<Text>();
        }
#endif
    }

    public void SetContent(string content) {

        FindLabel();

        UIUtil.SetLabelValue(labelLocalized, content);

    }

    public string GetContent() {

        FindLabel();

        return UIUtil.GetLabelValue(labelLocalized);
    }

    public void OnGameLocalizationChanged(string localeTo) {
        UpdateContent();
    }

    public void UpdateContent() {

        if (string.IsNullOrEmpty(gameLocalizationCode)) {
            return;
        }

        // Get from code
        string content = Locos.GetString(gameLocalizationCode);

        if (string.IsNullOrEmpty(content)) {

            // try lookup from current content
            string currentContent = GetContent();
            string currentContentCode = Locos.GetCodeFromContent(currentContent);

            content = Locos.GetString(currentContentCode);
        }

        if (!string.IsNullOrEmpty(content)) {
            SetContent(content);
        }
    }
}