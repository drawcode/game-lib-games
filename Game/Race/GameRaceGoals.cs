#if USE_AGNOSTIC_VEHICLE
using System;

using UnityEngine;

using Agnostic.Racing;
using Agnostic.Vehicle;

// What the player did in a race round, for racing mission goals (Action Bikes iter 8 item 8). GameRace calls OnStep
// after the race rules of every fixed step (a third seam next to the ghosts and onAfterStep); the tallies stay after
// the race ends, until the next Begin, so the round-end mission check can read them. Nothing allocates per step.
//
// Goal kinds (a mission item's data.code): value(kind) is what the round reached, the item's compare_type and val
// decide (CompareFor gives the kind's usual compare when the row names none).
//   race_place_max   the finish place, wins at <= val
//   race_win         1 for a win else 0
//   race_clean_laps  laps finished with no bail and no respawn in them
//   air_time         the longest single jump, seconds
//   tricks           tricks landed in the round
//   lap_time_max     the best lap, seconds; wins at <= val (no lap finished = never)
public static class GameRaceGoals {

    public const string placeMax = "race_place_max";
    public const string win = "race_win";
    public const string cleanLaps = "race_clean_laps";
    public const string airTime = "air_time";
    public const string tricks = "tricks";
    public const string lapTimeMax = "lap_time_max";

    public const string compareAtMost = "less-than-or-equal-to";
    public const string compareAtLeast = "greater-than-or-equal-to";

    public static float bestAir { get; private set; }         // longest jump so far, seconds
    public static int tricksLanded { get; private set; }
    public static int cleanLapCount { get; private set; }
    public static int crashes { get; private set; }           // bails and respawns of the player
    public static float bestLap { get; private set; }         // 0 = no lap finished

    private static bool seen;
    private static bool closed;
    private static bool lapCrashed;
    private static int lapSeen;
    private static int respawnMark;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        Reset();
    }

    public static void Reset() {
        bestAir = 0f;
        tricksLanded = 0;
        cleanLapCount = 0;
        crashes = 0;
        bestLap = 0f;
        seen = false;
        closed = false;
        lapCrashed = false;
        lapSeen = 0;
        respawnMark = 0;
    }

    public static void Begin() {
        Reset();
    }

    // After the race rules of one fixed step: the player's progress and the arcade's step events.
    public static void OnStep() {

        if (!GameRace.active || closed) {
            return;
        }

        int index = GameRace.playerIndex;
        RaceRacer r = GameRace.Racer(index);
        GameVehicleArcade arcade = GameRace.Arcade(index);

        if (arcade == null || (r.flags & (int)RaceFlags.started) == 0) {
            return;
        }

        Tally(r.lap, r.lastLapTime, arcade.stepEvents.flags, arcade.state.airTime, GameRace.playerRespawns);

        if ((r.flags & (int)RaceFlags.finished) != 0) {
            closed = true;
        }
    }

    // One step of the player: laps finished and the last lap's time, the step's VehicleEventFlags, the jump so far
    // (VehicleState.airTime) and the race's respawn count. Public so tests can drive it.
    public static void Tally(int lap, float lastLapTime, int eventFlags, float air, int respawns) {

        if (!seen) {
            seen = true;
            lapSeen = lap;
            respawnMark = respawns;
        }

        if ((eventFlags & (int)VehicleEventFlags.bailed) != 0) {
            crashes++;
            lapCrashed = true;
        }

        if (respawns > respawnMark) {
            crashes += respawns - respawnMark;
            respawnMark = respawns;
            lapCrashed = true;
        }

        if ((eventFlags & (int)VehicleEventFlags.trick) != 0) {
            tricksLanded++;
        }

        if (air > bestAir) {
            bestAir = air;
        }

        if (lap > lapSeen) {
            lapSeen = lap;

            if (!lapCrashed) {
                cleanLapCount++;
            }

            lapCrashed = false;

            if (lastLapTime > 0f && (bestLap <= 0f || lastLapTime < bestLap)) {
                bestLap = lastLapTime;
            }
        }
    }

    public static bool IsKind(string kind) {
        return kind == placeMax || kind == win || kind == cleanLaps || kind == airTime || kind == tricks
            || kind == lapTimeMax;
    }

    // The compare a kind uses when its row names none.
    public static string CompareFor(string kind) {
        return kind == placeMax || kind == lapTimeMax ? compareAtMost : compareAtLeast;
    }

    // What the round reached for a kind; false for a kind not known (the item is left alone).
    public static bool Value(string kind, int place, out double value) {

        value = 0;

        if (kind == placeMax) {
            value = place > 0 ? place : int.MaxValue;
        }
        else if (kind == win) {
            value = place == 1 ? 1 : 0;
        }
        else if (kind == cleanLaps) {
            value = cleanLapCount;
        }
        else if (kind == airTime) {
            value = bestAir;
        }
        else if (kind == tricks) {
            value = tricksLanded;
        }
        else if (kind == lapTimeMax) {
            value = bestLap > 0f ? bestLap : double.MaxValue;
        }
        else {
            return false;
        }

        return true;
    }
}
#endif
