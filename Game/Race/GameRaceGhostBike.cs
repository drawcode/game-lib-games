#if USE_AGNOSTIC_VEHICLE
using UnityEngine;

// The ghost bike's per-frame pose: GameRaceGhostPlayer feeds it the lap clock each fixed step, this eases it between
// the steps the way a ridden arcade is eased (one step behind the newest state), so the ghost glides at any frame rate.
public class GameRaceGhostBike : MonoBehaviour {

    public float stepClock;
    public float stepTime;
    public bool playing;

    private void LateUpdate() {

        if (!playing) {
            return;
        }

        float dt = GameVehicleArcade.fixedDt;
        float elapsed = Mathf.Clamp(Time.time - stepTime, 0f, dt);
        GameRaceGhostPlayer.PoseAt(Mathf.Max(0f, stepClock - dt + elapsed));
    }
}
#endif
