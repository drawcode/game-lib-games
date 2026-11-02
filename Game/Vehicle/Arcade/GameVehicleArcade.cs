#if USE_AGNOSTIC_VEHICLE
using Agnostic.Core;
using Agnostic.Vehicle;

using Engine.AgnosticHost;
using Engine.Events;

using UnityEngine;

// The arcade vehicle host (P2.3, arcade design section 4). Owns one vehicle's VehicleState, steps
// Agnostic.Vehicle.VehicleModel at a fixed 60 Hz (D10), maps the rider's dual sticks through StickMapper with the
// camera yaw, poses the vehicle and its rig, and plays the seat rider's riding clips (GameVehicleArcadeRide).
//
// - Rate: Unity physics runs at 50 Hz in these titles, so the 60 Hz steps come from a GameClock accumulator in
//   Update rather than from FixedUpdate. The root is posed at the state interpolated between the last two steps;
//   its Rigidbody is kinematic, so triggers and pickups still see it. No steps while the round is not running.
// - Rig: lean and pitch rotate the data's visual child; wheel bones spin only when data.host.wheels names them
//   (the chibi riding clips already animate their wheels).
// - Attach: GameObjectMountVehicle attaches one on mount when the vehicle's game-vehicle-data row has a tuning
//   (Attach, registered as GameObjectMountVehicle.attachArcade). A vehicle with a GameVehicleDrive never gets one.
// - Control mode (D8): the player's profile attribute (GameVehicleControlModes: "" = the class default, heading,
//   direct) over data.control.mode, resolved on every mount so a Settings change applies to the next ride.
// - Events: each frame's steps merge into frameEvents, and Messenger<GameVehicleArcade> broadcasts messageEvents
//   when any flag is set.
// - Statics: Bikes enters Play without a domain reload; the registration below runs again on every Play.
[DefaultExecutionOrder(-50)]   // before the mount and the player controller, so the rider tracks this frame's pose
public class GameVehicleArcade : MonoBehaviour, IGameVehicleMountDriver {

    public const float fixedDt = 1f / 60f;
    public const string messageEvents = "game-vehicle-arcade-events";

    public string vehicleCode;
    public VehicleState state;
    public VehicleInput input;          // the last step's mapped input
    public VehicleEvents frameEvents;   // this frame's steps, merged
    public GameObject rider;
    public Transform cameraTransform;   // unset = Camera.main
    public Transform visual;
    public int stepCount;

    public GameVehicleArcadeConfig config { get; private set; }
    public ControlTuning control { get { return controlTuning; } }
    public GameVehicleArcadeRide ride { get; private set; }

    public float speed {
        get {
            return state.speed;
        }
    }

    private readonly GameClock clock = new GameClock();
    private readonly UnityVehicleWorld world = new UnityVehicleWorld();
    private VehicleState previous;
    private Sticks sticks;
    private bool pedalsFromStick = true;
    private ControlTuning controlTuning;
    private float rootScale = 1f;
    private Vector3 visualBasePosition;
    private Quaternion visualBaseRotation = Quaternion.identity;
    private Transform[] wheels = new Transform[0];
    private Quaternion[] wheelRest = new Quaternion[0];
    private Quaternion[] wheelWritten = new Quaternion[0];

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register() {
        GameObjectMountVehicle.attachArcade = Attach;
    }

    // The arcade already on the vehicle, else a new one when its data row has a tuning, else null.
    public static IGameVehicleMountDriver Attach(GameObject vehicle) {

        if (vehicle == null) {
            return null;
        }

        GameVehicleArcade arcade = vehicle.GetComponent<GameVehicleArcade>();

        if (arcade != null) {
            return arcade;
        }

        GameVehicleArcadeConfig c = GameVehicleArcadeData.Get(CodeOf(vehicle));

        if (c == null) {
            return null;
        }

        arcade = vehicle.AddComponent<GameVehicleArcade>();
        arcade.Init(c);
        return arcade;
    }

    // Layout vehicles are instances of prefab/vehicles/<code>.
    public static string CodeOf(GameObject go) {
        string name = go.name;
        int clone = name.IndexOf("(Clone)", System.StringComparison.Ordinal);
        return (clone >= 0 ? name.Substring(0, clone) : name).Trim();
    }

    public void Init(GameVehicleArcadeConfig c) {

        config = c;
        vehicleCode = c.code;

        Rigidbody body = GetComponent<Rigidbody>();

        if (body != null) {
            body.isKinematic = true;
            body.useGravity = false;
            body.interpolation = RigidbodyInterpolation.None;
        }

        world.mask = c.layerMask;
        world.ignore0 = transform;
        world.SetSurfaces(c.surfaces);
        clock.fixedDt = fixedDt;
        rootScale = Mathf.Max(1e-4f, transform.lossyScale.y);

        visual = FindDeep(transform, c.visual);

        if (visual != null) {
            visualBasePosition = visual.localPosition;
            visualBaseRotation = visual.localRotation;
        }

        wheels = new Transform[c.wheels.Length];
        wheelRest = new Quaternion[c.wheels.Length];
        wheelWritten = new Quaternion[c.wheels.Length];

        for (int i = 0; i < c.wheels.Length; i++) {
            wheels[i] = FindDeep(transform, c.wheels[i]);

            if (wheels[i] != null) {
                wheelRest[i] = wheels[i].localRotation;
                wheelWritten[i] = wheelRest[i];
            }
        }

        ride = new GameVehicleArcadeRide(GetComponentInChildren<Animation>(true), c.ridePresets);
        ResolveControl();
        ResetAt(transform.position, transform.rotation);
    }

    public void ResetAt(Vector3 position, Quaternion rotation) {
        VehicleModel.Reset(ref state, UnityFrame.ToCore(position), YawOf(rotation * Vector3.forward));
        previous = state;
        clock.Reset();
    }

    public void ResolveControl() {

        controlTuning = config.control;
        string mode = GameProfiles.Current != null ? GameProfiles.Current.GetAttributeStringValue(GameVehicleControlModes.profileKey) : "";
        controlTuning.mode = VehicleTuningJson.ResolveMode(mode, config.control.mode);
    }

    // IGameVehicleMountDriver

    // Until a title feeds SetPedals (the throttle and brake actions, P2.4), direct mode takes them from the stick's
    // y, as GameVehicleDrive does: up throttles, down brakes and reverses.
    public void SetMoveStick(float x, float y) {
        sticks.move = new Vec2(x, y);

        if (pedalsFromStick) {
            sticks.throttle = Mathf.Max(0f, y);
            sticks.brake = Mathf.Max(0f, -y);
        }
    }

    public void SetAimStick(float x, float y) {
        sticks.aim = new Vec2(x, y);
    }

    public void SetButtons(int buttons) {
        sticks.buttons = buttons;
    }

    // Direct mode only: throttle and brake from their own actions. Once called, the stick no longer drives them.
    public void SetPedals(float throttle, float brake) {
        pedalsFromStick = false;
        sticks.throttle = throttle;
        sticks.brake = brake;
    }

    public void OnRiderMounted(GameObject go) {
        rider = go;
        world.ignore1 = go != null ? go.transform : null;
        sticks = default(Sticks);
        ResolveControl();
        LeaveLevelItem();
    }

    // A layout vehicle sits under its GameDraggableLevelItem, which hides everything under it once the player is
    // out of render distance of the item's cell, and a ridden vehicle leaves its cell. Move it up to the item's own
    // parent: still inside the level (torn down with it), but no longer culled by where it was parked.
    private void LeaveLevelItem() {

        GameDraggableLevelItem item = GetComponentInParent<GameDraggableLevelItem>();

        if (item != null) {
            transform.SetParent(item.transform.parent, true);
        }
    }

    public void OnRiderUnmounted(GameObject go) {
        rider = null;
        world.ignore1 = null;
        sticks = default(Sticks);
    }

    // Loop

    private void Update() {

        if (config == null) {
            return;
        }

        // Frozen outside a running round (menus, results, pause): nothing should coast behind a panel.
        clock.timeScale = Time.timeScale;
        clock.paused = !GameConfigs.isGameRunning || GameConfigs.isGamePaused;
        int steps = clock.Tick(Time.unscaledDeltaTime);
        frameEvents = default(VehicleEvents);
        float cameraYaw = CameraYaw();

        for (int i = 0; i < steps; i++) {
            previous = state;
            input = rider != null ? StickMapper.Map(in sticks, cameraYaw, in state, in controlTuning) : default(VehicleInput);
            VehicleEvents ev = default(VehicleEvents);
            VehicleModel.Step(ref state, in input, in config.tuning, world, fixedDt, ref ev);
            Merge(ref frameEvents, in ev);
            stepCount++;
        }

        Pose(clock.fixedAlpha);

        if (ride != null) {
            ride.Update(this, Time.time);
        }

        if (frameEvents.flags != 0) {
            Messenger<GameVehicleArcade>.Broadcast(messageEvents, this);
        }
    }

    // After the Animation has sampled, so the spin sits on top of whatever the clip did to the wheel bones.
    private void LateUpdate() {

        for (int i = 0; i < wheels.Length; i++) {
            Transform w = wheels[i];

            if (w == null) {
                continue;
            }

            // A bone no clip touched still holds last frame's write: start again from the rest pose.
            Quaternion from = w.localRotation == wheelWritten[i] ? wheelRest[i] : w.localRotation;
            w.localRotation = from * Quaternion.AngleAxis(WheelSpin(i) * Mathf.Rad2Deg, config.wheelAxis);
            wheelWritten[i] = w.localRotation;
        }
    }

    private void Pose(float alpha) {

        Vector3 position = Vector3.Lerp(UnityFrame.ToUnity(previous.position), UnityFrame.ToUnity(state.position), alpha);
        Quaternion rotation = Quaternion.Slerp(BodyRotation(previous), BodyRotation(state), alpha);
        transform.SetPositionAndRotation(position, rotation);

        if (visual == null) {
            return;
        }

        // Roll outside pitch, as the core composes them; nose up and lean right are both negative in Unity.
        float pitch = Mathf.LerpAngle(previous.pitch * Mathf.Rad2Deg, state.pitch * Mathf.Rad2Deg, alpha);
        float roll = Mathf.LerpAngle(previous.roll * Mathf.Rad2Deg, state.roll * Mathf.Rad2Deg, alpha);
        Quaternion r = Quaternion.AngleAxis(-roll, Vector3.forward) * Quaternion.AngleAxis(-pitch, Vector3.right);
        Vector3 pivot = Pivot(pitch);
        visual.localRotation = r * visualBaseRotation;
        visual.localPosition = pivot + r * (visualBasePosition - pivot);
    }

    // Body yaw on the ground plane under the smoothed up.
    private static Quaternion BodyRotation(VehicleState s) {

        Vector3 up = UnityFrame.ToUnity(s.up);
        Vector3 forward = Vector3.ProjectOnPlane(UnityFrame.ToUnity(VehicleMath.Forward(s.yaw)), up);

        if (forward.sqrMagnitude < 1e-8f) {
            forward = UnityFrame.ToUnity(VehicleMath.Forward(s.yaw));
        }

        return Quaternion.LookRotation(forward, up);
    }

    // Where the visual leans and pitches about, in the root's local space. On the ground: the contact line, so a
    // lean keeps the wheels where they are, moved to the rear wheel for a wheelie and the front for a stoppie. In
    // the air: the hull's centre.
    private Vector3 Pivot(float pitchDeg) {

        VehicleTuning t = config.tuning;
        float k = 1f / rootScale;

        if ((state.flags & (int)VehicleFlags.grounded) == 0) {
            return new Vector3(0f, (t.restLength + t.hullRadius) * k, 0f);
        }

        if (t.probeCount == 0 || Mathf.Abs(pitchDeg) <= 0.01f) {
            return Vector3.zero;
        }

        float rear = float.MinValue;
        float front = float.MaxValue;

        for (int i = 0; i < t.probeCount; i++) {
            float px;
            float pz;
            t.GetProbe(i, out px, out pz);
            rear = Mathf.Max(rear, pz);
            front = Mathf.Min(front, pz);
        }

        // Core z is -forward; Unity local z is forward.
        return new Vector3(0f, 0f, -(pitchDeg > 0f ? rear : front) * k);
    }

    private float WheelSpin(int i) {
        switch (i) {
            case 0: return state.wheelSpin0;
            case 1: return state.wheelSpin1;
            case 2: return state.wheelSpin2;
            default: return state.wheelSpin3;
        }
    }

    // Camera yaw in the vehicle convention, so stick up is always away from the camera. A camera looking straight
    // down has no forward on the ground, so its up stands in.
    private float CameraYaw() {

        Transform cam = cameraTransform;

        if (cam == null) {
            Camera main = Camera.main;
            cam = main != null ? main.transform : null;
        }

        if (cam == null) {
            return 0f;
        }

        Vector3 f = cam.forward;
        return f.x * f.x + f.z * f.z > 1e-4f ? YawOf(f) : YawOf(cam.up);
    }

    // Unity direction -> core yaw: 0 = Unity +Z (core -Z), positive turns left.
    public static float YawOf(Vector3 f) {
        return f.x * f.x + f.z * f.z > 1e-8f ? Mathf.Atan2(-f.x, f.z) : 0f;
    }

    private static void Merge(ref VehicleEvents into, in VehicleEvents ev) {

        if (ev.flags == 0) {
            return;
        }

        into.flags |= ev.flags;

        if ((ev.flags & (int)VehicleEventFlags.landed) != 0) {
            into.landQuality = ev.landQuality;
        }

        if ((ev.flags & (int)VehicleEventFlags.trick) != 0) {
            into.trickKind = ev.trickKind;
            into.trickAmount = ev.trickAmount;
        }

        into.impactSpeed = Mathf.Max(into.impactSpeed, ev.impactSpeed);
    }

    private static Transform FindDeep(Transform root, string name) {

        if (string.IsNullOrEmpty(name)) {
            return null;
        }

        if (root.name == name) {
            return root;
        }

        for (int i = 0; i < root.childCount; i++) {
            Transform found = FindDeep(root.GetChild(i), name);

            if (found != null) {
                return found;
            }
        }

        return null;
    }
}
#endif
