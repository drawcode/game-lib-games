---
name: context-per-frame-actor-costs
description: A sweep of every Update/FixedUpdate/LateUpdate on the gameplay path — the per-frame component lookups, layer-name lookups, string-building log calls and SendMessage reflection that ran once a frame PER ACTOR, plus two logic faults found alongside (attract force that could never run, and indicator cleanup parked behind the round gate). Now carries the FIRST live profiler capture: the item collect path, which this sweep missed, was 63% of a frame's GC allocation on its own.
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-09-03
  updated: 2026-09-05
---

# What the actor update path was paying for every frame

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

Plus the big one, in the engine: `IsRenderersVisibleByCamera` allocated on every call and is
reached from two of these paths. See
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
