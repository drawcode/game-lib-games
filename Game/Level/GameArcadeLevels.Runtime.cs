using System;
using System.Collections.Generic;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Utility;

// The engine side of GameArcadeLevels: a generated row becomes a GameLevel in GameLevels and the
// current level. The arcade route (P2 in contexts games/shared/context-arcade-vs-missions.md)
// calls StartRound where the mission route calls GameLevels.Instance.ChangeCurrent.
//
// An arcade row is an app-content-collection-data row with `type: arcade` and a top-level
// `play_type`; its actions are a copy of the type's first mission's. Each row keeps a STAGE per
// world in the profile (0..tierMax, shown as stage + 1): a round whose type action is done when it
// ends moves that world's stage up one, and a failed round leaves it.
public static partial class GameArcadeLevels {

    // profile content-collect type and attribute holding the stages
    public static string stageCollectType = "arcade";
    public static string stageKey = "arcade_stage";

    public static bool IsArcade(AppContentCollect row) {
        return row != null && row.type == GameArcadeLevelKeys.arcade;
    }

    public static string PlayType(AppContentCollect row) {
        object value;
        return row != null && row.TryGetValue(GameArcadeLevelKeys.play_type, out value) && value != null
            ? value.ToString() : null;
    }

    static string StageKey(AppContentCollect row, string worldCode) {
        return row.code + "-" + worldCode;
    }

    // The row's stage in the world, 0 when never cleared. Read without Get: a JSON load gives a
    // long or double, and Get<int> would throw and write a default into the profile.
    public static int Stage(AppContentCollect row, string worldCode) {

        if (row == null || GameProfileModes.Current == null) {
            return 0;
        }

        GameProfileContentCollectItem item =
            GameProfileModes.Current.GetContentCollectItem(stageCollectType, StageKey(row, worldCode));

        object raw;

        if (item == null || !item.TryGetValue(stageKey, out raw) || raw == null) {
            return 0;
        }

        try {
            return ClampTier(Convert.ToInt32(raw));
        }
        catch (Exception) {
            return 0;
        }
    }

    public static void SetStage(AppContentCollect row, string worldCode, int stage) {
        GameProfileModes.Current.SetContentCollectValue(
            stageCollectType, StageKey(row, worldCode), stageKey, ClampTier(stage));
    }

    // Sets the row's type action to the stage's goal (Goals), so the list, the HUD and the
    // round's scoring all read the same target. The row is the loaded data, edited in memory.
    public static void ApplyGoal(AppContentCollect row, int stage) {

        string playType = PlayType(row);
        string actionCode = ActionFor(playType);

        if (actionCode == null) {
            return;
        }

        List<object> goals = Goals(playType, stage, AppContentCollects.Instance.items);

        if (goals == null) {
            return;
        }

        object val = null;

        foreach (object goal in goals) {
            if (Str(Get(Dict(goal), "code")) == actionCode) {
                val = Get(Dict(Get(Dict(goal), "data")), "val");
            }
        }

        if (val == null) {
            return;
        }

        foreach (AppContentCollectItem action in row.data.data) {
            if (action != null && action.code == actionCode && action.data != null) {
                action.data.Set("val", val);
            }
        }
    }

    // The arcade route: the row's goal and level for the current world at its stage, the level
    // registered and current. Null, with nothing changed but the goal, when the type has no
    // mission levels in the world.
    public static GameLevel StartRound(AppContentCollect row) {

        string playType = PlayType(row);

        if (!IsArcade(row) || ActionFor(playType) == null || GameWorlds.Current == null) {
            return null;
        }

        int stage = Stage(row, GameWorlds.Current.code);

        ApplyGoal(row, stage);

        return GenerateCurrent(playType, stage, UnityEngine.Random.Range(0, int.MaxValue));
    }

    // Called when an arcade round ends: if the current level is the current arcade row's
    // generated level and the row's type action is done, that world's stage goes up one.
    // A created level also plays in the arcade state, so the level is checked, not the mode.
    public static bool RecordRound(GameGameRuntimeData gameRuntimeData, GamePlayerRuntimeData playerRuntimeData) {

        AppContentCollect row = AppContentCollects.Current;
        string playType = PlayType(row);
        string actionCode = ActionFor(playType);

        if (!IsArcade(row) || actionCode == null || GameWorlds.Current == null || GameLevels.Current == null
            || GameLevels.Current.data == null) {
            return false;
        }

        object levelPlayType;

        if (!GameLevels.Current.data.TryGetValue(GameArcadeLevelKeys.play_type, out levelPlayType)
            || levelPlayType == null || levelPlayType.ToString() != playType) {
            return false;
        }

        foreach (AppContentCollectItem action in row.GetItemsData()) {
            if (action != null && action.code == actionCode) {

                if (!action.IsCompleted(gameRuntimeData, playerRuntimeData)) {
                    return false;
                }

                string worldCode = GameWorlds.Current.code;
                SetStage(row, worldCode, Stage(row, worldCode) + 1);

                return true;
            }
        }

        return false;
    }

    // Generates the type's row for the current world from the loaded missions and levels,
    // registers it and makes it current. Returns null, and changes nothing, when the type has no
    // mission levels in this world.
    public static GameLevel GenerateCurrent(string playType, int tier, int seed) {

        if (GameWorlds.Current == null) {
            return null;
        }

        Dictionary<string, object> row = Generate(
            playType, GameWorlds.Current.data.world_num, tier, seed,
            AppContentCollects.Instance.items, GameLevels.Instance.items);

        if (row == null) {
            return null;
        }

        GameLevel level = Register(row);
        GameLevels.Instance.ChangeCurrent(level.code);

        return level;
    }

    // Adds the row to GameLevels, replacing an earlier row with its code: a duplicate code would
    // keep DataObjects rebuilding its lookups on every GetByCode.
    public static GameLevel Register(Dictionary<string, object> row) {

        GameLevel level = row.ToJson().FromJson<GameLevel>();
        List<GameLevel> items = GameLevels.Instance.items;

        for (int i = 0; i < items.Count; i++) {
            if (items[i] != null && items[i].code == level.code) {
                items[i] = level;
                return level;
            }
        }

        items.Add(level);

        return level;
    }
}
