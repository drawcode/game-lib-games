using System.Collections;

public interface IBaseAIController {
    void Awake();
    void broadcastCharacterMessage(GameAIDirectorData actor);
    void checkSpawns();
    void directAI();
    void directAICharacters(string actorType, double spawnCount, double spawnMin, double spawnLimit, bool limitFps = true);
    void directAIEnemies();
    void directAISidekicks();
    GameAIDifficulty getDifficultyLevelEnumFromValue(float difficultyCheck);
    float getDifficultyLevelValueFromEnum(float difficultyCheck);
    GamePlayerSpawn getSpawn(string code);
    void handlePeriodic();
    void handleUpdate();
    void incrementRoundsCompleted();
    void init();
    void load(string code, string type = "enemy");
    void load(string code, string type, float speed, float attack, float scale);
    void loadCharacter(GameAIDirectorData actor);
    void preload();
    IEnumerator preloadCo();
    void run();
    void run(bool run);
    void runEnemies();
    void runEnemies(bool run);
    void runSidekicks();
    void runSidekicks(bool run);
    void setDifficultyLevel(GameAIDifficulty difficultyTo);
    void setDifficultyLevel(float difficultyTo);
    void Start();
    void stop();
    void stopEnemies();
    void stopSidekicks();
    void Update();
    void updateDirector(GameDataDirector director);
}