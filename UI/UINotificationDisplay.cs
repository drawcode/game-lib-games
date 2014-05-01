using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Engine.Events;

public enum UINotificationState {
	Showing,
	Hidden
}

public enum UINotificationType {
	Info,
	Achievement,
	Tip,
	Error,
	Point
}

public class UINotificationItem {
	public string code = "";
	public string title = "";
	public string description = "";
	public string score = "";
	public string icon = "";
	public UINotificationType notificationType = UINotificationType.Info;
	
	public UINotificationItem() {
		
	}
}

public class UINotificationDisplay
	: GameObjectBehavior
{
	public GameObject notificationPanel;
	
	public GameObject notificationContainerAchievement;
	public GameObject notificationContainerPoint;
	public GameObject notificationContainerInfo;
	public GameObject notificationContainerTip;
	public GameObject notificationContainerError;
	
	// Achievement
	public UILabel achievementTitle;
	public UILabel achievementDescription;
	public UILabel achievementScore;
	public UIImageButton achievementIcon;
	
	// Point
	public UILabel pointTitle;
	public UILabel pointDescription;
	public UILabel pointScore;
	public UIImageButton pointContinue;
	
	// Error
	public UILabel errorTitle;
	public UILabel errorDescription;
	public UILabel errorScore;
	public UIImageButton errorContinue;
	
	// Info
	public UILabel infoTitle;
	public UILabel infoDescription;
	public UILabel infoScore;
	public UIImageButton infoContinue;
	
	// Tip
	public UILabel tipTitle;
	public UILabel tipDescription;
	public UILabel tipScore;
	public UIImageButton tipContinue;
        
    UINotificationItem notificationItem;
	
	float positionYOpenInGame = 0;
	float positionYClosedInGame = 900;
	
	public static UINotificationDisplay Instance;	
    
    public double currentScore = 0;
    public double lastScore = 0;
	
	UINotificationItem currentItem;
	
	UINotificationState notificationState = UINotificationState.Hidden;
	
	public bool paused = false;
	
    Queue<UINotificationItem> notificationQueue = new Queue<UINotificationItem>();
	
	public bool IsHidden {
		get {
			if(notificationState == UINotificationState.Hidden)
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
		notificationState = UINotificationState.Hidden;
		HideDialog();
	}
	
    void OnEnable() {
		Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    void OnDisable() {
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
		
    void OnButtonClickEventHandler(string buttonName) {
		
		if(UIUtil.IsButtonClicked(achievementIcon, buttonName)) {
			HideDialog();			
		}
		else if(UIUtil.IsButtonClicked(pointContinue, buttonName)) {
			HideDialog();						
		}
		else if(UIUtil.IsButtonClicked(errorContinue, buttonName)) {
			HideDialog();						
		}
		else if(UIUtil.IsButtonClicked(infoContinue, buttonName)) {
			HideDialog();			
		}
		else if(UIUtil.IsButtonClicked(tipContinue, buttonName)) {
			HideDialog();			
		}

    }	
	
	public void QueueNotification(
		string title, 
		string description, 
		double score, 
		UINotificationType notificationType) {		
		
		UINotificationItem notification = new UINotificationItem();
		notification.title = title;
		notification.description = description;
		notification.notificationType = notificationType;
		notification.score = score.ToString("N0");
		QueueNotification(notification);
	}
	
	
	public void QueueAchievement(string title, string description, double points) {				
		QueueNotification(title, description, points, UINotificationType.Achievement);		
	}
	
	public void QueuePoint(string title, string description, double points) {				
		QueueNotification(title, description, points, UINotificationType.Point);		
	}
		
	public void QueueInfo(string title, string description) {				
		QueueNotification(title, description, 0, UINotificationType.Info);		
	}
		
	public void QueueError(string title, string description) {				
		QueueNotification(title, description, 0, UINotificationType.Error);		
	}
		
	public void QueueTip(string title, string description) {				
		QueueNotification(title, description, 0, UINotificationType.Tip);		
	}
	
	public void QueueNotification(UINotificationItem notificationItem) {

        foreach(UINotificationItem item in notificationQueue) {
            if(item.title == notificationItem.title) {
                return;
            }
        }

        if(currentItem != null) {
            if(currentItem.title == notificationItem.title) {
                return;
            }
        }

		notificationQueue.Enqueue(notificationItem);
					
		LogUtil.Log("Notification Queue(" 
			+ notificationQueue.Count+ ") " 
			+ "Notification Added:title:" 
			+ notificationItem.title
			+ " notificationType:" 
			+ notificationItem.notificationType
			
		);	
		
		ProcessNotifications();
	}
	
	public void QueueAchievement(string achievementCode) {
		
		Debug.Log("Queueing Achievement:achievementCode:" + achievementCode);
		string packCode = GamePacks.Current.code;
		string appState = AppStates.Current.code;
		string appContentState = AppContentStates.Current.code;
		
		string achievementBaseCode = achievementCode;
		achievementBaseCode = achievementBaseCode.Replace("-" + appState,"");
		achievementBaseCode = achievementBaseCode.Replace("_" + GameAchievementCodes.formatAchievementCode(appState),"");
		achievementBaseCode = achievementBaseCode.Replace("-" + appContentState,"");
		achievementBaseCode = achievementBaseCode.Replace("_" + GameAchievementCodes.formatAchievementCode(appContentState),"");
		achievementBaseCode = achievementBaseCode.Replace("-" + packCode,"");
		achievementBaseCode = achievementBaseCode.Replace("_" + GameAchievementCodes.formatAchievementCode(packCode),"");		
		
		GameAchievement achievement 
			= GameAchievements.Instance.GetByCodeAndPack(
                achievementCode, 
				packCode//,
				//appContentState
			);
		
		
		if(achievement != null) {
			//achievement.description = GameAchievements.Instance.FormatAchievementTags(
			//	appState,
			//	appContentState, 
			//	achievement.description);
			//Debug.Log("Queueing Achievement display:" + achievement.display_name);
			
		}
		else {
			Debug.Log("Achievement not found:" + achievementCode);
		}
		
		if(achievement != null) {
			UINotificationItem item = new UINotificationItem();
			item.code = achievement.code;
			item.description = achievement.description;
			item.icon = "";
			item.notificationType = UINotificationType.Achievement;
			item.score = achievement.points.ToString();
			item.title = achievement.display_name;
			QueueNotification(item);
		}
		
		if(achievementCode == "achieve_test1"){
			
			UINotificationItem item = new UINotificationItem();
			item.code = achievementCode;
			item.description = "This is an achievement test, you did awesome!";
			item.icon = "";
			item.notificationType = UINotificationType.Achievement;
			item.score = 3.ToString();
			item.title = "First Achievement Tested";
			QueueNotification(item);
		}
	}
		
	public void ToggleDialog() {
		if(notificationState == UINotificationState.Hidden) {
			// Show
			ShowDialog();
		}
		else {
			// Hide
			HideDialog();
		}
	}
	
	public UITweener FindTweener() {		
		UITweener twn = ObjectUtil.FindObject<UITweener>();
		if(twn != null) {
			twn.method = UITweener.Method.EaseInOut;
			twn.style = UITweener.Style.Once;
		}
		return twn;
	}
	
	public void ShowDialog() {
				
		FindTweener();
		TweenPosition.Begin(
			notificationPanel, 
			.6f, 
			Vector3.zero.WithY(positionYOpenInGame));
				
		Invoke("HideDialog", 4.5f);

		SetStateShowing();
		
		bool audioPlaySuccess = false;
		
		if(currentItem != null) {
			if(currentItem.notificationType == UINotificationType.Achievement) {
				audioPlaySuccess = true;
			}
			else if(currentItem.notificationType == UINotificationType.Error) {
			}
			else if(currentItem.notificationType == UINotificationType.Info) {
				audioPlaySuccess = true;
			}
			else if(currentItem.notificationType == UINotificationType.Point) {
				audioPlaySuccess = true;
			}
			else if(currentItem.notificationType == UINotificationType.Tip) {
			}
		}
		
		if(audioPlaySuccess) {
			//GameAudio.PlayEffect(GameAudioEffects.audio_effect_achievement_1);
			//GameAudio.PlayEffect(GameAudioEffects.audio_effect_achievement_2);
			//GameAudio.PlayEffect(GameAudioEffects.audio_effect_achievement_3);
		}

        currentItem = null;
	}
	
	public void HideDialog() {
		
		FindTweener();
		TweenPosition.Begin(
			notificationPanel, 
			.2f, 
			Vector3.zero.WithY(positionYClosedInGame));
		
		Invoke("DisplayNextNotification", 1);
	}
	
	public void DisplayNextNotification() {
		SetStateHidden();
		ProcessNotifications();
	}
	
	public void Update() {

/*
		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			//achievementNumber++;
			QueueAchievement("achieve_test1");
			//QueueAchievement("achieve_find_first");
			QueueAchievement("Achievement here", "This is an achievement", 10);
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha2)) {
			//achievementNumber++;
			QueueError("Error Here", "This is an error, oh snap!");
		}		
		
		if(Input.GetKeyDown(KeyCode.Alpha3)) {
			//achievementNumber++;
			QueueInfo("Info Here", "This is an info, just an FYI!");
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha4)) {
			//achievementNumber++;
			QueueTip("Tip Here", "This is an tip, do better!");
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha5)) {
			//achievementNumber++;
			QueuePoint("Point Here", "This is an point, do better!", 1);
		}
  */

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
				if(notificationState == UINotificationState.Hidden)
					ProcessNextNotification();
		}
	}
		
	public void ShowNotificationContainerType(UINotificationType type) {
		
		if(type == UINotificationType.Achievement) {
			GameObjectHelper.ShowObject(notificationContainerAchievement);
		}
		else {	
			GameObjectHelper.HideObject(notificationContainerAchievement);
		}
		
		if(type == UINotificationType.Point) {
			GameObjectHelper.ShowObject(notificationContainerPoint);
		}
		else {	
			GameObjectHelper.HideObject(notificationContainerPoint);
		}
		
		if(type == UINotificationType.Error) {
			GameObjectHelper.ShowObject(notificationContainerError);
		}
		else {	
			GameObjectHelper.HideObject(notificationContainerError);
		}		
		
		if(type == UINotificationType.Tip) {
			GameObjectHelper.ShowObject(notificationContainerTip);
		}
		else {	
			GameObjectHelper.HideObject(notificationContainerTip);
		}		
		
		if(type == UINotificationType.Info) {
			GameObjectHelper.ShowObject(notificationContainerInfo);
		}
		else {	
			GameObjectHelper.HideObject(notificationContainerInfo);
		}	
	}
	
	public void ProcessNextNotification() {
		if(!Paused) {
			if(notificationQueue.Count > 0) {
				
                currentItem = notificationQueue.Dequeue();
				
                bool found = false;
				
				
                if(currentItem.notificationType == UINotificationType.Achievement) {		
					
                    ShowNotificationContainerType(currentItem.notificationType);					
                    UIUtil.SetLabelValue(achievementTitle, currentItem.title);
                    UIUtil.SetLabelValue(achievementDescription, currentItem.description);

                    if(GameConfigs.useCoinRewardsForAchievements) {
                        double score = Convert.ToDouble(currentItem.score);
                        score *= 50; // 50 coins per   
                        lastScore = 0;
                        currentScore = score;
                        currentItem.score = currentScore.ToString("N0");
                        GameProfileRPGs.Current.AddCurrency(currentScore);
                    }

                    UIUtil.SetLabelValue(achievementScore, "+" + currentItem.score);
					
					found = true;
				}
                else if(currentItem.notificationType == UINotificationType.Point) {		
					
                    ShowNotificationContainerType(currentItem.notificationType);					
                    UIUtil.SetLabelValue(pointTitle, currentItem.title);
                    UIUtil.SetLabelValue(pointDescription, currentItem.description);
                    UIUtil.SetLabelValue(pointScore, "+" + currentItem.score);
					
					found = true;
				}				
                else if(currentItem.notificationType == UINotificationType.Info) {		
					
                    ShowNotificationContainerType(currentItem.notificationType);					
                    UIUtil.SetLabelValue(infoTitle, currentItem.title);
                    UIUtil.SetLabelValue(infoDescription, currentItem.description);
					UIUtil.SetLabelValue(infoScore, "");

					
					found = true;
				}				
                else if(currentItem.notificationType == UINotificationType.Tip) {		
					
                    ShowNotificationContainerType(currentItem.notificationType);					
                    UIUtil.SetLabelValue(tipTitle, currentItem.title);
                    UIUtil.SetLabelValue(tipDescription, currentItem.description);
					UIUtil.SetLabelValue(tipScore, "");
					
					found = true;
				}				
                else if(currentItem.notificationType == UINotificationType.Error) {		
					
                    ShowNotificationContainerType(currentItem.notificationType);					
                    UIUtil.SetLabelValue(errorTitle, currentItem.title);
                    UIUtil.SetLabelValue(errorDescription, currentItem.description);
					UIUtil.SetLabelValue(errorScore, "");
					
					found = true;
				}
				
				if(found) {
					
					LogUtil.Log("Notification Queue(" 
						+ notificationQueue.Count+ ") " 
						+ "Notification Removed:title:" 
                                + currentItem.title
						+ " notificationType:" 
                                + currentItem.notificationType
						
					);
					
					ShowDialog();
				}
			}
		}
	}

	public void SetStateShowing() {
		notificationState = UINotificationState.Showing;
	}
	
	public void SetStateHidden() {
		notificationState = UINotificationState.Hidden;
	}
}

