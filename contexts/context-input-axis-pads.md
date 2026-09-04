---
name: context-input-axis-pads
description: GameTouchInputAxis — the virtual pads are FLOATING sticks whose placement object carries its own collider and gets moved to the finger; PointHitTest took only the NEAREST collider, so the HUD's own "Ignore" input shields blocked the right pad; it returns hitPad, which is false while the placement is being dragged; and the isGameRunning gate latches input into the next round.
metadata:
  type: repo
  repo: game-lib-games
  path: Game/Events/GameTouchInputAxis.cs
---

# Context: the virtual input pads (`GameTouchInputAxis`)

Shared gameplay layer, so this applies to every title that mounts `game-lib-games`, not just
Action Bots. Written up after two device faults in that title (2026-08-30) and one
regression introduced while fixing them.

## Shape

One component per axis. `axisName` is an `InputSystemKeys` value (`move`, `attack`,
`main`, …). Per frame, `Update`:

1. bails if `!GameConfigs.isGameRunning`;
2. if a touch or the mouse is down, calls `PointHitTest(point)` for each touch, breaking on
   the first that returns true;
3. otherwise restores the placement;
4. falls through to a keyboard branch for `main`/`move`;
5. `if (!handled) ResetPad()`.

`PointHitTest` raycasts from `collisionCamera` and looks at the nearest collider's
identity — a `GameTouchInputAxisPad` whose owner's `axisName` matches (the pad), or a name
containing `AxisInputPlacement-<axisName>` (the zone).

## Three things about it that are easy to get wrong

### 1. The pads are FLOATING sticks, and the zone moves with them

When `controlsMoveable` is set, a touch on the zone does
`objectPlacement.transform.<position> = worldPoint`. `objectPlacement` **carries the zone's
own collider**, so moving it moves the zone: the stick walks under the thumb. In Action Bots
`AxisInputPlacement-move` is a 236.2 x 226.7 collider and `-attack` is 150 x 150, both on
objects whose own localScale is 1.

Originally there was **no bound at all** — the pad could be dragged anywhere on screen,
including on top of other controls, where its collider wins the raycast and they stop
responding. Now clamped to the zone collider's own half extents around the position captured
in `Start()`, with `placementTravelLimit` to override. If no limit can be derived (no
`BoxCollider` on `objectPlacement`) it moves **unclamped**, deliberately: a control that
refuses to move is a worse failure than an unbounded one.

### 2. `PointHitTest` returns `hitPad`, NOT "did I handle this touch"

The branch that drags the zone is `else if (hitPlacement && !hitPad)`, so the method returns
**false on exactly the frames the stick is being dragged**. Anything keyed off `!handled`
will therefore fire mid-drag. That is a real regression that shipped and had to be fixed:
restoring the placement on `!handled` snapped the stick home every frame and the pads went
dead. `hitPlacementLast` now reports the placement touch separately, and the restore is
gated on `!handled && !placementTouched`.

**Rule: a hit-test boolean says what was hit, not whether the control is idle.** Check every
branch that does real work while returning false before keying anything off the negation.

### 3. Do NOT layer-mask the hit-test raycast

Masking to `collisionCamera.cullingMask` looks obviously right. It is only right if the
camera wired into the component is the one that draws the pad — and if it is not, nothing is
ever hit, there is no error, and the control is silently dead. It was tried and reverted.
Check the mask against the pad's layer in a live round before re-adding it.

## The `isGameRunning` gate is a latch, not a pause

`Update`'s early return freezes `axisInput` and the pad's visual deflection at whatever the
last running frame left. Paired with `BaseGamePlayerThirdPersonController` (whose own Update
is gated the same way, and whose `HandleThirdPersonControllerAxis` ALSO early-returns, so
the axis cannot even be cleared from outside) and `BaseGamePlayerController.impact` (its
decay is behind the gate too), a round that ends with a finger on the stick hands all of it
to the next round. In a title whose player actor is scene-resident — Action Bots — that
means the replay spawns already walking.

Both now release once on the not-running transition; the actor also releases in
`BaseGamePlayerController.Reset()`, which restart flows already reach. The pad's release does
**not** call `ResetPad()`, because that method's move-pad branch is gated on the static
`GameController.touchHandled`, which is itself frozen by the same latch.

## Related

`BaseGameController.handleTouchInputPoint` is the tap-to-move path and is a separate input
route into the same actor; `InputSystem.checkIfAllowedTouch` is what stops a tap on a name
containing `ButtonInput` / `Axis` / `Ignore` / `Pad` from also steering the player.
Title-specific measurements and the device reports are in the Action Bots workspace context
`context-input-pads-and-round-reset`.

## 4. The hit test must step over the HUD's own "Ignore" shields

Device report (2026-09-04): *"something is wrong on the right d-pad, you can't place it or move
to shoot easily unless you start to the left. It seems maybe a touch collision or something
blocking on right/lower side."*

`PointHitTest` used `Physics.Raycast` — the **single nearest** collider. If that collider was not
this axis's pad or its placement, both flags stayed false and the touch did nothing for this pad.

The HUD hangs large colliders named **`Ignore`** over each control cluster. They are deliberate
**input shields**: their job is to stop a tap on the controls falling through to the world, and
`InputSystem` honours exactly that, by name —

```csharp
if (hit.transform.name.Contains("ButtonInput")
    || hit.transform.name.Contains("Axis")
    || hit.transform.name.Contains("Ignore")
    || hit.transform.name.Contains("Pad")) {
    allowedTouch = false;
}
```

`GameTouchInputAxis` never honoured the same convention, so a shield sitting in front of the zone
it is shielding made the control under it unusable.

It bit the **right** pad hardest. Under `HUDTemplate`:

| | left (`InputLeft`) | right (`InputRight`) |
| --- | --- | --- |
| shields | 3, grouped under an `Ignores` node | **3, direct children** |
| placement zone | `AxisInputPlacement-move`, **236.2 x 226.7** | `AxisInputPlacement-attack`, **150 x 150** |
| travel limit (half extents) | ±118 x ±113 | **±75 x ±75** |

The right shields are 376.7x353, 263.1x341.1 and 376.7x353, sitting at positive x / negative y —
the right and lower part of the screen, exactly where the report said. Starting the touch further
left, outside the shields, was the only way to get the stick to come to the thumb.

`PointHitTest` now walks **all** hits (`RaycastNonAlloc` into a reused static buffer, nearest
non-shield wins) and skips anything whose name contains `Ignore`. **Only the shields are skipped**
— a real control still blocks the pad, which is the behaviour that was wanted all along.

### CORRECTION (2026-09-04, measured live): the shields were NOT the cause

The section above reasoned the shields out of the prefab YAML. A live probe then showed
**all six `Ignore` colliders are inactive** — at the menu *and* inside a running round
(`activeInHierarchy=False` on every one). They cannot have blocked anything.

The change is kept: walking the hits and honouring the project's own `Ignore` convention is
correct hygiene, and it makes the pad robust if those shields are ever switched on. **It is not
the fix for the device report.**

### THE ACTUAL CAUSE: the right pad's zone is 85% off the right edge of the screen

Measured in a live round, `Screen = 2137 x 1357`:

| zone | screen x | screen y | on screen? |
| --- | --- | --- | --- |
| `AxisInputPlacement-move` | **144 .. 576** | 48 .. 463 | fully |
| `AxisInputPlacement-attack` | **2097 .. 2371** | 55 .. 330 | **only x 2097..2137 — a ~40px strip** |

`PointHitTest` driven directly across the row confirms it:

```
x=1900:--  x=2000:--  x=2050:--  x=2100:place  x=2120:place  x=2136:place
```

Everything left of ~2100 returns nothing. The pad only answers in the sliver at the screen edge,
which is exactly the "can't place it or move to shoot easily" report.

**Why:** the two zones are anchored as mirrors but their collider centres are not.

| | parent anchor | collider `center.x` | effect |
| --- | --- | --- | --- |
| move | `BottomLeft` | **+30.8** | pushed INWARD, onto the screen |
| attack | `BottomRight` | **+3.4** | pushed OUTWARD, past the corner |

The left pad's centre offset moves it toward the middle of the screen; the right pad's moves it
further right, past its own anchor. The attack zone needs a **negative** `center.x` to mirror the
left pad. It is a one-value change to the collider on `AxisInputPlacement-attack` in
`HUDTemplate.prefab` — authored geometry, so it is a design call and has deliberately been left
for a human, but the correction is roughly `-125` to sit flush inside the right edge, or about
`-200` to give it the same edge margin the move pad has on the left.

### Also found: the travel clamp never engages

`GetPlacementTravelLimit` does `objectPlacement.GetComponent<BoxCollider>()`, but `objectPlacement`
is `ContainerPlacement` and the collider is on its CHILD (`AxisInputPlacement-*`). So it returns
`Vector2.zero` and both pads move **unclamped** — the zone-bounding feature described in section 1
is inert in this HUD. Harmless today, but it is not doing what its comment says.

### Still open: the two zones are not the same size

The attack zone is **less than half the area** of the move zone and its stick may only travel ±75
against the move stick's ±118. That is authored prefab geometry, not code, and it is a design call
— but it is the second half of "you can't place it easily on the right" and it is worth a look on
a device now the shields are out of the way.

### The rule

**A single-hit raycast is the wrong tool in a HUD that stacks colliders.** There is a shield layer
over the controls in this project and its convention is a name. Any hit test that has to see a
control *through* that layer must walk the hits, not take the first one.

