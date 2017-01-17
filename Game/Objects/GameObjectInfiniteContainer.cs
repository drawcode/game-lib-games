using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GameObjectInfinteData {

    public string code;
    public string codeGamePartItems = "game-part-items";
    public string codeGamePartItem = "game-part-item";

    public string codeGameBlock = "game-block-1";
    public string codeGameBlockLow = "game-block-low";
    public string codeGameBlockHigh = "game-block-high";

    //public Dictionary<string, object> positions;

    public List<Vector3> lines;

    public int currentLine = 0;

    public List<GameObject> lineObjects;

    public GameObjectInfinteData() {
        //positions = new Dictionary<string, object>();
        lines = new List<Vector3>();
        lineObjects = new List<GameObject>();
    }

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

            LoadLevelAssetDynamicByIndex(loadIndex);

            lastLoadIndex = loadIndex;
        }
    }

    /*
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
    */

    void LoadLevelAssetDynamicByIndex(int indexItem, bool clear = false) {

        // ADD PART ITEMS CONTAINER

        GameObject go = AppContentAssets.LoadAssetLevelAssets(data.codeGamePartItems);

        if (go == null) {
            Debug.Log("Asset not found levelassets/" + data.codeGamePartItems);
            return;
        }

        go.DestroyChildren();

        go.name = StringUtil.Dashed(data.code, indexItem.ToString());
        go.transform.parent = transform;

        GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

        if (part == null) {
            Debug.Log("GameObjectInfinitePart not found part:" + data.codeGamePartItems);
            return;
        }

        part.index = indexItem;
        Vector3 bounds = part.bounds;

        go.transform.position = go.transform.position.WithZ(
            (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

        // ADD PART ITEM CONTAINER AT LINES

        string template1 = "   ";
        string template2 = "X  ";
        string template3 = " X ";
        string template4 = "  X";

        for (int i = 0; i < data.lines.Count; i++) {

            GameObject goItem = AppContentAssets.LoadAssetLevelAssets(data.codeGamePartItem);

            if (goItem == null) {
                Debug.Log("Asset not found levelassets/" + data.codeGamePartItem);
                continue;
            }

            goItem.DestroyChildren();

            goItem.transform.parent = go.transform;
            goItem.transform.position = go.transform.position;
            goItem.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

            GameObjectInfinitePartItem partItem = goItem.Get<GameObjectInfinitePartItem>();

            if (partItem == null) {
                continue;
            }

            partItem.code = i.ToString();

            int rand = UnityEngine.Random.Range(1,10);

            if (rand < 2 && !clear) {
                //continue;
            }

            // ADD PART BLOCK AND ASSETS FROM TEMPLATE

            GameObject goAssetBlock = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlock);

            if (goAssetBlock == null) {
                Debug.Log("Asset not found levelassets/" + data.codeGameBlock);
                continue;
            }

            goAssetBlock.Hide();
            
            goAssetBlock.transform.parent = goItem.transform;
            goAssetBlock.transform.position = goItem.transform.position;
            //goAssetBlock.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

            goAssetBlock.Show();

            // ADD ITEMS


            int randCoin = UnityEngine.Random.Range(1, 10);

            if (randCoin < 2 && !clear) {

                string itemCoin = "item-coin";

                GameObject goAssetItemCoin = AppContentAssets.LoadAssetItems(itemCoin);

                if (goAssetItemCoin == null) {
                    Debug.Log("Asset not found items/" + itemCoin);
                    continue;
                }

                goAssetItemCoin.Hide();

                goAssetItemCoin.transform.parent = goItem.transform;
                goAssetItemCoin.transform.position = goItem.transform.position;
                goAssetItemCoin.transform.localPosition = goItem.transform.localPosition.WithY(4f);

                goAssetItemCoin.Show();
            }

            





            //
            continue;


            // ADD PART BLOCK JUMP AND ASSETS FROM TEMPLATE

            int randObstacleLow = UnityEngine.Random.Range(1, 30);

            if (randObstacleLow < 5 && !clear) {
                GameObject goAssetObstacleLow = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlockLow);

                if (goAssetObstacleLow == null) {
                    Debug.Log("Asset not found levelassets/" + data.codeGameBlockLow);
                    continue;
                }

                goAssetObstacleLow.Hide();

                goAssetObstacleLow.transform.parent = goItem.transform;
                goAssetObstacleLow.transform.position = goItem.transform.position;

                goAssetObstacleLow.transform.localPosition = goAssetObstacleLow.transform.localPosition.WithY(bounds.y);

                goAssetObstacleLow.Show();
            }           

        }

        /*


        string assetCodeBlock1 = "block-rock-1";

        for (int i = 0; i < data.lines.Count; i++) {

            GameObject go = AppContentAssets.LoadAssetLevelAssets(assetCodeBlock1);
            go.name = StringUtil.Dashed(data.code, "0", indexItem.ToString());
            go.transform.parent = transform;


            if (go != null) {
                GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

                part.index = indexItem;
                Vector3 bounds = part.bounds;

                go.transform.position = go.transform.position.WithZ(
                    (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);
            }
        }
        */


        // Layer in blocks

        // Layer in obstacles

        // Layer in collectables

        // 


        /*
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
        */
    }

    void LoadInitialParts() {
        
        // Add initial parts that can spawn other parts
        // Fill out parts to boundaries

        for (int i = 0; i < (int)rangeBoundsMax.z/distanceTickZ; i++) {

            bool shouldClear = false;

            if (i < 10) {
                shouldClear = true;
            }

            LoadLevelAssetDynamicByIndex(i, shouldClear);

            padIndex = i;
            lastLoadIndex = i;
        }
        
        // Place 10 parts back from view

        for (int i = 0; i < 10; i++) {
            LoadLevelAssetDynamicByIndex(-i, true);
        }
    }

    void FixedUpdate() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (!initialized) {
            return;
        }

        UpdateParts();
    }
}
