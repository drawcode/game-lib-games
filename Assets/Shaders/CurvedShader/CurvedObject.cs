using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this script to all 3D models having textures.
/// It sets the shader value, which is responsible for bending of path.
/// </summary>
/// 
public class CurvedObject : MonoBehaviour {

    void Start() {
        UpdateShader();
    }

    void Update() {
        UpdateShader();
    }

    void UpdateShader() {
        GetComponent<Renderer>().sharedMaterial.SetVector("_QOffset", GameController.GetCurveInfiniteAmount());
        GetComponent<Renderer>().sharedMaterial.SetFloat("_Distance", GameController.GetCurveInfiniteDistance());
    }

    private void OnApplicationQuit() {
        GetComponent<Renderer>().sharedMaterial.SetVector("_QOffset", Vector4.zero);
        //GetComponent<Renderer>().sharedMaterial.SetFloat("_Distance", GameController.GetCurveInfiniteDistance());
    }
}
