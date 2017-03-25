using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this script to all 3D models having textures.
/// It sets the shader value, which is responsible for bending of path.
/// </summary>
/// 
public class CurvedObject : MonoBehaviour {

    bool straighten = false;
    Vector4 curveAmount = Vector4.zero;
    float curveDistance = 50;
    
    void Start() {

        UpdateShader();
    }

    void Update() {

        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.RightControl)) {

                if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
                    bool curveEnabled = GameController.CurveInfiniteEnabledGet();
                    curveEnabled = curveEnabled ? false : true;
                    GameController.CurveInfiniteEnabledSet(curveEnabled);
                }                       
            }
        } 

        UpdateShader();
    }

    void UpdateShader() {

        if (!GameController.CurveInfiniteEnabledGet()) {
            curveAmount = Vector4.zero;
            //curveDistance = 0;
        }
        else {
            curveAmount = GameController.CurveInfiniteAmountGet();
            curveDistance = GameController.CurveInfiniteDistanceGet();
        }

        Renderer render = GetComponent<Renderer>();

        if(render == null) {
            return;
        }

        if(render.sharedMaterial == null) {
            return;
        }

        render.sharedMaterial.SetVector("_QOffset", curveAmount);
        render.sharedMaterial.SetFloat("_Dist", curveDistance);
    }

    private void OnApplicationQuit() {

        GameController.CurveInfiniteEnabledSet(false);

        UpdateShader();
    }
}
