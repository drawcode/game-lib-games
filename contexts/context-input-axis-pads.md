---
name: context-input-axis-pads
description: GameTouchInputAxis — the virtual pads are FLOATING sticks whose placement object carries its own collider and gets moved to the finger; PointHitTest returns hitPad, which is false while the placement is being dragged; and the isGameRunning gate latches input into the next round.
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
