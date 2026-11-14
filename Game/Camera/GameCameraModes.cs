#if USE_AGNOSTIC_VEHICLE
// The player's camera choice (D21), the profile attribute the iso rig reads on game start and on a Settings change.
// "" (never picked) = the vehicle row's data.camera.mode. The int `camera-mode` key is a different, dormant setting.
public static class GameCameraModes {

    public const string profileKey = "vehicle-camera-mode";
    public const string follow = "follow";   // Settings: "Follow", today's fixed-yaw follow camera
    public const string iso = "iso";         // Settings: "Isometric"

    // A picked follow or iso wins; "" and anything else falls back to the data's default (follow when that is junk).
    public static string Resolve(string picked, string dataDefault) {

        if (picked == follow || picked == iso) {
            return picked;
        }

        return dataDefault == iso ? iso : follow;
    }

    public static string Pick(string dataDefault) {
        string picked = GameProfiles.Current != null ? GameProfiles.Current.GetAttributeStringValue(profileKey) : "";
        return Resolve(picked, dataDefault);
    }
}
#endif
