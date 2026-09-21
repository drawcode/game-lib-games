---
name: context-weapon-hitscan-and-gameover-latch
description: How a hit is landed, scored and ended in this lib, and every way it has silently failed — the pooled laser that cast on the PREVIOUS life's aim, the isGameOver flag nothing cleared, the particle damage branch that was commented out, the melee CastAttack that had never run once in normal play (five independent defects, each of which looked like the fix on its own), and the app-mode branch that leaves a round with no way to end. Carries the current scoring/death rules and the two mis-attributions still open.
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-09-04
  updated: 2026-09-20
---

# Three device reports, three latched or mis-ordered states

## 1. The laser missed, and its beams stayed on screen

Two separate faults, both from the same fact: **`Start` on a pooled object runs at a moment when
nothing about the current shot is set up yet.**

`ObjectPool.instantiate` re-sends `Start` **synchronously**, inside `createPooled`, inside
`GameObjectHelper.CreateGameObject`. At that instant:

- the launcher has not assigned `TargetTag`, `gamePlayerController`, or the spread-adjusted
  forward for this shot, and
- `CreateGameObject` has not yet bumped the object's `useSerial`.

### Why the beams stayed

`GameRayShoot.Start` scheduled its own recycle (`DestroyGameObject(gameObject, LifeTime)`).
`destroyPooled` captures the use serial **at the moment it is called** and the stale-recycle guard
drops the timer if the serial has since moved on. The serial moved on one line later. So every
recycle a pooled object scheduled from its own `Start` was discarded as somebody else's, and the
object was never returned to the pool — it stayed in the world, visible, for the session.

**First life was fine**, because Unity sends that `Start` a frame later, after the bump. Only the
**second and later** uses leaked, which is why one shot looked correct.

Fixed in the engine: the bump now happens in `ObjectPool.instantiate` immediately **before** the
`Start` re-send, and `CreateGameObject` only bumps an object the pool freshly instantiated.

**This was not laser-specific.** Any pooled object that schedules its own delayed recycle from
`Start` had the same leak.

### Why it missed

`GameRayShoot` did all its work in `Start`, so it cast along the **previous life's** aim and handed
the **previous life's** `TargetTag` to the explosion that applies the damage. A ray recycled from an
enemy's shot went looking for the player.

`GameDamageBase.OnLaunched` exists for exactly this and says so in its own doc comment — the
launcher calls it once everything is wired for this shot. `GameDamage` was moved over when its
`IgnoreCollision` pairing hit the same problem; `GameRayShoot` never was. It fires from `OnLaunched`
now.

**`GameWeaponLauncher` had to change too:** it set `bullet.transform.forward = direction` **after**
calling `OnLaunched`. Harmless for a projectile that flies and reads its transform on a later frame;
wrong for a **hitscan** one that casts immediately. Aim first, then launch.

Three more faults in the same method, all fixed:

- the miss branch set `AimPoint = transform.forward * Range` — a **direction** scaled by 10000, not
  a position — so a missed beam's far end landed near the world origin, not in front of the gun;
- the miss branch called `CreateGameObject(Explosion, …)` with no null check while the hit branch
  had one, so a ray prefab with no Explosion threw *after* the LineRenderer was fetched but *before*
  the beam was drawn or the recycle scheduled;
- only `TargetTag` was copied to the explosion, not `gamePlayerController` (no friendly-fire
  distinction) or `Damage` (the explosion silently used its own authored value).

### The rule

**On a pooled object, `Start` means "the pool handed me out", not "I am ready".** Anything that
depends on who fired it, where it is aimed, or on its own identity for this life belongs in
`OnLaunched`. And anything a pooled object schedules for itself must be scheduled after the pool has
marked the new life.

## 2. Results never appeared after a second level

`checkForGameOver` does all of its work inside `if (!isGameOver)`. Nothing cleared the flag:

- `resetRuntimeData()` is the only thing that sets it false;
- it is reached only from `reset()`;
- both `reset()` calls on the level-start path — in `prepareGame` and in `startGame` — are
  **commented out**;
- `restartGame()` does call it.

So the first round to end set it true and it stayed true for the process. Restarting a level always
worked (hence never reported); playing **two levels back to back** did not — the second played to
its end and then never asked for results. No game over, no panel, the worlds backer left on screen.

Cleared in `prepareGame`, which every level start funnels through. One line deliberately: calling
the full `reset()` would also tear down the level actors and swap the runtime data mid-prepare,
which is presumably why it was commented out.

**Same family as `isgamerunning-gate-latches-state`**: a flag set on a transition and never released
on the way back. Worth grepping for others — a `bool` that is only ever assigned `true` outside its
declaration is the shape.

## 3. The flame thrower did nothing to enemies

It has no projectile object — it is a particle system, so it never reaches the launcher's damage
path at all. Its only route is `GameDamageManager.OnParticleCollision`, which had:

```csharp
if (gamePlayerController != null) {
    //gamePlayerController.OnParticleCollision(other);   // the whole actor case, commented out
}
else {
    if (other.name.Contains("projectile-")) {
        float projectilePower = 3;    // todo lookup projectile and power
        ApplyDamage(projectilePower);
    }
}
```

So a particle weapon could only ever damage **destructible props**, for a flat 3, with no weapon
term and no distance term.

Both cases go through `ApplyDamage` now (it already routes an actor hit through
`gamePlayerController.Hit` and applies the friendly-fire reduction), and the damage scales with
distance from the emitter: `particleDamageCloseMultiplier` (4x) at point blank falling linearly to
1x at `particleDamageCloseRange` (6). Dials on `GameDamageDirector`, tunable in one place, like
`GameIndicatorConfigs`.

**These numbers are a starting point, not a measurement.** Nothing has been played.

## Also seen, not changed

`GameDamageDirector.AllowRayShoot` is a **global static** 0.05 s gate shared by every ray shooter in
the scene, and its getter mutates state when it returns true. With more than one laser firing, most
beams destroy themselves at spawn. That is plausibly a second reason the laser felt weak, but it is
a deliberate-looking throttle and changing it changes the weapon's feel, so it was left alone.

`GameRayShoot`'s raycast has no layer mask and no origin offset — it can hit the shooter's own
collider. Untouched for the same reason: worth checking on a device before changing.

## Not verified (as of 2026-09-04)

Sections 1–3 have never been played. At the time the Editor could not get past content-sync at
boot, so all three were read off the code; `Assembly-CSharp` compiled clean.

**That boot blocker is long gone** — from iteration 13 onward a scripted session drives a live
round and reads state back (`workspace context-gameplay-probe-the-level-not-the-boot`,
`skill unity-gameplay-testing`). Nothing has gone back to re-verify 1–3 in play, but "it cannot be
played" is no longer the reason. Section 4 below WAS measured live.


## 4. The melee ray attack had never run once (2026-09-20, measured live)

`BaseGamePlayerController.CastAttack` (`:4359`) is the non-projectile attack: a `Physics.RaycastAll`
from the actor, `ScoreAttack()` + `Hit(1f)` + `InputAttack()` on whatever it resolves to. It had
never landed a hit in normal play, and getting it to needed **five independent fixes**. Each one,
on its own, looked like *the* fix — and after each one it still did nothing.

1. **The guard was not negated.** `if (controllerReady) { return; }`. The file has **17**
   `controllerReady` guards and this was the only one written without the `!`, so the method
   returned precisely when the actor was ready. A single character, invisible in review because the
   line is the same shape as sixteen correct ones.
2. **The interaction guard was inverted, and it `return`ed.** `AllowControllerInteraction(other)` is
   TRUE when the pair MAY interact — every other call site negates it. This one did not, so the ray
   could only ever act on a pair that is **forbidden** to interact (a sidekick, a dead actor, one
   entering or leaving). Worse, it `return`ed rather than `continue`d, abandoning every remaining
   hit in the list.
3. **The direction was zero.** `thirdPersonController.aimingDirection` is `(0,0,0)` whenever the
   player is not actively aiming — measured live — and `RaycastAll` with a zero direction returns
   nothing. It falls back to `transform.forward` (`:4389`).
4. **The origin was at the actor's feet.** `transform.position` sits at y ≈ 0.08 while actor
   colliders span roughly **y 0.1 – 4.3**, so the ray passed underneath every target: measured
   **0 hits at +0.0 against a target 2.58 m away, 2 hits at +1.0**. Now
   `transform.position + Vector3.up * attackHeightOffset`, chest height, default 1.2 (`:146`).
5. **One actor scored twice.** See the dedupe rule below.

Verified live after all five: **+10 for a hit, +35 with the kill, +20 for two distinct actors in
one cast**, −0.833 health per landed hit, scenery ignored. `attackDistance` is 50 on the live
prefab (the field default at `:143` is 10).

### The dedupe rule: one actor carries several colliders that all resolve to it

An actor has **two enabled colliders that both resolve to the same controller** —
`GamePlayerObject` and `GamePlayerCollider` — plus `Helmet`, `Facemask` and `Main` on the model. A
single `RaycastAll` through one torso returns several of them, so a single hit scored **+20 where
+10 was intended**, and `ScoreAttack` broadcasts `gameActionScore` and writes `SetStatScore`, so
the doubling reached the HUD **and** the saved stat. The damage half was hidden: `Hit()` has its
own `intervalHit` rate limit (`:161`) and swallowed the second call, which is exactly why only the
score showed it.

`castAttackHandled` (`:150`) is a reused `List<GamePlayerController>`, cleared per cast, and a
controller already in it is skipped (`:4429`).

**Whenever you widen a hit test, dedupe per cast by the RESOLVED entity, not by collider.** This
cuts both ways: the old code's name filter (`hitObject.name.IndexOf("Game") > -1`) dropped a hit on
a `Helmet`, `Facemask` or `Main` collider *before* anything tried to resolve it, so whether an
attack counted depended on which of an actor's colliders the ray happened to reach first. The name
filter is gone — `GetController` answers null for scenery, so it is the only test that belongs
there — and `GetController` (`:1807`) now falls back to
`GameController.GetGamePlayerControllerObject`, resolving **upward** through the collision
component, because a collider child has no controller beneath it.

The same name-list-versus-component fault was live in `BaseGameController`: the lists in
`getGamePlayerControllerObject` (`:741`) and `hasGamePlayerControllerObject` (`:790`) **did not
agree with each other**, and neither matched the prefabs. A `Helmet` or `Facemask` hit passed
`has...` and then resolved to `null` in `get...`, so `BaseGamePlayerItem.OnCollisionEnter` saw true
followed by null and **the item silently failed to collect**; a `Main` collider was missed by both.
Both resolve by component now.

## 5. Scoring and death, as they now stand

Before this pass the only combat score in the game paid the player **for being hit**.

| event | now | before |
| --- | --- | --- |
| player takes damage | nothing (`Hit`, `:3639`) | `ProgressScore(2 * power)` |
| player lands a hit on an enemy | nothing from `Hit` | `ProgressScore(-1)` — see below |
| ray attack lands | `ScoreAttack()` → 10 (`:4460`) | never ran |
| enemy dies | `scoreKill` 25 (`:177`), awarded in `Die()` (`:4186`) | nothing |
| surviving | 1 a second (unchanged) | — |

**`ProgressScore` is not a private counter.** `ProgressScore(val)` (`:4774`) adds to the caller's
`runtimeData.score`, broadcasts `GameMessages.gameActionScore` **and** writes
`GamePlayerProgress.SetStatScore` — all player-wide. That is why the old `ProgressScore(-1)` on the
*enemy* being hit actually docked the **player** a point, and why `ScoreAttack(double)` (`:4464`)
had to stop writing `runtimeData.score` directly: a direct write skipped the broadcast and the
stat, so ray score reached neither the HUD nor the save.

**Death now uses authored data.** The agent branch was
`if (runtimeData.hitCount > UnityEngine.Random.Range(2, 4))` — the **int** overload, so 2 or 3,
**re-drawn on every gated frame**. An agent's toughness changed frame to frame and no authored
value could reach it. It reads `runtimeData.hitLimit` (`:6345`), seeded for non-players from
`agentHitLimitDefault` = 3 (`:171`) in `LoadCharacter` — so an enemy dies on the **4th** hit,
deterministically. The seeding is written as `if (IsAgentControlled || IsSidekickControlled)`,
tested positively rather than as `!IsPlayerControlled`, because an actor whose state has not been
set yet must not be handed the agent limit and then become the player.

### Two mis-attributions still OPEN

- **`Die()` credits the player a kill for a sidekick's death.** The branch is `IsPlayerControlled`
  / else, so *any* non-player death — including the player's own sidekick — increments
  `GameController.CurrentGamePlayerController.runtimeData.kills`, writes `SetStatKills(1f)` and now
  also pays `scoreKill` 25. The new score award inherits the existing bug rather than adding one,
  but it makes it visible on the HUD.
- **`ScoreAttack` credits the ATTACKER.** It routes through the attacker's own `ProgressScore`, so
  an armed agent would score for itself into the player-wide stat. Inert today only because
  `agentWeaponsEnabled` (`:185`) is off — the agent-weapon path is a prototype and has never been
  exercised.

## 6. The app-mode branch that leaves a round with no way to end

`checkForGameOver` (`BaseGameController.cs:3424`) decides game over inside a four-way
`if/else if` over `AppModes.Instance` — arcade, challenge, mission, training. **If none of the four
matches, nothing sets `gameOverMode`** except `runtimeData.outOfBounds`: neither the expired timer
nor the player's health reaching zero ends the round.

Two ways that happened:

- The content data ships the **plural** `app-mode-game-missions` while
  `AppModeMeta.appModeGameMission` was the singular, so `isAppModeGameMission` never matched. Fixed
  additively in the engine (`BaseAppModes.cs:29`, `:103`) — both spellings are accepted rather than
  renaming a constant another product's data may still use.
- A **scripted Play button carries no `app_content_state`**, so `AppModes.Current.code` stays `""`
  and all four tests miss. This is a probe artefact, but it is also why
  **`runtimeData.timeExpired` has never been observed ending a round**: every round measured so far
  ended on player death. A round ending on the clock needs a real app mode to be set.

**Round length is real seconds now** (`SubtractRoundTimeElapsed`, `:3406`), which changes the game
as much as any of the above: a 90 s round genuinely lasts 90 s, where it used to run roughly twice
that. `defaultLevelTime` (`:85`) is the dial if that now feels short. See
`game-lib-engine/contexts/context-timer-throttle-design.md` for why the clock was wrong.

## Related (added 2026-09-20)

- `game-lib-engine/contexts/context-timer-throttle-design.md` — the gate the countdown sits behind
- `context-per-frame-actor-costs.md` — the performance half of the same commit (`c18933c`)
- `context-enemy-pooling-damage-routing.md` (workspace, action-bots) — how a hit reaches an actor
