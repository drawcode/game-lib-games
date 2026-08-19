using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Per-frame cache of tag sweeps for seeking weapons.
///
/// GameObject.FindGameObjectsWithTag walks every tagged object and allocates a fresh
/// array on each call. Both the launcher's Seeker block and every in-flight seeking
/// missile ran that sweep in their own Update, so the cost scaled with
/// (missiles in flight x target tags) every single frame. The results are identical
/// within a frame, so they are shared here and thrown away when the frame changes.
///
/// The array is only valid for the frame it was taken in. Callers already null-check
/// each entry, which covers targets destroyed mid-frame; an object deactivated into a
/// pool mid-frame stays listed until the next frame, which is harmless for target
/// selection.
/// </summary>
public static class GameWeaponTargets {

    static readonly Dictionary<string, GameObject[]> targetsByTag
        = new Dictionary<string, GameObject[]>();

    static readonly GameObject[] emptyTargets = new GameObject[0];

    static int cachedFrame = -1;

    public static GameObject[] GetByTag(string tag) {

        if (string.IsNullOrEmpty(tag)) {
            return emptyTargets;
        }

        if (cachedFrame != Time.frameCount) {
            targetsByTag.Clear();
            cachedFrame = Time.frameCount;
        }

        GameObject[] targets;

        if (targetsByTag.TryGetValue(tag, out targets)) {
            return targets;
        }

        targets = GameObject.FindGameObjectsWithTag(tag);

        targetsByTag[tag] = targets;

        return targets;
    }

    public static void Clear() {

        targetsByTag.Clear();

        cachedFrame = -1;
    }
}
