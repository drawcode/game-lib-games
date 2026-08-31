---
name: context-offscreen-indicator-dials
description: The two dials the off-screen edge indicators read — GameIndicatorConfigs.scale (ships .9, the 10% shrink) and edgeBorderScale (.5, halving the prefab's authored 90-unit margin) — with the live measurements that show why a flat border insets the top and bottom dots twice as far as the sides. Also the defect in 11c021e's stale-indicator replace path.
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-08-31
---

# The indicator size and edge dials

`cff01a8`. Device report: the off-screen edge indicators are too big, and *some* of them sit
too far in from the edge. Both are now dials on `Engine.Game.App.BaseApp.GameIndicatorConfigs`,
which `BaseGamePlayerIndicator` reads directly. Placement mechanism itself is unchanged — see
the workspace `context-offscreen-indicators.md` for how the container space works.

## SIZE — `GameIndicatorConfigs.scale`, ships `.9`

`ScaleIndicator` multiplies its distance-derived size by the dial, **after** the `[.6, 4]`
clamp. That is deliberate: the Settings: Controls slider has to be able to take the dots below
the `.6` far-size floor, which is the point of a "smaller" setting. The profile read is
clamped to `[.5, 1.5]` at its own end, so the product still cannot run away.

Read off the **config**, not the profile. This runs for every indicator on every late tick and
a profile lookup walks an attribute dictionary — and it keeps this lib from having to know
the app's `GameProfiles` exists. The panel pushes profile → config; nothing else copies.

Measured in a live round: far dots land at **0.540 = 0.6 x 0.9** exactly. Near ones at
1.152 and 1.083 (1.28 and 1.203 before the dial).

## EDGE — `edgeBorderScale`, ships `.5`

```csharp
float border = clampBorderSize * GameIndicatorConfigs.edgeBorderScale;
```

It **multiplies** the prefab's authored `clampBorderSize` rather than replacing it, so the
authored margin stays the one place that number is written down and the two shipped indicator
prefabs keep one margin between them. `.5` takes the authored 90 to 45.

### Why only SOME of them read as far from the edge

This is the useful part. The visible area, measured in the container's own units, is
**+/-692.5 x +/-320** (see the workspace context for how that is measured — it is *not*
derivable from the screen aspect). A single flat border applies the same absolute inset to
both axes, so 90 units is:

| axis | half-extent | inset as a fraction |
| --- | --- | --- |
| x | 692.5 | **13%** |
| y | 320 | **28%** |

The top and bottom dots were held more than twice as far in as the side dots. Anyone reading
the code sees one symmetric-looking number and no asymmetry at all.

Live after the change: x at **±648** (= 692.5 − 45, was 602.5), y at **275**
(= 320 − 45, was 230).

**If you want them tighter still, move `edgeBorderScale`, not the prefab.** The floor is the
dot's own half-size — the position is the indicator's centre, so a margin below that lets a
large near-distance dot overhang the edge.

## The defect in `11c021e`'s replace path

`11c021e` (iter 8, committed uncompiled) added a branch to `SetGameIndicatorType` that
destroys a stale indicator item before building the right one. **It compiles, and it could
never have worked.**

```
GameObjectHelper.DestroyGameObject(item.gameObject, GameConfigs.usePooledIndicators);
gamePlayerIndicatorItem = null;
...
if (!indicatorObject.Has<GamePlayerIndicatorItem>()) {   // <-- still true
```

`usePooledIndicators` is **false** (`BaseGameConfigs`), so `DestroyGameObject` is a plain
`GameObject.Destroy` and the corpse survives to the end of the frame; the pooled path only
deactivates. `Has<T>` searches children with `GetComponentsInChildren<T>(true)` — **including
inactive**. So the guard saw the thing that had just been destroyed, reported "there is
already one", and the method **fell off the end with no else branch**: the zone was left with
**no indicator at all**, strictly worse than the stale one it replaced.

Fixed by unparenting before destroying.

**The general shape, worth carrying:** `Destroy` is deferred and `Has<>`/`GetComponentsInChildren`
sees inactive objects, so "destroy then re-query the parent" is a lie within the same frame.
Detach first, or track the field rather than re-querying.

## Related

- workspace `context-offscreen-indicators.md` — the container-space placement fix these sit on
- `context-zone-action-indicators.md` — the late action code, from the same commit
- `game-lib-engine/contexts/context-ui-control-change-events.md` — where the dials live and why
