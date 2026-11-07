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
// - Ghosts: GameRaceGhostRecorder records the player's laps and GameRaceGhostPlayer chases the best one in a time trial;
//   both are called from here (a second seam next to onAfterStep, which stays free for probes) and inert off a race.
// - Statics: Bikes enters Play without a domain reload; the reset below runs again on every Play.
public static class GameRace {

    // Seams, all off by default.
    public static bool enabled;                     // the title turns the race on
    public static bool autopilotPlayer;             // tests: an AI drives the player's bike
    public static int playerSlotOverride = -1;      // tests: the player's grid slot (-1 = by rules)
    public static string[] qualifiedOrder;          // qualifying: the rider ids in grid-slot order (GameRaceQualifying), used by the next Begin then cleared
    public static int lapsOverride = -1;            // the lap count in place of the rules' (a qualifying lap = 1); -1 = off
    public static uint seedOverride;                // 0 = none
    public static int aiRidersOverride = -1;        // the AI rider count in place of the rules' (a solo time trial = 0); -1 = off
    public static string defaultVehicleCode = "vehicle-bike-dirt-1";   // when the rules name none
    public static string playerName = "Player";
    public static bool ghostEnabled;                // the time trial chases the player's best ghost (a race with no AI riders does too)
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
    public static string playerVehicleCode { get; private set; }
    public static int playerRespawns { get; private set; }     // times the player was put back on the track this race

    public static bool isDone {
        get {
            return active && session != null && session.state == RaceSession.done;
        }
    }

    private const int settleMaxSteps = 120;
    private const float settleSpeed = 0.2f;

    private static readonly Action<int> stepHandler = OnStep;
    private const float respawnClear = 3f;
    // Sideways tries for a taken respawn spot, as fractions of the half width: width/4 then 0.8 of width/2, either side.
    private static readonly float[] spreadFractions = { 0.5f, -0.5f, 0.8f, -0.8f };
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
    private static int lapTimesPerRacer;
    private static string[] racerCodes;
    private static string[] racerNames;
    private static string[] profileCodes;       // the AI profile code per racer, "" for the player
    private static Vector3[] slotPos;
    private static Quaternion slotRot;
    private static RaceRules coreRules;
    private static float lowestY;
    private static bool resultsMade;
    private static bool countdownWanted;
    private static int settleSteps;

    private static int[] pairA;
    private static int[] pairB;
    private static float[] dvA;
    private static float[] dvB;
    private static int pairCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {

        enabled = false;
        autopilotPlayer = false;
        playerSlotOverride = -1;
        qualifiedOrder = null;
        lapsOverride = -1;
        seedOverride = 0;
        aiRidersOverride = -1;
        ghostEnabled = false;
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

        if (aiRidersOverride >= 0) {
            rules.aiRiders = Mathf.Min(aiRidersOverride, 11);
        }

        if (lapsOverride > 0) {
            rules.laps = lapsOverride;
        }

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

        // A qualified grid is used once, and only when it is for this many riders and nothing pins the player's slot.
        string[] grid = qualifiedOrder;
        qualifiedOrder = null;

        if (grid != null && (grid.Length != n || playerSlotOverride >= 0)) {
            grid = null;
        }

        int player = playerSlotOverride;

        if (player < 0 && grid != null) {
            player = Array.IndexOf(grid, GameRaceQualifying.playerId);
        }

        if (player < 0) {
            player = rules.playerSlot;
        }

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
                row = grid != null ? GameRaceAiProfiles.Get(grid[i]) : null;

                if (row == null) {
                    row = GameRaceQualifying.AiRow(rules, ai);
                }

                ai++;
            }

            string code = row != null && !string.IsNullOrEmpty(row.vehicleCode) ? row.vehicleCode
                : !string.IsNullOrEmpty(rules.vehicleCode) ? rules.vehicleCode : defaultVehicleCode;
            string racerCode = isPlayer ? "player" : "ai-" + (row != null ? row.code : i.ToString());
            string racerName = isPlayer ? playerName : (row != null ? row.displayName : racerCode);

            // More AIs than profiles repeat a row: suffix the repeats so udids and names stay unique.
            string baseCode = racerCode;
            int repeat = 1;

            while (CodeTaken(racerCode, i)) {
                repeat++;
                racerCode = baseCode + "-" + repeat;
            }

            if (repeat > 1) {
                racerName = racerName + " " + repeat;
            }

            racerCodes[i] = racerCode;
            racerNames[i] = racerName;
            profileCodes[i] = row != null && !isPlayer ? row.code : "";

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

            // Riders touch through VehicleModel.Bump (circles). The solid bodies would wall each other off in the hull
            // sweep before the bump radius is reached, so the race bikes keep their triggers (mount, pickups) only.
            foreach (Collider body in go.GetComponentsInChildren<Collider>(true)) {
                if (!body.isTrigger) {
                    body.enabled = false;
                }
            }

            mounts[i] = mount;
            mount.arcade = arcade;
            arcades[i] = arcade;
            arcade.hold = true;

            if (isPlayer) {
                playerMount = mount;
                playerVehicleCode = code;
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
        countdownWanted = false;
        pairCount = 0;
        FillHud();
        GameVehicleArcadeStepper.onFixedStep = stepHandler;
        GameVehicleArcadeStepper.onBump = bumpHandler;
        GameVehicleArcadeStepper.enabled = true;
        playerRespawns = 0;
        active = true;

        // Ghosts: the recorder loads the track's file, the player spawns the best lap's bike where the mode wants it.
        GameLevel level = GameLevels.Current;
        GameRaceGhostRecorder.Begin(level != null ? level.code : "", track, playerVehicleCode);
        GameRaceGhostPlayer.Begin(container, playerVehicleCode);
        return true;
    }

    private static bool CodeTaken(string code, int before) {

        for (int j = 0; j < before; j++) {
            if (racerCodes[j] == code) {
                return true;
            }
        }

        return false;
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
        lapTimesPerRacer = Mathf.Max(1, rules.laps);
        lapTimes = new float[n * lapTimesPerRacer];
        int pairs = Mathf.Max(1, n * (n - 1) / 2);
        pairA = new int[pairs];
        pairB = new int[pairs];
        dvA = new float[pairs];
        dvB = new float[pairs];
        racerCodes = new string[n];
        racerNames = new string[n];
        profileCodes = new string[n];
        slotPos = new Vector3[n];
    }

    // The grid goes live: the countdown starts once every bike has settled on the ground (at most settleMaxSteps
    // after the call), so the first number is shown over bikes at rest.
    public static void StartCountdown() {

        if (!active || session.state != RaceSession.staged) {
            return;
        }

        countdownWanted = true;
        settleSteps = 0;
    }

    private static bool AllSettled() {

        for (int i = 0; i < racerCount; i++) {
            if ((arcades[i].state.flags & (int)VehicleFlags.grounded) == 0 || arcades[i].state.speed > settleSpeed) {
                return false;
            }
        }

        return true;
    }

    private static void BeginCountdown() {

        countdownWanted = false;
        session.Begin();

        for (int i = 0; i < racerCount; i++) {
            if (i != playerIndex && mounts[i] != null && mounts[i].riderSeat != null) {
                mounts[i].riderSeat.enabled = true;
            }
        }
    }

    public static void End() {

        // Ghosts first: the recorder writes this session's laps, the player's bike goes with the level.
        GameRaceGhostRecorder.End();
        GameRaceGhostPlayer.End();

        if (GameVehicleArcadeStepper.onFixedStep == stepHandler) {
            GameVehicleArcadeStepper.onFixedStep = null;
        }

        if (GameVehicleArcadeStepper.onBump == bumpHandler) {
            GameVehicleArcadeStepper.onBump = null;
        }

        // Only this race's own stepper: a race that never began must not switch off someone else's.
        if (active) {
            GameVehicleArcadeStepper.enabled = false;
        }

        if (arcades != null) {
            for (int i = 0; i < arcades.Length; i++) {
                if (arcades[i] != null) {
                    arcades[i].driverOverride = null;
                    arcades[i].controlModeOverride = -1;
                    arcades[i].hold = true;
                }
            }
        }

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
        pairA = null;
        pairB = null;
        dvA = null;
        dvB = null;
        racerCodes = null;
        racerNames = null;
        profileCodes = null;
        slotPos = null;
        playerMount = null;
        playerVehicleCode = null;
        track = null;
        session = null;
        racerCount = 0;
        playerIndex = 0;
        pairCount = 0;
        countdownWanted = false;
    }

    // ------------------------------------------------------------------ the stepper hooks

    private static void OnBump(GameVehicleArcade a, GameVehicleArcade b, float impulse, in VehicleEvents ea, in VehicleEvents eb) {

        if (!active || pairCount >= pairA.Length) {
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

        if (countdownWanted && session.state == RaceSession.staged && (++settleSteps >= settleMaxSteps || AllSettled())) {
            BeginCountdown();
        }

        int ev = session.Step();

        if ((ev & (int)RaceSessionEvents.go) != 0) {
            for (int i = 0; i < racerCount; i++) {
                arcades[i].hold = false;
            }
        }

        // The rules object is live (tests change it): the core copy follows the two distances.
        coreRules.missDistance = rules.missDistance;
        coreRules.wrongWayDistance = rules.wrongWayDistance;

        string state = session.state;

        if (state == RaceSession.running || state == RaceSession.finishing) {
            RiderAheadRule();
            HoldGrace();
            pairCount = 0;

            for (int i = 0; i < racerCount; i++) {
                StepRacer(i);
            }

            RacePlaces.Sort(racers, racerCount, order);

            if (AllFinished()) {
                session.AllFinished();
            }
            else if (state == RaceSession.running && racerCount > 1 && AllAiFinished()) {
                // The player never finished: the finish wait starts as if it had, and it is placed by progress.
                session.PlayerFinished();
            }
        }

        pairCount = 0;

        if (!resultsMade && session.state == RaceSession.done) {
            resultsMade = true;
            lastResults = BuildResults();
            RecordAiBestLaps();

            for (int i = 0; i < racerCount; i++) {
                arcades[i].hold = true;
            }
        }

        FillHud();
        GameRaceGhostRecorder.OnStep(step);
        GameRaceGhostPlayer.OnStep(step);

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

            // Signed gap along the track, wrapped: a lapped backmarker is behind whatever the lap counts say.
            float d = track.DeltaS(racers[b].s, racers[a].s);

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

    // A respawned rider keeps its bump grace while it still overlaps another rider: grace only expires once clear.
    // Two fixed steps, as the arcade takes one off at the start of the next step before the bumps look at it.
    private static void HoldGrace() {

        float hold = RaceSession.fixedDt * 2f;

        for (int i = 0; i < racerCount; i++) {

            GameVehicleArcade a = arcades[i];

            if (a.config == null || a.contactGrace <= 0f || a.contactGrace >= hold) {
                continue;
            }

            for (int j = 0; j < racerCount; j++) {

                GameVehicleArcade b = arcades[j];

                if (j == i || b.config == null || Finished(j)) {
                    continue;
                }

                float r = a.config.tuning.BumpRadius() + b.config.tuning.BumpRadius();
                float dx = b.state.position.x - a.state.position.x;
                float dz = b.state.position.z - a.state.position.z;

                if (dx * dx + dz * dz < r * r) {
                    a.contactGrace = hold;
                    break;
                }
            }
        }
    }

    // True when another racer stands within respawnClear metres (ground plane) of p.
    private static bool SpotTaken(int self, Vector3 p) {

        for (int j = 0; j < racerCount; j++) {

            if (j == self || arcades[j].config == null) {
                continue;
            }

            float dx = arcades[j].state.position.x - p.x;
            float dz = arcades[j].state.position.z - p.z;

            if (dx * dx + dz * dz < respawnClear * respawnClear) {
                return true;
            }
        }

        return false;
    }

    private static bool Finished(int i) {
        return (racers[i].flags & (int)RaceFlags.finished) != 0;
    }

    private static bool Bailed(int i) {
        return (arcades[i].state.flags & (int)VehicleFlags.bailed) != 0;
    }

    private static bool AllAiFinished() {

        for (int i = 0; i < racerCount; i++) {
            if (i != playerIndex && !Finished(i)) {
                return false;
            }
        }

        return true;
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

            if (lapIndex >= 0 && lapIndex < lapTimesPerRacer) {
                lapTimes[i * lapTimesPerRacer + lapIndex] = racers[i].lastLapTime;
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
            RaceProgress.Init(track, out racers[i], a.state.position);
        }
        else {
            RaceProgress.Respawn(track, ref racers[i]);
            float s = racers[i].s;
            Vector3 p = UnityFrame.ToUnity(track.PointAt(s));
            Vector3 t = UnityFrame.ToUnity(track.TangentAt(s));
            t.y = 0f;
            t = t.sqrMagnitude > 1e-8f ? t.normalized : Vector3.forward;

            // Another rider on the spot (bumped together, respawned together): slide sideways, inside the track.
            Vector3 right = Vector3.Cross(Vector3.up, t);
            float half = track.WidthAt(s) * 0.5f;

            for (int k = 0; k < spreadFractions.Length && SpotTaken(i, p); k++) {
                p = UnityFrame.ToUnity(track.PointAt(s)) + right * (half * spreadFractions[k]);
            }

            a.ResetAt(p + Vector3.up * rules.respawnLift,
                Quaternion.LookRotation(t.sqrMagnitude > 1e-8f ? t : Vector3.forward, Vector3.up));
        }

        if (i == playerIndex) {
            playerRespawns++;
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

            for (int l = 0; l < racers[i].lap && l < lapTimesPerRacer; l++) {
                player.lapTimes.Add(lapTimes[i * lapTimesPerRacer + l]);
            }

            results.players.Add(player);
        }

        return results;
    }

    // The AI riders' best laps of this race go to the track's ghost file (ai_best_laps): the next qualifying
    // takes them as the AIs' times. The recorder saves them with the player's laps.
    private static void RecordAiBestLaps() {
        for (int i = 0; i < racerCount; i++) {
            if (i != playerIndex) {
                GameRaceGhostRecorder.RecordAiBest(profileCodes[i], racers[i].bestLapTime);
            }
        }
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
