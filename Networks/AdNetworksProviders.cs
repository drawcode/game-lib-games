using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

using Engine.Events;
using Engine.Utility;

// ----------------------------------------------------------------------
// MODERN AD PROVIDERS
//
// Adds current SDKs (LevelPlay mediation, direct Google Mobile Ads) to the
// AdNetworks facade WITHOUT touching the legacy vendor paths in AdNetworks.cs.
//
// How a request is served:
//   1. Ad removal gate (AdRemoval.adsRemoved) - suppresses banner/interstitial
//      for every path, legacy included.
//   2. Configured providers, in AdNetworksConfig.providerOrder - first one
//      that is ready serves the request.
//   3. Nothing claimed it - falls through to the legacy code unchanged.
//
// Apps that never call AdNetworks.Configure() get exactly the old behaviour.
//
// Provider SDKs are compiled in per define:
//   AD_USE_LEVELPLAY  - com.unity.services.levelplay 9.x  (AdNetworkType.LevelPlay)
//   AD_USE_ADMOB_GMA  - Google Mobile Ads Unity plugin 9.x+ (AdNetworkType.AdmobDirect)
// (AD_USE_ADMOB is the legacy Prime31 binding and is unrelated.)

public enum AdFormat {
    Banner,
    Interstitial,
    Rewarded
}

public interface IAdNetworkProvider {

    AdNetworkType networkType { get; }

    bool isInitialized { get; }

    // onRewarded is called with 1.0 once a rewarded ad grants its reward.
    void Init(AdNetworksConfig config, Action<double> onRewarded);

    bool IsReady(AdFormat format);

    bool ShowBanner(AdPosition position);

    void HideBanner();

    bool ShowInterstitial(string placement);

    bool ShowRewarded(string placement);
}

[Serializable]
public class AdNetworksPlatformIds {

    // LevelPlay (Unity mediation) - app key and ad unit ids from the LevelPlay dashboard.
    // AdMob served THROUGH LevelPlay needs no ids here, only the AdMob adapter and the
    // AdMob App ID in Ads Mediation > Network Settings (checked by AdNetworksBuildValidator).
    public string levelPlayAppKey = "";
    public string levelPlayBannerAdUnitId = "";
    public string levelPlayInterstitialAdUnitId = "";
    public string levelPlayRewardedAdUnitId = "";

    // AdMob direct (Google Mobile Ads SDK, not mediated).
    // App ID format: ca-app-pub-XXXXXXXXXXXXXXXX~NNNNNNNNNN
    // The publisher id (pub-XXXXXXXXXXXXXXXX) is NOT an App ID.
    public string admobAppId = "";
    public string admobBannerAdUnitId = "";
    public string admobInterstitialAdUnitId = "";
    public string admobRewardedAdUnitId = "";
}

[Serializable]
public class AdNetworksConfig {

    public bool enabled = true;

    // Test ads / integration validation. Off in release builds by default.
    public bool testMode = isDevelopmentBuild;

    // Providers tried in order. AdmobDirect only when not mediating AdMob via LevelPlay.
    public List<AdNetworkType> providerOrder =
        new List<AdNetworkType> { AdNetworkType.LevelPlay };

    public bool bannersEnabled = true;
    public bool interstitialsEnabled = true;
    public bool rewardedEnabled = true;

    // Rewarded ads are opt-in, so by default they survive ad removal.
    public bool keepRewardedWhenAdsRemoved = true;

    // Frequency cap between interstitials, real seconds.
    public float interstitialMinSecondsBetween = 60f;

    // Privacy - applied before SDK init.
    public bool childDirected = false;
    public bool doNotSell = false;
    // -1 unknown (leave to CMP / SDK default), 0 denied, 1 granted.
    public int consentState = -1;

    // iOS: pause Unity while a fullscreen ad shows.
    public bool pauseGameDuringAds = true;

    public AdNetworksPlatformIds ios = new AdNetworksPlatformIds();
    public AdNetworksPlatformIds android = new AdNetworksPlatformIds();

    public AdNetworksPlatformIds current {
        get {
#if UNITY_IOS
            return ios;
#else
            return android;
#endif
        }
    }

    public static bool isDevelopmentBuild {
        get {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return true;
#else
            return false;
#endif
        }
    }

    static readonly Regex admobAppIdPattern =
        new Regex(@"^ca-app-pub-\d{16}~\d{10}$");

    static readonly Regex admobPublisherIdPattern =
        new Regex(@"^(ca-app-)?pub-\d{16}$");

    public static bool IsValidAdmobAppId(string appId) {
        return !string.IsNullOrEmpty(appId) && admobAppIdPattern.IsMatch(appId.Trim());
    }

    public static bool IsAdmobPublisherId(string id) {
        return !string.IsNullOrEmpty(id) && admobPublisherIdPattern.IsMatch(id.Trim());
    }

    public static AdNetworksConfig FromJson(string json) {
        return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<AdNetworksConfig>(json);
    }
}

public partial class AdNetworks {

    static AdNetworksConfig _config;
    static readonly List<IAdNetworkProvider> _providers = new List<IAdNetworkProvider>();

    // Static: GameGlobal adds AdNetworks more than once (InitAds runs twice), and every
    // instance calls Init() from Start - providers must initialise exactly once.
    static bool providersInitialized = false;
    float providersLastInterstitialTime = float.MinValue;

    public static AdNetworksConfig config {
        get {
            return _config;
        }
    }

    // Call once at boot (before the first ad request) to turn on the modern providers.
    public static void Configure(AdNetworksConfig config) {

        _config = config;
        _providers.Clear();

        if (config == null) {
            return;
        }

        adNetworksEnabled = config.enabled;
        adNetworkTestingEnabled = config.testMode;

        if (!config.enabled) {
            return;
        }

        foreach (AdNetworkType networkType in config.providerOrder) {

            IAdNetworkProvider provider = providerCreate(networkType);

            if (provider != null) {
                _providers.Add(provider);
            }
            else {
                LogUtil.LogWarning("AdNetworks: provider " + networkType +
                    " is not compiled in (missing define) - skipped.");
            }
        }

        AdNetworks instance = Instance;

        if (instance.transform.parent == null) {
            DontDestroyOnLoad(instance.gameObject);
        }

        instance.providersInit();
    }

    static IAdNetworkProvider providerCreate(AdNetworkType networkType) {

        switch (networkType) {
#if AD_USE_LEVELPLAY
            case AdNetworkType.LevelPlay:
                return new AdNetworkProviderLevelPlay();
#endif
#if AD_USE_ADMOB_GMA
            case AdNetworkType.AdmobDirect:
                return new AdNetworkProviderAdMob();
#endif
            default:
                return null;
        }
    }

    void providersInit() {

        if (providersInitialized || _config == null || _providers.Count == 0) {
            return;
        }

        providersInitialized = true;

        foreach (IAdNetworkProvider provider in _providers) {
            try {
                provider.Init(_config, providersOnRewarded);
            }
            catch (Exception e) {
                Debug.LogError("AdNetworks: " + provider.networkType + " init failed: " + e);
            }
        }
    }

    void providersOnRewarded(double fraction) {
        Messenger<double>.Broadcast(AdNetworksMessages.videoAd, fraction);
    }

    // Applies to every path, legacy included. Inert until AdRemoval grants removal.
    static bool adsSuppressed(AdFormat format) {

        if (!AdRemoval.adsRemoved) {
            return false;
        }

        if (format == AdFormat.Rewarded) {
            return _config != null && !_config.keepRewardedWhenAdsRemoved;
        }

        return true;
    }

    bool providersFormatEnabled(AdFormat format) {

        if (_config == null || !adNetworksEnabled || _providers.Count == 0) {
            return false;
        }

        switch (format) {
            case AdFormat.Banner:
                return _config.bannersEnabled;
            case AdFormat.Interstitial:
                return _config.interstitialsEnabled;
            default:
                return _config.rewardedEnabled;
        }
    }

    bool providersIsReady(AdFormat format) {

        if (!providersFormatEnabled(format)) {
            return false;
        }

        foreach (IAdNetworkProvider provider in _providers) {
            if (provider.isInitialized && provider.IsReady(format)) {
                return true;
            }
        }

        return false;
    }

    // Returns true when a provider owns banners (then legacy banners are skipped).
    bool providersShowBanner(AdPosition position) {

        if (!providersFormatEnabled(AdFormat.Banner)) {
            return false;
        }

        foreach (IAdNetworkProvider provider in _providers) {
            if (provider.isInitialized && provider.ShowBanner(position)) {
                return true;
            }
        }

        return false;
    }

    void providersHideBanner() {

        foreach (IAdNetworkProvider provider in _providers) {
            if (provider.isInitialized) {
                provider.HideBanner();
            }
        }
    }

    bool providersShowInterstitial(string placement) {

        if (!providersFormatEnabled(AdFormat.Interstitial)) {
            return false;
        }

        float now = Time.realtimeSinceStartup;

        if (now - providersLastInterstitialTime < _config.interstitialMinSecondsBetween) {
            // Capped - count as handled so legacy doesn't show one either.
            return true;
        }

        foreach (IAdNetworkProvider provider in _providers) {
            if (provider.isInitialized
                && provider.IsReady(AdFormat.Interstitial)
                && provider.ShowInterstitial(placement)) {

                providersLastInterstitialTime = now;
                return true;
            }
        }

        return false;
    }

    bool providersShowRewarded(string placement) {

        if (!providersFormatEnabled(AdFormat.Rewarded)) {
            return false;
        }

        foreach (IAdNetworkProvider provider in _providers) {
            if (provider.isInitialized
                && provider.IsReady(AdFormat.Rewarded)
                && provider.ShowRewarded(placement)) {
                return true;
            }
        }

        return false;
    }

    // Legacy callers pass "rewardedVideo" as the default code; that is not a
    // LevelPlay/AdMob placement, so let the SDK use its default placement.
    static string providersPlacement(string code) {
        return string.IsNullOrEmpty(code) || code == "rewardedVideo" ? null : code;
    }
}
