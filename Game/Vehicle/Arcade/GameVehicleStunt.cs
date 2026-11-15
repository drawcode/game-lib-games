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
    public float points;         // the held pose's points so far, or the awarded points on a landing
}

// Stunt host (D24): feeds the pure core (Stunt.Step / Land) from the arcade's rider branch. The arcade owns one only
// when its data row has an enabled stunt block, so an AI arcade, a ghost, a vehicle without data and Action Bots never
// have one. Order per fixed step, as the core asks: BeforeStep (forecast, Stunt.Step, the pop) then
// VehicleModel.Step, then AfterStep (Stunt.Land on the landed event, the bail, the boost, the result).
// - Forecast: LandingForecast.Predict every 2nd airborne step. When the drive assist already ran this step its
//   forecast is reused (GameVehicleArcade.assistForecast); otherwise it is run here, so the assist off costs one
//   more forecast on the player only.
// - Pop: only when the forecast landing normal is flat (StuntTuning.popFlatDot).
// - Nothing allocates per step.
public class GameVehicleStunt {

    public const int forecastEvery = 2;

    public const int landNone = 0, landClean = 1, landSketchy = 2, landBail = 3;

    public StuntState state;
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
        lastLandResult = landNone;
        lastLandPoints = 0f;
        stepClean = false;
        Ready = false;
    }

    // Before VehicleModel.Step. `input` is the step's input (the assist's output); StickMapper passes the stunt bit. a.state
    // is still the pre-step state here, as the core wants.
    public void BeforeStep(GameVehicleArcade a, ref VehicleInput input) {

        stepClean = false;
        StuntTuning tuning = a.config.stunt;

        if ((a.state.flags & (int)VehicleFlags.bailed) != 0) {
            state = default(StuntState);
        }

        float timeToLand = ForecastTime(a);
        Ready = Stunt.Ready(in tuning, in a.state, in state, timeToLand);

        StuntEvents ev;
        Stunt.Step(in tuning, in a.riderStats, in a.state, input.buttons, timeToLand, forecast.normal.y, a.assist != null, ref state,
            GameVehicleArcade.fixedDt, out ev);

        if (ev.started != 0) {
            a.state.velocity.y += ev.popDv;
            a.state.speed = a.state.velocity.length;
        }

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

    // After VehicleModel.Step, before the events are stored: a landing resolves the stunt. `ev` takes the bail.
    public void AfterStep(GameVehicleArcade a, ref VehicleEvents ev) {

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
