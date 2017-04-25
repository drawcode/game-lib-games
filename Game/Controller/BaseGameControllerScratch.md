using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



/*
Vector3 currentGamePlayerDistance = Vector3.zero;
Vector3 overallGamePlayerDistance = Vector3.zero;

internal virtual void handleLateUpdateStationary() {

    if (currentGamePlayerController == null) {
        return;
    }

    currentGamePlayerDistance = currentGamePlayerController.transform.position;

    overallGamePlayerDistance += currentGamePlayerDistance;

    currentGamePlayerController.transform.position = -currentGamePlayerDistance;

    Messenger<Vector3>.Broadcast(GamePlayerMessages.PlayerCurrentDistance, currentGamePlayerDistance);
    Messenger<Vector3>.Broadcast(GamePlayerMessages.PlayerOverallDistance, overallGamePlayerDistance);

    Debug.Log("GameController: handleLateUpdateStationary: currentGamePlayerDistance:" + currentGamePlayerDistance);
    //Debug.Log("GameController: handleLateUpdateStationary: overallGamePlayerDistance:" + overallGamePlayerDistance);

    //foreach (GameObjectInfinitePart part in gameObject.GetList<GameObjectInfinitePart>()) {

    //    part.transform.position = part.transform.position  + -currentGamePlayerDistance;

    //}
}
*/

/*
    // --------------------------------------------------------------------
    // LINES

    for (int i = 0; i < data.lines.Count; i++) {

        // add part item

        GameObject goItem = LoadAssetLevelPlaceholder(data, data.codeGamePartItem, spawnLocation, indexItem);

        if (goItem == null) {
            continue;
        }

        goItem.DestroyChildren();

        goItem.transform.parent = go.transform;
        goItem.transform.position = go.transform.position;
        goItem.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

        GameObjectInfinitePartItem partItem = goItem.Get<GameObjectInfinitePartItem>();

        if (partItem == null) {
            continue;
        }

        partItem.code = i.ToString();

        // --------------------------------------------------------------------
        // BLOCK PLACEHOLDER

        bool fillBlock = true;

        string codeBlock = data.codeGameBlock;
        string codeItem = "";

        if (indexItem > data.partBackCount) {

            GameDataObject gridObject =
                GetLevelAssetDynamicObject(data, i, 0, data.currentLevelGridIndex);

            if (gridObject != null) {
                codeItem = gridObject.code;
            }
        }

        if (indexItem % 10 == 0) {
            // Every tenth load the environment pads
            //Debug.Log("dynamicPart:10:" + indexItem);
        }

        if (codeItem.IsEqualLowercase(BaseDataObjectKeys.empty)) {
            fillBlock = false;
        }

        if (fillBlock) {

            // ADD PART BLOCK AND ASSETS FROM TEMPLATE

            GameObject goAssetBlock = LoadAssetLevelPlaceholder(data, codeBlock, spawnLocation, indexItem);

            if (goAssetBlock == null) {
                continue;
            }

            goAssetBlock.Hide();

            goAssetBlock.transform.parent = goItem.transform;
            goAssetBlock.transform.position = goItem.transform.position;
            //goAssetBlock.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

            goAssetBlock.Show();
        }

        // --------------------------------------------------------------------
        // LOAD DATA GRID ITEM

        if (indexItem > data.partBackCount
            && !codeItem.IsNullOrEmpty()
            && fillBlock
            && codeItem != codeBlock) {

            // TODO change to game specific lookup

            codeItem = GameController.GameItemCodeContextGet(codeItem);

            GameObject goAssetItem;

            bool isItem = codeItem.StartsWith(BaseDataObjectKeys.item);

            if (isItem) {

                goAssetItem = AppContentAssets.LoadAssetItems(codeItem, spawnLocation);
            }
            else {

                goAssetItem = AppContentAssets.LoadAssetLevelAssets(codeItem, spawnLocation);
            }

            if (goAssetItem == null) {
                Debug.Log("Asset not found items/" + codeItem);
                continue;
            }

            goAssetItem.Hide();

            goAssetItem.transform.parent = goItem.transform;

            float posY = 0f;

            if (isItem) {
                posY = 2f;
            }

            // ADD ITEM COIN LOCATION
            //if (codeItem.IsEqualLowercase("item-coin")) {
            //if (codeItem.IsEqualLowercase("item-special")) {

            goAssetItem.transform.position = goItem.transform.position.WithX(0);
            goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(posY).WithZ(0);

            goAssetItem.Show();
        }

        // reset position to view

        go.transform.position = go.transform.position.WithY(0);
    }
    */


/*
if(gameZoneEndLeft == null) {
    Transform gameZoneEndLeftTransform
        = levelZonesContainerObject.transform.FindChild("GameGoalZoneLeft");
    if(gameZoneEndLeftTransform != null) {
        gameZoneEndLeft = gameZoneEndLeftTransform.gameObject;
    }
}

if(gameZoneEndRight == null) {
    Transform gameZoneEndRightTransform
        = levelZonesContainerObject.transform.FindChild("GameGoalZoneRight");
    if(gameZoneEndRightTransform != null) {
        gameZoneEndRight = gameZoneEndRightTransform.gameObject;
    }
}

goalZoneChange(zone);
*/


/*

AppContentCollect appContentCollect = AppContentCollects.Current;

List<AppContentCollectItem> appContentCollectItems = appContentCollect.GetItemsData();

// --------------
// SAVE

gameZoneActionAsset.Load(
    GameZoneKeys.action_save, 
    GameZoneActions.action_save,
    "level-building-" + UnityEngine.Random.Range(1,10),
    "platform-large-1");

// --------------
// ATTACK

gameZoneActionAsset.Load(
    GameZoneKeys.action_attack, 
    GameZoneActions.action_attack,
    "level-building-" + UnityEngine.Random.Range(1,10),
    "platform-large-1");

// --------------
// BUILD

gameZoneActionAsset.Load(
    GameZoneKeys.action_build, 
    GameZoneActions.action_build,
    "level-building-" + UnityEngine.Random.Range(1,10),
    "platform-large-1");

// --------------
// REPAIR

gameZoneActionAsset.Load(
    GameZoneKeys.action_repair, 
    GameZoneActions.action_repair,
    "level-building-" + UnityEngine.Random.Range(1,10),
    "platform-large-1");

// --------------
// DEFEND

gameZoneActionAsset.Load(
    GameZoneKeys.action_defend, 
    GameZoneActions.action_defend,
    "level-building-" + UnityEngine.Random.Range(1,10),
    "platform-large-1");


*/



//LogUtil.Log("OnGameResults");

//if(runtimeData.localPlayerWin){
//GameUIPanelResults.Instance.ShowSuccess();
//GameUIPanelResults.Instance.HideFailed();
//}
//else {
//GameUIPanelResults.Instance.HideSuccess();
//GameUIPanelResults.Instance.ShowFailed();
//}

/*

// ---------------------------------------------------------------------

// LEVELS

public static void LoadLevelActions() {
    if(isInst) {
        Instance.loadLevelActions();
    }
}

    // -------------------------------------------------------
// LOAD LEVEL ACTION
// Applies all Collects items to the already built level finding the 
// game action areas.

public override void loadLevelActions() {

    AppContentCollect appContentCollect = AppContentCollects.Current;

    List<AppContentCollectItem> appContentCollectItems = appContentCollect.GetItemsData();

    int currentActionIndex = 0;

    foreach(GameZoneActionAsset gameZoneActionAsset in 
            levelItemsContainerObject.GetList<GameZoneActionAsset>()) {

        string assetBuildingRandom = "level-building-" + UnityEngine.Random.Range(1,10);
        string platformAsset = "platform-large-1";

        if(gameZoneActionAsset.gameZoneType == GameZoneKeys.action_none) {

            AppContentCollectItem appContentCollectItem = null;

            if(appContentCollectItems.Count > currentActionIndex) {
                appContentCollectItem = appContentCollectItems[currentActionIndex];
            }

            if(appContentCollectItem == null) {
                // Load save into placeholder by default, TODO other defaults.
                continue;
            }

            // Make it a type of needed action or none.
            // Update placeholder actions to actual actions of default

            if(appContentCollectItem.code == GameZoneActions.action_attack) {

                gameZoneActionAsset.Load(
                    GameZoneKeys.action_attack, 
                    GameZoneActions.action_attack,
                    assetBuildingRandom,
                    platformAsset);                

                currentActionIndex++;
            }
            else if(appContentCollectItem.code == GameZoneActions.action_build) {

                gameZoneActionAsset.Load(
                    GameZoneKeys.action_build, 
                    GameZoneActions.action_build,
                    assetBuildingRandom,
                    platformAsset);

                currentActionIndex++;
            }
            else if(appContentCollectItem.code == GameZoneActions.action_defend) {

                gameZoneActionAsset.Load(
                    GameZoneKeys.action_defend, 
                    GameZoneActions.action_defend,
                    assetBuildingRandom,
                    platformAsset);

                currentActionIndex++;
            }
            else if(appContentCollectItem.code == GameZoneActions.action_repair) {

                gameZoneActionAsset.Load(
                    GameZoneKeys.action_repair, 
                    GameZoneActions.action_repair,
                    assetBuildingRandom,
                    platformAsset);

                currentActionIndex++;
            }
            else if(appContentCollectItem.code == GameZoneActions.action_save) {

                gameZoneActionAsset.Load(
                    GameZoneKeys.action_save, 
                    GameZoneActions.action_save,
                    "",
                    "platform-1");

                currentActionIndex++;
            }

            / *

            AppContentCollect appContentCollect = AppContentCollects.Current;

            List<AppContentCollectItem> appContentCollectItems = appContentCollect.GetItemsData();

            gameZoneActionAsset.Load(
                GameZoneKeys.action_save, 
                GameZoneActions.action_save,
                "level-building-" + UnityEngine.Random.Range(1,10),
                "platform-large-1");

            // --------------
            // SAVE

            gameZoneActionAsset.Load(
                GameZoneKeys.action_save, 
                GameZoneActions.action_save,
                "level-building-" + UnityEngine.Random.Range(1,10),
                "platform-large-1");

            // --------------
            // ATTACK

            gameZoneActionAsset.Load(
                GameZoneKeys.action_attack, 
                GameZoneActions.action_attack,
                "level-building-" + UnityEngine.Random.Range(1,10),
                "platform-large-1");

            // --------------
            // BUILD

            gameZoneActionAsset.Load(
                GameZoneKeys.action_build, 
                GameZoneActions.action_build,
                "level-building-" + UnityEngine.Random.Range(1,10),
                "platform-large-1");

            // --------------
            // REPAIR

            gameZoneActionAsset.Load(
                GameZoneKeys.action_repair, 
                GameZoneActions.action_repair,
                "level-building-" + UnityEngine.Random.Range(1,10),
                "platform-large-1");

            // --------------
            // DEFEND

            gameZoneActionAsset.Load(
                GameZoneKeys.action_defend, 
                GameZoneActions.action_defend,
                "level-building-" + UnityEngine.Random.Range(1,10),
                "platform-large-1");


            * /

        }
    }
}

    // -------------------------------------------------------

public static void LoadLevelAssets(string code) {
if(isInst) {
    Instance.loadLevelAssets(code);
}
}

public static void LoadLevel(string code) {
if(isInst) {
    Instance.loadLevel(code);
}
}

public static void LoadLevelItems() {
if(isInst) {
    Instance.loadLevelItems();
}
}

    // -------------------------------------------------------
public override void loadLevelItems() {
///base.loadLevelItems();

//LogUtil.Log("GAME START FLOW: STEP #11: loadLevelItems");

// Load level items by game type....

runDirectorsDelay = 10f;

bool updated = false;

if(AppModes.Instance.isAppModeGameChallenge) {

    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppModeGameChallenge:"
        + AppModes.Instance.isAppModeGameChallenge);

    if(AppModeTypes.Instance.isAppModeTypeGameDefault) {

        LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
            + AppModeTypes.Instance.isAppModeTypeGameDefault);

        if(AppContentStates.Instance.isAppContentStateGameChallenge) {

            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameChallenge:"
                + AppContentStates.Instance.isAppContentStateGameChallenge);

            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGridAssets(GameLevelGridData.GetDefault());

            updated = true;
        }
    }
}
else if(AppModes.Instance.isAppModeGameTraining) {
    LogUtil.Log("loadLevelItems: AppModes.Instance.isAppModeGameTraining:"
        + AppModes.Instance.isAppModeGameTraining);

    if(AppModeTypes.Instance.isAppModeTypeGameDefault) {

        LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
            + AppModeTypes.Instance.isAppModeTypeGameDefault);

    }
    else if(AppModeTypes.Instance.isAppModeTypeGameChoice) {

        LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameChoice:"
            + AppModeTypes.Instance.isAppModeTypeGameChoice);

        // LOAD CHOICE GAME LEVEL ITEMS

        if(AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz) {

            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingChoiceQuiz:"
                + AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz);

            / *
            int countData = 3;

            AppContentChoice choice = null;

            if(UIPanelModeTypeChoice.Instance != null) {
                choice = UIPanelModeTypeChoice.Instance.GetCurrentChoice();
            }

            if(choice != null) {
                //countData = choice.choices.Count;
            }

            //if(countData > 0) {
            GameLevelGridData gridData = GameLevelGridData.GetModeTypeChoice(countData);
            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGridAssets(gridData);
            //}
            ///


            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGridAssets(GameLevelGridData.GetDefault());

            updated = true;
        }

    }
    else if(AppModeTypes.Instance.isAppModeTypeGameCollection) {

        LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameCollection:"
            + AppModeTypes.Instance.isAppModeTypeGameCollection);

        // LOAD COLLECTION GAME LEVEL ITEMS

        if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts) {

            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingCollectionSmarts:"
                + AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts);

            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGridAssets(GameLevelGridData.GetDefault());

            updated = true;
        }
        else if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {

            LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameTrainingCollectionSafety:"
                + AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety);

            GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGridAssets(GameLevelGridData.GetDefault());

            updated = true;
        }

    }
    else if(AppModeTypes.Instance.isAppModeTypeGameContent) {

        LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameContent:"
            + AppModeTypes.Instance.isAppModeTypeGameContent);

    }
    else if(AppModeTypes.Instance.isAppModeTypeGameTips) {

        LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameTips:"
            + AppModeTypes.Instance.isAppModeTypeGameTips);

    }
    else { //AppModes.Instance.isAppModeGameArcade) {
        LogUtil.Log("loadLevelItems: AppModes.Instance.isAppModeGameArcade:"
            + AppModes.Instance.isAppModeGameArcade);

        if(AppModeTypes.Instance.isAppModeTypeGameDefault) {

            LogUtil.Log("loadLevelItems: AppModeTypes.Instance.isAppModeTypeGameDefault:"
                + AppModeTypes.Instance.isAppModeTypeGameDefault);

            if(AppContentStates.Instance.isAppContentStateGameArcade) {

                LogUtil.Log("loadLevelItems: AppModes.Instance.isAppContentStateGameArcade:"
                    + AppContentStates.Instance.isAppContentStateGameArcade);

                GameLevelItems.Current.level_items
                    = GameController.GetLevelRandomizedGridAssets(GameLevelGridData.GetDefault());

                updated = true;
            }
        }
    }
}

if(!updated) {
    GameLevelItems.Current.level_items
                = GameController.GetLevelRandomizedGridAssets(GameLevelGridData.GetDefault());
}

LogUtil.Log("loadLevelItem:Count:" + GameLevelItems.Current.level_items.Count);

}

    
    // -------------------------------------------------------

public static void UpdateDirectors(bool run) {
    if (isInst) {
        Instance.updateDirectors(run);
    }
}

public override void updateDirectors(bool run) {
    //base.updateDirectors(run);

    if (!run) {
        return;
    }

    bool runAI = run;
    bool runItem = run;

    if (run) {

        // DEFAULTS
        if (AppModes.Instance.isAppModeGameArcade) {

            // TODO depends on action/round iteration
            GameAIController.Instance.spawnEnemyMin = 5;
            GameAIController.Instance.spawnEnemyLimit = 11;
        }
        else if (AppModes.Instance.isAppModeGameChallenge) {
            // TODO depends on level
            GameAIController.Instance.spawnEnemyMin = 5;
            GameAIController.Instance.spawnEnemyLimit = 11;
        }

        List<GameDataDirector> directorsLevels = GameLevels.Current.data.directors;
        List<GameDataDirector> directors = GameWorlds.Current.data.directors;

        if(directors == null) {
            directors = new List<GameDataDirector>();
        }

        if(directorsLevels != null && directors.Count > 0) {
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

    if (AppModes.Instance.isAppModeGameTraining) {

        // Don't run ai or items in training currently.

        if (AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz
            || AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts
            || AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {

            runAI = false;
            runItem = false;
        }
    }

    GameController.UpdateDirectorAI(runAI);
    GameController.UpdateDirectorItem(runItem);
}

    
    // -------------------------------------------------------

    public override void onGamePrepare(bool startLevel) {

        string levelCode = GameLevels.Current.code;

        base.onGamePrepare(false);
                       
        if (startLevel) {
            GameController.StartGame(levelCode);
        }

        if (AppContentStates.Instance.isAppContentStateGameArcade) {
            //LogUtil.Log("onGamePrepare:isAppContentStateGameArcade");
            GameController.GameContentDisplay(GameContentDisplayTypes.gameModeContentOverview);

        }
        else if (AppContentStates.Instance.isAppContentStateGameChallenge) {
            LogUtil.Log("onGamePrepare:isAppContentStateGameChallenge");
            GameController.GameContentDisplay(GameContentDisplayTypes.gameModeContentOverview);

        }
        else if (AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz) {
            LogUtil.Log("onGamePrepare:isAppContentStateGameTrainingChoiceQuiz");

            UIPanelModeTypeChoice.Instance.Reset();

            startLevel = false;

            GameController.GameContentDisplay(GameContentDisplayTypes.gameChoicesOverview);
        }
        else if (AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {
            LogUtil.Log("onGamePrepare:isAppContentStateGameTrainingCollectionSafety");
            GameController.GameContentDisplay(GameContentDisplayTypes.gameModeContentOverview);
        }
        else if (AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts) {
            LogUtil.Log("onGamePrepare:isAppContentStateGameTrainingCollectionSmarts");
            GameController.GameContentDisplay(GameContentDisplayTypes.gameModeContentOverview);
        }
        else {
            //LogUtil.Log("onGamePrepare:isAppContentStateGameArcade");
            GameController.GameContentDisplay(GameContentDisplayTypes.gameModeContentOverview);

        }
    }

    public override void onGameContentDisplay() {
        // Show runtime content display data
        //GameRunningStatePause();

        LogUtil.Log("onGameContentDisplay:contentDisplayCode:" + contentDisplayCode);

        if (contentDisplayCode == GameContentDisplayTypes.gameChoicesOverview
            || contentDisplayCode == GameContentDisplayTypes.gameChoicesItemStart
            || contentDisplayCode == GameContentDisplayTypes.gameChoicesItemResult) {

            if (AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz) {
                handleContentTrainingChoiceQuiz();
            }
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameCollect
            || contentDisplayCode == GameContentDisplayTypes.gameCollectOverview
            || contentDisplayCode == GameContentDisplayTypes.gameCollectItemStart
            || contentDisplayCode == GameContentDisplayTypes.gameCollectItemResult) {

            if (AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts) {
                handleContentTrainingCollectionSmarts();
            }
            if (AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {
                handleContentTrainingCollectionSafety();
            }
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameModeContentOverview) {
            handleContentOverview();
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameTutorial) {
            handleContentTutorial();
        }
        else if (contentDisplayCode == GameContentDisplayTypes.gameTips) {
            handleContentTips();
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
        else if (contentDisplayCode == GameContentDisplayTypes.gamePlayerOutOfBounds) {

            GameController.GamePlayerOutOfBoundsDelayed(3f);

            UIPanelDialogBackground.ShowDefault();
            UIPanelDialogDisplay.SetTitle("OUT OF BOUNDS");
            //UIPanelDialogDisplay.SetDescription("RUN, BUT STAY IN BOUNDS...");
            UIPanelDialogDisplay.ShowDefault();
            GamePlayerProgress.Instance.ProcessProgressTotal("out-of-bounds", 1);
        }
        else {
            UIPanelDialogBackground.HideAll();
        }

        //GameRunningStateRun();
    }

    public virtual void handleContentTrainingChoiceQuiz() {

        if (contentDisplayCode == GameContentDisplayTypes.gameChoicesOverview
            || contentDisplayCode == GameContentDisplayTypes.gameChoicesItemStart
            || contentDisplayCode == GameContentDisplayTypes.gameChoicesItemResult
            || contentDisplayCode == GameContentDisplayTypes.gameChoices) {
            
            UIPanelDialogBackground.ShowDefault();
            UIPanelModeTypeChoice.ShowDefault();
        }
    }

    public virtual void handleContentTrainingCollectionSmarts() {

        if (contentDisplayCode == GameContentDisplayTypes.gameChoicesOverview
            || contentDisplayCode == GameContentDisplayTypes.gameChoicesItemStart
            || contentDisplayCode == GameContentDisplayTypes.gameChoicesItemResult
            || contentDisplayCode == GameContentDisplayTypes.gameChoices) {

        }
    }

    public virtual void handleContentTrainingCollectionSafety() {

        if (contentDisplayCode == GameContentDisplayTypes.gameCollect
            || contentDisplayCode == GameContentDisplayTypes.gameCollectOverview
            || contentDisplayCode == GameContentDisplayTypes.gameCollectItemStart
            || contentDisplayCode == GameContentDisplayTypes.gameCollectItemResult) {

        }
    }
    
    // -------------------------------------------------------

    // GAME PLAYER GOAL COUNTDOWN

    public static void GamePlayerGoalZoneCountdown(GameObject goalObject) {
        if (isInst) {
            Instance.gamePlayerGoalZoneCountdown(goalObject);
        }
    }

    public override void gamePlayerGoalZoneCountdown(GameObject goalObject) {

        base.gamePlayerGoalZoneCountdown(goalObject);

        if (!AppModes.Instance.isAppModeGameTraining) {

            bool playCountdown = false;

            if (goalObject.name.Contains("Right")
                && currentGameZone == GameZoneKeys.goal_right) {
                playCountdown = true;
            }
            else if (goalObject.name.Contains("Left")
                && currentGameZone == GameZoneKeys.goal_left) {
                playCountdown = true;
            }

            if (playCountdown) {
                if (goalObject.name.Contains("GameZone5")) {
                    GameAudioController.Instance.playSoundGoalRange1();
                }
                else if (goalObject.name.Contains("GameZone10")) {
                    GameAudioController.Instance.playSoundGoalRange2();
                }
                else if (goalObject.name.Contains("GameZone15")) {
                    GameAudioController.Instance.playSoundGoalRange3();
                }
                else if (goalObject.name.Contains("GameZone20")) {
                    GameAudioController.Instance.playSoundGoalRange4();
                }
            }
        }
    }

    // -------------------------------------------------------

    // GAME PLAYER GOAL ZONE

    public static void GamePlayerGoalZone(GameObject goalObject) {
        if (isInst) {
            Instance.gamePlayerGoalZone(goalObject);
        }
    }

    public static void GamePlayerGoalZoneDelayed(GameObject goalObject, float delay) {
        if (isInst) {
            Instance.gamePlayerGoalZoneDelayed(goalObject, delay);
        }
    }

    public override void gamePlayerGoalZone(GameObject goalObject) {

        base.gamePlayerGoalZone(goalObject);

        if (!AppModes.Instance.isAppModeGameTraining) {

            GameZoneGoal goalZone = goalObject.Get<GameZoneGoal>();
    
            //LogUtil.Log("goalZone:obj:");
            //LogUtil.Log("goalZone:goalObject:" + goalObject.name);

            if (goalZone != null) {

                //LogUtil.Log("goalZone:currentGameZone:" + currentGameZone);
                //LogUtil.Log("goalZone.gameEndZoneType:" + goalZone.gameEndZoneType);

                bool scoreIt = false;

                if (goalZone.gameEndZoneType == GameZoneKeys.goal_left
                    && currentGameZone == GameZoneKeys.goal_left) {

                    scoreIt = true;
    
                    GameController.ChangeGameZone(GameZoneKeys.goal_right);
                }
                else if (goalZone.gameEndZoneType == GameZoneKeys.goal_right
                    && currentGameZone == GameZoneKeys.goal_right) {

                    scoreIt = true;
    
                    GameController.ChangeGameZone(GameZoneKeys.goal_left);
                }

                if (scoreIt) {
                    // handle score
                    gamePlayerScores(1);
                    GameAudioController.PlaySoundScores();
                    goalZone.StopEffectsScore();
                    goalZone.PlayEffectsScore();

                    // extend time
                    GameRuntimeTimeExtend(1f);

                    // rpg properties
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealthRuntime(
                        UnityEngine.Random.Range(.03f, .07f),
                        UnityEngine.Random.Range(.03f, .07f));
                }
            }
            
            //string spawnCode = "";
    
            if (currentGameZone == GameZoneKeys.goal_right) {
                //spawnCode = rightMiddle;
            }
            else if (currentGameZone == GameZoneKeys.goal_left) {
                //spawnCode = leftMiddle;
            }
        }
    }
    
    public static void GameRuntimeTimeExtend(double extendAmount) {
        if (isInst) {
            Instance.gameRuntimeTimeExtend(extendAmount);
        }
    }





    
    // -------------------------------------------------------

    // GAME SCORE/CHECK GAME OVER

    public static void CheckForGameOver() {
        if (isInst) {
            Instance.checkForGameOver();
        }
    }

    public override void checkForGameOver() {
        // base.checkForGameOver();

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
                bool runModeAction = false;
    
                if (AppModes.Instance.isAppModeGameArcade) {
                    if (currentGamePlayerController.runtimeData.health <= 0f
                        || runtimeData.timeExpired) {
                        gameOverMode = true;
                        
                        AppContentCollects.Current.ScoreCompleted(
                            BaseDataObjectKeys.mission, runtimeData, currentGamePlayerController.runtimeData);
                    }
                    runModeAction = true;
                }
                else if (AppModes.Instance.isAppModeGameChallenge) {
                    if (currentGamePlayerController.runtimeData.health <= 0f
                        || runtimeData.timeExpired) {
                        gameOverMode = true;
                        
                        AppContentCollects.Current.ScoreCompleted(
                            BaseDataObjectKeys.mission, runtimeData, currentGamePlayerController.runtimeData);
                    }
                    runModeAction = true;
                }
                else if (AppModes.Instance.isAppModeGameTraining) {

                    // Handle mode internally
                }
                else if (AppModes.Instance.isAppModeGameMission) {
                    if (currentGamePlayerController.runtimeData.health <= 0f
                        || runtimeData.timeExpired) {
                        gameOverMode = true;
                        
                        AppContentCollects.Current.ScoreCompleted(
                            BaseDataObjectKeys.mission, runtimeData, currentGamePlayerController.runtimeData);
                        
                    }
                    runModeAction = true;
                }
                
                // TODO other modes
    
                if (runModeAction) {                
                    if (runtimeData.outOfBounds) {
                        //LogUtil.Log("CheckForGameOver:runtimeData.outOfBounds:" + runtimeData.outOfBounds);
                        gameOverMode = true;
                        //LogUtil.Log("CheckForGameOver:gameOverMode:" + gameOverMode);
                    }

                    if (gameOverMode) {
                        isGameOver = true;
                        GameController.ResultsGameDelayed();
                    }
        
                    runtimeData.SubtractTime(Time.deltaTime);
                }
    
                //if(runtimeData.timeExpired) {
                // Change level/flash
                //ChangeLevelFlash();
                //}
            }
        }
    }

    // ------------------------------------------------------------------------
    
    public override List<GameLevelItemAsset> getLevelRandomizedGrid(GameLevelGridData gameLevelGridData) {

        levelItems = base.getLevelRandomizedGrid(gameLevelGridData);

        LogUtil.Log("GAME START FLOW: STEP #12: getLevelRandomizedGrid");

        LogUtil.Log("getLevelRandomizedGrid:ameLevelGridData.assets.Count:" + gameLevelGridData.assets.Count);

        LogUtil.Log("getLevelRandomizedGrid:gameLevelGridData.gridDepth:" + gameLevelGridData.gridDepth);
        LogUtil.Log("getLevelRandomizedGrid:gameLevelGridData.gridHeight:" + gameLevelGridData.gridHeight);
        LogUtil.Log("getLevelRandomizedGrid:gameLevelGridData.gridWidth:" + gameLevelGridData.gridWidth);

        bool useAssetMap = true;

        for (int z = 0; z < (int)gameLevelGridData.gridDepth; z++) {

            for (int y = 0; y < (int)gameLevelGridData.gridHeight; y++) {

                for (int x = 0; x < (int)gameLevelGridData.gridWidth; x++) {

                    float posX = gameLevelGridData.gridBoxSize * x;
                    float posY = gameLevelGridData.gridBoxSize * y;
                    float posZ = gameLevelGridData.gridBoxSize * z;  

                    if (gameLevelGridData.centeredX) {
                        posX = posX -
                            ((gameLevelGridData.gridWidth *
                            gameLevelGridData.gridBoxSize) / 2);
                    }

                    if (gameLevelGridData.centeredY) {
                        posY = posY -
                            ((gameLevelGridData.gridHeight *
                            gameLevelGridData.gridBoxSize) / 2);
                    }

                    if (gameLevelGridData.centeredZ) {
                        posZ = posZ -
                            ((gameLevelGridData.gridDepth *
                            gameLevelGridData.gridBoxSize) / 2);
                    }

                    Vector3 gridPos = Vector3.zero
                        .WithX(posX)
                        .WithY(posY)
                        .WithZ(posZ);

                    if (useAssetMap) {

                        //LogUtil.Log("getLevelRandomizedGrid:assetCode:" + assetCode);'

                        Dictionary<string, GameLevelItemAssetData> assetItemLayout = gameLevelGridData.GetAssetLayoutData();
                        
                        string keyLayout = 
                            string.Format(
                                "{0}-{1}-{2}", 
                                (int)x, 
                                (int)y, 
                                (int)z);

                        if (assetItemLayout != null) {

                            if (assetItemLayout.ContainsKey(keyLayout)) {

                                GameLevelItemAssetData assetData = assetItemLayout.Get(keyLayout);

                                if (assetData != null) {

                                    LogUtil.Log("getLevelRandomizedGrid:posX:" + posX);
                                    LogUtil.Log("getLevelRandomizedGrid:posY:" + posY);
                                    LogUtil.Log("getLevelRandomizedGrid:posZ:" + posZ);

                                    if (!assetData.code.Contains("game-choice")) {
                                        //assetData.range_scale = Vector2.one.WithX(.8f).WithY(10f);
                                        //assetData.asset_rotation = Vector2.zero.WithX(-90).WithY(90);
                                    }

                                    SyncLevelItem(gridPos, assetData);
                                }
                            }
                        }
                    }
                }
            }
        }

        return levelItems;
    }

    public static List<GameLevelItemAsset> GetLevelRandomizedGridAssets(GameLevelGridData gameLevelGridData) {
        if (isInst) {
            return Instance.getLevelRandomizedGridAssets(gameLevelGridData);
        }
        return new List<GameLevelItemAsset>();
    }

    public override List<GameLevelItemAsset> getLevelRandomizedGridAssets(GameLevelGridData gameLevelGridData) {

        levelItems = base.getLevelRandomizedGridAssets(gameLevelGridData);

        //LogUtil.Log("GAME START FLOW: STEP #12: getLevelRandomizedGrid");

        //LogUtil.Log("getLevelRandomizedGrid:ameLevelGridData.assets.Count:" + gameLevelGridData.assets.Count);

        //LogUtil.Log("getLevelRandomizedGrid:gameLevelGridData.gridDepth:" + gameLevelGridData.gridDepth);
        //LogUtil.Log("getLevelRandomizedGrid:gameLevelGridData.gridHeight:" + gameLevelGridData.gridHeight);
        //LogUtil.Log("getLevelRandomizedGrid:gameLevelGridData.gridWidth:" + gameLevelGridData.gridWidth);

        bool useAssetMap = true;

        for (int z = 0; z < (int)gameLevelGridData.gridDepth; z++) {

            for (int y = 0; y < (int)gameLevelGridData.gridHeight; y++) {

                for (int x = 0; x < (int)gameLevelGridData.gridWidth; x++) {

                    float posX = gameLevelGridData.gridBoxSize * x;
                    float posY = gameLevelGridData.gridBoxSize * y;
                    float posZ = gameLevelGridData.gridBoxSize * z;

                    if (gameLevelGridData.centeredX) {
                        posX = posX -
                            ((gameLevelGridData.gridWidth *
                            gameLevelGridData.gridBoxSize) / 2);
                    }

                    if (gameLevelGridData.centeredY) {
                        posY = posY -
                            ((gameLevelGridData.gridHeight *
                            gameLevelGridData.gridBoxSize) / 2);
                    }

                    if (gameLevelGridData.centeredZ) {
                        posZ = posZ -
                            ((gameLevelGridData.gridDepth *
                            gameLevelGridData.gridBoxSize) / 2);
                    }

                    Vector3 gridPos = Vector3.zero
                        .WithX(posX)
                        .WithY(posY)
                        .WithZ(posZ);

                    if (useAssetMap) {

                        Dictionary<string, GameLevelItemAssetData> assetItemLayout = gameLevelGridData.GetAssetLayoutData();
                        
                        string keyLayout = 
                            string.Format(
                                "{0}-{1}-{2}", 
                                (int)x, 
                                (int)y, 
                                (int)z);
                        
                        if (assetItemLayout != null) {
                            
                            if (assetItemLayout.ContainsKey(keyLayout)) {
                                
                                GameLevelItemAssetData assetData = assetItemLayout.Get(keyLayout);
                                
                                if (assetData != null) {

                                    assetData.position_data = new Vector3Data(gridPos);
                                    
                                    if (!assetData.code.Contains("game-choice")) {
                                        //assetData.range_scale = Vector2.one.WithX(.8f).WithY(10f);
                                        //assetData.asset_rotation = Vector3.zero.WithX(-90).WithY(90);
                                    }
                                    
                                    SyncLevelItem(gridPos, assetData);
                                }
                            }
                        }
                    }
                }
            }
        }

        return levelItems;
    }








    
    // ------------------------------------------------------------------------
    // ASSETS CONTEXT

    public override string GameAssetPresetCode(string assetCode) {

        GamePreset assetPreset = GamePresets.Instance.GetById(assetCode);

        if(assetPreset != null) {

            GamePresetItem presetItem = assetPreset.GetItemRandomByProbability(assetPreset.data.items);

            if(presetItem != null) {
                assetCode = presetItem.code;
            }
        }
        else {
            //Debug.Log("GamePreset NOT FOUND: " + assetCode);
        }

        return assetCode;
    }

    //

    public override void GameAssetObjectContextGet(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        // Handle template by level/world/character

        // CUSTOM GAME BLOCK

        if(assetCode == data.codeGameBlockFloor) {

            GameAssetObjectContextGetBlock(data, assetCode, go);
        }

        // CUSTOM GAME BLOCK LOW

        else if (assetCode == data.codeGameBlockLow) {

            GameAssetObjectContextGetLow(data, assetCode, go);
        }

        // CUSTOM GAME BLOCK HIGH

        else if (assetCode == data.codeGameBlockHigh) {

            GameAssetObjectContextGetHigh(data, assetCode, go);
        }

        // CUSTOM GAME SIDE

        else if(assetCode == data.codeGameSide) {

            GameAssetObjectContextGetSide(data, assetCode, go);
        }

        //return go;
    }

    public override void GameAssetObjectContextGetBlock(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        if(go.Has<GameObjectInfiniteAssetCache>()) {
            return;
        }

        go.SetOnly<GameObjectInfiniteAssetCache>();

        foreach(GameObjectInactive container in
                go.GetList<GameObjectInactive>()) {

            if(!container.type.IsEqualLowercase(BaseDataObjectKeys.container)
                && !container.code.IsEqualLowercase(BaseDataObjectKeys.assets)) {

                continue;
            }

            foreach(GameObjectInactive asset in
                container.gameObject.GetList<GameObjectInactive>()) {

                if(go.transform == null) {
                    continue;
                }

                // handle main object

                if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.main)) {

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    GameAssetObjectContextItem(assetObject, assetCode);
                }

                if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.environment)) {
                    
                    float chance = UnityEngine.Random.Range(0, 10);

                    if(chance < 3) {

                        GameObject assetObject = asset.gameObject;

                        GameObject goAsset = GameAssetObjectContextItem(
                            assetObject, assetCode, BaseDataObjectKeys.environment);

                        if(goAsset != null) {
                            
                            goAsset.transform.localPosition = 
                                MathUtil.RandomRange(-.3f, .3f, 0, 0, -data.distanceTickZ, data.distanceTickZ);
                            
                            goAsset.transform.localScale = 
                                MathUtil.RandomRangeConstrain(1f, 3f);
                            
                            goAsset.transform.localRotation = 
                                Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                        }
                    }
                }                
            }
        }

        //return go;
    }

    public override void GameAssetObjectContextGetLow(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        if(go.Has<GameObjectInfiniteAssetCache>()) {
            return;
        }

        go.SetOnly<GameObjectInfiniteAssetCache>();

        foreach(GameObjectInactive container in
            go.GetList<GameObjectInactive>()) {

            if(!container.type.IsEqualLowercase(BaseDataObjectKeys.container)
                && !container.code.IsEqualLowercase(BaseDataObjectKeys.assets)) {

                continue;
            }

            foreach(GameObjectInactive asset in
                container.gameObject.GetList<GameObjectInactive>()) {

                if(go.transform == null) {
                    continue;
                }

                // handle main object

                if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.main)) {

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    //GameObject goAsset = GameAssetObjectContextItem(assetObject, assetCode);
                    GameAssetObjectContextItem(assetObject, assetCode);
                }
            }
        }
        
        //return go;
    }

    public override void GameAssetObjectContextGetHigh(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        if(go.Has<GameObjectInfiniteAssetCache>()) {
            return;
        }

        go.SetOnly<GameObjectInfiniteAssetCache>();

        foreach(GameObjectInactive container in
            go.GetList<GameObjectInactive>()) {

            if(!container.type.IsEqualLowercase(BaseDataObjectKeys.container)
                && !container.code.IsEqualLowercase(BaseDataObjectKeys.assets)) {

                continue;
            }

            foreach(GameObjectInactive asset in
                container.gameObject.GetList<GameObjectInactive>()) {

                if(go.transform == null) {
                    continue;
                }

                // handle main object

                if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.main)) {

                    GameObject assetObject = asset.gameObject;

                    GameAssetObjectContextItem(assetObject, assetCode);
                }
            }
        }

        //return go;
    }

    public override void GameAssetObjectContextGetSide(
        GameObjectInfinteData data, string assetCode, GameObject go) {

        //   StartCoroutine(GameAssetObjectContextGetSideCo(
        //       data, assetCode, go));
        //}

        //public IEnumerator GameAssetObjectContextGetSideCo(
        //    GameObjectInfinteData data, string assetCode, GameObject go) {

        if(go.Has<GameObjectInfiniteAssetCache>()) {
            return;
        }

        go.SetOnly<GameObjectInfiniteAssetCache>();
        
        foreach(GameObjectInactive container in
            go.GetList<GameObjectInactive>()) {

            if(!container.type.IsEqualLowercase(BaseDataObjectKeys.container)
                && !container.code.IsEqualLowercase(BaseDataObjectKeys.assets)) {

                continue;
            }

            foreach(GameObjectInactive asset in
                container.gameObject.GetList<GameObjectInactive>()) {

                //yield return new WaitForEndOfFrame();

                // handle main object

                // TERRAIN                
                
                if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.terrain)) {
                    
                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    GameAssetObjectContextItem(assetObject, assetCode, asset.code);

                    //yield return new WaitForEndOfFrame();
                }

                // BUMPER
                
                else if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.bumper)) {

                    //yield return new WaitForEndOfFrame();

                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    int maxCount = 6;

                    for(int i = 0; i < 6; i++) {
                        
                        GameObject goAsset = GameAssetObjectContextItem(
                            assetObject, assetCode, 
                            StringUtil.Dashed(asset.code, BaseDataObjectKeys.blocks), maxCount);

                        if(goAsset != null) {
                            float posX = 0;
                            float widthX = 128;
                            float itemX = data.distanceTickZ;

                            posX = (i + 1) * itemX;
                            posX = posX - widthX / 2;

                            goAsset.transform.localPosition = Vector3.zero.WithX(posX);

                            if(goAsset.name.Contains(BaseDataObjectKeys.plant)) {
                                goAsset.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                                goAsset.transform.localScale = MathUtil.RandomRangeConstrain(1f, 4f);
                            }
                        }
                    }
                }

                // ENVIRONMENT

                else if(asset.type.IsEqualLowercase(BaseDataObjectKeys.asset)
                    && asset.code.IsEqualLowercase(BaseDataObjectKeys.environment)) {

                    //yield return new WaitForEndOfFrame();
                    
                    // replace main asset with template one

                    GameObject assetObject = asset.gameObject;

                    int maxCount = 6;

                    float rangeXStart = -data.distanceTickZ * 4;
                    float rangeXEnd = data.distanceTickZ * 4;

                    for(int i = 0; i < 4; i++) {

                        GameObject goAssetTrees = GameAssetObjectContextItem(
                            assetObject, assetCode, 
                            StringUtil.Dashed(asset.code, BaseDataObjectKeys.trees), maxCount);

                        if(goAssetTrees != null) {

                            goAssetTrees.transform.localPosition = MathUtil.RandomRange(rangeXStart, rangeXEnd, 1, 1, 50, 256);
                            goAssetTrees.transform.localScale = MathUtil.RandomRangeConstrain(2f, 12f);
                            goAssetTrees.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                        }
                    }

                    for(int i = 0; i < 3; i++) {

                        GameObject goAssetPlants = GameAssetObjectContextItem(
                            assetObject, assetCode, StringUtil.Dashed(asset.code, BaseDataObjectKeys.plants), maxCount);

                        if(goAssetPlants != null) {

                            goAssetPlants.transform.localPosition = MathUtil.RandomRange(rangeXStart, rangeXEnd, 1, 1, 32, 256);
                            goAssetPlants.transform.localScale = MathUtil.RandomRangeConstrain(2f, 18f);
                            goAssetPlants.transform.localRotation = Quaternion.Euler(MathUtil.RandomRangeY(0, 360));
                        }
                    }
                }
            }
        }

        //return go;
    }

    // ------------------------------------------------------------------------

    // ASSET PARTS

    public override void LoadPartDynamicByIndexPart(GameObjectInfinteData data, int indexItem, bool clear = false) {
        //StartCoroutine(LoadPartDynamicByIndexPartCo(data, indexItem, clear));
    //}

    //public IEnumerator LoadPartDynamicByIndexPartCo(GameObjectInfinteData data, int indexItem, bool clear = false) {

        // Use off screen location to spawn before move

        Vector3 spawnLocation = Vector3.zero.WithY(-5000);

        // --------------------------------------------------------------------
        // ADD PART ITEMS CONTAINER

        GameObject go = LoadAssetLevelPlaceholder(data, data.codeGamePartItems, spawnLocation, indexItem);

        if (go == null) {
            //yield break;
            return;
        }

        //yield return new WaitForEndOfFrame();

        go.DestroyChildren();

        //go.name = StringUtil.Dashed(data.code, indexItem.ToString());
        go.transform.parent = data.parentContainer.transform;

        //yield return new WaitForEndOfFrame();

        // --------------------------------------------------------------------
        // PART ITEMS       

        GameObjectInfinitePart part = go.Get<GameObjectInfinitePart>();

        if (part == null) {
            //Debug.Log("GameObjectInfinitePart not found part:" + data.codeGamePartItems);

            //yield break;
            return;
        }

        part.index = indexItem;

        Vector3 bounds = part.bounds;

        //yield return new WaitForEndOfFrame();

        //Vector3 infinityPosition = go.transform.position.WithZ(   
        //    (((indexItem + 1) * bounds.z) - bounds.z) + distance.z);

        go.transform.position = go.transform.position.WithZ(
            (((indexItem + 1) * bounds.z) - bounds.z) + data.distance.z);

        if (indexItem > data.partBackCount) {
            LoadPartDynamicByIndexPartData(data, indexItem);
        }

        //yield return new WaitForEndOfFrame();

        // --------------------------------------------------------------------
        // LINES

        //yield return new WaitForEndOfFrame();

        LoadLevelAssetsLines(data, go, indexItem, spawnLocation);

        // --------------------------------------------------------------------
        // LOAD ENVIRONMENT

        //yield return new WaitForEndOfFrame();

        LoadLevelAssetsPeriodic(data, go, indexItem, clear);

    }

    public override void LoadLevelAssetsLines(
        GameObjectInfinteData data, GameObject go, int indexItem, Vector3 spawnLocation) {

   //     StartCoroutine(LoadLevelAssetsLinesCo(
    //        data, go, indexItem, spawnLocation));
    //}


    //public IEnumerator LoadLevelAssetsLinesCo(
    //   GameObjectInfinteData data, GameObject go, int indexItem, Vector3 spawnLocation) {

        // --------------------------------------------------------------------
        // LINES

        for (int i = 0; i < data.lines.Count; i++) {
            
            //yield return new WaitForEndOfFrame();

            // add part item

            GameObject goItem = LoadAssetLevelPlaceholder(data, data.codeGamePartItem, spawnLocation, indexItem);

            if (goItem == null) {
                continue;
            }

            goItem.DestroyChildren();

            if(go.transform == null) {
                goItem.DestroyGameObject();
                //yield break;
                return;
            }

            goItem.transform.parent = go.transform;
            goItem.transform.position = go.transform.position;
            goItem.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

            //yield return new WaitForEndOfFrame();

            GameObjectInfinitePartItem partItem = goItem.Get<GameObjectInfinitePartItem>();

            if (partItem == null) {
                continue;
            }

            partItem.code = i.ToString();

            // --------------------------------------------------------------------
            // BLOCK PLACEHOLDER

            bool fillBlock = true;

            string codeBlock = data.codeGameBlockFloor;
            string codeItem = "";

            if (indexItem > data.partBackCount) {

                GameDataObject gridObject =
                    GetLevelAssetDynamicObject(data, i, 0, data.currentLevelGridIndex);

                if (gridObject != null) {
                    codeItem = gridObject.code;
                }

                //yield return new WaitForEndOfFrame();
            }

            if (indexItem % 10 == 0) {
                // Every tenth load the environment pads
                //Debug.Log("dynamicPart:10:" + indexItem);
            }

            if (codeItem.IsEqualLowercase(BaseDataObjectKeys.empty)) {
                fillBlock = false;
            }

            if (fillBlock) {

                // ADD PART BLOCK AND ASSETS FROM TEMPLATE

                GameObject goAssetBlock = LoadAssetLevelPlaceholder(
                    data, codeBlock, spawnLocation, indexItem);

                if (goAssetBlock == null) {
                    continue;
                }

                goAssetBlock.Hide();

                if(goItem.transform == null) {
                    goAssetBlock.DestroyGameObject();
                    //yield break;
                    return;
                }

                goAssetBlock.transform.parent = goItem.transform;
                goAssetBlock.transform.position = goItem.transform.position;
                //goAssetBlock.transform.localPosition = goItem.transform.localPosition.WithX(data.lines[i].x);

                goAssetBlock.Show();

                //yield return new WaitForEndOfFrame();
            }

            // --------------------------------------------------------------------
            // LOAD DATA GRID ITEM

            if (indexItem > data.partBackCount
                && !codeItem.IsNullOrEmpty()
                && fillBlock
                && codeItem != codeBlock) {

                // TODO change to game specific lookup

                codeItem = GameController.GameItemCodeContextGet(codeItem);

                GameObject goAssetItem;

                bool isItem = codeItem.StartsWith(BaseDataObjectKeys.item);

                if (isItem) {

                    goAssetItem = AppContentAssets.LoadAssetItems(codeItem, spawnLocation);
                }
                else {

                    goAssetItem = LoadAssetLevelPlaceholder(data, codeItem, spawnLocation, indexItem);
                }

                if (goAssetItem == null) {
                    //Debug.Log("Asset not found items/" + codeItem);
                    continue;
                }

                goAssetItem.Hide();

                float posY = 0f;

                if (isItem) {
                    posY = 2f;
                }

                // ADD ITEM COIN LOCATION
                //if (codeItem.IsEqualLowercase("item-coin")) {
                //if (codeItem.IsEqualLowercase("item-special")) {

                if(goItem.transform == null) {
                    goAssetItem.DestroyGameObject();
                    //yield break;
                    return;
                }

                goAssetItem.transform.parent = goItem.transform;

                goAssetItem.transform.position = goItem.transform.position.WithX(0);
                goAssetItem.transform.localPosition = goItem.transform.localPosition.WithX(0).WithY(posY).WithZ(0);

                goAssetItem.Show();

                goAssetItem.SetOnly<GameObjectInfiniteAssetItem>();

                //yield return new WaitForEndOfFrame();
            }

            // reset position to view

            go.transform.position = go.transform.position.WithY(0);
        }
    }


    public override void LoadLevelAssetsPeriodic(
        GameObjectInfinteData data, GameObject parentGo, int indexItem, bool clear = true) {

    //    StartCoroutine(LoadLevelAssetsPeriodicCo(data, parentGo, indexItem, clear));
    //}

    //public override IEnumerator LoadLevelAssetsPeriodicCo(
    //         GameObjectInfinteData data, GameObject parentGo, int indexItem, bool clear = true) {

        if(parentGo == null) {
            //yield break;
            return;
        }
        //yield break;

        if(indexItem <= 0) {
            //return;
        }

        //if (((indexItem + 1) * data.distanceTickZ) % data.distanceTickZ == 0) {
        if ((indexItem + 1) % (data.distanceTickZ / 2) == 0) { // every 8

            // Load terrain and ambience

            GameObject go = AppContentAssets.LoadAssetLevelAssets(
                data.codeGameFloor, 
                parentGo.transform.position);

            if (go == null) {
                //Debug.Log("Asset not found levelassets/" + data.codeGameFloor);

                //yield break;
                return;
            }

            if(parentGo.transform == null) {
                go.DestroyGameObject();

                //yield break;
                return;
            }

            go.transform.parent = parentGo.transform;

            go.transform.localPosition = go.transform.localPosition.WithY(-data.distanceTickZ);
            go.ResetRotation();

            go.SetOnly<GameObjectInfiniteAsset>();
        }

        //yield return new WaitForEndOfFrame();

        //return;

        if ((indexItem + 1) % (data.distanceTickZ / 2) == 0) {

            // Load terrain and ambience

            // right

            GameObject goSideRight = LoadAssetLevelPlaceholder(data, data.codeGameSide, parentGo.transform.position, indexItem);

            if(goSideRight == null) {
                //Debug.Log("Asset not found levelassets/" + data.codeGameSide);

                //yield break;
                return;
            }

            if(parentGo.transform == null) {
                goSideRight.DestroyGameObject();

                //yield break;
                return;
            }

            goSideRight.transform.parent = parentGo.transform;
            goSideRight.transform.localRotation = Quaternion.Euler(0, 90, 0);
            goSideRight.transform.localPosition = goSideRight.transform.localPosition.WithX(24).WithY(0);

            goSideRight.SetOnly<GameObjectInfiniteAsset>();


            //yield return new WaitForEndOfFrame();

            // left
            
            GameObject goSideLeft = LoadAssetLevelPlaceholder(data, data.codeGameSide, parentGo.transform.position, indexItem);

            if (goSideLeft == null ) {
                //Debug.Log("Asset not found levelassets/" + data.codeGameSide);

                //yield break;
                return;
            }

            if(parentGo.transform == null) {
                goSideRight.DestroyGameObject();

                //yield break;
                return;
            }

            goSideLeft.transform.parent = parentGo.transform;
            goSideLeft.transform.localRotation = Quaternion.Euler(0, -90, 0);
            goSideLeft.transform.localPosition = goSideLeft.transform.localPosition.WithX(-24).WithY(0);

            goSideRight.SetOnly<GameObjectInfiniteAsset>();

            //yield return new WaitForEndOfFrame();
            
        }
    }

 */
