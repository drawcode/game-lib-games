#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Agnostic.Core;
using Agnostic.Host;
using Agnostic.Racing;

using Engine.AgnosticHost;
using Engine.Data.Json;

namespace Game.Race.Tests {

    // The ghost file: the tolerant reader (unknown keys kept, a missing column empty, a malformed one parked under
    // unreadable:<key>), the temp-file recovery, the keep rule (best 3 + last 5) and the track hash.
    public class GameRaceGhostTests {

        // A host that keeps files in memory; only the storage calls the store makes do anything.
        private class MemoryStorage : IHostStorage {

            public readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            public bool KvGet(string key, out string value) { value = null; return false; }
            public HostStatus KvSet(string key, string value) { return HostStatus.ok; }
            public HostStatus KvDelete(string key) { return HostStatus.ok; }
            public HostStatus KvFlush() { return HostStatus.ok; }

            public HostStatus Read(string path, out byte[] bytes) {
                return files.TryGetValue(path, out bytes) ? HostStatus.ok : HostStatus.notFound;
            }

            public HostStatus WriteAtomic(string path, byte[] bytes) {
                files[path] = bytes;
                return HostStatus.ok;
            }

            public bool Exists(string path) { return files.ContainsKey(path); }
            public HostStatus Delete(string path) { files.Remove(path); return HostStatus.ok; }
            public int List(string directory, List<string> results) { return 0; }
        }

        private MemoryStorage host;

        [SetUp]
        public void SetUp() {
            host = new MemoryStorage();
            GameRaceGhostStore.storage = host;
        }

        [TearDown]
        public void TearDown() {
            GameRaceGhostStore.storage = null;
        }

        private static GameRaceGhostLap MakeLap(string id, float lapTime, string recorded, bool clean = true, string hash = "aaaa0001") {

            GameRaceGhostLap lap = new GameRaceGhostLap();
            lap.id = id;
            lap.lapTime = lapTime;
            lap.recorded = recorded;
            lap.clean = clean;
            lap.trackHash = hash;
            lap.count = 3;
            lap.columns[GameRaceGhostLap.colT] = new float[] { 0f, 0.05f, 0.1f };
            lap.columns[GameRaceGhostLap.colX] = new float[] { 1f, 2f, 3f };
            return lap;
        }

        private const string sample = "{\"kind\":\"race-ghosts\",\"track\":\"t1\",\"track_hash\":\"aaaa0001\",\"best_lap_time\":41.233,"
            + "\"future_file_key\":{\"a\":[1,2]},\"laps\":[{\"id\":\"l1\",\"rider\":\"player\",\"vehicle_code\":\"v\",\"control_mode\":\"heading\","
            + "\"lap_time\":41.233,\"recorded\":\"2026-01-01T00:00:00Z\",\"hz\":20,\"future_lap_key\":\"x\","
            + "\"samples\":{\"t\":[0,0.05,0.1],\"x\":[1,2,3],\"future_col\":[9,9,9],\"buttons\":\"oops\",\"yaw\":[0,1,\"bad\"]}}]}";

        [Test]
        public void Parse_UnknownKeysRoundTrip() {

            GameRaceGhostFile file = GameRaceGhostFile.Parse(sample);
            Assert.NotNull(file);
            GameRaceGhostFile again = GameRaceGhostFile.Parse(file.ToJson());

            Assert.AreEqual(1, again.extras.Count);
            Assert.AreEqual("future_file_key", again.extras[0].Key);
            Assert.AreEqual(2, again.extras[0].Value["a"].Count);
            GameRaceGhostLap lap = again.laps[0];
            Assert.AreEqual("future_lap_key", lap.extras[0].Key);
            Assert.AreEqual("x", (string)lap.extras[0].Value);
            Assert.IsTrue(lap.sampleExtras.Exists(e => e.Key == "future_col" && e.Value.Count == 3));
            Assert.AreEqual(41.233f, lap.lapTime, 1e-4f);
            Assert.AreEqual("v", lap.vehicleCode);
        }

        [Test]
        public void Parse_MissingColumnIsEmpty() {

            GameRaceGhostLap lap = GameRaceGhostFile.Parse(sample).laps[0];

            Assert.AreEqual(3, lap.count);
            Assert.IsNull(lap.columns[GameRaceGhostLap.colZ]);
            Assert.AreEqual(7f, lap.Get(GameRaceGhostLap.colZ, 1, 7f));
            Assert.AreEqual(2f, lap.Get(GameRaceGhostLap.colX, 1, 7f));
            Assert.IsTrue(lap.usable);
        }

        [Test]
        public void Parse_MalformedColumnParkedAsUnreadable() {

            GameRaceGhostFile file = GameRaceGhostFile.Parse(sample);
            GameRaceGhostLap lap = file.laps[0];

            Assert.IsNull(lap.columns[GameRaceGhostLap.colButtons]);
            Assert.IsNull(lap.columns[GameRaceGhostLap.colYaw]);
            Assert.IsTrue(lap.sampleExtras.Exists(e => e.Key == "unreadable:buttons"));
            Assert.IsTrue(lap.sampleExtras.Exists(e => e.Key == "unreadable:yaw"));

            // Written back under that key, so the next read finds an unknown key and the original value is not lost.
            GameRaceGhostLap again = GameRaceGhostFile.Parse(file.ToJson()).laps[0];
            Assert.IsTrue(again.sampleExtras.Exists(e => e.Key == "unreadable:buttons" && e.Value.IsString));
        }

        [Test]
        public void Parse_NotAGhostFileIsNull() {
            Assert.IsNull(GameRaceGhostFile.Parse(""));
            Assert.IsNull(GameRaceGhostFile.Parse("{\"kind\":\"profile\"}"));
            Assert.IsNull(GameRaceGhostFile.Parse("{not json"));
        }

        [Test]
        public void Store_SaveLoadRoundTrip() {

            GameRaceGhostFile file = new GameRaceGhostFile();
            GameRaceGhostStore.Add(file, MakeLap("a", 40.5f, "2026-01-01T00:00:01Z"), "aaaa0001");
            Assert.IsTrue(GameRaceGhostStore.Save("my level/1", file));

            Assert.IsTrue(host.files.ContainsKey("save:/ghosts/ghost-my_level_1.json"));
            GameRaceGhostFile back = GameRaceGhostStore.Load("my level/1");
            Assert.AreEqual(1, back.laps.Count);
            Assert.AreEqual("aaaa0001", back.trackHash);
            Assert.AreEqual(40.5f, back.bestLapTime, 1e-4f);
            Assert.AreEqual(3, back.laps[0].count);
        }

        [Test]
        public void Store_LeftoverTempIsIgnoredWhenTheFileIsGood() {

            host.files["save:/ghosts/ghost-t1.json"] = Encoding.UTF8.GetBytes(sample);
            host.files["save:/ghosts/ghost-t1.json.tmp"] = Encoding.UTF8.GetBytes("{\"kind\":\"race-ghosts\",\"laps\":[]}");

            GameRaceGhostFile file = GameRaceGhostStore.Load("t1");
            Assert.AreEqual(1, file.laps.Count);
            Assert.IsFalse(file.recovered);
        }

        [Test]
        public void Store_TempIsRecoveredWhenTheFileIsMissing() {

            host.files["save:/ghosts/ghost-t1.json.tmp"] = Encoding.UTF8.GetBytes(sample);

            GameRaceGhostFile file = GameRaceGhostStore.Load("t1");
            Assert.AreEqual(1, file.laps.Count);
            Assert.IsTrue(file.recovered);
        }

        [Test]
        public void Store_UnreadableFileIsKeptAsBadAndTempRecovered() {

            host.files["save:/ghosts/ghost-t1.json"] = Encoding.UTF8.GetBytes("{torn");
            host.files["save:/ghosts/ghost-t1.json.tmp"] = Encoding.UTF8.GetBytes(sample);

            GameRaceGhostFile file = GameRaceGhostStore.Load("t1");
            Assert.IsTrue(file.recovered);
            Assert.AreEqual("{torn", Encoding.UTF8.GetString(host.files["save:/ghosts/ghost-t1.json.bad"]));
        }

        [Test]
        public void Store_NothingOnDiskGivesAnEmptyFile() {

            GameRaceGhostFile file = GameRaceGhostStore.Load("t1");
            Assert.AreEqual(0, file.laps.Count);
            Assert.AreEqual("t1", file.track);
            Assert.IsNull(GameRaceGhostStore.Best(file, "aaaa0001"));
        }

        [Test]
        public void Keep_BestThreePlusLastFive() {

            GameRaceGhostFile file = new GameRaceGhostFile();

            // Ten laps, recorded in order; the three fastest are the 1st, 2nd and 3rd recorded.
            for (int i = 0; i < 10; i++) {
                GameRaceGhostStore.Add(file, MakeLap("l" + i, 40f + i, "2026-01-01T00:00:0" + i + "Z"), "aaaa0001");
            }

            List<string> ids = new List<string>();
            file.laps.ForEach(l => ids.Add(l.id));

            CollectionAssert.AreEquivalent(new[] { "l0", "l1", "l2", "l5", "l6", "l7", "l8", "l9" }, ids);
            Assert.AreEqual(40f, file.bestLapTime, 1e-4f);
        }

        [Test]
        public void Keep_SixLapsKeepsFiveOrSix() {

            GameRaceGhostFile file = new GameRaceGhostFile();

            // The slowest lap is the first recorded: it is in neither the best 3 nor the last 5.
            float[] times = { 50f, 41f, 40f, 42f, 43f, 44f };

            for (int i = 0; i < times.Length; i++) {
                GameRaceGhostStore.Add(file, MakeLap("l" + i, times[i], "2026-01-01T00:00:0" + i + "Z"), "aaaa0001");
            }

            Assert.AreEqual(5, file.laps.Count);
            Assert.IsFalse(file.laps.Exists(l => l.id == "l0"));
        }

        [Test]
        public void Keep_DuplicateIdKeptOnce() {

            GameRaceGhostFile file = new GameRaceGhostFile();
            GameRaceGhostStore.Add(file, MakeLap("same", 41f, "2026-01-01T00:00:01Z"), "aaaa0001");
            GameRaceGhostStore.Add(file, MakeLap("same", 42f, "2026-01-01T00:00:02Z"), "aaaa0001");

            Assert.AreEqual(1, file.laps.Count);
        }

        [Test]
        public void Best_PrefersCleanLapsEvenWhenSlower() {

            GameRaceGhostFile file = new GameRaceGhostFile();
            GameRaceGhostStore.Add(file, MakeLap("fast-dirty", 39f, "2026-01-01T00:00:01Z", false), "aaaa0001");
            GameRaceGhostStore.Add(file, MakeLap("slow-clean", 45f, "2026-01-01T00:00:02Z", true), "aaaa0001");

            Assert.AreEqual("slow-clean", GameRaceGhostStore.Best(file, "aaaa0001").id);
            Assert.AreEqual(45f, file.bestLapTime, 1e-4f);
        }

        [Test]
        public void Best_ChangedTrackHashKeepsLapsButPlaysNone() {

            GameRaceGhostFile file = new GameRaceGhostFile();
            GameRaceGhostStore.Add(file, MakeLap("old", 40f, "2026-01-01T00:00:01Z"), "aaaa0001");

            Assert.IsNull(GameRaceGhostStore.Best(file, "bbbb0002"));
            Assert.AreEqual(1, file.laps.Count);

            // A lap on the new shape: the old one stays on disk, only the new one plays.
            GameRaceGhostStore.Add(file, MakeLap("new", 50f, "2026-01-01T00:00:02Z", true, "bbbb0002"), "bbbb0002");

            Assert.AreEqual(2, file.laps.Count);
            Assert.AreEqual("new", GameRaceGhostStore.Best(file, "bbbb0002").id);
            Assert.AreEqual("old", file.laps[0].id);
            Assert.AreEqual("aaaa0001", file.laps[0].trackHash);
        }

        [Test]
        public void TrackHash_FollowsNodePositionsAndWidths() {

            RaceNode[] nodes = {
                new RaceNode(new Vec3(0f, 0f, 0f), 8f, 0),
                new RaceNode(new Vec3(30f, 0f, 0f), 8f, 1),
                new RaceNode(new Vec3(30f, 0f, 30f), 8f, 1)
            };

            string h = GameRaceGhostStore.TrackHash(new RaceTrack(nodes));
            Assert.AreEqual(8, h.Length);
            Assert.AreEqual(h, GameRaceGhostStore.TrackHash(new RaceTrack((RaceNode[])nodes.Clone())));

            RaceNode[] moved = (RaceNode[])nodes.Clone();
            moved[2].position = new Vec3(30f, 0f, 31f);
            Assert.AreNotEqual(h, GameRaceGhostStore.TrackHash(new RaceTrack(moved)));

            RaceNode[] wider = (RaceNode[])nodes.Clone();
            wider[1].width = 9f;
            Assert.AreNotEqual(h, GameRaceGhostStore.TrackHash(new RaceTrack(wider)));
        }
    }
}
#endif
