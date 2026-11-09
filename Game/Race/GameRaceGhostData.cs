#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;
using System.Text;

using Engine.AgnosticHost;
using Engine.Data.Json;

// One recorded lap: keyed sample columns at hz. A missing column reads as empty (every sample then takes the
// column's default), a malformed one is kept under "unreadable:<key>" and skipped, and a key this build does not
// know is kept and written back as it came (a newer build's extra columns survive an older build's save).
public class GameRaceGhostLap {

    public const int colT = 0;
    public const int colX = 1;
    public const int colY = 2;
    public const int colZ = 3;
    public const int colYaw = 4;
    public const int colPitch = 5;
    public const int colRoll = 6;
    public const int colUpX = 7;
    public const int colUpZ = 8;
    public const int colFlags = 9;
    public const int colMoveX = 10;
    public const int colMoveY = 11;
    public const int colCamYaw = 12;
    public const int colButtons = 13;
    public const int columnCount = 14;

    // x, y, z and up_* are in Unity world axes, yaw / pitch / roll are the arcade state's radians, flags and
    // buttons are the state's VehicleFlags and the sticks' VehicleButtons.
    public static readonly string[] columnKeys = {
        "t", "x", "y", "z", "yaw", "pitch", "roll", "up_x", "up_z", "flags", "move_x", "move_y", "cam_yaw", "buttons"
    };

    // Decimals written per column; flags and buttons are whole numbers.
    private static readonly int[] columnDecimals = { 3, 3, 3, 3, 4, 4, 4, 4, 4, -1, 3, 3, 4, -1 };

    public string id = "";
    public string rider = "player";
    public string vehicleCode = "";
    public string controlMode = "heading";
    public string recorded = "";
    public string trackHash = "";
    public float lapTime;
    public int hz = 20;
    public bool clean = true;
    public int count;                                           // samples; the length of the t column
    public readonly float[][] columns = new float[columnCount][];
    public readonly List<KeyValuePair<string, JsonData>> extras = new List<KeyValuePair<string, JsonData>>();
    public readonly List<KeyValuePair<string, JsonData>> sampleExtras = new List<KeyValuePair<string, JsonData>>();

    public float Get(int column, int i, float fallback) {
        float[] c = columns[column];
        return c != null && i >= 0 && i < c.Length ? c[i] : fallback;
    }

    // Playable: a time column with at least two samples.
    public bool usable {
        get {
            return count >= 2 && columns[colT] != null;
        }
    }

    internal static GameRaceGhostLap Parse(JsonData n) {

        if (n == null || !n.IsObject) {
            return null;
        }

        GameRaceGhostLap lap = new GameRaceGhostLap();
        lap.id = VehicleTuningJson.Str(n, "id");
        lap.rider = VehicleTuningJson.Has(n, "rider") ? VehicleTuningJson.Str(n, "rider") : "player";
        lap.vehicleCode = VehicleTuningJson.Str(n, "vehicle_code");
        lap.controlMode = VehicleTuningJson.Has(n, "control_mode") ? VehicleTuningJson.Str(n, "control_mode") : "heading";
        lap.recorded = VehicleTuningJson.Str(n, "recorded");
        lap.trackHash = VehicleTuningJson.Str(n, "track_hash");
        lap.lapTime = VehicleTuningJson.Num(n, "lap_time", 0f);
        lap.hz = (int)VehicleTuningJson.Num(n, "hz", 20f);
        lap.clean = !VehicleTuningJson.Has(n, "clean") || !n["clean"].IsBoolean || (bool)n["clean"];

        foreach (string key in n.Keys) {
            switch (key) {
                case "id": case "rider": case "vehicle_code": case "control_mode": case "recorded": case "track_hash":
                case "lap_time": case "hz": case "clean": case "samples":
                    break;
                default:
                    lap.extras.Add(new KeyValuePair<string, JsonData>(key, n[key]));
                    break;
            }
        }

        if (VehicleTuningJson.Has(n, "samples") && n["samples"].IsObject) {
            JsonData s = n["samples"];

            foreach (string key in s.Keys) {

                int column = Array.IndexOf(columnKeys, key);

                if (column < 0) {
                    lap.sampleExtras.Add(new KeyValuePair<string, JsonData>(key, s[key]));
                    continue;
                }

                float[] values = ReadColumn(s[key]);

                if (values == null) {
                    lap.sampleExtras.Add(new KeyValuePair<string, JsonData>("unreadable:" + key, s[key]));
                }
                else {
                    lap.columns[column] = values;
                }
            }
        }

        lap.count = lap.columns[colT] != null ? lap.columns[colT].Length : 0;
        return lap;
    }

    // null = not an array of numbers.
    private static float[] ReadColumn(JsonData d) {

        if (d == null || !d.IsArray) {
            return null;
        }

        float[] values = new float[d.Count];

        for (int i = 0; i < values.Length; i++) {

            JsonData v = d[i];

            if (v == null || !(v.IsDouble || v.IsInt || v.IsLong)) {
                return null;
            }

            values[i] = VehicleTuningJson.Number(v, 0f);
        }

        return values;
    }

    internal void Write(JsonWriter w) {

        w.WriteObjectStart();
        Prop(w, "id", id);
        Prop(w, "rider", rider);
        Prop(w, "vehicle_code", vehicleCode);
        Prop(w, "control_mode", controlMode);
        w.WritePropertyName("lap_time");
        w.Write(Math.Round((double)lapTime, 3));
        Prop(w, "recorded", recorded);
        Prop(w, "track_hash", trackHash);
        w.WritePropertyName("hz");
        w.Write(hz);
        w.WritePropertyName("clean");
        w.Write(clean);

        for (int i = 0; i < extras.Count; i++) {
            w.WritePropertyName(extras[i].Key);
            extras[i].Value.ToJson(w);
        }

        w.WritePropertyName("samples");
        w.WriteObjectStart();

        for (int c = 0; c < columnCount; c++) {

            float[] values = columns[c];

            if (values == null) {
                continue;
            }

            w.WritePropertyName(columnKeys[c]);
            w.WriteArrayStart();
            int n = c == colT ? count : Math.Min(values.Length, count);

            for (int i = 0; i < n; i++) {
                if (columnDecimals[c] < 0) {
                    w.Write((int)Math.Round(values[i]));
                }
                else {
                    w.Write(Math.Round((double)values[i], columnDecimals[c]));
                }
            }

            w.WriteArrayEnd();
        }

        for (int i = 0; i < sampleExtras.Count; i++) {
            w.WritePropertyName(sampleExtras[i].Key);
            sampleExtras[i].Value.ToJson(w);
        }

        w.WriteObjectEnd();
        w.WriteObjectEnd();
    }

    internal static void Prop(JsonWriter w, string key, string value) {
        w.WritePropertyName(key);
        w.Write(value ?? "");
    }
}

// The ghosts file of one track: <persistentDataPath>/ghosts/ghost-<level_code>.json, see GameRaceGhostStore.
public class GameRaceGhostFile {

    public const string kindValue = "race-ghosts";

    public string track = "";
    public string trackHash = "";
    public float bestLapTime;

    // ai_best_laps: each AI profile's best race lap on the track with trackHash (seconds), for qualifying.
    public readonly Dictionary<string, float> aiBestLaps = new Dictionary<string, float>();
    public readonly List<GameRaceGhostLap> laps = new List<GameRaceGhostLap>();
    public readonly List<KeyValuePair<string, JsonData>> extras = new List<KeyValuePair<string, JsonData>>();

    // Set by the store when the file came from the leftover temp sibling of a crashed write.
    public bool recovered;

    // null when the text is not a ghosts file.
    public static GameRaceGhostFile Parse(string json) {

        if (string.IsNullOrEmpty(json)) {
            return null;
        }

        JsonData root;

        try {
            root = JsonMapper.ToObject(json);
        }
        catch (Exception) {
            return null;
        }

        if (root == null || !root.IsObject || VehicleTuningJson.Str(root, "kind") != kindValue) {
            return null;
        }

        GameRaceGhostFile file = new GameRaceGhostFile();
        file.track = VehicleTuningJson.Str(root, "track");
        file.trackHash = VehicleTuningJson.Str(root, "track_hash");
        file.bestLapTime = VehicleTuningJson.Num(root, "best_lap_time", 0f);

        foreach (string key in root.Keys) {
            switch (key) {
                case "kind": case "track": case "track_hash": case "best_lap_time": case "laps":
                    break;
                case "ai_best_laps":
                    ReadAiBestLaps(root[key], file.aiBestLaps);
                    break;
                default:
                    file.extras.Add(new KeyValuePair<string, JsonData>(key, root[key]));
                    break;
            }
        }

        if (VehicleTuningJson.Has(root, "laps") && root["laps"].IsArray) {
            for (int i = 0; i < root["laps"].Count; i++) {

                GameRaceGhostLap lap = GameRaceGhostLap.Parse(root["laps"][i]);

                if (lap == null) {
                    continue;
                }

                // A lap written before the file's hash changed keeps the hash it was recorded on.
                if (string.IsNullOrEmpty(lap.trackHash)) {
                    lap.trackHash = file.trackHash;
                }

                file.laps.Add(lap);
            }
        }

        return file;
    }

    // Tolerant: anything but an object of positive numbers is skipped.
    private static void ReadAiBestLaps(JsonData d, Dictionary<string, float> into) {

        if (d == null || !d.IsObject) {
            return;
        }

        foreach (string code in d.Keys) {

            JsonData v = d[code];

            if (v != null && (v.IsDouble || v.IsInt || v.IsLong)) {
                float time = VehicleTuningJson.Number(v, 0f);

                if (time > 0f && !float.IsInfinity(time) && !float.IsNaN(time)) {
                    into[code] = time;
                }
            }
        }
    }

    public string ToJson() {

        StringBuilder sb = new StringBuilder(4096);
        JsonWriter w = new JsonWriter(sb);
        w.WriteObjectStart();
        GameRaceGhostLap.Prop(w, "kind", kindValue);
        GameRaceGhostLap.Prop(w, "track", track);
        GameRaceGhostLap.Prop(w, "track_hash", trackHash);
        w.WritePropertyName("best_lap_time");
        w.Write(Math.Round((double)bestLapTime, 3));

        if (aiBestLaps.Count > 0) {
            List<string> codes = new List<string>(aiBestLaps.Keys);
            codes.Sort(StringComparer.Ordinal);
            w.WritePropertyName("ai_best_laps");
            w.WriteObjectStart();

            for (int i = 0; i < codes.Count; i++) {
                w.WritePropertyName(codes[i]);
                w.Write(Math.Round((double)aiBestLaps[codes[i]], 3));
            }

            w.WriteObjectEnd();
        }

        for (int i = 0; i < extras.Count; i++) {
            w.WritePropertyName(extras[i].Key);
            extras[i].Value.ToJson(w);
        }

        w.WritePropertyName("laps");
        w.WriteArrayStart();

        for (int i = 0; i < laps.Count; i++) {
            laps[i].Write(w);
        }

        w.WriteArrayEnd();
        w.WriteObjectEnd();
        return sb.ToString();
    }
}
#endif
