---
name: context-tween-port-map
description: mechanical UITweenerUtil/iTween/LeanTween → TweenUtil call-site rewrite map for plan chunk 1.4
metadata:
  type: reference
  repo: game-lib-games
  plan: plan-ui-migration-uitoolkit
  chunk: "1.3"
  created: 2026-07-12
---

# Tween call-site port map (drives chunk 1.4's mechanical rewrite)

Produced by chunk 1.3. `TweenUtil.cs` (game-lib-engine) was extended with `FadeInObject`,
`FadeOutObject`, `FadeOutObjectNow` (thin shims over the existing `FadeToObject` family) so every
**live** `UITweenerUtil` call shape now has a 1:1 `TweenUtil` equivalent. `UITweenerUtil.cs` itself
was left untouched (it stays the NGUI execution path TweenUtil's `USE_EASING_NGUI` branches call
into, until deleted in a later chunk).

## Headline inventory result

Full-repo grep + agent-verified read of every hit (dead code identified by `//` line comments AND
`/* ... */` block comments, both confirmed by direct file reads, not just the leading-slash
heuristic — several files have entire classes wrapped in a single block comment that a naive grep
misses):

| Family | Live call sites | Dead/commented references |
|---|---|---|
| `UITweenerUtil.*` (external callers, excluding UITweenerUtil.cs itself) | **12** | ~79 |
| `iTween.*` (excluding TweenUtil.cs) | **0** | 33 |
| `LeanTween.*` (excluding TweenUtil.cs) | **6** | 0 |

All 12 live `UITweenerUtil` sites are in `game-lib-games-ui`; `game-lib-engine` and
`game-lib-games` have **zero** live `UITweenerUtil` sites (every reference there is commented out,
almost always immediately next to the already-migrated live `TweenUtil.*` call that replaced it —
e.g. `CameraExtensions.cs:93/117`, `UIUtil.cs` `SetSpriteColor`/`SetLabelColor`/`SetButtonColor`).
`iTween` is **entirely dead** across the whole codebase — including two duplicate, fully
block-commented `GameHUD`/`BaseGameHUD` classes (`Assets/Code/Game/Game/UI/GameHUD.cs:158-1692`,
`Assets/Code/Libs/game-lib-games-ui/Game/UI/Panels/BaseGameHUD.cs:566-2100`) whose iTween calls
look live under a simple `grep -v "^\s*//"` but sit inside one big `/* */`. Always verify block
comments before treating a hit as live.

---

## UITweenerUtil → TweenUtil (live sites only — this is what 1.4's mechanical rewrite touches)

| Source pattern | Target pattern | Sites |
|---|---|---|
| `UITweenerUtil.FadeTo(go, UITweener.Method.EaseInOut, UITweener.Style.PingPong, 2f, 0f, .5f);` (bare stmt, return discarded) | `TweenUtil.FadeToObject(go, .5f, 2f, 0f, true, TweenCoord.world, TweenEaseType.quadEaseInOut, TweenLoopType.pingPong);` | 4: `BaseGameUIPanelVRSettings.cs:166`, `BaseGameUIPanelAR.cs:168`, `BaseGameUIPanelVR.cs:163`, `BaseGameUIPanelARSettings.cs:166` (all byte-identical call, `containerStartObject`) — **see pingPong/LeanTween flag below before porting these 4** |
| `UITweenerUtil.FadeTo(go, UITweener.Method.Linear, UITweener.Style.Once, T, D, 1f);` (alpha=1, fade-in shape) | `TweenUtil.FadeInObject(go, T, D, true, TweenCoord.world, TweenEaseType.linear);` (new 1.3 method; pass `TweenEaseType.linear` explicitly — the method's own default is `quadEaseIn` to match `UITweenerUtil.FadeIn`'s default, but these sites used `Method.Linear` explicitly) — equally valid as plain `TweenUtil.FadeToObject(go, 1f, T, D, true, TweenCoord.world, TweenEaseType.linear, TweenLoopType.once);` | 2: `BaseGameUIPanelStore.cs:434-435` (T=.3f,D=0f), `:456-457` (T=.3f,D=0f) |
| `UITweenerUtil.FadeTo(go, UITweener.Method.Linear, UITweener.Style.Once, T, D, 0f);` (alpha=0, fade-out shape) | `TweenUtil.FadeOutObject(go, T, D, true, TweenCoord.world, TweenEaseType.linear);` or plain `TweenUtil.FadeToObject(go, 0f, T, D, true, TweenCoord.world, TweenEaseType.linear, TweenLoopType.once);` | 2: `BaseGameUIPanelStore.cs:446-447` (T=.2f,D=0f), `:468-469` (T=.2f,D=0f) |
| `UITweenerUtil.MoveTo(go, UITweener.Method.EaseInOut, UITweener.Style.Once, T, D, posTo);` (bare stmt, return discarded; note: `posFrom` NOT passed — this is the 6-arg "posTo only" overload) | `TweenUtil.MoveToObject(go, posTo, T, D, true, TweenCoord.world, TweenEaseType.quadEaseInOut, TweenLoopType.once);` | 4: `BaseGameUIPanelStore.cs:431-432` (T=.3f,D=0f → `Vector3.zero.WithY(0)`), `:443-444` (T=.2f,D=0f → `bottomClosedY`), `:453-454` (T=.3f,D=0f), `:465-466` (T=.2f,D=0f) |

**Enum conversion (all 12 sites use only these values):**

| `UITweener.Method` | `TweenEaseType` |
|---|---|
| `Linear` | `linear` |
| `EaseIn` | `quadEaseIn` |
| `EaseOut` | `quadEaseOut` |
| `EaseInOut` | `quadEaseInOut` |

| `UITweener.Style` | `TweenLoopType` |
|---|---|
| `Once` | `once` |
| `Loop` | `loop` |
| `PingPong` | `pingPong` |

**Return-value usage:** none of the 12 live sites capture the `TweenAlpha`/`TweenPosition` return
value — all are bare fire-and-forget statements. No `.delay`/`.onFinished`/`.eventReceiver`/
`.isFinished` reads anywhere. **Every live site is a straightforward mechanical port** — the only
judgment call is whether to use the new `FadeInObject`/`FadeOutObject` shorthand vs. plain
`FadeToObject`/`MoveToObject`; either is correct, pick `FadeInObject`/`FadeOutObject` for the
alpha 0/1 cases for readability.

### UITweenerUtil methods with zero live external callers (already covered, no action needed)

- `ColorTo` (all overloads) — zero live callers anywhere. `TweenUtil.ColorToObject(lib, go, color, ...)` already routes to `UITweenerUtil.ColorTo` internally for the NGUI path; nothing to port.
- `RotateTo` — zero live callers. `TweenUtil.RotateToObject(lib, go, pos, ...)` already covers it the same way.
- `FadeIn` / `FadeOut` / `FadeOutNow` — zero direct callers of these UITweenerUtil shorthand methods (all real fade-in/out call sites use the general `FadeTo` with explicit alpha, see table above). Added anyway (`TweenUtil.FadeInObject`/`FadeOutObject`/`FadeOutObjectNow`) per plan chunk 1.3 spec, matching UITweenerUtil's own defaults (`EaseIn`, duration 1f) since the design doc lists them as part of UITweenerUtil's covered op set and future call sites may use the shorthand.
- `CameraFade(float, float)` / `CameraColor(Color|Texture2D)` — **zero external callers, and the method bodies inside `UITweenerUtil.cs` are themselves fully commented out** (dead `iTween.CameraFadeTo`/`CameraTexture`/`CameraFadeAdd` stubs, never implemented). Per plan instructions, **skipped** — no `TweenUtil.FadeScreenOverlay`-style facade helper added in 1.3. If a real screen-fade need appears later, it can be added then; nothing today depends on it.
- `ColorToHandler<T>` / `ColorToHandler(GameObject,...)` — zero live callers (one dead reference, `GamePlayerPad.cs:36`, commented). Its `-a-NN` child alpha-cap behavior is already implemented directly in `TweenUtil.FadeToObject`/`ColorToObject` per the 1.1 design doc, so nothing to port even if revived.
- `Begin<T>` / `ResetTween<T>` — internal implementation helpers of `UITweenerUtil` itself (used only by its own `MoveTo`/`ColorTo`/`RotateTo`/`FadeTo` bodies), never called externally. Not part of the public port surface.

---

## iTween.* — cataloged, zero rewrite needed (nothing is live)

Every `iTween.*` reference in the repo (33 total) is dead — either `//`-commented or inside a
`/* ... */` block. No file has a live `iTween` call. Counts below are for completeness /
future-proofing only; **1.4 does not need to touch any of these**.

| Repo folder | Method | Dead count |
|---|---|---|
| Assets/Code/Game | `MoveTo` | 5 |
| Assets/Code/Game | `ShakePosition` | 1 |
| game-lib-engine | `FadeTo` | 9 |
| game-lib-engine | `MoveTo` | 1 |
| game-lib-engine | `ScaleTo` | 1 |
| game-lib-games | `MoveTo` | 4 |
| game-lib-games | `RotateTo` | 4 |
| game-lib-games | `ShakePosition` | 1 |
| game-lib-games | `FadeTo` | 1 |
| game-lib-games-ui | `Stop` | 4 |
| game-lib-games-ui | `MoveTo` | 2 |

Reference mapping (for if any of these are ever revived, or new code copies the pattern):

| Source pattern | Target pattern |
|---|---|
| `iTween.MoveTo(go, iTween.Hash("position", pos, "time", t, "delay", d, "easetype", e, "looptype", l));` | `TweenUtil.MoveToObject(go, pos, t, d, true, TweenCoord.world, <mapped e>, <mapped l>);` |
| `iTween.MoveTo(go, iTween.Hash("x", x, "y", y, "z", z, ...));` (per-axis hash keys, e.g. `GamePlayerObstacle.cs`/`GamePlayerBoundary.cs`/`GamePlayerSpawn.cs`) | Compose `new Vector3(x, y, z)` (or `currentPos + distance` as the dead code does) then `TweenUtil.MoveToObject(go, composedPos, ...)`. **Note:** these 3 dead sites use `"easetype", iTween.EaseType.easeInBounce` — `TweenEaseType` has no bounce ease equivalent (only linear/quadIn/quadOut/quadInOut); if revived, falls back to `quadEaseInOut` or needs a new ease case added upstream (out of scope for 1.3/1.4). |
| `iTween.ScaleTo(go, scale, t);` | `TweenUtil.ScaleToObject(go, scale, t, 0f);` — **but see the `ScaleToObject` no-op/misroute caveat in the LeanTween section below; applies equally here.** |
| `iTween.RotateTo(go, iTween.Hash("y", angle, "time", t, "delay", d, "easetype", "linear", "space", "local"));` | `TweenUtil.RotateToObject(go, Vector3.zero.WithY(angle), t, d, true, TweenCoord.local, TweenEaseType.linear);` |
| `iTween.FadeTo(go, iTween.Hash("alpha", a, "time", t, "delay", d));` / `iTween.FadeTo(go, a, t)` (positional) | `TweenUtil.FadeToObject(go, a, t, d);` |
| `iTween.Stop(go);` | `TweenUtil.Cancel(go);` |
| `iTween.CameraFadeAdd/CameraFadeTo/CameraTexture(...)` | Not covered (see CameraFade note above) — none of these are live either (only inside UITweenerUtil.cs's own dead stub bodies). |

---

## LeanTween.* — 6 live sites (all real, all need review before mechanical swap)

`USE_EASING_LEANTWEEN` is defined for every build target in `ProjectSettings/ProjectSettings.asset`,
so unlike iTween, these actually compile and run today.

| Source (file:line) | Call | Target pattern | Flag |
|---|---|---|---|
| `game-lib-games-ui/Game/UI/UIPanelBase.cs:467` | `LeanTween.cancelAll();` | `TweenUtil.CancelAll();` | Clean mechanical swap — already called out in the 1.1 design doc. |
| `game-lib-games/Game/Actor/BaseGamePlayerController.cs:1219` | `LeanTween.cancel(gamePlayerModelHolder);` | `TweenUtil.Cancel(gamePlayerModelHolder);` | Clean, but see next row — can likely be dropped entirely if folded into `stopCurrent: true` below. |
| `game-lib-games/Game/Actor/BaseGamePlayerController.cs:1221-1224` | `LeanTween.moveLocalY(gamePlayerModelHolder, height, time).setEase(LeanTweenType.easeInOutQuad).setDelay(delay);` | `TweenUtil.MoveToObject(gamePlayerModelHolder, gamePlayerModelHolder.transform.localPosition.WithY(height), time, delay, true, TweenCoord.local, TweenEaseType.quadEaseInOut);` | `stopCurrent: true` already cancels before tweening, so the separate `LeanTween.cancel` call above folds into this one call. Only Y changes — must read current local X/Z via `.transform.localPosition.WithY(...)` since `MoveToObject` takes a full `Vector3`, not a single-axis delta. Also note the dead `#else` fallback (`gamePlayerModelHolder.transform.localPosition.WithY(height);` with no assignment) looks like a pre-existing discarded-return-value no-op bug — unrelated to this port, flagging for awareness only. |
| `game-lib-games/Game/Actor/BaseGamePlayerIndicator.cs:411-413` | `LeanTween.scale(indicatorObject, indicatorObject.transform.localScale.WithX(scaleTo).WithY(scaleTo), currentLateTickTime);` (bare, no chain, called every late-tick while in range, no cancel-before-tween) | `TweenUtil.ScaleToObject(indicatorObject, indicatorObject.transform.localScale.WithX(scaleTo).WithY(scaleTo), currentLateTickTime, 0f, true, TweenCoord.local, TweenEaseType.linear);` | **Non-mechanical — needs manual attention at 1.5, not a safe 1.4 swap.** Per the 1.1 design doc, `ScaleToObjectLeanTween`'s underlying dispatch has a pre-existing bug where the lib-named entry points call `MoveToObject` instead of `ScaleToObject`, and `ScaleToObject(meta,...)`'s NGUI branch is fully commented out (no-op under the current forced-NGUI override). A naive swap to `TweenUtil.ScaleToObject(...)` changes runtime behavior (today's LeanTween scale actually runs; going through the facade's default lib resolution may hit the no-op NGUI branch). Flag and defer to 1.5 when the forced-NGUI override is removed. |
| `game-lib-games/Game/Objects/GameObjectCallToAction.cs:48-51` | `LeanTween.scale(gameObject, Vector3.one * scale, scaleTime).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong().setOnComplete(onScaleComplete);` | Needs the `TweenMeta` overload, not the simple positional one (no `onComplete` param exists on `ScaleToObject(go, pos, time, delay, ...)`): `var meta = TweenUtil.GetMetaDefault(TweenLib.leanTween, gameObject, scaleTime, 0f, true, TweenCoord.world, TweenEaseType.quadEaseInOut, TweenLoopType.pingPong); meta.onComplete = onScaleComplete; TweenUtil.ScaleToObject(meta, Vector3.one * scale);` | **Non-mechanical, two compounding issues:** (1) same `ScaleToObject` no-op/misroute bug as the row above; (2) `TweenUtil.ConvertLibLoopType<LeanTweenType>` maps **both** `TweenLoopType.loop` and `TweenLoopType.pingPong` to `LeanTweenType.clamp` (TweenUtil.cs `ConvertLibLoopType`, LeanTween branch) — a pre-existing bug that means porting `setLoopPingPong()` through `TweenLoopType.pingPong` silently loses ping-pong behavior on the LeanTween path today. `onScaleComplete` also self-recursively re-triggers `AnimateScale()` with mutated `scale`/`scaleTime` fields and has no cancel guard — verify the new path doesn't double-fire or stack. Flag for manual 1.4/1.5 handling, not an automated rewrite. |
| `game-lib-games/Game/Objects/GameObjectCallToAction.cs:70-73` | `LeanTween.rotateLocal(gameObject, Vector3.zero.WithZ(rotate), rotateTime).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong().setOnComplete(onRotateComplete);` | Same `TweenMeta` pattern as above via `TweenUtil.RotateToObject(meta, ...)` (`RotateToObject` does NOT have the scale misroute bug, but does share the `ConvertLibLoopType` pingPong→clamp bug). | **Non-mechanical** — same `ConvertLibLoopType` pingPong bug and self-recursive `onComplete` pattern as the row above (minus the `ScaleToObject` misroute issue, since this is Rotate). Flag for manual handling. |

### Cross-cutting flag: `TweenLoopType.pingPong` is currently broken on the LeanTween path

`TweenUtil.ConvertLibLoopType<LeanTweenType>` (Engine/Utility/TweenUtil.cs) maps:
```
TweenLoopType.loop     → LeanTweenType.clamp
TweenLoopType.pingPong → LeanTweenType.clamp   // bug: same as loop, should differ
```
This predates chunk 1.3 and was **not** touched here (out of scope — 1.3 only adds new methods,
per plan instructions not to change existing lib branches). It matters for two groups of 1.4 work:
1. The 4 identical `UITweenerUtil.FadeTo(..., UITweener.Style.PingPong, ...)` sites
   (`BaseGameUIPanelVRSettings/AR/VR/ARSettings.cs`) — today they call UITweenerUtil directly and
   get correct NGUI ping-pong. After porting to `TweenUtil.FadeToObject(..., TweenLoopType.pingPong)`,
   `FadeToObject`'s lib resolution defaults to `TweenLib.leanTween` for a plain UGUI container
   (Fade only force-routes to NGUI for `UISlicedSprite`/`UISprite`/`UITiledSprite`), so it will hit
   this bug and silently stop ping-ponging. **Verify visually after porting**, or force
   `TweenLib.nguiUITweener` explicitly (`FadeToObject(TweenLib.nguiUITweener, go, ...)`) to
   preserve current behavior until the bug is fixed.
2. The two `GameObjectCallToAction.cs` `setLoopPingPong()` sites above, for the same reason.

This bug is pre-existing and outside chunk 1.3's scope to fix; recorded here so 1.4/1.5 don't
mistake a silent behavior change for a porting mistake.

---

## Sites that CANNOT be cleanly mechanically ported (summary, file:line)

- `game-lib-games/Game/Actor/BaseGamePlayerIndicator.cs:411-413` — `LeanTween.scale` — blocked on the pre-existing `ScaleToObject` misroute/no-op bug (design doc TweenUtil.cs:772-815, :932-949); defer to 1.5.
- `game-lib-games/Game/Objects/GameObjectCallToAction.cs:48-51` — `LeanTween.scale(...).setLoopPingPong().setOnComplete(...)` — needs `TweenMeta` overload (no `onComplete` on the simple overload) + blocked on both the `ScaleToObject` misroute bug and the `ConvertLibLoopType` pingPong→clamp bug; defer to 1.5.
- `game-lib-games/Game/Objects/GameObjectCallToAction.cs:70-73` — `LeanTween.rotateLocal(...).setLoopPingPong().setOnComplete(...)` — needs `TweenMeta` overload + blocked on the `ConvertLibLoopType` pingPong→clamp bug; defer to 1.5.
- `BaseGameUIPanelVRSettings.cs:166`, `BaseGameUIPanelAR.cs:168`, `BaseGameUIPanelVR.cs:163`, `BaseGameUIPanelARSettings.cs:166` — mechanically portable (see table above) but **verify ping-pong fade behavior post-port** due to the same `ConvertLibLoopType` bug, or force `TweenLib.nguiUITweener` explicitly as a safe interim.

Everything else inventoried (all 12 `UITweenerUtil` live sites except none excluded, `UIPanelBase.cs`
`cancelAll`, and the `BaseGamePlayerController.cs` `cancel`+`moveLocalY` pair) is a clean 1:1
mechanical rewrite per the tables above.

## What changed in TweenUtil.cs (chunk 1.3)

Added to `Assets/Code/Libs/game-lib-engine/Engine/Utility/TweenUtil.cs`, immediately after
`FadeToObjectUITweener`, routing through the existing `FadeToObject(go, alpha, ...)` overload
(so the forced-NGUI/sprite-detection lib resolution is unchanged, not bypassed):

- `FadeInObject(GameObject go, float time = 1f, float delay = 1f, bool stopCurrent = true, TweenCoord coord = TweenCoord.world, TweenEaseType easeType = TweenEaseType.quadEaseIn, TweenLoopType loopType = TweenLoopType.once)`
- `FadeOutObject(GameObject go, float time = 1f, float delay = 0f, bool stopCurrent = true, TweenCoord coord = TweenCoord.world, TweenEaseType easeType = TweenEaseType.quadEaseIn, TweenLoopType loopType = TweenLoopType.once)`
- `FadeOutObjectNow(GameObject go, bool stopCurrent = true, TweenCoord coord = TweenCoord.world)`

Defaults (`quadEaseIn`, duration 1f) mirror `UITweenerUtil.FadeIn`/`FadeOut`/`FadeOutNow`'s own
defaults (`UITweener.Method.EaseIn`, duration 1f/1f, 1f/0f, 0f/0f). No screen-fade
(`CameraFade`/`CameraColor`) equivalent was added — zero external callers exist, and the
`UITweenerUtil` implementation itself is an unimplemented stub (dead code), so there is nothing to
preserve. `ColorTo`/`RotateTo`/`MoveTo` needed no new methods — their existing
`TweenUtil.{Color,Rotate,Move}ToObject(lib, go, ...)` overloads already cover every live call
shape 1:1.

## Post-1.4 correction (2026-07-12, ports complete)

The "live inventory" above overstated: `BaseGameUIPanelStore.cs` is one 501-line `/* */`
block (no live subclass anywhere — fully uncompiled), and the Backgrounds/Footer "live"
entries were also inside block comments. **Actual live sites, all ported in 1.4:**

- 5× `UITweenerUtil.FadeTo` → `TweenUtil.FadeToObject` (BaseGameUIPanelAR/ARSettings/VR/VRSettings + BaseGameUIPanelMain, games-ui)
- `LeanTween.cancelAll()` → `TweenUtil.CancelAll()` (UIPanelBase.cs, unconditional — no `#if`)
- `LeanTween.moveLocalY` → explicit `TweenLib.internalEasing` meta (BaseGamePlayerController.GamePlayerModelHolderEase)
- `LeanTween.scale` per-tick → explicit internalEasing, linear ease (BaseGamePlayerIndicator)
- `LeanTween.scale/rotateLocal` pingPong → explicit internalEasing, `stopCurrent=false` (GameObjectCallToAction; the old `setOnComplete` chains never fired on infinite pingPong and were left unwired)

Zero live `UITweenerUtil.` / `LeanTween.` / `iTween.` references remain outside
`TweenUtil.cs`'s `#if` branches and `UITweenerUtil.cs` itself (cross-verified by two
independent full-context audits). 1.6's zero-ref grep gate must count **live** refs only —
~140 dead references in comments/block-comments remain by design and go away with 4.x
file deletions.
