using System.Collections;
using Engine.Game.App;

public interface IBaseItemController {
    void broadcastItemMessage(GameItemData item);
    void direct();
    void directItems();
    void directItems(string itemType, double spawnCount, double spawnMin, double spawnLimit, bool limitFps = true);
    void directWeapons();
    void directWeapons(string itemType, double spawnCount, double spawnMin, double spawnLimit, bool limitFps = true);
    GameItemDifficulty getDifficultyLevelEnumFromValue(float difficultyCheck);
    float getDifficultyLevelValueFromEnum(float difficultyCheck);
    void handlePeriodic();
    void handleUpdate();
    void incrementRoundsCompleted();
    void init();
    void load(string code);
    void loadItem(GameItemData item);
    void preload();
    IEnumerator preloadCo();
    void run();
    void run(bool run);
    void runItems();
    void runItems(bool run);
    void runWeapons();
    void runWeapons(bool run);
    void setDifficultyLevel(GameItemDifficulty difficultyTo);
    void setDifficultyLevel(float difficultyTo);
    void Start();
    void stop();
    void stopItems();
    void stopWeapons();
    void Update();
    void updateDirector(GameDataDirector director);
}