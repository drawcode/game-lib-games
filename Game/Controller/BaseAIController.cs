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
    public float speed = .3f;
    public float attack = .3f;
    public float scale = .3f;
    public int randomCharacter = 1;
    public float currentSpawnAmount = .3f;
    public string characterCode = "";
    public GameProfileRPGItem actorRPG;

    public GameAIDirectorData() {
        Reset();
    }

    public void Reset() {
        randomCharacter = SetRandomCharacter(2);
        actorRPG = new GameProfileRPGItem();
    }

    public int SetRandomCharacter(int maxCharacters) {
        return UnityEngine.Random.Range(1, maxCharacters);
    }
}

public class BaseAIController : GameObjectBehavior {

    public static BaseAIController BaseInstance;
    public bool runDirector = false;
    public float currentDifficultyLevel = .1f;
    public float difficultyLevelEasy = .1f;
    public float difficultyLevelNormal = .5f;
    public float difficultyLevelHard = .9f;
    public float difficultyLevelEpic = .99f;
    public float currentSpawnAmount = 1;
    public float currentCharacterMin = 5;
    public float currentCharacterTypeCount = 2; // TODO change to characters data
    public float currentCharacterLimit = 11;
    public float currentFPS = 0;
    public int roundsCompleted = 0;
    public float lastPeriodicSeconds = 0f;
    public float currentActorCount = 0;
    public bool stopDirector = false;
    public GameAIDifficulty difficultyLevelEnum = GameAIDifficulty.EASY;
    public Dictionary<string, GamePlayerSpawn> spawns;

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

    // RUNNER/STOPPER

    public virtual void run(bool run) {
        runDirector = run;
    }

    public virtual void run() {
        runDirector = true;
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

        currentFPS = FPSDisplay.GetCurrentFPS();
    
        if ((currentActorCount < currentCharacterLimit
            && currentFPS > 20f)
            || currentActorCount < currentCharacterMin) {
        
            // do some spawning
    
            if (currentActorCount < currentCharacterMin * 2) {
                currentSpawnAmount = 1;
            }

            int randomCharacter = UnityEngine.Random.Range(0, (int)currentCharacterTypeCount);

            float speed = .3f;
            float attack = .3f;
            float scale = 1f;
    
            scale = UnityEngine.Random.Range(.8f, 1.6f);
            speed = UnityEngine.Random.Range(.8f, 1.6f);

            GameAIDirectorData actor = new GameAIDirectorData();
            actor.SetRandomCharacter(randomCharacter);
            actor.speed = speed;
            actor.attack = attack;
            actor.scale = scale;

            GameAIController.LoadCharacter(actor);
        }
    }

    // MESSAGING

    public virtual void broadcastCharacterMessage(GameAIDirectorData actor) {
        Messenger<GameAIDirectorData>.Broadcast(GameAIDirectorMessages.gameAIDirectorSpawnActor, actor);
    }

    public virtual void loadCharacter(GameAIDirectorData actor) {
        GameAIController.BroadcastCharacterMessage(actor);
    }

    // DATA
    
    public virtual void incrementRoundsCompleted() {
        roundsCompleted += 1;
    }

    // UPDATE/TICK

    public virtual void handlePeriodic() {

        if (lastPeriodicSeconds > UnityEngine.Random.Range(3, 10)) {
            lastPeriodicSeconds = 0f;
            GameAIController.DirectAI();
        }

        lastPeriodicSeconds += Time.deltaTime;
    }

    public virtual void handleUpdate() {
        // do on update always
    
        currentActorCount = GameController.Instance.characterActorsCount;
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