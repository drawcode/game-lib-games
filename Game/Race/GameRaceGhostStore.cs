#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Core;
using Agnostic.Host;
using Agnostic.Racing;

using Engine.AgnosticHost;

// The ghost files (Action Bikes R2 step 10): <persistentDataPath>/ghosts/ghost-<level_code>.json, one per track,
// written through the host's WriteAtomic (temp sibling, then rename).
//
// - Keep rule: the best 3 laps by time (clean laps first) plus the last 5 recorded, deduped by id.
// - track_hash: CRC32 of the node positions and widths. A file whose hash is not the track's keeps its laps, but
//   none of them plays as this track's best ghost, and the keep rule leaves them alone.
// - Recovery: a temp sibling left by a crashed write is used only when the file itself is missing or unreadable.
//   An unreadable file is copied to <file>.bad before anything overwrites it.
public static class GameRaceGhostStore {

    public const int keepBest = 3;
    public const int keepLast = 5;
    public const string folder = "ghosts";

    private static IHostStorage storageOverride;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        storageOverride = null;
    }

    // Tests swap in a memory host.
    public static IHostStorage storage {
        get {
            return storageOverride ?? AgnosticStorage.storage;
        }
        set {
            storageOverride = value;
        }
    }

    public static string PathOf(string levelCode) {

        char[] chars = (levelCode ?? "").ToCharArray();

        for (int i = 0; i < chars.Length; i++) {
            if (!char.IsLetterOrDigit(chars[i]) && chars[i] != '-' && chars[i] != '_' && chars[i] != '.') {
                chars[i] = '_';
            }
        }

        return UnityStorage.saveRoot + folder + "/ghost-" + new string(chars) + ".json";
    }

    // The track's file, or a new empty one (never null).
    public static GameRaceGhostFile Load(string levelCode) {

        string path = PathOf(levelCode);
        IHostStorage host = storage;
        byte[] bytes;
        GameRaceGhostFile file = null;

        if (host.Read(path, out bytes) == HostStatus.ok) {

            file = GameRaceGhostFile.Parse(AgnosticStorage.DecodeUtf8(bytes));

            if (file == null) {
                Debug.LogWarning("GameRaceGhostStore: " + path + " is unreadable, kept as .bad");
                host.WriteAtomic(path + ".bad", bytes);
            }
        }

        if (file == null && host.Read(path + ".tmp", out bytes) == HostStatus.ok) {
            file = GameRaceGhostFile.Parse(AgnosticStorage.DecodeUtf8(bytes));

            if (file != null) {
                file.recovered = true;
            }
        }

        if (file == null) {
            file = new GameRaceGhostFile();
        }

        file.track = levelCode ?? "";
        return file;
    }

    public static bool Save(string levelCode, GameRaceGhostFile file) {

        if (file == null || string.IsNullOrEmpty(levelCode)) {
            return false;
        }

        file.track = levelCode;
        string json = file.ToJson();
        return storage.WriteAtomic(PathOf(levelCode), AgnosticStorage.utf8NoBom.GetBytes(json)) == HostStatus.ok;
    }

    // CRC32 (SaveEnvelope's) of the nodes in order: each position and the width, in millimetres, little endian.
    public static string TrackHash(RaceTrack track) {

        int n = track.nodeCount;
        byte[] bytes = new byte[n * 16];
        int at = 0;

        for (int i = 0; i < n; i++) {
            RaceNode node = track.Node(i);
            Put(bytes, ref at, node.position.x);
            Put(bytes, ref at, node.position.y);
            Put(bytes, ref at, node.position.z);
            Put(bytes, ref at, node.width);
        }

        return SaveEnvelope.Crc32(bytes, 0, bytes.Length).ToString("x8");
    }

    private static void Put(byte[] bytes, ref int at, float metres) {
        int mm = (int)Math.Round(metres * 1000.0);
        bytes[at++] = (byte)mm;
        bytes[at++] = (byte)(mm >> 8);
        bytes[at++] = (byte)(mm >> 16);
        bytes[at++] = (byte)(mm >> 24);
    }

    // Adds a lap recorded on the track with this hash and applies the keep rule to that track's laps.
    public static void Add(GameRaceGhostFile file, GameRaceGhostLap lap, string trackHash) {

        // Laps of an older hash keep it before the file takes the new one.
        for (int i = 0; i < file.laps.Count; i++) {
            if (string.IsNullOrEmpty(file.laps[i].trackHash)) {
                file.laps[i].trackHash = file.trackHash;
            }
        }

        file.trackHash = trackHash;
        lap.trackHash = trackHash;
        file.laps.Add(lap);
        Prune(file);
        file.bestLapTime = BestLapTime(file, trackHash);
    }

    // Drops this track's laps outside best 3 + last 5. Laps of other hashes stay.
    public static void Prune(GameRaceGhostFile file) {

        List<int> mine = new List<int>();

        for (int i = 0; i < file.laps.Count; i++) {
            if (file.laps[i].trackHash == file.trackHash) {
                mine.Add(i);
            }
        }

        HashSet<GameRaceGhostLap> keep = new HashSet<GameRaceGhostLap>();
        List<int> byTime = new List<int>(mine);
        byTime.Sort((a, b) => Compare(file.laps[a], file.laps[b], a, b));

        for (int k = 0; k < byTime.Count && k < keepBest; k++) {
            keep.Add(file.laps[byTime[k]]);
        }

        List<int> byRecorded = new List<int>(mine);
        byRecorded.Sort((a, b) => {
            int c = string.CompareOrdinal(file.laps[b].recorded, file.laps[a].recorded);
            return c != 0 ? c : b.CompareTo(a);
        });

        for (int k = 0; k < byRecorded.Count && k < keepLast; k++) {
            keep.Add(file.laps[byRecorded[k]]);
        }

        // A repeated id keeps its first lap.
        HashSet<string> ids = new HashSet<string>();
        string hash = file.trackHash;

        file.laps.RemoveAll(lap => lap.trackHash == hash && (!keep.Contains(lap) || (lap.id.Length > 0 && !ids.Add(lap.id))));
    }

    // Clean laps first, then faster; the list order breaks a tie.
    private static int Compare(GameRaceGhostLap a, GameRaceGhostLap b, int ia, int ib) {

        if (a.clean != b.clean) {
            return a.clean ? -1 : 1;
        }

        int c = a.lapTime.CompareTo(b.lapTime);
        return c != 0 ? c : ia.CompareTo(ib);
    }

    // The lap to chase on the track with this hash: the fastest clean one, else the fastest of the rest. Laps with
    // no playable samples never qualify. null = none (first run, or the track changed).
    public static GameRaceGhostLap Best(GameRaceGhostFile file, string trackHash) {

        GameRaceGhostLap best = null;
        int bestIndex = -1;

        for (int i = 0; i < file.laps.Count; i++) {

            GameRaceGhostLap lap = file.laps[i];

            if (lap.trackHash != trackHash || !lap.usable || lap.lapTime <= 0f) {
                continue;
            }

            if (best == null || Compare(lap, best, i, bestIndex) < 0) {
                best = lap;
                bestIndex = i;
            }
        }

        return best;
    }

    public static float BestLapTime(GameRaceGhostFile file, string trackHash) {
        GameRaceGhostLap best = Best(file, trackHash);
        return best != null ? best.lapTime : 0f;
    }
}
#endif
