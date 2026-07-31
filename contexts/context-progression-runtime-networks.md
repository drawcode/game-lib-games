---
name: context-progression-runtime-networks
description: game-lib-games runtime for Progression/RPG — BaseGamePlayerProgress (stat increment, composite keys, achievement filter engine, auto leaderboard submit) and the native platform layer (GameNetworks router → GameNetworkUnity Unity Social API → Apple GameCenter / Google Play Games; SocialNetworks FB/Twitter). Reports using the data-driven networks[] IDs defined in game-lib-engine. Referenced by context-profile-progression-rpg-shared.
metadata:
  type: reference
  repo: game-lib-games
  created: 2026-07-21
---

# game-lib-games — Progression runtime + native networks

Runtime half of the shared systems: turns gameplay events into stat writes,
awards achievements, and reports scores/achievements to Apple GameCenter + Google
Play Games. Data model + catalogs live in game-lib-engine
([[context-profile-progression-data]]); overview + generator + easy-setup in
[[context-profile-progression-rpg-shared]].

## `BaseGamePlayerProgress` (title `GamePlayerProgress`, singleton `Instance`)
`Game/Progression/BaseGamePlayerProgress.cs` (~3148 lines). The heart of tracking.

- **Composite stat keys** — `BaseGameStatisticCodes` builds keys from a base code +
  dimensions (content-state, pack, action, type, tracker, level) with prefixes
  `total-`/`high-`/`low-`/`action-`; one event fans out into many stat writes.
  `BaseGameAchievementCodes.formatAchievementCode` replaces `-`→`_` (platform-safe).
- **Write dispatcher** `SetStatisticValue(key,float)` reads the stat definition's
  `order` → `SetStatAccumulate` (add) / `SetStatHighPoint` (max, `ascending`) /
  `SetStatLowPoint` (min, `descending`). Underlying write funnels through
  `SetStatisticValue(bool sendToGameverses,key,val)` → writes
  `GameProfileStatistics.Current`, then (unless `GameConfigs.isGameRunning`) casts
  `long` and calls `GameNetworks.SendScore(key,long)` when >0 — **every stat write
  is an automatic leaderboard submission when a board shares that code.**
- **Title-facing helpers** (~lines 2498–3147): ~50 debounced `static SetStatX(double)`
  — evaded, deaths, hits, hitsReceived, kills, saves, ammo, attacks, defends,
  builds, repairs, score(s), xp, coins/coinsPickup/coinsEarned, boosts, spins,
  cuts, items(code), specials, letters, words, custom(code), highScore(s). Call
  these from gameplay.
- **Achievement filter engine** `ProcessPackRuntimeAchievements(packCode)` walks
  `achievement.data.filters[]`: statisticSingle (any key passes), statisticSet (all
  AND), statisticAll (all pack/state actions), statisticLike (substring), Compare/
  achievementSet (stubbed). `CheckStatCondition(key,StatEqualityTypeEnum,value)`
  (`>=`,`>`,`<`,`<=`,`==`). On pass → `SetAchievement(key,true)`: gates on not-
  already-complete, writes profile bool, `GameNetworks.SendAchievement(key,true)`,
  broadcasts a toast-queue message.
- **Leaderboard push** `ProcessProgressLeaderboards()` iterates all leaderboards,
  reads the matching stat, `SendScore(code,value)` when >0.
- **Lifecycle** `ProcessProgressPackChange` / `…AppContentStateChange` /
  `ProcessProgressRuntimeAchievements` run the filter pass + `GameState.SaveProfile()`.
- Place scoring: `GetPointsByPlace` (1st=10,2nd=5,3rd=3,4th=2,5th=1), `GetPrettyPlace`.
- `gameCenterLeaderboards` list + `IsGameCenterLeaderboard(key)` filter which local
  stats map to native boards. Name remap `FilterThirdpartyNetworkLeaderboard`
  (`-`→`_`, special cases). `time-played` scores ×100 before submit.

## Native platform (`Networks/`)
- **`GameNetworks : GameObjectBehavior`** (singleton router, ~2577 lines). Selected
  backend by compile symbol: **`GAMENETWORK_USE_UNITY`** (active) vs
  `GAMENETWORK_USE_PRIME31` (compiled-out legacy). `GameNetworkType`:
  `game-network-apple-gamecenter`, `game-network-google-playservices`, (Prime31-only)
  amazon-gamecircle, samsung, and `game-network-gameverses`. `currentNetwork` =
  Play Games on Android, GameCenter on iOS.
  - `LoadNetwork/InitNetwork` (Android: `PlayGamesPlatform.Activate()` then login),
    `LoginNetwork`, `ShowAchievementsOrLogin`, `ShowLeaderboardsOrLogin`.
  - **`SendScore(code,long)`** → `GameLeaderboards.Instance.GetById(code)` → for each
    `data.networks[]` → `reportScore(type, networkCode, value)`.
  - **`SendAchievement(code,bool)`** → `GameAchievements.Instance.GetById(code)` → for
    each `data.networks[]` → `reportAchievement(type, networkCode, progress%)`.
  - `CheckAchievementsState()` two-way sync (local↔remote); `GetNetworkUsername` +
    3-sec `InvokeRepeating` detects GameCenter user switch → `GameState.ChangeUser`.
  - Facebook Scores block under `USE_GAME_LIB_GAMEVERSES` (Graph API) — see
    [[context-community-social]].
- **`GameNetworkUnity : MonoBehaviour`** — the genuine **Unity Social API** wrapper
  (`using UnityEngine.SocialPlatforms`): `Social.localUser.Authenticate`,
  `Social.ReportScore`, `Social.ReportProgress`, `Social.ShowLeaderboardUI`,
  `Social.ShowAchievementsUI`, `Social.LoadScores/LoadAchievements/
  LoadAchievementDescriptions`. Board/achievement IDs pass straight through as
  strings, so `Social.Active` (Play Games on Android after Activate, GameCenter on
  iOS) is driven by identical calls.
- **`SocialNetworks`** — Facebook/Twitter share+login only (Prime31-gated, mostly
  no-op unless symbols set); not GameCenter/Play Games.

## What a title supplies
- Per-definition platform IDs inside each `data.networks[]` (GameCenter + Play
  Games IDs) — authored in the engine's `data/*.json` catalogs.
- `AppConfigs.gameNetworkGooglePlayGameServicesClientId` (Prime31 init path).
- Info.plist GameCenter capability (Unity iOS) + Play Games app-id in
  AndroidManifest (GooglePlayGames plugin). No plist/manifest authored in C#.

## UI wiring points
`BaseUIController` GameCenter buttons → `ShowLeaderboards/AchievementsOrLogin`;
`BaseGameGlobal:270` iOS startup `loadNetwork`; `BaseGameUIPanelFooter`
`showButtonGameNetworkGameCenter()`.
