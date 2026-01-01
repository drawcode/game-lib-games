using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameObjectInfinitePartItem : GameObjectBehavior {

    public GameObjectInfinitePart part;
    public string code;

    void Start() {

        FindItems();
    }

    void FindItems() {

        //if(part == null) {
        part = gameObject.FindTypeAboveRecursive<GameObjectInfinitePart>();
        //}
    }

    public void ClearItems(bool removeCached = false) {

        GameController.ResetLevelAssetCacheItem(gameObject, removeCached);

        gameObject.DestroyChildren();
    }

    public void DestroyItems(bool removeCached = false) {

        ClearItems(removeCached);

        gameObject.DestroyGameObject();
    }

    /*
    void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        / *
        bool destroy = false;
        
        if (controller != null) {
            destroy = MathUtil.IsVector3OutOfRange(
                gameObject.transform.position, 
                controller.rangeBoundsMin, controller.rangeBoundsMax, bounds);
        }
        
        if (destroy) {
            gameObject.DestroyGameObject();
        }
        * /

    }
*/
}