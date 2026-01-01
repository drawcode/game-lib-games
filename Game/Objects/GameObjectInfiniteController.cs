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

        containersInfinite = new Dictionary<string, GameObjectInfiniteContainer>();

        UpdateContainers();
    }

    public void UpdateContainers() {

        foreach (GameObjectInfiniteContainer container in gameObject.GetList<GameObjectInfiniteContainer>()) {
            containersInfinite.Set(container.data.code, container);
        }

        int containersCount = 0;

        if (containersInfinite != null) {
            containersCount = containersInfinite.Count;
        }

        Debug.Log("UpdateContainers: count:" + containersCount);
    }
}