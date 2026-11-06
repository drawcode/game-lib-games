using System.Collections;
using System.Collections.Generic;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;

// Tutorial route (Action Bots, owner call 2026-11-03): the first time a player starts a mission
// whose type has a tutorial, that tutorial plays first and is marked done. No menu UI.
//
// Data-driven: a tutorial is an app-content-collection row of type "tutorial" whose FIRST action
// is the type's (action-attack, ...), with data.level_kind "tutorial" and its level suffix list,
// so its level is "tutorial-<world>-<n>" through GameContentGate.PickLevelCode. The round plays
// under the tutorial row, never the mission row, so its stars and completion are recorded under
// the tutorial's code and the mission's are untouched (stars are keyed by row code + action uid).
// A game with no tutorial rows (Action Bikes) never gates. BaseUIController only asks while its
// useTutorialGate virtual is on (off by default).
public static class GameTutorials {

    // the row type, and the profile content-collect type the done flags live under
    public static string collectType = "tutorial";
    public static string doneKey = "tutorial_done";

    public static bool IsTutorial(AppContentCollect row) {
        return row != null && row.type == collectType;
    }

    // A row's type action: its first action's code (the mission rows carry no type key).
    public static string TypeAction(AppContentCollect row) {

        if (row == null || row.data == null || row.data.data == null) {
            return null;
        }

        foreach (AppContentCollectItem action in row.data.data) {
            return action != null ? action.code : null;
        }

        return null;
    }

    // The tutorial for a mission: the first tutorial row with the mission's type action. Null for
    // anything that is not a player mission (arcade, tutorial and test rows) and when no tutorial
    // row teaches that type.
    public static AppContentCollect For(AppContentCollect mission, IEnumerable rows) {

        if (mission == null || rows == null || mission.type != BaseDataObjectKeys.mission
            || mission.HasTag(GameContentGate.testTag)) {
            return null;
        }

        string action = TypeAction(mission);

        if (string.IsNullOrEmpty(action)) {
            return null;
        }

        foreach (object raw in rows) {
            AppContentCollect row = raw as AppContentCollect;
            if (IsTutorial(row) && TypeAction(row) == action) {
                return row;
            }
        }

        return null;
    }

    // DONE FLAGS: one per tutorial row, in the profile (content-collect "tutorial", key = row code)

    public static bool IsDone(BaseGameProfileMode profile, AppContentCollect tutorial) {

        if (profile == null || tutorial == null) {
            return false;
        }

        GameProfileContentCollectItem item = profile.GetContentCollectItem(collectType, tutorial.code);

        object raw;

        // read without Get<bool>: a JSON load can hand back a string or a number
        if (item == null || !item.TryGetValue(doneKey, out raw) || raw == null) {
            return false;
        }

        string text = raw.ToString().ToLowerInvariant();
        return text == "true" || text == "1";
    }

    // Only in memory: the caller's round end saves the profile (BaseGameController results).
    public static void MarkDone(BaseGameProfileMode profile, AppContentCollect tutorial) {
        if (profile != null && tutorial != null) {
            profile.SetContentCollectValue(collectType, tutorial.code, doneKey, true);
        }
    }

    // THE GATE: the tutorial to play instead of this mission in this world, or null to play the
    // mission. Null when the type has no tutorial, it is done, or the world has no tutorial level.
    public static AppContentCollect Pending(AppContentCollect mission, int worldNum) {

        AppContentCollect tutorial = For(mission, AppContentCollects.Instance.items);

        if (tutorial == null || IsDone(GameProfileModes.Current, tutorial)) {
            return null;
        }

        return LevelCode(tutorial, worldNum) != null ? tutorial : null;
    }

    // The tutorial's level in the world, or null when that level row does not exist (Resolve
    // hands back the prefixed code even when nothing has it).
    public static string LevelCode(AppContentCollect tutorial, int worldNum) {

        if (tutorial == null) {
            return null;
        }

        string code = GameContentGate.PickLevelCode(
            GameLevelCodes.MissionLevelKind(tutorial), worldNum, tutorial.GetAllowedLevelSuffixList());

        return code != null && GameLevels.Instance.GetById(code) != null ? code : null;
    }

    // Round end (won, lost or out of time): a round played under a tutorial row marks it done, so
    // nobody is stuck replaying it. Quitting mid-round never reaches here, so it plays again.
    public static bool RecordRound() {

        AppContentCollect row = AppContentCollects.Current;

        if (!IsTutorial(row)) {
            return false;
        }

        MarkDone(GameProfileModes.Current, row);
        return true;
    }
}
