using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;

// This is the Item Director.

public enum GameItemDifficulty {
    EASY,
    NORMAL,
    HARD,
    EPIC
}

public class GameItemDirectorMessages {
    public static string gameItemDirectorSpawnItem = "game-item-director-spawn-item";
}

public class GameItemDirectorData {
    public float speed = .3f;
    public float attack = .3f;
    public float scale = .3f;
    public int randomValue = 1;
    public float currentSpawnAmount = .3f;
    public GamePlayerItemType itemType = GamePlayerItemType.Coin;

    public GameProfileRPGItem rpg;

    public GameItemDirectorData() {
        Reset();
    }

    public void Reset() {
        randomValue = SetRandomValue(1, 2);
        rpg = new GameProfileRPGItem();
        speed = .3f;
        attack = .3f;
        scale = .3f;
        currentSpawnAmount = .3f;
    }

    public int SetRandomValue(int min, int max) {
        return UnityEngine.Random.Range(min, max);
    }
}

public class BaseItemController : MonoBehaviour {
 
    public static BaseItemController BaseInstance;
    public bool runDirector = false;
    public bool stopDirector = false;
    public float currentDifficultyLevel = .1f;
    public float difficultyLevelEasy = .1f;
    public float difficultyLevelNormal = .5f;
    public float difficultyLevelHard = .9f;
    public float difficultyLevelEpic = .99f;
    public float currentSpawnAmount = 2;
    public float currentItemMin = 3;
    public float currentItemLimit = 18;
    public float currentFPS = 0;
    public int roundsCompleted = 0;
    float lastPeriodicSeconds = 0f;
    float currentItemCount = 0;
    public GameItemDifficulty difficultyLevelEnum = GameItemDifficulty.EASY;

    public static bool isBaseInst {
        get {
            if(BaseInstance != null) {
                return true;
            }
            return false;
        }
    }
 
    void Awake() {

        if(BaseInstance != null && this != BaseInstance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
    
        BaseInstance = this;
    
        // Init();
    }

    public virtual void Start() {

    }

    public virtual void init() {

    }

    public virtual void run(bool run) {
        runDirector = run;
    }
 
    public virtual void run() {
        runDirector = true;
    }

    public virtual void stop() {
        runDirector = false;
    }

    public virtual void setDifficultyLevel(GameItemDifficulty difficultyTo) {
        difficultyLevelEnum = difficultyTo;
    }
 
    //

    public virtual void setDifficultyLevel(float difficultyTo) {
        currentDifficultyLevel = difficultyTo;
    }
 
    //
 
    public virtual GameItemDifficulty getDifficultyLevelEnumFromValue(float difficultyCheck) {
     
        GameItemDifficulty difficultyType = GameItemDifficulty.EASY;
     
        if(difficultyCheck >= difficultyLevelEpic) {
            difficultyType = GameItemDifficulty.EPIC;
        }
        else if(difficultyCheck >= difficultyLevelHard) {
            difficultyType = GameItemDifficulty.HARD;
        }
        else if(difficultyCheck >= difficultyLevelNormal) {
            difficultyType = GameItemDifficulty.NORMAL;
        }
     
        return difficultyType;
    }
 
    //
 
    public virtual float getDifficultyLevelValueFromEnum(float difficultyCheck) {
     
        if(difficultyLevelEnum == GameItemDifficulty.EPIC) {
            return difficultyLevelEpic;
        }
        else if(difficultyLevelEnum == GameItemDifficulty.HARD) {
            return difficultyLevelHard;
        }
        else if(difficultyLevelEnum == GameItemDifficulty.NORMAL) {
            return difficultyLevelNormal;
        }
     
        return .3f;
    }
 
    //

    public virtual void direct() {
     
        currentFPS = FPSDisplay.GetCurrentFPS();    
     
        if((currentItemCount < currentItemLimit
         && currentFPS > 20f) || currentItemCount < currentItemMin) {
     
            // do some spawning
         
            if(currentItemCount < currentItemMin * 2) {
                currentSpawnAmount = 1;
            }
         
            int randomValue = UnityEngine.Random.Range(0, 50);

            float speed = .3f;
            float attack = .3f;
            float scale = 1f;
    
            scale = UnityEngine.Random.Range(.7f, 1.5f);
            speed = UnityEngine.Random.Range(.7f, 1.2f);

            GameItemDirectorData item = new GameItemDirectorData();
            item.SetRandomValue(1, randomValue);
            item.speed = speed;
            item.attack = attack;
            item.scale = scale;

            if(randomValue > 0 && randomValue < 20) {
                item.itemType = GamePlayerItemType.Coin;
            }
            else if(randomValue > 20 && randomValue < 22) {
                item.itemType = GamePlayerItemType.Health;
            }

            GameItemController.LoadItem(item);
        }
     
    }

    /*
    public virtual void loadItem(GameItemDirectorData itemData) {
        StartCoroutine(LoadItemCo(itemData));
    }
 
    public IEnumerator LoadItemCo(GameItemDirectorData itemData) {
        string modelPath = Contents.appCacheVersionSharedPrefabLevelItems;
        string characterType = "coin";
             
        if(type == GamePlayerItemType.Coin) {
            modelPath = Path.Combine(modelPath, "GamePlayerItemCoin");
            characterType = "coin";
        }
        else if(type == GamePlayerItemType.Health) {
            modelPath = Path.Combine(modelPath, "GamePlayerItemHealth");
            characterType = "health";
        }
     
        // TODO data and pooling and network
     
        UnityEngine.Object prefabObject = Resources.Load(modelPath);
        Vector3 spawnLocation = Vector3.zero;
        Vector3 currentPlayerPosition = Vector3.zero;

        if(GameController.CurrentGamePlayerController != null) {
            if(GameController.CurrentGamePlayerController.gameObject != null) {
                currentPlayerPosition = GameController.CurrentGamePlayerController.gameObject.transform.position;
            }
        }
        spawnLocation.z = UnityEngine.Random.Range(currentPlayerPosition.z - 5f, currentPlayerPosition.z + 5f); 
        spawnLocation.x = UnityEngine.Random.Range(currentPlayerPosition.x - 20f, currentPlayerPosition.x + 20f); 
        spawnLocation.y = currentPlayerPosition.y + 100f;
     
        spawnLocation = GameController.FilterBounds(spawnLocation);
     
        Debug.Log("characterType:" + characterType);
     
        if(prefabObject == null) {
            yield break;
        }       
     
        GameObject itemObject = Instantiate(prefabObject, spawnLocation, Quaternion.identity) as GameObject;        
        itemObject.transform.parent = GameController.Instance.itemContainerObject.transform;
     
        //GamePlayerController characterGamePlayerController = itemObject.GetComponentInChildren<GamePlayerController>();
        //characterGamePlayerController.transform.localScale = characterGamePlayerController.transform.localScale * character.scale;
     
        // Wire up ai controller to setup player health, speed, attack etc.
     
        // 
        //characterGamePlayerController.runtimeData.
     
     
     
        //if(characterGamePlayerController != null) {
        //  characterObject.Hide();
        //  yield return new WaitForEndOfFrame();
        // wire up properties
         
         
        // TODO network and player target
        //characterGamePlayerController.currentTarget = GameController.CurrentGamePlayerController.gameObject.transform;            
        //characterGamePlayerController.ChangeContextState(GamePlayerContextState.ContextFollowAgent);
        //characterGamePlayerController.ChangePlayerState(GamePlayerControllerState.ControllerAgent);           
        //  characterObject.Show();
         
        // Add indicator to HUD
         
        //  GameHUD.Instance.AddIndicator(characterObject, characterType);
         
        //characterGamePlayerController.Init(GamePlayerControllerState.ControllerAgent);
        //}
     
        //GameEnemyGoblin
        //GameEnemyRobotDinosaur
        //GameEnemyZombie
    }
    */
 
    public virtual void spawnItem(GamePlayerItemType type) {
        GameObject go = null;
     
        if(type == GamePlayerItemType.Coin) {
            if(go != null) {
                //go = 
            }
        }
     
        // Position
     
        // Get boundaries
     
        //Vector3
        //
     
    }

    // MESSAGING

    public virtual void broadcastItemMessage(GameItemDirectorData item) {
        Messenger<GameItemDirectorData>.Broadcast(GameItemDirectorMessages.gameItemDirectorSpawnItem, item);
    }

    public virtual void loadItem(GameItemDirectorData item) {
        GameItemController.BroadcastItemMessage(item);
    }

    // DATA
    
    public virtual void incrementRoundsCompleted() {
        roundsCompleted += 1;
    }

    // UPDATE/TICK

    public virtual void handlePeriodic() {

        if(Time.time > lastPeriodicSeconds + 3f) {
            lastPeriodicSeconds = Time.time;
            // every second
            GameItemController.Direct();
        }
    }
    
    public virtual void handleUpdate() {
        // do on update always
    
        currentItemCount = GameController.Instance.collectableItemsCount;
    }

    public virtual void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        if(Input.GetKey(KeyCode.RightControl)) {
            if(Input.GetKey(KeyCode.G)) {
                runDirector = false;
            }
            else if(Input.GetKey(KeyCode.H)) {
                runDirector = true;
            }
            else if(Input.GetKey(KeyCode.J)) {
                // kill all enemies

                GameController.Instance.levelItemsContainerObject.DestroyChildren();
            }
        }

        if(stopDirector) {
            return;
        }
    
        if(!runDirector || stopDirector
        || GameDraggableEditor.isEditing) {
            return;
        }
    
        if(GameController.IsGameRunning) {
            // if game running spawn and direct characters and events
    
            GameItemController.HandlePeriodic();
    
            GameItemController.HandleUpdate();
    
        }
    }

}

