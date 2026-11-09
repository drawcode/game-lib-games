#if USE_AGNOSTIC_VEHICLE
using NUnit.Framework;
using UnityEngine;

namespace Game.Race.Tests {

    // GameVehicleArcadeStepper's pure parts: default off, registration is idempotent and removable, and a vehicle
    // with no stepper frame is never "driven" (so its own Update steps it).
    public class GameVehicleArcadeStepperTests {

        GameObject go;
        GameVehicleArcade arcade;

        [SetUp]
        public void SetUp() {
            go = new GameObject("stepper-test");
            arcade = go.AddComponent<GameVehicleArcade>();
            GameVehicleArcadeStepper.Remove(arcade);
        }

        [TearDown]
        public void TearDown() {
            GameVehicleArcadeStepper.Remove(arcade);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void DefaultsOff() {
            Assert.IsFalse(GameVehicleArcadeStepper.enabled);
            Assert.IsNull(GameVehicleArcadeStepper.onFixedStep);
        }

        [Test]
        public void AddIsIdempotentAndRemovable() {
            int before = GameVehicleArcadeStepper.count;
            GameVehicleArcadeStepper.Add(arcade);
            GameVehicleArcadeStepper.Add(arcade);
            Assert.AreEqual(before + 1, GameVehicleArcadeStepper.count);
            GameVehicleArcadeStepper.Remove(arcade);
            Assert.AreEqual(before, GameVehicleArcadeStepper.count);
        }

        [Test]
        public void NotDrivenWithoutARunnerFrame() {
            GameVehicleArcadeStepper.Add(arcade);
            Assert.IsFalse(GameVehicleArcadeStepper.Drives(arcade));
        }
    }
}
#endif
