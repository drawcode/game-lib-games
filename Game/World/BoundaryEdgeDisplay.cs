using UnityEngine;

// Shows where a level's playable rectangle ends: one generated mesh, built once per level load -
// a ground fade skirt brightening toward the edge, and a low wall strip that glows at its base and
// fades up to nothing. Vertex colour only: one draw call, no texture, no lights or shadows, no
// per-frame work (there is no Update). Display only - the walls that stop the player are elsewhere.
// Plan: contexts games/action-bots/plans/plan-level-edge-display.md.
public class BoundaryEdgeDisplay : GameObjectBehavior {

    public const string shaderName = "Drawlabs/BoundaryEdgeUnlit";

    // ground skirt: how far inside the edge the fade starts (alpha 0 there)
    public float skirtWidth = 8f;
    // wall strip: glow band height, then fade to 0 at wallHeight
    public float glowHeight = 0.4f;
    public float wallHeight = 3f;
    // lift above the ground hit so the skirt never z-fights the terrain
    public float groundLift = 0.05f;

    public float alphaEdge = 0.55f;
    public float alphaGlow = 0.8f;

    static Material sharedMaterial;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // 4 rings x 4 corners
    readonly Vector3[] vertices = new Vector3[16];
    readonly Color[] colors = new Color[16];
    // 4 sides x (skirt quad + 2 wall quads) x 2 triangles x 3
    readonly int[] triangles = new int[4 * 3 * 6];

    public static BoundaryEdgeDisplay Ensure(Transform parent) {
        Transform existing = parent.Find("BoundaryEdgeDisplay");
        if (existing != null) {
            BoundaryEdgeDisplay found = existing.GetComponent<BoundaryEdgeDisplay>();
            if (found != null) {
                return found;
            }
        }
        GameObject go = new GameObject("BoundaryEdgeDisplay");
        go.transform.SetParent(parent, false);
        return go.AddComponent<BoundaryEdgeDisplay>();
    }

    static Material GetMaterial() {
        if (sharedMaterial == null) {
            Shader shader = Shader.Find(shaderName);
            if (shader == null) {
                Debug.LogWarning("BoundaryEdgeDisplay: shader not found: " + shaderName);
                return null;
            }
            sharedMaterial = new Material(shader);
            sharedMaterial.name = "BoundaryEdgeDisplay";
        }
        return sharedMaterial;
    }

    void EnsureComponents() {
        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
        }
        if (meshRenderer == null) {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;
        }
        if (mesh == null) {
            mesh = new Mesh();
            mesh.name = "BoundaryEdgeDisplay";
            mesh.MarkDynamic();
            meshFilter.sharedMesh = mesh;
        }
    }

    // min/max are the rectangle's x/z extents in world space; groundY is the ground height.
    // The object sits at the world origin with no rotation or scale, so vertices are world space.
    public void Build(float minX, float minZ, float maxX, float maxZ, float groundY, Color color) {

        Material material = GetMaterial();
        if (material == null || maxX <= minX || maxZ <= minZ) {
            Clear();
            return;
        }

        EnsureComponents();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        float y = groundY + groundLift;
        float inset = Mathf.Min(skirtWidth, (maxX - minX) * 0.5f, (maxZ - minZ) * 0.5f);

        // corners, counter-clockwise seen from above: (minX,minZ) (maxX,minZ) (maxX,maxZ) (minX,maxZ)
        SetRing(0, minX + inset, minZ + inset, maxX - inset, maxZ - inset, y, color, 0f);   // inset
        SetRing(1, minX, minZ, maxX, maxZ, y, color, alphaEdge);                            // edge
        SetRing(2, minX, minZ, maxX, maxZ, y + glowHeight, color, alphaGlow);               // glow
        SetRing(3, minX, minZ, maxX, maxZ, y + wallHeight, color, 0f);                      // top

        int t = 0;
        for (int side = 0; side < 4; side++) {
            int next = (side + 1) % 4;
            t = Quad(t, 0, 1, side, next);  // skirt: inset -> edge
            t = Quad(t, 1, 2, side, next);  // wall: edge -> glow
            t = Quad(t, 2, 3, side, next);  // wall: glow -> top
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        meshRenderer.sharedMaterial = material;
        meshRenderer.enabled = true;
    }

    public void Clear() {
        if (meshRenderer != null) {
            meshRenderer.enabled = false;
        }
    }

    void SetRing(int ring, float minX, float minZ, float maxX, float maxZ, float y, Color color, float alpha) {
        Color c = new Color(color.r, color.g, color.b, alpha);
        int i = ring * 4;
        vertices[i + 0] = new Vector3(minX, y, minZ);
        vertices[i + 1] = new Vector3(maxX, y, minZ);
        vertices[i + 2] = new Vector3(maxX, y, maxZ);
        vertices[i + 3] = new Vector3(minX, y, maxZ);
        colors[i + 0] = c;
        colors[i + 1] = c;
        colors[i + 2] = c;
        colors[i + 3] = c;
    }

    // one quad between ring a and ring b along corners c0 -> c1 (the shader culls nothing)
    int Quad(int t, int ringA, int ringB, int c0, int c1) {
        int a0 = ringA * 4 + c0;
        int a1 = ringA * 4 + c1;
        int b0 = ringB * 4 + c0;
        int b1 = ringB * 4 + c1;
        triangles[t++] = a0;
        triangles[t++] = b0;
        triangles[t++] = a1;
        triangles[t++] = a1;
        triangles[t++] = b0;
        triangles[t++] = b1;
        return t;
    }

    void OnDestroy() {
        if (mesh != null) {
            Destroy(mesh);
            mesh = null;
        }
    }
}
