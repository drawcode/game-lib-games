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
}

public class GameActorDataItem {
    public float health = 1f;
    public float difficulty = .3f;
    public float scale = 1f;
    public float speed = 1f;
    public float attack = 1f;
    public float defense = 1f;
    
    public string characterCode = "character-enemy-goblin";
    public string prefabCode = "GameEnemyGobln";
}

public class GameItemDataItem {
    public float health = 0f;
    public float difficulty = .3f;
    public float scale = 0f;
    public float speed = 0f;
    public float attack = 0f;
    public float defense = 0f;

    public GamePlayerItemType type = GamePlayerItemType.Coin;

    public string itemCode = "coin";
    public string prefabCode = "GamePlayerItemCoin";
}

public class BaseGameMessages {
    public static string scores = "game-shooter-scores";	
	public static string score = "game-shooter-score";
	public static string ammo = "game-shooter-ammo";
	public static string save = "game-shooter-save";
	public static string shot = "game-shooter-shot";
	public static string coin = "game-shooter-coin";
	public static string launch = "game-shooter-launch";
	public static string state = "game-shooter-state";
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
            if(timeRemaining <= 0) {
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
        if(timeRemaining > 0) {
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

public class GameLevelGridData {
    public float gridHeight = 1f;
    public float gridWidth = 45f;
    public float gridDepth = 30f;
    public float gridBoxSize = 4f;

    public bool centeredX = true;
    public bool centeredY = false;
    public bool centeredZ = true;

    public List<AppContentAsset> assets;

    public string[,,] assetMap;

    public GameLevelGridData() {
        Reset();
    }

    public void Reset() {
        ResetGrid(1, 45, 30);
        ClearAssets();
        ClearMap();
    }

    public void ResetGrid(int height, int width, int depth) {
        ResetGrid(height, width, depth, 4, true, false, true);
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

    public static GameLevelGridData GetBaseDefault() {

        GameLevelGridData data = new GameLevelGridData();
        data = AddAssets(data, "barrel-1", 10);

        return data;
    }

    public static GameLevelGridData GetDefault() {

        GameLevelGridData data = GameLevelGridData.GetBaseDefault();
        data.RandomizeAssetsInAssetMap();

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

    public string[,,] GetAssetMap() {
        return assetMap;
    }

    public void ClearAssets() {
        assets = new List<AppContentAsset>();
    }

    public void ClearMap() {
        assetMap = new string[(int)gridWidth,(int)gridHeight,(int)gridDepth];
    }

    public void SetAssets(string code, int count) {
        for(int i = 0; i < count; i++) {
            SetAsset(code);
        }
    }

    public void SetAsset(string code) {
        AppContentAsset asset = AppContentAssets.Instance.GetByCode(code);
        if(asset != null) {
            assets.Add(asset);
        }
    }

    public void SetAssetsInAssetMap(Vector3 pos, string code) {
        if(pos.x > gridWidth - 1) {
            pos.x = gridWidth - 1;
        }
        if(pos.y > gridHeight - 1) {
            pos.y = gridHeight - 1;
        }
        if(pos.z > gridDepth - 1) {
            pos.z = gridDepth - 1;
        }

        assetMap[(int)pos.x,(int)pos.y,(int)pos.z] = code;
    }

    public void RandomizeAssetsInAssetMap() {
        foreach(AppContentAsset asset in assets) {
            string row = "";

            int x = 0;
            int y = 0;
            int z = 0;

            Debug.Log("RandomizeAssetsInAssetMap:row" + row);

            while(string.IsNullOrEmpty(row)) {

                x = UnityEngine.Random.Range(0, (int)gridWidth - 1);
                y = UnityEngine.Random.Range(0, (int)gridHeight - 1);
                z = UnityEngine.Random.Range(0, (int)gridDepth - 1);

                if(string.IsNullOrEmpty(row)) {
                    Vector3 pos = Vector3.one.WithX(x).WithY(y).WithZ(z);
                    SetAssetsInAssetMap(pos, asset.code);
                }

                row = assetMap[x,y,z];

            Debug.Log("RandomizeAssetsInAssetMap:row:" + row);
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

public class BaseGameController : MonoBehaviour {

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

    // ----------------------------------------------------------------------

    public virtual void Awake() {

    }

    public virtual void Start() {

    }

    public virtual void Init() {

        GameController.Reset();

        foreach(GamePlayerController gamePlayerController in ObjectUtil.FindObjects<GamePlayerController>()) {
            if(gamePlayerController.uuid == UniqueUtil.Instance.currentUniqueId) {
                gamePlayerController.UpdateNetworkContainer(gamePlayerController.uuid);
                break;
            }
        }
        
        GameController.InitGameWorldBounds();
        
        GameController.LoadCharacterTypes();
        GameDraggableEditor.LoadDraggableContainerObject();

        GameCustomController.BroadcastCustomColorsSync();
    }

    public virtual void OnEnable() {
        Gameverses.GameMessenger<string>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameAIDirectorData>.AddListener(
            GameAIDirectorMessages.gameAIDirectorSpawnActor,
            OnGameAIDirectorData);

        Messenger<GameItemDirectorData>.AddListener(
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

        Messenger<GameItemDirectorData>.RemoveListener(
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

            if(GameDraggableEditor.isEditing) {
                return false;
            }

            return true;

        }
    }
    
    // ---------------------------------------------------------------------
    
    // EVENTS

    public virtual void OnProfileShouldBeSavedEventHandler() {
        GameState.SaveProfile();
    }
    
    public virtual void OnEditStateHandler(GameDraggableEditEnum state) {
    
        if(state == GameDraggableEditEnum.StateEditing) {
            ////GameHUD.Instance.ShowCurrentCharacter();
        }
        else {

            GameHUD.Instance.ShowGameState();
    
            GameUIController.ShowHUD();
            //ShowUIPanelEditButton();
        }
    }   
    
    // Listen to object creation events and create them such as network players...
    
    public virtual void OnNetworkPlayerContainerAdded(string uuid) {
    
        // Look for object by that uuid, if not create it

        GamePlayerController[] playerControllers = ObjectUtil.FindObjects<GamePlayerController>();
    
        if(playerControllers.Length > 0) {
    
            bool found = false;

            foreach(GamePlayerController gamePlayerController in playerControllers) {
                if(gamePlayerController.uuid == uuid) {
                    // already added
                    gamePlayerController.uuid = uuid;
                    gamePlayerController.UpdateNetworkContainer(uuid);
                    //gamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerNetwork);
                    LogUtil.Log("Updating character:" + uuid);
                    found = true;
                    break;
                }
            }

            if(!found) {
                // create
                // Prefabs/Characters/GamePlayerObject
    
                UnityEngine.Object prefabGameplayer = Resources.Load("Prefabs/Characters/GamePlayerObject");
                if(prefabGameplayer != null) {
                    Vector3 placementPos = Vector3.zero;
                    placementPos.z = -3f;
                    GamePlayerController playerControllerOther = (Instantiate(prefabGameplayer, placementPos, Quaternion.identity) as GameObject).GetComponent<GamePlayerController>();
                    playerControllerOther.ChangePlayerState(GamePlayerControllerState.ControllerNetwork);
                    playerControllerOther.uuid = uuid;
                    playerControllerOther.UpdateNetworkContainer(uuid);
                    LogUtil.Log("Creating character:" + uuid);
                    LogUtil.Log("playerControllerOther.uuid:" + playerControllerOther.uuid);
                }
            }
        }
    }

    public virtual void OnGameAIDirectorData(GameAIDirectorData actor) {

    }

    public virtual void OnGameItemDirectorData(GameItemDirectorData item) {

    }

    // ---------------------------------------------------------------------

    // GAMEPLAYER CONTROLLER
    
    public virtual GamePlayerController getCurrentPlayerController {
        get {
            return getCurrentController();
        }
    }

    public virtual GamePlayerController getCurrentController() {
        if(GameController.Instance.currentGamePlayerController != null) {
            return GameController.Instance.currentGamePlayerController;
        }
        return null;
    }


    public virtual GamePlayerController getGamePlayerController(GameObject go) {
        if(go != null) {
            GamePlayerController gamePlayerController = go.GetComponentInChildren<GamePlayerController>();
            if(gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual GamePlayerController getGamePlayerControllerParent(GameObject go) {
        if(go != null) {
            GamePlayerCollision gamePlayerCollision = go.Get<GamePlayerCollision>();

            if(gamePlayerCollision != null) {
                return null;
            }

            if(gamePlayerCollision.gamePlayerController == null) {
                return null;
            }
               
            GamePlayerController gamePlayerController = gamePlayerCollision.gamePlayerController;
            if(gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual GamePlayerController getGamePlayerControllerObject(GameObject go, bool onlyPlayerControlled) {

        GamePlayerController gamePlayerController = null;

        if(go == null) {
            return gamePlayerController;
        }

        if(go.name.Contains("GamePlayerObject")) {

            gamePlayerController = GameController.GetGamePlayerController(go);

            if(gamePlayerController != null) {

                if(!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return gamePlayerController;
                }
            }
        }

        if(gamePlayerController == null
           && (go.name.Contains("Game")
            || go.name.Contains("Helmet")
            || go.name.Contains("Facemask"))) {
            //&& (go.name.Contains("Helmet")
            //|| go.name.Contains("Facemask"))) {

            //Debug.Log("GameObjectChoice:HelmetFacemask:" + go.name);

            gamePlayerController = GameController.GetGamePlayerControllerParent(go);

            if(gamePlayerController != null) {

                Debug.Log("GameObjectChoice:gamePlayerController:" + gamePlayerController.name);

                if(!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return gamePlayerController;
                }
            }
        }

        return gamePlayerController;
    }


    public virtual bool hasGamePlayerControllerObject(GameObject go, bool onlyPlayerControlled) {

        GamePlayerController gamePlayerController = null;

        if(go == null) {
            return false;
        }

        if(go.name.Contains("GamePlayerObject")) {

            gamePlayerController = GameController.GetGamePlayerController(go);

            if(gamePlayerController != null) {

                if(!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return true;
                }
            }
        }

        if(gamePlayerController == null
           && (go.name.Contains("Game")
            || go.name.Contains("Helmet")
            || go.name.Contains("Facemask"))) {

            Debug.Log("GameObjectChoice:HelmetFacemask:" + go.name);

            gamePlayerController = GameController.GetGamePlayerControllerParent(go);

            if(gamePlayerController != null) {

                Debug.Log("GameObjectChoice:gamePlayerController:" + gamePlayerController.name);

                if(!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return true;
                }
            }
        }

        return false;
    }

    // SCORING

    public virtual void gamePlayerScores(double val) {
        if(GameController.CurrentGamePlayerController != null) {
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
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttack();
        }
    }

    //public static void GamePlayerAttackAlt() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackAlt();
    //    }
    //}

    public virtual void gamePlayerAttackAlt() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackAlt();
        }
    }

    //public static void GamePlayerAttackRight() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackRight();
    //    }
    //}

    public virtual void gamePlayerAttackRight() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendAttackRight();
        }
    }

    //public static void GamePlayerAttackLeft() {
    //    if(isInst) {
    //        Instance.gamePlayerAttackLeft();
    //    }
    //}

    public virtual void gamePlayerAttackLeft() {
        if(GameController.CurrentGamePlayerController != null) {
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
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefend();
        }
    }

    //public static void GamePlayerDefendAlt() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendAlt();
    //    }
    //}

    public virtual void gamePlayerDefendAlt() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendAlt();
        }
    }

    //public static void GamePlayerDefendRight() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendRight();
    //    }
    //}

    public virtual void gamePlayerDefendRight() {
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.SendDefendRight();
        }
    }

    //public static void GamePlayerDefendLeft() {
    //    if(isInst) {
    //        Instance.gamePlayerDefendLeft();
    //    }
    //}

    public virtual void gamePlayerDefendLeft() {
        if(GameController.CurrentGamePlayerController != null) {
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
        if(GameController.CurrentGamePlayerController != null) {
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
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputUse();
        }
    }

    // SKILL

    //public static void GamePlayerSkill() {
    //    if(isInst) {
    //        Instance.gamePlayerSkill();
    //    }
    //}

    public virtual void gamePlayerSkill() {
        if(GameController.CurrentGamePlayerController != null) {
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
        if(GameController.CurrentGamePlayerController != null) {
            GameController.CurrentGamePlayerController.InputMagic();
        }
    }

    // ----------------------------------------------------------------------

    // ZONES

    public virtual GameZone getGameZone(GameObject go) {
        if(go != null) {
            return go.GetComponent<GameZone>();
        }
        return null;
    }

    public virtual GameGoalZone getGoalZone(GameObject go) {
        if(go != null) {
            return go.GetComponent<GameGoalZone>();
        }
        return null;
    }

    public virtual GameBadZone getBadZone(GameObject go) {
        if(go != null) {
            return go.GetComponent<GameBadZone>();
        }
        return null;
    }

    public virtual void changeGameZone(GameZones zones) {

        if(gameEndZoneLeft == null) {
            Transform gameEndZoneLeftTransform
                = levelZonesContainerObject.transform.FindChild("GameGoalZoneLeft");
            if(gameEndZoneLeftTransform != null) {
                gameEndZoneLeft = gameEndZoneLeftTransform.gameObject;
            }
        }

        if(gameEndZoneLeft == null) {
            Transform gameEndZoneRightTransform
                = levelZonesContainerObject.transform.FindChild("GameGoalZoneRight");
            if(gameEndZoneRightTransform != null) {
                gameEndZoneRight = gameEndZoneRightTransform.gameObject;
            }
        }

        GameController.GoalZoneChange(zones);
    }

    // ---------------------------------------------------------------------
    
    // BOUNDS
    
    public virtual void initGameWorldBounds() {
        if(gameBounds == null) {
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

        Debug.Log("GAME START FLOW: STEP #10: loadLevelAssets: code:" + code);

        GameDraggableEditor.levelItemsContainerObject = GameController.Instance.levelItemsContainerObject;
    
        GameLevelItems.Current.code = code;
    
        // Clear items from LevelContainer
        GameController.Reset();

        // Change data codes
        GameLevels.Instance.ChangeCurrentAbsolute(code);
        GameLevelItems.Instance.ChangeCurrentAbsolute(code);

        // Prepare game level items for this mode
        GameController.LoadLevelItems();
    
        // Load in the level assets
        GameDraggableEditor.LoadLevelItems();
    
    }

    public virtual void loadLevel(string code) {


        Debug.Log("GAME START FLOW: STEP #6: loadLevel: code:" + code);

        // Load the game levelitems for the game level code
        ////GameController.StartGame(code);
        GameController.PrepareGame(code);

        GameController.LoadLevelAssets(code);
    
        GameHUD.Instance.SetLevelInit(GameLevels.Current);
    
        GameHUD.Instance.AnimateIn();
    
        //GameUI.Instance.ToggleGameUI();
    
    }
    
    public virtual void loadLevelItems() {

        bool updated = false;

        // Load level items by game type....

        if(AppModes.Instance.isAppModeGameArcade) {
            Debug.Log("loadLevelItems: AppModes.Instance.isAppModeGameArcade:"
                + AppModes.Instance.isAppModeGameArcade);

            if(AppModeTypes.Instance.isAppModeTypeGameDefault) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                    + AppModeTypes.Instance.isAppModeTypeGameDefault);

                if(AppContentStates.Instance.isAppContentStateGameArcade) {
    
                    Debug.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameArcade:"
                        + AppContentStates.Instance.isAppContentStateGameArcade);
    
                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                    updated = true;
                }
            }
        }
        else if(AppModes.Instance.isAppModeGameChallenge) {

            Debug.Log("loadLevelItems: AppModes.Instance.isAppModeGameChallenge:"
                + AppModes.Instance.isAppModeGameChallenge);

            if(AppModeTypes.Instance.isAppModeTypeGameDefault) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                    + AppModeTypes.Instance.isAppModeTypeGameDefault);

                if(AppContentStates.Instance.isAppContentStateGameChallenge) {

                    Debug.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameChallenge:"
                        + AppContentStates.Instance.isAppContentStateGameChallenge);

                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                    updated = true;
                }
            }
        }
        else if(AppModes.Instance.isAppModeGameTraining) {
            Debug.Log("loadLevelItems: AppModes.Instance.isAppModeGameTraining:"
                + AppModes.Instance.isAppModeGameTraining);

            if(AppModeTypes.Instance.isAppModeTypeGameDefault) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                    + AppModeTypes.Instance.isAppModeTypeGameDefault);

            }
            else if(AppModeTypes.Instance.isAppModeTypeGameChoice) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameChoice:"
                    + AppModeTypes.Instance.isAppModeTypeGameChoice);

                // LOAD CHOICE GAME LEVEL ITEMS

                if(AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz) {

                    Debug.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingChoiceQuiz:"
                        + AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz);

                    //



                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid(GameLevelGridData.GetModeTypeChoice(4));
                    updated = true;
                }

            }
            else if(AppModeTypes.Instance.isAppModeTypeGameCollection) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameCollection:"
                    + AppModeTypes.Instance.isAppModeTypeGameCollection);

                // LOAD COLLECTION GAME LEVEL ITEMS

                /*
                if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts) {

                    Debug.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingCollectionSmarts:"
                        + AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts);

                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                }
                else if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {

                    Debug.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingCollectionSafety:"
                        + AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety);

                    GameLevelItems.Current.level_items
                        = GameController.GetLevelRandomizedGrid();
                }*/

            }
            else if(AppModeTypes.Instance.isAppModeTypeGameContent) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameContent:"
                    + AppModeTypes.Instance.isAppModeTypeGameContent);

            }
            else if(AppModeTypes.Instance.isAppModeTypeGameTips) {

                Debug.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameTips:"
                    + AppModeTypes.Instance.isAppModeTypeGameTips);

            }
        }

        if(!updated) {
            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGrid();
        }
    }

    public virtual void saveCurrentLevel() {
        GameDraggableEditor.SaveCurrentLevel(
            GameController.Instance.levelItemsContainerObject);
    }

    public virtual void loadStartLevel(string levelCode) {

        Debug.Log("GAME START FLOW: STEP #1: loadStartLevel: levelCode:" + levelCode);

        string characterCode = GameProfileCharacters.Current.GetCurrentCharacterCode();
        GameController.LoadCharacterStartLevel(characterCode, levelCode);
    }

    public virtual void loadStartLevel(string characterCode, string levelCode) {
        GameController.LoadCharacterStartLevel(characterCode, levelCode);
    }

    public virtual void startLevel(string levelCode) {
        StartCoroutine(startLevelCo(levelCode));
    }
       
    public virtual IEnumerator startLevelCo(string levelCode) {

        Debug.Log("GAME START FLOW: STEP #5: startLevelCo: levelCode:" + levelCode);

        GameController.ResetCurrentGamePlayer();
        GameController.ResetLevelEnemies();
        
        if(currentGamePlayerController != null) {
            currentGamePlayerController.PlayerEffectWarpFadeIn();
        }
        GameUIPanelOverlays.Instance.ShowOverlayWhite();

        yield return new WaitForSeconds(1f);
        
        GameHUD.Instance.ResetIndicators();
        GameController.LoadLevel(levelCode);
        
        // TODO load anim
        
        yield return new WaitForSeconds(1f);
        
        if(currentGamePlayerController != null) {
            currentGamePlayerController.PlayerEffectWarpFadeOut();
        }
        GameUIPanelOverlays.Instance.HideOverlayWhiteFlashOut();
    }
     
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

    public virtual void changeGameStates(string appContentState) {

        LogUtil.Log("changeGameStates:appContentState:" + appContentState);

        //try {
            AppContentStates.Instance.ChangeState(appContentState);
        //}
        //catch (Exception e) {
        //    LogUtil.Log("ERROR:changeGameState:e:" + e.Message + e.StackTrace);
        //}
        GameController.SaveGameStates();

        LogUtil.Log("changeGameStates:appContentState:AFTER:" + appContentState);

    }//AppContentStates.Instance.ChangeState(AppContentStateMeta.appContentStateGameArcade);
    
    public virtual void loadProfileCharacter(string characterCode) {

        Debug.Log("GAME START FLOW: STEP #3: loadProfileCharacter: characterCode:" + characterCode);

        GameProfileCharacters.Current.SetCurrentCharacterCode(characterCode);

        string characterSkinCode = GameProfileCharacters.Current.GetCurrentCharacterCostumeCode();
        GameController.CurrentGamePlayerController.LoadCharacter(characterSkinCode);
    
        GameCustomController.SetCustomColorsPlayer(
            GameController.CurrentGamePlayerController.gameObject);
    }
    
    public virtual void loadCharacterStartLevel(string characterCode, string levelCode) {

        Debug.Log("GAME START FLOW: STEP #2: loadCharacterStartLevel: characterCode:" + characterCode +
            " levelCode:" + levelCode);

        loadProfileCharacter(characterCode);
        ////GameHUD.Instance.ShowCharacter(characterCode);
        
        GameDraggableEditor.ChangeStateEditing(GameDraggableEditEnum.StateNotEditing);
        GameController.StartLevel(levelCode);

       // GameController(levelCode);
    }

    public virtual void loadEnemyBot1(float scale, float speed, float attack) {
        GameActorDataItem character = new GameActorDataItem();
        character.characterCode = "character-enemy-bot1";
        character.prefabCode = "GameEnemyBot1";
        character.scale = scale;
        character.attack = attack;
        character.speed = speed;
        GameController.LoadActor(character);
    }

    public virtual void loadActor(GameActorDataItem character) {
        StartCoroutine(loadActorCo(character));
    }

    public virtual void loadItemData(GameItemDirectorData data) {
        GameItemDataItem item = new GameItemDataItem();
        if(item.type == GamePlayerItemType.Coin) {
            item.itemCode = "coin";
            item.prefabCode = "GamePlayerItemCoin";
        }
        item.scale = data.scale;
        item.attack = data.attack;
        item.speed = data.speed;
        GameController.LoadItem(item);
    }

    public virtual void loadItem(GameItemDataItem item) {
        StartCoroutine(loadItemCo(item));
    }

    public virtual Vector3 getCurrentPlayerPosition() {
        Vector3 currentPlayerPosition = Vector3.zero;
        if(GameController.CurrentGamePlayerController != null) {
            if(GameController.CurrentGamePlayerController.gameObject != null) {
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
    
        if(playerBottomLeft < boundaryBottomLeftPosition.x) {
            playerBottomLeft = boundaryBottomLeftPosition.x;
        }
        else if(playerBottomRight > boundaryBottomRightPosition.x) {
            playerBottomRight = boundaryBottomRightPosition.x;
        }
        else if(playerTopRight < boundaryTopRightPosition.z) {
            playerTopRight = boundaryTopRightPosition.z;
        }
        else if(playerTopLeft < boundaryTopLeftPosition.z) {
            playerTopLeft = boundaryTopLeftPosition.z;
        }

        spawnLocation.z = UnityEngine.Random.Range(playerTopLeft,playerTopRight);
        spawnLocation.x = UnityEngine.Random.Range(playerBottomLeft,playerBottomRight);
        spawnLocation.y = 0f;

        return spawnLocation;
    }

    public virtual string getCharacterModelPath(GameActorDataItem character) {
        string modelPath = Contents.appCacheVersionSharedPrefabCharacters;
        // TODO load up dater
        modelPath = PathUtil.Combine(modelPath, "GameEnemyBot1");
        return modelPath;
    }

    public virtual string getCharacterType(GameActorDataItem character) {
        string type = "bot1";
        // TODO load up
        type = "bot1";
        return type;
    }

    public virtual IEnumerator loadActorCo(GameActorDataItem character) {

        string modelPath = GameController.GetCharacterModelPath(character);
        string characterType = GameController.GetCharacterType(character);

        // TODO data and pooling and network
    
        GameObject prefabObject = Resources.Load(modelPath) as GameObject;
        Vector3 spawnLocation = Vector3.zero;

        bool isZoned = true;

        if(isZoned) {
            // get left/right spawn location
            string leftMiddle = "left-middle";
            string rightMiddle = "right-middle";
            string spawnCode = rightMiddle;
            if(currentGameZone == GameZones.right) {
                spawnCode = rightMiddle;
            }
            else if(currentGameZone == GameZones.left) {
                spawnCode = leftMiddle;
            }

            Debug.Log("spawnCode:" + spawnCode);

            //GamePlayerSpawn spawn = GameAIController.GetSpawn(spawnCode);
            //if(spawn != null) {
            //    spawnLocation = spawn.gameObject.transform.position;
            //}
            //else {

                // get random
                if(currentGameZone == GameZones.right) {
                    spawnLocation = Vector3.zero.WithX(80f).WithZ(UnityEngine.Random.Range(-20, 20));
                }
                else if(currentGameZone == GameZones.left) {
                    spawnLocation = Vector3.zero.WithX(-80f).WithZ(UnityEngine.Random.Range(-20, 20));
                }
            //}

        }
        else {
            // get random
            spawnLocation = GameController.GetRandomSpawnLocation();
        }

        if(prefabObject == null) {
            yield break;
        }

        GameObject characterObject = GameObjectHelper.CreateGameObject(
            prefabObject, spawnLocation, Quaternion.identity, GameConfigs.usePooledGamePlayers) as GameObject;
    
        characterObject.transform.parent = levelActorsContainerObject.transform;

        GameCustomController.SetCustomColorsEnemy(characterObject);
    
        GamePlayerController characterGamePlayerController
            = characterObject.GetComponentInChildren<GamePlayerController>();

        characterGamePlayerController.LoadCharacter(characterType);

        characterGamePlayerController.transform.localScale
            = characterGamePlayerController.transform.localScale * character.scale;

        // Wire up ai controller to setup player health, speed, attack etc.

        //characterGamePlayerController.runtimeData.

        if(characterGamePlayerController != null) {
            characterObject.Hide();
            yield return new WaitForEndOfFrame();
            // wire up properties
    
            // TODO network and player target
            //characterGamePlayerController.currentTarget = GameController.CurrentGamePlayerController.gameObject.transform;
            //characterGamePlayerController.ChangeContextState(GamePlayerContextState.ContextFollowAgent);
            //characterGamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerAgent);
            characterObject.Show();

            // Add indicator to HUD
    
            GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators, characterObject, characterType);

            //characterGamePlayerController.Init(GamePlayerControllerState.ControllerAgent);
        }
    }

    public virtual string getItemPath(GameItemDataItem item) {
        string modelPath = Contents.appCacheVersionSharedPrefabLevelItems;
        modelPath = PathUtil.Combine(modelPath, item.prefabCode);
        return modelPath;
    }

    public virtual string getItemType(GameItemDataItem item) {
        string type = "coin";
        type = item.itemCode;
        return type;
    }

    public virtual IEnumerator loadItemCo(GameItemDataItem item) {

        string modelPath = GameController.GetItemPath(item);
        //string characterType = GameController.GetItemType(item);

        // TODO data and pooling and network

        // 1) Without using the ObjectPoolManager:
// Projectile bullet = Instantiate( bulletPrefab, position, rotation ) as Projectile;
// Destroy( bullet.gameObject );
//
// 2) Using the ObjetPoolManager:
// Projectile bullet = ObjectPoolManager.createPooled( bulletPrefab.gameObject, position, rotation ).GetComponent<Bullet>();
// ObjectPoolManager.destroyPooled( bullet.gameObject );


    
        GameObject prefabObject = Resources.Load(modelPath) as GameObject;
        Vector3 spawnLocation = Vector3.zero;

        bool isZoned = true;

        if(isZoned) {
            // get left/right spawn location
            string leftMiddle = "left-middle";
            string rightMiddle = "right-middle";
            string spawnCode = rightMiddle;
            if(currentGameZone == GameZones.right) {
                spawnCode = rightMiddle;
            }
            else if(currentGameZone == GameZones.left) {
                spawnCode = leftMiddle;
            }

            //Debug.Log("spawnCode:" + spawnCode);

            //GamePlayerSpawn spawn = GameAIController.GetSpawn(spawnCode);
            //if(spawn != null) {
            //    spawnLocation = spawn.gameObject.transform.position;
            //}
            //else {

                // get random
                if(currentGameZone == GameZones.right) {

                    spawnLocation = Vector3.zero
                        .WithX(UnityEngine.Random.Range(
                            0, gameBounds.boundaryTopRight.transform.position.x))
                        .WithY(50f)
                        .WithZ(UnityEngine.Random.Range(
                                boundaryBottomLeft.transform.position.z,
                                boundaryTopLeft.transform.position.z));
                }
                else if(currentGameZone == GameZones.left) {

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
        else {
            // get random
            spawnLocation = GameController.GetRandomSpawnLocation();
        }

        if(prefabObject == null) {
            yield break;
        }

        GameObject spawnObj = GameObjectHelper.CreateGameObject(
            prefabObject, spawnLocation, Quaternion.identity, GameConfigs.usePooledItems) as GameObject;

        string itemTypeString = GamePlayerIndicatorType.pickup.ToString();

        if(item.type == GamePlayerItemType.Coin) {
            itemTypeString = "coin";
        }
        else if(item.type == GamePlayerItemType.Health) {
            itemTypeString = "health";
        }
    
        spawnObj.transform.parent = levelActorsContainerObject.transform;
        GamePlayerIndicator.AddIndicator(spawnObj, itemTypeString);

        /*
        GamePlayerController characterGamePlayerController
            = characterObject.GetComponentInChildren<GamePlayerController>();

        characterGamePlayerController.transform.localScale
            = characterGamePlayerController.transform.localScale * character.scale;

        // Wire up ai controller to setup player health, speed, attack etc.

        //characterGamePlayerController.runtimeData.

        if(characterGamePlayerController != null) {
            characterObject.Hide();
            yield return new WaitForEndOfFrame();
            // wire up properties
    
            // TODO network and player target
            //characterGamePlayerController.currentTarget = GameController.CurrentGamePlayerController.gameObject.transform;
            //characterGamePlayerController.ChangeContextState(GamePlayerContextState.ContextFollowAgent);
            //characterGamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerAgent);
            characterObject.Show();

            // Add indicator to HUD

            GameHUD.Instance.AddIndicator(characterObject, characterType);

            //characterGamePlayerController.Init(GamePlayerControllerState.ControllerAgent);
        }
        */
    }


    // ---------------------------------------------------------------------
    // RESETS
    
    public virtual void resetLevelEnemies() {
        if(levelActorsContainerObject != null) {
            levelActorsContainerObject.DestroyChildren();
        }
    }

    public virtual void resetCurrentGamePlayer() {
        if(currentGamePlayerController != null) {
            currentGamePlayerController.Reset();
        }
    }

    public virtual void reset() {
        
        GameController.StopDirectors();

        GameController.ResetRuntimeData();
        GameController.ResetCurrentGamePlayer();

        GameController.ResetLevelEnemies();
        GameUIController.HideGameCanvas();

        GameDraggableEditor.ClearLevelItems(levelItemsContainerObject);
        GameDraggableEditor.ResetCurrentGrabbedObject();
        GameDraggableEditor.HideAllEditDialogs();
    }

    public virtual void resetRuntimeData() {

        GameController.ResetCurrentGamePlayer();
        GameController.ResetLevelEnemies();

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
        foreach(GameCharacterType type in GameCharacterTypes.Instance.GetAll()) {
            if(!gameCharacterTypes.Contains(type.code)) {
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
    
        if(updatedIndex > gameCharacterTypes.Count - 1) {
            currentCharacterTypeIndex = 0;
        }
        else if(updatedIndex < 0) {
            currentCharacterTypeIndex = gameCharacterTypes.Count - 1;
        }
        else {
            currentCharacterTypeIndex = updatedIndex;
        }
    
        if(currentGamePlayerController != null) {
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
        Invoke ("resultsGame", .5f);
    }

    public virtual void resultsGame() {
        GameController.ChangeGameState(GameStateGlobal.GameResults);
    }

    public virtual void togglePauseGame() {
        if(gameState == GameStateGlobal.GamePause) {
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
        gameSetTimeScale(timeScale);
        gameRunningState = GameRunningState.PAUSED;
        GameController.Instance.gameState = GameStateGlobal.GameContentDisplay;
    }

    // EVENTS ON

    public virtual void onGameContentDisplay() {
        // Show runtime content display data
        //GameRunningStatePause();
    
        if(contentDisplayCode == GameContentDisplayTypes.gamePlayerOutOfBounds) {
    
            GameController.GamePlayerOutOfBoundsDelayed(3f);
    
            UIPanelDialogBackground.ShowDefault();
            UIPanelDialogDisplay.SetTitle("OUT OF BOUNDS");
            //UIPanelDialogDisplay.SetDescription("RUN, BUT STAY IN BOUNDS...");
            UIPanelDialogDisplay.ShowDefault();
        }
        else if(contentDisplayCode == GameContentDisplayTypes.gameEnergy) {
            handleContentDialogEnergy();
        }
        else if(contentDisplayCode == GameContentDisplayTypes.gameHealth) {
            handleContentDialogHealth();
        }
        else if(contentDisplayCode == GameContentDisplayTypes.gameXP) {
            handleContentDialogXP();
        }
        else {
        
            //AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz
            UIPanelDialogBackground.HideAll();
        }
    
        //GameRunningStateRun();
    }

    public virtual void handleContentTutorial() {
        
        if(contentDisplayCode == GameContentDisplayTypes.gameTutorial) {            
            UIPanelDialogBackground.ShowDefault();
        }
    }
    
    public virtual void handleContentTips() {
        
        if(contentDisplayCode == GameContentDisplayTypes.gameTips) {            
            UIPanelDialogBackground.ShowDefault();
        }
    }
    
    public virtual void handleContentOverview() {
        
        if(contentDisplayCode == GameContentDisplayTypes.gameModeContentOverview) {
            UIPanelOverviewMode.ShowDefault();
        }
    }

    public virtual void handleContentDialogEnergy() {
        UIPanelDialogRPGEnergy.ShowDefault();
        UIPanelDialogRPGHealth.HideAll();
        UIPanelDialogRPGXP.HideAll();
    }

    public virtual void handleContentDialogHealth() {
        UIPanelDialogRPGEnergy.HideAll();
        UIPanelDialogRPGHealth.ShowDefault();
        UIPanelDialogRPGXP.HideAll();
    }

    public virtual void handleContentDialogXP() {
        UIPanelDialogRPGEnergy.HideAll();
        UIPanelDialogRPGHealth.HideAll();
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

    public virtual void onGamePrepare(bool startGame) {

        Debug.Log("GAME START FLOW: STEP #7: onGamePrepare: startGame:" + startGame);

        GameController.ResetRuntimeData();

        GameUIController.HideUI(true);
        GameUIController.HideHUD();

        if(allowedEditing) {
            GameDraggableEditor.ShowUIPanelEditButton();
        }

        if(startGame) {
            GameController.StartGame(GameLevels.Current.code);
        }
    }
    
    public virtual void onGameStarted() {

        Debug.Log("GAME START FLOW: STEP #8: onGameStarted");

        GameController.StartLevelStats();

        GameUIController.HideUI(true);
        GameUIController.ShowHUD();

        if(allowedEditing) {
            GameDraggableEditor.ShowUIPanelEditButton();
        }
    
        GameController.GameRunningStateRun();

        GameUIController.ShowGameCanvas();
    
        GameController.RunDirectorsDelayed(6f);
    }   

    public virtual void onGameQuit() {
    
        // Cleanup
        GameUIController.HideHUD();
        GameDraggableEditor.HideAllEditDialogs();
        GameDraggableEditor.HideAllUIEditPanels();
    
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
    }
    
    public virtual void onGameNotStarted() {
        //
    }
    
    public virtual void onGameResults() {

        LogUtil.Log("OnGameResults");
    
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
    }
    
    public virtual void changeGameState(GameStateGlobal gameStateTo) {
        gameState = gameStateTo;
    
        Messenger<GameStateGlobal>.Broadcast(GameMessages.state, gameState);
    
        if(gameState == GameStateGlobal.GameStarted) {
            GameController.OnGameStarted();
        }
        else if(gameState == GameStateGlobal.GamePause) {
            GameController.OnGamePause();
        }
        else if(gameState == GameStateGlobal.GameResume) {
            GameController.OnGameResume();
        }
        else if(gameState == GameStateGlobal.GameQuit) {
            GameController.OnGameQuit();
        }
        else if(gameState == GameStateGlobal.GameNotStarted) {
            GameController.OnGameNotStarted();
        }
        else if(gameState == GameStateGlobal.GameResults) {
            GameController.OnGameResults();
        }
        else if(gameState == GameStateGlobal.GameContentDisplay) {
            GameController.OnGameContentDisplay();
        }
        else if(gameState == GameStateGlobal.GamePrepare) {
            GameController.OnGamePrepare();
        }
    }

    public bool isGameRunning {
        get {
            if(gameState == GameStateGlobal.GameStarted) {
                return true;
            }
            return false;
        }
    }

    // -------------------------------------------------------
    
    // GAME CAMERA
    
    public virtual void changeGameCameraMode(GameCameraView cameraViewTo) {
        if(cameraViewTo == cameraView) {
            return;
        }
        else {
            cameraView = cameraViewTo;
        }
    
        LogUtil.Log("ChangeGameCameraMode:cameraViewTo: " + cameraViewTo);
    
        if(cameraGame != null
            && cameraGameGround != null) {

            if(cameraView == GameCameraView.ViewSide) {

                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(30);

                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if(cameraView == GameCameraView.ViewSideTop) {

                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(80);

                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if(cameraView == GameCameraView.ViewBackTilt) {

                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(45).WithY(90);

                GameController.ChangeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                GameController.ChangeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if(cameraView == GameCameraView.ViewBackTop) {

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
        iTween.RotateTo(cameraObject, rotationTo, timeDelay);
    }

    public virtual void cycleGameCameraMode() {

        LogUtil.Log("CycleGameCameraMode: " + cameraView);

        if(cameraView == GameCameraView.ViewSide) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewSideTop);
        }
        else if(cameraView == GameCameraView.ViewSideTop) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewBackTilt);
        }
        else if(cameraView == GameCameraView.ViewBackTilt) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewBackTop);
        }
        else if(cameraView == GameCameraView.ViewBackTop) {
            GameController.ChangeGameCameraMode(GameCameraView.ViewSide);
        }
    }


    // -------------------------------------------------------

    // GAME PLAYER BOUNDS
    
    public virtual void gamePlayerOutOfBounds() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        GameAudioController.Instance.PlayWhistle();
        GameAudioController.Instance.PlayOh();
        GameController.GameContentDisplay(GameContentDisplayTypes.gamePlayerOutOfBounds);
    }

    public virtual void gamePlayerOutOfBoundsDelayed(float delay) {
        StartCoroutine(gamePlayerOutOfBoundsDelayedCo(delay));
    }

    public virtual IEnumerator gamePlayerOutOfBoundsDelayedCo(float delay) {

        yield return new WaitForSeconds(delay);

        Debug.Log("GamePlayerOutOfBoundsDelayed:");

        runtimeData.outOfBounds = true;

        Debug.Log("GamePlayerOutOfBoundsDelayed:runtimeData.outOfBounds:" + runtimeData.outOfBounds);

        gameState = GameStateGlobal.GameStarted;

        Debug.Log("GamePlayerOutOfBoundsDelayed:gameState:" + gameState);

        GameController.CheckForGameOver();
    }

    // -------------------------------------------------------

    // GAME PLAYER GOAL ZONE

    public virtual void goalZoneChange() {
        if(currentGameZone == GameZones.left) {
            GameController.GoalZoneChange(GameZones.right);
        }
        else if(currentGameZone == GameZones.right) {
            GameController.GoalZoneChange(GameZones.left);
        }
    }

    public virtual void goalZoneChange(GameZones goalZone) {
        //if(currentGameZone == goalZone) {
        //    return;
        //}

        if(goalZone == GameZones.left) {
            currentGameZone = goalZone;
        }
        else if(goalZone == GameZones.right) {
            currentGameZone = goalZone;
        }

        GameController.HandleGoalZoneChange();
    }

    // -------------------------------------------------------

    // HANDLE GOAL ZONE CHANGE

    public virtual void handleGoalZoneChange() {
        if(currentGameZone == GameZones.left) {
            // move goal markers
        }
        else if(currentGameZone == GameZones.right) {
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

        Debug.Log("gamePlayerGoalZoneDelayedCo:");

        GameController.GoalZoneChange();
    }

    // GAME RUNTIME DATA
    
    public virtual void gameRuntimeTimeExtend(double extendAmount) {
        if(runtimeData != null) {
            runtimeData.AppendTime(extendAmount);
        }
    }

    // -------------------------------------------------------

    // STATS HANDLING

    public virtual void processLevelStats() {
        StartCoroutine(processLevelStatsCo());
    }

    public virtual IEnumerator processLevelStatsCo() {
         
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

        if(runtimeData.localPlayerWin) {
            GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.wins, 1f);
        }
        else {
            GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.losses, 1f);
        }

        // HANDLE RPG

        GameProfileRPGs.Current.AddCurrency(runtimeData.coins);

        // TODO by skill/RPG

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(
            currentGamePlayerController.runtimeData.totalScoreValue);

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealth(
            -UnityEngine.Random.Range(.01f, .05f),
            -UnityEngine.Random.Range(.01f, .05f));

        GameUIPanelResults.Instance.UpdateDisplay(currentGamePlayerController.runtimeData, 0f);

        yield return new WaitForEndOfFrame();
             
        GameController.EndLevelStats();
    
        yield return new WaitForEndOfFrame();
    
        GamePlayerProgress.Instance.ProcessProgressRuntimeAchievements();
        
        yield return new WaitForEndOfFrame();
    
        if(!isAdvancing) {
            GameController.AdvanceToResults();
        }
    
        //GC.Collect();
        //GC.WaitForPendingFinalizers();
        //yield return new WaitForSeconds(8f);
    }
    
    public virtual void advanceToResults() {
        isAdvancing = true;
        Invoke("advanceToResultsDelayed", .5f);
    }
    
    public virtual void advanceToResultsDelayed() {
        GameUIController.ShowUIPanelDialogResults();
        //GameAudio.StartGameLoopForLap(4);
        isAdvancing = false;
        GameController.GameRunningStateStopped();
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
    
        //Debug.Log("CheckForGameOver:isGameOver:" + isGameOver);
        //Debug.Log("CheckForGameOver:isGameRunning:" + isGameRunning);

        if(isGameRunning) {
        
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
            
            if(!isGameOver) {
            
                bool gameOverMode = false;
    
                if(AppModes.Instance.isAppModeGameArcade) {
                    if(currentGamePlayerController.runtimeData.health <= 0f
                    || runtimeData.timeExpired) {
                        gameOverMode = true;
                    }
                }
                else if(AppModes.Instance.isAppModeGameChallenge) {
                    if(currentGamePlayerController.runtimeData.health <= 0f
                    || runtimeData.timeExpired) {
                        gameOverMode = true;
                    }
                }
                else if(AppModes.Instance.isAppModeGameTraining) {

                    // TODO other modes

                    if(runtimeData.timeExpired) {
                        gameOverMode = true;
                    }
                }
    
                if(runtimeData.outOfBounds) {
                    Debug.Log("CheckForGameOver:runtimeData.outOfBounds:" + runtimeData.outOfBounds);
                    gameOverMode = true;
                    Debug.Log("CheckForGameOver:gameOverMode:" + gameOverMode);
                }
    
                if(gameOverMode) {
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

    public virtual void handleTouchInputPoint(Vector3 point) {
        //&& currentGamePlayerController.thirdPersonController.aimingDirection != Vector3.zero) {

        if(!isGameRunning) {
            return;
        }

        if(!GameUIController.CheckIfAllowedTouch(point)) {
            return;
        }

        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();

        if(currentGamePlayerController != null) {
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

            //Debug.Log("directionNormal:" + directionNormal);
            //Debug.Log("controlInputTouchOnScreen:" + controlInputTouchOnScreen);
    
            updateFingerNavigate = true;
    
            if(controlInputTouchOnScreen) {
                // If on screen controls are on don't do touch navigate just off the edge of the
                /// backer on the virtual joystick to prevent random movements.
    
                var center = Vector3.zero;//.WithX(Screen.width/2).WithY(Screen.height/2);
    
                var directionAllow = touchPos - center;
                directionAllow.Normalize();
                //var directionAllowNormal = directionAllow.normalized;
    
                //Debug.Log("directionAllowNormal:" + directionAllowNormal);
                //Debug.Log("touchPos:" + touchPos);
                //Debug.Log("pointNormalized:" + pointNormalized);
                //Debug.Log("point:" + point);
    
                if(pointNormalized.y < .2f) {
                    if(pointNormalized.x < .2f) {
                        updateFingerNavigate = false;
                    }
    
                    if(pointNormalized.x > .8f) {
                        updateFingerNavigate = false;
                    }
                }

                //Debug.Log("updateFingerNavigate:" + updateFingerNavigate);
            }
        
            if(updateFingerNavigate) {

                //Debug.Log("updateFingerNavigate::directionNormal.y" + directionNormal.y);
                //Debug.Log("updateFingerNavigate::directionNormal.x" + directionNormal.x);

                Vector3 axisInput = Vector3.zero;
                axisInput.x = directionNormal.x;
                axisInput.y = directionNormal.y;

                GameController.SendInputAxisMessage("move", axisInput);
            }
        }
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

        if(levelGrid.ContainsKey(key)) {
            filled = true;
        }

        LogUtil.Log("gameLevelSpaceFilled: key:" + key + " filled:" +  filled);

        return filled;
    }

    public virtual void setGameLevelGridSpaceFilled(Vector3 gridPos, GameLevelItemAsset asset) {

        GameController.CheckGameLevelGrid();

        string key = GameController.GetGameLevelGridKey(gridPos);

        if(levelGrid.ContainsKey(key)) {
            levelGrid[key] = asset;
            LogUtil.Log("SetLevelSpaceFilled: key:" + key);
        }
        else {
            levelGrid.Add(key, asset);
            LogUtil.Log("SetLevelSpaceFilled: key:" + key);
        }
    }

    public virtual void checkGameLevelItems() {
        if(levelItems == null) {
            levelItems = new List<GameLevelItemAsset>();
        }
    }

    public virtual void checkGameLevelGrid() {
        if(levelGrid == null) {
            levelGrid = new Dictionary<string, GameLevelItemAsset>();
        }
    }

    public virtual void clearGameLevelItems() {
        GameController.CheckGameLevelItems();

        if(levelItems != null) {
            levelItems.Clear();
        }
    }

    public virtual void clearGameLevelGrid() {
        GameController.CheckGameLevelGrid();

        if(levelGrid != null) {
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
            .WithY (UnityEngine.Random.Range(.1f, gameBounds.boundaryTopCeiling.transform.position.y/4));
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

        data.startPosition = GameController.GetRandomVectorInGameBounds();

        return GameController.GetLevelItemAssetFull(data);
    }

    public virtual void syncLevelItem(Vector3 gridPos, GameLevelItemAssetData data) {
        if(!GameController.IsGameLevelGridSpaceFilled(gridPos)) {
            //&& GameController.CheckBounds(gridPos)) {

            GameLevelItemAsset asset = GameController.GetLevelItemAssetFull(data);

            levelItems.Add(asset);

            GameController.SetGameLevelGridSpaceFilled(gridPos, asset);
        }
    }
    
    public virtual GameLevelItemAsset getLevelItemAssetFull(
        GameLevelItemAssetData data) {
        
        GameLevelItemAssetStep step = new GameLevelItemAssetStep();
        step.position.FromVector3(data.startPosition);

        step.scale.FromVector3(
            Vector3.one *
            UnityEngine.Random.Range(data.rangeScale.x, data.rangeScale.y));

        //rangeScale, 1.2f));

        step.rotation.FromVector3(
            Vector3.zero
                .WithZ(
                    UnityEngine.Random.Range(data.rangeRotation.x, data.rangeRotation.y)));

        //step.rotation.FromVector3(Vector3.zero.WithZ(UnityEngine.Random.Range(-.1f, .1f)));

        GameLevelItemAsset asset = new GameLevelItemAsset();
        if(data.countLimit == 0) {
            asset.asset_code = data.asset_code;
        }
        else {
            asset.asset_code = data.asset_code + "-" + UnityEngine.Random.Range(1, data.countLimit).ToString();
        }

        asset.physics_type = data.physicsType;
        asset.destructable = data.destructable;
        asset.reactive = data.reactive;
        asset.kinematic = data.kinematic;
        asset.gravity = data.gravity;
        asset.steps.Add(step);

        return asset;
    }
        
    public virtual List<GameLevelItemAsset> getLevelRandomized(List<GameLevelItemAsset> levelItems) {
        
        for(int i = 0; i < UnityEngine.Random.Range(3, 9); i++) {

            GameLevelItemAssetData data = new GameLevelItemAssetData();
            data.asset_code = "portal";
            data.countLimit = 5;
            data.destructable = true;
            data.gravity = true;
            data.kinematic = true;
            data.physicsType = GameLevelItemAssetPhysicsType.physicsStatic;
            //data.rangeRotation = Vector2.zero.WithX(.7f).WithY(1.2f);
            //data.rangeRotation = Vector2.zero.WithX(-.1f).WithY(.1f);

            GameLevelItemAsset asset = GameController.GetLevelItemAssetRandom(data);

            levelItems.Add(asset);
        }
        
        
        for(int i = 0; i < UnityEngine.Random.Range(5, 20); i++) {

            GameLevelItemAssetData data = new GameLevelItemAssetData();
            data.asset_code = "box";
            data.countLimit = 3;
            data.destructable = true;
            data.gravity = true;
            data.kinematic = true;
            data.physicsType = GameLevelItemAssetPhysicsType.physicsStatic;
            //data.rangeRotation = Vector2.zero.WithX(.7f).WithY(1.2f);
            //data.rangeRotation = Vector2.zero.WithX(-.1f).WithY(.1f);

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
    
    // Update is called once per frame
    public virtual void Update () {

        if(!isGameRunning) {
            return;
        }
    
        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        //bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();
                
        //if(controlInputTouchFinger) {

        if(Input.touchCount > 0) {
            foreach(Touch touch in Input.touches) {
                GameController.HandleTouchInputPoint(touch.position);
            }
        }
        else if(Input.GetMouseButton(0)) {
            GameController.HandleTouchInputPoint(Input.mousePosition);
        }
        else {
            if(GameController.CurrentGamePlayerController != null) {
				
				if(GameController.CurrentGamePlayerController.thirdPersonController != null) {
                	GameController.CurrentGamePlayerController.thirdPersonController.verticalInput = 0f;
                	GameController.CurrentGamePlayerController.thirdPersonController.horizontalInput = 0f;
                	GameController.CurrentGamePlayerController.thirdPersonController.verticalInput2 = 0f;
                	GameController.CurrentGamePlayerController.thirdPersonController.horizontalInput2 = 0f;
				}
            }
        }
        //}
        
        if(gameState == GameStateGlobal.GamePause
        || GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            return;
        }
        
        if(isGameRunning) {
            GameController.CheckForGameOver();
        }
        
        currentTimeBlockBase += Time.deltaTime;
        
        if(currentTimeBlockBase > actionIntervalBase) {
            currentTimeBlockBase = 0.0f;
        }       
        
        // debug/dev
        
        if(Application.isEditor) {
            if(Input.GetKeyDown(KeyCode.L)) {
                GameController.LoadEnemyBot1(UnityEngine.Random.Range(1.5f, 2.5f),
                UnityEngine.Random.Range(.3f, 1.3f),
                UnityEngine.Random.Range(.3f, 1.3f));
            }
            else if(Input.GetKeyDown(KeyCode.K)) {
                GameController.LoadEnemyBot1(UnityEngine.Random.Range(1.5f, 2.5f),
                UnityEngine.Random.Range(.3f, 1.3f),
                UnityEngine.Random.Range(.3f, 1.3f));
            }
            else if(Input.GetKeyDown(KeyCode.J)) {
                GameController.LoadEnemyBot1(UnityEngine.Random.Range(1.5f, 2.5f),
                UnityEngine.Random.Range(.3f, 1.3f),
                UnityEngine.Random.Range(.3f, 1.3f));
            }
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
        angles.x = Vector3.Angle( Vector3.forward, pos2 - Vector3.right * pos2.x );
    
        // Rotation to get from World +Z to pos2, rotated around World Y (degrees right? from Z axis)
        angles.y = Vector3.Angle( Vector3.forward, pos2 - Vector3.up * pos2.y );
    
        // Rotation to get from World +X to pos2, rotated around World Z (degrees up from X axis)
        angles.z = Vector3.Angle( Vector3.right, pos2 - Vector3.forward * pos2.z );
    
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