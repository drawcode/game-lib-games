using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Game.App.BaseApp;

// Race track nodes (Action Bikes R2, plan-action-bikes-race-core step 6). A "track-ai-node" level
// item marks the racing line: the nodes in order make the spline the AI rides, and the same nodes
// give the checkpoints and the race distance. Order 0 is the start/finish line.
//
// The node's fields live in the item's data block, by keys (unknown keys are kept):
//   "data": {"race_node": {"order": 0, "width": 10, "checkpoint": true}}
// A missing order sorts after the ordered nodes (by uuid), a missing width uses defaultWidth and a
// missing checkpoint means true. Nothing here runs unless a game uses the code: Action Bots has no
// track-ai-node items and does not set GameDraggableEditor.onLevelItemCreating.
public static class GameRaceNodes {

    public static string code = "track-ai-node";

    public const string keyRaceNode = "race_node";
    public const string keyOrder = "order";
    public const string keyWidth = "width";
    public const string keyCheckpoint = "checkpoint";

    public static float defaultWidth = 10f;

    public static bool IsNode(GameLevelItemAsset asset) {
        return asset != null && asset.code == code;
    }

    // The race_node block, or null. Loaded JSON can hand back a generic or a non-generic dictionary.
    public static IDictionary NodeData(GameLevelItemAsset asset) {

        if (asset == null) {
            return null;
        }

        Dictionary<string, object> data = asset.data;
        object raw;

        if (data == null || !data.TryGetValue(keyRaceNode, out raw)) {
            return null;
        }

        return raw as IDictionary;
    }

    internal static bool TryNumber(IDictionary node, string key, out double value) {

        value = 0;

        if (node == null || !node.Contains(key) || node[key] == null) {
            return false;
        }

        object raw = node[key];

        if (raw is IConvertible && !(raw is string) && !(raw is bool)) {
            value = Convert.ToDouble(raw, System.Globalization.CultureInfo.InvariantCulture);
            return true;
        }

        return double.TryParse(raw.ToString(), System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out value);
    }

    // -1 when the node has no order.
    public static int OrderOf(GameLevelItemAsset asset) {
        double value;
        return TryNumber(NodeData(asset), keyOrder, out value) ? (int)Math.Round(value) : -1;
    }

    public static float WidthOf(GameLevelItemAsset asset) {
        double value;
        return TryNumber(NodeData(asset), keyWidth, out value) && value > 0 ? (float)value : defaultWidth;
    }

    public static bool CheckpointOf(GameLevelItemAsset asset) {

        IDictionary node = NodeData(asset);

        if (node == null || !node.Contains(keyCheckpoint) || node[keyCheckpoint] == null) {
            return true;
        }

        object raw = node[keyCheckpoint];

        if (raw is bool) {
            return (bool)raw;
        }

        bool parsed;
        return !bool.TryParse(raw.ToString(), out parsed) || parsed;
    }

    // Gives a node without an order the next one after `existing`, and a width; keeps every other key.
    public static void EnsureNodeData(GameLevelItemAsset asset, IEnumerable<GameLevelItemAsset> existing) {

        if (!IsNode(asset)) {
            return;
        }

        Dictionary<string, object> data = asset.data ?? new Dictionary<string, object>();
        IDictionary node = NodeData(asset);
        Dictionary<string, object> filled = new Dictionary<string, object>();

        if (node != null) {
            foreach (DictionaryEntry entry in node) {
                filled[entry.Key.ToString()] = entry.Value;
            }
        }

        if (OrderOf(asset) < 0) {

            int next = 0;

            if (existing != null) {
                foreach (GameLevelItemAsset other in existing) {
                    if (other != asset && IsNode(other)) {
                        next = Math.Max(next, OrderOf(other) + 1);
                    }
                }
            }

            filled[keyOrder] = next;
        }

        if (!filled.ContainsKey(keyWidth)) {
            filled[keyWidth] = (double)defaultWidth;
        }

        if (!filled.ContainsKey(keyCheckpoint)) {
            filled[keyCheckpoint] = true;
        }

        data[keyRaceNode] = filled;
        asset.data = data;
    }

    // The nodes in race order: by order, then the unordered ones by uuid.
    public static List<GameLevelItemAsset> Sort(IEnumerable<GameLevelItemAsset> items) {

        List<GameLevelItemAsset> nodes = new List<GameLevelItemAsset>();

        if (items != null) {
            foreach (GameLevelItemAsset item in items) {
                if (IsNode(item)) {
                    nodes.Add(item);
                }
            }
        }

        nodes.Sort((a, b) => {
            int oa = OrderOf(a);
            int ob = OrderOf(b);

            if (oa >= 0 && ob >= 0 && oa != ob) {
                return oa.CompareTo(ob);
            }

            if ((oa >= 0) != (ob >= 0)) {
                return oa >= 0 ? -1 : 1;
            }

            return string.CompareOrdinal(a.uuid, b.uuid);
        });

        return nodes;
    }

    // The node items placed in a level container (saved and not yet saved), in race order.
    public static List<GameLevelItemAsset> LevelNodes(GameObject container) {

        List<GameLevelItemAsset> items = new List<GameLevelItemAsset>();

        if (container != null) {
            foreach (GameDraggableLevelItem item in container.GetComponentsInChildren<GameDraggableLevelItem>(true)) {
                if (item.gameLevelItemAsset != null) {
                    items.Add(item.gameLevelItemAsset);
                }
            }
        }

        return Sort(items);
    }

    // For GameDraggableEditor.onLevelItemCreating: a node placed in the editor gets the next order.
    public static void OnLevelItemCreating(GameLevelItemAsset asset) {
        if (IsNode(asset)) {
            EnsureNodeData(asset, LevelNodes(GameDraggableEditor.levelItemsContainerObject));
        }
    }
}
