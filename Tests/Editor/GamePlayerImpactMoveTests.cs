using NUnit.Framework;

using UnityEngine;

namespace Game.Actor.Tests {

    // CharacterController.Move DEPENETRATES on every call, even when handed zero motion. Every
    // actor carries the same oversized capsule (radius 1.88, height 4.88), so calling Move every
    // frame shoves the actor out of whatever its capsule overlaps.
    //
    // While an enemy is alive the NavMeshAgent re-pins the transform each frame and the drift is
    // invisible. On death StopAgent() disables the agent, and enemies never get a
    // GamePlayerThirdPersonController, so nothing applies gravity and nothing re-pins them: the
    // drift accumulates permanently and the corpse creeps up off the ground while its
    // ground-projected shadow stays behind. Measured live: a dying actor climbed from y=0.083 to
    // y=6.13 with impact exactly (0,0,0), and the drift stopped when the capsule was disabled.
    //
    // These pin the guard that keeps Move off unless there is real motion to apply.
    public class GamePlayerImpactMoveTests {

        GameObject actor;
        GamePlayerController controller;

        [SetUp]
        public void SetUp() {

            actor = new GameObject("test-actor");
            controller = actor.AddComponent<GamePlayerController>();

            GameObject modelHolder = new GameObject("model-holder");
            modelHolder.transform.parent = actor.transform;
            controller.gamePlayerModelHolderModel = modelHolder;

            controller.SetControllerData(new GamePlayerControllerData());
            controller.SetRuntimeData(new GamePlayerRuntimeData());

            // alive by default: health above zero and not dying
            controller.runtimeData.health = 1f;
            controller.currentControllerData.dying = false;
            controller.currentControllerData.actorExiting = false;
        }

        [TearDown]
        public void TearDown() {

            if (actor != null) {
                Object.DestroyImmediate(actor);
            }
        }

        [Test]
        public void NoImpact_DoesNotMove() {

            controller.currentControllerData.impact = Vector3.zero;

            Assert.IsFalse(controller.ShouldApplyImpactMove(),
                "an idle actor must not call Move, or its capsule depenetrates every frame");
        }

        [Test]
        public void RealImpact_WhileAlive_Moves() {

            controller.currentControllerData.impact = new Vector3(0f, 0f, 5f);

            Assert.IsTrue(controller.ShouldApplyImpactMove(),
                "a live actor with an impact still has to be moved by it");
        }

        [Test]
        public void ImpactBelowThreshold_DoesNotMove() {

            // impact decays by Lerp, which approaches zero without reaching it
            controller.currentControllerData.impact = Vector3.one * .0001f;

            Assert.IsFalse(controller.ShouldApplyImpactMove(),
                "a decayed impact must stop producing Move calls");
        }

        [Test]
        public void DyingActor_DoesNotMove() {

            controller.currentControllerData.impact = new Vector3(0f, 0f, 5f);
            controller.currentControllerData.dying = true;

            Assert.IsTrue(controller.isDead);
            Assert.IsFalse(controller.ShouldApplyImpactMove(),
                "a corpse must not be depenetrated: nothing re-pins or lowers a dead enemy");
        }

        [Test]
        public void ZeroHealthActor_DoesNotMove() {

            controller.currentControllerData.impact = new Vector3(0f, 0f, 5f);
            controller.runtimeData.health = 0f;

            Assert.IsTrue(controller.isDead);
            Assert.IsFalse(controller.ShouldApplyImpactMove());
        }

        [Test]
        public void ExitingActor_DoesNotMove() {

            controller.currentControllerData.impact = new Vector3(0f, 0f, 5f);
            controller.currentControllerData.actorExiting = true;

            Assert.IsFalse(controller.ShouldApplyImpactMove(),
                "matches the existing AddImpact guard, which refuses impact while exiting");
        }

        [Test]
        public void NullControllerData_DoesNotMove() {

            controller.SetControllerData(null);
            controller.controllerData = null;

            Assert.IsFalse(controller.ShouldApplyImpactMove());
        }
    }
}
