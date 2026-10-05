using NUnit.Framework;

using UnityEngine;

namespace Game.Actor.Tests {

    // Actors are pooled, and the local player shares the GamePlayerObject prefab with agents
    // (BaseGameController.loadActorCo), so a recycled instance can come back carrying the
    // local player's uniqueId while being set up as an agent.
    //
    // Reset() is the method that ASSIGNS uniqueId, so it must not ask a question whose answer
    // depends on the uniqueId already there. When it did, a recycled ex-player re-affirmed the
    // player id forever: GameDamageManager.ApplyDamage skips player-controlled targets, and
    // Remove() early-returns for them, so that actor was both unkillable and unremovable.
    //
    // IsPlayerControlled deliberately still counts a uniqueId match, because ~120 call sites
    // rely on that meaning. Only the assignment site uses the state-only form.
    public class GamePlayerIdentityTests {

        GameObject actor;
        GamePlayerController controller;

        [SetUp]
        public void SetUp() {

            actor = new GameObject("test-actor");
            controller = actor.AddComponent<GamePlayerController>();

            GameObject modelHolder = new GameObject("model-holder");
            modelHolder.transform.parent = actor.transform;
            controller.gamePlayerModelHolderModel = modelHolder;
        }

        [TearDown]
        public void TearDown() {

            if (actor != null) {
                Object.DestroyImmediate(actor);
            }
        }

        void MakeAgent() {

            controller.controllerState = GamePlayerControllerState.ControllerAgent;
            controller.contextState = GamePlayerContextState.ContextFollowAgentAttack;
        }

        void MakePlayer() {

            controller.controllerState = GamePlayerControllerState.ControllerPlayer;
            controller.contextState = GamePlayerContextState.ContextInput;
        }

        // ------------------------------------------------------------------
        // STATE-ONLY PREDICATE

        [Test]
        public void IsPlayerControlledState_IsFalseForAnAgentHoldingThePlayerUniqueId() {

            MakeAgent();
            controller.uniqueId = UniqueUtil.Instance.currentUniqueId;

            Assert.IsFalse(controller.IsPlayerControlledState,
                "state decides identity; a stale uniqueId must not make an agent look player controlled");
        }

        [Test]
        public void IsPlayerControlled_StillCountsAUniqueIdMatch() {

            MakeAgent();
            controller.uniqueId = UniqueUtil.Instance.currentUniqueId;

            Assert.IsTrue(controller.IsPlayerControlled,
                "IsPlayerControlled must keep its existing meaning for the call sites that read it");
        }

        [Test]
        public void IsPlayerControlledState_IsTrueForPlayerState() {

            MakePlayer();
            controller.uniqueId = "not-the-player-uuid";

            Assert.IsTrue(controller.IsPlayerControlledState);
        }

        // ------------------------------------------------------------------
        // RESET AS THE EDITOR CALLBACK

        [Test]
        public void AddingTheComponent_DoesNotThrow() {

            // Reset() is also Unity's editor Reset callback, so it fires on AddComponent
            // before any holder is wired up. It must survive that.
            GameObject bare = new GameObject("bare-actor");

            try {
                Assert.DoesNotThrow(() => bare.AddComponent<GamePlayerController>());
            }
            finally {
                Object.DestroyImmediate(bare);
            }
        }

        // ------------------------------------------------------------------
        // RESET: the assignment site

        [Test]
        public void Reset_GivesARecycledExPlayerAFreshIdWhenItComesBackAsAnAgent() {

            string playerUniqueId = UniqueUtil.Instance.currentUniqueId;

            // The pooled instance still carries the id from its previous life as the player.
            controller.uniqueId = playerUniqueId;
            MakeAgent();

            controller.Reset();

            Assert.AreNotEqual(playerUniqueId, controller.uniqueId,
                "an agent must not keep the local player's uniqueId across pooled reuse");
            Assert.IsFalse(string.IsNullOrEmpty(controller.uniqueId));
            Assert.IsFalse(controller.IsPlayerControlled,
                "after Reset the recycled agent must no longer read as player controlled");
        }

        [Test]
        public void Reset_KeepsThePlayerIdentityForTheRealPlayer() {

            controller.uniqueId = "some-stale-uuid";
            MakePlayer();

            controller.Reset();

            Assert.AreEqual(UniqueUtil.Instance.currentUniqueId, controller.uniqueId,
                "the player actor must take the local player's uniqueId");
            Assert.IsTrue(controller.IsPlayerControlled);
        }

        [Test]
        public void Reset_GivesDistinctIdsToSeparatelyRecycledAgents() {

            MakeAgent();
            controller.Reset();
            string first = controller.uniqueId;

            controller.Reset();

            Assert.AreNotEqual(first, controller.uniqueId, "each agent life gets its own id");
        }
    }
}
