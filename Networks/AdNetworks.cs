using System;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

/*
 * 
 http://prime31.com/docs#iosAdmob
  
  
// Sets the publiser Id and prepares AdMob for action.  Must be called before any other methods!	
public static void init( string publisherId, bool isTesting )

// Creates a banner of the given type ad the given position
public static void createBanner( AdMobBannerType bannerType, AdMobAdPosition position )

// Destroys the banner and removes it from view
public static void destroyBanner()

// Starts loading an interstitial ad
public static void requestInterstitalAd( string interstitialUnitId )

// Checks to see if the interstitial ad is loaded and ready to show
public static bool isInterstitialAdReady()

// If an interstitial ad is loaded this will take over the screen and show the ad
public static void showInterstitialAd()

// Fired when the ad view receives an ad
public static event Action adViewDidReceiveAdEvent;

// Fired when the ad view fails to receive an ad
public static event Action<string> adViewFailedToReceiveAdEvent;

// Fired when an interstitial is ready to show
public static event Action interstitialDidReceiveAdEvent;

// Fired when the interstitial download fails
public static event Action<string> interstitialFailedToReceiveAdEvent;
 
Android
 
// Initializes the AdMob plugin. Must be called before any other methods.
public static void init( string androidPublisherId, string iosPublisherId )

// Sets test devices. This needs to be set BEFORE a banner is created.
public static void setTestDevices( string[] testDevices )

// Creates a banner of the given type placed based on the position parameter
public static void createBanner( AdMobBanner type, AdMobLocation placement )

// Destroys the banner if it is showing
public static void destroyBanner()

// Requests an interstitial ad.  When it is loaded, the the interstitialReceivedAdEvent will be fired
public static void requestInterstital( string androidInterstitialUnitId, string iosInterstitialUnitId )

// Check to see if an interstitial ad is ready to be displayed
public static bool isInterstitalReady()

// Displays an interstitial if it is ready to be displayed
public static void displayInterstital()

// Fired when a new ad is loaded
public static event Action receivedAdEvent;

// Fired when an ad fails to be loaded
public static event Action<string> failedToReceiveAdEvent;

// Fired when an interstitial is loaded and ready for use
public static event Action interstitialReceivedAdEvent;

// Fired when an interstitial fails to receive an ad
public static event Action<string> interstitialFailedToReceiveAdEvent;

*/

public enum AdNetworkType {
	Drawlabs,
	Admob
}

public enum AdBanner {
	Phone_320x50,
	Tablet_300x250 = 3,
	Tablet_468x60 = 2,
	Tablet_728x90 = 1,
	SmartBanner = 4
}

public enum AdBannerType {
	iPhone_320x50,
	iPad_728x90,
	iPad_468x60,
	iPad_320x250,
	SmartBannerPortrait,
	SmartBannerLandscape
}

public enum AdPlacementType {
	TopLeft,
	TopCenter,
	TopRight,
	Centered,
	BottomLeft,
	BottomCenter,
	BottomRight
}


public enum AdPosition {
	TopLeft,
	TopCenter,
	TopRight,
	Centered,
	BottomLeft,
	BottomCenter,
	BottomRight
}

public class AdNetworks : MonoBehaviour
{
#if UNITY_EDITOR	
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
	[NonSerialized]
	public AdMobManager admobManager;
	[NonSerialized]
	public AdMobEventListener admobEventListener;
#elif UNITY_IPHONE
	[NonSerialized]
	public AdMobManager admobManager;
	[NonSerialized]
	public AdMobEventListener admobEventListener;
#endif
		
	const string PUBLISHER_ID_ADMOB_IOS = "a151f11dd7e7779";
	const string PUBLISHER_ID_ADMOB_ANDROID = "a151f11e296f9de";
	
	public static bool adNetworksEnabled = true;
	public static bool adNetworkTestingEnabled = true;
	
	public static AdNetworks Instance;
	
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
#endif
	}
	
	public void Init() {		
		Invoke("InitAdmob", 1);
	}
	
	public void InitAdmob() {
		LogUtil.Log("InitAdmob PUBLISHER_ID_ADMOB_IOS..." + PUBLISHER_ID_ADMOB_IOS);
		LogUtil.Log("InitAdmob PUBLISHER_ID_ADMOB_ANDROID..." + PUBLISHER_ID_ADMOB_ANDROID);
		
		// Social Network Prime31
		if(Application.platform == RuntimePlatform.Android) {
			LogUtil.Log("InitAdmob RuntimePlatform.Android..." + Application.platform);			
#if UNITY_ANDROID
			AdMob.init(PUBLISHER_ID_ADMOB_ANDROID, PUBLISHER_ID_ADMOB_IOS);			
			LogUtil.Log("InitAdmob Admob init..." + PUBLISHER_ID_ADMOB_ANDROID);
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer) {
#if UNITY_IPHONE
			AdMobBinding.init(PUBLISHER_ID_ADMOB_IOS, adNetworkTestingEnabled);
			LogUtil.Log("InitAdmob RuntimePlatform.IPhonePlayer..." + PUBLISHER_ID_ADMOB_IOS);
#endif
		}
		else {
			// Web player...
			
#if UNITY_WEBPLAYER
			//Application.ExternalCall("if(window.console) window.console.log","web facebook init");
#endif
		}
	}
	
	public AdMobBanner GetBannerAdmob(AdBanner banner) {
		if(banner == AdBanner.Phone_320x50) {
			return AdMobBanner.Phone_320x50;
		}
		else if(banner == AdBanner.Tablet_300x250) {
			return AdMobBanner.Tablet_300x250;
		}
		else if(banner == AdBanner.Tablet_468x60) {
			return AdMobBanner.Tablet_468x60;
		}
		else if(banner == AdBanner.Tablet_728x90) {
			return AdMobBanner.Tablet_728x90;
		}
		else {
			return AdMobBanner.SmartBanner;
		}
	}
#if !UNITY_ANDROID	
	public AdMobBannerType GetBannerTypeAdmob(AdBannerType bannerType) {
		if(bannerType == AdBannerType.iPad_320x250) {
			return AdMobBannerType.iPad_320x250;
		}
		else if(bannerType == AdBannerType.iPad_468x60) {
			return AdMobBannerType.iPad_468x60;
		}
		else if(bannerType == AdBannerType.iPad_728x90) {
			return AdMobBannerType.iPad_728x90;
		}
		else if(bannerType == AdBannerType.iPhone_320x50) {
			return AdMobBannerType.iPhone_320x50;
		}
		else if(bannerType == AdBannerType.SmartBannerPortrait) {
			return AdMobBannerType.SmartBannerPortrait;
		}
		else {
			return AdMobBannerType.SmartBannerLandscape;
		}
	}
#endif
	public AdMobLocation GetPlacementTypeAdmob(AdPlacementType bannerType) {
		if(bannerType == AdPlacementType.BottomLeft) {
			return AdMobLocation.BottomLeft;
		}
		else if(bannerType == AdPlacementType.BottomCenter) {
			return AdMobLocation.BottomCenter;
		}
		else if(bannerType == AdPlacementType.BottomRight) {
			return AdMobLocation.BottomRight;
		}
		else if(bannerType == AdPlacementType.TopLeft) {
			return AdMobLocation.TopLeft;
		}
		else if(bannerType == AdPlacementType.TopRight) {
			return AdMobLocation.TopRight;
		}
		else if(bannerType == AdPlacementType.TopLeft) {
			return AdMobLocation.TopLeft;
		}
		else if(bannerType == AdPlacementType.Centered) {
			return AdMobLocation.Centered;
		}
		else {
			return AdMobLocation.TopCenter;
		}
	}
#if !UNITY_ANDROID	
	public AdMobAdPosition GetPositionAdmob(AdPosition position) {
		if(position == AdPosition.BottomLeft) {
			return AdMobAdPosition.BottomLeft;
		}
		else if(position == AdPosition.BottomCenter) {
			return AdMobAdPosition.BottomCenter;
		}
		else if(position == AdPosition.BottomRight) {
			return AdMobAdPosition.BottomRight;
		}
		else if(position == AdPosition.TopLeft) {
			return AdMobAdPosition.TopLeft;
		}
		else if(position == AdPosition.TopRight) {
			return AdMobAdPosition.TopRight;
		}
		else if(position == AdPosition.TopLeft) {
			return AdMobAdPosition.TopLeft;
		}
		else if(position == AdPosition.Centered) {
			return AdMobAdPosition.Centered;
		}
		else {
			return AdMobAdPosition.TopCenter;
		}
	}
#endif
	public static void ShowAd() {
		if(Instance != null) {
			Instance.showAd();
		}
	}
	
	public static void ShowAd(AdBannerType bannerType, AdPosition position) {
		if(Instance != null) {
			Instance.showAd(bannerType, position);
		}
	}
	
	public void showAd() {
		showAd(AdBannerType.SmartBannerLandscape, AdPosition.TopCenter);
	}
	
	public void showAd(AdBannerType bannerType, AdPosition position) {
		if(Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
			//AdMob.createBanner(
				//GetBannerTypeAdmob(bannerType), 
				//GetPlacementTypeAdmob(placementType)
				//);
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer) {			
#if UNITY_IPHONE
			AdMobBinding.createBanner(
				GetBannerTypeAdmob(bannerType), 
				GetPositionAdmob(position)
				);
#endif
		}
		else {
			// Web player...
#if UNITY_WEBPLAYER
			//Application.ExternalCall("if(window.console) window.console.log","web show twitter login");
#endif
		}
		
	}
	
	public static void HideAd() {
		if(Instance != null) {
			Instance.hideAd();
		}
	}
	
	public void hideAd() {
		if(Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
			AdMob.destroyBanner();
#endif
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer) {			
#if UNITY_IPHONE
			AdMobBinding.destroyBanner();
#endif
		}
		else {
			// Web player...
#if UNITY_WEBPLAYER
			//Application.ExternalCall("if(window.console) window.console.log","web show twitter login");
#endif
		}
		
	}
	
}

