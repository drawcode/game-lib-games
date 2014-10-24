using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Engine.Events;

[Serializable]
public class ButtonEvenDataItem {
    public string key = "";
    public object data = "";
}

public class ButtonEvents : GameObjectBehavior {

    //public List<ButtonEvenDataItem> eventData = new List<ButtonEvenDataItem>();
    public Dictionary<string,string> eventData = new Dictionary<string, string>();

    public static string EVENT_BUTTON_CLICK = "event-button-click";
    public static string EVENT_BUTTON_CLICK_OBJECT = "event-button-click-object";
    
    void OnClick() {
        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
        Messenger<GameObject>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, gameObject);
        //Messenger<string>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK, transform.name);
    }
}
