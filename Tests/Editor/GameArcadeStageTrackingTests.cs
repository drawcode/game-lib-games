using System.Collections.Generic;
using NUnit.Framework;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Utility;

namespace Game.Level.Tests {

    // Arcade stages moved to the gameplay tracking blob (2026-11-06): one typed "arcade-stages" item
    // keyed "<row>-<world>", and a stage not in it yet reads the old mode-blob value, so a saved stage
    // survives the move. Swaps both profile statics and puts them back (EditMode statics outlive the
    // test into Play mode).
    public class GameArcadeStageTrackingTests {

        GameProfileTracking savedTracking;
        GameProfileMode savedMode;
        AppContentCollect battle;
        AppContentCollect rescue;

        [SetUp]
        public void SetUp() {
            savedTracking = GameProfileTracking.Current;
            savedMode = GameProfileModes.Current;
            GameProfileTracking.Current = new GameProfileTracking();
            GameProfileModes.Current = new GameProfileMode();

            List<AppContentCollect> rows = ("["
                + "{\"type\":\"arcade\",\"code\":\"arcade-battle\",\"play_type\":\"battle\"},"
                + "{\"type\":\"arcade\",\"code\":\"arcade-rescue\",\"play_type\":\"rescue\"}"
                + "]").FromJson<List<AppContentCollect>>();
            battle = rows[0];
            rescue = rows[1];
        }

        [TearDown]
        public void TearDown() {
            GameProfileTracking.Current = savedTracking;
            GameProfileModes.Current = savedMode;
        }

        [Test]
        public void AStageIsWrittenToTheTrackingItemNotTheModeBlob() {
            GameArcadeLevels.SetStage(battle, "world-1", 3);

            Assert.AreEqual(3, GameArcadeLevels.Stage(battle, "world-1"));
            Assert.AreEqual("arcade-stages", GameProfileTracking.Current.ItemType(
                GameArcadeLevels.stageTrackingGroup, GameArcadeLevels.stageTrackingCode));
            Assert.IsNull(GameProfileModes.Current.GetContentCollectItem(
                GameArcadeLevels.stageCollectType, "arcade-battle-world-1"));
        }

        [Test]
        public void StagesAreKeptPerRowAndWorld() {
            GameArcadeLevels.SetStage(battle, "world-1", 2);
            GameArcadeLevels.SetStage(battle, "world-2", 5);
            GameArcadeLevels.SetStage(rescue, "world-1", 1);

            Assert.AreEqual(2, GameArcadeLevels.Stage(battle, "world-1"));
            Assert.AreEqual(5, GameArcadeLevels.Stage(battle, "world-2"));
            Assert.AreEqual(1, GameArcadeLevels.Stage(rescue, "world-1"));
            Assert.AreEqual(0, GameArcadeLevels.Stage(rescue, "world-2"));
        }

        [Test]
        public void AnOldModeBlobStageStillReadsUntilTheRowIsWrittenAgain() {
            GameProfileModes.Current.SetContentCollectValue(
                GameArcadeLevels.stageCollectType, "arcade-battle-world-1", GameArcadeLevels.stageKey, 4);

            Assert.AreEqual(4, GameArcadeLevels.Stage(battle, "world-1"));

            // another row written to tracking doesn't hide it
            GameArcadeLevels.SetStage(rescue, "world-1", 1);
            Assert.AreEqual(4, GameArcadeLevels.Stage(battle, "world-1"));

            // the row's own write wins from then on
            GameArcadeLevels.SetStage(battle, "world-1", 5);
            Assert.AreEqual(5, GameArcadeLevels.Stage(battle, "world-1"));
        }

        [Test]
        public void StagesSurviveTheSaveRoundTrip() {
            GameArcadeLevels.SetStage(battle, "world-1", 6);

            GameProfileTracking.Current = GameProfileTracking.Current.ToJson().FromJson<GameProfileTracking>();

            Assert.AreEqual(6, GameArcadeLevels.Stage(battle, "world-1"));
        }

        [Test]
        public void AStageIsClampedToTheTiers() {
            GameArcadeLevels.SetStage(battle, "world-1", GameArcadeLevels.tierMax + 5);
            Assert.AreEqual(GameArcadeLevels.tierMax, GameArcadeLevels.Stage(battle, "world-1"));
        }
    }
}
