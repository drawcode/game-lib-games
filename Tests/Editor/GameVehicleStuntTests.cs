#if USE_AGNOSTIC_VEHICLE
using Agnostic.Core;
using Agnostic.Vehicle;

using Engine.AgnosticHost;
using Engine.Data.Json;

using NUnit.Framework;
using UnityEngine;

namespace Game.Race.Tests {

    // The D24 stunt host seam: no data means no stunt, only the player's rider branch steps it, ResetAt clears it, the
    // rider's stats are read once on mount, the take-off boost, the data block parses, and the stunts goal.
    public class GameVehicleStuntTests {

        class Driver : IGameVehicleArcadeDriver {
            public void Drive(GameVehicleArcade arcade, int step, ref Sticks sticks, ref float cameraYaw) {
            }
        }

        GameObject go;
        GameObject riderGo;
        GameVehicleArcade arcade;
        int sourceCalls;

        [SetUp]
        public void SetUp() {
            go = new GameObject("stunt-test");
            riderGo = new GameObject("stunt-test-rider");
            arcade = go.AddComponent<GameVehicleArcade>();
            sourceCalls = 0;
            GameVehicleArcade.riderStatsSource = null;
            GameRaceGoals.Reset();
        }

        [TearDown]
        public void TearDown() {
            GameVehicleArcade.riderStatsSource = null;
            Object.DestroyImmediate(riderGo);
            Object.DestroyImmediate(go);
        }

        GameVehicleArcadeConfig Config(bool stunt) {
            GameVehicleArcadeConfig c = new GameVehicleArcadeConfig();
            c.code = "stunt-test";
            c.tuning = VehicleTuning.Defaults();
            c.assist = DriveAssistTuning.Defaults();
            c.stunt = StuntTuning.Defaults();
            c.stunt.enabled = stunt ? 1 : 0;
            return c;
        }

        StuntStats CountingSource() {
            sourceCalls++;
            StuntStats s = new StuntStats();
            s.speed = 0.9f;
            s.power = 0.7f;
            s.health = 0.5f;
            return s;
        }

        [Test]
        public void NoDataMeansNoStuntAndTheBitIsIgnored() {
            arcade.Init(Config(false));
            Assert.IsNull(arcade.stunt);
            arcade.SetStunt(true);
            Assert.AreEqual(0, arcade.StuntButton());
            arcade.SetButtons((int)(VehicleButtons.stunt | VehicleButtons.boost));
            Assert.AreEqual(0, arcade.StuntButton());
        }

        [Test]
        public void DataMakesAStuntAndTheButtonSetsTheBit() {
            arcade.Init(Config(true));
            Assert.IsNotNull(arcade.stunt);
            arcade.SetStunt(true);
            Assert.AreEqual((int)VehicleButtons.stunt, arcade.StuntButton());
            arcade.SetStunt(false);
            Assert.AreEqual(0, arcade.StuntButton());
        }

        [Test]
        public void AiArcadeNeverStepsIt() {
            arcade.Init(Config(true));
            arcade.driverOverride = new Driver();

            for (int i = 0; i < 5; i++) {
                arcade.StepOnce(0f);
            }

            Assert.AreEqual(0f, arcade.stepStunt.timeToLand);
            Assert.IsFalse(arcade.stunt.Ready);
        }

        [Test]
        public void GhostArcadeNeverStepsIt() {
            arcade.Init(Config(true));
            arcade.hold = true;
            arcade.OnRiderMounted(riderGo);

            for (int i = 0; i < 5; i++) {
                arcade.StepOnce(0f);
            }

            Assert.AreEqual(0f, arcade.stepStunt.timeToLand);
            Assert.IsFalse(arcade.stunt.Ready);
        }

        [Test]
        public void PlayerRiderStepsIt() {
            arcade.Init(Config(true));
            arcade.OnRiderMounted(riderGo);
            arcade.StepOnce(0f);
            Assert.Greater(arcade.stepStunt.timeToLand, 0f);
        }

        [Test]
        public void APressInTheAirStartsAStunt() {
            arcade.Init(Config(true));
            arcade.OnRiderMounted(riderGo);
            arcade.state.flags &= ~(int)VehicleFlags.grounded;
            arcade.state.airTime = 0.3f;
            arcade.state.buttons = 0;
            arcade.assistForecast = new LandingForecast();
            arcade.assistForecast.timeToLand = 0.8f;
            arcade.assistForecastStep = arcade.stepCount;
            arcade.SetStunt(true);
            arcade.StepOnce(0f);
            Assert.AreEqual(1, arcade.stepStunt.started);
            Assert.AreEqual((int)StuntPhase.start, arcade.stunt.phase);
            Assert.AreEqual(0.8f, arcade.stepStunt.timeToLand, 1e-6f);
            Assert.AreEqual(0, arcade.stunt.landCount);
        }

        // The take-off boost (D24): popMax x power m/s on the airborne event of a stunt-length jump with a flat landing.

        StuntTuning BoostTuning() {
            StuntTuning t = StuntTuning.Defaults();
            t.enabled = 1;
            return t;
        }

        // A bike that has just left the ground at (0, y, 0), climbing at `vy`.
        VehicleState Airborne(float y, float vy) {
            VehicleState v = new VehicleState();
            v.position = new Vec3(0f, y, 0f);
            v.velocity = new Vec3(0f, vy, 0f);
            v.speed = Mathf.Abs(vy);
            v.flags = 0;
            return v;
        }

        VehicleTestWorld FlatWorld() {
            VehicleTestWorld w = new VehicleTestWorld();
            VehicleTestWorld.Ground g = new VehicleTestWorld.Ground();
            g.y = 0f;
            w.grounds.Add(g);
            return w;
        }

        StuntStats Power(float p) {
            StuntStats s = new StuntStats();
            s.power = p;
            return s;
        }

        [Test]
        public void TakeoffOnAFlatLandingBoostsByPopMaxTimesPower() {
            StuntTuning t = BoostTuning();
            VehicleTuning vt = VehicleTuning.Defaults();
            DriveAssistTuning a = DriveAssistTuning.Defaults();
            Assert.AreEqual(1.2f, GameVehicleStunt.BoostFor(in t, Power(1f), Airborne(0.1f, 5.5f), in vt, FlatWorld(), in a), 1e-5f);
            Assert.AreEqual(0.24f, GameVehicleStunt.BoostFor(in t, Power(0.2f), Airborne(0.1f, 5.5f), in vt, FlatWorld(), in a), 1e-5f);
            Assert.AreEqual(0f, GameVehicleStunt.BoostFor(in t, Power(0f), Airborne(0.1f, 5.5f), in vt, FlatWorld(), in a));
        }

        [Test]
        public void TakeoffOnASlopeLandingIsCancelled() {
            StuntTuning t = BoostTuning();
            VehicleTuning vt = VehicleTuning.Defaults();
            DriveAssistTuning a = DriveAssistTuning.Defaults();

            // A wide ramp (slope 0.5, normal.y about 0.89) under the bike: the landing is not flat.
            VehicleTestWorld w = FlatWorld();
            VehicleTestWorld.Ramp r = new VehicleTestWorld.Ramp();
            r.start = Vec3.zero;
            r.yaw = 0f;
            r.length = 20f;
            r.width = 20f;
            r.height = 10f;
            w.ramps.Add(r);
            VehicleState v = Airborne(5.5f, 5.5f);
            v.position = VehicleMath.Forward(0f) * 10f + new Vec3(0f, 5.5f, 0f);
            Assert.AreEqual(0f, GameVehicleStunt.BoostFor(in t, Power(1f), v, in vt, w, in a));
        }

        [Test]
        public void TakeoffWithShortAirGetsNoBoost() {
            StuntTuning t = BoostTuning();
            VehicleTuning vt = VehicleTuning.Defaults();
            DriveAssistTuning a = DriveAssistTuning.Defaults();

            // A whoop: about 0.2 s of air, under minAir + the gate air time (0.55 s).
            Assert.AreEqual(0f, GameVehicleStunt.BoostFor(in t, Power(1f), Airborne(0.1f, 1f), in vt, FlatWorld(), in a));
        }

        [Test]
        public void TakeoffBoostIsOffWhenTheStuntIsDisabled() {
            StuntTuning t = BoostTuning();
            t.enabled = 0;
            VehicleTuning vt = VehicleTuning.Defaults();
            DriveAssistTuning a = DriveAssistTuning.Defaults();
            Assert.AreEqual(0f, GameVehicleStunt.BoostFor(in t, Power(1f), Airborne(0.1f, 5.5f), in vt, FlatWorld(), in a));
        }

        [Test]
        public void ThePlayersTakeoffEventBoostsVelocityOnceAndOnlyOnce() {
            arcade.Init(Config(true));
            arcade.OnRiderMounted(riderGo);
            arcade.riderStats = Power(1f);

            // No colliders under the test arcade: the forecast finds no landing, which is a long flat fall (boost given).
            arcade.state.flags &= ~(int)VehicleFlags.grounded;
            arcade.state.velocity = new Vec3(0f, 5.5f, 0f);
            VehicleEvents ev = default(VehicleEvents);
            ev.flags = (int)VehicleEventFlags.airborne;
            arcade.stunt.AfterStep(arcade, ref ev);
            Assert.AreEqual(6.7f, arcade.state.velocity.y, 1e-5f);
            Assert.AreEqual(1, arcade.stunt.boostCount);
            Assert.AreEqual(1.2f, arcade.stunt.lastBoost, 1e-5f);
            Assert.AreEqual(1.2f, arcade.stepStunt.takeoffBoost, 1e-5f);

            // The steps after the take-off carry no airborne event: nothing more is added.
            ev = default(VehicleEvents);

            for (int i = 0; i < 5; i++) {
                arcade.stunt.AfterStep(arcade, ref ev);
            }

            Assert.AreEqual(6.7f, arcade.state.velocity.y, 1e-5f);
            Assert.AreEqual(1, arcade.stunt.boostCount);
        }

        [Test]
        public void ABailedTakeoffGetsNoBoost() {
            arcade.Init(Config(true));
            arcade.OnRiderMounted(riderGo);
            arcade.riderStats = Power(1f);
            arcade.state.flags &= ~(int)VehicleFlags.grounded;
            arcade.state.velocity = new Vec3(0f, 5.5f, 0f);
            VehicleEvents ev = default(VehicleEvents);
            ev.flags = (int)(VehicleEventFlags.airborne | VehicleEventFlags.bailed);
            arcade.stunt.AfterStep(arcade, ref ev);
            Assert.AreEqual(5.5f, arcade.state.velocity.y, 1e-5f);
            Assert.AreEqual(0, arcade.stunt.boostCount);
        }

        [Test]
        public void AiAndGhostTakeoffsGetNoBoost() {

            // The AI branch (a driver override) and a ghost (hold) never reach the stunt host's AfterStep.
            arcade.Init(Config(true));
            arcade.riderStats = Power(1f);
            arcade.driverOverride = new Driver();

            for (int i = 0; i < 5; i++) {
                arcade.StepOnce(0f);
            }

            Assert.AreEqual(0, arcade.stunt.boostCount);
            Assert.AreEqual(0f, arcade.stepStunt.takeoffBoost);

            arcade.driverOverride = null;
            arcade.hold = true;
            arcade.OnRiderMounted(riderGo);
            arcade.riderStats = Power(1f);

            for (int i = 0; i < 5; i++) {
                arcade.StepOnce(0f);
            }

            Assert.AreEqual(0, arcade.stunt.boostCount);
            Assert.AreEqual(0f, arcade.stepStunt.takeoffBoost);
        }

        [Test]
        public void ResetAtClearsIt() {
            arcade.Init(Config(true));
            arcade.stunt.state.phase = (int)StuntPhase.hold;
            arcade.stunt.state.points = 80f;
            arcade.stunt.landCount = 3;
            arcade.stunt.lastLandResult = GameVehicleStunt.landClean;
            arcade.stepStunt.phase = (int)StuntPhase.hold;
            arcade.ResetAt(Vector3.zero, Quaternion.identity);
            Assert.AreEqual((int)StuntPhase.none, arcade.stunt.phase);
            Assert.AreEqual(0f, arcade.stunt.points);
            Assert.AreEqual(0, arcade.stunt.landCount);
            Assert.AreEqual(GameVehicleStunt.landNone, arcade.stunt.lastLandResult);
            Assert.AreEqual(0, arcade.stepStunt.phase);
        }

        [Test]
        public void RiderStatsAreReadOnceOnMount() {
            GameVehicleArcade.riderStatsSource = CountingSource;
            arcade.Init(Config(true));
            Assert.AreEqual(0, sourceCalls);
            arcade.OnRiderMounted(riderGo);
            Assert.AreEqual(1, sourceCalls);

            for (int i = 0; i < 10; i++) {
                arcade.StepOnce(0f);
            }

            Assert.AreEqual(1, sourceCalls);
            Assert.AreEqual(0.9f, arcade.riderStats.speed, 1e-6f);
            Assert.AreEqual(0.7f, arcade.riderStats.power, 1e-6f);
            Assert.AreEqual(0.5f, arcade.riderStats.health, 1e-6f);
        }

        [Test]
        public void NoStuntMeansTheStatsAreNotReadAndStayAtTheDefault() {
            GameVehicleArcade.riderStatsSource = CountingSource;
            arcade.Init(Config(false));
            arcade.OnRiderMounted(riderGo);
            Assert.AreEqual(0, sourceCalls);
            Assert.AreEqual(GameVehicleArcade.defaultRiderStat, arcade.riderStats.speed, 1e-6f);
            Assert.AreEqual(GameVehicleArcade.defaultRiderStat, arcade.riderStats.power, 1e-6f);
            Assert.AreEqual(GameVehicleArcade.defaultRiderStat, arcade.riderStats.health, 1e-6f);
        }

        [Test]
        public void MissingStuntBlockIsOff() {
            StuntTuning t = VehicleTuningJson.ParseStunt(null);
            Assert.AreEqual(0, t.enabled);
            Assert.AreEqual(StuntTuning.Defaults().minAir, t.minAir, 1e-6f);
            Assert.AreEqual(0, VehicleTuningJson.ParseStunt(JsonMapper.ToObject("{}")).enabled);
        }

        [Test]
        public void ParsesAFullBlock() {
            JsonData block = JsonMapper.ToObject("{\"enabled\":1,\"minAir\":0.5,\"startTime\":0.2,\"rateMin\":1.1,"
                + "\"rateMax\":1.6,\"popMax\":1.3,\"recoverMax\":0.35,\"recoverMin\":0.25,\"graceMin\":0.3,"
                + "\"graceMax\":0.55,\"pointsPerSec\":110,\"landBonus\":60,\"toBoost\":0.003,\"assistMargin\":0.07}");
            StuntTuning t = VehicleTuningJson.ParseStunt(block);
            Assert.AreEqual(1, t.enabled);
            Assert.AreEqual(0.5f, t.minAir, 1e-6f);
            Assert.AreEqual(0.2f, t.startTime, 1e-6f);
            Assert.AreEqual(1.1f, t.rateMin, 1e-6f);
            Assert.AreEqual(1.6f, t.rateMax, 1e-6f);
            Assert.AreEqual(1.3f, t.popMax, 1e-6f);
            Assert.AreEqual(0.35f, t.recoverMax, 1e-6f);
            Assert.AreEqual(0.25f, t.recoverMin, 1e-6f);
            Assert.AreEqual(0.3f, t.graceMin, 1e-6f);
            Assert.AreEqual(0.55f, t.graceMax, 1e-6f);
            Assert.AreEqual(110f, t.pointsPerSec, 1e-4f);
            Assert.AreEqual(60f, t.landBonus, 1e-4f);
            Assert.AreEqual(0.003f, t.toBoost, 1e-7f);
            Assert.AreEqual(0.07f, t.assistMargin, 1e-6f);
        }

        [Test]
        public void StuntStatsDefaultTo02WhenTheProfileHasNoKeys() {
            GameProfile saved = GameProfiles.Current;
            GameProfiles.Current = new GameProfile();

            try {
                arcade.Init(Config(true));
                arcade.OnRiderMounted(riderGo);
                Assert.AreEqual(0.2f, arcade.riderStats.speed, 1e-6f);
                Assert.AreEqual(0.2f, arcade.riderStats.power, 1e-6f);
                Assert.AreEqual(0.2f, arcade.riderStats.health, 1e-6f);
            }
            finally {
                GameProfiles.Current = saved;
            }
        }

        [Test]
        public void StoredStuntStatsAreReadOnMountAndClamped() {
            GameProfile saved = GameProfiles.Current;
            GameProfile profile = new GameProfile();
            StuntStats s = new StuntStats();
            s.speed = 0.9f;
            s.power = 1.5f;
            s.health = 0f;
            GameStuntStatKeys.Write(profile, s);
            GameProfiles.Current = profile;

            try {
                arcade.Init(Config(true));
                arcade.OnRiderMounted(riderGo);
                Assert.AreEqual(0.9f, arcade.riderStats.speed, 1e-6f);
                Assert.AreEqual(1f, arcade.riderStats.power, 1e-6f);
                Assert.AreEqual(0f, arcade.riderStats.health, 1e-6f);
            }
            finally {
                GameProfiles.Current = saved;
            }
        }

        [Test]
        public void ParsesPopFlatDotAndDefaultsIt() {
            Assert.AreEqual(0.97f, VehicleTuningJson.ParseStunt(JsonMapper.ToObject("{\"enabled\":1}")).popFlatDot, 1e-6f);
            Assert.AreEqual(0.9f, VehicleTuningJson.ParseStunt(JsonMapper.ToObject("{\"popFlatDot\":0.9}")).popFlatDot, 1e-6f);
        }

        [Test]
        public void StuntsGoalCountsCleanStunts() {
            double v;
            Assert.IsTrue(GameRaceGoals.IsKind(GameRaceGoals.stunts));
            Assert.AreEqual(GameRaceGoals.compareAtLeast, GameRaceGoals.CompareFor(GameRaceGoals.stunts));
            GameRaceGoals.TallyStunt();
            GameRaceGoals.TallyStunt();
            Assert.IsTrue(GameRaceGoals.Value(GameRaceGoals.stunts, 1, out v));
            Assert.AreEqual(2.0, v);
            GameRaceGoals.Reset();
            Assert.AreEqual(0, GameRaceGoals.stuntsLanded);
        }
    }
}
#endif
