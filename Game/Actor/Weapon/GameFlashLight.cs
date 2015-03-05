using UnityEngine;
using System.Collections;

public class GameFlashLight : MonoBehaviour {

    public float LightMult = 2;

    void Update() {
        if (!gameObject.Has<Light>())
            return;
        
        gameObject.Get<Light>().intensity -= LightMult * Time.deltaTime;
    }
}
