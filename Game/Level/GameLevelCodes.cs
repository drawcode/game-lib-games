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
//
// PLAYER-CREATED LEVELS (iter 43, owner): a level made in CREATE is "create-<world_num>-<n>", never
// a bare "<world_num>-<n>". Official codes and player codes then live in separate namespaces: a
// level that ships LATER as 1-5 can no longer be shadowed by a player's saved 1-5 (saved wins over
// shipped), and a create never takes a number the official worlds may want. No shipped row may use
// the prefix (tools/content-lint checks it). To make a created level official, promote it with
// tools/level-promote --as <official code>.
public class GameLevelCodes {

    // n stops below the 90s block the shipped data uses for its special rows (x-90..x-93).
    public static int maxLevelNum = 89;

    // the namespace for player-created levels, and how many one world can hold
    public static string createdPrefix = "create-";
    public static int maxCreatedNum = 999;

    public static string Format(int worldNum, int levelNum) {
        return worldNum + "-" + levelNum;
    }

    public static string FormatCreated(int worldNum, int levelNum) {
        return createdPrefix + Format(worldNum, levelNum);
    }

    public static bool IsCreated(string code) {
        return !string.IsNullOrEmpty(code)
            && code.StartsWith(createdPrefix, StringComparison.OrdinalIgnoreCase);
    }

    // "create-2-7" -> world 2, level 7; false for anything else (official codes included).
    public static bool TryParseCreated(string code, out int worldNum, out int levelNum) {

        worldNum = 0;
        levelNum = 0;

        if (!IsCreated(code)) {
            return false;
        }

        string[] parts = code.Substring(createdPrefix.Length).Split('-');

        return parts.Length == 2
            && int.TryParse(parts[0], out worldNum)
            && int.TryParse(parts[1], out levelNum);
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

    // The first free "create-<worldNum>-<n>", or null when the world is full.
    public static string NextFreeCreated(int worldNum) {

        for (int n = 1; n <= maxCreatedNum; n++) {

            string code = FormatCreated(worldNum, n);

            if (!IsTaken(code)) {
                return code;
            }
        }

        return null;
    }

    // Adds a row for the next free created code in the world and returns the code (null if none).
    public static string CreateInWorld(GameWorld world) {

        if (world == null || world.data == null) {
            return null;
        }

        int worldNum = world.data.world_num;
        string code = NextFreeCreated(worldNum);

        if (code == null) {
            LogUtil.LogError("GameLevelCodes: world " + world.code + " has no free level code");
            return null;
        }

        AddRow(world, code);

        return code;
    }

    // A created level saved in an earlier session has no row (rows live for the session), and
    // ChangeCurrent on a code without one plays 1-1 instead. Puts the row back from the code's
    // world. True when the code has a row afterwards.
    public static bool EnsureRow(string code) {

        if (string.IsNullOrEmpty(code)) {
            return false;
        }

        if (GameLevels.Instance.GetById(code) != null) {
            return true;
        }

        int worldNum;
        int levelNum;

        if (!TryParseCreated(code, out worldNum, out levelNum)) {
            return false;
        }

        GameWorld world = GameWorlds.Instance.GetByWorldNum(worldNum);

        if (world == null || world.data == null) {
            LogUtil.LogError("GameLevelCodes: no world " + worldNum + " for " + code);
            return false;
        }

        AddRow(world, code);

        return true;
    }

    static void AddRow(GameWorld world, string code) {

        GameLevel template = GetTemplate(world, world.data.world_num);

        GameLevel level = new GameLevel();
        level.active = true;
        level.code = code;
        level.key = code;
        level.type = template != null ? template.type : "level";
        string num = DisplayName(code);
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
    }

    // "create-1-3" -> "1-3" (the namespace is not shown); an official code is its own name.
    public static string DisplayName(string code) {

        if (IsCreated(code)) {
            return code.Substring(createdPrefix.Length);
        }

        return code.Substring(code.IndexOf('-') + 1);
    }

    // Every created level the player has SAVED with items, newest first. Read from the saved
    // files' names (game-level-item-data-<code>-<version>.json, via GetSavedPath), so a level
    // shows up across restarts without a row. A create that was never saved is not listed.
    public static List<string> ListCreated() {

        List<string> codes = new List<string>();

        string sentinel = FormatCreated(0, 0);
        string probe = GameLevelItemSources.GetSavedPath(sentinel);

        if (string.IsNullOrEmpty(probe)) {
            return codes;
        }

        string dir = System.IO.Path.GetDirectoryName(probe);
        string file = System.IO.Path.GetFileName(probe);
        int at = file.IndexOf(sentinel, StringComparison.Ordinal);

        if (at < 0 || string.IsNullOrEmpty(dir) || !System.IO.Directory.Exists(dir)) {
            return codes;
        }

        string head = file.Substring(0, at) + createdPrefix;
        string tail = file.Substring(at + sentinel.Length);

        List<KeyValuePair<DateTime, string>> found = new List<KeyValuePair<DateTime, string>>();

        try {
            foreach (string path in System.IO.Directory.GetFiles(dir, head + "*" + tail)) {

                string name = System.IO.Path.GetFileName(path);

                if (name.Length <= head.Length + tail.Length) {
                    continue;
                }

                string code = createdPrefix
                    + name.Substring(head.Length, name.Length - head.Length - tail.Length);

                int worldNum;
                int levelNum;

                if (!TryParseCreated(code, out worldNum, out levelNum) || !HasSavedItems(code)) {
                    continue;
                }

                found.Add(new KeyValuePair<DateTime, string>(System.IO.File.GetLastWriteTimeUtc(path), code));
            }
        }
        catch (Exception e) {
            LogUtil.LogError("GameLevelCodes: could not list " + dir + ": " + e.Message);
        }

        found.Sort((a, b) => b.Key.CompareTo(a.Key));

        for (int i = 0; i < found.Count; i++) {
            codes.Add(found[i].Value);
        }

        return codes;
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
