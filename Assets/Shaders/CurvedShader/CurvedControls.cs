using UnityEngine;
using System.Collections;

public class CurvedControls : MonoBehaviour {
    Vector2 Offset = Vector2.zero;
    float camPos = -20;
    public Material[] Mats;
    public Transform cam;
    void Start() {

    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, Screen.height - 10));

        GUILayout.BeginHorizontal();
        GUILayout.Label("xOffset", GUILayout.Width(100));
        Offset.x = GUILayout.HorizontalSlider(Offset.x, -20, 20);
        if (GUILayout.Button("0", GUILayout.Width(30)))
            Offset.x = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("yOffset", GUILayout.Width(100));
        Offset.y = GUILayout.HorizontalSlider(Offset.y, -20, 20);
        if (GUILayout.Button("0", GUILayout.Width(30)))
            Offset.y = 0;

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cam pos", GUILayout.Width(100));
        camPos = GUILayout.HorizontalSlider(camPos, -40, 30);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        foreach (Material M in Mats) {
            M.SetVector("_QOffset", Offset);
        }
        Vector3 P = cam.position;
        P.z = camPos;
        cam.position = P;

    }
}
