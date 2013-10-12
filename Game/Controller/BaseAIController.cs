using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;

// This is the AI Director.

public class GameDirectorMessages {
    public static string gameDirectorSpawnActor = "game-director-spawn-actor";
}

public enum GameAIDifficulty {
    EASY,
    NORMAL,
    HARD,
    EPIC
}


public class GameDirectorActor {
    public float speed = .3f;
    public float attack = .3f;
    public float scale = .3f;
    public int randomCharacter = 1;
    public float currentSpawnAmount = .3f;

    public GameProfileRPGItem actorRPG;

    public GameDirectorActor() {
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

public class BaseAIController : MonoBehaviour {

    public static BaseAIController BaseInstance;

    public bool runDirector = false;
    
    public float currentDifficultyLevel = .1f;

    public float difficultyLevelEasy = .1f;
    public float difficultyLevelNormal = .5f;
    public float difficultyLevelHard = .9f;
    public float difficultyLevelEpic = .99f;
     
    public float currentSpawnAmount = 1;
    public float currentCharacterMin = 3;
    public float currentCharacterTypeCount = 2; // TODO change to characters data
    public float currentCharacterLimit = 0;
    public float currentFPS = 0;
    
    public int roundsCompleted = 0;
    public float lastPeriodicSeconds = 0f;
    public float currentActorCount = 0;
    
    public GameAIDifficulty difficultyLevelEnum = GameAIDifficulty.EASY;
    
    public Dictionary<string, GamePlayerSpawn> spawns;

    // ----------------------------------------------------------------------

    public static bool isBaseInst {
        get {
            if(BaseInstance != null) {
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

        if(spawns == null) {
            spawns = new Dictionary<string, GamePlayerSpawn>();

            GameObject containerSpawns = GameController.Instance.levelSpawnsContainerObject;

            if(containerSpawns != null) {
                foreach(GamePlayerSpawn spawn
                    in containerSpawns.GetComponentsInChildren<GamePlayerSpawn>(true)) {
                    if(!spawns.ContainsKey(spawn.code)) {
                        spawns.Add(spawn.code, spawn);
                    }
                }
            }
        }
    }

    // RUNNER/STOPPER

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

        if(difficultyCheck >= difficultyLevelEpic) {
            difficultyType = GameAIDifficulty.EPIC;
        }
        else if(difficultyCheck >= difficultyLevelHard) {
            difficultyType = GameAIDifficulty.HARD;
        }
        else if(difficultyCheck >= difficultyLevelNormal) {
            difficultyType = GameAIDifficulty.NORMAL;
        }
    
        return difficultyType;
    }

    public float getDifficultyLevelValueFromEnum(float difficultyCheck) {

        if(difficultyLevelEnum == GameAIDifficulty.EPIC) {
            return difficultyLevelEpic;
        }
        else if(difficultyLevelEnum == GameAIDifficulty.HARD) {
            return difficultyLevelHard;
        }
        else if(difficultyLevelEnum == GameAIDifficulty.NORMAL) {
            return difficultyLevelNormal;
        }
        else if(difficultyLevelEnum == GameAIDifficulty.EASY) {
            return difficultyLevelEasy;
        }
    
        return .3f;
    }


    public virtual void directAI() {

        currentFPS = FPSDisplay.GetCurrentFPS();
    
        if((currentActorCount < currentCharacterLimit
        && currentFPS > 20f) || currentActorCount < currentCharacterMin) {
        
            // do some spawning
    
            if(currentActorCount < currentCharacterMin * 2) {
                currentSpawnAmount = 1;
            }

            int randomCharacter = UnityEngine.Random.Range(0, (int)currentCharacterTypeCount);

            float speed = .3f;
            float attack = .3f;
            float scale = 1f;
    
            scale = UnityEngine.Random.Range(.7f, 1.5f);
            speed = UnityEngine.Random.Range(.7f, 1.2f);


            GameDirectorActor actor = new GameDirectorActor();
            actor.SetRandomCharacter(randomCharacter);
            actor.speed = speed;
            actor.attack = attack;
            actor.scale = scale;

            GameAIController.LoadCharacter(actor);

            /*
            if(randomCharacter == 1) {
                for(int i = 0; i < currentSpawnAmount; i++) {
                    GameController.Instance.LoadEnemyBot1(scale, speed, attack);
                }
            }
            else if(randomCharacter == 2) {
                for(int i = 0; i < currentSpawnAmount; i++) {
                    GameController.Instance.LoadEnemyBot1(scale, speed, attack);
                }
            }
            */
        }
    }

    // MESSAGING

    public virtual void broadcastCharacterMessage(GameDirectorActor actor) {
        Messenger<GameDirectorActor>.Broadcast(GameDirectorMessages.gameDirectorSpawnActor, actor);
    }

    public virtual void loadCharacter(GameDirectorActor actor) {
        GameAIController.BroadcastCharacterMessage(actor);
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
            GameAIController.DirectAI();
        }
    }
    
    public virtual void handleUpdate() {
        // do on update always
    
        currentActorCount = GameController.Instance.characterActorsCount;
    }

    bool stopDirector = true;
    
    public virtual void Update() {

        if(Input.GetKey(KeyCode.RightControl)) {
            if(Input.GetKey(KeyCode.G)) {
                runDirector = false;
            }
            else if(Input.GetKey(KeyCode.H)) {
                runDirector = true;
            }
            else if(Input.GetKey(KeyCode.J)) {
                // kill all enemies

                GameController.Instance.levelActorsContainerObject.DestroyChildren();
            }
        }

        if(stopDirector) {
            return;
        }
    
        if(!runDirector && stopDirector
        || GameDraggableEditor.isEditing) {
            return;
        }
    
        if(GameController.Instance.isGameRunning) {
            // if game running spawn and direct characters and events
    
            GameAIController.HandlePeriodic();
    
            GameAIController.HandleUpdate();
    
        }
    }

}