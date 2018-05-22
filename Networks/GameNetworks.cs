#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used. 

#define GAMENETWORK_USE_UNITY
//#define GAMENETWORK_USE_PRIME31

#if UNITY_IPHONE
    #if GAMENETWORK_USE_UNITY
        #define GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
    #endif
    #if GAMENETWORK_USE_PRIME31
        #define GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
    #endif
#endif

#if UNITY_ANDROID
    #if GAMENETWORK_USE_UNITY
        #define GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
    #endif
    #if GAMENETWORK_USE_PRIME31
        #define GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        #define GAMENETWORK_ANDROID_AMAZON_CIRCLE
        #define GAMENETWORK_ANDROID_SAMSUNG
    #endif
#endif

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;

#if GAMENETWORK_USE_PRIME31
using Prime31;
#endif
#if GAMENETWORK_USE_UNITY 
#if UNITY_ANDROID
using GooglePlayGames;
#endif
using UnityEngine.SocialPlatforms;
#endif

public class GameNetworkType {
    public static string gameNetworkAppleGameCenter = "game-network-apple-gamecenter";
    public static string gameNetworkGooglePlayServices = "game-network-google-playservices";
    public static string gameNetworkAmazonGameCircle = "game-network-amazon-gamecicle";
    public static string gameNetworkSamsung = "game-network-samsung";
    public static string gameNetworkGameverses = "game-network-gameverses";
}

public class GameNetworkMessages {
    public static string gameNetworkScoresSet = "game-network-scores-set";
    public static string gameNetworkLeaderboardListGet = "game-network-leaderboard-list-get";
}

public class GameNetworkLeaderboardUserFacebook {
    public string name = "";
    public string id = "";
}

public class GameNetworkApplicationFacebook {
    public string name = "";
    public string @namespace = "";
    public string id = "";
}

public class GameNetworkLeaderboardItemFacebook {
    public GameNetworkLeaderboardUserFacebook user = new GameNetworkLeaderboardUserFacebook();
    public int score = 0;
    public GameNetworkApplicationFacebook application = new GameNetworkApplicationFacebook();
}

public class GameNetworkLeaderboardsFacebook {
    public List<GameNetworkLeaderboardItemFacebook> data = new List<GameNetworkLeaderboardItemFacebook>(); 
}

public class GameNetworks : GameObjectBehavior {

    public GameObject gameNetworkContainer;

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31 || GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
    public static bool gameNetworkiOSAppleGameCenterEnabled = true;
#else
    public static bool gameNetworkiOSAppleGameCenterEnabled = false;
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31 || GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
    public static bool gameNetworkAndroidGooglePlayEnabled = true;
#else
    public static bool gameNetworkAndroidGooglePlayEnabled = false;
#endif

#if GAMENETWORK_ANDROID_AMAZON
    public static bool gameNetworkAndroidAmazonCircleEnabled = true;
#else
    public static bool gameNetworkAndroidAmazonCircleEnabled = false;
#endif

#if GAMENETWORK_ANDROID_SAMSUNG
    public static bool gameNetworkAndroidSamsungEnabled = true;
#else
    public static bool gameNetworkAndroidSamsungEnabled = false;
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
    //[NonSerialized]
    //public GameCenterManager gameCenterManager;
    //[NonSerialized]
    //public GameCenterEventListener gameCenterEventListener;
    [NonSerialized]
    public List<GameCenterAchievement> gameNetworkAchievements;
    [NonSerialized]
    public List<GameCenterAchievementMetadata> gameNetworkAchievementsMeta;
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
    [NonSerialized]
    public GPGManager gamePlayServicesManager;
    [NonSerialized]
    public GPGSEventListener gamePlayServicesEventListener;
    //[NonSerialized]
    //public List<GPGAchievementMetadata> gameNetworkAchievements;
    [NonSerialized]
    public List<GPGAchievementMetadata> gameNetworkAchievementsMeta;
#endif

#if GAMENETWORK_USE_UNITY
    public GameNetworkUnity _gameNetworkManager;
    public IAchievement[] gameNetworkAchievements;
    public IAchievementDescription[] gameNetworkAchievementsMeta;
    public IScore[] gameNetworkLeaderboardScores;


    public GameNetworkUnity gameNetworkManager {
        get {
            if(_gameNetworkManager == null) {
                _gameNetworkManager = gameObject.Set<GameNetworkUnity>();
            }
            return _gameNetworkManager;
        }
    }

#endif

    public static string currentLoggedInUserNetwork = "";

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31 || GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
    public static string currentNetwork = GameNetworkType.gameNetworkGooglePlayServices;
#elif GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31 || GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
    public static string currentNetwork = GameNetworkType.gameNetworkAppleGameCenter;
#else
    public static string currentNetwork = "";
#endif

    private static GameNetworks _instance = null;

    public static GameNetworks instance {
        get {
            if(!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(GameNetworks)) as GameNetworks;

                // nope, create a new one
                if(!_instance) {
                    var obj = new GameObject("_GameNetworks");
                    _instance = obj.AddComponent<GameNetworks>();
                }
            }

            return _instance;
        }
    }

    void Start() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        gameNetworkAchievements = new List<GameCenterAchievement>();
        gameNetworkAchievementsMeta = new List<GameCenterAchievementMetadata>();
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        //achievementsNetwork = new List<GameCenterAchievement>();
        gameNetworkAchievementsMeta = new List<GPGAchievementMetadata>();
#endif

        InvokeRepeating("checkThirdPartyNetworkLoggedInUser", 3, 3);
    }

    void OnEnable() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        InitEvents(GameNetworkType.gameNetworkAppleGameCenter);
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        InitEvents(GameNetworkType.gameNetworkGooglePlayServices);
#endif

#if GAMENETWORK_USE_UNITY
        InitEvents(currentNetwork);
#endif
    }

    void OnDisable() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        RemoveEvents(GameNetworkType.gameNetworkAppleGameCenter);
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        RemoveEvents(GameNetworkType.gameNetworkGooglePlayServices);
#endif

#if GAMENETWORK_USE_UNITY
        RemoveEvents(currentNetwork);
#endif
    }

    // -------------------------------------------------------------------------
    // NETWORK USER CHECK

    public static void CheckThirdPartyNetworkLoggedInUser() {
        if(instance != null) {
            instance.checkThirdPartyNetworkLoggedInUser();
        }
    }

    void checkThirdPartyNetworkLoggedInUser() {
        checkThirdPartyNetworkLoggedInUser(currentNetwork);
    }

    public static void CheckThirdPartyNetworkLoggedInUser(string networkType) {
        if(instance != null) {
            instance.checkThirdPartyNetworkLoggedInUser(networkType);
        }
    }

    void checkThirdPartyNetworkLoggedInUser(string networkType) {
        // Check the third party network logged in name and the current logged in name, 
        // if the name is different change user.  
        // This helps the final case of when a user changes gamecenter while the app
        // is running and the auth changed event is just not broadcasting.

        if(GameGlobal.isReady) {

            if(GameState.Instance != null
                && GameProfiles.Current != null) {

                currentLoggedInUserNetwork = GameProfiles.Current.username;

                if((gameNetworkiOSAppleGameCenterEnabled
                    || gameNetworkAndroidGooglePlayEnabled)
                    && IsThirdPartyNetworkUserAuthenticated(networkType)) {

                    string networkName = GetNetworkUsername();
                    if(!string.IsNullOrEmpty(networkName)) {
                        currentLoggedInUserNetwork = networkName;
                    }
                }

                if(currentLoggedInUserNetwork != GameProfiles.Current.username) {
                    LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: currentLoggedInUserNetwork: " + currentLoggedInUserNetwork);

                    LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: changed: " + GameProfiles.Current.username);
                    GameState.ChangeUser(currentLoggedInUserNetwork);
                    LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: GameProfiles.Current.username: " + GameProfiles.Current.username);

                    AnalyticsNetworks.LogEventGameNetworkUser(currentLoggedInUserNetwork, networkType, null);
                }
            }
        }
    }

    // -------------------------------------------------------------------------
    // LOAD NETWORKS        

    public static void LoadNetwork(string networkType) {
        if(instance != null) {
            instance.loadNetwork(networkType);
        }
    }

    public void loadNetwork(string networkType) {
        currentNetwork = networkType;
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        if(networkType == GameNetworkType.gameNetworkAppleGameCenter) {
            InitNetwork();
        }
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        if(networkType == GameNetworkType.gameNetworkGooglePlayServices) {
            InitNetwork();
        }
#endif

#if GAMENETWORK_USE_UNITY
        if(networkType == currentNetwork) {
            InitNetwork();
        }
#endif
    }

    public static void InitNetwork() {
        if(instance != null) {
            instance.initNetwork();
        }
    }

    public void initNetwork() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        //gameCenterManager = gameObject.Set<GameCenterManager>();
        //gameCenterEventListener = gameObject.Set<GameCenterEventListener>();
           
        LoginNetwork(GameNetworkType.gameNetworkAppleGameCenter);           
        
        LogUtil.Log("InitNetwork iOS Apple GameCenter init...");
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        gamePlayServicesManager = gameObject.Set<GPGManager>();
        gamePlayServicesEventListener = gameObject.Set<GPGSEventListener>();
        
        LoginNetwork(GameNetworkType.gameNetworkGooglePlayServices);    
        
        LogUtil.Log("InitNetwork Android Google Play init...");
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
        if(currentNetwork == GameNetworkType.gameNetworkGooglePlayServices) {
            PlayGamesPlatform.Activate();
        }
#endif

#if GAMENETWORK_USE_UNITY
        LoginNetwork(currentNetwork);

        LogUtil.Log("InitNetwork Unity GameNetwork...");
#endif

        // Web player...

#if UNITY_WEBPLAYER
            Application.ExternalCall("if(window.console) window.console.log","web init");
#endif
    }

    // -------------------------------------------------------------------------
    // NETWORK AVAILABILITY     

    public static bool IsThirdPartyNetworkAvailable(string networkTypeTo) {

        LogUtil.Log("IsThirdPartyNetworkAvailable:" + networkTypeTo);

        bool isAvailable = false;

        if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
            isAvailable = isAvailableiOSAppleGameCenter;
        }
        else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
            isAvailable = isAvailableAndroidGooglePlay;
        }

        LogUtil.Log("IsThirdPartyNetworkAvailable:isAvailable:" + isAvailable);

        return isAvailable;
    }

    public static bool isAvailableiOSAppleGameCenter {
        get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
            return true;//return GameCenterBinding.isGameCenterAvailable(); 
#elif GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
            return true;
#else
            return false;
#endif  
        }
    }

    public static bool isAvailableAndroidGooglePlay {
        get {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31 

            return true;//PlayGameServices. false;//GameCenterBinding.isGameCenterAvailable();
#elif GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
            return true;
#else
            return false;
#endif  
        }
    }

    // -------------------------------------------------------------------------
    // USER AUTHENTICATED       

    public static bool IsThirdPartyNetworkUserAuthenticated(string networkType) {
        bool isAuthenticated = false;

        //Debug.Log("IsThirdPartyNetworkUserAuthenticated:networkType:" + networkType);

        if(networkType == GameNetworkType.gameNetworkAppleGameCenter) {
            isAuthenticated = isAuthenticatediOSAppleGameCenter;
        }
        else if(networkType == GameNetworkType.gameNetworkGooglePlayServices) {
            isAuthenticated = isAuthenticatedAndroidGooglePlay;
        }

        //Debug.Log("IsThirdPartyNetworkUserAuthenticated:isAuthenticated:" + isAuthenticated);

        return isAuthenticated;
    }

    public static bool isAuthenticatediOSAppleGameCenter {
        get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
            return GameCenterBinding.isPlayerAuthenticated();
#elif GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
            return true;
#else
            return false;
#endif  
        }
    }

    public static bool isAuthenticatedAndroidGooglePlay {
        get {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31 
            return PlayGameServices.isSignedIn();
#elif GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
            return true;
#else
            return false;
#endif  
        }
    }

    public bool IsThirdPartyNetworkReady(string networkType) {
        bool isReady = false;
        if(IsThirdPartyNetworkAvailable(networkType)
            && IsThirdPartyNetworkUserAuthenticated(networkType)) {
            isReady = true;
        }
        return isReady;
    }

    // -------------------------------------------------------------------------
    // ACHIEVEMENTS UI      

    public static void ShowAchievementsOrLogin() {

        //LogUtil.Log("ShowAchievementsOrLogin");

        if(instance != null) {
            instance.showAchievementsOrLogin(currentNetwork);
        }
    }

    public static void ShowAchievementsOrLogin(string networkTypeTo) {

        LogUtil.Log("ShowAchievementsOrLogin:networkTypeTo:" + networkTypeTo);

        if(instance != null) {
            instance.showAchievementsOrLogin(networkTypeTo);
        }
    }

    public void showAchievementsOrLogin(string networkTypeTo) {

        Debug.Log("showAchievementsOrLogin:networkTypeTo:" + networkTypeTo);

        if(IsThirdPartyNetworkAvailable(networkTypeTo)) {

            if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                showAchievementsOrLoginiOSAppleGameCenter();
            }
            else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                showAchievementsOrLoginAndroidGooglePlay();
            }
        }
    }

    public void showAchievementsOrLoginiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        
        //LogUtil.Log("showAchievementsOrLoginiOSAppleGameCenter:GameNetworks.gameNetworkiOSAppleGameCenterEnabled:" + 
                  //GameNetworks.gameNetworkiOSAppleGameCenterEnabled);
            
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {

            bool authenticated = IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkAppleGameCenter);
                               
            Debug.Log("showAchievementsOrLoginiOSAppleGameCenter:authenticated:" + 
                  authenticated);

            if(authenticated) {
                GameCenterBinding.showAchievements();
            }
            else {
                GameCenterBinding.authenticateLocalPlayer();
            }
        }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY

        //LogUtil.Log("showAchievementsOrLoginiOSAppleGameCenter:GameNetworks.gameNetworkiOSAppleGameCenterEnabled:" + 
        //GameNetworks.gameNetworkiOSAppleGameCenterEnabled);

        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {

            bool authenticated = IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkAppleGameCenter);

            Debug.Log("showAchievementsOrLoginiOSAppleGameCenter:authenticated:" +
                  authenticated);

            if(authenticated) {
                gameNetworkManager.showAchievements();
            }
            else {
                gameNetworkManager.authenticate();
            }
        }
#endif
    }

    public void showAchievementsOrLoginAndroidGooglePlay() {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        
        LogUtil.Log("showAchievementsOrLoginAndroidGooglePlay:GameNetworks.gameNetworkAndroidGooglePlayEnabled:" + 
            GameNetworks.gameNetworkAndroidGooglePlayEnabled);
        
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
        
            bool authenticated = IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkGooglePlayServices);
        
            LogUtil.Log("showAchievementsOrLoginAndroidGooglePlay:authenticated:" + 
                  authenticated);

            if(authenticated) {
                PlayGameServices.showAchievements();
            }
            else {
                PlayGameServices.authenticate();
            }
        }
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY

        LogUtil.Log("showAchievementsOrLoginAndroidGooglePlay:GameNetworks.gameNetworkAndroidGooglePlayEnabled:" +
            GameNetworks.gameNetworkAndroidGooglePlayEnabled);

        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {

            bool authenticated = IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkAppleGameCenter);

            Debug.Log("showAchievementsOrLoginiOSAppleGameCenter:authenticated:" +
                  authenticated);

            if(authenticated) {
                gameNetworkManager.showAchievements();
            }
            else {
                gameNetworkManager.authenticate();
            }
        }
#endif
    }

    // -------------------------------------------------------------------------
    // LEADERBOARDS UI

    public static void ShowLeaderboardsOrLogin() {

        Debug.Log("ShowLeaderboardsOrLogin");

        if(instance != null) {
            instance.showLeaderboardsOrLogin(currentNetwork);
        }
    }

    public static void ShowLeaderboardsOrLogin(string networkTypeTo) {
        if(instance != null) {
            instance.showLeaderboardsOrLogin(networkTypeTo);
        }
    }

    public void showLeaderboardsOrLogin(string networkTypeTo) {

        Debug.Log("showLeaderboardsOrLogin:networkTypeTo:" + networkTypeTo);

        if(IsThirdPartyNetworkAvailable(networkTypeTo)) {

            if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                showLeaderboardsOrLoginiOSAppleGameCenter();
            }
            else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                showLeaderboardsOrLoginAndroidGooglePlay();
            }
        }
    }

    public void showLeaderboardsOrLoginiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        
        Debug.Log("showLeaderboardsOrLoginiOSAppleGameCenter");

        //LogUtil.Log("showLeaderboardsOrLoginiOSAppleGameCenter:GameNetworks.gameNetworkiOSAppleGameCenterEnabled:" + 
                  //GameNetworks.gameNetworkiOSAppleGameCenterEnabled);
                
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                if(IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkAppleGameCenter)) {
                    
                    Debug.Log("showLeaderboardWithTimeScope::");

                    GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
                }
                else {
                    GameCenterBinding.authenticateLocalPlayer();
                }
            }
        }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY

        Debug.Log("showLeaderboardsOrLoginiOSAppleGameCenter");

        //LogUtil.Log("showLeaderboardsOrLoginiOSAppleGameCenter:GameNetworks.gameNetworkiOSAppleGameCenterEnabled:" + 
        //GameNetworks.gameNetworkiOSAppleGameCenterEnabled);

        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                if(IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkAppleGameCenter)) {

                    Debug.Log("showLeaderboardWithTimeScope::");

                    gameNetworkManager.showLeaderboards();
                    //GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
                }
                else {
                    gameNetworkManager.authenticate();
                }
            }
        }
#endif
    }

    public void showLeaderboardsOrLoginAndroidGooglePlay() {

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31

        LogUtil.Log("showLeaderboardsOrLoginAndroidGooglePlay");
        
        LogUtil.Log("showLeaderboardsOrLoginAndroidGooglePlay:GameNetworks.gameNetworkAndroidGooglePlayEnabled:" + 
                  GameNetworks.gameNetworkAndroidGooglePlayEnabled);
            
            if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
                if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                    if(IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkGooglePlayServices)) {   
                    
                    LogUtil.Log("showLeaderboardsOrLoginAndroidGooglePlay:showLeaderboards::");

                        PlayGameServices.showLeaderboards();
                    }
                    else {
                        PlayGameServices.authenticate();
                    }
                }
            }
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY

        Debug.Log("showLeaderboardsOrLoginAndroidGooglePlay");

        //LogUtil.Log("showLeaderboardsOrLoginiOSAppleGameCenter:GameNetworks.gameNetworkiOSAppleGameCenterEnabled:" + 
        //GameNetworks.gameNetworkiOSAppleGameCenterEnabled);

        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                if(IsThirdPartyNetworkUserAuthenticated(GameNetworkType.gameNetworkGooglePlayServices)) {

                    LogUtil.Log("showLeaderboardsOrLoginAndroidGooglePlay:showLeaderboards::");

                    gameNetworkManager.showLeaderboards();
                    //GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
                }
                else {
                    gameNetworkManager.authenticate();
                }
            }
        }
#endif
    }

    // -------------------------------------------------------------------------
    // LEADERBOARDS

    public static void SendScore(string key, long keyValue) {
        if(instance != null) {
            instance.sendScore(key, keyValue);
        }
    }

    public static void SendScore(string networkTypeTo, string key, long keyValue) {
        if(instance != null) {
            instance.sendScore(networkTypeTo, key, keyValue);
        }
    }

    public void sendScore(string key, long keyValue) {

        GameLeaderboard item = GameLeaderboards.Instance.GetById(key);

        //LogUtil.Log("sendScore:" + " key:" + key + " keyValue:" + keyValue);

        if(item == null) {
            return;
        }

        if(item.data == null) {
            return;
        }

        foreach(GameNetworkData networkData in item.data.networks) {

            reportScore(networkData.type, networkData.code, keyValue);
        }
    }

    public void sendScore(string networkTypeTo, string key, long keyValue) {
        reportScore(networkTypeTo, key, keyValue);
    }

    public static void ReportScore(string key, long keyValue) {
        if(instance != null) {
            instance.reportScore(currentNetwork, key, keyValue);
        }
    }

    public static void ReportScore(string networkTypeTo, string key, long keyValue) {
        if(instance != null) {
            instance.reportScore(networkTypeTo, key, keyValue);
        }
    }

    public void reportScore(string networkTypeTo, string key, long keyValue) {

        Debug.Log("reportScore:" +
                    " networkTypeTo:" + networkTypeTo +
                    " key:" + key +
                    " keyValue:" + keyValue);

        if(IsThirdPartyNetworkAvailable(networkTypeTo)) {

            if(key.IndexOf("time-played") > -1) {
                keyValue = keyValue * 100;
            }

            Debug.Log("reportScore:IsThirdPartyNetworkAvailable:" +
                        " networkTypeTo:" + networkTypeTo +
                        " key:" + key +
                        " keyValue:" + keyValue);

            if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                if(isAuthenticatediOSAppleGameCenter) {
                    reportScoreAppleGameCenter(key, keyValue);
                }
            }
            else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                if(isAuthenticatedAndroidGooglePlay) {
                    reportScoreGooglePlay(key, keyValue);
                }
            }
        }
    }

    public void reportScoreAppleGameCenter(string key, long keyValue) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                LogUtil.Log("reportScoreAppleGameCenter:" + " key:" + key + " keyValue:" + keyValue); 
                GameCenterBinding.reportScore(keyValue, key);
            }
        }
#endif
#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                Debug.Log("reportScoreAppleGameCenter:" + " key:" + key + " keyValue:" + keyValue);
                //GameCenterBinding.reportScore(keyValue, key);
                gameNetworkManager.reportScore(keyValue, key);
            }
        }
#endif
    }

    public void reportScoreGooglePlay(string key, long keyValue) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                LogUtil.Log("reportScoreGooglePlay:" + " key:" + key + " keyValue:" + keyValue); 
                PlayGameServices.submitScore(key, keyValue);
            }
        }
#endif
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                LogUtil.Log("reportScoreGooglePlay:" + " key:" + key + " keyValue:" + keyValue);
                //PlayGameServices.submitScore(key, keyValue);
                gameNetworkManager.reportScore(keyValue, key);
            }
        }
#endif
    }

    // -------------------------------------------------------------------------
    // ACHIEVEMENTS

    #region ACHIEVEMENTS

    public static void SendAchievement(string key, bool completed) {
        if(instance != null) {
            instance.sendAchievement(key, completed);
        }
    }

    public void sendAchievement(string key, bool completed) {

        sendAchievement(key, completed ? 100f : 1f);
    }

    public static void SendAchievement(string key, float progress) {
        if(instance != null) {
            instance.sendAchievement(key, progress);
        }
    }

    public void sendAchievement(string key, float progress) {

        GameAchievement item = GameAchievements.Instance.GetById(key);

        if(item == null) {
            return;
        }

        if(item.data == null) {
            return;
        }

        foreach(GameNetworkData networkData in item.data.networks) {

            reportAchievement(networkData.type, networkData.code, progress);
        }
    }

    public static void ReportAchievement(string key, bool completed) {
        if(instance != null) {
            instance.reportAchievement(currentNetwork, key, completed);
        }
    }

    public static void ReportAchievement(string networkTypeTo, string key, bool completed) {
        if(instance != null) {
            instance.reportAchievement(networkTypeTo, key, completed);
        }
    }

    public void reportAchievement(string networkTypeTo, string key, bool completed) {
        ReportAchievement(networkTypeTo, key, 100.0f);
    }

    public static void ReportAchievement(string key, float progress) {
        if(instance != null) {
            instance.reportAchievement(currentNetwork, key, progress);
        }
    }

    public static void ReportAchievement(string networkTypeTo, string key, float progress) {
        if(instance != null) {
            instance.reportAchievement(networkTypeTo, key, progress);
        }
    }

    public void reportAchievement(string networkTypeTo, string key, float progress) {
        if(IsThirdPartyNetworkAvailable(networkTypeTo)) {
            if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                if(isAuthenticatediOSAppleGameCenter) {
                    reportAchievementAppleGameCenter(key, progress);
                }
            }
            else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                if(isAuthenticatedAndroidGooglePlay) {
                    reportAchievementGooglePlay(key, progress);
                }
            }
        }
    }

    public void reportAchievementAppleGameCenter(string key, float progress) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                LogUtil.Log("reportAchievementAppleGameCenter:" + " key:" + key + " progress:" + progress); 
                GameCenterBinding.reportAchievement(key, progress);
            }
        }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                LogUtil.Log("reportAchievementAppleGameCenter:" + " key:" + key + " progress:" + progress);
                //GameCenterBinding.reportAchievement(key, progress);
                gameNetworkManager.reportProgress(key, progress, null);
            }
        }
#endif
    }

    public void reportAchievementGooglePlay(string key, float progress) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                LogUtil.Log("reportAchievementGooglePlay:" + " key:" + key + " progress:" + progress);
                if(progress / 100 > .95f) {
                    PlayGameServices.unlockAchievement(key, true);
                }
                else {
                    PlayGameServices.incrementAchievement(key, (int)progress);             
                }
            }
        }
#endif
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                LogUtil.Log("reportAchievementGooglePlay:" + " key:" + key + " progress:" + progress);
                if(progress / 100 > .95f) {
                    //PlayGameServices.unlockAchievement(key, true);
                    gameNetworkManager.reportProgress(key, progress, null);
                }
                else {
                    //PlayGameServices.incrementAchievement(key, (int)progress); 
                    gameNetworkManager.reportProgress(key, progress, null);
                }
            }
        }
#endif
    }


    // -------------------------------------------------------------------------
    // ACHIEVEMENT DATA

    public static void GetAchievements() {
        if(instance != null) {
            instance.getAchievements();
        }
    }

    public void getAchievements() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        GameCenterBinding.getAchievements();
            
        LogUtil.Log("GameCenter GetAchievements...");
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        PlayGameServices.reloadAchievementAndLeaderboardData();
        
        //LogUtil.Log("GameCenter GetAchievements...");
#endif

#if GAMENETWORK_USE_UNITY
        //PlayGameServices.reloadAchievementAndLeaderboardData();

        gameNetworkManager.loadAchievements();
        //LogUtil.Log("GameCenter GetAchievements...");
#endif

    }

    public static void GetAchievementsMetadata() {
        if(instance != null) {
            instance.getAchievementsMetadata();
        }
    }

    public void getAchievementsMetadata() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        GameCenterBinding.retrieveAchievementMetadata();
            
        //LogUtil.Log("GameCenter GetAchievements...");
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        PlayGameServices.getAllAchievementMetadata();
        
        //LogUtil.Log("PlayGameServices GetAchievements...");
#endif


#if GAMENETWORK_USE_UNITY
        gameNetworkManager.loadAchievementDescriptions();
        //LogUtil.Log("GameCenter GetAchievements...");
#endif
    }

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
    
    public static GameCenterAchievement GetGameCenterAchievement(string identifier) {
        if(Instance != null) {
            return Instance.getGameCenterAchievement(identifier);
        }
        return null;
    }   
    
    public GameCenterAchievement getGameCenterAchievement(string identifier) {      
        foreach(GameCenterAchievement achievement in gameNetworkAchievements) {
            if(achievement.identifier == identifier)
                return achievement;
        }
        return null;    
    }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
    public static GameCenterAchievementMetadata GetGameCenterAchievementMetadata(string identifier) {
        if(Instance != null) {
            Instance.getGameCenterAchievementMetadata(identifier);
        }
        return null;
    }   
    
    public GameCenterAchievementMetadata getGameCenterAchievementMetadata(string identifier)
    {       
        foreach(GameCenterAchievementMetadata achievement in gameNetworkAchievementsMeta) {
            if(achievement.identifier == identifier)
                return achievement;
        }
        return null;    
    }
#endif


#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31

#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31

#endif

#if GAMENETWORK_USE_UNITY

    public static IAchievement GetGameNetworkAchievement(string id) {
        if(instance != null) {
            return instance.getGameNetworkAchievement(id);
        }
        return null;
    }

    public IAchievement getGameNetworkAchievement(string id) {
        foreach(IAchievement achievement in gameNetworkAchievements) {
            if(achievement.id == id)
                return achievement;
        }
        return null;
    }
#endif

    public static void CheckAchievementsState() {
        if(instance != null) {
            instance.checkAchievementsState();
        }
    }

    public void checkAchievementsState() {

#if GAMENETWORK_USE_UNITY

        if(gameNetworkAchievements == null) {
            return;
        }

        if(gameNetworkAchievements.Length == 0) {
            return;
        }

        // Sync from other devices.GameNetworks:onGameNetworkUnityAchievements:0
        foreach(IAchievement achievement in gameNetworkAchievements) {
            bool localAchievementValue = GameProfileAchievements.Current.GetAchievementValue(achievement.id);
            bool remoteAchievementValue = achievement.completed;

            // If different on local and remote and local is true, set it...
            if(localAchievementValue != remoteAchievementValue && remoteAchievementValue) {
                GameProfileAchievements.Current.SetAchievementValue(achievement.id, true);
            }
        }

        if(gameNetworkAchievementsMeta == null) {
            return;
        }

        if(gameNetworkAchievementsMeta.Length == 0) {
            return;
        }

        foreach(IAchievementDescription meta in gameNetworkAchievementsMeta) {
            // TODO - pull down any new achievements from iTunesConnect
            LogUtil.Log("UnityAchievementMetadata:" + meta.id);
        }

        // Sync from local.
        foreach(GameAchievement meta in GameAchievements.Instance.GetAll()) {

            IAchievement gcAchievement = GetGameNetworkAchievement(meta.code);

            if(gcAchievement != null) {
                bool localAchievementValue = GameProfileAchievements.Current.GetAchievementValue(meta.code);
                bool remoteAchievementValue = gcAchievement.completed;

                // If different on local and remote and remote is false, set it...
                if(localAchievementValue != remoteAchievementValue && !remoteAchievementValue) {
                    GameNetworks.SendAchievement(meta.code, true);
                }
            }
        }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        // Sync from other devices.
        foreach(GameCenterAchievement achievement in gameNetworkAchievements) {
            bool localAchievementValue = GameProfileAchievements.Current.GetAchievementValue(achievement.identifier);
            bool remoteAchievementValue = achievement.completed;
            
            // If different on local and remote and local is true, set it...
            if(localAchievementValue != remoteAchievementValue && remoteAchievementValue) {
                GameProfileAchievements.Current.SetAchievementValue(achievement.identifier, true);
            }
        }
        
        foreach(GameCenterAchievementMetadata meta in gameNetworkAchievementsMeta) {
            // TODO - pull down any new achievements from iTunesConnect
            LogUtil.Log("GameCenterAchievementMetadata:" + meta.identifier);
        }
        
        // Sync from local.
        foreach(GameAchievement meta in GameAchievements.Instance.GetAll()) {
            GameCenterAchievement gcAchievement = GetGameCenterAchievement(meta.code);
            if(gcAchievement != null)
            {
                bool localAchievementValue = GameProfileAchievements.Current.GetAchievementValue(meta.code);
                bool remoteAchievementValue = gcAchievement.completed;
                
                // If different on local and remote and remote is false, set it...
                if(localAchievementValue != remoteAchievementValue && !remoteAchievementValue) {
                    GameNetworks.SendAchievement(meta.code, true);
                }
            }
        }
        
        //LogUtil.Log("GameCenter CheckAchievementsState...");
#endif

    }

    #endregion

    // -------------------------------------------------------------------------
    // NETWORK LOGIN

    #region AUTHENTICATION

    public static void LoginNetwork(string networkTypeTo) {
        if(instance != null) {
            instance.loginNetwork(networkTypeTo);
        }
    }

    public void loginNetwork(string networkTypeTo) {

        if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
            if(isAvailableiOSAppleGameCenter) {
                GameCenterBinding.authenticateLocalPlayer();
                Debug.Log("GameCenter LoginNetwork is available...");
            }
            
        // Check existing achievements and update them if missing
            
            LogUtil.Log("GameCenter LoginNetwork...");
#endif
#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
            if(isAvailableiOSAppleGameCenter) {
                gameNetworkManager.authenticate();
                Debug.Log("Unity GameCenter LoginNetwork is available...");
            }

            // Check existing achievements and update them if missing

            LogUtil.Log("Unity GameCenter LoginNetwork...");
#endif
        }
        else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                PlayGameServices.authenticate();
            }
            
            // Check existing achievements and update them if missing
            
            LogUtil.Log("PlayGameServices LoginNetwork...");
#endif
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                gameNetworkManager.authenticate();
                Debug.Log("Unity PlayGameServices LoginNetwork is available...");
            }

            // Check existing achievements and update them if missing

            LogUtil.Log("Unity PlayGameServices LoginNetwork...");
#endif
        }


    }

    // -------------------------------------------------------------------------
    // USERNAME


    public static void SetLocalProfileToNetworkUsername() {
        if(instance != null) {
            instance.setLocalProfileToNetworkUsername(currentNetwork);
        }
    }

    public static void SetLocalProfileToNetworkUsername(string networkTypeTo) {
        if(instance != null) {
            instance.setLocalProfileToNetworkUsername(networkTypeTo);
        }
    }

    public void setLocalProfileToNetworkUsername(string networkTypeTo) {
        LogUtil.Log("setLocalProfileToNetworkUsername");
        if(IsThirdPartyNetworkAvailable(networkTypeTo)) {
            if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                setLocalProfileToNetworkUsernameAppleGameCenter();
            }

            if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                setLocalProfileToNetworkUsernameGooglePlay();
            }
        }
    }

    public void setLocalProfileToNetworkUsernameAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        string networkUsername = GameCenterBinding.playerAlias();
        LogUtil.Log("setLocalProfileToNetworkUsernameiOSAppleGameCenter: " + networkUsername);
        
        if(!string.IsNullOrEmpty(networkUsername)) {
            //GameState.ChangeUser(networkUsername);
            //GameState.Instance.profile.SetThirdPartyNetworkUser(true);
            //GameState.SaveProfile();
            
            //LogUtil.Log("Logging in user GameCenter: " + networkUsername);
        }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
        string networkUsername = gameNetworkManager.currentUser.userName;
        LogUtil.Log("setLocalProfileToNetworkUsernameiOSAppleGameCenter: " + networkUsername);

        if(!string.IsNullOrEmpty(networkUsername)) {
            //GameState.ChangeUser(networkUsername);
            //GameState.Instance.profile.SetThirdPartyNetworkUser(true);
            //GameState.SaveProfile();

            //LogUtil.Log("Logging in user GameCenter: " + networkUsername);
        }
#endif
    }

    public void setLocalProfileToNetworkUsernameGooglePlay() {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        GPGPlayerInfo playerInfo = PlayGameServices.getLocalPlayerInfo();
        string networkUsername = playerInfo.playerId; // name
        LogUtil.Log("setLocalProfileToNetworkUsernameAndroidGooglePlay: " + networkUsername);
        
        if(!string.IsNullOrEmpty(networkUsername)) {
            //GameState.ChangeUser(networkUsername);
            //GameState.Instance.profile.SetThirdPartyNetworkUser(true);
            //GameState.SaveProfile();
            
            //LogUtil.Log("Logging in user GameCenter: " + networkUsername);
        }
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
        string networkUsername = gameNetworkManager.currentUser.userName;
        LogUtil.Log("setLocalProfileToNetworkUsernameAndroidGooglePlay: " + networkUsername);

        if(!string.IsNullOrEmpty(networkUsername)) {
            //GameState.ChangeUser(networkUsername);
            //GameState.Instance.profile.SetThirdPartyNetworkUser(true);
            //GameState.SaveProfile();

            //LogUtil.Log("Logging in user GameCenter: " + networkUsername);
        }
#endif
    }

    public static string GetNetworkUsername() {
        if(instance != null) {
            return instance.getNetworkUsername(currentNetwork);
        }
        return "";
    }

    public static string GetNetworkUsername(string networkTypeTo) {
        if(instance != null) {
            return instance.getNetworkUsername(networkTypeTo);
        }
        return "";
    }

    public string getNetworkUsername(string networkTypeTo) {

        string username = "";

        if(IsThirdPartyNetworkAvailable(networkTypeTo)) {
            if(currentNetwork == GameNetworkType.gameNetworkAppleGameCenter) {
                username = getNetworkUsernameiOSAppleGameCenter();
            }
            else if(currentNetwork == GameNetworkType.gameNetworkGooglePlayServices) {
                username = getNetworkUsernameAndroidGooglePlay();
            }
        }

        //LogUtil.Log("getNetworkUsername:" + username);

        return username;
    }

    public string getNetworkUsernameFiltered(string name) {

        if(name.IsNullOrEmpty()) {
            return ProfileConfigs.defaultPlayerName;
        }

        if(name.ToLower() == "uninitialized") {
            return ProfileConfigs.defaultPlayerName;
        }

        return name;
    }

    public string getNetworkUsernameiOSAppleGameCenter() {
        //LogUtil.Log("GetNetworkUsername");
        string networkUser = "";
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
        networkUser = getNetworkUsernameFiltered(GameCenterBinding.playerAlias());
        if(networkUser != GameProfiles.Current.username 
            && !string.IsNullOrEmpty(networkUser)) {
            LogUtil.Log("GetNetworkUsername: " + networkUser);          
        }
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
        networkUser = getNetworkUsernameFiltered(gameNetworkManager.currentUser.userName);
        if(networkUser != GameProfiles.Current.username
            && !string.IsNullOrEmpty(networkUser)) {
            LogUtil.Log("GetNetworkUsername: " + networkUser);
        }
#endif
        return networkUser;
    }

    public string getNetworkUsernameAndroidGooglePlay() {
        //LogUtil.Log("GetNetworkUsername");
        string networkUser = "";
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
        GPGPlayerInfo playerInfo = PlayGameServices.getLocalPlayerInfo();
        networkUser = getNetworkUsernameFiltered(playerInfo.playerId);// name
        if(networkUser != GameProfiles.Current.username 
            && !string.IsNullOrEmpty(networkUser)) {
            LogUtil.Log("GetNetworkUsername: " + networkUser);          
        }
#endif
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY
        networkUser = getNetworkUsernameFiltered(gameNetworkManager.currentUser.userName);
        if(networkUser != GameProfiles.Current.username
            && !string.IsNullOrEmpty(networkUser)) {
            LogUtil.Log("GetNetworkUsername: " + networkUser);
        }
#endif
        return networkUser;
    }

    #endregion

    // -------------------------------------------------------------------------
    // FACEBOOK - SCORES

    #region FACEBOOK

    public string facebookOpenGraphUrl = "https://graph.facebook.com/";

    public static void PostScoreFacebook(int score) {
        if(instance != null) {
            instance.postScoreFacebook(score);
        }
    }

    public void postScoreFacebook(int score) {
        //PostScoreFacebook(GameProfiles.Current.GetSocialNetworkUserId(), score);
    }

    public static void PostScoreFacebook(string userId, int score) {
        if(instance != null) {
            instance.postScoreFacebook(userId, score);
        }
    }

    public void postScoreFacebook(string userId, int score) {

        Dictionary<string, object> data = new Dictionary<string, object>();

        string networkType = SocialNetworkTypes.facebook;

        string access_token = GameProfiles.Current.GetNetworkValueToken(networkType);

        data.Add("score", score);
        data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
        data.Add("access_token", access_token);//SocialNetworks.faGetSocialNetworkAuthTokenUser());

        LogUtil.Log("PostScoreFacebook score:" + score);
        LogUtil.Log("PostScoreFacebook app_access_token:" + SocialNetworks.Instance.appAccessToken);
        LogUtil.Log("PostScoreFacebook access_token:" + access_token);

        string url = facebookOpenGraphUrl + userId + "/scores";

        LogUtil.Log("PostScoreFacebook url:" + url);

        WebRequests.Instance.Request(WebRequests.RequestType.HTTP_POST, url, data, HandlePostScoreFacebookCallback);
    }

    void HandlePostScoreFacebookCallback(WebRequests.ResponseObject response) {
        string responseText = response.dataValueText;
        LogUtil.Log("HandlePostScoreFacebookCallback responseText:" + responseText);
        bool success = false;
        if(bool.TryParse(responseText, out success)) {
            if(success) {
                LogUtil.Log("Score post success!");
            }
            else {
                LogUtil.Log("Score post failed!");
            }
        }
    }

    public static void GetScoresFacebookFriends() {
        if(instance != null) {
            instance.getScoresFacebookFriends();
        }
    }

    public void getScoresFacebookFriends() {

        Dictionary<string, object> data = new Dictionary<string, object>();

        string accessToken = GameProfiles.Current.GetNetworkValueToken(SocialNetworkTypes.facebook);//.GetSocialNetworkAuthTokenUser();
        string userId = GameProfiles.Current.GetNetworkValueId(SocialNetworkTypes.facebook);//GetSocialNetworkUserId();
        string appId = SocialNetworks.Instance.FACEBOOK_APP_ID;

        //data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
        data.Add("access_token", accessToken);

        string url = facebookOpenGraphUrl
            + userId + "/scores";

        LogUtil.Log("GetScoresFacebookFriends access_token:" + accessToken);
        LogUtil.Log("GetScoresFacebookFriends url:" + url);

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_GET,
            url,
            data,
            HandleGetScoresFacebookFriendsCallback);
    }

    void HandleGetScoresFacebookFriendsCallback(WebRequests.ResponseObject response) {
        string responseText = response.dataValueText;
        LogUtil.Log("HandleGetScoresFacebookFriendsCallback responseText:" + responseText);

        if(string.IsNullOrEmpty(responseText)) {
            return;
        }

        GameCommunityLeaderboardData leaderboardData = ParseScoresFacebook(responseText);

        if(leaderboardData != null) {
            Messenger<GameCommunityLeaderboardData>.Broadcast(
                GameCommunityMessages.gameCommunityLeaderboardFriendData, leaderboardData);
        }

    }

    public static void GetScoresFacebook() {
        if(instance != null) {
            instance.getScoresFacebook();
        }
    }

    private void getScoresFacebook() {

        Dictionary<string, object> data = new Dictionary<string, object>();

        string accessToken =
            GameProfiles.Current.GetNetworkValueToken(SocialNetworkTypes.facebook);//GetSocialNetworkAuthTokenUser();
        string appId = SocialNetworks.Instance.FACEBOOK_APP_ID;

        //data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
        data.Add("access_token", accessToken);

        string url = facebookOpenGraphUrl
            + appId + "/scores";

        LogUtil.Log("GetScoresFacebook access_token:" + accessToken);
        LogUtil.Log("GetScoresFacebook url:" + url);

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_GET,
            url,
            data,
            HandleGetScoresFacebookCallback);
    }

    void HandleGetScoresFacebookCallback(Engine.Networking.WebRequests.ResponseObject response) {
        string responseText = response.dataValueText;
        LogUtil.Log("HandleGetScoresFacebookCallback responseText:" + responseText);

        if(string.IsNullOrEmpty(responseText)) {
            return;
        }

        GameCommunityLeaderboardData leaderboardData = parseScoresFacebook(responseText);

        if(leaderboardData != null) {
            Messenger<GameCommunityLeaderboardData>.Broadcast(
                GameCommunityMessages.gameCommunityLeaderboardData, leaderboardData);
        }
    }

    public static string testFacebookScoresResult = "";

    public static void ParseTestScoresFacebook(string responseText) {
        GameCommunityLeaderboardData leaderboardData = ParseScoresFacebook(responseText);

        Messenger<GameCommunityLeaderboardData>.Broadcast(
            GameCommunityMessages.gameCommunityLeaderboardData, leaderboardData);
    }

    public static GameCommunityLeaderboardData ParseScoresFacebook(string responseText) {
        if(instance != null) {
            return instance.parseScoresFacebook(responseText);
        }
        return null;
    }

    public GameCommunityLeaderboardData parseScoresFacebook(string responseText) {

        /*
             * // facebook used 'namespace' in a property so we need to 
             * parse as json object instead of one shot map to object
                     {
                   "data":[
                      {
                         "user":{
                            "name":"Draw Code",
                            "id":"111111"
                         },
                         "score":240,
                         "application":{
                            "name":"Game Community",
                            "namespace":"community",
                            "id":"111111"
                         }
                      },
                      {
                         "user":{
                            "name":"Draw Labs",
                            "id":"111111"
                         },
                         "score":23,
                         "application":{
                            "name":"Game Community",
                            "namespace":"community",
                            "id":"11111"
                         }
                      }
                   ]
                }
     */




        GameCommunityLeaderboardData leaderboardData = new GameCommunityLeaderboardData();

        if(string.IsNullOrEmpty(responseText)) {
            return leaderboardData;
        }

        List<GameCommunityLeaderboardItem> leaderboardItems = new List<GameCommunityLeaderboardItem>();

        string json = responseText.Replace("\\\"", "\"");

        Dictionary<string, object> jsonData = json.FromJson<Dictionary<string, object>>();

        if(jsonData != null && jsonData.Count > 0) {

            List<Dictionary<string, object>> dataNode = jsonData.Get<List<Dictionary<string, object>>>("data");

            if(dataNode != null && dataNode.Count > 0) {

                for(int i = 0; i < dataNode.Count; i++) {

                    GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();

                    Dictionary<string, object> data = dataNode[i];

                    Dictionary<string, object> user = data.Get<Dictionary<string, object>>("user");
                    Dictionary<string, object> application = data.Get<Dictionary<string, object>>("application");
                    double score = data.Get<double>("score");

                    leaderboardItem.value = float.Parse(score.ToString());
                    leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");

                    if(user != null && user.Count > 0) {

                        string name = user.Get<string>("name");
                        leaderboardItem.username = name;

                        string id = user.Get<string>("id");
                        leaderboardItem.userId = id;

                        leaderboardItem.network = "facebook";
                        leaderboardItem.name = leaderboardItem.username;
                        leaderboardItem.type = "int";
                        leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);
                    }

                    if(application != null && application.Count > 0) {

                        string name = user.Get<string>("name");
                        leaderboardData.appName = name;

                        string namespaceNode = user.Get<string>("namespace");
                        leaderboardData.appName = namespaceNode;

                        string id = user.Get<string>("id");
                        leaderboardData.appId = id;
                    }

                    leaderboardItems.Add(leaderboardItem);
                }
            }
        }


        //JsonData jsonData = JsonMapper.ToObject(responseText.Replace("\\\"", "\""));

        //if(jsonData != null) {
        //    if(jsonData.IsObject) {

        //        JsonData dataNode = jsonData["data"];

        //        if(dataNode != null) {
        //            if(dataNode.IsArray) {

        //                for(int i = 0; i < dataNode.Count; i++) {

        //                    GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();

        //                    var data = dataNode[i];

        //                    JsonData user = data["user"];
        //                    JsonData application = data["application"];
        //                    JsonData score = data["score"];

        //                    if(score != null) {
        //                        if(score.IsInt) {
        //                            leaderboardItem.value = float.Parse(score.ToString());
        //                            leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");
        //                        }
        //                    }

        //                    if(user != null) {
        //                        if(user.IsObject) {

        //                            JsonData name = user["name"];
        //                            if(name != null) {
        //                                if(name.IsString) {
        //                                    string nameValue = name.ToString();
        //                                    if(!string.IsNullOrEmpty(nameValue)) {
        //                                        leaderboardItem.username = nameValue;
        //                                    }
        //                                }
        //                            }

        //                            JsonData id = user["id"];
        //                            if(id != null) {
        //                                if(id.IsString) {
        //                                    string idValue = id.ToString();
        //                                    if(!string.IsNullOrEmpty(idValue)) {
        //                                        leaderboardItem.userId = idValue;
        //                                    }
        //                                }
        //                            }

        //                            leaderboardItem.network = "facebook";
        //                            leaderboardItem.name = leaderboardItem.username;
        //                            leaderboardItem.type = "int";
        //                            leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);
        //                            ;
        //                        }
        //                    }

        //                    if(application != null) {
        //                        if(application.IsObject) {
        //                            JsonData name = application["name"];
        //                            if(name != null) {
        //                                if(name.IsString) {
        //                                    string nameValue = name.ToString();
        //                                    if(!string.IsNullOrEmpty(nameValue)) {
        //                                        leaderboardData.appName = nameValue;
        //                                    }
        //                                }
        //                            }

        //                            JsonData namespaceNode = application["name"];
        //                            if(namespaceNode != null) {
        //                                if(namespaceNode.IsString) {
        //                                    string namespaceValue = namespaceNode.ToString();
        //                                    if(!string.IsNullOrEmpty(namespaceValue)) {
        //                                        leaderboardData.appNamespace = namespaceValue;
        //                                    }
        //                                }
        //                            }

        //                            JsonData appId = application["id"];
        //                            if(appId != null) {
        //                                if(appId.IsString) {
        //                                    string appIdValue = appId.ToString();
        //                                    if(!string.IsNullOrEmpty(appIdValue)) {
        //                                        leaderboardData.appId = appIdValue;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    leaderboardItems.Add(leaderboardItem);
        //                }
        //            }
        //        }
        //    }
        //}

        leaderboardData.leaderboards.Add("high-score", leaderboardItems);

        return leaderboardData;
    }

    #endregion

    // -------------------------------------------------------------------------
    // EVENTS

    #region EVENTS

    private void InitEvents(string networkTypeTo) {

#if GAMENETWORK_USE_UNITY

        Messenger<ILocalUser>.AddListener(
            GameNetworkUnityMessages.gameNetworkUnityLoggedIn,
            onGameNetworkUnityAuthenticated);

        Messenger<IAchievement[]>.AddListener(
            GameNetworkUnityMessages.gameNetworkUnityAchievements,
            onGameNetworkUnityAchievements);

        Messenger<IAchievementDescription[]>.AddListener(
            GameNetworkUnityMessages.gameNetworkUnityAchievementDescriptions,
            onGameNetworkUnityAchievementDescriptions);

        Messenger<IScore[]>.AddListener(
            GameNetworkUnityMessages.gameNetworkUnityLeaderboardScores,
            onGameNetworkUnityLeaderboardScores);
#endif

        if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31        
            GameCenterManager.playerAuthenticatedEvent += playerAuthenticated;
            GameCenterManager.achievementsLoadedEvent += achievementsLoaded;
            GameCenterManager.achievementMetadataLoadedEvent += achievementMetadataLoaded;
#endif
        }
        else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31        

            // Fired when authentication succeeds. Includes the user_id
            GPGManager.authenticationSucceededEvent += authenticationSucceededEvent;
            
            // Fired when authentication fails
            GPGManager.authenticationFailedEvent += authenticationFailedEvent;
            
            // iOS only. Fired when the user signs out. This could happen if in a leaderboard they touch the settings button and logout from there.
            GPGManager.userSignedOutEvent += userSignedOutEvent;
            
            // Fired when data fails to reload for a key. This particular model data is usually the player info or leaderboard/achievment metadata that is auto loaded.
            GPGManager.reloadDataForKeyFailedEvent += reloadDataForKeyFailedEvent;
            
            // Fired when data is reloaded for a key
            GPGManager.reloadDataForKeySucceededEvent += reloadDataForKeySucceededEvent;
            
            // Android only. Fired when a license check fails
            GPGManager.licenseCheckFailedEvent += licenseCheckFailedEvent;
            
            
            // ##### ##### ##### ##### ##### ##### #####
            // ## Cloud Data
            // ##### ##### ##### ##### ##### ##### #####
            
            // Fired when loading cloud data fails
            GPGManager.loadCloudDataForKeyFailedEvent += loadCloudDataForKeyFailedEvent;
            
            // Fired when loading cloud data succeeds and includes the key and data
            GPGManager.loadCloudDataForKeySucceededEvent += loadCloudDataForKeySucceededEvent;
            
            // Fired when updating cloud data fails
            GPGManager.updateCloudDataForKeyFailedEvent += updateCloudDataForKeyFailedEvent;
            
            // Fired when updating cloud data succeeds and includes the key and data
            GPGManager.updateCloudDataForKeySucceededEvent += updateCloudDataForKeySucceededEvent;
            
            // Fired when clearing cloud data fails
            GPGManager.clearCloudDataForKeyFailedEvent += clearCloudDataForKeyFailedEvent;
            
            // Fired when clearing cloud data succeeds and includes the key
            GPGManager.clearCloudDataForKeySucceededEvent += clearCloudDataForKeySucceededEvent;
            
            // Fired when deleting cloud data fails
            GPGManager.deleteCloudDataForKeyFailedEvent += deleteCloudDataForKeyFailedEvent;
            
            // Fired when deleting cloud data succeeds and includes the key
            GPGManager.deleteCloudDataForKeySucceededEvent += deleteCloudDataForKeySucceededEvent;
            
            
            // ##### ##### ##### ##### ##### ##### #####
            // ## Achievements
            // ##### ##### ##### ##### ##### ##### #####
            
            // Fired when unlocking an achievement fails. Provides the achievmentId and the error in that order.
            GPGManager.unlockAchievementFailedEvent += unlockAchievementFailedEvent;
            
            // Fired when unlocking an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
            GPGManager.unlockAchievementSucceededEvent += unlockAchievementSucceededEvent;
            
            // Fired when incrementing an achievement fails. Provides the achievmentId and the error in that order.
            GPGManager.incrementAchievementFailedEvent += incrementAchievementFailedEvent;
            
            // Fired when incrementing an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
            GPGManager.incrementAchievementSucceededEvent += incrementAchievementSucceededEvent;
            
            // Fired when revealing an achievement fails. Provides the achievmentId and the error in that order.
            GPGManager.revealAchievementFailedEvent += revealAchievementFailedEvent;
            
            // Fired when revealing an achievement succeeds. The string lets you know the achievementId.
            GPGManager.revealAchievementSucceededEvent += revealAchievementSucceededEvent;
            
            // ##### ##### ##### ##### ##### ##### #####
            // ## Leaderboards
            // ##### ##### ##### ##### ##### ##### #####
            
            // Fired when submitting a score fails. Provides the leaderboardId and the error in that order.
            GPGManager.submitScoreFailedEvent += submitScoreFailedEvent;
            
            // Fired when submitting a scores succeeds. Returns the leaderboardId and a dictionary with some extra data with the fields from
            // the GPGScoreReport class: https://developers.google.com/games/services/ios/api/interface_g_p_g_score_report
            GPGManager.submitScoreSucceededEvent += submitScoreSucceededEvent;
            
            // Fired when loading scores fails. Provides the leaderboardId and the error in that order.
            GPGManager.loadScoresFailedEvent += loadScoresFailedEvent;
            
            // Fires when loading scores succeeds
            GPGManager.loadScoresSucceededEvent += loadScoresSucceededEvent;

            PlayGameServices.init(AppConfigs.gameNetworkGooglePlayGameServicesClientId,
                                  false, true, true);
#endif
        }
    }

    private void RemoveEvents(string networkTypeTo) {

#if GAMENETWORK_USE_UNITY

        Messenger<ILocalUser>.RemoveListener(
            GameNetworkUnityMessages.gameNetworkUnityLoggedIn,
            onGameNetworkUnityAuthenticated);

        Messenger<IAchievement[]>.RemoveListener(
            GameNetworkUnityMessages.gameNetworkUnityAchievements,
            onGameNetworkUnityAchievements);

        Messenger<IAchievementDescription[]>.RemoveListener(
            GameNetworkUnityMessages.gameNetworkUnityAchievementDescriptions,
            onGameNetworkUnityAchievementDescriptions);

        Messenger<IScore[]>.RemoveListener(
            GameNetworkUnityMessages.gameNetworkUnityLeaderboardScores,
            onGameNetworkUnityLeaderboardScores);

#endif

        if(networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31    
            GameCenterManager.playerAuthenticatedEvent -= playerAuthenticated;
            GameCenterManager.achievementsLoadedEvent -= achievementsLoaded;
            GameCenterManager.achievementMetadataLoadedEvent -= achievementMetadataLoaded;
#endif
        }
        else if(networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31        
        
        // Fired when authentication succeeds. Includes the user_id
            GPGManager.authenticationSucceededEvent -= authenticationSucceededEvent;
        
        // Fired when authentication fails
            GPGManager.authenticationFailedEvent -= authenticationFailedEvent;
        
        // iOS only. Fired when the user signs out. This could happen if in a leaderboard they touch the settings button and logout from there.
            GPGManager.userSignedOutEvent -= userSignedOutEvent;
        
        // Fired when data fails to reload for a key. This particular model data is usually the player info or leaderboard/achievment metadata that is auto loaded.
            GPGManager.reloadDataForKeyFailedEvent -= reloadDataForKeyFailedEvent;
        
        // Fired when data is reloaded for a key
            GPGManager.reloadDataForKeySucceededEvent -= reloadDataForKeySucceededEvent;
        
        // Android only. Fired when a license check fails
            GPGManager.licenseCheckFailedEvent -= licenseCheckFailedEvent;
        
        
        // ##### ##### ##### ##### ##### ##### #####
        // ## Cloud Data
        // ##### ##### ##### ##### ##### ##### #####
        
        // Fired when loading cloud data fails
            GPGManager.loadCloudDataForKeyFailedEvent -= loadCloudDataForKeyFailedEvent;
        
        // Fired when loading cloud data succeeds and includes the key and data
            GPGManager.loadCloudDataForKeySucceededEvent -= loadCloudDataForKeySucceededEvent;
        
        // Fired when updating cloud data fails
            GPGManager.updateCloudDataForKeyFailedEvent -= updateCloudDataForKeyFailedEvent;
        
        // Fired when updating cloud data succeeds and includes the key and data
            GPGManager.updateCloudDataForKeySucceededEvent -= updateCloudDataForKeySucceededEvent;
        
        // Fired when clearing cloud data fails
            GPGManager.clearCloudDataForKeyFailedEvent -= clearCloudDataForKeyFailedEvent;
        
        // Fired when clearing cloud data succeeds and includes the key
            GPGManager.clearCloudDataForKeySucceededEvent -= clearCloudDataForKeySucceededEvent;
        
        // Fired when deleting cloud data fails
            GPGManager.deleteCloudDataForKeyFailedEvent -= deleteCloudDataForKeyFailedEvent;
        
        // Fired when deleting cloud data succeeds and includes the key
            GPGManager.deleteCloudDataForKeySucceededEvent -= deleteCloudDataForKeySucceededEvent;
        
        
        // ##### ##### ##### ##### ##### ##### #####
        // ## Achievements
        // ##### ##### ##### ##### ##### ##### #####
        
        // Fired when unlocking an achievement fails. Provides the achievmentId and the error in that order.
            GPGManager.unlockAchievementFailedEvent -= unlockAchievementFailedEvent;
        
        // Fired when unlocking an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
            GPGManager.unlockAchievementSucceededEvent -= unlockAchievementSucceededEvent;
        
        // Fired when incrementing an achievement fails. Provides the achievmentId and the error in that order.
            GPGManager.incrementAchievementFailedEvent -= incrementAchievementFailedEvent;
        
        // Fired when incrementing an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
            GPGManager.incrementAchievementSucceededEvent -= incrementAchievementSucceededEvent;
        
        // Fired when revealing an achievement fails. Provides the achievmentId and the error in that order.
            GPGManager.revealAchievementFailedEvent -= revealAchievementFailedEvent;
        
        // Fired when revealing an achievement succeeds. The string lets you know the achievementId.
            GPGManager.revealAchievementSucceededEvent -= revealAchievementSucceededEvent;
        
        // ##### ##### ##### ##### ##### ##### #####
        // ## Leaderboards
        // ##### ##### ##### ##### ##### ##### #####
        
        // Fired when submitting a score fails. Provides the leaderboardId and the error in that order.
            GPGManager.submitScoreFailedEvent -= submitScoreFailedEvent;
        
        // Fired when submitting a scores succeeds. Returns the leaderboardId and a dictionary with some extra data with the fields from
        // the GPGScoreReport class: https://developers.google.com/games/services/ios/api/interface_g_p_g_score_report
            GPGManager.submitScoreSucceededEvent -= submitScoreSucceededEvent;
        
        // Fired when loading scores fails. Provides the leaderboardId and the error in that order.
            GPGManager.loadScoresFailedEvent -= loadScoresFailedEvent;
        
        // Fires when loading scores succeeds
            GPGManager.loadScoresSucceededEvent -= loadScoresSucceededEvent;
                
#endif        
        }
    }

    // -------------------------------------------------------------------------
    // EVENTS UNITY

#if GAMENETWORK_USE_UNITY

    //

    void gameNetworkUnityPlayerAuthenticated() {
        Debug.Log("GameNetworks:gameNetworkUnityPlayerAuthenticated");
        SetLocalProfileToNetworkUsername();
        GetAchievements();
    }

    void onGameNetworkUnityAuthenticated(ILocalUser user) {
        Debug.Log("GameNetworks:onGameNetworkUnityAuthenticated:" + user.userName);
        gameNetworkUnityPlayerAuthenticated();
    }

    //

    void gameNetworkUnityAchievementsLoaded(IAchievement[] achievements) {
        LogUtil.Log("GameNetworks:gameNetworkUnityAchievementsLoaded");
        gameNetworkAchievements = achievements;
        CheckAchievementsState();
    }

    void onGameNetworkUnityAchievements(IAchievement[] achievements) {
        Debug.Log("GameNetworks:onGameNetworkUnityAchievements:" + achievements.Length);
        gameNetworkUnityAchievementsLoaded(achievements);
    }

    void gameNetworkUnityAchievementMetadataLoaded(IAchievementDescription[] achievementsMeta) {
        LogUtil.Log("GameNetworks:gameNetworkUnityAchievementMetadataLoaded");
        gameNetworkAchievementsMeta = achievementsMeta;
        CheckAchievementsState();
    }

    void onGameNetworkUnityAchievementDescriptions(IAchievementDescription[] achievementsMeta) {
        Debug.Log("GameNetworks:onGameNetworkUnityAchievementDescriptions:" + achievementsMeta.Length);
        gameNetworkUnityAchievementMetadataLoaded(achievementsMeta);
    }

    //

    void onGameNetworkUnityLeaderboardScores(IScore[] scores) {
        Debug.Log("GameNetworks:onGameNetworkUnityLeaderboardScores:" + scores.Length);
        gameNetworkUnityLeaderboardScoresLoaded(scores);
    }

    void gameNetworkUnityLeaderboardScoresLoaded(IScore[] scores) {
        LogUtil.Log("GameNetworks:gameNetworkUnityLeaderboardScoresLoaded");
        gameNetworkLeaderboardScores = scores;
    }

    //

#endif


    // -------------------------------------------------------------------------
    // EVENTS IOS GAME CENTER UNITY

#if GAMENETWORK_IOS_APPLE_GAMECENTER_UNITY
    /*
void achievementsLoaded(List<GameCenterAchievement> achievementsNetworkResult) {
    LogUtil.Log("GameNetworks:GameCenter:achievementsLoaded"); 
    gameNetworkAchievements = achievementsNetworkResult;
    CheckAchievementsState();
}

void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementsMetaNetworkResult) {
    LogUtil.Log("GameNetworks:GameCenter:achievementMetadataLoaded"); 
    gameNetworkAchievementsMeta = achievementsMetaNetworkResult;
    CheckAchievementsState();
}
*/

#endif

    // -------------------------------------------------------------------------
    // EVENTS ANDROID GOOGLE PLAY UNITY

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_UNITY

#endif

    // -------------------------------------------------------------------------
    // EVENTS IOS GAME CENTER PRIME31

#if GAMENETWORK_IOS_APPLE_GAMECENTER_PRIME31
    void achievementsLoaded(List<GameCenterAchievement> achievementsNetworkResult) {
        LogUtil.Log("GameNetworks:GameCenter:achievementsLoaded"); 
        gameNetworkAchievements = achievementsNetworkResult;
        CheckAchievementsState();
    }
    
    void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementsMetaNetworkResult) {
        LogUtil.Log("GameNetworks:GameCenter:achievementMetadataLoaded"); 
        gameNetworkAchievementsMeta = achievementsMetaNetworkResult;
        CheckAchievementsState();
    }
    
    void playerAuthenticated() {
        Debug.Log("GameNetworks:GameCenter:playerAuthenticated"); 
        SetLocalProfileToNetworkUsername();
        GetAchievements();
    }
#endif

    // -------------------------------------------------------------------------
    // EVENTS ANDROID GOOGLE PLAY PRIME31

#if GAMENETWORK_ANDROID_GOOGLE_PLAY_PRIME31
    // Fired when authentication succeeds. Includes the user_id

    public void authenticationSucceededEvent(string val) {
            LogUtil.Log("GameNetworks:PlayServices:authenticationSucceededEvent:" + " val:" + val);
            SetLocalProfileToNetworkUsername();
            GetAchievements();
    }
    
    // Fired when authentication fails
    public void authenticationFailedEvent(string val) {
            LogUtil.Log("GameNetworks:PlayServices:authenticationFailedEvent:" + " val:" + val);
    }
    
    // iOS only. Fired when the user signs out. This could happen if in a leaderboard they touch the settings button and logout from there.
    public void userSignedOutEvent() {
            LogUtil.Log("GameNetworks:PlayServices:userSignedOutEvent"); 
    }
    
    // Fired when data fails to reload for a key. This particular model data is usually the player info or leaderboard/achievment metadata that is auto loaded.
    public void reloadDataForKeyFailedEvent(string val) {
            LogUtil.Log("GameNetworks:PlayServices:reloadDataForKeyFailedEvent:" + " val:" + val);
    }

    // Fired when data is reloaded for a key
    public void reloadDataForKeySucceededEvent(string val) {
            LogUtil.Log("GameNetworks:PlayServices:reloadDataForKeySucceededEvent:" + " val:" + val);
    }
    
    // Android only. Fired when a license check fails
    public void licenseCheckFailedEvent() {
            LogUtil.Log("GameNetworks:PlayServices:licenseCheckFailedEvent");
    }
    
    // ##### ##### ##### ##### ##### ##### #####
    // ## Cloud Data
    // ##### ##### ##### ##### ##### ##### #####
    
    // Fired when loading cloud data fails
    public void loadCloudDataForKeyFailedEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:loadCloudDataForKeyFailedEvent:" + " val:" + val);
    }

    // Fired when loading cloud data succeeds and includes the key and data
    public void loadCloudDataForKeySucceededEvent(int key, string val) {
        LogUtil.Log("GameNetworks:PlayServices:loadCloudDataForKeySucceededEvent:" + " key:" + key + " val:" + val);
    }
    
    // Fired when updating cloud data fails
    public void updateCloudDataForKeyFailedEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:updateCloudDataForKeyFailedEvent:" + " val:" + val);
    }
    
    // Fired when updating cloud data succeeds and includes the key and data
    public void updateCloudDataForKeySucceededEvent(int key, string val) {
        LogUtil.Log("GameNetworks:PlayServices:updateCloudDataForKeySucceededEvent:" + " key:" + key + " val:" + val);      
    }
    
    // Fired when clearing cloud data fails
    public void clearCloudDataForKeyFailedEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:clearCloudDataForKeyFailedEvent:" + " val:" + val);        
    }
    
    // Fired when clearing cloud data succeeds and includes the key
    public void clearCloudDataForKeySucceededEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:clearCloudDataForKeySucceededEvent:" + " val:" + val);      
    }
    
    // Fired when deleting cloud data fails
    public void deleteCloudDataForKeyFailedEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:deleteCloudDataForKeyFailedEvent:" + " val:" + val);  
    }
    
    // Fired when deleting cloud data succeeds and includes the key
    public void deleteCloudDataForKeySucceededEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:deleteCloudDataForKeySucceededEvent:" + " val:" + val);
    }
    
    // ##### ##### ##### ##### ##### ##### #####
    // ## Achievements
    // ##### ##### ##### ##### ##### ##### #####
    
    // Fired when unlocking an achievement fails. Provides the achievmentId and the error in that order.
    public void unlockAchievementFailedEvent(string val, string error) {
        LogUtil.Log("GameNetworks:PlayServices:unlockAchievementFailedEvent:" + " val:" + val + " error:" + error);
    }

    // Fired when unlocking an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
    public void unlockAchievementSucceededEvent(string val, bool completed) {
        LogUtil.Log("GameNetworks:PlayServices:unlockAchievementSucceededEvent:" + " val:" + val + " completed:" + completed);
    }
    
    // Fired when incrementing an achievement fails. Provides the achievmentId and the error in that order.
    public void incrementAchievementFailedEvent(string val, string error) {
        LogUtil.Log("GameNetworks:PlayServices:incrementAchievementFailedEvent:" + " val:" + val + " error:" + error);
    }
    
    // Fired when incrementing an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
    public void incrementAchievementSucceededEvent(string val, bool completed) {
        LogUtil.Log("GameNetworks:PlayServices:incrementAchievementSucceededEvent:" + " val:" + val + " completed:" + completed); 
    }
    
    // Fired when revealing an achievement fails. Provides the achievmentId and the error in that order.
    public void revealAchievementFailedEvent(string val, string error) {
        LogUtil.Log("GameNetworks:PlayServices:revealAchievementFailedEvent:" + " val:" + val + " error:" + error);
    }
    
    // Fired when revealing an achievement succeeds. The string lets you know the achievementId.
    public void revealAchievementSucceededEvent(string val) {
        LogUtil.Log("GameNetworks:PlayServices:revealAchievementSucceededEvent:" + " val:" + val);
    }
    
    // ##### ##### ##### ##### ##### ##### #####
    // ## Leaderboards
    // ##### ##### ##### ##### ##### ##### #####
    
    // Fired when submitting a score fails. Provides the leaderboardId and the error in that order.
    public void submitScoreFailedEvent(string val, string error) {
        LogUtil.Log("GameNetworks:PlayServices:submitScoreFailedEvent:" + " val:" + val + " error:" + error);     
    }
    
    // Fired when submitting a scores succeeds. Returns the leaderboardId and a dictionary with some extra data with the fields from
    // the GPGScoreReport class: https://developers.google.com/games/services/ios/api/interface_g_p_g_score_report
    public void submitScoreSucceededEvent(string val, Dictionary<string,object> data) {
        LogUtil.Log("GameNetworks:PlayServices:submitScoreSucceededEvent:" + " val:" + val + " data:" + data.ToJson());
    }
    
    // Fired when loading scores fails. Provides the leaderboardId and the error in that order.
    public void loadScoresFailedEvent(string val, string error) {
        LogUtil.Log("GameNetworks:PlayServices:loadScoresFailedEvent:" + " val:" + val + " error:" + error);
    }
    
    // Fires when loading scores succeeds
    public void loadScoresSucceededEvent(List<GPGScore> val) {
            LogUtil.Log("GameNetworks:PlayServices:loadScoresSucceededEvent:" + val.ToJson());
    }
#endif

    #endregion

    #region SCRATCH
    /*
    // Fired when authentication succeeds. Includes the user_id
    public static event Action<string> authenticationSucceededEvent;
    
    // Fired when authentication fails
    public static event Action<string> authenticationFailedEvent;
    
    // iOS only. Fired when the user signs out. This could happen if in a leaderboard they touch the settings button and logout from there.
    public static event Action userSignedOutEvent;
    
    // Fired when data fails to reload for a key. This particular model data is usually the player info or leaderboard/achievment metadata that is auto loaded.
    public static event Action<string> reloadDataForKeyFailedEvent;
    
    // Fired when data is reloaded for a key
    public static event Action<string> reloadDataForKeySucceededEvent;
    
    // Android only. Fired when a license check fails
    public static event Action licenseCheckFailedEvent;
    
    
    // ##### ##### ##### ##### ##### ##### #####
    // ## Cloud Data
    // ##### ##### ##### ##### ##### ##### #####
    
    // Fired when loading cloud data fails
    public static event Action<string> loadCloudDataForKeyFailedEvent;
    
    // Fired when loading cloud data succeeds and includes the key and data
    public static event Action<int,string> loadCloudDataForKeySucceededEvent;
    
    // Fired when updating cloud data fails
    public static event Action<string> updateCloudDataForKeyFailedEvent;
    
    // Fired when updating cloud data succeeds and includes the key and data
    public static event Action<int,string> updateCloudDataForKeySucceededEvent;
    
    // Fired when clearing cloud data fails
    public static event Action<string> clearCloudDataForKeyFailedEvent;
    
    // Fired when clearing cloud data succeeds and includes the key
    public static event Action<string> clearCloudDataForKeySucceededEvent;
    
    // Fired when deleting cloud data fails
    public static event Action<string> deleteCloudDataForKeyFailedEvent;
    
    // Fired when deleting cloud data succeeds and includes the key
    public static event Action<string> deleteCloudDataForKeySucceededEvent;
    
    
    // ##### ##### ##### ##### ##### ##### #####
    // ## Achievements
    // ##### ##### ##### ##### ##### ##### #####
    
    // Fired when unlocking an achievement fails. Provides the achievmentId and the error in that order.
    public static event Action<string,string> unlockAchievementFailedEvent;
    
    // Fired when unlocking an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
    public static event Action<string,bool> unlockAchievementSucceededEvent;
    
    // Fired when incrementing an achievement fails. Provides the achievmentId and the error in that order.
    public static event Action<string,string> incrementAchievementFailedEvent;
    
    // Fired when incrementing an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
    public static event Action<string,bool> incrementAchievementSucceededEvent;
    
    // Fired when revealing an achievement fails. Provides the achievmentId and the error in that order.
    public static event Action<string,string> revealAchievementFailedEvent;
    
    // Fired when revealing an achievement succeeds. The string lets you know the achievementId.
    public static event Action<string> revealAchievementSucceededEvent;
    
    
    // ##### ##### ##### ##### ##### ##### #####
    // ## Leaderboards
    // ##### ##### ##### ##### ##### ##### #####
    
    // Fired when submitting a score fails. Provides the leaderboardId and the error in that order.
    public static event Action<string,string> submitScoreFailedEvent;
    
    // Fired when submitting a scores succeeds. Returns the leaderboardId and a dictionary with some extra data with the fields from
    // the GPGScoreReport class: https://developers.google.com/games/services/ios/api/interface_g_p_g_score_report
    public static event Action<string,Dictionary<string,object>> submitScoreSucceededEvent;
    
    // Fired when loading scores fails. Provides the leaderboardId and the error in that order.
    public static event Action<string,string> loadScoresFailedEvent;
    
    // Fires when loading scores succeeds
    public static event Action<List<GPGScore>> loadScoresSucceededEvent;
    
    
    */


    /*
     PLAY SERVICES DOCS
     
     // Sets the placement of the welcome back toast
public static void setWelcomeBackToastSettings( GPGToastPlacement placement, int offset )

// Sets the placement of the achievement toast
public static void setAchievementToastSettings( GPGToastPlacement placement, int offset )

// iOS only. This should be called at application launch. It will attempt to authenticate the user silently. If you need the AppState scope permission
// (cloud storage requires it) pass true for the requestAppStateScope parameter
public static void init( string clientId, bool requestAppStateScope, bool fetchMetadataAfterAuthentication = true, bool pauseUnityWhileShowingFullScreenViews = true )

// Starts the authentication process which will happen either in the Google+ app, Chrome or Mobile Safari
public static void authenticate()

// Logs the user out
public static void signOut()

// Checks to see if there is a currently signed in user. Utilizes a terrible hack due to a bug with Play Game Services connection status.
public static bool isSignedIn()

// Gets the logged in players details
public static GPGPlayerInfo getLocalPlayerInfo()


// ##### ##### ##### ##### ##### ##### #####
// ## Cloud Data
// ##### ##### ##### ##### ##### ##### #####

// Sets the cloud data for the given key
public static void setStateData( string data, int key )

// Gets the cloud data for the given key
public static string stateDataForKey( int key )

// Loads cloud data for the given key. The associated loadCloudDataForKeyFailed/Succeeded event will fire when complete.
public static void loadCloudDataForKey( int key, bool useRemoteDataForConflictResolution = true )

// Updates cloud data for the given key. The associated updateCloudDataForKeyFailed/Succeeded event will fire when complete.
public static void updateCloudDataForKey( int key, bool useRemoteDataForConflictResolution = true )

// Clears cloud data for the given key
public static void clearCloudDataForKey( int key, bool useRemoteDataForConflictResolution = true )

// Deletes cloud data for the given key
public static void deleteCloudDataForKey( int key, bool useRemoteDataForConflictResolution = true )


// ##### ##### ##### ##### ##### ##### #####
// ## Achievements
// ##### ##### ##### ##### ##### ##### #####

// Shows the achievements screen
public static void showAchievements()

// Reveals the achievement if it was previously hidden
public static void revealAchievement( string achievementId )

// Unlocks the achievement
public static void unlockAchievement( string achievementId, bool showsCompletionNotification = true )

// Increments the achievement. Only works on achievements setup as incremental in the Google Developer Console.
// FIres the incrementAchievementFailed/Succeeded event when complete.
public static void incrementAchievement( string achievementId, int numSteps )

// Gets the achievement metadata
public static List<GPGAchievementMetadata> getAllAchievementMetadata()


// ##### ##### ##### ##### ##### ##### #####
// ## Leaderboards
// ##### ##### ##### ##### ##### ##### #####

// Shows the requested leaderboard and time scope
public static void showLeaderboard( string leaderboardId, GPGLeaderboardTimeScope timeScope )

// Shows a list of all learderboards
public static void showLeaderboards()

// Submits a score for the given leaderboard. Fires the submitScoreFailed/Succeeded event when complete.
public static void submitScore( string leaderboardId, System.Int64 score )

// Loads scores for the given leaderboard. Fires the loadScoresFailed/Succeeded event when complete.
public static void loadScoresForLeaderboard( string leaderboardId, GPGLeaderboardTimeScope timeScope, bool isSocial, bool personalWindow )

// Gets all the leaderboards metadata
public static List<GPGLeaderboardMetadata> getAllLeaderboardMetadata()
     
     
     EVENTS
     
     // Fired when authentication succeeds. Includes the user_id
public static event Action<string> authenticationSucceededEvent;

// Fired when authentication fails
public static event Action<string> authenticationFailedEvent;

// iOS only. Fired when the user signs out. This could happen if in a leaderboard they touch the settings button and logout from there.
public static event Action userSignedOutEvent;

// Fired when data fails to reload for a key. This particular model data is usually the player info or leaderboard/achievment metadata that is auto loaded.
public static event Action<string> reloadDataForKeyFailedEvent;

// Fired when data is reloaded for a key
public static event Action<string> reloadDataForKeySucceededEvent;

// Android only. Fired when a license check fails
public static event Action licenseCheckFailedEvent;


// ##### ##### ##### ##### ##### ##### #####
// ## Cloud Data
// ##### ##### ##### ##### ##### ##### #####

// Fired when loading cloud data fails
public static event Action<string> loadCloudDataForKeyFailedEvent;

// Fired when loading cloud data succeeds and includes the key and data
public static event Action<int,string> loadCloudDataForKeySucceededEvent;

// Fired when updating cloud data fails
public static event Action<string> updateCloudDataForKeyFailedEvent;

// Fired when updating cloud data succeeds and includes the key and data
public static event Action<int,string> updateCloudDataForKeySucceededEvent;

// Fired when clearing cloud data fails
public static event Action<string> clearCloudDataForKeyFailedEvent;

// Fired when clearing cloud data succeeds and includes the key
public static event Action<string> clearCloudDataForKeySucceededEvent;

// Fired when deleting cloud data fails
public static event Action<string> deleteCloudDataForKeyFailedEvent;

// Fired when deleting cloud data succeeds and includes the key
public static event Action<string> deleteCloudDataForKeySucceededEvent;


// ##### ##### ##### ##### ##### ##### #####
// ## Achievements
// ##### ##### ##### ##### ##### ##### #####

// Fired when unlocking an achievement fails. Provides the achievmentId and the error in that order.
public static event Action<string,string> unlockAchievementFailedEvent;

// Fired when unlocking an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
public static event Action<string,bool> unlockAchievementSucceededEvent;

// Fired when incrementing an achievement fails. Provides the achievmentId and the error in that order.
public static event Action<string,string> incrementAchievementFailedEvent;

// Fired when incrementing an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
public static event Action<string,bool> incrementAchievementSucceededEvent;

// Fired when revealing an achievement fails. Provides the achievmentId and the error in that order.
public static event Action<string,string> revealAchievementFailedEvent;

// Fired when revealing an achievement succeeds. The string lets you know the achievementId.
public static event Action<string> revealAchievementSucceededEvent;


// ##### ##### ##### ##### ##### ##### #####
// ## Leaderboards
// ##### ##### ##### ##### ##### ##### #####

// Fired when submitting a score fails. Provides the leaderboardId and the error in that order.
public static event Action<string,string> submitScoreFailedEvent;

// Fired when submitting a scores succeeds. Returns the leaderboardId and a dictionary with some extra data with the fields from
// the GPGScoreReport class: https://developers.google.com/games/services/ios/api/interface_g_p_g_score_report
public static event Action<string,Dictionary<string,object>> submitScoreSucceededEvent;

// Fired when loading scores fails. Provides the leaderboardId and the error in that order.
public static event Action<string,string> loadScoresFailedEvent;

// Fires when loading scores succeeds
public static event Action<List<GPGScore>> loadScoresSucceededEvent;
     
     */
    #endregion
}