#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using Agnostic.Framing;
using Agnostic.Vehicle;

using Engine.AgnosticHost;
using Engine.Data.Json;

using UnityEngine;

// What GameVehicleArcade needs from one game-vehicle-data row (arcade design section 3, riders doc section 4.3).
public class GameVehicleArcadeConfig {

    public string code;
    public string vehicleClass;
    public VehicleTuning tuning;                 // data.tuning, with data.surfaces folded in
    public DriveAssistTuning assist;             // data.assist (D20); a missing block is enabled 0
    public ControlTuning control;                // data.control; mode is the class default (D8)
    public List<string> surfaces = new List<string>();
    public int layerMask = Physics.DefaultRaycastLayers;   // data.host.layers
    public string ridePresets = "";              // data.rider.ride_presets: animation-ride-<slot>-<this>
    public string visual = "vehicle";            // data.host.visual: the child that leans and pitches
    public string[] wheels = new string[0];      // data.host.wheels: wheel bones in probe order
    public Vector3 wheelAxis = Vector3.right;    // data.host.wheel_axis: local spin axis, sign = forward roll
    public FrameTuning camera = FrameTuning.Defaults();   // data.camera (D21): enabled 0 without the block
    public string cameraMode = GameCameraModes.follow;    // data.camera.mode: the default until the player picks
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
        c.assist = VehicleTuningJson.ParseAssist(VehicleTuningJson.Has(data, "assist") ? data["assist"] : null);

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

        if (VehicleTuningJson.Has(data, "camera")) {
            c.camera = ParseFrame(data["camera"]);
            string mode = VehicleTuningJson.Str(data["camera"], "mode");
            c.cameraMode = mode == GameCameraModes.iso ? GameCameraModes.iso : GameCameraModes.follow;
        }

        return c;
    }

    // data.camera (D21 plan section 4). A missing block is FrameTuning.Defaults() (enabled 0); a block without
    // "enabled" is on. Keys are the FrameTuning field names, angles as <name>Deg. yawDeg is the Unity euler y of the
    // camera (45 = looking toward +X +Z), so the core yaw is its negative. Insets are an "inset" object {l, r, t, b}.
    // projection is "ortho" or "perspective".
    public static FrameTuning ParseFrame(JsonData n) {

        FrameTuning t = FrameTuning.Defaults();

        if (n == null || !n.IsObject) {
            return t;
        }

        t.enabled = VehicleTuningJson.Num(n, "enabled", 1f) > 0.5f ? 1 : 0;
        t.projection = VehicleTuningJson.Str(n, "projection") == "perspective" ? FrameTuning.perspective : FrameTuning.orthographic;
        t.yaw = -VehicleTuningJson.Num(n, "yawDeg", -t.yaw / VehicleMath.degToRad) * VehicleMath.degToRad;
        t.pitch = VehicleTuningJson.Num(n, "pitchDeg", t.pitch / VehicleMath.degToRad) * VehicleMath.degToRad;
        t.pitchMax = VehicleTuningJson.Num(n, "pitchMaxDeg", t.pitchMax / VehicleMath.degToRad) * VehicleMath.degToRad;
        t.fov = VehicleTuningJson.Num(n, "fovDeg", t.fov / VehicleMath.degToRad) * VehicleMath.degToRad;
        t.sizeMin = VehicleTuningJson.Num(n, "sizeMin", t.sizeMin);
        t.sizeMax = VehicleTuningJson.Num(n, "sizeMax", t.sizeMax);
        t.freeRideSize = VehicleTuningJson.Num(n, "freeRideSize", t.freeRideSize);
        t.distMin = VehicleTuningJson.Num(n, "distMin", t.distMin);
        t.distMax = VehicleTuningJson.Num(n, "distMax", t.distMax);
        t.backOff = VehicleTuningJson.Num(n, "backOff", t.backOff);
        t.near = VehicleTuningJson.Num(n, "near", t.near);
        t.far = VehicleTuningJson.Num(n, "far", t.far);

        if (VehicleTuningJson.Has(n, "inset")) {
            JsonData i = n["inset"];
            t.insetLeft = VehicleTuningJson.Num(i, "l", t.insetLeft);
            t.insetRight = VehicleTuningJson.Num(i, "r", t.insetRight);
            t.insetTop = VehicleTuningJson.Num(i, "t", t.insetTop);
            t.insetBottom = VehicleTuningJson.Num(i, "b", t.insetBottom);
        }

        t.aheadBase = VehicleTuningJson.Num(n, "aheadBase", t.aheadBase);
        t.aheadTime = VehicleTuningJson.Num(n, "aheadTime", t.aheadTime);
        t.aheadMin = VehicleTuningJson.Num(n, "aheadMin", t.aheadMin);
        t.aheadMax = VehicleTuningJson.Num(n, "aheadMax", t.aheadMax);
        t.sampleStep = VehicleTuningJson.Num(n, "sampleStep", t.sampleStep);
        t.focusBias = VehicleTuningJson.Num(n, "focusBias", t.focusBias);
        t.edgeShare = VehicleTuningJson.Num(n, "edgeShare", t.edgeShare);
        t.bikeRadius = VehicleTuningJson.Num(n, "bikeRadius", t.bikeRadius);
        t.rivalsInFrame = VehicleTuningJson.Num(n, "rivalsInFrame", t.rivalsInFrame) > 0.5f ? 1 : 0;
        t.rivalRange = VehicleTuningJson.Num(n, "rivalRange", t.rivalRange);
        t.airLookTime = VehicleTuningJson.Num(n, "airLookTime", t.airLookTime);
        t.focusTau = VehicleTuningJson.Num(n, "focusTau", t.focusTau);
        t.focusYTau = VehicleTuningJson.Num(n, "focusYTau", t.focusYTau);
        t.focusYAirTau = VehicleTuningJson.Num(n, "focusYAirTau", t.focusYAirTau);
        t.zoomOutTau = VehicleTuningJson.Num(n, "zoomOutTau", t.zoomOutTau);
        t.zoomInTau = VehicleTuningJson.Num(n, "zoomInTau", t.zoomInTau);
        t.deadBand = VehicleTuningJson.Num(n, "deadBand", t.deadBand);
        t.pitchRaiseTau = VehicleTuningJson.Num(n, "pitchRaiseTau", t.pitchRaiseTau);
        t.pitchRelaxTau = VehicleTuningJson.Num(n, "pitchRelaxTau", t.pitchRelaxTau);
        t.occlusionMargin = VehicleTuningJson.Num(n, "occlusionMargin", t.occlusionMargin);
        t.snapDistance = VehicleTuningJson.Num(n, "snapDistance", t.snapDistance);
        t.switchBlend = VehicleTuningJson.Num(n, "switchBlend", t.switchBlend);
        return t;
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
