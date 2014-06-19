#define ANALYTICS_GAMEANALYTICS
//#define ANALYTICS_GOOGLE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum AnalyticsNetworkType {
    GameAnalytics,
    Google
}

public class AnalyticMessages {
    public static string analyticsStartApp = "analytics-start-app";
    public static string analyticsEvent = "analytics-event";
    public static string analyticsEventSceneChange = "analytics-event-scene-change";
    public static string analyticsEventLevelStarted = "analytics-event-level-started";
    public static string analyticsEventLevelResults = "analytics-event-level-results";
    public static string analyticsEventLevelQuit = "analytics-event-level-quit";
    public static string analyticsQuitApp = "analytics-quit-app";
}

public class AnalyticsNetworksMessages {
    
    public static string analyticsTrackScene = "analytics-track-scene";
}

public class AnalyticsNetworks : GameObjectBehavior {

    #if ANALYTICS_GAMEANALYTICS
    public GA_SystemTracker analyticsGameAnalytics;
    #endif
    
    public static bool analyticsNetworksEnabled = AppConfigs.analyticsNetworkEnabled;
    public static bool analyticsNetworksTestingEnabled = AppConfigs.analyticsNetworkTestingEnabled;
    
    // Only one AnalyticsNetworks can exist. We use a singleton pattern to enforce this.
    private static AnalyticsNetworks _instance = null;
    
    public static AnalyticsNetworks Instance {
        get {
            if (!_instance) {
                
                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(AnalyticsNetworks)) as AnalyticsNetworks;
                
                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_AnalyticsNetworks");
                    _instance = obj.AddComponent<AnalyticsNetworks>();
                }
            }
            
            return _instance;
        }
    }
    
    void Start() {
        Init();
    }
    
    void OnEnable() {
        
        
        #if ANALYTICS_GAMEANALYTICS
        // ------------
        // GAME ANALYTICS
        
        //Everyplay.SharedInstance.RecordingStarted += everyplayRecordingStartedDelegate;
        //Everyplay.SharedInstance.RecordingStopped += everyplayRecordingStoppedDelegate;
        //Everyplay.SharedInstance.ThumbnailReadyAtFilePath += everyplayThumbnailReadyAtFilePathDelegate;
        
        #endif
        
        
    }

    void OnDisable() {
        
        
        #if ANALYTICS_GAMEANALYTICS
        // ------------
        // GAME ANALYTICS

        GA.SettingsGA.AllowRoaming = true;

        //Everyplay.SharedInstance.ThumbnailReadyAtFilePath -= everyplayThumbnailReadyAtFilePathDelegate;
        //Everyplay.SharedInstance.RecordingStarted -= everyplayRecordingStartedDelegate;
        //Everyplay.SharedInstance.RecordingStopped -= everyplayRecordingStoppedDelegate;
        
        #endif
        
    }
    
    public void Init() {  
        
        #if ANALYTICS_GAMEANALYTICS
        Invoke("gameAnalyticsInit", 1);
        #endif       
    }
    
    #if ANALYTICS_GAMEANALYTICS
    // ----------------------------------------------------------------------
    // GA - http://support.gameanalytics.com/hc/en-us/articles/200841396-Tips#customArea
    
    public void gameAnalyticsInit() {
        
        if(analyticsGameAnalytics == null) {
            
            analyticsGameAnalytics = FindObjectOfType(typeof(GA_SystemTracker)) as GA_SystemTracker;
            
            if (!analyticsGameAnalytics) {
                var obj = new GameObject("_analyticsGameAnalytics");
                analyticsGameAnalytics = obj.AddComponent<GA_SystemTracker>();
                DontDestroyOnLoad(analyticsGameAnalytics);
            }
        }

        //analyticsEveryplay.clientId = AppConfigs.analyticsEveryplayClientId;
        //analyticsEveryplay.clientSecret = AppConfigs.analyticsEveryplayClientSecret;
        //analyticsEveryplay.redirectURI = AppConfigs.analyticsEveryplayAuthUrl;
        
    }
        
    #endif
    
    
    // ----------------------------------------------------------------------
    
    // GENERIC CALLS
    
    
    // ----------------------------------------------------------------------
    
    // analytics
    
    // IS SUPPORTED
    
    public static bool IsSupported() {
        if (Instance != null) {
            return Instance.isSupported();
        }
        return false;
    }
    
    public bool isSupported() {

        #if ANALYTICS_GAMEANALYTICS
        return true;
        #else

        return false;
        #endif
    }
        
    // LOG AREA 
    
    public static void ChangeArea(string areaName) {
        if (Instance != null) {
            Instance.changeArea(areaName);
        }
    }
    
    public void changeArea(string areaName) {
        
        #if ANALYTICS_GAMEANALYTICS
        GA.SettingsGA.SetCustomArea(areaName);
        #endif
    }

    
    // LOG AREA 
    
    public static void Log(object data) {
        if (Instance != null) {
            Instance.log(data);
        }
    }
    
    public void log(object data) {
        
        #if ANALYTICS_GAMEANALYTICS
        GA.Log(data);
        #endif
    }
    
    // CHANGE USER
    
    public static void ChangeUser(string userId) {
        if (Instance != null) {
            Instance.changeUser(userId);
        }
    }
    
    public void changeUser(string userId) {
        
        #if ANALYTICS_GAMEANALYTICS
        GA.SettingsGA.SetCustomUserID(userId);
        #endif
    }

    // LOG EVENTS

    public static void LogEvent(string eventName, params object[] args) {
        if (Instance != null) {
            Instance.logEvent(eventName, args);
        }
    }
    
    public void logEvent(string eventName, params object[] args) {
      
        #if ANALYTICS_GAMEANALYTICS

        StringBuilder sb = new StringBuilder();
        sb.Append("GAME:");
        sb.Append(eventName);
        sb.Append(" :: ");

        foreach(object o in args) {
            sb.Append(o);
            sb.Append(" :: ");
        }

        GA.API.Design.NewEvent(sb.ToString(), 1);
        #endif
    }

    // LOG EVENTS

    // SCENE CHANGE
    
    public static void LogEventSceneChange(string val, string title) {
        if (Instance != null) {
            Instance.logEventSceneChange(val, title);
        }
    }
    
    public void logEventSceneChange(string val, string title) {

        AnalyticsNetworks.ChangeArea(val);

        logEvent(AnalyticMessages.analyticsEventSceneChange, val, title);
    }

    // LEVEL START
    
    public static void LogEventLevelStart(string val, string title) {
        if (Instance != null) {
            Instance.logEventLevelStart(val, title);
        }
    }
    
    public void logEventLevelStart(string val, string title) {
        
        logEvent(AnalyticMessages.analyticsEventLevelStarted, val, title);
    }
    
    // LEVEL RESULTS
    
    public static void LogEventLevelResults(string val, string title) {
        if (Instance != null) {
            Instance.logEventLevelResults(val, title);
        }
    }
    
    public void logEventLevelResults(string val, string title) {
        
        logEvent(AnalyticMessages.analyticsEventLevelResults, val, title);
    }
    
    // LEVEL QUIT
    
    public static void LogEventLevelQuit(string val, string title) {
        if (Instance != null) {
            Instance.logEventLevelQuit(val, title);
        }
    }
    
    public void logEventLevelQuit(string val, string title) {
        
        logEvent(AnalyticMessages.analyticsEventLevelQuit, val, title);
    }

    // LEVEL EVENT GENERIC DATA
    
    public static void LogEvent(string key, object  val) {
        if (Instance != null) {
            Instance.logEvent(key, val);
        }
    }
    
    public void logEvent(string key, object  val) {
        
        logEvent(AnalyticMessages.analyticsEvent, key, val);
    }

    
    // FLUSH/SEND
    
    public static void Send() {
        if (Instance != null) {
            Instance.send();
        }
    }
    
    public void send() {
        
        #if ANALYTICS_GAMEANALYTICS
        ////GA.GA_Queue.ForceSubmit();
        #endif
    }    
    
    // ----------------------------------------------------------------------
    
    public static void HandleUpdate() {        
        /*
        #if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                #if PROMO_USE_CHARTBOOST
                if (CBBinding.onBackPressed())
                    return;
                else
                    Application.Quit();
                #else 
                Application.Quit();
                #endif
            }
        }
        #endif
        */
    }
    
}


