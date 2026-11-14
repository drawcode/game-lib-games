#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using Agnostic.Core;
using Agnostic.Framing;
using Agnostic.Vehicle;

using Engine.AgnosticHost;

using UnityEngine;

// The isometric camera host (D21, plan-action-bikes-iso-camera.md). The pure solve (Agnostic.Framing.CameraFraming)
// decides where to look and how much to show; this owns the transform, the occlusion cast, the blend between modes
// and the steering frame.
//
// - Apply(mode, arcade): iso saves the camera's pose, projection, size, clip planes and the follower's enabled flag,
//   disables GameIsoCameraSmoothFollow and blends to the iso pose over switchBlend. follow restores all of it. A row
//   with no data.camera block (enabled 0) never goes iso, so Action Bots and old rows keep today's camera.
// - Steering frame: in iso the arcade's cameraTransform is the rig's own steerFrame child, already turned to the
//   destination pose, so a point-to-steer stick frame changes once on the switch and not over the blend. follow
//   resets it to null (= Camera.main).
// - Snaps: the first frame, a new arcade, a new round and GameRace.playerRespawns changing jump with no smoothing.
// - Statics: Bikes enters Play without a domain reload; the registration below runs again on every Play.
[DefaultExecutionOrder(100)]
public class GameCameraIsoRig : MonoBehaviour {

    public const string steerFrameName = "iso-steer-frame";

    public static GameCameraIsoRig Current { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        Current = null;
    }

    private const int pointCapacity = 96;
    private const int hitCapacity = 8;

    public bool isIso { get; private set; }
    public Transform steerFrame { get; private set; }
    public FrameTuning tuning { get { return frame; } }
    public FrameMemory memory { get { return mem; } }
    public int snaps { get; private set; }    // frames that snapped, for tests and the probe

    private Camera cam;
    private GameIsoCameraSmoothFollow follower;
    private GameVehicleArcade arcade;
    private FrameTuning frame = FrameTuning.Defaults();
    private FrameMemory mem;

    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private bool savedOrthographic;
    private float savedSize;
    private float savedFov;
    private float savedNear;
    private float savedFar;
    private bool savedFollowerEnabled;
    private readonly List<Camera> children = new List<Camera>();
    private readonly List<float> childSizes = new List<float>();

    private bool snapNext;
    private int lastRespawns;
    private bool lastRaceActive;
    private float blend = 1f;
    private Vector3 blendFromPosition;
    private Quaternion blendFromRotation;
    private float blendFromSize;

    private readonly Vec3[] points = new Vec3[pointCapacity];
    private readonly Vec3[] trackPoints = new Vec3[pointCapacity - 5];
    private readonly RaycastHit[] hits = new RaycastHit[hitCapacity];

    // Adds the rig to the camera, or finds the one it has.
    public static GameCameraIsoRig Ensure(Camera cam) {

        if (cam == null) {
            return Current;
        }

        GameCameraIsoRig rig = cam.GetComponent<GameCameraIsoRig>();

        if (rig == null) {
            rig = cam.gameObject.AddComponent<GameCameraIsoRig>();
        }

        rig.cam = cam;
        Current = rig;
        return rig;
    }

    void Awake() {
        cam = GetComponent<Camera>();
        follower = GetComponent<GameIsoCameraSmoothFollow>();
        Current = this;
    }

    void OnEnable() {
        Current = this;
    }

    void OnDestroy() {

        if (Current == this) {
            Current = null;
        }
    }

    // mode is GameCameraModes.iso or follow (anything else counts as follow). The tuning comes from the arcade's
    // data row.
    public void Apply(string mode, GameVehicleArcade arcade) {
        Apply(mode, arcade != null && arcade.config != null ? arcade.config.camera : FrameTuning.Defaults(), arcade);
    }

    public void Apply(string mode, FrameTuning tuning, GameVehicleArcade arcade) {

        if (cam == null) {
            cam = GetComponent<Camera>();
        }

        if (follower == null) {
            follower = GetComponent<GameIsoCameraSmoothFollow>();
        }

        bool wantIso = mode == GameCameraModes.iso && tuning.enabled != 0 && cam != null && arcade != null;

        if (!wantIso) {
            Leave();
            return;
        }

        bool entering = !isIso;
        GameVehicleArcade previous = this.arcade;

        if (previous != null && previous != arcade && previous.cameraTransform == steerFrame) {
            previous.cameraTransform = null;
        }

        this.arcade = arcade;
        frame = tuning;
        snapNext = true;

        if (entering) {
            Save();
            blend = frame.switchBlend > 1e-4f ? 0f : 1f;
            blendFromPosition = savedPosition;
            blendFromRotation = savedRotation;
            blendFromSize = savedSize;

            if (follower != null) {
                follower.enabled = false;
            }

            isIso = true;
        }

        EnsureSteerFrame();
        TurnSteerFrame();
        arcade.cameraTransform = steerFrame;
        lastRespawns = GameRace.playerRespawns;
        lastRaceActive = GameRace.active;
    }

    private void Leave() {

        if (!isIso) {
            return;
        }

        isIso = false;

        if (arcade != null && arcade.cameraTransform == steerFrame) {
            arcade.cameraTransform = null;
        }

        arcade = null;
        Restore();

        if (follower != null) {
            follower.enabled = savedFollowerEnabled;
        }

        mem = default(FrameMemory);
    }

    private void Save() {

        savedPosition = cam.transform.position;
        savedRotation = cam.transform.rotation;
        savedOrthographic = cam.orthographic;
        savedSize = cam.orthographicSize;
        savedFov = cam.fieldOfView;
        savedNear = cam.nearClipPlane;
        savedFar = cam.farClipPlane;
        savedFollowerEnabled = follower != null && follower.enabled;
        children.Clear();
        childSizes.Clear();

        foreach (Camera child in GetComponentsInChildren<Camera>(true)) {
            if (child != cam) {
                children.Add(child);
                childSizes.Add(child.orthographicSize);
            }
        }
    }

    private void Restore() {

        cam.transform.SetPositionAndRotation(savedPosition, savedRotation);
        cam.orthographic = savedOrthographic;
        cam.orthographicSize = savedSize;
        cam.fieldOfView = savedFov;
        cam.nearClipPlane = savedNear;
        cam.farClipPlane = savedFar;

        for (int i = 0; i < children.Count; i++) {
            if (children[i] != null) {
                children[i].orthographicSize = childSizes[i];
            }
        }
    }

    private void EnsureSteerFrame() {

        if (steerFrame != null) {
            return;
        }

        Transform existing = transform.Find(steerFrameName);
        steerFrame = existing != null ? existing : new GameObject(steerFrameName).transform;
        steerFrame.SetParent(transform, false);
    }

    // The iso orientation at the base pitch, in world space.
    private Quaternion IsoRotation(float pitch) {

        FrameBasis b = CameraFraming.Basis(frame.yaw, pitch);
        return Quaternion.LookRotation(UnityFrame.ToUnity(b.forward), UnityFrame.ToUnity(b.up));
    }

    private void TurnSteerFrame() {

        if (steerFrame != null) {
            steerFrame.rotation = IsoRotation(frame.pitch);
        }
    }

    void LateUpdate() {
        Tick(Time.deltaTime, GameRace.playerRespawns);
    }

    // One frame of the rig. Public so a test can drive it without a running race.
    public void Tick(float dt, int respawns) {

        if (!isIso || cam == null || arcade == null) {
            return;
        }

        bool raceActive = GameRace.active;
        int snap = snapNext || respawns != lastRespawns || (raceActive && !lastRaceActive) ? 1 : 0;
        snapNext = false;
        lastRespawns = respawns;
        lastRaceActive = raceActive;

        VehicleState s = arcade.state;
        bool grounded = (s.flags & (int)VehicleFlags.grounded) != 0;
        float pitchNow = mem.valid != 0 ? mem.pitch : frame.pitch;
        FrameBasis basis = CameraFraming.Basis(frame.yaw, pitchNow);

        int n = CameraFraming.BikePoints(s.position, in basis, frame.bikeRadius, points, 0);
        bool track = false;

        if (raceActive && GameRace.track != null) {
            track = true;
            float ahead = CameraFraming.AheadDistance(s.speed, in frame);
            int count = CameraFraming.TrackAhead(GameRace.track, GameRace.Racer(GameRace.playerIndex).s, ahead,
                frame.sampleStep, frame.edgeShare, trackPoints);

            for (int i = 0; i < count && n < pointCapacity; i++) {
                points[n++] = trackPoints[i];
            }

            if (frame.rivalsInFrame != 0) {
                n = AddRivals(n);
            }
        }
        else {
            Vec3 ground = new Vec3(s.velocity.x, 0f, s.velocity.z);
            points[n++] = s.position + ground * frame.aheadTime;
        }

        if (!grounded && n < pointCapacity) {
            float g = VehicleModel.gravity * (arcade.config != null ? arcade.config.tuning.gravityMul : 1f);
            points[n++] = CameraFraming.AirPoint(s.position, s.velocity, CameraFraming.AirLookTime(s.velocity.y, g, in frame), g);
        }

        FrameSolve solve;
        CameraFraming.Solve(points, n, in basis, cam.aspect, in frame, out solve);

        if (!track && frame.projection == FrameTuning.orthographic && solve.size < frame.freeRideSize) {
            solve.size = frame.freeRideSize;
        }

        float pitchTarget = OcclusionTarget(s.position, basis);
        CameraFraming.Step(ref mem, in solve, pitchTarget, grounded ? 1 : 0, snap, dt, in frame);

        if (snap != 0) {
            snaps++;
        }

        Pose(basis: CameraFraming.Basis(frame.yaw, mem.pitch), dt: dt);
    }

    private int AddRivals(int n) {

        float mine = GameRace.Racer(GameRace.playerIndex).progress;

        for (int i = 0; i < GameRace.racerCount && n < pointCapacity; i++) {
            GameVehicleArcade rival = GameRace.Arcade(i);

            if (i == GameRace.playerIndex || rival == null) {
                continue;
            }

            if (Mathf.Abs(GameRace.Racer(i).progress - mine) <= frame.rivalRange) {
                points[n++] = rival.state.position;
            }
        }

        return n;
    }

    // The pitch that lifts the line of sight over a wall between the bike and the camera.
    private float OcclusionTarget(Vec3 bikeCore, in FrameBasis basis) {

        Vector3 bike = UnityFrame.ToUnity(bikeCore);
        Vector3 toCamera = -UnityFrame.ToUnity(basis.forward);
        float reach = frame.projection == FrameTuning.perspective ? frame.distMax : frame.backOff;
        int count = Physics.SphereCastNonAlloc(bike + Vector3.up, 0.5f, toCamera, hits, reach,
            arcade.config != null ? arcade.config.layerMask : Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        float best = float.MaxValue;
        float h = 0f;
        float d = 0f;

        for (int i = 0; i < count; i++) {
            RaycastHit hit = hits[i];

            if (hit.collider == null || hit.collider.transform.IsChildOf(arcade.transform) || hit.distance >= best) {
                continue;
            }

            float top = hit.collider.bounds.max.y - bike.y;
            Vector3 flat = hit.point - bike;
            flat.y = 0f;

            if (top > 0f) {
                best = hit.distance;
                h = top;
                d = flat.magnitude;
            }
        }

        return best < float.MaxValue ? CameraFraming.OcclusionPitch(h, d, in frame) : frame.pitch;
    }

    private void Pose(FrameBasis basis, float dt) {

        Vector3 forward = UnityFrame.ToUnity(basis.forward);
        Quaternion rotation = Quaternion.LookRotation(forward, UnityFrame.ToUnity(basis.up));
        Vector3 focus = UnityFrame.ToUnity(mem.focus);
        bool persp = frame.projection == FrameTuning.perspective;
        Vector3 position = focus - forward * (persp ? mem.zoom : frame.backOff);
        float size = persp ? savedSize : mem.zoom;

        if (blend < 1f) {
            blend = Mathf.Min(1f, blend + dt / Mathf.Max(frame.switchBlend, 1e-4f));
            float k = blend * blend * (3f - 2f * blend);
            position = Vector3.Lerp(blendFromPosition, position, k);
            rotation = Quaternion.Slerp(blendFromRotation, rotation, k);
            size = persp ? size : Mathf.Lerp(blendFromSize, size, k);
        }

        cam.transform.SetPositionAndRotation(position, rotation);
        cam.orthographic = !persp;
        cam.nearClipPlane = frame.near;
        cam.farClipPlane = frame.far;

        if (persp) {
            cam.fieldOfView = frame.fov / VehicleMath.degToRad;
        }
        else {
            cam.orthographicSize = size;

            for (int i = 0; i < children.Count; i++) {
                if (children[i] != null && children[i].gameObject.activeInHierarchy) {
                    children[i].orthographicSize = size;
                }
            }
        }
    }
}
#endif
