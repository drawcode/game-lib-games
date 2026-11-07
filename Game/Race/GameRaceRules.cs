#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections;
using System.Collections.Generic;

using Agnostic.Racing;

using Engine.Game.App.BaseApp;
using Engine.Game.Data;

// The rules of one race, from the level row's data.race block (Action Bikes R2 step 7). Every key is optional: a
// missing block, key or unreadable value keeps the default. Numbers may be written as numbers or strings.
//   "data": {"level_kind": "race", "race": {"laps": 3, "ai_riders": 5, "player_slot": -1, ...}}
public class GameRaceRules {

    public const string levelKindRace = "race";
    public const string keyRace = "race";

    public int laps = 3;
    public int aiRiders = 5;                          // clamped 0..11
    public string[] aiProfiles = new string[0];      // profile codes, the k-th AI takes [k % length]
    public string vehicleCode = "vehicle-bike-dirt-1";
    public int playerSlot = -1;                       // -1 = random
    public float countdown = 3f;
    public float finishWait = 8f;
    public float respawnDelay = 2f;                   // a bailed rider's wait before it is put back on the track
    public float contactGrace = 1f;                   // seconds without bumps after a respawn
    public float defaultWidth = 10f;                  // a node without a width
    public float missDistance = 15f;
    public float missRespawn = 3f;
    public float wrongWayDistance = 5f;
    public float offCourseFactor = 2f;                // |lateral| beyond this many track widths counts as off course
    public float offCourseTime = 3f;
    public float fallDepth = 10f;                     // metres under the lowest level item
    public float respawnLift = 0.5f;
    public float bumpAheadFloor = 3f;                 // m/s of bump delta-v that bails the rider ahead; 0 = off
    public float bumpAheadMin = 0.3f;                 // metres of progress between two riders before "ahead" counts
    public float aiLateralAccel = 20f;                // RaceAi's lateral budget
    public uint seed = 0;                             // 0 = time-based

    // The current level row's data block, read without Get (which writes defaults); null when there is none.
    public static IDictionary<string, object> LevelData() {

        GameLevel level = GameLevels.Current;

        if (level == null) {
            return null;
        }

        object rawData;

        if (!level.TryGetValue(BaseDataObjectKeys.data, out rawData) || rawData == null) {
            return null;
        }

        return rawData as IDictionary<string, object>;
    }

    public static bool IsRaceLevel() {

        IDictionary<string, object> data = LevelData();
        object kind;

        return data != null && data.TryGetValue(GameArcadeLevelKeys.level_kind, out kind) && kind != null
            && string.Equals(kind.ToString(), levelKindRace, StringComparison.Ordinal);
    }

    public static GameRaceRules FromCurrentLevel() {

        IDictionary<string, object> data = LevelData();
        object race = null;

        if (data != null) {
            data.TryGetValue(keyRace, out race);
        }

        return Parse(race as IDictionary);
    }

    public static GameRaceRules Parse(IDictionary race) {

        GameRaceRules r = new GameRaceRules();

        if (race == null) {
            return r;
        }

        r.laps = Math.Max(1, Int(race, "laps", r.laps));
        r.aiRiders = Math.Max(0, Math.Min(11, Int(race, "ai_riders", r.aiRiders)));
        r.playerSlot = Int(race, "player_slot", r.playerSlot);
        r.countdown = Math.Max(0f, Num(race, "countdown", r.countdown));
        r.finishWait = Math.Max(0f, Num(race, "finish_wait", r.finishWait));
        r.respawnDelay = Math.Max(0f, Num(race, "respawn_delay", r.respawnDelay));
        r.contactGrace = Math.Max(0f, Num(race, "contact_grace", r.contactGrace));
        r.defaultWidth = Num(race, "default_width", r.defaultWidth);
        r.missDistance = Num(race, "miss_distance", r.missDistance);
        r.missRespawn = Math.Max(0f, Num(race, "miss_respawn", r.missRespawn));
        r.wrongWayDistance = Num(race, "wrong_way_distance", r.wrongWayDistance);
        r.offCourseFactor = Num(race, "off_course_factor", r.offCourseFactor);
        r.offCourseTime = Math.Max(0f, Num(race, "off_course_time", r.offCourseTime));
        r.fallDepth = Num(race, "fall_depth", r.fallDepth);
        r.respawnLift = Num(race, "respawn_lift", r.respawnLift);
        r.bumpAheadFloor = Math.Max(0f, Num(race, "bump_ahead_floor", r.bumpAheadFloor));
        r.bumpAheadMin = Math.Max(0f, Num(race, "bump_ahead_min", r.bumpAheadMin));
        r.aiLateralAccel = Num(race, "ai_lateral_accel", r.aiLateralAccel);

        double seed;

        if (GameRaceNodes.TryNumber(race, "seed", out seed) && seed > 0) {
            r.seed = (uint)seed;
        }

        if (race.Contains("vehicle_code") && race["vehicle_code"] != null && race["vehicle_code"].ToString().Length > 0) {
            r.vehicleCode = race["vehicle_code"].ToString();
        }

        IList profiles = race.Contains("ai_profiles") ? race["ai_profiles"] as IList : null;

        if (profiles != null) {
            List<string> codes = new List<string>();

            foreach (object item in profiles) {
                if (item != null && item.ToString().Length > 0) {
                    codes.Add(item.ToString());
                }
            }

            r.aiProfiles = codes.ToArray();
        }

        return r;
    }

    static float Num(IDictionary race, string key, float fallback) {
        double value;
        return GameRaceNodes.TryNumber(race, key, out value) ? (float)value : fallback;
    }

    static int Int(IDictionary race, string key, int fallback) {
        double value;
        return GameRaceNodes.TryNumber(race, key, out value) ? (int)Math.Round(value) : fallback;
    }

    public RaceRules ToCore() {

        RaceRules r = RaceRules.Defaults();
        r.laps = laps;
        r.missDistance = missDistance;
        r.wrongWayDistance = wrongWayDistance;
        r.countdown = countdown;
        r.finishWait = finishWait;
        return r;
    }
}
#endif
