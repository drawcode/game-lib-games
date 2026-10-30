using UnityEngine;

// The seam between GameObjectMountVehicle and a vehicle model other than GameVehicleDrive: the arcade host
// (GameVehicleArcade, Action Bikes). The mount forwards the rider's sticks here and reads the speed back for the
// HUD. It is an interface so the mount and the player controller compile in every title, whether or not the
// title builds the arcade core (USE_AGNOSTIC_VEHICLE).
public interface IGameVehicleMountDriver {

    float speed { get; }

    // Left stick in the screen frame (x right, y up the screen), after the dead zone.
    void SetMoveStick(float x, float y);

    // Right stick, same frame. Twin-stick fire past the class's aimFireThreshold.
    void SetAimStick(float x, float y);

    // Agnostic.Vehicle.VehicleButtons bits: boost 1, jump 2, drift 4, use 8, fire 16.
    void SetButtons(int buttons);

    void OnRiderMounted(GameObject rider);

    void OnRiderUnmounted(GameObject rider);
}

// The player's steering choice (D8), the profile attribute every arcade vehicle reads on mount. "" (never picked)
// = the vehicle class's data.control.mode.
public static class GameVehicleControlModes {

    public const string profileKey = "vehicle-control-mode";
    public const string heading = "heading";   // Settings: "Point to steer"
    public const string direct = "direct";     // Settings: "Classic"
}
