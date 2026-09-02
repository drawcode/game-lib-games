---
name: context-audio-boot-volume-swap
description: BaseGameGlobal.InitAudio passed UpdateAudio its two volumes in the wrong order, so every non-editor boot started with music set from the effects setting and effects from music. Guarded by !Application.isEditor, which is exactly why it could only ever be seen on a device.
metadata:
  type: repo
  repo: game-lib-games
  path: Assets/Code/Libs/game-lib-games
  created: 2026-09-02
---

# The boot swapped the two audio volumes, on devices only

`BaseGameGlobal.UpdateAudio(double volumeMusic, double volumeEffects)` — music first. Every
call site in the file passes them that way except the one in `InitAudio`, which passed
`(currentVolumeEffects, currentVolumeMusic)`.

```csharp
currentVolumeEffects = GameProfiles.Current.GetAudioEffectsVolume();
currentVolumeMusic   = GameProfiles.Current.GetAudioMusicVolume();

if (!Application.isEditor) {
    UpdateAudio(currentVolumeEffects, currentVolumeMusic);   // <- swapped
}
```

`UpdateAudio` then writes both values straight back into the profile, so the swap is not just
applied — it is **persisted**. A player whose music was 1.0 and effects 0.5 boots to music 0.5,
effects 1.0, and that is what gets saved.

**Why nobody caught it:** `!Application.isEditor`. The one branch that runs it is the one branch
no editor session ever executes. Found while auditing the audio settings page, not by playing.

## The general shape

A defect inside a platform guard has no editor half to disagree with it. When a call is
editor-excluded, read its arguments rather than trusting that a wrong one would have shown up
by now.

## Related

- `game-lib-games-ui/contexts/context-settings-audio-binding.md` — the settings page found in the same pass
