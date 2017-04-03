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

public class GameItemData : GameDataObject {

    public GameItemData() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        code = "item-coin";
        type = BaseDataObjectKeys.item;
        data_type = GameSpawnType.randomType;
        position_data = new Vector3Data(0, 0, 0);
        scale_data = new Vector3Data(1, 1, 1);
        rotation_data = new Vector3Data(0, 0, 0);
    }
}

public class BaseItemController : GameObjectBehavior, IBaseItemController {
 
    public static BaseItemController BaseInstance;
    //
    public bool runDirector = false;
    //
    public bool runDirectorItems = false;
    //
    public bool runDirectorWeapons = false;
    //
    public float currentDifficultyLevel = .1f;
    //
    public float difficultyLevelEasy = .1f;
    public float difficultyLevelNormal = .5f;
    public float difficultyLevelHard = .9f;
    public float difficultyLevelEpic = .99f;
    //
    //
    public double spawnItemAmount = 1;
    public double spawnItemMin = 3;
    public double spawnItemLimit = 8;
    public double spawnItemCount = 0;
    //
    public double spawnWeaponAmount = 1;
    public double spawnWeaponMin = 1;
    public double spawnWeaponLimit = 2;
    public double spawnWeaponCount = 0;

    //
    public double currentFPS = 0;
    //
    public int roundsCompleted = 0;
    //
    public float lastPeriodicSeconds = 0f;
    //
    public float spawnTimeRangeMin = 3.2f;
    public float spawnTimeRangeLimit = 9.3f;
    //
    public GameItemDifficulty difficultyLevelEnum = GameItemDifficulty.EASY;
    //public Dictionary<string, GamePlayerSpawn> spawns;
    //public static GameAICharacterGenerateType generateType = GameAICharacterGenerateType.probabalistic;
    public List<GameDataItemPreset> presetItems = null;
    public List<GamePresetItem> presetItemsAppend = null;
    public List<float> presetItemProbabilities = null;
    //
    public List<GameDataWeaponPreset> presetWeapons = null;
    public List<GamePresetItem> presetWeaponsAppend = null;
    public List<float> presetWeaponProbabilities = null;
    //
    public static bool isBaseInst {
        get {
            if (BaseInstance != null) {
                return true;
            }
            return false;
        }
    }

    void Awake() {

        if (BaseInstance != null && this != BaseInstance) {
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
    
    // PRELOAD
    
    public virtual void preload() {
        StartCoroutine(preloadCo());
    }

    public virtual IEnumerator preloadCo() {
        
        //yield return new WaitForEndOfFrame();        
        
        GamePreset preset = GamePresets.Instance.GetCurrentPresetDataItem();
        
        if (preset == null) {
            yield break;
        }
        
        List<GamePresetItem> presetItems = preset.data.items;
        
        foreach (GamePresetItem item in presetItems) {
            
            for (int i = 0; i < item.limit / 3; i++) {
                //yield return new WaitForEndOfFrame();
                
                GameItemController.Load(item.code);
            }
        }

        yield return new WaitForEndOfFrame();

        // remove all characters

        GameController.ResetLevelActors();
        
        //yield return new WaitForEndOfFrame();
    }

    public virtual void load(string code) {
        // Load by character code
        
        //float speed = 1f;
        //float attack = 1f;
        //float scale = 1f;
        
        //speed = UnityEngine.Random.Range(.8f, 1.6f);
        //attack = UnityEngine.Random.Range(.3f, .4f);
        //scale = UnityEngine.Random.Range(.8f, 1.6f);
        
        GameItemData itemData = new GameItemData();
        itemData.code = code;
        //itemData.type = GameActorType.enemy;
        //itemData.speed = speed;
        //itemData.attack = attack;
        //itemData.scale = scale;
        
        GameItemController.LoadItem(itemData);
    }
    // -------------------------------------------------

    // RUNNER/STOPPER

    // ALL

    public virtual void run(bool run) {
        runDirector = run;
    }

    public virtual void run() {
        run(true);
    }

    public virtual void stop() {
        run(false);
    }

    // RUN SIDEKICKS

    public virtual void runItems(bool run) {
        runDirectorItems = run;
    }

    public virtual void runItems() {
        runItems(true);
    }

    public virtual void stopItems() {
        runItems(false);
    }

    // RUN ENEMIES

    public virtual void runWeapons(bool run) {
        runDirectorWeapons = run;
    }

    public virtual void runWeapons() {
        runWeapons(true);
    }

    public virtual void stopWeapons() {
        runWeapons(false);
    }

    // -------------------------------------------------

    // LEVELS

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
     
        if (difficultyCheck >= difficultyLevelEpic) {
            difficultyType = GameItemDifficulty.EPIC;
        }
        else if (difficultyCheck >= difficultyLevelHard) {
            difficultyType = GameItemDifficulty.HARD;
        }
        else if (difficultyCheck >= difficultyLevelNormal) {
            difficultyType = GameItemDifficulty.NORMAL;
        }
     
        return difficultyType;
    }
 
    //
 
    public virtual float getDifficultyLevelValueFromEnum(float difficultyCheck) {
     
        if (difficultyLevelEnum == GameItemDifficulty.EPIC) {
            return difficultyLevelEpic;
        }
        else if (difficultyLevelEnum == GameItemDifficulty.HARD) {
            return difficultyLevelHard;
        }
        else if (difficultyLevelEnum == GameItemDifficulty.NORMAL) {
            return difficultyLevelNormal;
        }
     
        return .3f;
    }

    // -------------------------------------------------

    // DIRECTION


    public virtual void direct() {

        if (!runDirector) {
            return;
        }

        if (runDirectorItems) {        
            directItems();
        }

        if (runDirectorWeapons) {        
            directWeapons();
        }
    }

    /*
    public virtual void direct() {
     

        currentFPS = FPSDisplay.GetCurrentFPS();    
     
        if ((currentItemCount < spawnLimit
            && currentFPS > 20f) || currentItemCount < spawnMin) {
     
            // do some spawning
         
            if (currentItemCount < spawnMin * 2) {
                spawnAmount = 1;
            }

            GamePreset preset = GamePresets.Instance.GetCurrentPresetDataItem();
            
            if (preset == null) {
                return;
            }

            List<GamePresetItem> presetItems = preset.data.items;

            List<float> probs = new List<float>();
            foreach (GamePresetItem item in presetItems) {
                probs.Add((float)item.probability);
            }
                        
            GamePresetItem selectByProbabilityItem = 
                MathUtil.ChooseProbability<GamePresetItem>(presetItems, probs); 
            
            if (selectByProbabilityItem == null) {
                return;
            }
            
            string code = selectByProbabilityItem.code;
            
            GameItemController.Load(code);
        }
    }

    */

    public virtual void directItems(
        string itemType, 
        double spawnCount, 
        double spawnMin,  
        double spawnLimit,
        bool limitFps = true) {

        currentFPS = FPSDisplay.GetCurrentFPS();  

        // DIRECT ENEMIES

        if ((spawnCount < spawnLimit
            && (currentFPS > 20f || !limitFps))
            || spawnCount < spawnMin) {

            // do some spawning

            if (spawnCount < spawnMin * 2) {
                //spawnAmount = 1;
            }

            if (presetItems == null) {
                presetItems = new List<GameDataItemPreset>();
            }
            else {
                presetItems.Clear();
            }

            if (GameLevels.Current.data != null
                && GameLevels.Current.data.HasItemPresets()) {

                foreach (GameDataItemPreset itemPreset 
                    in GameLevels.Current.data.item_presets) {
                    if (itemPreset.type == itemType) {
                        presetItems.Add(itemPreset);
                    }
                }
            }
            else if (GameWorlds.Current.data != null
                     && GameWorlds.Current.data.HasItemPresets()) {

                foreach (GameDataItemPreset itemPreset 
                    in GameWorlds.Current.data.item_presets) {
                    if (itemPreset.type == itemType) {
                        presetItems.Add(itemPreset);
                    }
                }
            }

            if (presetItemsAppend == null) {
                presetItemsAppend = new List<GamePresetItem>();
            }
            else {
                presetItemsAppend.Clear();
            }

            if (presetItemProbabilities == null) {
                presetItemProbabilities = new List<float>();
            }
            else {
                presetItemProbabilities.Clear();
            }

            foreach (GameDataItemPreset itemPreset in presetItems) {

                GamePreset preset = GamePresets.Get(itemPreset.code);
                //GamePresets.Instance.GetCurrentPresetDataCharacter();

                if (preset == null) {
                    return;
                }

                List<GamePresetItem> presetItemsData = preset.data.items;

                foreach (GamePresetItem item in presetItemsData) {
                    if (item.type == itemType) {
                        presetItemProbabilities.Add((float)item.probability);
                        presetItemsAppend.Add(item);
                    }
                }
            }

            if(presetItemsAppend == null 
                || presetItemProbabilities == null) {
                return;
            }

            if(presetItemsAppend.Count == 0 
                || presetItemProbabilities.Count == 0) {
                return;
            }

            GamePresetItem selectByProbabilityItem = 
                MathUtil.ChooseProbability<GamePresetItem>(
                    presetItemsAppend, presetItemProbabilities); 

            if (selectByProbabilityItem == null) {
                return;
            }

            string code = selectByProbabilityItem.code;

            GameItemController.Load(code);//, itemType);
        }
    }

    public virtual void directItems() {

        directItems(
            GameItemType.item, 
            spawnItemCount, 
            spawnItemMin, 
            spawnItemLimit, true);
    }

    public virtual void directWeapons(
        string itemType, 
        double spawnCount, 
        double spawnMin,  
        double spawnLimit,
        bool limitFps = true) {

        currentFPS = FPSDisplay.GetCurrentFPS();  

        // DIRECT ENEMIES

        if ((spawnCount < spawnLimit
            && (currentFPS > 20f || !limitFps))
            || spawnCount < spawnMin) {

            // do some spawning

            if (spawnCount < spawnMin * 2) {
                //spawnAmount = 1;
            }

            if (presetWeapons == null) {
                presetWeapons = new List<GameDataWeaponPreset>();
            }
            else {
                presetWeapons.Clear();
            }

            Debug.Log("directWeapons"); 

            if (GameLevels.Current.data != null
                && GameLevels.Current.data.HasWeaponPresets()) {

                Debug.Log("directWeapons:HasWeaponPresets:level"); 

                foreach (GameDataWeaponPreset weaponPreset 
                    in GameLevels.Current.data.weapon_presets) {
                    if (weaponPreset.type == itemType) {
                        presetWeapons.Add(weaponPreset);
                    }
                }
            }
            else if (GameWorlds.Current.data != null
                && GameWorlds.Current.data.HasWeaponPresets()) {

                Debug.Log("directWeapons:HasWeaponPresets:world"); 

                foreach (GameDataWeaponPreset weaponPreset 
                    in GameWorlds.Current.data.weapon_presets) {
                    if (weaponPreset.type == itemType) {
                        presetWeapons.Add(weaponPreset);
                    }
                }
            }

            if (presetWeaponsAppend == null) {
                presetWeaponsAppend = new List<GamePresetItem>();
            }
            else {
                presetWeaponsAppend.Clear();
            }

            if (presetWeaponProbabilities == null) {
                presetWeaponProbabilities = new List<float>();
            }
            else {
                presetWeaponProbabilities.Clear();
            }

            foreach (GameDataWeaponPreset itemPreset in presetWeapons) {
                
                Debug.Log("directWeapons:GameDataWeaponPreset: " + itemPreset.code); 

                GamePreset preset = GamePresets.Get(itemPreset.code);
                //GamePresets.Instance.GetCurrentPresetDataCharacter();

                if (preset == null) {
                    return;
                }

                Debug.Log("directWeapons:GamePreset:code: " + preset.code); 
                Debug.Log("directWeapons:GamePreset:type: " + preset.type);
                Debug.Log("directWeapons:GamePreset:data: " + preset.ToJson());  

                List<GamePresetItem> presetItemsData = preset.data.items;

                foreach (GamePresetItem item in presetItemsData) {
                    
                    Debug.Log("directWeapons:GamePresetItem:code: " + item.code); 
                    Debug.Log("directWeapons:GamePresetItem:type: " + item.type); 
                    Debug.Log("directWeapons:GamePresetItem:data: " + item.ToJson()); 

                    if (itemPreset.type == itemType) {
                        presetWeaponProbabilities.Add((float)item.probability);
                        presetWeaponsAppend.Add(item);

                        Debug.Log("directWeapons:GamePresetItem: " + item.code); 
                    }
                }
            }

            if(presetWeaponsAppend == null 
                || presetWeaponProbabilities == null) {

                Debug.Log("directWeapons:presetWeaponProbabilities:null ");
                return;
            }

            if(presetWeaponsAppend.Count == 0 
                || presetWeaponProbabilities.Count == 0) {

                Debug.Log("directWeapons:presetWeaponProbabilities:empty ");
                return;
            }
                   
            GamePresetItem selectByProbabilityItem = 
                MathUtil.ChooseProbability<GamePresetItem>(
                    presetWeaponsAppend, presetWeaponProbabilities); 

            if (selectByProbabilityItem == null) {

                Debug.Log("directWeapons:selectByProbabilityItem:null ");
                return;
            }

            string code = selectByProbabilityItem.code;

            Debug.Log("directWeapons:selectByProbabilityItem:code: " + 
                code); 

            GameItemController.Load(code);//, itemType);
        }
    }

    public virtual void directWeapons() {

        directWeapons(
            GameItemType.weapon, 
            spawnWeaponCount, 
            spawnWeaponMin, 
            spawnWeaponLimit, false);
    }



    /*
    public virtual void loadItem(GameItemDirectorData itemData) {
        StartCoroutine(LoadItemCo(itemData));
    }
 
    public IEnumerator LoadItemCo(GameItemDirectorData itemData) {
        string modelPath = ContentPaths.appCacheVersionSharedPrefabLevelItems;
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
     
        LogUtil.Log("characterType:" + characterType);
     
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
 
    //public virtual void spawnItem(GamePlayerItemType type) {
          
    // Position
     
    // Get boundaries
     
    //Vector3
    //
     
    //}

    // MESSAGING

    public virtual void broadcastItemMessage(GameItemData item) {
        Messenger<GameItemData>.Broadcast(GameItemDirectorMessages.gameItemDirectorSpawnItem, item);
    }

    public virtual void loadItem(GameItemData item) {
        GameItemController.BroadcastItemMessage(item);
    }

    // DATA
    
    public virtual void updateDirector(GameDataDirector director) {
        
        if (director != null) {
            
            if (director.code == GameDataDirectorType.item) {
                if (director.min > 0) {
                    spawnItemMin = director.min;                        
                }
                
                if (director.max > 0) {
                    spawnItemLimit = director.max;                        
                }

                runDirectorItems = director.run;
            }
            else if (director.code == GameDataDirectorType.weapon) {
                if (director.min > 0) {
                    spawnWeaponMin = director.min;                        
                }

                if (director.max > 0) {
                    spawnWeaponLimit = director.max;                        
                }

                runDirectorWeapons = director.run;
            }
        }
    }

    public virtual void incrementRoundsCompleted() {
        roundsCompleted += 1;
    }

    // UPDATE/TICK

    public virtual void handlePeriodic() {

        if (Time.time > lastPeriodicSeconds + UnityEngine.Random.Range(5, 15)) {
            lastPeriodicSeconds = Time.time;
            // every second
            GameItemController.Direct();
        }
    }

    public virtual void handleUpdate() {
        // do on update always
    
        spawnItemCount = GameController.Instance.itemsCount;
        spawnWeaponCount = GameController.Instance.itemWeaponsCount;
    }

    public virtual void Update() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (Input.GetKey(KeyCode.RightControl)) {
            if (Input.GetKey(KeyCode.G)) {
                runDirector = false;
            }
            else if (Input.GetKey(KeyCode.H)) {
                runDirector = true;
            }
            else if (Input.GetKey(KeyCode.J)) {
                // kill all enemies

                GameController.Instance.levelItemsContainerObject.DestroyChildren();
            }
        }
            
        if (!runDirector
            || GameDraggableEditor.isEditing) {
            return;
        }
    
        if (GameController.IsGameRunning) {
            // if game running spawn and direct characters and events
    
            GameItemController.HandlePeriodic();
    
            GameItemController.HandleUpdate();
    
        }
    }

}

