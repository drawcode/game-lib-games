using UnityEngine;
using System.Collections;

using Engine.Events;
using Engine.Utility;

public class GameObjectItemDisplayItem : GameObjectBehavior {

    bool _collected = false;

    public bool collected {
        get {
            return _collected;
        }
        set {
            _collected = value;
            UpdateState();
        }
    }

    void Start() {
        UpdateState();
    }
    
    void UpdateState() {

        TweenUtil.FadeToObject(gameObject, collected ? 1f : .4f);
    }
}