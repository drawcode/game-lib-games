#if USE_AGNOSTIC_VEHICLE
using Agnostic.Vehicle;

using UnityEngine;

// What the stunt host did on one fixed step, for tests, the probes and the HUD; the last step's, like stepAssist.
public struct GameVehicleStuntTrace {

    public int phase;            // StuntPhase after the step
    public int started;
    public int released;
    public int autoReleased;
    public int landResult;       // GameVehicleStunt.landNone / landClean / landSketchy / landBail
    public float timeToLand;     // the forecast the step used, s
    public float rate;
    public float takeoffBoost;   // m/s added to velocity.y at take-off this step, 0 when none
    public float points;         // the held pose's points so far, or the awarded points on a landing
}

// Stunt host (D24): feeds the pure core (Stunt.Step / Land) from the arcade's rider branch. The arcade owns one only
// when its data row has an enabled stunt block, so an AI arcade, a ghost, a vehicle without data and Action Bots never
// have one. Order per fixed step, as the core asks: BeforeStep (forecast, Stunt.Step) then VehicleModel.Step, then
// AfterStep (the take-off boost on the airborne event, Stunt.Land on the landed event, the bail, the boost, the result).
// - Forecast: LandingForecast.Predict every 2nd airborne step. When the drive assist already ran this step its
//   forecast is reused (GameVehicleArcade.assistForecast); otherwise it is run here, so the assist off costs one
//   more forecast on the player only.
// - Take-off boost (no input): the model's airborne event (grounded the step before, airborne after VehicleModel.Step) is
//   the take-off signal, so the boost is applied in AfterStep, after the model step and before stepEvents are stored,
//   to the post-step velocity.y. The drive assist's stamped forecast (assistForecast) is of the pre-step, grounded
//   state at that moment (no landing), so it is never reusable here: the host runs LandingForecast.Predict on the
//   airborne state (one forecast at take-off, one more for the boosted arc). Stunt.TakeoffBoost gives the dv for a
//   stunt-length jump with a flat landing; a forecast with no hit inside the horizon gives no boost (nothing known to
//   land on); the boosted arc is forecast again and the boost is cancelled when it lands on a slope (normal.y <
//   popFlatDot). At most once per take-off; the player's rider only, as the whole host is.
// - Nothing allocates per step.
public class GameVehicleStunt {

    public const int forecastEvery = 2;

    public const int landNone = 0, landClean = 1, landSketchy = 2, landBail = 3;

    public StuntState state;
    public int boostCount;            // take-off boosts given so far
    public float lastBoost;           // m/s of the last one
    public float stepBoost;           // this step's take-off boost, 0 when none
    public int landCount;             // landings with a stunt so far; a HUD watches it change
    public int lastLandResult;        // landNone until the first one, then clean / sketchy / bail
    public float lastLandPoints;      // 0 on a bail
    public bool stepClean;            // this step's landing was a clean stunt (sketchy included); the goals read it

    private LandingForecast forecast;
    private int airSteps;

    // True when a press would start a stunt now (the HUD button glows on it). As of the last BeforeStep.
    public bool Ready { get; private set; }

    public int phase { get { return state.phase; } }

    // The held pose's points so far; after the landing, lastLandPoints.
    public float points { get { return state.points; } }

    public void Reset() {
        state = default(StuntState);
        forecast = default(LandingForecast);
        airSteps = 0;
        landCount = 0;
        boostCount = 0;
        lastBoost = 0f;
        stepBoost = 0f;
        lastLandResult = landNone;
        lastLandPoints = 0f;
        stepClean = false;
        Ready = false;
    }

    // Before VehicleModel.Step. `input` is the step's input (the assist's output); StickMapper passes the stunt bit. a.state
    // is still the pre-step state here, as the core wants.
    public void BeforeStep(GameVehicleArcade a, ref VehicleInput input) {

        stepClean = false;
        stepBoost = 0f;
        StuntTuning tuning = a.config.stunt;

        if ((a.state.flags & (int)VehicleFlags.bailed) != 0) {
            state = default(StuntState);
        }

        float timeToLand = ForecastTime(a);
        Ready = Stunt.Ready(in tuning, in a.state, in state, timeToLand);

        StuntEvents ev;
        Stunt.Step(in tuning, in a.riderStats, in a.state, input.buttons, timeToLand, a.assist != null, ref state,
            GameVehicleArcade.fixedDt, out ev);

        GameVehicleStuntTrace t = new GameVehicleStuntTrace();
        t.phase = state.phase;
        t.started = ev.started;
        t.released = ev.released;
        t.autoReleased = ev.autoReleased;
        t.timeToLand = timeToLand;
        t.rate = state.rate;
        t.points = state.points;
        a.stepStunt = t;
    }

    // After VehicleModel.Step, before the events are stored: a take-off gets its boost, a landing resolves the stunt. `ev`
    // takes the bail.
    public void AfterStep(GameVehicleArcade a, ref VehicleEvents ev) {

        if ((ev.flags & (int)VehicleEventFlags.airborne) != 0) {
            TakeOff(a, in ev);
        }

        if ((ev.flags & (int)VehicleEventFlags.landed) == 0) {
            return;
        }

        bool modelBailed = (ev.flags & (int)VehicleEventFlags.bailed) != 0 || (a.state.flags & (int)VehicleFlags.bailed) != 0;
        bool had = (state.flags & (int)StuntFlags.used) != 0;
        StuntEvents s;
        Stunt.Land(in a.config.stunt, ev.landQuality, ref state, out s);

        if (!had) {
            return;
        }

        // The model's bail wins: Land only had to clear the state.
        int result = landBail;

        if (!modelBailed && s.bail != 0) {
            VehicleModel.Bail(ref a.state, in a.config.tuning, ref ev);
        }
        else if (!modelBailed && s.landedClean != 0) {
            result = s.sketchy != 0 ? landSketchy : landClean;
            stepClean = true;
            a.state.boostMeter = Mathf.Min(1f, a.state.boostMeter + s.boost);
        }

        landCount++;
        lastLandResult = result;
        lastLandPoints = result == landBail ? 0f : s.points;

        GameVehicleStuntTrace t = a.stepStunt;
        t.phase = state.phase;
        t.landResult = result;
        t.points = lastLandPoints;
        a.stepStunt = t;
    }

    // The take-off boost, on the airborne event step (once per take-off). Sets stepBoost / boostCount / lastBoost.
    private void TakeOff(GameVehicleArcade a, in VehicleEvents ev) {

        stepBoost = 0f;

        if ((ev.flags & (int)VehicleEventFlags.bailed) != 0 || (a.state.flags & (int)VehicleFlags.bailed) != 0) {
            return;
        }

        float dv = BoostFor(in a.config.stunt, in a.riderStats, in a.state, in a.config.tuning, a.vehicleWorld, in a.config.assist);

        if (dv <= 0f) {
            return;
        }

        a.state.velocity.y += dv;
        a.state.speed = a.state.velocity.length;
        stepBoost = dv;
        boostCount++;
        lastBoost = dv;
        GameVehicleStuntTrace t = a.stepStunt;
        t.takeoffBoost = dv;
        a.stepStunt = t;
    }

    // The boost for a vehicle that has just left the ground (`airborne` is its post-step state): the core's TakeoffBoost on
    // the forecast of the unboosted arc (0 when that forecast has no hit), cancelled (0) when the boosted arc lands on a
    // slope. Pure, no allocation.
    public static float BoostFor(in StuntTuning tuning, in StuntStats stats, in VehicleState airborne, in VehicleTuning vt,
                                 IVehicleWorld world, in DriveAssistTuning assist) {

        LandingForecast f;
        LandingForecast.Predict(in airborne, in vt, world, in assist, out f);

        if (f.hit == 0) {
            return 0f;
        }

        float dv = Stunt.TakeoffBoost(in tuning, in stats, f.timeToLand, f.normal.y);

        if (dv <= 0f) {
            return 0f;
        }

        VehicleState boosted = airborne;
        boosted.velocity.y += dv;
        LandingForecast after;
        LandingForecast.Predict(in boosted, in vt, world, in assist, out after);
        return after.hit != 0 && after.normal.y < tuning.popFlatDot ? 0f : dv;
    }

    // Seconds of air the forecast has left. Grounded: the horizon (nothing to land on). The assist's forecast when it
    // ran this step, else one of ours every forecastEvery airborne steps and the cache in between.
    private float ForecastTime(GameVehicleArcade a) {

        DriveAssistTuning tuning = a.config.assist;

        if ((a.state.flags & (int)VehicleFlags.grounded) != 0) {
            airSteps = 0;
            forecast = new LandingForecast();
            forecast.normal = Agnostic.Core.Vec3.up;
            forecast.timeToLand = tuning.forecastHorizon;
            return forecast.timeToLand;
        }

        if (a.assistForecastStep == a.stepCount) {
            forecast = a.assistForecast;
            airSteps++;
        }
        else {

            if (airSteps % forecastEvery == 0) {
                LandingForecast.Predict(in a.state, in a.config.tuning, a.vehicleWorld, in tuning, out forecast);
            }

            airSteps++;
        }

        return forecast.timeToLand;
    }
}
#endif
