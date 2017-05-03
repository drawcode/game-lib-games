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

        //if(controller == null) {
        controller = gameObject.FindTypeAboveRecursive<GameObjectInfiniteController>();
        //}
        //if(container == null) {
        container = gameObject.FindTypeAboveRecursive<GameObjectInfiniteContainer>();
        //}
    }

    public void ClearItems(bool removeCached = false) {

        bool cached = GameController.ResetLevelAssetCacheItem(gameObject, removeCached);

        if(!cached || removeCached) {

            foreach(GameObjectInfinitePartItem item in
                gameObject.GetList<GameObjectInfinitePartItem>()) {

                item.ClearItems(removeCached);
            }
        }

        gameObject.DestroyChildren();
    }

    public void DestroyItems(bool removeCached = false) {

        ClearItems(removeCached);

        gameObject.DestroyGameObject();
    }

    void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        bool destroy = false;

        FindController();

        if(container != null) {
            destroy = MathUtil.IsVector3OutOfRange(
                gameObject.transform.position,
                container.data.rangeBoundsMin, container.data.rangeBoundsMax, bounds);
        }

        if(destroy) {
            DestroyItems();
        }
    }
}