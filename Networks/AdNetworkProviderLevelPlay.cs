#if AD_USE_LEVELPLAY
using System;

using UnityEngine;

using Unity.Services.LevelPlay;

using Engine.Utility;

// LevelPlay (Unity / ironSource mediation) 9.x provider for the AdNetworks facade.
// Networks mediated here (Unity Ads, AdMob, AppLovin, ...) are chosen in the LevelPlay
// dashboard + Network Manager adapters, not in code. Callbacks arrive on the main thread.
public class AdNetworkProviderLevelPlay : IAdNetworkProvider {

    const float reloadDelaySeconds = 30f;

    AdNetworksConfig config;
    AdNetworksPlatformIds ids;
    Action<double> onRewarded;

    LevelPlayBannerAd banner;
    AdPosition bannerPosition;

    LevelPlayInterstitialAd interstitial;
    float interstitialLoadFailedTime = float.MinValue;
    bool interstitialLoading = false;

    LevelPlayRewardedAd rewarded;
    float rewardedLoadFailedTime = float.MinValue;
    bool rewardedLoading = false;

    public AdNetworkType networkType {
        get {
            return AdNetworkType.LevelPlay;
        }
    }

    public bool isInitialized { get; private set; }

    public void Init(AdNetworksConfig config, Action<double> onRewarded) {

        this.config = config;
        this.onRewarded = onRewarded;
        ids = config.current;

        if (string.IsNullOrEmpty(ids.levelPlayAppKey)) {
            Debug.LogError("AdNetworks: LevelPlay app key is not set for this platform - provider disabled.");
            return;
        }

        // Privacy flags must be set before Init.
        if (config.childDirected) {
            LevelPlay.SetMetaData("is_child_directed", "true");
        }

        if (config.doNotSell) {
            LevelPlay.SetMetaData("do_not_sell", "true");
        }

        if (config.consentState >= 0) {
            LevelPlay.SetConsent(config.consentState == 1);
        }

        if (config.testMode) {
            LevelPlay.SetAdaptersDebug(true);
            LevelPlay.SetMetaData("is_test_suite", "enable");
        }

        LevelPlay.OnInitSuccess += onInitSuccess;
        LevelPlay.OnInitFailed += onInitFailed;

        LevelPlay.Init(ids.levelPlayAppKey);
    }

    void onInitSuccess(LevelPlayConfiguration configuration) {

        isInitialized = true;

        LogUtil.LogAds("AdNetworks: LevelPlay initialized.");

        if (config.testMode) {
            LevelPlay.ValidateIntegration();
        }

        if (config.pauseGameDuringAds) {
            LevelPlay.SetPauseGame(true);
        }

        if (!string.IsNullOrEmpty(ids.levelPlayInterstitialAdUnitId)) {
            interstitial = new LevelPlayInterstitialAd(ids.levelPlayInterstitialAdUnitId);
            interstitial.OnAdLoaded += info => interstitialLoading = false;
            interstitial.OnAdLoadFailed += error => {
                interstitialLoading = false;
                interstitialLoadFailedTime = Time.realtimeSinceStartup;
                LogUtil.LogAds("AdNetworks: LevelPlay interstitial load failed: " + error);
            };
            interstitial.OnAdClosed += info => interstitialLoad();
            interstitial.OnAdDisplayFailed += (info, error) => interstitialLoad();
            interstitialLoad();
        }

        if (!string.IsNullOrEmpty(ids.levelPlayRewardedAdUnitId)) {
            rewarded = new LevelPlayRewardedAd(ids.levelPlayRewardedAdUnitId);
            rewarded.OnAdLoaded += info => rewardedLoading = false;
            rewarded.OnAdLoadFailed += error => {
                rewardedLoading = false;
                rewardedLoadFailedTime = Time.realtimeSinceStartup;
                LogUtil.LogAds("AdNetworks: LevelPlay rewarded load failed: " + error);
            };
            // Reward on the reward callback itself - some networks fire it after close.
            rewarded.OnAdRewarded += (info, reward) => {
                if (onRewarded != null) {
                    onRewarded(1.0);
                }
            };
            rewarded.OnAdClosed += info => rewardedLoad();
            rewarded.OnAdDisplayFailed += (info, error) => rewardedLoad();
            rewardedLoad();
        }
    }

    void onInitFailed(LevelPlayInitError error) {
        Debug.LogError("AdNetworks: LevelPlay init failed: " + error);
    }

    void interstitialLoad() {
        if (interstitial != null && !interstitialLoading) {
            interstitialLoading = true;
            interstitial.LoadAd();
        }
    }

    void rewardedLoad() {
        if (rewarded != null && !rewardedLoading) {
            rewardedLoading = true;
            rewarded.LoadAd();
        }
    }

    // Retry a failed load lazily, when someone next asks, instead of on a timer.
    static bool retryDue(float failedTime) {
        return Time.realtimeSinceStartup - failedTime >= reloadDelaySeconds;
    }

    public bool IsReady(AdFormat format) {

        if (!isInitialized) {
            return false;
        }

        switch (format) {

            case AdFormat.Banner:
                return !string.IsNullOrEmpty(ids.levelPlayBannerAdUnitId);

            case AdFormat.Interstitial:
                if (interstitial == null) {
                    return false;
                }
                if (interstitial.IsAdReady()) {
                    return true;
                }
                if (retryDue(interstitialLoadFailedTime)) {
                    interstitialLoad();
                }
                return false;

            default:
                if (rewarded == null) {
                    return false;
                }
                if (rewarded.IsAdReady()) {
                    return true;
                }
                if (retryDue(rewardedLoadFailedTime)) {
                    rewardedLoad();
                }
                return false;
        }
    }

    public bool ShowBanner(AdPosition position) {

        if (!IsReady(AdFormat.Banner)) {
            return false;
        }

        // Position is fixed at creation, so a new position means a new banner.
        if (banner != null && bannerPosition != position) {
            banner.DestroyAd();
            banner = null;
        }

        if (banner == null) {
            bannerPosition = position;
            banner = new LevelPlayBannerAd(ids.levelPlayBannerAdUnitId,
                new LevelPlayBannerAd.Config.Builder()
                    .SetSize(LevelPlayAdSize.CreateAdaptiveAdSize())
                    .SetPosition(bannerPositionGet(position))
                    .SetDisplayOnLoad(true)
                    .SetRespectSafeArea(true)
                    .Build());
            banner.LoadAd();
        }
        else {
            banner.ShowAd();
        }

        return true;
    }

    public void HideBanner() {
        if (banner != null) {
            banner.HideAd();
        }
    }

    public bool ShowInterstitial(string placement) {

        if (interstitial == null || !interstitial.IsAdReady()) {
            return false;
        }

        interstitial.ShowAd(placement);
        return true;
    }

    public bool ShowRewarded(string placement) {

        if (rewarded == null || !rewarded.IsAdReady()) {
            return false;
        }

        rewarded.ShowAd(placement);
        return true;
    }

    static LevelPlayBannerPosition bannerPositionGet(AdPosition position) {

        switch (position) {
            case AdPosition.TopLeft:
                return LevelPlayBannerPosition.TopLeft;
            case AdPosition.TopCenter:
                return LevelPlayBannerPosition.TopCenter;
            case AdPosition.TopRight:
                return LevelPlayBannerPosition.TopRight;
            case AdPosition.Centered:
                return LevelPlayBannerPosition.Center;
            case AdPosition.BottomLeft:
                return LevelPlayBannerPosition.BottomLeft;
            case AdPosition.BottomRight:
                return LevelPlayBannerPosition.BottomRight;
            default:
                return LevelPlayBannerPosition.BottomCenter;
        }
    }
}
#endif
