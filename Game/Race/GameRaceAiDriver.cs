#if USE_AGNOSTIC_VEHICLE
using Agnostic.Racing;
using Agnostic.Vehicle;

// One AI rider's brain (Action Bikes R2 step 7): RaceAi.Steer once per fixed step, on GameVehicleArcade.driverOverride.
// The sticks are in the camera-yaw-0 frame the core makes them in, so cameraYaw is zeroed.
public class GameRaceAiDriver : IGameVehicleArcadeDriver {

    public int index;                 // the racer's slot
    public RaceAiProfile profile;
    public RaceAiMemory memory;
    public uint seed;

    public GameRaceAiDriver(int index, RaceAiProfile profile, uint seed) {
        this.index = index;
        this.profile = profile;
        this.seed = seed;
    }

    // A respawned rider starts its line and reaction ring afresh.
    public void ResetMemory() {
        memory = default(RaceAiMemory);
    }

    public void Drive(GameVehicleArcade arcade, int step, ref Sticks sticks, ref float cameraYaw) {

        if (!GameRace.active || GameRace.track == null) {
            sticks = default(Sticks);
            return;
        }

        RaceRival rival;
        GameRace.NearestRival(index, out rival);
        sticks = RaceAi.Steer(GameRace.track, in arcade.state, in profile, ref memory, in rival, step, seed,
            GameRace.rules.aiLateralAccel);
        cameraYaw = 0f;
    }
}
#endif
