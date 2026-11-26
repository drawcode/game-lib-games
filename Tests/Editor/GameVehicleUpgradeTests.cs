#if USE_AGNOSTIC_VEHICLE
using Agnostic.Vehicle;

using Engine.Data.Json;

using NUnit.Framework;
using UnityEngine;

namespace Game.Race.Tests {

    // D28 step 2: the instance tuning. Two arcades on one config, one upgraded: the config is untouched; no levels means
    // the instance equals the config bit for bit; the data.upgrades block parses (missing = x1) and applies.
    public class GameVehicleUpgradeTests {

        const string upgradesJson = "{\"stats\": {" +
            "\"speed\": {\"mul\": [1, 1.04, 1.08, 1.12, 1.16, 1.20], \"cost\": [0, 200, 400, 800, 1600, 3200]}," +
            "\"accel\": {\"mul\": [1, 1.04, 1.08, 1.12, 1.16, 1.20]}," +
            "\"handling\": {\"mul\": [1, 1.03, 1.06, 1.09, 1.12, 1.15]}," +
            "\"boost\": {\"mul\": [1, 1.05, 1.10, 1.15, 1.20, 1.25]}," +
            "\"air\": {\"mul\": [1, 1.04, 1.08, 1.12, 1.16, 1.20]}," +
            "\"toughness\": {\"mul\": [1, 1.05, 1.10, 1.15, 1.20, 1.25]}}}";

        GameObject goA;
        GameObject goB;
        GameVehicleArcade a;
        GameVehicleArcade b;

        [SetUp]
        public void SetUp() {
            goA = new GameObject("upgrade-test-a");
            goB = new GameObject("upgrade-test-b");
            a = goA.AddComponent<GameVehicleArcade>();
            b = goB.AddComponent<GameVehicleArcade>();
        }

        [TearDown]
        public void TearDown() {
            Object.DestroyImmediate(goA);
            Object.DestroyImmediate(goB);
        }

        GameVehicleArcadeConfig Config(bool withUpgrades) {
            GameVehicleArcadeConfig c = new GameVehicleArcadeConfig();
            c.code = "upgrade-test";
            c.tuning = VehicleTuning.Defaults();
            c.assist = DriveAssistTuning.Defaults();
            c.stunt = StuntTuning.Defaults();
            c.upgrades = withUpgrades ? GameVehicleArcadeData.ParseUpgrades(JsonMapper.ToObject(upgradesJson)) : new UpgradeTable();
            return c;
        }

        static UpgradeLevels Levels(int speed, int air, int toughness) {
            UpgradeLevels l = new UpgradeLevels();
            l.speed = speed;
            l.air = air;
            l.toughness = toughness;
            return l;
        }

        [Test]
        public void NoLevelsEqualsTheConfigBitForBit() {
            GameVehicleArcadeConfig c = Config(true);
            a.Init(c);
            Assert.AreEqual(c.tuning.maxSpeed, a.tuning.maxSpeed);
            Assert.AreEqual(c.tuning.jumpImpulse, a.tuning.jumpImpulse);
            Assert.AreEqual(c.tuning.bailImpact, a.tuning.bailImpact);
            Assert.AreEqual(c.tuning.grip, a.tuning.grip);
            Assert.AreEqual(c.tuning.accel.y3, a.tuning.accel.y3);
            Assert.AreEqual(c.tuning.steerRate.y3, a.tuning.steerRate.y3);
        }

        [Test]
        public void ZeroLevelsWithATableStillEqualsTheConfig() {
            GameVehicleArcadeConfig c = Config(true);
            a.Init(c);
            a.SetUpgrades(new UpgradeLevels());
            Assert.AreEqual(c.tuning.maxSpeed, a.tuning.maxSpeed);
            Assert.AreEqual(c.tuning.boostAccel, a.tuning.boostAccel);
        }

        [Test]
        public void UpgradedInstanceLeavesTheConfigAndTheOtherInstanceAlone() {
            GameVehicleArcadeConfig c = Config(true);
            float speed = c.tuning.maxSpeed;
            float jump = c.tuning.jumpImpulse;
            float bail = c.tuning.bailImpact;
            a.Init(c);
            b.Init(c);
            a.SetUpgrades(Levels(2, 5, 5));

            Assert.AreEqual(speed * 1.08f, a.tuning.maxSpeed, 1e-4f);
            Assert.AreEqual(jump * 1.20f, a.tuning.jumpImpulse, 1e-4f);
            Assert.AreEqual(bail * 1.25f, a.tuning.bailImpact, 1e-4f);
            Assert.AreEqual(speed, c.tuning.maxSpeed);
            Assert.AreEqual(jump, c.tuning.jumpImpulse);
            Assert.AreEqual(bail, c.tuning.bailImpact);
            Assert.AreEqual(speed, b.tuning.maxSpeed);
        }

        [Test]
        public void LevelsWithoutATableChangeNothing() {
            GameVehicleArcadeConfig c = Config(false);
            a.Init(c);
            a.SetUpgrades(Levels(3, 3, 3));
            Assert.AreEqual(c.tuning.maxSpeed, a.tuning.maxSpeed);
            Assert.AreEqual(c.tuning.jumpImpulse, a.tuning.jumpImpulse);
        }

        [Test]
        public void SetUpgradesBackToZeroRestoresTheConfig() {
            GameVehicleArcadeConfig c = Config(true);
            a.Init(c);
            a.SetUpgrades(Levels(5, 0, 0));
            a.SetUpgrades(new UpgradeLevels());
            Assert.AreEqual(c.tuning.maxSpeed, a.tuning.maxSpeed);
        }

        [Test]
        public void ParseUpgradesReadsTheMultipliersAndClampsPastTheTable() {
            UpgradeTable t = GameVehicleArcadeData.ParseUpgrades(JsonMapper.ToObject(upgradesJson));
            Assert.AreEqual(6, t.speed.count);
            Assert.AreEqual(1.08f, t.speed.Multiplier(2), 1e-6f);
            Assert.AreEqual(1.25f, t.toughness.Multiplier(99), 1e-6f);
            Assert.AreEqual(1f, t.handling.Multiplier(0), 1e-6f);
        }

        [Test]
        public void MissingBlockOrStatIsNoTable() {
            UpgradeTable none = GameVehicleArcadeData.ParseUpgrades(null);
            Assert.AreEqual(0, none.speed.count);
            Assert.AreEqual(1f, none.speed.Multiplier(4));

            UpgradeTable partial = GameVehicleArcadeData.ParseUpgrades(JsonMapper.ToObject("{\"stats\": {\"speed\": {\"mul\": [1, 1.5]}}}"));
            Assert.AreEqual(2, partial.speed.count);
            Assert.AreEqual(0, partial.air.count);
            Assert.AreEqual(1f, partial.air.Multiplier(3));
        }

        [Test]
        public void RowParseCarriesTheUpgrades() {
            string row = "{\"code\": \"upgrade-row\", \"data\": {\"tuning\": {}, \"upgrades\": " + upgradesJson + "}}";
            GameVehicleArcadeConfig c = GameVehicleArcadeData.Parse(JsonMapper.ToObject(row));
            Assert.IsNotNull(c);
            Assert.AreEqual(6, c.upgrades.accel.count);

            string bare = "{\"code\": \"upgrade-bare\", \"data\": {\"tuning\": {}}}";
            Assert.AreEqual(0, GameVehicleArcadeData.Parse(JsonMapper.ToObject(bare)).upgrades.accel.count);
        }
    }
}
#endif
