---
name: context-zone-action-indicators
description: An action zone's indicator is built from actionCode at gameInitLevelStart, but placeholder zones are only given their real code a second later by loadLevelActions — so the indicator resolved `indicator-none`, showed nothing, and could never correct itself. Rescue points hit it every time because save is the default branch.
metadata:
  type: repo
  repo: game-lib-games
  path: Game/World/GameZoneActionAsset.cs
---

# Context: why an action zone can end up with an invisible indicator

Shared gameplay layer. Found in Action Bots (2026-08-30, "I didn't see one for the rescue
points") but the ordering is structural, not title-specific.

## The two clocks

An off-screen edge indicator for a zone is created by
`GameZoneActionAsset.LoadPlayerIndicator()`, which passes `actionCode` down to
`BaseGamePlayerIndicator.SetGameIndicatorType()`, which loads
`Resources/…/prefab/ui/indicator-<code>`.

Those two things happen on different clocks:

| when | what |
| --- | --- |
| `Start()` → `Load()` | `actionCode` is whatever the level prefab authored — `none` for a placeholder |
| `gameInitLevelStart` → `OnGameInitLevelStart` → `LoadPlayerIndicator()` | **the indicator is built here** |
| `OnGameLevelItemsLoaded` → `loadLevelActions(1f)` → 1 s later → `Load(zoneType, actionCode, …)` | **the real code is assigned here** |

`gameInitLevelStart` is broadcast at the top of `initLevelFinishCo`. `loadLevelActions` is
a coroutine on a one-second delay hanging off a different event. The indicator is therefore
routinely built *before* the zone knows what it is.

## Why that produced nothing rather than a wrong icon

`SetGameIndicatorType("none")` resolves `indicator-none`. **There is no such prefab**, so
`Resources.Load` returns null and the branch quietly does nothing — the `GamePlayerIndicator`
exists, is parented, has a target and runs, and carries no `GamePlayerIndicatorItem` at all.
Nothing logs.

And it could never recover, because both guards asked the wrong question:

```csharp
// LoadPlayerIndicator
if (gamePlayerIndicator == null) { …build… }        // non-null: never runs again

// SetGameIndicatorType
if (!indicatorObject.Has<GamePlayerIndicatorItem>()) { …load… }  // "is one there", not "is it RIGHT"
```

`Load()` had the re-apply call sitting in it, commented out, exactly where it was needed.

## Why RESCUE, of all of them

`GameController.loadLevelActions` maps attack / defend / repair explicitly and sends
**everything else** to the default branch — commented "Show save/rescue device action by
default". So save/rescue zones are precisely the ones typed on the late path. Zones whose
level prefab authored a real code at `Start()` were fine, which is why some indicators
worked and these did not.

## The fix

- `LoadPlayerIndicator()` returns early while the code is still `none`/empty, instead of
  building a dead indicator, and re-applies the type when it already has one.
- `Load()` calls it at the end, where `actionCode` has just become real.
- `SetGameIndicatorType()` **replaces** a stale item rather than keeping the first, and
  refuses to build anything for an untyped target.

## Prefab coverage — checked, not the problem

| zone code | indicator prefab |
| --- | --- |
| `action-attack` / `action-defend` / `action-repair` / `action-save` | present |
| `action-build` / `action-rescue` | present, unreachable from `loadLevelActions` |
| `action-collect` / `action-kill` | none — but both are folded into `action_save` before they reach an indicator |

## Rules learned

- **A `Resources.Load` miss is silent.** A lookup built from runtime data that can be
  unset needs either a guard against the unset value or a log — otherwise "not configured
  yet" and "configured wrong" both render as nothing.
- **Guard on "is it the RIGHT one", not "is there one".** Any state assembled from data
  that arrives asynchronously must be able to be re-applied, or the first value wins forever.
- **When only SOME instances of a thing are broken, look for two clocks** before looking for
  two configurations.
