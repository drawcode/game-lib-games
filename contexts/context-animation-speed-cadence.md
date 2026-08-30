---
name: context-animation-speed-cadence
description: BaseGamePlayerControllerAnimation drove legacy normalizedSpeed from a CONSTANT, so the leg cycle ran at one rate whatever the actor's speed was; it now follows speed, with the NavMeshAgent branch excluded because it quantises currentSpeed to 0-or-15.
metadata:
  type: repo
  repo: game-lib-games
  path: Game/Actor/BaseGamePlayerControllerAnimation.cs
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
