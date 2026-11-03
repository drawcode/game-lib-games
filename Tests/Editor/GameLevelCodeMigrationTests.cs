using System.Collections.Generic;
using NUnit.Framework;
using Engine.Game.App.BaseApp;

namespace Game.Level.Tests {

    // The level-code migration (GameLevelCodeMigration): bare "<w>-<n>" statistics move to the
    // renamed "<kind>-<w>-<n>" and merge with what is already there, prefixed keys are left alone,
    // and a second run changes nothing.
    public class GameLevelCodeMigrationTests {

        // Action Bots' renumbering, cut down (contexts games/shared/context-level-codes.md)
        static Dictionary<int, string> Renames() {
            return new Dictionary<int, string> {
                { 1, "mission-1" },
                { 10, "mission-4" },
                { 11, "tutorial-1" },
                { 90, "test-1" }
            };
        }

        static BaseGameProfileStatistic Stats(params object[] pairs) {
            BaseGameProfileStatistic stats = new BaseGameProfileStatistic();
            for (int i = 0; i < pairs.Length; i += 2) {
                stats.SetStatisticValue((string)pairs[i], pairs[i + 1]);
            }
            return stats;
        }

        [Test]
        public void MapBare_RenamesByNumberInEveryWorld() {
            Assert.AreEqual("mission-1-4", GameLevelCodeMigration.MapBare("1-10", Renames()));
            Assert.AreEqual("tutorial-3-1", GameLevelCodeMigration.MapBare("3-11", Renames()));
            Assert.AreEqual("test-2-1", GameLevelCodeMigration.MapBare("2-90", Renames()));
        }

        [Test]
        public void MapBare_NullForUnmappedOrPrefixed() {
            Assert.IsNull(GameLevelCodeMigration.MapBare("1-5", Renames()));
            Assert.IsNull(GameLevelCodeMigration.MapBare("mission-1-1", Renames()));
            Assert.IsNull(GameLevelCodeMigration.MapBare("create-1-2", Renames()));
            Assert.IsNull(GameLevelCodeMigration.MapBare("", Renames()));
        }

        [Test]
        public void SplitLevelKey_FindsBareLevelPart() {
            string head;
            string bare;

            Assert.IsTrue(GameLevelCodeMigration.SplitLevelKey("high-score-1-10", out head, out bare));
            Assert.AreEqual("high-score", head);
            Assert.AreEqual("1-10", bare);

            Assert.IsTrue(GameLevelCodeMigration.SplitLevelKey("times-played-action-kills-2-1", out head, out bare));
            Assert.AreEqual("times-played-action-kills", head);
            Assert.AreEqual("2-1", bare);
        }

        [Test]
        public void SplitLevelKey_SkipsPrefixedAndNonLevelKeys() {
            string head;
            string bare;

            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("high-score-mission-1-4", out head, out bare));
            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("high-score-arcade-battle-1-1", out head, out bare));
            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("high-score-create-1-2", out head, out bare));
            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("high-score", out head, out bare));
            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("high-score-app-content-state-game-arcade", out head, out bare));
            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("high-1-1", out head, out bare));
            Assert.IsFalse(GameLevelCodeMigration.SplitLevelKey("coins-1-1", out head, out bare));
        }

        [Test]
        public void Merge_HighKeepsBestLowKeepsLowestTotalAdds() {
            Assert.AreEqual(9, GameLevelCodeMigration.Merge("high-score", 4, 9));
            Assert.AreEqual(9, GameLevelCodeMigration.Merge("high-score", 9, 4));
            Assert.AreEqual(3, GameLevelCodeMigration.Merge("low-time", 3, 7));
            Assert.AreEqual(7, GameLevelCodeMigration.Merge("low-time", 0, 7));
            Assert.AreEqual(3, GameLevelCodeMigration.Merge("low-time", 3, 0));
            Assert.AreEqual(11, GameLevelCodeMigration.Merge("total-kills", 4, 7));
        }

        [Test]
        public void MigrateStatistics_MovesMergesAndLeavesTheRest() {
            BaseGameProfileStatistic stats = Stats(
                "high-score-1-10", 500,
                "high-score-mission-1-4", 300,
                "total-kills-1-10", 12,
                "high-score-1-5", 70,
                "high-score", 900);

            int moved = GameLevelCodeMigration.MigrateStatistics(stats, Renames());

            Assert.AreEqual(2, moved);
            Assert.AreEqual(500, stats.GetStatisticValue("high-score-mission-1-4"));
            Assert.AreEqual(12, stats.GetStatisticValue("total-kills-mission-1-4"));
            Assert.IsFalse(stats.CheckIfAttributeExists("high-score-1-10"));
            Assert.IsFalse(stats.CheckIfAttributeExists("total-kills-1-10"));
            // never official: stays (its save becomes a create- level, which has no statistics yet)
            Assert.AreEqual(70, stats.GetStatisticValue("high-score-1-5"));
            Assert.AreEqual(900, stats.GetStatisticValue("high-score"));
        }

        [Test]
        public void MigrateStatistics_SecondRunChangesNothing() {
            BaseGameProfileStatistic stats = Stats("total-kills-1-10", 12);

            GameLevelCodeMigration.MigrateStatistics(stats, Renames());
            stats.SetStatisticValue("total-kills-1-10", 5);

            Assert.AreEqual(0, GameLevelCodeMigration.MigrateStatistics(stats, Renames()));
            Assert.AreEqual(12, stats.GetStatisticValue("total-kills-mission-1-4"));
            // the flag is not a statistic: the statistics screen never lists it
            Assert.IsFalse(stats.GetList().Exists(a => a.code == GameLevelCodeMigration.statisticsFlag));
        }

        [Test]
        public void MigrateStatistics_EmptyBlobOnlySetsTheFlag() {
            BaseGameProfileStatistic stats = new BaseGameProfileStatistic();

            Assert.AreEqual(0, GameLevelCodeMigration.MigrateStatistics(stats, Renames()));
            Assert.IsTrue(stats.CheckIfAttributeExists(GameLevelCodeMigration.statisticsFlag));
        }
    }
}
