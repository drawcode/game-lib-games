#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Core;

using Engine.AgnosticHost;
using Engine.Data.Json;

// The racer palettes (D22): game-race-palette-data rows, read raw with LitJson like GameRaceAiProfiles. A row is one
// vendor skin (material name) with its sampled key and accent colours. No file or no usable rows = off (enabled is
// false) and the race looks as it always did.
//   racer-rider   {family, material, key, accent}
//   racer-vehicle {family, vehicle_codes[], material, key, accent}
//   racer-palette-rules {min_rival, min_player, max_neutral, neutral_chroma, salt}
public static class GameRacePalettes {

    public const string dataKey = "game-race-palette-data";
    public const string typeRider = "racer-rider";
    public const string typeVehicle = "racer-vehicle";
    public const string typeRules = "racer-palette-rules";

    public class Row {
        public string code;
        public string type;
        public string displayName;
        public int sortOrder;
        public string family;
        public string[] vehicleCodes;
        public string material;
        public ColorRgba key;
        public ColorRgba accent;
        public OkLab keyLab;

        public bool isRider { get { return type == typeRider; } }

        public bool ForVehicle(string vehicleCode) {

            for (int i = 0; i < vehicleCodes.Length; i++) {
                if (vehicleCodes[i] == vehicleCode) {
                    return true;
                }
            }

            return false;
        }
    }

    private static List<Row> rows = new List<Row>();
    private static PaletteRules rules = PaletteRules.Defaults();
    private static bool loaded;
    private static readonly List<Row> none = new List<Row>();
    private static readonly List<Row> riderRows = new List<Row>();
    private static readonly Dictionary<string, List<Row>> vehicleRows = new Dictionary<string, List<Row>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        Clear();
        loaded = false;
    }

    public static PaletteRules Rules {
        get {
            Ensure();
            return rules;
        }
    }

    // True when there is at least one skin row.
    public static bool enabled {
        get {
            Ensure();
            return rows.Count > 0;
        }
    }

    public static int count {
        get {
            Ensure();
            return rows.Count;
        }
    }

    // The rider skins in sort_order.
    public static List<Row> Riders() {
        Ensure();
        return riderRows;
    }

    // The skins that fit a vehicle code, in sort_order (empty when none). Cached per vehicle code.
    public static List<Row> ForVehicle(string vehicleCode) {

        Ensure();

        if (string.IsNullOrEmpty(vehicleCode)) {
            return none;
        }

        List<Row> list;

        if (!vehicleRows.TryGetValue(vehicleCode, out list)) {
            list = new List<Row>();

            for (int i = 0; i < rows.Count; i++) {
                if (!rows[i].isRider && rows[i].ForVehicle(vehicleCode)) {
                    list.Add(rows[i]);
                }
            }

            vehicleRows[vehicleCode] = list;
        }

        return list;
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
        if (!loaded) {
            Load(LoadText());
        }
    }

    private static string LoadText() {

        string path = ContentsConfig.contentRootFolder + "/" + ContentsConfig.contentAppFolder + "/version/data/" + dataKey + ".json";
        TextAsset asset = Resources.Load<TextAsset>(path);
        return asset != null ? asset.text : "";
    }

    private static void Clear() {
        rows = new List<Row>();
        rules = PaletteRules.Defaults();
        riderRows.Clear();
        vehicleRows.Clear();
    }

    // Replaces the cache from json; an empty or unreadable text leaves it empty (off).
    public static void Load(string json) {

        Clear();
        loaded = true;

        if (string.IsNullOrEmpty(json)) {
            return;
        }

        try {
            JsonData parsed = JsonMapper.ToObject(json);

            if (parsed.IsArray) {
                for (int i = 0; i < parsed.Count; i++) {
                    JsonData n = parsed[i];

                    if (VehicleTuningJson.Str(n, "type") == typeRules) {
                        ParseRules(n);
                        continue;
                    }

                    Row row = Parse(n);

                    if (row != null) {
                        rows.Add(row);
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogWarning("GameRacePalettes: " + dataKey + " rejected: " + e.Message);
            Clear();
            return;
        }

        rows.Sort(CompareRows);

        for (int i = 0; i < rows.Count; i++) {
            if (rows[i].isRider) {
                riderRows.Add(rows[i]);
            }
        }
    }

    private static int CompareRows(Row a, Row b) {
        int c = a.sortOrder.CompareTo(b.sortOrder);
        return c != 0 ? c : string.CompareOrdinal(a.code, b.code);
    }

    // A skin row, or null for an inactive, unknown-type or incomplete one.
    public static Row Parse(JsonData n) {

        string code = VehicleTuningJson.Str(n, "code");
        string type = VehicleTuningJson.Str(n, "type");

        if (string.IsNullOrEmpty(code) || (type != typeRider && type != typeVehicle) || !VehicleTuningJson.Has(n, "data")) {
            return null;
        }

        if (VehicleTuningJson.Has(n, "active") && !VehicleTuningJson.Bool(n, "active")) {
            return null;
        }

        JsonData d = n["data"];
        string material = VehicleTuningJson.Str(d, "material");
        ColorRgba key;

        if (string.IsNullOrEmpty(material) || !ColorRgba.TryParseHex(VehicleTuningJson.Str(d, "key"), out key)) {
            return null;
        }

        Row row = new Row();
        row.code = code;
        row.type = type;
        row.displayName = VehicleTuningJson.Has(n, "display_name") ? VehicleTuningJson.Str(n, "display_name") : code;
        row.sortOrder = (int)VehicleTuningJson.Num(n, "sort_order", 0f);
        row.family = VehicleTuningJson.Str(d, "family");
        row.material = material;
        row.key = key;

        if (!ColorRgba.TryParseHex(VehicleTuningJson.Str(d, "accent"), out row.accent)) {
            row.accent = key;
        }

        row.keyLab = OkLab.FromSrgb(key);

        List<string> codes = new List<string>();

        if (VehicleTuningJson.Has(d, "vehicle_codes") && d["vehicle_codes"].IsArray) {
            JsonData list = d["vehicle_codes"];

            for (int i = 0; i < list.Count; i++) {
                if (list[i].IsString) {
                    codes.Add((string)list[i]);
                }
            }
        }

        if (type == typeVehicle && codes.Count == 0) {
            return null;
        }

        row.vehicleCodes = codes.ToArray();
        return row;
    }

    private static void ParseRules(JsonData n) {

        PaletteRules r = PaletteRules.Defaults();

        if (VehicleTuningJson.Has(n, "data")) {
            JsonData d = n["data"];
            r.minRival = VehicleTuningJson.Num(d, "min_rival", r.minRival);
            r.minPlayer = VehicleTuningJson.Num(d, "min_player", r.minPlayer);
            r.maxNeutral = (int)VehicleTuningJson.Num(d, "max_neutral", r.maxNeutral);
            r.neutralChroma = VehicleTuningJson.Num(d, "neutral_chroma", r.neutralChroma);

            // the salt does not fit a float: read it as an integer
            if (VehicleTuningJson.Has(d, "salt")) {
                JsonData s = d["salt"];

                if (s.IsLong) {
                    r.salt = unchecked((uint)(long)s);
                }
                else if (s.IsInt) {
                    r.salt = unchecked((uint)(int)s);
                }
                else if (s.IsDouble) {
                    r.salt = unchecked((uint)(long)(double)s);
                }
            }
        }

        rules = r;
    }
}
#endif
