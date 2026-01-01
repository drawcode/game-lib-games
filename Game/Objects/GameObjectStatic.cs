using UnityEngine;
using System.Collections;

public class GameObjectStatic : GameObjectBehavior {

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}