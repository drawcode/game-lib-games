#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;
using NUnit.Framework;

using Agnostic.Racing;

namespace Game.Race.Tests {

    // Qualifying: the fallback time formula, the seeded noise, the grid order (an unfinished qualifier last) and the
    // ai_best_laps keys of the ghost file.
    public class GameRaceQualifyingTests {

        private static GameRaceAiProfiles.Row Row(string code, float cornerSpeed, float handling) {

            GameRaceAiProfiles.Row row = new GameRaceAiProfiles.Row();
            row.code = code;
            row.profile = RaceAiProfile.Defaults();
            row.profile.cornerSpeed = cornerSpeed;
            row.profile.handling = handling;
            return row;
        }

        [Test]
        public void FallbackTime_IsLengthOver18TimesTheProfileFactor() {

            // 900 m at 18 m/s = 50 s; corner 0.95 and handling 1 is factor 1
            Assert.AreEqual(50f, GameRaceQualifying.FallbackTime(900f, Row("a", 0.95f, 1f).profile), 1e-3f);

            // corner 0.8: + 0.15 * 0.6 = 0.09; handling 0.6: + 0.4 * 0.1 = 0.04 -> 1.13
            Assert.AreEqual(56.5f, GameRaceQualifying.FallbackTime(900f, Row("b", 0.8f, 0.6f).profile), 1e-3f);
        }

        [Test]
        public void Noise_IsSeededInRangeAndVariesByProfile() {

            for (uint seed = 1; seed < 200; seed++) {
                float n = GameRaceQualifying.Noise(seed, "ai-rider-3", 2);
                Assert.That(n, Is.InRange(-1f, 1f));
                Assert.AreEqual(n, GameRaceQualifying.Noise(seed, "ai-rider-3", 2));
            }

            Assert.AreNotEqual(GameRaceQualifying.Noise(7u, "ai-rider-1", 0), GameRaceQualifying.Noise(7u, "ai-rider-2", 1));
        }

        [Test]
        public void AiTime_UsesTheRecordedLapWhenThereIsOne_WithinOnePointFivePercent() {

            GameRaceAiProfiles.Row row = Row("ai-rider-1", 0.95f, 1f);
            Dictionary<string, float> bests = new Dictionary<string, float> { { "ai-rider-1", 40f } };
            bool known;

            float time = GameRaceQualifying.AiTime(row, 0, 900f, bests, 5u, out known);
            Assert.IsTrue(known);
            Assert.That(time, Is.InRange(40f * 0.985f, 40f * 1.015f));

            time = GameRaceQualifying.AiTime(row, 0, 900f, new Dictionary<string, float>(), 5u, out known);
            Assert.IsFalse(known);
            Assert.That(time, Is.InRange(50f * 0.985f, 50f * 1.015f));
        }

        [Test]
        public void Build_SortsByTime_PlayerAmongThem() {

            List<GameRaceAiProfiles.Row> rows = new List<GameRaceAiProfiles.Row> {
                Row("slow", 0.8f, 0.6f), Row("fast", 0.95f, 1f), Row("mid", 0.9f, 0.8f)
            };

            // 900 m: fast ~50, mid ~53.7, slow ~56.5; the player's 52 lands second
            GameRaceQualifyingResult r = GameRaceQualifying.Build(rows, 900f, null, 52f, 3u);

            Assert.AreEqual(new[] { "fast", GameRaceQualifying.playerId, "mid", "slow" }, r.order);
            Assert.AreEqual(1, r.playerSlot);
            Assert.IsFalse(r.pole);
            Assert.AreEqual(52f, r.times[1]);

            for (int i = 1; i < r.times.Length; i++) {
                Assert.LessOrEqual(r.times[i - 1], r.times[i]);
            }
        }

        [Test]
        public void Build_PlayerWhoDidNotFinishIsLast() {

            List<GameRaceAiProfiles.Row> rows = new List<GameRaceAiProfiles.Row> { Row("a", 0.8f, 0.6f), Row("b", 0.95f, 1f) };

            foreach (float time in new[] { 0f, -1f, float.NaN, float.PositiveInfinity }) {
                GameRaceQualifyingResult r = GameRaceQualifying.Build(rows, 900f, null, time, 3u);
                Assert.AreEqual(2, r.playerSlot);
                Assert.AreEqual(GameRaceQualifying.playerId, r.order[2]);
                Assert.IsTrue(float.IsPositiveInfinity(r.times[2]));
            }
        }

        [Test]
        public void Build_FastPlayerIsPoleAndATieGoesToThePlayer() {

            List<GameRaceAiProfiles.Row> rows = new List<GameRaceAiProfiles.Row> { Row("a", 0.95f, 1f) };

            Assert.IsTrue(GameRaceQualifying.Build(rows, 900f, null, 10f, 3u).pole);

            float tie = GameRaceQualifying.Build(rows, 900f, null, 99f, 3u).times[0];
            Assert.IsTrue(GameRaceQualifying.Build(rows, 900f, null, tie, 3u).pole);
        }

        [Test]
        public void Build_SameSeedSameGrid() {

            List<GameRaceAiProfiles.Row> rows = new List<GameRaceAiProfiles.Row> { Row("a", 0.9f, 0.8f), Row("b", 0.9f, 0.8f), Row("c", 0.9f, 0.8f) };
            GameRaceQualifyingResult x = GameRaceQualifying.Build(rows, 900f, null, 0f, 11u);
            GameRaceQualifyingResult y = GameRaceQualifying.Build(rows, 900f, null, 0f, 11u);

            Assert.AreEqual(x.order, y.order);
            Assert.AreEqual(x.times, y.times);
        }

        [Test]
        public void GhostFile_AiBestLapsRoundTrip_KeepsTheFasterAndTheTrackHash() {

            GameRaceGhostFile file = new GameRaceGhostFile();

            Assert.IsTrue(GameRaceGhostStore.SetAiBest(file, "ai-rider-1", 23.4567f, "aaaa0001"));
            Assert.IsTrue(GameRaceGhostStore.SetAiBest(file, "ai-rider-2", 25f, "aaaa0001"));
            Assert.IsFalse(GameRaceGhostStore.SetAiBest(file, "ai-rider-1", 24f, "aaaa0001"));
            Assert.IsTrue(GameRaceGhostStore.SetAiBest(file, "ai-rider-1", 23f, "aaaa0001"));
            Assert.IsFalse(GameRaceGhostStore.SetAiBest(file, "ai-rider-3", 0f, "aaaa0001"));

            GameRaceGhostFile back = GameRaceGhostFile.Parse(file.ToJson());

            Assert.AreEqual(2, back.aiBestLaps.Count);
            Assert.AreEqual(23f, back.aiBestLaps["ai-rider-1"], 1e-3f);
            Assert.AreEqual(25f, back.aiBestLaps["ai-rider-2"], 1e-3f);
            Assert.AreEqual(2, GameRaceGhostStore.AiBestLaps(back, "aaaa0001").Count);
            Assert.AreEqual(0, GameRaceGhostStore.AiBestLaps(back, "bbbb0002").Count);

            // another track: the old times go
            Assert.IsTrue(GameRaceGhostStore.SetAiBest(back, "ai-rider-1", 30f, "bbbb0002"));
            Assert.AreEqual(1, back.aiBestLaps.Count);
        }

        [Test]
        public void GhostFile_AiBestLapsReaderIsTolerant() {

            string json = "{\"kind\":\"race-ghosts\",\"track\":\"t\",\"track_hash\":\"aaaa0001\",\"ai_best_laps\":{\"ai-rider-1\":23.5,\"bad\":\"x\",\"zero\":0},\"laps\":[]}";
            GameRaceGhostFile file = GameRaceGhostFile.Parse(json);

            Assert.AreEqual(1, file.aiBestLaps.Count);
            Assert.AreEqual(23.5f, file.aiBestLaps["ai-rider-1"], 1e-3f);

            file = GameRaceGhostFile.Parse("{\"kind\":\"race-ghosts\",\"ai_best_laps\":[1,2],\"laps\":[]}");
            Assert.AreEqual(0, file.aiBestLaps.Count);
        }
    }
}
#endif
