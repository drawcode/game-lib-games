#define SOCIAL_USE_FACEBOOK
#define SOCIAL_USE_TWITTER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

using Engine.Events;

public class SocialNetworksMessages {
    public static string socialLoaded = "social-loaded";
    public static string socialLoggedIn = "social-loggedin";
    public static string socialProfileData = "social-profile-data";
}

public class SocialNetworkTypes {
    public static string facebook = "facebook";
    public static string twitter = "twitter";
    public static string gameverses = "gameverses";
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

public class SocialNetworks : GameObjectBehavior {
    public GameObject socialNetworkFacebookAndroid;
    public GameObject socialNetworkTwitterAndroid;
    public GameObject socialNetworkiOS;
    public string FACEBOOK_APP_ID = AppConfigs.socialFacebookAppId;
    public string FACEBOOK_SECRET = AppConfigs.socialFacebookSecret;
    public string TWITTER_KEY = AppConfigs.socialTwitterAppId;
    public string TWITTER_SECRET = AppConfigs.socialTwitterSecret;
    public static SocialNetworks Instance;
    [NonSerializedAttribute]
    public string
        appAccessToken = "";
    public bool publishActions = false;
    [NonSerializedAttribute]
    public string
        facebookOpenGraphUrl = "https://graph.facebook.com/";
            
    public void Awake() {
        
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(gameObject);
            return;
        }
        
        Instance = this;    
    }

    /*
     * 
// ----------------------------------------------------------------

FacebookManager.cs

// Fired after a successful login attempt was made
public static event Action sessionOpenedEvent;

// Fired just before the login succeeded event. For interal use only.
public static event Action preLoginSucceededEvent;

// Fired when an error occurs while logging in
public static event Action<P31Error> loginFailedEvent;

// Fired when a custom dialog completes with the url passed back from the dialog
public static event Action<string> dialogCompletedWithUrlEvent;

// Fired when the post message or custom dialog fails
public static event Action<P31Error> dialogFailedEvent;

// Fired when a graph request finishes
public static event Action<object> graphRequestCompletedEvent;

// Fired when a graph request fails
public static event Action<P31Error> graphRequestFailedEvent;

// iOS only. Fired when the Facebook composer completes. True indicates success and false cancel/failure.
public static event Action<bool> facebookComposerCompletedEvent;

// Fired when reauthorization succeeds
public static event Action reauthorizationSucceededEvent;

// Fired when reauthorization fails
public static event Action<P31Error> reauthorizationFailedEvent;

// Fired when the share dialog fails
public static event Action<P31Error> shareDialogFailedEvent;

// Fired when the share dialog succeeds
public static event Action<Dictionary<string,object>> shareDialogSucceededEvent;

*/

    /*

// ----------------------------------------------------------------

TwitterBinding.cs

// Fired after a successful login attempt was made. Provides the screenname of the user.
public static event Action<string> loginSucceededEvent;

// Fired when an error occurs while logging in
public static event Action<string> loginFailedEvent;

// Fired when a custom request completes. The object will be either an IList or an IDictionary
public static event Action<object> requestDidFinishEvent;

// Fired when a custom request fails
public static event Action<string> requestDidFailEvent;

// iOS only. Fired when the tweet sheet completes. True indicates success and false cancel/failure.
public static event Action<bool> tweetSheetCompletedEvent;
*/
        
    void OnEnable() {

        // -------------------------------------------------------------------
        // FACEBOOK

#if UNITY_ANDROID || UNITY_IPHONE
        FacebookManager.sessionOpenedEvent += facebookSessionOpenedEvent;
        FacebookManager.preLoginSucceededEvent += facebookPreLoginSucceededEvent;
        FacebookManager.loginFailedEvent += facebookLoginFailed;

        FacebookManager.dialogCompletedWithUrlEvent += facebookDialogCompletedWithUrlEvent;
        FacebookManager.dialogFailedEvent += facebookDialogFailedEvent;
                
        FacebookManager.graphRequestCompletedEvent += facebookGraphRequestCompletedEvent;
        FacebookManager.graphRequestFailedEvent += facebookGraphRequestFailedEvent;

        FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;
        
        FacebookManager.reauthorizationFailedEvent += facebookReauthorizationFailedEvent;
        FacebookManager.reauthorizationSucceededEvent += facebookReauthorizationSucceededEvent;
        
        FacebookManager.shareDialogFailedEvent += facebookShareDialogFailedEvent;
        FacebookManager.shareDialogSucceededEvent += facebookShareDialogSucceededEvent;
#endif

        
        // -------------------------------------------------------------------
        // TWITTER
        
        #if UNITY_ANDROID || UNITY_IPHONE
        TwitterManager.loginSucceededEvent += twitterLoginSucceededEvent;
        TwitterManager.loginFailedEvent += twitterLoginFailedEvent;
        TwitterManager.requestDidFinishEvent += twitterRequestDidFinishEvent;
        
        TwitterManager.requestDidFailEvent += twitterRequestDidFailEvent;
        TwitterManager.tweetSheetCompletedEvent += twitterTweetSheetCompletedEvent;
        #endif


    }

    void OnDisable() {
        
        // -------------------------------------------------------------------
        // FACEBOOK

#if UNITY_ANDROID || UNITY_IPHONE
        // Remove all the event handlers when disabled
        FacebookManager.sessionOpenedEvent -= facebookSessionOpenedEvent;
        FacebookManager.preLoginSucceededEvent -= facebookPreLoginSucceededEvent;
        FacebookManager.loginFailedEvent -= facebookLoginFailed;
        
        FacebookManager.dialogCompletedWithUrlEvent -= facebookDialogCompletedWithUrlEvent;
        FacebookManager.dialogFailedEvent -= facebookDialogFailedEvent;
        
        FacebookManager.graphRequestCompletedEvent -= facebookGraphRequestCompletedEvent;
        FacebookManager.graphRequestFailedEvent -= facebookGraphRequestFailedEvent;
        
        FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;
        
        FacebookManager.reauthorizationFailedEvent -= facebookReauthorizationFailedEvent;
        FacebookManager.reauthorizationSucceededEvent -= facebookReauthorizationSucceededEvent;
        
        FacebookManager.shareDialogFailedEvent -= facebookShareDialogFailedEvent;
        FacebookManager.shareDialogSucceededEvent -= facebookShareDialogSucceededEvent;
#endif

        // -------------------------------------------------------------------
        // TWITTER
        
        #if UNITY_ANDROID || UNITY_IPHONE
        TwitterManager.loginSucceededEvent -= twitterLoginSucceededEvent;
        TwitterManager.loginFailedEvent -= twitterLoginFailedEvent;
        TwitterManager.requestDidFinishEvent -= twitterRequestDidFinishEvent;
        
        TwitterManager.requestDidFailEvent -= twitterRequestDidFailEvent;
        TwitterManager.tweetSheetCompletedEvent -= twitterTweetSheetCompletedEvent;
        #endif
    }
    
    // -------------------------------------------------------------------
    // FACEBOOK - EVENTS
    
    // Fired after a successful login attempt was made
    void facebookSessionOpenedEvent() {

        Debug.Log("SocialNetworks:facebookSessionOpenedEvent");
        Debug.Log("Successfully logged in to Facebook");
        
        GetProfileDataFacebook();   
        //GetPermissionsFacebook();
    }
    
    // Fired when an error occurs while logging in
    void facebookLoginFailed(Prime31.P31Error val) {
        
        Debug.Log("SocialNetworks:facebookLoginFailed" + " val:" + val.ToJson());

    }
    
    // Fired just before the login succeeded event. For interal use only.
    void facebookPreLoginSucceededEvent() {
        
        Debug.Log("SocialNetworks:facebookPreLoginSucceededEvent");
    
    }


    // Fired when a custom dialog completes with the url passed back from the dialog
    void facebookDialogCompletedWithUrlEvent(string val) {
        
        Debug.Log("SocialNetworks:facebookDialogCompletedWithUrlEvent" + " val:" + val.ToJson());
    
    }
    
    // Fired when the post message or custom dialog fails
    void facebookDialogFailedEvent(Prime31.P31Error val) {
        
        Debug.Log("SocialNetworks:facebookDialogFailedEvent" + " val:" + val.ToJson());
        
    }
    
    // Fired when a graph request finishes
    void facebookGraphRequestCompletedEvent(object val) {
        
        Debug.Log("SocialNetworks:facebookGraphRequestCompletedEvent" + " val:" + val.ToJson());
        
    }
    
    // Fired when a graph request fails
    void facebookGraphRequestFailedEvent(Prime31.P31Error val) {

        Debug.Log("SocialNetworks:facebookGraphRequestFailedEvent" + " val:" + val.ToJson());
        
    }
    
    // iOS only. Fired when the Facebook composer completes. True indicates success and false cancel/failure.
    void facebookComposerCompletedEvent(bool val) {
        
        Debug.Log("SocialNetworks:facebookComposerCompletedEvent" + " val:" + val.ToJson());
        
    }
    
    // Fired when reauthorization succeeds
    void facebookReauthorizationSucceededEvent() {
        
        Debug.Log("SocialNetworks:facebookReauthorizationSucceededEvent");

        publishActions = true;
        
        GetProfileDataFacebook();
        
    }
    
    // Fired when reauthorization fails
    void facebookReauthorizationFailedEvent(Prime31.P31Error val) {
        
        Debug.Log("SocialNetworks:facebookReauthorizationFailedEvent" + " val:" + val.ToJson());
        
    }
    
    // Fired when the share dialog succeeds
    void facebookShareDialogSucceededEvent(Dictionary<string,object> val) {
        
        Debug.Log("SocialNetworks:facebookShareDialogSucceededEvent" + " val:" + val.ToJson());
        
    }
    
    // Fired when the share dialog fails
    void facebookShareDialogFailedEvent(Prime31.P31Error val) {
        
        Debug.Log("SocialNetworks:facebookShareDialogFailedEvent" + " val:" + val.ToJson());
        
    }
    
    // -------------------------------------------------------------------
    // TWITTER - EVENTS
        
    // Fired after a successful login attempt was made. Provides the screenname of the user.
    void twitterLoginSucceededEvent(string val) {

        Debug.Log("SocialNetworks:twitterLoginSucceededEvent" + " val:" + val.ToJson());
        
        GameProfiles.Current.SetNetworkValueType(SocialNetworkTypes.twitter, SocialNetworkTypes.twitter);
        GameProfiles.Current.SetNetworkValueUsername(SocialNetworkTypes.twitter, val);
    
    }
    
    // Fired when an error occurs while logging in
    void twitterLoginFailedEvent(string val) {
        
        Debug.Log("SocialNetworks:twitterLoginFailedEvent" + " val:" + val.ToJson());
    
    }
        
    // Fired when a custom request completes. The object will be either an IList or an IDictionary
    void twitterRequestDidFinishEvent(object val) {
        
        Debug.Log("SocialNetworks:twitterRequestDidFinishEvent" + " val:" + val.ToJson());
    
    }
        
    // Fired when a custom request fails
    void twitterRequestDidFailEvent(string val) {
        
        Debug.Log("SocialNetworks:twitterRequestDidFailEvent" + " val:" + val.ToJson());
    
    }
        
    // iOS only. Fired when the tweet sheet completes. True indicates success and false cancel/failure.
    void twitterTweetSheetCompletedEvent(bool val) {
        
        Debug.Log("SocialNetworks:twitterTweetSheetCompletedEvent" + " val:" + val.ToJson());
    
    }
        
    // -------------------------------------------------------------------
    // COMMON
    
    public static void LoadSocialLibs() {
        if (Instance != null) {
            Instance.loadSocialLibs();
        }
    }
    
    // TODO cleanup...
    public void loadSocialLibs() {      
        if (AppConfigs.featureEnableFacebook) {
            initFacebook();
        }
        
        if (AppConfigs.featureEnableTwitter) {
            initTwitter();
        }
        
        Messenger.Broadcast(SocialNetworksMessages.socialLoaded);
    }
    
    public static void InitFacebook() {
        if (Instance != null) {
            Instance.initFacebook();
        }
    }
    
    public void initFacebook() {
        
        // optionally enable logging of all requests that go through the Facebook class
        Facebook.instance.debugRequests = true;
        
        Debug.Log("LoadSocialLibs FACEBOOK_APP_ID..." + FACEBOOK_APP_ID);
                
        // Social Network Prime31       
#if UNITY_ANDROID
        
        Debug.Log("LoadSocialLibs RuntimePlatform.Android..." + Application.platform);    
        
        socialNetworkFacebookAndroid = new GameObject("SocialNetworkingManager");
        socialNetworkFacebookAndroid.AddComponent<FacebookManager>();
        socialNetworkFacebookAndroid.AddComponent<FacebookEventListener>();

        //FacebookAndroid.init(FACEBOOK_APP_ID);
        FacebookAndroid.init(); 
        
        Debug.Log("LoadSocialLibs Facebook init..." + FACEBOOK_APP_ID);
        
#elif UNITY_IPHONE
        
        socialNetworkiOS = new GameObject("SocialNetworkingManager");
        socialNetworkiOS.AddComponent<FacebookManager>();
        socialNetworkiOS.AddComponent<FacebookEventListener>();
        
        FacebookBinding.init();     
        
        Debug.Log("LoadSocialLibs Facebook init..." + FACEBOOK_APP_ID);
    
        // iOS 6 only for system prefs
        //FacebookBinding.renewCredentialsForAllFacebookAccounts();     
#elif UNITY_WEBPLAYER
            
        Application.ExternalCall("if(window.console) window.console.log","web facebook init");
#endif  
        
        Facebook.instance.getAppAccessToken(FACEBOOK_APP_ID, FACEBOOK_SECRET, onFacebookAppAccessToken);
    }
    
    public static bool IsLoggedInFacebook() {
        if (Instance != null) {
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
        if (Instance != null) {
            Instance.showLoginFacebook();
        }
    }
    
    public void dumpPermissionsToLog(string[] permissions) {
        if (permissions != null) {
            Debug.Log("asking permissions:" + string.Join(",", permissions));
        }
    }
    
    public void dumpPermissionsToLog(List<string> permissions) {
        if (permissions != null) {
            Debug.Log("granted permissions:" + string.Join(",", permissions.ToArray()));
        }
    }

    public void showLoginFacebook() {
        
        var permissions = AppConfigs.socialFacebookPermissionsRead;
        
        dumpPermissionsToLog(permissions);
        
#if UNITY_ANDROID
        
        Debug.Log("Logging in facebook");
        FacebookAndroid.loginWithReadPermissions(permissions);
#elif UNITY_IPHONE  
            
        if(!SystemHelper.CanOpenUrl("fb://profile")) {
            Debug.Log("Facebook App is NOT installed, forcing Facebook WEB flow");
            FacebookBinding.setSessionLoginBehavior(FacebookSessionLoginBehavior.ForcingWebView);
        }
        else {
            Debug.Log("Facebook App is installed, loading Facebook APP flow, actually web view for now. Still a problem pulling auth creds from ios6.");      
            FacebookBinding.setSessionLoginBehavior(FacebookSessionLoginBehavior.ForcingWebView);    
        }           
        
        /*

        The following login method uses the URL Scheme suffix, which
        is explained in Step 8 of
        https://developers.facebook.com/docs/mobile/ios/build/

        This defines the app that should be returned to.
        This is ALSO DEFINED in the info.plist and Prime31 has 
        a menu item to update this at
        Prime31->Info.plist additions. 
        This is in the format fb[appid][scheme]

        */
        
        Debug.Log("Logging in facebook: urlscheme:" + AppConfigs.appUrlScheme);
        
        if(!string.IsNullOrEmpty(AppConfigs.appUrlScheme)) {
            FacebookBinding.loginWithReadPermissions(permissions, AppConfigs.appUrlScheme);
        }
        else {
            FacebookBinding.loginWithReadPermissions(permissions);
        }
            
#elif UNITY_WEBPLAYER
            
        //Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption);
#endif
    }
    
    public static List<object> GetSessionPermissionsFacebook() {
        if (Instance != null) {
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
        if (Instance != null) {
            Instance.getProfileDataFacebook();
        }
    }
    
    public int reAuthAttempts = 0;
    public Hashtable htFacebook = null;
        
    public void getProfileDataFacebook() { 
        
        Debug.Log("SocialNetworks:getProfileDataFacebook");
    
        Facebook.instance.graphRequest("me", Prime31.HTTPVerb.GET, ( error, obj ) => {
            // if we have an error we dont proceed any further
            if (error != null)
                return;
            
            if (obj == null)
                return;
            
            Debug.Log("me Graph Request finished: ");
            
            Debug.Log("obj: ");
            Prime31.Utils.logObject(obj);
                
            bool reauth = false;
                
            if (!checkPermissionFacebook(SocialNetworksFacebookPermissions.write_publish_actions)) {
                //|| !checkPermissionFacebook(SocialNetworksFacebookPermissions.write_publish_stream)) {
                reauth = true;
            }
                        
            if (reauth && reAuthAttempts == 0) {
                var permissions = AppConfigs.socialFacebookPermissionsWrite;
                
                dumpPermissionsToLog(permissions);
                
                //GameCommunityUIPanelLoading.ShowGameCommunityLoading(
                //    "Loading...", 
                //    "Asking for permission to post scores."
                //);
                
#if UNITY_IPHONE
                FacebookBinding.reauthorizeWithPublishPermissions(permissions, FacebookSessionDefaultAudience.Everyone);        
#elif UNITY_ANDROID
                FacebookAndroid.reauthorizeWithPublishPermissions(permissions, FacebookSessionDefaultAudience.Everyone);    
#else
#endif
                reAuthAttempts++;
                
                Invoke("ResetReAuthAttempts", 10);
                
            }
            Debug.Log("getProfileDataFacebook: appUrlScheme: " + AppConfigs.appUrlScheme);            
            Debug.Log("getProfileDataFacebook: key: " + SocialNetworksMessages.socialProfileData);
            Debug.Log("getProfileDataFacebook: network: " + SocialNetworkTypes.facebook);
            Debug.Log("getProfileDataFacebook: type: " + SocialNetworkDataTypes.profile);
            
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
        Debug.Log("appAccessToken:" + appAccessToken);

        GameProfiles.Current.SetNetworkValueToken(SocialNetworkTypes.facebook, token);
        // TODO save?
    }
    
    public static void GetPermissionsFacebook() {
        if (Instance != null) {
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
        //Debug.Log( "permission:" + perm );
        return permissions;
    }
    
    public List<string> getPermissionsFacebookString() {
        List<string> permissions = new List<string>();
        List<object> permissionObjects = getPermissionsFacebook();
        if (permissionObjects != null) {
            foreach (var perm in permissionObjects) {
                string permission = perm.ToString();
                if (!permissions.Contains(permission)) {
                    permissions.Add(perm.ToString());
                }
            }
        }
        
        dumpPermissionsToLog(permissions);
        
        return permissions;
    }
        
    public static bool CheckPermissionFacebook(string permission) {
        if (Instance != null) {
            return Instance.checkPermissionFacebook(permission);
        }
        
        return false;
    }
    
    public bool checkPermissionFacebook(string permission) {
        List<string> permissions = getPermissionsFacebookString();
        dumpPermissionsToLog(permissions);
        if (permissions != null) {
            if (permissions.Contains(permission)) {
                return true;    
            }
        }
        return false;
    }
    
    public static string GetAccessTokenAppFacebook() {
        if (Instance != null) {
            return Instance.getAccessTokenAppFacebook();
        }
        return string.Empty;
    }
    
    public string getAccessTokenAppFacebook() {
        return appAccessToken;
    }
    
    public static string GetAccessTokenUserFacebook() {
        if (Instance != null) {
            return Instance.getAccessTokenUserFacebook();
        }
        return string.Empty;
    }
    
    public string getAccessTokenUserFacebook() {
#if UNITY_IPHONE
        return FacebookBinding.getAccessToken();
#elif UNITY_ANDROID
        return FacebookAndroid.getAccessToken();
#else
        return "";
#endif

    }
        
    public static void LikeUrlFacebook(string urlLike) {
        // https://developers.facebook.com/docs/technical-guides/opengraph/built-in-actions/likes/
        // POST \
        // -F 'access_token=USER_ACCESS_TOKEN' \
        // -F 'object=OG_OBJECT_URL' \
        // https://graph.facebook.com/[User FB ID]/og.likes
         
        if (Instance != null) {
            Instance.likeUrlFacebook(urlLike);
        }
    }
    
    public void likeUrlFacebook(string urlLike) {
                
        string userId = GameProfiles.Current.GetNetworkValueId(SocialNetworkTypes.facebook);//GetSocialNetworkUserId();
            
        if (!string.IsNullOrEmpty(userId) && IsLoggedInFacebook()) {
        
            Dictionary<string, object> data = new Dictionary<string, object>();

            string access_token = GameProfiles.Current.GetNetworkValueToken(SocialNetworkTypes.facebook);
            
            data.Add("object", urlLike);
            data.Add("app_access_token", SocialNetworks.Instance.appAccessToken);
            data.Add("access_token", access_token);//GetSocialNetworkAuthTokenUser());
                
            Debug.Log("likeUrlFacebook object:" + urlLike);
            Debug.Log("likeUrlFacebook app_access_token:" + SocialNetworks.Instance.appAccessToken);
            Debug.Log("likeUrlFacebook access_token:" + access_token);//);
            
            string url = facebookOpenGraphUrl + userId + "/og.likes";
            
            Debug.Log("likeUrlFacebook url:" + url);
            
            Engine.Networking.WebRequests.Instance.Request(
                Engine.Networking.WebRequests.RequestType.HTTP_POST, 
                url, 
                data, 
                HandleLikeUrlFacebookCallback);
            
        }
        
    }
    
    void HandleLikeUrlFacebookCallback(Engine.Networking.WebRequests.ResponseObject response) {
        string responseText = response.dataValueText;
        Debug.Log("HandleLikeUrlFacebookCallback responseText:" + responseText);
        bool success = false;
        if (bool.TryParse(responseText, out success)) {
            if (success) {
                Debug.Log("LIKE success!");
            }
            else {
                Debug.Log("LIKE failed!");
            }
        }
    }
    
    public static void PostScoreFacebook(int score) {
        if (Instance != null) {
            Instance.postScoreFacebook(score);
        }
    }
    
    public void postScoreFacebook(int score) {
        
        string userId = GameProfiles.Current.GetNetworkValueId(SocialNetworkTypes.facebook);
        Debug.Log("PostScoreFacebook: userId:" + userId);
        Debug.Log("PostScoreFacebook: score:" + score);
        
        GameNetworks.PostScoreFacebook(score);
    }

    void completionHandler(string error, object result) {


        if (error != null) {
            
            Debug.Log("SocialNetworks:completionHandler:"
                + " error:" + error
            );

            Debug.LogError(error);
        }
        else {
            
            Debug.Log("SocialNetworks:completionHandler:"
                + " result:" + result
            );

            Prime31.Utils.logObject(result);
        }
    }
    
    public void ShowComposerFacebook(string message, string url, string title, string linkToImage, string caption) {
        if (Instance != null) {
            Instance.showComposerFacebook(message, url, title, linkToImage, caption);
        }
    }
    
    public void showComposerFacebook(string message, string url, string title, string linkToImage, string caption) {
        
        Debug.Log("SocialNetworks:showComposerFacebook:"
            + " message:" + message
            + " url:" + url
            + " title:" + title
            + " linkToImage:" + linkToImage
            + " caption:" + caption);
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
            Debug.Log(String.Format("Facebook posting for web: title:{0} caption:{0} message:{0} url:{0} caption:{0}", title, caption, message, url, caption) );
#endif
    }

    public bool canUseComposer(string networkType) {

        if(networkType == SocialNetworkTypes.facebook) {
            #if UNITY_ANDROID
            return true;
            #elif UNITY_IPHONE
            return FacebookBinding.canUserUseFacebookComposer();
            #elif UNITY_WEBPLAYER
            return true;
            #endif
        }
        else {
            return true;
        }
    }
        
    // Shows the Facebook share dialog. Valid dictionary keys 
    // (from FBShareDialogParams) are: link, name, caption, description, picture, friends (array)
    
    public static void PostMessageFacebook(string message, string url, string title, string linkToImage, string caption) {
        if (Instance != null) {
            Instance.postMessageFacebook(message, url, title, linkToImage, caption);
        }
    }

    public void postMessageFacebook(string message, string url, string title, string linkToImage, string caption) {

        Debug.Log("SocialNetworks:postMessageFacebook:"
            + " message:" + message
            + " url:" + url
            + " title:" + title
            + " linkToImage:" + linkToImage
            + " caption:" + caption);

#if UNITY_ANDROID       
        Facebook.instance.postMessageWithLinkAndLinkToImage(message, url, title, linkToImage, caption, completionHandler);
        //FacebookAndroid.postMessage("feed", url, title, linkToImage, caption);
#elif UNITY_IPHONE
        Facebook.instance.postMessageWithLinkAndLinkToImage(message, url, title, linkToImage, caption, completionHandler);
#elif UNITY_WEBPLAYER
        Application.ExternalCall("postFacebookMessage", title, caption, message, url, caption, linkToImage);            
        Debug.Log(String.Format("Facebook posting for web: title:{0} caption:{0} message:{0} url:{0} caption:{0}", title, caption, message, url, caption) );
#endif      
    }
    
    public static void ShowLoginOrPostMessageFacebook(string message, string url, string title, string linkToImage, string caption) {
        if (Instance != null) {
            Instance.showLoginOrPostMessageFacebook(message, url, title, linkToImage, caption);
        }
    }
    
    public void showLoginOrPostMessageFacebook(string message, string url, string title, string linkToImage, string caption) {

        bool loggedIn = GameCommunity.IsLoggedIn(SocialNetworkTypes.facebook);
        
        Debug.Log("SocialNetworks:showLoginOrPostMessageFacebook:"
            + " loggedIn:" + loggedIn
        );

        Debug.Log("SocialNetworks:showLoginOrPostMessageFacebook:"
            + " message:" + message
            + " url:" + url
            + " title:" + title
            + " linkToImage:" + linkToImage
            + " caption:" + caption
        );

        if (loggedIn) {

            bool hasComposer = canUseComposer(SocialNetworkTypes.facebook);

                Debug.Log("SocialNetworks:showLoginOrPostMessageFacebook:"
                      + " hasComposer:" + hasComposer
                      );

            if(hasComposer) {
                ShowComposerFacebook(message, url, title, linkToImage, caption);
            }
            else {
                PostMessageFacebook(message, url, title, linkToImage, caption);
            }
        }
        else {
            GameCommunity.Login(SocialNetworkTypes.facebook);
        }
    }
            
    void onPostScoreFacebook(bool success) {
        if (!success) {
            Debug.Log("Facebook score submit failed!");
        }
        else {          
            Debug.Log("Facebook score submit SUCCESS!");  
        }
    }
                
    void customRequestReceivedEvent(object obj) {
        Debug.Log("customRequestReceivedEvent:obj:" + obj);
    }
    
    void customRequestFailedEvent(string msg) {
        Debug.Log("customRequestFailedEvent:" + msg);
    }
    
    void failedToExtendTokenEvent() {
        Debug.Log("failedToExtendTokenEvent:");       
    }
    
    void sessionInvalidatedEvent() {
        Debug.Log("sessionInvalidatedEvent:");        
    }
    
    // ##############################################################################################
    // TWITTER
    
    public static void InitTwitter() {
        if (Instance != null) {
            Instance.initTwitter();
        }
    }
    
    public void initTwitter() {
        Debug.Log("LoadSocialLibs TWITTER_KEY..." + TWITTER_KEY);
        //Debug.Log("LoadSocialLibs TWITTER_SECRET..." + TWITTER_SECRET);
        
        // Social Network Prime31
#if UNITY_ANDROID           
        Debug.Log("LoadSocialLibs RuntimePlatform.Android..." + Application.platform);            
        
        socialNetworkTwitterAndroid = new GameObject("TwitterAndroidManager");
        socialNetworkTwitterAndroid.AddComponent<TwitterManager>();
        socialNetworkTwitterAndroid.AddComponent<TwitterEventListener>();
        
        TwitterAndroid.init(TWITTER_KEY, TWITTER_SECRET);
        Debug.Log("Twitter init..." + TWITTER_KEY);
#elif UNITY_IPHONE
        TwitterBinding.init(TWITTER_KEY, TWITTER_SECRET);   
        Debug.Log("Twitter init..." + TWITTER_KEY);   
#elif UNITY_WEBPLAYER
        Application.ExternalCall("if(window.console) window.console.log","web twitter init");
#endif
    }

    public static bool IsTwitterAvailable() {   
        if (Instance != null) {
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
        if (Instance != null) {
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
        if (Instance != null) {
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
        if (Instance != null) {
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
            Debug.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
    }
    
    public static void PostMessageTwitter(string message, string pathToImage) {
        if (Instance != null) {
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
            Debug.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
    }
        
    public static void ShowComposerTwitter(string message) {
        if (Instance != null) {
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
        //Application.ExternalCall("postMessageTwitter", message, pathToImage, link);
        //Debug.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
    }
    
    public static void ShowComposerTwitter(string message, string pathToImage) {
        if (Instance != null) {
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
        //Application.ExternalCall("postMessageTwitter", message, pathToImage, link);
        //Debug.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
    }
    
    public static void ShowComposerTwitter(string message, string pathToImage, string link) {
        if (Instance != null) {
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
        Debug.Log(String.Format("Twitter posting for web: message:{0}", message));
#endif
    }
    
    public static void ShowLoginOrPostMessageTwitter(string message) {
        if (Instance != null) {
            Instance.showLoginOrPostMessageTwitter(message);
        }
    }
    
    public void showLoginOrPostMessageTwitter(string message) {
        //if (IsLoggedInTwitter()) {
        
        bool loggedIn = GameCommunity.IsLoggedIn(SocialNetworkTypes.twitter);

        if(loggedIn) {
            postMessageTwitter(message);
        }
        else {
            showLoginTwitter();
        }
    }
    
    public static void ShowLoginOrPostMessageTwitter(string message, string pathToImage) {
        if (Instance != null) {
            Instance.showLoginOrPostMessageTwitter(message, pathToImage);
        }
    }
    
    public void showLoginOrPostMessageTwitter(string message, string pathToImage) {

        
        bool loggedIn = GameCommunity.IsLoggedIn(SocialNetworkTypes.twitter);
        
        Debug.Log("SocialNetworks:showLoginOrPostMessageTwitter:"
                  + " loggedIn:" + loggedIn
                  );
        
        Debug.Log("SocialNetworks:showLoginOrPostMessageTwitter:"
                  + " message:" + message
                  + " pathToImage:" + pathToImage
                  );
        
        if (loggedIn) {
            postMessageTwitter(message, pathToImage);
        }
        else {
            GameCommunity.Login(SocialNetworkTypes.twitter);
        }


        //if (IsLoggedInTwitter()) {
        //    postMessageTwitter(message, pathToImage);
        //}
        //else {
        //    showLoginTwitter();
        //}
    }
    
    public static void ShowLoginOrComposerTwitter(string message) {
        if (Instance != null) {
            Instance.showLoginOrComposerTwitter(message);
        }
    }
    
    public void showLoginOrComposerTwitter(string message) {
        //if (IsLoggedInTwitter()) {
        
        bool loggedIn = GameCommunity.IsLoggedIn(SocialNetworkTypes.twitter);

        if(loggedIn) {
            showComposerTwitter(message);
        }
        else {
            showLoginTwitter();
        }
    }
    
    public static void ShowLoginOrComposerTwitter(string message, string pathToImage) {
        if (Instance != null) {
            Instance.showLoginOrComposerTwitter(message, pathToImage);
        }
    }
    
    public void showLoginOrComposerTwitter(string message, string pathToImage) {
       // if (IsLoggedInTwitter()) {
        
        bool loggedIn = GameCommunity.IsLoggedIn(SocialNetworkTypes.twitter);

        if(loggedIn) {
            showComposerTwitter(message, pathToImage);
        }
        else {
            showLoginTwitter();
        }
    }            
}

/*
Facebook.instance.graphRequest( "me", HTTPVerb.GET, ( error, obj ) => {
    // if we have an error we dont proceed any further
    if( error != null )
        return;
    
    if( obj == null )
        return;
    
    // grab the userId and persist it for later use
    var ht = obj as Hashtable;
    userId = ht["id"].ToString();
    
    Debug.Log( "me Graph Request finished: " );
    Prime31.Utils.logObject( ht );
});
*/

/* 
 * 

// ----------------------------------------------------------------

PRIME 31 IOS DOCS

// ----------------------------------------------------------------

FacebookBinding.cs

// Initializes the Facebook plugin for your application
public static void init()

// Gets the url used to launch the application. If no url was used returns string.Empty
public static string getAppLaunchUrl()

// Sets the login behavior. Must be called before any login calls! Understand what the login behaviors are and how they work before using this!
public static void setSessionLoginBehavior( FacebookSessionLoginBehavior loginBehavior )

// Enables frictionless requests
public static void enableFrictionlessRequests()

// iOS 6 only. Renews the credentials that iOS stores for any native Facebook accounts. You can safely call this at app launch or when logging a user out.
public static void renewCredentialsForAllFacebookAccounts()

// Checks to see if the current session is valid
public static bool isSessionValid()

// Gets the current access token
public static string getAccessToken()

// Gets the permissions granted to the current access token
public static List<object> getSessionPermissions()

public static void loginUsingDeprecatedAuthorizationFlowWithRequestedPermissions( string[] permissions )

public static void loginUsingDeprecatedAuthorizationFlowWithRequestedPermissions( string[] permissions, string urlSchemeSuffix )

// Opens the Facebook single sign on login in Safari, the official Facebook app or uses iOS 6 Accounts if available
public static void login()

public static void loginWithReadPermissions( string[] permissions )

// Shows the native authorization dialog (iOS 6), opens the Facebook single sign on login in Safari or the official Facebook app with the requested read (not publish!) permissions
public static void loginWithReadPermissions( string[] permissions, string urlSchemeSuffix )

// Reauthorizes with the requested read permissions
public static void reauthorizeWithReadPermissions( string[] permissions )

// Reauthorizes with the requested publish permissions and audience
public static void reauthorizeWithPublishPermissions( string[] permissions, FacebookSessionDefaultAudience defaultAudience )

// Logs the user out and invalidates the token
public static void logout()

// Full access to any existing or new Facebook dialogs that get added.  See Facebooks documentation for parameters and dialog types
public static void showDialog( string dialogType, Dictionary<string,string> options )

// Allows you to use any available Facebook Graph API method
public static void graphRequest( string graphPath, string httpMethod, Dictionary<string,object> keyValueHash )

// Checks to see if the user is using a version of iOS that supports the Facebook composer and if they have an account setup
public static bool canUserUseFacebookComposer()

public static void showFacebookComposer( string message )

// Shows the Facebook composer with optional image path and link
public static void showFacebookComposer( string message, string imagePath, string link )

// Shows the Facebook share dialog. Valid dictionary keys (from FBShareDialogParams) are: link, name, caption, description, picture, friends (array)
public static void showFacebookShareDialog( Dictionary<string,object> parameters )

// Sets the app version that Facebook will use for all events
public static void setAppVersion( string version )

// Logs an event with optional parameters
public static void logEvent( string eventName, Dictionary<string,object> parameters = null )

// Logs an event, valueToSum and optional parameters
public static void logEvent( string eventName, double valueToSum, Dictionary<string,object> parameters = null )

// ----------------------------------------------------------------

FacebookManager.cs

// Fired after a successful login attempt was made
public static event Action sessionOpenedEvent;

// Fired just before the login succeeded event. For interal use only.
public static event Action preLoginSucceededEvent;

// Fired when an error occurs while logging in
public static event Action<P31Error> loginFailedEvent;

// Fired when a custom dialog completes with the url passed back from the dialog
public static event Action<string> dialogCompletedWithUrlEvent;

// Fired when the post message or custom dialog fails
public static event Action<P31Error> dialogFailedEvent;

// Fired when a graph request finishes
public static event Action<object> graphRequestCompletedEvent;

// Fired when a graph request fails
public static event Action<P31Error> graphRequestFailedEvent;

// iOS only. Fired when the Facebook composer completes. True indicates success and false cancel/failure.
public static event Action<bool> facebookComposerCompletedEvent;

// Fired when reauthorization succeeds
public static event Action reauthorizationSucceededEvent;

// Fired when reauthorization fails
public static event Action<P31Error> reauthorizationFailedEvent;

// Fired when the share dialog fails
public static event Action<P31Error> shareDialogFailedEvent;

// Fired when the share dialog succeeds
public static event Action<Dictionary<string,object>> shareDialogSucceededEvent;


// ----------------------------------------------------------------

Facebook.cs

// Sends off a graph request. The completion handler will return a Dictionary<string,object> or List<object> if successful depending on the path called.
// See Facebook's documentation for the returned data and parameters
public void graphRequest( string path, Action<string, object> completionHandler )
public void graphRequest( string path, HTTPVerb verb, Action<string, object> completionHandler )
public void graphRequest( string path, HTTPVerb verb, Dictionary<string, object> parameters, Action<string, object> completionHandler )

public void graphRequestBatch( IEnumerable<FacebookBatchRequest> requests, Action<string, object> completionHandler )

// Fetches a profile image for the user with userId. completionHandler will fire with the Texture2D or null
public void fetchProfileImageForUserId( string userId, Action<Texture2D> completionHandler )

// Fetches an image for the url. completionHandler will fire with the Texture2D or null
public IEnumerator fetchImageAtUrl( string url, Action<Texture2D> completionHandler )


// Scores API methods. Note that the publish_actions permission is required for these.

// Fetches the app access token
public void getAppAccessToken( string appId, string appSecret, Action<string> completionHandler )

// Posts a score for your app
public void postScore( string userId, int score, Action<bool> completionHandler )

// Retrieves the scores for your app
public void getScores( string userId, Action<string, object> onComplete )


// The following methods are included for backwards compatibility. They are all just standard graph requests that use the graphRequest method above

// Posts the message to the user's wall
public void postMessage( string message, Action<string, object> completionHandler )

// Posts the message to the user's wall with a link and a name for the link
public void postMessageWithLink( string message, string link, string linkName, Action<string, object> completionHandler )

// Posts the message to the user's wall with a link, a name for the link, a link to an image and a caption for the image
public void postMessageWithLinkAndLinkToImage( string message, string link, string linkName, string linkToImage, string caption, Action<string, object> completionHandler )

// Posts an image on the user's wall along with a caption.
public void postImage( byte[] image, string message, Action<string, object> completionHandler )

// Posts an image to a specific album along with a caption.
public void postImageToAlbum( byte[] image, string caption, string albumId, Action<string, object> completionHandler )

// Sends a request to fetch the currently logged in users details
public void getMe( Action<string,FacebookMeResult> completionHandler )

// Sends a request to fetch the currently logged in users friends
public void getFriends( Action<string,FacebookFriendsResult> completionHandler )

// Extends a short lived access token. Completion handler returns either the expiry date or null if unsuccessful. Note that it is highly recommended to
// only call this from a server. Your app secret should not be included with your application
public void extendAccessToken( string appId, string appSecret, Action<DateTime?> completionHandler )

// Checks the validity of a session on Facebook\'s servers. This is the authoritative way to check a session\'s validity.
public void checkSessionValidityOnServer( Action<bool> completionHandler )

// Fetches the session permissions directly from Facebook\'s servers. This is the authoritative way to get a session\'s granted permissions.
public void getSessionPermissionsOnServer( Action<string, List<string>> completionHandler )


// ----------------------------------------------------------------

SharingBinding.cs

// Shows the share sheet with the given items. Items can be text, urls or full and proper paths to sharable files
public static void shareItems( string[] items )

// Shows the share sheet with the given items with a list of excludedActivityTypes. See Apple\'s docs for more information on excludedActivityTypes.
public static void shareItems( string[] items, string[] excludedActivityTypes )

// ----------------------------------------------------------------

SharingManager.cs

// Fired when sharing completes and the user chose one of the sharing options
public static event Action<string> sharingFinishedWithActivityTypeEvent;

// Fired when the user cancels sharing without choosing any share options
public static event Action sharingCancelledEvent;

// ----------------------------------------------------------------

TWITTER

// ----------------------------------------------------------------

TwitterBinding.cs


// Initializes the Twitter plugin and sets up the required oAuth information
public static void init( string consumerKey, string consumerSecret )

// Checks to see if there is a currently logged in user
public static bool isLoggedIn()

// Retuns the currently logged in user\'s username
public static string loggedInUsername()

// Shows the login dialog via an in-app browser
public static void showLoginDialog()

// Logs out the current user
public static void logout()

// Posts the status text.  Be sure status text is less than 140 characters!
public static void postStatusUpdate( string status )

// Posts the status text and an image.  Note that the url will be appended onto the tweet so you don\'t have the full 140 characters
public static void postStatusUpdate( string status, string pathToImage )

// Receives tweets from the users home timeline
public static void getHomeTimeline()

// Performs a request for any available Twitter API methods.  methodType must be either \"get\" or \"post\".  path is the
// url fragment from the API docs (excluding https://api.twitter.com) and parameters is a dictionary of key/value pairs
// for the given method.  Path must request .json!  See Twitter\'s API docs for all available methods.
public static void performRequest( string methodType, string path, Dictionary<string,string> parameters )

// Checks to see if the current iOS version supports the tweet sheet
public static bool isTweetSheetSupported()

// Checks to see if a user can tweet (are they logged in with a Twitter account) using the native UI via the showTweetComposer method
public static bool canUserTweet()

public static void showTweetComposer( string status )

public static void showTweetComposer( string status, string pathToImage )

// Shows the tweet composer with the status message and optional image and link
public static void showTweetComposer( string status, string pathToImage, string link )


// ----------------------------------------------------------------

TwitterBinding.cs

// Fired after a successful login attempt was made. Provides the screenname of the user.
public static event Action<string> loginSucceededEvent;

// Fired when an error occurs while logging in
public static event Action<string> loginFailedEvent;

// Fired when a custom request completes. The object will be either an IList or an IDictionary
public static event Action<object> requestDidFinishEvent;

// Fired when a custom request fails
public static event Action<string> requestDidFailEvent;

// iOS only. Fired when the tweet sheet completes. True indicates success and false cancel/failure.
public static event Action<bool> tweetSheetCompletedEvent;


*/