#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Racing;

using Engine.AgnosticHost;
using Engine.Data.Json;

// The AI riders' profiles (Action Bikes R2 step 7): game-race-ai-profile-data rows, read raw with LitJson like
// GameVehicleArcadeData. A missing or empty file gives five built-in rows (ai-rider-1..5), the same five the
// content pack ships.
public static class GameRaceAiProfiles {

    public const string dataKey = "game-race-ai-profile-data";

    public class Row {
        public string code;
        public string displayName;
        public string vehicleCode;
        public RaceAiProfile profile;
    }

    private static List<Row> rows;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        rows = null;
    }

    public static int count {
        get {
            Ensure();
            return rows.Count;
        }
    }

    public static Row At(int i) {
        Ensure();
        return rows.Count > 0 ? rows[((i % rows.Count) + rows.Count) % rows.Count] : null;
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

    // Replaces the cache from json; an empty or unreadable text gives the built-in rows.
    public static void Load(string json) {

        rows = new List<Row>();

        if (!string.IsNullOrEmpty(json)) {
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
                Debug.LogWarning("GameRaceAiProfiles: " + dataKey + " rejected: " + e.Message);
                rows.Clear();
            }
        }

        if (rows.Count == 0) {
            rows = BuiltIn();
        }
    }

    public static Row Parse(JsonData n) {

        string code = VehicleTuningJson.Str(n, "code");

        if (string.IsNullOrEmpty(code)) {
            return null;
        }

        Row row = new Row();
        row.code = code;
        row.displayName = VehicleTuningJson.Has(n, "display_name") ? VehicleTuningJson.Str(n, "display_name") : code;
        row.vehicleCode = VehicleTuningJson.Has(n, "vehicle_code") ? VehicleTuningJson.Str(n, "vehicle_code") : "";
        row.profile = RaceAiProfile.Defaults();

        if (VehicleTuningJson.Has(n, "data")) {
            JsonData d = n["data"];
            RaceAiProfile p = row.profile;
            p.lineOffset = VehicleTuningJson.Num(d, "line_offset", p.lineOffset);
            p.lookaheadMin = VehicleTuningJson.Num(d, "lookahead_min", p.lookaheadMin);
            p.lookaheadMax = VehicleTuningJson.Num(d, "lookahead_max", p.lookaheadMax);
            p.cornerSpeed = VehicleTuningJson.Num(d, "corner_speed", p.cornerSpeed);
            p.reactionDelay = VehicleTuningJson.Num(d, "reaction_delay", p.reactionDelay);
            p.noise = VehicleTuningJson.Num(d, "noise", p.noise);
            p.handling = VehicleTuningJson.Num(d, "handling", p.handling);
            p.aggression = VehicleTuningJson.Num(d, "aggression", p.aggression);
            p.aggroRange = VehicleTuningJson.Num(d, "aggro_range", p.aggroRange);
            row.profile = p;
        }

        return row;
    }

    private static List<Row> BuiltIn() {

        // line_offset, corner_speed, aggression, reaction_delay, noise, handling
        float[][] v = {
            new[] { 0f, .95f, .3f, .12f, .03f, .9f },
            new[] { .3f, .9f, .7f, .15f, .05f, .8f },
            new[] { -.3f, .85f, .5f, .18f, .06f, .75f },
            new[] { .15f, .88f, .9f, .2f, .08f, .7f },
            new[] { -.15f, .8f, .2f, .22f, .1f, .6f }
        };
        string[] names = { "Ace", "Rex", "Dot", "Blaze", "Pip" };
        List<Row> list = new List<Row>();

        for (int i = 0; i < v.Length; i++) {
            Row row = new Row();
            row.code = "ai-rider-" + (i + 1);
            row.displayName = names[i];
            row.vehicleCode = "vehicle-bike-dirt-1";
            RaceAiProfile p = RaceAiProfile.Defaults();
            p.lineOffset = v[i][0];
            p.cornerSpeed = v[i][1];
            p.aggression = v[i][2];
            p.reactionDelay = v[i][3];
            p.noise = v[i][4];
            p.handling = v[i][5];
            row.profile = p;
            list.Add(row);
        }

        return list;
    }
}
#endif
