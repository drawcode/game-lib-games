using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;

// CUSTOM

public enum GameZones {
    left,
    right
}

public class BaseGameContentDisplayTypes {
    public static string gamePlayerOutOfBounds = "content-game-player-out-of-bounds";
    public static string gameChoices = "content-game-game-choices";
    public static string gameChoicesOverview = "content-game-game-choices-overview";
    public static string gameChoicesItemStart = "content-game-game-choices-item-start";
    public static string gameChoicesItemResult = "content-game-game-choices-item-result";
    public static string gameCollect = "content-game-game-collect";
    public static string gameCollectOverview = "content-game-game-collect-overview";
    public static string gameCollectItemStart = "content-game-game-collect-item-start";
    public static string gameCollectItemResult = "content-game-game-collect-item-result";
    public static string gameEnergy = "content-game-game-energy";
    public static string gameHealth = "content-game-game-health";
    public static string gameXP = "content-game-game-xp";
    public static string gameTips = "content-game-tips";
    public static string gameTutorial = "content-game-tutorial";
    public static string gameModeContentOverview = "content-game-mode-content-overview";
}

// GLOBAL

public enum GameControllerType {
    Iso2DSide,
    Iso3D,
    Iso2DTop,
    Perspective3D
}

public class BaseGamePlayerMessages {

    public static string PlayerAnimation = "playerAnimation";
    public static string PlayerAnimationSkill = "skill";
    public static string PlayerAnimationAttack = "attack";
    public static string PlayerAnimationFall = "fall";
}

public enum GameStateGlobal {
    GameNotStarted,
    GameInit,
    GamePrepare,
    GameStarted,
    GameQuit,
    GamePause,
    GameResume,
    GameResults,
    GameContentDisplay, // dialog or in progress choice/content/collection status
    GameOverlay, // external dialog such as sharing/community/over
}

public class GameActorType {
    public static string enemy = "enemy";
    public static string player = "player";
    public static string sidekick = "sidekick";
}

public class GameSpawnType {
    public static string centeredType = "centered";
    public static string zonedType = "zoned";
    public static string pointsType = "points";
    public static string explicitType = "explicit";
    public static string randomType = "random";
}

public class GameActorDataItem : GameDataObject {

    // RPG
    
    public virtual GameDataItemRPG rpg {
        get {
            return Get<GameDataItemRPG>(BaseDataObjectKeys.rpg, new GameDataItemRPG());
        }
        
        set {
            Set<GameDataItemRPG>(BaseDataObjectKeys.rpg, value);
        }
    }

    public GameActorDataItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        rpg = new GameDataItemRPG();

        code = "";
        type = BaseDataObjectKeys.character;
        data_type = GameSpawnType.zonedType;
        display_type = GameActorType.enemy;
        rotation_data = new Vector3Data();
        position_data = new Vector3Data(0, 0, 0);
        scale_data = new Vector3Data(1, 1, 1);
    }
}

public class BaseGameMessages {
    //
    public static string gameActionItem = "game-action-item";
    public static string gameActionScores = "game-action-scores";
    public static string gameActionScore = "game-action-score";
    public static string gameActionAmmo = "game-action-ammo";
    public static string gameActionSave = "game-action-save";
    public static string gameActionShot = "game-action-shot";
    public static string gameActionLaunch = "game-action-launch";
    public static string gameActionState = "game-action-state";
    //
    public static string gameInitLevelStart = "game-init-level-start";
    public static string gameInitLevelEnd = "game-init-level-end";
    public static string gameLevelStart = "game-level-start";
    public static string gameLevelEnd = "game-level-end";
    public static string gameLevelQuit = "game-level-quit";
    public static string gameLevelPause = "game-level-pause";
    public static string gameLevelResume = "game-level-resume";
    public static string gameResultsStart = "game-results-start";
    public static string gameResultsEnd = "game-results-end";
}

public class BaseGameStatCodes {
    
    public static string timesPlayed = "times-played";//
    public static string timePlayed = "time-played";//
    public static string timesPlayedAction = "times-played-action";//

    // totals
    public static string wins = "wins";//--
    public static string losses = "losses";//--
    public static string shots = "shots";//--
    public static string destroyed = "destroyed";//--

    public static string score = "score";//--
    public static string scores = "scores";//--
    public static string evaded = "evaded";//--
    public static string kills = "kills";
    public static string deaths = "deaths";
    public static string hits = "hits";
    public static string hitsReceived = "hits-received";
    public static string hitsObstacles = "hits-obstacles";
    public static string xp = "xp";
    public static string coins = "coins";
    public static string coinsPickup = "coinsPickup";
    public static string coinsPurchased = "coinsPurchased";
    public static string coinsEarned = "coinsEarned";
    public static string ammo = "ammo";
    public static string attacks = "attacks";//
    public static string defends = "defends";//

    public static string cuts = "cuts";//
    public static string cutsLeft = "cuts-left";//
    public static string cutsRight = "cuts-right";//
    public static string boosts = "boosts";//
    public static string spins = "spins";//
    public static string total = "total";//
    public static string item = "item";//

    // lows

    // absolute

    // accumulate (total)
}

public class BaseGameGameRuntimeData {
    public double currentLevelTime = 0;
    public double timeRemaining = 90;
    public double coins = 0;
    public string levelCode = "";
    public double score = 0;
    public bool outOfBounds = false;
    
    public BaseGameGameRuntimeData() {
        Reset();
    }
    
    public virtual void Reset() {
        currentLevelTime = 0;
        timeRemaining = 90;
        coins = 0;
        levelCode = "";
        score = 0;
        outOfBounds = false;
        ResetTimeDefault();
    }
    
    public virtual bool timeExpired {
        get {
            if (timeRemaining <= 0) {
                timeRemaining = 0;
                return true;
            }
            return false;
        }
    }

    public virtual bool localPlayerWin {
        get {
            return !timeExpired;
        }
    }
    
    public virtual void SubtractTime(double delta) {
        if (timeRemaining > 0) {
            timeRemaining -= delta;
        }
    }
    
    public virtual void ResetTimeDefault() {
        timeRemaining = 90;
    }
    
    public virtual void ResetTime(double timeTo) {
        timeRemaining = timeTo;
    }
    
    public virtual void AppendTime(double timeAppend) {
        timeRemaining += timeAppend;
    }
}

public class GameLevelItemDataType {
    public static string randomType = "random";
    public static string explicitType = "explicit";
}

public class GameLevelItemData {
    public string code = "rock-1";
    public int count = 1;
    public Vector3 pos = Vector3.zero;
    public string type = GameLevelItemDataType.randomType;
    
    public GameLevelItemData() {
        
    }
    
    public GameLevelItemData(string codeTo, string typeTo, int countTo, Vector3 posTo) {
        code = codeTo;
        type = typeTo;
        count = countTo;
        pos = posTo;
    }
    
}

public class GameLevelTemplate {
    public Dictionary<string, GameLevelItemData> randomAssets = new Dictionary<string, GameLevelItemData>();
    public Dictionary<string, Vector3> placedAssets = new Dictionary<string, Vector3>();
}

public class GameLevelGridData {
    public float gridHeight = GameLevels.gridHeight;
    public float gridWidth = GameLevels.gridWidth;
    public float gridDepth = GameLevels.gridDepth;
    public float gridBoxSize = GameLevels.gridBoxSize;
    public bool centeredX = GameLevels.centeredX;
    public bool centeredY = GameLevels.centeredY;
    public bool centeredZ = GameLevels.centeredZ;
    public List<string> presets = new List<string>();
    public List<AppContentAsset> assets;
    //public string[,,] assetMap;

    public Dictionary<string, GameLevelItemAssetData> assetLayoutData;

    public GameLevelGridData() {
        Reset();
    }

    public void Reset() {
        ResetGrid((int)gridHeight, (int)gridWidth, (int)gridDepth);
        ClearAssets();
        ClearMap();
        ClearPresets();
    }

    public void ResetGrid(int height, int width, int depth) {
        ResetGrid(height, width, depth, (int)gridBoxSize, true, false, true);
    }

    public void ResetGrid(int height, int width, int depth, int boxSize, bool centerX, bool centerY, bool centerZ) {
        gridHeight = (float)height;
        gridWidth = (float)width;
        gridDepth = (float)depth;
        gridBoxSize = (float)boxSize;

        centeredX = centerX;
        centeredY = centerY;
        centeredZ = centerZ;
    }

    public static GameLevelGridData GetLevelTemplate(string template) {

        //string wall1 = "wall-1";

        //GameLevelGridData gridData = new GameLevelGridData();


        return GameLevels.GetLevelGridBaseDefault();
        
        /*
        GameLevelGridData data = new GameLevelGridData();
        data = AddAssets(data, "bush-1", UnityEngine.Random.Range(4,8));
        data = AddAssets(data, "box-1", UnityEngine.Random.Range(2, 4));
        data = AddAssets(data, "padding-1", UnityEngine.Random.Range(1, 3));

        return data;
        */
    }

    public static GameLevelGridData GetBaseDefault() {

        return GameLevels.GetLevelGridBaseDefault();

        /*
        GameLevelGridData data = new GameLevelGridData();
        data = AddAssets(data, "bush-1", UnityEngine.Random.Range(4,8));
        data = AddAssets(data, "box-1", UnityEngine.Random.Range(2, 4));
        data = AddAssets(data, "padding-1", UnityEngine.Random.Range(1, 3));

        return data;
        */
    }

    public static GameLevelGridData GetDefault() {

        GameLevelGridData data = GameLevelGridData.GetBaseDefault();
        data.RandomizeAssetsInAssetMap();

        return data;
    }

    public static GameLevelGridData GetTemplate(string templateCode) {
        
        GameLevelGridData data = GameLevelGridData.GetLevelTemplate(templateCode);
        //data.RandomizeAssetsInAssetMap();
        
        return data;
    }

    public static GameLevelGridData GetModeTypeChoice(int choiceCount) {

        GameLevelGridData data = GameLevelGridData.GetBaseDefault();
        //data = AddAssets(data, "game-choice-item", choiceCount);
        data.RandomizeAssetsInAssetMap();

        return data;
    }

    public static GameLevelGridData AddAssets(GameLevelGridData data, string assetCode) {
        data.SetAssets(assetCode, 1);
        return data;
    }

    public static GameLevelGridData AddAssets(GameLevelGridData data, string assetCode, int count) {
        data.SetAssets(assetCode, count);
        return data;
    }

    //public string[,,] GetAssetMap() {
    //    return assetMap;
    //}

    public Dictionary<string, GameLevelItemAssetData> GetAssetLayoutData() {
        return assetLayoutData;
    }

    public void ClearAssets() {
        assets = new List<AppContentAsset>();
    }

    public void ClearMap() {
        //assetMap = new string[(int)gridWidth, (int)gridHeight, (int)gridDepth];
        assetLayoutData = new Dictionary<string, GameLevelItemAssetData>();
    }
    
    public void ClearPresets() {
        presets = new List<string>();
    }

    public void SetAssets(string code, int count) {
        for (int i = 0; i < count; i++) {
            SetAsset(code);
        }
    }

    public void SetAsset(string code) {
        AppContentAsset asset = AppContentAssets.Instance.GetByCode(code);
        if (asset != null) {
            //if(!HasAsset(asset.code)) {
            assets.Add(asset);
            //}
        }
    }

    public bool HasAsset(string code) {
        foreach (AppContentAsset asset in assets) {
            if (asset.code == code) {
                return true;
            }
        }

        return false;
    }

    public void SetAssetsInAssetMap(
        string code, string type, string dataType, string displayType, Vector3 pos) {

        GameLevelItemAssetData assetData = new GameLevelItemAssetData();

        assetData.code = code;
        assetData.type = type;
        assetData.data_type = dataType;
        assetData.display_type = displayType;
        assetData.position_data = new Vector3Data(pos);
        assetData.SetAssetScaleRange(.7f, 1.2f);
        assetData.SetAssetRotationRangeY(-180, 180);

        SetAssetsInAssetMap(assetData);
    }

    public void SetAssetsInAssetMap(
        string code, string type, string dataType, string displayType, 
        Vector3 pos, Vector3 scale, Vector3 rotation) {
        
        GameLevelItemAssetData assetData = new GameLevelItemAssetData();
        
        assetData.code = code;
        assetData.type = type;
        assetData.data_type = dataType;
        assetData.display_type = displayType;
        assetData.position_data = new Vector3Data(pos);
        assetData.scale_data = new Vector3Data(scale);
        assetData.rotation_data = new Vector3Data(rotation);
        
        SetAssetsInAssetMap(assetData);
    }

    public void SetAssetsInAssetMap(
        string code, string type, string dataType, string displayType, 
        Vector3 pos, Vector3 scale, Vector3 rotation, Vector3 localPosition) {
        
        GameLevelItemAssetData assetData = new GameLevelItemAssetData();
        
        assetData.code = code;
        assetData.type = type;
        assetData.data_type = dataType;
        assetData.display_type = displayType;
        assetData.position_data = new Vector3Data(pos);
        assetData.scale_data = new Vector3Data(scale);
        assetData.rotation_data = new Vector3Data(rotation);
        assetData.local_position_data = new Vector3Data(localPosition);
        
        SetAssetsInAssetMap(assetData);
    }

    public void SetAssetsInAssetMap(GameLevelItemAssetData assetData) {

        Vector3 pos = assetData.position_data.GetVector3();

        if (pos.x > gridWidth - 1) {
            pos.x = gridWidth - 1;
        }

        if (pos.y > gridHeight - 1) {
            pos.y = gridHeight - 1;
        }

        if (pos.z > gridDepth - 1) {
            pos.z = gridDepth - 1;
        }

        string keyLayout = 
            string.Format(
                "{0}-{1}-{2}", 
                (int)pos.x, 
                (int)pos.y, 
                (int)pos.z);

        assetData.position_data.FromVector3(pos);

        if (!assetLayoutData.ContainsKey(keyLayout)) {

            //Debug.Log("SetAssetsIntoMap:" + keyLayout);

            assetLayoutData.Set(keyLayout, assetData);
        }
    }

    // data.rangeScale = Vector3.zero.WithX(.7f).WithY(1.2f);
    // data.range_rotation = Vector3.zero.WithX(-180).WithY(180); 

    public void RandomizeAssetsInAssetMap() {

        foreach (AppContentAsset asset in assets) {

            int x = 0;
            int y = 0;
            int z = 0;

            x = UnityEngine.Random.Range(0, (int)gridWidth - 1);
            y = UnityEngine.Random.Range(0, (int)gridHeight - 1);
            z = UnityEngine.Random.Range(0, (int)gridDepth - 1);

            int midX = ((int)((gridWidth - 1) / 2));
            // TODO 2d version
            //int midY = ((int)((gridHeight - 1) / 2));
            int midZ = ((int)((gridDepth - 1) / 2));

            // Dont' add if in the middle spawn area until player
            // items grid out in level data.
            // TODO switch to area around player to gid out items
            // if spawns on level items
          
            if ((x < (midX + 2)) && (x > (midX - 2))
                && (z < (midZ + 2)) && (z > (midZ - 2))) {
                continue;
            }

            string keyLayout = string.Format("{0}-{1}-{2}", x, y, z);

            if (!assetLayoutData.ContainsKey(keyLayout)) {
                Vector3 pos = Vector3.one.WithX(x).WithY(y).WithZ(z);
                SetAssetsInAssetMap(asset.code, asset.type, asset.data_type, asset.display_type, pos);
            }
        }
    }
}

public enum GameCameraView {
    ViewSide, // tecmo
    ViewSideTop, // tecmo
    ViewBackTilt, // backbreaker cam
    ViewBackTop // john elway cam
}

public enum GameRunningState {
    PAUSED,
    RUNNING,
    STOPPED
}

public class BaseGameController : GameObjectTimerBehavior {

    public GamePlayerController currentGamePlayerController;
    public Dictionary<string, GamePlayerController> gamePlayerControllers;
    public Dictionary<string, GamePlayerProjectile> gamePlayerProjectiles;
    public List<string> gameCharacterTypes = new List<string>();
    int currentCharacterTypeIndex = 0;
    //int lastCharacterTypeIndex = 0;

    public bool initialized = false;
    public bool allowedEditing = true;
    public bool isAdvancing = false;
    public GameStateGlobal gameState = GameStateGlobal.GameNotStarted;
    public UnityEngine.Object prefabDraggableContainer;
    public Dictionary<string, GameLevelItemAsset> levelGrid = null;
    public List<GameLevelItemAsset> levelItems = null;
    public GameObject levelBoundaryContainerObject;
    public GameObject levelContainerObject;
    public GameObject levelItemsContainerObject;
    public GameObject levelItemsTempContainerObject;
    public GameObject levelActorsContainerObject;
    public GameObject levelZonesContainerObject;
    public GameObject levelSpawnsContainerObject;
    public GameObject levelMarkersContainerObject;
    public GameObject itemContainerObject;
    public GameObject worldTerrainContainerObject;
    public GameObject gameContainerObject;
    public GameObject boundaryEdgeObjectTopRight;
    public GameObject boundaryEdgeObjectTopLeft;
    public GameObject boundaryEdgeObjectBottomRight;
    public GameObject boundaryEdgeObjectBottomLeft;
    public GameObject boundaryObjectTopRight;
    public GameObject boundaryObjectTopLeft;
    public GameObject boundaryObjectBottomRight;
    public GameObject boundaryObjectBottomLeft;
    public GameBounds gameBounds;
    public GameObject boundaryTopLeft;
    public GameObject boundaryTopRight;
    public GameObject boundaryBottomLeft;
    public GameObject boundaryBottomRight;
    public GameObject boundaryTopCeiling;
    public GameObject boundaryBottomAbyss;
    public GameObject gameEndZoneLeft;
    public GameObject gameEndZoneRight;
    public GameGameRuntimeData runtimeData;
    public Camera cameraGame;
    public Camera cameraGameGround;
    public GameCameraView cameraView = GameCameraView.ViewSide;
    public GameRunningState gameRunningState = GameRunningState.STOPPED;
    public GameControllerType gameControllerType = GameControllerType.Iso3D;
    float currentTimeBlockBase = 0.0f;
    float actionIntervalBase = 1.3f;
    public float defaultLevelTime = 90;
    public string contentDisplayCode = "default";
    public bool isGameOver = false;
    public bool updateFingerNavigate = false;
    
    // CUSTOM
    
    public GameZones currentGameZone = GameZones.right;    
    
    // CAMERAS
    
    public List<Camera> camerasAlwaysOn;
    public List<Camera> camerasGame;
    public List<Camera> camerasUI;
    public GameObject camerasContainerGame;
    public GameObject cameraContainersUI;
    public GameObject cameraContainersAlwaysOn;
    public float runDirectorsDelay = 10f;

    // ----------------------------------------------------------------------

    public static bool isFingerNavigating {
        get {
            if (GameController.Instance != null) {
                return GameController.Instance.updateFingerNavigate;
            }
            return false;
        }
    }

    public virtual void Awake() {

    }

    public virtual void Start() {

    }

    public virtual void Init() {

        GameController.Reset();

        foreach (GamePlayerController gamePlayerController in UnityObjectUtil.FindObjects<GamePlayerController>()) {
            if (gamePlayerController.uniqueId == UniqueUtil.Instance.currentUniqueId) {
                gamePlayerController.UpdateNetworkContainer(gamePlayerController.uniqueId);
                break;
            }
        }
        
        GameController.InitGameWorldBounds();
        
        GameController.LoadCharacterTypes();
        GameDraggableEditor.LoadDraggableContainerObject();

        initCustomProfileCharacters();
    }
        
    public virtual void initCustomProfileCharacters() {       
        
        StartCoroutine(initCustomProfileCharactersCo());
    }

    public virtual IEnumerator initCustomProfileCharactersCo() {
        yield return new WaitForEndOfFrame();
        
        GameCustomController.BroadcastCustomCharacterProfileCodeSync();
        GameCustomController.BroadcastCustomSync();
    }

    public virtual void OnEnable() {
        Gameverses.GameMessenger<string>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameAIDirectorData>.AddListener(
            GameAIDirectorMessages.gameAIDirectorSpawnActor,
            OnGameAIDirectorData);

        Messenger<GameItemData>.AddListener(
            GameItemDirectorMessages.gameItemDirectorSpawnItem,
            OnGameItemDirectorData);

        Messenger.AddListener(BaseGameProfileMessages.ProfileShouldBeSaved, OnProfileShouldBeSavedEventHandler);
    }
    
    public virtual void OnDisable() {
        Gameverses.GameMessenger<string>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameAIDirectorData>.RemoveListener(
            GameAIDirectorMessages.gameAIDirectorSpawnActor,
            OnGameAIDirectorData);

        Messenger<GameItemData>.RemoveListener(
            GameItemDirectorMessages.gameItemDirectorSpawnItem,
            OnGameItemDirectorData);

        Messenger.RemoveListener(BaseGameProfileMessages.ProfileShouldBeSaved, OnProfileShouldBeSavedEventHandler);
    }


    // ---------------------------------------------------------------------
    
    // PROPERTIES
    
    public int characterActorsCount {
        get {
            return levelActorsContainerObject.transform.childCount;
        }
    }
    
    public int collectableItemsCount {
        get {
            return itemContainerObject.transform.childCount;
        }
    }

    public static bool shouldRunGame {
        get {

            if (GameDraggableEditor.isEditing) {
                return false;
            }

            return true;

        }
    }
    
    // ---------------------------------------------------------------------
    
    // EVENTS

    public virtual void OnProfileShouldBeSavedEventHandler() {
        if (!GameConfigs.isGameRunning) {
            GameState.SaveProfile();
        }
    }
    
    public virtual void OnEditStateHandler(GameDraggableEditEnum state) {
    
        if (state == GameDraggableEditEnum.StateEditing) {
            ////GameHUD.Instance.ShowCurrentCharacter();
        }
        else {

            GameHUD.Instance.ShowGameState();
    
            GameUIController.ShowHUD();
            //ShowUIPanelEditButton();
        }
    }   
    
    // Listen to object creation events and create them such as network players...
    
    public virtual void OnNetworkPlayerContainerAdded(string uid) {
    
        // Look for object by that uuid, if not create it

        LogUtil.Log("OnNetworkPlayerContainerAdded:uid:", uid);

        if (uid == UniqueUtil.Instance.currentUniqueId
            || string.IsNullOrEmpty(uid)) {
            return;
        }

        GamePlayerController[] playerControllers = UnityObjectUtil.FindObjects<GamePlayerController>();
    
        if (playerControllers.Length > 0) {
    
            bool found = false;

            foreach (GamePlayerController gamePlayerController in playerControllers) {
                if (gamePlayerController.uniqueId == uid) {
                    // already added
                    gamePlayerController.uniqueId = uid;
                    gamePlayerController.UpdateNetworkContainer(uid);
                    //gamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerNetwork);
                    LogUtil.Log("OnNetworkPlayerContainerAdded:Updating character:" + uid);
                    found = true;
                    break;
                }
            }

            if (!found) {
                // create
                // Prefabs/Characters/GamePlayerObject

                string pathPlayer = Path.Combine(
                    ContentPaths.appCacheVersionSharedPrefabCharacters,
                    "GamePlayerObject");
    
                UnityEngine.Object prefabGameplayer = Resources.Load(pathPlayer);
                if (prefabGameplayer != null) {
                    Vector3 placementPos = Vector3.zero;
                    placementPos.z = -3f;
                    GamePlayerController playerControllerOther = 
                        (Instantiate(prefabGameplayer, placementPos, Quaternion.identity) 
                         as GameObject).GetComponent<GamePlayerController>();
                    playerControllerOther.ChangePlayerState(GamePlayerControllerState.ControllerNetwork);
                    playerControllerOther.UpdateNetworkContainer(uid);
                    LogUtil.Log("OnNetworkPlayerContainerAdded:Creating character:" + uid);
                    LogUtil.Log("OnNetworkPlayerContainerAdded:playerControllerOther.uniqueId:" + playerControllerOther.uniqueId);
                }
            }
        }
    }

    public virtual void OnGameAIDirectorData(GameAIDirectorData actor) {
        GameController.LoadActor(actor.code, actor.type);
    }

    public virtual void OnGameItemDirectorData(GameItemData item) {

        GameController.LoadItem(item.code);
    }

    // ---------------------------------------------------------------------

    // GAMEPLAYER CONTROLLER   
    
    public virtual GamePlayerController getCurrentPlayerController {
        get {
            return getCurrentController();
        }
    }

    public virtual GamePlayerController getCurrentController() {
        if (GameController.Instance.currentGamePlayerController != null) {
            return GameController.Instance.currentGamePlayerController;
        }
        return null;
    }

    public virtual GamePlayerController getGamePlayerController(string uid) {
        foreach (GamePlayerController gamePlayerController 
                in UnityObjectUtil.FindObjects<GamePlayerController>()) {
            if (gamePlayerController.uniqueId == uid) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual GamePlayerController getGamePlayerController(GameObject go) {
        if (go != null) {
            GamePlayerController gamePlayerController = go.GetComponentInChildren<GamePlayerController>();
            if (gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual GamePlayerController getGamePlayerControllerParent(GameObject go) {
        if (go != null) {
            GamePlayerCollision gamePlayerCollision = go.Get<GamePlayerCollision>();

            if (gamePlayerCollision == null) {
                return null;
            }

            if (gamePlayerCollision.gamePlayerController == null) {
                return null;
            }
               
            GamePlayerController gamePlayerController = gamePlayerCollision.gamePlayerController;
            if (gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual GamePlayerController getGamePlayerControllerObject(GameObject go, bool onlyPlayerControlled) {

        GamePlayerController gamePlayerController = null;

        if (go == null) {
            return gamePlayerController;
        }

        if (go.name.Contains("GamePlayerObject")) {

            gamePlayerController = GameController.GetGamePlayerController(go);

            if (gamePlayerController != null) {

                if (!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return gamePlayerController;
                }
            }
        }

        if (gamePlayerController == null
            && (go.name.Contains("Game")
            || go.name.Contains("GamePlayerCollider"))) {
            //&& (go.name.Contains("Helmet")
            //|| go.name.Contains("Facemask"))) {

            //LogUtil.Log("GameObjectChoice:HelmetFacemask:" + go.name);

            gamePlayerController = GameController.GetGamePlayerControllerParent(go);

            if (gamePlayerController != null) {

                //LogUtil.Log("GameObjectChoice:gamePlayerController:" + gamePlayerController.name);

                if (!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return gamePlayerController;
                }
            }
        }

        return gamePlayerController;
    }

    public virtual bool hasGamePlayerControllerObject(GameObject go, bool onlyPlayerControlled) {

        GamePlayerController gamePlayerController = null;

        if (go == null) {
            return false;
        }

        if (go.name.Contains("GamePlayerObject")) {

            gamePlayerController = GameController.GetGamePlayerController(go);

            if (gamePlayerController != null) {

                if (!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return true;
                }
            }
        }

        if (gamePlayerController == null
            && (go.name.Contains("Game")
            || go.name.Contains("Helmet")
            || go.name.Contains("Facemask"))) {

            //LogUtil.Log("GameObjectChoice:HelmetFacemask:" + go.name);

            gamePlayerController = GameController.GetGamePlayerControllerParent(go);

            if (gamePlayerController != null) {

                //LogUtil.Log("GameObjectChoice:gamePlayerController:" + gamePlayerController.name);

                if (!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return true;
                }
            }
        }

        return false;
    }

    // SCORING

    public virtual void gamePlayerScores(double val) {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.Scores(val);
        }
    }
 
    // ATTACK
    
    //public static void GamePlayerAttack() {
    //    if(isInst) {
    //        Instance.gamePlayerAttack();
    //    }
    //}
    
    public virtual void gamePlayerAttack() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttack();
        }
    }

    //public static void GamePlayerAttackAlt() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackAlt();
    //    }
    //}

    public virtual void gamePlayerAttackAlt() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackAlt();
        }
    }

    //public static void GamePlayerAttackRight() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackRight();
    //    }
    //}

    public virtual void gamePlayerAttackRight() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackRight();
        }
    }

    //public static void GamePlayerAttackLeft() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackLeft();
    //    }
    //}

    public virtual void gamePlayerAttackLeft() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackLeft();
        }
    }

    // DEFEND

    //public static void GamePlayerDefend() {
    //    if(isInst) {
    //        Instance.gamePlayerDefend();
    //    }
    //}

    public virtual void gamePlayerDefend() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefend();
        }
    }

    //public static void GamePlayerDefendAlt() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendAlt();
    //    }
    //}

    public virtual void gamePlayerDefendAlt() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendAlt();
        }
    }

    //public static void GamePlayerDefendRight() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendRight();
    //    }
    //}

    public virtual void gamePlayerDefendRight() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendRight();
        }
    }

    //public static void GamePlayerDefendLeft() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendLeft();
    //    }
    //}

    public virtual void gamePlayerDefendLeft() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendLeft();
        }
    }
 
    // JUMP

    //public static void GamePlayerJump() {
    //    if(isInst) {
    //        Instance.gamePlayerJump();
    //    }
    //}
    
    public virtual void gamePlayerJump() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputJump();
        }
    }
     
    // USE
    
    //public static void GamePlayerUse() {
    //    if(isInst) {
    //        Instance.gamePlayerUse();
    //    }
    //}
    
    public virtual void gamePlayerUse() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputUse();
        }
    }
    
    // MOUNT
    
    //public static void GamePlayerUse() {
    //    if(isInst) {
    //        Instance.gamePlayerUse();
    //    }
    //}
    
    public virtual void gamePlayerMount() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputMount();
        }
    }

    // SKILL

    //public static void GamePlayerSkill() {
    //    if(isInst) {
    //        Instance.gamePlayerSkill();
    //    }
    //}

    public virtual void gamePlayerSkill() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputSkill();
        }
    }

    // MAGIC

    //public static void GamePlayerMagic() {
    //    if(isInst) {
    //        Instance.gamePlayerMagic();
    //    }
    //}

    public virtual void gamePlayerMagic() {
        if (GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputMagic();
        }
    }

    // ----------------------------------------------------------------------

    // ZONES

    public virtual GameZone getGameZone(GameObject go) {
        if (go != null) {
            return go.GetComponent<GameZone>();
        }
        return null;
    }

    public virtual GameGoalZone getGoalZone(GameObject go) {
        if (go != null) {
            return go.GetComponent<GameGoalZone>();
        }
        return null;
    }

    public virtual GameBadZone getBadZone(GameObject go) {
        if (go != null) {
            return go.GetComponent<GameBadZone>();
        }
        return null;
    }

    public virtual void changeGameZone(GameZones zones) {

        if (gameEndZoneLeft == null) {
            Transform gameEndZoneLeftTransform
                = levelZonesContainerObject.transform.FindChild("GameGoalZoneLeft");
            if (gameEndZoneLeftTransform != null) {
                gameEndZoneLeft = gameEndZoneLeftTransform.gameObject;
            }
        }

        if (gameEndZoneLeft == null) {
            Transform gameEndZoneRightTransform
                = levelZonesContainerObject.transform.FindChild("GameGoalZoneRight");
            if (gameEndZoneRightTransform != null) {
                gameEndZoneRight = gameEndZoneRightTransform.gameObject;
            }
        }

        GameController.GoalZoneChange(zones);
    }

    // ---------------------------------------------------------------------
    
    // BOUNDS
    
    public virtual void initGameWorldBounds() {
        if (gameBounds == null) {
            gameBounds = gameObject.AddComponent<GameBounds>();
            gameBounds.boundaryTopLeft = boundaryTopLeft;
            gameBounds.boundaryTopRight = boundaryTopRight;
            gameBounds.boundaryBottomLeft = boundaryBottomLeft;
            gameBounds.boundaryBottomRight = boundaryBottomRight;
            gameBounds.boundaryTopCeiling = boundaryTopCeiling;
            gameBounds.boundaryBottomAbyss = boundaryBottomAbyss;
        }
    }

    public virtual bool checkBounds(Vector3 point) {
        return gameBounds.CheckBounds(point);
    }

    public virtual Vector3 filterBounds(Vector3 point) {
        return gameBounds.FilterBounds(point);
    }

    public virtual bool shouldUpdateBounds() {
        return gameBounds.ShouldUpdateBounds();
    }

    // ---------------------------------------------------------------------

    // LEVELS

    public virtual void loadLevelAssets() {
        GameController.LoadLevelAssets(GameLevels.Current.code);
    }

    public virtual void loadLevelAssets(string code) {

        // LogUtil.Log("GAME START FLOW: STEP #10: loadLevelAssets: code:" + code);

        GameDraggableEditor.levelItemsContainerObject = GameController.Instance.levelItemsContainerObject;
    
        //GameLevelItems.Current.code = code;
            
        // Clear items from LevelContainer
        GameController.Reset();

        Debug.Log("loadLevelAssets:" + " code:" + code);

        // Change data codes
        GameLevels.Instance.ChangeCurrentAbsolute(code);
        GameLevelItems.Instance.ChangeCurrentAbsolute(code);

        // Prepare game level items for this mode
        GameController.LoadLevelItems();
    
        // Load in the level assets
        GameDraggableEditor.LoadLevelItems();
    
    }

    public virtual void loadLevel(string code) {

        //LogUtil.Log("GAME START FLOW: STEP #6: loadLevel: code:" + code);

        // Load the game levelitems for the game level code
        ////GameController.StartGame(code);
        GameController.PrepareGame(code);

        //GameController.LoadLevelAssets(code);
    
        //GameHUD.Instance.SetLevelInit(GameLevels.Current);
    
        //GameHUD.Instance.AnimateIn();
    
        //GameUI.Instance.ToggleGameUI();
    
    }
    
    public virtual void loadLevelItems() {

        bool updated = false;

        // Load level items by game type....

        if (AppModes.Instance.isAppModeGameChallenge) {

            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppModeGameChallenge:"
                + AppModes.Instance.isAppModeGameChallenge);

            if (AppModeTypes.Instance.isAppModeTypeGameDefault) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                    + AppModeTypes.Instance.isAppModeTypeGameDefault);

                if (AppContentStates.Instance.isAppContentStateGameChallenge) {

                    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameChallenge:"
                        + AppContentStates.Instance.isAppContentStateGameChallenge);

                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                    updated = true;
                }
            }
        }
        else if (AppModes.Instance.isAppModeGameTraining) {
            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppModeGameTraining:"
                + AppModes.Instance.isAppModeGameTraining);

            if (AppModeTypes.Instance.isAppModeTypeGameDefault) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                    + AppModeTypes.Instance.isAppModeTypeGameDefault);

            }
            else if (AppModeTypes.Instance.isAppModeTypeGameChoice) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameChoice:"
                    + AppModeTypes.Instance.isAppModeTypeGameChoice);

                // LOAD CHOICE GAME LEVEL ITEMS

                if (AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz) {

                    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingChoiceQuiz:"
                        + AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz);

                    //



                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid(GameLevelGridData.GetModeTypeChoice(4));
                    updated = true;
                }

            }
            else if (AppModeTypes.Instance.isAppModeTypeGameCollection) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameCollection:"
                    + AppModeTypes.Instance.isAppModeTypeGameCollection);

                // LOAD COLLECTION GAME LEVEL ITEMS

                /*
                if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts) {

                    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingCollectionSmarts:"
                        + AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts);

                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                }
                else if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {

                    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingCollectionSafety:"
                        + AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety);

                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                }*/

            }
            else if (AppModeTypes.Instance.isAppModeTypeGameContent) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameContent:"
                    + AppModeTypes.Instance.isAppModeTypeGameContent);

            }
            else if (AppModeTypes.Instance.isAppModeTypeGameTips) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameTips:"
                    + AppModeTypes.Instance.isAppModeTypeGameTips);

            }
        }
        else {
            // if(AppModes.Instance.isAppModeGameArcade) {
            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppModeGameArcade:"
                + AppModes.Instance.isAppModeGameArcade);
            
            if (AppModeTypes.Instance.isAppModeTypeGameDefault) {
                
                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                    + AppModeTypes.Instance.isAppModeTypeGameDefault);
                
                if (AppContentStates.Instance.isAppContentStateGameArcade) {
                    
                    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameArcade:"
                        + AppContentStates.Instance.isAppContentStateGameArcade);
                    
                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                    updated = true;
                }
            }
            //}
        }

        if (!updated) {
            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGrid();
        }
    }

    public virtual void saveCurrentLevel() {
        GameDraggableEditor.SaveCurrentLevel(
            GameController.Instance.levelItemsContainerObject);
    }

    public virtual void playGame() {

        if (!levelInitializing) {
        
            //AdNetworks.ShowFullscreenAd();
            
            //if(string.IsNullOrEmpty(GameLevels.Current.code)) {
            //    GameLevels.Instance.ChangeCurrentAbsolute("1-1");
            //}
            
            //UITweenerUtil.CameraColor(new Color(1f, 0f, 0f, .5f));    
            //UITweenerUtil.CameraColor(new Color(1f, 0f, 0f, .5f));
                        
            GameController.LoadStartLevel();
        }
    }

    public virtual void initLevel(string levelCode) {
        StartCoroutine(initLevelCo(levelCode));
    }

    public bool levelInitializing = false;
       
    public virtual IEnumerator initLevelCo(string levelCode) {

        //LogUtil.Log("GAME START FLOW: STEP #5: startLevelCo: levelCode:" + levelCode);
        levelInitializing = true;

        //GameController.ResetRuntimeData();

        if (currentGamePlayerController != null) {
            currentGamePlayerController.PlayerEffectWarpFadeIn();
        }

        UIPanelOverlayPrepare.ShowDefault();

        GameUIPanelOverlays.Instance.ShowOverlayWhite();

        yield return new WaitForSeconds(2f);

        // PRELOAD/CACHE/POOL CONTROLLERS

        yield return StartCoroutine(GameAIController.Instance.preloadCo());
        
        yield return StartCoroutine(GameItemController.Instance.preloadCo());
        
        GameHUD.Instance.ResetIndicators();

        // TODO load customizations
        GameController.LoadLevel(levelCode);
        
        // TODO load anim
    }
    
    public virtual void initLevelFinish(string levelCode) {
        StartCoroutine(initLevelFinishCo(levelCode));
    }
    
    public virtual IEnumerator initLevelFinishCo(string levelCode) {        
        if (UIPanelOverlayPrepare.Instance != null) {
            UIPanelOverlayPrepare.Instance.ShowTipsObjectMode();
        }
                       
        Messenger<string>.Broadcast(GameMessages.gameInitLevelStart, levelCode);

        yield return new WaitForSeconds(1f);
        
        if (currentGamePlayerController != null) {
            currentGamePlayerController.PlayerEffectWarpFadeOut();
        }
        
        GameUIPanelOverlays.Instance.HideOverlayWhiteFlashOut();
        
        UIPanelOverlayPrepare.HideAll();
        
        Messenger<string>.Broadcast(GameMessages.gameInitLevelEnd, levelCode);
                
        Messenger<string>.Broadcast(GameMessages.gameLevelStart, levelCode);
                
        //UIPanelOverviewMode.ShowDefault();

        levelInitializing = false;
    }
    
    public virtual void startLevel(string levelCode) {
        initLevel(levelCode);
    }

    /*
    public virtual IEnumerator startLevelCo(string levelCode) {

        yield return StartCoroutine(initLevelCo(levelCode));
        
        yield return StartCoroutine(initLevelFinishCo(levelCode));
    }
    */
         
    public virtual void changeLevelFlash() {
        startLevel("1-2");
    }
    
    // ---------------------------------------------------------------------
    // PROFILE

    public virtual void saveGameStates() {
        GameProfiles.Current.SetCurrentAppMode(AppModes.Current.code);
        GameProfiles.Current.SetCurrentAppModeType(AppModeTypes.Current.code);
        GameProfiles.Current.SetCurrentAppState(AppStates.Current.code);
        GameProfiles.Current.SetCurrentAppContentState(AppContentStates.Current.code);

        LogUtil.Log("saveGameStates:AppModes.Current.code:" + AppModes.Current.code);
        LogUtil.Log("saveGameStates:AppModeTypes.Current.code:" + AppModeTypes.Current.code);
        LogUtil.Log("saveGameStates:AppStates.Current.code:" + AppStates.Current.code);
        LogUtil.Log("saveGameStates:AppContentStates.Current.code:" + AppContentStates.Current.code);

        GameState.SaveProfile();
    }

    public virtual void changeGameStates(string app_content_state) {

        LogUtil.Log("changeGameStates:app_content_state:" + app_content_state);

        //try {
        AppContentStates.Instance.ChangeState(app_content_state);
        //}
        //catch (Exception e) {
        //    LogUtil.Log("ERROR:changeGameState:e:" + e.Message + e.StackTrace);
        //}
        GameController.SaveGameStates();

        LogUtil.Log("changeGameStates:app_content_state:AFTER:" + app_content_state);

    }//AppContentStates.Instance.ChangeState(AppContentStateMeta.appContentStateGameArcade);

    public virtual void changeCharacterModel(string characterCode) {
        GameController.CurrentGamePlayerController.ChangeCharacter(characterCode);
    }

    public virtual void loadCurrentProfileCharacter() {

        string characterProfileCode = GameProfileCharacters.Current.GetCurrentCharacterProfileCode();

        loadProfileCharacter(characterProfileCode);
    }

    public virtual void loadProfileCharacter(string characterProfileCode) {

        GameProfileCharacterItem profileCharacterItem = 
            GameProfileCharacters.Current.GetCharacter(characterProfileCode);

        if (profileCharacterItem == null) {
            return;
        }

        GameProfileCharacters.Current.SetCurrentCharacterProfileCode(characterProfileCode);
               
        string characterCode = ProfileConfigs.defaultGameCharacterCode;
        
        if (!string.IsNullOrEmpty(profileCharacterItem.characterCode)) {
            characterCode = profileCharacterItem.characterCode;
        }
        
        GameController.ChangeCharacterModel(characterCode);
    }
    
    public virtual void loadStartLevel() {
        GameController.LoadStartLevel(GameConfigs.defaultGameLevelCode);
    }
    
    public virtual void loadStartLevel(string levelCode) {
        
        GameDraggableEditor.ChangeStateEditing(GameDraggableEditEnum.StateNotEditing);
        GameController.StartLevel(levelCode);
    }

    public virtual void loadActor(string characterCode, string displayType) {

        GameActorDataItem actorDataItem = new GameActorDataItem();
        actorDataItem.code = characterCode;
        actorDataItem.type = BaseDataObjectKeys.character;
        actorDataItem.data_type = GameSpawnType.zonedType;
        actorDataItem.display_type = displayType;
        
        //for (int i = 0; i < actorDataItem.currentSpawnAmount; i++) {
        GameController.LoadActor(actorDataItem);
        //}
    }

    public virtual void loadActor(string characterCode, string characterType, string spawnType, string displayType, Vector3 pos) {
        
        GameActorDataItem actorDataItem = new GameActorDataItem();
        actorDataItem.code = characterCode;
        actorDataItem.type = characterType;
        actorDataItem.data_type = spawnType;
        actorDataItem.display_type = displayType;
        actorDataItem.position_data.FromVector3(pos);
        
        //for (int i = 0; i < actor.currentSpawnAmount; i++) {
        GameController.LoadActor(actorDataItem);
        //}
    }

    public virtual void loadActor(GameActorDataItem data) {
        StartCoroutine(loadActorCo(data));
    }

    public virtual void loadItem(string itemCode, string itemType, string spawnType, Vector3 pos) {
        
        GameItemData data = new GameItemData();
        data.code = itemCode;
        data.type = itemType;
        data.data_type = spawnType;
        data.position_data.FromVector3(pos);
        
        // DEFAULT ITEM LOADING
        
        //for (int i = 0; i < item.currentSpawnAmount; i++) {
        GameController.LoadItem(data);
        //} 
    }

    public virtual void loadItem(string itemCode) {

        GameItemData data = new GameItemData();
        data.code = itemCode;

        // DEFAULT ITEM LOADING
        
        //for (int i = 0; i < item.currentSpawnAmount; i++) {
        GameController.LoadItem(data);
        //}
    }

    public virtual void loadItem(GameItemData data) {
        StartCoroutine(loadItemCo(data));
    }

    public virtual Vector3 getCurrentPlayerPosition() {
        Vector3 currentPlayerPosition = Vector3.zero;
        if (GameController.CurrentGamePlayerController != null) {
            if (GameController.CurrentGamePlayerController.gameObject != null) {
                currentPlayerPosition = GameController.CurrentGamePlayerController.gameObject.transform.position;
            }
        }
        return currentPlayerPosition;
    }

    public virtual Vector3 getRandomSpawnLocation() {
        Vector3 spawnLocation = Vector3.zero;
        Vector3 currentPlayerPosition = GameController.CurrentPlayerPosition;

        Vector3 boundaryBottomLeftPosition = boundaryEdgeObjectBottomLeft.transform.position;
        Vector3 boundaryBottomRightPosition = boundaryEdgeObjectBottomRight.transform.position;
        Vector3 boundaryTopLeftPosition = boundaryEdgeObjectTopLeft.transform.position;
        Vector3 boundaryTopRightPosition = boundaryEdgeObjectTopRight.transform.position;

        float playerTopLeft = currentPlayerPosition.z - 50f;
        float playerTopRight = currentPlayerPosition.z + 50f;
        float playerBottomLeft = currentPlayerPosition.x - 50f;
        float playerBottomRight = currentPlayerPosition.x + 50f;
    
        //Rect rect = new Rect(0, 0, 150, 150);
        //if (rect.Contains())
        //    print("Inside");
    
        if (playerBottomLeft < boundaryBottomLeftPosition.x) {
            playerBottomLeft = boundaryBottomLeftPosition.x;
        }
        else if (playerBottomRight > boundaryBottomRightPosition.x) {
            playerBottomRight = boundaryBottomRightPosition.x;
        }
        else if (playerTopRight < boundaryTopRightPosition.z) {
            playerTopRight = boundaryTopRightPosition.z;
        }
        else if (playerTopLeft < boundaryTopLeftPosition.z) {
            playerTopLeft = boundaryTopLeftPosition.z;
        }

        spawnLocation.z = UnityEngine.Random.Range(playerTopLeft, playerTopRight);
        spawnLocation.x = UnityEngine.Random.Range(playerBottomLeft, playerBottomRight);
        spawnLocation.y = 0f;

        return spawnLocation;
    }
    
    //bool loadingCharacterContainer = false;

    public virtual IEnumerator loadActorCo(GameActorDataItem data) {

        //if (loadingCharacterContainer) {
        //    yield break; 
        //}

        //loadingCharacterContainer = true;

        GameCharacter gameCharacter = GameCharacters.Instance.GetById(data.code);

        if (gameCharacter == null) {
            //loadingCharacterContainer = false;
            yield break;
        }
        
        string modelPath = ContentPaths.appCacheVersionSharedPrefabCharacters;
        modelPath = PathUtil.Combine(modelPath, "GamePlayerObject");

        // TODO data and pooling and network

        GameObject prefabObject = PrefabsPool.PoolPrefab(modelPath);

        Vector3 spawnLocation = data.position_data.GetVector3();
                
        //Debug.Log("loadActorCo:" + " data.json:" + data.ToJson());
        //Debug.Log("loadActorCo:" + " modelPath:" + modelPath);
        //Debug.Log("loadActorCo:" + " gameCharacter:" + gameCharacter.code);

        if (data.data_type == GameSpawnType.zonedType) {
            // get left/right spawn location
            string leftMiddle = "left-middle";
            string rightMiddle = "right-middle";
            string spawnCode = rightMiddle;
            if (currentGameZone == GameZones.right) {
                spawnCode = rightMiddle;
            }
            else if (currentGameZone == GameZones.left) {
                spawnCode = leftMiddle;
            }

            //LogUtil.Log("spawnCode:" + spawnCode);

            GamePlayerSpawn spawn = GameAIController.GetSpawn(spawnCode);
            if (spawn != null) {
                spawnLocation = spawn.gameObject.transform.position;
            }
            else {

                // get random
                if (currentGameZone == GameZones.right) {
                    spawnLocation = Vector3.zero.WithX(80f).WithZ(GameController.CurrentPlayerPosition.z);// UnityEngine.Random.Range(-20, 20));
                }
                else if (currentGameZone == GameZones.left) {
                    spawnLocation = Vector3.zero.WithX(-80f).WithZ(GameController.CurrentPlayerPosition.z);// UnityEngine.Random.Range(-20, 20));
                }
            }

        }
        else if (data.data_type == GameSpawnType.randomType) {
            // get random
            spawnLocation = GameController.GetRandomSpawnLocation();
        }
        else if (data.data_type == GameSpawnType.explicitType) {
            // get random
            spawnLocation = data.position_data.GetVector3();
        }
        else if (data.data_type == GameSpawnType.pointsType) {
            // FIND spawn location
        }
        else if (data.data_type == GameSpawnType.centeredType) {
            spawnLocation = Vector3.zero;
        }

        if (prefabObject == null) {            
            //loadingCharacterContainer = false;
            yield break;
        }

        GameObject characterObject = GameObjectHelper.CreateGameObject(
            prefabObject, spawnLocation, Quaternion.identity, GameConfigs.usePooledGamePlayers) as GameObject;

        
        //Debug.Log("loadActorCo:" + " characterObject:" + characterObject.name);


        if (characterObject != null) {
    
            characterObject.transform.parent = levelActorsContainerObject.transform;
            
            characterObject.transform.localScale = Vector3.one;
            //characterObject.transform.localPosition = Vector3.zero;
            characterObject.transform.position = spawnLocation;
        
            GamePlayerController characterGamePlayerController
                = characterObject.GetComponentInChildren<GamePlayerController>();

            if (characterGamePlayerController != null) {

                if (data.display_type == GameActorType.player) {
                    characterGamePlayerController.Init(
                        GamePlayerControllerState.ControllerPlayer, GamePlayerContextState.ContextInput);
                    
                    characterGamePlayerController.attackRange = 12f;
                }
                else if (data.display_type == GameActorType.sidekick) {
                    characterGamePlayerController.Init(
                        GamePlayerControllerState.ControllerAgent, GamePlayerContextState.ContextFollowAgent);
                    
                    characterGamePlayerController.attackRange = 12f;
                }
                else { //if (data.display_type == GameActorType.enemy) {                        
                    //characterGamePlayerController.currentTarget = GameController.CurrentGamePlayerController.gameObject.transform;
                    //characterGamePlayerController.ChangeContextState(GamePlayerContextState.ContextFollowAgentAttack);
                    //characterGamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerAgent);
                    characterGamePlayerController.Init(
                        GamePlayerControllerState.ControllerAgent, GamePlayerContextState.ContextFollowAgentAttack);
                    
                    characterGamePlayerController.attackRange = 12f;
                }
                
                characterGamePlayerController.LoadCharacter(data.code);
                
                characterGamePlayerController.transform.localScale
                    = data.scale_data.GetVector3();

            }
        }
        
        //loadingCharacterContainer = false;
    }

    public virtual IEnumerator loadItemCo(GameItemData data) {

        if (data == null) {
            yield break;
        }
        
        GameItem item = GameItems.Instance.GetById(data.code);     
        
        if (item == null) {
            //Debug.Log("loadItemCo:" + "Item not found" + " code:" + data.code);
            yield break;
        }

        string path = Path.Combine(ContentPaths.appCacheVersionSharedPrefabLevelItems, item.data.GetModel().code);
        GameObject prefabObject = PrefabsPool.PoolPrefab(path);
        Vector3 spawnLocation = Vector3.zero;

        //Debug.Log("loadItemCo:" + " data.json:" + data.ToJson());

        if (data.data_type == GameSpawnType.zonedType) {

            // get left/right spawn location
            //string leftMiddle = "left-middle";
            //string rightMiddle = "right-middle";
            //string spawnCode = rightMiddle;
            if (currentGameZone == GameZones.right) {
                //spawnCode = rightMiddle;
            }
            else if (currentGameZone == GameZones.left) {
                //spawnCode = leftMiddle;
            }
            
            // LogUtil.Log("spawnCode:" + spawnCode);
            
            //GamePlayerSpawn spawn = GameAIController.GetSpawn(spawnCode);
            //if(spawn != null) {
            //    spawnLocation = spawn.gameObject.transform.position;
            //}
            //else {
            
            // get random
            if (currentGameZone == GameZones.right) {
                
                spawnLocation = Vector3.zero
                    .WithX(UnityEngine.Random.Range(
                        0, gameBounds.boundaryTopRight.transform.position.x))
                        .WithY(50f)
                        .WithZ(UnityEngine.Random.Range(
                            boundaryBottomLeft.transform.position.z,
                            boundaryTopLeft.transform.position.z));
            }
            else if (currentGameZone == GameZones.left) {
                
                spawnLocation = Vector3.zero
                    .WithX(UnityEngine.Random.Range(
                        gameBounds.boundaryTopLeft.transform.position.x, 0))
                        .WithY(50f)
                        .WithZ(UnityEngine.Random.Range(
                            gameBounds.boundaryBottomLeft.transform.position.z,
                            gameBounds.boundaryTopLeft.transform.position.z));
            }
            //}

        }
        else if (data.data_type == GameSpawnType.explicitType) {
            // get random
            spawnLocation = data.position_data.GetVector3();
        }
        else if (data.data_type == GameSpawnType.pointsType) {
            // FIND spawn location
        }
        else if (data.data_type == GameSpawnType.centeredType) {
            spawnLocation = Vector3.zero;
        }
        else {//if (data.data_type == GameSpawnType.randomType) {
            // get random
            spawnLocation = GameController.GetRandomSpawnLocation();
        }

        if (prefabObject == null) {
            yield break;
        }

        GameObject spawnObj = GameObjectHelper.CreateGameObject(
            prefabObject, spawnLocation, Quaternion.identity, GameConfigs.usePooledItems) as GameObject;

        if (spawnObj != null && levelActorsContainerObject != null) {
            spawnObj.transform.parent = levelActorsContainerObject.transform;
            GamePlayerIndicator.AddIndicator(spawnObj, item.code);
        }
    }


    // ---------------------------------------------------------------------
    // RESETS
    
    public virtual void resetLevelActors() {
        if (levelActorsContainerObject != null) {
            levelActorsContainerObject.DestroyChildren(GameConfigs.usePooledGamePlayers);
        }
    }

    public virtual void resetCurrentGamePlayer() {
        if (currentGamePlayerController != null) {
            currentGamePlayerController.Reset();
        }
    }

    public virtual void reset() {

        GameController.ResetRuntimeData();

        GameController.ResetLevel();
    }

    public virtual void resetLevel() {
        
        GameController.StopDirectors();
                
        GameUIController.HideGameCanvas();

        GameDraggableEditor.ClearLevelItems(levelItemsContainerObject);
        GameDraggableEditor.ResetCurrentGrabbedObject();
        GameDraggableEditor.HideAllEditDialogs();
    }

    public virtual void resetRuntimeData() {

        GameController.ResetCurrentGamePlayer();
        GameController.ResetLevelActors();

        runtimeData = new GameGameRuntimeData();
        runtimeData.ResetTime(defaultLevelTime);
        isGameOver = false;
    }
    /*
    // ---------------------------------------------------------------------
    // GAME MODES
    
    public virtual void changeGameMode(GameModeGlobal gameModeTo) {
        if(gameModeTo != gameMode) {
            gameMode = gameModeTo;
        }
    }
    
    public bool isGameModeArcade {
        get {
            if(gameMode == GameModeGlobal.GameModeArcade) {
                return true;
            }
            return false;
        }
    }       
    
    public bool isGameModeChallenge {
        get {
            if(gameMode == GameModeGlobal.GameModeChallenge) {
                return true;
            }
            return false;
        }
    }           
    
    public bool isGameModeTraining {
        get {
            if(gameMode == GameModeGlobal.GameModeTraining) {
                return true;
            }
            return false;
        }
    }
    */

    // ---------------------------------------------------------------------
    // CHARACTER TYPES
    
    public virtual void loadCharacterTypes() {
        foreach (GameCharacterType type in GameCharacterTypes.Instance.GetAll()) {
            if (!gameCharacterTypes.Contains(type.code)) {
                gameCharacterTypes.Add(type.code);
            }
        }
    }
    
    public virtual void cycleCharacterTypesNext() {
        currentCharacterTypeIndex++;
        GameController.CycleCharacterTypes(currentCharacterTypeIndex);
    }
    
    public virtual void cycleCharacterTypesPrevious() {
        currentCharacterTypeIndex--;
        GameController.CycleCharacterTypes(currentCharacterTypeIndex);
    }
    
    public virtual void cycleCharacterTypes(int updatedIndex) {
    
        if (updatedIndex > gameCharacterTypes.Count - 1) {
            currentCharacterTypeIndex = 0;
        }
        else if (updatedIndex < 0) {
            currentCharacterTypeIndex = gameCharacterTypes.Count - 1;
        }
        else {
            currentCharacterTypeIndex = updatedIndex;
        }
    
        if (currentGamePlayerController != null) {
            currentGamePlayerController.LoadCharacter(gameCharacterTypes[currentCharacterTypeIndex]);
        }
    }


    // ---------------------------------------------------------------------
    // GAME MODES
    
    public virtual void restartGame() {
        GameController.Reset();
        GameController.PrepareGame(GameLevels.Current.code);
    }

    public virtual void startGame(string levelCode) {
        //GameController.Reset();
        GameController.ChangeGameState(GameStateGlobal.GameStarted);
    }
        
    public virtual void stopGame() {
        GameController.ProcessLevelStats();
        GameController.StopDirectors();
    }

    public virtual void prepareGame(string levelCode) {

        //GameController.Reset();

        Debug.Log("prepareGame:" + " levelCode:" + levelCode);
        
        GameController.LoadLevelAssets(levelCode);
        
        GameHUD.Instance.SetLevelInit(GameLevels.Current);
        
        GameHUD.Instance.AnimateIn();

        GameController.ChangeGameState(GameStateGlobal.GamePrepare);
    }

    public virtual void gameContentDisplay(string contentDisplayCodeTo) {
        contentDisplayCode = contentDisplayCodeTo;
        GameController.ChangeGameState(GameStateGlobal.GameContentDisplay);
    }

    public virtual void pauseGame() {
        GameController.ChangeGameState(GameStateGlobal.GamePause);
    }

    public virtual void resumeGame() {
        GameController.ChangeGameState(GameStateGlobal.GameResume);
    }

    public virtual void quitGame() {
        GameController.Reset();
        GameController.ChangeGameState(GameStateGlobal.GameQuit);
    }

    public virtual void resultsGameDelayed() {
        Invoke("resultsGame", .5f);
    }

    public virtual void resultsGame() {
        GameController.ChangeGameState(GameStateGlobal.GameResults);
    }

    public virtual void togglePauseGame() {
        if (gameState == GameStateGlobal.GamePause) {
            GameController.ResumeGame();
        }
        else {
            GameController.PauseGame();
        }
    }

    // -------------------------------------------------------
    // DIRECTORS
    
    public virtual void runDirectorsDelayed(float delay) {
        StartCoroutine(runDirectorsDelayedCo(delay));
    }

    public virtual IEnumerator runDirectorsDelayedCo(float delay) {
        yield return new WaitForSeconds(delay);
        GameController.RunDirectors();
    }
    
    public virtual void runDirectors() {
        GameController.UpdateDirectors(true);
    }
    
    public virtual void stopDirectors() {
        GameController.UpdateDirectors(false);
    }

    public virtual void updateDirectors(bool run) {
        GameController.UpdateDirectorAI(run);
        GameController.UpdateDirectorItem(run);
    }

    public virtual void updateDirectorAI(bool run) {
        GameAIController.Run(run);
    }

    public virtual void updateDirectorItem(bool run) {
        GameItemController.Run(run);
    }

    // -------------------------------------------------------
    // GAME STATES / HANDLERS

    public virtual void gameSetTimeScale(float timeScale) {
        Time.timeScale = timeScale;
    }

    // STOPPED
    
    public virtual void gameRunningStateStopped() {
        gameRunningStateStopped(1f);
    }

    public virtual void gameRunningStateStopped(float timeScale) {
        gameSetTimeScale(timeScale);
        gameRunningState = GameRunningState.STOPPED;
        GameController.Instance.gameState = GameStateGlobal.GameNotStarted;
    }

    // PAUSED

    public virtual void gameRunningStatePause() {
        gameRunningStatePause(0f);
    }

    public virtual void gameRunningStatePause(float timeScale) {
        gameSetTimeScale(timeScale);
        gameRunningState = GameRunningState.PAUSED;
        GameController.Instance.gameState = GameStateGlobal.GamePause;
    }

    // RUN
    
    public virtual void gameRunningStateRun() {
        gameRunningStateRun(1f);
    }
    
    public virtual void gameRunningStateRun(float timeScale) {
        gameSetTimeScale(timeScale);
        gameRunningState = GameRunningState.RUNNING;
        GameController.Instance.gameState = GameStateGlobal.GameStarted;
    }

    // CONTENT

    public virtual void gameRunningStateContent() {
        gameRunningStateContent(1f);
    }

    public virtual void gameRunningStateContent(float timeScale) {
        gameRunningState = GameRunningState.PAUSED;
        GameController.Instance.gameState = GameStateGlobal.GameContentDisplay;
        gameSetTimeScale(timeScale);
    }

    // OVERLAY
    
    public virtual void gameRunningStateOverlay() {
        gameRunningStateOverlay(1f);
    }
    
    public virtual void gameRunningStateOverlay(float timeScale) {
        gameRunningState = GameRunningState.PAUSED;
        GameController.Instance.gameState = GameStateGlobal.GameOverlay;
        gameSetTimeScale(timeScale);
    }

    // EVENTS ON
        
    public virtual void onGameOverlay() {
        handleOverlay();
    }

    //

    public virtual void handleOverlay() {
        // handled externally
    }

    public virtual void onGameContentDisplay() {
        // Show runtime content display data
        //GameRunningStatePause();
    
        if (contentDisplayCode == GameContentDisplayTypes.gamePlayerOutOfBounds) {
    
            GameController.GamePlayerOutOfBoundsDelayed(3f);
    
            UIPanelDialogBackground.ShowDefault();
            UIPanelDialogDisplay.SetTitle("OUT OF BOUNDS");
            //UIPanelDialogDisplay.SetDescription("RUN, BUT STAY IN BOUNDS...");
            UIPanelDialogDisplay.ShowDefault();
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameEnergy) {
            handleContentDialogEnergy();
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameHealth) {
            handleContentDialogHealth();
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameXP) {
            handleContentDialogXP();
        }
        else {
        
            //AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz
            UIPanelDialogBackground.HideAll();
        }
    
        //GameRunningStateRun();
    }

    public virtual void handleContentTutorial() {
        
        if (contentDisplayCode == GameContentDisplayTypes.gameTutorial) {            
            //UIPanelDialogBackground.ShowDefault();
        }
    }
    
    public virtual void handleContentTips() {
        
        if (contentDisplayCode == GameContentDisplayTypes.gameTips) {            
            //UIPanelDialogBackground.ShowDefault();
        }
    }
    
    public virtual void handleContentOverview() {
        
        if (contentDisplayCode == GameContentDisplayTypes.gameModeContentOverview) {
            UIPanelOverviewMode.ShowDefault();
        }
    }

    public virtual void handleContentDialogEnergy() {
        //UIPanelDialogRPGHealth.HideAll();
        //UIPanelDialogRPGXP.HideAll();
        UIPanelDialogRPGEnergy.ShowDefault();
    }

    public virtual void handleContentDialogHealth() {
        //UIPanelDialogRPGEnergy.HideAll();
        //UIPanelDialogRPGXP.HideAll();
        UIPanelDialogRPGHealth.ShowDefault();
    }

    public virtual void handleContentDialogXP() {
        //UIPanelDialogRPGEnergy.HideAll();
        //UIPanelDialogRPGHealth.HideAll();
        UIPanelDialogRPGXP.ShowDefault();
    }

    public virtual void onGameContentDisplayPause() {
        GameDraggableEditor.HideAllEditDialogs();
        UIPanelDialogBackground.ShowDefault();
        GameController.GameRunningStatePause();
    }
    
    public virtual void onGameContentDisplayResume() {
        GameDraggableEditor.HideAllEditDialogs();
        UIPanelDialogBackground.HideAll();
        GameController.GameRunningStateRun();
    }
    
    public virtual void onGameOverlayPause() {
        GameDraggableEditor.HideAllEditDialogs();
        //UIPanelDialogBackground.ShowDefault();
        GameController.GameRunningStatePause();
    }
    
    public virtual void onGameOverlayResume() {
        GameDraggableEditor.HideAllEditDialogs();
        //UIPanelDialogBackground.HideAll();
        GameController.GameRunningStateRun();
    }

    public virtual void onGamePrepare(bool startGame) {

        //LogUtil.Log("GAME START FLOW: STEP #7: onGamePrepare: startGame:" + startGame);

        //GameController.ResetRuntimeData();

        //GameUIController.HideUI(true);
        GameUIController.HideHUD();

        if (allowedEditing) {
            GameDraggableEditor.ShowUIPanelEditButton();
        }

        if (startGame) {
            GameController.StartGame(GameLevels.Current.code);
        }
    }
    
    public virtual void onGameStarted() {

        //LogUtil.Log("GAME START FLOW: STEP #8: onGameStarted");

        GameController.StartLevelStats();

        GameUIController.HideUI(true);
        GameUIController.ShowHUD();

        if (allowedEditing) {
            GameDraggableEditor.ShowUIPanelEditButton();
        }
    
        GameController.GameRunningStateRun();

        GameUIController.ShowGameCanvas();
        
        AnalyticsNetworks.LogEventLevelStart(GameLevels.Current.code, GameLevels.Current.display_name);
    
        GameController.RunDirectorsDelayed(runDirectorsDelay);
    }

    public virtual void onGameQuit() {
    
        // Cleanup
        GameUIController.HideHUD();
        GameDraggableEditor.HideAllEditDialogs();
        GameDraggableEditor.HideAllUIEditPanels();
        
        AnalyticsNetworks.LogEventLevelQuit(GameLevels.Current.code, GameLevels.Current.display_name);

        Messenger<string>.Broadcast(GameMessages.gameLevelQuit, GameLevels.Current.code);
    
        // Back
        GameUIController.ShowUI();
    
        //ChangeGameState(GameStateGlobal.GameResults);

        GameController.QuitGameRunning();
    }

    public virtual void quitGameRunning() {
        GameController.Reset();
        GameController.StopDirectors();
        GameController.GameRunningStateStopped();
        Time.timeScale = 1f;
    }
    
    public virtual void onGamePause() {
        
        Messenger<string>.Broadcast(GameMessages.gameLevelPause, GameLevels.Current.code);

        // Show pause, resume, quit menu
        GameUIController.ShowUIPanelPause();
        UIPanelDialogBackground.ShowDefault();
        GameController.GameRunningStatePause();
    }
    
    public virtual void onGameResume() {
        GameDraggableEditor.HideAllEditDialogs();
        GameUIController.ShowHUD();
        GameUIController.HideUIPanelPause();
        UIPanelDialogBackground.HideAll();
        GameController.GameRunningStateRun();
        
        Messenger<string>.Broadcast(GameMessages.gameLevelResume, GameLevels.Current.code);
    }
    
    public virtual void onGameNotStarted() {
        //
    }
    
    public virtual void onGameResults() {

        //LogUtil.Log("OnGameResults");
    
        //if(runtimeData.localPlayerWin){
        //GameUIPanelResults.Instance.ShowSuccess();
        //GameUIPanelResults.Instance.HideFailed();
        //}
        //else {
        //GameUIPanelResults.Instance.HideSuccess();
        //GameUIPanelResults.Instance.ShowFailed();
        //}
    
        GameUIPanelOverlays.Instance.ShowOverlayWhiteStatic();
    
        GameController.StopGame();
        
        AnalyticsNetworks.LogEventLevelResults(GameLevels.Current.code, GameLevels.Current.display_name);
    }
    
    public virtual void changeGameState(GameStateGlobal gameStateTo) {
        gameState = gameStateTo;
    
        Messenger<GameStateGlobal>.Broadcast(GameMessages.gameActionState, gameState);
    
        if (gameState == GameStateGlobal.GameStarted) {
            GameController.OnGameStarted();
        }
        else if (gameState == GameStateGlobal.GamePause) {
            GameController.OnGamePause();
        }
        else if (gameState == GameStateGlobal.GameResume) {
            GameController.OnGameResume();
        }
        else if (gameState == GameStateGlobal.GameQuit) {
            GameController.OnGameQuit();
        }
        else if (gameState == GameStateGlobal.GameNotStarted) {
            GameController.OnGameNotStarted();
        }
        else if (gameState == GameStateGlobal.GameResults) {
            GameController.OnGameResults();
        }
        else if (gameState == GameStateGlobal.GameContentDisplay) {
            GameController.OnGameContentDisplay();
        }
        else if (gameState == GameStateGlobal.GameOverlay) {
            GameController.OnGameOverlay();
        }
        else if (gameState == GameStateGlobal.GamePrepare) {
            GameController.OnGamePrepare();
        }
    }

    public bool isGameRunning {
        get {
            if (gameState == GameStateGlobal.GameStarted) {
                return true;
            }
            return false;
        }
    }
        
    // -------------------------------------------------------
    
    // GAME CAMERA
    
    public virtual void initCameras() {
        
        if (camerasAlwaysOn == null) {
            camerasAlwaysOn = new List<Camera>();
            
            if (cameraContainersAlwaysOn != null) {
                foreach (Camera cam 
                         in cameraContainersAlwaysOn.GetComponentsInChildren<Camera>()) {
                    if (!camerasAlwaysOn.Contains(cam)) {
                        camerasAlwaysOn.Add(cam);
                    }
                }
            }
        }
        
        if (camerasGame == null) {
            camerasGame = new List<Camera>();
            if (camerasContainerGame != null) {
                foreach (Camera cam 
                         in camerasContainerGame.GetComponentsInChildren<Camera>()) {
                    if (!camerasGame.Contains(cam)) {
                        camerasGame.Add(cam);
                    }
                }
            }
        }
        
        if (camerasUI == null) {
            camerasUI = new List<Camera>();
            if (cameraContainersUI != null) {
                foreach (Camera cam 
                         in cameraContainersUI.GetComponentsInChildren<Camera>()) {
                    if (!camerasUI.Contains(cam)) {
                        camerasUI.Add(cam);
                    }
                }
            }
        }
    }
    
    public virtual void showCameras(List<Camera> cams) {
        StartCoroutine(showCamerasCo(cams));
    }
    
    public virtual void hideCameras(List<Camera> cams) {
        StartCoroutine(hideCamerasCo(cams));
    }
    
    public virtual IEnumerator showCamerasCo(List<Camera> cams) {
        
        foreach (Camera cam in cams) { 
            if (cam != null) {
                if (cam.gameObject != null) {
                    cam.gameObject.Show();
                    UITweenerUtil.FadeTo(cam.gameObject, UITweener.Method.EaseIn, UITweener.Style.Once, .5f, .5f, 1f);
                }
            }
        }
        yield return new WaitForSeconds(2f);       
    }
    
    public virtual IEnumerator hideCamerasCo(List<Camera> cams) {
        foreach (Camera cam in cams) {
            if (cam != null) {
                if (cam.gameObject != null) {
                    UITweenerUtil.FadeTo(cam.gameObject, UITweener.Method.EaseIn, UITweener.Style.Once, .5f, .5f, 0f);
                }
            }
        }
        yield return new WaitForSeconds(2f);
        
        foreach (Camera cam in cams) {  
            if (cam != null) {
                if (cam.gameObject != null) {
                    cam.gameObject.Hide();
                }
            }
        }
    }
    
    public virtual void handleCamerasInGame() {
        GameController.ShowCameras(camerasGame);
        GameController.HideCameras(camerasUI);
    }
    
    public virtual void handleCamerasInUI() {
        GameController.HideCameras(camerasGame);
        GameController.ShowCameras(camerasUI);
    }
    
    public virtual void changeGameCameraMode(GameCameraView cameraViewTo) {
        if (cameraViewTo == cameraView) {
            return;
        }
        else {
            cameraView = cameraViewTo;
        }
        
        LogUtil.Log("ChangeGameCameraMode:cameraViewTo: " + cameraViewTo);
        
        if (cameraGame != null
            && cameraGameGround != null) {
            
            if (cameraView == GameCameraView.ViewSide) {
                
                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(30);
                
                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);
                
                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if (cameraView == GameCameraView.ViewSideTop) {
                
                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(80);
                
                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);
                
                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if (cameraView == GameCameraView.ViewBackTilt) {
                
                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(45).WithY(90);
                
                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);
                
                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if (cameraView == GameCameraView.ViewBackTop) {
                
                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(80).WithY(90);
                
                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);
                
                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
        }
    }
    
    public virtual void changeGameCameraProperties(
        GameObject cameraObject, Vector3 positionTo, Vector3 rotationTo, float timeDelay) {
        //cameraObject.transform.rotation = Quaternion.Euler(rotationTo);
        //iTween.RotateTo(cameraObject, rotationTo, timeDelay);
        UITweenerUtil.RotateTo(cameraObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, timeDelay, rotationTo);
    }
    
    public virtual void cycleGameCameraMode() {
        
        LogUtil.Log("CycleGameCameraMode: " + cameraView);
        
        if (cameraView == GameCameraView.ViewSide) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewSideTop);
        }
        else if (cameraView == GameCameraView.ViewSideTop) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewBackTilt);
        }
        else if (cameraView == GameCameraView.ViewBackTilt) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewBackTop);
        }
        else if (cameraView == GameCameraView.ViewBackTop) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewSide);
        }
    }

    // -------------------------------------------------------

    // GAME PLAYER BOUNDS
    
    public virtual void gamePlayerOutOfBounds() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (AppModes.Instance.isAppModeGameTraining) {
            return;
        }

        GameAudioController.PlaySoundPlayerOutOfBounds();
        GameController.GameContentDisplay(GameContentDisplayTypes.gamePlayerOutOfBounds);
    }

    public virtual void gamePlayerOutOfBoundsDelayed(float delay) {
        StartCoroutine(gamePlayerOutOfBoundsDelayedCo(delay));
    }

    public virtual IEnumerator gamePlayerOutOfBoundsDelayedCo(float delay) {

        yield return new WaitForSeconds(delay);

        //LogUtil.Log("GamePlayerOutOfBoundsDelayed:");

        runtimeData.outOfBounds = true;

        //LogUtil.Log("GamePlayerOutOfBoundsDelayed:runtimeData.outOfBounds:" + runtimeData.outOfBounds);

        gameState = GameStateGlobal.GameStarted;

        //LogUtil.Log("GamePlayerOutOfBoundsDelayed:gameState:" + gameState);

        GameController.CheckForGameOver();
    }

    // -------------------------------------------------------

    // GAME PLAYER GOAL ZONE

    public virtual void goalZoneChange() {
        if (currentGameZone == GameZones.left) {
            GameController.GoalZoneChange(GameZones.right);
        }
        else if (currentGameZone == GameZones.right) {
            GameController.GoalZoneChange(GameZones.left);
        }
    }

    public virtual void goalZoneChange(GameZones goalZone) {
        //if(currentGameZone == goalZone) {
        //    return;
        //}

        if (goalZone == GameZones.left) {
            currentGameZone = goalZone;
        }
        else if (goalZone == GameZones.right) {
            currentGameZone = goalZone;
        }

        GameController.HandleGoalZoneChange();
    }

    // -------------------------------------------------------

    // HANDLE GOAL ZONE CHANGE

    public virtual void handleGoalZoneChange() {
        if (currentGameZone == GameZones.left) {
            // move goal markers
        }
        else if (currentGameZone == GameZones.right) {
            // move goal markers
        }
    }
    // -------------------------------------------------------

    // GAME PLAYER GOAL ZONE COUNTDOWN

    public virtual void gamePlayerGoalZoneCountdown(GameObject goalObject) {

    }

    // -------------------------------------------------------

    // GAME PLAYER GOAL ZONE

    public virtual void gamePlayerGoalZone(GameObject goalObject) {

    }

    public virtual void gamePlayerGoalZoneDelayed(GameObject goalObject, float delay) {
        StartCoroutine(gamePlayerGoalZoneDelayedCo(goalObject, delay));
    }

    public virtual IEnumerator gamePlayerGoalZoneDelayedCo(GameObject goalObject, float delay) {

        yield return new WaitForSeconds(delay);

        //LogUtil.Log("gamePlayerGoalZoneDelayedCo:");

        GameController.GoalZoneChange();
    }

    // GAME RUNTIME DATA
    
    public virtual void gameRuntimeTimeExtend(double extendAmount) {
        if (runtimeData != null) {
            runtimeData.AppendTime(extendAmount);
        }
    }

    // -------------------------------------------------------

    // STATS HANDLING

    public virtual void processLevelStats() {
        StartCoroutine(processLevelStatsCo());
    }

    public virtual IEnumerator processLevelStatsCo() {

        // END OF LEVEL PROCESSING WHILE SETTING UP RESULTS
         
        yield return new WaitForSeconds(.5f);

        // TOTAL SCORES

        GamePlayerProgress.SetStatScore(currentGamePlayerController.runtimeData.score);
        GamePlayerProgress.SetStatXP(currentGamePlayerController.runtimeData.totalScoreValue);

        yield return new WaitForEndOfFrame();

        // HIGH SCORES

        GamePlayerProgress.SetStatHighScore(currentGamePlayerController.runtimeData.score);
        GamePlayerProgress.SetStatHighScores(currentGamePlayerController.runtimeData.scores);
        GamePlayerProgress.SetStatHighXP(currentGamePlayerController.runtimeData.totalScoreValue);

        yield return new WaitForEndOfFrame();

        // WINS / LOSSES

        if (runtimeData.localPlayerWin) {
            GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.wins, 1f);
        }
        else {
            GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.losses, 1f);
        }

        // HANDLE RPG

        GameProfileRPGs.Current.AddCurrency(currentGamePlayerController.runtimeData.coins);

        // TODO by skill/RPG

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(
            currentGamePlayerController.runtimeData.totalScoreValue);

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealth(
            -UnityEngine.Random.Range(.01f, .05f),
            -UnityEngine.Random.Range(.01f, .05f));

        GameUIPanelResults.Instance.UpdateDisplay(currentGamePlayerController.runtimeData, 0f);

        yield return new WaitForEndOfFrame();

        // STOP ALL STAT COUNTS / TIMERS
             
        GameController.EndLevelStats();
    
        yield return new WaitForEndOfFrame();
    
        GamePlayerProgress.Instance.ProcessProgressRuntimeAchievements();
        
        yield return new WaitForEndOfFrame();

        // PROCESS GAME TYPE SPECIFIC STATS MODE DATA
        // COLLECTION

        GameController.ProcessProgressCollections(
            runtimeData, currentGamePlayerController.runtimeData);
                
        yield return new WaitForEndOfFrame();
    
        if (!isAdvancing) {
            GameController.AdvanceToResults();
        }
        
        GameState.SyncProfile();

        yield return new WaitForEndOfFrame();
        
        GamePlayerProgress.Instance.ProcessProgressLeaderboards();

        //GC.Collect();
        //GC.WaitForPendingFinalizers();
        //yield return new WaitForSeconds(8f);
    }

    public static void ProcessProgressCollections(
        GameGameRuntimeData runtimeData, GamePlayerRuntimeData playerRuntimeData) {
        if(GameController.isInst) {
            GameController.Instance.processProgressCollections(runtimeData, playerRuntimeData);
        }
    }

    public virtual void processProgressCollections(
        GameGameRuntimeData gameRuntimeData, GamePlayerRuntimeData playerRuntimeData) {
        // PROCESS GAME TYPE SPECIFIC STATS MODE DATA
        // COLLECTION
        
        if (AppModes.Instance.isAppModeGameArcade
            || AppModes.Instance.isAppModeGameChallenge
            || AppModes.Instance.isAppModeGameMission) {  

            double collectScore = AppContentCollects.Current.ScoreCompleted(
                BaseDataObjectKeys.mission, gameRuntimeData, playerRuntimeData);
                        
            // CHECK ON ADDING BY ALREADY COMPLETED...
            GameProfileRPGs.Current.AddCurrency(collectScore);
        }
    }
    
    public virtual void advanceToResults() {
        isAdvancing = true;
        Invoke("advanceToResultsDelayed", .5f);
    }
    
    public virtual void advanceToResultsDelayed() {
        GameUIController.ShowUIPanelDialogResults();
        //GameAudio.StartGameLoop(4);
        isAdvancing = false;
        GameController.GameRunningStateStopped();

        broadcastResultsDelayed();
    }

    public virtual void broadcastResultsDelayed(float delay) {
        Invoke("broadcastResultsDelayed", .5f);    
    }

    public virtual void broadcastResultsDelayed() {
        Messenger<string>.Broadcast(GameMessages.gameLevelEnd, GameLevels.Current.code);
    }
    
    //public virtual void ProcessStatShot() {
    //    GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.shots, 1f);
    //}

    //public virtual void ProcessStatDestroy() {
    //    GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.destroyed, 1f);
    //}
    
    public virtual void startLevelStats() {
        GamePlayerProgress.Instance.ProcessProgressPack("default");
        GamePlayerProgress.Instance.ProcessProgressAction(GameLevels.Current.code);
        GamePlayerProgress.Instance.ProcessProgressAction(AppModes.Current.code);
    }
    
    public virtual void endLevelStats() {
        GamePlayerProgress.Instance.EndProcessProgressPack("default");
        GamePlayerProgress.Instance.EndProcessProgressAction(GameLevels.Current.code);
        GamePlayerProgress.Instance.EndProcessProgressAction(AppModes.Current.code);
    }


    // -------------------------------------------------------
    
    // GAME SCORE/CHECK GAME OVER

    public virtual void checkForGameOver() {
    
        //LogUtil.Log("CheckForGameOver:isGameOver:" + isGameOver);
        //LogUtil.Log("CheckForGameOver:isGameRunning:" + isGameRunning);

        if (isGameRunning) {
        
            // Check player health/status
            // Go to results if health gone
            
            // if time expired on current round advance to next.
            
            //LogUtil.Log("runtimeData:" + runtimeData.currentLevelTime);
            
            // Check for out of ammo
            // Check if user destroyed or completed mission
            // destroyed all destructable objects
            //if(runtimeData.ammo == 0
            //  || IsLevelDestructableItemsComplete()) {
            
            // TODO HANDLE MODES
            
            if (!isGameOver) {
            
                bool gameOverMode = false;
    
                if (AppModes.Instance.isAppModeGameArcade) {
                    if (currentGamePlayerController.runtimeData.health <= 0f
                        || runtimeData.timeExpired) { 
                        gameOverMode = true;
                    }
                }
                else if (AppModes.Instance.isAppModeGameChallenge) {
                    if (currentGamePlayerController.runtimeData.health <= 0f
                        || runtimeData.timeExpired) {
                        gameOverMode = true;
                    }
                }
                else if (AppModes.Instance.isAppModeGameMission) {
                    if (currentGamePlayerController.runtimeData.health <= 0f
                        || runtimeData.timeExpired) {
                        gameOverMode = true;

                    }
                }
                else if (AppModes.Instance.isAppModeGameTraining) {

                    // TODO other modes

                    if (runtimeData.timeExpired) {
                        //gameOverMode = true;
                    }
                }
    
                if (runtimeData.outOfBounds) {
                    //LogUtil.Log("CheckForGameOver:runtimeData.outOfBounds:" + runtimeData.outOfBounds);
                    gameOverMode = true;
                    //LogUtil.Log("CheckForGameOver:gameOverMode:" + gameOverMode);
                }
    
                if (gameOverMode) {
                    isGameOver = true;
                    GameController.ResultsGameDelayed();
                }
    
                runtimeData.SubtractTime(Time.deltaTime);
    
                //if(runtimeData.timeExpired) {
                // Change level/flash
                //ChangeLevelFlash();
                //}
            }
        }
    }

    // -------------------------------------------------------

    // HANDLE TOUCH MOVEMENT - TODO MOVE TO UI CONTROLLER

    public virtual bool handleTouchInputPoint(Vector3 point) {
        //&& currentGamePlayerController.thirdPersonController.aimingDirection != Vector3.zero) {

        bool handled = false;

        if (!isGameRunning) {
            return handled;
        }

        if (!GameUIController.CheckIfAllowedTouch(point)) {
            return handled;
        }

        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();

        if (currentGamePlayerController != null) {
            var playerPos = currentGamePlayerController.transform.position;
            var touchPos = Camera.main.ScreenToWorldPoint(point);
    
            var direction = touchPos - playerPos;
            direction.Normalize();
            var directionNormal = direction.normalized;
    
            //touchPos.Normalize();
            //var touchPosNormalized = touchPos.normalized;
    
            var pointNormalized = point;
            pointNormalized.Normalize();
            pointNormalized = pointNormalized.normalized;

            //LogUtil.Log("directionNormal:" + directionNormal);
            //LogUtil.Log("controlInputTouchOnScreen:" + controlInputTouchOnScreen);
    
            updateFingerNavigate = true;
    
            if (controlInputTouchOnScreen) {
                // If on screen controls are on don't do touch navigate just off the edge of the
                /// backer on the virtual joystick to prevent random movements.
    
                var center = Vector3.zero;//.WithX(Screen.width/2).WithY(Screen.height/2);
    
                var directionAllow = touchPos - center;
                directionAllow.Normalize();
                //var directionAllowNormal = directionAllow.normalized;
    
                //LogUtil.Log("directionAllowNormal:" + directionAllowNormal);
                //LogUtil.Log("touchPos:" + touchPos);
                //LogUtil.Log("pointNormalized:" + pointNormalized);
                //LogUtil.Log("point:" + point);
    
                if (pointNormalized.y < .2f) {
                    if (pointNormalized.x < .2f) {
                        updateFingerNavigate = false;
                    }
    
                    if (pointNormalized.x > .8f) {
                        updateFingerNavigate = false;
                    }
                }

                //LogUtil.Log("updateFingerNavigate:" + updateFingerNavigate);
            }
        
            if (updateFingerNavigate) {

                handled = true;

                //LogUtil.Log("updateFingerNavigate::directionNormal.y" + directionNormal.y);
                //LogUtil.Log("updateFingerNavigate::directionNormal.x" + directionNormal.x);

                Vector3 axisInput = Vector3.zero;
                axisInput.x = directionNormal.x;
                axisInput.y = directionNormal.y;

                GameController.SendInputAxisMessage("move", axisInput);
            }
        }

        return handled;
    }

    public virtual void sendInputAxisMessage(string axisNameTo, Vector3 axisInputTo) {
        Messenger<string, Vector3>.Broadcast(
            GameTouchInputMessages.inputAxis,
            GameTouchInputMessages.inputAxis + "-" + axisNameTo, axisInputTo
        );
    }

    // -------------------------------------------------------
    
    // LEVEL ITEMS, DATA, RANDOMIZER

    public virtual string getGameLevelGridKey(Vector3 gridPos) {
        string key = "pos" +
            "-" + ((int)gridPos.x).ToString() +
            "-" + ((int)gridPos.y).ToString() +
            "-" + ((int)gridPos.z).ToString();
        //LogUtil.Log("gameLevelKey:" + key);
        return key;
    }

    public virtual bool isGameLevelGridSpaceFilled(Vector3 gridPos) {

        bool filled = false;

        GameController.CheckGameLevelGrid();

        string key = GameController.GetGameLevelGridKey(gridPos);

        if (levelGrid.ContainsKey(key)) {
            filled = true;
        }

        LogUtil.Log("gameLevelSpaceFilled: key:" + key + " filled:" + filled);

        return filled;
    }

    public virtual void setGameLevelGridSpaceFilled(Vector3 gridPos, GameLevelItemAsset asset) {

        GameController.CheckGameLevelGrid();

        string key = GameController.GetGameLevelGridKey(gridPos);

        if (levelGrid.ContainsKey(key)) {
            levelGrid[key] = asset;
            LogUtil.Log("SetLevelSpaceFilled: key:" + key);
        }
        else {
            levelGrid.Add(key, asset);
            LogUtil.Log("SetLevelSpaceFilled: key:" + key);
        }
    }

    public virtual void checkGameLevelItems() {
        if (levelItems == null) {
            levelItems = new List<GameLevelItemAsset>();
        }
    }

    public virtual void checkGameLevelGrid() {
        if (levelGrid == null) {
            levelGrid = new Dictionary<string, GameLevelItemAsset>();
        }
    }

    public virtual void clearGameLevelItems() {

        Debug.Log("clearGameLevelItems:");

        GameController.CheckGameLevelItems();

        if (levelItems != null) {
            levelItems.Clear();
        }
    }

    public virtual void clearGameLevelGrid() {

        Debug.Log("clearGameLevelGrid:");

        GameController.CheckGameLevelGrid();

        if (levelGrid != null) {
            levelGrid.Clear();
        }
    }

    public virtual List<GameLevelItemAsset> getLevelRandomized() {

        List<GameLevelItemAsset> levelItems = new List<GameLevelItemAsset>();

        return GameController.GetLevelRandomized(levelItems);
    }

    public virtual Vector3 getRandomVectorInGameBounds() {
        return Vector3.zero
            .WithX(UnityEngine.Random.Range(
                gameBounds.boundaryTopLeft.transform.position.x,
                gameBounds.boundaryTopRight.transform.position.x))
            .WithY(UnityEngine.Random.Range(.1f, gameBounds.boundaryTopCeiling.transform.position.y / 4));
    }

    public virtual List<GameLevelItemAsset> getLevelRandomizedGrid() {
        return GameController.GetLevelRandomizedGrid(GameLevelGridData.GetDefault());
    }
    
    public virtual List<GameLevelItemAsset> getLevelRandomizedGrid(GameLevelGridData gameLevelGridData) {

        GameController.ClearGameLevelItems();
        GameController.ClearGameLevelGrid();

        /*
            if(randomChance <= 8) {
    
                // Fill level item
    
                for(int a = 0; a < UnityEngine.Random.Range(1, 5); a++) {
    
                    Vector3 gridPos = Vector3.zero
                     .WithX(((gridBoxSize * x)))
                     .WithY(((gridBoxSize * y)) + 1)
                     .WithZ(((gridBoxSize * z) + gridBoxSize + (a * 5)) - gridWidth / 2);
    
                    GameLevelItemAsset asset = GameController.GetLevelItemAssetFull(gridPos, "padding-1", 5,
                        GameLevelItemAssetPhysicsType.physicsOnStart, true, false, false, false,
                        Vector2.zero.WithX(1f).WithY(1f),
                        Vector2.zero.WithX(0f).WithY(0f));
    
                    levelItems.Add(asset);
                }
            }
        */

        return levelItems;
    }

    public virtual List<GameLevelItemAsset> getLevelRandomizedGridAssets(GameLevelGridData gameLevelGridData) {

        GameController.ClearGameLevelItems();
        GameController.ClearGameLevelGrid();

        return levelItems;
    }
    
    public virtual GameLevelItemAsset getLevelItemAsset(
        GameLevelItemAssetData data) {

        return GameController.GetLevelItemAssetRandom(data);
    }
    
    public virtual GameLevelItemAsset getLevelItemAssetRandom(
        GameLevelItemAssetData data) {

        data.position_data = new Vector3Data(GameController.GetRandomVectorInGameBounds());

        return GameController.GetLevelItemAssetFull(data);
    }

    public virtual void syncLevelItem(Vector3 gridPos, GameLevelItemAssetData data) {
        if (!GameController.IsGameLevelGridSpaceFilled(gridPos)) {
            //&& GameController.CheckBounds(gridPos)) {

            GameLevelItemAsset asset = GameController.GetLevelItemAssetFull(data);

            asset.local_position_data = new Vector3Data();
            asset.local_position_data = data.local_position_data;
            
            asset.local_rotation_data = new Vector3Data();
            asset.local_rotation_data = data.local_rotation_data;
            
            asset.scale_data = new Vector3Data();
            asset.scale_data = data.scale_data;

            levelItems.Add(asset);

            GameController.SetGameLevelGridSpaceFilled(gridPos, asset);
        }
    }
    
    public virtual GameLevelItemAsset getLevelItemAssetFull(
        GameLevelItemAssetData data) {
        
        GameLevelItemAssetStep step = new GameLevelItemAssetStep();
        step.position_data = data.position_data;

        step.scale_data = data.scale_data;
        //step.scale.FromVector3(
        //    Vector3.one *
        //    UnityEngine.Random.Range(data.range_scale.x, data.range_scale.y));

        //rangeScale, 1.2f));

        step.rotation_data = data.rotation_data;//
        //Vector3.zero
        //.WithX(data.range_rotation.x)
        //.WithY(data.range_rotation.y)
        //.WithZ(data.range_rotation.z)
        //);//
        //UnityEngine.Random.Range(data.range_rotation.x, data.range_rotation.y)));

        //step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));

        GameLevelItemAsset asset = new GameLevelItemAsset();

        if (data.limit == 0) {
            asset.code = data.code;
        }
        else {
            asset.code = data.code + "-" + UnityEngine.Random.Range(1, (int)data.limit).ToString();
        }

        asset.type = data.type;
        asset.data_type = data.data_type;
        asset.display_type = data.display_type;

        asset.physics_type = data.physics_type;
        asset.destructable = data.destructable;
        asset.reactive = data.reactive;
        asset.kinematic = data.kinematic;
        asset.gravity = data.gravity;
        asset.steps.Add(step);

        return asset;
    }
        
    public virtual List<GameLevelItemAsset> getLevelRandomized(List<GameLevelItemAsset> levelItems) {
        
        for (int i = 0; i < UnityEngine.Random.Range(3, 9); i++) {

            GameLevelItemAssetData data = new GameLevelItemAssetData();
            data.code = "portal";
            data.limit = 5;
            data.destructable = true;
            data.gravity = true;
            data.kinematic = true;
            data.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            //data.range_rotation = Vector2.zero.WithX(.7f).WithY(1.2f);
            //data.range_rotation = Vector2.zero.WithX(-.1f).WithY(.1f);

            GameLevelItemAsset asset = GameController.GetLevelItemAssetRandom(data);

            levelItems.Add(asset);
        }
        
        
        for (int i = 0; i < UnityEngine.Random.Range(5, 20); i++) {

            GameLevelItemAssetData data = new GameLevelItemAssetData();
            data.code = "box";
            data.limit = 3;
            data.destructable = true;
            data.gravity = true;
            data.kinematic = true;
            data.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            //data.range_rotation = Vector2.zero.WithX(.7f).WithY(1.2f);
            //data.range_rotation = Vector2.zero.WithX(-.1f).WithY(.1f);

            GameLevelItemAsset asset = GameController.GetLevelItemAssetRandom(data);

            levelItems.Add(asset);
        }

        /*
        for(int i = 0; i < UnityEngine.Random.Range(0, 3); i++) {
            GameLevelItemAssetStep step = new GameLevelItemAssetStep();
            step.position.FromVector3(GetRandomVectorInGameBounds());
            step.scale.FromVector3(Vector3.one * UnityEngine.Random.Range(.7f, 1.2f));
            step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));
            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.asset_code = "stone" + UnityEngine.Random.Range(1, 2).ToString();
            asset.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            asset.destructable = false;
            asset.reactive = false;
            asset.kinematic = false;
            asset.gravity = false;
            asset.steps.Add(step);
            levelItems.Add(asset);
        }
        
        for(int i = 0; i < UnityEngine.Random.Range(0, 1); i++) {
            GameLevelItemAssetStep step = new GameLevelItemAssetStep();
            step.position.FromVector3(GetRandomVectorInGameBounds());
            step.scale.FromVector3(Vector3.one * UnityEngine.Random.Range(.3f, .7f));
            step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));
            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.asset_code = "stone-spinny-thing";
            asset.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            asset.destructable = false;
            asset.reactive = false;
            asset.kinematic = false;
            asset.gravity = false;
            asset.rotation_speed.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-10f, 20f)));
            asset.steps.Add(step);
            levelItems.Add(asset);
        }
        
        for(int i = 0; i < UnityEngine.Random.Range(0, 1); i++) {
            GameLevelItemAssetStep step = new GameLevelItemAssetStep();
            step.position.FromVector3(GetRandomVectorInGameBounds());
            step.scale.FromVector3(Vector3.one * UnityEngine.Random.Range(.5f, 1f));
            step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));
            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.asset_code = "stone-spinny-thing2";
            asset.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            asset.destructable = false;
            asset.reactive = false;
            asset.kinematic = false;
            asset.gravity = false;
            asset.rotation_speed.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-10f, 50f)));
            asset.steps.Add(step);
            levelItems.Add(asset);
        }
        
        for(int i = 0; i < UnityEngine.Random.Range(0, 1); i++) {
            GameLevelItemAssetStep step = new GameLevelItemAssetStep();
            step.position.FromVector3(GetRandomVectorInGameBounds());
            step.scale.FromVector3(Vector3.one * UnityEngine.Random.Range(.5f, 1f));
            step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));
            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.asset_code = "blocks-gray-large";
            asset.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            asset.destructable = false;
            asset.reactive = false;
            asset.kinematic = false;
            asset.gravity = false;
            asset.rotation_speed.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-10f, 50f)));
            asset.steps.Add(step);
            levelItems.Add(asset);
        }
        
        for(int i = 0; i < UnityEngine.Random.Range(0, 1); i++) {
            GameLevelItemAssetStep step = new GameLevelItemAssetStep();
            step.position.FromVector3(GetRandomVectorInGameBounds());
            step.scale.FromVector3(Vector3.one * UnityEngine.Random.Range(.5f, 1f));
            step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));
            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.asset_code = "blocks-gray-small";
            asset.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            asset.destructable = false;
            asset.reactive = false;
            asset.kinematic = false;
            asset.gravity = false;
            asset.rotation_speed.FromVector3(Vector3.zero);
            asset.steps.Add(step);
            levelItems.Add(asset);
        }
        
        for(int i = 0; i < UnityEngine.Random.Range(0, 1); i++) {
            GameLevelItemAssetStep step = new GameLevelItemAssetStep();
            step.position.FromVector3(GetRandomVectorInGameBounds());
            step.scale.FromVector3(Vector3.one * UnityEngine.Random.Range(.5f, 1f));
            step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));
            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.asset_code = "rocket";
            asset.physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
            asset.destructable = true;
            asset.reactive = false;
            asset.kinematic = false;
            asset.gravity = false;
            asset.rotation_speed.FromVector3(Vector3.zero);
            asset.steps.Add(step);
            levelItems.Add(asset);
        }
        */

        return levelItems;
    }

    // -------------------------------------------------------
    
    // UPDATE

    public static bool touchHandled = false;
    
    // Update is called once per frame
    public virtual void Update() {

        // TOOLS
        
        if (UIGameKeyCodes.isActionProfileSave) {
            GameState.SaveProfile();
        }
        else if (UIGameKeyCodes.isActionProfileSync) {
            GameState.SyncProfile();
        }

        // UPDATE

        if (!isGameRunning) {
            return;
        }

        bool mousePressed = InputSystem.isMousePressed;
        //bool mouseSecondaryPressed = InputSystem.isMouseSecondaryPressed;
        bool touchPressed = InputSystem.isTouchPressed;
        bool handled = false;
    
        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        //bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();
                
        //if(controlInputTouchFinger) {

        if (touchPressed) {
            foreach (Touch touch in Input.touches) {
                handled = GameController.HandleTouchInputPoint(touch.position);
                
                if (handled)
                    break;
            }
        }
        else if (mousePressed) {
            handled = GameController.HandleTouchInputPoint(Input.mousePosition);
        }
        else {
            if (GameController.CurrentGamePlayerController != null) {
                
                // reset handled in input axis, might be keyboard input so don't reset.
                //if(GameController.CurrentGamePlayerController.controllerData.thirdPersonController != null) {
                //    GameController.CurrentGamePlayerController.controllerData.thirdPersonController.verticalInput = 0f;
                //    GameController.CurrentGamePlayerController.controllerData.thirdPersonController.horizontalInput = 0f;
                //    GameController.CurrentGamePlayerController.controllerData.thirdPersonController.verticalInput2 = 0f;
                //    GameController.CurrentGamePlayerController.controllerData.thirdPersonController.horizontalInput2 = 0f;
                //}
            }
        }

        //}

        touchHandled = handled;
        
        if (gameState == GameStateGlobal.GamePause
            || GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            return;
        }
        
        
        if (!gameObjectTimer.IsTimerPerf(
            GameObjectTimerKeys.gameUpdateAll)) {
            return;
        }
        
        if (isGameRunning) {
            GameController.CheckForGameOver();
        }
        
        currentTimeBlockBase += Time.deltaTime;
        
        if (currentTimeBlockBase > actionIntervalBase) {
            currentTimeBlockBase = 0.0f;
        }
    }    

    // ----------------------------------------------------------------------

    // EXTRA

    public virtual Vector3 cardinalAngles(Vector3 pos1, Vector3 pos2) {
    
        // Adjust both positions to be relative to our origin point (pos1)
        pos2 -= pos1;
        pos1 -= pos1;
    
        Vector3 angles = Vector3.zero;
    
        // Rotation to get from World +Z to pos2, rotated around World X (degrees up from Z axis)
        angles.x = Vector3.Angle(Vector3.forward, pos2 - Vector3.right * pos2.x);
    
        // Rotation to get from World +Z to pos2, rotated around World Y (degrees right? from Z axis)
        angles.y = Vector3.Angle(Vector3.forward, pos2 - Vector3.up * pos2.y);
    
        // Rotation to get from World +X to pos2, rotated around World Z (degrees up from X axis)
        angles.z = Vector3.Angle(Vector3.right, pos2 - Vector3.forward * pos2.z);
    
        return angles;
    }
    
    public virtual float contAngle(Vector3 fwd, Vector3 targetDir, Vector3 upDir) {
        var angle = Vector3.Angle(fwd, targetDir);
    
        if (angleDir(fwd, targetDir, upDir) == -1) {
            return 360 - angle;
        }
        else {
            return angle;
        }
    }
    
    //returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public virtual float angleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
    
        Vector3 perp = Vector3.Cross(fwd, targetDir);
    
        float dir = Vector3.Dot(perp, up);
    
        if (dir > 0.0) {
            return 1.0f;
        }
        else if (dir < 0.0) {
            return -1.0f;
        }
        else {
            return 0.0f;
        }
    }
}