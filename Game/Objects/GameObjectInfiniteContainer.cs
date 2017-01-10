using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameObjectInfiniteContainer : GameObjectBehavior {

    public string code;
    public string codeGamePart = "game-part-col-3";
            
    Dictionary<int, GameObjectInfinitePart> parts;

    int padIndex = 0;
    public Vector3 distance = Vector3.zero;

    public Vector3 rangeBoundsMin = Vector3.zero.WithX(-200f).WithY(-200f).WithZ(-100f);
    public Vector3 rangeBoundsMax = Vector3.zero.WithX(200f).WithY(200f).WithZ(1000f);

    public float distanceTickZ = 16f;

    bool initialized = false;

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

        LoadInitialParts();

        initialized = true;
    }

    public void UpdatePositionPartsX(float x) {

        distance.x += x;

        foreach (GameObjectInfinitePart part in gameObject.GetList<GameObjectInfinitePart>()) {
            part.transform.position = part.transform.position.WithX(part.transform.position.x + x);
        }
    }

    public void UpdatePositionPartsY(float y) {

        distance.y += y;

        foreach (GameObjectInfinitePart part in gameObject.GetList<GameObjectInfinitePart>()) {
            part.transform.position = part.transform.position.WithY(part.transform.position.y + y);
        }
    }

    public void UpdatePositionPartsZ(float z) {

        distance.z += z;

        foreach (GameObjectInfinitePart part in gameObject.GetList<GameObjectInfinitePart>()) {
            part.transform.position = part.transform.position.WithZ(part.transform.position.z + z);
        }
    }

    int lastLoadIndex = 0;
    int currentIndex = 0;

    public void UpdateParts() {

        // index is 31 range 1000 
        // rangeBoundsMax.z / distanceTickZ;

        currentIndex = (int)(-distance.z/distanceTickZ);
        int loadIndex = currentIndex + padIndex;

        if (lastLoadIndex < loadIndex) {

            LoadLevelAssetByIndex(loadIndex);

            lastLoadIndex = loadIndex;
        }
    }

    void LoadLevelAssetByIndex(int indexItem) {

        GameObject go = AppContentAssets.LoadAssetLevelAssets(codeGamePart);
        go.name = StringUtil.Dashed(code, indexItem.ToString());
        go.transform.parent = transform;

        if (go != null) {

            GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

            if (part != null) {

                part.index = indexItem;
                Vector3 bounds = part.bounds;

                go.transform.position = go.transform.position.WithZ(
                    (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

                
            }
        }
    }

    
    void LoadInitialParts() {
        
        // Add initial parts that can spawn other parts
        // Fill out parts to boundaries

        for (int i = 0; i < (int)rangeBoundsMax.z/distanceTickZ; i++) {
            LoadLevelAssetByIndex(i);

            padIndex = i;
            lastLoadIndex = i;
        }
        
        // Place 10 parts back from view

        for (int i = 0; i < 10; i++) {
            LoadLevelAssetByIndex(-i);
        }
    }

    void Update() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (!initialized) {
            return;
        }

        UpdateParts();
    }
}
