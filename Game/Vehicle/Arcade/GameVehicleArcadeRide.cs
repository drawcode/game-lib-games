#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using Agnostic.Vehicle;

using Engine.Game.App;

using UnityEngine;

// The seat rider's riding clips (riders doc section 5.3), picked each frame from the arcade state and events. Each
// slot is the game-preset-data row animation-ride-<slot>-<ride_presets>, whose items are legacy clip names with
// probabilities. Clips the model does not have are dropped, and a slot with none falls back to a calmer slot.
// One-shots (landing, trick, hit, fail) hold for their clip length unless something more urgent comes in.
public class GameVehicleArcadeRide {

    public const float fade = 0.2f;
    public const float fastFrac = 0.6f;    // speed / maxSpeed above which drive and turn use their fast clips
    public const float turnSteer = 0.35f;  // |steer| above which the rider turns
    public const float moveSpeed = 0.75f;  // m/s below which the rider idles

    public const int idle = 0, drive = 1, driveFast = 2, turnLeft = 3, turnRight = 4, turnFastLeft = 5, turnFastRight = 6,
        nitro = 7, jumpUp = 8, jumpDown = 9, trick = 10, fail = 11, hitLeft = 12, hitRight = 13, slotCount = 14;

    public static readonly string[] slotNames = {
        "idle", "drive", "drive-fast", "turn-left", "turn-right", "turn-fast-left", "turn-fast-right",
        "nitro", "jump-up", "jump-down", "trick", "fail", "hit-left", "hit-right"
    };

    // Where a slot with no clip goes; -1 = keep what is playing.
    private static readonly int[] fallback = {
        -1, idle, drive, drive, drive, turnLeft, turnRight, driveFast, -1, -1, -1, -1, -1, -1
    };

    // 0 loops, 1 one-shots, 2 the crash.
    private static readonly int[] priority = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 1, 1 };

    private readonly Animation anim;
    private readonly List<string>[] clips = new List<string>[slotCount];
    private readonly List<float>[] weights = new List<float>[slotCount];

    public int slot = -1;
    public string clip = "";
    private float holdUntil;

    public GameVehicleArcadeRide(Animation anim, string presets) {

        this.anim = anim;

        for (int i = 0; i < slotCount; i++) {
            clips[i] = new List<string>();
            weights[i] = new List<float>();
        }

        if (anim == null || string.IsNullOrEmpty(presets)) {
            return;
        }

        for (int i = 0; i < slotCount; i++) {
            GamePreset preset = GamePresets.Instance.GetById("animation-ride-" + slotNames[i] + "-" + presets);

            if (preset == null || preset.data == null || preset.data.items == null) {
                continue;
            }

            foreach (GamePresetItem item in preset.data.items) {
                if (!string.IsNullOrEmpty(item.code) && anim[item.code] != null) {
                    clips[i].Add(item.code);
                    weights[i].Add((float)item.probability);
                }
            }
        }
    }

    public int ClipCount(int s) {
        return clips[s].Count;
    }

    public void Update(GameVehicleArcade arcade, float time) {

        if (anim == null) {
            return;
        }

        int want = Resolve(Choose(arcade));

        if (want < 0 || want == slot) {
            return;
        }

        if (time < holdUntil && priority[want] <= priority[slot]) {
            return;
        }

        Play(want, time);
    }

    public static int Choose(GameVehicleArcade arcade) {

        VehicleState s = arcade.state;
        VehicleEvents e = arcade.frameEvents;

        if (((s.flags & (int)VehicleFlags.bailed) | (e.flags & (int)VehicleEventFlags.bailed)) != 0) {
            return fail;
        }

        if ((e.flags & (int)VehicleEventFlags.trick) != 0) {
            return trick;
        }

        if ((e.flags & (int)VehicleEventFlags.wallHit) != 0) {
            return arcade.input.steer >= 0f ? hitRight : hitLeft;
        }

        if ((e.flags & (int)VehicleEventFlags.landed) != 0) {
            return jumpDown;
        }

        if ((s.flags & (int)VehicleFlags.grounded) == 0) {
            return jumpUp;
        }

        if ((s.flags & (int)VehicleFlags.boosting) != 0) {
            return nitro;
        }

        if (s.speed < moveSpeed) {
            return idle;
        }

        bool fast = s.speed > fastFrac * arcade.config.tuning.maxSpeed;
        float steer = arcade.input.steer;

        if (steer > turnSteer) {
            return fast ? turnFastRight : turnRight;
        }

        if (steer < -turnSteer) {
            return fast ? turnFastLeft : turnLeft;
        }

        return fast ? driveFast : drive;
    }

    private int Resolve(int s) {

        while (s >= 0 && clips[s].Count == 0) {
            s = fallback[s];
        }

        return s;
    }

    private void Play(int s, float time) {

        List<string> list = clips[s];
        List<float> w = weights[s];
        float total = 0f;

        for (int i = 0; i < w.Count; i++) {
            total += w[i];
        }

        float pick = Random.value * total;
        int chosen = list.Count - 1;

        for (int i = 0; i < w.Count; i++) {
            pick -= w[i];

            if (pick <= 0f) {
                chosen = i;
                break;
            }
        }

        slot = s;
        clip = list[chosen];
        anim.CrossFade(clip, fade);
        holdUntil = priority[s] > 0 ? time + anim[clip].length : 0f;
    }
}
#endif
