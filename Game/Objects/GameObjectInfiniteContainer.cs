using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GameObjectInfinteData {

    public string code;
    public string codeGamePart = "game-part-col-3";

    public Dictionary<string, object> positions = new Dictionary<string, object>();
    public List<Vector3> lines;
    public int currentLine = 0;
}

public class GameObjectInfiniteContainer : GameObjectBehavior {

    public GameObjectInfinteData data = new GameObjectInfinteData();
                
    Dictionary<int, GameObjectInfinitePart> parts;


    int padIndex = 0;
    public Vector3 distance = Vector3.zero;

    public Vector3 rangeBoundsMin = Vector3.zero.WithX(-200f).WithY(-200f).WithZ(-100f);
    public Vector3 rangeBoundsMax = Vector3.zero.WithX(200f).WithY(200f).WithZ(1000f);

    public float distanceTickZ = 16f;
    public float distanceX = 16f;
    
    int lastLoadIndex = 0;
    int currentIndex = 0;

    bool initialized = false;

    void Start() {
        Init();
    }

    void Init() {

        if (data.code.IsNullOrEmpty()) {
            data.code = UniqueUtil.CreateUUID4();
        }

        InitParts();

        if (data.lines == null) {

            data.lines = new List<Vector3>();

            data.lines.Add(Vector3.zero.WithX(-distanceX));
            data.lines.Add(Vector3.zero.WithX(0f));
            data.lines.Add(Vector3.zero.WithX(distanceX));
        }

        SwitchLine(data.currentLine);

    }

    public Vector3 GetCurrentLine() {
        if (data.lines.Count > data.currentLine) {
            return data.lines[data.currentLine];
        }
        return Vector3.zero;
    }

    public void SetCurrentLine(int index) {
        if (index > data.lines.Count - 1) {
            return;
        }

        data.currentLine = index;
    }

    public Vector3 SwitchLine(int index) {

        if (index > data.lines.Count - 1) {
            return GetCurrentLine();
        }

        if (index < 0) {
            return GetCurrentLine();
        }

        SetCurrentLine(index);

        return GetCurrentLine();
    }

    public Vector3 SwitchLineLeft() {
        return SwitchLine(data.currentLine - 1);
    }

    public Vector3 SwitchLineRight() {
        return SwitchLine(data.currentLine + 1);
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

        GameObject go = AppContentAssets.LoadAssetLevelAssets(data.codeGamePart);
        go.name = StringUtil.Dashed(data.code, indexItem.ToString());
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
