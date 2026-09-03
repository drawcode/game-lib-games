---
name: context-per-frame-actor-costs
description: A sweep of every Update/FixedUpdate/LateUpdate on the gameplay path — the per-frame component lookups, layer-name lookups, string-building log calls and SendMessage reflection that ran once a frame PER ACTOR, plus two logic faults found alongside (attract force that could never run, and indicator cleanup parked behind the round gate).
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-09-03
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

None of this has been measured live or on a device — the Editor session this was written in does
not get past the content-sync stage of boot, so there is no before/after profile capture. The
changes are read off the code. `Assembly-CSharp` compiles clean and the console is clear.

## Related

- `game-lib-engine/contexts/context-renderer-visibility-allocations.md` — the allocation half
- `context-weapon-audio-particles-gc.md` — the earlier per-shot pass, same class of problem
- workspace `context-profile-save-cost.md` — where the Debug.Log stack-trace cost was measured
