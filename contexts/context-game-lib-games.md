---
name: context-game-lib-games
description: game-lib-games submodule — shared gameplay layer: actors, levels, level editor, items, vehicles, minigames
metadata:
  type: repo
  repo: game-lib-games
  path: .
---

# Context: game-lib-games (submodule)

- **Workspace mount:** `Assets/Code/Libs/game-lib-games`
- **Repo:** git@github.com:drawcode/game-lib-games.git (tracks `dev`; **currently has local modifications** per git status)
- **Purpose:** Shared gameplay layer — game-level abstractions built on game-lib-engine, reused across drawlabs titles. The game's `Game*` classes extend these.

## Structure (`Game/`)
- **Actor/** — base actor/player/enemy classes.
- **Controller/** — base game controllers (the app's `GameController` etc. build on these).
- **Level/** — level system: `GameLevelObjects`, `GameLevelItemObject`, `BaseGameLevelSprite`, and **LevelEditor/** (`GameDraggableEditor.cs`, `GameDraggableLevelItem.cs` — in-game drag/drop level editor).
- **Item/** — game item/pickup system.
- **Gameplay/**, **Minigame/** — game mode and minigame logic.
- **Progression/** — player progression/achievements.
- **Vehicle/** — vehicle gameplay.
- **World/** — world management.
- **Audio/**, **Camera/**, **Effects/**, **Data/**, **Enums/**, **Events/**, **Objects/** — supporting systems.

## Other dirs
`Networks/`, `Tools/`, `Tests/`, `Assets/`, own `DEFINES.md`. Asmdef disabled (`game-lib-games.1asmdef`).

## Topic contexts in this repo
- [`context-input-axis-pads.md`](./context-input-axis-pads.md) — the virtual pads: floating
  sticks whose zone collider moves with them, why `PointHitTest`'s return value is not an
  idle flag, why the hit-test raycast must not be layer-masked, and how the `isGameRunning`
  gate latches input into the next round.
- [`context-animation-speed-cadence.md`](./context-animation-speed-cadence.md) — the legacy
  walk/run cycle ran at a constant `normalizedSpeed`; it now follows speed, with the
  NavMeshAgent branch excluded. Also flags the `walkSpeed = modifiedRunSpeed` copy-paste and
  the run clip that never plays.
- [`context-zone-action-indicators.md`](./context-zone-action-indicators.md) — an action
  zone's off-screen indicator is built from `actionCode` at `gameInitLevelStart`, but a
  placeholder zone is only typed a second later by `loadLevelActions`, so it resolved
  `indicator-none`, drew nothing, and could never correct itself.
- [`context-tween-port-map.md`](./context-tween-port-map.md),
  [`context-progression-runtime-networks.md`](./context-progression-runtime-networks.md).

## Notes
Depends on game-lib-engine. This is the most actively edited submodule (dirty in working tree; `LTDescr.cs`/LeanTween and URP-related churn nearby).
