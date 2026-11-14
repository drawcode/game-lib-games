#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;

namespace Game.Race.Tests {

    // D22 racer looks: the palette rows parse, no data = off, Apply is a sharedMaterial swap that makes no instance,
    // and a seed gives the same assignment every time.
    public class GameRacerLookTests {

        private const string json = "["
            + "{\"code\":\"rider-skin-01\",\"type\":\"racer-rider\",\"active\":true,\"sort_order\":1,\"display_name\":\"Racer 1\",\"data\":{\"family\":\"chibi-racer\",\"material\":\"racer_mat 1\",\"key\":\"#D8D8D9\",\"accent\":\"#CA0808\"}},"
            + "{\"code\":\"rider-skin-03\",\"type\":\"racer-rider\",\"active\":true,\"sort_order\":3,\"data\":{\"family\":\"chibi-racer\",\"material\":\"racer_mat 3\",\"key\":\"#792954\",\"accent\":\"#BAF106\"}},"
            + "{\"code\":\"rider-skin-06\",\"type\":\"racer-rider\",\"active\":true,\"sort_order\":6,\"data\":{\"family\":\"chibi-racer\",\"material\":\"racer_mat 6\",\"key\":\"#275787\",\"accent\":\"#F99615\"}},"
            + "{\"code\":\"rider-skin-07\",\"type\":\"racer-rider\",\"active\":true,\"sort_order\":7,\"data\":{\"family\":\"chibi-racer\",\"material\":\"racer_mat 7\",\"key\":\"#368A88\",\"accent\":\"#FFD900\"}},"
            + "{\"code\":\"rider-skin-off\",\"type\":\"racer-rider\",\"active\":false,\"sort_order\":9,\"data\":{\"family\":\"chibi-racer\",\"material\":\"racer_mat 9\",\"key\":\"#FF0000\"}},"
            + "{\"code\":\"rider-skin-bad\",\"type\":\"racer-rider\",\"active\":true,\"sort_order\":10,\"data\":{\"family\":\"chibi-racer\",\"material\":\"racer_mat 10\",\"key\":\"nope\"}},"
            + "{\"code\":\"bike-dirt-skin-01\",\"type\":\"racer-vehicle\",\"active\":true,\"sort_order\":1,\"data\":{\"family\":\"chibi-dirtbike\",\"vehicle_codes\":[\"vehicle-bike-dirt-1\"],\"material\":\"dirtbike_mat 1\",\"key\":\"#C91116\",\"accent\":\"#2B2B2C\"}},"
            + "{\"code\":\"bike-dirt-skin-03\",\"type\":\"racer-vehicle\",\"active\":true,\"sort_order\":3,\"data\":{\"family\":\"chibi-dirtbike\",\"vehicle_codes\":[\"vehicle-bike-dirt-1\"],\"material\":\"dirtbike_mat 3\",\"key\":\"#952D6A\",\"accent\":\"#14B371\"}},"
            + "{\"code\":\"bike-dirt-skin-06\",\"type\":\"racer-vehicle\",\"active\":true,\"sort_order\":6,\"data\":{\"family\":\"chibi-dirtbike\",\"vehicle_codes\":[\"vehicle-bike-dirt-1\"],\"material\":\"dirtbike_mat 6\",\"key\":\"#204E79\",\"accent\":\"#F68B12\"}},"
            + "{\"code\":\"racer-palette-rules\",\"type\":\"racer-palette-rules\",\"data\":{\"min_rival\":0.12,\"min_player\":0.18,\"max_neutral\":1,\"neutral_chroma\":0.04,\"salt\":3225640685}}"
            + "]";

        private readonly List<Object> made = new List<Object>();

        [SetUp]
        public void SetUp() {
            GameRacePalettes.Load(json);
        }

        [TearDown]
        public void TearDown() {
            foreach (Object o in made) {
                if (o != null) {
                    Object.DestroyImmediate(o);
                }
            }

            made.Clear();
            GameRacerSkinSet.Use(null);
            GameRacePalettes.Load("");
            GameRacerLook.Reset();
        }

        private Material Mat(string name) {
            Shader shader = Shader.Find("Sprites/Default");

            if (shader == null) {
                shader = Shader.Find("Hidden/InternalErrorShader");
            }

            Material m = new Material(shader);
            m.name = name;
            made.Add(m);
            return m;
        }

        private GameRacerSkinSet SkinSet() {
            GameObject go = new GameObject("skins");
            made.Add(go);
            GameRacerSkinSet set = go.AddComponent<GameRacerSkinSet>();
            List<Material> list = new List<Material>();

            for (int i = 1; i <= 10; i++) {
                list.Add(Mat("racer_mat " + i));
                list.Add(Mat("dirtbike_mat " + i));
            }

            set.materials = list.ToArray();
            return set;
        }

        private GameObject Bike(GameRacerSkinSet set, out Renderer body, out Renderer seat) {
            GameObject root = new GameObject("bike");
            made.Add(root);
            body = AddRenderer(root, "body", set.Find("dirtbike_mat 1"));
            seat = AddRenderer(root, "seat", set.Find("racer_mat 1"));
            return root;
        }

        private Renderer AddRenderer(GameObject parent, string name, Material m) {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            MeshRenderer r = go.AddComponent<MeshRenderer>();
            r.sharedMaterial = m;
            return r;
        }

        [Test]
        public void Parse_ReadsRowsRulesAndSkipsBadOnes() {
            Assert.IsTrue(GameRacePalettes.enabled);
            Assert.AreEqual(7, GameRacePalettes.count);       // inactive and bad-colour rows are dropped, the rules row is not a skin
            Assert.AreEqual(4, GameRacePalettes.Riders().Count);
            Assert.AreEqual("rider-skin-01", GameRacePalettes.Riders()[0].code);
            Assert.AreEqual("racer_mat 7", GameRacePalettes.Riders()[3].material);
            Assert.AreEqual(3, GameRacePalettes.ForVehicle("vehicle-bike-dirt-1").Count);
            Assert.AreEqual(0, GameRacePalettes.ForVehicle("vehicle-nope").Count);

            GameRacePalettes.Row row = GameRacePalettes.Get("bike-dirt-skin-01");
            Assert.AreEqual("chibi-dirtbike", row.family);
            Assert.AreEqual(0xC9 / 255f, row.key.r, 1e-4f);

            Assert.AreEqual(0.12f, GameRacePalettes.Rules.minRival, 1e-5f);
            Assert.AreEqual(0.18f, GameRacePalettes.Rules.minPlayer, 1e-5f);
            Assert.AreEqual(3225640685u, GameRacePalettes.Rules.salt);
        }

        [Test]
        public void MissingOrBrokenData_IsOffAndChangesNothing() {
            GameRacePalettes.Load("");
            Assert.IsFalse(GameRacePalettes.enabled);

            GameRacerSkinSet set = SkinSet();
            GameRacerSkinSet.Use(set);
            Renderer body, seat;
            GameObject root = Bike(set, out body, out seat);
            Material before = body.sharedMaterial;

            Assert.IsFalse(GameRacerLook.BeginRace(new[] { root }, new GameObjectMountVehicle[1], new[] { "vehicle-bike-dirt-1" }, 0, 7u));
            Assert.AreSame(before, body.sharedMaterial);

            GameRacePalettes.Load("{ not json");
            Assert.IsFalse(GameRacePalettes.enabled);
            GameRacePalettes.Load("[]");
            Assert.IsFalse(GameRacePalettes.enabled);

            // data but no skin set: also off
            GameRacePalettes.Load(json);
            GameRacerSkinSet.Use(null);
            Assert.IsFalse(GameRacerLook.available);
        }

        [Test]
        public void Apply_SwapsSharedMaterialWithoutInstances() {
            GameRacerSkinSet set = SkinSet();
            Renderer body, seat;
            GameObject root = Bike(set, out body, out seat);
            Material bike = set.Find("dirtbike_mat 6");
            Material rider = set.Find("racer_mat 7");

            GameRacerLook.Apply(root, null, bike, rider);

            Assert.AreSame(bike, body.sharedMaterial);
            Assert.AreSame(rider, seat.sharedMaterial);

            foreach (Renderer r in root.GetComponentsInChildren<Renderer>()) {
                foreach (Material m in r.sharedMaterials) {
                    StringAssert.DoesNotContain("(Instance)", m.name);
                }
            }
        }

        [Test]
        public void FamilyOf_StripsTheSkinNumber() {
            Assert.AreEqual("racer_mat", GameRacerLook.FamilyOf("racer_mat 7"));
            Assert.AreEqual("dirtbike_mat", GameRacerLook.FamilyOf("dirtbike_mat 10 (Instance)"));
            Assert.AreEqual("", GameRacerLook.FamilyOf(null));
        }

        [Test]
        public void BeginRace_AppliesEveryRacerIncludingTheDefault() {
            GameRacerSkinSet set = SkinSet();
            GameRacerSkinSet.Use(set);
            int n = 4;
            GameObject[] roots = new GameObject[n];
            Renderer[] bodies = new Renderer[n];
            GameObjectMountVehicle[] mounts = new GameObjectMountVehicle[n];
            string[] codes = new string[n];

            for (int i = 0; i < n; i++) {
                Renderer seat;
                roots[i] = Bike(set, out bodies[i], out seat);
                codes[i] = "vehicle-bike-dirt-1";
            }

            Assert.IsTrue(GameRacerLook.BeginRace(roots, mounts, codes, 1, 11u, "", null));

            // the player keeps the default (first row); every body shows the skin its code names
            Assert.AreEqual("bike-dirt-skin-01", GameRacerLook.bikeCodes[1]);
            Assert.AreEqual("rider-skin-01", GameRacerLook.riderCodes[1]);

            HashSet<string> seen = new HashSet<string>();

            for (int i = 0; i < n; i++) {
                Assert.AreEqual(GameRacePalettes.Get(GameRacerLook.bikeCodes[i]).material, bodies[i].sharedMaterial.name);
                Assert.IsTrue(seen.Add(GameRacerLook.bikeCodes[i]), "3 skins for 4 racers repeat only after all are used");

                if (i == 2) {
                    seen.Clear();
                }
            }
        }

        [Test]
        public void Seed_GivesAStableAssignment() {
            List<GameRacePalettes.Row> pool = GameRacePalettes.Riders();
            int[] a = GameRacerLook.Assign(pool, 4, -1, 42u, 1u);
            int[] b = GameRacerLook.Assign(pool, 4, -1, 42u, 1u);
            CollectionAssert.AreEqual(a, b);

            HashSet<string> orders = new HashSet<string>();

            for (uint seed = 1; seed <= 8; seed++) {
                orders.Add(string.Join(",", GameRacerLook.Assign(pool, 4, -1, seed, 1u)));
            }

            Assert.GreaterOrEqual(orders.Count, 2);

            // a reserved player keeps their pick
            Assert.AreEqual(2, GameRacerLook.Assign(pool, 4, 2, 5u, 1u)[0]);
        }

        [Test]
        public void PickIndex_DefaultRandomAndCode() {
            List<GameRacePalettes.Row> pool = GameRacePalettes.Riders();
            Assert.AreEqual(0, GameRacerLook.PickIndex(pool, ""));
            Assert.AreEqual(-1, GameRacerLook.PickIndex(pool, GameRacerLookKeys.random));
            Assert.AreEqual(2, GameRacerLook.PickIndex(pool, "rider-skin-06"));
            Assert.AreEqual(0, GameRacerLook.PickIndex(pool, "no-such-code"));
        }
    }
}
#endif
