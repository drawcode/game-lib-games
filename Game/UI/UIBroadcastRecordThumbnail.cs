using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIBroadcastRecordThumbnail : GameObjectBehavior {

    public GameObject placeholderReplayTextureObject;

    float initialHeight = 160;
    float initialWidth = 120;

    public void Awake() {

    }

    public void Init() {

        if(placeholderReplayTextureObject != null) {

            initialWidth = placeholderReplayTextureObject.transform.localScale.x;
            initialHeight = placeholderReplayTextureObject.transform.localScale.y;

            placeholderReplayTextureObject.ResizePreservingAspectToScreen(initialWidth, initialHeight);
        }
    }

    public void Start() {
        Init();
    }
}