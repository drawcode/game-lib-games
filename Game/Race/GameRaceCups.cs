#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using UnityEngine;

using Engine.AgnosticHost;
using Engine.Data.Json;

// The career cups (Action Bikes career): game-race-cup-data rows, read raw with LitJson like GameRaceAiProfiles. One
// row is a cup: its tracks (level codes), a race block laid over each track's own, a per_race block per round, the
// points table and the rewards. Inactive rows are skipped. A missing or unreadable file gives no cups.
//   {"code": "cup-n00b", "active": true, "data": {"tracks": [...], "race": {...}, "per_race": [{}, {"laps": 3}], ...}}
public static class GameRaceCups {

    public const string dataKey = "game-race-cup-data";
    public const string gridReverseStandings = "reverse_standings";

    public class Reward {
        public int placeMax;
        public bool once;
        public string[] unlock = new string[0];
    }

    public class Row {
        public string code;
        public string displayName;
        public int sortOrder;
        public int tier = 1;
        public string unlockAfter = "";
        public string[] tracks = new string[0];
        public Dictionary<string, object> race = new Dictionary<string, object>();
        public List<Dictionary<string, object>> perRace = new List<Dictionary<string, object>>();
        public string grid = "";
        public string boss = "";
        public int[] points = { 10, 8, 6, 4, 2, 1 };
        public int podium = 3;
        public int[] coinsPlace = new int[0];
        public int[] coinsPodium = new int[0];
        public int[] xpPodium = new int[0];
        public float repeatBonus = 0.5f;
        public List<Reward> rewards = new List<Reward>();

        // The race block of round k: the cup's, then per_race[k] over it (null when neither names a key).
        public Dictionary<string, object> RaceBlock(int k) {

            Dictionary<string, object> block = new Dictionary<string, object>(race);

            if (k >= 0 && k < perRace.Count && perRace[k] != null) {
                foreach (KeyValuePair<string, object> e in perRace[k]) {
                    block[e.Key] = e.Value;
                }
            }

            return block;
        }
    }

    private static List<Row> rows;
    private static HashSet<string> warned = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        rows = null;
        warned = new HashSet<string>();
    }

    public static int count {
        get {
            Ensure();
            return rows.Count;
        }
    }

    public static Row At(int i) {
        Ensure();
        return i >= 0 && i < rows.Count ? rows[i] : null;
    }

    public static Row Get(string code) {

        Ensure();

        for (int i = 0; i < rows.Count; i++) {
            if (rows[i].code == code) {
                return rows[i];
            }
        }

        return null;
    }

    private static void Ensure() {
        if (rows == null) {
            Load(LoadText());
        }
    }

    private static string LoadText() {

        string path = ContentsConfig.contentRootFolder + "/" + ContentsConfig.contentAppFolder + "/version/data/" + dataKey + ".json";
        TextAsset asset = Resources.Load<TextAsset>(path);
        return asset != null ? asset.text : "";
    }

    // Replaces the cache from json, sorted by sort_order; an empty or unreadable text gives no cups.
    public static void Load(string json) {

        rows = new List<Row>();

        if (string.IsNullOrEmpty(json)) {
            return;
        }

        try {
            JsonData parsed = JsonMapper.ToObject(json);

            if (parsed.IsArray) {
                for (int i = 0; i < parsed.Count; i++) {
                    Row row = Parse(parsed[i]);

                    if (row != null) {
                        rows.Add(row);
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogWarning("GameRaceCups: " + dataKey + " rejected: " + e.Message);
            rows.Clear();
        }

        rows.Sort((a, b) => a.sortOrder.CompareTo(b.sortOrder));
    }

    // null for a row with no code or one that is not active.
    public static Row Parse(JsonData n) {

        string code = VehicleTuningJson.Str(n, "code");

        if (string.IsNullOrEmpty(code)) {
            return null;
        }

        if (VehicleTuningJson.Has(n, "active") && n["active"].IsBoolean && !(bool)n["active"]) {
            return null;
        }

        Row row = new Row();
        row.code = code;
        row.displayName = VehicleTuningJson.Has(n, "display_name") ? VehicleTuningJson.Str(n, "display_name") : code;
        row.sortOrder = (int)VehicleTuningJson.Num(n, "sort_order", 0f);

        if (!VehicleTuningJson.Has(n, "data") || !n["data"].IsObject) {
            return row;
        }

        JsonData d = n["data"];
        row.tier = (int)VehicleTuningJson.Num(d, "tier", row.tier);
        row.unlockAfter = VehicleTuningJson.Str(d, "unlock_after");
        row.tracks = Strings(d, "tracks");
        row.grid = VehicleTuningJson.Str(d, "grid");
        row.boss = VehicleTuningJson.Str(d, "boss");
        row.podium = (int)VehicleTuningJson.Num(d, "podium", row.podium);
        row.repeatBonus = VehicleTuningJson.Num(d, "repeat_bonus", row.repeatBonus);

        if (VehicleTuningJson.Has(d, "points")) {
            row.points = Ints(d["points"], row.points);
        }

        row.coinsPlace = Ints(Member(d, "coins_place"), row.coinsPlace);
        row.coinsPodium = Ints(Member(d, "coins_podium"), row.coinsPodium);
        row.xpPodium = Ints(Member(d, "xp_podium"), row.xpPodium);

        object race = Member(d, "race") != null ? ToObject(d["race"]) : null;

        if (race is Dictionary<string, object>) {
            row.race = (Dictionary<string, object>)race;
        }

        if (VehicleTuningJson.Has(d, "per_race") && d["per_race"].IsArray) {
            for (int i = 0; i < d["per_race"].Count; i++) {
                row.perRace.Add(ToObject(d["per_race"][i]) as Dictionary<string, object>);
            }
        }

        if (VehicleTuningJson.Has(d, "rewards") && d["rewards"].IsArray) {
            for (int i = 0; i < d["rewards"].Count; i++) {
                JsonData r = d["rewards"][i];
                Reward reward = new Reward();
                reward.placeMax = (int)VehicleTuningJson.Num(r, "place_max", 0f);
                reward.once = !VehicleTuningJson.Has(r, "once") || !r["once"].IsBoolean || (bool)r["once"];
                reward.unlock = Strings(r, "unlock");
                row.rewards.Add(reward);
            }
        }

        return row;
    }

    // True when every track of the cup has an active level row and a shipped level file; a hidden cup logs the missing
    // codes once.
    public static bool Visible(Row cup) {
        return Visible(cup, TrackReady);
    }

    // trackReady(levelCode): the row and the file are both there. Split out so tests can stand in for the content.
    public static bool Visible(Row cup, Func<string, bool> trackReady) {

        if (cup == null) {
            return false;
        }

        string missing = null;

        for (int i = 0; i < cup.tracks.Length; i++) {
            if (!trackReady(cup.tracks[i])) {
                missing = missing == null ? cup.tracks[i] : missing + ", " + cup.tracks[i];
            }
        }

        if (missing != null && warned.Add(cup.code)) {
            Debug.LogWarning("GameRaceCups: " + cup.code + " is hidden, missing tracks: " + missing);
        }

        return missing == null && cup.tracks.Length > 0;
    }

    public static bool TrackReady(string levelCode) {
        return GameLevels.Instance.GetById(levelCode) != null
            && Resources.Load<TextAsset>(GameLevelItemSources.GetShippedResourcePath(levelCode)) != null;
    }

    private static JsonData Member(JsonData n, string key) {
        return VehicleTuningJson.Has(n, key) ? n[key] : null;
    }

    private static string[] Strings(JsonData n, string key) {

        if (!VehicleTuningJson.Has(n, key) || !n[key].IsArray) {
            return new string[0];
        }

        JsonData a = n[key];
        List<string> list = new List<string>();

        for (int i = 0; i < a.Count; i++) {
            if (a[i].IsString && ((string)a[i]).Length > 0) {
                list.Add((string)a[i]);
            }
        }

        return list.ToArray();
    }

    private static int[] Ints(JsonData a, int[] fallback) {

        if (a == null || !a.IsArray) {
            return fallback;
        }

        int[] values = new int[a.Count];

        for (int i = 0; i < values.Length; i++) {
            values[i] = (int)VehicleTuningJson.Number(a[i], 0f);
        }

        return values;
    }

    // JsonData to plain objects (Dictionary, List, double, int, long, string, bool), what a level row's data holds.
    public static object ToObject(JsonData n) {

        if (n == null) {
            return null;
        }

        if (n.IsObject) {
            Dictionary<string, object> map = new Dictionary<string, object>();

            foreach (string key in n.Keys) {
                map[key] = ToObject(n[key]);
            }

            return map;
        }

        if (n.IsArray) {
            List<object> list = new List<object>();

            for (int i = 0; i < n.Count; i++) {
                list.Add(ToObject(n[i]));
            }

            return list;
        }

        if (n.IsInt) {
            return (int)n;
        }

        if (n.IsLong) {
            return (long)n;
        }

        if (n.IsDouble) {
            return (double)n;
        }

        if (n.IsBoolean) {
            return (bool)n;
        }

        return n.IsString ? (string)n : null;
    }
}
#endif
