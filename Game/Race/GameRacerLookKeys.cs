#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

// The profile keys of the racer look (D22). Values: "" = today's look (the family's first skin), "random" = seeded
// per race, or a palette row code. R3 later moves them into GameProfileCustomItem.
public static class GameRacerLookKeys {

    public const string riderSkin = "racer-rider-skin";
    public const string vehicleSkinPrefix = "racer-vehicle-skin-";

    public const string none = "";
    public const string random = "random";

    private static readonly Dictionary<string, string> vehicleKeys = new Dictionary<string, string>();

    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        vehicleKeys.Clear();
    }

    // racer-vehicle-skin-<family>, built once per family.
    public static string VehicleSkin(string family) {

        if (string.IsNullOrEmpty(family)) {
            return vehicleSkinPrefix;
        }

        string key;

        if (!vehicleKeys.TryGetValue(family, out key)) {
            key = vehicleSkinPrefix + family;
            vehicleKeys[family] = key;
        }

        return key;
    }
}
#endif
