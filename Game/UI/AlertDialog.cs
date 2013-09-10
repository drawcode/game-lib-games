using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Animation;
using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum AlertDialogType  {

		DIALOG_YES_NO,
		DIALOG_OK
}

public class AlertDialogMessages {
	public static string DIALOG_QUIT = "dg-quit";
	public static string DIALOG_CLEAR_DATA = "dg-clear-data";
	public static string DIALOG_STORE = "dg-store";
}

public class AlertDialogResultMessages {
	public static string DIALOG_RESULT_YES = "dg-result-yes";
	public static string DIALOG_RESULT_NO = "dg-result-no";
}

public class AlertDialog : MonoBehaviour
{	
	public static AlertDialog Instance;
	
	public GameObject dialogObject;
	public GameObject dialogBackgroundObject;
	
	public UIButton backgroundButton;
	
	public bool alertActive = false;
	
	public AlertDialogType alertDialogType = AlertDialogType.DIALOG_YES_NO;
	
	public UIButton buttonYes;	
	public UIButton buttonNo;
				
	public Vector3 buttonYesPosition;
	public Vector3 buttonYesCurrent;	
	public Vector3 buttonYesCurrentDown;		
			
	public Vector3 buttonNoPosition;
	public Vector3 buttonNoCurrent;	
	public Vector3 buttonNoCurrentDown;
	
	public UILabel labelMessage;
	public string dialogMessage = "Are you sure you want to quit and step down from Supa Supa Fame?";	
	public string currentDialogCode = AlertDialogMessages.DIALOG_QUIT;
	
	void Awake() {
		if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(this);
            return;
        }
		
        Instance = this;
		
		DontDestroyOnLoad(gameObject);
		
		LoadControls();
	}
	
	public static bool IsActive {
		get {
			if(AlertDialog.Instance.IsReady()) {
				return AlertDialog.Instance.alertActive;
			}
			return false;
		}
	}
	
	public void ShowAlertQuitDialog() {
		ShowAlert(AlertDialogMessages.DIALOG_QUIT, 
		          AlertDialogType.DIALOG_YES_NO, 
		          "Are you sure you want to quit and step down from Supa Supa Fame?");
	}
	
	public void ShowAlertClearData() {
		ShowAlert(AlertDialogMessages.DIALOG_CLEAR_DATA, 
		          AlertDialogType.DIALOG_YES_NO, 
		          "Are you sure you want to clear your series races?");
	}
	
	public void ShowAlert(string dialogCode, AlertDialogType dialogType, string message) {
		SetupAlert(dialogCode, dialogType, message);
		
		iTween.Stop(dialogObject);
		iTween.Stop(dialogBackgroundObject);
		
		Tweens.Instance.FadeToObject(dialogObject, 1f, .5f, 0f);
		Tweens.Instance.FadeToObject(dialogBackgroundObject, .8f, .5f, 0f);
		
		Invoke("StopTime", .6f);
		
		alertActive = true;
	}
	
	public void HideAlert() {
		
		iTween.Stop(dialogObject);
		iTween.Stop(dialogBackgroundObject);
		
		Tweens.Instance.FadeToObject(dialogObject, 0f, .5f, 0f);
		Tweens.Instance.FadeToObject(dialogBackgroundObject, 0f, .5f, 0f);
		
		Invoke("StartTime", .6f);
		
		alertActive = false;
	}
	
	public void SetupAlert(string dialogCode, AlertDialogType dialogType, string message) {
		if(labelMessage) {
			currentDialogCode = dialogCode;
			dialogMessage = message;
			alertDialogType = dialogType;
			labelMessage.text = dialogMessage;
		}
	}
	
	
	void LoadControls() {		
		
		Tweens.Instance.FadeToObject(dialogObject, 0f, 0f, 0f);
		Tweens.Instance.FadeToObject(dialogBackgroundObject, 0f, 0f, 0f);
		
		/*
		if(buttonYes != null) {
			buttonYesPosition = buttonYes.transform.localPosition;
			if(buttonYes.UILabel != null) {
				buttonYesCurrent = buttonYes.UILabel.gameObject.transform.localPosition;
				buttonYesCurrentDown = buttonYes.UILabel.gameObject.transform.localPosition;
				buttonYesCurrentDown.y = buttonYesCurrentDown.y - .05f;
			}
			buttonYes.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {
					StartTime();
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					Messenger<string>.Broadcast(currentDialogCode, 
					                            AlertDialogResultMessages.DIALOG_RESULT_YES);
				}
				else if(info.evt == POINTER_INFO.INPUT_EVENT.PRESS) {
					Vector3 temp = buttonYes.UILabel.gameObject.transform.localPosition;
					temp.y = buttonYesCurrentDown.y;
					buttonYes.UILabel.gameObject.transform.localPosition = temp;
				}
				else if(info.evt == POINTER_INFO.INPUT_EVENT.RELEASE
				        || info.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF) {
					Vector3 temp = buttonYes.UILabel.gameObject.transform.localPosition;
					temp.y = buttonYesCurrent.y;
					buttonYes.UILabel.gameObject.transform.localPosition = temp;
					buttonYes.transform.localPosition = buttonYesPosition;
					
				}
			});
		}
		
		if(buttonNo != null) {
			buttonNoPosition = buttonNo.transform.localPosition;
			if(buttonNo.UILabel != null) {
				buttonNoCurrent = buttonNo.UILabel.gameObject.transform.localPosition;
				buttonNoCurrentDown = buttonNo.UILabel.gameObject.transform.localPosition;
				buttonNoCurrentDown.y = buttonNoCurrentDown.y - .05f;
			}
			buttonNo.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {
					StartTime();
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					Messenger<string>.Broadcast(currentDialogCode, 
					                            AlertDialogResultMessages.DIALOG_RESULT_NO);					
				}
				else if(info.evt == POINTER_INFO.INPUT_EVENT.PRESS) {
					Vector3 temp = buttonNo.UILabel.gameObject.transform.localPosition;
					temp.y = buttonNoCurrentDown.y;
					buttonNo.UILabel.gameObject.transform.localPosition = temp;
				}
				else if(info.evt == POINTER_INFO.INPUT_EVENT.RELEASE
				        || info.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF) {
					Vector3 temp = buttonNo.UILabel.gameObject.transform.localPosition;
					temp.y = buttonNoCurrent.y;
					buttonNo.UILabel.gameObject.transform.localPosition = temp;
					buttonNo.transform.localPosition = buttonNoPosition;
					
				}
			});
		}
		*/
	}
	
	public void LateUpdate() {
		
		ResetButtons();
	}
	
	public void ResetButtons() {
				
		if(buttonYes) {
			buttonYes.transform.localPosition = buttonYesPosition;
		}
		if(buttonNo) {
			buttonNo.transform.localPosition = buttonNoPosition;
		}
	}
	
	public void StopTime() {
		SetTimeScale(0f);
		AudioListener.pause = true;
	}
	
	public void StartTime() {
		SetTimeScale(1f);
		AudioListener.pause = false;
	}
	
	public void SetTimeScale(float timeScale) {
		Time.timeScale = timeScale;
	}
	
}


