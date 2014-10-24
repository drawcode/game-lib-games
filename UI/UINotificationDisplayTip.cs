using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Engine.Events;

public enum UINotificationTipState {
    Showing,
    Hidden
}

public enum UINotificationTipType {
    Info,
    Achievement,
    Tip,
    Error,
    Point
}

public class UINotificationTipItem {
    public string code = "";
    public string title = "";
    public string description = "";
    public string score = "";
    public string icon = "";
    public bool immediate = false;
    public UINotificationTipType notificationType = UINotificationTipType.Info;
    
    public UINotificationTipItem() {
        
    }
}

public class UINotificationDisplayTip
    : GameObjectBehavior {
    
    public static UINotificationDisplayTip Instance;

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
    public UIButton achievementContinue;
    
    // Point
    public UILabel pointTitle;
    public UILabel pointDescription;
    public UILabel pointScore;
    public UIButton pointContinue;
    
    // Error
    public UILabel errorTitle;
    public UILabel errorDescription;
    public UILabel errorScore;
    public UIButton errorContinue;
    
    // Info
    public UILabel infoTitle;
    public UILabel infoDescription;
    public UILabel infoScore;
    public UIButton infoContinue;       
    
    // Tip
    public UILabel tipTitle;
    public UILabel tipDescription;
    public UILabel tipScore;
    public UIImageButton tipContinue;

    float positionYOpenInGame = 0;
    float positionYClosedInGame = -900;
    UINotificationTipItem currentItem;

    UINotificationTipState notificationState = UINotificationTipState.Hidden;
    public bool paused = false;
    Queue<UINotificationTipItem> notificationQueue = new Queue<UINotificationTipItem>();
    
    public bool IsHidden {
        get {
            if (notificationState == UINotificationTipState.Hidden)
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
        notificationState = UINotificationTipState.Hidden;
        HideDialog();
    }
    
    void OnEnable() {
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    void OnDisable() {
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    void OnButtonClickEventHandler(string buttonName) {
        
        if (UIUtil.IsButtonClicked(tipContinue, buttonName)) {
            HideDialog();           
        }
        
    }
    
    public void QueueNotification(
        string title, 
        string description, 
        double score, 
        UINotificationTipType notificationType) {      

        QueueNotification(title, description, score, notificationType, false);
    }
    
    public void QueueNotification(
        string title, 
        string description, 
        double score, 
        UINotificationTipType notificationType,
        bool immediate) { 
        
        UINotificationTipItem notification = new UINotificationTipItem();
        notification.title = title;
        notification.description = description;
        notification.notificationType = notificationType;
        notification.score = score.ToString("N0");
        notification.immediate = immediate;
        QueueNotification(notification);
    }
    
    public void QueueAchievement(string title, string description, double points) {             
        QueueNotification(title, description, points, UINotificationTipType.Achievement);       
    }
    
    public void QueuePoint(string title, string description, double points) {               
        QueueNotification(title, description, points, UINotificationTipType.Point);     
    }
    
    public void QueueInfo(string title, string description) {   
        if (notificationQueue.Count < 10) {
            QueueNotification(title, description, 0, UINotificationTipType.Info);       
        }
    }
    
    public void QueueError(string title, string description) {
        if (notificationQueue.Count < 10) {              
            QueueNotification(title, description, 0, UINotificationTipType.Error);  
        }
    }
    
    public void QueueTip(string title, string description) {    
        if (notificationQueue.Count < 10) {          
            QueueNotification(title, description, 0, UINotificationTipType.Tip);    
        }
    }

    public void QueueTip(string title, string description, bool immediate) {    
        if (notificationQueue.Count < 10) {          
            QueueNotification(title, description, 0, UINotificationTipType.Tip, immediate);    
        }
    }
    
    public void QueueNotification(UINotificationTipItem notificationItem) {
        notificationQueue.Enqueue(notificationItem);
        
        LogUtil.Log("Notification Queue(" 
            + notificationQueue.Count + ") " 
            + "Notification Added:title:" 
            + notificationItem.title
            + " notificationType:" 
            + notificationItem.notificationType
                    
        );  

        if(notificationItem.immediate) {
            HideDialog();
        }

        ProcessNotifications();
    }
    
    public void QueueAchievement(string achievementCode) {      
        
        string packCode = GamePacks.Current.code;
        string app_state = AppStates.Current.code;
        string app_content_state = AppContentStates.Current.code;
        
        LogUtil.Log("QueueAchievement:", 
                    " achievementCode:" + achievementCode
            + " packCode:" + packCode
            + " app_state:" + app_state
            + " app_content_state:" + app_content_state
        );
        
        string achievementBaseCode = achievementCode;
        achievementBaseCode = achievementBaseCode.Replace("-" + app_state, "");
        achievementBaseCode = achievementBaseCode.Replace("_" + GameAchievementCodes.formatAchievementCode(app_state), "");
        achievementBaseCode = achievementBaseCode.Replace("-" + app_content_state, "");
        achievementBaseCode = achievementBaseCode.Replace("_" + GameAchievementCodes.formatAchievementCode(app_content_state), "");
        achievementBaseCode = achievementBaseCode.Replace("-" + packCode, "");
        achievementBaseCode = achievementBaseCode.Replace("_" + GameAchievementCodes.formatAchievementCode(packCode), "");       
        
        LogUtil.Log("QueueAchievement2:",
                    " achievementCode:" + achievementCode
            + " achievementBaseCode:" + achievementBaseCode
        );      
        
        GameAchievement achievement 
            = GameAchievements.Instance.GetByCodeAndPack(
                achievementBaseCode, 
                packCode,
                app_content_state
        );
        
        
        if (achievement != null) {
            //achievement.description = GameAchievements.Instance.FormatAchievementTags(
            //  app_state,
            //  app_content_state, 
            //  achievement.description);
            //LogUtil.Log("Queueing Achievement display:" + achievement.display_name);
        }
        else {
            LogUtil.Log("Achievement not found:" + achievementCode);
        }
        
        if (achievement != null) {
            UINotificationTipItem item = new UINotificationTipItem();
            item.code = achievement.code;
            item.description = achievement.description;
            item.icon = "";
            item.notificationType = UINotificationTipType.Achievement;
            item.score = achievement.data.points.ToString();
            item.title = achievement.display_name;
            QueueNotification(item);
        }
        
        if (achievementCode == "achieve_test1") {
            
            UINotificationTipItem item = new UINotificationTipItem();
            item.code = achievementCode;
            item.description = "This is an achievement test, you did awesome!";
            item.icon = "";
            item.notificationType = UINotificationTipType.Achievement;
            item.score = 3.ToString();
            item.title = "First Achievement Tested";
            QueueNotification(item);
        }
    }
    
    public void ClearQueue() {
        if (notificationQueue != null) {
            if (notificationQueue.Count > 0) {
                notificationQueue.Clear();
            }
        }
    }
    
    public void HideAndClearQueue() {
        ClearQueue();
        HideDialog();
    }
    
    public void ToggleDialog() {
        if (notificationState == UINotificationTipState.Hidden) {
            // Show
            ShowDialog();
        }
        else {
            // Hide
            HideDialog();
        }
    }
    
    public UITweener FindTweener() {        
        UITweener twn = UnityObjectUtil.FindObject<UITweener>();
        if (twn != null) {
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
        
        Invoke("HideDialog", 3.0f);
        
        SetStateShowing();
        
        bool audioPlaySuccess = false;
        
        if (currentItem != null) {
            if (currentItem.notificationType == UINotificationTipType.Achievement) {
                audioPlaySuccess = true;
            }
            else if (currentItem.notificationType == UINotificationTipType.Error) {
                audioPlaySuccess = false;
                GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
            }
            else if (currentItem.notificationType == UINotificationTipType.Info) {
                audioPlaySuccess = false;
                GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
                GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_2);
            }
            else if (currentItem.notificationType == UINotificationTipType.Point) {
                audioPlaySuccess = true;
            }
            else if (currentItem.notificationType == UINotificationTipType.Tip) {
            }
        }
        
        if (audioPlaySuccess) {
            GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_achievement_2);
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_achievement_3);
        }
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
        
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            //achievementNumber++;
            //QueueAchievement("achieve_test1");
            //QueueAchievement("achieve_find_first");
            //QueueAchievement("Achievement here", "This is an achievement", 10);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            //achievementNumber++;
            //QueueError("Error Here", "This is an error, oh snap!");
        }       
        
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            //achievementNumber++;
            QueueInfo("Info Here", "This is an info, just an FYI!");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            //achievementNumber++;
            QueueTip("CONTROL TIPS", "SWIPE TO ROTATE | PINCH TO ZOOM | TAP TO ADVANCE");
            QueueTip("SPECIAL TIPS", "TAP the crank on the box to start the box.");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            //achievementNumber++;
            //QueuePoint("Point Here", "This is an point, do better!", 1);
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
        if (!Paused) {
            if (notificationQueue.Count > 0)
            if (notificationState == UINotificationTipState.Hidden)
                ProcessNextNotification();
        }
    }
    
    public void ShowNotificationContainerType(UINotificationTipType type) {
        
        if (type == UINotificationTipType.Achievement) {
            GameObjectHelper.ShowObject(notificationContainerAchievement);
        }
        else {  
            GameObjectHelper.HideObject(notificationContainerAchievement);
        }
        
        if (type == UINotificationTipType.Point) {
            GameObjectHelper.ShowObject(notificationContainerPoint);
        }
        else {  
            GameObjectHelper.HideObject(notificationContainerPoint);
        }
        
        if (type == UINotificationTipType.Error) {
            GameObjectHelper.ShowObject(notificationContainerError);
        }
        else {  
            GameObjectHelper.HideObject(notificationContainerError);
        }       
        
        if (type == UINotificationTipType.Tip) {
            GameObjectHelper.ShowObject(notificationContainerTip);
        }
        else {  
            GameObjectHelper.HideObject(notificationContainerTip);
        }       
        
        if (type == UINotificationTipType.Info) {
            GameObjectHelper.ShowObject(notificationContainerInfo);
        }
        else {  
            GameObjectHelper.HideObject(notificationContainerInfo);
        }   
    }
    
    public void ProcessNextNotification() {
        if (!Paused) {
            if (notificationQueue.Count > 0) {
                UINotificationTipItem notificationItem = notificationQueue.Dequeue();
                bool found = false;
                
                
                if (notificationItem.notificationType == UINotificationTipType.Achievement) {        
                    
                    ShowNotificationContainerType(notificationItem.notificationType);                   
                    UIUtil.SetLabelValue(achievementTitle, notificationItem.title);
                    UIUtil.SetLabelValue(achievementDescription, notificationItem.description);
                    UIUtil.SetLabelValue(achievementScore, notificationItem.score);
                    
                    found = true;
                }
                else if (notificationItem.notificationType == UINotificationTipType.Point) {     
                    
                    ShowNotificationContainerType(notificationItem.notificationType);                   
                    UIUtil.SetLabelValue(pointTitle, notificationItem.title);
                    UIUtil.SetLabelValue(pointDescription, notificationItem.description);
                    UIUtil.SetLabelValue(pointScore, notificationItem.score);
                    
                    found = true;
                }
                else if (notificationItem.notificationType == UINotificationTipType.Info) {      
                    
                    ShowNotificationContainerType(notificationItem.notificationType);                   
                    UIUtil.SetLabelValue(infoTitle, notificationItem.title);
                    UIUtil.SetLabelValue(infoDescription, notificationItem.description);
                    UIUtil.SetLabelValue(infoScore, notificationItem.score);
                    
                    
                    found = true;
                }
                else if (notificationItem.notificationType == UINotificationTipType.Tip) {       
                    
                    ShowNotificationContainerType(notificationItem.notificationType);                   
                    UIUtil.SetLabelValue(tipTitle, notificationItem.title);
                    UIUtil.SetLabelValue(tipDescription, notificationItem.description);
                    UIUtil.SetLabelValue(tipScore, notificationItem.score);
                    
                    found = true;
                }
                else if (notificationItem.notificationType == UINotificationTipType.Error) {     
                    
                    ShowNotificationContainerType(notificationItem.notificationType);                   
                    UIUtil.SetLabelValue(errorTitle, notificationItem.title);
                    UIUtil.SetLabelValue(errorDescription, notificationItem.description);
                    UIUtil.SetLabelValue(errorScore, notificationItem.score);
                    
                    found = true;
                }
                
                if (found) {
                    
                    LogUtil.Log("Notification Queue(" 
                        + notificationQueue.Count + ") " 
                        + "Notification Removed:title:" 
                        + notificationItem.title
                        + " notificationType:" 
                        + notificationItem.notificationType
                                
                    );  
                    
                    ShowDialog();
                }
                
                currentItem = notificationItem;
            }
        }
    }
    
    public void SetStateShowing() {
        notificationState = UINotificationTipState.Showing;
    }
    
    public void SetStateHidden() {
        notificationState = UINotificationTipState.Hidden;
    }
}
