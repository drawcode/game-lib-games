#if AD_USE_ADMOB_GMA
using System;

using UnityEngine;

using GoogleMobileAds.Api;
using GmaAdPosition = GoogleMobileAds.Api.AdPosition;

using Engine.Utility;

// Direct AdMob provider (Google Mobile Ads Unity plugin 9.x+), for apps that run AdMob
// WITHOUT LevelPlay mediation. When AdMob is mediated through LevelPlay, leave this out
// of providerOrder - running both would initialise the Google SDK twice.
//
// The App ID the SDK actually starts with comes from Assets > Google Mobile Ads > Settings
// (written to AndroidManifest / Info.plist); AdNetworksBuildValidator checks it at build time.
// The copy in AdNetworksPlatformIds.admobAppId is validated here so a bad config disables
// the provider instead of reaching the SDK.
public class AdNetworkProviderAdMob : IAdNetworkProvider {

    const float reloadDelaySeconds = 30f;

    AdNetworksPlatformIds ids;
    Action<double> onRewarded;

    BannerView banner;
    global::AdPosition bannerPosition;

    InterstitialAd interstitial;
    bool interstitialLoading = false;
    float interstitialLoadFailedTime = float.MinValue;

    RewardedAd rewarded;
    bool rewardedLoading = false;
    float rewardedLoadFailedTime = float.MinValue;

    public AdNetworkType networkType {
        get {
            return AdNetworkType.AdmobDirect;
        }
    }

    public bool isInitialized { get; private set; }

    public void Init(AdNetworksConfig config, Action<double> onRewarded) {

        this.onRewarded = onRewarded;
        ids = config.current;

        if (!AdNetworksConfig.IsValidAdmobAppId(ids.admobAppId)) {
            Debug.LogError("AdNetworks: AdMob App ID '" + ids.admobAppId + "' is not valid " +
                (AdNetworksConfig.IsAdmobPublisherId(ids.admobAppId)
                    ? "(that is a publisher id, not an App ID) "
                    : "") +
                "- expected ca-app-pub-XXXXXXXXXXXXXXXX~NNNNNNNNNN. AdMob provider disabled.");
            return;
        }

        // Ad events otherwise arrive on a background thread.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        RequestConfiguration requestConfiguration = new RequestConfiguration();

        if (config.childDirected) {
            requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
        }

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(status => {
            isInitialized = true;
            LogUtil.LogAds("AdNetworks: AdMob initialized.");
            interstitialLoad();
            rewardedLoad();
        });
    }

    void interstitialLoad() {

        if (interstitialLoading || string.IsNullOrEmpty(ids.admobInterstitialAdUnitId)) {
            return;
        }

        interstitialLoading = true;

        InterstitialAd.Load(ids.admobInterstitialAdUnitId, new AdRequest(), (ad, error) => {

            interstitialLoading = false;

            if (error != null || ad == null) {
                interstitialLoadFailedTime = Time.realtimeSinceStartup;
                LogUtil.LogAds("AdNetworks: AdMob interstitial load failed: " + error);
                return;
            }

            interstitial = ad;
            ad.OnAdFullScreenContentClosed += () => {
                ad.Destroy();
                interstitial = null;
                interstitialLoad();
            };
            ad.OnAdFullScreenContentFailed += adError => {
                ad.Destroy();
                interstitial = null;
                interstitialLoad();
            };
        });
    }

    void rewardedLoad() {

        if (rewardedLoading || string.IsNullOrEmpty(ids.admobRewardedAdUnitId)) {
            return;
        }

        rewardedLoading = true;

        RewardedAd.Load(ids.admobRewardedAdUnitId, new AdRequest(), (ad, error) => {

            rewardedLoading = false;

            if (error != null || ad == null) {
                rewardedLoadFailedTime = Time.realtimeSinceStartup;
                LogUtil.LogAds("AdNetworks: AdMob rewarded load failed: " + error);
                return;
            }

            rewarded = ad;
            ad.OnAdFullScreenContentClosed += () => {
                ad.Destroy();
                rewarded = null;
                rewardedLoad();
            };
            ad.OnAdFullScreenContentFailed += adError => {
                ad.Destroy();
                rewarded = null;
                rewardedLoad();
            };
        });
    }

    static bool retryDue(float failedTime) {
        return Time.realtimeSinceStartup - failedTime >= reloadDelaySeconds;
    }

    public bool IsReady(AdFormat format) {

        if (!isInitialized) {
            return false;
        }

        switch (format) {

            case AdFormat.Banner:
                return !string.IsNullOrEmpty(ids.admobBannerAdUnitId);

            case AdFormat.Interstitial:
                if (interstitial != null && interstitial.CanShowAd()) {
                    return true;
                }
                if (interstitial == null && retryDue(interstitialLoadFailedTime)) {
                    interstitialLoad();
                }
                return false;

            default:
                if (rewarded != null && rewarded.CanShowAd()) {
                    return true;
                }
                if (rewarded == null && retryDue(rewardedLoadFailedTime)) {
                    rewardedLoad();
                }
                return false;
        }
    }

    public bool ShowBanner(global::AdPosition position) {

        if (!IsReady(AdFormat.Banner)) {
            return false;
        }

        if (banner != null && bannerPosition != position) {
            banner.Destroy();
            banner = null;
        }

        if (banner == null) {
            bannerPosition = position;
            banner = new BannerView(ids.admobBannerAdUnitId,
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth),
                bannerPositionGet(position));
            banner.LoadAd(new AdRequest());
        }
        else {
            banner.Show();
        }

        return true;
    }

    public void HideBanner() {
        if (banner != null) {
            banner.Hide();
        }
    }

    public bool ShowInterstitial(string placement) {

        if (interstitial == null || !interstitial.CanShowAd()) {
            return false;
        }

        interstitial.Show();
        return true;
    }

    public bool ShowRewarded(string placement) {

        if (rewarded == null || !rewarded.CanShowAd()) {
            return false;
        }

        rewarded.Show(reward => {
            if (onRewarded != null) {
                onRewarded(1.0);
            }
        });

        return true;
    }

    static GmaAdPosition bannerPositionGet(global::AdPosition position) {

        switch (position) {
            case global::AdPosition.TopLeft:
                return GmaAdPosition.TopLeft;
            case global::AdPosition.TopCenter:
                return GmaAdPosition.Top;
            case global::AdPosition.TopRight:
                return GmaAdPosition.TopRight;
            case global::AdPosition.Centered:
                return GmaAdPosition.Center;
            case global::AdPosition.BottomLeft:
                return GmaAdPosition.BottomLeft;
            case global::AdPosition.BottomRight:
                return GmaAdPosition.BottomRight;
            default:
                return GmaAdPosition.Bottom;
        }
    }
}
#endif
