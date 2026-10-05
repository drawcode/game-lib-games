---
name: context-animation-speed-cadence
description: BaseGamePlayerControllerAnimation drove legacy normalizedSpeed from a CONSTANT, so the leg cycle ran at one rate whatever the actor's speed was; it now follows speed, with the NavMeshAgent branch excluded because it quantises currentSpeed to 0-or-15. Re-checked after the 2026-09-20 throttle fix dropped this Update from ~127 to ~26 ticks a second — the cadence claims hold, because it SAMPLES speed rather than integrating time — and that distinction is the reason the round clock in the same commit did not.
metadata:
  type: repo
  repo: game-lib-games
  path: Game/Actor/BaseGamePlayerControllerAnimation.cs
  created: 2026-08-30
  updated: 2026-09-20
---

# Context: making the walk/run cycle follow speed

## What it did

For a legacy `Animation` actor, `Update` set

```csharp
actorAnimation[currentAnimationRun].normalizedSpeed  = animationData.runSpeedScale;
actorAnimation[currentAnimationWalk].normalizedSpeed = animationData.walkSpeedScale;
```

`normalizedSpeed` is **cycles per second**, and both scales are constants (set once in
`InitActorAnimation`, then again from the RPG modifiers). So the leg cycle ran at one fixed
cadence while `BaseGamePlayerThirdPersonController.moveSpeed` lerps up from a standstill and
swaps target between `walkSpeed` and `trotSpeed` after `trotAfterSeconds`. The feet slid
against the ground whenever the two disagreed.

## What it does now

```
cadence = authored scale x clamp(currentSpeed / trotSpeed, 0.45, 1.75)
```

`trotSpeed` is the reference because it is the speed the controller actually settles at —
`walkSpeed` only applies for the first `trotAfterSeconds` — so sustained movement looks
exactly as it did and only the ramp in and out changes. Falls back to `walkSpeed` if
`trotSpeed` is unset, and to cadence 1 if neither is.

**Agents are excluded.** The `ContextFollowAgent` / `ContextFollowAgentAttack` /
`ContextRandom` branch overwrites `currentSpeed` with a **0-or-15 stand-in** derived from
`navAgent.velocity.magnitude`, which is not a continuous speed and would give a constant
ratio. `animationData.speedFromController` is set true only where `currentSpeed` comes from
`thirdPersonController.GetSpeed()`, and `GetSpeedCycleScale()` returns 1 otherwise.

`isMecanim` actors were already passing `currentSpeed` into the animator as
`GameDataActionKeys.speed`; whether their controller uses it for a blend tree or a speed
multiplier is a per-title question and was not touched.

## Two dials next to this that look wrong and were left alone

Both change movement feel, and changing feel in the same pass as the animation would make a
playtest unreadable about either.

1. **`walkSpeed = modifiedRunSpeed`** in `BaseGamePlayerController`'s RPG-modifier block is a
   copy-paste: walk ends up FASTER than trot (14-24 against 9-14), so the actor accelerates
   hard and then settles *slower* after half a second.
2. Because `walkSpeed` is therefore the top speed, the run-clip gate `currentSpeed >
   walkSpeed` is essentially never true — **the run clip never plays**. What is on screen is
   always the walk clip. The cadence change covers both clips, so it lands either way.


## Re-checked 2026-09-20: this Update now runs ~26 times a second, not ~127

`Update` sits behind `gameObjectTimer.IsTimerPerf(GameObjectTimerKeys.gameUpdateAll,
IsPlayerControlled ? 1f : 2f)` (`:1553`). Until 2026-09-20 that gate's modifier shrank its interval
as the framerate rose, so on a fast machine it passed on **every** frame — a measured **127
ticks/s**, following the hardware. The modifier is clamped now, so the interval is a floor in real
seconds: at most 30/s for the player, half that for every other actor. **Measured 26/s** at
105–124fps and the same 26/s at 28fps. See
`game-lib-engine/contexts/context-timer-throttle-design.md`.

**Everything above still holds, and that is the point worth keeping.** The cadence code SAMPLES —
each tick reads the controller's current speed and writes `normalizedSpeed`, which is *cycles per
second on the AnimationState* and keeps playing at that rate until the next write. Nothing here
integrates `Time.deltaTime`, so running a fifth as often changes only how *often* the cadence is
retargeted, never how fast the clip plays. The `SubtractTime(Time.deltaTime)` round clock in the
same commit *did* integrate behind the same gate, and it was wrong by a factor of four the moment
the gate was fixed.

**Ask of any gated code: does it sample a value, or accumulate one?** Sampling behind a gate is
merely coarser. Accumulating behind a gate is wrong, and it is wrong by whatever the gate's
skip ratio happens to be — which used to be 1.0 by accident.

The idle re-roll next door (`:603`, `animationItem.last_update + 1f < Time.time`) is the pattern
done right: a wall-clock comparison, so it fires once a second whatever the tick rate is. A frame
counter or a per-tick decrement there would have changed meaning under the same fix.

### What DID change, and has not been looked at

- **The ramp is resampled ~26 times a second instead of ~127.** `currentSpeed` and
  `speedFromController` are assigned inside the gated `Update` (`:1571`–`:1604`), so the
  clamp-to-`[0.45, 1.75]` ratio now steps in ~38 ms increments as the actor accelerates out of a
  standstill and settles into trot. Sustained movement is unaffected — the ratio is constant there.
  Whether the ramp reads as a step rather than a glide is **NOT verified**: no screenshots and no
  video were taken in that pass, and the scripted route does not enable the gameplay cameras.
- The field comment on `speedFromController` (`:171`) says "set per frame by the update". It is set
  per **tick**, and for a non-player that is roughly 13 times a second. Anything reading
  `GetSpeedCycleScale()` from outside a gated path is reading a value up to a tick old.
- `CrossFade`/`Blend` are still called every tick while an actor moves (an anti-pattern on legacy
  `Animation`, noted in `game-lib-engine/contexts/context-spawn-path-costs.md` as measured-cheap).
  There are now ~5x fewer of them per second on a 120fps machine, for free.

### The two dials below are unchanged and still wrong

Re-read 2026-09-20: `walkSpeed = modifiedRunSpeed` is still there
(`BaseGamePlayerController.cs:5779`, against `trotSpeed` at `:5780`), so walk is still the top
speed and the run clip still essentially never plays. Nothing in the iteration-22 combat or timing
work touched movement feel.
