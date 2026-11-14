#if USE_AGNOSTIC_VEHICLE
using Agnostic.Core;
using Agnostic.Framing;
using Agnostic.Vehicle;

using Engine.Data.Json;

using NUnit.Framework;
using UnityEngine;

namespace Game.Race.Tests {

    // The iso camera host (D21): the mode choice, the data.camera parse, and the rig's Apply and snap behaviour. No
    // running race is needed: the rig is driven with Tick.
    public class GameCameraIsoRigTests {

        GameObject camGo;
        GameObject bikeGo;
        Camera cam;
        GameIsoCameraSmoothFollow follower;
        GameVehicleArcade arcade;
        GameCameraIsoRig rig;
        FrameTuning on;

        [SetUp]
        public void SetUp() {
            camGo = new GameObject("iso-rig-test-camera");
            cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 14f;
            cam.nearClipPlane = -300f;
            cam.farClipPlane = 100f;
            camGo.transform.SetPositionAndRotation(new Vector3(1f, 20f, -30f), Quaternion.Euler(30f, 0f, 0f));
            follower = camGo.AddComponent<GameIsoCameraSmoothFollow>();
            bikeGo = new GameObject("iso-rig-test-bike");
            arcade = bikeGo.AddComponent<GameVehicleArcade>();
            rig = GameCameraIsoRig.Ensure(cam);
            on = FrameTuning.Defaults();
            on.enabled = 1;
        }

        [TearDown]
        public void TearDown() {
            Object.DestroyImmediate(camGo);
            Object.DestroyImmediate(bikeGo);
        }

        [Test]
        public void ResolvePicksAndFallsBack() {
            Assert.AreEqual(GameCameraModes.iso, GameCameraModes.Resolve("", GameCameraModes.iso));
            Assert.AreEqual(GameCameraModes.follow, GameCameraModes.Resolve("", GameCameraModes.follow));
            Assert.AreEqual(GameCameraModes.follow, GameCameraModes.Resolve(GameCameraModes.follow, GameCameraModes.iso));
            Assert.AreEqual(GameCameraModes.iso, GameCameraModes.Resolve(GameCameraModes.iso, GameCameraModes.follow));
            Assert.AreEqual(GameCameraModes.iso, GameCameraModes.Resolve("junk", GameCameraModes.iso));
            Assert.AreEqual(GameCameraModes.follow, GameCameraModes.Resolve("junk", "junk"));
        }

        [Test]
        public void MissingBlockParsesToDefaultsOff() {
            JsonData row = JsonMapper.ToObject("{\"code\":\"t\",\"data\":{\"tuning\":{}}}");
            GameVehicleArcadeConfig c = GameVehicleArcadeData.Parse(row);
            Assert.IsNotNull(c);
            Assert.AreEqual(0, c.camera.enabled);
            Assert.AreEqual(GameCameraModes.follow, c.cameraMode);
        }

        [Test]
        public void BlockParsesDegreesToRadiansWithCoreYaw() {
            JsonData row = JsonMapper.ToObject("{\"code\":\"t\",\"data\":{\"tuning\":{},\"camera\":{\"mode\":\"iso\"," +
                "\"yawDeg\":45,\"pitchDeg\":40,\"sizeMax\":28,\"inset\":{\"b\":0.25}}}}");
            GameVehicleArcadeConfig c = GameVehicleArcadeData.Parse(row);
            Assert.AreEqual(1, c.camera.enabled);
            Assert.AreEqual(GameCameraModes.iso, c.cameraMode);
            Assert.AreEqual(-Mathf.PI * 0.25f, c.camera.yaw, 1e-5f);
            Assert.AreEqual(40f * Mathf.Deg2Rad, c.camera.pitch, 1e-5f);
            Assert.AreEqual(28f, c.camera.sizeMax, 1e-5f);
            Assert.AreEqual(0.25f, c.camera.insetBottom, 1e-5f);
            Assert.AreEqual(0.06f, c.camera.insetLeft, 1e-5f);
        }

        [Test]
        public void ApplyIsoWithoutABlockIsANoOp() {
            Vector3 position = camGo.transform.position;
            rig.Apply(GameCameraModes.iso, FrameTuning.Defaults(), arcade);
            Assert.IsFalse(rig.isIso);
            Assert.IsTrue(follower.enabled);
            Assert.IsNull(arcade.cameraTransform);
            Assert.AreEqual(position, camGo.transform.position);
        }

        [Test]
        public void ApplyIsoThenFollowRestoresTheCamera() {
            Vector3 position = camGo.transform.position;
            Quaternion rotation = camGo.transform.rotation;
            rig.Apply(GameCameraModes.iso, on, arcade);
            Assert.IsTrue(rig.isIso);
            Assert.IsFalse(follower.enabled);

            for (int i = 0; i < 40; i++) {
                rig.Tick(1f / 60f, 0);
            }

            Assert.AreNotEqual(position, camGo.transform.position);
            rig.Apply(GameCameraModes.follow, on, arcade);
            Assert.IsFalse(rig.isIso);
            Assert.IsTrue(follower.enabled);
            Assert.AreEqual(position, camGo.transform.position);
            Assert.AreEqual(0f, Quaternion.Angle(rotation, camGo.transform.rotation), 1e-3f);
            Assert.IsTrue(cam.orthographic);
            Assert.AreEqual(14f, cam.orthographicSize, 1e-5f);
            Assert.AreEqual(-300f, cam.nearClipPlane, 1e-5f);
            Assert.AreEqual(100f, cam.farClipPlane, 1e-5f);
        }

        [Test]
        public void SteerFrameIsSetInIsoAndClearedInFollow() {
            rig.Apply(GameCameraModes.iso, on, arcade);
            Assert.IsNotNull(rig.steerFrame);
            Assert.AreSame(rig.steerFrame, arcade.cameraTransform);
            Assert.AreEqual(-Mathf.PI * 0.25f, arcade.CameraYaw(), 1e-4f);
            rig.Apply(GameCameraModes.follow, on, arcade);
            Assert.IsNull(arcade.cameraTransform);
        }

        [Test]
        public void SteerFrameKeepsTheIsoYawThroughTheBlend() {
            rig.Apply(GameCameraModes.iso, on, arcade);
            Assert.AreEqual(45f, SteerYaw(), 0.01f);

            for (int i = 0; i < 6; i++) {
                rig.Tick(0.05f, 0);
                Assert.AreEqual(45f, SteerYaw(), 0.01f, "tick " + i);
            }

            Assert.IsFalse(rig.steerFrame.IsChildOf(camGo.transform));
        }

        float SteerYaw() {
            Vector3 f = rig.steerFrame.forward;
            f.y = 0f;
            return Vector3.SignedAngle(Vector3.forward, f, Vector3.up);
        }

        [Test]
        public void RespawnCountChangeSnaps() {
            arcade.state.position = new Vec3(0f, 0f, 0f);
            rig.Apply(GameCameraModes.iso, on, arcade);
            rig.Tick(1f / 60f, 0);
            int before = rig.snaps;
            arcade.state.position = new Vec3(3f, 0f, 0f);   // under snapDistance, so only the respawn can snap
            rig.Tick(1f / 60f, 0);
            Assert.AreEqual(before, rig.snaps);
            arcade.state.position = new Vec3(3f, 0f, 1f);
            rig.Tick(1f / 60f, 1);
            Assert.AreEqual(before + 1, rig.snaps);
        }

        [Test]
        public void FocusJumpCountsASnap() {
            arcade.state.position = new Vec3(0f, 0f, 0f);
            rig.Apply(GameCameraModes.iso, on, arcade);
            rig.Tick(1f / 60f, 0);
            int before = rig.snaps;
            arcade.state.position = new Vec3(40f, 0f, 0f);  // past snapDistance with the same respawn count
            rig.Tick(1f / 60f, 0);
            Assert.AreEqual(before + 1, rig.snaps);
        }
    }
}
#endif
