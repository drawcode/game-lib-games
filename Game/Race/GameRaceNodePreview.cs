#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Core;
using Agnostic.Racing;

using Engine.Game.App.BaseApp;

// The racing line in the level editor: while editing, the closed spline through the track-ai-node
// items (Agnostic.Racing.RaceTrack, the same curve the race measures on) and a gate across each node
// at its width. Created by the first GameRaceNodeMarker that sees the editor on; hidden otherwise.
// It follows a dragged node (rebuilt a few times a second while the nodes move, never in a race).
public class GameRaceNodePreview : MonoBehaviour {

    static GameRaceNodePreview instance;

    public static float sampleSpacing = 1f;
    public static float lineWidth = 0.35f;
    public static float gateWidth = 0.2f;
    public static float lift = 0.15f;
    public static float refreshInterval = 0.2f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() {
        instance = null;
    }

    public static void Ensure(Material material) {

        if (instance != null) {
            return;
        }

        GameObject go = new GameObject("GameRaceNodePreview");
        instance = go.AddComponent<GameRaceNodePreview>();
        instance.material = material;
    }

    Material material;
    LineRenderer line;
    readonly List<LineRenderer> gates = new List<LineRenderer>();
    readonly List<GameDraggableLevelItem> items = new List<GameDraggableLevelItem>();
    readonly List<GameLevelItemAsset> assets = new List<GameLevelItemAsset>();
    readonly List<Vector3> points = new List<Vector3>();
    float nextRefresh;
    float lastSignature = float.NaN;

    LineRenderer NewLine(string name, float width, bool loop) {

        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = loop;
        lr.widthMultiplier = width;
        lr.numCornerVertices = 2;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.sharedMaterial = material;
        lr.positionCount = 0;
        return lr;
    }

    void Awake() {
        line = NewLine("Line", lineWidth, true);
    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    void Update() {

        if (!GameDraggableEditor.isEditing) {
            Show(false, 0);
            lastSignature = float.NaN;
            return;
        }

        if (Time.unscaledTime < nextRefresh) {
            return;
        }

        nextRefresh = Time.unscaledTime + refreshInterval;
        Refresh();
    }

    void Show(bool show, int gateCount) {

        if (line.enabled != show) {
            line.enabled = show;
        }

        for (int i = 0; i < gates.Count; i++) {
            bool on = show && i < gateCount;
            if (gates[i].enabled != on) {
                gates[i].enabled = on;
            }
        }
    }

    void Refresh() {

        GameObject container = GameDraggableEditor.levelItemsContainerObject;

        items.Clear();
        assets.Clear();

        if (container != null) {
            foreach (GameDraggableLevelItem item in container.GetComponentsInChildren<GameDraggableLevelItem>()) {
                if (GameRaceNodes.IsNode(item.gameLevelItemAsset)) {
                    items.Add(item);
                    assets.Add(item.gameLevelItemAsset);
                }
            }
        }

        if (items.Count < 3) {
            Show(false, 0);
            lastSignature = float.NaN;
            return;
        }

        List<GameLevelItemAsset> sorted = GameRaceNodes.Sort(assets);
        RaceNode[] nodes = new RaceNode[sorted.Count];
        float signature = 0f;

        for (int i = 0; i < sorted.Count; i++) {

            Vector3 p = items[assets.IndexOf(sorted[i])].transform.position;
            float width = GameRaceNodes.WidthOf(sorted[i]);

            nodes[i] = new RaceNode(new Vec3(p.x, p.y, p.z), width, GameRaceNodes.CheckpointOf(sorted[i]) ? 1 : 0);
            signature += (i + 1) * (p.x * 31f + p.z * 17f + p.y + width * 7f);
        }

        if (signature == lastSignature) {
            return;
        }

        lastSignature = signature;

        RaceTrack track = new RaceTrack(nodes);
        int count = Mathf.Clamp(Mathf.CeilToInt(track.length / sampleSpacing), 3, 2048);

        points.Clear();

        for (int i = 0; i < count; i++) {
            Vec3 p = track.PointAt(track.length * i / count);
            points.Add(new Vector3(p.x, p.y + lift, p.z));
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

        while (gates.Count < nodes.Length) {
            gates.Add(NewLine("Gate" + gates.Count, gateWidth, false));
        }

        for (int i = 0; i < nodes.Length; i++) {

            Vec3 t = track.NodeTangent(i);
            Vector3 right = new Vector3(t.z, 0f, -t.x) * nodes[i].width;
            Vector3 at = new Vector3(nodes[i].position.x, nodes[i].position.y + lift, nodes[i].position.z);

            gates[i].positionCount = 2;
            gates[i].SetPosition(0, at - right);
            gates[i].SetPosition(1, at + right);
        }

        Show(true, nodes.Length);
    }
}
#endif
