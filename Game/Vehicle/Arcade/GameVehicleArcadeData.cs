#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using Agnostic.Vehicle;

using Engine.AgnosticHost;
using Engine.Data.Json;

using UnityEngine;

// What GameVehicleArcade needs from one game-vehicle-data row (arcade design section 3, riders doc section 4.3).
public class GameVehicleArcadeConfig {

    public string code;
    public string vehicleClass;
    public VehicleTuning tuning;                 // data.tuning, with data.surfaces folded in
    public ControlTuning control;                // data.control; mode is the class default (D8)
    public List<string> surfaces = new List<string>();
    public int layerMask = Physics.DefaultRaycastLayers;   // data.host.layers
    public string ridePresets = "";              // data.rider.ride_presets: animation-ride-<slot>-<this>
    public string visual = "vehicle";            // data.host.visual: the child that leans and pitches
    public string[] wheels = new string[0];      // data.host.wheels: wheel bones in probe order
    public Vector3 wheelAxis = Vector3.right;    // data.host.wheel_axis: local spin axis, sign = forward roll
}

// game-vehicle-data rows, read raw with LitJson: the typed GameVehicle loader keeps no `tuning` block. A row
// without data.tuning is not an arcade vehicle, so Action Bots (no such rows) never attaches one.
public static class GameVehicleArcadeData {

    public const string dataKey = "game-vehicle-data";

    private static Dictionary<string, GameVehicleArcadeConfig> configs;

    // Bikes enters Play without a domain reload, so the cache is dropped here or edited data never reloads.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        configs = null;
    }

    public static GameVehicleArcadeConfig Get(string code) {

        if (string.IsNullOrEmpty(code)) {
            return null;
        }

        if (configs == null) {
            configs = new Dictionary<string, GameVehicleArcadeConfig>(StringComparer.Ordinal);
            Load(LoadText());
        }

        GameVehicleArcadeConfig config;
        return configs.TryGetValue(code, out config) ? config : null;
    }

    // The content pack's file, as DataObjects.LoadDataFromResources finds it.
    private static string LoadText() {

        string path = ContentsConfig.contentRootFolder + "/" + ContentsConfig.contentAppFolder + "/version/data/" + dataKey + ".json";
        TextAsset asset = Resources.Load<TextAsset>(path);
        return asset != null ? asset.text : "";
    }

    public static void Load(string json) {

        if (configs == null) {
            configs = new Dictionary<string, GameVehicleArcadeConfig>(StringComparer.Ordinal);
        }

        if (string.IsNullOrEmpty(json)) {
            return;
        }

        JsonData rows;

        try {
            rows = JsonMapper.ToObject(json);
        }
        catch (Exception e) {
            Debug.LogWarning("GameVehicleArcadeData: " + dataKey + " rejected: " + e.Message);
            return;
        }

        if (!rows.IsArray) {
            return;
        }

        for (int i = 0; i < rows.Count; i++) {
            GameVehicleArcadeConfig config = Parse(rows[i]);

            if (config != null) {
                configs[config.code] = config;
            }
        }
    }

    public static GameVehicleArcadeConfig Parse(JsonData row) {

        string code = VehicleTuningJson.Str(row, "code");

        if (string.IsNullOrEmpty(code) || !VehicleTuningJson.Has(row, "data")) {
            return null;
        }

        JsonData data = row["data"];

        if (!VehicleTuningJson.Has(data, "tuning")) {
            return null;
        }

        GameVehicleArcadeConfig c = new GameVehicleArcadeConfig();
        c.code = code;
        c.vehicleClass = VehicleTuningJson.Str(data, "class");
        c.tuning = VehicleTuningJson.ParseTuning(data["tuning"]);
        c.tuning.surfaces = VehicleTuningJson.ParseSurfaces(VehicleTuningJson.Has(data, "surfaces") ? data["surfaces"] : null, c.surfaces);
        c.control = VehicleTuningJson.ParseControl(VehicleTuningJson.Has(data, "control") ? data["control"] : null, c.tuning.wheelie);

        if (VehicleTuningJson.Has(data, "rider")) {
            c.ridePresets = VehicleTuningJson.Str(data["rider"], "ride_presets");
        }

        if (VehicleTuningJson.Has(data, "host")) {
            JsonData host = data["host"];
            string[] layers = Strings(host, "layers");

            if (layers.Length > 0) {
                c.layerMask = LayerMask.GetMask(layers);
            }

            if (VehicleTuningJson.Has(host, "visual")) {
                c.visual = VehicleTuningJson.Str(host, "visual");
            }

            c.wheels = Strings(host, "wheels");

            if (VehicleTuningJson.Has(host, "wheel_axis") && host["wheel_axis"].IsArray && host["wheel_axis"].Count == 3) {
                JsonData a = host["wheel_axis"];
                c.wheelAxis = new Vector3(VehicleTuningJson.Number(a[0], 0f), VehicleTuningJson.Number(a[1], 0f), VehicleTuningJson.Number(a[2], 0f));
            }
        }

        return c;
    }

    private static string[] Strings(JsonData d, string key) {

        if (!VehicleTuningJson.Has(d, key) || !d[key].IsArray) {
            return new string[0];
        }

        JsonData a = d[key];
        List<string> list = new List<string>();

        for (int i = 0; i < a.Count; i++) {
            if (a[i] != null && a[i].IsString) {
                list.Add((string)a[i]);
            }
        }

        return list.ToArray();
    }
}
#endif
