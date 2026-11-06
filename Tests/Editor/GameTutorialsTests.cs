using System.Collections.Generic;
using NUnit.Framework;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Utility;

namespace Game.Level.Tests {

    // The tutorial route (GameTutorials, Action Bots gameplay iter 39): a player mission's type
    // action picks the tutorial row with the same first action; arcade, test and tutorial rows never
    // gate; a game without tutorial rows never gates; the done flag lives in the profile, per row.
    public class GameTutorialsTests {

        static string Row(string type, string code, string action, string extraTags = "", string extraData = "") {
            return "{\"type\":\"" + type + "\",\"code\":\"" + code + "\""
                + ",\"tags\":[\"" + type + "\"" + extraTags + "]"
                + ",\"data\":{" + extraData + "\"data\":["
                + "{\"code\":\"" + action + "\",\"type\":\"action\",\"data\":{\"code\":\"stat\",\"val\":1}},"
                + "{\"code\":\"action-collect\",\"type\":\"action\",\"data\":{\"code\":\"item-coin\",\"val\":1}}]}}";
        }

        List<AppContentCollect> rows;

        AppContentCollect ByCode(string code) {
            foreach (AppContentCollect row in rows) {
                if (row.code == code) {
                    return row;
                }
            }
            return null;
        }

        [SetUp]
        public void SetUp() {
            string json = "["
                + Row("mission", "mission-default-1", "action-kill") + ","
                + Row("mission", "mission-attack-1", "action-attack") + ","
                + Row("mission", "mission-attack-2", "action-attack") + ","
                + Row("mission", "mission-build-1", "action-build") + ","
                + Row("mission", "mission-repair-1", "action-repair") + ","
                + Row("mission", "mission-test-attack-1", "action-attack", ",\"test\"") + ","
                + Row("arcade", "arcade-attack", "action-attack") + ","
                + Row("tutorial", "tutorial-attack", "action-attack", "", "\"level_kind\":\"tutorial\",\"level_num_suffix_list\":[1],") + ","
                + Row("tutorial", "tutorial-build", "action-build", "", "\"level_kind\":\"tutorial\",\"level_num_suffix_list\":[2],") + ","
                + Row("tutorial", "tutorial-repair", "action-repair", "", "\"level_kind\":\"tutorial\",\"level_num_suffix_list\":[3],")
                + "]";

            rows = json.FromJson<List<AppContentCollect>>();
        }

        [Test]
        public void EachTypeMapsToItsTutorial() {
            Assert.AreEqual("tutorial-attack", GameTutorials.For(ByCode("mission-attack-1"), rows).code);
            Assert.AreEqual("tutorial-attack", GameTutorials.For(ByCode("mission-attack-2"), rows).code);
            Assert.AreEqual("tutorial-build", GameTutorials.For(ByCode("mission-build-1"), rows).code);
            Assert.AreEqual("tutorial-repair", GameTutorials.For(ByCode("mission-repair-1"), rows).code);
        }

        [Test]
        public void TheTutorialPlaysTutorialLevelsUnderItsOwnRow() {
            AppContentCollect tutorial = GameTutorials.For(ByCode("mission-build-1"), rows);

            // its own code, so the round's stars land on the tutorial, never on mission-build-1
            Assert.AreNotEqual("mission-build-1", tutorial.code);
            Assert.AreEqual(GameLevelCodes.tutorialKind, GameLevelCodes.MissionLevelKind(tutorial));
            CollectionAssert.AreEqual(new List<int> { 2 }, tutorial.GetAllowedLevelSuffixList());
        }

        [Test]
        public void ATypeWithoutATutorialNeverGates() {
            Assert.IsNull(GameTutorials.For(ByCode("mission-default-1"), rows));
        }

        [Test]
        public void TestArcadeAndTutorialRowsNeverGate() {
            Assert.IsNull(GameTutorials.For(ByCode("mission-test-attack-1"), rows));
            Assert.IsNull(GameTutorials.For(ByCode("arcade-attack"), rows));
            Assert.IsNull(GameTutorials.For(ByCode("tutorial-attack"), rows));
        }

        [Test]
        public void AGameWithNoTutorialRowsNeverGates() {
            rows.RemoveAll(GameTutorials.IsTutorial);
            Assert.IsNull(GameTutorials.For(ByCode("mission-attack-1"), rows));
            Assert.IsNull(GameTutorials.For(ByCode("mission-attack-1"), null));
            Assert.IsNull(GameTutorials.For(null, rows));
        }

        [Test]
        public void DoneIsPerTutorialAndStartsFalse() {
            GameProfileTracking profile = new GameProfileTracking();
            AppContentCollect attack = ByCode("tutorial-attack");
            AppContentCollect build = ByCode("tutorial-build");

            Assert.IsFalse(GameTutorials.IsDone(profile, attack));

            GameTutorials.MarkDone(profile, attack);

            Assert.IsTrue(GameTutorials.IsDone(profile, attack));
            Assert.IsFalse(GameTutorials.IsDone(profile, build));
        }

        [Test]
        public void DoneSurvivesAJsonRoundTrip() {
            GameProfileTracking profile = new GameProfileTracking();
            AppContentCollect repair = ByCode("tutorial-repair");

            GameTutorials.MarkDone(profile, repair);

            GameProfileTracking loaded = profile.ToJson().FromJson<GameProfileTracking>();

            Assert.IsTrue(GameTutorials.IsDone(loaded, repair));
        }

        [Test]
        public void TheFlagIsATrackingMarkerUnderTheTutorialGroup() {
            GameProfileTracking tracking = new GameProfileTracking();
            GameTutorials.MarkDone(tracking, ByCode("tutorial-build"));
            Assert.IsTrue(tracking.IsSet("tutorial", "tutorial-build"));
            CollectionAssert.AreEqual(new List<string> { "tutorial-build" }, tracking.Codes(GameTutorials.collectType));
        }

        [Test]
        public void NoProfileOrRowIsNotDone() {
            Assert.IsFalse(GameTutorials.IsDone((GameProfileTracking)null, ByCode("tutorial-attack")));
            Assert.IsFalse(GameTutorials.IsDone(new GameProfileTracking(), null));
        }
    }
}
