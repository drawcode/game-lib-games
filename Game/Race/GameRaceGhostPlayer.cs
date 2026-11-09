#if USE_AGNOSTIC_VEHICLE
using UnityEngine;

using Agnostic.Core;
using Agnostic.Racing;
using Agnostic.Vehicle;

using Engine.AgnosticHost;

// The time trial's ghost (Action Bikes R2 step 10): one see-through, visual-only copy of the player's bike that replays
// the best recorded lap of the track from each lap start, and the gap to it. On in a time trial (GameRace.ghostEnabled,
// or a race with no AI riders); there is no bike at all until a lap has been recorded (the first run), and a track whose
// hash changed has no ghost until a lap is recorded on the new shape.
//
// - The ghost is picked at Begin (none = no bike all run) and, while one is chasing, again at every lap start, so a
//   faster lap takes over the next lap.
// - Time: the player's lap clock (race time minus the lap start). The pose interpolates the samples through the arcade's
//   own pose code, so the ghost leans, pitches and jumps as the rider did.
// - Gap: the player's lap clock minus the time the ghost took to reach the player's progress along the track
//   (positive = behind the ghost).
public static class GameRaceGhostPlayer {

    public const float ghostAlpha = 0.4f;
    private static readonly Color ghostColor = new Color(0.45f, 0.85f, 1f, ghostAlpha);

    public static bool gapValid { get; private set; }
    public static float gap { get; private set; }
    public static GameRaceGhostLap lap { get; private set; }

    public static bool hasGhost {
        get {
            return lap != null;
        }
    }

    public static GameObject bike {
        get {
            return root;
        }
    }

    private static bool enabled;
    private static GameObject container;
    private static string vehicleCode;
    private static GameObject root;
    private static GameVehicleArcade arcade;
    private static GameRaceGhostBike driver;
    private static float[] progress;      // per sample of the chosen lap: the running maximum of its progress along the track
    private static GameRaceGhostLap progressFor;
    private static VehicleState from;
    private static VehicleState to;
    private static Material shared;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        shared = null;
        Reset();
    }

    private static void Reset() {
        enabled = false;
        container = null;
        vehicleCode = null;
        root = null;
        arcade = null;
        driver = null;
        progress = null;
        progressFor = null;
        lap = null;
        gapValid = false;
        gap = 0f;
    }

    // After the recorder's Begin, which loaded the file.
    public static void Begin(GameObject levelContainer, string vehicle) {

        Reset();
        enabled = GameRace.ghostEnabled || GameRace.aiRidersOverride == 0;

        if (!enabled || levelContainer == null) {
            return;
        }

        container = levelContainer;
        vehicleCode = vehicle;
        Select();

        if (lap != null) {
            Spawn();
            Show(true);
            PoseAt(0f);
        }
    }

    public static void End() {

        if (root != null) {
            Object.Destroy(root);
        }

        Reset();
    }

    public static void OnStep(int step) {

        if (!enabled) {
            return;
        }

        // A first run has no ghost and gets none from its own laps; once one is chasing, a faster lap takes over.
        if (GameRaceGhostRecorder.lapStarted && lap != null) {
            Select();
        }

        RaceRacer r = GameRace.Racer(GameRace.playerIndex);
        bool started = (r.flags & (int)RaceFlags.started) != 0;
        bool finished = (r.flags & (int)RaceFlags.finished) != 0;

        if (lap == null || root == null || finished) {
            gapValid = false;
            Show(false);
            return;
        }

        Show(true);
        float t = started ? Mathf.Max(0f, GameRace.session.raceTime - r.lapStartTime) : 0f;
        driver.stepClock = t;
        driver.stepTime = Time.time;
        driver.playing = true;
        PoseAt(t);

        if (!started) {
            gapValid = false;
            return;
        }

        float along = r.progress - r.lap * GameRace.track.length;
        gap = t - TimeAt(along);
        gapValid = true;
    }

    private static void Select() {

        GameRaceGhostFile file = GameRaceGhostRecorder.file;
        lap = file != null ? GameRaceGhostStore.Best(file, GameRaceGhostRecorder.trackHash) : null;

        if (lap != null && lap != progressFor) {
            ComputeProgress();
        }
    }

    private static void ComputeProgress() {

        RaceTrack track = GameRace.track;
        progress = new float[lap.count];
        progressFor = lap;
        int seg = -1;
        float prevS = 0f;
        float acc = 0f;

        for (int i = 0; i < lap.count; i++) {

            RaceProjection pr;
            track.Project(UnityFrame.ToCore(new Vector3(lap.Get(GameRaceGhostLap.colX, i, 0f),
                lap.Get(GameRaceGhostLap.colY, i, 0f), lap.Get(GameRaceGhostLap.colZ, i, 0f))), seg, out pr);
            seg = pr.seg;

            acc = i == 0 ? track.DeltaS(0f, pr.s) : acc + track.DeltaS(prevS, pr.s);
            progress[i] = i == 0 ? acc : Mathf.Max(progress[i - 1], acc);
            prevS = pr.s;
        }
    }

    // The ghost's lap time at a progress along the track (metres from the line), clamped to the lap.
    private static float TimeAt(float along) {

        float[] t = lap.columns[GameRaceGhostLap.colT];
        int n = progress.Length;

        if (along <= progress[0]) {
            return t[0];
        }

        if (along >= progress[n - 1]) {
            return t[n - 1];
        }

        int lo = 0;
        int hi = n - 1;

        while (hi - lo > 1) {
            int mid = (lo + hi) >> 1;

            if (progress[mid] <= along) {
                lo = mid;
            }
            else {
                hi = mid;
            }
        }

        float span = progress[hi] - progress[lo];
        return span > 1e-4f ? Mathf.Lerp(t[lo], t[hi], (along - progress[lo]) / span) : t[lo];
    }

    // Poses the ghost at a lap clock time, interpolating between the two samples around it.
    public static void PoseAt(float time) {

        if (lap == null || arcade == null) {
            return;
        }

        float[] t = lap.columns[GameRaceGhostLap.colT];
        int n = lap.count;
        int lo = 0;
        int hi = n - 1;

        if (time >= t[n - 1]) {
            lo = n - 2;
        }
        else if (time > t[0]) {
            while (hi - lo > 1) {
                int mid = (lo + hi) >> 1;

                if (t[mid] <= time) {
                    lo = mid;
                }
                else {
                    hi = mid;
                }
            }
        }

        float span = t[lo + 1] - t[lo];
        float f = span > 1e-5f ? Mathf.Clamp01((time - t[lo]) / span) : 0f;
        Fill(ref from, lo);
        Fill(ref to, lo + 1);
        arcade.PoseFrom(in from, in to, f);
    }

    private static void Fill(ref VehicleState s, int i) {

        float ux = lap.Get(GameRaceGhostLap.colUpX, i, 0f);
        float uz = lap.Get(GameRaceGhostLap.colUpZ, i, 0f);
        s.position = UnityFrame.ToCore(new Vector3(lap.Get(GameRaceGhostLap.colX, i, 0f),
            lap.Get(GameRaceGhostLap.colY, i, 0f), lap.Get(GameRaceGhostLap.colZ, i, 0f)));
        s.yaw = lap.Get(GameRaceGhostLap.colYaw, i, 0f);
        s.pitch = lap.Get(GameRaceGhostLap.colPitch, i, 0f);
        s.roll = lap.Get(GameRaceGhostLap.colRoll, i, 0f);
        s.up = UnityFrame.ToCore(new Vector3(ux, Mathf.Sqrt(Mathf.Max(0f, 1f - ux * ux - uz * uz)), uz));
        s.flags = (int)lap.Get(GameRaceGhostLap.colFlags, i, (int)VehicleFlags.grounded);
    }

    private static void Show(bool on) {
        if (root != null && root.activeSelf != on) {
            root.SetActive(on);
        }
    }

    // ------------------------------------------------------------------ the bike

    private static void Spawn() {

        GameObject go = GameDraggableEditor.LoadSprite(container, vehicleCode, Vector3.one);

        if (go == null) {
            Debug.LogWarning("GameRaceGhostPlayer: vehicle " + vehicleCode + " not found, no ghost");
            lap = null;
            return;
        }

        GameObjectMountVehicle mount = go.GetComponentInChildren<GameObjectMountVehicle>(true);
        GameVehicleArcade a = mount != null ? GameVehicleArcade.Attach(mount.vehicleObject) as GameVehicleArcade : null;

        if (a == null) {
            Debug.LogWarning("GameRaceGhostPlayer: vehicle " + vehicleCode + " has no arcade tuning, no ghost");
            Object.Destroy(go);
            lap = null;
            return;
        }

        // Visual only: no colliders, body, sound or driving scripts. The arcade stays, switched off and out of the
        // stepper, to pose the visual with.
        a.enabled = false;

        foreach (Collider c in go.GetComponentsInChildren<Collider>(true)) {
            Object.Destroy(c);
        }

        foreach (AudioSource s in go.GetComponentsInChildren<AudioSource>(true)) {
            Object.Destroy(s);
        }

        foreach (Light l in go.GetComponentsInChildren<Light>(true)) {
            l.enabled = false;
        }

        foreach (Animation anim in go.GetComponentsInChildren<Animation>(true)) {
            anim.enabled = false;
        }

        foreach (Rigidbody body in go.GetComponentsInChildren<Rigidbody>(true)) {
            body.isKinematic = true;
            Object.Destroy(body);
        }

        foreach (MonoBehaviour m in go.GetComponentsInChildren<MonoBehaviour>(true)) {
            if (m != a) {
                m.enabled = false;
                Object.Destroy(m);
            }
        }

        Material material = GhostMaterial();

        foreach (Renderer rend in go.GetComponentsInChildren<Renderer>(true)) {

            if (rend is MeshRenderer || rend is SkinnedMeshRenderer) {

                Material[] mats = new Material[rend.sharedMaterials.Length];

                for (int i = 0; i < mats.Length; i++) {
                    mats[i] = material;
                }

                rend.sharedMaterials = mats;
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;
            }
            else {
                rend.enabled = false;
            }
        }

        root = go;
        arcade = a;
        driver = go.AddComponent<GameRaceGhostBike>();
    }

    // One translucent unlit material for every ghost, made on first use.
    private static Material GhostMaterial() {

        if (shared != null) {
            return shared;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null) {
            shader = Shader.Find("Sprites/Default");
        }

        shared = new Material(shader);
        shared.name = "GameRaceGhost";
        shared.hideFlags = HideFlags.DontSave;
        shared.color = ghostColor;

        if (shared.HasProperty("_BaseColor")) {
            shared.SetColor("_BaseColor", ghostColor);
        }

        if (shared.HasProperty("_Surface")) {
            shared.SetFloat("_Surface", 1f);
            shared.SetFloat("_Blend", 0f);
            shared.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            shared.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            shared.SetFloat("_ZWrite", 0f);
            shared.SetOverrideTag("RenderType", "Transparent");
            shared.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            shared.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        return shared;
    }
}
#endif
