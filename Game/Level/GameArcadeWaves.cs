using System;

// ZOMBIE WAVES (arcade P5, owner call A4; contexts games/shared/context-arcade-vs-missions.md): the
// arcade play type `waves`. Its level is generated like battle's (GameArcadeLevels, kill goal) with
// the arcade row's zombie enemy preset, and a runner in the app drives the enemy spawner in waves:
//
//   wave n      WaveSize(n) enemies; the spawner holds at most AliveCap(stage) of them at once
//   cleared     every enemy of the wave is down: waveBonusSeconds on the clock, a breakSeconds
//               pause with no spawns, then wave n + 1
//   stage goal  kill GoalKills(stage), which is clearing WavesToClear(stage) waves
//
// The round still ends on the player's death or the clock, as every arcade round does. The numbers
// are plain statics so tuning is one edit; nothing here touches the engine, so it is unit tested.
public static class GameArcadeWaves {

    public static int firstWaveSize = 4;
    public static int waveGrowth = 2;

    // enemies alive at once: aliveBase at stage 0, one more every second stage
    public static int aliveBase = 6;

    // waves a stage asks for: wavesBase at stage 0, one more every second stage
    public static int wavesBase = 2;

    public static double waveBonusSeconds = 15;
    public static float breakSeconds = 3f;

    // spawn cadence while a wave is filling (the app's normal window is 1.8-5.0 s)
    public static float spawnTimeMin = 0.4f;
    public static float spawnTimeMax = 1.0f;

    public static int WaveSize(int wave) {
        return firstWaveSize + waveGrowth * (Math.Max(1, wave) - 1);
    }

    // kills needed to clear waves 1..waves
    public static int KillsThrough(int waves) {
        int kills = 0;
        for (int wave = 1; wave <= waves; wave++) {
            kills += WaveSize(wave);
        }
        return kills;
    }

    public static int WavesToClear(int stage) {
        return wavesBase + GameArcadeLevels.ClampTier(stage) / 2;
    }

    public static int GoalKills(int stage) {
        return KillsThrough(WavesToClear(stage));
    }

    public static int AliveCap(int stage) {
        return aliveBase + GameArcadeLevels.ClampTier(stage) / 2;
    }

    // How many enemies the spawner should hold alive now: enough to finish the wave, never more
    // than the cap, and never fewer than are already alive (nothing despawns).
    public static int SpawnTarget(int waveSize, int killedInWave, int alive, int aliveCap) {

        int remaining = waveSize - Math.Max(0, killedInWave) - Math.Max(0, alive);

        if (remaining <= 0) {
            return Math.Max(0, alive);
        }

        return Math.Max(alive, Math.Min(aliveCap, alive + remaining));
    }
}
