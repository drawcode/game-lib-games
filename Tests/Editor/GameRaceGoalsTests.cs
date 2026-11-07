#if USE_AGNOSTIC_VEHICLE
using NUnit.Framework;

using Agnostic.Vehicle;

namespace Game.Race.Tests {

    // Racing mission goals: the player's tallies (air, tricks, clean laps, best lap) and what each goal kind reads.
    public class GameRaceGoalsTests {

        private const int bailed = (int)VehicleEventFlags.bailed;
        private const int trick = (int)VehicleEventFlags.trick | (int)VehicleEventFlags.landed;

        [SetUp]
        public void SetUp() {
            GameRaceGoals.Reset();
        }

        [Test]
        public void Air_KeepsTheLongestJump() {

            GameRaceGoals.Tally(0, 0f, 0, 0.5f, 0);
            GameRaceGoals.Tally(0, 0f, 0, 2.5f, 0);
            GameRaceGoals.Tally(0, 0f, 0, 0f, 0);
            GameRaceGoals.Tally(0, 0f, 0, 1.0f, 0);

            Assert.AreEqual(2.5f, GameRaceGoals.bestAir, 1e-5f);
        }

        [Test]
        public void Tricks_CountEachLandedTrick() {

            GameRaceGoals.Tally(0, 0f, trick, 0f, 0);
            GameRaceGoals.Tally(0, 0f, 0, 0f, 0);
            GameRaceGoals.Tally(0, 0f, trick, 0f, 0);

            Assert.AreEqual(2, GameRaceGoals.tricksLanded);
        }

        [Test]
        public void CleanLaps_SkipALapWithABailOrARespawn() {

            GameRaceGoals.Tally(0, 0f, 0, 0f, 0);
            GameRaceGoals.Tally(1, 31f, 0, 0f, 0);             // lap 1 clean
            GameRaceGoals.Tally(1, 31f, bailed, 0f, 0);        // a bail in lap 2
            GameRaceGoals.Tally(2, 29f, 0, 0f, 0);             // lap 2 not clean
            GameRaceGoals.Tally(2, 29f, 0, 0f, 1);             // a respawn in lap 3
            GameRaceGoals.Tally(3, 30f, 0, 0f, 1);             // lap 3 not clean
            GameRaceGoals.Tally(3, 30f, 0, 0f, 1);
            GameRaceGoals.Tally(4, 28f, 0, 0f, 1);             // lap 4 clean

            Assert.AreEqual(2, GameRaceGoals.cleanLapCount);
            Assert.AreEqual(2, GameRaceGoals.crashes);
            Assert.AreEqual(28f, GameRaceGoals.bestLap, 1e-5f);
        }

        [Test]
        public void Values_ReadPerKind() {

            GameRaceGoals.Tally(0, 0f, trick, 3.2f, 0);
            GameRaceGoals.Tally(1, 26.5f, 0, 0f, 0);

            double v;

            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.placeMax, 2, out v));
            Assert.AreEqual(2, v);
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.win, 2, out v));
            Assert.AreEqual(0, v);
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.win, 1, out v));
            Assert.AreEqual(1, v);
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.cleanLaps, 1, out v));
            Assert.AreEqual(1, v);
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.airTime, 1, out v));
            Assert.AreEqual(3.2, v, 1e-5);
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.tricks, 1, out v));
            Assert.AreEqual(1, v);
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.lapTimeMax, 1, out v));
            Assert.AreEqual(26.5, v, 1e-5);
            Assert.IsFalse(GameRaceGoals.Value("action-kill", 1, out v), "an unknown kind is left alone");
        }

        [Test]
        public void LapTime_NoFinishedLapNeverWins() {

            double v;

            GameRaceGoals.Value(GameRaceGoals.lapTimeMax, 1, out v);

            Assert.Greater(v, 1e9);
            Assert.AreEqual(GameRaceGoals.compareAtMost, GameRaceGoals.CompareFor(GameRaceGoals.lapTimeMax));
            Assert.AreEqual(GameRaceGoals.compareAtLeast, GameRaceGoals.CompareFor(GameRaceGoals.tricks));
        }
    }
}
#endif
