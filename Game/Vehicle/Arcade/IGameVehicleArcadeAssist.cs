#if USE_AGNOSTIC_VEHICLE
using Agnostic.Vehicle;

// The drive assist seam (D20): called once per fixed step in the rider branch of GameVehicleArcade.StepOnce, on the
// mapped input, and returns the input to step the model with. Never called under hold or a driverOverride (an AI
// rider, a test autopilot). GameDriveAssist (Race) is the implementation; the arcade holds null until it is wanted.
public interface IGameVehicleArcadeAssist {

    // `a.stepEvents` is still the previous step's events when this runs.
    VehicleInput Apply(GameVehicleArcade a, int step, in VehicleInput mapped);

    // After ResetAt (a respawn): start the grace window again and forget the track projection.
    void Reset();
}
#endif
