using System;
using System.Collections;
using System.Collections.Generic;

// Arcade levels (contexts games/shared/context-arcade-vs-missions.md, owner calls A1 + A2).
//
// An arcade round never plays a mission level: it plays a level row GENERATED from the play
// type's missions in the current world.
//
//   entry - the easiest level the type's first mission plays (lowest min_level, then sort_order).
//           Its layout, terrain and characters are what the type needs (attack/build/repair
//           need their action zones), so the arcade row starts as a copy of it.
//   cap   - the hardest level any of the type's missions plays.
//
// Tier 0 is fun first: half the entry mission's enemies and extra weapon and item drops. Each tier
// climbs toward the cap and never past it, so an arcade type opens before its missions and stays
// easier than them. A type whose missions spawn no enemies gets none in arcade either.
//
// Same inputs, same row. The seed only picks prop density inside the entry level's authored
// range, through a hash, never UnityEngine.Random. Rows are plain dictionaries in the
// game-level-data shape, so this file has no engine dependency; GameArcadeLevels.Runtime.cs turns
// a row into the current GameLevel.
public class GameArcadeLevelKeys {

    // level_kind on a level row's data block: absent reads as mission
    public static string level_kind = "level_kind";
    public static string mission = "mission";
    public static string arcade = "arcade";
    public static string test = "test";

    public static string play_type = "play_type";
    public static string arcade_tier = "arcade_tier";
    public static string arcade_seed = "arcade_seed";

    public static string battle = "battle";
    public static string rescue = "rescue";
    public static string attack = "attack";
    public static string build = "build";
    public static string repair = "repair";
}

public static partial class GameArcadeLevels {

    // Tiers run 0..tierMax; the top tier plays at the cap.
    public static int tierMax = 9;

    // Extra weapon and item drops at tier 0, falling to the entry level's own counts at tierMax.
    public static int pickupBonus = 2;

    // A mission's type is its first action (the mission rows carry no type key).
    public static Dictionary<string, string> playTypeActions = new Dictionary<string, string> {
        { GameArcadeLevelKeys.battle, "action-kill" },
        { GameArcadeLevelKeys.rescue, "action-save" },
        { GameArcadeLevelKeys.attack, "action-attack" },
        { GameArcadeLevelKeys.build, "action-build" },
        { GameArcadeLevelKeys.repair, "action-repair" },
    };

    static readonly string[] copiedRowKeys = { "world_code", "display_name", "name", "description" };

    // Play types in code order: each gets a block of tierMax + 1 level numbers (battle 1-10,
    // rescue 11-20, ...), so a generated code is "arcade-<world>-<n>" like every other level kind
    // (GameLevelCodes) and still names one type and tier.
    public static string[] playTypes = {
        GameArcadeLevelKeys.battle, GameArcadeLevelKeys.rescue, GameArcadeLevelKeys.attack,
        GameArcadeLevelKeys.build, GameArcadeLevelKeys.repair,
    };

    public static string Code(string playType, int worldNum, int tier) {
        int index = Array.IndexOf(playTypes, playType);
        if (index < 0) {
            return GameArcadeLevelKeys.arcade + "-" + playType + "-" + worldNum + "-" + ClampTier(tier);
        }
        return GameArcadeLevelKeys.arcade + "-" + worldNum + "-" + (index * (tierMax + 1) + ClampTier(tier) + 1);
    }

    public static int ClampTier(int tier) {
        return Math.Max(0, Math.Min(tierMax, tier));
    }

    // The generated row, or null when the type has no missions or none of their levels exist in
    // the world. Never changes the rows it reads.
    public static Dictionary<string, object> Generate(
        string playType, int worldNum, int tier, int seed,
        IEnumerable missionRows, IEnumerable levelRows) {

        IDictionary<string, object> entry;
        IDictionary<string, object> cap;

        if (!FindEntryAndCap(playType, worldNum, missionRows, levelRows, out entry, out cap)) {
            return null;
        }

        tier = ClampTier(tier);
        double t = tierMax > 0 ? (double)tier / tierMax : 1;

        Dictionary<string, object> row = new Dictionary<string, object>();
        row["type"] = "level";
        row["code"] = Code(playType, worldNum, tier);
        row["active"] = true;
        row["sort_order"] = 0;

        foreach (string key in copiedRowKeys) {
            object value;
            if (entry.TryGetValue(key, out value) && value != null) {
                row[key] = Clone(value);
            }
        }

        IDictionary<string, object> entryData = Dict(Get(entry, "data"));
        Dictionary<string, object> data = entryData != null
            ? (Dictionary<string, object>)Clone(entryData)
            : new Dictionary<string, object>();

        data["directors"] = Directors(entryData, Dict(Get(cap, "data")), t);
        data["asset_presets"] = AssetPresets(Get(entryData, "asset_presets"), seed);

        data[GameArcadeLevelKeys.level_kind] = GameArcadeLevelKeys.arcade;
        data[GameLevelItemSourceKeys.level_items_source] = GameLevelItemSourceKeys.generated;
        data[GameArcadeLevelKeys.play_type] = playType;
        data[GameArcadeLevelKeys.arcade_tier] = tier;
        data[GameArcadeLevelKeys.arcade_seed] = seed;

        row["data"] = data;

        return row;
    }

    // The type's action goal for a tier: 1 at tier 0, rising to the largest goal any of the
    // type's missions sets. The other actions (coins, win) come from the entry mission as they are.
    public static List<object> Goals(string playType, int tier, IEnumerable missionRows) {

        List<IDictionary<string, object>> missions = MissionsOf(missionRows, playType);

        if (missions.Count == 0) {
            return null;
        }

        string actionCode = ActionFor(playType);
        double capVal = 1;

        foreach (IDictionary<string, object> mission in missions) {
            foreach (object action in List(Get(Dict(Get(mission, "data")), "data"))) {
                if (Str(Get(Dict(action), "code")) == actionCode) {
                    capVal = Math.Max(capVal, Num(Get(Dict(Get(Dict(action), "data")), "val"), 1));
                }
            }
        }

        double t = tierMax > 0 ? (double)ClampTier(tier) / tierMax : 1;
        List<object> goals = new List<object>();

        foreach (object action in List(Get(Dict(Get(missions[0], "data")), "data"))) {

            Dictionary<string, object> goal = Clone(action) as Dictionary<string, object>;

            if (goal == null) {
                continue;
            }

            Dictionary<string, object> goalData = Dict(Get(goal, "data")) as Dictionary<string, object>;

            if (Str(Get(goal, "code")) == actionCode && goalData != null) {
                goalData["val"] = Round(Lerp(1, capVal, t));
            }

            goals.Add(goal);
        }

        return goals;
    }

    public static string ActionFor(string playType) {
        string action;
        return playType != null && playTypeActions.TryGetValue(playType, out action) ? action : null;
    }

    // The type's missions, first mission first: rows typed mission, not tagged test, whose first
    // action is the type's, ordered by min_level (absent = 0) then sort_order.
    public static List<IDictionary<string, object>> MissionsOf(IEnumerable missionRows, string playType) {

        List<IDictionary<string, object>> missions = new List<IDictionary<string, object>>();
        string actionCode = ActionFor(playType);

        if (missionRows == null || actionCode == null) {
            return missions;
        }

        foreach (object raw in missionRows) {

            IDictionary<string, object> row = Dict(raw);

            if (row == null || Str(Get(row, "type")) != "mission" || IsTest(row)) {
                continue;
            }

            List<object> actions = List(Get(Dict(Get(row, "data")), "data"));

            if (actions.Count > 0 && Str(Get(Dict(actions[0]), "code")) == actionCode) {
                missions.Add(row);
            }
        }

        // a stable sort by hand: List.Sort is not stable, and source order breaks ties
        List<IDictionary<string, object>> sorted = new List<IDictionary<string, object>>(missions.Count);

        foreach (IDictionary<string, object> mission in missions) {
            int at = sorted.Count;
            while (at > 0 && CompareMissions(sorted[at - 1], mission) > 0) {
                at--;
            }
            sorted.Insert(at, mission);
        }

        return sorted;
    }

    // The level rows a mission can play in a world: "<kind>-<world>-<suffix>" for its suffix list,
    // kind from the mission's data.level_kind (mission when absent), else the older bare
    // "<world>-<suffix>" (GameLevelCodes.Resolve, without the engine).
    public static List<IDictionary<string, object>> MissionLevels(
        IDictionary<string, object> mission, int worldNum, IEnumerable levelRows) {

        List<IDictionary<string, object>> levels = new List<IDictionary<string, object>>();
        IDictionary<string, object> missionData = Dict(Get(mission, "data"));
        string kind = Str(Get(missionData, GameArcadeLevelKeys.level_kind)) ?? GameArcadeLevelKeys.mission;

        foreach (object suffix in List(Get(missionData, "level_num_suffix_list"))) {

            string bare = worldNum + "-" + (int)Num(suffix, 0);
            IDictionary<string, object> level = LevelRow(levelRows, kind + "-" + bare) ?? LevelRow(levelRows, bare);

            if (level != null) {
                levels.Add(level);
            }
        }

        return levels;
    }

    public static IDictionary<string, object> LevelRow(IEnumerable levelRows, string code) {

        if (levelRows == null) {
            return null;
        }

        foreach (object raw in levelRows) {
            IDictionary<string, object> row = Dict(raw);
            if (row != null && Str(Get(row, "code")) == code) {
                return row;
            }
        }

        return null;
    }

    // How hard a level pushes: the enemy spawn limit, then the enemy minimum; 0 with no enemies.
    public static double Pressure(IDictionary<string, object> levelRow) {

        IDictionary<string, object> enemy = Director(Dict(Get(levelRow, "data")), "enemy");

        if (enemy == null || !Bool(Get(enemy, "run"), true)) {
            return 0;
        }

        return Num(Get(enemy, "max"), 0) * 1000 + Num(Get(enemy, "min"), 0);
    }

    public static bool FindEntryAndCap(
        string playType, int worldNum, IEnumerable missionRows, IEnumerable levelRows,
        out IDictionary<string, object> entry, out IDictionary<string, object> cap) {

        entry = null;
        cap = null;

        bool entryFound = false;

        foreach (IDictionary<string, object> mission in MissionsOf(missionRows, playType)) {

            List<IDictionary<string, object>> levels = MissionLevels(mission, worldNum, levelRows);

            foreach (IDictionary<string, object> level in levels) {

                if (cap == null || Pressure(level) > Pressure(cap)) {
                    cap = level;
                }

                // entry: the easiest level of the first mission that has any in this world
                if (!entryFound && (entry == null || Pressure(level) < Pressure(entry))) {
                    entry = level;
                }
            }

            entryFound = entryFound || levels.Count > 0;
        }

        return entry != null && cap != null;
    }

    // ----------------------------------------------------------------------------------------

    static List<object> Directors(IDictionary<string, object> entryData, IDictionary<string, object> capData, double t) {

        List<object> directors = new List<object>();

        foreach (object raw in List(Get(entryData, "directors"))) {

            Dictionary<string, object> director = Clone(raw) as Dictionary<string, object>;

            if (director == null) {
                continue;
            }

            string code = Str(Get(director, "code"));
            double min = Num(Get(director, "min"), 0);
            double max = Num(Get(director, "max"), 0);

            if (code == "enemy" && Bool(Get(director, "run"), true)) {

                IDictionary<string, object> capEnemy = Director(capData, "enemy");
                double capMin = Num(Get(capEnemy, "min"), min);
                double capMax = Math.Max(max, Num(Get(capEnemy, "max"), max));

                double startMax = Math.Max(1, Math.Floor(max / 2));
                double startMin = Math.Min(startMax, Math.Max(1, Math.Floor(min / 2)));
                double endMin = Math.Min(capMax, Math.Max(startMin, capMin));

                double toMax = Round(Lerp(startMax, capMax, t));
                director["max"] = toMax;
                director["min"] = Math.Min(toMax, Round(Lerp(startMin, endMin, t)));
            }
            else if (code == "weapon" || code == "item") {

                director["run"] = true;
                director["min"] = Round(Lerp(min + pickupBonus, min, t));
                director["max"] = Round(Lerp(max + pickupBonus, max, t));
            }

            directors.Add(director);
        }

        return directors;
    }

    // Each preset keeps its authored max; the seed lifts its min up to halfway, so prop density
    // varies per seed but stays inside what the level allows.
    static List<object> AssetPresets(object raw, int seed) {

        List<object> presets = new List<object>();
        int index = 0;

        foreach (object item in List(raw)) {

            Dictionary<string, object> preset = Clone(item) as Dictionary<string, object>;

            if (preset != null && preset.ContainsKey("min") && preset.ContainsKey("max")) {
                double min = Num(preset["min"], 0);
                double max = Math.Max(min, Num(preset["max"], min));
                preset["min"] = Math.Floor(min + (max - min) / 2 * Unit(seed, index));
                preset["max"] = max;
            }

            if (preset != null) {
                presets.Add(preset);
            }

            index++;
        }

        return presets;
    }

    static int CompareMissions(IDictionary<string, object> a, IDictionary<string, object> b) {
        int byLevel = Num(Get(a, "min_level"), 0).CompareTo(Num(Get(b, "min_level"), 0));
        return byLevel != 0 ? byLevel : Num(Get(a, "sort_order"), 0).CompareTo(Num(Get(b, "sort_order"), 0));
    }

    static bool IsTest(IDictionary<string, object> row) {
        foreach (object tag in List(Get(row, "tags"))) {
            if (Str(tag) == GameArcadeLevelKeys.test) {
                return true;
            }
        }
        return false;
    }

    static IDictionary<string, object> Director(IDictionary<string, object> data, string code) {
        foreach (object raw in List(Get(data, "directors"))) {
            IDictionary<string, object> director = Dict(raw);
            if (director != null && Str(Get(director, "code")) == code) {
                return director;
            }
        }
        return null;
    }

    // A hash in [0, 1) of the seed and a salt (splitmix64 finaliser); the same on every runtime.
    static double Unit(int seed, int salt) {
        ulong z = unchecked((ulong)(uint)seed * 0x9E3779B97F4A7C15UL + (ulong)(uint)salt * 0xBF58476D1CE4E5B9UL + 1);
        z = unchecked((z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL);
        z = unchecked((z ^ (z >> 27)) * 0x94D049BB133111EBUL);
        z ^= z >> 31;
        return (z >> 11) / (double)(1UL << 53);
    }

    static double Lerp(double from, double to, double t) {
        return from + (to - from) * t;
    }

    static double Round(double value) {
        return Math.Round(value, MidpointRounding.AwayFromZero);
    }

    // A deep copy into plain dictionaries and lists, so typed DataObjects (GameDataDirector, ...)
    // and the source rows are never shared with the generated row.
    static object Clone(object value) {

        IDictionary<string, object> dict = Dict(value);

        if (dict != null) {
            Dictionary<string, object> copy = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> pair in dict) {
                copy[pair.Key] = Clone(pair.Value);
            }
            return copy;
        }

        if (value is IList) {
            List<object> copy = new List<object>();
            foreach (object item in (IList)value) {
                copy.Add(Clone(item));
            }
            return copy;
        }

        return value;
    }

    static object Get(IDictionary<string, object> dict, string key) {
        object value;
        return dict != null && dict.TryGetValue(key, out value) ? value : null;
    }

    static IDictionary<string, object> Dict(object value) {
        return value as IDictionary<string, object>;
    }

    static List<object> List(object value) {
        List<object> items = new List<object>();
        IEnumerable enumerable = value as IEnumerable;
        if (enumerable != null && !(value is string) && !(value is IDictionary<string, object>)) {
            foreach (object item in enumerable) {
                items.Add(item);
            }
        }
        return items;
    }

    static string Str(object value) {
        return value == null ? null : value.ToString();
    }

    static double Num(object value, double fallback) {
        if (value == null || value is string || value is bool) {
            return fallback;
        }
        try {
            return Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (Exception) {
            return fallback;
        }
    }

    static bool Bool(object value, bool fallback) {
        return value is bool ? (bool)value : fallback;
    }
}
