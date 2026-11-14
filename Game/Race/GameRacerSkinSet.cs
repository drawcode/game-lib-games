#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using UnityEngine;

using Engine.AgnosticHost;

// The vendor skin materials a runtime swap can reach (D22). The materials sit outside Resources, so a prefab
// (version/shared/prefab/data/racer-skin-set) holds the references and Find answers by material name. No file or no
// prefab = no skins, which leaves the look off.
public class GameRacerSkinSet : MonoBehaviour {

    public Material[] materials;

    private Dictionary<string, Material> byName;

    private static GameRacerSkinSet current;
    private static bool loaded;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        current = null;
        loaded = false;
    }

    public static string resourcePath {
        get {
            return ContentsConfig.contentRootFolder + "/" + ContentsConfig.contentAppFolder + "/version/shared/prefab/data/racer-skin-set";
        }
    }

    // The set the race uses: the prefab's component, or the one Use() installed (tests).
    public static GameRacerSkinSet Current {
        get {
            if (!loaded) {
                loaded = true;
                GameObject prefab = Resources.Load<GameObject>(resourcePath);
                current = prefab != null ? prefab.GetComponent<GameRacerSkinSet>() : null;
            }

            return current;
        }
    }

    public static void Use(GameRacerSkinSet set) {
        current = set;
        loaded = true;
    }

    public Material Find(string materialName) {

        if (string.IsNullOrEmpty(materialName) || materials == null) {
            return null;
        }

        if (byName == null) {
            byName = new Dictionary<string, Material>();

            for (int i = 0; i < materials.Length; i++) {
                if (materials[i] != null) {
                    byName[materials[i].name] = materials[i];
                }
            }
        }

        Material m;
        return byName.TryGetValue(materialName, out m) ? m : null;
    }

    public static Material FindIn(GameRacerSkinSet set, string materialName) {
        return set != null ? set.Find(materialName) : null;
    }
}
#endif
