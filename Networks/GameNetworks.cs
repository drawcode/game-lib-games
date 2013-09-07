#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used. 

#define GAMENETWORK_IOS_APPLE_GAMECENTER
#define GAMENETWORK_ANDROID_GOOGLE_PLAY
//#define GAMENETWORK_ANDROID_AMAZON_CIRCLE
//#define GAMENETWORK_ANDROID_SAMSUNG

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;

public enum GameNetworkType {
	IOS_GAME_CENTER,
	IOS_GOOGLE_PLAY,
	ANDROID_GOOGLE_PLAY,
	ANDROID_AMAZON_CIRCLE,
	ANDROID_SAMSUNGE,
	GAMEVERSES	
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

public class GameNetworks : MonoBehaviour {
	
	public GameObject gameNetworkContainer;
	
	public static bool gameNetworkiOSAppleGameCenterEnabled = true;
	public static bool gameNetworkAndroidGooglePlayEnabled = true;
	public static bool gameNetworkAndroidAmazonCircleEnabled = false;
	public static bool gameNetworkAndroidSamsunEnabled = false;
	
#if GAMENETWORK_IOS_APPLE_GAMECENTER
	[NonSerialized]
	public GameCenterManager gameCenterManager;
	[NonSerialized]
	public GameCenterEventListener gameCenterEventListener;
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
	public static GameNetworkType currentNetwork = GameNetworkType.IOS_GAME_CENTER;
	
	public static GameNetworks Instance;
	
    public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
				
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
		//GameNetworks.gameCenterEnabled = true;
#endif
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
		RemoveEvents();
	}	
	
	// -------------------------------------------------------------------------
	// NETWORK USER CHECK		
	
	public static void CheckThirdPartyNetworkLoggedInUser() {
		if(Instance != null) {
			Instance.checkThirdPartyNetworkLoggedInUser();
		}
	}
	
	void checkThirdPartyNetworkLoggedInUser() {
		// Check the third party network logged in name and the current logged in name, 
		// if the name is different change user.  
		// This helps the final case of when a user changes gamecenter while the app
		// is running and the auth changed event is just not broadcasting.
		
		if(GameState.Instance != null
		   && GameProfiles.Current != null) {
			
			currentLoggedInUserNetwork = GameProfiles.Current.username;

			if((gameNetworkiOSAppleGameCenterEnabled
				|| gameNetworkAndroidGooglePlayEnabled)
				&& isThirdPartyNetworkUserAuthenticated) {
				
				string networkName = GetNetworkUsername();
				if(!string.IsNullOrEmpty(networkName)) {
					currentLoggedInUserNetwork = networkName;
				}
			}
	
			if(currentLoggedInUserNetwork != GameProfiles.Current.username) {
				LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: currentLoggedInUserNetwork: " + currentLoggedInUserNetwork);
			
				LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: changed: " + GameProfiles.Current.username);
				GameState.Instance.ChangeUser(currentLoggedInUserNetwork);
				LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: GameProfiles.Current.username: " + GameProfiles.Current.username);
			}
		}
	}	
	
	// -------------------------------------------------------------------------
	// LOAD NETWORKS		
	
	public static void LoadNetwork(GameNetworkType networkType) {	
		if(Instance != null) {
			Instance.loadNetwork(networkType);
		}
	}
		
	public void loadNetwork(GameNetworkType networkType) {		
		currentNetwork = networkType;

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		if(networkType == GameNetworkType.IOS_GAME_CENTER) {
			InitNetwork();
		}
#endif

#if GAMENETWORK_ANDROID_GOOGLE_PLAY
		if(networkType == GameNetworkType.ANDROID_GOOGLE_PLAY) {
			InitNetwork();
		}
#endif
		// TODO OPENFEINT
	}
	
	public static void InitNetwork() {	
		if(Instance != null) {
			Instance.initNetwork();
		}
	}
	
	public void initNetwork() {		
		if(gameNetworkContainer == null) {
			gameNetworkContainer = new GameObject("GameNetworks");	
		}

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		gameCenterManager = gameNetworkContainer.AddComponent<GameCenterManager>();
		gameCenterEventListener = gameNetworkContainer.AddComponent<GameCenterEventListener>();
		
		InitEvents();		
		LoginNetwork();			
		
		Debug.Log("InitNetwork iOS Apple GameCenter init...");
#endif
		
#if GAMENETWORK_ANDROID_GOOGLE_PLAY	
		gamePlayServicesManager = gameNetworkContainer.AddComponent<GPGManager>();
		gamePlayServicesEventListener = gameNetworkContainer.AddComponent<GPGSEventListener>();
		
		InitEvents();		
		LoginNetwork();	
		
		Debug.Log("InitNetwork Android Google Play init...");
#endif	

			// Web player...
			
#if UNITY_WEBPLAYER
			Application.ExternalCall("if(window.console) window.console.log","web init");
#endif
	}
	
	// -------------------------------------------------------------------------
	// NETWORK AVAILABILITY		
	
	public static bool isThirdPartyNetworkAvailable {
		get {
			bool isAvailable = false;
			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				isAvailable = isAvailableiOSAppleGameCenter;
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				isAvailable = isAvailableAndroidGooglePlay;
			}
		
			return isAvailable;
		}
	}
	
	public static bool isAvailableiOSAppleGameCenter {
		get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			return GameCenterBinding.isGameCenterAvailable();
#else
			return false;
#endif	
		}
	}
	
	public static bool isAvailableAndroidGooglePlay {
		get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			return GameCenterBinding.isGameCenterAvailable();
#else
			return false;
#endif	
		}
	}
	
	// -------------------------------------------------------------------------
	// USER AUTHENTICATED		
	
	public static bool isThirdPartyNetworkUserAuthenticated {
		get {
			bool isAuthenticated = false;
			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				isAuthenticated = isAuthenticatediOSAppleGameCenter;
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				isAuthenticated = isAuthenticatedAndroidGooglePlay;
			}
			return isAuthenticated;
		}
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
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			return PlayGameServices.isSignedIn();
#else
			return false;
#endif	
		}
	}	
	
	public bool isThirdPartyNetworkReady {
		get {
			bool isReady = false;
			if(isThirdPartyNetworkAvailable 
			   && isThirdPartyNetworkUserAuthenticated) {
				isReady = true;			
			}
			return isReady;
		}
	}
	
	// -------------------------------------------------------------------------
	// ACHIEVEMENTS UI		
	
	public static void ShowAchievementsOrLogin() {	
		if(Instance != null) {
			Instance.showAchievementsOrLogin();
		}
	}	
	
	public void showAchievementsOrLogin() {
		if(isThirdPartyNetworkAvailable) {
			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				showAchievementsOrLoginiOSAppleGameCenter();
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				showAchievementsOrLoginAndroidGooglePlay();
			}
		}	
	}
	
	public static void showAchievementsOrLoginiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
			
			if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
				if(isThirdPartyNetworkUserAuthenticated) {
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
			
			if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
				if(isThirdPartyNetworkUserAuthenticated) {
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
		if(Instance != null) {
			Instance.showLeaderboardsOrLogin();
		}
	}	
	
	public void showLeaderboardsOrLogin() {
		if(isThirdPartyNetworkAvailable) {
			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				showLeaderboardsOrLoginiOSAppleGameCenter();
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				showLeaderboardsOrLoginAndroidGooglePlay();
			}
		}	
	}
	
	public static void showLeaderboardsOrLoginiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			if(isThirdPartyNetworkAvailable) {
				if(isThirdPartyNetworkUserAuthenticated) {
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
			
			if(GameNetworks.gameNetworkAndroidGooglePlayEnabled) {
				if(isThirdPartyNetworkAvailable) {
					if(isThirdPartyNetworkUserAuthenticated) {					
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
	
	public static void ReportScore(string key, long keyValue) {
		if(Instance != null) {
			Instance.reportScore(key, keyValue);
		}
	}	
	
	public void reportScore(string key, long keyValue) {
		if(isThirdPartyNetworkAvailable) {			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				reportScoreiOSAppleGameCenter(key, keyValue);
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				reportScoreAndroidGooglePlay(key, keyValue);
			}
		}		
	}
	
	public static void reportScoreiOSAppleGameCenter(string key, long keyValue) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			if(isThirdPartyNetworkAvailable) {
				GameCenterBinding.reportScore(keyValue, key);
			}
		}
#endif		
	}
	
	public static void reportScoreAndroidGooglePlay(string key, long keyValue) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			if(isThirdPartyNetworkAvailable) {
				PlayGameServices.submitScore(key, keyValue);
			}
		}
#endif		
	}	
	
	// -------------------------------------------------------------------------
	// ACHIEVEMENTS
		
	public static void ReportAchievement(string key, bool completed) {
		if(Instance != null) {
			Instance.reportAchievement(key, completed);
		}
	}	
	
	public void reportAchievement(string key, bool completed) {
		ReportAchievement(key, 100.0f);
	}
	
	public static void ReportAchievement(string key, float progress) {
		if(Instance != null) {
			Instance.reportAchievement(key, progress);
		}
	}	
	
	public void reportAchievement(string key, float progress) {
		if(isThirdPartyNetworkAvailable) {			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				reportAchievementiOSAppleGameCenter(key, progress);
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				reportAchievementAndroidGooglePlay(key, progress);
			}
		}	
	}	
	
	public static void reportAchievementiOSAppleGameCenter(string key, float progress) {
#if GAMENETWORK_IOS_APPLE_GAMECENTER
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			if(isThirdPartyNetworkAvailable) {
				GameCenterBinding.reportAchievement(key, progress);
			}
		}
#endif		
	}
	
	public static void reportAchievementAndroidGooglePlay(string key, float progress) {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			if(isThirdPartyNetworkAvailable) {
				PlayGameServices.incrementAchievement(key, (int)progress);
				//.unlockAchievement(key, keyValue / 100 > .95f ? true : false);
			}
		}
#endif		
	}
	
	// -------------------------------------------------------------------------
	// NETWORK LOGIN
	
	public static void LoginNetwork() {
		if(Instance != null) {
			Instance.loginNetwork();
		}
	}	
	
	public void loginNetwork() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER			
		if(GameCenterBinding.isGameCenterAvailable()) {
			GameCenterBinding.authenticateLocalPlayer();
		}
			
		// Check existing achievements and update them if missing
			
		//Debug.Log("GameCenter LoginNetwork...");
#endif		
	}
	
	
	// -------------------------------------------------------------------------
	// ACHIEVEMENT DATA
	
	public static void GetAchievements() {
		if(Instance != null) {
			Instance.getAchievements();
		}
	}	
	
	public void getAchievements() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		GameCenterBinding.getAchievements();
			
		//Debug.Log("GameCenter GetAchievements...");
#endif		
	}
	
	public static void GetAchievementsMetadata() {
		if(Instance != null) {
			Instance.getAchievementsMetadata();
		}
	}	
	
	public void getAchievementsMetadata() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		GameCenterBinding.retrieveAchievementMetadata();
			
		//Debug.Log("GameCenter GetAchievements...");
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
	
	public static void CheckAchievementsState() {
		if(Instance != null) {
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
					GameCenterBinding.reportAchievement(meta.code, 100);
				}
			}
		}
		
		//Debug.Log("GameCenter CheckAchievementsState...");
#endif		
	}
	
	// -------------------------------------------------------------------------
	// USERNAME
	
	public static void SetLocalProfileToNetworkUsername() {
		if(Instance != null) {
			Instance.setLocalProfileToNetworkUsername();
		}
	}
	
	public void setLocalProfileToNetworkUsername() {
		LogUtil.Log("setLocalProfileToNetworkUsername");
		if(isThirdPartyNetworkAvailable) {			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				setLocalProfileToNetworkUsernameiOSAppleGameCenter();
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				setLocalProfileToNetworkUsernameAndroidGooglePlay();
			}
		}			
	}
	
	public void setLocalProfileToNetworkUsernameiOSAppleGameCenter() {
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
		string networkUsername = GameCenterBinding.playerAlias();
		LogUtil.Log("setLocalProfileToNetworkUsernameiOSAppleGameCenter: " + networkUsername);
		
		if(!string.IsNullOrEmpty(networkUsername)) {
			//GameState.Instance.ChangeUser(networkUsername);
			//GameState.Instance.profile.SetThirdPartyNetworkUser(true);
			//GameState.Instance.SaveProfile();
			
			//Debug.Log("Logging in user GameCenter: " + networkUsername);
		}
#endif			
	}
	
	public void setLocalProfileToNetworkUsernameAndroidGooglePlay() {
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
		GPGPlayerInfo playerInfo = PlayGameServices.getLocalPlayerInfo();
		string networkUsername = playerInfo.playerId; // name
		LogUtil.Log("setLocalProfileToNetworkUsernameAndroidGooglePlay: " + networkUsername);
		
		if(!string.IsNullOrEmpty(networkUsername)) {
			//GameState.Instance.ChangeUser(networkUsername);
			//GameState.Instance.profile.SetThirdPartyNetworkUser(true);
			//GameState.Instance.SaveProfile();
			
			//Debug.Log("Logging in user GameCenter: " + networkUsername);
		}
#endif			
	}
	
	public static string GetNetworkUsername() {
		if(Instance != null) {
			return Instance.getNetworkUsername();
		}
		return "";
	}
	
	public string getNetworkUsername() {
		LogUtil.Log("getNetworkUsername");
		if(isThirdPartyNetworkAvailable) {			
			if(currentNetwork == GameNetworkType.IOS_GAME_CENTER) {
				return getNetworkUsernameiOSAppleGameCenter();
			}
			
			if(currentNetwork == GameNetworkType.ANDROID_GOOGLE_PLAY) {
				return getNetworkUsernameAndroidGooglePlay();
			}
		}	
		return "";
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
		if(Instance != null) {
			Instance.postScoreFacebook(score);
		}
	}
	
	public void postScoreFacebook(int score) {
		//PostScoreFacebook(GameProfiles.Current.GetSocialNetworkUserId(), score);
	}
	
	public static void PostScoreFacebook(string userId, int score) {
		if(Instance != null) {
			Instance.postScoreFacebook(userId, score);
		}
	}
	
	public void postScoreFacebook(string userId, int score) {
		
		Dictionary<string, object> data = new Dictionary<string, object>();
		
		data.Add("score", score);
		data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
		data.Add("access_token", GameProfiles.Current.GetSocialNetworkAuthTokenUser());
		
		Debug.Log("PostScoreFacebook score:" + score);
		Debug.Log("PostScoreFacebook app_access_token:" + SocialNetworks.Instance.appAccessToken);
		Debug.Log("PostScoreFacebook access_token:" + GameProfiles.Current.GetSocialNetworkAuthTokenUser());
		
		string url = facebookOpenGraphUrl + userId + "/scores";
		
		Debug.Log("PostScoreFacebook url:" + url);
		
		WebRequests.Instance.Request(WebRequests.RequestType.HTTP_POST, url, data, HandlePostScoreFacebookCallback);
	}
	
	void HandlePostScoreFacebookCallback(WebRequests.ResponseObject response) {
		string responseText = response.dataValueText;
		Debug.Log("HandlePostScoreFacebookCallback responseText:" + responseText);
		bool success = false;
		if(bool.TryParse(responseText, out success)) {
			if(success) {
				Debug.Log("Score post success!");
			}
			else {
				Debug.Log("Score post failed!");
			}
		}
	}
	
	public static void GetScoresFacebookFriends() {
		if(Instance != null) {
			Instance.getScoresFacebookFriends();
		}
	}
	
	public void getScoresFacebookFriends() {
		
		Dictionary<string, object> data = new Dictionary<string, object>();
		
		string accessToken = GameProfiles.Current.GetSocialNetworkAuthTokenUser();
		string userId = GameProfiles.Current.GetSocialNetworkUserId();
		string appId = SocialNetworks.Instance.FACEBOOK_APP_ID;
		
		//data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
		data.Add("access_token", accessToken);		
		
		string url = facebookOpenGraphUrl 
			+ userId + "/scores";
		
		Debug.Log("GetScoresFacebookFriends access_token:" + accessToken);
		Debug.Log("GetScoresFacebookFriends url:" + url);
		
		WebRequests.Instance.Request(
			WebRequests.RequestType.HTTP_GET, 
			url, 
			data, 
			HandleGetScoresFacebookFriendsCallback);
	}
	
	void HandleGetScoresFacebookFriendsCallback(WebRequests.ResponseObject response) {
		string responseText = response.dataValueText;
		Debug.Log("HandleGetScoresFacebookFriendsCallback responseText:" + responseText);
		
		if(string.IsNullOrEmpty(responseText)) {
			return;
		}
		
	 	GameCommunityLeaderboardData leaderboardData = ParseScoresFacebook(responseText);
		
		if(leaderboardData != null) {
			Messenger<GameCommunityLeaderboardData>.Broadcast(
				GameCommunityPlatformMessages.gameCommunityLeaderboardFriendData, leaderboardData);
		}
		
	}
	
	public static void GetScoresFacebook() {
		if(Instance != null) {
			Instance.getScoresFacebook();
		}
	}
	
	private void getScoresFacebook() {
		
		Dictionary<string, object> data = new Dictionary<string, object>();
		
		string accessToken = GameProfiles.Current.GetSocialNetworkAuthTokenUser();
		string appId = SocialNetworks.Instance.FACEBOOK_APP_ID;
		
		//data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
		data.Add("access_token", accessToken);
				
		string url = facebookOpenGraphUrl 
			+ appId + "/scores";
		
		Debug.Log("GetScoresFacebook access_token:" + accessToken);
		Debug.Log("GetScoresFacebook url:" + url);
		
		WebRequests.Instance.Request(
			WebRequests.RequestType.HTTP_GET, 
			url, 
			data, 
			HandleGetScoresFacebookCallback);
	}
	
	
	void HandleGetScoresFacebookCallback(Engine.Networking.WebRequests.ResponseObject response) {
		string responseText = response.dataValueText;
		Debug.Log("HandleGetScoresFacebookCallback responseText:" + responseText);
		
		if(string.IsNullOrEmpty(responseText)) {
			return;
		}
		
	 	GameCommunityLeaderboardData leaderboardData = parseScoresFacebook(responseText);
		
		if(leaderboardData != null) {
			Messenger<GameCommunityLeaderboardData>.Broadcast(
				GameCommunityPlatformMessages.gameCommunityLeaderboardData, leaderboardData);
		}
	}	
	
	public static string testFacebookScoresResult = "{\"data\":[{\"user\":{\"name\":\"Draw Code\",\"id\":\"1351861467\"},\"score\":240,\"application\":{\"name\":\"PopAR Game Community\",\"namespace\":\"popartoyscommunity\",\"id\":\"135612503258930\"}},{\"user\":{\"name\":\"Draw Labs\",\"id\":\"1494687700\"},\"score\":23,\"application\":{\"name\":\"PopAR Game Community\",\"namespace\":\"popartoyscommunity\",\"id\":\"135612503258930\"}}]}";
	
	public static void ParseTestScoresFacebook(string responseText) {
		GameCommunityLeaderboardData leaderboardData = ParseScoresFacebook(responseText);
		
		Messenger<GameCommunityLeaderboardData>.Broadcast(
			GameCommunityPlatformMessages.gameCommunityLeaderboardData, leaderboardData);
	}
	
	
	public static GameCommunityLeaderboardData ParseScoresFacebook(string responseText) {
		if(Instance != null) {
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
				            "id":"1351861467"
				         },
				         "score":240,
				         "application":{
				            "name":"PopAR Game Community",
				            "namespace":"popartoyscommunity",
				            "id":"135612503258930"
				         }
				      },
				      {
				         "user":{
				            "name":"Draw Labs",
				            "id":"1494687700"
				         },
				         "score":23,
				         "application":{
				            "name":"PopAR Game Community",
				            "namespace":"popartoyscommunity",
				            "id":"135612503258930"
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
		
		JsonData jsonData = JsonMapper.ToObject(responseText.Replace("\\\"", "\""));
		
		if(jsonData != null) {
			if(jsonData.IsObject) {
		
				JsonData dataNode = jsonData["data"];
				
				if(dataNode != null) {
					if(dataNode.IsArray) {
				
						for(int i = 0; i < dataNode.Count; i++) {
							
							GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();
							
							var data = dataNode[i];
											
							JsonData user = data["user"];
							JsonData application = data["application"];
							JsonData score = data["score"];
							
							if(score != null) {
								if(score.IsInt) {
									leaderboardItem.value = float.Parse(score.ToString());
									leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");						
								}
							}
							
							if(user != null) {
								if(user.IsObject) {
									
									JsonData name = user["name"];
									if(name != null) {
										if(name.IsString) {
											string nameValue = name.ToString();
											if(!string.IsNullOrEmpty(nameValue)) {
												leaderboardItem.username = nameValue;
											}
										}
									}
									
									JsonData id = user["id"];
									if(id != null) {
										if(id.IsString) {
											string idValue = id.ToString();
											if(!string.IsNullOrEmpty(idValue)) {
												leaderboardItem.userId = idValue;
											}
										}
									}
									
									leaderboardItem.network = "facebook";
									leaderboardItem.name = leaderboardItem.username;
									leaderboardItem.type = "int";
									leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);;
								}
							}
							
							if(application != null) {
								if(application.IsObject) {
									JsonData name = application["name"];
									if(name != null) {
										if(name.IsString) {
											string nameValue = name.ToString();
											if(!string.IsNullOrEmpty(nameValue)) {
												leaderboardData.appName = nameValue;
											}
										}
									}
									
									JsonData namespaceNode = application["name"];
									if(namespaceNode != null) {
										if(namespaceNode.IsString) {
											string namespaceValue = namespaceNode.ToString();
											if(!string.IsNullOrEmpty(namespaceValue)) {
												leaderboardData.appNamespace = namespaceValue;
											}
										}
									}
									
									JsonData appId = application["id"];
									if(appId != null) {
										if(appId.IsString) {
											string appIdValue = appId.ToString();
											if(!string.IsNullOrEmpty(appIdValue)) {
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
	
	private void InitEvents() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER		
			GameCenterManager.playerAuthenticated += playerAuthenticated;
			GameCenterManager.achievementsLoaded += achievementsLoaded;
			GameCenterManager.achievementMetadataLoaded += achievementMetadataLoaded;
#endif
	}
	
	private void RemoveEvents() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			GameCenterManager.playerAuthenticated -= playerAuthenticated;
			GameCenterManager.achievementsLoaded -= achievementsLoaded;
			GameCenterManager.achievementMetadataLoaded -= achievementMetadataLoaded;
#endif
	}
	
	// -------------------------------------------------------------------------
	// EVENTS IOS GAME CENTER
	
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
	void achievementsLoaded(List<GameCenterAchievement> achievementsNetworkResult) {
		gameCenterAchievementsNetwork = achievementsNetworkResult;
		CheckAchievementsState();
	}
	
	void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementsMetaNetworkResult) {
		gameCenterAchievementsMetaNetwork = achievementsMetaNetworkResult;
		CheckAchievementsState();
	}
	
	void playerAuthenticated() {
		SetLocalProfileToNetworkUsername();
		GetAchievements();
 	}
#endif	
	
	/*
	// -------------------------------------------------------------------------
	// EVENTS ANDROID GOOGLE PLAY
	
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
