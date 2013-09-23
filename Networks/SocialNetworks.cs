using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Prime31;

using Engine.Events;

public class SocialNetworksMessages {
	public static string socialLoaded = "social-loaded";
	public static string socialLoggedIn = "social-loggedin";
	public static string socialProfileData = "social-profile-data";
}

public class SocialNetworkTypes {
	public static string facebook = "facebook";
	public static string twitter = "twitter";
	public static string vidari = "vidari";
}

public class SocialNetworkDataTypes {
	public static string profile = "profile";
	public static string scores = "scores";
	public static string achievements = "achievements";
}


//https://developers.facebook.com/docs/reference/login/#permissions
public static class SocialNetworksFacebookPermissions {
	public static string read_user_games_activity = "user_games_activity";
	public static string read_user_about_me = "user_about_me";
	public static string read_user_birthday = "user_birthday";
	public static string read_user_location = "user_location";
	public static string read_friends_games_activity = "friends_games_activity";
	
	public static string write_publish_actions = "publish_actions";
	public static string write_publish_stream = "publish_stream";

}

public class SocialNetworks : MonoBehaviour {
	public GameObject socialNetworkFacebookAndroid;
	public GameObject socialNetworkTwitterAndroid;
	public GameObject socialNetworkiOS;
		
	public string FACEBOOK_APP_ID = GameCommunityConfig.socialFacebookAppId;// "133833223395676";
	public string FACEBOOK_SECRET = GameCommunityConfig.socialFacebookSecret;//"133833223395676";
	public string TWITTER_KEY = GameCommunityConfig.socialTwitterAppId;//"nQuQQSKCPg0uVl8Im1ykMQ";
	public string TWITTER_SECRET = GameCommunityConfig.socialTwitterSecret;//;
	
	public static SocialNetworks Instance;	
	[NonSerializedAttribute]
	public string appAccessToken = "";
	public bool publishActions = false;
	
	[NonSerializedAttribute]
	public string facebookOpenGraphUrl = "https://graph.facebook.com/";
		    
	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
	}
		
	void OnEnable() {
		FacebookManager.sessionOpenedEvent += facebookLogin;
		//FacebookManager.loginFailedEvent += facebookLoginFailed;
		//FacebookManager.sessionInvalidatedEvent += facebookSessionInvalidatedEvent;

		//FacebookManager.dialogCompletedEvent += facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent += facebookDialogCompletedWithUrl;
		//FacebookManager.dialogFailedEventdialogDidNotCompleteEvent += facebookDialogDidntComplete;
		//FacebookManager.dialogFailedEvent += facebookDialogFailed;
		FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;
		
		//FacebookManager.reauthorizationFailedEvent += facebookReauthorizationFailedEvent;
		//FacebookManager.reauthorizationSucceededEvent += facebookReauthorizationSucceededEvent;
	}

	void OnDisable() {
		// Remove all the event handlers when disabled
		FacebookManager.sessionOpenedEvent -= facebookLogin;
		//FacebookManager.loginFailedEvent -= facebookLoginFailed;

		//FacebookManager.dialogCompletedEvent -= facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent -= facebookDialogCompletedWithUrl;
		//FacebookManager.dialogDidNotCompleteEvent -= facebookDialogDidntComplete;
		//FacebookManager.dialogFailedEvent -= facebookDialogFailed;
		FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;
		
		//FacebookManager.reauthorizationFailedEvent += facebookReauthorizationFailedEvent;
		//FacebookManager.reauthorizationSucceededEvent += facebookReauthorizationSucceededEvent;
	}
	
	// ##############################################################################################
	// FACEBOOK
	
	void facebookLogin() {
		LogUtil.Log( "Successfully logged in to Facebook" );
		
		GetProfileDataFacebook();	
		//GetPermissionsFacebook();
	}	
	
	void facebookLoginFailed( string error ) {
		LogUtil.Log( "Facebook login failed: " + error );
	}
	
	void facebookDidExtendTokenEvent( System.DateTime newExpiry ) {
		LogUtil.Log( "facebookDidExtendTokenEvent: " + newExpiry );
	}
	
	void facebokDialogCompleted() {
		LogUtil.Log( "facebokDialogCompleted" );
	}
	
	
	void facebookDialogCompletedWithUrl( string url ) {
		LogUtil.Log( "facebookDialogCompletedWithUrl: " + url );
	}
	
	
	void facebookDialogDidntComplete() {
		LogUtil.Log( "facebookDialogDidntComplete" );
	}
	
	
	void facebookDialogFailed( string error ) {
		LogUtil.Log( "facebookDialogFailed: " + error );
	}
			
	void facebookComposerCompletedEvent( bool didSucceed ) {
		LogUtil.Log( "facebookComposerCompletedEvent did succeed: " + didSucceed );
	}
	
	void facebookReauthorizationFailedEvent( string error ) {
		LogUtil.Log( "facebookReauthorizationFailedEvent: " + error );
	}

	void facebookReauthorizationSucceededEvent() {
		publishActions = true;
		
		LogUtil.Log( "facebookReauthorizationSucceededEvent" );
		
		GetProfileDataFacebook();
	}
	
	/*
	 Facebook.instance.graphRequest( "me", HTTPVerb.GET, ( error, obj ) =>
			{
				// if we have an error we dont proceed any further
				if( error != null )
					return;
				
				if( obj == null )
					return;
				
				// grab the userId and persist it for later use
				var ht = obj as Hashtable;
				userId = ht["id"].ToString();
				
				LogUtil.Log( "me Graph Request finished: " );
				Prime31.Utils.logObject( ht );
			});
			*/
	
	public static void LoadSocialLibs() {
		if(Instance != null) {
			Instance.loadSocialLibs();
		}
	}
	
	// TODO cleanup...
	public void loadSocialLibs() {		
		if(GameCommunityConfig.featureEnableFacebook) {
			initFacebook();
		}
		
		if(GameCommunityConfig.featureEnableTwitter) {
			initTwitter();
		}
		
		Messenger.Broadcast(SocialNetworksMessages.socialLoaded);
	}
	
	public static void InitFacebook() {
		if(Instance != null) {
			Instance.initFacebook();
		}
	}
	
	public void initFacebook() {
		
		// optionally enable logging of all requests that go through the Facebook class
		Facebook.instance.debugRequests = true;
		
		LogUtil.Log("LoadSocialLibs FACEBOOK_APP_ID..." + FACEBOOK_APP_ID);
				
		// Social Network Prime31		
#if UNITY_ANDROID
		
		LogUtil.Log("LoadSocialLibs RuntimePlatform.Android..." + Application.platform);	
		
		socialNetworkFacebookAndroid = new GameObject("SocialNetworkingManager");
		socialNetworkFacebookAndroid.AddComponent<FacebookManager>();
		socialNetworkFacebookAndroid.AddComponent<FacebookEventListener>();

		//FacebookAndroid.init(FACEBOOK_APP_ID);
		FacebookAndroid.init();	
		
		LogUtil.Log("LoadSocialLibs Facebook init..." + FACEBOOK_APP_ID);
		
#elif UNITY_IPHONE
		
		socialNetworkiOS = new GameObject("SocialNetworkingManager");
		socialNetworkiOS.AddComponent<FacebookManager>();
		socialNetworkiOS.AddComponent<FacebookEventListener>();
		
		FacebookBinding.init();		
		
		LogUtil.Log("LoadSocialLibs Facebook init..." + FACEBOOK_APP_ID);
	
		// iOS 6 only for system prefs
		//FacebookBinding.renewCredentialsForAllFacebookAccounts();		
#elif UNITY_WEBPLAYER
			
		Application.ExternalCall("if(window.console) window.console.log","web facebook init");
#endif	
		
		Facebook.instance.getAppAccessToken(FACEBOOK_APP_ID, FACEBOOK_SECRET, onFacebookAppAccessToken);
	}
	
	public static bool IsLoggedInFacebook() {
		if(Instance != null) {
			return Instance.isLoggedInFacebook();
		}
		return false;
	}
	
	public bool isLoggedInFacebook() {
#if UNITY_ANDROID	
		return FacebookAndroid.isSessionValid();
#elif UNITY_IPHONE
		return FacebookBinding.isSessionValid();
#elif UNITY_WEBPLAYER
		return true;//Application.ExternalEval(true);
#else
		return false;
#endif
	}
	
	public static void ShowLoginFacebook() {
		if(Instance != null) {
			Instance.showLoginFacebook();
		}
	}
	
	public void dumpPermissionsToLog(string[] permissions) {
		if(permissions != null) {
			LogUtil.Log("asking permissions:" + string.Join(",", permissions));
		}
	}
	
	public void dumpPermissionsToLog(List<string> permissions) {
		if(permissions != null) {
			LogUtil.Log("granted permissions:" + string.Join(",", permissions.ToArray()));
		}
	}

	public void showLoginFacebook() {
		
		var permissions = GameCommunityConfig.socialFacebookPermissionsRead;
		
		dumpPermissionsToLog(permissions);
		
#if UNITY_ANDROID
		
		LogUtil.Log("Logging in facebook");
		FacebookAndroid.loginWithReadPermissions(permissions);
#elif UNITY_IPHONE	
			
		if(!SystemHelper.CanOpenUrl("fb://profile")) {
			LogUtil.Log("Facebook App is NOT installed, forcing Facebook WEB flow");
			FacebookBinding.setSessionLoginBehavior(FacebookSessionLoginBehavior.ForcingWebView);
		}
		else {
			LogUtil.Log("Facebook App is installed, loading Facebook APP flow, actually web view for now. Still a problem pulling auth creds from ios6.");		
			FacebookBinding.setSessionLoginBehavior(FacebookSessionLoginBehavior.ForcingWebView);	
		}			
		
		/*
		* [CH] The following login method uses the URL Scheme suffix, which
		* is explained in Step 8 of
		* https://developers.facebook.com/docs/mobile/ios/build/
		* 
		* This defines the app that should be returned to.
		* This is ALSO DEFINED in the info.plist and Prime31 has 
		* a menu item to update this at
		* Prime31->Info.plist additions. 
		* This is in the format fb319879304747058constructionBook
		*/
		
		LogUtil.Log("Logging in facebook: urlscheme:" + GameCommunityConfig.appUrlScheme);
		
		if(!string.IsNullOrEmpty(GameCommunityConfig.appUrlScheme)) {
			FacebookBinding.loginWithReadPermissions(permissions, GameCommunityConfig.appUrlScheme);
		}
		else {
			FacebookBinding.loginWithReadPermissions(permissions);
		}
			
#elif UNITY_WEBPLAYER
			//Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption);
#endif
	}	
	
	public static List<object> GetSessionPermissionsFacebook() {
		if(Instance != null) {
			return Instance.getSessionPermissionsFacebook();
		}
		return null;
	}
		
	public List<object> getSessionPermissionsFacebook() {
		var currentPermissions = new List<object>();
#if UNITY_IPHONE
		currentPermissions = FacebookBinding.getSessionPermissions();
#elif UNITY_ANDROID
		currentPermissions = FacebookAndroid.getSessionPermissions();
#endif			
		return currentPermissions;
	}
	
	public static void GetProfileDataFacebook() {
		if(Instance != null) {
			Instance.getProfileDataFacebook();
		}
	}
	
	public int reAuthAttempts = 0;
	Hashtable htFacebook = null;
		
	public void getProfileDataFacebook() {  
	
		Facebook.instance.graphRequest( "me", HTTPVerb.GET, ( error, obj ) =>
		{
			// if we have an error we dont proceed any further
			if( error != null )
				return;
			
			if( obj == null )
				return;
			
			LogUtil.Log( "me Graph Request finished: " );
			
			LogUtil.Log( "obj: " );
			Prime31.Utils.logObject( obj );
				
			bool reauth = false;
				
			if(!checkPermissionFacebook(SocialNetworksFacebookPermissions.write_publish_actions)) {
				//|| !checkPermissionFacebook(SocialNetworksFacebookPermissions.write_publish_stream)) {
					reauth = true;
			}
						
			if(reauth && reAuthAttempts == 0) {
				var permissions = GameCommunityConfig.socialFacebookPermissionsWrite;
				
				dumpPermissionsToLog(permissions);
				
				GameCommunityUIPanelLoading.ShowGameCommunityLoading(
					"Loading...", 
					"Asking for permission to post scores."
				);
				
#if UNITY_IPHONE
				FacebookBinding.reauthorizeWithPublishPermissions(permissions, FacebookSessionDefaultAudience.Everyone);		
#elif UNITY_ANDROID
				FacebookAndroid.reauthorizeWithPublishPermissions(permissions, FacebookSessionDefaultAudience.Everyone);	
#else
#endif
				reAuthAttempts++;
				
				Invoke("ResetReAuthAttempts", 10);
				
			}
			LogUtil.Log("getProfileDataFacebook: appUrlScheme: " + GameCommunityConfig.appUrlScheme);			
			LogUtil.Log("getProfileDataFacebook: key: " + SocialNetworksMessages.socialProfileData);
			LogUtil.Log("getProfileDataFacebook: network: " + SocialNetworkTypes.facebook);
			LogUtil.Log("getProfileDataFacebook: type: " + SocialNetworkDataTypes.profile);
			
			Messenger<string, string, object>.Broadcast(
				SocialNetworksMessages.socialProfileData, 
				SocialNetworkTypes.facebook, 
				SocialNetworkDataTypes.profile, obj);
		});
	}
	
	void ResetReAuthAttempts() {
		reAuthAttempts = 0;
	}
	
	void onFacebookAppAccessToken(string token) {
		appAccessToken = token;
				LogUtil.Log( "appAccessToken:" + appAccessToken );
			// TODO save?
	}
	
	public static void GetPermissionsFacebook() {
		if(Instance != null) {
			Instance.getPermissionsFacebook();
		}
	}
	
	public List<object> getPermissionsFacebook() {
		List<object> permissions = null;
#if UNITY_ANDROID
		permissions = FacebookAndroid.getSessionPermissions();
#elif UNITY_IPHONE
		permissions = FacebookBinding.getSessionPermissions();
#else
#endif
		//if(permissions != null)
			//foreach( var perm in permissions )
				//LogUtil.Log( "permission:" + perm );
		return permissions;
	}
	
	public List<string> getPermissionsFacebookString() {
		List<string> permissions = new List<string>();
		List<object> permissionObjects = getPermissionsFacebook();
		if(permissionObjects != null) {
			foreach( var perm in permissionObjects) {
				string permission = perm.ToString();
				if(!permissions.Contains(permission)) {
					permissions.Add(perm.ToString());
				}
			}
		}
		
		dumpPermissionsToLog(permissions);
		
		return permissions;
	}
		
	public static bool CheckPermissionFacebook(string permission) {
		if(Instance != null) {
			return Instance.checkPermissionFacebook(permission);
		}
		
		return false;
	}
	
	public bool checkPermissionFacebook(string permission) {
		List<string> permissions = getPermissionsFacebookString();
		dumpPermissionsToLog(permissions);
		if(permissions != null) {
			if(permissions.Contains(permission)) {
				return true;	
			}
		}
		return false;
	}
	
	public static string GetAccessTokenAppFacebook() {
		if(Instance != null) {
			return Instance.getAccessTokenAppFacebook();
		}
		return string.Empty;
	}	
	
	public string getAccessTokenAppFacebook() {
		return appAccessToken;
	}	
	
	public static string GetAccessTokenUserFacebook() {
		if(Instance != null) {
			return Instance.getAccessTokenUserFacebook();
		}
		return string.Empty;
	}	
	
	public string getAccessTokenUserFacebook() {
#if UNITY_IPHONE
		return FacebookBinding.getAccessToken();
#elif UNITY_ANDROID
		return FacebookAndroid.getAccessToken();
#endif
	}
		
	public static void LikeUrlFacebook(string urlLike) {
		 // https://developers.facebook.com/docs/technical-guides/opengraph/built-in-actions/likes/
		 // POST \
	     // -F 'access_token=USER_ACCESS_TOKEN' \
	     // -F 'object=OG_OBJECT_URL' \
	     // https://graph.facebook.com/[User FB ID]/og.likes
	     
		if(Instance != null) {
			Instance.likeUrlFacebook(urlLike);
		}
	}
	
	public void likeUrlFacebook(string urlLike) {
				
		string userId = GameProfiles.Current.GetSocialNetworkUserId();
			
		if(!string.IsNullOrEmpty(userId) && IsLoggedInFacebook()) {
		
			Dictionary<string, object> data = new Dictionary<string, object>();
			
			data.Add("object", urlLike);
			data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
			data.Add("access_token", GameProfiles.Current.GetSocialNetworkAuthTokenUser());
				
			LogUtil.Log("likeUrlFacebook object:" + urlLike);
			LogUtil.Log("likeUrlFacebook app_access_token:" + SocialNetworks.Instance.appAccessToken);
			LogUtil.Log("likeUrlFacebook access_token:" + GameProfiles.Current.GetSocialNetworkAuthTokenUser());
			
			string url = facebookOpenGraphUrl + userId + "/og.likes";
			
			LogUtil.Log("likeUrlFacebook url:" + url);
			
			Engine.Networking.WebRequests.Instance.Request(
				Engine.Networking.WebRequests.RequestType.HTTP_POST, 
				url, 
				data, 
				HandleLikeUrlFacebookCallback);
			
		}
		
	}
	
	void HandleLikeUrlFacebookCallback(Engine.Networking.WebRequests.ResponseObject response) {
		string responseText = response.dataValueText;
		LogUtil.Log("HandleLikeUrlFacebookCallback responseText:" + responseText);
		bool success = false;
		if(bool.TryParse(responseText, out success)) {
			if(success) {
				LogUtil.Log("LIKE success!");
			}
			else {
				LogUtil.Log("LIKE failed!");
			}
		}
	}
	
	public static void PostScoreFacebook(int score) {
		if(Instance != null) {
			Instance.postScoreFacebook(score);
		}
	}	
	
	public void postScoreFacebook(int score) {
		string userId = GameProfiles.Current.GetSocialNetworkUserId();
		LogUtil.Log("PostScoreFacebook: userId:" + userId);
		LogUtil.Log("PostScoreFacebook: score:" + score);
		
		GameNetworks.PostScoreFacebook(score);
	}	
	
	public static void PostMessageFacebook(string message, string url, string title, string linkToImage, string caption) {
		if(Instance != null) {
			Instance.postMessageFacebook(message, url, title, linkToImage, caption);
		}
	}	
	
	void completionHandler( string error, object result ) {
		if( error != null )
			LogUtil.LogError( error );
		else
			Prime31.Utils.logObject( result );
	}
	
	
	public void ShowComposerFacebook(string message, string url, string title, string linkToImage, string caption) {
		if(Instance != null) {
			Instance.showComposerFacebook(message, url, title, linkToImage, caption);
		}
	}
	
	public void showComposerFacebook(string message, string url, string title, string linkToImage, string caption) {

#if UNITY_ANDROID
		var parameters = new Dictionary<string,string>
		{
			{ "link", url},
			{ "name", title },
			{ "picture", linkToImage },
			{ "caption", caption }
		};
		FacebookAndroid.showDialog( "stream.publish", parameters );
#elif UNITY_IPHONE
		FacebookBinding.showFacebookComposer(title, linkToImage, url);
#elif UNITY_WEBPLAYER
			Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption, linkToImage);			
			LogUtil.Log(String.Format("Facebook posting for web: title:{0} caption:{0} message:{0} url:{0} caption:{0}", title, caption, message, url, caption) );
#endif
	}
	
	
	public void postMessageFacebook(string message, string url, string title, string linkToImage, string caption) {
#if UNITY_ANDROID		
		Facebook.instance.postMessageWithLinkAndLinkToImage(message, url, title, linkToImage, caption, completionHandler);
		//FacebookAndroid.postMessage("feed", url, title, linkToImage, caption);
#elif UNITY_IPHONE
			FacebookBinding.showFacebookComposer(title, linkToImage, url);
#elif UNITY_WEBPLAYER
			Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption, linkToImage);			
			LogUtil.Log(String.Format("Facebook posting for web: title:{0} caption:{0} message:{0} url:{0} caption:{0}", title, caption, message, url, caption) );
#endif		
	}
	
	public static void ShowLoginOrPostMessageFacebook(string message, string url, string title, string linkToImage, string caption) {
		if(Instance != null) {
			Instance.showLoginOrPostMessageFacebook(message, url, title, linkToImage, caption);
		}
	}	
	
	public void showLoginOrPostMessageFacebook(string message, string url, string title, string linkToImage, string caption) {
		if(IsLoggedInFacebook()) {
			PostMessageFacebook(message, url, title, linkToImage, caption);
		}
		else {
			ShowLoginFacebook();
		}
	}
			
	void onPostScoreFacebook(bool success) {
		if(!success) {
			LogUtil.Log("Facebook score submit failed!");
		}
		else {			
			LogUtil.Log("Facebook score submit SUCCESS!");	
		}
	}			
				
	void customRequestReceivedEvent (object obj){
		LogUtil.Log("customRequestReceivedEvent:obj:" + obj);
	}
	
	void customRequestFailedEvent (string msg){
		LogUtil.Log("customRequestFailedEvent:" + msg);
	}
	
	void failedToExtendTokenEvent() {
		LogUtil.Log("failedToExtendTokenEvent:");		
	}
	
	void sessionInvalidatedEvent() {
		LogUtil.Log("sessionInvalidatedEvent:");		
	}
	
	// ##############################################################################################
	// TWITTER
	
	public static void InitTwitter() {
		if(Instance != null) {
			Instance.initTwitter();
		}
	}
	
	public void initTwitter() {
		LogUtil.Log("LoadSocialLibs TWITTER_KEY..." + TWITTER_KEY);
		//LogUtil.Log("LoadSocialLibs TWITTER_SECRET..." + TWITTER_SECRET);
		
		// Social Network Prime31
#if UNITY_ANDROID			
		LogUtil.Log("LoadSocialLibs RuntimePlatform.Android..." + Application.platform);			
		
		socialNetworkTwitterAndroid = new GameObject("TwitterAndroidManager");
		socialNetworkTwitterAndroid.AddComponent<TwitterManager>();
		socialNetworkTwitterAndroid.AddComponent<TwitterEventListener>();
		
		TwitterAndroid.init(TWITTER_KEY, TWITTER_SECRET);
		LogUtil.Log("Twitter init..." + TWITTER_KEY);
#elif UNITY_IPHONE
		TwitterBinding.init(TWITTER_KEY, TWITTER_SECRET);	
		LogUtil.Log("Twitter init..." + TWITTER_KEY);	
#elif UNITY_WEBPLAYER
		Application.ExternalCall("if(window.console) window.console.log","web twitter init");
#endif
	}
	public static bool IsTwitterAvailable() {	
		if(Instance != null) {
			return Instance.isTwitterAvailable();
		}
		return false;
	}	
		
	public bool isTwitterAvailable() {
#if UNITY_EDITOR
		//return true;
#else
		//return TwitterBinding.canUserTweet();
#endif
			return true;
	}
		
	public static bool IsLoggedInTwitter() {
		if(Instance != null) {
			return Instance.isLoggedInTwitter();
		}
		return false;
	}
	
	public bool isLoggedInTwitter() {
#if UNITY_ANDROID	
			return TwitterAndroid.isLoggedIn();
#elif UNITY_IPHONE
			return TwitterBinding.isLoggedIn();
#elif UNITY_WEBPLAYER
			return true;//Application.ExternalEval("true");
#else
		return false;		
#endif
	}
	
	public static void ShowLoginTwitter() {
		if(Instance != null) {
			Instance.showLoginTwitter();
		}
	}
	
	public void showLoginTwitter() {
#if UNITY_ANDROID				
			TwitterAndroid.showLoginDialog();
#elif UNITY_IPHONE
			TwitterBinding.showLoginDialog();
#elif UNITY_WEBPLAYER
			Application.ExternalCall("if(window.console) window.console.log","web show twitter login");
#endif
	}
	
	public static void PostMessageTwitter(string message) {
		if(Instance != null) {
			Instance.postMessageTwitter(message);
		}
	}
	
	public void postMessageTwitter(string message) {
#if UNITY_ANDROID				
			//TwitterAndroid.postUpdate(message);
#elif UNITY_IPHONE
			TwitterBinding.postStatusUpdate(message);
#elif UNITY_WEBPLAYER
			Application.ExternalCall("postMessageTwitter", message);			
			LogUtil.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
	}
	
	public static void PostMessageTwitter(string message, string pathToImage) {
		if(Instance != null) {
			Instance.postMessageTwitter(message, pathToImage);
		}
	}
	
	public void postMessageTwitter(string message, string pathToImage) {
#if UNITY_ANDROID				
			//TwitterAndroid.postUpdate(message);
#elif UNITY_IPHONE
			TwitterBinding.postStatusUpdate(message, pathToImage);
#elif UNITY_WEBPLAYER
			Application.ExternalCall("postMessageTwitter", message);			
			LogUtil.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
	}

		
	public static void ShowComposerTwitter(string message) {
		if(Instance != null) {
			Instance.showComposerTwitter(message);
		}
	}
		
	public void showComposerTwitter(string message) {
#if UNITY_ANDROID				
		// TODO, link, image path
		//TwitterAndroid.postUpdate(message);
#elif UNITY_IPHONE
		TwitterBinding.showTweetComposer(message);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("postMessageTwitter", message, pathToImage, link);			
		LogUtil.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
	}
	
	public static void ShowComposerTwitter(string message, string pathToImage) {
		if(Instance != null) {
			Instance.showComposerTwitter(message, pathToImage);
		}
	}
		
	public void showComposerTwitter(string message, string pathToImage) {
#if UNITY_ANDROID				
		// TODO, link, image path
		//TwitterAndroid.postUpdate(message);
#elif UNITY_IPHONE
		TwitterBinding.showTweetComposer(message, pathToImage);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("postMessageTwitter", message, pathToImage, link);			
		LogUtil.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
	}
	
	public static void ShowComposerTwitter(string message, string pathToImage, string link) {
		if(Instance != null) {
			Instance.showComposerTwitter(message, pathToImage, link);
		}
	}
		
	public void showComposerTwitter(string message, string pathToImage, string link) {
#if UNITY_ANDROID				
		// TODO, link, image path
		//TwitterAndroid.postUpdate(message);
#elif UNITY_IPHONE
		TwitterBinding.showTweetComposer(message, pathToImage, link);
#elif UNITY_WEBPLAYER
		Application.ExternalCall("postMessageTwitter", message, pathToImage, link);			
		LogUtil.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
	}
	
	public static void ShowLoginOrPostMessageTwitter(string message) {
		if(Instance != null) {
			Instance.showLoginOrPostMessageTwitter(message);
		}
	}
	
	public void showLoginOrPostMessageTwitter(string message) {
		if(IsLoggedInTwitter()) {
			postMessageTwitter(message);
		}
		else {
			showLoginTwitter();
		}
	}
	
	public static void ShowLoginOrPostMessageTwitter(string message, string pathToImage) {
		if(Instance != null) {
			Instance.showLoginOrPostMessageTwitter(message, pathToImage);
		}
	}
	
	public void showLoginOrPostMessageTwitter(string message, string pathToImage) {
		if(IsLoggedInTwitter()) {
			postMessageTwitter(message, pathToImage);
		}
		else {
			showLoginTwitter();
		}
	}
	
	public static void ShowLoginOrComposerTwitter(string message) {
		if(Instance != null) {
			Instance.showLoginOrComposerTwitter(message);
		}
	}
	
	public void showLoginOrComposerTwitter(string message) {
		if(IsLoggedInTwitter()) {
			showComposerTwitter(message);
		}
		else {
			showLoginTwitter();
		}
	}
	
	public static void ShowLoginOrComposerTwitter(string message, string pathToImage) {
		if(Instance != null) {
			Instance.showLoginOrComposerTwitter(message, pathToImage);
		}
	}
	
	public void showLoginOrComposerTwitter(string message, string pathToImage) {
		if(IsLoggedInTwitter()) {
			showComposerTwitter(message, pathToImage);
		}
		else {
			showLoginTwitter();
		}
	}
	
	//failedToExtendTokenEvent
	//sessionInvalidatedEvent
			
}
