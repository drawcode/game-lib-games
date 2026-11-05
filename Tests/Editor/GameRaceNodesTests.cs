using System.Collections.Generic;
using NUnit.Framework;
using Engine.Game.App.BaseApp;

namespace Game.Race.Tests {

    // track-ai-node data (GameRaceNodes): defaults, auto order, sort, and that a node's data and an
    // unknown key survive the level-item JSON round trip the editor save and the shipped loader use.
    public class GameRaceNodesTests {

        static GameLevelItemAsset Node(string uuid, int order = -1, double width = -1, string extraKey = null) {

            GameLevelItemAsset asset = new GameLevelItemAsset();
            asset.code = GameRaceNodes.code;
            asset.uuid = uuid;

            Dictionary<string, object> node = new Dictionary<string, object>();

            if (order >= 0) {
                node[GameRaceNodes.keyOrder] = order;
            }

            if (width > 0) {
                node[GameRaceNodes.keyWidth] = width;
            }

            if (extraKey != null) {
                node[extraKey] = "kept";
            }

            if (node.Count > 0) {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data[GameRaceNodes.keyRaceNode] = node;
                asset.data = data;
            }

            return asset;
        }

        [Test]
        public void MissingFields_UseDefaults() {

            GameLevelItemAsset bare = Node("a");

            Assert.AreEqual(-1, GameRaceNodes.OrderOf(bare));
            Assert.AreEqual(GameRaceNodes.defaultWidth, GameRaceNodes.WidthOf(bare));
            Assert.IsTrue(GameRaceNodes.CheckpointOf(bare));
        }

        [Test]
        public void EnsureNodeData_GivesNextOrder_KeepsUnknownKeys() {

            List<GameLevelItemAsset> existing = new List<GameLevelItemAsset> { Node("a", 0), Node("b", 3), Node("c") };
            GameLevelItemAsset added = Node("d", -1, -1, "future_key");

            GameRaceNodes.EnsureNodeData(added, existing);

            Assert.AreEqual(4, GameRaceNodes.OrderOf(added));
            Assert.AreEqual(GameRaceNodes.defaultWidth, GameRaceNodes.WidthOf(added));
            Assert.AreEqual("kept", GameRaceNodes.NodeData(added)["future_key"]);

            // An ordered node keeps its order and width.
            GameLevelItemAsset ordered = Node("e", 7, 14);
            GameRaceNodes.EnsureNodeData(ordered, existing);
            Assert.AreEqual(7, GameRaceNodes.OrderOf(ordered));
            Assert.AreEqual(14f, GameRaceNodes.WidthOf(ordered));
        }

        [Test]
        public void Sort_OrderedFirst_ThenUnorderedByUuid() {

            List<GameLevelItemAsset> sorted = GameRaceNodes.Sort(new List<GameLevelItemAsset> {
                Node("z"), Node("b", 2), Node("y"), Node("a", 0), new GameLevelItemAsset() });

            List<string> uuids = sorted.ConvertAll(n => n.uuid);

            CollectionAssert.AreEqual(new[] { "a", "b", "y", "z" }, uuids);
        }

        [Test]
        public void NodeData_SurvivesJsonRoundTrip() {

            GameLevelItem level = new GameLevelItem();
            level.code = "race-test-1";
            level.level_items = new List<GameLevelItemAsset> { Node("a", 0, 12, "future_key"), Node("b") };

            GameLevelItem loaded = level.ToJson().FromJson<GameLevelItem>();
            List<GameLevelItemAsset> nodes = GameRaceNodes.Sort(loaded.level_items);

            Assert.AreEqual(2, nodes.Count);
            Assert.AreEqual(0, GameRaceNodes.OrderOf(nodes[0]));
            Assert.AreEqual(12f, GameRaceNodes.WidthOf(nodes[0]));
            Assert.AreEqual("kept", GameRaceNodes.NodeData(nodes[0])["future_key"].ToString());
            Assert.AreEqual(-1, GameRaceNodes.OrderOf(nodes[1]));
        }
    }
}
