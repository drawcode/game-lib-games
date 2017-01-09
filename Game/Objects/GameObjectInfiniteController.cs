using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameObjectInfiniteController : GameObjectBehavior {
    
    Dictionary<string, GameObjectInfiniteContainer> containersInfinite;

    void Start() {

        Init();
    }

    public void Init() {

        UpdateContainers();
    }
    
    void UpdateContainers() {

        foreach (GameObjectInfiniteContainer container in gameObject.GetList<GameObjectInfiniteContainer>()) {
            containersInfinite.Set(container.code, container);
        }

    }



}
