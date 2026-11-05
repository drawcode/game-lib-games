using UnityEngine;

// On the track-ai-node prefab: the marker is a level-editor aid, so its renderers show only while
// the level editor is on (the node still exists in a race; only its visual is off).
public class GameRaceNodeMarker : MonoBehaviour {

    Renderer[] renderers;
    bool shown = true;

    void Awake() {
        renderers = GetComponentsInChildren<Renderer>(true);
        Apply(GameDraggableEditor.isEditing);
    }

    void Update() {

        bool editing = GameDraggableEditor.isEditing;

        if (editing != shown) {
            Apply(editing);
        }

#if USE_AGNOSTIC_VEHICLE
        // The racing line through the nodes, drawn while editing.
        if (editing) {
            GameRaceNodePreview.Ensure(renderers.Length > 0 && renderers[0] != null ? renderers[0].sharedMaterial : null);
        }
#endif
    }

    void Apply(bool show) {

        shown = show;

        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i] != null) {
                renderers[i].enabled = show;
            }
        }
    }
}
