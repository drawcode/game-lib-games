using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;
using Engine.Utility;

public class BaseGameController : GameObjectTimerBehavior {

    public GamePlayerController currentGamePlayerController;

    public string gameplayType = GameplayType.gameDasher;
    public string gameplayWorldType = GameplayWorldType.gameDefault;

    internal Dictionary<string, GamePlayerController> gamePlayerControllers;
    internal Dictionary<string, GamePlayerProjectile> gamePlayerProjectiles;
    internal List<string> gameCharacterTypes = new List<string>();
    int currentCharacterTypeIndex = 0;
    //int lastCharacterTypeIndex = 0;

    internal bool initialized = false;
    internal bool allowedEditing = true;
    internal bool isAdvancing = false;
    //
    public GameStateGlobal gameState = GameStateGlobal.GameNotStarted;
    //
    public UnityEngine.Object prefabDraggableContainer;
    //
    internal Dictionary<string, GameLevelItemAsset> levelGrid = null;
    internal List<GameLevelItemAsset> levelItems = null;
    //
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
    //
    public GameObject gameZoneEndLeft;
    public GameObject gameZoneEndRight;
    //
    public GameObjectInfiniteController controllerInfinity;
    public GameObjectInfiniteContainer containerInfinity;
    //
    public GameGameRuntimeData runtimeData;
    //
    public Camera cameraGame;
    public Camera cameraGameGround;
    //
    public GameCameraView cameraView = GameCameraView.ViewSide;
    public GameRunningState gameRunningState = GameRunningState.STOPPED;
    public GameControllerType gameControllerType = GameControllerType.Iso3D;
    //
    internal float currentTimeBlockBase = 0.0f;
    internal float actionIntervalBase = 1.3f;
    //
    public float defaultLevelTime = 90;
    public string contentDisplayCode = "default";
    public bool isGameOver = false;
    public bool updateFingerNavigate = false;

    internal bool levelInitializing = false;

    // CUSTOM

    public string currentGameZone = GameZoneKeys.goal_right;

    // CAMERAS

    public List<Camera> camerasAlwaysOn;
    public GameObject cameraContainersAlwaysOn;
    //
    public List<Camera> camerasGame;
    public GameObject camerasContainerGame;
    //
    public List<Camera> camerasUI;
    public GameObject cameraContainersUI;
    //
    public List<Camera> camerasBackground;
    public GameObject cameraContainersBackground;
    //
    #if ENABLE_FEATURE_AR
    public List<Camera> camerasAR;
    public GameObject cameraContainersAR;
    #endif
    //
    public float runDirectorsDelay = 10f;

    // QUEUES

    internal List<GameObjectQueueItem> queueGameObjectItems;

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

        reset();

        initCameras();

        foreach (GamePlayerController gamePlayerController in UnityObjectUtil.FindObjects<GamePlayerController>()) {
            if (gamePlayerController.uniqueId == UniqueUtil.Instance.currentUniqueId) {
                gamePlayerController.UpdateNetworkContainer(gamePlayerController.uniqueId);
                break;
            }
        }

        initGameWorldBounds();

        loadCharacterTypes();

        GameDraggableEditor.LoadDraggableContainerObject();

        initCustomProfileCharacters();
    }

    public virtual void initCustomProfileCharacters() {

        StartCoroutine(initCustomProfileCharactersCo());
    }

    IEnumerator initCustomProfileCharactersCo() {
        yield return new WaitForEndOfFrame();

        GameCustomController.BroadcastCustomCharacterProfileCodeSync();
        GameCustomController.BroadcastCustomSync();
    }

    public virtual void OnEnable() {

        Messenger<string>.AddListener(GameMessages.gameLevelStart, OnGameLevelStart);

        Messenger<string>.AddListener(GameMessages.gameInitLevelStart, OnGameInitLevelStart);

        Messenger.AddListener(GameMessages.gameLevelPlayerReady, OnGameLevelPlayerReady);

        Gameverses.GameMessenger<string>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameAIDirectorData>.AddListener(
            GameAIDirectorMessages.gameAIDirectorSpawnActor,
            OnGameAIDirectorData);

        Messenger<GameItemData>.AddListener(
            GameItemDirectorMessages.gameItemDirectorSpawnItem,
            OnGameItemDirectorData);

        Messenger.AddListener(
            BaseGameProfileMessages.ProfileShouldBeSaved,
            OnProfileShouldBeSavedEventHandler);

        Messenger.AddListener(
            GameDraggableEditorMessages.GameLevelItemsLoaded,
            OnGameLevelItemsLoaded);

        Messenger<InputSystemSwipeDirection, Vector3, float>.AddListener(
            InputSystemEvents.inputSwipe, OnInputSwipe);

    }

    public virtual void OnDisable() {

        Messenger<string>.RemoveListener(GameMessages.gameLevelStart, OnGameLevelStart);

        Messenger<string>.RemoveListener(GameMessages.gameInitLevelStart, OnGameInitLevelStart);

        Messenger.RemoveListener(GameMessages.gameLevelPlayerReady, OnGameLevelPlayerReady);

        Gameverses.GameMessenger<string>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameAIDirectorData>.RemoveListener(
            GameAIDirectorMessages.gameAIDirectorSpawnActor,
            OnGameAIDirectorData);

        Messenger<GameItemData>.RemoveListener(
            GameItemDirectorMessages.gameItemDirectorSpawnItem,
            OnGameItemDirectorData);

        Messenger.RemoveListener(
            BaseGameProfileMessages.ProfileShouldBeSaved,
            OnProfileShouldBeSavedEventHandler);

        Messenger.RemoveListener(
            GameDraggableEditorMessages.GameLevelItemsLoaded,
            OnGameLevelItemsLoaded);

        Messenger<InputSystemSwipeDirection, Vector3, float>.RemoveListener(
            InputSystemEvents.inputSwipe, OnInputSwipe);
    }

    //


    public virtual void OnGameLevelStart(string levelCode) {

    }

    public virtual void OnGameInitLevelStart(string levelCode) {

    }

    public virtual void OnGameLevelPlayerReady() {

    }

    // ---------------------------------------------------------------------
    // GAMEPLAY CONTROLLER TYPES

    public bool isGameplayType(string gameplayTypeTo) {
        return gameplayType == gameplayTypeTo;
    }

    public void gameplayTypeSet(string gameplayTypeTo) {
        gameplayType = gameplayTypeTo;
    }

    public string gameplayTypeGet() {
        return gameplayType;
    }

    public static bool IsGameplayType(string gameplayTypeTo) {
        if (GameController.isInst) {
            return GameController.Instance.isGameplayType(gameplayTypeTo);
        }
        return false;
    }

    public static void GameplayTypeSet(string gameplayTypeTo) {
        if (GameController.isInst) {
            GameController.Instance.gameplayTypeSet(gameplayTypeTo);
        }
    }

    public static string GameplayTypeGet() {
        if (GameController.isInst) {
            return GameController.Instance.gameplayTypeGet();
        }
        return null;
    }

    // GAMEPLAY TYPE HELPERS
    // TODO leaf out types

    // RUNNER

    public static bool IsGameplayTypeRunner() {
        if (GameController.isInst) {
            return GameController.Instance.isGameplayTypeRunner();
        }
        return false;
    }

    public bool isGameplayTypeRunner() {
        return gameplayType == GameplayType.gameRunner;
    }

    // DASHER

    public static bool IsGameplayTypeDasher() {
        if (GameController.isInst) {
            return GameController.Instance.isGameplayTypeDasher();
        }
        return false;
    }

    public bool isGameplayTypeDasher() {
        return gameplayType == GameplayType.gameDasher;
    }


    // ---------------------------------------------------------------------
    // GAMEPLAY WORLD TYPES

    public bool isGameplayWorldType(string gameplayWorldTypeTo) {
        return gameplayWorldType == gameplayWorldTypeTo;
    }

    public void gameplayWorldTypeSet(string gameplayWorldTypeTo) {
        gameplayWorldType = gameplayWorldTypeTo;
    }

    public string gameplayWorldTypeGet() {
        return gameplayWorldType;
    }

    public static bool IsGameplayWorldType(string gameplayWorldTypeTo) {
        if (GameController.isInst) {
            return GameController.Instance.isGameplayWorldType(gameplayWorldTypeTo);
        }
        return false;
    }

    public static void GameplayWorldTypeSet(string gameplayWorldTypeTo) {
        if (GameController.isInst) {
            GameController.Instance.gameplayWorldTypeSet(gameplayWorldTypeTo);
        }
    }

    public static string GameplayWorldTypeGet() {
        if (GameController.isInst) {
            return GameController.Instance.gameplayWorldTypeGet();
        }
        return null;
    }

    // GAMEPLAY TYPE HELPERS
    // TODO leaf out types
    // TODO enums possible instead of stringly but settings
    //  come from server as strings so conversion process needed

    // RUNNER

    public static bool IsGameplayWorldTypeDefault() {
        if (GameController.isInst) {
            return GameController.Instance.isGameplayWorldTypeDefault();
        }
        return false;
    }

    public bool isGameplayWorldTypeDefault() {
        return gameplayWorldType == GameplayWorldType.gameDefault;
    }

    // DASHER

    public static bool IsGameplayWorldTypeStationary() {
        if (GameController.isInst) {
            return GameController.Instance.isGameplayWorldTypeStationary();
        }
        return false;
    }

    public bool isGameplayWorldTypeStationary() {
        return gameplayWorldType == GameplayWorldType.gameStationary;
    }

    // ---------------------------------------------------------------------

    // PROPERTIES

    public int characterActorsCount {
        get {
            return levelActorsContainerObject.transform.childCount;
        }
    }

    public int characterActorEnemyCount {
        get {

            int countEnemies = 0;

            foreach (GamePlayerController gamePlayerController in
                    levelActorsContainerObject.GetComponentsInChildren<GamePlayerController>()) {

                if (gamePlayerController.IsAgentControlled) {
                    countEnemies += 1;
                }
            }

            return countEnemies;
        }
    }

    public int characterActorSidekickCount {
        get {

            int countEnemies = 0;

            foreach (GamePlayerController gamePlayerController in
                    levelActorsContainerObject.GetComponentsInChildren<GamePlayerController>()) {

                if (gamePlayerController.IsSidekickControlled) {
                    countEnemies += 1;
                }
            }

            return countEnemies;
        }
    }

    public int itemsCount {
        get {
            int countItems = 0;

            //foreach (GamePlayerController gamePlayerController in 
            //    levelActorsContainerObject.GetComponentsInChildren<GamePlayerController>()) {

            //    if (gamePlayerController.IsSidekickControlled) {
            //        countItems += 1;
            //    }
            //}

            return countItems;
        }
    }

    public int itemWeaponsCount {
        get {
            int countWeapons = 0;

            //foreach (GamePlayerController gamePlayerController in 
            //    levelActorsContainerObject.GetComponentsInChildren<GamePlayerController>()) {
            //
            //    if (gamePlayerController.IsSidekickControlled) {
            //        countItems += 1;
            //    }
            //}

            return countWeapons;
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

    internal virtual void OnProfileShouldBeSavedEventHandler() {
        if (!GameConfigs.isGameRunning) {
            GameState.SaveProfile();
        }
    }

    internal void OnGameLevelItemsLoaded() {

        loadLevelActions(1f);
    }

    internal virtual void OnEditStateHandler(GameDraggableEditEnum state) {

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

    internal virtual void OnNetworkPlayerContainerAdded(string uid) {

        // Look for object by that uuid, if not create it

        //LogUtil.Log("OnNetworkPlayerContainerAdded:uid:", uid);

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
                    //LogUtil.Log("OnNetworkPlayerContainerAdded:Updating character:" + uid);
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
                    //LogUtil.Log("OnNetworkPlayerContainerAdded:Creating character:" + uid);
                    //LogUtil.Log("OnNetworkPlayerContainerAdded:playerControllerOther.uniqueId:" + playerControllerOther.uniqueId);
                }
            }
        }
    }

    internal virtual void OnGameAIDirectorData(GameAIDirectorData actor) {
        loadActor(actor.code, actor.type);
    }

    internal virtual void OnGameItemDirectorData(GameItemData item) {

        loadItem(item.code);
    }

    // ---------------------------------------------------------------------

    // GAMEPLAYER CONTROLLER

    public virtual GamePlayerController currentPlayerController {
        get {
            return getCurrentController();
        }
    }

    public virtual GamePlayerController getCurrentController() {
        //if (GameController.Instance.currentGamePlayerController != null) {
        //    return GameController.Instance.currentGamePlayerController;
        //}
        return currentGamePlayerController;
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

            gamePlayerController = getGamePlayerController(go);

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

            gamePlayerController = getGamePlayerControllerParent(go);

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

            gamePlayerController = getGamePlayerController(go);

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

            gamePlayerController = getGamePlayerControllerParent(go);

            if (gamePlayerController != null) {

                //LogUtil.Log("GameObjectChoice:gamePlayerController:" + gamePlayerController.name);

                if (!onlyPlayerControlled || gamePlayerController.IsPlayerControlled) {
                    return true;
                }
            }
        }

        return false;
    }

    // ----------------------------------------------------------------------

    // SCORING

    public virtual void gamePlayerScores(double val) {
        if (currentPlayerController != null) {
            currentPlayerController.ProgressScores(val);
        }
    }

    // ----------------------------------------------------------------------

    // ATTACK

    public virtual void gamePlayerAttack() {
        if (currentPlayerController != null) {
            currentPlayerController.SendAttack();
        }
    }

    public virtual void gamePlayerAttackAlt() {
        if (currentPlayerController != null) {
            currentPlayerController.SendAttackAlt();
        }
    }

    public virtual void gamePlayerAttackRight() {
        if (currentPlayerController != null) {
            currentPlayerController.SendAttackRight();
        }
    }

    public virtual void gamePlayerAttackLeft() {
        if (currentPlayerController != null) {
            currentPlayerController.SendAttackLeft();
        }
    }

    // ----------------------------------------------------------------------

    // DEFEND

    public virtual void gamePlayerDefend() {
        if (currentPlayerController != null) {
            currentPlayerController.SendDefend();
        }
    }

    public virtual void gamePlayerDefendAlt() {
        if (currentPlayerController != null) {
            currentPlayerController.SendDefendAlt();
        }
    }

    public virtual void gamePlayerDefendRight() {
        if (currentPlayerController != null) {
            currentPlayerController.SendDefendRight();
        }
    }

    public virtual void gamePlayerDefendLeft() {
        if (currentPlayerController != null) {
            currentPlayerController.SendDefendLeft();
        }
    }

    // ----------------------------------------------------------------------

    // JUMP

    public virtual void gamePlayerJump() {
        if (currentPlayerController != null) {
            currentPlayerController.InputJump();
        }
    }

    // ----------------------------------------------------------------------

    // MOVE

    public virtual void gamePlayerMove(Vector3 amount) {
        if (currentPlayerController != null) {
            currentPlayerController.InputMove(amount);
        }
    }

    // ----------------------------------------------------------------------

    // STRAFE

    public virtual void gamePlayerStrafe(Vector3 amount) {
        if (currentPlayerController != null) {
            currentPlayerController.InputStrafe(amount);
        }
    }

    // ----------------------------------------------------------------------

    // STRAFE

    public virtual void gamePlayerMove(Vector3 amount, Vector3 rangeStart, Vector3 rangeEnd, bool append = true) {
        if (currentPlayerController != null) {
            currentPlayerController.InputMove(amount, rangeStart, rangeEnd, append);
        }
    }


    // ----------------------------------------------------------------------

    // STRAFE

    public virtual void gamePlayerSlide(Vector3 amount) {
        if (currentPlayerController != null) {
            currentPlayerController.InputSlide(amount);
        }
    }

    // ----------------------------------------------------------------------

    // SPEED

    public virtual void gamePlayerSetSpeed(float amount) {
        if (currentPlayerController != null) {
            currentPlayerController.GamePlayerMoveSpeedSet(amount);
        }
    }

    // ----------------------------------------------------------------------

    // USE

    public virtual void gamePlayerUse() {
        if (currentPlayerController != null) {
            currentPlayerController.InputUse();
        }
    }

    // ----------------------------------------------------------------------

    // MOUNT

    public virtual void gamePlayerMount() {
        if (currentPlayerController != null) {
            currentPlayerController.InputMount();
        }
    }

    // ----------------------------------------------------------------------

    // SKILL

    public virtual void gamePlayerSkill() {
        if (currentPlayerController != null) {
            currentPlayerController.InputSkill();
        }
    }

    // ----------------------------------------------------------------------

    // MAGIC

    public virtual void gamePlayerMagic() {
        if (currentPlayerController != null) {
            currentPlayerController.InputMagic();
        }
    }

    // ----------------------------------------------------------------------

    // ASSET CONTEXT LOADING

    public virtual string gameItemCodeContextGet(string codeItem) {

        return codeItem;
    }

    // ----------------------------------------------------------------------

    // ZONES

    public virtual GameZone getGameZone(GameObject go) {
        if (go != null) {
            return go.GetComponent<GameZone>();
        }
        return null;
    }

    public virtual GameZoneGoal getGoalZone(GameObject go) {
        if (go != null) {
            return go.GetComponent<GameZoneGoal>();
        }
        return null;
    }

    public virtual GameZoneBad getBadZone(GameObject go) {
        if (go != null) {
            return go.GetComponent<GameZoneBad>();
        }
        return null;
    }

    public virtual void changeGameZone(string zone) {

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

    public virtual void loadLevelActions(float delay) {
        StartCoroutine(loadLevelActionsCo(delay));
    }

    IEnumerator loadLevelActionsCo(float delay) {
        yield return new WaitForSeconds(delay);
        loadLevelActions();
    }

    public virtual void loadLevelActions() {
        foreach (GameZoneActionAsset gameZoneActionAsset in
                levelItemsContainerObject.GetList<GameZoneActionAsset>()) {

            if (gameZoneActionAsset.gameZoneType == GameZoneKeys.action_none) {
                // Make it a type of needed action or none. 
                // Update placeholder actions to actual actions of default

            }
        }
    }

    public virtual void loadLevelAssets() {
        loadLevelAssets(GameLevels.Current.code);
    }

    public virtual void loadLevelAssets(string code) {

        // LogUtil.Log("GAME START FLOW: STEP #10: loadLevelAssets: code:" + code);

        GameDraggableEditor.levelItemsContainerObject = levelItemsContainerObject;

        //GameLevelItems.Current.code = code;

        // Clear items from LevelContainer
        reset();

        Debug.Log("loadLevelAssets:" + " code:" + code);

        // Change data codes
        GameLevels.Instance.ChangeCurrentAbsolute(code);
        GameLevelItems.Instance.ChangeCurrentAbsolute(code);

        // Prepare game level items for this mode
        loadLevelItems();

        // Load in the level assets
        GameDraggableEditor.LoadLevelItems();

        handlePostLoadLevelAssetsGameplayType();

        // Find actions and make them current actions or none

        //List<AppContentCollectItem> actionItemsAsset = new List<AppContentCollectItem>();

        //foreach(AppContentCollectItem collectItem in 
        //        AppContentCollects.Current.GetItemsData()) {
        //    if(collectItem.IsCodeActionAssetAttack()) {
        //        actionCodes.Add(collectItem);
        //    }
        //}

        /*
        foreach(GameZoneActionAsset gameZoneActionAsset in 
                levelItemsContainerObject.GetList<GameZoneActionAsset>()) {

            if(gameZoneActionAsset.gameZoneType == GameZoneKeys.action_none) {
                // Make it a type of needed action or none.

                gameZoneActionAsset.Load(
                    GameZoneKeys.action_attack, 
                    GameZoneActions.action_attack,
                    "level-building-" + UnityEngine.Random.Range(1,10),
                    "platform-large-1");
            }
        }
        */
    }

    public virtual void loadLevel(string code) {

        //LogUtil.Log("GAME START FLOW: STEP #6: loadLevel: code:" + code);

        // Load the game levelitems for the game level code

        ////GameController.StartGame(code);

        prepareGame(code);

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
                        = getLevelRandomizedGrid();
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

                    GameLevelItems.Current.level_items
                        = getLevelRandomizedGrid(GameLevelGridData.GetModeTypeChoice(4));
                    updated = true;
                }

            }
            else if (AppModeTypes.Instance.isAppModeTypeGameCollection) {

                LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameCollection:"
                + AppModeTypes.Instance.isAppModeTypeGameCollection);

                // LOAD COLLECTION GAME LEVEL ITEMS
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
                        = getLevelRandomizedGrid();
                    updated = true;
                }
            }
            //}
        }

        if (!updated) {
            GameLevelItems.Current.level_items
                = getLevelRandomizedGrid();
        }
    }

    public virtual void saveCurrentLevel() {
        GameDraggableEditor.SaveCurrentLevel(
            levelItemsContainerObject);
    }

    public virtual void playGame() {

        if (!levelInitializing) {

            //AdNetworks.ShowFullscreenAd();

            //if(string.IsNullOrEmpty(GameLevels.Current.code)) {
            //    GameLevels.Instance.ChangeCurrentAbsolute("1-1");
            //}

            loadStartLevel();
        }
    }

    public virtual void initLevel(string levelCode) {
        StartCoroutine(initLevelCo(levelCode));
    }

    IEnumerator initLevelCo(string levelCode) {

        //LogUtil.Log("GAME START FLOW: STEP #5: startLevelCo: levelCode:" + levelCode);
        levelInitializing = true;

        //resetRuntimeData();

        if (currentGamePlayerController != null) {
            currentGamePlayerController.PlayerEffectWarpFadeIn();
        }

        //return;

        UIPanelOverlayPrepare.ShowDefault();

        GameUIPanelOverlays.Instance.ShowOverlayWhite();

        yield return new WaitForSeconds(1.1f);

        // PRELOAD/CACHE/POOL CONTROLLERS

        //yield return StartCoroutine(GameAIController.Instance.preloadCo());

        //yield return StartCoroutine(GameItemController.Instance.preloadCo());

        GameHUD.Instance.ResetIndicators();

        // TODO load customizations
        loadLevel(levelCode);

        // TODO load anim
    }

    public virtual void preloadLevelAssetPreset(GameDataAssetPreset assetDataItem) {
        if (Application.isEditor) {
            return;
        }

        StartCoroutine(preloadLevelAssetPresetCo(assetDataItem));
    }

    IEnumerator preloadLevelAssetPresetCo(GameDataAssetPreset assetDataItem) {

        //yield return new WaitForEndOfFrame();

        if (Application.isEditor) {
            yield break;
        }

        int minAssetLimit = (int)assetDataItem.min;
        int maxAssetLimit = (int)assetDataItem.max;

        GamePreset assetPreset = GamePresets.Instance.GetById(assetDataItem.code);

        if (assetPreset != null) {

            foreach (GamePresetItem presetItem in assetPreset.data.items) {

                //Debug.Log("preloadLevelAssetPreset: " + presetItem.code);

                for (int i = 0; i < 5; i++) {

                    //yield return new WaitForEndOfFrame();

                    GameObject go = AppContentAssets.LoadAssetLevelAssets(presetItem.code);

                    //yield return new WaitForEndOfFrame();

                    go.DestroyGameObject();
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }

    internal virtual void initLevelFinish(string levelCode) {
        StartCoroutine(initLevelFinishCo(levelCode));
    }

    IEnumerator initLevelFinishCo(string levelCode) {
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
        startLevel(GameLevels.Current.code);
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
        saveGameStates();

        LogUtil.Log("changeGameStates:app_content_state:AFTER:" + app_content_state);

    }
//AppContentStates.Instance.ChangeState(AppContentStateMeta.appContentStateGameArcade);

    public virtual void changeCharacterModel(string characterCode) {
        currentPlayerController.ChangeCharacter(characterCode);
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

        changeCharacterModel(characterCode);
    }

    public virtual void loadStartLevel() {
        loadStartLevel(GameLevels.Current.code);// GameConfigs.defaultGameLevelCode);
    }

    public virtual void loadStartLevel(string levelCode) {

        GameDraggableEditor.ChangeStateEditing(GameDraggableEditEnum.StateNotEditing);
        startLevel(levelCode);
    }

    public virtual void loadActor(string characterCode, string displayType) {

        GameActorDataItem actorDataItem = new GameActorDataItem();
        actorDataItem.code = characterCode;
        actorDataItem.type = BaseDataObjectKeys.character;
        actorDataItem.data_type = GameSpawnType.zonedType;
        actorDataItem.display_type = displayType;

        //for (int i = 0; i < actorDataItem.currentSpawnAmount; i++) {
        loadActor(actorDataItem);
        //}
    }

    public virtual void loadActor(
        string characterCode,
        string characterType,
        string spawnType,
        string displayType,
        Vector3 pos,
        Quaternion rot,
        bool overrideLoading = false) {

        GameActorDataItem actorDataItem = new GameActorDataItem();
        actorDataItem.code = characterCode;
        actorDataItem.type = characterType;
        actorDataItem.data_type = spawnType;
        actorDataItem.display_type = displayType;
        actorDataItem.position_data.FromVector3(pos);
        actorDataItem.rotation_data.FromVector3(rot.eulerAngles);
        actorDataItem.overrideLoading = overrideLoading;

        //for (int i = 0; i < actor.currentSpawnAmount; i++) {
        loadActor(actorDataItem);
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
        loadItem(data);
        //} 
    }

    public virtual void loadItem(string itemCode) {

        GameItemData data = new GameItemData();
        data.code = itemCode;

        // DEFAULT ITEM LOADING

        //for (int i = 0; i < item.currentSpawnAmount; i++) {
        loadItem(data);
        //}
    }

    public virtual void loadItem(GameItemData data) {
        StartCoroutine(loadItemCo(data));
    }

    public virtual Vector3 currentPlayerPosition {
        get {
            return getCurrentPlayerPosition();
        }
    }

    public virtual Vector3 getCurrentPlayerPosition() {
        Vector3 currentPlayerPosition = Vector3.zero;
        if (currentPlayerController != null) {
            if (currentPlayerController.gameObject != null) {
                currentPlayerPosition = currentPlayerController.gameObject.transform.position;
            }
        }
        return currentPlayerPosition;
    }

    public virtual Vector3 getRandomSpawnLocation() {
        Vector3 spawnLocation = Vector3.zero;
        Vector3 currentPlayerPosition = getCurrentPlayerPosition();

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

    IEnumerator loadActorCo(GameActorDataItem data) {

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

            if (currentGameZone == GameZoneKeys.goal_right) {
                spawnCode = rightMiddle;
            }
            else if (currentGameZone == GameZoneKeys.goal_left) {
                spawnCode = leftMiddle;
            }

            //LogUtil.Log("spawnCode:" + spawnCode);

            GamePlayerSpawn spawn = GameAIController.GetSpawn(spawnCode);

            if (spawn != null) {
                spawnLocation = spawn.gameObject.transform.position;
            }
            else {

                // get random
                if (currentGameZone == GameZoneKeys.goal_right) {
                    spawnLocation = Vector3.zero.WithX(80f).WithZ(currentPlayerPosition.z);// UnityEngine.Random.Range(-20, 20));
                }
                else if (currentGameZone == GameZoneKeys.goal_left) {
                    spawnLocation = Vector3.zero.WithX(-80f).WithZ(currentPlayerPosition.z);// UnityEngine.Random.Range(-20, 20));
                }
            }

            if (spawnLocation == Vector3.zero) {
                spawnLocation = getRandomSpawnLocation();
            }
        }
        else if (data.data_type == GameSpawnType.randomType) {
            // get random
            spawnLocation = getRandomSpawnLocation();
        }
        else if (data.data_type == GameSpawnType.explicitType) {
            // get random
            spawnLocation = data.position_data.GetVector3();
        }
        else if (data.data_type == GameSpawnType.pointsType) {
            // FIND spawn location
        }

        if (spawnLocation == Vector3.zero) {
            spawnLocation = getRandomSpawnLocation();
        }

        if (data.data_type == GameSpawnType.centeredType) {
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
                = characterObject.Get<GamePlayerController>();

            if (characterGamePlayerController != null) {

                if (data.display_type == GameActorType.player) {
                    characterGamePlayerController.Init(
                        GamePlayerControllerState.ControllerPlayer,
                        GamePlayerContextState.ContextInput,
                        data.overrideLoading);

                    characterGamePlayerController.attackRange = 12f;
                }
                else if (data.display_type == GameActorType.sidekick) {
                    characterGamePlayerController.Init(
                        GamePlayerControllerState.ControllerSidekick,
                        GamePlayerContextState.ContextFollowAgent,
                        data.overrideLoading);

                    characterGamePlayerController.attackRange = 12f;
                }
                else { //if (data.display_type == GameActorType.enemy) {                        
                    //characterGamePlayerController.currentTarget = getCurrentPlayerController.gameObject.transform;
                    //characterGamePlayerController.ChangeContextState(GamePlayerContextState.ContextFollowAgentAttack);
                    //characterGamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerAgent);
                    characterGamePlayerController.Init(
                        GamePlayerControllerState.ControllerAgent,
                        GamePlayerContextState.ContextFollowAgentAttack,
                        data.overrideLoading);

                    characterGamePlayerController.attackRange = 12f;
                }

                characterGamePlayerController.LoadCharacter(data.code);

                characterGamePlayerController.transform.localScale
                    = data.scale_data.GetVector3();

            }
        }

        //loadingCharacterContainer = false;
    }

    IEnumerator loadItemCo(GameItemData data) {

        if (data == null) {
            yield break;
        }

        GameItem item = GameItems.Instance.GetById(data.code);

        if (item == null) {
            //Debug.Log("loadItemCo:" + "Item not found" + " code:" + data.code);
            yield break;
        }

        string path = Path.Combine(
                          ContentPaths.appCacheVersionSharedPrefabLevelItems,
                          item.data.GetModel().code);

        GameObject prefabObject = PrefabsPool.PoolPrefab(path);
        Vector3 spawnLocation = Vector3.zero;

        //Debug.Log("loadItemCo:" + " data.json:" + data.ToJson());

        if (data.data_type == GameSpawnType.zonedType) {

            // get left/right spawn location
            //string leftMiddle = "left-middle";
            //string rightMiddle = "right-middle";
            //string spawnCode = rightMiddle;
            if (currentGameZone == GameZoneKeys.goal_right) {
                //spawnCode = rightMiddle;
            }
            else if (currentGameZone == GameZoneKeys.goal_left) {
                //spawnCode = leftMiddle;
            }

            // LogUtil.Log("spawnCode:" + spawnCode);

            //GamePlayerSpawn spawn = GameAIController.GetSpawn(spawnCode);
            //if(spawn != null) {
            //    spawnLocation = spawn.gameObject.transform.position;
            //}
            //else {

            // get random
            if (currentGameZone == GameZoneKeys.goal_right) {

                spawnLocation = Vector3.zero
                    .WithX(UnityEngine.Random.Range(
                    0, gameBounds.boundaryTopRight.transform.position.x))
                        .WithY(50f)
                        .WithZ(UnityEngine.Random.Range(
                    boundaryBottomLeft.transform.position.z,
                    boundaryTopLeft.transform.position.z));
            }
            else if (currentGameZone == GameZoneKeys.goal_left) {

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
            spawnLocation = getRandomSpawnLocation().WithY(50f);
        }

        if (spawnLocation == Vector3.zero) {
            spawnLocation = getRandomSpawnLocation().WithY(50f);
        }

        if (prefabObject == null) {
            yield break;
        }

        GameObject spawnObj = GameObjectHelper.CreateGameObject(
                                  prefabObject, spawnLocation, Quaternion.identity,
                                  GameConfigs.usePooledItems) as GameObject;

        if (spawnObj != null && levelActorsContainerObject != null) {
            spawnObj.transform.parent = levelActorsContainerObject.transform;
            GamePlayerIndicator.AddIndicator(spawnObj, item.code);
        }
    }

    // ---------------------------------------------------------------------
    // GAME OBJECT LEVEL START LOAD

    internal virtual void queueGameObjectTypeData(
        string type, string code, string data_type, string display_type, Vector3 pos, Quaternion rot) {

        GameObjectQueueItem queueItem = new GameObjectQueueItem();
        queueItem.type = type;
        queueItem.code = code;
        queueItem.data_type = data_type;
        queueItem.display_type = display_type;
        queueItem.pos = pos;
        queueItem.rot = rot;

        queueGameObjectTypeData(queueItem);
    }

    internal virtual void checkQueueGameObjectTypeData() {

        if (queueGameObjectItems == null) {
            queueGameObjectItems = new List<GameObjectQueueItem>();
        }
    }

    internal virtual void queueGameObjectTypeData(GameObjectQueueItem queueItem) {

        checkQueueGameObjectTypeData();

        queueGameObjectItems.Add(queueItem);
    }

    internal virtual void clearQueueGameObjectTypeData() {

        checkQueueGameObjectTypeData();

        queueGameObjectItems.Clear();
    }

    internal virtual void processQueueGameObjectTypeData() {

        if (queueGameObjectItems != null) {

            foreach (GameObjectQueueItem queueItem in queueGameObjectItems) {

                if (queueItem.type == BaseDataObjectKeys.character) {

                    loadActor(
                        queueItem.code,
                        queueItem.type,
                        queueItem.data_type,
                        queueItem.display_type,
                        queueItem.pos,
                        queueItem.rot,
                        true);
                }
            }

        }

        clearQueueGameObjectTypeData();
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

        resetRuntimeData();

        resetLevel();
    }

    //-------------------------------------------------------------------------

    public static void ResetLevelAssetCacheItems(bool removeCached = false) {
        if (GameController.isInst) {
            GameController.Instance.resetLevelAssetCacheItems(removeCached);
        }
    }

    public virtual void resetLevelAssetCacheItems(bool removeCached = false) {
        resetLevelAssetCacheItems(levelItemsContainerObject, removeCached);
    }

    public virtual void resetLevelAssetCacheItems(GameObject go, bool removeCached = false) {

        if (go == null) {
            return;
        }

        bool cached = resetLevelAssetCacheItem(go, removeCached);

        if (!cached || removeCached) {

            foreach (GameObjectInfinitePartItem partItem in
                go.GetList<GameObjectInfinitePartItem>()) {

                partItem.DestroyItems(removeCached);
            }

            foreach (GameObjectInfinitePart part in
                go.GetList<GameObjectInfinitePart>()) {

                part.DestroyItems(removeCached);
            }

            foreach (GameObjectInfiniteContainer container in
                go.GetList<GameObjectInfiniteContainer>()) {

                container.DestroyItems(removeCached);
            }
        }
    }

    //-------------------------------------------------------------------------

    public static bool ResetLevelAssetCacheItem(GameObject go, bool removeCached = false) {
        if (GameController.isInst) {
            return GameController.Instance.resetLevelAssetCacheItem(go, removeCached);
        }

        return false;
    }

    public virtual bool resetLevelAssetCacheItem(GameObject go, bool removeCached = false) {

        bool cached = false;

        if (go == null) {
            return cached;
        }
        
        foreach (GameObjectInfiniteAssetCache item in
            go.GetList<GameObjectInfiniteAssetCache>()) {

            if (removeCached) {
                item.gameObject.Remove<GameObjectInfiniteAssetCache>();
            }
            else {
                cached = true;
                item.gameObject.DestroyGameObject();
            }
        }

        if (!cached || removeCached) {

            foreach (GameObjectInfiniteAssetItem item in
                go.GetList<GameObjectInfiniteAssetItem>()) {

                item.gameObject.Remove<GameObjectInfiniteAssetItem>();

                item.gameObject.DestroyGameObject();
            }

            foreach (GameObjectInfiniteAsset item in
                go.GetList<GameObjectInfiniteAsset>()) {

                item.gameObject.Remove<GameObjectInfiniteAsset>();

                item.gameObject.DestroyGameObject();
            }
        }

        return cached;
    }

    //-------------------------------------------------------------------------

    public virtual void resetLevel() {

        stopDirectors();

        GameUIController.HideGameCanvas();

        GameController.ResetLevelAssetCacheItems(true);

        GameDraggableEditor.ClearLevelItems(levelItemsContainerObject);
        GameDraggableEditor.ResetCurrentGrabbedObject();
        GameDraggableEditor.HideAllEditDialogs();

        ObjectPoolKeyedManager.clearPooled();
    }

    public virtual void resetRuntimeData() {

        resetCurrentGamePlayer();
        resetLevelActors();

        runtimeData = new GameGameRuntimeData();
        runtimeData.ResetTime(defaultLevelTime);
        isGameOver = false;
    }

    public virtual void handlePostLoadLevelAssetsGameplayType() {

        handleGametypeReset();

        handleGametypeInit();

        if (isGameplayWorldTypeStationary()) {

            if (containerInfinity != null) {
                //containerInfinity.Reset();
            }
        }
    }

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
        cycleCharacterTypes(currentCharacterTypeIndex);
    }

    public virtual void cycleCharacterTypesPrevious() {
        currentCharacterTypeIndex--;
        cycleCharacterTypes(currentCharacterTypeIndex);
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
        reset();
        prepareGame(GameLevels.Current.code);
    }

    public virtual void startGame(string levelCode) {
        //GameController.Reset();
        changeGameState(GameStateGlobal.GameStarted);
    }

    public virtual void stopGame() {
        processLevelStats();
        stopDirectors();
    }

    public virtual void prepareGame(string levelCode) {

        //GameController.Reset();

        Debug.Log("prepareGame:" + " levelCode:" + levelCode);

        loadLevelAssets(levelCode);

        GameHUD.Instance.SetLevelInit(GameLevels.Current);

        GameHUD.Instance.AnimateIn();

        //return;

        changeGameState(GameStateGlobal.GamePrepare);
    }

    public virtual void gameContentDisplay(string contentDisplayCodeTo) {
        contentDisplayCode = contentDisplayCodeTo;
        changeGameState(GameStateGlobal.GameContentDisplay);
    }

    public virtual void pauseGame() {
        changeGameState(GameStateGlobal.GamePause);
    }

    public virtual void resumeGame() {
        changeGameState(GameStateGlobal.GameResume);
    }

    public virtual void quitGame() {
        reset();
        changeGameState(GameStateGlobal.GameQuit);
    }

    public virtual void resultsGameDelayed() {
        Invoke("resultsGame", .5f);
    }

    public virtual void resultsGame() {
        changeGameState(GameStateGlobal.GameResults);
    }

    public virtual void togglePauseGame() {
        if (gameState == GameStateGlobal.GamePause) {
            resumeGame();
        }
        else {
            pauseGame();
        }
    }

    // -------------------------------------------------------
    // DIRECTORS

    public virtual void runDirectorsDelayed(float delay) {
        StartCoroutine(runDirectorsDelayedCo(delay));
    }

    IEnumerator runDirectorsDelayedCo(float delay) {
        yield return new WaitForSeconds(delay);
        runDirectors();

        //getCurrentPlayerController.GamePlayerModelHolderEaseIn();
        //getCurrentPlayerController.PlayerEffectWarpFadeOut();
    }

    public virtual void runDirectors() {
        updateDirectors(true);
    }

    public virtual void stopDirectors() {
        updateDirectors(false);
    }

    public virtual void updateDirectors(bool run) {

        bool runAI = run;
        bool runItem = run;

        if (run) {

            List<GameDataDirector> directorsLevels = GameLevels.Current.data.directors;
            List<GameDataDirector> directors = GameWorlds.Current.data.directors;

            if (directors == null) {
                directors = new List<GameDataDirector>();
            }

            if (directorsLevels != null && directors.Count > 0) {
                directors.AddRange(directorsLevels);
            }

            if (directors != null) {
                foreach (GameDataDirector director in directors) {
                    if (director.code == GameDataDirectorType.enemy
                       || director.code == GameDataDirectorType.sidekick) {
                        runAI = director.run;
                        GameAIController.UpdateDirector(director);
                    }
                    else if (director.code == GameDataDirectorType.item
                            || director.code == GameDataDirectorType.weapon) {
                        runItem = director.run;
                        GameItemController.UpdateDirector(director);
                    }
                }
            }
        }

        updateDirectorAI(runAI);
        updateDirectorItem(runItem);
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
        gameState = GameStateGlobal.GameNotStarted;
    }

    // PAUSED

    public virtual void gameRunningStatePause() {
        gameRunningStatePauseDelayed(1);
    }

    public virtual void gameRunningStatePauseDelayed(float delay) {
        StartCoroutine(gameRunningStatePauseDelayedCo(delay));
    }

    IEnumerator gameRunningStatePauseDelayedCo(float delay) {

        yield return new WaitForSeconds(delay);

        gameRunningStatePause(0f);
    }

    public virtual void gameRunningStatePause(float timeScale) {
        gameSetTimeScale(timeScale);
        gameRunningState = GameRunningState.PAUSED;
        gameState = GameStateGlobal.GamePause;
    }

    // RUN

    public virtual void gameRunningStateRun() {
        gameRunningStateRun(1f);
    }

    public virtual void gameRunningStateRun(float timeScale) {
        gameSetTimeScale(timeScale);
        gameRunningState = GameRunningState.RUNNING;
        gameState = GameStateGlobal.GameStarted;
    }

    // CONTENT

    public virtual void gameRunningStateContent() {
        gameRunningStateContent(1f);
    }

    public virtual void gameRunningStateContent(float timeScale) {
        gameRunningState = GameRunningState.PAUSED;
        gameState = GameStateGlobal.GameContentDisplay;
        gameSetTimeScale(timeScale);
    }

    // OVERLAY

    public virtual void gameRunningStateOverlay() {
        gameRunningStateOverlay(1f);
    }

    public virtual void gameRunningStateOverlay(float timeScale) {
        gameRunningState = GameRunningState.PAUSED;
        gameState = GameStateGlobal.GameOverlay;
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

            gamePlayerOutOfBoundsDelayed(3f);

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
        //UIPanelDialogBackground.ShowDefault();
        gameRunningStatePause();
    }

    public virtual void onGameContentDisplayResume() {
        GameDraggableEditor.HideAllEditDialogs();
        UIPanelDialogBackground.HideAll();
        gameRunningStateRun();
    }

    public virtual void onGameOverlayPause() {
        GameDraggableEditor.HideAllEditDialogs();
        //UIPanelDialogBackground.ShowDefault();
        gameRunningStatePause();
    }

    public virtual void onGameOverlayResume() {
        GameDraggableEditor.HideAllEditDialogs();
        //UIPanelDialogBackground.HideAll();
        gameRunningStateRun();
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

        startLevelStats();

        handleGameStart();

        GameUIController.HideUI(true);
        GameUIController.ShowHUD();

        if (allowedEditing) {
            GameDraggableEditor.ShowUIPanelEditButton();
        }

        gameRunningStateRun();

        GameUIController.ShowGameCanvas();

        AnalyticsNetworks.LogEventLevelStart(GameLevels.Current.code, GameLevels.Current.display_name);

        runDirectorsDelayed(runDirectorsDelay);

        processQueues();
    }

    public virtual void processQueues() {

        StartCoroutine(processQueuesCo());
    }

    IEnumerator processQueuesCo() {
        yield return new WaitForSeconds(2);

        processQueueGameObjectTypeData();
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

        quitGameRunning();
    }

    public virtual void quitGameRunning() {
        reset();
        stopDirectors();
        gameRunningStateStopped();
        Time.timeScale = 1f;
    }

    public virtual void onGamePause() {

        Messenger<string>.Broadcast(GameMessages.gameLevelPause, GameLevels.Current.code);

        // Show pause, resume, quit menu
        GameUIController.ShowUIPanelPause();
        //UIPanelDialogBackground.ShowDefault();
        gameRunningStatePause();
    }

    public virtual void onGameResume() {
        GameDraggableEditor.HideAllEditDialogs();
        GameUIController.ShowHUD();
        GameUIController.HideUIPanelPause();
        UIPanelDialogBackground.HideAll();
        gameRunningStateRun();

        Messenger<string>.Broadcast(GameMessages.gameLevelResume, GameLevels.Current.code);
    }

    public virtual void onGameNotStarted() {
        //
    }

    public virtual void onGameResults() {

        GameUIPanelOverlays.Instance.ShowOverlayWhiteStatic();

        stopGame();

        AnalyticsNetworks.LogEventLevelResults(GameLevels.Current.code, GameLevels.Current.display_name);
    }

    public virtual void changeGameState(GameStateGlobal gameStateTo) {
        gameState = gameStateTo;

        Messenger<GameStateGlobal>.Broadcast(GameMessages.gameActionState, gameState);

        if (gameState == GameStateGlobal.GameStarted) {
            onGameStarted();
        }
        else if (gameState == GameStateGlobal.GamePause) {
            onGamePause();
        }
        else if (gameState == GameStateGlobal.GameResume) {
            onGameResume();
        }
        else if (gameState == GameStateGlobal.GameQuit) {
            onGameQuit();
        }
        else if (gameState == GameStateGlobal.GameNotStarted) {
            onGameNotStarted();
        }
        else if (gameState == GameStateGlobal.GameResults) {
            onGameResults();
        }
        else if (gameState == GameStateGlobal.GameContentDisplay) {
            onGameContentDisplay();
        }
        else if (gameState == GameStateGlobal.GameOverlay) {
            onGameOverlay();
        }
        else if (gameState == GameStateGlobal.GamePrepare) {
            onGamePrepare(true);
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

        if (camerasBackground == null) {
            camerasBackground = new List<Camera>();
            if (cameraContainersBackground != null) {
                foreach (Camera cam
                         in cameraContainersBackground.GetComponentsInChildren<Camera>()) {
                    if (!camerasBackground.Contains(cam)) {
                        camerasBackground.Add(cam);
                    }
                }
            }
        }

#if ENABLE_FEATURE_AR

        if (camerasAR == null) {
            camerasAR = new List<Camera>();
            if (cameraContainersAR != null) {
                foreach (Camera cam
                         in cameraContainersAR.GetComponentsInChildren<Camera>()) {
                    if (!camerasAR.Contains(cam)) {
                        camerasAR.Add(cam);
                    }
                }
            }
        }
#endif
    }

    public virtual void showCameras(List<Camera> cams, bool includeParent = false) {
        StartCoroutine(showCamerasCo(cams, includeParent));
    }

    public virtual void hideCameras(List<Camera> cams, bool includeParent = false) {
        StartCoroutine(hideCamerasCo(cams, includeParent));
    }

    public virtual IEnumerator showCamerasCo(List<Camera> cams, bool includeParent = false) {

        foreach (Camera cam in cams) {
            if (cam != null) {
                if (cam.gameObject != null) {

                    if (includeParent) {
                        if (cam.transform.parent != null) {
                            cam.transform.parent.gameObject.Show();
                        }
                    }

                    cam.gameObject.Show();

                    TweenUtil.FadeToObject(cam.gameObject, 1f, .5f, .5f);
                }
            }
        }
        yield return new WaitForSeconds(2f);
    }

    public virtual IEnumerator hideCamerasCo(List<Camera> cams, bool includeParent = false) {
        foreach (Camera cam in cams) {
            if (cam != null) {
                if (cam.gameObject != null) {
                    TweenUtil.FadeToObject(cam.gameObject, 0f, .5f, .5f);
                }
            }
        }

        yield return new WaitForSeconds(1f);

        foreach (Camera cam in cams) {
            if (cam != null) {
                if (cam.gameObject != null) {

                    cam.gameObject.Hide();

                    if (includeParent) {
                        if (cam.transform.parent != null) {
                            cam.transform.parent.gameObject.Hide();
                        }
                    }
                }
            }
        }
    }

    public virtual void handleCamerasInGame() {
        showCameras(camerasGame);
        hideCameras(camerasUI);
        hideCameras(camerasBackground);
        #if ENABLE_FEATURE_AR
        hideCameras(camerasAR, true);
        #endif
    }

    public virtual void handleCamerasInUI() {
        hideCameras(camerasGame);
        showCameras(camerasUI);
        showCameras(camerasBackground);
        #if ENABLE_FEATURE_AR
        hideCameras(camerasAR, true);
        #endif
    }

    public virtual void handleCamerasInAR() {
        hideCameras(camerasGame);
        showCameras(camerasUI);
        hideCameras(camerasBackground);
        #if ENABLE_FEATURE_AR
        showCameras(camerasAR, true);
        #endif
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

                changeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                changeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if (cameraView == GameCameraView.ViewSideTop) {

                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(80);

                changeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                changeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if (cameraView == GameCameraView.ViewBackTilt) {

                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(45).WithY(90);

                changeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                changeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
            else if (cameraView == GameCameraView.ViewBackTop) {

                Vector3 positionTo = Vector3.zero;
                Vector3 rotationTo = Vector3.zero.WithX(80).WithY(90);

                changeGameCameraProperties(
                    cameraGame.gameObject, positionTo, rotationTo, 2f);

                changeGameCameraProperties(
                    cameraGameGround.gameObject, positionTo, rotationTo, 2f);
            }
        }
    }

    public virtual void changeGameCameraProperties(
        GameObject cameraObject, Vector3 positionTo, Vector3 rotationTo, float timeDelay) {
        //cameraObject.transform.rotation = Quaternion.Euler(rotationTo);
        //iTween.RotateTo(cameraObject, rotationTo, timeDelay);

        TweenUtil.RotateToObject(cameraObject, rotationTo, .5f, timeDelay);

        //UITweenerUtil.RotateTo(cameraObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, timeDelay, rotationTo);
    }

    public virtual void cycleGameCameraMode() {

        LogUtil.Log("CycleGameCameraMode: " + cameraView);

        if (cameraView == GameCameraView.ViewSide) {
            changeGameCameraMode(GameCameraView.ViewSideTop);
        }
        else if (cameraView == GameCameraView.ViewSideTop) {
            changeGameCameraMode(GameCameraView.ViewBackTilt);
        }
        else if (cameraView == GameCameraView.ViewBackTilt) {
            changeGameCameraMode(GameCameraView.ViewBackTop);
        }
        else if (cameraView == GameCameraView.ViewBackTop) {
            changeGameCameraMode(GameCameraView.ViewSide);
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

        gameContentDisplay(GameContentDisplayTypes.gamePlayerOutOfBounds);
    }

    public virtual void gamePlayerOutOfBoundsDelayed(float delay) {
        StartCoroutine(gamePlayerOutOfBoundsDelayedCo(delay));
    }

    IEnumerator gamePlayerOutOfBoundsDelayedCo(float delay) {

        yield return new WaitForSeconds(delay);

        //LogUtil.Log("GamePlayerOutOfBoundsDelayed:");

        runtimeData.outOfBounds = true;

        //LogUtil.Log("GamePlayerOutOfBoundsDelayed:runtimeData.outOfBounds:" + runtimeData.outOfBounds);

        gameState = GameStateGlobal.GameStarted;

        //LogUtil.Log("GamePlayerOutOfBoundsDelayed:gameState:" + gameState);

        checkForGameOver();
    }

    // -------------------------------------------------------

    // GAME PLAYER GOAL ZONE

    public virtual void goalZoneChange() {
        if (currentGameZone == GameZoneKeys.goal_left) {
            goalZoneChange(GameZoneKeys.goal_right);
        }
        else if (currentGameZone == GameZoneKeys.goal_right) {
            goalZoneChange(GameZoneKeys.goal_left);
        }
    }

    public virtual void goalZoneChange(string zone) {
        //if(currentGameZone == goalZone) {
        //    return;
        //}

        if (zone == GameZoneKeys.goal_left) {
            currentGameZone = zone;
        }
        else if (zone == GameZoneKeys.goal_right) {
            currentGameZone = zone;
        }

        handleGoalZoneChange();
    }

    // -------------------------------------------------------

    // HANDLE GOAL ZONE CHANGE

    public virtual void handleGoalZoneChange() {
        if (currentGameZone == GameZoneKeys.goal_left) {
            // move goal markers
        }
        else if (currentGameZone == GameZoneKeys.goal_right) {
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

    IEnumerator gamePlayerGoalZoneDelayedCo(GameObject goalObject, float delay) {

        yield return new WaitForSeconds(delay);

        //LogUtil.Log("gamePlayerGoalZoneDelayedCo:");

        goalZoneChange();
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

    IEnumerator processLevelStatsCo() {

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

        endLevelStats();

        yield return new WaitForEndOfFrame();

        GamePlayerProgress.Instance.ProcessProgressRuntimeAchievements();

        yield return new WaitForEndOfFrame();

        // PROCESS GAME TYPE SPECIFIC STATS MODE DATA
        // COLLECTION

        processProgressCollections(
            runtimeData, currentGamePlayerController.runtimeData);

        yield return new WaitForEndOfFrame();

        if (!isAdvancing) {
            advanceToResults();
        }

        GameState.SyncProfile();

        yield return new WaitForEndOfFrame();

        GamePlayerProgress.Instance.ProcessProgressLeaderboards();

        //GC.Collect();
        //GC.WaitForPendingFinalizers();
        //yield return new WaitForSeconds(8f);
    }

    //public static void ProcessProgressCollections(
    //    GameGameRuntimeData runtimeData, GamePlayerRuntimeData playerRuntimeData) {
    //    processProgressCollections(runtimeData, playerRuntimeData);
    //}

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
        gameRunningStateStopped();

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
                    if ((currentGamePlayerController.runtimeData.health <= 0f
                       || runtimeData.timeExpired)
                        // TODO move to config/controller type
                       && !isGameplayTypeRunner()) {
                        gameOverMode = true;
                    }
                }
                else if (AppModes.Instance.isAppModeGameChallenge) {
                    if ((currentGamePlayerController.runtimeData.health <= 0f
                       || runtimeData.timeExpired)
                        // TODO move to config/controller type
                       && !isGameplayTypeRunner()) {
                        gameOverMode = true;
                    }
                }
                else if (AppModes.Instance.isAppModeGameMission) {
                    if ((currentGamePlayerController.runtimeData.health <= 0f
                       || runtimeData.timeExpired)
                        // TODO move to config/controller type
                       && !isGameplayTypeRunner()) {
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
                    resultsGameDelayed();
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

        if (isGameplayTypeDasher()) {

            if (!isGameRunning) {
                return handled;
            }

            if (!InputSystem.CheckIfAllowedTouch(point)) {
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

                    sendInputAxisMoveMessage(directionNormal.x, directionNormal.y);
                }
            }
        }

        return handled;
    }

    public virtual void sendInputAxisMoveMessage(float x, float y) {
        Vector3 axisInput = Vector3.zero;
        axisInput.x = x;
        axisInput.y = y;

        sendInputAxisMessage(InputSystemKeys.moveKey, axisInput);
    }

    public virtual void sendInputAxisMessage(string axisNameTo, Vector3 axisInputTo) {
        Messenger<string, Vector3>.Broadcast(
            InputSystemEvents.inputAxis,
            InputSystemEvents.inputAxis + "-" + axisNameTo, axisInputTo
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

        checkGameLevelGrid();

        string key = getGameLevelGridKey(gridPos);

        if (levelGrid.ContainsKey(key)) {
            filled = true;
        }

        LogUtil.Log("gameLevelSpaceFilled: key:" + key + " filled:" + filled);

        return filled;
    }

    public virtual void setGameLevelGridSpaceFilled(Vector3 gridPos, GameLevelItemAsset asset) {

        checkGameLevelGrid();

        string key = getGameLevelGridKey(gridPos);

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

        checkGameLevelItems();

        if (levelItems != null) {
            levelItems.Clear();
        }
    }

    public virtual void clearGameLevelGrid() {

        Debug.Log("clearGameLevelGrid:");

        checkGameLevelGrid();

        if (levelGrid != null) {
            levelGrid.Clear();
        }
    }

    public virtual List<GameLevelItemAsset> getLevelRandomized() {

        List<GameLevelItemAsset> levelItems = new List<GameLevelItemAsset>();

        return getLevelRandomized(levelItems);
    }

    public virtual Vector3 getRandomVectorInGameBounds() {
        return Vector3.zero
            .WithX(UnityEngine.Random.Range(
            gameBounds.boundaryTopLeft.transform.position.x,
            gameBounds.boundaryTopRight.transform.position.x))
            .WithY(UnityEngine.Random.Range(.1f, gameBounds.boundaryTopCeiling.transform.position.y / 4));
    }

    public virtual List<GameLevelItemAsset> getLevelRandomizedGrid() {
        return getLevelRandomizedGrid(GameLevelGridData.GetDefault());
    }

    public virtual List<GameLevelItemAsset> getLevelRandomizedGrid(GameLevelGridData gameLevelGridData) {

        clearGameLevelItems();
        clearGameLevelGrid();

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

        clearGameLevelItems();
        clearGameLevelGrid();

        return levelItems;
    }

    public virtual GameLevelItemAsset getLevelItemAsset(
        GameLevelItemAssetData data) {

        return getLevelItemAssetRandom(data);
    }

    public virtual GameLevelItemAsset getLevelItemAssetRandom(
        GameLevelItemAssetData data) {

        data.position_data = new Vector3Data(getRandomVectorInGameBounds());

        return getLevelItemAssetFull(data);
    }

    public virtual void syncLevelItem(Vector3 gridPos, GameLevelItemAssetData data) {
        if (!isGameLevelGridSpaceFilled(gridPos)) {
            //&& GameController.CheckBounds(gridPos)) {

            GameLevelItemAsset asset = getLevelItemAssetFull(data);

            asset.local_position_data = new Vector3Data();
            asset.local_position_data = data.local_position_data;

            asset.local_rotation_data = new Vector3Data();
            asset.local_rotation_data = data.local_rotation_data;

            asset.scale_data = new Vector3Data();
            asset.scale_data = data.scale_data;

            levelItems.Add(asset);

            setGameLevelGridSpaceFilled(gridPos, asset);
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

            GameLevelItemAsset asset = getLevelItemAssetRandom(data);

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

            GameLevelItemAsset asset = getLevelItemAssetRandom(data);

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
        */

        return levelItems;
    }

    // -------------------------------------------------------

    // UPDATE

    public static bool touchHandled = false;

    internal virtual void handleInputTouch() {

        bool mousePressed = InputSystem.isMousePressed;
        //bool mouseSecondaryPressed = InputSystem.isMouseSecondaryPressed;
        bool touchPressed = InputSystem.isTouchPressed;
        bool handled = false;

        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        //bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();

        //if(controlInputTouchFinger) {

        if (touchPressed) {
            foreach (Touch touch in Input.touches) {
                handled = handleTouchInputPoint(touch.position);

                if (handled)
                    break;
            }
        }
        else if (mousePressed) {
            handled = handleTouchInputPoint(Input.mousePosition);
        }
        else {
            if (currentPlayerController != null) {

                // reset handled in input axis, might be keyboard input so don't reset.
                //if(getCurrentPlayerController.controllerData.thirdPersonController != null) {
                //    getCurrentPlayerController.controllerData.thirdPersonController.verticalInput = 0f;
                //    getCurrentPlayerController.controllerData.thirdPersonController.horizontalInput = 0f;
                //    getCurrentPlayerController.controllerData.thirdPersonController.verticalInput2 = 0f;
                //    getCurrentPlayerController.controllerData.thirdPersonController.horizontalInput2 = 0f;
                //}
            }
        }

        //}

        touchHandled = handled;
    }

    internal virtual void handleInput() {

        handleGameInput();

        if (isGameplayTypeDasher()) {
            handleInputTouch();
        }
        else if (isGameplayTypeRunner()) {
            //handleInputTouch();
        }
    }

    public enum GamePlayerDirection {
        None,
        Up,
        Down,
        Left,
        Right,
        LowerLeftDiagonal,
        UpperLeftDiagonal,
        LowerRightDiagonal,
        UpperRightDiagonal
    }

    public virtual void handleGameInputDirection(GamePlayerDirection direction) {
        if (!GameConfigs.isGameRunning) {
            return;
        }

        //if (controllerData == null) {
        //    return;
        //}

        if (isGameplayTypeRunner()) {

            if (direction == GamePlayerDirection.Up) {
                gamePlayerJump();
            }
            else if (direction == GamePlayerDirection.Down) {
                //GameController.GamePlayerSlide(Vector3.zero.WithZ(3f));
                //GameController.GamePlayerAttack();
                gamePlayerSlide(Vector3.zero.WithZ(.15f));
            }
            else if (direction == GamePlayerDirection.Left
                    || direction == GamePlayerDirection.LowerLeftDiagonal
                    || direction == GamePlayerDirection.UpperLeftDiagonal) {
                //GameController.GamePlayerMove(Vector3.zero.WithX(-16f), rangeStart, rangeEnd);

                Vector3 pos = containerInfinity.SwitchLineLeft();

                //Debug.Log("handleGameInputDirection:left:" + pos);

                gamePlayerMove(pos);
            }
            else if (direction == GamePlayerDirection.Right
                    || direction == GamePlayerDirection.LowerRightDiagonal
                    || direction == GamePlayerDirection.UpperRightDiagonal) {
                //GameController.GamePlayerMove(Vector3.zero.WithX(16f), rangeStart, rangeEnd);

                Vector3 pos = containerInfinity.SwitchLineRight();

                //Debug.Log("handleGameInputDirection:right:" + pos);

                gamePlayerMove(pos);
            }
            else {

                ////infiniteSpeed += .3f * Time.deltaTime;

                //speedInfinite = Mathf.Clamp(speedInfinite, 0, 400);

                //GameController.GamePlayerSetSpeed(speedInfinite);
                ////GameController.SendInputAxisMoveMessage(0, 1);
            }
        }
    }


    // TODO move to player controller and events

    public virtual void OnInputSwipe(InputSystemSwipeDirection direction, Vector3 pos, float velocity) {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        //if (controllerData == null) {
        //    return;
        //}

        if (isGameplayTypeRunner()) {

            if (direction == InputSystemSwipeDirection.Up) {
                handleGameInputDirection(GamePlayerDirection.Up);
            }
            else if (direction == InputSystemSwipeDirection.Down) {
                handleGameInputDirection(GamePlayerDirection.Down);
            }
            else if (direction == InputSystemSwipeDirection.Left
                    || direction == InputSystemSwipeDirection.LowerLeftDiagonal
                    || direction == InputSystemSwipeDirection.UpperLeftDiagonal) {
                handleGameInputDirection(GamePlayerDirection.Left);
            }
            else if (direction == InputSystemSwipeDirection.Right
                    || direction == InputSystemSwipeDirection.LowerRightDiagonal
                    || direction == InputSystemSwipeDirection.UpperRightDiagonal) {
                handleGameInputDirection(GamePlayerDirection.Right);
            }
            else {
                handleGameInputDirection(GamePlayerDirection.None);
            }
        }
    }

    internal virtual void handleGameInput() {

        if (isGameplayTypeRunner()) {

            if (InputSystem.isUpPressDown) {
                handleGameInputDirection(GamePlayerDirection.Up);
            }
            else if (InputSystem.isDownPressDown) {
                handleGameInputDirection(GamePlayerDirection.Down);
            }
            else if (InputSystem.isLeftPressDown) {
                handleGameInputDirection(GamePlayerDirection.Left);
            }
            else if (InputSystem.isRightPressDown) {
                handleGameInputDirection(GamePlayerDirection.Right);
            }
            else {
                handleGameInputDirection(GamePlayerDirection.None);
            }
        }
        else if (isGameplayTypeDasher()) {
            //DetectSwipe();
            InputSystem.UpdateTouchLaunch();
        }
    }


    // ------------------------------------------------------------------------
    // CURVE INFINITE HANDLING

    // ------------------------------------------------------------------------
    // CURVE ENABLED

    // get

    public static bool CurveInfiniteEnabledGet() {
        if (GameController.isInst) {
            return GameController.Instance.curveInfiniteEnabledGet();
        }

        return true;
    }

    public bool curveInfiniteEnabledGet() {

        if (runtimeData == null) {
            return true;
        }

        return runtimeData.curveEnabled;
    }

    // set

    public static void CurveInfiniteEnabledSet(bool val) {
        if (GameController.isInst) {
            GameController.Instance.curveInfiniteEnabledSet(val);
        }
    }

    public void curveInfiniteEnabledSet(bool val) {

        if (runtimeData == null) {
            return;
        }

        runtimeData.curveEnabled = val;
    }

    // ------------------------------------------------------------------------
    // CURVE AMOUNT

    // get

    public static Vector4 CurveInfiniteAmountGet() {
        if (GameController.isInst) {
            return GameController.Instance.curveInfiniteAmountGet();
        }

        return Vector4.zero;
    }

    public Vector4 curveInfiniteAmountGet() {

        if (runtimeData == null) {
            return Vector4.zero;
        }

        return runtimeData.curveInfiniteAmount;
    }

    // set

    public static void CurveInfiniteAmountSet(Vector4 val) {
        if (GameController.isInst) {
            GameController.Instance.curveInfiniteAmountSet(val);
        }
    }

    public void curveInfiniteAmountSet(Vector4 val) {

        if (runtimeData == null) {
            return;
        }

        runtimeData.curveInfiniteAmount = val;
    }

    // ------------------------------------------------------------------------
    // CURVE DISTANCE

    // get

    public static float CurveInfiniteDistanceGet() {
        if (GameController.isInst) {
            return GameController.Instance.curveInfiniteDistanceGet();
        }

        return 50f;
    }

    public float curveInfiniteDistanceGet() {

        if (runtimeData == null) {
            return 50f;
        }

        return runtimeData.curveInfiniteDistance;
    }

    // set

    public static void CurveInfiniteDistanceSet(float val) {
        if (GameController.isInst) {
            GameController.Instance.curveInfiniteDistanceSet(val);
        }
    }

    public void curveInfiniteDistanceSet(float val) {

        if (runtimeData == null) {
            return;
        }

        runtimeData.curveInfiniteDistance = val;
    }

    // ------------------------------------------------------------------------
    // ASSETS CONTEXT

    public virtual string GameAssetPresetCode(string assetCode) {

        GamePreset assetPreset = GamePresets.Instance.GetById(assetCode);

        if (assetPreset != null) {

            GamePresetItem presetItem = assetPreset.GetItemRandomByProbability(assetPreset.data.items);

            if (presetItem != null) {
                assetCode = presetItem.code;
            }
        }
        else {
            //Debug.Log("GamePreset NOT FOUND: " + assetCode);
        }

        return assetCode;
    }

    //

    public virtual void GameAssetObjectContextGet(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        // Handle template by level/world/character

        // CUSTOM GAME BLOCK

        if (assetCode == data.codeGameBlockFloor) {

            GameAssetObjectContextGetBlock(data, assetCode, go);
        }

        // CUSTOM GAME BLOCK LOW

        else if (assetCode == data.codeGameBlockLow) {

            GameAssetObjectContextGetBlockLow(data, assetCode, go);
        }

        // CUSTOM GAME BLOCK HIGH

        else if (assetCode == data.codeGameBlockHigh) {

            GameAssetObjectContextGetBlockHigh(data, assetCode, go);
        }

        // CUSTOM GAME SIDE

        else if (assetCode == data.codeGameSide) {

            GameAssetObjectContextGetSide(data, assetCode, go);
        }

        // CUSTOM GAME WORLD FLOOR

        else if(assetCode == data.codeGameFloor) {

            GameAssetObjectContextGetWorldFloor(data, assetCode, go);
        }
        
        // CUSTOM GAME BLOCK EMPTY

        else if(assetCode == data.codeGameBlockEmpty) {

            GameAssetObjectContextGetBlockEmpty(data, assetCode, go);
        }

        // CUSTOM GAME BLOCK FULL

        else if(assetCode == data.codeGameBlockFull) {

            GameAssetObjectContextGetBlockFull(data, assetCode, go);
        }
    }

    public virtual void GameAssetObjectContextGetBlockFull(
        GameObjectInfinteData data, string assetCode, GameObject go) {

    }

    public virtual void GameAssetObjectContextGetBlockEmpty(
        GameObjectInfinteData data, string assetCode, GameObject go) {

    }

    public virtual void GameAssetObjectContextGetBlock(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        /*
        foreach (GameObjectInactive container in
                go.GetList<GameObjectInactive>()) {

            if (!container.type.IsEqualLowercase(BaseDataObjectKeys.container)
                && !container.code.IsEqualLowercase(BaseDataObjectKeys.assets)) {

                continue;
            }

            foreach (GameObjectInactive asset in
                container.gameObject.GetList<GameObjectInactive>()) {

                // handle main object

                if (asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.main)) {

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    assetObject.DestroyChildren();

                    // asset-game-block-world-tiger-1
                    string codeAsset = "";

                    codeAsset = StringUtil.Dashed(BaseDataObjectKeys.asset,
                                            assetCode, GameWorlds.Current.code);

                    codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

                    GameObject goMain = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                    if (goMain == null) {
                        codeAsset = StringUtil.Dashed(assetCode, BaseDataObjectKeys.defaultKey);
                    }

                    goMain = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                    if (goMain == null) {
                        continue;
                    }

                    goMain.transform.parent = assetObject.transform;
                }
                
                if (asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.environment)) {

                    float chance = UnityEngine.Random.Range(0, 10);

                    if (chance < 3) {
                        
                        GameObject goAsset = GameAssetObjectContextItem(assetCode, BaseDataObjectKeys.environment, asset);

                        goAsset.transform.localPosition = MathUtil.RandomRange(-.3f, .3f, 0, 0, -data.distanceTickZ, data.distanceTickZ);
                        goAsset.transform.localScale = MathUtil.RandomRange(1f, 4f);
                        goAsset.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                    }
                }
            }
        }  
        */

        //return go;
    }

    public virtual void GameAssetObjectContextGetBlockLow(
        GameObjectInfinteData data, string assetCode, GameObject go) {

    }

    public virtual void GameAssetObjectContextGetBlockHigh(
        GameObjectInfinteData data, string assetCode, GameObject go) {

    }

    public virtual GameObject GameAssetObjectContextItem(
        GameObject assetObject, string assetCode, string key = "", int maxCount = 1) {

        if (assetObject != null
           && assetObject.transform != null
           && (assetObject.transform.childCount == 0
           || assetObject.transform.childCount <= maxCount)) {

            //if(assetObject.transform.childCount == 0) {
            //    assetObject.DestroyChildren();
            //}

            string codeAsset = "";

            codeAsset = StringUtil.Dashed(assetCode, key, GameWorlds.Current.code);

            codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

            GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

            if (goAsset == null) {
                codeAsset = StringUtil.Dashed(assetCode, key, BaseDataObjectKeys.defaultKey);
                goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);
            }

            if (goAsset == null) {
                codeAsset = StringUtil.Dashed(assetCode, BaseDataObjectKeys.defaultKey);
                goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);
            }

            if (goAsset != null) {
                goAsset.transform.parent = assetObject.transform;
                goAsset.transform.position = assetObject.transform.position;

                goAsset.SetOnly<GameObjectInfiniteAssetItem>();

                return goAsset;
            }
        }

        return null;
    }

    /*
    public virtual void GameAssetObjectContextLoad(GameObject assetObject, string assetCode, string key = "") {

        if(assetObject != null 
            && assetObject.transform != null 
            && assetObject.transform.childCount == 0) {

            assetObject.DestroyChildren();

            // asset-game-block-world-tiger-1
            string codeAsset = "";

            codeAsset = StringUtil.Dashed(BaseDataObjectKeys.asset, key,
                assetCode, GameWorlds.Current.code);

            codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, key, codeAsset));

            GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

            if(goAsset == null) {
                codeAsset = StringUtil.Dashed(assetCode, key, BaseDataObjectKeys.defaultKey);
                goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);
            }

            if(goAsset == null) {
                continue;
            }

            goAsset.transform.parent = assetObject.transform;
            goAsset.transform.position = assetObject.transform.position;

            goAsset.SetOnly<GameObjectInfiniteAssetItem>();
        }
    }

    public virtual GameObject GameAssetObjectContextItem(string assetCode, string key, GameObjectInactive asset) {

        GameObject assetObject = asset.gameObject;

        if(assetObject != null 
            && assetObject.transform != null 
            && assetObject.transform.childCount == 0) {

            assetObject.DestroyChildren();

            string codeAsset = "";

            codeAsset = StringUtil.Dashed(
                assetCode, key, GameWorlds.Current.code);

            codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

            GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

            if (goAsset == null) {
                codeAsset = StringUtil.Dashed(assetCode, key, BaseDataObjectKeys.defaultKey);
                goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);
            }

            if (goAsset != null) {
                goAsset.transform.parent = assetObject.transform;
            }
        }

        return goAsset;
    }
    */
    
    public virtual void GameAssetObjectContextGetWorldFloor(
        GameObjectInfinteData data, string assetCode, GameObject go) {

    }

    public virtual void GameAssetObjectContextGetSide(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        /*
        foreach (GameObjectInactive container in
            go.GetList<GameObjectInactive>()) {

            if (!container.type.IsEqualLowercase(BaseDataObjectKeys.container)
                && !container.code.IsEqualLowercase(BaseDataObjectKeys.assets)) {

                continue;
            }


            foreach (GameObjectInactive asset in
                container.gameObject.GetList<GameObjectInactive>()) {

                // handle main object

                // TERRAIN

                if (asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.terrain)) {

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    assetObject.DestroyChildren();

                    // asset-game-block-world-tiger-1
                    string codeAsset = "";

                    codeAsset = StringUtil.Dashed(assetCode, asset.code, GameWorlds.Current.code);

                    codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

                    GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                    if (goAsset == null) {
                        codeAsset = StringUtil.Dashed(assetCode, asset.code, BaseDataObjectKeys.defaultKey);
                    }

                    goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                    if (goAsset == null) {
                        continue;
                    }

                    goAsset.transform.parent = assetObject.transform;
                }

                // BUMPER

                else if (asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.bumper)) {

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    assetObject.DestroyChildren();

                    // ADD SIDE BLOCKS 

                    for(int i = 0; i < 8; i++) {

                        string codeAsset = "";

                        codeAsset = StringUtil.Dashed(
                            assetCode, asset.code, "blocks", GameWorlds.Current.code);

                        codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

                        //codeAsset = StringUtil.Dashed(assetCode, asset.code, BaseDataObjectKeys.defaultKey);

                        GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                        if(goAsset == null) {
                            codeAsset = StringUtil.Dashed(assetCode, BaseDataObjectKeys.defaultKey);
                        }

                        goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                        if(goAsset == null) {
                            continue;
                        }

                        goAsset.transform.parent = assetObject.transform;
                        goAsset.transform.position = assetObject.transform.position;

                        float posX = 0;
                        float widthX = 128;
                        float itemX = data.distanceTickZ;

                        posX = (i + 1) * itemX;
                        posX = posX - widthX / 2;

                        goAsset.transform.localPosition = Vector3.zero.WithX(posX);

                        if(codeAsset.Contains("plant")) {
                            goAsset.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                            goAsset.transform.localScale = MathUtil.RandomRange(1f, 4f);
                        }

                        //goAsset.transform.localPosition = MathUtil.RandomRange(-64, 64, 1, 1, 2, 256);
                        //goAsset.transform.localScale = MathUtil.RandomRange(1f, 4f);
                        //goAsset.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                    }
                }

                // ENVIRONMENT

                else if (asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.environment)) {

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    assetObject.DestroyChildren();

                    // ADD TREES 

                    for(int i = 0; i < 6; i++) {

                        string codeAsset = "";

                        codeAsset = StringUtil.Dashed(
                            assetCode, asset.code, "trees", GameWorlds.Current.code);

                        codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

                        //codeAsset = StringUtil.Dashed(assetCode, asset.code, BaseDataObjectKeys.defaultKey);

                        GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                        if(goAsset == null) {
                            codeAsset = StringUtil.Dashed(assetCode, BaseDataObjectKeys.defaultKey);
                        }

                        goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                        if(goAsset == null) {
                            continue;
                        }

                        goAsset.transform.parent = assetObject.transform;
                        goAsset.transform.position = assetObject.transform.position;

                        goAsset.transform.localPosition = MathUtil.RandomRange(-64, 64, 1, 1, 8, 256);
                        goAsset.transform.localScale = MathUtil.RandomRange(1f, 4f);
                        goAsset.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                    }

                    // ADD PLANTS 

                    for(int i = 0; i < 6; i++) {

                        string codeAsset = "";

                        codeAsset = StringUtil.Dashed(
                            assetCode, asset.code, "plants", GameWorlds.Current.code);

                        codeAsset = GameAssetPresetCode(StringUtil.Dashed(BaseDataObjectKeys.asset, codeAsset));

                        //codeAsset = StringUtil.Dashed(assetCode, asset.code, BaseDataObjectKeys.defaultKey);

                        GameObject goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                        if(goAsset == null) {
                            codeAsset = StringUtil.Dashed(assetCode, BaseDataObjectKeys.defaultKey);
                        }

                        goAsset = AppContentAssets.LoadAssetLevelAssets(codeAsset, assetObject.transform.position);

                        if(goAsset == null) {
                            continue;
                        }

                        goAsset.transform.parent = assetObject.transform;
                        goAsset.transform.position = assetObject.transform.position;

                        goAsset.transform.localPosition = MathUtil.RandomRange(-64, 64, 1, 1, 2, 256);
                        goAsset.transform.localScale = MathUtil.RandomRange(1f, 4f);
                        goAsset.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                    }

                }
            }
        }
        */

        //return go;
    }

    public virtual GameDataObject GetLevelAssetDynamicObject(GameObjectInfinteData data, double x, double y, double z) {

        if (data.dataObjects == null) {
            return null;
        }

        foreach (GameDataObject obj in data.dataObjects) {
            if (obj.grid_data.x == x
               && obj.grid_data.y == y
               && obj.grid_data.z == z) {
                return obj;
            }
        }

        return null;
    }

    public virtual void LoadPartDynamicByIndexPart(GameObjectInfinteData data, int indexItem, bool clear = false) {

        /*

        // Use off screen location to spawn before move

        Vector3 spawnLocation = Vector3.zero.WithY(5000);

        // --------------------------------------------------------------------
        // ADD PART ITEMS CONTAINER

        GameObject go = LoadAssetLevelPlaceholder(data, data.codeGamePartItems, spawnLocation, indexItem);

        if (go == null) {
            return;
        }

        go.DestroyChildren();

        go.name = StringUtil.Dashed(data.code, indexItem.ToString());
        go.transform.parent = data.parentContainer.transform;

        // --------------------------------------------------------------------
        // PART ITEMS       

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

        if (indexItem > data.partBackCount) {
            LoadPartDynamicByIndexPartData(data, indexItem);
        }

        // --------------------------------------------------------------------
        // LOAD ENVIRONMENT

        LoadLevelAssetsPeriodic(data, go, indexItem, clear);

        // --------------------------------------------------------------------
        // LINES

        LoadLevelAssetsLines(data, go, indexItem, spawnLocation);
        */
    }

    public virtual void LoadLevelAssetsLines(GameObjectInfinteData data, GameObject go, int indexItem, Vector3 spawnLocation) {

    }

    public virtual void LoadLevelAssetsPeriodic(GameObjectInfinteData data, GameObject parentGo, int indexItem, bool clear = true) {

        StartCoroutine(LoadLevelAssetsPeriodicCo(data, parentGo, indexItem, clear));
    }

    IEnumerator LoadLevelAssetsPeriodicCo(
        GameObjectInfinteData data, GameObject parentGo, int indexItem, bool clear = true) {

        yield return null;

        /*
        //if (((indexItem + 1) * data.distanceTickZ) % data.distanceTickZ == 0) {
        if((indexItem + 1) % (data.distanceTickZ / 2) == 0) { // every 8

            // Load terrain and ambience

            GameObject go = AppContentAssets.LoadAssetLevelAssets(data.codeGameFloor, parentGo.transform.position);

            if(go == null) {
                Debug.Log("Asset not found levelassets/" + data.codeGameFloor);
                yield return null;
            }

            go.transform.parent = parentGo.transform;

            go.transform.localPosition = go.transform.localPosition.WithY(-data.distanceTickZ);
        }
        
        */
    }

    void LoadLevelAssetDynamicByIndex(GameObjectInfinteData data, int indexItem, bool clear = false) {

        LoadPartDynamicByIndexPart(data, indexItem, clear);
    }

    public GameObject LoadAssetLevelPlaceholder(GameObjectInfinteData data, string assetCode, Vector3 spawnLocation, int indexItem) {

        GameObject goAssetBlock = AppContentAssets.LoadAssetLevelAssets(assetCode, spawnLocation);

        if (goAssetBlock == null) {
            //Debug.Log("Asset not found levelassets/" + assetCode);
        }
        else {
            GameAssetObjectContextGet(data, assetCode, goAssetBlock);
        }

        return goAssetBlock;
    }

    public void LoadPartDynamicByIndexPartData(GameObjectInfinteData data, int indexItem, bool clear = false) {

        // Get current index part

        // Cycle through index parts until new index part needed

        // Get new index part and continue 

        if (data.data == null) {
            data.data = new GameLevelLayout();
        }

        if (data.currentLevelGridIndex == -1) {

            data.data = GameLevels.GetGameLevelLayoutFromPresets(
                GameLevels.Current.data.layout_presets, BaseDataObjectKeys.dynamicKey);

            data.dataObjects = GameLevels.GetGameLevelLayoutObjects(data.data);
        }

        data.currentLevelGridIndex += 1;

        if (data.currentLevelGridIndex >= data.data.GetGridHeight()) {
            data.currentLevelGridIndex = -1;
        }
    }

    public static void LoadInitialParts(GameObjectInfinteData data) {
        if (GameController.isInst) {
            GameController.Instance.loadInitialParts(data);
        }
    }

    public void loadInitialParts(GameObjectInfinteData data) {

        // Add initial parts that can spawn other parts
        // Fill out parts to boundaries

        for (int i = 0; i < (int)data.rangeBoundsMax.z / data.distanceTickZ; i++) {

            bool shouldClear = false;

            if (i < data.partStartCount) {
                shouldClear = true;
            }

            LoadLevelAssetDynamicByIndex(data, i, shouldClear);

            data.padIndex = i;
            data.lastLoadIndex = i;
        }

        // Place 10 parts back from view

        for (int i = 0; i < data.partBackCount; i++) {
            LoadLevelAssetDynamicByIndex(data, -i, true);
        }
    }

    public static void UpdateParts(GameObjectInfinteData data) {
        if (GameController.isInst) {
            GameController.Instance.updatePartsStationary(data);
        }
    }

    public virtual void updatePartsStationary(GameObjectInfinteData data) {

        //StartCoroutine(updatePartsStationaryCo(data));
        //}

        //public IEnumerator updatePartsStationaryCo(GameObjectInfinteData data) {

        // index is 31 range 1000 
        // rangeBoundsMax.z / distanceTickZ;

        data.currentIndex = (int)(-data.distance.z / data.distanceTickZ);
        int loadIndex = data.currentIndex + data.padIndex;

        if (data.lastLoadIndex < loadIndex) {

            //Debug.Log("LoadingPart");

            LoadLevelAssetDynamicByIndex(data, loadIndex);

            data.lastLoadIndex = loadIndex;
        }

        //yield return null;
    }

    // ------------------------------------------------------------------------
    // HANDLE

    internal virtual void handleGameStart() {

        if (isGameplayWorldTypeStationary()) {
            handleInfiniteCurve();
        }
    }

    internal virtual void handleInfiniteCurve() {

        if (!isGameRunning) {
            return;
        }

        if (currentGamePlayerController == null) {
            return;
        }

        if (curveInfiniteEnabledGet()
           && currentGamePlayerController.GamePlayerMoveSpeedGet() > 15f) {
            runtimeData.curve.x = UnityEngine.Random.Range(-5, 5);
            runtimeData.curve.z = UnityEngine.Random.Range(-4, 4);
        }
        else {
            runtimeData.curve.x = 0;
            runtimeData.curve.z = 0;
        }

        //Debug.Log("handleInfiniteCurve:" + runtimeData.curve);

        Invoke("handleInfiniteCurve", UnityEngine.Random.Range(5, 10));
    }

    internal virtual void handleGametypeInit() {

        if (isGameplayWorldTypeStationary()) {
            //if (controllerInfinity == null) {
            controllerInfinity = gameObject.Get<GameObjectInfiniteController>();
            //}

            //if (containerInfinity == null) {
            containerInfinity = gameObject.Get<GameObjectInfiniteContainer>();
            //}
        }
    }

    internal virtual void handleGametypeReset() {

        if (isGameplayWorldTypeStationary()) {

            if (containerInfinity != null) {
                //containerInfinity.Reset();
                containerInfinity.gameObject.DestroyGameObject();
                containerInfinity = null;
            }

            if (controllerInfinity != null) {
                controllerInfinity.gameObject.DestroyGameObject();
                controllerInfinity = null;
            }
        }
    }

    internal virtual void handleUpdateStationary() {

        if (currentGamePlayerController == null) {
            return;
        }

        if (containerInfinity == null) {
            handleGametypeInit();
        }

        if (containerInfinity == null
           || controllerInfinity == null) {
            return;
        }

        if (GameConfigs.isGameRunning) {

            if (!curveInfiniteEnabledGet()) {
                runtimeData.curve = Vector4.zero;
                runtimeData.curveInfiniteAmount = runtimeData.curve;
            }

            float speedThrottle =
                (currentGamePlayerController.controllerData.speedInfinite / currentGamePlayerController.controllerData.speedInfiniteMax) * .1f;

            containerInfinity.UpdatePositionPartsZ(
                (-currentGamePlayerController.controllerData.moveGamePlayerPosition.z *
                currentGamePlayerController.controllerData.speedInfinite) * Time.deltaTime);

            // .15f *
            //Debug.Log("speedThrottle:" + speedThrottle);
            //Debug.Log("currentGamePlayerController.controllerData.speedInfinite:" + currentGamePlayerController.controllerData.speedInfinite);
            //Debug.Log("currentGamePlayerController.controllerData.moveGamePlayerPosition.z:" + currentGamePlayerController.controllerData.moveGamePlayerPosition.z);

            runtimeData.curveInfiniteAmount =
                Vector4.Lerp(
                runtimeData.curveInfiniteAmount, runtimeData.curve, speedThrottle * Time.deltaTime);
        }
    }

    internal virtual void handleUpdate() {

        handleInput();

        if (IsGameplayWorldTypeDefault()) {
            handleUpdateDefault();
        }
        else if (isGameplayWorldTypeStationary()) {
            //handleUpdateStationary();
        }
    }

    internal virtual void handleUpdateDefault() {
        if (IsGameplayWorldTypeDefault()) {
            // handleLateUpdateDefault();
        }
        else if (isGameplayWorldTypeStationary()) {
            //handleUpdateStationary();
        }

    }

    internal virtual void handleFixedUpdate() {

        //handleInput();

        if (IsGameplayWorldTypeDefault()) {
            //handleLateUpdateDefault();
        }
        else if (isGameplayWorldTypeStationary()) {
            //handleUpdateStationary();
        }
    }


    internal virtual void handleLateUpdate() {

        //handleInput();

        if (IsGameplayWorldTypeDefault()) {
            handleLateUpdateDefault();
        }
        else if (isGameplayWorldTypeStationary()) {
            handleUpdateStationary();
        }
    }

    internal virtual void handleLateUpdateDefault() {


    }

    // ------------------------------------------------------------------------
    // UPDATE

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

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (gameState == GameStateGlobal.GamePause
           || GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            return;
        }

        handleUpdate();

        if (!gameObjectTimer.IsTimerPerf(
               GameObjectTimerKeys.gameUpdateAll)) {
            return;
        }

        if (isGameRunning) {
            checkForGameOver();
        }

        currentTimeBlockBase += Time.deltaTime;

        if (currentTimeBlockBase > actionIntervalBase) {
            currentTimeBlockBase = 0.0f;
        }
    }

    public virtual void FixedUpdate() {

        // UPDATE

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (gameState == GameStateGlobal.GamePause
           || GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            return;
        }

        handleFixedUpdate();
    }

    public virtual void LateUpdate() {

        // UPDATE

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (gameState == GameStateGlobal.GamePause
           || GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            return;
        }

        handleLateUpdate();
    }
}