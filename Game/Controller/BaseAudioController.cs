using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Events;

public class GameAudioDataItem : GameDataObject {

    public List<GameDataSound> items;

    public GameAudioDataItem() {
        items = new List<GameDataSound>();
    }
}

public class BaseAudioController : GameObjectBehavior {

    private static GameAudioController _instance = null;

    public static GameAudioController Instance {
        get {
            if(!_instance) {

                // check if an object is already available in the scene graph
                _instance = FindObjectOfType(typeof(GameAudioController)) as GameAudioController;

                // nope, create a new one
                if(!_instance) {
                    var obj = new GameObject("_GameAudioController");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<GameAudioController>();

                }
            }

            return _instance;
        }
    }

    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public Dictionary<string, GameAudioDataItem> items;

    void Awake() {

        // Init();
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {

        items = new Dictionary<string, GameAudioDataItem>();

        //GameGlobal.Instance.UpdateAudio(
        //    GameProfiles.Current.GetAudioMusicVolume(),
        //    GameProfiles.Current.GetAudioEffectsVolume());
    }

    public void OnEnable() {

        Messenger<GameAudioData>.AddListener(GameAudioMessages.eventAudioVolumeChanged, OnAudioVolumeChangeEventHandler);
    }

    public void OnDisable() {

        Messenger<GameAudioData>.RemoveListener(GameAudioMessages.eventAudioVolumeChanged, OnAudioVolumeChangeEventHandler);

    }

    void OnAudioVolumeChangeEventHandler(GameAudioData gameAudioData) {

        if(gameAudioData.code == BaseDataObjectKeys.effects) {
            GameAudio.SetProfileEffectsVolume(gameAudioData.volume);
        }
        else if(gameAudioData.code == BaseDataObjectKeys.music) {
            GameAudio.SetProfileAmbienceVolume(gameAudioData.volume);
        }
    }

    // MUSIC

    public GameObject lastUIIntro;
    public GameObject lastUILoop;
    public GameObject lastGameLoop;
    public GameObject currentUIIntro;
    public GameObject currentUILoop;
    public GameObject currentGameLoop;

    public bool isUIMusicPlaying {
        get {
            if(currentUILoop == null) {
                return false;
            }

            if(currentUIIntro == null) {
                return false;
            }

            if(currentUILoop.IsAudioSourcePlaying()) {
                return true;
            }

            if(currentUIIntro.IsAudioSourcePlaying()) {
                return true;
            }

            return false;
        }
    }

    public bool isGameMusicPlaying {
        get {
            if(currentGameLoop == null) {
                return false;
            }

            if(currentGameLoop.IsAudioSourcePlaying()) {
                return true;
            }

            return false;
        }
    }

    public bool isMusicPlaying {
        get {
            if(isGameMusicPlaying && isUIMusicPlaying) {
                return true;
            }

            return false;
        }
    }

    // volume

    public virtual void setVolume(double volume) {
        setVolumeGame(volume);
        setVolumeUI(volume);
    }

    public virtual void setVolumeGame(double volume) {
        if(currentGameLoop != null) {
            AudioSource audioSource = currentGameLoop.GetComponent<AudioSource>();
            if(audioSource != null) {
                audioSource.volume = (float)volume;
            }
        }
    }

    public virtual void setVolumeUI(double volume) {
        if(currentUILoop != null) {
            AudioSource audioSource = currentUILoop.GetComponent<AudioSource>();
            if(audioSource != null) {
                audioSource.volume = (float)volume;
            }
        }

        if(currentUIIntro != null) {
            AudioSource audioSource = currentUIIntro.GetComponent<AudioSource>();
            if(audioSource != null) {
                audioSource.volume = (float)volume;
            }
        }
    }


    // ui play ui

    public virtual void playMusic() {
        if(GameConfigs.isUIRunning) {
            playUIMusic();
        }
        else {
            playGameMusicLoop();
        }
    }

    public virtual void playUIMusic() {

        if(isUIMusicPlaying) {
            return;
        }

        if(isGameMusicPlaying) {
            stopGameMusic();
        }

        StartCoroutine(playUIMusicCo());
    }

    public virtual IEnumerator playUIMusicCo() {

        if(!GameConfigs.isUIRunning) {
            yield break;
        }

        playUIMusicIntro();

        float waitTime = 0f;

        AudioSource audioSource = currentUIIntro.Get<AudioSource>();

        if(audioSource != null) {

            waitTime = audioSource.clip == null ? 0 : audioSource.clip.length;
        }

        yield return new WaitForSeconds(waitTime);

        playUIMusicLoop();
    }

    public virtual void stopUIMusic() {

        if(!isUIMusicPlaying) {
            return;
        }

        StartCoroutine(stopUIMusicCo());
    }

    public virtual IEnumerator stopUIMusicCo() {

        yield return new WaitForEndOfFrame();

        stopUIMusicIntro();
        stopUIMusicLoop();
    }

    public virtual void stopUIMusicIntro() {
        if(currentUIIntro.IsAudioSourcePlaying()) {
            currentUIIntro.StopSounds();
            currentUIIntro.GetComponent<AudioSource>().FadeOut(1.5f);
        }
    }

    public virtual void stopUIMusicLoop() {
        if(currentUILoop.IsAudioSourcePlaying()) {
            currentUILoop.GetComponent<AudioSource>().FadeOut(1.5f);
        }
    }

    // game music


    public virtual void playGameMusic() {

        if(isGameMusicPlaying) {
            return;
        }

        if(isUIMusicPlaying) {
            stopUIMusic();
        }

        StartCoroutine(playGameMusicCo());
    }

    public virtual IEnumerator playGameMusicCo() {

        if(GameConfigs.isUIRunning) {
            yield break;
        }

        playGameMusicLoop();
    }

    public virtual void stopGameMusic() {

        if(!isGameMusicPlaying) {
            return;
        }

        StartCoroutine(stopGameMusicCo());
    }

    public virtual IEnumerator stopGameMusicCo() {

        yield return new WaitForEndOfFrame();

        stopGameMusicLoop();
    }

    public virtual void stopGameMusicLoop() {
        if(currentGameLoop.IsAudioSourcePlaying()) {
            currentGameLoop.GetComponent<AudioSource>().FadeOut(1.3f);
        }
    }

    // ui intro

    public virtual void playUIMusicIntro() {

        if(!GameConfigs.isUIRunning) {
            return;
        }

        foreach(GameDataSound sound in GetSounds(GameDataActionKeys.music_ui_intro)) {

            bool handled = false;

            foreach(GameObjectAudio objectAudio in UnityObjectUtil.FindObjects<GameObjectAudio>()) {

                if(objectAudio.type == GameDataActionKeys.music_ui_intro) {

                    lastUIIntro = currentUIIntro;
                    currentUIIntro = objectAudio.gameObject;
                    handled = true;
                }
            }

            if(!handled) {
                lastUIIntro = currentUIIntro;

                currentUIIntro = AudioSystem.Instance.PrepareFromResources(
                    sound.type, sound.code, false, 0f);
            }

            break;
        }

        if(currentUIIntro != null) {
            currentUIIntro.GetComponent<AudioSource>().FadeIn(
                (float)GameProfiles.Current.GetAudioMusicVolume(), 2f);
        }
    }

    // ui loops

    public virtual void playUIMusicLoop() {

        if(!GameConfigs.isUIRunning) {
            return;
        }

        foreach(GameDataSound sound in GetSounds(GameDataActionKeys.music_ui_loop)) {

            bool handled = false;

            foreach(GameObjectAudio objectAudio in UnityObjectUtil.FindObjects<GameObjectAudio>()) {

                if(objectAudio.type == GameDataActionKeys.music_ui_loop) {

                    lastUILoop = currentUILoop;
                    currentUILoop = objectAudio.gameObject;
                    handled = true;
                }
            }

            if(!handled) {
                lastUILoop = currentUILoop;

                currentUILoop = AudioSystem.Instance.PrepareFromResources(
                    sound.type, sound.code, true, 0f);
            }

            break;
        }

        if(currentUILoop != null) {
            currentUILoop.GetComponent<AudioSource>().FadeIn(
                (float)GameProfiles.Current.GetAudioMusicVolume(), 1.7f);
        }
    }

    // game loops
    public virtual void playGameMusicLoop() {

        if(GameConfigs.isUIRunning) {
            return;
        }

        foreach(GameDataSound sound in GetSounds(GameDataActionKeys.music_game)) {

            foreach(GameObjectAudio objectAudio in UnityObjectUtil.FindObjects<GameObjectAudio>()) {

                if(objectAudio.type == GameDataActionKeys.music_game) {
                    currentGameLoop = objectAudio.gameObject;
                    lastGameLoop = currentGameLoop;
                }
                else {
                    currentGameLoop = AudioSystem.Instance.PrepareFromResources(
                        sound.type, sound.code, true, 0f);
                    lastGameLoop = currentGameLoop;
                }
            }
            break;
        }

        if(currentGameLoop != null) {
            currentGameLoop.GetComponent<AudioSource>().FadeIn(
                (float)GameProfiles.Current.GetAudioMusicVolume(), 2f);
        }
    }

    // SOUNDS

    // scores

    public virtual void playSoundScores() {

        playSoundType(GameDataActionKeys.scores);
    }

    // goals

    public virtual void playSoundGoalRange1() {

        playSoundType(GameDataActionKeys.goal_range_1);
    }

    public virtual void playSoundGoalRange2() {

        playSoundType(GameDataActionKeys.goal_range_2);
    }

    public virtual void playSoundGoalRange3() {

        playSoundType(GameDataActionKeys.goal_range_3);
    }

    public virtual void playSoundGoalRange4() {

        playSoundType(GameDataActionKeys.goal_range_4);
    }

    // level end

    public virtual void playSoundLevelStart() {

        playSoundType(GameDataActionKeys.level_start);
    }

    public virtual void playSoundLevelEnd() {

        playSoundType(GameDataActionKeys.level_end);
    }

    // player end

    public virtual void playSoundPlayerStart() {

        playSoundType(GameDataActionKeys.player_start);
    }

    public virtual void playSoundPlayerEnd() {

        playSoundType(GameDataActionKeys.player_end);
    }

    // player out of bounds

    public virtual void playSoundPlayerOutOfBounds() {

        playSoundType(GameDataActionKeys.player_out_of_bounds);
    }

    // player action good

    public virtual void playSoundPlayerActionGood() {

        playSoundType(GameDataActionKeys.player_action_good);
    }

    public virtual void playSoundPlayerActionBad() {

        playSoundType(GameDataActionKeys.player_action_bad);
    }

    // types

    public Dictionary<string, float> playSoundTypeTimes;

    public virtual void playSoundType(string type) {

        // play sound by world, level or character if overidden
        // TODO custom overrides

        if(playSoundTypeTimes == null) {
            playSoundTypeTimes = new Dictionary<string, float>();
        }

        foreach(GameDataSound sound in GetSounds(type)) {

            float lastPlayed = 0f;

            if(playSoundTypeTimes.ContainsKey(sound.code)) {
                lastPlayed = playSoundTypeTimes.Get(sound.code);
            }

            if(lastPlayed + sound.play_delay < Time.time) {

                GameAudio.PlayEffect(
                    sound.code,
                    GameProfiles.Current.GetAudioEffectsVolume() * sound.modifier,
                    sound.isPlayTypeLoop);

                playSoundTypeTimes.Set(sound.code, Time.time);
            }
        }
    }

    // LOADING SOUND DATA

    public List<GameDataSound> GetSounds(string type) {

        List<GameDataSound> dataItems = new List<GameDataSound>();

        if(items == null) {
            items = new Dictionary<string, GameAudioDataItem>();
        }

        if(items.Count == 0 || !items.ContainsKey(type)) {
            foreach(GameDataSound obj in GetDataSounds(type)) {
                if(!dataItems.Contains(obj)) {
                    dataItems.Add(obj);
                }
            }

            GameAudioDataItem dataItem = new GameAudioDataItem();

            dataItem.type = type;
            dataItem.code = type;
            dataItem.items = dataItems;

            items.Set(type, dataItem);
        }

        if(items.ContainsKey(type)) {

            GameAudioDataItem dataItem = items.Get<GameAudioDataItem>(type);

            if(dataItem.last_update + dataItem.play_delay < Time.time) {
                dataItem.last_update = Time.time;

                dataItems.Clear();

                foreach(GameDataSound obj in GetDataSounds(type)) {
                    dataItems.Add(obj);
                }

                dataItem.items = dataItems;

                items.Set(type, dataItem);
            }
            else {
                dataItems = items.Get(type).items;
            }
        }
           
        return dataItems;
    }

    public List<GameDataSound> GetDataSounds(string type) {

        List<GameDataSound> dataItems = new List<GameDataSound>();

        GameWorld gameWorld = GameWorlds.Current;

        if(gameWorld != null) {
            GameDataObjectItem data = gameWorld.data;

            foreach(GameDataSound sound in data.GetSoundListByType(type)) {
                dataItems.Add(sound);
            }
        }

        return dataItems;
    }

    //

    public virtual void Update() {

    }
}