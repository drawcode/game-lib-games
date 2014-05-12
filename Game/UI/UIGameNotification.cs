using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Engine.Animation;
using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum GameNotificationState {
	Showing,
	Hidden
}

#region foldme

public enum GameNotificationType {
	Info,
	Achievement,
	Tip,
	Error
}

#endregion

public class GameNotificationItem {
	public string code = "";
	public string title = "";
	public string description = "";
	public string score = "";
	public string icon = "";
	public GameNotificationType notificationType = GameNotificationType.Info;
	
	public GameNotificationItem() {
	}
}

public class UIGameNotification
	: GameObjectBehavior
{
	public GameObject notificationPanel;
	
	public UILabel labelTitle;
	public UILabel labelDisplayName;
	public UILabel labelDescription;
	public UILabel labelScore;
	public GameObject iconObject;
	
	public UIButton icon;
	
	float positionYOpenInGame = 0f;
	float positionYClosedInGame = 333f;
	
	public static UIGameNotification Instance;	
	
	GameNotificationState notificationState = GameNotificationState.Hidden;
	
	public bool paused = false;
	
    Queue<GameNotificationItem> notificationQueue = new Queue<GameNotificationItem>();
	
	public bool IsHidden {
		get {
			if(notificationState == GameNotificationState.Hidden)
				return true;
				
			return false;
		}
	}
	
	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
		
		DontDestroyOnLoad(gameObject);
	}
	
	public void Start() {	
		notificationState = GameNotificationState.Hidden;
		HideDialog();
	}
	
	public void QueueAchievement(string code) {
	
		SetAchievementContent(code);
	}
				
	public bool SetAchievementContent(string achievementCode) {		
		GameAchievement achievementMeta = GameAchievements.Instance.GetById(achievementCode);
		
		if(achievementMeta == null) {
			string tempCode = achievementCode.Replace("_" + GamePacks.Current.code, "");
			achievementMeta = GameAchievements.Instance.GetById(tempCode);
		}
		
		if(achievementMeta != null) {			
			
			GameNotificationItem item = new GameNotificationItem();
			item.title = achievementMeta.display_name;
			item.description = FormatUtil.GetStringTrimmedWithBreaks(achievementMeta.description, 40);
			item.score = "+" + achievementMeta.data.points.ToString();
			item.notificationType = GameNotificationType.Achievement;
			QueueNotification(item);
			
			return true;
		}
		else {
			LogUtil.Log("SetAchievmentContent:: null achievementMeta: " + achievementCode);
			return false;
		}
	}
	
	public void QueueNotification(string title, string description, GameNotificationType notificationType) {		
		GameNotificationItem notification = new GameNotificationItem();
		notification.title = title;
		notification.description = description;
		notification.notificationType = notificationType;
		UIGameNotification.Instance.QueueNotification(notification);
	}
	
	public void QueueNotification(GameNotificationItem notification) {
		notificationQueue.Enqueue(notification);
		LogUtil.Log("Notification Queue(" + notificationQueue.Count+ ") Notification Added :" + notification.title);
		ProcessNotifications();		
		GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
	}
		
	public void ToggleDialog() {
		if(notificationState == GameNotificationState.Hidden) {
			// Show
			ShowDialog();
		}
		else {
			// Hide
			HideDialog();
		}
	}
	
	public void ShowDialog() {
		float openPos = positionYOpenInGame;
		
		LogUtil.Log("ShowDialog:" + openPos);
		
		Vector3 temp = notificationPanel.transform.position;
		temp.y = openPos;

        UITweenerUtil.MoveTo(notificationPanel, UITweener.Method.EaseIn, UITweener.Style.Once, .5f, 0f, temp);
		
		Invoke("HideDialog", 4f);

		SetStateShowing();
	}
	
	public void HideDialog() {
		if(notificationPanel == null) {
			return;
		}
		
		float closedPos = positionYClosedInGame;
		
		LogUtil.Log("HideDialog:" + closedPos);		
		
		Vector3 temp = notificationPanel.transform.position;
		temp.y = closedPos;
        
        UITweenerUtil.MoveTo(notificationPanel, UITweener.Method.EaseIn, UITweener.Style.Once, .6f, .9f, temp);
		
		Invoke("DisplayNextNotification", 1);
	}
	
	public void DisplayNextNotification() {
		SetStateHidden();
		ProcessNotifications();
	}
	
	public void Update() {
		
		if(Application.isEditor && Input.GetKeyDown(KeyCode.Space)) {
			//achievementNumber++;
			QueueAchievement("achieve_win_3");
			QueueAchievement("SOME CODE NOT FOUND");
			QueueAchievement("achieve_win_1");
		}
		
	}
	
	public bool Paused {
		get {
			return false;
		}
		set {
			paused = value;
		}
	}
	
	public void ProcessNotifications() {
		if(!Paused) {
			if(notificationQueue.Count > 0)
				if(notificationState == GameNotificationState.Hidden)
					ProcessNextNotification();
		}
	}
	
	public void ProcessNextNotification() {
		if(Paused) {
			return;
		}
				
		if(notificationQueue.Count > 0) {
			GameNotificationItem notificationItem = notificationQueue.Dequeue();
			
			UIUtil.SetLabelValue(labelDisplayName, notificationItem.title);
			UIUtil.SetLabelValue(labelDescription, notificationItem.description);
			UIUtil.SetLabelValue(labelScore, notificationItem.score);
			
			if(notificationItem.notificationType == GameNotificationType.Achievement) {				
				UIUtil.SetLabelValue(labelTitle, "ACHIEVEMENT");
				UIUtil.ShowLabel(labelScore);
				if(iconObject != null) {
					iconObject.Show();
				}
			}
			else if(notificationItem.notificationType == GameNotificationType.Info) {				
				UIUtil.SetLabelValue(labelTitle, "INFORMATION");
				UIUtil.HideLabel(labelScore);
				if(iconObject != null) {
					iconObject.Hide();
				}
			}
			else if(notificationItem.notificationType == GameNotificationType.Tip) {				
				UIUtil.SetLabelValue(labelTitle, "TIP");
				UIUtil.HideLabel(labelScore);
				if(iconObject != null) {
					iconObject.Hide();
				}
			}
			else if(notificationItem.notificationType == GameNotificationType.Error) {				
				UIUtil.SetLabelValue(labelTitle, "ERROR");
				UIUtil.HideLabel(labelScore);
				if(iconObject != null) {
					iconObject.Hide();
				}
			}
			
			LogUtil.Log("Notification Queue(" + notificationQueue.Count+ ") Notification Removed :" + notificationItem.title);				

			ShowDialog();
		}
	}
	
	/*
	public void EnablePanelRender(bool enabled) {
		//achievementPanel.SetActive(enabled);
	}	
	
	public bool SetAchievementContent(string achievementCode) {		
		GameAchievement achievementMeta = GameAchievements.Instance.GetById(achievementCode);
		
		if(achievementMeta != null) {
			textAchievement.Text = achievementMeta.name;
			textAchievementDescription.Text = FormatUtil.GetStringTrimmedWithBreaks(achievementMeta.description, 40);
			labelScore.Text = "+" + achievementMeta.points.ToString();
			return true;
		}
		else {
			LogUtil.Log("SetAchievmentContent:: null achievementMeta: " + achievementCode);
			return false;
		}
	}
	
	public bool IsIgnoredAchievementDisplay(string code) {
		if(code == GameAchievements.ACHIEVE_BOOST
			|| code == GameAchievements.ACHIEVE_FIRST_RACE
			|| code == GameAchievements.ACHIEVE_HITS_1
			|| code == GameAchievements.ACHIEVE_LOSS
			|| code == GameAchievements.ACHIEVE_PASSED_5
		) {
			return true;
		}
		return false;
	}
	
	public void Reset() {
		currentFastestTime = GetCurrentFastestLap();
		currentFastestTimeString = GetCurrentFastestLapFormatted();
		currentFastestRace = GetCurrentFastestRace();
		currentFastestRaceString = GetCurrentFastestRaceFormatted();
	}
	
	public double GetCurrentFastestLap() {
		string statCodeLevel = GameStatistics.STAT_FASTEST_LAP + "-" + GameLevels.Current.code;
		currentFastestTime = GameProfiles.Current.GetStatisticValue(statCodeLevel);
		return currentFastestTime;
	}
	
	public string GetCurrentFastestLapFormatted() {
		currentFastestTimeString = FormatUtil.GetFormattedTimeMinutesSecondsMs(currentFastestTime);
		return currentFastestTimeString;
	}
	
	public double GetCurrentFastestRace() {
		string statCodeLevel = GameStatistics.STAT_FASTEST_RACE + "-" + GameLevels.Current.code;
		currentFastestRace = GameProfiles.Current.GetStatisticValue(statCodeLevel);
		return currentFastestRace;
	}
	
	public string GetCurrentFastestRaceFormatted() {
		currentFastestRaceString = FormatUtil.GetFormattedTimeMinutesSecondsMs(currentFastestRace);
		return currentFastestRaceString;
	}
	
	public void CheckFastestLap(double lapToCheck) {
		if(currentFastestTime == 0.0) {
			currentFastestTime = GetCurrentFastestLap();
		}
		if((lapToCheck < currentFastestTime && lapToCheck > 0.0)
		   || currentFastestTime == 0.0) {
			currentFastestTime = lapToCheck;
			GetCurrentFastestLapFormatted();
			string statCodeLevel = GameStatistics.STAT_FASTEST_LAP + "$" + currentFastestTimeString;
			QueueAchievement(statCodeLevel);
		}
	}
	
	public void CheckFastestRace(double raceToCheck) {
		if(currentFastestRace == 0.0) {
			currentFastestRace = GetCurrentFastestRace();
		}
		if((raceToCheck < currentFastestRace && raceToCheck > 0.0)
		   || currentFastestRace == 0.0) {
			currentFastestRace = raceToCheck;
			GetCurrentFastestRaceFormatted();
			string statCodeLevel = GameStatistics.STAT_FASTEST_RACE + "$" + currentFastestRaceString;
			QueueAchievement(statCodeLevel);
		}
	}
	
	public bool SetAchievementContentProduct(string title, string message, string score) {
		textAchievement.Text = title;
		textAchievementDescription.Text = message;
		labelScore.Text = score;
		return true;
	}
	
	public bool SetAchievementContentFastestLap(string fastestTime) 
	{
		textAchievement.Text = "Personal Best Lap!";
		textAchievementDescription.Text = "" + GameLevels.Current.display_name + " fastest lap has been beaten!";
		labelScore.Text = fastestTime;
		return true;
	}
	
	public bool SetAchievementContentFastestRace(string fastestRace) {
		textAchievement.Text = "Personal Best Race!";
		textAchievementDescription.Text = "" + GameLevels.Current.display_name + " fastest race has been beaten!";
		labelScore.Text = fastestRace;
		return true;
	}
	
	public void QueueAchievement(string achievementCode) {	
		if(!IsIgnoredAchievementDisplay(achievementCode)) {
			achievementQueue.Enqueue(achievementCode);
			LogUtil.Log("Achievement Queue(" + achievementQueue.Count+ ") Achievement Added :" + achievementCode);
			ProcessAchievements();		
			GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
		}
	}
	*/
	
	public void SetStateShowing() {
		notificationState = GameNotificationState.Showing;
	}
	
	public void SetStateHidden() {
		notificationState = GameNotificationState.Hidden;
	}
}

