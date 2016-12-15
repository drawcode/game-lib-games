using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameZoneKeys {
    public static string none = "game-zone-none";
    public static string goal = "game-zone-goal";
    public static string goal_left = "game-zone-goal-left";
    public static string goal_right = "game-zone-goal-right";
    public static string boundary = "game-zone-boundary";
    public static string action = "game-zone-action";
    public static string bad = "game-zone-bad";
    public static string bad_score = "game-zone-bad-score";
    public static string bad_out_of_bounds = "game-zone-bad-out-of-bounds";
    //
    public static string action_none = "game-zone-action-none";
    public static string action_attack = "game-zone-action-attack";
    public static string action_defend = "game-zone-action-defend";
    public static string action_repair = "game-zone-action-repair";
    public static string action_save = "game-zone-action-save";
    public static string action_build = "game-zone-action-build";
}

public class GameZoneActions {    
    public static string action_kill = "action-kill";
    public static string action_collect = "action-collect";
    public static string action_attack = "action-attack";
    public static string action_defend = "action-defend";
    public static string action_repair = "action-repair";
    public static string action_build = "action-build";
    public static string action_none = "action-none";
    public static string action_save = "action-save";
}

public enum GameZoneActionAssetState {
    none,
    creating,
    destroying,
    created,
    destroyed
}

public class GameZone : GameObjectBehavior {

    public string gameZoneType = GameZoneKeys.none;
    //
    public GameObject container;
    public GameObject containerAssets;
    public GameObject containerAssetsPlatforms;
    //    
    public GameObject containerIcons;
    //
    public GameObject containerEffects;
    public GameObject containerEffectsScore;
    public GameObject containerEffectsIndicator;
    public GameObject containerEffectsRepair;
    public GameObject containerEffectsBuild;
    //
    public string assetCode = BaseDataObjectKeys.none;
    internal string lastAssetCode = null;
    public string assetPlatformCode = BaseDataObjectKeys.none;
    internal string lastAssetPlatformCode = null;
    //
    public string assetEffectProgressRepairCode = BaseDataObjectKeys.none;
    internal string lastAssetEffectProgressRepairCode = null;
    public string assetEffectProgressBuildCode = BaseDataObjectKeys.none;
    internal string lastAssetEffectProgressBuildCode = null;
    //
    public string assetColorCode = BaseDataObjectKeys.none;
    internal string lastAssetColorCode = null;
    //
    public string actionCode = BaseDataObjectKeys.none;
    internal string lastActionCode = BaseDataObjectKeys.none;
    //
    public string assetAnimationNamePlay = "start";
    public string assetAnimationNameIdle = "idle";

    // 
    public double totalNeeded = 1.0;
    public double totalCompleted = 0.0;

    public enum GameZoneActionAssetState {
        none,
        creating,
        destroying,
        created,
        destroyed
    }

    // ------------------------------------------------------------

    public virtual void Start() {

    }

    public virtual void OnEnable() {

    }

    public virtual void OnDisable() {

    }

    public virtual void Reset() {
        gameZoneType = GameZoneKeys.none;
        assetCode = BaseDataObjectKeys.none;
        lastAssetCode = null;
        assetPlatformCode = BaseDataObjectKeys.none;
        lastAssetPlatformCode = null;
        assetColorCode = BaseDataObjectKeys.none;
        lastAssetColorCode = null;
        actionCode = BaseDataObjectKeys.none;
        lastActionCode = BaseDataObjectKeys.none;
        totalNeeded = 1.0;
        totalCompleted = 0.0;

    }

    // 

    public bool IsActionCode(string codeTo) {
        return actionCode == codeTo && !string.IsNullOrEmpty(actionCode);
    }

    // CREATE ASSETS

    public bool isActionCodeAttack {
        get {
            return IsActionCode(GameZoneActions.action_attack);
        }
    }

    public bool isActionCodeDefend {
        get {
            return IsActionCode(GameZoneActions.action_defend);
        }
    }

    public bool isActionCodeRepair {
        get {
            return IsActionCode(GameZoneActions.action_repair);
        }
    }

    public bool isActionCodeBuild {
        get {
            return IsActionCode(GameZoneActions.action_build);
        }
    }

    public bool isActionCodeNone {
        get {
            return IsActionCode(GameZoneActions.action_none);
        }
    }


    // COLLECT + SAVE

    public bool isActionCodeSave {
        get {
            return IsActionCode(GameZoneActions.action_save);
        }
    }

    public bool isActionCodeCollect {
        get {
            return IsActionCode(GameZoneActions.action_collect);
        }
    }

    // 

    public bool isActionCodeKill {
        get {
            return IsActionCode(GameZoneActions.action_kill);
        }
    }

    // ------------------------------------------------------------
    // EFFECTS

    public virtual void PlayEffects() {

        if (FPSDisplay.isUnder25FPS) {
            return;
        }

        if (containerEffects != null) {
            containerEffects.Show();
            containerEffects.PlayParticleSystem(true);
        }
    }

    public virtual void StopEffects() {
        if (containerEffects != null) {
            containerEffects.StopParticleSystem(true);
        }
    }

    // ------------------------------------------------------------
    // EFFECTS SCORE

    public virtual void PlayEffectsScore() {

        if (FPSDisplay.isUnder25FPS) {
            return;
        }

        Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();

        if (containerEffectsScore != null) {
            containerEffectsScore.Show();
            containerEffectsScore.SetParticleSystemStartColor(colorTo, true);
            containerEffectsScore.PlayParticleSystem(true);
        }
    }

    public virtual void StopEffectsScore() {
        if (containerEffectsScore != null) {
            containerEffectsScore.StopParticleSystem(true);
        }
    }

    // ------------------------------------------------------------
    // EFFECTS INDICATOR

    public virtual void PlayEffectsIndicator() {

        if (FPSDisplay.isUnder25FPS) {
            return;
        }

        if (containerEffectsIndicator != null) {
            containerEffectsIndicator.Show();
            containerEffectsIndicator.PlayParticleSystem(true);
        }
    }

    public virtual void StopEffectsIndicator() {
        if (containerEffectsIndicator != null) {
            containerEffectsIndicator.StopParticleSystem(true);
        }
    }

    // ------------------------------------------------------------
    // ASSETS

    public virtual void LoadAsset(string assetCodeTo) {
        LoadAsset(assetCodeTo, Vector3.zero);
    }

    public virtual void LoadAsset(string assetCodeTo, Vector3 pos) {
        if (string.IsNullOrEmpty(assetCodeTo)) {
            return;
        }

        if (containerAssets == null) {
            return;
        }

        assetCode = assetCodeTo;

        // Load in asset at position

        containerAssets.DestroyChildren();
    }
}