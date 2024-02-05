﻿using System.Collections.Generic;
using Engine.Game.App.BaseApp;
using UnityEngine;

public interface IGameController
{
    void Awake();
    void changeGameZone(string zone);
    void checkForGameOver();
    void checkQueueGameObjectTypeData();
    void clearQueueGameObjectTypeData();
    void gamePlayerGoalZone(GameObject goalObject);
    void gamePlayerGoalZoneCountdown(GameObject goalObject);
    void gamePlayerScores(double val);
    void gamePlayerUse();
    Vector3 getCurrentPlayerPosition();
    List<GameLevelItemAsset> getLevelRandomized(List<GameLevelItemAsset> levelItems);
    List<GameLevelItemAsset> getLevelRandomizedGrid();
    List<GameLevelItemAsset> getLevelRandomizedGrid(GameLevelGridData gameLevelGridData);
    List<GameLevelItemAsset> getLevelRandomizedGridAssets(GameLevelGridData gameLevelGridData);
    Vector3 getRandomSpawnLocation();
    void goalZoneChange();
    void goalZoneChange(string zone);
    void handleGoalZoneChange();
    void Init();
    void loadLevelActions();
    void loadLevelItems();
    void loadStartLevel();
    void OnDisable();
    void OnEditStateHandler(GameDraggableEditEnum state);
    void OnEnable();
    void OnGameAIDirectorData(GameAIDirectorData actor);
    void onGameContentDisplay();
    void OnGameItemDirectorData(GameItemData item);
    void onGamePrepare(bool startLevel);
    void onGameStarted();
    void OnNetworkPlayerContainerAdded(string uuid);
    void processQueueGameObjectTypeData();
    void queueGameObjectTypeData(GameObjectQueueItem queueItem);
    void queueGameObjectTypeData(string type, string code, string data_type, string display_type, Vector3 pos, Quaternion rot);
    void quitGameRunning();
    void Start();
    void Update();
    void updateDirectors(bool run);
}