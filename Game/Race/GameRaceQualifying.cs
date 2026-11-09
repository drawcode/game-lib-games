#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Racing;

// Qualifying sets the grid (Action Bikes iter 8, owner decision D3): the player rides one solo lap (GameRace with
// aiRidersOverride 0 and lapsOverride 1), every AI profile gets a time, and the riders line up fastest first.
//
// - AI time: the profile's best race lap on this track when the track's ghost file has one (ai_best_laps, written
//   at the end of every race), else the estimate  track.length / 18 m/s * factor  with
//       factor = 1 + (0.95 - corner_speed) * 0.6 + (1 - handling) * 0.1
//   (a profile at corner speed 0.95 and handling 1 rides 18 m/s; each 0.1 of corner speed costs 6%, each 0.1 of
//   handling 1%). Either way up to +-1.5% of seeded noise is added: an integer hash of the seed, the profile code
//   and the AI's index (never System.Random), so a seed gives the same grid.
// - Order: by time, the player first on a tie, an AI on the same time by its index. A player who did not finish
//   the lap is last.
// - The grid goes to GameRace.qualifiedOrder (rider ids in slot order: "player" or the AI profile code); the next
//   Begin uses it once.
public class GameRaceQualifyingResult {

    public string[] order;          // rider ids by grid slot: GameRaceQualifying.playerId or an AI profile code
    public float[] times;           // the qualifying time per slot, seconds; the player's is +infinity when unfinished
    public bool[] known;            // per slot: true = a recorded lap, false = an estimate (the player's is a recorded lap)
    public int playerSlot;          // 0-based

    public bool pole {
        get {
            return playerSlot == 0;
        }
    }
}

public static class GameRaceQualifying {

    public const string playerId = "player";
    public const float fallbackSpeed = 18f;      // m/s
    public const float noiseMax = 0.015f;        // of the time, either way

    public static GameRaceQualifyingResult last { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        last = null;
    }

    // The k-th AI's profile row, as the grid takes them: the level's ai_profiles in turn, else the built-in order.
    public static GameRaceAiProfiles.Row AiRow(GameRaceRules rules, int k) {

        GameRaceAiProfiles.Row row = rules.aiProfiles.Length > 0 ? GameRaceAiProfiles.Get(rules.aiProfiles[k % rules.aiProfiles.Length]) : null;
        return row ?? GameRaceAiProfiles.At(k);
    }

    // The estimate for a profile with no recorded lap, before noise.
    public static float FallbackTime(float trackLength, RaceAiProfile p) {
        float factor = 1f + (0.95f - p.cornerSpeed) * 0.6f + (1f - p.handling) * 0.1f;
        return trackLength / fallbackSpeed * factor;
    }

    // -1..1, from the seed, the profile code and the AI's index.
    public static float Noise(uint seed, string code, int index) {

        uint h = 2166136261u ^ seed;

        for (int i = 0; i < code.Length; i++) {
            h = (h ^ code[i]) * 16777619u;
        }

        h = (h ^ (uint)(index + 1)) * 16777619u;
        h ^= h >> 16;
        h *= 0x85EBCA6Bu;
        h ^= h >> 13;
        h *= 0xC2B2AE35u;
        h ^= h >> 16;
        return (h & 0xFFFFu) / 32767.5f - 1f;
    }

    public static float AiTime(GameRaceAiProfiles.Row row, int index, float trackLength, IDictionary<string, float> bests, uint seed, out bool known) {

        float time;
        known = bests != null && bests.TryGetValue(row.code, out time) && time > 0f;

        if (!known) {
            time = FallbackTime(trackLength, row.profile);
        }
        else {
            time = bests[row.code];
        }

        return time * (1f + Noise(seed, row.code, index) * noiseMax);
    }

    // aiRows: the AIs of the race in index order. playerTime <= 0 or not finite = the player did not set a lap.
    public static GameRaceQualifyingResult Build(IList<GameRaceAiProfiles.Row> aiRows, float trackLength, IDictionary<string, float> bests, float playerTime, uint seed) {

        int n = 1 + aiRows.Count;
        float[] times = new float[n];
        bool[] known = new bool[n];
        string[] ids = new string[n];
        times[0] = playerTime > 0f && !float.IsNaN(playerTime) && !float.IsInfinity(playerTime) ? playerTime : float.PositiveInfinity;
        known[0] = true;
        ids[0] = playerId;

        for (int k = 0; k < aiRows.Count; k++) {
            ids[k + 1] = aiRows[k].code;
            times[k + 1] = AiTime(aiRows[k], k, trackLength, bests, seed, out known[k + 1]);
        }

        // Entry 0 is the player; a stable order on (time, entry) puts the player first on a tie.
        int[] by = new int[n];

        for (int i = 0; i < n; i++) {
            by[i] = i;
        }

        Array.Sort(by, (a, b) => {
            int c = times[a].CompareTo(times[b]);
            return c != 0 ? c : a.CompareTo(b);
        });

        GameRaceQualifyingResult result = new GameRaceQualifyingResult();
        result.order = new string[n];
        result.times = new float[n];
        result.known = new bool[n];

        for (int slot = 0; slot < n; slot++) {
            result.order[slot] = ids[by[slot]];
            result.times[slot] = times[by[slot]];
            result.known[slot] = known[by[slot]];

            if (by[slot] == 0) {
                result.playerSlot = slot;
            }
        }

        return result;
    }

    // Called when the qualifying lap is done (GameRace still active): builds the grid from the player's lap and
    // the track's AI times, and hands it to the next Begin. Null when no race is active.
    public static GameRaceQualifyingResult Finish() {

        if (!GameRace.active || GameRace.track == null) {
            return null;
        }

        GameRaceRules race = GameRaceRules.FromCurrentLevel();
        uint seed = GameRace.seedOverride != 0 ? GameRace.seedOverride : race.seed != 0 ? race.seed : (uint)Environment.TickCount | 1u;
        List<GameRaceAiProfiles.Row> rows = new List<GameRaceAiProfiles.Row>();

        for (int k = 0; k < race.aiRiders; k++) {
            rows.Add(AiRow(race, k));
        }

        RaceRacer me = GameRace.Racer(GameRace.playerIndex);
        float playerTime = (me.flags & (int)RaceFlags.finished) != 0 ? me.lastLapTime : 0f;
        IDictionary<string, float> bests = GameRaceGhostStore.AiBestLaps(GameRaceGhostRecorder.file, GameRaceGhostRecorder.trackHash);
        GameRaceQualifyingResult result = Build(rows, GameRace.track.length, bests, playerTime, seed);

        GameRace.qualifiedOrder = result.order;
        last = result;
        return result;
    }
}
#endif
