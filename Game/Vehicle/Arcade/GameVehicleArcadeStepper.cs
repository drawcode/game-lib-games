#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using Agnostic.Core;
using Agnostic.Vehicle;

using UnityEngine;

// One fixed-step clock for every arcade vehicle (Action Bikes R2). Each GameVehicleArcade normally steps itself
// from its own Update, so two vehicles are never stepped together and VehicleModel.Bump (vehicle vs vehicle) cannot
// run in phase. With enabled set, a hidden runner (created on first registration) ticks one GameClock and per fixed
// step steps every registered arcade, then Bumps each pair, then calls onFixedStep (the race hooks in there); after
// the steps every arcade poses and plays its clips (EndFrame).
//
// - Default off: nothing here runs, and each arcade's Update behaves as it always did. Toggling enabled hands over
//   within a frame: an arcade skips its own Update only on a frame the runner drove, and the stepper clock resets
//   when it turns on, so there is no double step and no catch-up burst.
// - Bump: pairs of configured arcades, skipped while either has contactGrace; the bump's events merge into both
//   vehicles' stepEvents and frameEvents.
// - No allocation per step: a preallocated list, for loops, no closures.
// - Statics: Bikes enters Play without a domain reload; the reset below runs again on every Play.
public static class GameVehicleArcadeStepper {

    public static bool enabled;
    public static System.Action<int> onFixedStep;   // after every arcade's step and the bumps; the stepper's step count

    private static readonly List<GameVehicleArcade> arcades = new List<GameVehicleArcade>(16);
    private static Runner runner;
    private static int drivenFrame = -1;

    public static int count {
        get {
            return arcades.Count;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() {
        enabled = false;
        onFixedStep = null;
        arcades.Clear();
        runner = null;
        drivenFrame = -1;
    }

    public static void Add(GameVehicleArcade arcade) {

        if (arcade == null || arcades.Contains(arcade)) {
            return;
        }

        arcades.Add(arcade);

        if (runner == null && Application.isPlaying) {
            GameObject go = new GameObject("GameVehicleArcadeStepper");
            go.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(go);
            runner = go.AddComponent<Runner>();
        }
    }

    public static void Remove(GameVehicleArcade arcade) {
        arcades.Remove(arcade);
    }

    // True when the runner already stepped this arcade this frame.
    public static bool Drives(GameVehicleArcade arcade) {
        return drivenFrame == Time.frameCount && arcades.Contains(arcade);
    }

    [DefaultExecutionOrder(-60)]   // before the arcades' own Update
    private class Runner : MonoBehaviour {

        private readonly GameClock clock = new GameClock();
        private bool wasEnabled;
        private int stepCount;

        private void Update() {

            if (!enabled) {
                wasEnabled = false;
                return;
            }

            if (!wasEnabled) {
                wasEnabled = true;
                clock.fixedDt = GameVehicleArcade.fixedDt;
                clock.Reset();
            }

            drivenFrame = Time.frameCount;
            clock.timeScale = Time.timeScale;
            clock.paused = !GameConfigs.isGameRunning || GameConfigs.isGamePaused;
            int steps = clock.Tick(Time.unscaledDeltaTime);

            for (int i = 0; i < arcades.Count; i++) {
                arcades[i].frameEvents = default(VehicleEvents);
            }

            for (int s = 0; s < steps; s++) {

                for (int i = 0; i < arcades.Count; i++) {
                    GameVehicleArcade a = arcades[i];

                    if (a.config != null) {
                        a.StepOnce(a.CameraYaw());
                    }
                }

                for (int i = 0; i < arcades.Count; i++) {
                    GameVehicleArcade a = arcades[i];

                    if (a.config == null || a.contactGrace > 0f) {
                        continue;
                    }

                    for (int j = i + 1; j < arcades.Count; j++) {
                        GameVehicleArcade b = arcades[j];

                        if (b.config == null || b.contactGrace > 0f) {
                            continue;
                        }

                        VehicleEvents ea = default(VehicleEvents);
                        VehicleEvents eb = default(VehicleEvents);
                        VehicleModel.Bump(ref a.state, in a.config.tuning, ref ea, ref b.state, in b.config.tuning, ref eb);
                        GameVehicleArcade.Merge(ref a.stepEvents, in ea);
                        GameVehicleArcade.Merge(ref a.frameEvents, in ea);
                        GameVehicleArcade.Merge(ref b.stepEvents, in eb);
                        GameVehicleArcade.Merge(ref b.frameEvents, in eb);
                    }
                }

                stepCount++;

                if (onFixedStep != null) {
                    onFixedStep(stepCount);
                }
            }

            for (int i = 0; i < arcades.Count; i++) {
                GameVehicleArcade a = arcades[i];

                if (a.config != null) {
                    a.EndFrame(clock.fixedAlpha);
                }
            }
        }
    }
}
#endif
