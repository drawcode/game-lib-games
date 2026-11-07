#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Core;
using Agnostic.Racing;
using Agnostic.Vehicle;

using Engine.AgnosticHost;
using Engine.Game.App;
using Engine.Game.App.BaseApp;

// What the race HUD polls each step (GameRace.hud). place is the player's, 1-based; lap is 1-based and clamped to laps.
public struct GameRaceHudState {

    public string state;      // RaceSession: staged, countdown, running, finishing, done
    public int countdown;     // 3, 2, 1 during the countdown, else 0
    public int place;
    public int racers;
    public int lap;
    public int laps;
    public float raceTime;
    public float lapTime;     // the current lap so far
    public float bestLap;     // 0 = none yet
    public float lastLap;     // 0 = none yet
    public int flags;         // the player's RaceFlags
}

// The race session (Action Bikes R2 step 7): the player and the AI riders on a grid, one countdown, laps over the
// track-ai-node line, places, respawns and results. It rides GameVehicleArcadeStepper: every arcade steps once per
// fixed step, the bumps run, then OnStep applies the race rules. Default off (enabled); the Bikes title turns it on
// (AppRaceConfigs) and its GameController calls Begin / StartCountdown / End. Nothing allocates per step.
//
// - Order per step: session clock, GO, the rider-ahead bump rule, every racer's progress and timers, places, done.
// - Frame: the track is in the core frame (UnityFrame), so lateral and a profile's line offset keep their sides.
// - Respawn: a bailed rider waits respawnDelay (its own timer, the core's bail timer padded to stay frozen), a missed
//   checkpoint waits missRespawn, an off-course rider offCourseTime, a fallen one at once; then it is put on its last
//   checkpoint facing the track, with contactGrace seconds of no bumps.
// - Statics: Bikes enters Play without a domain reload; the reset below runs again on every Play.
public static class GameRace {

    // Seams, all off by default.
    public static bool enabled;                     // the title turns the race on
    public static bool autopilotPlayer;             // tests: an AI drives the player's bike
    public static int playerSlotOverride = -1;      // tests: the player's grid slot (-1 = by rules)
    public static int qualifiedPlayerSlot = -1;     // qualifying: the slot it earned, used once then reset
    public static uint seedOverride;                // 0 = none
    public static string defaultVehicleCode = "vehicle-bike-dirt-1";   // when the rules name none
    public static string playerName = "Player";
    public static Action<int> onAfterStep;          // after the race rules each step (ghosts, probes); the stepper's step count

    public static bool active { get; private set; }
    public static GameRaceRules rules { get; private set; } = new GameRaceRules();
    public static RaceTrack track { get; private set; }
    public static RaceSession session { get; private set; }
    public static GameRaceResultsData lastResults { get; private set; }
    public static GameRaceHudState hud { get { return hudState; } }
    public static int racerCount { get; private set; }
    public static int playerIndex { get; private set; }
    public static GameObjectMountVehicle playerMount { get; private set; }

    public static bool isDone {
        get {
            return active && session != null && session.state == RaceSession.done;
        }
    }

    private const int pairMax = 15;

    private static readonly Action<int> stepHandler = OnStep;
    private static readonly GameVehicleArcadeStepper.BumpHandler bumpHandler = OnBump;

    private static GameRaceHudState hudState;
    private static GameVehicleArcade[] arcades;
    private static GameObjectMountVehicle[] mounts;
    private static GameRaceAiDriver[] drivers;
    private static GameObject[] roots;
    private static RaceRacer[] racers;
    private static int[] order;
    private static Vec3[] prev;
    private static float[] lateral;
    private static int[] respawnSteps;
    private static int[] missSteps;
    private static int[] offSteps;
    private static float[] lapTimes;
    private static string[] racerCodes;
    private static string[] racerNames;
    private static Vector3[] slotPos;
    private static Quaternion slotRot;
    private static RaceRules coreRules;
    private static float lowestY;
    private static bool resultsMade;

    private static readonly int[] pairA = new int[pairMax];
    private static readonly int[] pairB = new int[pairMax];
    private static readonly float[] dvA = new float[pairMax];
    private static readonly float[] dvB = new float[pairMax];
    private static int pairCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {

        enabled = false;
        autopilotPlayer = false;
        playerSlotOverride = -1;
        qualifiedPlayerSlot = -1;
        seedOverride = 0;
        defaultVehicleCode = "vehicle-bike-dirt-1";
        playerName = "Player";
        onAfterStep = null;
        Clear();
        active = false;
        rules = new GameRaceRules();
        lastResults = null;
        hudState = default(GameRaceHudState);
    }

    public static GameVehicleArcade Arcade(int i) {
        return arcades != null && i >= 0 && i < racerCount ? arcades[i] : null;
    }

    public static RaceRacer Racer(int i) {
        return racers != null && i >= 0 && i < racerCount ? racers[i] : default(RaceRacer);
    }

    public static Vector3 SlotPosition(int i) {
        return slotPos != null && i >= 0 && i < racerCount ? slotPos[i] : Vector3.zero;
    }

    // ------------------------------------------------------------------ Begin / End

    // Lays the race out in a loaded level's container. False (and nothing changed) when this is no race round:
    // the round then plays as a free ride.
    public static bool Begin(GameObject container) {

        if (!enabled || active || container == null || !GameRaceRules.IsRaceLevel()) {
            return false;
        }

        rules = GameRaceRules.FromCurrentLevel();
        GameDraggableLevelItem gridItem;
        track = GameRaceTrackBuilder.Build(container, rules.defaultWidth, out lowestY, out gridItem);

        if (track == null) {
            Debug.LogWarning("GameRace: the level has fewer than 3 track nodes, playing as a free ride");
            return false;
        }

        int n = 1 + rules.aiRiders;
        uint seed = seedOverride != 0 ? seedOverride : rules.seed != 0 ? rules.seed : (uint)Environment.TickCount | 1u;
        coreRules = rules.ToCore();

        Allocate(n);

        GameRaceGridSpec spec = GameRaceGridSpec.Defaults();
        Vector3 origin;

        if (gridItem != null) {
            spec = GameRaceGrid.Read(gridItem.gameLevelItemAsset);
            origin = gridItem.transform.position;
            slotRot = Quaternion.Euler(0f, gridItem.transform.eulerAngles.y, 0f);
        }
        else {
            spec.back = 4f;
            origin = UnityFrame.ToUnity(track.Node(0).position);
            Vector3 along = UnityFrame.ToUnity(track.NodeTangent(0));
            along.y = 0f;
            slotRot = Quaternion.LookRotation(along.sqrMagnitude > 1e-8f ? along : Vector3.forward, Vector3.up);
        }

        GameRaceGrid.Slots(origin, slotRot, spec, slotPos);

        int player = playerSlotOverride;

        if (player < 0) {
            player = qualifiedPlayerSlot;
        }

        qualifiedPlayerSlot = -1;

        if (player < 0) {
            player = rules.playerSlot;
        }

        if (player < 0) {
            player = GameRaceGrid.PickPlayerSlot(n, seed);
        }

        playerIndex = Mathf.Clamp(player, 0, n - 1);
        racerCount = n;
        int ai = 0;

        for (int i = 0; i < n; i++) {

            bool isPlayer = i == playerIndex;
            GameRaceAiProfiles.Row row = null;

            if (!isPlayer) {
                row = rules.aiProfiles.Length > 0 ? GameRaceAiProfiles.Get(rules.aiProfiles[ai % rules.aiProfiles.Length]) : null;

                if (row == null) {
                    row = GameRaceAiProfiles.At(ai);
                }

                ai++;
            }

            string code = row != null && !string.IsNullOrEmpty(row.vehicleCode) ? row.vehicleCode
                : !string.IsNullOrEmpty(rules.vehicleCode) ? rules.vehicleCode : defaultVehicleCode;
            racerCodes[i] = isPlayer ? "player" : "ai-" + (row != null ? row.code : i.ToString());
            racerNames[i] = isPlayer ? playerName : (row != null ? row.displayName : racerCodes[i]);

            GameObject go = GameDraggableEditor.LoadSprite(container, code, Vector3.one);

            if (go == null) {
                Debug.LogWarning("GameRace: vehicle " + code + " not found");
                End();
                return false;
            }

            roots[i] = go;
            go.transform.SetPositionAndRotation(slotPos[i] + Vector3.up * rules.respawnLift, slotRot);
            GameObjectMountVehicle mount = go.GetComponentInChildren<GameObjectMountVehicle>(true);
            GameVehicleArcade arcade = mount != null ? GameVehicleArcade.Attach(mount.vehicleObject) as GameVehicleArcade : null;

            if (arcade == null) {
                Debug.LogWarning("GameRace: vehicle " + code + " has no mount or arcade tuning");
                End();
                return false;
            }

            mounts[i] = mount;
            mount.arcade = arcade;
            arcades[i] = arcade;
            arcade.hold = true;

            if (isPlayer) {
                playerMount = mount;
            }

            if (!isPlayer || autopilotPlayer) {
                RaceAiProfile profile = row != null ? row.profile : RaceAiProfile.Defaults();

                if (isPlayer) {
                    profile = RaceAiProfile.Defaults();
                    profile.noise = 0f;
                    profile.aggression = 0f;
                }

                GameRaceAiDriver driver = new GameRaceAiDriver(i, profile, seed ^ (uint)(i + 1) * 0x9E3779B1u);
                drivers[i] = driver;
                arcade.driverOverride = driver;
                arcade.controlModeOverride = (int)ControlMode.heading;
                arcade.ResolveControl();
            }

            RaceProgress.Init(track, out racers[i], arcade.state.position);
            prev[i] = arcade.state.position;
            RaceProjection pr;
            track.Project(prev[i], racers[i].seg, out pr);
            lateral[i] = pr.lateral;

            if (racers[i].progress >= 0f) {
                Debug.LogWarning("GameRace: slot " + i + " is past the start line (progress " + racers[i].progress + ")");
            }
        }

        session = new RaceSession(rules.countdown, rules.finishWait);
        lastResults = null;
        resultsMade = false;
        pairCount = 0;
        FillHud();
        GameVehicleArcadeStepper.onFixedStep = stepHandler;
        GameVehicleArcadeStepper.onBump = bumpHandler;
        GameVehicleArcadeStepper.enabled = true;
        active = true;
        return true;
    }

    private static void Allocate(int n) {

        arcades = new GameVehicleArcade[n];
        mounts = new GameObjectMountVehicle[n];
        drivers = new GameRaceAiDriver[n];
        roots = new GameObject[n];
        racers = new RaceRacer[n];
        order = new int[n];
        prev = new Vec3[n];
        lateral = new float[n];
        respawnSteps = new int[n];
        missSteps = new int[n];
        offSteps = new int[n];
        lapTimes = new float[n * Mathf.Max(1, rules.laps)];
        racerCodes = new string[n];
        racerNames = new string[n];
        slotPos = new Vector3[n];
    }

    // The grid goes live: the countdown starts, and the AI riders' seat riders show.
    public static void StartCountdown() {

        if (!active || session.state != RaceSession.staged) {
            return;
        }

        session.Begin();

        for (int i = 0; i < racerCount; i++) {
            if (i != playerIndex && mounts[i] != null && mounts[i].riderSeat != null) {
                mounts[i].riderSeat.enabled = true;
            }
        }

        FillHud();
    }

    public static void End() {

        if (GameVehicleArcadeStepper.onFixedStep == stepHandler) {
            GameVehicleArcadeStepper.onFixedStep = null;
        }

        if (GameVehicleArcadeStepper.onBump == bumpHandler) {
            GameVehicleArcadeStepper.onBump = null;
        }

        GameVehicleArcadeStepper.enabled = false;

        if (roots != null) {
            for (int i = 0; i < roots.Length; i++) {
                if (roots[i] != null) {
                    UnityEngine.Object.Destroy(roots[i]);
                }
            }
        }

        Clear();
        active = false;
    }

    private static void Clear() {

        arcades = null;
        mounts = null;
        drivers = null;
        roots = null;
        racers = null;
        order = null;
        prev = null;
        lateral = null;
        respawnSteps = null;
        missSteps = null;
        offSteps = null;
        lapTimes = null;
        racerCodes = null;
        racerNames = null;
        slotPos = null;
        playerMount = null;
        track = null;
        session = null;
        racerCount = 0;
        playerIndex = 0;
        pairCount = 0;
    }

    // ------------------------------------------------------------------ the stepper hooks

    private static void OnBump(GameVehicleArcade a, GameVehicleArcade b, float impulse, in VehicleEvents ea, in VehicleEvents eb) {

        if (!active || pairCount >= pairMax) {
            return;
        }

        int ia = IndexOf(a);
        int ib = IndexOf(b);

        if (ia < 0 || ib < 0) {
            return;
        }

        pairA[pairCount] = ia;
        pairB[pairCount] = ib;
        dvA[pairCount] = ea.bumpDv;
        dvB[pairCount] = eb.bumpDv;
        pairCount++;
    }

    private static int IndexOf(GameVehicleArcade a) {

        for (int i = 0; i < racerCount; i++) {
            if (arcades[i] == a) {
                return i;
            }
        }

        return -1;
    }

    // After every arcade's step and the bumps of one fixed step.
    private static void OnStep(int step) {

        if (!active) {
            return;
        }

        int ev = session.Step();

        if ((ev & (int)RaceSessionEvents.go) != 0) {
            for (int i = 0; i < racerCount; i++) {
                arcades[i].hold = false;
            }
        }

        string state = session.state;

        if (state == RaceSession.running || state == RaceSession.finishing) {
            RiderAheadRule();
            pairCount = 0;

            for (int i = 0; i < racerCount; i++) {
                StepRacer(i);
            }

            RacePlaces.Sort(racers, racerCount, order);

            if (AllFinished()) {
                session.AllFinished();
            }
        }

        pairCount = 0;

        if (!resultsMade && session.state == RaceSession.done) {
            resultsMade = true;
            lastResults = BuildResults();

            for (int i = 0; i < racerCount; i++) {
                arcades[i].hold = true;
            }
        }

        FillHud();

        if (onAfterStep != null) {
            onAfterStep(step);
        }
    }

    // A bump knocks the rider ahead off, not the one behind: it is the one that was hit from behind or cut up.
    private static void RiderAheadRule() {

        if (rules.bumpAheadFloor <= 0f) {
            return;
        }

        for (int k = 0; k < pairCount; k++) {

            int a = pairA[k];
            int b = pairB[k];

            if (Finished(a) || Finished(b) || Bailed(a) || Bailed(b)) {
                continue;
            }

            float d = racers[a].progress - racers[b].progress;

            if (Mathf.Abs(d) < rules.bumpAheadMin) {
                continue;
            }

            int ahead = d > 0f ? a : b;
            float dv = d > 0f ? dvA[k] : dvB[k];

            if (dv > rules.bumpAheadFloor) {
                GameVehicleArcade arc = arcades[ahead];
                VehicleModel.Bail(ref arc.state, in arc.config.tuning, ref arc.stepEvents);
                arc.frameEvents.flags |= (int)VehicleEventFlags.bailed;
            }
        }
    }

    private static bool Finished(int i) {
        return (racers[i].flags & (int)RaceFlags.finished) != 0;
    }

    private static bool Bailed(int i) {
        return (arcades[i].state.flags & (int)VehicleFlags.bailed) != 0;
    }

    private static bool AllFinished() {

        for (int i = 0; i < racerCount; i++) {
            if (!Finished(i)) {
                return false;
            }
        }

        return true;
    }

    private static void StepRacer(int i) {

        if (Finished(i)) {
            return;
        }

        GameVehicleArcade a = arcades[i];

        if ((a.stepEvents.flags & (int)VehicleEventFlags.bailed) != 0 && respawnSteps[i] == 0) {
            respawnSteps[i] = Mathf.Max(1, Mathf.RoundToInt(rules.respawnDelay * RaceSession.stepsPerSecond));
            a.state.bailTimer = rules.respawnDelay + 1f;
        }

        Vec3 pos = a.state.position;
        int rev = RaceProgress.Step(track, ref racers[i], in coreRules, prev[i], pos, session.raceTime, RaceSession.fixedDt);
        prev[i] = pos;

        if ((rev & (int)RaceEvents.lap) != 0) {
            int lapIndex = racers[i].lap - 1;

            if (lapIndex >= 0 && i * rules.laps + lapIndex < lapTimes.Length) {
                lapTimes[i * rules.laps + lapIndex] = racers[i].lastLapTime;
            }
        }

        if ((rev & (int)RaceEvents.finished) != 0) {
            if (i == playerIndex) {
                session.PlayerFinished();
            }

            return;
        }

        if ((rev & (int)RaceEvents.missed) != 0) {
            missSteps[i] = Mathf.Max(1, Mathf.RoundToInt(rules.missRespawn * RaceSession.stepsPerSecond));
        }

        RaceProjection pr;
        track.Project(pos, racers[i].seg, out pr);
        lateral[i] = pr.lateral;

        if (Mathf.Abs(pr.lateral) > rules.offCourseFactor * track.WidthAt(pr.s)) {
            offSteps[i]++;
        }
        else {
            offSteps[i] = 0;
        }

        bool respawn = false;

        if (respawnSteps[i] > 0 && --respawnSteps[i] == 0) {
            respawn = true;
        }

        if (missSteps[i] > 0 && --missSteps[i] == 0) {
            respawn = true;
        }

        if (offSteps[i] >= Mathf.RoundToInt(rules.offCourseTime * RaceSession.stepsPerSecond)) {
            respawn = true;
        }

        if (pos.y < lowestY - rules.fallDepth) {
            respawn = true;
        }

        if (respawn) {
            Respawn(i);
        }
    }

    // Back on the track: the grid slot before the line was ever crossed, else the last checkpoint facing the way on.
    private static void Respawn(int i) {

        GameVehicleArcade a = arcades[i];

        if ((racers[i].flags & (int)RaceFlags.started) == 0) {
            a.ResetAt(slotPos[i] + Vector3.up * rules.respawnLift, slotRot);
        }
        else {
            RaceProgress.Respawn(track, ref racers[i]);
            float s = racers[i].s;
            Vector3 p = UnityFrame.ToUnity(track.PointAt(s));
            Vector3 t = UnityFrame.ToUnity(track.TangentAt(s));
            t.y = 0f;
            a.ResetAt(p + Vector3.up * rules.respawnLift,
                Quaternion.LookRotation(t.sqrMagnitude > 1e-8f ? t : Vector3.forward, Vector3.up));
        }

        a.contactGrace = rules.contactGrace;
        prev[i] = a.state.position;
        respawnSteps[i] = 0;
        missSteps[i] = 0;
        offSteps[i] = 0;

        if (drivers[i] != null) {
            drivers[i].ResetMemory();
        }
    }

    // The nearest other unfinished racer, from the cached arrays of the last step.
    internal static void NearestRival(int self, out RaceRival rival) {

        rival = default(RaceRival);

        if (racers == null) {
            return;
        }

        float best = float.MaxValue;

        for (int j = 0; j < racerCount; j++) {

            if (j == self || Finished(j)) {
                continue;
            }

            float dx = prev[j].x - prev[self].x;
            float dz = prev[j].z - prev[self].z;
            float d = dx * dx + dz * dz;

            if (d < best) {
                best = d;
                rival.present = 1;
                rival.position = prev[j];
                rival.s = racers[j].s;
                rival.lateral = lateral[j];
            }
        }
    }

    // ------------------------------------------------------------------ results and HUD

    private static GameRaceResultsData BuildResults() {

        GameRaceResultsData results = new GameRaceResultsData();
        GameLevel level = GameLevels.Current;
        string levelCode = level != null ? level.code : "";

        for (int p = 0; p < racerCount; p++) {

            int i = order[p];
            GameRaceResultPlayer player = new GameRaceResultPlayer();
            player.udid = racerCodes[i];
            player.levelName = levelCode;
            player.playerName = racerNames[i];
            player.playerType = i == playerIndex ? GamePlayerType.PLAYER_HUMAN : GamePlayerType.PLAYER_AI;
            player.place = racers[i].place;
            player.totalTime = Finished(i) ? racers[i].finishTime : session.raceTime;

            for (int l = 0; l < racers[i].lap && l < rules.laps; l++) {
                player.lapTimes.Add(lapTimes[i * rules.laps + l]);
            }

            results.players.Add(player);
        }

        return results;
    }

    private static void FillHud() {

        RaceRacer r = racers[playerIndex];
        hudState.state = session.state;
        hudState.countdown = session.countdownNumber;
        hudState.place = r.place > 0 ? r.place : 1;
        hudState.racers = racerCount;
        hudState.lap = Mathf.Min(rules.laps, r.lap + 1);
        hudState.laps = rules.laps;
        hudState.raceTime = session.raceTime;
        hudState.lapTime = (r.flags & (int)RaceFlags.started) != 0 && (r.flags & (int)RaceFlags.finished) == 0
            ? Mathf.Max(0f, session.raceTime - r.lapStartTime) : 0f;
        hudState.bestLap = r.bestLapTime;
        hudState.lastLap = r.lastLapTime;
        hudState.flags = r.flags;
    }
}
#endif
