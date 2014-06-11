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
 
    public static BaseAudioController BaseInstance;
    public Dictionary<string,GameAudioDataItem> items;

    public static bool isBaseInst {
        get {
            if (BaseInstance != null) {
                return true;
            }
            return false;
        }
    }
 
    void Awake() {

        if (BaseInstance != null && this != BaseInstance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
    
        BaseInstance = this;
    
        // Init();
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {
        items = new Dictionary<string, GameAudioDataItem>();
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

    public virtual void playSoundType(string type) {
        
        // play sound by world, level or character if overidden
        // TODO custom overrides

        foreach (GameDataSound sound in GetSounds(type)) {
            GameAudio.PlayEffect(
                sound.code, 
                GameProfiles.Current.GetAudioEffectsVolume() * sound.modifier, 
                sound.isPlayTypeLoop);
        }
    }
    
    // LOADING SOUND DATA

    public  List<GameDataSound> GetSounds(string type) {

        List<GameDataSound> dataItems = new List<GameDataSound>();

        if (items == null) {
            items = new Dictionary<string, GameAudioDataItem>();
        }

        if (items.Count == 0 || !items.ContainsKey(type)) {            
            foreach (GameDataSound obj in GetDataSounds(type)) {
                if (!dataItems.Contains(obj)) {
                    dataItems.Add(obj);           
                }
            }

            GameAudioDataItem dataItem = new GameAudioDataItem();

            dataItem.type = type;
            dataItem.code = type;
            dataItem.items = dataItems;

            items.Set(type, dataItem);
        }
                
        if (items.ContainsKey(type)) {
            
            GameAudioDataItem dataItem = items.Get<GameAudioDataItem>(type);
            
            if (dataItem.last_update + 5 < Time.time) {
                dataItem.last_update = Time.time;

                dataItems.Clear();

                foreach (GameDataSound obj in GetDataSounds(type)) {
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
        
        if (gameWorld != null) {
            GameDataObjectItem data = gameWorld.data;
            
            foreach (GameDataSound sound in data.GetSoundListByType(type)) {
                dataItems.Add(sound);
            }
        }
        
        return dataItems;
    }

    //

    public virtual void Update() {
    
    }
}