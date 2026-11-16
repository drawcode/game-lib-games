#if USE_AGNOSTIC_VEHICLE
using Agnostic.Vehicle;

using Engine.Game.App.BaseApp;

// The rider's own stunt stats (D24), speed / power / health, 0..1, stored as profile attributes. They are independent of
// the on-foot RPG fields. A missing key reads as defaultStat; there is no UI to raise them yet.
public static class GameStuntStatKeys {

    public const string speed = "stunt-stat-speed";
    public const string power = "stunt-stat-power";
    public const string health = "stunt-stat-health";

    public const float defaultStat = 0.2f;

    // Each stat from the profile, clamped to 0..1; a missing key or a null profile gives defaultStat.
    public static StuntStats Read(BaseGameProfile profile) {

        StuntStats s = new StuntStats();
        s.speed = ReadOne(profile, speed);
        s.power = ReadOne(profile, power);
        s.health = ReadOne(profile, health);
        return s;
    }

    public static void Write(BaseGameProfile profile, StuntStats s) {

        if (profile == null) {
            return;
        }

        profile.SetAttributeDoubleValue(speed, s.speed);
        profile.SetAttributeDoubleValue(power, s.power);
        profile.SetAttributeDoubleValue(health, s.health);
    }

    private static float ReadOne(BaseGameProfile profile, string key) {

        if (profile == null || !profile.CheckIfAttributeExists(key)) {
            return defaultStat;
        }

        double v = profile.GetAttributeDoubleValue(key);
        return v < 0.0 ? 0f : (v > 1.0 ? 1f : (float)v);
    }
}
#endif
