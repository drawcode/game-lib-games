using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameObjectInfinitePart : GameObjectBehavior {

    public GameObjectInfiniteController controller;
    public GameObjectInfiniteContainer container;

    public int index = 0;

    public Vector3 bounds = Vector3.zero.WithX(16f).WithY(16f).WithZ(16f);

    void OnEnable() {
        //Messenger<Vector3, float>.AddListener(GamePlayerMessages.PlayerCurrentDistance, OnPlayerCurrentDistance);
    }

    void Disable() {
        //Messenger<Vector3, float>.RemoveListener(GamePlayerMessages.PlayerCurrentDistance, OnPlayerCurrentDistance);
    }

    void Start() {

        FindController();
    }

    void OnPlayerCurrentDistance(Vector3 pos, float speed) {

        //transform.position =
        //    Vector3.Lerp(
        //        transform.position,
        //       transform.position.WithZ(transform.position.z + -pos.z), speed * Time.deltaTime);

        //transform.position = transform.position.WithZ(transform.position.z + -pos.z);
    }

    void FindController() {

        if (controller == null) {
            controller = gameObject.FindTypeAboveRecursive<GameObjectInfiniteController>();
        }
        if (container == null) {
            container = gameObject.FindTypeAboveRecursive<GameObjectInfiniteContainer>();
        }
    }

    public void ClearItems() {
        gameObject.DestroyChildren();
    }

    void Update() {

        bool destroy = false;

        if (container != null) {
            destroy = MathUtil.IsVector3OutOfRange(
                gameObject.transform.position,
                container.rangeBoundsMin, container.rangeBoundsMax, bounds);
        }

        if (destroy) {

            foreach (GameObjectInfinitePart part in gameObject.GetList<GameObjectInfinitePart>()) {
                part.ClearItems();
            }

            ClearItems();

            gameObject.DestroyGameObject();
        }
    }
}