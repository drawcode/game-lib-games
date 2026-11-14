#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using UnityEngine;

using Agnostic.Core;

using Engine.Game.App;

// Seeded, distinct racer skins (D22 stage 1). Every racer's rider and bike gets one vendor skin by a sharedMaterial
// swap: Simple Lit stays SRP-Batcher compatible, no material instance is made (never renderer.materials), and the
// seat copy in GameObjectMountVehicle still works. Off without palette data or a skin set, so Action Bots is untouched.
//
// The default look of a family is its first row in sort_order (today's white and red). A race always applies a look,
// the default included, because pooled sprites keep the last swap.
//
// Per race: AssignRace once (the arrays here), Rig.Capture once per racer at spawn, Rig.Apply as often as the look
// changes. Nothing allocates per frame.
public static class GameRacerLook {

    // What a renderer list needs to swap: the renderers (and which material slots) that carry a skin of the family,
    // cached at spawn.
    public class Rig {

        public Renderer[] renderers = new Renderer[0];
        public Material[][] slots = new Material[0][];     // the renderer's sharedMaterials, edited in place
        public int[][] matched = new int[0][];             // per renderer, the slot indexes that carry the family skin
        public Renderer seat;                              // the mount's riderSeat (rider rigs only)

        public int count { get { return renderers.Length; } }

        // Renderers under root (and the seat) with a slot whose material is of the family (name minus the trailing number).
        public static Rig Capture(GameObject root, Renderer seat, string materialFamily) {

            Rig rig = new Rig();
            rig.seat = seat;

            if (root == null || string.IsNullOrEmpty(materialFamily)) {
                return rig;
            }

            List<Renderer> found = new List<Renderer>();
            List<Material[]> foundSlots = new List<Material[]>();
            List<int[]> foundMatched = new List<int[]>();

            foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true)) {

                if (r == seat) {
                    continue;
                }

                Material[] mats = r.sharedMaterials;
                List<int> idx = null;

                for (int i = 0; i < mats.Length; i++) {
                    if (mats[i] != null && FamilyOf(mats[i].name) == materialFamily) {
                        if (idx == null) {
                            idx = new List<int>();
                        }

                        idx.Add(i);
                    }
                }

                if (idx != null) {
                    found.Add(r);
                    foundSlots.Add(mats);
                    foundMatched.Add(idx.ToArray());
                }
            }

            rig.renderers = found.ToArray();
            rig.slots = foundSlots.ToArray();
            rig.matched = foundMatched.ToArray();
            return rig;
        }

        // Swaps the family slots to target (null = leave) and the seat to it too. sharedMaterial(s) only.
        public void Apply(Material target) {

            if (target == null) {
                return;
            }

            for (int i = 0; i < renderers.Length; i++) {
                if (renderers[i] == null) {
                    continue;
                }

                Material[] mats = slots[i];

                if (mats.Length == 1) {
                    renderers[i].sharedMaterial = target;
                    mats[0] = target;
                    continue;
                }

                int[] idx = matched[i];

                for (int k = 0; k < idx.Length; k++) {
                    mats[idx[k]] = target;
                }

                renderers[i].sharedMaterials = mats;
            }

            if (seat != null) {
                seat.sharedMaterial = target;
            }
        }
    }

    // The per-race result, indexed by racer: the chosen row code ("" = untouched). For capture sheets and the HUD.
    public static string[] riderCodes = new string[0];
    public static string[] bikeCodes = new string[0];

    private static Rig[] bikeRigs = new Rig[0];
    private static Rig[] riderRigs = new Rig[0];

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        Reset();
    }

    public static void Reset() {
        riderCodes = new string[0];
        bikeCodes = new string[0];
        bikeRigs = new Rig[0];
        riderRigs = new Rig[0];
    }

    // True when palette data and a skin set are both there.
    public static bool available {
        get { return GameRacePalettes.enabled && GameRacerSkinSet.Current != null; }
    }

    // "racer_mat 7" and "racer_mat 7 (Instance)" -> "racer_mat".
    public static string FamilyOf(string materialName) {

        if (string.IsNullOrEmpty(materialName)) {
            return "";
        }

        int end = materialName.Length;
        const string instance = " (Instance)";

        if (materialName.EndsWith(instance)) {
            end -= instance.Length;
        }

        while (end > 0 && (char.IsDigit(materialName[end - 1]) || materialName[end - 1] == ' ')) {
            end--;
        }

        return materialName.Substring(0, end);
    }

    // The row to start from: the picked code, "random" = -1, "" = the default (index 0), an unknown code = default.
    public static int PickIndex(List<GameRacePalettes.Row> pool, string pick) {

        if (pool == null || pool.Count == 0) {
            return -1;
        }

        if (pick == GameRacerLookKeys.random) {
            return -1;
        }

        for (int i = 0; i < pool.Count && !string.IsNullOrEmpty(pick); i++) {
            if (pool[i].code == pick) {
                return i;
            }
        }

        return 0;
    }

    // The rows of the pool whose material the skin set holds, so every index maps to a Material.
    public static List<GameRacePalettes.Row> Usable(List<GameRacePalettes.Row> pool, GameRacerSkinSet set) {

        List<GameRacePalettes.Row> list = new List<GameRacePalettes.Row>(pool.Count);

        for (int i = 0; i < pool.Count; i++) {
            if (set != null && set.Find(pool[i].material) != null) {
                list.Add(pool[i]);
            }
        }

        return list;
    }

    // One seeded, distinct run over a pool. members[0] is the "player" of the run (reserved pick or the first of the
    // shuffle); result[j] is the pool row of members[j]. salt differs per run so riders and bikes are independent.
    // Returns null when the pool is empty.
    public static int[] Assign(List<GameRacePalettes.Row> pool, int racers, int reserved, uint seed, uint saltMix) {

        if (pool == null || pool.Count == 0 || racers <= 0) {
            return null;
        }

        int n = pool.Count;
        OkLab[] keys = new OkLab[n];
        bool[] neutral = new bool[n];

        for (int i = 0; i < n; i++) {
            keys[i] = pool[i].keyLab;
        }

        PaletteRules rules = GameRacePalettes.Rules;
        PaletteAssign.FillNeutral(keys, neutral, rules.neutralChroma);
        int[] result = new int[racers];

        if (PaletteAssign.Pick(keys, neutral, racers, reserved, seed ^ saltMix, rules, result, new int[2 * n]) < 0) {
            return null;
        }

        return result;
    }

    // Assigns and applies every racer's look for a race. roots/mounts/vehicleCodes are indexed by racer. The player
    // takes the profile's picks. Off (a no-op) without data or a skin set. Returns whether anything was applied.
    public static bool BeginRace(GameObject[] roots, GameObjectMountVehicle[] mounts, string[] vehicleCodes, int playerIndex, uint seed) {

        Reset();

        if (!available || roots == null) {
            return false;
        }

        return BeginRace(roots, mounts, vehicleCodes, playerIndex, seed, ProfileValue(GameRacerLookKeys.riderSkin), null);
    }

    // The same with explicit picks: riderPick for the player's rider and bikePicks (family -> pick, may be null) for bikes.
    public static bool BeginRace(GameObject[] roots, GameObjectMountVehicle[] mounts, string[] vehicleCodes, int playerIndex, uint seed,
        string riderPick, Dictionary<string, string> bikePicks) {

        Reset();
        GameRacerSkinSet set = GameRacerSkinSet.Current;

        if (!GameRacePalettes.enabled || set == null || roots == null) {
            return false;
        }

        int n = roots.Length;
        riderCodes = new string[n];
        bikeCodes = new string[n];
        riderRigs = new Rig[n];
        bikeRigs = new Rig[n];
        bool any = false;

        for (int i = 0; i < n; i++) {
            riderCodes[i] = "";
            bikeCodes[i] = "";
        }

        // Riders: one run over every racer; slot 0 is the player, the rivals follow in grid order.
        List<GameRacePalettes.Row> riderPool = Usable(GameRacePalettes.Riders(), set);

        if (riderPool.Count > 0) {
            int[] slot = Slots(n, playerIndex, null, null);
            int[] picked = Assign(riderPool, n, PickIndex(riderPool, riderPick), seed, 0x52494445u);

            if (picked != null) {
                string family = FamilyOf(riderPool[0].material);

                for (int j = 0; j < n; j++) {
                    int i = slot[j];

                    if (roots[i] == null) {
                        continue;
                    }

                    Material target = set.Find(riderPool[picked[j]].material);
                    Renderer seat = mounts != null && mounts[i] != null ? mounts[i].riderSeat : null;
                    riderRigs[i] = Rig.Capture(roots[i], seat, family);
                    riderRigs[i].Apply(target);
                    riderCodes[i] = riderPool[picked[j]].code;
                    any = true;
                }
            }
        }

        // Bikes: one run per vehicle code (family), over the racers that ride it.
        if (vehicleCodes != null) {
            for (int i = 0; i < n; i++) {
                if (bikeCodes[i].Length > 0 || roots[i] == null || string.IsNullOrEmpty(vehicleCodes[i])) {
                    continue;
                }

                string code = vehicleCodes[i];
                List<GameRacePalettes.Row> pool = Usable(GameRacePalettes.ForVehicle(code), set);

                if (pool.Count == 0) {
                    continue;
                }

                int[] slot = Slots(n, playerIndex, vehicleCodes, code);
                string pick = "";

                if (bikePicks != null) {
                    bikePicks.TryGetValue(pool[0].family, out pick);
                }
                else {
                    pick = ProfileValue(GameRacerLookKeys.VehicleSkin(pool[0].family));
                }

                bool hasPlayer = slot.Length > 0 && slot[0] == playerIndex;
                int[] picked = Assign(pool, slot.Length, hasPlayer ? PickIndex(pool, pick) : -1, seed, 0x42494B45u);

                if (picked == null) {
                    continue;
                }

                string family = FamilyOf(pool[0].material);

                for (int j = 0; j < slot.Length; j++) {
                    int r = slot[j];
                    bikeRigs[r] = Rig.Capture(roots[r], null, family);
                    bikeRigs[r].Apply(set.Find(pool[picked[j]].material));
                    bikeCodes[r] = pool[picked[j]].code;
                    any = true;
                }
            }
        }

        return any;
    }

    // The racer indexes of a run, the player first when it is in the run, then the rest in grid order. vehicleCodes and
    // code select the racers of one vehicle code (both null = everyone).
    public static int[] Slots(int n, int playerIndex, string[] vehicleCodes, string code) {

        int count = 0;

        for (int i = 0; i < n; i++) {
            if (vehicleCodes == null || vehicleCodes[i] == code) {
                count++;
            }
        }

        int[] slot = new int[count];
        int at = 0;

        if (playerIndex >= 0 && playerIndex < n && (vehicleCodes == null || vehicleCodes[playerIndex] == code)) {
            slot[at++] = playerIndex;
        }

        for (int i = 0; i < n; i++) {
            if (i != playerIndex && (vehicleCodes == null || vehicleCodes[i] == code)) {
                slot[at++] = i;
            }
        }

        return slot;
    }

    // Applies one rider and bike material to a root (null = leave): captures, then swaps. For one-off use (tests, a
    // preview); the seat takes the rider material.
    public static void Apply(GameObject root, GameObjectMountVehicle mount, Material bikeMat, Material riderMat) {

        if (root == null) {
            return;
        }

        Renderer seat = mount != null ? mount.riderSeat : null;

        if (bikeMat != null) {
            Rig.Capture(root, null, FamilyOf(bikeMat.name)).Apply(bikeMat);
        }

        if (riderMat != null) {
            Rig.Capture(root, seat, FamilyOf(riderMat.name)).Apply(riderMat);
        }
    }

    // The player's own look from the profile keys, for the on-foot racer and the vehicle they take (Bikes
    // GameController, the picker). "random" is seeded by `seed` (pass the race seed, or any number for a preview).
    // Off without data or skin set. riderRoot or vehicleRoot may be null.
    public static void ApplyPlayer(GameObject riderRoot, GameObject vehicleRoot, string vehicleCode, uint seed) {

        GameRacerSkinSet set = GameRacerSkinSet.Current;

        if (!GameRacePalettes.enabled || set == null) {
            return;
        }

        if (riderRoot != null) {
            List<GameRacePalettes.Row> pool = Usable(GameRacePalettes.Riders(), set);
            Material m = Resolve(pool, ProfileValue(GameRacerLookKeys.riderSkin), seed, set);

            if (m != null) {
                Rig rig = Rig.Capture(riderRoot, null, FamilyOf(pool[0].material));
                rig.Apply(m);
            }
        }

        if (vehicleRoot != null) {
            List<GameRacePalettes.Row> pool = Usable(GameRacePalettes.ForVehicle(vehicleCode), set);

            if (pool.Count > 0) {
                Material m = Resolve(pool, ProfileValue(GameRacerLookKeys.VehicleSkin(pool[0].family)), seed, set);

                // the seat keeps the rider's skin (OnRiderMounted copies it): the bike rig has no seat
                Rig.Capture(vehicleRoot, null, FamilyOf(pool[0].material)).Apply(m);
            }
        }
    }

    // pick -> material: "" = default, "random" = seeded, a code = that row. Null when the pool is empty.
    public static Material Resolve(List<GameRacePalettes.Row> pool, string pick, uint seed, GameRacerSkinSet set) {

        if (pool == null || pool.Count == 0 || set == null) {
            return null;
        }

        int i = PickIndex(pool, pick);

        if (i < 0) {
            i = (int)(SeedHash.Mix(seed ^ GameRacePalettes.Rules.salt) % (uint)pool.Count);
        }

        return set.Find(pool[i].material);
    }

    private static string ProfileValue(string key) {
        return GameProfiles.Current != null ? GameProfiles.Current.GetAttributeStringValue(key) : "";
    }
}
#endif
