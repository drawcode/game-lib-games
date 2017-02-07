using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

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

    public Vector3 distance = Vector3.zero;

    public Vector3 rangeBoundsMin = Vector3.zero.WithX(-200f).WithY(-200f).WithZ(-100f);
    public Vector3 rangeBoundsMax = Vector3.zero.WithX(200f).WithY(200f).WithZ(1000f);

    public int padIndex = 0;

    public float distanceTickZ = 16f;
    public float distanceX = 16f;

    public int lastLoadIndex = 0;
    public int currentIndex = 0;

    public Dictionary<int, GameObjectInfinitePart> parts;

    public GameObjectInfinteData() {
        Reset();
    }

    public void Reset() {
        //positions = new Dictionary<string, object>();
        currentLine = 0;
        
        distance = Vector3.zero;

        padIndex = 0;
        distanceTickZ = 16f;
        distanceX = 16f;
        lastLoadIndex = 0;
        currentIndex = 0;
    }
}

public class GameObjectInfiniteContainer : GameObjectBehavior {

    public GameObjectInfinteData data = new GameObjectInfinteData();

    bool initialized = false;

    void OnEnable() {
        //Messenger<Vector3, float>.AddListener(GamePlayerMessages.PlayerCurrentDistance, OnPlayerCurrentDistance);
    }

    void Disable() {
        //Messenger<Vector3, float>.RemoveListener(GamePlayerMessages.PlayerCurrentDistance, OnPlayerCurrentDistance);
    }

    void Start() {
        Init();
    }

    void OnPlayerCurrentDistance(Vector3 pos, float speed) {

        //distance.z += -pos.z;
    }

    public void Reset() {

        initialized = false;

        if (data == null) {
            data = new GameObjectInfinteData();
        }
        else {
            data.Reset();
        }
                
        if (data.code.IsNullOrEmpty()) {
            data.code = UniqueUtil.CreateUUID4();
        }

        InitParts();

        if (data.lines == null) {

            data.lines = new List<Vector3>();

            data.lines.Add(Vector3.zero.WithX(-data.distanceX));
            data.lines.Add(Vector3.zero.WithX(0f));
            data.lines.Add(Vector3.zero.WithX(data.distanceX));
        }

        SwitchLine(data.currentLine);
    }

    void Init() {
        Reset();
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

    public void UpdatePositionX(float val) {
        data.distance.x += val;
    }

    public void UpdatePositionY(float val) {
        data.distance.y += val;
    }

    public void UpdatePositionZ(float val) {
        data.distance.z += val;
    }
    
    public void UpdatePositionPartsX(float val) {

        UpdatePositionY(val);

        foreach (GameObjectInfinitePart part in GetComponentsInChildren<GameObjectInfinitePart>(true)) {
            part.transform.position = part.transform.position.WithX(part.transform.position.x + val);
        }
    }

    public void UpdatePositionPartsY(float val) {

        UpdatePositionX(val);

        foreach (GameObjectInfinitePart part in GetComponentsInChildren<GameObjectInfinitePart>(true)) {
            part.transform.position = part.transform.position.WithY(part.transform.position.y + val);
        }
    }

    public void UpdatePositionPartsZ(float val) {

        UpdatePositionZ(val);

        foreach (GameObjectInfinitePart part in GetComponentsInChildren<GameObjectInfinitePart>(true)) {
            part.transform.position = part.transform.position.WithZ(part.transform.position.z + val);
        }
    }

    public void UpdateParts() {

        // index is 31 range 1000 
        // rangeBoundsMax.z / distanceTickZ;

        data.currentIndex = (int)(-data.distance.z / data.distanceTickZ);
        int loadIndex = data.currentIndex + data.padIndex;

        if (data.lastLoadIndex < loadIndex) {

            LoadLevelAssetDynamicByIndex(loadIndex);

            data.lastLoadIndex = loadIndex;
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

        // Use off screen location to spawn before move
        Vector3 spawnLocation = Vector3.zero.WithY(5000);

        bool used = false;

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

        //Vector3 infinityPosition = go.transform.position.WithZ(
        //    (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

        go.transform.position = go.transform.position.WithZ(
            (((indexItem + 1) * bounds.z) - bounds.z) + data.distance.z);

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

            int rand = UnityEngine.Random.Range(1, 10);

            bool fillBlock = true;

            if (rand > 2 && !clear) {
                fillBlock = true;
            }
            else if (clear) {
                fillBlock = true;
            }

            if (fillBlock) {

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
            }

            // ADD ITEMS

            // ADD ITEM COIN

            /*

            int randCoin = UnityEngine.Random.Range(1, 10);

            if (randCoin < 2 && !clear && !used) {

                string itemCoin = "item-coin";

                GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemCoin);

                if (goAssetItem == null) {
                    Debug.Log("Asset not found items/" + itemCoin);
                    continue;
                }

                goAssetItem.Hide();

                goAssetItem.transform.parent = goItem.transform;
                goAssetItem.transform.position = goItem.transform.position.WithX(0);
                goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

                goAssetItem.Show();

                used = true;
            }

            // ADD ITEM SPECIAL

            int randLetter = UnityEngine.Random.Range(1, 10);

            if (randLetter < 2 && !clear && !used) {

                string itemLetter = "item-special-watermelon";

                GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemLetter);

                if (goAssetItem == null) {
                    Debug.Log("Asset not found items/" + itemLetter);
                    continue;
                }

                goAssetItem.Hide();

                goAssetItem.transform.parent = goItem.transform;
                goAssetItem.transform.position = goItem.transform.position.WithX(0);
                goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

                goAssetItem.Show();

                used = true;
            }
            */
            

            // ADD PART BLOCK JUMP AND ASSETS FROM TEMPLATE

            int randObstacleLow = UnityEngine.Random.Range(1, 30);

            if (randObstacleLow < 5 && !clear && !used) {
                GameObject goAssetObstacleLow = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlockLow);

                if (goAssetObstacleLow == null) {
                    Debug.Log("Asset not found levelassets/" + data.codeGameBlockLow);
                    continue;
                }

                goAssetObstacleLow.Hide();

                goAssetObstacleLow.transform.parent = goItem.transform;
                goAssetObstacleLow.transform.position = goItem.transform.position;

                //goAssetObstacleLow.transform.localPosition = goAssetObstacleLow.transform.localPosition.WithY(bounds.y);

                goAssetObstacleLow.Show();

                used = true;
            }

        }
        
        //go.transform.position = infinityPosition.WithY(0);


        //go.ResetLocalPosition();

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


    /*

        
    void LoadLevelAssetDynamicByIndex(int indexItem, bool clear = false) {

        // Use off screen location to spawn before move
        Vector3 spawnLocation = Vector3.zero.WithY(5000);

        bool used = false;

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

        //Vector3 infinityPosition = go.transform.position.WithZ(
        //    (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

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

            int rand = UnityEngine.Random.Range(1, 10);

            bool fillBlock = false;

            if (rand > 2 && !clear) {
                fillBlock = true;
            }
            else if (clear) {
                fillBlock = true;
            }

            if (fillBlock) {

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
            }

            // ADD ITEMS

            // ADD ITEM COIN

            int randCoin = UnityEngine.Random.Range(1, 10);

            if (randCoin < 2 && !clear && !used) {

                string itemCoin = "item-coin";

                GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemCoin);

                if (goAssetItem == null) {
                    Debug.Log("Asset not found items/" + itemCoin);
                    continue;
                }

                goAssetItem.Hide();

                goAssetItem.transform.parent = goItem.transform;
                goAssetItem.transform.position = goItem.transform.position.WithX(0);
                goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

                goAssetItem.Show();

                used = true;
            }

            // ADD ITEM SPECIAL

            int randLetter = UnityEngine.Random.Range(1, 10);

            if (randLetter < 2 && !clear && !used) {

                string itemLetter = "item-special-watermelon";

                GameObject goAssetItem = AppContentAssets.LoadAssetItems(itemLetter);

                if (goAssetItem == null) {
                    Debug.Log("Asset not found items/" + itemLetter);
                    continue;
                }

                goAssetItem.Hide();

                goAssetItem.transform.parent = goItem.transform;
                goAssetItem.transform.position = goItem.transform.position.WithX(0);
                goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(2f).WithZ(0);

                goAssetItem.Show();

                used = true;
            }
            

            // ADD PART BLOCK JUMP AND ASSETS FROM TEMPLATE

            int randObstacleLow = UnityEngine.Random.Range(1, 30);

            if (randObstacleLow < 5 && !clear && !used) {
                GameObject goAssetObstacleLow = AppContentAssets.LoadAssetLevelAssets(data.codeGameBlockLow);

                if (goAssetObstacleLow == null) {
                    Debug.Log("Asset not found levelassets/" + data.codeGameBlockLow);
                    continue;
                }

                goAssetObstacleLow.Hide();

                goAssetObstacleLow.transform.parent = goItem.transform;
                goAssetObstacleLow.transform.position = goItem.transform.position;

                //goAssetObstacleLow.transform.localPosition = goAssetObstacleLow.transform.localPosition.WithY(bounds.y);

                goAssetObstacleLow.Show();

                used = true;
            }

        }
        
        //go.transform.position = infinityPosition.WithY(0);


        //go.ResetLocalPosition();
        
}



    */

    void LoadInitialParts() {

        // Add initial parts that can spawn other parts
        // Fill out parts to boundaries

        for (int i = 0; i < (int)data.rangeBoundsMax.z / data.distanceTickZ; i++) {

            bool shouldClear = false;

            if (i < 10) {
                shouldClear = true;
            }

            LoadLevelAssetDynamicByIndex(i, shouldClear);

            data.padIndex = i;
            data.lastLoadIndex = i;
        }

        // Place 10 parts back from view

        for (int i = 0; i < 10; i++) {
            LoadLevelAssetDynamicByIndex(-i, true);
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