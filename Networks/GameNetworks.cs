#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used. 

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;


public enum GameNetworkType
{
	IOS_GAME_CENTER,
	IOS_OPEN_FEINT,
	ANDROID_OPEN_FEINT,
	IMPOSSIBLE	
}

public class GameNetworks : MonoBehaviour
{
	public GameObject gameNetworkGameCenter;
	public GameObject gameNetworkOpenFeint;
	
	public static bool gameCenterEnabled = true;
		
	public string currentLoggedInUserNetwork = "";	

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
	[NonSerialized]
	public GameCenterManager gameCenterManager;
	[NonSerialized]
	public GameCenterEventListener gameCenterEventListener;
	[NonSerialized]
	public List<GameCenterAchievement> achievementsNetwork;
	[NonSerialized]
	public List<GameCenterAchievementMetadata> achievementsMetaNetwork;
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
				
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
		GameNetworks.gameCenterEnabled = true;
#endif
	}
	
	void Start()
	{
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
		achievementsNetwork = new List<GameCenterAchievement>();
		achievementsMetaNetwork = new List<GameCenterAchievementMetadata>();
#endif
		
		InvokeRepeating("CheckThirdPartyNetworkLoggedInUser", 3, 3);
	}
	
	void OnDisable()
	{
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
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE	
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
	
	public void LoadNetwork(GameNetworkType networkType)
	{		
		currentNetwork = networkType;


#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE	
		if(networkType == GameNetworkType.IOS_GAME_CENTER) {
			InitNetwork();
		}
#endif
		// TODO OPENFEINT
	}
	
	public void InitNetwork()
	{		
		// GameCenter Prime31
		
#if UNITY_ANDROID			

#endif


#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
			gameNetworkGameCenter = new GameObject("GameNetworks");			
			gameCenterManager = gameNetworkGameCenter.AddComponent<GameCenterManager>();
			gameCenterEventListener = gameNetworkGameCenter.AddComponent<GameCenterEventListener>();
			
			InitEvents();
			
			LoginNetwork();
			
			// Check existing achievements and update them if missing			
			
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
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
			isAvailable = GameCenterBinding.isGameCenterAvailable();
#endif	
			return isAvailable;
		}
	}
	
	public bool isThirdPartyNetworkUserAuthenticated {
		get {
			bool isAuthenticated = false;
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
			isAuthenticated = GameCenterBinding.isPlayerAuthenticated();
#endif	
			return isAuthenticated;
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
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
		if(GameNetworks.gameCenterEnabled) {
			if(GameCenterBinding.isGameCenterAvailable()) {
				if(GameCenterBinding.isPlayerAuthenticated()) {
					GameCenterBinding.showAchievements();
				}
				else {
					GameCenterBinding.authenticateLocalPlayer();
				}
			}
		}
#endif		
	}
	
	public void ShowLeaderboardsOrLogin() {
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
		if(GameNetworks.gameCenterEnabled) {
			if(GameCenterBinding.isGameCenterAvailable()) {
				if(GameCenterBinding.isPlayerAuthenticated())
					GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
				else
					GameCenterBinding.authenticateLocalPlayer();
			}
		}
#endif		
	}
	
	
	public void ReportScore(string key, long keyValue) {
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
		if(GameCenterBinding.isGameCenterAvailable() 
		   && GameNetworks.gameCenterEnabled) {
			GameCenterBinding.reportScore(keyValue, key);
		}
#endif		
	}	
	
	
	
	public void ReportAchievement(string key, bool completed) {
		ReportAchievement(key, 100.0f);
	}
	
	public void ReportAchievement(string key, float progress) {
		if(GameNetworks.gameCenterEnabled) {
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
		if(GameCenterBinding.isGameCenterAvailable()) {				
			GameCenterBinding.reportAchievement(key, progress);
		}
#endif		
		}
	}	
	
	public void LoginNetwork()
	{

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE			
		if(GameCenterBinding.isGameCenterAvailable()) {
			GameCenterBinding.authenticateLocalPlayer();
		}
			
		// Check existing achievements and update them if missing
			
		Debug.Log("GameCenter LoginNetwork...");
#endif		
	}
	
	public void GetAchievements() 
	{

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
		GameCenterBinding.getAchievements();
			
		Debug.Log("GameCenter GetAchievements...");
#endif		
	}
	
	public void GetAchievementsMetadata() 
	{

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
		GameCenterBinding.retrieveAchievementMetadata();
			
		Debug.Log("GameCenter GetAchievements...");
#endif		
	}
	
#if UNITY_EDITOR	
#elif UNITY_OSX
#elif UNITY_IPHONE
	public GameCenterAchievement GetGameCenterAchievement(string identifier)
	{		
		foreach(GameCenterAchievement achievement in achievementsNetwork) {
			if(achievement.identifier == identifier)
				return achievement;
		}
		return null;	
	}
#endif	
	

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
	public GameCenterAchievementMetadata GetGameCenterAchievementMetadata(string identifier)
	{		
		foreach(GameCenterAchievementMetadata achievement in achievementsMetaNetwork) {
			if(achievement.identifier == identifier)
				return achievement;
		}
		return null;	
	}
#endif	
	
	public void CheckAchievementsState()
	{

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE

		// Sync from other devices.
		foreach(GameCenterAchievement achievement in achievementsNetwork) {
			bool localAchievementValue = GameProfileAchievements.Current.GetAchievementValue(achievement.identifier);
			bool remoteAchievementValue = achievement.completed;
			
			// If different on local and remote and local is true, set it...
			if(localAchievementValue != remoteAchievementValue && remoteAchievementValue) {
				GameProfileAchievements.Current.SetAchievementValue(achievement.identifier, true);
			}
		}
		
		foreach(GameCenterAchievementMetadata meta in achievementsMetaNetwork) {
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
	
	public void InitEvents()
	{

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE			
			GameCenterManager.playerAuthenticated += playerAuthenticated;
			GameCenterManager.achievementsLoaded += achievementsLoaded;
			GameCenterManager.achievementMetadataLoaded += achievementMetadataLoaded;
#endif
	}
	
	public void RemoveEvents()
	{

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE	
			GameCenterManager.playerAuthenticated -= playerAuthenticated;
			GameCenterManager.achievementsLoaded -= achievementsLoaded;
			GameCenterManager.achievementMetadataLoaded -= achievementMetadataLoaded;
#endif
	}
	
	void playerAuthenticated()
 	{
		SetLocalProfileToNetworkUsername();
		GetAchievements();
 	}
	
	void SetLocalProfileToNetworkUsername() {
		LogUtil.Log("SetLocalProfileToNetworkUsername");
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
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
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE		
		networkUser = GameCenterBinding.playerAlias();
		if(networkUser != GameProfiles.Current.username 
			&& !string.IsNullOrEmpty(networkUser)) {
			LogUtil.Log("GetNetworkUsername: " + networkUser);			
		}
#endif			
		return networkUser; 
	}
	

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE
	void achievementsLoaded(List<GameCenterAchievement> achievementsNetworkResult) 
	{
		achievementsNetwork = achievementsNetworkResult;
		CheckAchievementsState();
	}
#endif
	

#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_IPHONE	
	void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementsMetaNetworkResult) 
	{
		achievementsMetaNetwork = achievementsMetaNetworkResult;
		CheckAchievementsState();
	}
#endif
	
}

