using System;
using System.Collections.Generic;

using UnityEngine;

using Engine.Game.App;
using Engine.Game.App.BaseApp;

// Content gating by bot level (Action Bots iter 35, owner 2026-10-28): roster items, levels and
// worlds carry an optional `min_level` (GameDataObject.min_level, absent = 0 = always). The level
// is the CURRENT bot's, derived from that bot's XP through the level rows of
// `game-content-level-data` (GameContentLevels), so switching bots changes what is unlocked.
//
// Locked content is hidden, except the next `teaser_count` locked items (lowest min_level first),
// which screens show greyed as "more to come". Spawning and the READY VS card never use a teaser.
//
// Two FeatureGates keys make the policy a data edit:
//   content-lock          off = nothing is gated (tuning, probes)
//   content-lock-teasers  off = locked content is fully hidden, no teasers
//
// Test content (owner 2026-10-31): rows tagged "test" (Action Bots' x-90..93 test missions) are
// for dev only. They show while the `dev-test-content` gate is on, which defaults to
// Debug.isDebugBuild - on in the Editor and development builds, off in release builds - so the
// data file only names the key to force it either way. Hidden test content is never unlocked
// either, so a stale play payload can't start it.
//
// A round keeps what it started with: BeginRound() snapshots the level (BaseGameController
// onGameStarted), and IsSpawnUnlocked reads the snapshot, so XP picked up mid-round never adds
// enemies mid-round. Screens read the live level.
//
// Game-agnostic: a game with no level rows has every bot at level 0, and only items with no
// min_level pass - which, with no min_level in data, is everything.
public static class GameContentGate {

    public const string gateKey = "content-lock";
    public const string teasersGateKey = "content-lock-teasers";
    public const string testContentGateKey = "dev-test-content";

    public const string testTag = "test";

    public const int defaultTeaserCount = 2;

    // XP thresholds ascending, with the level each one reaches (parallel lists).
    static List<double> xpThresholds = new List<double>();
    static List<int> xpThresholdLevels = new List<int>();
    static int teaserCountData = defaultTeaserCount;
    static bool loaded;

    // >= 0 forces the bot level (tests and probes); never set by the game.
    public static double levelOverride = -1;

    static double roundLevel = -1;

    public static List<double> xpLevels {
        get {
            EnsureLoaded();
            return xpThresholds;
        }
    }

    public static int teaserCount {
        get {
            EnsureLoaded();
            return FeatureGates.IsOn(teasersGateKey, true) ? Mathf.Max(0, teaserCountData) : 0;
        }
    }

    public static bool enabled {
        get {
            return FeatureGates.IsOn(gateKey, true);
        }
    }

    public static bool showTestContent {
        get {
            return FeatureGates.IsOn(testContentGateKey, Debug.isDebugBuild);
        }
    }

    // A "test" row while test content is off: left out of every list and never unlocked.
    public static bool IsHiddenTest(GameDataObject item) {
        return item != null && !showTestContent && item.HasTag(testTag);
    }

    static void EnsureLoaded() {
        if (!loaded) {
            Reload();
        }
    }

    // Re-read GameContentLevels (after a data edit, or after a test loaded its own rows).
    public static void Reload() {
        Load(GameContentLevels.Instance.GetAll());
    }

    // Active `level` rows give the thresholds (a row without `level` is the next one up, in xp
    // order); the `settings` row gives teaser_count. Anything unreadable is skipped.
    public static void Load(List<GameContentLevel> rows) {

        loaded = true;
        teaserCountData = defaultTeaserCount;

        List<GameContentLevel> levels = new List<GameContentLevel>();

        if (rows != null) {
            foreach (GameContentLevel row in rows) {

                if (row == null || !row.active) {
                    continue;
                }

                if (row.type == GameContentLevel.typeSettings) {
                    if (row.teaser_count >= 0) {
                        teaserCountData = row.teaser_count;
                    }
                }
                else if (row.type == GameContentLevel.typeLevel && row.xp > 0) {
                    levels.Add(row);
                }
            }
        }

        levels.Sort((a, b) => a.xp.CompareTo(b.xp));

        xpThresholds = new List<double>();
        xpThresholdLevels = new List<int>();

        int last = 0;

        foreach (GameContentLevel row in levels) {
            last = row.level > 0 ? row.level : last + 1;
            xpThresholds.Add(row.xp);
            xpThresholdLevels.Add(last);
        }
    }

    // LEVEL

    public static int LevelForXP(double xp) {

        EnsureLoaded();

        int level = 0;

        for (int i = 0; i < xpThresholds.Count; i++) {
            if (xp >= xpThresholds[i]) {
                level = Mathf.Max(level, xpThresholdLevels[i]);
            }
        }

        return level;
    }

    // The XP the next level needs, or -1 at the top of the table.
    public static double NextLevelXP(double xp) {

        EnsureLoaded();

        for (int i = 0; i < xpThresholds.Count; i++) {
            if (xp < xpThresholds[i]) {
                return xpThresholds[i];
            }
        }

        return -1;
    }

    // The current bot's XP (0 with no profile).
    public static double CurrentXP() {

        GameProfilePlayerProgressItem progress = GameProfileCharacters.currentProgress;

        return progress == null ? 0 : progress.GetGamePlayerProgressXP(0);
    }

    // The current bot's level, live.
    public static double CurrentLevel() {

        if (levelOverride >= 0) {
            return levelOverride;
        }

        return LevelForXP(CurrentXP());
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
        return item == null || (!IsHiddenTest(item) && IsUnlocked(item.min_level, CurrentLevel()));
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

            if (item == null || IsHiddenTest(item)) {
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

            if (item == null || IsHiddenTest(item)) {
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
