#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;
using System.Text;

using Agnostic.Racing;

using Engine.AgnosticHost;
using Engine.Game.App;
using Engine.Data.Json;

// The career cup in progress (Action Bikes career): one cup, its standings and what was paid, kept as the
// career-state profile string. Pure bookkeeping over RaceSeries: the title owns the profile save and the screens.
//   {"cup": "cup-n00b", "race": 2, "standings": [{"id": "player", "pts": 18, "places": [1, 0, ...], "last": 2}], "paid": []}
// Reading is tolerant: a missing key keeps its default, unreadable text is no cup running, and a key this build does
// not know (top level or inside a standing) is kept and written back as it came. Default off (enabled); the Bikes
// title turns it on (AppRaceConfigs). With it off, and GameRace.rulesOverride and playerVehicleOverride null, a race
// is today's race.
public static class GameRaceCareer {

    public const string stateKey = "career-state";
    public const string playerId = "player";
    public const string aiUdidPrefix = "ai-";

    public static bool enabled;             // the title turns the career on

    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
        enabled = false;
    }

    public class State {

        public string cup = "";
        public int race;                                    // the next round, 0-based; tracks.Length = the cup is done
        public SeriesStanding[] standings = new SeriesStanding[0];
        public List<string> paid = new List<string>();
        public readonly List<KeyValuePair<string, JsonData>> extras = new List<KeyValuePair<string, JsonData>>();
        public readonly List<List<KeyValuePair<string, JsonData>>> standingExtras = new List<List<KeyValuePair<string, JsonData>>>();

        public bool running {
            get {
                return !string.IsNullOrEmpty(cup);
            }
        }

        // "" is no cup running; so is text that cannot be read.
        public static State Parse(string json) {

            State s = new State();

            if (string.IsNullOrEmpty(json)) {
                return s;
            }

            JsonData n;

            try {
                n = JsonMapper.ToObject(json);
            }
            catch (Exception) {
                return s;
            }

            if (n == null || !n.IsObject) {
                return s;
            }

            s.cup = VehicleTuningJson.Str(n, "cup");
            s.race = Math.Max(0, (int)VehicleTuningJson.Num(n, "race", 0f));

            List<SeriesStanding> list = new List<SeriesStanding>();

            if (VehicleTuningJson.Has(n, "standings") && n["standings"].IsArray) {
                for (int i = 0; i < n["standings"].Count; i++) {
                    JsonData row = n["standings"][i];
                    string id = VehicleTuningJson.Str(row, "id");

                    if (string.IsNullOrEmpty(id)) {
                        continue;
                    }

                    SeriesStanding st = new SeriesStanding(id);
                    st.points = (int)VehicleTuningJson.Num(row, "pts", 0f);
                    st.lastPlace = (int)VehicleTuningJson.Num(row, "last", 0f);

                    if (VehicleTuningJson.Has(row, "places") && row["places"].IsArray) {
                        for (int p = 0; p < row["places"].Count && p < st.placeCounts.Length; p++) {
                            st.placeCounts[p] = (int)VehicleTuningJson.Number(row["places"][p], 0f);
                        }
                    }

                    List<KeyValuePair<string, JsonData>> extra = new List<KeyValuePair<string, JsonData>>();

                    foreach (string key in row.Keys) {
                        if (key != "id" && key != "pts" && key != "last" && key != "places") {
                            extra.Add(new KeyValuePair<string, JsonData>(key, row[key]));
                        }
                    }

                    list.Add(st);
                    s.standingExtras.Add(extra);
                }
            }

            s.standings = list.ToArray();

            if (VehicleTuningJson.Has(n, "paid") && n["paid"].IsArray) {
                for (int i = 0; i < n["paid"].Count; i++) {
                    if (n["paid"][i].IsString) {
                        s.paid.Add((string)n["paid"][i]);
                    }
                }
            }

            foreach (string key in n.Keys) {
                if (key != "cup" && key != "race" && key != "standings" && key != "paid") {
                    s.extras.Add(new KeyValuePair<string, JsonData>(key, n[key]));
                }
            }

            return s;
        }

        // "" when no cup is running.
        public string ToJson() {

            if (!running) {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            JsonWriter w = new JsonWriter(sb);
            w.WriteObjectStart();
            w.WritePropertyName("cup");
            w.Write(cup);
            w.WritePropertyName("race");
            w.Write(race);
            w.WritePropertyName("standings");
            w.WriteArrayStart();

            for (int i = 0; i < standings.Length; i++) {

                SeriesStanding st = standings[i];
                w.WriteObjectStart();
                w.WritePropertyName("id");
                w.Write(st.id);
                w.WritePropertyName("pts");
                w.Write(st.points);
                w.WritePropertyName("places");
                w.WriteArrayStart();

                for (int p = 0; p < st.placeCounts.Length; p++) {
                    w.Write(st.placeCounts[p]);
                }

                w.WriteArrayEnd();
                w.WritePropertyName("last");
                w.Write(st.lastPlace);

                if (i < standingExtras.Count) {
                    foreach (KeyValuePair<string, JsonData> e in standingExtras[i]) {
                        w.WritePropertyName(e.Key);
                        e.Value.ToJson(w);
                    }
                }

                w.WriteObjectEnd();
            }

            w.WriteArrayEnd();
            w.WritePropertyName("paid");
            w.WriteArrayStart();

            for (int i = 0; i < paid.Count; i++) {
                w.Write(paid[i]);
            }

            w.WriteArrayEnd();

            foreach (KeyValuePair<string, JsonData> e in extras) {
                w.WritePropertyName(e.Key);
                e.Value.ToJson(w);
            }

            w.WriteObjectEnd();
            return sb.ToString();
        }

        // The standings order, leader first: indices into standings.
        public int[] Order() {

            int[] order = new int[standings.Length];
            RaceSeries.Rank(standings, standings.Length, order);
            return order;
        }

        // The player's place in the standings, 1-based; 0 when the player has no standing.
        public int PlayerPlace() {

            int[] order = Order();

            for (int i = 0; i < order.Length; i++) {
                if (standings[order[i]].id == playerId) {
                    return i + 1;
                }
            }

            return 0;
        }
    }

    // A new run of the cup: the player and the cup's AI profile codes at zero. An AI count past the profile list repeats
    // codes with the same "-2" suffix GameRace gives the repeats, so the ids match the race results.
    public static State Start(GameRaceCups.Row cup) {

        State s = new State();
        s.cup = cup.code;
        GameRaceRules rules = GameRaceRules.Parse(GameRaceRules.Merge(null, cup.RaceBlock(0)));
        List<SeriesStanding> list = new List<SeriesStanding>();
        list.Add(new SeriesStanding(playerId));

        for (int k = 0; k < rules.aiRiders && rules.aiProfiles.Length > 0; k++) {

            string baseId = rules.aiProfiles[k % rules.aiProfiles.Length];
            string id = baseId;
            int repeat = 1;

            while (list.Exists(x => x.id == id)) {
                repeat++;
                id = baseId + "-" + repeat;
            }

            list.Add(new SeriesStanding(id));
        }

        s.standings = list.ToArray();

        for (int i = 0; i < s.standings.Length; i++) {
            s.standingExtras.Add(new List<KeyValuePair<string, JsonData>>());
        }

        return s;
    }

    // Sets up round k (0-based) of the cup: GameRace.rulesOverride is the cup's race block with per_race[k] over it, and
    // from round 2 on a cup with the reverse_standings grid sets GameRace.qualifiedOrder (the leader starts last).
    // The caller then runs the level of cup.tracks[k]. Returns false for a round outside the cup.
    public static bool ApplyRace(State state, GameRaceCups.Row cup, int k) {

        if (state == null || cup == null || k < 0 || k >= cup.tracks.Length) {
            return false;
        }

        GameRace.rulesOverride = cup.RaceBlock(k);
        GameRace.qualifiedOrder = k > 0 && cup.grid == GameRaceCups.gridReverseStandings ? QualifiedOrder(state) : null;
        return true;
    }

    // The rider ids in grid-slot order, the leader last; null with no standings.
    public static string[] QualifiedOrder(State state) {

        if (state == null || state.standings.Length == 0) {
            return null;
        }

        int n = state.standings.Length;
        int[] order = state.Order();
        int[] grid = new int[n];
        RaceSeries.ReverseGrid(order, n, grid);
        string[] ids = new string[n];

        for (int i = 0; i < n; i++) {
            ids[i] = state.standings[grid[i]].id;
        }

        return ids;
    }

    // Scores one finished race into the state and moves to the next round. Result udids are "player" or
    // "ai-<profile code>"; the "ai-" prefix is stripped to give the standing id.
    public static void Record(State state, GameRaceCups.Row cup, GameRaceResultsData results) {

        if (results == null || results.players == null) {
            return;
        }

        int count = results.players.Count;
        string[] ids = new string[count];
        int[] places = new int[count];

        for (int i = 0; i < count; i++) {
            ids[i] = StandingId(results.players[i].udid);
            places[i] = results.players[i].place;
        }

        Record(state, cup, ids, places, count);
    }

    public static void Record(State state, GameRaceCups.Row cup, string[] ids, int[] places, int count) {

        RaceSeries.Score(cup.points, ids, places, count, state.standings, state.standings.Length);
        state.race++;
    }

    public static string StandingId(string udid) {

        if (!string.IsNullOrEmpty(udid) && udid.StartsWith(aiUdidPrefix, StringComparison.Ordinal)) {
            return udid.Substring(aiUdidPrefix.Length);
        }

        return udid;
    }

    // True once every round of the cup is recorded.
    public static bool Done(State state, GameRaceCups.Row cup) {
        return state != null && cup != null && state.race >= cup.tracks.Length;
    }
}
#endif
