using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomRotation : GameObjectBehavior {

    public float rotationMaxX = 0f;
    public float rotationMaxY = 360f;
    public float rotationMaxZ = 0f;

    public void Start() {
        var rotX = Random.Range(-rotationMaxX, rotationMaxX);
        var rotY = Random.Range(-rotationMaxY, rotationMaxY);
        var rotZ = Random.Range(-rotationMaxZ, rotationMaxZ);

        transform.Rotate(rotX, rotY, rotZ);
    }
}