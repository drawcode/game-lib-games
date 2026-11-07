#if USE_AGNOSTIC_VEHICLE
using System;

using UnityEngine;

using Agnostic.Core;
using Agnostic.Racing;
using Agnostic.Vehicle;

using Engine.AgnosticHost;

// Records the player's laps while a race is active (Action Bikes R2 step 10; owner decision D6: player laps only).
// GameRace calls OnStep after the race rules of every fixed step. A lap runs from one crossing of the line to the
// next (the first crossing starts lap 1), sampled every 3rd fixed step = 20 Hz. A lap with a respawn in it is kept
// but marked "clean": false. Nothing allocates per step: the column buffers are made once for a 5 minute lap and
// only grow past that; a closed lap copies them into arrays of its own length.
public static class GameRaceGhostRecorder {

    public const int hz = 20;
    public const int stepsPerSample = 3;
    private const int startCapacity = 5 * 60 * hz;
    private const int maxSamples = 60 * 60 * hz;

    // The file being filled (the track's, loaded in Begin) and the lap that opened this step.
    public static GameRaceGhostFile file { get; private set; }
    public static string trackHash { get; private set; }
    public static bool lapStarted { get; private set; }
    public static int lapsRecorded { get; private set; }

    private static string levelCode;
    private static string vehicleCode;
    private static bool recording;
    private static bool dirty;
    private static bool savedAtDone;
    private static bool wasStarted;
    private static bool lapOpen;
    private static int lastLap;
    private static int respawnMark;
    private static int stepInLap;
    private static int count;
    private static float[][] buffers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        file = null;
        trackHash = null;
        buffers = null;
        Reset();
    }

    private static void Reset() {
        levelCode = null;
        vehicleCode = null;
        recording = false;
        dirty = false;
        savedAtDone = false;
        wasStarted = false;
        lapOpen = false;
        lapStarted = false;
        lapsRecorded = 0;
        lastLap = 0;
        stepInLap = 0;
        count = 0;
    }

    public static bool isRecording {
        get {
            return recording;
        }
    }

    public static bool hasUnsaved {
        get {
            return dirty;
        }
    }

    // Race begun: loads the track's file. A level without a code records nothing.
    public static void Begin(string code, RaceTrack track, string vehicle) {

        Reset();

        if (string.IsNullOrEmpty(code) || track == null) {
            return;
        }

        levelCode = code;
        vehicleCode = vehicle ?? "";
        trackHash = GameRaceGhostStore.TrackHash(track);
        file = GameRaceGhostStore.Load(code);
        dirty = file.recovered;
        recording = true;

        if (buffers == null) {
            buffers = new float[GameRaceGhostLap.columnCount][];

            for (int c = 0; c < buffers.Length; c++) {
                buffers[c] = new float[startCapacity];
            }
        }
    }

    // Writes the file when this session recorded a lap (or recovered one). Called at the race's end and on quit.
    public static void Save() {

        if (!recording || !dirty || file == null) {
            return;
        }

        dirty = !GameRaceGhostStore.Save(levelCode, file);
    }

    public static void End() {

        Save();
        file = null;
        trackHash = null;
        Reset();
    }

    public static void OnStep(int step) {

        lapStarted = false;

        if (!recording) {
            return;
        }

        int index = GameRace.playerIndex;
        RaceRacer r = GameRace.Racer(index);
        GameVehicleArcade arcade = GameRace.Arcade(index);

        if (arcade == null || (r.flags & (int)RaceFlags.started) == 0) {
            return;
        }

        if (!wasStarted) {
            wasStarted = true;
            lastLap = r.lap;
            OpenLap();
        }
        else if (r.lap != lastLap) {
            lastLap = r.lap;

            if (lapOpen) {
                CloseLap(r.lastLapTime, arcade);
            }

            if ((r.flags & (int)RaceFlags.finished) == 0) {
                OpenLap();
            }
        }

        // The race is over: write the session's laps now (End writes any that are left).
        if (dirty && !savedAtDone && GameRace.isDone) {
            savedAtDone = true;
            Save();
        }

        if (!lapOpen) {
            return;
        }

        if (stepInLap % stepsPerSample == 0) {
            Sample(r, arcade);
        }

        stepInLap++;
    }

    private static void OpenLap() {
        lapOpen = true;
        lapStarted = true;
        count = 0;
        stepInLap = 0;
        respawnMark = GameRace.playerRespawns;
    }

    private static void Sample(RaceRacer r, GameVehicleArcade arcade) {

        if (count >= maxSamples) {
            return;
        }

        if (count >= buffers[0].Length) {
            for (int c = 0; c < buffers.Length; c++) {
                Array.Resize(ref buffers[c], buffers[c].Length * 2);
            }
        }

        VehicleState s = arcade.state;
        Vector3 p = UnityFrame.ToUnity(s.position);
        Vector3 up = UnityFrame.ToUnity(s.up);
        Sticks sticks = arcade.stepSticks;
        int i = count++;

        buffers[GameRaceGhostLap.colT][i] = Mathf.Max(0f, GameRace.session.raceTime - r.lapStartTime);
        buffers[GameRaceGhostLap.colX][i] = p.x;
        buffers[GameRaceGhostLap.colY][i] = p.y;
        buffers[GameRaceGhostLap.colZ][i] = p.z;
        buffers[GameRaceGhostLap.colYaw][i] = s.yaw;
        buffers[GameRaceGhostLap.colPitch][i] = s.pitch;
        buffers[GameRaceGhostLap.colRoll][i] = s.roll;
        buffers[GameRaceGhostLap.colUpX][i] = up.x;
        buffers[GameRaceGhostLap.colUpZ][i] = up.z;
        buffers[GameRaceGhostLap.colFlags][i] = s.flags;
        buffers[GameRaceGhostLap.colMoveX][i] = sticks.move.x;
        buffers[GameRaceGhostLap.colMoveY][i] = sticks.move.y;
        buffers[GameRaceGhostLap.colCamYaw][i] = arcade.stepCameraYaw;
        buffers[GameRaceGhostLap.colButtons][i] = sticks.buttons;
    }

    private static void CloseLap(float lapTime, GameVehicleArcade arcade) {

        lapOpen = false;

        if (count < 2 || lapTime <= 0f) {
            return;
        }

        GameRaceGhostLap lap = new GameRaceGhostLap();
        DateTime now = DateTime.UtcNow;
        lap.recorded = now.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", System.Globalization.CultureInfo.InvariantCulture);
        lap.id = now.ToString("yyyyMMdd'T'HHmmssfff", System.Globalization.CultureInfo.InvariantCulture) + "-" + (++lapsRecorded);
        lap.rider = "player";
        lap.vehicleCode = vehicleCode;
        lap.controlMode = arcade.control.mode == (int)ControlMode.direct ? "direct" : "heading";
        lap.lapTime = lapTime;
        lap.hz = hz;
        lap.clean = GameRace.playerRespawns == respawnMark;
        lap.count = count;

        for (int c = 0; c < GameRaceGhostLap.columnCount; c++) {
            lap.columns[c] = new float[count];
            Array.Copy(buffers[c], lap.columns[c], count);
        }

        GameRaceGhostStore.Add(file, lap, trackHash);
        dirty = true;
    }
}
#endif
