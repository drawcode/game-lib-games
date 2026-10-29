using System;
using System.Collections.Generic;

using UnityEngine;

using Engine.Game.App;
using Engine.Game.App.BaseApp;

// Content gating by bot level (Action Bots iter 35, owner 2026-10-28): roster items, levels and
// worlds carry an optional `min_level` (GameDataObject.min_level, absent = 0 = always). The level
// is the CURRENT bot's, derived from that bot's XP through a threshold table in data, so switching
// bots changes what is unlocked.
//
// Locked content is hidden, except the next `teaser_count` locked items (lowest min_level first),
// which screens show greyed as "more to come". Spawning and the READY VS card never use a teaser.
//
// Two FeatureGates keys make the policy a data edit:
//   content-lock          off = nothing is gated (tuning, probes)
//   content-lock-teasers  off = locked content is fully hidden, no teasers
//
// A round keeps what it started with: BeginRound() snapshots the level (BaseGameController
// onGameStarted), and IsSpawnUnlocked reads the snapshot, so XP picked up mid-round never adds
// enemies mid-round. Screens read the live level.
//
// Game-agnostic: until a game calls Load, the XP table is empty, every bot is level 0, and only
// items with no min_level pass - which, with no min_level in data, is everything.
public static class GameContentGate {

    public const string gateKey = "content-lock";
    public const string teasersGateKey = "content-lock-teasers";

    [Serializable]
    public class Data {
        // XP needed to reach level 1, 2, 3 ... ascending. Level = how many entries the XP has reached.
        public List<double> xp_levels = new List<double>();
        public int teaser_count = 2;
    }

    static Data data = new Data();

    // >= 0 forces the bot level (tests and probes); never set by the game.
    public static double levelOverride = -1;

    static double roundLevel = -1;

    public static string loadedFrom { get; private set; }

    public static List<double> xpLevels {
        get {
            return data.xp_levels;
        }
    }

    public static int teaserCount {
        get {
            return FeatureGates.IsOn(teasersGateKey, true) ? Mathf.Max(0, data.teaser_count) : 0;
        }
    }

    public static bool enabled {
        get {
            return FeatureGates.IsOn(gateKey, true);
        }
    }

    public static void Load(string resourcePath) {

        loadedFrom = resourcePath;

        TextAsset asset = Resources.Load<TextAsset>(resourcePath);

        if (asset == null) {
            Debug.LogWarning("GameContentGate: no data at Resources/" + resourcePath + " - every bot is level 0");
            data = new Data();
            return;
        }

        LoadJson(asset.text);
    }

    public static void LoadJson(string json) {

        Data loaded = null;

        try {
            loaded = string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<Data>(json);
        }
        catch (Exception e) {
            Debug.LogWarning("GameContentGate: unreadable data - every bot is level 0: " + e.Message);
        }

        data = loaded ?? new Data();

        if (data.xp_levels == null) {
            data.xp_levels = new List<double>();
        }

        data.xp_levels.Sort();
    }

    // LEVEL

    public static int LevelForXP(double xp) {

        int level = 0;

        for (int i = 0; i < data.xp_levels.Count; i++) {
            if (xp >= data.xp_levels[i]) {
                level = i + 1;
            }
        }

        return level;
    }

    // The current bot's level, live.
    public static double CurrentLevel() {

        if (levelOverride >= 0) {
            return levelOverride;
        }

        GameProfilePlayerProgressItem progress = GameProfileCharacters.currentProgress;

        if (progress == null) {
            return 0;
        }

        return LevelForXP(progress.GetGamePlayerProgressXP(0));
    }

    public static void BeginRound() {
        roundLevel = CurrentLevel();
    }

    public static void ResetRound() {
        roundLevel = -1;
    }

    // The level the round in progress started with (live until the first round).
    public static double RoundLevel() {
        return levelOverride >= 0 || roundLevel < 0 ? CurrentLevel() : roundLevel;
    }

    // CHECKS

    public static bool IsUnlocked(double minLevel, double level) {
        return !enabled || minLevel <= 0 || level >= minLevel;
    }

    public static bool IsUnlocked(GameDataObject item) {
        return item == null || IsUnlocked(item.min_level, CurrentLevel());
    }

    // For spawns and the VS card: the round's level, not the live one.
    public static bool IsSpawnUnlocked(GameDataObject item) {
        return item == null || IsUnlocked(item.min_level, RoundLevel());
    }

    // A mission plays a random level of its world (`<world_num>-<suffix>`): pick among the
    // suffixes whose level row is unlocked. A suffix with no level row is unlocked (generated
    // levels). If every one is locked, fall back to any, so a mission never fails to start.
    public static int PickLevelSuffix(int worldNum, List<int> suffixes) {

        if (suffixes == null || suffixes.Count == 0) {
            return 0;
        }

        List<int> open = new List<int>();

        foreach (int suffix in suffixes) {
            GameLevel level = GameLevels.Instance.GetByCode(worldNum + "-" + suffix);
            if (IsUnlocked(level)) {
                open.Add(suffix);
            }
        }

        List<int> from = open.Count > 0 ? open : suffixes;
        return from[UnityEngine.Random.Range(0, from.Count)];
    }

    // For screens: the unlocked items in source order, plus the next teaserCount locked ones
    // (lowest min_level first, source order on ties), which also go in `teasers` when given.
    // Allocates its result; not for a hot path.
    public static List<T> Visible<T>(List<T> items, List<T> teasers = null) where T : GameDataObject {
        return Visible(items, CurrentLevel(), teaserCount, teasers);
    }

    public static List<T> Visible<T>(List<T> items, double level, int teasersMax, List<T> teasers)
        where T : GameDataObject {

        List<T> visible = new List<T>();

        if (teasers != null) {
            teasers.Clear();
        }

        if (items == null) {
            return visible;
        }

        List<T> locked = null;

        foreach (T item in items) {

            if (item == null) {
                continue;
            }

            if (IsUnlocked(item.min_level, level)) {
                continue;
            }

            if (locked == null) {
                locked = new List<T>();
            }

            // insertion by min_level, stable
            int at = locked.Count;

            while (at > 0 && locked[at - 1].min_level > item.min_level) {
                at--;
            }

            locked.Insert(at, item);
        }

        int teaserTotal = locked == null ? 0 : Mathf.Min(teasersMax, locked.Count);

        foreach (T item in items) {

            if (item == null) {
                continue;
            }

            bool teaser = false;

            if (!IsUnlocked(item.min_level, level)) {

                int rank = locked.IndexOf(item);

                if (rank < 0 || rank >= teaserTotal) {
                    continue;
                }

                teaser = true;
            }

            visible.Add(item);

            if (teaser && teasers != null) {
                teasers.Add(item);
            }
        }

        return visible;
    }
}
