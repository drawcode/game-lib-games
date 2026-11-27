#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using Agnostic.Vehicle;

using Engine.Game.App;

using UnityEngine;

// The seat rider's riding clips (riders doc section 5.3), picked each frame from the arcade state and events. Each
// slot is the game-preset-data row animation-ride-<slot>-<ride_presets>, whose items are legacy clip names with
// probabilities. Clips the model does not have are dropped, and a slot with none falls back to a calmer slot.
// One-shots (landing, trick, spin, obstacle, hit, fail) hold for their clip length unless something more urgent
// comes in.
//
// Tricks play in the air, while the core turns the bike: once it has turned airTrick on one axis since takeoff,
// the rider strikes a trick pose (trick for flips and rolls, spin for yaw), once per jump. When the pose ends
// still airborne, the rider holds <clip>_loop if the model has one, else goes back to jump-up. Landing cuts any
// air clip for jump-down. Trick clips must move only the rider: a clip that turns the whole vehicle (salto) would
// fight the core's own rotation, so it stays out of these slots.
//
// Stunts (D24) are the button's rider poses, from the stunt and stunt-spin slots (rows animation-ride-stunt-<presets>
// and animation-ride-stunt-spin-<presets>), driven by the arcade's GameVehicleStunt phase rather than by Choose: start
// plays a pose at the stunt's rate, hold plays its _loop, recover cross-fades back to jump-up over the stunt's recover
// time. The pose is round robin over the clips of both slots (deterministic). A bail is the ordinary fail path.
public class GameVehicleArcadeRide {

    public const float fade = 0.2f;
    public const float fastFrac = 0.6f;    // speed / maxSpeed above which drive and turn use their fast clips
    public const float turnSteer = 0.35f;  // |steer| above which the rider turns
    public const float moveSpeed = 0.75f;  // m/s below which the rider idles
    public const float airTrick = 1.5708f; // rad turned on one axis in the air before the rider does a trick
    public const string loopSuffix = "_loop";

    public const int idle = 0, drive = 1, driveFast = 2, turnLeft = 3, turnRight = 4, turnFastLeft = 5, turnFastRight = 6,
        nitro = 7, jumpUp = 8, jumpDown = 9, trick = 10, fail = 11, hitLeft = 12, hitRight = 13, obstacle = 14,
        spin = 15, stunt = 16, stuntSpin = 17, slotCount = 18;

    public static readonly string[] slotNames = {
        "idle", "drive", "drive-fast", "turn-left", "turn-right", "turn-fast-left", "turn-fast-right",
        "nitro", "jump-up", "jump-down", "trick", "fail", "hit-left", "hit-right", "obstacle", "spin", "stunt", "stunt-spin"
    };

    // Where a slot with no clip goes; -1 = keep what is playing.
    private static readonly int[] fallback = {
        -1, idle, drive, drive, drive, turnLeft, turnRight, driveFast, -1, -1, jumpUp, -1, -1, -1, -1, trick, jumpUp, stunt
    };

    // 0 loops, 1 one-shots, 2 the crash.
    private static readonly int[] priority = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 1, 1, 1, 1, 0, 0 };

    private readonly Animation anim;
    private readonly List<string>[] clips = new List<string>[slotCount];
    private readonly List<float>[] weights = new List<float>[slotCount];

    public int slot = -1;
    public string clip = "";
    public bool airTricked;     // a trick pose has played since takeoff
    private float holdUntil;
    private int poseCount;      // stunts posed so far, the round robin index
    private string poseClip = "";

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

        if (UpdateStunt(arcade, time)) {
            return;
        }

        bool airborne = (arcade.state.flags & (int)VehicleFlags.grounded) == 0
            && (arcade.frameEvents.flags & (int)VehicleEventFlags.landed) == 0;

        if (!airborne) {
            airTricked = false;

            if (IsAirPose(slot)) {
                holdUntil = 0f;
            }
        }
        else if (IsAirPose(slot) && time >= holdUntil && !clip.EndsWith(loopSuffix, System.StringComparison.Ordinal)) {
            HoldAirPose(time);
        }

        int want = Resolve(Choose(arcade, airTricked));

        if (want < 0 || want == slot) {
            return;
        }

        if (time < holdUntil && priority[want] <= priority[slot]) {
            return;
        }

        Play(want, time);
    }

    // The stunt poses. True while start or hold owns the clip (nothing else plays then); recover starts the cross-fade
    // to jump-up and hands over to the regular choice, which then finds jump-up already playing.
    private bool UpdateStunt(GameVehicleArcade arcade, float time) {

        GameVehicleStunt host = arcade.stunt;

        if (host == null || (arcade.state.flags & (int)VehicleFlags.bailed) != 0) {
            return false;
        }

        int phase = host.state.phase;
        bool posing = slot == stunt || slot == stuntSpin;

        if (phase == (int)StuntPhase.start || phase == (int)StuntPhase.hold) {

            int pool = clips[stunt].Count + clips[stuntSpin].Count;

            if (pool == 0) {
                return false;
            }

            if (!posing) {
                PlayPose(poseCount++ % pool, host.state.rate);
            }
            else if (phase == (int)StuntPhase.hold && !clip.EndsWith(loopSuffix, System.StringComparison.Ordinal)) {
                string loop = clip + loopSuffix;

                if (anim[loop] != null) {
                    ResetSpeed();
                    clip = loop;
                    anim.CrossFade(clip, fade);
                }
            }

            return true;
        }

        if (posing) {
            ResetSpeed();
        }

        if (posing && phase == (int)StuntPhase.recover) {
            int up = Resolve(jumpUp);

            if (up >= 0) {
                Play(up, time, host.state.recover);
            }
        }

        return false;
    }

    // Pose `index` of the stunt clips then the stunt-spin clips, at `rate`.
    private void PlayPose(int index, float rate) {

        int s = index < clips[stunt].Count ? stunt : stuntSpin;
        int i = s == stunt ? index : index - clips[stunt].Count;
        slot = s;
        clip = clips[s][i];
        poseClip = clip;
        holdUntil = 0f;
        anim[clip].speed = rate;
        anim.CrossFade(clip, fade);
    }

    // The pose clip and its loop run at their own speed again for the next user.
    private void ResetSpeed() {

        if (poseClip.Length == 0) {
            return;
        }

        if (anim[poseClip] != null) {
            anim[poseClip].speed = 1f;
        }

        poseClip = "";
    }

    public static int Choose(GameVehicleArcade arcade, bool airTricked) {

        VehicleState s = arcade.state;
        VehicleEvents e = arcade.frameEvents;

        if (((s.flags & (int)VehicleFlags.bailed) | (e.flags & (int)VehicleEventFlags.bailed)) != 0) {
            return fail;
        }

        if ((e.flags & (int)VehicleEventFlags.wallHit) != 0) {
            return arcade.input.steer >= 0f ? hitRight : hitLeft;
        }

        if ((e.flags & (int)VehicleEventFlags.bump) != 0) {
            return obstacle;
        }

        if ((e.flags & (int)VehicleEventFlags.landed) != 0) {
            return jumpDown;
        }

        if ((s.flags & (int)VehicleFlags.grounded) == 0) {
            return AirSlot(s, airTricked);
        }

        if ((s.flags & (int)VehicleFlags.boosting) != 0) {
            return nitro;
        }

        if (s.speed < moveSpeed) {
            return idle;
        }

        bool fast = s.speed > fastFrac * arcade.tuning.maxSpeed;
        float steer = arcade.input.steer;

        if (steer > turnSteer) {
            return fast ? turnFastRight : turnRight;
        }

        if (steer < -turnSteer) {
            return fast ? turnFastLeft : turnLeft;
        }

        return fast ? driveFast : drive;
    }

    // Flips and rolls strike a trick pose, yaw spins a spin. One pose per jump: after it, -1 keeps what is playing
    // (the pose, its hold or jump-up; see Update) until landing.
    public static int AirSlot(VehicleState s, bool airTricked) {

        if (airTricked) {
            return -1;
        }

        float p = System.Math.Abs(s.trickPitchAcc);
        float r = System.Math.Abs(s.trickRollAcc);
        float y = System.Math.Abs(s.trickYawAcc);
        float most = System.Math.Max(p, System.Math.Max(r, y));

        if (most < airTrick) {
            return jumpUp;
        }

        return y >= most ? spin : trick;
    }

    private static bool IsAirPose(int s) {
        return s == trick || s == spin;
    }

    // The air pose has played out and the bike is still up: hold its _loop, or go back to jump-up.
    private void HoldAirPose(float time) {

        string hold = clip + loopSuffix;

        if (anim[hold] != null) {
            clip = hold;
            anim.CrossFade(clip, fade);
            holdUntil = 0f;
            return;
        }

        int up = Resolve(jumpUp);

        if (up >= 0) {
            Play(up, time);
        }
    }

    private int Resolve(int s) {

        while (s >= 0 && clips[s].Count == 0) {
            s = fallback[s];
        }

        return s;
    }

    private void Play(int s, float time, float fadeTime = fade) {

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
        airTricked |= IsAirPose(s);
        anim.CrossFade(clip, fadeTime);
        holdUntil = priority[s] > 0 ? time + anim[clip].length : 0f;
    }
}
#endif
