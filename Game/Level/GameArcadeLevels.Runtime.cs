using System.Collections.Generic;
using Engine.Game.App;
using Engine.Utility;

// The engine side of GameArcadeLevels: a generated row becomes a GameLevel in GameLevels and the
// current level. The arcade route (P2 in contexts games/shared/context-arcade-vs-missions.md)
// calls GenerateCurrent where the mission route calls GameLevels.Instance.ChangeCurrent.
public static partial class GameArcadeLevels {

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
