using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// MESSAGES / EVENTS

// CONTROLLER

public class BaseGameContentDisplayTypes {
    public static string gamePlayerOutOfBounds = "content-game-player-out-of-bounds";
    public static string gameChoices = "content-game-game-choices";
    public static string gameChoicesOverview = "content-game-game-choices-overview";
    public static string gameChoicesItemStart = "content-game-game-choices-item-start";
    public static string gameChoicesItemResult = "content-game-game-choices-item-result";
    public static string gameCollect = "content-game-game-collect";
    public static string gameCollectOverview = "content-game-game-collect-overview";
    public static string gameCollectItemStart = "content-game-game-collect-item-start";
    public static string gameCollectItemResult = "content-game-game-collect-item-result";
    public static string gameEnergy = "content-game-game-energy";
    public static string gameHealth = "content-game-game-health";
    public static string gameXP = "content-game-game-xp";
    public static string gameTips = "content-game-tips";
    public static string gameTutorial = "content-game-tutorial";
    public static string gameModeContentOverview = "content-game-mode-content-overview";
}

public class BaseGamePlayerMessages {

    public static string PlayerAnimation = "playerAnimation";
    public static string PlayerAnimationSkill = "skill";
    public static string PlayerAnimationAttack = "attack";
    public static string PlayerAnimationFall = "fall";
    //

    public static string PlayerCurrentDistance = "player-current-distance";
    public static string PlayerOverallDistance = "player-overall-distance";
}


public class BaseGameplayType {
    public static string gameDasher = "game-dasher";
    public static string gameRunner = "game-runner";
}

public class BaseGameplayWorldType {
    public static string gameDefault = "game-default";
    public static string gameStationary = "game-stationary";
}

public class GameplayType : BaseGameplayType {

}

public class GameplayWorldType : BaseGameplayWorldType {

}

public class GameActorType {
    public static string enemy = "enemy";
    public static string player = "player";
    public static string sidekick = "sidekick";
}

public class GameSpawnType {
    public static string centeredType = "centered";
    public static string zonedType = "zoned";
    public static string pointsType = "points";
    public static string explicitType = "explicit";
    public static string randomType = "random";
}


public class BaseGameMessages {
    //
    public static string gameActionItem = "game-action-item";
    public static string gameActionScores = "game-action-scores";
    public static string gameActionScore = "game-action-score";
    public static string gameActionAmmo = "game-action-ammo";
    public static string gameActionSave = "game-action-save";
    public static string gameActionShot = "game-action-shot";
    public static string gameActionLaunch = "game-action-launch";
    public static string gameActionState = "game-action-state";
    //

    public static string gameActionAssetAttack = "game-action-asset-attack";
    public static string gameActionAssetSave = "game-action-asset-save";
    public static string gameActionAssetBuild = "game-action-asset-build";
    public static string gameActionAssetRepair = "game-action-asset-repair";
    public static string gameActionAssetDefend = "game-action-asset-defend";
    //
    public static string gameInitLevelStart = "game-init-level-start";
    public static string gameInitLevelEnd = "game-init-level-end";
    public static string gameLevelPlayerReady = "game-level-player-ready";
    public static string gameLevelStart = "game-level-start";
    public static string gameLevelEnd = "game-level-end";
    public static string gameLevelQuit = "game-level-quit";
    public static string gameLevelPause = "game-level-pause";
    public static string gameLevelResume = "game-level-resume";
    public static string gameResultsStart = "game-results-start";
    public static string gameResultsEnd = "game-results-end";
}

public class BaseGameStatCodes {

    public static string timesPlayed = "times-played";//
    public static string timePlayed = "time-played";//
    public static string timesPlayedAction = "times-played-action";//

    // totals
    public static string wins = "wins";//--
    public static string losses = "losses";//--
    public static string shots = "shots";//--
    public static string destroyed = "destroyed";//--

    public static string score = "score";//--
    public static string scores = "scores";//--
    public static string special = "special";//--
    public static string evaded = "evaded";//--
    public static string kills = "kills";
    public static string deaths = "deaths";
    public static string hits = "hits";
    public static string hitsReceived = "hits-received";
    public static string hitsObstacles = "hits-obstacles";
    public static string xp = "xp";
    public static string coins = "coins";
    public static string coinsPickup = "coinsPickup";
    public static string coinsPurchased = "coinsPurchased";
    public static string coinsEarned = "coinsEarned";
    public static string ammo = "ammo";
    public static string attacks = "attacks";//
    public static string defends = "defends";//
    public static string repairs = "repairs";//
    public static string builds = "builds";//
    public static string saves = "saves";

    public static string cuts = "cuts";//
    public static string cutsLeft = "cuts-left";//
    public static string cutsRight = "cuts-right";//
    public static string boosts = "boosts";//
    public static string spins = "spins";//
    public static string total = "total";//
    public static string item = "item";//

    // lows

    // absolute

    // accumulate (total)
}

// PLAYER

public class GameActionKeys {
    public static string GameGoalZone = "GameGoalZone";
    public static string GameBadZone = "GameBadZone";
    public static string GameZone = "GameZone";
    public static string GameBoundaryZone = "GameBoundaryZone";
    //
    public static string GameZoneActionTrigger = "GameZoneActionTrigger";
    public static string GameZoneAction = "GameZoneAction";
    public static string GameZoneActionArea = "GameZoneActionArea";
    public static string GameZoneActionCollider = "GameZoneActionCollider";
    //
}