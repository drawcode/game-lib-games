using NUnit.Framework;

using UnityEngine;

namespace Game.Actor.Tests {

    // Every actor used to be given the SAME movement capsule -- radius 1.88, height 4.88,
    // centre y 2.39 -- whatever character it was wearing. Measured against the shipped
    // models that is about 4x too big for a droid and too short for the boss, and an
    // oversized capsule is not cosmetic: CharacterController depenetrates against it, so a
    // small enemy shoves the player from well outside its own silhouette.
    //
    // The size now comes off the character record (capsule_radius/height/center_y) and is
    // OPTIONAL, so this pins the two halves that matter:
    //   - a character with no authored capsule must keep the controller's own defaults,
    //     which is what makes the change additive against data nobody has updated
    //   - an authored capsule shorter than its own diameter must be raised HERE, because
    //     Unity would otherwise clamp it silently and hand back a size nobody asked for
    //
    // The authored numbers themselves are tuning, not behaviour, so they are deliberately
    // not asserted -- this test must not have to change every time a character is retuned.
    public class GamePlayerCapsuleSizeTests {

        GameObject actor;
        GamePlayerController controller;

        double authoredRadius;
        double authoredHeight;
        double authoredCenterY;
        string restoreCode;

        [SetUp]
        public void SetUp() {

            actor = new GameObject("test-actor");
            controller = actor.AddComponent<GamePlayerController>();

            controller.SetControllerData(new GamePlayerControllerData());
            controller.currentControllerData.characterController
                = actor.AddComponent<CharacterController>();
        }

        [TearDown]
        public void TearDown() {

            // The character records are a loaded singleton shared with every other test,
            // so anything written onto one has to be put back.
            if (restoreCode != null) {

                GameCharacter character = GameCharacters.Instance.GetById(restoreCode);

                if (character != null && character.data != null) {
                    character.data.capsule_radius = authoredRadius;
                    character.data.capsule_height = authoredHeight;
                    character.data.capsule_center_y = authoredCenterY;
                }

                restoreCode = null;
            }

            if (actor != null) {
                Object.DestroyImmediate(actor);
            }
        }

        /// <summary>
        /// Point the actor at a real character record and overwrite its capsule for the
        /// duration of one test. Returns null when there is no character data loaded at
        /// all, which is the one condition these tests cannot make an assertion about.
        /// </summary>
        GameCharacter UseCharacter(double radius, double height, double centerY) {

            foreach (GameCharacter character in GameCharacters.Instance.GetAll()) {

                if (character == null || character.data == null) {
                    continue;
                }

                restoreCode = character.code;
                authoredRadius = character.data.capsule_radius;
                authoredHeight = character.data.capsule_height;
                authoredCenterY = character.data.capsule_center_y;

                character.data.capsule_radius = radius;
                character.data.capsule_height = height;
                character.data.capsule_center_y = centerY;

                controller.characterCode = character.code;

                return character;
            }

            return null;
        }

        [Test]
        public void NoAuthoredCapsule_KeepsTheControllerDefaults() {

            GameCharacter character = UseCharacter(0, 0, 0);

            if (character == null) {
                Assert.Ignore("no character data loaded");
            }

            controller.characterRadius = 1.88f;
            controller.characterHeight = 4.88f;
            controller.characterCenter = new Vector3(0f, 2.39f, 0f);

            controller.ApplyCharacterCapsule();

            CharacterController capsule = controller.currentControllerData.characterController;

            Assert.AreEqual(1.88f, capsule.radius, .001f,
                "an unauthored character must keep the controller's own radius");
            Assert.AreEqual(4.88f, capsule.height, .001f,
                "an unauthored character must keep the controller's own height");
            Assert.AreEqual(2.39f, capsule.center.y, .001f,
                "an unauthored character must keep the controller's own centre");
        }

        [Test]
        public void AuthoredCapsule_ReplacesTheControllerDefaults() {

            GameCharacter character = UseCharacter(.6, 1.8, .9);

            if (character == null) {
                Assert.Ignore("no character data loaded");
            }

            controller.characterRadius = 1.88f;
            controller.characterHeight = 4.88f;
            controller.characterCenter = new Vector3(0f, 2.39f, 0f);

            controller.ApplyCharacterCapsule();

            CharacterController capsule = controller.currentControllerData.characterController;

            Assert.AreEqual(.6f, capsule.radius, .001f);
            Assert.AreEqual(1.8f, capsule.height, .001f);
            Assert.AreEqual(.9f, capsule.center.y, .001f);
        }

        [Test]
        public void CapsuleShorterThanItsOwnDiameter_IsRaisedToTheDiameter() {

            // Unity clamps this case itself, without saying so, which would leave the
            // authored value and the live value quietly disagreeing.
            GameCharacter character = UseCharacter(1.0, 1.2, .6);

            if (character == null) {
                Assert.Ignore("no character data loaded");
            }

            controller.ApplyCharacterCapsule();

            CharacterController capsule = controller.currentControllerData.characterController;

            Assert.AreEqual(2.0f, capsule.height, .001f,
                "height must be raised to 2x radius before it reaches Unity");
        }

        [Test]
        public void NoCharacterController_IsNotAnError() {

            // InitControlsCo can run before anything has a capsule to size.
            controller.currentControllerData.characterController = null;

            Assert.DoesNotThrow(() => controller.ApplyCharacterCapsule());
        }
    }
}
