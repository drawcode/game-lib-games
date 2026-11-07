#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Game.App.BaseApp;

// The starting grid of a race (Action Bikes R2 step 7). A "track-dirt-start-positions" level item marks the line
// the grid sits behind, facing the way the race runs; its data.race_grid block sizes it:
//   "data": {"race_grid": {"rows": 3, "cols": 2, "row_gap": 5, "col_gap": 3, "back": 2}}
// Slot k sits at row k / cols, column k % cols: local x = (col - (cols - 1) / 2) * colGap (right of the facing),
// local z = -(back + row * rowGap) (behind the item). More riders than rows * cols add rows.
public struct GameRaceGridSpec {

    public int rows;
    public int cols;
    public float rowGap;
    public float colGap;
    public float back;

    public static GameRaceGridSpec Defaults() {

        GameRaceGridSpec s = new GameRaceGridSpec();
        s.rows = 3;
        s.cols = 2;
        s.rowGap = 5f;
        s.colGap = 3f;
        s.back = 2f;
        return s;
    }
}

public static class GameRaceGrid {

    public const string gridCode = "track-dirt-start-positions";
    public const string keyRaceGrid = "race_grid";

    public static bool IsGrid(GameLevelItemAsset asset) {
        return asset != null && asset.code == gridCode;
    }

    public static GameRaceGridSpec Read(GameLevelItemAsset asset) {

        GameRaceGridSpec s = GameRaceGridSpec.Defaults();
        object raw;

        if (asset == null || asset.data == null || !asset.data.TryGetValue(keyRaceGrid, out raw)) {
            return s;
        }

        IDictionary block = raw as IDictionary;

        if (block == null) {
            return s;
        }

        double value;

        if (GameRaceNodes.TryNumber(block, "rows", out value) && value >= 1) {
            s.rows = (int)Math.Round(value);
        }

        if (GameRaceNodes.TryNumber(block, "cols", out value) && value >= 1) {
            s.cols = (int)Math.Round(value);
        }

        if (GameRaceNodes.TryNumber(block, "row_gap", out value) && value > 0) {
            s.rowGap = (float)value;
        }

        if (GameRaceNodes.TryNumber(block, "col_gap", out value) && value >= 0) {
            s.colGap = (float)value;
        }

        if (GameRaceNodes.TryNumber(block, "back", out value) && value >= 0) {
            s.back = (float)value;
        }

        return s;
    }

    // World positions for outPos[0..outPos.Length); the vehicles face `facing`.
    public static void Slots(Vector3 origin, Quaternion facing, GameRaceGridSpec s, Vector3[] outPos) {

        int cols = Mathf.Max(1, s.cols);

        for (int k = 0; k < outPos.Length; k++) {
            int row = k / cols;
            int col = k % cols;
            float x = (col - (cols - 1) * 0.5f) * s.colGap;
            float z = -(s.back + row * s.rowGap);
            outPos[k] = origin + facing * new Vector3(x, 0f, z);
        }
    }

    // The player's slot from the race seed: an integer hash, never UnityEngine.Random.
    public static int PickPlayerSlot(int count, uint seed) {

        if (count <= 1) {
            return 0;
        }

        uint h = seed * 2654435761u + 0x9E3779B9u;
        h ^= h >> 16;
        h *= 0x85EBCA6Bu;
        h ^= h >> 13;
        h *= 0xC2B2AE35u;
        h ^= h >> 16;
        return (int)(h % (uint)count);
    }
}
#endif
