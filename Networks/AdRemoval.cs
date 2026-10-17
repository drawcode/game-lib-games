using System;

using UnityEngine;

using Engine.Events;
using Engine.Utility;

// ----------------------------------------------------------------------
// AD REMOVAL
//
// One entitlement ("ads removed"), reachable by any combination of three paths.
// Each app / syndicated variant picks its paths in AdRemovalConfig, so the same game
// can ship under different IP with different payment models (A/B by variant).
//
//   Coins     - spend virtual coins (bought as consumables or earned via rewarded
//               ads / play). Typical consumable-coin apps like Action Bots.
//   Freeward  - earn the unlock over time: points from daily visits, rewarded ads
//               and game actions accumulate, never expire, and unlock at a threshold.
//               Nothing is spent from the coin wallet.
//   Direct    - non-consumable store IAP. The store restores it; the others are
//               ours to remember (see idea-free-usage-gated-unlock: store trap).
//
// When removed: banners and interstitials stop on every AdNetworks path (legacy
// included); rewarded ads stay unless AdNetworksConfig.keepRewardedWhenAdsRemoved
// is false, since players opt into those for coins.
//
// The Unity side of Lib.Freeward.Core is not built yet; when it lands, the Freeward
// path here becomes an adapter onto it and the persisted state below migrates.

[Flags]
public enum AdRemovalPaths {
    None = 0,
    Coins = 1 << 0,
    Freeward = 1 << 1,
    Direct = 1 << 2
}

public class AdRemovalMessages {
    // string: the path that granted it ("coins", "freeward", "direct", "restore")
    public static string adsRemoved = "ad-removal-ads-removed";
    // double: freeward progress 0..1
    public static string freewardProgress = "ad-removal-freeward-progress";
}

// Coin balance owner - the app's profile / currency system.
public interface IAdRemovalWallet {
    double GetCoins();
    bool TrySpendCoins(double amount);
}

// Where the entitlement and freeward progress persist. Default is PlayerPrefs; apps with
// a game profile should back it with the profile (it then rides profile save / cloud sync).
// The profile save filters escaped quotes out of nested JSON, so profile-backed stores must
// encode the value (see AdRemovalProfileEncoding).
public interface IAdRemovalStore {
    string Load(string key);
    void Save(string key, string json);
}

// Base64 wrapper so the state JSON survives being nested inside another JSON document.
public static class AdRemovalProfileEncoding {

    public static string Encode(string json) {
        return string.IsNullOrEmpty(json)
            ? ""
            : Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
    }

    public static string Decode(string value) {

        if (string.IsNullOrEmpty(value)) {
            return "";
        }

        try {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
        catch (FormatException) {
            return "";
        }
    }
}

public class AdRemovalPlayerPrefsStore : IAdRemovalStore {

    public string Load(string key) {
        return PlayerPrefs.GetString(key, "");
    }

    public void Save(string key, string json) {
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
}

[Serializable]
public class AdRemovalConfig {

    // A/B or syndication tag, recorded with the unlock for analytics.
    public string variant = "";

    public AdRemovalPaths paths = AdRemovalPaths.None;

    // Coins
    public double coinCost = 0;

    // Freeward
    public double freewardPointsRequired = 0;
    public double freewardPointsDailyVisit = 0;
    public double freewardPointsRewardedAd = 0;

    // Direct - store product id of a NON-consumable registered with the store
    // (and in the app's product data so ProductNetworks registers it).
    public string directProductCode = "";

    public string storageKey = "ad-removal-state";

    public static AdRemovalConfig FromJson(string json) {
        return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<AdRemovalConfig>(json);
    }

    // e.g. a per-variant TextAsset at Resources/<path>.json
    public static AdRemovalConfig LoadFromResources(string path) {
        TextAsset asset = Resources.Load<TextAsset>(path);
        return asset == null ? null : FromJson(asset.text);
    }
}

[Serializable]
public class AdRemovalState {
    public bool removed = false;
    public string removedVia = "";
    public string removedVariant = "";
    public double freewardPoints = 0;
    public string freewardLastVisitDay = "";
}

public static class AdRemoval {

    public const string viaCoins = "coins";
    public const string viaFreeward = "freeward";
    public const string viaDirect = "direct";

    static AdRemovalConfig _config = new AdRemovalConfig();
    static AdRemovalState _state = new AdRemovalState();
    static IAdRemovalWallet _wallet;
    static IAdRemovalStore _store;
    static bool _listening = false;

    public static AdRemovalConfig config {
        get {
            return _config;
        }
    }

    public static bool adsRemoved {
        get {
            return _state.removed;
        }
    }

    public static string removedVia {
        get {
            return _state.removedVia;
        }
    }

    public static bool IsPathEnabled(AdRemovalPaths path) {
        return (_config.paths & path) == path && path != AdRemovalPaths.None;
    }

    // Call once at boot. wallet is required for Coins; store defaults to PlayerPrefs.
    public static void Configure(
        AdRemovalConfig config,
        IAdRemovalWallet wallet = null,
        IAdRemovalStore store = null) {

        _config = config ?? new AdRemovalConfig();
        _wallet = wallet;
        _store = store ?? new AdRemovalPlayerPrefsStore();

        stateLoad();

        if (IsPathEnabled(AdRemovalPaths.Coins) && _wallet == null) {
            Debug.LogError("AdRemoval: Coins path enabled without a wallet - coin unlock unavailable.");
        }

        if (IsPathEnabled(AdRemovalPaths.Direct) && string.IsNullOrEmpty(_config.directProductCode)) {
            Debug.LogError("AdRemoval: Direct path enabled without directProductCode.");
        }

        if (!_listening) {
            _listening = true;
            Messenger<double>.AddListener(AdNetworksMessages.videoAd, onVideoAdWatched);
            Messenger<ProductNetworkRecord>.AddListener(
                ProductNetworkMessages.productPurchaseSuccess, onProductPurchaseSuccess);
        }

        freewardDailyVisit();
    }

    // Re-read persisted state, e.g. after the active profile changes.
    public static void Reload() {

        if (_store == null) {
            return;
        }

        bool wasRemoved = _state.removed;

        stateLoad();
        freewardDailyVisit();

        if (_state.removed && !wasRemoved) {
            AdNetworks.HideBannerAd();
        }
    }

    // ------------------------------------------------------------------
    // COINS

    public static bool CanPurchaseWithCoins() {
        return !adsRemoved
            && IsPathEnabled(AdRemovalPaths.Coins)
            && _wallet != null
            && _config.coinCost > 0
            && _wallet.GetCoins() >= _config.coinCost;
    }

    public static bool PurchaseWithCoins() {

        if (!CanPurchaseWithCoins()) {
            return false;
        }

        if (!_wallet.TrySpendCoins(_config.coinCost)) {
            return false;
        }

        grant(viaCoins);
        return true;
    }

    // ------------------------------------------------------------------
    // FREEWARD

    public static double freewardPoints {
        get {
            return _state.freewardPoints;
        }
    }

    public static double freewardProgress {
        get {
            if (_config.freewardPointsRequired <= 0) {
                return 0;
            }
            return Math.Min(1.0, _state.freewardPoints / _config.freewardPointsRequired);
        }
    }

    // Game actions call this (round finished, level cleared, coins earned...).
    public static void AddFreewardPoints(double points, string source = null) {

        if (adsRemoved || points <= 0 || !IsPathEnabled(AdRemovalPaths.Freeward)) {
            return;
        }

        _state.freewardPoints += points;

        LogUtil.LogAds("AdRemoval: freeward +" + points +
            (string.IsNullOrEmpty(source) ? "" : " (" + source + ")") +
            " = " + _state.freewardPoints + "/" + _config.freewardPointsRequired);

        if (_config.freewardPointsRequired > 0
            && _state.freewardPoints >= _config.freewardPointsRequired) {
            grant(viaFreeward);
            return;
        }

        stateSave();
        Messenger<double>.Broadcast(AdRemovalMessages.freewardProgress, freewardProgress);
    }

    static void freewardDailyVisit() {

        if (_config.freewardPointsDailyVisit <= 0) {
            return;
        }

        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (_state.freewardLastVisitDay == today) {
            return;
        }

        _state.freewardLastVisitDay = today;
        stateSave();

        AddFreewardPoints(_config.freewardPointsDailyVisit, "daily");
    }

    static void onVideoAdWatched(double fraction) {
        if (fraction > .9) {
            AddFreewardPoints(_config.freewardPointsRewardedAd, "rewarded-ad");
        }
    }

    // ------------------------------------------------------------------
    // DIRECT

    public static void PurchaseDirect() {

        if (adsRemoved || !IsPathEnabled(AdRemovalPaths.Direct)
            || string.IsNullOrEmpty(_config.directProductCode)) {
            return;
        }

        ProductNetworks.PurchaseProduct(_config.directProductCode, 1);
    }

    public static void RestoreDirect() {
        if (IsPathEnabled(AdRemovalPaths.Direct)) {
            ProductNetworks.RestoreTransactions();
        }
    }

    // For apps whose store flow validates receipts elsewhere. A completed purchase or restore
    // of the product ALWAYS grants, even when the Direct path isn't offered in this variant:
    // the SKU can still be bought from a store list, and restores cross variants - anyone
    // who paid gets the unlock.
    public static void GrantDirect(string productCode) {
        if (!string.IsNullOrEmpty(productCode) && productCode == _config.directProductCode) {
            grant(viaDirect);
        }
    }

    static void onProductPurchaseSuccess(ProductNetworkRecord record) {

        if (record == null || string.IsNullOrEmpty(_config.directProductCode)) {
            return;
        }

        if (record.code == _config.directProductCode || record.productId == _config.directProductCode) {
            GrantDirect(_config.directProductCode);
        }
    }

    // ------------------------------------------------------------------

    static void grant(string via) {

        if (_state.removed) {
            return;
        }

        _state.removed = true;
        _state.removedVia = via;
        _state.removedVariant = _config.variant;
        stateSave();

        LogUtil.LogAds("AdRemoval: ads removed via " + via +
            (string.IsNullOrEmpty(_config.variant) ? "" : " [" + _config.variant + "]"));

        AdNetworks.HideBannerAd();

        Messenger<string>.Broadcast(AdRemovalMessages.adsRemoved, via);
    }

    static void stateLoad() {

        _state = new AdRemovalState();

        string json = _store.Load(_config.storageKey);

        if (string.IsNullOrEmpty(json)) {
            return;
        }

        try {
            _state = JsonUtility.FromJson<AdRemovalState>(json) ?? new AdRemovalState();
        }
        catch (Exception e) {
            Debug.LogError("AdRemoval: state unreadable, keeping defaults: " + e.Message);
        }
    }

    static void stateSave() {
        if (_store != null) {
            _store.Save(_config.storageKey, JsonUtility.ToJson(_state));
        }
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public static void ResetForTesting() {
        _state = new AdRemovalState();
        stateSave();
    }
#endif
}
