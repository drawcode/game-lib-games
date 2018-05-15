//#define ANALYTICS_GAMEANALYTICS
//#define ANALYTICS_GOOGLE
#define ANALYTICS_UNITY

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
#if ANALYTICS_UNITY
using UnityEngine.Analytics;
#endif

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

#if ANALYTICS_GAMEANALYTICS
using GameAnalyticsSDK;
#endif

public enum AnalyticsNetworkType {
    Unity,
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
    public static string analyticsEventStoreThirdPartyPurchase = "analytics-event-store-third-party-purchase";
    public static string analyticsEventStorePurchase = "analytics-event-store-purchase";
    public static string analyticsEventGameNetworkUser = "analytics-event-game-network-user";
}

public class AnalyticsNetworksMessages {

    public static string analyticsTrackScene = "analytics-track-scene";
}

public class AnalyticsNetworks : GameObjectBehavior {

#if ANALYTICS_GAMEANALYTICS
    public GameAnalytics analyticsGameAnalytics;
#endif

    public static bool analyticsNetworksEnabled = AppConfigs.analyticsNetworkEnabled;
    public static bool analyticsNetworksTestingEnabled = AppConfigs.analyticsNetworkTestingEnabled;

    // Only one AnalyticsNetworks can exist. We use a singleton pattern to enforce this.
    private static AnalyticsNetworks _instance = null;

    public static AnalyticsNetworks Instance {

        get {
            if(!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(AnalyticsNetworks)) as AnalyticsNetworks;

                // nope, create a new one
                if(!_instance) {
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

#if ANALYTICS_UNITY
        Debug.Log("AnalyticsNetworks: ANALYTICS_UNITY Start   " +
            AnalyticsSessionInfo.userId + " " +
            AnalyticsSessionInfo.sessionState + " " +
            AnalyticsSessionInfo.sessionId + " " +
            AnalyticsSessionInfo.sessionElapsedTime);

        AnalyticsSessionInfo.sessionStateChanged += unityAnalyticsOnSessionStateChanged;
#endif


    }

    void OnDisable() {

#if ANALYTICS_GAMEANALYTICS
        // ------------
        // GAME ANALYTICS
        
        // TODO Analytics update version
        //GA.SettingsGA.AllowRoaming = true;        
#endif

#if ANALYTICS_UNITY
        Debug.Log("AnalyticsNetworks: ANALYTICS_UNITY End   " +
            AnalyticsSessionInfo.userId + " " +
            AnalyticsSessionInfo.sessionState + " " +
            AnalyticsSessionInfo.sessionId + " " +
            AnalyticsSessionInfo.sessionElapsedTime);

        AnalyticsSessionInfo.sessionStateChanged -= unityAnalyticsOnSessionStateChanged;
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
            
            analyticsGameAnalytics = FindObjectOfType(typeof(GameAnalytics)) as GameAnalytics;
            
            if (!analyticsGameAnalytics) {
                var obj = new GameObject("_analyticsGameAnalytics");
                analyticsGameAnalytics = obj.AddComponent<GameAnalytics>();
                DontDestroyOnLoad(analyticsGameAnalytics);
            }
        }

        //analyticsEveryplay.clientId = AppConfigs.analyticsEveryplayClientId;
        //analyticsEveryplay.clientSecret = AppConfigs.analyticsEveryplayClientSecret;
        //analyticsEveryplay.redirectURI = AppConfigs.analyticsEveryplayAuthUrl;
        
    }
        
#endif

#if ANALYTICS_UNITY
    // ----------------------------------------------------------------------
    // UNITY - https://docs.unity3d.com/ScriptReference/Analytics.Analytics.html
    void unityAnalyticsOnSessionStateChanged(
        AnalyticsSessionState sessionState,
        long sessionId,
        long sessionElapsedTime,
        bool sessionChanged) {

        Debug.Log("AnalyticsNetworks: ANALYTICS_UNITY unityAnalyticsOnSessionStateChanged: " +
            " userId:" + AnalyticsSessionInfo.userId + " " +
            " sessionState:" + sessionState + " " +
            " sessionId:" + sessionId + " " +
            " sessionElapsedTime:" + sessionElapsedTime + " " +
            " sessionChanged:" + sessionChanged);
    }
#endif

    // ----------------------------------------------------------------------

    // GENERIC CALLS


    // ----------------------------------------------------------------------

    // analytics

    // IS SUPPORTED

    public static bool IsSupported() {

        if(Instance != null) {
            return Instance.isSupported();
        }

        return false;
    }

    public bool isSupported() {

#if ANALYTICS_UNITY
        return true;
#elif ANALYTICS_GAMEANALYTICS
        return true;
#else 
        return false;
#endif
    }

    // LOG AREA 

    public static void Log(object data) {

        if(Instance != null) {
            Instance.log(data);
        }
    }

    public void log(object data) {

#if ANALYTICS_GAMEANALYTICS
        // TODO Analytics update version
        //GA.Log(data);
        //analyticsGameAnalytics.
#endif

#if ANALYTICS_UNITY
        AnalyticsTracker.print(data);
#endif
    }

    // SET USER ID

    public static void SetUserId(string userId) {

        if(Instance != null) {
            Instance.setUserId(userId);
        }
    }

    public void setUserId(string userId) {

#if ANALYTICS_GAMEANALYTICS
        // TODO Analytics update version
        //GA.SettingsGA.SetCustomUserID(userId);
#endif

#if ANALYTICS_UNITY
        Analytics.SetUserId(userId);
#endif
    }

    // SET USER GENDER

    public static void SetUserGender(Gender gender) {

        if(Instance != null) {
            Instance.setUserGender(gender);
        }
    }

    public void setUserGender(Gender gender) {

#if ANALYTICS_GAMEANALYTICS
        // NOOP
#endif

#if ANALYTICS_UNITY
        Analytics.SetUserGender(gender);
#endif
    }

    // SET USER BIRTH YEAR

    public static void SetUserBirthYear(int birthYear) {

        if(Instance != null) {
            Instance.setUserBirthYear(birthYear);
        }
    }

    public void setUserBirthYear(int birthYear) {

#if ANALYTICS_GAMEANALYTICS
        // NOOP
#endif

#if ANALYTICS_UNITY
        Analytics.SetUserBirthYear(birthYear);
#endif
    }

    // LOG EVENTS

    public static void LogEvent(
        string eventName, Dictionary<string, object> data) {

        if(Instance != null) {
            Instance.logEvent(eventName, data);
        }
    }

    public void logEvent(
        string eventName, Dictionary<string, object> data) {

#if ANALYTICS_GAMEANALYTICS

        StringBuilder sb = new StringBuilder();
        sb.Append("GAME:");
        sb.Append(eventName);
        sb.Append(" :: ");

        foreach(object o in args) {
            sb.Append(o);
            sb.Append(" :: ");
        }

        // TODO Analytics update version
        //GA.API.Design.NewEvent(sb.ToString(), 1);
#endif

#if ANALYTICS_UNITY


        //List<string> objectKeys = new List<string>();

        //foreach (KeyValuePair<string, object> pair in dataDict) {
        //    string key = pair.Key;
        //    object val = pair.Value;

        //    if (t == typeof(object)) {
        //        Debug.Log("key:" + key);
        //    }
        //}

        Analytics.CustomEvent(eventName, data);
#endif
    }

    // LOG EVENTS

    // SCENE CHANGE

    public static void LogEventSceneChange(string val, string title,
        Dictionary<string, object> data = null) {
        if(Instance != null) {
            Instance.logEventSceneChange(val, title, data);
        }
    }

    public void logEventSceneChange(string val, string title,
        Dictionary<string, object> data = null) {

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        data.Set(BaseDataObjectKeys.val, val);
        data.Set(BaseDataObjectKeys.title, title);

        logEvent(AnalyticMessages.analyticsEventSceneChange, data);
    }

    // GAME NETOWRK USER CHANGE

    public static void LogEventGameNetworkUser(string username, string network,
        Dictionary<string, object> data = null) {
        if(Instance != null) {
            Instance.logEventGameNetworkUser(username, network, data);
        }
    }

    public void logEventGameNetworkUser(string username, string network,
        Dictionary<string, object> data = null) {

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        data.Set(BaseDataObjectKeys.username, username);
        data.Set(BaseDataObjectKeys.network, network);

        logEvent(AnalyticMessages.analyticsEventGameNetworkUser, data);
    }

    // STORE PURCHASES THIRD PARTY

    public static void LogEventStoreThirdPartyPurchase(
        string productCode,
        int quantity,
        string productThirdPartyCode,
        decimal amount,
        string currency,
        string receiptPurchaseData,
        string signature,
        Dictionary<string, object> data = null) {

        if(Instance != null) {

            Instance.logEventStoreThirdPartyPurchase(
                productCode, quantity, productThirdPartyCode,
                amount, currency, receiptPurchaseData, signature, data);
        }
    }

    public void logEventStoreThirdPartyPurchase(
        string productCode,
        int quantity,
        string productThirdPartyCode,
        decimal amount,
        string currency,
        string receiptPurchaseData,
        string signature,
        Dictionary<string, object> data = null) {

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        data.Set(BaseDataObjectKeys.code, productCode);
        data.Set(BaseDataObjectKeys.quantity, quantity);
        data.Set(BaseDataObjectKeys.codeThirdParty, productThirdPartyCode);
        data.Set(BaseDataObjectKeys.amount, amount);
        data.Set(BaseDataObjectKeys.currency, currency);
        data.Set(BaseDataObjectKeys.receipt, receiptPurchaseData);
        data.Set(BaseDataObjectKeys.signature, signature);

#if ANALYTICS_UNITY

        data.Set("usingIAPService", true);

        Analytics.Transaction(productCode, amount, currency, receiptPurchaseData, signature, true);
#endif

        data.Remove(BaseDataObjectKeys.data);
        data.Remove(BaseDataObjectKeys.attributes);

        logEvent(AnalyticMessages.analyticsEventStoreThirdPartyPurchase, data);
    }

    // STORE PURCHASES VIRTUAL

    public static void LogEventStorePurchase(
        string productCode,
        int quantity,
        decimal amount,
        string currency,
        Dictionary<string, object> data = null) {

        if(Instance != null) {

            Instance.logEventStorePurchase(
                productCode, quantity, amount, currency, data);
        }
    }

    public void logEventStorePurchase(
        string productCode,
        int quantity,
        decimal amount,
        string currency,
        Dictionary<string, object> data = null) {

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        data.Set(BaseDataObjectKeys.code, productCode);
        data.Set(BaseDataObjectKeys.amount, amount);
        data.Set(BaseDataObjectKeys.currency, currency);

        data.Remove(BaseDataObjectKeys.data);
        data.Remove(BaseDataObjectKeys.attributes);

        logEvent(AnalyticMessages.analyticsEventStorePurchase, data);
    }

    // LEVEL START

    public static void LogEventLevelStart(Dictionary<string, object> data = null) {
        if(Instance != null) {
            Instance.logEventLevelStart(data);
        }
    }

    public void logEventLevelStart(Dictionary<string, object> data = null) {

        data = updateEventDataGame(data);

        logEvent(AnalyticMessages.analyticsEventLevelStarted, data);
    }

    // LEVEL RESULTS

    public static void LogEventLevelResults(Dictionary<string, object> data = null) {
        if(Instance != null) {
            Instance.logEventLevelResults(data);
        }
    }

    public void logEventLevelResults(Dictionary<string, object> data = null) {

        data = updateEventDataGame(data);

        logEvent(AnalyticMessages.analyticsEventLevelResults, data);
    }

    // LEVEL QUIT

    public static void LogEventLevelQuit(Dictionary<string, object> data = null) {
        if(Instance != null) {
            Instance.logEventLevelQuit(data);
        }
    }

    public void logEventLevelQuit(Dictionary<string, object> data = null) {

        data = updateEventDataGame(data);

        logEvent(AnalyticMessages.analyticsEventLevelResults, data);
    }

    public Dictionary<string, object> updateEventDataGame(
        Dictionary<string, object> data = null) {

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        data.Set(BaseDataObjectKeys.world_code, GameWorlds.Current.code);
        data.Set(BaseDataObjectKeys.world, GameWorlds.Current.display_name);
        data.Set(BaseDataObjectKeys.level_code, GameLevels.Current.code);
        data.Set(BaseDataObjectKeys.level, GameLevels.Current.display_name);

        return data;
    }

    // LEVEL EVENT GENERIC DATA

    public static void LogEvent(string key, object val,
        Dictionary<string, object> data = null) {

        if(Instance != null) {
            Instance.logEvent(key, val, data);
        }
    }

    public void logEvent(string key, object val,
        Dictionary<string, object> data = null) {

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        data.Set(BaseDataObjectKeys.key, key);
        data.Set(BaseDataObjectKeys.val, val);

        logEvent(AnalyticMessages.analyticsEvent, data);
    }

    // FLUSH/SEND

    public static void Send() {

        if(Instance != null) {
            Instance.send();
        }
    }

    public void send() {

#if ANALYTICS_GAMEANALYTICS
        // TODO Analytics update version
        ////GA.GA_Queue.ForceSubmit();
#endif

#if ANALYTICS_UNITY
        Analytics.FlushEvents();
#endif
    }

    private void OnApplicationQuit() {

        AnalyticsNetworks.Send();
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