//#define AD_USE_ADMOB
//#define AD_USE_IAD
//#define AD_USE_AMAZON
//#define PROMO_USE_VUNGLE
//#define PROMO_USE_CHARTBOOST
//#define PROMO_USE_TAPJOY
//#define AD_USE_UNITY
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
#if AD_USE_UNITY
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WEBGL || UNITY_STANDALONE
using UnityEngine.Advertisements;
#endif
#endif

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

//using Prime31;

#if PROMO_USE_CHARTBOOST
using ChartboostSDK;
#endif

public enum AdNetworkType {
    Drawlabs,
    Unity,
    Admob,
    iAds,
    Chartboost,
    Tapjoy,
    Vungle
}

public enum AdBanner {
    Phone_320x50,
    Tablet_300x250 = 3,
    Tablet_468x60 = 2,
    Tablet_728x90 = 1,
    SmartBanner = 4,
    Interstitial = 5,
    Full = 6
}

public enum AdDisplayType {
    iPhone_320x50,
    iPad_728x90,
    iPad_468x60,
    iPad_320x250,
    SmartBannerPortrait,
    SmartBannerLandscape,
    Banner,
    Video,
    VideoIncentivized,
    Interstitial,
    Full
}

public enum AdPlacementType {
    TopLeft,
    TopCenter,
    TopRight,
    Centered,
    BottomLeft,
    BottomCenter,
    BottomRight,
    Interstitial,
    Full
}

public enum AdPosition {
    TopLeft,
    TopCenter,
    TopRight,
    Centered,
    BottomLeft,
    BottomCenter,
    BottomRight,
    Interstitial,
    Full
}

public class AdNetworksMessages {

    // daily
    public static string videoAd = "ad-networks-video-ad";
    public static string moreGames = "ad-networks-more-games";

    // one time

    public static string website = "ad-networks-website";
    public static string twitterFollow = "ad-networks-twitter-follow";
    public static string facebookLike = "ad-networks-facebook-like";
}

public class AdNetworks : GameObjectBehavior
#if AD_USE_UNITY
//#if UNITY_IPHONE || UNITY_ANDROID  || UNITY_WEBGL
    //, IUnityAdsListener // Implement for the events, annoyingly in an interface now...
//#endif
#endif
    {
#if AD_USE_ADMOB
#if UNITY_EDITOR
#elif UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
    //[NonSerialized]
    //public AdMobManager admobManager;
    //[NonSerialized]
    //public AdMobEventListener admobEventListener;
#elif UNITY_IPHONE
    //[NonSerialized]
    //public AdMobManager admobManager;
    //[NonSerialized]
    //public AdMobEventListener admobEventListener;
#endif
#endif

#if USE_CONFIG_APP
    public static bool adNetworksEnabled = AppConfigs.adNetworksEnabled;
    public static bool adNetworkTestingEnabled = AppConfigs.adNetworkTestingEnabled;
#else
    public static bool adNetworksEnabled = false;
    public static bool adNetworkTestingEnabled = false;
#endif
    public bool tapjoyOpeningFullScreenAd = false;
    private static AdNetworks _instance = null;

    public static AdNetworks Instance {
        get {
            if (!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(AdNetworks)) as AdNetworks;

                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_AdNetworks");
                    _instance = obj.AddComponent<AdNetworks>();
                }
            }

            return _instance;
        }
    }

    void Start() {
        Init();
    }

    void OnEnable() {

#if AD_USE_UNITY

#endif

#if AD_USE_IAD
        // ------------
        // iAd

        AdManager.bannerViewDidChangeEvent += iadBannerViewDidChangeEvent; // Action<bool> bannerViewDidChangeEvent;
        AdManager.interstitalAdFailedEvent += iadInterstitalAdFailedEvent; // Action<string> interstitalAdFailedEvent
        AdManager.interstitialAdLoadedEvent += iadInterstitalAdLoadedEvent; // Action interstitialAdLoadedEvent;
        AdManager.interstitialDidUnloadEvent += iadInterstitialDidUnloadEvent; // Action interstitialDidUnloadEvent;
        AdManager.moviePlaybackCompletedEvent += iadMoviePlaybackCompletedEvent; // Action moviePlaybackCompletedEvent;
#endif


#if PROMO_USE_CHARTBOOST
        // ------------
        // CHARTBOOST

        // Listen to some interstitial-related events
        ChartboostSDK.Chartboost.didFailToLoadInterstitial += chartboostDidFailToLoadInterstitialEvent;
        ChartboostSDK.Chartboost.didCloseInterstitial += chartboostDidCloseInterstitialEvent;
        ChartboostSDK.Chartboost.didCacheInterstitial += chartboostDidCacheInterstitialEvent;
        ChartboostSDK.Chartboost.didDisplayInterstitial += chartboostDidShowInterstitialEvent;
#endif

#if PROMO_USE_VUNGLE
        // ------------
        // VUNGLE

        Vungle.onAdViewedEvent += vungleOnAdViewedEvent;
        Vungle.onAdStartedEvent += vungleOnAdStartedEvent;
        Vungle.onAdEndedEvent += vungleOnAdEndedEvent;
#endif

#if PROMO_USE_TAPJOY
        // ------------
        // TAPJOY

        // Tapjoy Connect Events
        TapjoyPlugin.connectCallSucceeded += tapjoyHandleTapjoyConnectSuccess;
        TapjoyPlugin.connectCallFailed += tapjoyHandleTapjoyConnectFailed;
        
        // Tapjoy Virtual Currency Events
        TapjoyPlugin.getTapPointsSucceeded += tapjoyHandleGetTapPointsSucceeded;
        TapjoyPlugin.getTapPointsFailed += tapjoyHandleGetTapPointsFailed;
        TapjoyPlugin.spendTapPointsSucceeded += tapjoyHandleSpendTapPointsSucceeded;
        TapjoyPlugin.spendTapPointsFailed += tapjoyHandleSpendTapPointsFailed;
        TapjoyPlugin.awardTapPointsSucceeded += tapjoyHandleAwardTapPointsSucceeded;
        TapjoyPlugin.awardTapPointsFailed += tapjoyHandleAwardTapPointsFailed;
        TapjoyPlugin.tapPointsEarned += tapjoyHandleTapPointsEarned;
        
        // Tapjoy Full Screen Ad Events
        TapjoyPlugin.getFullScreenAdSucceeded += tapjoyHandleGetFullScreenAdSucceeded;
        TapjoyPlugin.getFullScreenAdFailed += tapjoyHandleGetFullScreenAdFailed;
        
        // Tapjoy Display Ad Events
        TapjoyPlugin.getDisplayAdSucceeded += tapjoyHandleGetDisplayAdSucceeded;
        TapjoyPlugin.getDisplayAdFailed += tapjoyHandleGetDisplayAdFailed;
        
        // Tapjoy Video Ad Events
        TapjoyPlugin.videoAdStarted += tapjoyHandleVideoAdStarted;
        TapjoyPlugin.videoAdFailed += tapjoyHandleVideoAdFailed;
        TapjoyPlugin.videoAdCompleted += tapjoyHandleVideoAdCompleted;
        
        // Tapjoy Ad View Closed Events
        TapjoyPlugin.viewOpened += tapjoyHandleViewOpened;
        TapjoyPlugin.viewClosed += tapjoyHandleViewClosed;
        
        // Tapjoy Show Offers Events
        TapjoyPlugin.showOffersFailed += tapjoyHandleShowOffersFailed;
#endif

    }

    void OnDisable() {

#if AD_USE_UNITY

#endif

#if AD_USE_IAD
        // ------------
        // iAd     
        
        AdManager.bannerViewDidChangeEvent -= iadBannerViewDidChangeEvent; // Action<bool> bannerViewDidChangeEvent;
        AdManager.interstitalAdFailedEvent -= iadInterstitalAdFailedEvent; // Action<string> interstitalAdFailedEvent
        AdManager.interstitialAdLoadedEvent -= iadInterstitalAdLoadedEvent; // Action interstitialAdLoadedEvent;
        AdManager.interstitialDidUnloadEvent -= iadInterstitialDidUnloadEvent; // Action interstitialDidUnloadEvent;
        AdManager.moviePlaybackCompletedEvent -= iadMoviePlaybackCompletedEvent; // Action moviePlaybackCompletedEvent;
#endif


#if PROMO_USE_CHARTBOOST
        // ------------
        // CHARTBOOST

        // Remove event handlers
        ChartboostSDK.Chartboost.didFailToLoadInterstitial -= chartboostDidFailToLoadInterstitialEvent;
        ChartboostSDK.Chartboost.didCloseInterstitial -= chartboostDidCloseInterstitialEvent;
        ChartboostSDK.Chartboost.didCacheInterstitial -= chartboostDidCacheInterstitialEvent;
        ChartboostSDK.Chartboost.didDisplayInterstitial -= chartboostDidShowInterstitialEvent;
#endif

#if PROMO_USE_VUNGLE
        // ------------
        // VUNGLE

        Vungle.onAdViewedEvent -= vungleOnAdViewedEvent;
        Vungle.onAdStartedEvent -= vungleOnAdStartedEvent;
        Vungle.onAdEndedEvent -= vungleOnAdEndedEvent;
#endif

#if PROMO_USE_TAPJOY
        // ------------
        // TAPJOY
                
        // Tapjoy Connect Events
        TapjoyPlugin.connectCallSucceeded -= tapjoyHandleTapjoyConnectSuccess;
        TapjoyPlugin.connectCallFailed -= tapjoyHandleTapjoyConnectFailed;
        
        // Tapjoy Virtual Currency Events
        TapjoyPlugin.getTapPointsSucceeded -= tapjoyHandleGetTapPointsSucceeded;
        TapjoyPlugin.getTapPointsFailed -= tapjoyHandleGetTapPointsFailed;
        TapjoyPlugin.spendTapPointsSucceeded -= tapjoyHandleSpendTapPointsSucceeded;
        TapjoyPlugin.spendTapPointsFailed -= tapjoyHandleSpendTapPointsFailed;
        TapjoyPlugin.awardTapPointsSucceeded -= tapjoyHandleAwardTapPointsSucceeded;
        TapjoyPlugin.awardTapPointsFailed -= tapjoyHandleAwardTapPointsFailed;
        TapjoyPlugin.tapPointsEarned -= tapjoyHandleTapPointsEarned;

        // Tapjoy Full Screen Ad Events
        TapjoyPlugin.getFullScreenAdSucceeded -= tapjoyHandleGetFullScreenAdSucceeded;
        TapjoyPlugin.getFullScreenAdFailed -= tapjoyHandleGetFullScreenAdFailed;
        
        // Tapjoy Display Ad Events
        TapjoyPlugin.getDisplayAdSucceeded -= tapjoyHandleGetDisplayAdSucceeded;
        TapjoyPlugin.getDisplayAdFailed -= tapjoyHandleGetDisplayAdFailed;
        
        // Tapjoy Video Ad Events
        TapjoyPlugin.videoAdStarted -= tapjoyHandleVideoAdStarted;
        TapjoyPlugin.videoAdFailed -= tapjoyHandleVideoAdFailed;
        TapjoyPlugin.videoAdCompleted -= tapjoyHandleVideoAdCompleted;
        
        // Tapjoy Ad View Closed Events
        TapjoyPlugin.viewOpened -= tapjoyHandleViewOpened;
        TapjoyPlugin.viewClosed -= tapjoyHandleViewClosed;
        
        // Tapjoy Show Offers Events
        TapjoyPlugin.showOffersFailed -= tapjoyHandleShowOffersFailed;
#endif
    }

    public void Init() {

#if AD_USE_UNITY
        unityAdsInit();
#endif

#if AD_USE_IAD
        Invoke("iadInit", 1);
#endif


#if AD_USE_ADMOB
        //Invoke("admobInit", 1);
#endif

#if PROMO_USE_CHARTBOOST
        Invoke("chartboostInit", .5f);
#endif

#if PROMO_USE_VUNGLE
        Invoke("vungleInit", .6f);
#endif

#if PROMO_USE_TAPJOY
        Invoke("tapjoyInit", .8f);
#endif
    }
    // ----------------------------------------------------------------------
    // TAPJOY - http://prime31.com/docs#comboVungle

    #region TAPJOY

#if PROMO_USE_TAPJOY
    
    public void tapjoyInit() {

        
        // Enable logging.
        TapjoyPlugin.EnableLogging(true);
        // Connect to the Tapjoy servers.
        if (Application.platform == RuntimePlatform.Android) {
            TapjoyPlugin.RequestTapjoyConnect(
                AppConfigs.publisherIdTapjoyAndroid, 
                AppConfigs.publisherSecretTapjoyAndroid);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            
            Dictionary<String, String> dict = new Dictionary<String, String>();
            dict.Add("TJC_OPTION_COLLECT_MAC_ADDRESS", TapjoyPlugin.MacAddressOptionOffWithVersionCheck);
            TapjoyPlugin.RequestTapjoyConnect(
                AppConfigs.publisherIdTapjoyiOS, 
                AppConfigs.publisherSecretTapjoyiOS,
                dict);                              
        }

    }
    
    // OFFERS
    
    public void tapjoyShowOffers() {
        LogUtil.Log("tapjoyShowOffers");        
        TapjoyPlugin.ShowOffers();
    }

    public void tapjoyShowDisplayAd() {
        LogUtil.Log("tapjoyShowDisplayAd");        
        TapjoyPlugin.ShowDisplayAd();
    }

    public void tapjoyShowFullscreenAd() {
        LogUtil.Log("tapjoyShowFullscreenAd");        
        TapjoyPlugin.ShowFullScreenAd();
    }

    // CONNECT
    public void tapjoyHandleTapjoyConnectSuccess() {
        LogUtil.Log("tapjoyHandleTapjoyConnectSuccess");
    }
    
    public void tapjoyHandleTapjoyConnectFailed() {
        LogUtil.Log("tapjoyHandleTapjoyConnectFailed");
    }
    
    // VIRTUAL CURRENCY
    public void tapjoyHandleGetTapPointsSucceeded(int points) {
        LogUtil.Log("tapjoyHandleGetTapPointsSucceeded: " + points);
        //tapPointsLabel = "Total TapPoints: " + TapjoyPlugin.QueryTapPoints();
    }
    
    public void tapjoyHandleGetTapPointsFailed() {
        LogUtil.Log("tapjoyHandleGetTapPointsFailed");
    }
    
    public void tapjoyHandleSpendTapPointsSucceeded(int points) {
        LogUtil.Log("HandleSpendTapPointsSucceeded: " + points);
        //tapPointsLabel = "Total TapPoints: " + TapjoyPlugin.QueryTapPoints();
    }
    
    public void tapjoyHandleSpendTapPointsFailed() {
        LogUtil.Log("HandleSpendTapPointsFailed");
    }
    
    public void tapjoyHandleAwardTapPointsSucceeded() {
        LogUtil.Log("HandleAwardTapPointsSucceeded");
        //tapPointsLabel = "Total TapPoints: " + TapjoyPlugin.QueryTapPoints();
    }
    
    public void tapjoyHandleAwardTapPointsFailed() {
        LogUtil.Log("HandleAwardTapPointsFailed");
    }
    
    public void tapjoyHandleTapPointsEarned(int points) {
        LogUtil.Log("CurrencyEarned: " + points);
        //tapPointsLabel = "Currency Earned: " + points;
        
        TapjoyPlugin.ShowDefaultEarnedCurrencyAlert();
    }
    
    // FULL SCREEN ADS
    public void tapjoyHandleGetFullScreenAdSucceeded() {
        LogUtil.Log("HandleGetFullScreenAdSucceeded");
        
        TapjoyPlugin.ShowFullScreenAd();
    }
    
    public void tapjoyHandleGetFullScreenAdFailed() {
        LogUtil.Log("HandleGetFullScreenAdFailed");
    }
    
    // DISPLAY ADS
    public void tapjoyHandleGetDisplayAdSucceeded() {
        LogUtil.Log("HandleGetDisplayAdSucceeded");
        
        if (!tapjoyOpeningFullScreenAd)
            TapjoyPlugin.ShowDisplayAd();
    }
    
    public void tapjoyHandleGetDisplayAdFailed() {
        LogUtil.Log("HandleGetDisplayAdFailed");
    }
    
    // VIDEO
    public void tapjoyHandleVideoAdStarted() {
        LogUtil.Log("HandleVideoAdStarted");
    }
    
    public void tapjoyHandleVideoAdFailed() {
        LogUtil.Log("HandleVideoAdFailed");
    }
    
    public void tapjoyHandleVideoAdCompleted() {
        LogUtil.Log("HandleVideoAdCompleted");
    }
    
    // VIEW OPENED  
    public void tapjoyHandleViewOpened(TapjoyViewType viewType) {
        LogUtil.Log("HandleViewOpened of view type " + viewType.ToString());
        tapjoyOpeningFullScreenAd = true;
    }
    
    // VIEW CLOSED  
    public void tapjoyHandleViewClosed(TapjoyViewType viewType) {
        LogUtil.Log("HandleViewClosed of view type " + viewType.ToString());
        tapjoyOpeningFullScreenAd = false;
    }
    
    // OFFERS
    public void tapjoyHandleShowOffersFailed() {
        LogUtil.Log("HandleShowOffersFailed");
    }
#endif

    #endregion

    // ----------------------------------------------------------------------
    // VUNGLE - http://prime31.com/docs#comboVungle

    #region VUNGLE

#if PROMO_USE_VUNGLE

    public void vungleInit() {
        LogUtil.Log("vungleInit");

        Vungle.init(AppConfigs.publisherIdVungleAndroid, AppConfigs.publisherIdVungleiOS);
        //Vungle.init(AppConfigs.publisherIdVungleAndroid, AppConfigs.publisherIdVungleiOS, int age, VungleGender gender );
        
    }
        
    public void vungleSetSoundEnabled(bool isEnabled) {
        LogUtil.Log("vungleSetSoundEnabled");
        Vungle.setSoundEnabled(isEnabled);
    }

    // Checks to see if a video is available
    public bool vungleIsAdvertAvailable() {
        LogUtil.Log("vungleIsAdvertAvailable");
        return Vungle.isAdvertAvailable();
    }
    
    public void vungleDisplayAdvert(bool showCloseButtonOnIOS) {
        LogUtil.Log("vungleDisplayAdvert");
        Vungle.displayAdvert(showCloseButtonOnIOS);
    }

    // Displays an incentivized advert with optional name
    public void vungleDisplayIncentivizedAdvert(bool showCloseButton, string user) {
        LogUtil.Log("vungleDisplayIncentivizedAdvert");
        Vungle.displayIncentivizedAdvert(showCloseButton, user);   
    }

    // Fired when a Vungle ad starts
    public void vungleOnAdStartedEvent() {
        LogUtil.Log("vungleOnAdStartedEvent");
        // Send started message to pause
    }

    // Fired when a Vungle ad finishes
    public void vungleOnAdEndedEvent() {
        LogUtil.Log("vungleOnAdEndedEvent");
        // Send started message to unpause and credit if successful  
        
        Messenger<double>.Broadcast(AdNetworksMessages.videoAd, 1f);
    }
    
    // Fired when a Vungle video is dismissed and provides the time watched and total duration in that order.
    public void vungleOnAdViewedEvent(double timeWatched, double totalDuration) {
        LogUtil.Log("vungleOnAdViewedEvent");
        // check for success if watched more than 90% of video.

        Messenger<double>.Broadcast(AdNetworksMessages.videoAd, timeWatched / totalDuration);
    }
#endif

    #endregion

    // ----------------------------------------------------------------------
    // CHARTBOOST

    #region CHARTBOOST

#if PROMO_USE_CHARTBOOST

    public void chartboostInit() {

//#if UNITY_ANDROID
           // ChartboostSDK.Chartboost.init(AppConfigs.publisherIdCharboostAndroid, AppConfigs.publisherSecretCharboostAndroid);
//#elif UNITY_IPHONE
            //CBBinding.init(AppConfigs.publisherIdCharboostiOS, AppConfigs.publisherSecretCharboostiOS);
//#endif
    }

    public void chartboostShowInterstitial() {
        chartboostShowInterstitial(null);
    }
    
    public void chartboostShowInterstitial(CBLocation location) {
        Chartboost.showInterstitial(location);
    }
    
    public void chartboostCacheInterstitial() {
        chartboostCacheInterstitial(null);
    }

    public void chartboostCacheInterstitial(CBLocation location) {
        Chartboost.cacheInterstitial(location);
    }
    
    public void chartboostCacheMoreApps(CBLocation location) {
        Chartboost.cacheMoreApps(location);
    }

    //public void chartboostForceOrientation(ScreenOrientation screenOrientation) {
    //    Chartboost..forceOrientation(screenOrientation);
    //}

    public bool chartboostHasInterstitial(CBLocation location) {
        return Chartboost.hasInterstitial(location);
    }
    
    public void chartboostHasMoreApps(CBLocation location) {
        Chartboost.hasMoreApps(location);
    }
    
    public bool chartboostIsImpressionVisible() {
        return Chartboost.isImpressionVisible();
    }
    
    public void chartboostShowMoreApps(CBLocation location) {
        Chartboost.showMoreApps(location);
    }
    
    public void chartboostTrackEvent(string eventIdentifier, double value, Dictionary<string,object> metaData) {
        //CBBinding.trackEvent(eventIdentifier, (float)value, metaData);
    }

    /// Fired when an interstitial fails to load
    /// First parameter is the location.
    public void chartboostDidFailToLoadInterstitialEvent(CBLocation data, ChartboostSDK.CBImpressionError error) {

    }
    
    /// Fired when an interstitial is finished via any method
    /// This will always be paired with either a close or click event
    /// First parameter is the location.
    public void chartboostDidDismissInterstitialEvent(CBLocation data) {
        
    }
    
    /// Fired when an interstitial is closed (i.e. by tapping the X or hitting the Android back button)
    /// First parameter is the location.
    public void chartboostDidCloseInterstitialEvent(CBLocation data) {
        
    }
    
    /// Fired when an interstitial is clicked
    /// First parameter is the location.
    public void didClickInterstitialEvent(CBLocation data) {
        
    }
    
    /// Fired when an interstitial is cached
    /// First parameter is the location.
    public void chartboostDidCacheInterstitialEvent(CBLocation data) {
        
    }
    
    /// Fired when an interstitial is shown
    /// First parameter is the location.
    public void chartboostDidShowInterstitialEvent(CBLocation data) {
        
    }
    
    /// Fired when the More Apps screen fails to load
    public void chartboostDidFailToLoadMoreAppsEvent() {

    }
    
    /// Fired when the More Apps screen is finished via any method
    /// This will always be paired with either a close or click event
    public void chartboostDidDismissMoreAppsEvent() {
        
    }
    
    /// Fired when the More Apps screen is closed (e.g., by tapping the X or hitting the Android back button)
    public void chartboostDidCloseMoreAppsEvent() {
        
    }
    
    /// Fired when a listing on the More Apps screen is clicked
    public void chartboostDidClickMoreAppsEvent() {
        
    }
    
    /// Fired when the More Apps screen is cached
    public void chartboostDidCacheMoreAppsEvent() {
        
    }
    
    /// Fired when the more app screen is shown
    public void chartboostDidShowMoreAppsEvent() {
        
    }
#endif

    #endregion

    public bool interstitialReady = false;

    #region IAD

#if AD_USE_IAD
    // ----------------------------------------------------------------------
    // APPLE iAD

    public void iadInit() {
        //LogUtil.Log("InitiAd AppConfigs.publisherIdAdmobiOS..." + 
        //            AppConfigs.publisherIdAdmobiOS);
        //LogUtil.Log("InitAdmob AppConfigs.publisherIdAdmobAndroid..." + 
        //            AppConfigs.publisherIdAdmobAndroid);

        //iadFireHideShowEvents(true);

        iadPreparePrerollAds();
    }

    public void iadBannerViewDidChangeEvent(bool visible) {
    
        if(visible) {
            // shown
        }
        else {
            // hidden
        }
    }

    public void iadInterstitalAdFailedEvent(string val) {
        interstitialReady = false;
    }

    public void iadInterstitalAdLoadedEvent() {
        interstitialReady = true;
    }
        
    public void iadInterstitialDidUnloadEvent() {
    
    }

    public void iadMoviePlaybackCompletedEvent() {
    
    }

    // Starts up iAd either on the top or bottom of the screen
    //public static void iadCreateAdBanner( bool bannerOnBottom = true ) {
    //    AdBinding.createAdBanner(bannerOnBottom);
    //}

    // Starts up iAd requests and ads the ad view
    public static void iadCreateAdBanner( iAdBannerPosition position, iAdBannerType type = iAdBannerType.Banner ) {
        AdBinding.createAdBanner(position, type);
    }
    
    // Destroys the ad banner and removes it from view
    public static void iadDestroyAdBanner() {
        AdBinding.destroyAdBanner();
    }
    
    // Starts loading a new interstitial ad. Interstitials are available on all iPads and iPhones running iOS 7+.
    public static void iadLoadInterstitial() {
        AdBinding.loadInterstitial();
    }
    
    // Checks to see if an interstitial ad is loaded
    public static bool iadIsInterstitalLoaded() {
        return AdBinding.isInterstitalLoaded();
    }
        
    //// Sets whether or not adDidShow events should be fired or not
    //public static void iadFireHideShowEvents( bool shouldFire ) {
    //    AdBinding.fireHideShowEvents(shouldFire);
    //}

    //// Starts loading a new interstitial ad.  Returns false when interstitials are not supported.
    //public static bool iadInitializeInterstitial() {
    //    return AdBinding.initializeInterstitial();
    //}
            
    // Shows an interstitial ad.  Will return false if it isn't loaded.
    public static bool iadShowInterstitial() {

        return AdBinding.showInterstitial();
    }    
    
    // Prepares iAd video preroll ads. This should be called at or near app launch time.
    public static void iadPreparePrerollAds() {

        AdBinding.preparePrerollAds();
    }
        
    // Plays a video with a pre roll ad. Accepted strings are a URL to a video file, the filename of a video in the app bundle or
    // a proper, absolutel path to a video file.
    public static void iAdPlayMovieWithPrerollAd( string videoPathOrUrl ) {
        AdBinding.playMovieWithPrerollAd(videoPathOrUrl);        
    }

#endif

    // HELPERS

    public bool iadShowBannerAd(AdDisplayType bannerType, AdPosition position) {
        // Try to get each typeof ad then show it

        bool hasAd = false;

#if AD_USE_IAD
        
        if(bannerType == AdDisplayType.Banner) {
            
            bool showOnBottom = true;
            
            if(position == AdPosition.TopCenter
               || position == AdPosition.TopRight
               || position == AdPosition.TopLeft) {
                showOnBottom = false;
            }

            if(showOnBottom) {
                iadCreateAdBanner(iAdBannerPosition.Bottom, iAdBannerType.Banner);
            } 
            else {
                iadCreateAdBanner(iAdBannerPosition.Top, iAdBannerType.Banner);
            }
            
            hasAd = true;
        }

#endif

        return hasAd;
    }

    public bool iadHideBannerAd() {

#if AD_USE_IAD
#if UNITY_IPHONE
            iadDestroyAdBanner();
#endif
#endif

        return true;
    }

    #endregion

    // ----------------------------------------------------------------------
    // UNITY ADS

#if AD_USE_UNITY

    const string unityRewardedPlacementId = "rewardedVideo";

    public void unityAdsInit() {

        if (!Advertisement.isInitialized) {

            // Now that Unity has changed their Ad api for the millionth time...
            ////Advertisement.AddListener(this);

#if UNITY_IOS
            Advertisement.Initialize(
                AppConfigs.adNetworksUnityPublisherIdiOS, 
                AppConfigs.adNetworksUnityTestModeEnabled);
#elif UNITY_ANDROID
            Advertisement.Initialize(
                AppConfigs.adNetworksUnityPublisherIdAndroid,
                AppConfigs.adNetworksUnityTestModeEnabled);
#elif UNITY_WEBGL
            Advertisement.Initialize(
                AppConfigs.adNetworksUnityPublisherIdAndroid,
                AppConfigs.adNetworksUnityTestModeEnabled);
#elif UNITY_STANDALONE
            Advertisement.Initialize(
                AppConfigs.adNetworksUnityPublisherIdAndroid,
                AppConfigs.adNetworksUnityTestModeEnabled);
#endif

        }
    }

    public void unityShowVideoAd() {

        unityAdShow();
    }

    public void unityShowRewardAd(string code = "rewardedVideo") {

        unityAdShow(code);
    }

    public bool unityIsAdReady(string code = null) {

        if (code.IsNullOrEmpty()) {
            //return Advertisement.IsReady();
        }
        else {
            //return Advertisement.IsReady(code);
        }

        return false;
    }

    public void unityAdShow(string code = null) {

        if (!unityIsAdReady()) {
            return;
        }

        //ShowOptions options = new ShowOptions { resultCallback = unityIsAdShowCompleted };
        //options.resultCallback.
        //Advertisement.AddListener(unityIsAdShowCompleted);

        if (code.IsNullOrEmpty()) {
            //Advertisement.Show();
        }
        else {
            Advertisement.Show(code);
        }
    }

    //public void unityIsAdShowCompleted(ShowResult result) {

    //    switch(result) {
    //        case ShowResult.Finished:
    //            Debug.Log("The ad was successfully shown.");

    //            Messenger<double>.Broadcast(AdNetworksMessages.videoAd, 1f);

    //            break;

    //        case ShowResult.Skipped:
    //            Debug.Log("The ad was skipped before reaching the end.");

    //            Messenger<double>.Broadcast(AdNetworksMessages.videoAd, .5f);
    //            break;

    //        case ShowResult.Failed:
    //            Debug.LogError("The ad failed to be shown.");
    //            break;
    //    }
    //}

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {

        // Define conditional logic for each ad completion status:

        if (showResult == ShowResult.Finished) {

            // Reward the user for watching the ad to completion.
            Messenger<double>.Broadcast(AdNetworksMessages.videoAd, 1f);
        }
        else if (showResult == ShowResult.Skipped) {

            // Do not reward the user for skipping the ad.

            Messenger<double>.Broadcast(AdNetworksMessages.videoAd, .5f);
        }
        else if (showResult == ShowResult.Failed) {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsReady(string placementId) {
        // If the ready Placement is rewarded, show the ad:
        //if (placementId == myPlacementId) {
        //    Advertisement.Show(myPlacementId);
        //}
    }

    public void OnUnityAdsDidError(string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId) {
        // Optional actions to take when the end-users triggers an ad.
    }

#endif

    // ----------------------------------------------------------------------

    // GOOGLE ADMOB

    #region ADMOB

#if AD_USE_ADMOB
    public void admobInit() {
        LogUtil.Log("InitAdmob AppConfigs.publisherIdAdmobiOS..." +
            AppConfigs.publisherIdAdmobiOS);
        LogUtil.Log("InitAdmob AppConfigs.publisherIdAdmobAndroid..." +
            AppConfigs.publisherIdAdmobAndroid);


        // Enable networks

        // Social Network Prime31
        if(Application.platform == RuntimePlatform.Android) {
            //LogUtil.Log("InitAdmob RuntimePlatform.Android..." + 
            //Application.platform);          
#if UNITY_ANDROID
            //AdMob.init(AppConfigs.publisherIdAdmobAndroid, adNetworkTestingEnabled);            
            //LogUtil.Log("InitAdmob Admob init..." + AppConfigs.publisherIdAdmobAndroid);
#endif
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer) {
#if UNITY_IPHONE
            
            //AdMob.setTestDevices(AppConfigs.adTestDeviceIdsiOS);

            //AdMobBinding.init(
            //    AppConfigs.publisherIdAdmobiOS, adNetworkTestingEnabled);
            LogUtil.Log("InitAdmob RuntimePlatform.IPhonePlayer..." + 
                        AppConfigs.publisherIdAdmobiOS);
#endif
        }
        else {
            // Web player...

#if UNITY_WEBPLAYER
            //Application.ExternalCall("if(window.console) window.console.log","web facebook init");
#endif
        }
    }

#endif

#if !UNITY_WEBPLAYER
#if AD_USE_ADMOB
    public AdMobBanner admobGetBanner(AdBanner banner) {
        if (banner == AdBanner.Phone_320x50) {
            return AdMobBanner.Phone_320x50;
        }
        else if (banner == AdBanner.Tablet_300x250) {
            return AdMobBanner.Tablet_300x250;
        }
        else if (banner == AdBanner.Tablet_468x60) {
            return AdMobBanner.Tablet_468x60;
        }
        else if (banner == AdBanner.Tablet_728x90) {
            return AdMobBanner.Tablet_728x90;
        }
        else {
            return AdMobBanner.SmartBanner;
        }
    }
#endif
#endif
#if !UNITY_ANDROID && !UNITY_WEBPLAYER && AD_USE_ADMOB
    public AdMobBannerType admobGetBannerType(AdDisplayType bannerType) {
        if (bannerType == AdDisplayType.iPad_320x250) {
            return AdMobBannerType.iPad_320x250;
        }
        else if (bannerType == AdDisplayType.iPad_468x60) {
            return AdMobBannerType.iPad_468x60;
        }
        else if (bannerType == AdDisplayType.iPad_728x90) {
            return AdMobBannerType.iPad_728x90;
        }
        else if (bannerType == AdDisplayType.iPhone_320x50) {
            return AdMobBannerType.iPhone_320x50;
        }
        else if (bannerType == AdDisplayType.SmartBannerPortrait) {
            return AdMobBannerType.SmartBannerPortrait;
        }
        else {
            return AdMobBannerType.SmartBannerLandscape;
        }
    }
#endif

#if !UNITY_WEBPLAYER
#if AD_USE_ADMOB
    public AdMobLocation admobGetPlacementType(AdPlacementType bannerType) {
        if (bannerType == AdPlacementType.BottomLeft) {
            return AdMobLocation.BottomLeft;
        }
        else if (bannerType == AdPlacementType.BottomCenter) {
            return AdMobLocation.BottomCenter;
        }
        else if (bannerType == AdPlacementType.BottomRight) {
            return AdMobLocation.BottomRight;
        }
        else if (bannerType == AdPlacementType.TopLeft) {
            return AdMobLocation.TopLeft;
        }
        else if (bannerType == AdPlacementType.TopRight) {
            return AdMobLocation.TopRight;
        }
        else if (bannerType == AdPlacementType.TopLeft) {
            return AdMobLocation.TopLeft;
        }
        else if (bannerType == AdPlacementType.Centered) {
            return AdMobLocation.Centered;
        }
        else {
            return AdMobLocation.TopCenter;
        }
    }
#endif
#endif

#if !UNITY_ANDROID && !UNITY_WEBPLAYER && AD_USE_ADMOB
    public AdMobAdPosition admobGetPosition(AdPosition position) {
        if (position == AdPosition.BottomLeft) {
            return AdMobAdPosition.BottomLeft;
        }
        else if (position == AdPosition.BottomCenter) {
            return AdMobAdPosition.BottomCenter;
        }
        else if (position == AdPosition.BottomRight) {
            return AdMobAdPosition.BottomRight;
        }
        else if (position == AdPosition.TopLeft) {
            return AdMobAdPosition.TopLeft;
        }
        else if (position == AdPosition.TopRight) {
            return AdMobAdPosition.TopRight;
        }
        else if (position == AdPosition.TopLeft) {
            return AdMobAdPosition.TopLeft;
        }
        else if (position == AdPosition.Centered) {
            return AdMobAdPosition.Centered;
        }
        else {
            return AdMobAdPosition.TopCenter;
        }
    }
#endif

    // HELPERS

    public bool admobShowBannerAd() {
#if AD_USE_ADMOB
        if (Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
            //AdMob.createBanner(
            //admobGetBannerType(bannerType), 
            //admobGetPosition(placementType)
            //);
#endif
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer) {            
#if UNITY_IPHONE
            AdMobBinding.createBanner(
                admobGetBannerType(bannerType), 
                admobGetPosition(position)
                );
#endif
        }
        else {
            // Web player...
#if UNITY_WEBPLAYER
            //Application.ExternalCall("if(window.console) window.console.log","web show twitter login");
#endif
        }
#endif

        return false;
    }

    public bool admobHideBannerAd() {

#if AD_USE_ADMOB
        if (Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
            AdMob.destroyBanner();
            return true;
#endif
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer) {            
#if UNITY_IPHONE
            AdMobBinding.destroyBanner();
            return true;
#endif
        }
        else {
            // Web player...
#if UNITY_WEBPLAYER
            //Application.ExternalCall("if(window.console) window.console.log","web show twitter login");
#endif
        }
#endif

        return false;
    }

    #endregion

    // ----------------------------------------------------------------------

    // GENERIC CALLS


    // ----------------------------------------------------------------------

    // ADS

    //!CBBinding.isImpressionVisible(); 

    //public static void ShowAd() {
    //    if (Instance != null) {
    //        Instance.showAd();
    //    }
    //}

    public static void ShowAd(
        AdDisplayType adDisplayType = AdDisplayType.Banner,
        AdPosition adPosition = AdPosition.BottomCenter) {

        if (Instance != null) {
            Instance.showAd(adDisplayType, adPosition);
        }
    }

    public void showAd() {
        showAd(AdDisplayType.Banner, AdPosition.BottomCenter);
    }

    public void showAd(
        AdDisplayType adDisplayType = AdDisplayType.Banner,
        AdPosition adPosition = AdPosition.BottomCenter) {

        if (adDisplayType == AdDisplayType.Banner) {
            showBannerAd(adDisplayType, adPosition);
        }
        else if (adDisplayType == AdDisplayType.Interstitial) {
        }
        else if (adDisplayType == AdDisplayType.Interstitial) {
        }
    }

    public static void HideAd() {
        if (Instance != null) {
            Instance.hideAd();
        }
    }

    public void hideAd() {
        // TODO current ad and ad queue

        hideBannerAd();
    }

    // ----------------------------------------------------------------------

    // BANNERS

    //public static void ShowBannerAd() {
    //    if (Instance != null) {
    //        Instance.showBannerAd();
    //    }
    //}

    public static void ShowBannerAd(
        AdDisplayType bannerType = AdDisplayType.Banner,
        AdPosition position = AdPosition.BottomCenter) {

        if (Instance != null) {
            Instance.showBannerAd(bannerType, position);
        }
    }

    public void showBannerAd() {
        showAd(AdDisplayType.SmartBannerLandscape, AdPosition.BottomCenter);
    }

    public void showBannerAd(
        AdDisplayType bannerType = AdDisplayType.Banner,
        AdPosition position = AdPosition.BottomCenter) {

        bool hasAd = false;

#if AD_USE_IAD
        if(!hasAd) {
            hasAd = iadShowBannerAd(bannerType, position);
        }
#endif

        if (!hasAd) {

        }

    }

    public static void HideBannerAd() {
        if (Instance != null) {
            Instance.hideBannerAd();
        }
    }

    public void hideBannerAd() {

#if AD_USE_IAD
        iadHideBannerAd();
#endif

#if AD_USE_ADMOB
        admobHideBannerAd();
#endif
    }

    // ----------------------------------------------------------------------

    // VIDEO ADS

    public static void SetVideoAdSoundEnabled(bool isEnabled) {
        if (Instance != null) {
            Instance.setVideoAdSoundEnabled(isEnabled);
        }
    }

    public void setVideoAdSoundEnabled(bool isEnabled) {
#if PROMO_USE_VUNGLE
        vungleSetSoundEnabled(isEnabled);
#endif
    }

    //

    public static void IsVideoAdAvailable() {
        if (Instance != null) {
            Instance.isVideoAdAvailable();
        }
    }

    public bool isVideoAdAvailable() {

#if PROMO_USE_VUNGLE
        return vungleIsAdvertAvailable();
#else
#if AD_USE_UNITY
        return unityIsAdReady();
#else
        return false;
#endif
#endif
    }

    //

    public static void ShowVideoAd(bool showCloseButtons = true, string code = null) {
        if (Instance != null) {
            Instance.showVideoAd(showCloseButtons);
        }
    }

    public void showVideoAd(bool showCloseButtons = true, string code = null) {

#if PROMO_USE_VUNGLE
        if (vungleIsAdvertAvailable()) {
            vungleDisplayAdvert(showCloseButtons);
        }
#endif

#if AD_USE_UNITY

        if (showCloseButtons) {
            if (code.IsNullOrEmpty()) {
                unityShowRewardAd();
            }
            else {
                unityShowRewardAd(code);
            }
        }
        else {
            unityShowVideoAd();
        }
#endif
    }

    //

    public static void ShowVideoAdIncentivized(bool showCloseButtons = true, string code = null, string user = null) {
        if (Instance != null) {
            Instance.showVideoAdIncentivized(showCloseButtons, code, user);
        }
    }

    public void showVideoAdIncentivized(bool showCloseButtons = true, string code = null, string user = null) {

#if PROMO_USE_VUNGLE
        if (vungleIsAdvertAvailable()) {
            vungleDisplayIncentivizedAdvert(showCloseButton, user);
        }
#endif

#if AD_USE_UNITY
        if (code.IsNullOrEmpty()) {
            unityShowRewardAd();
        }
        else {
            unityShowRewardAd(code);
        }
#endif
    }

    // ----------------------------------------------------------------------

    // OFFERS

    public static void ShowOfferWall() {
        if (Instance != null) {
            Instance.showOfferWall();
        }
    }

    public void showOfferWall() {

#if PROMO_USE_TAPJOY
        tapjoyShowOffers();
#endif
    }

    // ----------------------------------------------------------------------
    // DISPLAY AD


    public static void ShowDisplayAd() {
        if (Instance != null) {
            Instance.showDisplayAd();
        }
    }

    public void showDisplayAd() {

#if PROMO_USE_TAPJOY
        tapjoyShowDisplayAd();
#endif
    }

    // ----------------------------------------------------------------------

    // FULLSCREEN ADS

    public static void ShowFullscreenAd() {
        if (Instance != null) {
            Instance.showFullscreenAd();
        }
    }

    public void showFullscreenAd() {

#if PROMO_USE_TAPJOY
        tapjoyShowFullscreenAd();
#endif
    }

    // ----------------------------------------------------------------------

    // MORE APPS

    public static void ShowMoreApps() {
        if (Instance != null) {
            Instance.showMoreApps();
        }
    }

    public void showMoreApps() {

#if PROMO_USE_CHARTBOOST
        CBLocation location = CBLocation.LevelStart;
        chartboostShowMoreApps(location);
#endif
    }

    public static void ShowInterstitial() {
        if (Instance != null) {
            Instance.showInterstitial();
        }
    }

    public void showInterstitial() {

#if PROMO_USE_CHARTBOOST
        chartboostShowInterstitial();
#endif
    }

    // ----------------------------------------------------------------------

    public static void HandleAdUpdate() {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKeyUp(KeyCode.Escape)) {
#if PROMO_USE_CHARTBOOST
                //if (Chartboost..onBackPressed())
                //    return;
                //else
                    Application.Quit();
#else
                Application.Quit();
#endif
            }
        }
#endif
    }
}

// ----------------------------------------------------------------------
// DOCS



/*

// ----------------------------------------------------------------------
UNITY ADS

using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsExample : MonoBehaviour {
  public void ShowAd() {
    if (Advertisement.IsReady()) {
      Advertisement.Show();
    }
  }
}

using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsExample : MonoBehaviour {
  public void ShowRewardedAd() {
    if (Advertisement.IsReady("rewardedVideo")) {
      var options = new ShowOptions { resultCallback = HandleShowResult };
      Advertisement.Show("rewardedVideo", options);
    }
  }

  private void HandleShowResult(ShowResult result) {
    switch (result) {
      case ShowResult.Finished:
        Debug.Log("The ad was successfully shown.");
        //
        // YOUR CODE TO REWARD THE GAMER
        // Give coins etc.
        break;
      case ShowResult.Skipped:
        Debug.Log("The ad was skipped before reaching the end.");
        break;
      case ShowResult.Failed:
        Debug.LogError("The ad failed to be shown.");
        break;
    }
  }
}

*/

// ----------------------------------------------------------------------
// CHARTBOOST

//https://help.chartboost.com/documentation/unity/delegates
/*
/// Initializes the Chartboost plugin and, on iOS, records the beginning of a user session
public static void init()

/// Caches an interstitial. Location is optional. Pass in "null" if you do not want to specify the location.
public static void cacheInterstitial( string location )

/// Checks to see if an interstitial is cached
public static bool hasCachedInterstitial( string location )

/// Shows an interstitial. Location is optional. Pass in "null" if you do not want to specify the location.
public static void showInterstitial( string location )

/// Caches the More Apps screen
public static void cacheMoreApps()

/// Checks to see if the More Apps screen is cached
public static bool hasCachedMoreApps()

/// Shows the More Apps screen
public static void showMoreApps()

/// Returns true if an impression (interstitial or More Apps page) is currently visible
public static bool isImpressionVisible()

/// Forces the orientation of impressions to the given orientation
public static void forceOrientation( ScreenOrientation orient )

/// Android only: return whether impressions are shown using an additional activity instead
///   of just overlaying on top of the Unity activity.  Default is true.
/// See `Plugins/Android/res/values/strings.xml` to set this value.
public static bool getImpressionsUseActivities()

/// Android only: used to notify Chartboost that the Android back button has been pressed
/// Returns true to indicate that Chartboost has handled the event and it should not be further processed
public static bool onBackPressed()

/// EVENTSSS>>>

/// Fired when an interstitial fails to load
/// First parameter is the location.
public static event Action<string> didFailToLoadInterstitialEvent;

/// Fired when an interstitial is finished via any method
/// This will always be paired with either a close or click event
/// First parameter is the location.
public static event Action<string> didDismissInterstitialEvent;

/// Fired when an interstitial is closed (i.e. by tapping the X or hitting the Android back button)
/// First parameter is the location.
public static event Action<string> didCloseInterstitialEvent;

/// Fired when an interstitial is clicked
/// First parameter is the location.
public static event Action<string> didClickInterstitialEvent;

/// Fired when an interstitial is cached
/// First parameter is the location.
public static event Action<string> didCacheInterstitialEvent;

/// Fired when an interstitial is shown
/// First parameter is the location.
public static event Action<string> didShowInterstitialEvent;

/// Fired when the More Apps screen fails to load
public static event Action didFailToLoadMoreAppsEvent;

/// Fired when the More Apps screen is finished via any method
/// This will always be paired with either a close or click event
public static event Action didDismissMoreAppsEvent;

/// Fired when the More Apps screen is closed (e.g., by tapping the X or hitting the Android back button)
public static event Action didCloseMoreAppsEvent;

/// Fired when a listing on the More Apps screen is clicked
public static event Action didClickMoreAppsEvent;

/// Fired when the More Apps screen is cached
public static event Action didCacheMoreAppsEvent;

/// Fired when the more app screen is shown
public static event Action didShowMoreAppsEvent;


 */

// ----------------------------------------------------------------------
// ADMOB

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

// ----------------------------------------------------------------------
// VUNGLE

/*VungleBinding.cs exposes the following methods:
// Starts up the SDK with the given appId
public static void startWithAppId( string appId )

// Enables/disables sound
public static void setSoundEnabled( bool enabled )

// Starts up the SDK with the given appId and userData. See VGUserData for allowed values.
public static void startWithAppIdAndUserData( string appId, Dictionary<string,object> userData )

// Enables verbose logging
public static void enableLogging()

// Sets the maximum size in megabytes of the video cache
public static void setCacheSize( int cacheSize )

// Checks to see if a video ad is available
public static bool isAdAvailable()

// Shuts down the Vungle SDK
public static void stop()

// Plays a modal video ad optionally showing a close button
public static void playModalAd( bool showCloseButton )

// Plays an insentivised video ad optionally showing a close button
public static void playInsentivisedAd( string user, bool showCloseButton )

// Sets Vungle to allow auto rotation or not
public static void allowAutoRotate( bool shouldAllow )
VungleManager.cs fires the following events:
// Fired when a video has finished playing
public static event Action<Dictionary<string,object>> vungleMoviePlayedEvent;

// Fired when the Vungle SDK has a status update
public static event Action<Dictionary<string,object>> vungleStatusUpdateEvent;

// Fired when the video is dismissed
public static event Action vungleViewDidDisappearEvent;

// Fired when the video is shown
public static event Action vungleViewWillAppearEvent;

// COMBO

Vungle.cs exposes the following methods:
// Initializes the Vungle SDK optionally with an age and gender
public static void init( string androidAppId, string iosAppId )

public static void init( string androidAppId, string iosAppId, int age, VungleGender gender )

// Sets if sound should be enabled or not
public static void setSoundEnabled( bool isEnabled )

// Checks to see if a video is available
public static bool isAdvertAvailable()

// Displays an advert
public static void displayAdvert( bool showCloseButtonOnIOS )

// Displays an incentivized advert with optional name
public static void displayIncentivizedAdvert( bool showCloseButton, string user )
Vungle.cs fires the following events:
// Fired when a Vungle ad starts
public static event Action onAdStartedEvent;

// Fired when a Vungle ad finishes
public static event Action onAdEndedEvent;

// Fired when a Vungle video is dismissed and provides the time watched and total duration in that order.
public static event Action<double,double> onAdViewedEvent;
*/
/*
 * 
 * iAd
// Starts up iAd either on the top or bottom of the screen
public static void createAdBanner( bool bannerOnBottom );

// Destroys the ad banner and removes it from view
public static void destroyAdBanner()

// Sets whether or not adDidShow events should be fired or not
public static void fireHideShowEvents( bool shouldFire )

// Starts loading a new interstitial ad.  Returns false when interstitials are not supported.
public static bool initializeInterstitial()

// Checks to see if an interstitial ad is loaded.
public static bool isInterstitalLoaded()

// Shows an interstitial ad.  Will return false if it isn't loaded.
public static bool showInterstitial()

*/
/*
 * iAd Manager
// Fired when the adView is either shown or hidden
public static event Action<bool> adViewDidChange;

// Fired when an interstial ad fails to load or show
public static event Action<string> interstitalAdFailed;

// Fired when an interstitial ad is loaded and ready to show
public static event Action interstitialAdLoaded;


// Fired when the adView is either shown or hidden. The bool indicates if it has been shown.
public static event Action<bool> bannerViewDidChangeEvent;

// Fired when an interstitial ad is loaded and ready to show
public static event Action interstitialAdLoadedEvent;

// Fired when an interstial ad fails to load or show
public static event Action<string> interstitalAdFailedEvent;

// Fired when an interstitial unloads. This can occur when it times out, expires or after it is done being displayed.
public static event Action interstitialDidUnloadEvent;

// Fired when movie playback completes
public static event Action moviePlaybackCompletedEvent;

*/
