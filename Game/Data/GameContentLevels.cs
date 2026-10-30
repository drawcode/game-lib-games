using System;
using System.Collections.Generic;

using Engine.Game.App;
using Engine.Game.Data;

// Content gating data (iter 35): `game-content-level-data`, one row per bot level - the XP that
// reaches it - plus one `settings` row (teaser_count). GameContentGate reads it; see there.
//
//   { "type": "level", "code": "content-level-1", "level": 1, "xp": 1000, "active": true }
//   { "type": "settings", "code": "content-level-settings", "teaser_count": 2, "active": true }
//
// A game without the file loads no rows: every bot is level 0 and nothing is gated.
public class GameContentLevels : DataObjects<GameContentLevel> {

    private static volatile GameContentLevels instance;
    private static System.Object syncRoot = new System.Object();
    private string DATA_KEY = "game-content-level-data";

    public static GameContentLevels Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new GameContentLevels(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public GameContentLevels() {
        Reset();
    }

    public GameContentLevels(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }
}

public class GameContentLevel : GameDataObject {

    public const string typeLevel = "level";
    public const string typeSettings = "settings";
    public const string teaserCountKey = "teaser_count";

    // Numbers come back from JSON as long or double, so every read converts, and a read never
    // writes a default back into the row.

    public virtual int level {
        get {
            return (int)Number(BaseDataObjectKeys.level, 0);
        }
        set {
            Set<int>(BaseDataObjectKeys.level, value);
        }
    }

    // The XP that reaches this level.
    public virtual double xp {
        get {
            return Number(BaseDataObjectKeys.xp, 0);
        }
        set {
            Set<double>(BaseDataObjectKeys.xp, value);
        }
    }

    public virtual int teaser_count {
        get {
            return (int)Number(teaserCountKey, -1);
        }
        set {
            Set<int>(teaserCountKey, value);
        }
    }

    public double Number(string key, double fallback) {

        object val;

        if (!TryGetValue(key, out val) || val == null) {
            return fallback;
        }

        try {
            return Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (Exception) {
            return fallback;
        }
    }
}
