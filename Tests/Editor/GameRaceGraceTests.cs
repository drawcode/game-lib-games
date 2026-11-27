#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

using Agnostic.Racing;

namespace Game.Race.Tests {

    // The two opt-in race keys: bump_grace_after_go (no bump bail just after GO) and respawn_pair_stagger (later bailed
    // riders on the same spot wait longer). Both default to 0 = the old behaviour.
    public class GameRaceGraceTests {

        [Test]
        public void Defaults_AreOff() {

            GameRaceRules r = GameRaceRules.Parse(null);

            Assert.AreEqual(0f, r.bumpGraceAfterGo);
            Assert.AreEqual(0f, r.respawnPairStagger);
            Assert.AreEqual(0f, GameRaceRules.Parse(new Dictionary<string, object> { { "laps", 2 } }).bumpGraceAfterGo);
        }

        [Test]
        public void Parse_ReadsBothKeys_ClampsNegative() {

            GameRaceRules r = GameRaceRules.Parse(new Dictionary<string, object> {
                { "bump_grace_after_go", 1.5 }, { "respawn_pair_stagger", "0.75" }
            });

            Assert.AreEqual(1.5f, r.bumpGraceAfterGo);
            Assert.AreEqual(0.75f, r.respawnPairStagger);

            r = GameRaceRules.Parse(new Dictionary<string, object> { { "bump_grace_after_go", -2 }, { "respawn_pair_stagger", -1 } });

            Assert.AreEqual(0f, r.bumpGraceAfterGo);
            Assert.AreEqual(0f, r.respawnPairStagger);
        }

        [Test]
        public void BumpGrace_OffWhenZero() {

            Assert.IsFalse(GameRace.InBumpGrace(0f, 0f));
            Assert.IsFalse(GameRace.InBumpGrace(0.7f, 0f));
        }

        [Test]
        public void BumpGrace_SuppressesBeforeGraceNotAfter() {

            Assert.IsTrue(GameRace.InBumpGrace(0.7f, 1.5f));
            Assert.IsTrue(GameRace.InBumpGrace(1.49f, 1.5f));
            Assert.IsFalse(GameRace.InBumpGrace(1.5f, 1.5f));
            Assert.IsFalse(GameRace.InBumpGrace(3f, 1.5f));
        }

        [Test]
        public void Stagger_ZeroKeepsTheBaseWait() {

            int baseSteps = GameRace.StaggeredRespawnSteps(2f, 0f, 0);

            Assert.AreEqual(baseSteps, GameRace.StaggeredRespawnSteps(2f, 0f, 1));
            Assert.AreEqual(baseSteps, GameRace.StaggeredRespawnSteps(2f, 0f, 3));
            Assert.AreEqual(Mathf.RoundToInt(2f * RaceSession.stepsPerSecond), baseSteps);
        }

        [Test]
        public void Stagger_OrdersLaterRespawnsLater() {

            int first = GameRace.StaggeredRespawnSteps(2f, 0.75f, 0);
            int second = GameRace.StaggeredRespawnSteps(2f, 0.75f, 1);
            int third = GameRace.StaggeredRespawnSteps(2f, 0.75f, 2);

            Assert.AreEqual(Mathf.RoundToInt(2f * RaceSession.stepsPerSecond), first);
            Assert.Less(first, second);
            Assert.Less(second, third);
            Assert.AreEqual(Mathf.RoundToInt(2.75f * RaceSession.stepsPerSecond), second);
            Assert.AreEqual(Mathf.RoundToInt(3.5f * RaceSession.stepsPerSecond), third);
        }
    }
}
#endif
