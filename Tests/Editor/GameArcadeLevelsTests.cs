using System.Collections.Generic;
using NUnit.Framework;
using Engine.Game.App;
using Engine.Game.Data;
using Engine.Utility;
using UnityEngine;

namespace Game.Level.Tests {

    // Arcade levels are generated from the play type's missions (GameArcadeLevels, contexts
    // games/shared/context-arcade-vs-missions.md A1 + A2): never a mission level, tier 0 easier
    // than the type's first mission with extra pickups, each tier climbing to the hardest mission
    // level and never past it, and the same row for the same inputs.
    public class GameArcadeLevelsTests {

        // The shipped shape (game-level-data, app-content-collection-data), cut down to what the
        // generator reads. Battle plays 1-1..1-3 with rising enemies, attack plays 1-11 with none,
        // and the test mission's 1-90 is far harder than anything a player mission plays.
        const string levelsJson = "["
            + "{\"type\":\"level\",\"code\":\"1-1\",\"world_code\":\"world-1\",\"display_name\":\"1\",\"data\":{"
            + "\"level_data\":{\"grid_width\":120,\"grid_height\":1,\"grid_depth\":120,\"grid_box_size\":4},"
            + "\"layout_presets\":[{\"code\":\"level-layout-basic-platform-1\",\"type\":\"centered\"}],"
            + "\"asset_presets\":[{\"code\":\"asset-rocks-1\",\"min\":50,\"max\":100},{\"code\":\"asset-crystals-1\",\"min\":40,\"max\":50}],"
            + "\"directors\":[{\"code\":\"enemy\",\"run\":true,\"min\":2,\"max\":4},{\"code\":\"sidekick\",\"run\":true,\"min\":3,\"max\":4},"
            + "{\"code\":\"weapon\",\"run\":true,\"min\":3,\"max\":4},{\"code\":\"item\",\"run\":true,\"min\":3,\"max\":4}]}},"
            + "{\"type\":\"level\",\"code\":\"1-2\",\"world_code\":\"world-1\",\"data\":{"
            + "\"directors\":[{\"code\":\"enemy\",\"run\":true,\"min\":3,\"max\":5}]}},"
            + "{\"type\":\"level\",\"code\":\"1-3\",\"world_code\":\"world-1\",\"data\":{"
            + "\"directors\":[{\"code\":\"enemy\",\"run\":true,\"min\":4,\"max\":6}]}},"
            + "{\"type\":\"level\",\"code\":\"1-11\",\"world_code\":\"world-1\",\"data\":{"
            + "\"layout_presets\":[{\"code\":\"level-layout-action-border-1\",\"type\":\"centered\"}],"
            + "\"directors\":[{\"code\":\"enemy\",\"run\":false,\"min\":3,\"max\":3},{\"code\":\"sidekick\",\"run\":false,\"min\":3,\"max\":3},"
            + "{\"code\":\"weapon\",\"run\":true,\"min\":3,\"max\":3},{\"code\":\"item\",\"run\":true,\"min\":3,\"max\":3}]}},"
            + "{\"type\":\"level\",\"code\":\"1-90\",\"world_code\":\"world-1\",\"data\":{"
            + "\"directors\":[{\"code\":\"enemy\",\"run\":true,\"min\":20,\"max\":30}]}}"
            + "]";

        const string battle = "battle";
        const string attack = "attack";

        List<AppContentCollect> missions;
        List<GameLevel> levels;

        static string Mission(string code, int sortOrder, int minLevel, string action, int val, string suffixes, bool test) {
            return "{\"type\":\"mission\",\"code\":\"" + code + "\",\"sort_order\":" + sortOrder
                + (minLevel >= 0 ? ",\"min_level\":" + minLevel : "")
                + ",\"tags\":[\"mission\",\"default\"" + (test ? ",\"test\"" : "") + "]"
                + ",\"data\":{\"level_num_suffix_list\":" + suffixes + ",\"world_num_list\":[1],\"data\":["
                + "{\"code\":\"" + action + "\",\"type\":\"action\",\"data\":{\"code\":\"stat\",\"val\":" + val + "}},"
                + "{\"code\":\"action-collect\",\"type\":\"action\",\"data\":{\"code\":\"item-coin\",\"val\":1}}]}}";
        }

        [SetUp]
        public void SetUp() {
            // the hardest battle mission is listed first: order comes from min_level, not the file
            string missionsJson = "["
                + Mission("mission-default-3", 3, 2, "action-kill", 3, "[1,2,3]", false) + ","
                + Mission("mission-default-1", 1, -1, "action-kill", 1, "[1,2,3]", false) + ","
                + Mission("mission-attack-1", 8, 1, "action-attack", 1, "[11]", false) + ","
                + Mission("mission-test-default-1", 0, -1, "action-kill", 9, "[90]", true)
                + "]";

            missions = missionsJson.FromJson<List<AppContentCollect>>();
            levels = levelsJson.FromJson<List<GameLevel>>();
        }

        Dictionary<string, object> Row(string playType, int tier, int seed = 7) {
            return GameArcadeLevels.Generate(playType, 1, tier, seed, missions, levels);
        }

        static IDictionary<string, object> Data(IDictionary<string, object> row) {
            return (IDictionary<string, object>)row["data"];
        }

        static IDictionary<string, object> Director(IDictionary<string, object> row, string code) {
            foreach (object raw in (IList<object>)Data(row)["directors"]) {
                IDictionary<string, object> director = (IDictionary<string, object>)raw;
                if ((string)director["code"] == code) {
                    return director;
                }
            }
            return null;
        }

        static double Num(IDictionary<string, object> director, string key) {
            return System.Convert.ToDouble(director[key]);
        }

        [Test]
        public void Battle_Tier0_IsHalfTheFirstMissionsEnemies_WithExtraPickups() {
            Dictionary<string, object> row = Row(battle, 0);

            // entry = 1-1 (mission-default-1's easiest level): enemies 2-4, weapons and items 3-4
            Assert.AreEqual(1, Num(Director(row, "enemy"), "min"));
            Assert.AreEqual(2, Num(Director(row, "enemy"), "max"));
            Assert.AreEqual(5, Num(Director(row, "weapon"), "min"));
            Assert.AreEqual(6, Num(Director(row, "weapon"), "max"));
            Assert.AreEqual(6, Num(Director(row, "item"), "max"));
            Assert.AreEqual(4, Num(Director(row, "sidekick"), "max"), "sidekicks are the entry level's");
        }

        [Test]
        public void Battle_TopTier_PlaysAtTheHardestMissionLevel_NotTheTestOne() {
            Dictionary<string, object> row = Row(battle, GameArcadeLevels.tierMax);

            // cap = 1-3 (4-6); the test mission's 1-90 (20-30) never counts
            Assert.AreEqual(4, Num(Director(row, "enemy"), "min"));
            Assert.AreEqual(6, Num(Director(row, "enemy"), "max"));
            Assert.AreEqual(3, Num(Director(row, "weapon"), "min"));
            Assert.AreEqual(4, Num(Director(row, "weapon"), "max"));
        }

        [Test]
        public void EveryTier_ClimbsAndStaysUnderTheCap() {
            double lastEnemyMax = 0;
            double lastWeaponMax = double.MaxValue;

            for (int tier = 0; tier <= GameArcadeLevels.tierMax; tier++) {
                Dictionary<string, object> row = Row(battle, tier);
                double enemyMin = Num(Director(row, "enemy"), "min");
                double enemyMax = Num(Director(row, "enemy"), "max");
                double weaponMax = Num(Director(row, "weapon"), "max");

                Assert.LessOrEqual(enemyMax, 6, "tier " + tier + " passes the cap");
                Assert.LessOrEqual(enemyMin, enemyMax, "tier " + tier);
                Assert.GreaterOrEqual(enemyMin, 1, "tier " + tier);
                Assert.GreaterOrEqual(enemyMax, lastEnemyMax, "tier " + tier + " got easier");
                Assert.LessOrEqual(weaponMax, lastWeaponMax, "tier " + tier + " gained pickups");

                lastEnemyMax = enemyMax;
                lastWeaponMax = weaponMax;
            }
        }

        [Test]
        public void TierOutOfRange_Clamps() {
            Assert.AreEqual(GameArcadeLevels.Code(battle, 1, 0), Row(battle, -5)["code"]);
            Assert.AreEqual(GameArcadeLevels.Code(battle, 1, GameArcadeLevels.tierMax), Row(battle, 99)["code"]);
        }

        [Test]
        public void AttackHasNoEnemiesInItsMission_SoNoneInArcade() {
            Dictionary<string, object> row = Row(attack, GameArcadeLevels.tierMax);

            Assert.AreEqual(false, Director(row, "enemy")["run"]);
            Assert.AreEqual("level-layout-action-border-1",
                ((IDictionary<string, object>)((IList<object>)Data(row)["layout_presets"])[0])["code"],
                "attack needs the action zones of its mission level");
        }

        [Test]
        public void Row_IsMarkedArcadeAndGenerated_AndIsNoMissionLevel() {
            Dictionary<string, object> row = Row(battle, 3);

            Assert.AreEqual("arcade-1-4", row["code"]);
            Assert.AreEqual("world-1", row["world_code"]);
            Assert.AreEqual(GameArcadeLevelKeys.arcade, Data(row)[GameArcadeLevelKeys.level_kind]);
            Assert.AreEqual(GameLevelItemSourceKeys.generated, Data(row)[GameLevelItemSourceKeys.level_items_source]);
            Assert.AreEqual(battle, Data(row)[GameArcadeLevelKeys.play_type]);

            foreach (GameLevel level in levels) {
                Assert.AreNotEqual(level.code, row["code"]);
            }
        }

        [Test]
        public void SameInputs_SameRow_AndTheSeedOnlyMovesPropDensity() {
            Assert.AreEqual(Row(battle, 4, 11).ToJson(), Row(battle, 4, 11).ToJson());

            for (int seed = 0; seed < 50; seed++) {
                foreach (object raw in (IList<object>)Data(Row(battle, 4, seed))["asset_presets"]) {
                    IDictionary<string, object> preset = (IDictionary<string, object>)raw;
                    string code = (string)preset["code"];
                    double low = code == "asset-rocks-1" ? 50 : 40;
                    double high = code == "asset-rocks-1" ? 100 : 50;
                    Assert.GreaterOrEqual(Num(preset, "min"), low, code + " seed " + seed);
                    Assert.LessOrEqual(Num(preset, "min"), low + (high - low) / 2, code + " seed " + seed);
                    Assert.AreEqual(high, Num(preset, "max"), code + " seed " + seed);
                }
            }
        }

        [Test]
        public void SourceRows_AreNotChanged() {
            string before = levels.ToJson() + missions.ToJson();

            Row(battle, 0);
            Row(battle, GameArcadeLevels.tierMax);
            Row(attack, 5);

            Assert.AreEqual(before, levels.ToJson() + missions.ToJson());
        }

        [Test]
        public void NoMissions_NoRow() {
            Assert.IsNull(Row("repair", 0), "no repair mission in the fixture");
            Assert.IsNull(GameArcadeLevels.Generate(battle, 2, 0, 1, missions, levels), "no world-2 levels");
            Assert.IsNull(Row("not-a-type", 0));
        }

        [Test]
        public void Goals_RiseFromOneToTheLargestMissionGoal() {
            List<object> easy = GameArcadeLevels.Goals(battle, 0, missions);
            List<object> hard = GameArcadeLevels.Goals(battle, GameArcadeLevels.tierMax, missions);

            Assert.AreEqual(2, easy.Count, "the first mission's actions");
            Assert.AreEqual(1, Goal(easy, "action-kill"));
            Assert.AreEqual(3, Goal(hard, "action-kill"), "mission-default-3, not the test mission's 9");
            Assert.AreEqual(1, Goal(hard, "action-collect"), "other actions are copied as they are");
        }

        static double Goal(List<object> goals, string code) {
            foreach (object raw in goals) {
                IDictionary<string, object> goal = (IDictionary<string, object>)raw;
                if ((string)goal["code"] == code) {
                    return Num((IDictionary<string, object>)goal["data"], "val");
                }
            }
            return -1;
        }

        [Test]
        public void Row_LoadsAsAGameLevel_WithItsDirectors() {
            GameLevel level = Row(battle, 0).ToJson().FromJson<GameLevel>();

            Assert.AreEqual("arcade-1-1", level.code);

            GameDataDirector enemy = null;
            foreach (GameDataDirector director in level.data.directors) {
                if (director.code == GameDataDirectorType.enemy) {
                    enemy = director;
                }
            }

            Assert.IsNotNull(enemy);
            Assert.IsTrue(enemy.run);
            Assert.AreEqual(2, enemy.max);
        }

        // The shipped Bots rows: every play type generates in every world, under its cap. Ignored
        // in an app that does not ship these files.
        [Test]
        public void ShippedRows_EveryTypeInEveryWorld() {
            TextAsset missionText = Resources.Load<TextAsset>("drawlabs/action-bots/version/data/app-content-collection-data.json");
            TextAsset levelText = Resources.Load<TextAsset>("drawlabs/action-bots/version/data/game-level-data.json");

            if (missionText == null || levelText == null) {
                Assert.Ignore("no Action Bots data in this project");
            }

            List<AppContentCollect> shippedMissions = missionText.text.FromJson<List<AppContentCollect>>();
            List<GameLevel> shippedLevels = levelText.text.FromJson<List<GameLevel>>();

            foreach (string playType in GameArcadeLevels.playTypeActions.Keys) {
                for (int world = 1; world <= 3; world++) {

                    IDictionary<string, object> entry;
                    IDictionary<string, object> cap;

                    Assert.IsTrue(GameArcadeLevels.FindEntryAndCap(playType, world, shippedMissions, shippedLevels,
                        out entry, out cap), playType + " world " + world);

                    for (int tier = 0; tier <= GameArcadeLevels.tierMax; tier++) {
                        Dictionary<string, object> row = GameArcadeLevels.Generate(
                            playType, world, tier, tier, shippedMissions, shippedLevels);
                        string label = playType + " " + world + " tier " + tier;

                        Assert.IsNotNull(row, label);
                        Assert.IsNull(GameArcadeLevels.LevelRow(shippedLevels, (string)row["code"]), label + " is a shipped code");
                        Assert.LessOrEqual(GameArcadeLevels.Pressure(row), GameArcadeLevels.Pressure(cap), label + " passes the cap");

                        if (tier == 0 && GameArcadeLevels.Pressure(entry) > 0) {
                            Assert.Less(GameArcadeLevels.Pressure(row), GameArcadeLevels.Pressure(entry), label + " is not easier");
                        }
                    }
                }
            }
        }

        // Level kinds (owner 2026-11-01): "<kind>-<world>-<n>" for every kind, arcade included.
        [Test]
        public void Codes_AreArcadeWorldNumber_OneBlockPerType() {
            Assert.AreEqual("arcade-1-1", GameArcadeLevels.Code(battle, 1, 0));
            Assert.AreEqual("arcade-1-10", GameArcadeLevels.Code(battle, 1, GameArcadeLevels.tierMax));
            Assert.AreEqual("arcade-2-11", GameArcadeLevels.Code(GameArcadeLevelKeys.rescue, 2, 0));
            Assert.AreEqual("arcade-3-50", GameArcadeLevels.Code(GameArcadeLevelKeys.repair, 3, 99));

            HashSet<string> codes = new HashSet<string>();
            foreach (string playType in GameArcadeLevels.playTypes) {
                for (int tier = 0; tier <= GameArcadeLevels.tierMax; tier++) {
                    Assert.IsTrue(codes.Add(GameArcadeLevels.Code(playType, 1, tier)), playType + " " + tier);
                }
            }
            Assert.AreEqual(GameArcadeLevels.playTypeActions.Count, GameArcadeLevels.playTypes.Length);
        }

        [Test]
        public void MissionLevels_PrefixedCodeFirst_ThenBare_ByTheMissionsLevelKind() {
            List<object> levelRows = new List<object> {
                new Dictionary<string, object> { { "code", "mission-1-5" } },
                new Dictionary<string, object> { { "code", "1-5" } },
                new Dictionary<string, object> { { "code", "1-6" } },
                new Dictionary<string, object> { { "code", "test-1-1" } },
            };
            Dictionary<string, object> mission = new Dictionary<string, object> {
                { "data", new Dictionary<string, object> { { "level_num_suffix_list", new List<object> { 5, 6, 7 } } } },
            };
            Dictionary<string, object> testMission = new Dictionary<string, object> {
                { "data", new Dictionary<string, object> {
                    { "level_kind", "test" }, { "level_num_suffix_list", new List<object> { 1 } } } },
            };

            List<IDictionary<string, object>> levels = GameArcadeLevels.MissionLevels(mission, 1, levelRows);
            Assert.AreEqual(2, levels.Count, "5 and 6 exist, 7 does not");
            Assert.AreEqual("mission-1-5", levels[0]["code"], "the prefixed row wins over the bare one");
            Assert.AreEqual("1-6", levels[1]["code"], "a bare row still plays (older data)");

            Assert.AreEqual("test-1-1", GameArcadeLevels.MissionLevels(testMission, 1, levelRows)[0]["code"]);
        }

        [Test]
        public void LevelCodes_KindIsThePrefix_BareReadsAsMission() {
            Assert.AreEqual("mission", GameLevelCodes.Kind("mission-1-4"));
            Assert.AreEqual("mission", GameLevelCodes.Kind("1-4"));
            Assert.AreEqual("tutorial", GameLevelCodes.Kind("tutorial-2-1"));
            Assert.AreEqual("test", GameLevelCodes.Kind("test-3-4"));
            Assert.AreEqual("arcade", GameLevelCodes.Kind(GameArcadeLevels.Code(battle, 1, 2)));
            Assert.AreEqual("create", GameLevelCodes.Kind("create-1-3"));
            Assert.IsNull(GameLevelCodes.Kind(""));
            Assert.IsNull(GameLevelCodes.Kind("nodash"));
            Assert.AreEqual("tutorial-1-2", GameLevelCodes.Format("tutorial", 1, 2));
            Assert.AreEqual("1-2", GameLevelCodes.Format("", 1, 2));
            Assert.AreEqual("1-3", GameLevelCodes.DisplayName("mission-1-3"));
            Assert.AreEqual("1-3", GameLevelCodes.DisplayName("create-1-3"));
        }

        // ZOMBIE WAVES (P5)
        [Test]
        public void Waves_SizesGoalsAndCaps() {
            Assert.AreEqual(4, GameArcadeWaves.WaveSize(1));
            Assert.AreEqual(8, GameArcadeWaves.WaveSize(3));
            Assert.AreEqual(4, GameArcadeWaves.WaveSize(0), "a wave before the first plays as the first");
            Assert.AreEqual(10, GameArcadeWaves.GoalKills(0), "stage 0 clears waves 1-2: 4 + 6");
            Assert.AreEqual(54, GameArcadeWaves.GoalKills(GameArcadeLevels.tierMax), "top stage clears 6 waves");
            Assert.AreEqual(6, GameArcadeWaves.AliveCap(0));
            Assert.AreEqual(10, GameArcadeWaves.AliveCap(99));
            for (int stage = 1; stage <= GameArcadeLevels.tierMax; stage++) {
                Assert.GreaterOrEqual(GameArcadeWaves.GoalKills(stage), GameArcadeWaves.GoalKills(stage - 1));
            }
        }

        [Test]
        public void Waves_SpawnTarget_FillsTheWave_UnderTheCap_AndNeverDespawns() {
            Assert.AreEqual(4, GameArcadeWaves.SpawnTarget(4, 0, 0, 6), "a fresh wave of 4");
            Assert.AreEqual(6, GameArcadeWaves.SpawnTarget(10, 0, 0, 6), "capped");
            Assert.AreEqual(6, GameArcadeWaves.SpawnTarget(10, 3, 2, 6), "5 left to spawn, cap 6");
            Assert.AreEqual(3, GameArcadeWaves.SpawnTarget(10, 7, 3, 6), "all spawned: hold what is alive");
            Assert.AreEqual(5, GameArcadeWaves.SpawnTarget(4, 0, 5, 6), "more alive than needed: none despawn");
        }

        [Test]
        public void Waves_GenerateFromBattle_InTheirOwnCodeBlock_WithTheZombiePreset() {
            Dictionary<string, object> waves = Row(GameArcadeLevelKeys.waves, 0);

            Assert.IsNotNull(waves, "waves is a kill mode: it generates from the battle missions");
            Assert.AreEqual("arcade-1-51", waves["code"]);
            Assert.AreEqual("arcade-1-60", GameArcadeLevels.Code(GameArcadeLevelKeys.waves, 1, 99));
            Assert.AreEqual(GameArcadeLevelKeys.waves, Data(waves)[GameArcadeLevelKeys.play_type]);

            Dictionary<string, object> row = new Dictionary<string, object> {
                { "data", new Dictionary<string, object> { { "character_presets", new List<object> {
                    new Dictionary<string, object> { { "code", "world-roster" }, { "type", "enemy" } },
                    new Dictionary<string, object> { { "code", "droids" }, { "type", "sidekick" } },
                } } } },
            };
            GameArcadeLevels.UseEnemyPreset(row, "zombies");
            List<object> presets = (List<object>)Data(row)["character_presets"];

            Assert.AreEqual(2, presets.Count);
            Assert.AreEqual("zombies", ((IDictionary<string, object>)presets[0])["code"]);
            Assert.AreEqual("enemy", ((IDictionary<string, object>)presets[0])["type"]);
            Assert.AreEqual("droids", ((IDictionary<string, object>)presets[1])["code"], "sidekicks stay");

            GameArcadeLevels.UseEnemyPreset(waves, null);
            Assert.IsFalse(Data(waves).ContainsKey("character_presets"), "no preset: the row is unchanged");
        }

        // The arcade list's rows (P2): each loads with its play_type, leads with the type's goal
        // and generates in every world it lists.
        [Test]
        public void ShippedArcadeRows_LoadAndGenerateInTheirWorlds() {
            TextAsset collectText = Resources.Load<TextAsset>("drawlabs/action-bots/version/data/app-content-collection-data.json");
            TextAsset levelText = Resources.Load<TextAsset>("drawlabs/action-bots/version/data/game-level-data.json");

            if (collectText == null || levelText == null) {
                Assert.Ignore("no Action Bots data in this project");
            }

            List<AppContentCollect> collects = collectText.text.FromJson<List<AppContentCollect>>();
            List<GameLevel> shippedLevels = levelText.text.FromJson<List<GameLevel>>();
            int arcadeRows = 0;

            foreach (AppContentCollect row in collects) {

                if (!GameArcadeLevels.IsArcade(row)) {
                    continue;
                }

                arcadeRows++;

                string playType = GameArcadeLevels.PlayType(row);

                Assert.IsNotNull(GameArcadeLevels.ActionFor(playType), row.code + " play_type " + playType);
                Assert.AreEqual(GameArcadeLevels.ActionFor(playType), row.data.data[0].code, row.code + " first action");
                Assert.IsNotEmpty(row.data.world_num_list, row.code);

                foreach (int world in row.data.world_num_list) {
                    Assert.IsNotNull(GameArcadeLevels.Generate(playType, world, 0, 0, collects, shippedLevels),
                        row.code + " world " + world);
                }
            }

            Assert.AreEqual(GameArcadeLevels.playTypeActions.Count, arcadeRows, "one arcade row per play type");
        }
    }
}
