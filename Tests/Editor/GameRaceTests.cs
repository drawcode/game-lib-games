#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

using Agnostic.Core;
using Agnostic.Racing;

using Engine.AgnosticHost;

namespace Game.Race.Tests {

    // The pure parts of the race session: the rules block, the grid slots, the player's slot pick and the AI profile rows.
    public class GameRaceTests {

        [Test]
        public void Parse_NullGivesDefaults() {

            GameRaceRules r = GameRaceRules.Parse(null);

            Assert.AreEqual(3, r.laps);
            Assert.AreEqual(5, r.aiRiders);
            Assert.AreEqual(-1, r.playerSlot);
            Assert.AreEqual("vehicle-bike-dirt-1", r.vehicleCode);
            Assert.AreEqual(2f, r.respawnDelay);
            Assert.AreEqual(3f, r.bumpAheadFloor);
            Assert.AreEqual(0, r.aiProfiles.Length);
            Assert.AreEqual(0u, r.seed);
        }

        [Test]
        public void Parse_ReadsNumbersAndStringNumbers_ClampsAiRiders() {

            Dictionary<string, object> race = new Dictionary<string, object> {
                { "laps", "2" }, { "ai_riders", 40 }, { "player_slot", 3.0 }, { "default_width", "12.5" },
                { "bump_ahead_floor", 0 }, { "seed", "77" }, { "vehicle_code", "vehicle-bike-x" },
                { "ai_profiles", new List<object> { "ai-rider-2", "ai-rider-4" } }, { "bogus", "ignored" }
            };

            GameRaceRules r = GameRaceRules.Parse(race);

            Assert.AreEqual(2, r.laps);
            Assert.AreEqual(11, r.aiRiders);
            Assert.AreEqual(3, r.playerSlot);
            Assert.AreEqual(12.5f, r.defaultWidth);
            Assert.AreEqual(0f, r.bumpAheadFloor);
            Assert.AreEqual(77u, r.seed);
            Assert.AreEqual("vehicle-bike-x", r.vehicleCode);
            Assert.AreEqual(new[] { "ai-rider-2", "ai-rider-4" }, r.aiProfiles);
            Assert.AreEqual(2, r.ToCore().laps);
        }

        [Test]
        public void Slots_PoleAtItemFacingPlusX() {

            Vector3[] slots = new Vector3[6];
            GameRaceGrid.Slots(new Vector3(-8f, 0f, -30f), Quaternion.Euler(0f, 90f, 0f), GameRaceGridSpec.Defaults(), slots);

            // The first row is centred on z = -30, 2 m behind the item (x = -10); rows follow 5 m apart.
            Assert.AreEqual(-10f, (slots[0].x + slots[1].x) * 0.5f, 1e-4f);
            Assert.AreEqual(-30f, (slots[0].z + slots[1].z) * 0.5f, 1e-4f);
            Assert.AreEqual(-15f, slots[2].x, 1e-4f);
            Assert.AreEqual(-20f, slots[4].x, 1e-4f);
            Assert.AreEqual(3f, Mathf.Abs(slots[0].z - slots[1].z), 1e-4f);
        }

        [Test]
        public void Slots_AddRowsPastTheSpec() {

            GameRaceGridSpec spec = GameRaceGridSpec.Defaults();
            Vector3[] slots = new Vector3[8];
            GameRaceGrid.Slots(Vector3.zero, Quaternion.identity, spec, slots);

            Assert.AreEqual(-(spec.back + 3 * spec.rowGap), slots[6].z, 1e-4f);
        }

        [Test]
        public void Slots_FallbackBehindNodeZero() {

            RaceNode[] nodes = {
                new RaceNode(UnityFrame.ToCore(new Vector3(0f, 0f, -30f)), 12f, 1),
                new RaceNode(UnityFrame.ToCore(new Vector3(40f, 0f, 0f)), 12f, 1),
                new RaceNode(UnityFrame.ToCore(new Vector3(0f, 0f, 30f)), 12f, 1),
                new RaceNode(UnityFrame.ToCore(new Vector3(-40f, 0f, 0f)), 12f, 1)
            };
            RaceTrack track = new RaceTrack(nodes);

            Vector3 origin = UnityFrame.ToUnity(track.Node(0).position);
            Vector3 along = UnityFrame.ToUnity(track.NodeTangent(0));
            along.y = 0f;
            GameRaceGridSpec spec = GameRaceGridSpec.Defaults();
            spec.back = 4f;
            Vector3[] slots = new Vector3[2];
            GameRaceGrid.Slots(origin, Quaternion.LookRotation(along, Vector3.up), spec, slots);

            Vector3 centre = (slots[0] + slots[1]) * 0.5f;
            Assert.AreEqual(4f, Vector3.Distance(centre, origin), 1e-3f);
            Assert.Less(Vector3.Dot(centre - origin, along.normalized), 0f);
        }

        [Test]
        public void PickPlayerSlot_DeterministicPerSeed_InRange() {

            for (uint seed = 1; seed < 50; seed++) {
                int a = GameRaceGrid.PickPlayerSlot(6, seed);
                Assert.AreEqual(a, GameRaceGrid.PickPlayerSlot(6, seed));
                Assert.GreaterOrEqual(a, 0);
                Assert.Less(a, 6);
            }

            Assert.AreEqual(0, GameRaceGrid.PickPlayerSlot(1, 9u));
        }

        [Test]
        public void Profiles_MissingFileGivesFiveBuiltIns() {

            GameRaceAiProfiles.Load("");

            Assert.AreEqual(5, GameRaceAiProfiles.count);
            Assert.AreEqual("ai-rider-1", GameRaceAiProfiles.At(0).code);
            Assert.AreEqual("ai-rider-3", GameRaceAiProfiles.Get("ai-rider-3").code);
            Assert.AreEqual(-0.3f, GameRaceAiProfiles.Get("ai-rider-3").profile.lineOffset, 1e-5f);
            Assert.AreEqual("ai-rider-1", GameRaceAiProfiles.At(5).code);
            Assert.IsNull(GameRaceAiProfiles.Get("nope"));
        }

        [Test]
        public void Profiles_ReadRowsAndIgnoreUnknownKeys() {

            GameRaceAiProfiles.Load("[{\"code\":\"p1\",\"display_name\":\"One\",\"vehicle_code\":\"v1\","
                + "\"data\":{\"line_offset\":0.5,\"noise\":0.2,\"future\":9}}]");

            Assert.AreEqual(1, GameRaceAiProfiles.count);
            GameRaceAiProfiles.Row row = GameRaceAiProfiles.Get("p1");
            Assert.AreEqual("v1", row.vehicleCode);
            Assert.AreEqual(0.5f, row.profile.lineOffset, 1e-5f);
            Assert.AreEqual(0.2f, row.profile.noise, 1e-5f);
            Assert.AreEqual(RaceAiProfile.Defaults().lookaheadMax, row.profile.lookaheadMax);

            GameRaceAiProfiles.Load("");
        }
    }
}
#endif
