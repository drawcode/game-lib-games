using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameObjectInfiniteContainer : GameObjectBehavior {

    public string code;
    public string codeGamePart = "game-part-col-3";
        
    Dictionary<int, GameObjectInfinitePart> parts;

    int index = 0;
    float distance = 0;

    public Vector3 rangeBoundsMin = Vector3.zero.WithX(-200f).WithY(-200f).WithZ(-1000f);
    public Vector3 rangeBoundsMax = Vector3.zero.WithX(200f).WithY(200f).WithZ(2000f);

    void Start() {
        Init();
    }

    void Init() {

        if (code.IsNullOrEmpty()) {
            code = UniqueUtil.CreateUUID4();
        }

        InitParts();

    }

    void InitParts() {

        gameObject.DestroyChildren();

        UpdateParts();
    }

    public void UpdatePositionPartsZ(float z) {
        foreach (GameObjectInfinitePart part in gameObject.GetList<GameObjectInfinitePart>()) {
            part.transform.position = part.transform.position.WithZ(part.transform.position.z + z);
        }
    }
    
    void UpdateParts() {
        
        // Add initial parts that can spawn other parts
        // Fill out parts to boundaries

        for (int i = 0; i < (int)rangeBoundsMax.z/16f; i++) {

            GameObject go = AppContentAssets.LoadAssetLevelAssets(codeGamePart);

            if (go != null) {

                GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

                if (part != null) {

                    part.index = i;
                    Vector3 bounds = part.bounds;

                    go.transform.parent = transform;
                    go.transform.position = go.transform.position.WithZ(((i + 1) * bounds.z) - bounds.z);
                    
                }
            }
        }


        // Place 10 parts back from view

        for (int i = 0; i < 10; i++) {

            GameObject go = AppContentAssets.LoadAssetLevelAssets(codeGamePart);

            if (go != null) {

                GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

                if (part != null) {

                    part.index = -i + 1;
                    Vector3 bounds = part.bounds;

                    go.transform.parent = transform;
                    go.transform.position = go.transform.position.WithZ(((-i + 1) * bounds.z));


                }
            }
        }
    }
}
