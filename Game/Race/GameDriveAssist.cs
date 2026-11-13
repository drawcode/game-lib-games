#if USE_AGNOSTIC_VEHICLE
using Agnostic.Racing;
using Agnostic.Vehicle;

using UnityEngine;

// Drive assist host (D20): the pure core (DriveAssist.Step) fed by the race track and a landing forecast. It owns its
// RaceAiMemory (a projection hint of its own, never shared with an AI rider) and the assist's DriveAssistMemory.
// - Track: sensed only while a race runs (RACE, TIME TRIAL, QUALIFY). Free ride has hasTrack 0, so only the air
//   levelling runs.
// - Forecast: only while airborne, recomputed every 2nd airborne step and cached in between.
// - Off while bailed or in the grace window: DriveAssist.Step covers both. No rider and hold, driverOverride never
//   reach Apply (the arcade's rider branch only).
public class GameDriveAssist : IGameVehicleArcadeAssist {

    public const int forecastEvery = 2;

    private DriveAssistMemory memory;
    private RaceAiMemory aiMemory;
    private LandingForecast forecast;
    private int airSteps;
    private bool begun;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register() {
        GameVehicleArcade.assistFactory = Create;
    }

    private static IGameVehicleArcadeAssist Create(GameVehicleArcade a) {
        return new GameDriveAssist();
    }

    public VehicleInput Apply(GameVehicleArcade a, int step, in VehicleInput mapped) {

        DriveAssistTuning tuning = a.config.assist;
        ControlTuning control = a.control;

        if (!begun) {
            Begin(in tuning);
        }

        DriveAssistSense sense = new DriveAssistSense();

        if (GameRace.active && GameRace.track != null) {
            int flags = GameRace.Arcade(GameRace.playerIndex) == a ? GameRace.Racer(GameRace.playerIndex).flags : 0;
            RaceAssistSense.Sense(GameRace.track, in a.state, ref aiMemory, flags, GameRace.rules.aiLateralAccel,
                in control, out sense);
        }

        if ((a.state.flags & (int)VehicleFlags.grounded) != 0) {
            airSteps = 0;
            forecast = NoLanding(in tuning);
        }
        else {

            if (airSteps % forecastEvery == 0) {
                LandingForecast.Predict(in a.state, in a.config.tuning, a.vehicleWorld, in tuning, out forecast);
            }

            airSteps++;
        }

        VehicleInput output = DriveAssist.Step(in mapped, in a.state, in a.config.tuning, in control, in sense, in forecast,
            a.stepEvents.flags, in tuning, ref memory, GameVehicleArcade.fixedDt, out a.stepAssist);
        return output;
    }

    // A respawn: the grace window again, and a fresh track projection.
    public void Reset() {
        begun = false;
    }

    private void Begin(in DriveAssistTuning tuning) {
        memory = DriveAssistMemory.Begin(in tuning);
        aiMemory = default(RaceAiMemory);
        airSteps = 0;
        forecast = NoLanding(in tuning);
        begun = true;
    }

    private static LandingForecast NoLanding(in DriveAssistTuning tuning) {
        LandingForecast f = new LandingForecast();
        f.normal = Agnostic.Core.Vec3.up;
        f.timeToLand = tuning.forecastHorizon;
        return f;
    }
}
#endif
