using System;
using System.Collections.Generic;
using Engine.Game.App;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;
using Engine.Utility;
using UnityEngine;

// Where a level's placed items come from when it starts.
//
//   saved     - the level the player saved in the level editor (the app cache, items/)
//   shipped   - a level file that ships with the game content (Resources .../data/levels/)
//   generated - the randomized grid every level used before (GetLevelRandomizedGridAssets)
//   auto      - saved, else shipped, else generated (the default)
//
// The source is data, not code: a level row can set "level_items_source" in its "data" block
// (game-level-data), and "level_items_code" to play another level's shipped/saved layout. A file
// with no level_items counts as absent, so every empty save written before iter 40 still plays the
// generated grid. Shipped levels are authored in the editor and promoted with tools/level-promote.
public class GameLevelItemSourceKeys {
    public static string generated = "generated";
    public static string saved = "saved";
    public static string shipped = "shipped";
    public static string auto = "auto";

    // keys on the level row's data block
    public static string level_items_source = "level_items_source";
    public static string level_items_code = "level_items_code";
}

public class GameLevelItemSources {

    // App-wide source when the level row does not set one.
    public static string defaultSource = GameLevelItemSourceKeys.auto;

    // Runtime override over the row and the default (tools, tests, a future settings toggle).
    public static string sourceOverride = null;

    // What the current level actually played, for logs and probes.
    public static string currentSource = GameLevelItemSourceKeys.generated;

    public static string shippedFolder = "data/levels/";

    // The source the level asks for: override, then the level row, then the app default.
    public static string GetRequestedSource() {

        if (!string.IsNullOrEmpty(sourceOverride)) {
            return sourceOverride;
        }

        string fromRow = GetLevelRowString(GameLevelItemSourceKeys.level_items_source);

        if (!string.IsNullOrEmpty(fromRow)) {
            return fromRow.ToLower();
        }

        return defaultSource;
    }

    // The layout code to play: the row's level_items_code, else the level's own code.
    public static string GetLayoutCode(string levelCode) {

        string fromRow = GetLevelRowString(GameLevelItemSourceKeys.level_items_code);

        return string.IsNullOrEmpty(fromRow) ? levelCode : fromRow;
    }

    // Sets GameLevelItems.Current.level_items from a saved or shipped level when the requested
    // source has one. Returns false when the caller should generate the grid as before.
    // GameLevelItems.Current must already hold the saved file for levelCode (ChangeCurrentAbsolute).
    public static bool ApplyCurrent(string levelCode) {

        string requested = GetRequestedSource();
        string layoutCode = GetLayoutCode(levelCode);

        currentSource = GameLevelItemSourceKeys.generated;

        if (requested == GameLevelItemSourceKeys.generated) {
            return false;
        }

        bool trySaved = requested == GameLevelItemSourceKeys.saved
            || requested == GameLevelItemSourceKeys.auto;
        bool tryShipped = requested == GameLevelItemSourceKeys.shipped
            || requested == GameLevelItemSourceKeys.auto;

        if (trySaved) {

            List<GameLevelItemAsset> savedItems = GetSavedItems(levelCode, layoutCode);

            if (HasItems(savedItems)) {
                GameLevelItems.Current.level_items = savedItems;
                currentSource = GameLevelItemSourceKeys.saved;
                LogUtil.Log("GameLevelItemSources: " + levelCode + " plays saved " + layoutCode
                    + " (" + savedItems.Count + " items)");
                return true;
            }
        }

        if (tryShipped) {

            GameLevelItem shipped = LoadShipped(layoutCode);

            if (shipped != null && HasItems(shipped.level_items)) {
                GameLevelItems.Current.level_items = shipped.level_items;
                currentSource = GameLevelItemSourceKeys.shipped;
                LogUtil.Log("GameLevelItemSources: " + levelCode + " plays shipped " + layoutCode
                    + " (" + shipped.level_items.Count + " items)");
                return true;
            }
        }

        return false;
    }

    // The saved items for the layout: Current when it is that level's file, else a load of it.
    static List<GameLevelItemAsset> GetSavedItems(string levelCode, string layoutCode) {

        GameLevelItem current = GameLevelItems.Current;

        if (layoutCode == levelCode) {
            return current != null && current.code == levelCode ? current.level_items : null;
        }

        GameLevelItem other = GameLevelItems.Instance.LoadItem(GameLevelItems.Instance.DATA_KEY, layoutCode);

        return other != null ? other.level_items : null;
    }

    public static bool HasItems(List<GameLevelItemAsset> items) {
        return items != null && items.Count > 0;
    }

    // Drops the grid's "empty" placeholders (the loader skips them; 1-1 saved 920 of 1158 entries,
    // 1.7 MB). Returns how many were removed.
    public static int StripEmpty(GameLevelItem item) {

        if (item == null || item.level_items == null) {
            return 0;
        }

        return item.level_items.RemoveAll(
            asset => asset == null || asset.code == BaseDataObjectKeys.empty);
    }

    // The player's saved file for a level code (versioned, in the app cache).
    public static string GetSavedPath(string code) {
        string path = GameLevelItems.Instance.GetPathItem(GameLevelItems.Instance.DATA_KEY, code, "items");
        return Engine.Content.Contents.GetFileVersioned(path);
    }

    // Drops the player's saved level so the level plays shipped/generated again. The old file is
    // kept beside it as .bak (one level of undo: rename it back). Writes an empty item rather than
    // deleting, the same file a level that was never saved has.
    public static void ClearSaved(string code) {

        if (string.IsNullOrEmpty(code)) {
            return;
        }

        string path = GetSavedPath(code);

        try {
            if (System.IO.File.Exists(path)) {
                System.IO.File.Copy(path, path + ".bak", true);
            }
        }
        catch (Exception e) {
            LogUtil.LogError("GameLevelItemSources: could not back up " + path + ": " + e.Message);
        }

        GameLevelItem empty = new GameLevelItem();
        empty.code = code;
        GameLevelItems.Instance.SaveItem(GameLevelItems.Instance.DATA_KEY, code, empty);

        LogUtil.Log("GameLevelItemSources: cleared saved " + code);
    }

    public static string GetShippedResourcePath(string layoutCode) {
        return ContentsConfig.contentRootFolder + "/" + ContentsConfig.contentAppFolder
            + "/version/" + shippedFolder + GameLevelItems.Instance.DATA_KEY + "-" + layoutCode + ".json";
    }

    // A shipped level, or null when none ships for this code or it cannot be read.
    public static GameLevelItem LoadShipped(string layoutCode) {

        if (string.IsNullOrEmpty(layoutCode)) {
            return null;
        }

        string resourcePath = GetShippedResourcePath(layoutCode);
        TextAsset text = Resources.Load<TextAsset>(resourcePath);

        if (text == null || string.IsNullOrEmpty(text.text)) {
            return null;
        }

        try {
            return text.text.FromJson<GameLevelItem>();
        }
        catch (Exception e) {
            LogUtil.LogError("GameLevelItemSources: could not read shipped " + resourcePath + ": " + e.Message);
            return null;
        }
    }

    // A string on the current level row's data block, read without Get (which writes defaults).
    static string GetLevelRowString(string key) {

        GameLevel level = GameLevels.Current;

        if (level == null) {
            return null;
        }

        object rawData;

        if (!level.TryGetValue(BaseDataObjectKeys.data, out rawData) || rawData == null) {
            return null;
        }

        IDictionary<string, object> data = rawData as IDictionary<string, object>;

        object raw;

        if (data == null || !data.TryGetValue(key, out raw) || raw == null) {
            return null;
        }

        return raw.ToString();
    }
}
