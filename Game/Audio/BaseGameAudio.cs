using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Engine.Data.Json;
using Engine.Utility;
using Engine.Game.Data;
using Engine.Audio;


public class GameAudioData {
    public string code = "";
    public double volume = 0;
}  

public class GameAudioMessages {

    public static string eventAudioVolumeChanged = "audio-volume-changed";
}

public class BaseGameAudioEffects {
    
    // UI sounds
    public static string audio_loop_intro_1 = "audio-loop-intro-1";
	public static string audio_loop_main_1 = "audio-loop-main-1";

	public static string audio_effect_splash = "audio_effect_splash";
	public static string audio_effect_start = "audio_effect_start";
	public static string audio_effect_main = "audio_effect_main";
	
	public static string audio_effect_ui_button_1 = "audio_effect_ui_button_1";
	public static string audio_effect_ui_button_2 = "audio_effect_ui_button_2";
	public static string audio_effect_ui_button_3 = "audio_effect_ui_button_3";
	public static string audio_effect_ui_button_4 = "audio_effect_ui_button_4";
	
	public static string audio_effect_pickup_1 = "audio_effect_pickup_1";
	public static string audio_effect_pickup_2 = "audio_effect_pickup_2";
	public static string audio_effect_pickup_3 = "audio_effect_pickup_3";
	public static string audio_effect_pickup_4 = "audio_effect_pickup_4";
	
	// new
	public static string audio_loop_ui_music = "audio_loop_ui_music";
	public static string audio_loop_logo_music = "audio_loop_logo_music";
	public static string audio_loop_game_1 = "audio_loop_game_1";
	public static string audio_loop_game_2 = "audio_loop_game_2";
	public static string audio_loop_game_3 = "audio_loop_game_3";
	public static string audio_loop_game_4 = "audio_loop_game_4";
		
	// Game sounds
	public static string audio_effect_bike_1 = "audio_effect_bike_1";
}

public enum AudioPlayingState {
	Playing = 0,
	Muted = 1
}

public class BaseGameAudio {

    private static volatile BaseGameAudio instance;
    private static System.Object syncRoot = new System.Object();

    public static AudioPlayingState currentAudioMusicState = AudioPlayingState.Playing;
    public static AudioPlayingState currentAudioEffectsState = AudioPlayingState.Playing;

    public static BaseGameAudio BaseInstance {
        get {
            if(instance == null) {
                lock(syncRoot) {
                    if(instance == null)
                        instance = new BaseGameAudio();
                }
            }
            return instance;
        }
    }

    public BaseGameAudio() {

    }

    public static string GetFileName(string key) {
        return key + ".wav"; // all currently saved as wav for high quality and on SD/persistence so room to.
    }

    public static AudioClip GetShuffledSound(List<AudioClip> clips) {
        if(clips != null) {
            if(clips.Count > 0) {
                clips.Shuffle();
                return clips[0];
            }
        }
        return null;
    }

    //var onSuccess = new Action<AudioClip>( clip =>
    //{
    //	AudioSystem.Instance.PlayAudioClipOneShot(clip);
    //});

    public static bool CheckIfEffectHasCustom(string audioEffectName) {
        bool hasCustomAudio = false;
        bool hasCustomAudioItem = false;
        //string audioEffectName, float volume

        if(GameProfiles.Current.CheckIfAttributeExists(GameProfileAttributes.ATT_CUSTOM_AUDIO)) {
            hasCustomAudio = true;
        }

        CustomPlayerAudio customPlayerAudio;
        CustomPlayerAudioItem customPlayerAudioItem;

        if(hasCustomAudio) {
            customPlayerAudio = GameProfiles.Current.GetCustomAudio();

            customPlayerAudioItem = customPlayerAudio.GetAudioItem(audioEffectName);

            if(customPlayerAudioItem != null) {
                hasCustomAudioItem = customPlayerAudioItem.useCustom;
            }
        }

        return hasCustomAudioItem;
    }

    public static float GetCurrentVolume() {
        return (float)GameProfiles.Current.GetAudioEffectsVolume();
    }

    public static float GetCurrentVolumeAdjust() {
        return GetCurrentVolumeAdjust(1.3f);
    }

    public static float GetCurrentVolumeAdjust(float adjust) {
        return GetCurrentVolume() * adjust;
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName, bool hasCustomAudioItem) {
        float volume = GetCurrentVolumeAdjust();
        PlayCustomOrDefaultEffect(audioEffectName, volume, hasCustomAudioItem);
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName) {

        bool hasCustomAudioItem = GameAudio.CheckIfEffectHasCustom(audioEffectName);
        float volume = GetCurrentVolumeAdjust();
        PlayCustomOrDefaultEffect(audioEffectName, volume, hasCustomAudioItem);
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName, float volume) {

        bool hasCustomAudioItem = GameAudio.CheckIfEffectHasCustom(audioEffectName);
        PlayCustomOrDefaultEffect(audioEffectName, volume, hasCustomAudioItem);
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName, float volume, bool hasCustomAudioItem) {

        if(hasCustomAudioItem) {
            if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeBoosting.ToLower()) {
                GameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioBikeBoosting),
                    GetCurrentVolumeAdjust(2f));
                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioBikeBoosting, 
                //	(float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRacing.ToLower()) {
                GameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioBikeRacing),
                    GetCurrentVolumeAdjust(2f));
                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioBikeRacing, 
                //	(float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRevving.ToLower()) {
                GameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioBikeRevving),
                    GetCurrentVolumeAdjust(2f));
                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioBikeRevving, 
                //	(float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdBoo.ToLower()) {
                GameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioCrowdBoo),
                    GetCurrentVolumeAdjust(2f));
                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioCrowdBoo, 
                //	(float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdCheer.ToLower()) {
                GameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioCrowdCheer),
                    GetCurrentVolumeAdjust(2f));
                GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioCrowdCheer,
                    GetCurrentVolumeAdjust(.25f));
            }
            else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdJump.ToLower()) {
                GameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioCrowdJump),
                    GetCurrentVolumeAdjust(2f));
                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioCrowdJump, 
                //	(float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }

        }
        else {
            PlayDefaultEffect(audioEffectName, volume);
        }
    }

    public static void PlayDefaultEffect(string audioEffectName) {
        GameAudio.PlayCustomOrDefaultEffect(audioEffectName, (float)GameProfiles.Current.GetAudioEffectsVolume());
    }

    public static void PlayDefaultEffect(string audioEffectName, float volume) {

        if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeBoosting.ToLower()) {
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_bike_jump2, (float)GameProfiles.Current.GetAudioEffectsVolume()*.4f);
        }
        else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRacing.ToLower()) {
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_bike_medium_gear);
        }
        else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRevving.ToLower()) {
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_bike_revs_idle);
        }
        else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdBoo.ToLower()) {
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_ohhh_1, (float)GameProfiles.Current.GetAudioEffectsVolume()*.42f);
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_boo_funny, (float)GameProfiles.Current.GetAudioEffectsVolume()*.5f);
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_boo_medium, (float)GameProfiles.Current.GetAudioEffectsVolume()*1.2f);
        }
        else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdCheer.ToLower()) {
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_crowd_cheer_boost_1, (float)GameProfiles.Current.GetAudioEffectsVolume()*.7f);
        }
        else if(audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdJump.ToLower()) {

            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_crowd_cheer_1, (float)GameProfiles.Current.GetAudioEffectsVolume()*.5f);
            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_woohoo, (float)GameProfiles.Current.GetAudioEffectsVolume()*.5f);
        }
        else {
            LogUtil.Log("No sound found with that key:" + audioEffectName);
        }
    }

    public static void PlayEffect(string audioEffectName) {

        //LogUtil.Log("PlayEffect: audioEffectName:" + audioEffectName);

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(audioEffectName,
                (float)GameProfiles.Current.GetAudioEffectsVolume());
    }


    public static void PlayEffectPath(string filename, Transform parentTransform, bool loop) {

        LogUtil.Log("PlayEffectPath: filename:" + filename);

        double volume = GameProfiles.Current.GetAudioEffectsVolume();

        if(AudioSystem.Instance != null) {
            AudioSystem.Instance.PlayEffectPath(filename, parentTransform, loop, (float)volume, true);
        }
    }

    public static void PlayEffectPathDelayed(
        string filename, Transform parentTransform,
        bool loop, float delay, bool is2dSound, float volume) {
        PlayEffectPathDelayed(filename, parentTransform, loop, delay, is2dSound, volume, true);
    }

    public static void PlayEffectPathDelayed(
        string filename, Transform parentTransform,
        bool loop, float delay, bool is2dSound, float volume, bool incrementing) {
        LogUtil.Log("PlayEffectPathDelayed: filename:" + filename);
        if(AudioSystem.Instance != null) {
            AudioSystem.Instance.PlayEffectPathDelayed(
                filename, parentTransform, loop, (float)volume, delay, is2dSound, incrementing);
        }
    }

    public static void PlayEffect(
    string audioEffectName, float volume,
    bool loop, float delay, GameObject parentObject, float spatialBlend = 0.9f) {
        if(parentObject != null) {
            PlayEffect(audioEffectName, volume, loop, delay, parentObject.transform, spatialBlend);
        }
    }

    public static void PlayEffect(
    string audioEffectName, float volume,
    bool loop = false, float delay = 0f, Transform parentTransform = null, float spatialBlend = 0.9f) {
        if(AudioSystem.Instance != null) {
            AudioSystem.Instance.PlayEffect(audioEffectName, volume, loop, delay, parentTransform, spatialBlend);
        }
    }

    public static void PlayEffect(string audioEffectName, bool loop) {

        //LogUtil.Log("PlayEffect: audioEffectName:" + audioEffectName);

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(audioEffectName,
                                            (float)GameProfiles.Current.GetAudioEffectsVolume(), loop);
    }

    public static void PlayEffect(string audioEffectName, double volume, bool loop) {

        //LogUtil.Log("PlayEffect: audioEffectName:" + audioEffectName);

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(audioEffectName, (float)volume, loop);
    }

    public static void PlayEffect(Transform parentTransform, string audioEffectName) {

        //LogUtil.Log("PlayEffect: audioEffectName:" + audioEffectName);

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(parentTransform, audioEffectName,
                false, (float)GameProfiles.Current.GetAudioEffectsVolume());
    }

    public static GameObject PlayEffectObject(Transform parentTransform, string audioEffectName, bool loop) {

        //LogUtil.Log("PlayEffect: audioEffectName:" + audioEffectName);

        if(AudioSystem.Instance != null)
            return AudioSystem.Instance.PlayEffectObject(parentTransform, audioEffectName, loop,
                                            (float)GameProfiles.Current.GetAudioEffectsVolume());

        return null;
    }

    public static void PlayEffect(Transform parentTransform, string audioEffectName, bool loop) {

        //LogUtil.Log("PlayEffect: audioEffectName:" + audioEffectName);

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(parentTransform, audioEffectName, loop,
                (float)GameProfiles.Current.GetAudioEffectsVolume());
    }

    public static void PlayEffect(string audioEffectName, float volume) {
        if(AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(audioEffectName, volume);
    }

    public static void SetProfileAmbienceVolume(double volume) {
        GameProfiles.Current.SetAudioMusicVolume(volume);
        GameAudio.SetAmbienceVolume(volume);
        GameState.SaveProfile();
    }

    public static void SetProfileEffectsVolume(double volume) {
        GameProfiles.Current.SetAudioEffectsVolume(volume);
        GameAudio.SetEffectsVolume(volume);
        GameState.SaveProfile();
    }

    public static void SetAmbienceVolume(double volume) {

        GameAudioController.SetVolume(volume);

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.SetAmbienceVolume(volume);
    }

    public static void SetEffectsVolume(double volume) {

        if(AudioSystem.Instance != null)
            AudioSystem.Instance.SetEffectsVolume(volume);
    }

    public static double GetAmbienceVolume() {
        if(AudioSystem.Instance != null)
            return AudioSystem.Instance.GetAmbienceVolume();
        else
            return 0.0;
    }

    public static double GetEffectsVolume() {
        if(AudioSystem.Instance != null)
            return AudioSystem.Instance.GetEffectsVolume();
        else
            return 0.0;
    }

    //static bool ambienceDelayed = false;

    public static void StartAmbience() {

        //if(ambienceDelayed) {
        GameAudioController.StopGameMusic();
        GameAudioController.PlayUIMusic();
        //    ambienceDelayed = true;
        //}
        //if(AudioSystem.Instance != null)
        //	AudioSystem.Instance.StartAmbience();
    }

    public static void StopAmbience() {

        GameAudioController.StopUIMusic();
        GameAudioController.PlayGameMusic();

        //if(AudioSystem.Instance != null)
        //	AudioSystem.Instance.StopAmbience();
    }

    public static void SetVolumeForRace(bool inRace) {
        //LogUtil.Log("AudioListener SetVolumeForRace:" + inRace);
        if(GameGlobal.Instance != null) {
            if(inRace) {
                AudioListener.volume = (float)(GameProfiles.Current.GetAudioEffectsVolume() * .9);
                //LogUtil.Log("AudioListener setting for race:" + AudioListener.volume);

            }
            else {
                AudioListener.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
                //LogUtil.Log("AudioListener setting for UI:" + AudioListener.volume);
            }
        }
    }

    public static void StartGameLoops() {
        if(AudioSystem.Instance != null)
            AudioSystem.Instance.StartGameLoopsForLaps();
    }

    public static void StartGameLoop(int lap) {

        //LogUtil.Log("StartGameLoop:", " lap:" + lap.ToString());

        if(AudioSystem.Instance != null) {

            //LogUtil.Log("StartGameLoop:", " inst:" + true);

            AudioSystem.Instance.StartGameLoop(lap);
            if(lap > 1) {
                //GamePlayerProgress.Instance.SetAchievement(GameAchievements.ACHIEVE_MIX_IT_UP, true);
            }
        }
    }

    public static void PlayAudioClip(AudioClip clip, bool loop, int increment, float volume) {
        if(AudioSystem.Instance) {
            AudioSystem.Instance.PlayAudioClip(clip, loop, increment, volume);
        }
    }

    public static void PlayAudioClip(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume) {
        if(AudioSystem.Instance) {
            AudioSystem.Instance.PlayAudioClip(pos, parent, clip, loop, increment, volume);
        }
    }

    public static AudioClip LoadLoop(string name) {
        if(AudioSystem.Instance) {
            return AudioSystem.Instance.LoadLoop(name);
        }
        return null;
    }
}