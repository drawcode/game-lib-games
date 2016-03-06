using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;

// This is the AI Director.

public class GameAIDirectorMessages {
    public static string gameAIDirectorSpawnActor = "game-ai-director-spawn-actor";
}

public enum GameAIDifficulty {
    EASY,
    NORMAL,
    HARD,
    EPIC
}

public class GameAIDirectorData {
    
    public string code = "";
    public string type = "";
    public double currentSpawnAmount = 1;
    public float speed = .3f;
    public float attack = .3f;
    public float scale = .3f;

    public GameAIDirectorData() {
        Reset();
    }

    public void Reset() {
        code = "";
    }
}

public enum GameAICharacterGenerateType {
    probabalistic,
    team
}

public class BaseAIController : GameObjectBehavior {

    public static BaseAIController BaseInstance;
    //
    public bool runDirector = false;
    public bool stopDirector = false;
    //
    public float currentDifficultyLevel = .1f;
    //
    public float difficultyLevelEasy = .1f;
    public float difficultyLevelNormal = .5f;
    public float difficultyLevelHard = .9f;
    public float difficultyLevelEpic = .99f;
    //
    public float currentCharacterTypeCount = 2; // TODO change to characters data
    //
    public double spawnEnemyAmount = 1;
    public double spawnEnemyMin = 5;
    public double spawnEnemyLimit = 11;
    public double spawnEnemyCount = 0;
    //
    public double spawnSidekickAmount = 1;
    public double spawnSidekickMin = 3;
    public double spawnSidekickLimit = 11;
    public double spawnSidekickCount = 0;

    //
    public float currentFPS = 0;
    //
    public int roundsCompleted = 0;
    //
    public float lastPeriodicSeconds = 0f;
    //
    public float spawnTimeRangeMin = 3.2f;
    public float spawnTimeRangeLimit = 9.3f;
    //
    //
    public GameAIDifficulty difficultyLevelEnum = GameAIDifficulty.EASY;
    //
    public Dictionary<string, GamePlayerSpawn> spawns;
    public static GameAICharacterGenerateType generateType = GameAICharacterGenerateType.probabalistic;

    // ----------------------------------------------------------------------

    public static bool isBaseInst {
        get {
            if (BaseInstance != null) {
                return true;
            }
            return false;
        }
    }

    public virtual void Awake() {

        if (BaseInstance != null && this != BaseInstance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }

        BaseInstance = this;
    }

    public virtual void Start() {

    }

    public virtual void init() {
        GameAIController.CheckSpawns();
    }

    public virtual void checkSpawns() {
        if (spawns == null) {
            spawns = new Dictionary<string, GamePlayerSpawn>();

            GameObject containerSpawns = GameController.Instance.levelSpawnsContainerObject;

            if (containerSpawns != null) {
                foreach (GamePlayerSpawn spawn
                    in containerSpawns.GetComponentsInChildren<GamePlayerSpawn>(true)) {
                    if (!spawns.ContainsKey(spawn.code)) {
                        spawns.Add(spawn.code, spawn);
                    }
                }
            }
        }
    }

    public virtual GamePlayerSpawn getSpawn(string code) {

        GameAIController.CheckSpawns();

        foreach (KeyValuePair<string, GamePlayerSpawn> pair in spawns) {
            if (pair.Key == code) {
                return pair.Value;
            }
        }
        return null;
    }
    
    // PRELOAD
    
    public virtual void preload() {
        StartCoroutine(preloadCo());
    }
    
    public virtual IEnumerator preloadCo() {
        
        yield return new WaitForEndOfFrame();        
        
        GamePreset preset = GamePresets.Instance.GetCurrentPresetDataCharacter();
        
        if (preset == null) {
            yield break;
        }
        
        List<GamePresetItem> presetItems = preset.data.items;

        foreach (GamePresetItem item in presetItems) {

            for (int i = 0; i < item.limit / 3; i++) {
                yield return new WaitForEndOfFrame();

                GameAIController.Load(item.code);
            }
        }
                
        yield return new WaitForSeconds(1f);

        // remove all characters

        GameController.ResetLevelActors();

        yield return new WaitForEndOfFrame();
    }

    public virtual void load(string code, string type = "enemy") {

        // Load by character code
        
        float speed = .3f;
        float attack = .3f;
        float scale = 1f;
        
        speed = UnityEngine.Random.Range(.8f, 1.6f);
        attack = UnityEngine.Random.Range(.3f, .4f);
        scale = UnityEngine.Random.Range(.8f, 1.6f);
                
        GameAIController.Load(code, type, speed, attack, scale);
    }

    public virtual void load(
        string code, 
        string type, 
        float speed, 
        float attack, 
        float scale) {
        // Load by character code

        GameAIDirectorData itemData = new GameAIDirectorData();
        itemData.code = code;
        itemData.type = type;
        itemData.speed = speed;
        itemData.attack = attack;
        itemData.scale = scale;
        
        GameAIController.LoadCharacter(itemData);
    }

    // RUNNER/STOPPER

    public virtual void run(bool run) {
        runDirector = run;
    }

    public virtual void run() {
        run(true);
    }
    
    public virtual void stop() {
        runDirector = false;
    }
    
    public virtual void setDifficultyLevel(GameAIDifficulty difficultyTo) {
        difficultyLevelEnum = difficultyTo;
    }
    
    public void setDifficultyLevel(float difficultyTo) {
        currentDifficultyLevel = difficultyTo;
    }
    
    public GameAIDifficulty getDifficultyLevelEnumFromValue(float difficultyCheck) {
    
        GameAIDifficulty difficultyType = GameAIDifficulty.EASY;

        if (difficultyCheck >= difficultyLevelEpic) {
            difficultyType = GameAIDifficulty.EPIC;
        }
        else if (difficultyCheck >= difficultyLevelHard) {
            difficultyType = GameAIDifficulty.HARD;
        }
        else if (difficultyCheck >= difficultyLevelNormal) {
            difficultyType = GameAIDifficulty.NORMAL;
        }
    
        return difficultyType;
    }

    public float getDifficultyLevelValueFromEnum(float difficultyCheck) {

        if (difficultyLevelEnum == GameAIDifficulty.EPIC) {
            return difficultyLevelEpic;
        }
        else if (difficultyLevelEnum == GameAIDifficulty.HARD) {
            return difficultyLevelHard;
        }
        else if (difficultyLevelEnum == GameAIDifficulty.NORMAL) {
            return difficultyLevelNormal;
        }
        else if (difficultyLevelEnum == GameAIDifficulty.EASY) {
            return difficultyLevelEasy;
        }
    
        return .3f;
    }

    public virtual void directAI() {

        if (!runDirector) {
            return;
        }

        directAIEnemies();

        directAISidekicks();
    }
    
    List<GameDataCharacterPreset> characters = null;
    List<GamePresetItem> presetItemsAppend = null;
    List<float> probs = null;

    public virtual void directAICharacters(
        string actorType, 
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
            
            if(characters == null) {
                characters = new List<GameDataCharacterPreset>();
            }
            else {
                characters.Clear();
            }

            if (GameLevels.Current.data != null && GameLevels.Current.data.HasCharacterPresets()) {
                characters.AddRange(GameLevels.Current.data.character_presets);
            }            
            else if (GameWorlds.Current.data != null && GameWorlds.Current.data.HasCharacterPresets()) {
                characters.AddRange(GameWorlds.Current.data.character_presets);
            }

            if(presetItemsAppend == null) {
                presetItemsAppend = new List<GamePresetItem>();
            }
            else {
                presetItemsAppend.Clear();
            }

            if(probs == null) {
                probs = new List<float>();
            }
            else {
                probs.Clear();
            }

            foreach (GameDataCharacterPreset characterPreset in characters) {
                
                GamePreset preset = GamePresets.Get(characterPreset.code);
                //GamePresets.Instance.GetCurrentPresetDataCharacter();
                
                if (preset == null) {
                    return;
                }
                
                List<GamePresetItem> presetItems = preset.data.items;

                foreach (GamePresetItem item in presetItems) {
                    if (item.type == actorType) {
                        probs.Add((float)item.probability);
                    }
                }
            }
            
            //string characterCode = "";            
            
            /*
            if (!IsPlayerControlled) {
                // apply team 
                
                GameTeam team = GameTeams.Current;
                
                // TODO randomize
                
                if (team != null) {
                    if (team.data != null) {
                        
                        GameDataModel item = team.data.GetModel();
                        
                        if (item != null) {
                            characterCode = item.code;
                            controllerData.lastCharacterCode = item.code;                    
                        }
                    }
                }            
            }
            */
            
            GamePresetItem selectByProbabilityItem = 
                MathUtil.ChooseProbability<GamePresetItem>(presetItemsAppend, probs); 
            
            if (selectByProbabilityItem == null) {
                return;
            }
            
            string code = selectByProbabilityItem.code;
            
            GameAIController.Load(code, actorType);
        }
    }

    public virtual void directAIEnemies() {

        directAICharacters(
            GameActorType.enemy, 
            spawnEnemyCount, 
            spawnEnemyMin, 
            spawnEnemyLimit, true);
    }
        
    public virtual void directAISidekicks() {
        
        directAICharacters(
            GameActorType.sidekick, 
            spawnSidekickCount, 
            spawnSidekickMin, 
            spawnSidekickLimit, false);
    }

    // MESSAGING

    public virtual void broadcastCharacterMessage(GameAIDirectorData actor) {
        Messenger<GameAIDirectorData>.Broadcast(GameAIDirectorMessages.gameAIDirectorSpawnActor, actor);
    }

    public virtual void loadCharacter(GameAIDirectorData actor) {
        GameAIController.BroadcastCharacterMessage(actor);
    }

    // DATA

    public virtual void updateDirector(GameDataDirector director) {

        if (director != null) {
            
            if (director.code == GameDataDirectorType.ai) {

                if (director.min > 0) {
                    spawnEnemyMin = director.min;                        
                }
                
                if (director.max > 0) {
                    spawnEnemyLimit = director.max;                        
                }
            }
        }
    }

    public virtual void incrementRoundsCompleted() {
        roundsCompleted += 1;
    }

    // UPDATE/TICK

    public virtual void handlePeriodic() {

        if (lastPeriodicSeconds > UnityEngine.Random.Range(spawnTimeRangeMin, spawnTimeRangeLimit)) {
            lastPeriodicSeconds = 0f;
            GameAIController.DirectAI();
        }

        lastPeriodicSeconds += Time.deltaTime;
    }

    public virtual void handleUpdate() {
        // do on update always
    
        spawnEnemyCount = GameController.Instance.characterActorEnemyCount;
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

                GameController.Instance.levelActorsContainerObject.DestroyChildren();
            }
        }

        if (stopDirector) {
            return;
        }
    
        if (!runDirector || stopDirector
            || GameDraggableEditor.isEditing) {
            return;
        }
    
        if (GameController.IsGameRunning) {
            // if game running spawn and direct characters and events
    
            GameAIController.HandlePeriodic();
    
            GameAIController.HandleUpdate();
    
        }
    }

}