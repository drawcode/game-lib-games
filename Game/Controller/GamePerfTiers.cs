using System;

using Agnostic.Core;

using Engine.AgnosticHost;

using UnityEngine;
using UnityEngine.SceneManagement;

// Adaptive performance tiers through the agnostic core's PerfGovernor (perf-tiers.v1). Design:
// contexts/gamedev/agnostic/context-agnostic-perf-tiers.md.
//
// A game opts in by shipping Resources/agnostic/perf-tiers.json. At boot the device is classified
// to a ceiling tier (memory, cores, shader level, GPU denylist) and the round starts there; during
// a round the governor steps down on sustained slow frames or heat and probes back up. Menus and
// loads are held, and a level load clears the frame window, so neither counts against a tier.
//
// With no asset, or with the switch off, nothing is created and every quality setting stays as
// authored, so other games on this lib are unaffected.
public static class GamePerfTiers {

    public const string resourcePath = "agnostic/perf-tiers";

    // Kill switch. Flip it at this source default (a runtime flip lands after boot); false leaves
    // every camera and quality setting as authored.
    public static bool enabled = true;

    public static UnityPerfTiers driver;

    public static bool active {
        get {
            return enabled && driver != null;
        }
    }

    public static string tierId {
        get {
            return active ? driver.activeTierId : "";
        }
    }

    // Domain reload is off in this project, so statics outlive a play session while the driver's
    // GameObject does not. Rebuild everything on every boot.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot() {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        driver = null;

        if (!enabled) {
            return;
        }

        PerfDef def = PerfTiersJson.LoadResource(resourcePath, new UnityLog());

        if (def == null || def.tiers.Count == 0) {
            return;
        }

        driver = UnityPerfTiers.Create(def, null, "_GamePerfTiers");
        driver.holdWhen = NotInRound;
        SceneManager.sceneLoaded += OnSceneLoaded;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PerfDevice d = driver.device;
        Debug.Log("[perf] " + d.platform + " " + d.model + " / " + d.gpu + " mem " + d.memoryMB + " MB, "
            + d.cores + " cores, SM " + d.shaderLevel + " -> ceiling " + def.tiers[driver.governor.ceiling].id);
#endif
    }

    // The cameras that draw the level (CameraMain). Their renderer follows the tier's `ao`.
    public static void RegisterWorldCamera(Camera cam) {

        if (active) {
            driver.RegisterWorldCamera(cam);
        }
    }

    // A settings-menu choice: cap at this tier id, or "" for automatic.
    public static void SetUserCap(string tier) {

        if (active) {
            driver.governor.SetUserCap(string.IsNullOrEmpty(tier) ? -1 : driver.def.IndexOf(tier));
            driver.Apply();
        }
    }

    private static bool NotInRound() {
        return !GameConfigs.isGameRunning;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        if (active) {
            driver.ResetWindow();
        }
    }
}
