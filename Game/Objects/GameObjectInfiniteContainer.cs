using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;
using Engine.Game.App;

[Serializable]
public class GameObjectInfinteData {

    public string code;
    public string codeGamePartItems = "game-part-items";
    public string codeGamePartItem = "game-part-item";

    public string codeGameFloor = "game-world-floor";
    public string codeGameSide = "game-world-side";
    public string codeGameSky = "game-world-sky";   
    public string codeGameWater = "game-world-water";

    public string codeGameBlockEmpty = "game-block-empty";
    public string codeGameBlockFloor = "game-block-floor";
    public string codeGameBlockLow = "game-block-low";
    public string codeGameBlockHigh = "game-block-high";
    public string codeGameBlockFull = "game-block-full";

    //public Dictionary<string, object> positions;

    public List<Vector3> lines;

    public int currentLine = 0;

    public List<GameObject> lineObjects;

    public Vector3 distance = Vector3.zero;

    public Vector3 rangeBoundsMin = Vector3.zero.WithX(-200f).WithY(-200f).WithZ(-100f);
    public Vector3 rangeBoundsMax = Vector3.zero.WithX(200f).WithY(200f).WithZ(400f);

    public int padIndex = 0;

    public float distanceTickZ = 16f;
    public float distanceX = 16f;

    public int lastLoadIndex = 0;
    public int currentIndex = 0;

    public int currentLevelGridIndex = -1;
    public GameLevelLayout data;
    public List<GameDataObject> dataObjects;

    public int partStartCount = 10;
    public int partBackCount = 10;

    //public Dictionary<int, GameObjectInfinitePart> parts;

    public GameObject parentContainer;
   
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
        lastLoadIndex = -1;
        currentIndex = 0;

        currentLevelGridIndex = -1;

        partStartCount = 10;
        partBackCount = 10;
    }
}

public class GameObjectInfiniteContainer : GameObjectBehavior {

    public GameObjectInfinteData data = new GameObjectInfinteData();

    bool initialized = false;

    void OnEnable() {
        //Messenger<Vector3, float>.AddListener(GamePlayerMessages.PlayerCurrentDistance, OnPlayerCurrentDistance);
        //Messenger<string>.AddListener(GameMessages.gameLevelStart, OnGameLevelStart);

        Messenger.AddListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }

    void Disable() {
        //Messenger<Vector3, float>.RemoveListener(GamePlayerMessages.PlayerCurrentDistance, OnPlayerCurrentDistance);
        //Messenger<string>.RemoveListener(GameMessages.gameLevelStart, OnGameLevelStart);

        Messenger.RemoveListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }

    void Start() {
        LoadData();
    }

    public void OnGameLevelItemsLoadedHandler() {
        //LoadData();
    }

    public void LoadData() {

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

            // TODO config lanes

            data.lines.Add(Vector3.zero.WithX(-data.distanceX));
            data.lines.Add(Vector3.zero.WithX(0f));
            data.lines.Add(Vector3.zero.WithX(data.distanceX));
        }

        SwitchLine(data.currentLine);
    }
    
    #region lines

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

    #endregion

    void InitParts() {

        gameObject.DestroyChildren();

        data.parentContainer = gameObject;

        GameController.LoadInitialParts(data);

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
    
    void LateUpdate() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (!initialized) {
            LoadData();
        }

        if(!initialized) {
            return;
        }

        GameController.UpdateParts(data);
    }


    public void ClearItems(bool removeCached = false) {

        //bool cached = GameController.ResetLevelAssetCacheItem(gameObject, removeCached);
        GameController.ResetLevelAssetCacheItem(gameObject, removeCached);

        //foreach (PoolGameObject item in gameObject.GetList<PoolGameObject>()) {
        //    item.gameObject.DestroyGameObject();
        //}

        foreach(GameObjectInfinitePartItem partItem in
            gameObject.GetList<GameObjectInfinitePartItem>()) {

            partItem.ClearItems(removeCached);
        }

        foreach(GameObjectInfinitePart part in
            gameObject.GetList<GameObjectInfinitePart>()) {

            part.ClearItems(removeCached);
        }

        gameObject.DestroyChildren();
    }

    public void DestroyItems(bool removeCached = false) {

        ClearItems(removeCached);

        gameObject.DestroyGameObject();
    }
}
