#pragma warning disable CS0169
#pragma warning disable CS0618
#pragma warning disable CS0649
#pragma warning disable CS0414
#pragma warning disable CS0108
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif


using Engine.Events;

[Serializable]
public class ButtonEvenDataItem {
    public string key = "";
    public object data = "";
}

public class ButtonEvents : GameObjectBehavior {

    //public List<ButtonEvenDataItem> eventData = new List<ButtonEvenDataItem>();
    public Dictionary<string, string> eventData = new Dictionary<string, string>();

    public static string EVENT_BUTTON_CLICK = "event-button-click";
    public static string EVENT_BUTTON_CLICK_OBJECT = "event-button-click-object";

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    UIButton btn;
#else
    Button btn;
#endif

    void Start() {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

#else
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
#endif
    }

    void OnClick() {
        Debug.Log("OnClick:" + gameObject.name);

        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
        Messenger<GameObject>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, gameObject);
        //Messenger<string>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK, transform.name);
    }
}