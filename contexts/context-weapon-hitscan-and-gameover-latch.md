---
name: context-weapon-hitscan-and-gameover-latch
description: Three device reports traced to one root cause each — the laser missed and left beams on screen because a pooled object's Start runs before the launcher has aimed it AND before its use serial is bumped; results never appeared after a second level because isGameOver is never cleared; and the flame thrower did nothing to actors because the particle-damage branch for them was commented out.
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-09-04
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

## Not verified

None of this has been played. The Editor still does not get past content-sync at boot (see
`context-handoff-gameplay-tuning-iter10`), so all three are read off the code. `Assembly-CSharp`
compiles clean.
