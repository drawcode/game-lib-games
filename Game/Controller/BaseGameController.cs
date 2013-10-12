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

public class GameContentDisplayTypes {
    public static string gamePlayerOutOfBounds = "content-game-player-out-of-bounds";

    public static string gameChoicesOverview = "content-game-game-choices-overview";

    public static string gameChoicesItemStart = "content-game-game-choices-item-start";
    public static string gameChoicesItemResult = "content-game-game-choices-item-result";

    public static string gameCollectOverview = "content-game-game-collect-overview";

    public static string gameCollectItemStart = "content-game-game-collect-item-start";
    public static string gameCollectItemResult = "content-game-game-collect-item-result";
}

// GLOBAL

public enum GameControllerType {
    Iso2DSide,
    Iso3D,
    Iso2DTop,
    Perspective3D
}

public enum GameModeChoiceFlowState {
    GameModeTrainingChoiceOverview,
    GameModeTrainingChoiceDisplayItem,
    GameModeTrainingChoiceAnswer,
    GameModeTrainingChoiceResults,
}

public enum GameModeCollectionFlowState {
    GameModeTrainingCollectionOverview,
    GameModeTrainingCollectionDisplayItem,
    GameModeTrainingCollectionResults,
}

public class GamePlayerMessages {

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

public class GameActorItem {
    public float health = 1f;
    public float difficulty = .3f;
    public float scale = 1f;
    public float speed = 1f;
    public float attack = 1f;
    public float defense = 1f;
    
    public string characterCode = "character-enemy-goblin";
    public string prefabCode = "GameEnemyGobln";
}

public class GameMessages {
    public static string scores = "game-shooter-scores";
    public static string score = "game-shooter-score";
    public static string ammo = "game-shooter-ammo";
    public static string coin = "game-shooter-coin";
    public static string state = "game-shooter-state";
}

public class GameStatCodes {
    public static string wins = "wins";
    public static string losses = "losses";
    public static string shots = "shots";
    public static string destroyed = "destroyed";
    public static string score = "score";
}

public class GameGameRuntimeData {
    public double currentLevelTime = 0;
    public double timeRemaining = 90;
    public double coins = 0;
    public string levelCode = "";
    public double score = 0;

    public bool outOfBounds = false;
    
    public GameGameRuntimeData() {
        Reset();
    }
    
    public void Reset() {
        currentLevelTime = 0;
        timeRemaining = 90;
        coins = 0;
        levelCode = "";
        score = 0;
        outOfBounds = false;
        ResetTimeDefault();
    }
    
    public bool timeExpired {
        get {
            if(timeRemaining <= 0) {
                timeRemaining = 0;
            return true;
            }
            return false;
        }
    }
    
    public void SubtractTime(double delta) {
        if(timeRemaining > 0) {
            timeRemaining -= delta;
        }
    }
    
    public void ResetTimeDefault() {
        timeRemaining = 90;
    }
    
    public void ResetTime(double timeTo) {
        timeRemaining = timeTo;
    }
    
    public void AppendTime(double timeAppend) {
        timeRemaining += timeAppend;
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
    public GameObject levelActorsContainerObject;
    public GameObject levelZonesContainerObject;
    public GameObject levelSpawnsContainerObject;
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
    
    float currentTimeBlock = 0.0f;
    float actionInterval = 1.3f;

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

        GameCustomController.Instance.BroadcastCustomColorsSync();
    }

    public virtual void OnEnable() {
        Gameverses.GameMessenger<string>.AddListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameDirectorActor>.AddListener(
            GameDirectorMessages.gameDirectorSpawnActor,
            OnGameDirectorActorLoad);

        Messenger.AddListener(BaseGameProfileMessages.ProfileShouldBeSaved, OnProfileShouldBeSavedEventHandler);
    }
    
    public virtual void OnDisable() {
        Gameverses.GameMessenger<string>.RemoveListener(
            Gameverses.GameNetworkPlayerMessages.PlayerAdded,
            OnNetworkPlayerContainerAdded);

        Messenger<GameDirectorActor>.RemoveListener(
            GameDirectorMessages.gameDirectorSpawnActor,
            OnGameDirectorActorLoad);

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

    public virtual void OnGameDirectorActorLoad(GameDirectorActor actor) {


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

        if(zones != currentGameZone) {
            currentGameZone = zones;

            if(zones == GameZones.left) {

            }
            else if(zones == GameZones.right) {

            }

        }
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

    public virtual void loadLevel(string code) {

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
    
        // Load the game levelitems for the game level code
        GameController.StartGame(code);
    
        GameHUD.Instance.SetLevelInit(GameLevels.Current);
    
        GameHUD.Instance.AnimateIn();
    
        //GameUI.Instance.ToggleGameUI();
    
    }
    
    public virtual void loadLevelItems() {
        GameLevelItems.Current.level_items
            = GameController.GetLevelRandomizedGrid();
    }

    public virtual void saveCurrentLevel() {
        GameDraggableEditor.SaveCurrentLevel(
            GameController.Instance.levelItemsContainerObject);
    }

    public virtual void loadStartLevel(string levelCode) {

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
        GameState.SaveProfile();
    }

    public virtual void changeGameStates(string appContentState) {
        AppContentStates.Instance.ChangeState(appContentState);
        GameController.SaveGameStates();
    }//AppContentStates.Instance.ChangeState(AppContentStateMeta.appContentStateGameArcade);
    
    public virtual void loadProfileCharacter(string characterCode) {

        GameProfileCharacters.Current.SetCurrentCharacterCode(characterCode);

        string characterSkinCode = GameProfileCharacters.Current.GetCurrentCharacterCostumeCode();
        GameController.CurrentGamePlayerController.LoadCharacter(characterSkinCode);
    
        GameCustomController.Instance.SetCustomColorsPlayer(
            GameController.CurrentGamePlayerController.gameObject);
    }
    
    public virtual void loadCharacterStartLevel(string characterCode, string levelCode) {
        loadProfileCharacter(characterCode);
        ////GameHUD.Instance.ShowCharacter(characterCode);
        
        GameDraggableEditor.ChangeStateEditing(GameDraggableEditEnum.StateNotEditing);
        startLevel(levelCode);
    }

    public virtual void loadEnemyBot1(float scale, float speed, float attack) {
        GameActorItem character = new GameActorItem();
        character.characterCode = "character-enemy-bot1";
        character.prefabCode = "GameEnemyBot1";
        character.scale = scale;
        character.attack = attack;
        character.speed = speed;
        GameController.LoadActor(character);
    }

    public virtual void loadActor(GameActorItem character) {
        StartCoroutine(loadActorCo(character));
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

    public virtual Vector3 getActorRandomSpawnLocation() {
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

    public virtual string getCharacterModelPath(GameActorItem character) {
        string modelPath = Contents.appCacheVersionSharedPrefabCharacters;
        // TODO load up dater
        modelPath = PathUtil.Combine(modelPath, "GameEnemyBot1");
        return modelPath;
    }

    public virtual string getCharacterType(GameActorItem character) {
        string type = "bot1";
        // TODO load up
        type = "bot1";
        return type;
    }

    public virtual IEnumerator loadActorCo(GameActorItem character) {

        string modelPath = GameController.GetCharacterModelPath(character);
        string characterType = GameController.GetCharacterType(character);

        // TODO data and pooling and network
    
        UnityEngine.Object prefabObject = Resources.Load(modelPath);
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
            spawnLocation = GameController.GetActorRandomSpawnLocation();
        }

        if(prefabObject == null) {
            yield break;
        }
    
        GameObject characterObject = Instantiate(
            prefabObject, spawnLocation, Quaternion.identity) as GameObject;
    
        characterObject.transform.parent = levelActorsContainerObject.transform;

        GameCustomController.Instance.SetCustomColorsEnemy(characterObject);
    
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

        GameController.ResetRuntimeData();
        GameController.ResetCurrentGamePlayer();

        GameController.ResetLevelEnemies();
        GameUIController.HideGameCanvas();

        GameDraggableEditor.ClearLevelItems(levelItemsContainerObject);
        GameDraggableEditor.ResetCurrentGrabbedObject();
        GameDraggableEditor.HideAllEditDialogs();
    }

    public virtual void resetRuntimeData() {
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
        GameController.StartGame(GameLevels.Current.code);
    }

    public virtual void startGame(string levelCode) {
        GameController.ChangeGameState(GameStateGlobal.GameStarted);
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
        GameAIController.Instance.runDirector = run;
        GameItemController.Instance.runDirector = run;
    }

    // -------------------------------------------------------
    // GAME STATES / HANDLERS   
    
    public virtual void gameRunningStateStopped() {
        Time.timeScale = 1f;
        gameRunningState = GameRunningState.STOPPED;
        GameController.Instance.gameState = GameStateGlobal.GameNotStarted;
    }
    
    public virtual void gameRunningStatePause() {
        Time.timeScale = 0f;
        gameRunningState = GameRunningState.PAUSED;
        GameController.Instance.gameState = GameStateGlobal.GamePause;
    }
    
    public virtual void gameRunningStateRun() {
        Time.timeScale = 1f;
        gameRunningState = GameRunningState.RUNNING;
        GameController.Instance.gameState = GameStateGlobal.GameStarted;
    }

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
        else if(contentDisplayCode == GameContentDisplayTypes.gamePlayerOutOfBounds) {

            GameController.GamePlayerOutOfBoundsDelayed(2f);
    
            UIPanelDialogBackground.ShowDefault();
            UIPanelDialogDisplay.SetTitle("OUT OF BOUNDS");
            UIPanelDialogDisplay.SetDescription("RUN, BUT STAY IN BOUNDS...");
            UIPanelDialogDisplay.ShowDefault();
        }
        else {
            UIPanelDialogBackground.HideAll();
        }
    
        //GameRunningStateRun();
    }
    
    public virtual void onGameStarted() {

        GameController.StartLevelStats();
    
        GameController.ResetRuntimeData();
    
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
        // Show dialog then dismiss to not started...
        GameController.Reset();

        GameController.GameRunningStateStopped();
    
        GameController.StopDirectors();
    
        //ChangeGameState(GameStateGlobal.GameNotStarted);
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

        GameController.ProcessLevelStats();
        //// Process stats
        //StartCoroutine(processLevelStatsCo());
    
        GameController.StopDirectors();
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

    // STATS HANDLING

    public virtual void processLevelStats() {
        StartCoroutine(processLevelStatsCo());
    }

    public virtual IEnumerator processLevelStatsCo() {
         
        yield return new WaitForSeconds(.5f);
    
        double score = currentGamePlayerController.runtimeData.score;
    
        //int ammo = runtimeData.ammo;
        //double currentLevelTime = runtimeData.currentLevelTime;
        //double ammoScore = ammo * 10;
        double totalScore = score; //(ammoScore + score);// * currentLevelTime * .5f;
    
        //GameUIPanelResults.Instance.SetScore(score.ToString("N0"));
        //GameUIPanelResults.Instance.SetAmmo(ammo.ToString("N0") + " x 10 = " + ammoScore.ToString("N0"));
        //GameUIPanelResults.Instance.SetAmmoScore(ammoScore.ToString("N0"));
        //GameUIPanelResults.Instance.SetLevelCode(runtimeData.levelCode);
        //GameUIPanelResults.Instance.SetLevelDisplayName(runtimeData.levelCode);
        //GameUIPanelResults.Instance.SetTotalScore(totalScore.ToString("N0"));
    
        GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.score, totalScore);
    
        yield return new WaitForEndOfFrame();
    
        GamePlayerProgress.Instance.SetStatHigh(GameStatCodes.score, totalScore);
        
        GameUIPanelResults.Instance.UpdateDisplay(currentGamePlayerController.runtimeData, 0f);
        
        //if(runtimeData.localPlayerWin) {
        //  GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.wins, 1f);
        //}
        //else {
        //  GamePlayerProgress.Instance.SetStatTotal(GameStatCodes.losses, 1f);
        //}
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
        GamePlayerProgress.Instance.ProcessProgressAction("level-" + GameLevels.Current.code);
    }
    
    public virtual void endLevelStats() {
        GamePlayerProgress.Instance.EndProcessProgressPack("default");
        GamePlayerProgress.Instance.EndProcessProgressAction("level-" + GameLevels.Current.code);
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

        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();

        if(currentGamePlayerController != null) {
            var playerPos = currentGamePlayerController.transform.position;
            var touchPos = Camera.mainCamera.ScreenToWorldPoint(point);
    
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
                var directionAllowNormal = directionAllow.normalized;
    
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
            data.rangeRotation = Vector2.zero.WithX(.7f).WithY(1.2f);
            data.rangeRotation = Vector2.zero.WithX(-.1f).WithY(.1f);

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
            data.rangeRotation = Vector2.zero.WithX(.7f).WithY(1.2f);
            data.rangeRotation = Vector2.zero.WithX(-.1f).WithY(.1f);

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
    
        //bool controlInputTouchFinger = GameProfiles.Current.GetControlInputTouchFinger();
        //bool controlInputTouchOnScreen = GameProfiles.Current.GetControlInputTouchOnScreen();
        
        if(isGameRunning) {
            GameController.CheckForGameOver();
        }
        
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
            if(currentGamePlayerController != null) {
                currentGamePlayerController.thirdPersonController.verticalInput = 0f;
                currentGamePlayerController.thirdPersonController.horizontalInput = 0f;
                currentGamePlayerController.thirdPersonController.verticalInput2 = 0f;
                currentGamePlayerController.thirdPersonController.horizontalInput2 = 0f;
            }
        }
        //}
        
        if(gameState == GameStateGlobal.GamePause
        || GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            return;
        }
        
        currentTimeBlock += Time.deltaTime;     
        
        if(currentTimeBlock > actionInterval) {
            currentTimeBlock = 0.0f;
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