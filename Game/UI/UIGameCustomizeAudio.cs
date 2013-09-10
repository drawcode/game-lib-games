using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum UIGameCustomAudioState {
	STOPPED,
	PLAYING,
	RECORDING
}

public class UIGameCustomizeAudio : MonoBehaviour {
	/*
	public string playerName = "default";
	
	UIGameCustomAudioState currentPlayerState = UIGameCustomAudioState.STOPPED;
	
	int currentSelectedItem = 0;
	CustomPlayerAudio currentPlayerAudio;
	CustomPlayerAudioItem currentPlayerAudioItem;
	public string currentCustomAudioKey = CustomPlayerAudioKeys.audioBikeRevving;
	public string currentCustomAudioName = "Bike Revving";
	
	public List<string> currentCustomAudioKeys = new List<string>();
	
	public bool initialized = false;
	
	public UIButton buttonSave;
	public UIButton buttonRecord;
	public UIButton buttonPlay;
	public UIButton buttonStop;
	public UIButton buttonLeft;
	public UIButton buttonRight;
	
	//public UIRadioBtn radioUseCustom;
	//public UIRadioBtn radioUseDefault;
	
	public UILabel labelCurrentAudioName;
	public UILabel labelCurrentPlayerState;
		
	void Start() {
		
		InitSounds();		
		
		InitEvents();
						
		FillKeys();
		
		SelectItem(0);
				
		initialized = true;
	}
	
	public void audioRecorderDidFinish( string filePath ) {
		LogUtil.Log( "audioRecorderDidFinish event: " + filePath );
		// Playback is not supported in Unity of files from the web or docs directory yet so you
		// have to use the native audio player to play them
	}
	
	public void audioRecorderFailed( string error ){
		LogUtil.Log( "audioRecorderFailed event: " + error );
	}
	
	public void OnEnable() {
	
#if UNITY_IPHONE				
		AudioRecorderManager.audioRecorderDidFinish += audioRecorderDidFinish;
		AudioRecorderManager.audioRecorderFailed += audioRecorderFailed;
#elif UNITY_ANDROID
		//AudioRecorderAndroidManager. += audioRecorderDidFinish;
		//AudioRecorderAndroidManager.audioRecorderFailed += audioRecorderFailed;		
#else
#endif		
	}
	
	public void OnDisable() {
#if UNITY_IPHONE				
		AudioRecorderManager.audioRecorderDidFinish -= audioRecorderDidFinish;
		AudioRecorderManager.audioRecorderFailed -= audioRecorderFailed;
#elif UNITY_ANDROID
		//AudioRecorderAndroidManager.audioRecorderDidFinish -= audioRecorderDidFinish;
		//AudioRecorderAndroidManager.audioRecorderFailed -= audioRecorderFailed;		
#endif
	}
	
	void InitSounds() {

		currentPlayerAudio = GameProfiles.Current.GetCustomAudio();
		
		if(currentPlayerAudio == null) {
			currentPlayerAudio = new CustomPlayerAudio();
		}				
		currentPlayerAudioItem = new CustomPlayerAudioItem();
	}
	
	public void FillKeys() {		
		
		currentCustomAudioKeys.Clear();
		
		currentCustomAudioKeys.Add(CustomPlayerAudioKeys.audioBikeRevving);
		currentCustomAudioKeys.Add(CustomPlayerAudioKeys.audioBikeRacing);
		currentCustomAudioKeys.Add(CustomPlayerAudioKeys.audioBikeBoosting);
		currentCustomAudioKeys.Add(CustomPlayerAudioKeys.audioCrowdCheer);
		currentCustomAudioKeys.Add(CustomPlayerAudioKeys.audioCrowdJump);
		currentCustomAudioKeys.Add(CustomPlayerAudioKeys.audioCrowdBoo);
	}
	
	public void PrepareForRecording() {
		GameAudioRecorder.Instance.PrepareAudioFilename(GetFileName(currentCustomAudioKey));
	}
	
	public string GetFileName(string key) {
	 	return key + ".wav"; // all currently saved as wav for high quality and on SD/persistence so room to.
	}
	
	public void PlayCurrent() {
		try {
			if(currentPlayerState != UIGameCustomAudioState.PLAYING) {
				if(currentPlayerAudioItem != null
					&& initialized) {
					//StopCurrent();
					currentPlayerState = UIGameCustomAudioState.PLAYING;
					if(currentPlayerAudioItem.useCustom) {
						GameAudioRecorder.Instance.Play(GetFileName(currentCustomAudioKey));
						
						//GameAudio.PlayAudioClip(riderPosition, transform, source, loop, soundPlayingIncrement++, volume, panLevel);
						//buttonPlay.SetState(2);
						Invoke("StopCurrent", 5.5f);
						LogUtil.Log("PlayCurrent:" + currentCustomAudioKey);
					} 
					else {
						GameAudio.PlayDefaultEffect(currentCustomAudioKey);
						Invoke("StopCurrent", 5.5f);
						LogUtil.Log("PlayCurrent Default:" + currentCustomAudioKey);
					}
				}
				else {
					currentPlayerState = UIGameCustomAudioState.STOPPED;				
				}
			}
		}
		catch (Exception e) {
			LogUtil.Log("ERROR:" + e.Message + e.StackTrace + e.Source);
		}
		
		SetPlayerState();
	}
	
	public void RecordCurrent() {
		try {
			if(currentPlayerState != UIGameCustomAudioState.RECORDING) {
				if(currentPlayerAudioItem != null
					&& initialized) {
					
					GameAudioRecorder.Instance.ClearLoadedClips();
					
					//StopCurrent();
					currentPlayerState = UIGameCustomAudioState.RECORDING;
	
					// Always switch to and record for useCustom when pressed.
					PrepareForRecording();
					
					bool recorded = GameAudioRecorder.Instance.Record();
					
					bool isRecording = GameAudioRecorder.Instance.IsRecording();
					
					LogUtil.Log("recorded:" + recorded);
					LogUtil.Log("IsRecording:" + isRecording);
					
					currentPlayerAudioItem.useCustom = recorded;
					currentPlayerAudio.SetAudioItem(currentCustomAudioKey, currentPlayerAudioItem);
					SetCheckedStates();

					Invoke("StopCurrent", 5.5f);
					
				}
				else {
					currentPlayerState = UIGameCustomAudioState.STOPPED;				
				}
			}
		}
		catch (Exception e) {
			LogUtil.Log("ERROR:" + e.Message + e.StackTrace + e.Source);
		}
		SetPlayerState();
	}
	
	public void StopCurrent() {
		currentPlayerState = UIGameCustomAudioState.STOPPED;
		
		try {			
			if(currentPlayerAudioItem != null 
				&& initialized) {
				currentPlayerState = UIGameCustomAudioState.STOPPED;
				
				GameAudioRecorder.Instance.Stop(true);
			}
			else {
				currentPlayerState = UIGameCustomAudioState.STOPPED;				
			}
		}
		catch (Exception e) {
			LogUtil.Log("ERROR:" + e.Message + e.StackTrace + e.Source);
		}		
		SetPlayerState();
	}
	
	
	public void SelectItemNext() {
		if(currentPlayerState == UIGameCustomAudioState.STOPPED) {
			SelectItem(currentSelectedItem + 1);
		}
	}
	
	public void SelectItemPrevious() {
		if(currentPlayerState == UIGameCustomAudioState.STOPPED) {
			SelectItem(currentSelectedItem - 1);
		}
	}
	
	public void SelectItem(int current) {
		
		StopCurrent();
		
		if(currentCustomAudioKeys == null) {
			currentCustomAudioKeys = new List<string>();
		}
		
		if(currentCustomAudioKeys.Count == 0) {
			FillKeys();
		}
		
		LogUtil.Log("SelectItem:" + current);
		
		if(current < 0) {
			current = currentCustomAudioKeys.Count - 1;
		}
		else if(current > currentCustomAudioKeys.Count - 1) {
			current = 0;
		}
		else  {
		}	
		
		LogUtil.Log("SelectItem2:" + current);
		
		LogUtil.Log("SelectItem: currentCustomAudioKeys:" + currentCustomAudioKeys.Count);
		
		currentSelectedItem = current;
		currentCustomAudioKey = currentCustomAudioKeys[current];
		currentPlayerAudioItem = currentPlayerAudio.GetAudioItem(currentCustomAudioKey);
		
		if(currentCustomAudioKey == CustomPlayerAudioKeys.audioBikeRacing) {				
			currentCustomAudioName = "Bike Racing";
		}
		else if(currentCustomAudioKey == CustomPlayerAudioKeys.audioBikeBoosting) {				
			currentCustomAudioName = "Bike Jumping";
		}
		else if(currentCustomAudioKey == CustomPlayerAudioKeys.audioCrowdCheer) {				
			currentCustomAudioName = "Crowd Cheer";
		}
		else if(currentCustomAudioKey == CustomPlayerAudioKeys.audioCrowdJump) {				
			currentCustomAudioName = "Crowd Jump";
		}
		else if(currentCustomAudioKey == CustomPlayerAudioKeys.audioCrowdBoo) {				
			currentCustomAudioName = "Crowd Boo";
		}
		else {				
			currentCustomAudioName = "Bike Revving";
		}
		
		RenderData();
	}
	
	public void RenderData() {
		
		SetCheckedStates(radioUseCustom);
		SetCheckedStates(radioUseDefault);
		
		if(labelCurrentAudioName) {						
			labelCurrentAudioName.text = currentCustomAudioName;
		}
		
		SetPlayerState();
	}
	
	public void InitEvents() {		
		
		if(buttonSave != null) {
			buttonSave.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					currentPlayerAudio.SetAudioItem(currentCustomAudioKey, currentPlayerAudioItem);
					
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					// Validate the audio items that the files exists					
					foreach(KeyValuePair<string, CustomPlayerAudioItem> item in currentPlayerAudio.audioItems) {
						if(item.Value.useCustom) {
							// If use custom is set make sure file is there, else reset it
							if(!GameAudioRecorder.Instance.CheckIfSoundExistsByKey(item.Key)) {
								item.Value.useCustom = false;
							}
							
							GamePlayerProgress.Instance.SetAchievement(GameAchievements.ACHIEVE_UI_CUSTOM_AUDIO, true);
						}
					}
										
					GameProfiles.Current.SetCustomAudio(currentPlayerAudio);										
					GameState.Instance.SaveProfile();
				}
			});
		}
		
		if(buttonRight != null) {
			buttonRight.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					SelectItemNext();
				}
			});
		}
		
		if(buttonLeft != null) {
			buttonLeft.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					SelectItemPrevious();
				}
			});
		}
		
		if(buttonPlay != null) {
			buttonPlay.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					PlayCurrent();
				}
			});
		}
		
		if(buttonRecord != null) {
			buttonRecord.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					RecordCurrent();
				}
			});
		}		
		
		if(buttonStop != null) {
			buttonStop.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {							
					StopCurrent();
				}
			});
		}
		
		if(radioUseCustom != null) {
			radioUseCustom.SetInputDelegate(InputDelegate);
			radioUseCustom.SetValueChangedDelegate(ValueChangedDelegate);
			SetCheckedStates(radioUseCustom);
		}	
		
		if(radioUseDefault != null) {
			radioUseDefault.SetInputDelegate(InputDelegate);
			radioUseDefault.SetValueChangedDelegate(ValueChangedDelegate);
			SetCheckedStates(radioUseDefault);
		}	
		
	}
	
	void HandleCheckedStates(UIRadioBtn radio) {
		if(radio == radioUseCustom) {
			if(currentPlayerAudioItem != null) {
				currentPlayerAudioItem.useCustom = true;
				currentPlayerAudio.SetAudioItem(currentCustomAudioKey, currentPlayerAudioItem);
			}
		}
		else if(radio == radioUseDefault) {
			if(currentPlayerAudioItem != null) {
				currentPlayerAudioItem.useCustom = false; 
				currentPlayerAudio.SetAudioItem(currentCustomAudioKey, currentPlayerAudioItem);
			}
		}
			
		SetCheckedStates(radio);
	}
	
	void SetCheckedStates() {
		SetCheckedStates(radioUseCustom);
		SetCheckedStates(radioUseDefault);
	}

	
	void SetCheckedStates(UIRadioBtn radio) {
		if(radio != null) {
			
			bool selected = false;
			if(currentPlayerAudioItem != null) {
				selected = currentPlayerAudioItem.useCustom;
			}
			LogUtil.Log("SetCheckedStates selected useCustom:" + selected);
			if(radio == radioUseCustom) {				
				//StopCurrent();
				radio.SetState(Convert.ToInt32(!selected)); // flip to trick ezgui radio to checkbox
				
			}
			else if(radio == radioUseDefault) {
				//StopCurrent();
				radio.SetState(Convert.ToInt32(selected));
			}
		}
	}
	
	void ValueChangedDelegate(IUIObject obj) {
		if(obj.GetType() == typeof(UIRadioBtn)) {
			//UIRadioBtn radio = (UIRadioBtn)obj;			
			//HandleCheckedStates(radio);
		}
	}		
	
	void InputDelegate(ref POINTER_INFO ptr) {
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP) {
			if(ptr.targetObj.GetType() == typeof(UIRadioBtn)) {	
				
				GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);				
				UIRadioBtn radio = (UIRadioBtn)ptr.targetObj;								
				HandleCheckedStates(radio);										
			}
		}
	}
	
	public CustomPlayerAudio GetCurrentAudio() {	
		CustomPlayerAudio defaultAudio = new CustomPlayerAudio();		
		LogUtil.Log("defaultAudio:" + defaultAudio.ToString());		
		return defaultAudio;
	}
	
	public void SetPlayerState() {
	
		if(buttonStop) {
			buttonStop.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}
		if(buttonPlay) {			
			buttonPlay.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}
		if(buttonRecord) {			
			buttonRecord.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}				
		
		if(currentPlayerState == UIGameCustomAudioState.PLAYING) {
			if(buttonPlay) {			
				buttonPlay.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
				if(labelCurrentPlayerState) {
					labelCurrentPlayerState.text = "Playing " + currentCustomAudioName;
				}
			}
		}
		else if(currentPlayerState == UIGameCustomAudioState.RECORDING) {			
			if(buttonRecord) {			
				buttonRecord.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
				if(labelCurrentPlayerState) {
					labelCurrentPlayerState.text = "Recording " + currentCustomAudioName;
				}
			}
		}	
		else {
			if(buttonStop) {			
				buttonStop.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
				if(labelCurrentPlayerState) {
					labelCurrentPlayerState.text = "Stopped " + currentCustomAudioName;
				}
			}			
		}
	}
	
	public void LateUpdate() {
		
		SetPlayerState();
		
		//SetCheckedStates(radioColorBike);
		//SetCheckedStates(radioColorRider);
		//SetMaterialColors();
	}	
	
	*/
}

