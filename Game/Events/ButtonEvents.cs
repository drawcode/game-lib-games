using System;
using System.Collections;

using UnityEngine;
using Engine.Events;

public class ButtonEvents : MonoBehaviour {
	
	public static string EVENT_BUTTON_CLICK = "event-button-click";
	public static string EVENT_BUTTON_CLICK_OBJECT = "event-button-click-object";
	
	void OnClick() {
		GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
		Messenger<string>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK, transform.name);
		Messenger<GameObject>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, gameObject);
	}
}
