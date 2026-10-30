using System;
using System.Collections.Generic;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Utility;

// Level codes are "<world_num>-<n>" (game-level-data rows). A new level created from the worlds
// screen takes the first n in its world that nothing already uses, so it can never open or
// overwrite another level's layout:
//
//   taken = a level row with the code, OR a saved layout with items (the player's editor save),
//           OR a shipped layout with items (tools/level-promote)
//
// A saved file with no items is NOT taken: every level that was ever loaded has one (the item
// loader writes an empty file on first load), including a create that was never saved.
//
// A level only plays as itself with a row (GameLevels.ChangeCurrent on an unknown code falls back
// to the first row, 1-1), so a new code gets a row copied from its world's first level - same
// presets, terrain and directors, new code. The row lives for the session; the saved layout keeps
// the code taken across restarts.
public class GameLevelCodes {

    // n stops below the 90s block the shipped data uses for its special rows (x-90..x-93).
    public static int maxLevelNum = 89;

    public static string Format(int worldNum, int levelNum) {
        return worldNum + "-" + levelNum;
    }

    public static bool IsTaken(string code) {

        if (string.IsNullOrEmpty(code)) {
            return true;
        }

        if (GameLevels.Instance.GetById(code) != null) {
            return true;
        }

        if (HasSavedItems(code)) {
            return true;
        }

        GameLevelItem shipped = GameLevelItemSources.LoadShipped(code);

        return shipped != null && GameLevelItemSources.HasItems(shipped.level_items);
    }

    // Reads the saved file directly: GameLevelItems.LoadItem writes an empty file when there is
    // none, which would make a probe create the thing it is checking for.
    public static bool HasSavedItems(string code) {

        string path = GameLevelItemSources.GetSavedPath(code);

        try {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) {
                return false;
            }

            string text = System.IO.File.ReadAllText(path);

            if (string.IsNullOrEmpty(text)) {
                return false;
            }

            GameLevelItem item = text.FromJson<GameLevelItem>();

            return item != null && GameLevelItemSources.HasItems(item.level_items);
        }
        catch (Exception e) {
            // unreadable counts as taken: never hand out a code whose file we cannot see into
            LogUtil.LogError("GameLevelCodes: could not read " + path + ": " + e.Message);
            return true;
        }
    }

    // The first free "<worldNum>-<n>", or null when the world is full.
    public static string NextFree(int worldNum) {

        for (int n = 1; n <= maxLevelNum; n++) {

            string code = Format(worldNum, n);

            if (!IsTaken(code)) {
                return code;
            }
        }

        return null;
    }

    // Adds a row for the next free code in the world and returns the code (null if none).
    public static string CreateInWorld(GameWorld world) {

        if (world == null || world.data == null) {
            return null;
        }

        int worldNum = world.data.world_num;
        string code = NextFree(worldNum);

        if (code == null) {
            LogUtil.LogError("GameLevelCodes: world " + world.code + " has no free level code");
            return null;
        }

        GameLevel template = GetTemplate(world, worldNum);

        GameLevel level = new GameLevel();
        level.active = true;
        level.code = code;
        level.key = code;
        level.type = template != null ? template.type : "level";
        string num = code.Substring(code.IndexOf('-') + 1);
        level.name = num;
        level.display_name = num;
        level.description = num;
        level.world_code = world.code;
        level.uuid = UniqueUtil.CreateUUID4();

        if (template != null) {
            level.data = template.data;
        }

        GameLevels.Instance.SetGameLevel(level);

        LogUtil.Log("GameLevelCodes: created " + code + " in " + world.code
            + (template != null ? " from " + template.code : ""));

        return code;
    }

    static GameLevel GetTemplate(GameWorld world, int worldNum) {

        GameLevel first = GameLevels.Instance.GetById(Format(worldNum, 1));

        if (first != null) {
            return first;
        }

        List<GameLevel> levels = GameLevels.Instance.GetByWorldId(world.code);

        return levels != null && levels.Count > 0 ? levels[0] : null;
    }
}
