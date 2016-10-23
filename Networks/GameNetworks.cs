#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used. 

// TODO wire in UNITY native social/game networks

#if UNITY_IPHONE
#define GAMENETWORK_IOS_APPLE_GAMECENTER
#endif

#if UNITY_ANDROID
//#define GAMENETWORK_ANDROID_GOOGLE_PLAY
//#define GAMENETWORK_ANDROID_AMAZON_CIRCLE
//#define GAMENETWORK_ANDROID_SAMSUNG
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;
using Prime31;

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
    
#if GAMENETWORK_IOS_APPLE_GAMECENTER
    public static bool gameNetworkiOSAppleGameCenterEnabled = true;
#else
    public static bool gameNetworkiOSAppleGameCenterEnabled = false;
#endif
    
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
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

#if GAMENETWORK_IOS_APPLE_GAMECENTER
    //[NonSerialized]
    //public GameCenterManager gameCenterManager;
    //[NonSerialized]
    //public GameCenterEventListener gameCenterEventListener;
    [NonSerialized]
    public List<GameCenterAchievement> gameCenterAchievementsNetwork;
    [NonSerialized]
    public List<GameCenterAchievementMetadata> gameCenterAchievementsMetaNetwork;
#endif
    
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
    [NonSerialized]
    public GPGManager gamePlayServicesManager;
    [NonSerialized]
    public GPGSEventListener gamePlayServicesEventListener;
    //[NonSerialized]
    //public List<GPGAchievementMetadata> gamePlayServicesAchievementsNetwork;
    [NonSerialized]
    public List<GPGAchievementMetadata> gamePlayServicesAchievementsMetaNetwork;
#endif
        
    public static string currentLoggedInUserNetwork = "";
    
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
    public static string currentNetwork = GameNetworkType.gameNetworkGooglePlayServices;
#elif GAMENETWORK_IOS_APPLE_GAMECENTER
    public static string currentNetwork = GameNetworkType.gameNetworkAppleGameCenter;
#else    
    public static string currentNetwork = "";
#endif
    
    public static GameNetworks Instance;
    
    public void Awake() {
        
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
        
    void Start() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
        gameCenterAchievementsNetwork = new List<GameCenterAchievement>();
        gameCenterAchievementsMetaNetwork = new List<GameCenterAchievementMetadata>();
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        //achievementsNetwork = new List<GameCenterAchievement>();
        gamePlayServicesAchievementsMetaNetwork = new List<GPGAchievementMetadata>();
#endif
        
        InvokeRepeating("checkThirdPartyNetworkLoggedInUser", 3, 3);
    }
    
    void OnDisable() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
        RemoveEvents(GameNetworkType.gameNetworkAppleGameCenter);
#endif
        
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        RemoveEvents(GameNetworkType.gameNetworkGooglePlayServices);
#endif
    }   
    
    // -------------------------------------------------------------------------
    // NETWORK USER CHECK

    public static void CheckThirdPartyNetworkLoggedInUser() {
        if (Instance != null) {
            Instance.checkThirdPartyNetworkLoggedInUser();
        }
    }
    
    void checkThirdPartyNetworkLoggedInUser() {
        checkThirdPartyNetworkLoggedInUser(currentNetwork);
    }
    
    public static void CheckThirdPartyNetworkLoggedInUser(string networkType) {
        if (Instance != null) {
            Instance.checkThirdPartyNetworkLoggedInUser(networkType);
        }
    }
    
    void checkThirdPartyNetworkLoggedInUser(string networkType) {
        // Check the third party network logged in name and the current logged in name, 
        // if the name is different change user.  
        // This helps the final case of when a user changes gamecenter while the app
        // is running and the auth changed event is just not broadcasting.
        
        if (GameState.Instance != null
            && GameProfiles.Current != null) {
            
            currentLoggedInUserNetwork = GameProfiles.Current.username;

            if ((gameNetworkiOSAppleGameCenterEnabled
                || gameNetworkAndroidGooglePlayEnabled)
                && IsThirdPartyNetworkUserAuthenticated(networkType)) {
                
                string networkName = GetNetworkUsername();
                if (!string.IsNullOrEmpty(networkName)) {
                    currentLoggedInUserNetwork = networkName;
                }
            }
    
            if (currentLoggedInUserNetwork != GameProfiles.Current.username) {
                LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: currentLoggedInUserNetwork: " + currentLoggedInUserNetwork);
            
                LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: changed: " + GameProfiles.Current.username);
                GameState.ChangeUser(currentLoggedInUserNetwork);
                LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: GameProfiles.Current.username: " + GameProfiles.Current.username);
            }
        }
    }   
    
    // -------------------------------------------------------------------------
    // LOAD NETWORKS        
    
    public static void LoadNetwork(string networkType) {    
        if (Instance != null) {
            Instance.loadNetwork(networkType);
        }
    }
        
    public void loadNetwork(string networkType) {       
        currentNetwork = networkType;
#if GAMENETWORK_IOS_APPLE_GAMECENTER
        if(networkType == GameNetworkType.gameNetworkAppleGameCenter) {
            InitNetwork();
        }
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        if(networkType == GameNetworkType.gameNetworkGooglePlayServices) {
            InitNetwork();
        }
#endif
    }
    
    public static void InitNetwork() {  
        if (Instance != null) {
            Instance.initNetwork();
        }
    }
    
    public void initNetwork() {     
        if (gameNetworkContainer == null) {
            gameNetworkContainer = new GameObject("GameNetworks");  
        }

#if GAMENETWORK_IOS_APPLE_GAMECENTER
        //gameCenterManager = gameNetworkContainer.AddComponent<GameCenterManager>();
        //gameCenterEventListener = gameNetworkContainer.AddComponent<GameCenterEventListener>();
        
        InitEvents(GameNetworkType.gameNetworkAppleGameCenter);     
        LoginNetwork(GameNetworkType.gameNetworkAppleGameCenter);           
        
        LogUtil.Log("InitNetwork iOS Apple GameCenter init...");
#endif
        
#if GAMENETWORK_ANDROID_GOOGLE_PLAY 
        gamePlayServicesManager = gameNetworkContainer.AddComponent<GPGManager>();
        gamePlayServicesEventListener = gameNetworkContainer.AddComponent<GPGSEventListener>();
        
        InitEvents(GameNetworkType.gameNetworkGooglePlayServices);      
        LoginNetwork(GameNetworkType.gameNetworkGooglePlayServices);    
        
        LogUtil.Log("InitNetwork Android Google Play init...");
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
        
        if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
            isAvailable = isAvailableiOSAppleGameCenter;
        }
        else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
            isAvailable = isAvailableAndroidGooglePlay;
        }
                
        LogUtil.Log("IsThirdPartyNetworkAvailable:isAvailable:" + isAvailable);
    
        return isAvailable;
    }
    
    public static bool isAvailableiOSAppleGameCenter {
        get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER    
            return true;//return GameCenterBinding.isGameCenterAvailable(); 
#else
            return false;
#endif  
        }
    }
    
    public static bool isAvailableAndroidGooglePlay {
        get {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY 

            return true;//PlayGameServices. false;//GameCenterBinding.isGameCenterAvailable();
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
        
        if (networkType == GameNetworkType.gameNetworkAppleGameCenter) {
            isAuthenticated = isAuthenticatediOSAppleGameCenter;
        }
        else if (networkType == GameNetworkType.gameNetworkGooglePlayServices) {
            isAuthenticated = isAuthenticatedAndroidGooglePlay;
        }
                
        //Debug.Log("IsThirdPartyNetworkUserAuthenticated:isAuthenticated:" + isAuthenticated);

        return isAuthenticated;
    }
    
    public static bool isAuthenticatediOSAppleGameCenter {
        get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER    
            return GameCenterBinding.isPlayerAuthenticated();
#else
            return false;
#endif  
        }
    }
    
    public static bool isAuthenticatedAndroidGooglePlay {
        get {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY 
            return PlayGameServices.isSignedIn();
#else
            return false;
#endif  
        }
    }
    
    public bool IsThirdPartyNetworkReady(string networkType) {
        bool isReady = false;
        if (IsThirdPartyNetworkAvailable(networkType)
            && IsThirdPartyNetworkUserAuthenticated(networkType)) {
            isReady = true;         
        }
        return isReady;
    }
    
    // -------------------------------------------------------------------------
    // ACHIEVEMENTS UI      
    
    public static void ShowAchievementsOrLogin() {  
        
        //LogUtil.Log("ShowAchievementsOrLogin");

        if (Instance != null) {
            Instance.showAchievementsOrLogin(currentNetwork);
        }
    }

    public static void ShowAchievementsOrLogin(string networkTypeTo) {  
        
        LogUtil.Log("ShowAchievementsOrLogin:networkTypeTo:" + networkTypeTo);

        if (Instance != null) {
            Instance.showAchievementsOrLogin(networkTypeTo);
        }
    }
    
    public void showAchievementsOrLogin(string networkTypeTo) {
        
        Debug.Log("showAchievementsOrLogin:networkTypeTo:" + networkTypeTo);

        if (IsThirdPartyNetworkAvailable(networkTypeTo)) {
            
            if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                showAchievementsOrLoginiOSAppleGameCenter();
            }
            else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                showAchievementsOrLoginAndroidGooglePlay();
            }
        }   
    }
    
    public static void showAchievementsOrLoginiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
        
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
    }
    
    public static void showAchievementsOrLoginAndroidGooglePlay() {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        
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
    }
    
    // -------------------------------------------------------------------------
    // LEADERBOARDS UI
        
    public static void ShowLeaderboardsOrLogin() {  
        
        Debug.Log("ShowLeaderboardsOrLogin");

        if (Instance != null) {
            Instance.showLeaderboardsOrLogin(currentNetwork);
        }
    }

    public static void ShowLeaderboardsOrLogin(string networkTypeTo) {  
        if (Instance != null) {
            Instance.showLeaderboardsOrLogin(networkTypeTo);
        }
    }
    
    public void showLeaderboardsOrLogin(string networkTypeTo) {
        
        Debug.Log("showLeaderboardsOrLogin:networkTypeTo:" + networkTypeTo);

        if (IsThirdPartyNetworkAvailable(networkTypeTo)) {
            
            if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                showLeaderboardsOrLoginiOSAppleGameCenter();
            }
            else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                showLeaderboardsOrLoginAndroidGooglePlay();
            }
        }   
    }
    
    public static void showLeaderboardsOrLoginiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
        
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
    }
    
    public static void showLeaderboardsOrLoginAndroidGooglePlay() {

#if GAMENETWORK_ANDROID_GOOGLE_PLAY

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
    }
    
    // -------------------------------------------------------------------------
    // LEADERBOARDS

    public static void SendScore(string key, long keyValue) {
        if (Instance != null) {
            Instance.sendScore(key, keyValue);
        }
    }
    
    public static void SendScore(string networkTypeTo, string key, long keyValue) {
        if (Instance != null) {
            Instance.sendScore(networkTypeTo, key, keyValue);
        }
    }
    
    public void sendScore(string key, long keyValue) {  
                
        GameLeaderboard item = GameLeaderboards.Instance.GetById(key);
        
        //LogUtil.Log("sendScore:" + " key:" + key + " keyValue:" + keyValue);
        
        if (item == null) {
            return;
        }
        
        if (item.data == null) {
            return;
        }
        
        foreach (GameNetworkData networkData in item.data.networks) {
            
            reportScore(networkData.type, networkData.code, keyValue);
        }
    }
        
    public void sendScore(string networkTypeTo, string key, long keyValue) {
        reportScore(networkTypeTo, key, keyValue);
    }
        
    public static void ReportScore(string key, long keyValue) {
        if (Instance != null) {
            Instance.reportScore(currentNetwork, key, keyValue);
        }
    }

    public static void ReportScore(string networkTypeTo, string key, long keyValue) {
        if (Instance != null) {
            Instance.reportScore(networkTypeTo, key, keyValue);
        }
    }
    
    public void reportScore(string networkTypeTo, string key, long keyValue) {
        
        //LogUtil.Log("reportScore:" + 
        //            " networkTypeTo:" + networkTypeTo+ 
        //            " key:" + key+ 
        //            " keyValue:" + keyValue);

        if (IsThirdPartyNetworkAvailable(networkTypeTo)) {  

            if (key.IndexOf("time-played") > -1) {
                keyValue = keyValue * 100;
            }
            
            //LogUtil.Log("reportScore:IsThirdPartyNetworkAvailable:" + 
            //            " networkTypeTo:" + networkTypeTo+ 
            //            " key:" + key+ 
            //            " keyValue:" + keyValue);

            if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                if (isAuthenticatediOSAppleGameCenter) {
                    reportScoreAppleGameCenter(key, keyValue);
                }
            }
            else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                if (isAuthenticatedAndroidGooglePlay) {
                    reportScoreGooglePlay(key, keyValue);
                }
            }
        }       
    }
    
    public static void reportScoreAppleGameCenter(string key, long keyValue) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                LogUtil.Log("reportScoreAppleGameCenter:" + " key:" + key + " keyValue:" + keyValue); 
                GameCenterBinding.reportScore(keyValue, key);
            }
        }
#endif      
    }
    
    public static void reportScoreGooglePlay(string key, long keyValue) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                LogUtil.Log("reportScoreGooglePlay:" + " key:" + key + " keyValue:" + keyValue); 
                PlayGameServices.submitScore(key, keyValue);
            }
        }
#endif      
    }   
    
    // -------------------------------------------------------------------------
    // ACHIEVEMENTS
    
    public static void SendAchievement(string key, bool completed) {
        if (Instance != null) {
            Instance.sendAchievement(key, completed);
        }
    }

    public void sendAchievement(string key, bool completed) {

        sendAchievement(key, completed ? 100f : 1f);
    }

    public static void SendAchievement(string key, float progress) {
        if (Instance != null) {
            Instance.sendAchievement(key, progress);
        }
    }
    
    public void sendAchievement(string key, float progress) {  
        
        GameAchievement item = GameAchievements.Instance.GetById(key);
        
        if (item == null) {
            return;
        }
        
        if (item.data == null) {
            return;
        }
        
        foreach (GameNetworkData networkData in item.data.networks) {
            
            reportAchievement(networkData.type, networkData.code, progress);
        }
    }

    public static void ReportAchievement(string key, bool completed) {
        if (Instance != null) {
            Instance.reportAchievement(currentNetwork, key, completed);
        }
    }

    public static void ReportAchievement(string networkTypeTo, string key, bool completed) {
        if (Instance != null) {
            Instance.reportAchievement(networkTypeTo, key, completed);
        }
    }
    
    public void reportAchievement(string networkTypeTo, string key, bool completed) {
        ReportAchievement(networkTypeTo, key, 100.0f);
    }
    
    public static void ReportAchievement(string key, float progress) {
        if (Instance != null) {
            Instance.reportAchievement(currentNetwork, key, progress);
        }
    }

    public static void ReportAchievement(string networkTypeTo, string key, float progress) {
        if (Instance != null) {
            Instance.reportAchievement(networkTypeTo, key, progress);
        }
    }
    
    public void reportAchievement(string networkTypeTo, string key, float progress) {
        if (IsThirdPartyNetworkAvailable(networkTypeTo)) {          
            if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {                
                if (isAuthenticatediOSAppleGameCenter) {
                    reportAchievementAppleGameCenter(key, progress);
                }
            }
            else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                if (isAuthenticatedAndroidGooglePlay) {
                    reportAchievementGooglePlay(key, progress);
                }
            }
        }   
    }
    
    public static void reportAchievementAppleGameCenter(string key, float progress) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkAppleGameCenter)) {
                LogUtil.Log("reportAchievementAppleGameCenter:" + " key:" + key + " progress:" + progress); 
                GameCenterBinding.reportAchievement(key, progress);
            }
        }
#endif      
    }
    
    public static void reportAchievementGooglePlay(string key, float progress) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
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
    }
    
    // -------------------------------------------------------------------------
    // NETWORK LOGIN
    
    public static void LoginNetwork(string networkTypeTo) {
        if (Instance != null) {
            Instance.loginNetwork(networkTypeTo);
        }
    }
    
    public void loginNetwork(string networkTypeTo) {

        if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER            
            if(isAvailableiOSAppleGameCenter) {
                GameCenterBinding.authenticateLocalPlayer();
                Debug.Log("GameCenter LoginNetwork is available...");
            }
            
        // Check existing achievements and update them if missing
            
            LogUtil.Log("GameCenter LoginNetwork...");
#endif      
        }
        else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY            
            if(IsThirdPartyNetworkAvailable(GameNetworkType.gameNetworkGooglePlayServices)) {
                PlayGameServices.authenticate();
            }
            
            // Check existing achievements and update them if missing
            
            LogUtil.Log("PlayGameServices LoginNetwork...");
#endif      
        }


    }
    
    
    // -------------------------------------------------------------------------
    // ACHIEVEMENT DATA
    
    public static void GetAchievements() {
        if (Instance != null) {
            Instance.getAchievements();
        }
    }
    
    public void getAchievements() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
        GameCenterBinding.getAchievements();
            
        LogUtil.Log("GameCenter GetAchievements...");
#endif  

#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        PlayGameServices.reloadAchievementAndLeaderboardData();
        
        //LogUtil.Log("GameCenter GetAchievements...");
#endif  

    }
    
    public static void GetAchievementsMetadata() {
        if (Instance != null) {
            Instance.getAchievementsMetadata();
        }
    }
    
    public void getAchievementsMetadata() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
        GameCenterBinding.retrieveAchievementMetadata();
            
        //LogUtil.Log("GameCenter GetAchievements...");
#endif  

#if GAMENETWORK_ANDROID_GOOGLE_PLAY
        PlayGameServices.getAllAchievementMetadata();
        
        //LogUtil.Log("PlayGameServices GetAchievements...");
#endif  
    }

#if GAMENETWORK_IOS_APPLE_GAMECENTER
    
    public static GameCenterAchievement GetGameCenterAchievement(string identifier) {
        if(Instance != null) {
            return Instance.getGameCenterAchievement(identifier);
        }
        return null;
    }   
    
    public GameCenterAchievement getGameCenterAchievement(string identifier) {      
        foreach(GameCenterAchievement achievement in gameCenterAchievementsNetwork) {
            if(achievement.identifier == identifier)
                return achievement;
        }
        return null;    
    }
#endif      

#if GAMENETWORK_IOS_APPLE_GAMECENTER
    public static GameCenterAchievementMetadata GetGameCenterAchievementMetadata(string identifier) {
        if(Instance != null) {
            Instance.getGameCenterAchievementMetadata(identifier);
        }
        return null;
    }   
    
    public GameCenterAchievementMetadata getGameCenterAchievementMetadata(string identifier)
    {       
        foreach(GameCenterAchievementMetadata achievement in gameCenterAchievementsMetaNetwork) {
            if(achievement.identifier == identifier)
                return achievement;
        }
        return null;    
    }
#endif  


#if GAMENETWORK_ANDROID_GOOGLE_PLAY

#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY

#endif

    public static void CheckAchievementsState() {
        if (Instance != null) {
            Instance.checkAchievementsState();
        }
    }
    
    public void checkAchievementsState() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
        // Sync from other devices.
        foreach(GameCenterAchievement achievement in gameCenterAchievementsNetwork) {
            bool localAchievementValue = GameProfileAchievements.Current.GetAchievementValue(achievement.identifier);
            bool remoteAchievementValue = achievement.completed;
            
            // If different on local and remote and local is true, set it...
            if(localAchievementValue != remoteAchievementValue && remoteAchievementValue) {
                GameProfileAchievements.Current.SetAchievementValue(achievement.identifier, true);
            }
        }
        
        foreach(GameCenterAchievementMetadata meta in gameCenterAchievementsMetaNetwork) {
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
                
#if GAMENETWORK_ANDROID_GOOGLE_PLAY

#endif

    }
    
    // -------------------------------------------------------------------------
    // USERNAME
    
    
    public static void SetLocalProfileToNetworkUsername() {
        if (Instance != null) {
            Instance.setLocalProfileToNetworkUsername(currentNetwork);
        }    
    }

    public static void SetLocalProfileToNetworkUsername(string networkTypeTo) {
        if (Instance != null) {
            Instance.setLocalProfileToNetworkUsername(networkTypeTo);
        }
    }
    
    public void setLocalProfileToNetworkUsername(string networkTypeTo) {
        LogUtil.Log("setLocalProfileToNetworkUsername");
        if (IsThirdPartyNetworkAvailable(networkTypeTo)) {          
            if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
                setLocalProfileToNetworkUsernameAppleGameCenter();
            }
            
            if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
                setLocalProfileToNetworkUsernameGooglePlay();
            }
        }           
    }
    
    public void setLocalProfileToNetworkUsernameAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER    
        string networkUsername = GameCenterBinding.playerAlias();
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
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
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
    }

    public static string GetNetworkUsername() {
        if (Instance != null) {
            return Instance.getNetworkUsername(currentNetwork);
        }
        return "";
    }
    
    public static string GetNetworkUsername(string networkTypeTo) {
        if (Instance != null) {
            return Instance.getNetworkUsername(networkTypeTo);
        }
        return "";
    }
    
    public string getNetworkUsername(string networkTypeTo) {

        string username = "";

        if (IsThirdPartyNetworkAvailable(networkTypeTo)) {          
            if (currentNetwork == GameNetworkType.gameNetworkAppleGameCenter) {
                username = getNetworkUsernameiOSAppleGameCenter();
            }
            else if (currentNetwork == GameNetworkType.gameNetworkGooglePlayServices) {
                username = getNetworkUsernameAndroidGooglePlay();
            }
        }   
                
        //LogUtil.Log("getNetworkUsername:" + username);

        return username;
    }
        
    public string getNetworkUsernameiOSAppleGameCenter() {
        //LogUtil.Log("GetNetworkUsername");
        string networkUser = "";
#if GAMENETWORK_IOS_APPLE_GAMECENTER    
        networkUser = GameCenterBinding.playerAlias();
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
#if GAMENETWORK_ANDROID_GOOGLE_PLAY 
        GPGPlayerInfo playerInfo = PlayGameServices.getLocalPlayerInfo();
        networkUser = playerInfo.playerId;// name
        if(networkUser != GameProfiles.Current.username 
            && !string.IsNullOrEmpty(networkUser)) {
            LogUtil.Log("GetNetworkUsername: " + networkUser);          
        }
#endif          
        return networkUser; 
    }
    
    // -------------------------------------------------------------------------
    // FACEBOOK SCORES
    
    public string facebookOpenGraphUrl = "https://graph.facebook.com/";
    
    public static void PostScoreFacebook(int score) {
        if (Instance != null) {
            Instance.postScoreFacebook(score);
        }
    }
    
    public void postScoreFacebook(int score) {
        //PostScoreFacebook(GameProfiles.Current.GetSocialNetworkUserId(), score);
    }
    
    public static void PostScoreFacebook(string userId, int score) {
        if (Instance != null) {
            Instance.postScoreFacebook(userId, score);
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
        if (bool.TryParse(responseText, out success)) {
            if (success) {
                LogUtil.Log("Score post success!");
            }
            else {
                LogUtil.Log("Score post failed!");
            }
        }
    }
    
    public static void GetScoresFacebookFriends() {
        if (Instance != null) {
            Instance.getScoresFacebookFriends();
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
        
        if (string.IsNullOrEmpty(responseText)) {
            return;
        }
        
        GameCommunityLeaderboardData leaderboardData = ParseScoresFacebook(responseText);
        
        if (leaderboardData != null) {
            Messenger<GameCommunityLeaderboardData>.Broadcast(
                GameCommunityMessages.gameCommunityLeaderboardFriendData, leaderboardData);
        }
        
    }
    
    public static void GetScoresFacebook() {
        if (Instance != null) {
            Instance.getScoresFacebook();
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
        
        if (string.IsNullOrEmpty(responseText)) {
            return;
        }
        
        GameCommunityLeaderboardData leaderboardData = parseScoresFacebook(responseText);
        
        if (leaderboardData != null) {
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
        if (Instance != null) {
            return Instance.parseScoresFacebook(responseText);
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
        
        if (string.IsNullOrEmpty(responseText)) {
            return leaderboardData;
        }
        
        List<GameCommunityLeaderboardItem> leaderboardItems = new List<GameCommunityLeaderboardItem>();
        
        JsonData jsonData = JsonMapper.ToObject(responseText.Replace("\\\"", "\""));
        
        if (jsonData != null) {
            if (jsonData.IsObject) {
        
                JsonData dataNode = jsonData["data"];
                
                if (dataNode != null) {
                    if (dataNode.IsArray) {
                
                        for (int i = 0; i < dataNode.Count; i++) {
                            
                            GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();
                            
                            var data = dataNode[i];
                                            
                            JsonData user = data["user"];
                            JsonData application = data["application"];
                            JsonData score = data["score"];
                            
                            if (score != null) {
                                if (score.IsInt) {
                                    leaderboardItem.value = float.Parse(score.ToString());
                                    leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");                      
                                }
                            }
                            
                            if (user != null) {
                                if (user.IsObject) {
                                    
                                    JsonData name = user["name"];
                                    if (name != null) {
                                        if (name.IsString) {
                                            string nameValue = name.ToString();
                                            if (!string.IsNullOrEmpty(nameValue)) {
                                                leaderboardItem.username = nameValue;
                                            }
                                        }
                                    }
                                    
                                    JsonData id = user["id"];
                                    if (id != null) {
                                        if (id.IsString) {
                                            string idValue = id.ToString();
                                            if (!string.IsNullOrEmpty(idValue)) {
                                                leaderboardItem.userId = idValue;
                                            }
                                        }
                                    }
                                    
                                    leaderboardItem.network = "facebook";
                                    leaderboardItem.name = leaderboardItem.username;
                                    leaderboardItem.type = "int";
                                    leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);
                                    ;
                                }
                            }
                            
                            if (application != null) {
                                if (application.IsObject) {
                                    JsonData name = application["name"];
                                    if (name != null) {
                                        if (name.IsString) {
                                            string nameValue = name.ToString();
                                            if (!string.IsNullOrEmpty(nameValue)) {
                                                leaderboardData.appName = nameValue;
                                            }
                                        }
                                    }
                                    
                                    JsonData namespaceNode = application["name"];
                                    if (namespaceNode != null) {
                                        if (namespaceNode.IsString) {
                                            string namespaceValue = namespaceNode.ToString();
                                            if (!string.IsNullOrEmpty(namespaceValue)) {
                                                leaderboardData.appNamespace = namespaceValue;
                                            }
                                        }
                                    }
                                    
                                    JsonData appId = application["id"];
                                    if (appId != null) {
                                        if (appId.IsString) {
                                            string appIdValue = appId.ToString();
                                            if (!string.IsNullOrEmpty(appIdValue)) {
                                                leaderboardData.appId = appIdValue;
                                            }
                                        }
                                    }
                                }
                            }
                            
                            leaderboardItems.Add(leaderboardItem);
                        }
                    }
                }
            }
        }
        
        leaderboardData.leaderboards.Add("high-score", leaderboardItems);
        
        return leaderboardData;
    }
            
    // -------------------------------------------------------------------------
    // EVENTS
    
    private void InitEvents(string networkTypeTo) {
        
        if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER        
            GameCenterManager.playerAuthenticatedEvent += playerAuthenticated;
            GameCenterManager.achievementsLoadedEvent += achievementsLoaded;
            GameCenterManager.achievementMetadataLoadedEvent += achievementMetadataLoaded;
#endif
        }
        else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY        

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

        if (networkTypeTo == GameNetworkType.gameNetworkAppleGameCenter) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER    
            GameCenterManager.playerAuthenticatedEvent -= playerAuthenticated;
            GameCenterManager.achievementsLoadedEvent -= achievementsLoaded;
            GameCenterManager.achievementMetadataLoadedEvent -= achievementMetadataLoaded;
#endif
        }
        else if (networkTypeTo == GameNetworkType.gameNetworkGooglePlayServices) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY        
        
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
    // EVENTS IOS GAME CENTER
    
#if GAMENETWORK_IOS_APPLE_GAMECENTER    
    void achievementsLoaded(List<GameCenterAchievement> achievementsNetworkResult) {
        LogUtil.Log("GameNetworks:GameCenter:achievementsLoaded"); 
        gameCenterAchievementsNetwork = achievementsNetworkResult;
        CheckAchievementsState();
    }
    
    void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementsMetaNetworkResult) {
        LogUtil.Log("GameNetworks:GameCenter:achievementMetadataLoaded"); 
        gameCenterAchievementsMetaNetwork = achievementsMetaNetworkResult;
        CheckAchievementsState();
    }
    
    void playerAuthenticated() {
        Debug.Log("GameNetworks:GameCenter:playerAuthenticated"); 
        SetLocalProfileToNetworkUsername();
        GetAchievements();
    }
#endif  
    

    // -------------------------------------------------------------------------
    // EVENTS ANDROID GOOGLE PLAY

#if GAMENETWORK_ANDROID_GOOGLE_PLAY
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
}
