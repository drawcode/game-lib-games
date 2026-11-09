#if USE_AGNOSTIC_VEHICLE
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Racing;

using Engine.AgnosticHost;
using Engine.Game.App.BaseApp;

// The race track of a loaded level: the track-ai-node items in the container, in race order, measured in the core
// frame (UnityFrame.ToCore flips z, so lateral and a profile's line_offset keep their sides).
public static class GameRaceTrackBuilder {

    // Null when the container holds fewer than three nodes. lowestY is the lowest level item (the fall line);
    // grid is the start-positions item, or null.
    public static RaceTrack Build(GameObject container, float defaultWidth, out float lowestY, out GameDraggableLevelItem grid) {

        lowestY = 0f;
        grid = null;

        if (container == null) {
            return null;
        }

        Dictionary<GameLevelItemAsset, GameDraggableLevelItem> byAsset = new Dictionary<GameLevelItemAsset, GameDraggableLevelItem>();
        bool any = false;

        foreach (GameDraggableLevelItem item in container.GetComponentsInChildren<GameDraggableLevelItem>(true)) {

            GameLevelItemAsset asset = item.gameLevelItemAsset;

            if (asset == null) {
                continue;
            }

            float y = item.transform.position.y;
            lowestY = any ? Mathf.Min(lowestY, y) : y;
            any = true;

            if (GameRaceNodes.IsNode(asset)) {
                byAsset[asset] = item;
            }
            else if (GameRaceGrid.IsGrid(asset) && grid == null) {
                grid = item;
            }
        }

        List<GameLevelItemAsset> sorted = GameRaceNodes.Sort(byAsset.Keys);

        if (sorted.Count < 3) {
            return null;
        }

        RaceNode[] nodes = new RaceNode[sorted.Count];

        for (int i = 0; i < nodes.Length; i++) {
            GameLevelItemAsset asset = sorted[i];
            IDictionary data = GameRaceNodes.NodeData(asset);
            float width = data != null && data.Contains(GameRaceNodes.keyWidth) ? GameRaceNodes.WidthOf(asset) : defaultWidth;
            nodes[i] = new RaceNode(UnityFrame.ToCore(byAsset[asset].transform.position), width,
                GameRaceNodes.CheckpointOf(asset) ? 1 : 0);
        }

        return new RaceTrack(nodes);
    }
}
#endif
