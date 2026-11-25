#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;
using NUnit.Framework;

using Agnostic.Racing;

using Engine.Data.Json;

namespace Game.Race.Tests {

    // The career pieces that need no scene: the rules override merge, cup visibility, the career-state JSON and Record.
    public class GameRaceCareerTests {

        private const string cupJson = "[{\"sort_order\":1,\"type\":\"race-cup\",\"code\":\"cup-test\",\"display_name\":\"Test Cup\",\"active\":true,"
            + "\"data\":{\"tier\":1,\"tracks\":[\"t1\",\"t2\",\"t3\",\"t4\"],"
            + "\"race\":{\"laps\":2,\"ai_riders\":5,\"ai_profiles\":[\"A\",\"B\",\"C\",\"D\",\"E\"],\"player_slot\":3,\"bump_ahead_floor\":4.5},"
            + "\"per_race\":[{},{},{\"laps\":3},{}],\"grid\":\"reverse_standings\",\"boss\":\"A\",\"points\":[10,8,6,4,2,1],\"podium\":3}}]";

        [TearDown]
        public void TearDown() {
            GameRace.rulesOverride = null;
            GameRace.playerVehicleOverride = null;
            GameRace.qualifiedOrder = null;
        }

        [Test]
        public void Merge_NullOverrideReturnsTheBase() {

            Dictionary<string, object> level = new Dictionary<string, object> { { "laps", 3 } };

            Assert.AreSame(level, GameRaceRules.Merge(level, null));
            Assert.IsNull(GameRaceRules.Merge(null, null));
        }

        [Test]
        public void Merge_PrecedencePerRaceOverCupOverLevelOverDefaults() {

            Dictionary<string, object> level = new Dictionary<string, object> { { "laps", 5 }, { "default_width", 12.5 }, { "ai_riders", 4 } };
            Dictionary<string, object> cup = new Dictionary<string, object> { { "laps", 2 }, { "ai_riders", 5 } };
            Dictionary<string, object> perRace = new Dictionary<string, object> { { "laps", 3 } };

            // The cup's race block, then per_race over it (GameRaceCups.Row.RaceBlock), then over the level row.
            GameRaceCups.Row row = new GameRaceCups.Row();
            row.race = cup;
            row.perRace.Add(perRace);
            GameRaceRules r = GameRaceRules.Parse(GameRaceRules.Merge(level, row.RaceBlock(0)));

            Assert.AreEqual(3, r.laps);                  // per_race
            Assert.AreEqual(5, r.aiRiders);              // cup over level
            Assert.AreEqual(12.5f, r.defaultWidth);      // the track keeps its own key
            Assert.AreEqual(2f, r.respawnDelay);         // default
            Assert.AreEqual(5, level["laps"] is int ? (int)level["laps"] : 0);   // the base is not changed
        }

        [Test]
        public void Merge_NoLevelBlockTakesTheOverrideAlone() {

            Dictionary<string, object> over = new Dictionary<string, object> { { "laps", 4 } };

            GameRaceRules r = GameRaceRules.Parse(GameRaceRules.Merge(null, over));

            Assert.AreEqual(4, r.laps);
            Assert.AreEqual(5, r.aiRiders);
        }

        [Test]
        public void Overrides_AreOffByDefault() {

            Assert.IsNull(GameRace.rulesOverride);
            Assert.IsTrue(string.IsNullOrEmpty(GameRace.playerVehicleOverride));
            Assert.IsFalse(GameRaceCareer.enabled);
        }

        [Test]
        public void Cups_ParseTheRow() {

            GameRaceCups.Load(cupJson);
            GameRaceCups.Row cup = GameRaceCups.Get("cup-test");

            Assert.AreEqual(1, GameRaceCups.count);
            Assert.AreEqual(4, cup.tracks.Length);
            Assert.AreEqual(new[] { 10, 8, 6, 4, 2, 1 }, cup.points);
            Assert.AreEqual(3, cup.podium);
            Assert.AreEqual(3, GameRaceRules.Parse(cup.RaceBlock(2)).laps);
            Assert.AreEqual(2, GameRaceRules.Parse(cup.RaceBlock(1)).laps);
            Assert.AreEqual(4.5f, GameRaceRules.Parse(cup.RaceBlock(0)).bumpAheadFloor);
        }

        [Test]
        public void Cups_InactiveRowIsSkipped() {

            GameRaceCups.Load("[{\"code\":\"off\",\"active\":false,\"data\":{\"tracks\":[\"t1\"]}}]");

            Assert.AreEqual(0, GameRaceCups.count);
        }

        [Test]
        public void Visible_FalseWhenATrackIsMissing() {

            GameRaceCups.Load(cupJson);
            GameRaceCups.Row cup = GameRaceCups.Get("cup-test");

            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning, new System.Text.RegularExpressions.Regex("cup-test is hidden, missing tracks: t3"));

            Assert.IsFalse(GameRaceCups.Visible(cup, code => code != "t3"));
            Assert.IsTrue(GameRaceCups.Visible(cup, code => true));
            Assert.IsFalse(GameRaceCups.Visible(null, code => true));
        }

        [Test]
        public void State_RoundTripKeepsUnknownKeys() {

            string json = "{\"cup\":\"cup-test\",\"race\":2,\"future\":{\"a\":[1,2]},"
                + "\"standings\":[{\"id\":\"player\",\"pts\":18,\"places\":[1,1,0,0,0,0,0,0,0,0,0,0],\"last\":2,\"mood\":\"calm\"},"
                + "{\"id\":\"A\",\"pts\":18,\"places\":[1,1,0,0,0,0,0,0,0,0,0,0],\"last\":1}],\"paid\":[\"cup-test:1\"]}";

            GameRaceCareer.State s = GameRaceCareer.State.Parse(json);

            Assert.IsTrue(s.running);
            Assert.AreEqual("cup-test", s.cup);
            Assert.AreEqual(2, s.race);
            Assert.AreEqual(2, s.standings.Length);
            Assert.AreEqual(18, s.standings[0].points);
            Assert.AreEqual(2, s.standings[0].lastPlace);
            Assert.AreEqual(new List<string> { "cup-test:1" }, s.paid);

            string again = s.ToJson();
            JsonData n = JsonMapper.ToObject(again);

            Assert.AreEqual(2, (int)n["race"]);
            Assert.AreEqual(2, (int)n["future"]["a"][1]);
            Assert.AreEqual("calm", (string)n["standings"][0]["mood"]);
            Assert.AreEqual(18, (int)n["standings"][1]["pts"]);

            GameRaceCareer.State third = GameRaceCareer.State.Parse(again);
            Assert.AreEqual(again, third.ToJson());
        }

        [Test]
        public void State_EmptyOrUnreadableIsNoCup() {

            Assert.IsFalse(GameRaceCareer.State.Parse("").running);
            Assert.IsFalse(GameRaceCareer.State.Parse("{not json").running);
            Assert.IsFalse(GameRaceCareer.State.Parse("[1,2]").running);
            Assert.AreEqual("", GameRaceCareer.State.Parse("").ToJson());
        }

        // The plan's worked table: places by race, finishing order 1st..6th.
        private static readonly string[][] fourRaces = {
            new[] { "A", "player", "B", "C", "D", "E" },
            new[] { "player", "A", "C", "B", "E", "D" },
            new[] { "B", "player", "A", "C", "D", "E" },
            new[] { "A", "B", "player", "C", "E", "D" }
        };

        [Test]
        public void Record_FourRacesTable() {

            GameRaceCups.Load(cupJson);
            GameRaceCups.Row cup = GameRaceCups.Get("cup-test");
            GameRaceCareer.State s = GameRaceCareer.Start(cup);

            Assert.AreEqual(6, s.standings.Length);
            Assert.AreEqual("player", s.standings[0].id);

            for (int r = 0; r < 4; r++) {

                string[] ids = new string[6];
                int[] places = new int[6];

                for (int p = 0; p < 6; p++) {
                    ids[p] = fourRaces[r][p] == "player" ? "player" : fourRaces[r][p];
                    places[p] = p + 1;
                }

                GameRaceCareer.Record(s, cup, ids, places, 6);

                if (r == 0) {
                    // After race 1 the grid is E D C B player A.
                    Assert.AreEqual(new[] { "E", "D", "C", "B", "player", "A" }, GameRaceCareer.QualifiedOrder(s));
                }
            }

            Assert.IsTrue(GameRaceCareer.Done(s, cup));
            Assert.AreEqual(4, s.race);

            int[] order = s.Order();
            string[] finalIds = new string[6];
            int[] pts = new int[6];

            for (int i = 0; i < 6; i++) {
                finalIds[i] = s.standings[order[i]].id;
                pts[i] = s.standings[order[i]].points;
            }

            Assert.AreEqual(new[] { "A", "player", "B", "C", "E", "D" }, finalIds);
            Assert.AreEqual(new[] { 34, 32, 28, 18, 6, 6 }, pts);
            Assert.AreEqual(2, s.PlayerPlace());
            Assert.IsTrue(RaceSeries.Podium(s.PlayerPlace(), cup.podium));
        }

        [Test]
        public void StandingId_StripsTheAiPrefix() {

            Assert.AreEqual("player", GameRaceCareer.StandingId("player"));
            Assert.AreEqual("ai-n00b-1", GameRaceCareer.StandingId("ai-ai-n00b-1"));
        }

        [Test]
        public void ApplyRace_SetsTheOverridesAndTheReverseGrid() {

            GameRaceCups.Load(cupJson);
            GameRaceCups.Row cup = GameRaceCups.Get("cup-test");
            GameRaceCareer.State s = GameRaceCareer.Start(cup);

            Assert.IsTrue(GameRaceCareer.ApplyRace(s, cup, 0));
            Assert.IsNull(GameRace.qualifiedOrder);
            Assert.AreEqual(2, GameRaceRules.Parse(GameRace.rulesOverride).laps);

            GameRaceCareer.Record(s, cup, new[] { "A", "player", "B", "C", "D", "E" }, new[] { 1, 2, 3, 4, 5, 6 }, 6);

            Assert.IsTrue(GameRaceCareer.ApplyRace(s, cup, 2));
            Assert.AreEqual(3, GameRaceRules.Parse(GameRace.rulesOverride).laps);
            Assert.AreEqual(new[] { "E", "D", "C", "B", "player", "A" }, GameRace.qualifiedOrder);
            Assert.IsFalse(GameRaceCareer.ApplyRace(s, cup, 4));
        }
    }
}
#endif
