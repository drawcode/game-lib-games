using System;
using System.Collections.Generic;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;
using Engine.Utility;

// LEVEL CODE MIGRATION (iter 45, owner): moves a player's saved data off the bare "<world_num>-<n>"
// codes when a game renames its shipped levels to "<kind>-<world_num>-<n>" (GameLevelCodes). The
// game supplies the renumbering as old n -> "<kind>-<new n>", the same in every world, e.g.
// Action Bots: 10 -> "mission-4", 11 -> "tutorial-1", 90 -> "test-1".
//
// Two passes, both safe to repeat:
//
//   statistics   per profile (each save slot loads its own blob): every per-level key
//                "<high|low|total|times-played-action>-<stat>-<w>-<n>" moves to the new code,
//                merged with what the new code already holds (high max, low min, else sum).
//                A flag attribute on the same blob stops a second run double-counting.
//   saved levels per install (one items folder): a bare save WITH items moves to its new code,
//                or, when the number was never official, to the next free "create-<w>-<n>", so a
//                level made in CREATE before iter 43 shows in MY LEVELS. A save already at the new
//                code wins; the old file then stays where it is.
//
// Not moved: mission stars and arcade stages (keyed by the collect row, not the level code), and
// the profile's stored current level (LoadState never applies it). A game that still ships bare
// codes (Action Bikes) must not run this.
public class GameLevelCodeMigration {

    public static string statisticsFlag = "migration-level-codes-v1";
    public static string migrationType = "migration";

    // the per-level statistic key heads (GameStatisticCodes *CodeLevel)
    static readonly string[] levelKeyHeads = {
        GameStatisticCodes.high + "-",
        GameStatisticCodes.low + "-",
        GameStatisticCodes.total + "-",
        GameStatisticCodes.timesPlayedAction + "-"
    };

    static readonly string[] kindNames = {
        GameLevelCodes.missionKind,
        GameLevelCodes.tutorialKind,
        GameLevelCodes.testKind,
        GameLevelCodes.arcadeKind,
        GameLevelCodes.createdKind
    };

    static bool savedLevelsRun = false;

    // Both passes for the current profile. Returns true when anything moved. Saved levels are on
    // disk at once; the statistics persist with the profile's next save (a caller inside profile
    // loading must not save from there).
    public static bool Run(Dictionary<int, string> renames) {

        int stats = MigrateStatistics(GameProfileStatistics.Current, renames);
        int levels = 0;

        if (!savedLevelsRun) {
            savedLevelsRun = true;
            levels = MigrateSavedLevels(renames);
        }

        if (stats > 0 || levels > 0) {
            LogUtil.Log("GameLevelCodeMigration: moved " + stats + " statistics, " + levels + " saved levels");
        }

        return stats > 0 || levels > 0;
    }

    // "1-10" -> "mission-1-4" with renames[10] = "mission-4"; null for a code that is not bare or
    // whose number has no rename.
    public static string MapBare(string code, Dictionary<int, string> renames) {

        int worldNum;
        int levelNum;

        if (renames == null || !TryParseBare(code, out worldNum, out levelNum)) {
            return null;
        }

        string to;

        if (!renames.TryGetValue(levelNum, out to) || string.IsNullOrEmpty(to)) {
            return null;
        }

        int at = to.LastIndexOf('-');
        int newNum;

        if (at <= 0 || !int.TryParse(to.Substring(at + 1), out newNum)) {
            return null;
        }

        return GameLevelCodes.Format(to.Substring(0, at), worldNum, newNum);
    }

    // exactly "<int>-<int>"
    public static bool TryParseBare(string code, out int worldNum, out int levelNum) {

        worldNum = 0;
        levelNum = 0;

        if (string.IsNullOrEmpty(code)) {
            return false;
        }

        string[] parts = code.Split('-');

        return parts.Length == 2
            && int.TryParse(parts[0], out worldNum)
            && int.TryParse(parts[1], out levelNum);
    }

    // "high-score-1-10" -> head "high-score", bare "1-10". False for a key that is not a per-level
    // statistic, or whose level part is already kind-prefixed ("high-score-mission-1-4", and the
    // older "high-score-arcade-battle-1-1").
    public static bool SplitLevelKey(string key, out string head, out string bare) {

        head = null;
        bare = null;

        if (string.IsNullOrEmpty(key)) {
            return false;
        }

        string keyHead = null;

        foreach (string candidate in levelKeyHeads) {
            if (key.StartsWith(candidate, StringComparison.Ordinal)) {
                keyHead = candidate;
                break;
            }
        }

        if (keyHead == null) {
            return false;
        }

        string[] parts = key.Split('-');
        int headParts = keyHead.Split('-').Length - 1;

        // head, at least one stat segment, world, n
        if (parts.Length < headParts + 3) {
            return false;
        }

        int worldNum;
        int levelNum;

        if (!int.TryParse(parts[parts.Length - 2], out worldNum)
            || !int.TryParse(parts[parts.Length - 1], out levelNum)) {
            return false;
        }

        for (int i = headParts; i < parts.Length - 2; i++) {
            if (Array.IndexOf(kindNames, parts[i]) >= 0) {
                return false;
            }
        }

        bare = parts[parts.Length - 2] + "-" + parts[parts.Length - 1];
        head = key.Substring(0, key.Length - bare.Length - 1);

        return true;
    }

    // What the new key holds after the move: a high keeps the best, a low the lowest set value,
    // a total or play count adds up.
    public static double Merge(string head, double existing, double moved) {

        if (head.StartsWith(GameStatisticCodes.high + "-", StringComparison.Ordinal)) {
            return Math.Max(existing, moved);
        }

        if (head.StartsWith(GameStatisticCodes.low + "-", StringComparison.Ordinal)) {
            if (existing <= 0) {
                return moved;
            }
            if (moved <= 0) {
                return existing;
            }
            return Math.Min(existing, moved);
        }

        return existing + moved;
    }

    // Returns how many keys moved; 0 when the blob was already migrated.
    public static int MigrateStatistics(BaseGameProfileStatistic stats, Dictionary<int, string> renames) {

        if (stats == null || renames == null || stats.CheckIfAttributeExists(statisticsFlag)) {
            return 0;
        }

        int moved = 0;
        Dictionary<string, DataAttribute> attributes = stats.GetAttributesDictionary();
        List<DataAttribute> list = attributes != null ? stats.GetList() : new List<DataAttribute>();

        foreach (DataAttribute att in list) {

            string head;
            string bare;

            if (!SplitLevelKey(att.code, out head, out bare)) {
                continue;
            }

            string to = MapBare(bare, renames);

            if (to == null) {
                continue;
            }

            string newKey = head + "-" + to;
            double value = stats.GetStatisticValue(att.code);
            double existing = stats.GetStatisticValue(newKey);

            stats.SetStatisticValue(newKey, Merge(head, existing, value));
            attributes.Remove(att.code);
            moved++;
        }

        DataAttribute flag = new DataAttribute();
        flag.code = statisticsFlag;
        flag.val = moved;
        flag.name = "";
        flag.type = "int";
        flag.otype = migrationType;
        stats.SetAttribute(flag);

        return moved;
    }

    // Every bare code with a saved file in the items folder (with or without items).
    public static List<string> ListSavedBare() {

        List<string> codes = new List<string>();

        string sentinel = GameLevelCodes.Format(0, 0);
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

        string head = file.Substring(0, at);
        string tail = file.Substring(at + sentinel.Length);

        try {
            foreach (string path in System.IO.Directory.GetFiles(dir, head + "*" + tail)) {

                string name = System.IO.Path.GetFileName(path);

                if (name.Length <= head.Length + tail.Length) {
                    continue;
                }

                string code = name.Substring(head.Length, name.Length - head.Length - tail.Length);

                int worldNum;
                int levelNum;

                if (TryParseBare(code, out worldNum, out levelNum)) {
                    codes.Add(code);
                }
            }
        }
        catch (Exception e) {
            LogUtil.LogError("GameLevelCodeMigration: could not list " + dir + ": " + e.Message);
        }

        return codes;
    }

    // Returns how many saved levels moved.
    public static int MigrateSavedLevels(Dictionary<int, string> renames) {

        int moved = 0;

        foreach (string code in ListSavedBare()) {

            if (!GameLevelCodes.HasSavedItems(code)) {
                continue;
            }

            string to = MapBare(code, renames);

            if (to == null) {
                int worldNum;
                int levelNum;
                TryParseBare(code, out worldNum, out levelNum);
                to = GameLevelCodes.NextFreeCreated(worldNum);
            }

            if (to == null || GameLevelCodes.HasSavedItems(to)) {
                LogUtil.Log("GameLevelCodeMigration: kept " + code + " (" + (to ?? "no free code") + " already saved)");
                continue;
            }

            if (MoveSaved(code, to)) {
                moved++;
            }
        }

        return moved;
    }

    // Rewrites the save under the new code (its own code field too), keeps its time so MY LEVELS
    // keeps its order, carries a .bak along, and removes the old file.
    public static bool MoveSaved(string from, string to) {

        string fromPath = GameLevelItemSources.GetSavedPath(from);
        string toPath = GameLevelItemSources.GetSavedPath(to);

        try {
            DateTime written = System.IO.File.GetLastWriteTimeUtc(fromPath);
            GameLevelItem item = System.IO.File.ReadAllText(fromPath).FromJson<GameLevelItem>();

            if (item == null) {
                return false;
            }

            item.code = to;
            GameLevelItems.Instance.SaveItem(GameLevelItems.Instance.DATA_KEY, to, item);
            System.IO.File.SetLastWriteTimeUtc(toPath, written);

            string fromBackup = GameLevelItemSources.GetSavedBackupPath(from);
            string toBackup = GameLevelItemSources.GetSavedBackupPath(to);

            if (System.IO.File.Exists(fromBackup) && !System.IO.File.Exists(toBackup)) {
                System.IO.File.Move(fromBackup, toBackup);
            }

            System.IO.File.Delete(fromPath);

            LogUtil.Log("GameLevelCodeMigration: saved " + from + " -> " + to);

            return true;
        }
        catch (Exception e) {
            LogUtil.LogError("GameLevelCodeMigration: could not move " + from + " -> " + to + ": " + e.Message);
            return false;
        }
    }
}
