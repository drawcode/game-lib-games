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
using Engine.Utility;


public enum GameNetworkType {
	IOS_GAME_CENTER,
	IOS_GOOGLE_PLAY,
	ANDROID_GOOGLE_PLAY,
	ANDROID_AMAZON_CIRCLE,
	ANDROID_SAMSUNGE,
	GAMEVERSES	
}

public class GameNetworks : MonoBehaviour {
	
	public GameObject gameNetworkContainer;
	
	public static bool gameNetworkiOSAppleGameCenterEnabled = true;
	public static bool gameNetworkAndroidGooglePlayEnabled = true;
	public static bool gameNetworkAndroidAmazonCircleEnabled = false;
	public static bool gameNetworkAndroidSamsunEnabled = false;
		
	public string currentLoggedInUserNetwork = "";	

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
	
	[NonSerialized]
	public GameNetworkType currentNetwork = GameNetworkType.IOS_GAME_CENTER;
	
	public static GameNetworks Instance;
	
    public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
				
#if GAMENETWORK_IOS_APPLE_GAMECENTER
		GameNetworks.gameNetworkiOSAppleGameCenterEnabled = true;
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
		
		InvokeRepeating("CheckThirdPartyNetworkLoggedInUser", 3, 3);
	}
	
	void OnDisable() {
		RemoveEvents();
	}
	
	void CheckThirdPartyNetworkLoggedInUser() {
		// Check the third party network logged in name and the current logged in name, 
		// if the name is different change user.  
		// This helps the final case of when a user changes gamecenter while the app
		// is running and the auth changed event is just not broadcasting.
		
		if(GameState.Instance != null
		   && GameProfiles.Current != null) {
			
			currentLoggedInUserNetwork = GameProfiles.Current.username;

#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			string networkName = GetNetworkUsername();
			if(!string.IsNullOrEmpty(networkName)) {
				currentLoggedInUserNetwork = networkName;
			}
#endif			
			if(currentLoggedInUserNetwork != GameProfiles.Current.username) {
				LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: currentLoggedInUserNetwork: " + currentLoggedInUserNetwork);
			
				LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: changed: " + GameProfiles.Current.username);
				GameState.Instance.ChangeUser(currentLoggedInUserNetwork);
				LogUtil.Log("CheckThirdPartyNetworkLoggedInUser: GameProfiles.Current.username: " + GameProfiles.Current.username);
			}
		}
	}
	
	public void LoadNetwork(GameNetworkType networkType) {		
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
	
	public void InitNetwork() {		
		
		if(gameNetworkContainer == null) {
			gameNetworkContainer = new GameObject("GameNetworks");	
		}

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		gameCenterManager = gameNetworkContainer.AddComponent<GameCenterManager>();
		gameCenterEventListener = gameNetworkContainer.AddComponent<GameCenterEventListener>();
		
		InitEvents();		
		LoginNetwork();			
		
		Debug.Log("InitNetwork GameCenter init...");
#endif
		
#if GAMENETWORK_ANDROID_GOOGLE_PLAY	
		gamePlayServicesManager = gameNetworkContainer.AddComponent<GPGManager>();
		gamePlayServicesEventListener = gameNetworkContainer.AddComponent<GPGSEventListener>();
		
		InitEvents();		
		LoginNetwork();	
		
		Debug.Log("InitNetwork GameCenter init...");
#endif		

		// Web player...
			
#if UNITY_WEBPLAYER
			Application.ExternalCall("if(window.console) window.console.log","web init");
#endif
	}
	
	public bool isThirdPartyNetworkAvailable {
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
	
	public bool isAvailableiOSAppleGameCenter {
		get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			return GameCenterBinding.isGameCenterAvailable();
#else
			return false;
#endif	
		}
	}
	
	public bool isAvailableAndroidGooglePlay {
		get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			return GameCenterBinding.isGameCenterAvailable();
#else
			return false;
#endif	
		}
	}
	
	public bool isThirdPartyNetworkUserAuthenticated {
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
	
	public bool isAuthenticatediOSAppleGameCenter {
		get {
#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			return GameCenterBinding.isPlayerAuthenticated();
#else
			return false;
#endif	
		}
	}	
	
	public bool isAuthenticatedAndroidGooglePlay {
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
	
	public void ShowAchievementsOrLogin() {
		
		if(isThirdPartyNetworkAvailable) {

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
	}
	
	public void ShowLeaderboardsOrLogin() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER	
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			if(isAvailableiOSAppleGameCenter) {
				if(isThirdPartyNetworkUserAuthenticated){
					GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
				}
				else {
					GameCenterBinding.authenticateLocalPlayer();
				}
			}
		}
#endif		
	}
	
	public void ReportScore(string key, long keyValue) {

#if GAMENETWORK_IOS_APPLE_GAMECENTER	
		if(isAvailableiOSAppleGameCenter 
		   && GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
			GameCenterBinding.reportScore(keyValue, key);
		}
#endif		
	}
	
	public void ReportAchievement(string key, bool completed) {
		ReportAchievement(key, 100.0f);
	}
	
	public void ReportAchievement(string key, float progress) {
		if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {

#if GAMENETWORK_IOS_APPLE_GAMECENTER	
			if(isAvailableiOSAppleGameCenter) {				
				GameCenterBinding.reportAchievement(key, progress);
			}
#endif		
		}
	}	
	
	public void LoginNetwork() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER			
		if(isAvailableiOSAppleGameCenter) {
			GameCenterBinding.authenticateLocalPlayer();
		}
			
		// Check existing achievements and update them if missing
			
		Debug.Log("GameCenter LoginNetwork...");
#endif		
	}
	
	public void GetAchievements() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		GameCenterBinding.getAchievements();
			
		Debug.Log("GameCenter GetAchievements...");
#endif		
	}
	
	public void GetAchievementsMetadata() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		GameCenterBinding.retrieveAchievementMetadata();
			
		Debug.Log("GameCenter GetAchievements...");
#endif		
	}
	
#if GAMENETWORK_IOS_APPLE_GAMECENTER
	public GameCenterAchievement GetGameCenterAchievement(string identifier) {		
		foreach(GameCenterAchievement achievement in gameCenterAchievementsNetwork) {
			if(achievement.identifier == identifier)
				return achievement;
		}
		return null;	
	}
#endif

#if GAMENETWORK_IOS_APPLE_GAMECENTER
	public GameCenterAchievementMetadata GetGameCenterAchievementMetadata(string identifier) {		
		foreach(GameCenterAchievementMetadata achievement in gameCenterAchievementsMetaNetwork) {
			if(achievement.identifier == identifier)
				return achievement;
		}
		return null;	
	}
#endif	
	
	public void CheckAchievementsState() {

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
		
		Debug.Log("GameCenter CheckAchievementsState...");
#endif		
	}
	
	public void InitEvents() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER		
			GameCenterManager.playerAuthenticated += playerAuthenticated;
			GameCenterManager.achievementsLoaded += achievementsLoaded;
			GameCenterManager.achievementMetadataLoaded += achievementMetadataLoaded;
#endif
	}
	
	public void RemoveEvents() {

#if GAMENETWORK_IOS_APPLE_GAMECENTER
			GameCenterManager.playerAuthenticated -= playerAuthenticated;
			GameCenterManager.achievementsLoaded -= achievementsLoaded;
			GameCenterManager.achievementMetadataLoaded -= achievementMetadataLoaded;
#endif
	}
	
	void playerAuthenticated() {
		SetLocalProfileToNetworkUsername();
		GetAchievements();
 	}
	
	void SetLocalProfileToNetworkUsername() {
		LogUtil.Log("SetLocalProfileToNetworkUsername");

#if GAMENETWORK_IOS_APPLE_GAMECENTER
		string networkUsername = GameCenterBinding.playerAlias();
		LogUtil.Log("SetLocalProfileToNetworkUsername: " + networkUsername);
		
		if(!string.IsNullOrEmpty(networkUsername)) {
			GameState.Instance.ChangeUser(networkUsername);
			GameState.Instance.profile.SetThirdPartyNetworkUser(true);
			GameState.Instance.SaveProfile();
			
			Debug.Log("Logging in user GameCenter: " + networkUsername);
		}
#endif			
	}
		
	public string GetNetworkUsername() {
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
	

#if GAMENETWORK_IOS_APPLE_GAMECENTER
	void achievementsLoaded(
		List<GameCenterAchievement> achievementsNetworkResult) {
		gameCenterAchievementsNetwork = achievementsNetworkResult;
		CheckAchievementsState();
	}
#endif
	

#if GAMENETWORK_IOS_APPLE_GAMECENTER
	void achievementMetadataLoaded(
		List<GameCenterAchievementMetadata> achievementsMetaNetworkResult) {
		gameCenterAchievementsMetaNetwork = achievementsMetaNetworkResult;
		CheckAchievementsState();
	}
#endif
	
#if GAMENETWORK_ANDROID_GOOGLE_PLAY
	// Fired when authentication succeeds. Includes the user_id
	public void authenticationSucceededEvent(Action<string> action) {
		
	}

	// Fired when authentication fails
	public void authenticationFailedEvent(Action<string> action) {
		
	}

	// iOS only. Fired when the user signs out. This could happen if in a 
	// leaderboard they touch the settings button and logout from there.
	public void  userSignedOutEvent(Action action) {
		
	}

	// Fired when data fails to reload for a key. This particular model 
	// data is usually the player info or leaderboard/achievment 
	// metadata that is auto loaded.
	public void reloadDataForKeyFailedEvent(Action<string> action) {
		
	}

	// Fired when data is reloaded for a key
	public void reloadDataForKeySucceededEvent(Action<string> action) {
		
	}

	// Android only. Fired when a license check fails
	public void licenseCheckFailedEvent(Action action) {
		
	}

	// ##### ##### ##### ##### ##### ##### #####
	// ## Cloud Data
	// ##### ##### ##### ##### ##### ##### #####

	// Fired when loading cloud data fails
	public void loadCloudDataForKeyFailedEvent(Action<string> action) {
		
	}

	// Fired when loading cloud data succeeds and includes the key and data
	public void loadCloudDataForKeySucceededEvent(Action<int,string> action) {
		
	}

	// Fired when updating cloud data fails
	public void updateCloudDataForKeyFailedEvent(Action<string> action) {
		
	}

	// Fired when updating cloud data succeeds and includes the key and data
	public void updateCloudDataForKeySucceededEvent(Action<int,string> action) {
		
	}

	// Fired when clearing cloud data fails
	public void clearCloudDataForKeyFailedEvent(Action<string> action) {
		
	}

	// Fired when clearing cloud data succeeds and includes the key
	public void clearCloudDataForKeySucceededEvent(Action<string> action) {
		
	}

	// Fired when deleting cloud data fails
	public void deleteCloudDataForKeyFailedEvent(Action<string> action) {
		
	}

	// Fired when deleting cloud data succeeds and includes the key
	public void deleteCloudDataForKeySucceededEvent(Action<string> action) {
		
	}

	// ##### ##### ##### ##### ##### ##### #####
	// ## Achievements
	// ##### ##### ##### ##### ##### ##### #####

	// Fired when unlocking an achievement fails. Provides the achievmentId and the error in that order.
	public void unlockAchievementFailedEvent(Action<string,string> action) {
		
	}

	// Fired when unlocking an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
	public void unlockAchievementSucceededEvent(Action<string,bool> action) {
		
	}

	// Fired when incrementing an achievement fails. Provides the achievmentId and the error in that order.
	public void incrementAchievementFailedEvent(Action<string,string> action) {
		
	}

	// Fired when incrementing an achievement succeeds. Provides the achievementId and a bool that lets you know if it was newly unlocked.
	public void incrementAchievementSucceededEvent(Action<string,bool> action) {
		
	}

	// Fired when revealing an achievement fails. Provides the achievmentId and the error in that order.
	public void revealAchievementFailedEvent(Action<string,string> action) {
		
	}

	// Fired when revealing an achievement succeeds. The string lets you know the achievementId.
	public void revealAchievementSucceededEvent(Action<string> action) {
		
	}


	// ##### ##### ##### ##### ##### ##### #####
	// ## Leaderboards
	// ##### ##### ##### ##### ##### ##### #####

	// Fired when submitting a score fails. Provides the leaderboardId and the error in that order.
	public void submitScoreFailedEvent(Action<string,string> action) {
		
	}

	// Fired when submitting a scores succeeds. Returns the leaderboardId and a dictionary with some extra data with the fields from
	// the GPGScoreReport class: https://developers.google.com/games/services/ios/api/interface_g_p_g_score_report
	public void submitScoreSucceededEvent(Action<string,Dictionary<string,object>> action) {
		
	}

	// Fired when loading scores fails. Provides the leaderboardId and the error in that order.
	public void loadScoresFailedEvent(Action<string,string> action) {
		
	}

	// Fires when loading scores succeeds
	public void  loadScoresSucceededEvent(Action<List<GPGScore>> action) {
		
	}
	
#endif
	
}

