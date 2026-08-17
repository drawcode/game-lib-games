using NUnit.Framework;

using UnityEngine;

namespace Game.Actor.Tests {

    // A projectile applies damage by looking up GameDamageManager with GetComponent on the
    // exact collider it struck (GameDamage.HandleApplyDamage). So every hit area must end up
    // with its OWN manager, bound to the actor above it.
    //
    // Two ways that used to break, both pinned here:
    //   - only the first GamePlayerCollision found on a character was wired, so a character
    //     with more than one hit area had dead ones (character-zombie-1 has Helmet + Facemask)
    //   - GetOrSet searches children, so a manager sitting on a child could be handed back
    //     instead of one being added to the hit area itself, leaving it unreachable
    public class GamePlayerHitAreaWiringTests {

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

        GameObject AddHitArea(string name) {

            GameObject hitArea = new GameObject(name);
            hitArea.transform.parent = controller.gamePlayerModelHolderModel.transform;
            hitArea.AddComponent<GamePlayerCollision>();

            return hitArea;
        }

        void AssertWired(GameObject hitArea) {

            GameDamageManager manager = hitArea.GetComponent<GameDamageManager>();

            Assert.IsNotNull(manager,
                "hit area '" + hitArea.name + "' has no GameDamageManager on itself, so damage landing on it is lost");
            Assert.AreSame(controller, manager.gamePlayerController,
                "hit area '" + hitArea.name + "' is not bound to the actor above it");
        }

        [Test]
        public void SingleHitArea_IsWiredToTheOwningActor() {

            GameObject hitArea = AddHitArea("GamePlayerCollider");

            hitArea.GetComponent<GamePlayerCollision>().UpdateGameObjects();

            AssertWired(hitArea);
        }

        [Test]
        public void EverySiblingHitArea_GetsItsOwnManager() {

            // The zombie-1 shape: two hit areas side by side under one holder.
            GameObject helmet = AddHitArea("Helmet");
            GameObject facemask = AddHitArea("Facemask");

            foreach (GamePlayerCollision collision
                    in actor.GetComponentsInChildren<GamePlayerCollision>(true)) {
                collision.UpdateGameObjects();
            }

            AssertWired(helmet);
            AssertWired(facemask);

            Assert.AreNotSame(helmet.GetComponent<GameDamageManager>(),
                facemask.GetComponent<GameDamageManager>(),
                "sibling hit areas must not share one manager");
        }

        [Test]
        public void HitArea_GetsItsOwnManager_EvenWhenAChildAlreadyHasOne() {

            GameObject hitArea = AddHitArea("GamePlayerCollider");

            // A manager further down the hierarchy must not satisfy the hit area's own
            // lookup: GetComponent on the collider would never see it.
            GameObject decoration = new GameObject("decoration");
            decoration.transform.parent = hitArea.transform;
            GameDamageManager childManager = decoration.AddComponent<GameDamageManager>();

            hitArea.GetComponent<GamePlayerCollision>().UpdateGameObjects();

            AssertWired(hitArea);
            Assert.AreNotSame(childManager, hitArea.GetComponent<GameDamageManager>(),
                "the hit area must not adopt a manager that lives on a child");
        }

        [Test]
        public void Rewiring_DoesNotAddASecondManager() {

            // Characters are reloaded onto pooled actors, so UpdateGameObjects runs again on
            // an object that may already carry a manager from a previous life.
            GameObject hitArea = AddHitArea("GamePlayerCollider");
            GamePlayerCollision collision = hitArea.GetComponent<GamePlayerCollision>();

            collision.UpdateGameObjects();
            collision.UpdateGameObjects();

            Assert.AreEqual(1, hitArea.GetComponents<GameDamageManager>().Length);
            AssertWired(hitArea);
        }
    }
}
