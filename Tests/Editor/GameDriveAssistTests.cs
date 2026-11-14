#if USE_AGNOSTIC_VEHICLE
using Agnostic.Vehicle;

using Engine.AgnosticHost;
using Engine.Data.Json;

using NUnit.Framework;
using UnityEngine;

namespace Game.Race.Tests {

    // The D20 drive assist seam: the data block, the default-null arcade, and the profile gate.
    public class GameDriveAssistTests {

        GameObject go;
        GameVehicleArcade arcade;

        [SetUp]
        public void SetUp() {
            go = new GameObject("drive-assist-test");
            arcade = go.AddComponent<GameVehicleArcade>();
        }

        [TearDown]
        public void TearDown() {
            Object.DestroyImmediate(go);
        }

        [Test]
        public void NullBlockIsOff() {
            Assert.AreEqual(0, VehicleTuningJson.ParseAssist(null).enabled);
        }

        [Test]
        public void ParsesABikesBlockInRadians() {
            JsonData block = JsonMapper.ToObject("{\"enabled\":1,\"wrongWayStartDeg\":100,\"offLineShare\":0.35}");
            DriveAssistTuning a = VehicleTuningJson.ParseAssist(block);
            Assert.AreEqual(1, a.enabled);
            Assert.AreEqual(100f * VehicleMath.degToRad, a.wrongWayStart, 1e-6f);
            Assert.AreEqual(0.35f, a.offLineShare, 1e-6f);
            Assert.AreEqual(DriveAssistTuning.Defaults().rampIn, a.rampIn, 1e-6f);
        }

        [Test]
        public void FreshArcadeHasNoAssist() {
            Assert.IsNull(arcade.assist);
        }

        [Test]
        public void ProfileOffMeansNotWanted() {
            DriveAssistTuning on = DriveAssistTuning.Defaults();
            on.enabled = 1;
            Assert.IsFalse(GameVehicleArcade.AssistWanted(GameVehicleDriveAssistModes.off, in on));
            Assert.IsTrue(GameVehicleArcade.AssistWanted(GameVehicleDriveAssistModes.on, in on));
            Assert.IsTrue(GameVehicleArcade.AssistWanted("", in on));
            Assert.IsFalse(GameVehicleArcade.AssistWanted("", DriveAssistTuning.Defaults()));
        }
    }
}
#endif
