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

## Notes
Depends on game-lib-engine. This is the most actively edited submodule (dirty in working tree; `LTDescr.cs`/LeanTween and URP-related churn nearby).
