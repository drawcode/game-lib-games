---
name: context-per-frame-actor-costs
description: What the actor update path allocates, across three passes — the per-frame component and layer-name lookups and SendMessage reflection of the original sweep, the item collect path that was 63% of a frame on its own, and the 2026-09-20 pass that took a gameplay frame from 10,225 to ~8,100 B and corrected the actor counts. READ THE HEADER FIRST — until that pass every "per frame" claim here was literally true because the IsTimerPerf gate was broken, and actors now tick 26/s, so no B/frame number from an earlier pass is comparable with a later one.
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-09-03
  updated: 2026-09-20
---

# What the actor update path was paying for every frame

> **The cadence under all of this changed on 2026-09-20.** Every `Update` named below sits behind
> `gameObjectTimer.IsTimerPerf(...)`, and until that date the gate's modifier shrank its interval as
> the framerate rose, so on a fast machine **every gate passed every frame** and "once a frame per
> actor" was the literal truth. The modifier is clamped now: actors tick a measured **26/s** instead
> of **127/s**, and the rate no longer follows the hardware. Two consequences for this file — the
> per-second cost of every row below fell by roughly 5x on a 120fps machine without any of them
> being touched again, and **a B/frame figure measured before that date cannot be compared with one
> measured after it.** See `game-lib-engine/contexts/context-timer-throttle-design.md`.


A scan of every `Update` / `FixedUpdate` / `LateUpdate` under `Game/` for work that does not
change between frames. Everything below ran **once a frame, per actor**.

| where | was | now |
| --- | --- | --- |
| `ActorShadow.Update`, `BaseGameActorShadow.LateUpdate` | `LayerMask.NameToLayer(string)` — a string lookup into the layer table | resolved once |
| `ActorShadow.Update` | `Camera.main` | cached on the component |
| `BaseGamePlayerThirdPersonController.Update` | `GetComponent<CharacterController>()` | resolved once, re-resolved only if null |
| `BaseGamePlayerControllerAnimation.Update` | `GetComponent<Animation>()` | keyed on the actor object, re-resolved on swap |
| `BaseGamePlayerControllerAnimation.Update` | run/walk `SendMessage("SyncAnimation", …)` | bound once into an `Action<string>`, skipped when nothing handles it |
| `GameVehicleDriveInput.Update` | two **unconditional** `Debug.Log` calls | behind `LogUtil.loggingEnabled` |
| `BaseGameActor.Update` | two interpolated `LogUtil.Log` calls | behind `LogUtil.loggingEnabled` |
| `GameWeaponLauncher.Update` | `AimObject.tag == …` | `CompareTag` |

Plus the big ones, in the engine: `IsRenderersVisibleByCamera` allocated on every call and is
reached from two of these paths, and `SetParticleSystemStartColor` — driven off the player tint
from `BaseGamePlayerController.cs:1193` — ran a `GetComponent` that always missed plus two
`GetComponentsInChildren` arrays per call, ~2 KB/frame. Both in
`game-lib-engine/contexts/context-renderer-visibility-allocations.md`.

## Three rules this pass is worth remembering for

**A runtime `if (!enabled) return` inside a logger does not make the call free.** `LogUtil.Log`
checks `loggingEnabled` *inside* the method, so `LogUtil.Log("x:" + value)` builds and discards the
string in a shipped build with logging off. The guard has to be at the CALL SITE for a per-frame
log. `Debug.Log` is worse again — it captures a managed stack trace on every call, the same cost
`context-profile-save-cost` measured at 34 ms.

**A component cache keyed on nothing is wrong when the thing can be swapped.** The animation code's
per-frame `GetComponent<Animation>()` had one virtue: it always matched the current actor, and the
actor model IS swapped (customisation, pooled reuse). Caching it flat would have broken that. Key
the cache on the object it was resolved from and compare references.

**`SendMessage` with `DontRequireReceiver` usually means there is no receiver.** The run/walk pair
fires every frame while an actor moves, and the handlers (`NetworkSyncAnimation`,
`GameNetworkPlayerContainer`) only exist on networked actors — so single-player paid a reflection
lookup per frame to find nothing. Probe once, bind a delegate, and skip when empty. Bind a
**delegate**, not a `MethodInfo`: `Invoke` boxes its arguments into a fresh `object[]` per call,
which just trades the lookup for an allocation.

## Two logic faults found in the same sweep

### Attract force could never have run

```csharp
if (GameDraggableEditor.isEditing && GameConfigs.isGameRunning) {
```

`isGameRunning` is `GameController.IsGameRunning && !isUIRunning`, and the level editor is a UI —
so `isEditing` implies `!isGameRunning` and the conjunction is never true. Every object flagged
`attractProjectiles` / `attractGamePlayers` has silently done nothing. Now `!isEditing &&
isGameRunning`.

It hid because the feature is opt-in and off by default: nothing looked broken, there was just
never anything to see.

Three faults were waiting inside it, all reachable the moment it started running:
`offset / offset.sqrMagnitude` divides by zero for a collider sitting exactly on the attractor
(Unity does not reject the NaN — the rigidbody's position becomes NaN and the object is gone for
good); `GetComponents(typeof(T))` allocated a `Component[]` per collider per physics step just to
test emptiness; and `Physics.OverlapSphere` allocated its result every step.

### Indicator cleanup was parked behind the round gate

`BaseGamePlayerIndicator.LateUpdate` early-returned on `!isGameRunning` **above** its
`target == null -> DestroyMe()` cleanup. Because `isGameRunning` is false for **any panel opened
mid-round**, not just at the end of one, an indicator whose target died while a panel was up kept
pointing at the corpse until the panel closed.

The general shape, and the third time it has bitten this project (see the
`isgamerunning-gate-latches-state` rule): **a gate at the top of an Update covers cleanup as well
as behaviour.** Put it directly above the code that must hold still — here, the code that MOVES
the indicator — not above the whole method.

## Checked and found clean

- Messenger `AddListener`/`RemoveListener` balance across every gameplay class: no leaks, no
  asymmetric `OnEnable`/`OnDisable` pairings.
- No allocating physics queries left in any Update loop.

## Two things that LOOK like findings and are not

- `ShowRaycasts.Update` / `ShowControllerRaycasts.Update` call `transform.Find(...)` every frame,
  but both bodies are inside `if (!Application.isPlaying)` — editor authoring helpers, never a
  runtime cost. Left alone.
- `GameWeaponLauncher.Update` mentions `Camera.main`, but only inside a `CurrentCamera == null`
  guard. Already cached.

## Not verified

The table above is still read off the code — **none of those individual rows has a before/after
number**. What HAS since been measured, with the profiler in a live round, is the item path
below; it was not in the table at all, and it turned out to dominate everything in it.

`Assembly-CSharp` compiles clean and the console is clear.

## MEASURED, 2026-09-05 — the item path, which this sweep missed entirely

First actual profiler capture of a live round (iteration 10 could not get one). The sweep above
looked at actor Updates and never looked at `GamePlayerItem`, where the real cost was.

**Before**, median frame in a running round, 84 active items:

| sample | bytes | share of frame |
| --- | --- | --- |
| frame total | 39,068 | — |
| `GamePlayerItem.Update()` | 22,916 | 63.1% |
| └ `GetComponentNullErrorMessage` | 21,556 | 59.3% |
| `AnimationEasing.Update()` | 3,408 | 9.4% |

100% of frames exceeded an 8 KB GC budget.

**The cause.** `GetCollectReach` asked the player actor for a `CharacterController` on every call,
and it is called **once per item per frame**. The actor root carries none
(`hasCharacterController=False` measured live), so all 84 lookups failed every frame and the
`characterRadius` fallback is what was actually used. A failing `GetComponent` also builds its own
error string.

Micro-benchmarked in the Editor, 8,400 calls: **537 bytes and 2.5 µs per failing lookup.**

**After** (`bb239bf`, resolve once per player instead of once per item per frame):
`GamePlayerItem.Update()` **no longer appears among the frame's top allocators at all**, and the
share of frames over the 8 KB budget fell from 100% to 60%.

### Two cautions on those numbers

- The after-capture ran with 8–16 items, not 84, so the **median figures are not like-for-like**.
  The claim that rests on evidence is the marker's *disappearance* plus the per-call benchmark,
  which is linear in item count — not the ratio of the two medians.
- **The allocation half is Editor/development-build only** — `GetComponentNullErrorMessage` is a
  diagnostic string. This was NOT verified against a release iOS build. What is removed on every
  platform is the lookup itself: 83 of 84 native component searches per frame.

### The lesson

`GC.GetTotalMemory` in an unfocused Editor is not a usable per-frame allocation measure. It read
1.4 MB/frame on the results screen; the profiler's median for the same period was **11.9 KB**. The
difference is EditorLoop overhead divided by a low player framerate. Two separate readings from it
were discarded this session before the profiler settled it — one where a collection landed
mid-window and turned the delta negative. **Use the profiler's frame summary, not a heap delta.**

## Related

- `game-lib-engine/contexts/context-renderer-visibility-allocations.md` — the allocation half
- `context-weapon-audio-particles-gc.md` — the earlier per-shot pass, same class of problem
- workspace `context-profile-save-cost.md` — where the Debug.Log stack-trace cost was measured


## MEASURED, 2026-09-20 — the third pass, and the gate that was hiding it

Live round, level 1-1, 6 actors, Editor on desktop at 105–124fps.

| | before | after |
| --- | --- | --- |
| whole frame GC | 10,225 B/frame | **~8,100 B/frame** (median) |
| `GameController.Update` | 772 B/call | **0** |
| `GamePlayerController.Update` | ~2,085 B/call (player, measured before its gate) | ~26 B/call |
| `GamePlayerControllerAnimation.Update` | 438 B/call (2026-09-16) | ~70–78 B/call |
| actor tick rate | 127/s, following the framerate | 26/s, framerate-independent |

The two largest sources were both in the engine and are written up there: the particle tint
(~2 KB/frame) and `InputSystem.updateTouchLaunch` (748 B/frame, and NOT throttled — it runs above
`GameController.Update`'s gate). The rest of this pass is in `game-lib-games`, commit `c18933c`.

### The fixes, and the class each one belongs to

**`collision.contacts` allocates a fresh `ContactPoint[]` on every read**, and
`BaseGamePlayerController.OnCollisionEnter` read it twice per collision event — once to test
emptiness, once to iterate. `contactCount` / `GetContact(i)` is the non-allocating pair
(`BaseGamePlayerController.cs:2741`). Exactly the same property-that-allocates trap as
`Input.touches`.

**`foreach` over a `Transform` boxes a non-generic `IEnumerator`.** `Transform` implements only
`IEnumerable`, so every `foreach (Transform t in someTransform)` allocates. Three of them ran per
frame per actor in the aim/jump-settle code; `childCount` / `GetChild(i)` does not allocate
(`:6372`, `:6394`, `:6521`). One of the three only ever touched the first child — the loop body
ended in an unconditional `break` — which is worth noticing before optimising a loop: it was not a
loop.

**`GameObject.tag` marshals a new string per read**; `GameDamage`'s impact test read four
(`GameDamage.cs:221`). Three became `CompareTag`. The fourth compares against **another object's**
tag, and there is no allocation-free overload for that, so it stays. Same class as
`Transform.name`, which `BaseGameController.getGamePlayerControllerObject` (`:741`) and
`hasGamePlayerControllerObject` (`:790`) read repeatedly — read once into a local.

**A count used as a budget must count the population it claims to.** `characterActorsCount` was
`levelActorsContainerObject.transform.childCount`, which is not a character count at all: the
actors container also parents spawned **items**, and pooled characters are returned by
**deactivating** them and left as children. Measured live it read **14 against 6 real
`GamePlayerController`s**. All three counts now come off one subtree walk
(`refreshLevelCharacterCounts`, `BaseGameController.cs:434`), verified exact at 6/2/4 against a
direct walk.

**Two getters read back to back is one walk, not two.** `BaseAIController.handleUpdate` reads the
enemy count and the sidekick count on consecutive lines, and its `Update` has **no timer gate**, so
the old getters ran two full `GetComponentsInChildren<GamePlayerController>()` sweeps over every
actor in the level, every frame. The walk is memoised per `Time.frameCount` (`:434`) — and
invalidated wholesale in the level teardown (`:2045`), because a throttled cache that survives a
level change hands the next round the previous one's numbers.

**`refreshLevelItemCounts` is throttled to 1 s** (`itemsCountInterval`, `:518`), not per frame. Its
only consumer is the item director's spawn gate, which decides every 5–15 s — a per-frame walk was
~60x more often than anything could read a new answer. **A time throttle was chosen over keeping a
running total on purpose:** spawn, collect, pool return and `DestroyChildren` would all have to
stay in step with an incremental count forever, and a single missed decrement silently closes the
spawn gate for the rest of the round. Re-walking is self-correcting, and it is now rare.

### The lesson worth keeping: an empty result is not a cache

`CheckVisibility` (`BaseGamePlayerController.cs:6146`) refilled its renderer list whenever the list
came back **empty**:

```csharp
if (currentControllerData.renderers.Count == 0) {   // "we haven't looked yet"
```

For an actor that genuinely has **no `SkinnedMeshRenderer`** — a droid, or any actor before its
character model has finished loading — empty is the correct, permanent answer, so the sentinel read
"not looked yet" forever and `GetComponentsInChildren` re-ran on every gated frame for the life of
the actor. **A negative result needs its own key.** The cache is now keyed on the loaded model
object (`:6172`), the same idiom as `actorAnimationResolvedFor` in
`BaseGamePlayerControllerAnimation`, with the list *instance* in the key as well — because
`UpdateCharacterStates` hands out a fresh `GamePlayerControllerData` (and so a new, empty list) on
every character load, which a pooled model object reused for the same prefab would otherwise hide.

### Still open

- **~8.1 KB/frame is unattributed.** Two known Editor-only lines inside it: `GUIUtility.BeginGUI`
  (2.9 KB) and `GetComponentNullErrorMessage` (2.4 KB).
- That `GetComponentNullErrorMessage` is now attributed to **`ActorShadow.Update`,
  `GamePlayerIndicator.LateUpdate` and `GamePlayerCollision.OnCollisionEnter`** — the same missing-
  `GetComponent` defect as the item path above, in three more places, and a reminder that the first
  sweep at the top of this file fixed `ActorShadow`'s `Camera.main` and layer lookup without ever
  noticing the miss underneath them. Same fix as the engine's particle cache.
- A one-off **261 KB frame** on actor spawn, inside `InitControlsCo`.
- The residual ~2.1 B/call in the animation update (open since 2026-09-16).

**NOT verified:** anything on a device. All of the above is the Editor on desktop, which
*overstates* every allocation number that comes from a failed `GetComponent`.

## Related (added 2026-09-20)

- `game-lib-engine/contexts/context-timer-throttle-design.md` — the gate, and why the tick rate moved
- `game-lib-engine/contexts/context-input-touch-launch-costs.md` — the un-throttled input path
- `context-weapon-hitscan-and-gameover-latch.md` — the combat half of the same commit
