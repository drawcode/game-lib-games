using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

// Handles character and level customization overrides on default

// color properties

public class BaseGameCustomColorProperties : DataObject {
    
    public List<GameCustomColorPropertiesItem> items;
    
    public BaseGameCustomColorProperties() {
        Init();
    }
    
    public virtual void Init() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        items = new List<GameCustomColorPropertiesItem>();
    }
}

// color properties item

public class BaseGameCustomColorPropertiesItem : DataObject {
    
    public string colorKey = "";
    public Color color = Color.white;
    
    public BaseGameCustomColorPropertiesItem() {
        Init();
    }
    
    public virtual void Init() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        colorKey = "";
        color = Color.white;
    }
}

// color sets

// color triggers

public class BaseGameCustomColorTriggers : DataObject {
    public List<GameCustomColorTrigger> items;

    public BaseGameCustomColorTriggers() {
        Init();
    }

    public virtual void Init() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        items = new List<GameCustomColorTrigger>();
    }

    public virtual bool GetTriggerValue(string colorKey) {

        GameCustomColorTrigger trigger = GetTrigger(colorKey);

        if(trigger != null) {
            return trigger.editing;
        }

        return false;
    }

    public virtual GameCustomColorTrigger GetTrigger(string colorKey) {
        foreach(GameCustomColorTrigger trigger in items) {
            if(trigger.colorKey.ToLower() == colorKey.ToLower()){
                return trigger;
            }
        }
        return null;
    }
    
    public virtual void SetTrigger(string colorKey, bool editing) {
        GameCustomColorTrigger trigger = new GameCustomColorTrigger();
        trigger.colorKey = colorKey;
        trigger.editing = editing;
        SetTrigger(trigger);
    }

    public virtual void SetTrigger(GameCustomColorTrigger trigger) {

        bool found = false;

        for (int i = 0; i < items.Count; i++) {
            if (items[i].colorKey.ToLower() == trigger.colorKey.ToLower()) {
                items[i].colorKey = trigger.colorKey;
                items[i].editing = trigger.editing;
                items[i].attributes = trigger.attributes;
            }
        }

        if (!found) {
            items.Add(trigger);
        }        
    }
}

// color trigger item

public class BaseGameCustomColorTrigger : DataObject {
    public bool editing = false;
    public string colorKey = "";
    
    public BaseGameCustomColorTrigger() {
        Init();
    }
    
    public virtual void Init() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        editing = false;
        colorKey = "";
    }
}

// messages

public class BaseCustomMessages {
    
    public static string customColorsChanged = "custom-colors-changed";
    public static string customColorChanged = "custom-color-changed";
    public static string customColorsPlayerChanged = "custom-colors-player-changed";
    public static string customColorsEnemyChanged = "custom-colors-enemy-changed";

    public static string customColorPresetChanged = "custom-color-preset-changed";

}

public class BaseGameCustomColorNames {

}

public class BaseGameCustomColors : DataObject {
    
    public static Color colorWhite = Color.white;
    public static Color colorBlack = Color.black;
    public static Color colorGray = Color.gray;
    
    //public static Color colorCardinalsRed = ColorHelper.FromRGB(135,6,25);//.FromHex("870619");
}

public class BaseGameCustomController : MonoBehaviour { 
        
    public bool runDirector = true;
    public float currentFPS = 0f;
    public float lastPeriodicSeconds = 0f;

    public List<GameCustomColorPropertiesItem> colorsSetCustomPlayers;
    public List<GameCustomColorPropertiesItem> colorsSetCustomEnemies;
    public List<GameCustomColorPropertiesItem> colorsSetCustomBoosts;
    public List<GameCustomColorPropertiesItem> colorsSetCustomEffects;
    public List<GameCustomColorPropertiesItem> colorsSetCustomWorlds;
    public List<GameCustomColorPropertiesItem> colorsSetCustomLevels;
    
    public GameCustomColorTriggers colorTriggers;

    public GameProfileCustomPresets colorPresets; 
    public GameProfileCustomItem currentProfileCustomItem;
    public GameProfileCustomItem initialProfileCustomItem;
    public GameObject currentPlayerObject;

    public int currentSelectedColorPreset = -1;

    float lastSave = 0f;
    bool save = false;

    public virtual void Awake() {
        
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {
        colorTriggers = new GameCustomColorTriggers();
        colorPresets = new GameProfileCustomPresets();
        currentProfileCustomItem = GameProfileCharacters.currentCustom;
        initialProfileCustomItem = GameProfileCharacters.currentCustom;
                
        GameCustomController.FillSetCustomColorsAll();

        foreach(GameCustomColorPropertiesItem item in colorsSetCustomPlayers) {

            if(initialProfileCustomItem != null) {
                            
                initialProfileCustomItem.SetCustomColor(
                    item.colorKey, 
                    currentProfileCustomItem.GetCustomColor(item.colorKey));
            }
        }

        GameCustomController.LoadCustomColors();
    }
        
    public virtual void OnEnable() {
        Messenger<Color>.AddListener(GameCustomMessages.customColorChanged, OnCustomColorChanged);
    }

    public virtual void OnDisable() {
        Messenger<Color>.RemoveListener(GameCustomMessages.customColorChanged, OnCustomColorChanged);
    }

    public virtual void updateCustomColorInit(
        GameObject currentPlayerObjectTo, 
        GameProfileCustomItem currentProfileCustomItemTo, 
        GameProfileCustomItem initialProfileCustomItemTo) {

        currentPlayerObject = currentPlayerObjectTo;
        currentProfileCustomItem = currentProfileCustomItemTo;
        initialProfileCustomItem = initialProfileCustomItemTo;

        GameCustomController.FillSetCustomColorsAll();

        foreach(GameCustomColorPropertiesItem item in colorsSetCustomPlayers) {

            if(initialProfileCustomItem != null) {
                initialProfileCustomItem.SetCustomColor(
                    item.colorKey, 
                    currentProfileCustomItem.GetCustomColor(item.colorKey));
            }
        }

        // TODO maybe update preset list....

        GameCustomController.UpdateCurrentCustomColorsPlayer();
    }

    public virtual void setCustomColorTrigger(string colorKey, bool editing) {
        if(colorTriggers == null) {
            return;
        }

        colorTriggers.SetTrigger(colorKey, editing);
    }

    public virtual GameCustomColorTrigger getCustomColorTrigger(string colorKey) {
        if(colorTriggers == null) {
            return null;
        }
        
        return colorTriggers.GetTrigger(colorKey);
    }

    public virtual bool getCustomColorTriggerValue(string colorKey) {
        if(colorTriggers == null) {
            return false;
        }
        
        return colorTriggers.GetTriggerValue(colorKey);
    }

    public virtual void OnCustomColorChanged(Color color) { 
        
        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);   
        
        GameCustomController.SetColorPropertiesTrigger(currentProfileCustomItem, color, colorTriggers);
        GameCustomController.SetMaterialColors(currentPlayerObject, currentProfileCustomItem);
    }

    public virtual void run() {
        runDirector = true;
    }
    
    public virtual void stop() {
        runDirector = false;
    }
    
    public virtual void directCustom() {
        
        currentFPS = FPSDisplay.GetCurrentFPS();    
        
        if (currentFPS > 20f) {
            
        }
    }
    
    public virtual void broadcastCustomColorsSync() {
        GameCustomController.BroadcastCustomColorsChanged();
        GameCustomController.BroadcastCustomColorsPlayerChanged();
    }
    
    public virtual void broadcastCustomColorsChanged() {
        Messenger.Broadcast(GameCustomMessages.customColorsChanged);
    }
    
    public virtual void broadcastCustomColorsPlayerChanged() {
        Messenger.Broadcast(GameCustomMessages.customColorsPlayerChanged);
    }
    
    public virtual GameProfileCustomItem fillDefaultCustomColorsPlayer() {
        
        GameProfileCustomItem colors = new GameProfileCustomItem();
        //item.SetCustomColor(GameCustomColorNames.colorHelmet, new Color(0.979f, 0.943f, 0.938f, 1.000f));
        //item.SetCustomColor(GameCustomColorNames.colorHelmetFacemask, new Color(0.7f, 0.7f, 0.7f, 1.000f));
        //item.SetCustomColor(GameCustomColorNames.colorHelmetHighlight, new Color(0.442f, 0.114f, 0.067f, 1.000f));
        //item.SetCustomColor(GameCustomColorNames.colorJersey, new Color(0.448f, 0.093f, 0.042f, 1.000f));
        //item.SetCustomColor(GameCustomColorNames.colorJerseyHighlight, new Color(0.974f, 0.955f, 0.952f, 1.000f));
        //item.SetCustomColor(GameCustomColorNames.colorPants, new Color(0.979f, 0.943f, 0.938f, 1.000f));

        foreach(GameCustomColorPropertiesItem item in colorsSetCustomPlayers) {
            colors.SetCustomColor(item.colorKey, item.color);
        }

        GameProfileCharacters.Current.SetCharacterCustom(colors);
        
        return GameProfileCharacters.currentCustom;
    }
    
    public virtual void setCustomColorsPlayer(GameObject go) {
        
        GameProfileCustomItem colors = GameProfileCharacters.currentCustom;
        
        GameCustomController.SetCustomColorsPlayer(go, colors);
    }
    
    public virtual GameProfileCustomItem checkCustomColorInit(GameProfileCustomItem customItem) {
        if (customItem.attributes == null || customItem.attributes.Count == 0) {
            // Fill default colors
            customItem = GameCustomController.FillDefaultCustomColorsPlayer();
        }
        return customItem;
    }
    
    public virtual void setCustomColorsPlayer(GameObject go, GameProfileCustomItem colors) {
        
        GameCustomPlayer player = go.GetComponent<GameCustomPlayer>();
        
        if (player == null) {
            GameCustomPlayer[] players = go.GetComponentsInChildren<GameCustomPlayer>(true);
            
            foreach (GameCustomPlayer playerTo in players) {
                player = playerTo;
                break;
            }
        }
        
        if (player != null) {            
            player.SetMaterialColors(colors);
        }
    }
    
    public virtual void setCustomColorsEnemy(GameObject go) {       
        
        GameProfileCustomItem colors = new GameProfileCustomItem();
        
        //colors.SetCustomColor(GameCustomColorNames.colorHelmet, Color.black);
        //colors.SetCustomColor(GameCustomColorNames.colorHelmetFacemask, Color.gray);
        //colors.SetCustomColor(GameCustomColorNames.colorHelmetHighlight, Color.gray);
        //colors.SetCustomColor(GameCustomColorNames.colorJersey, Color.black);
        //colors.SetCustomColor(GameCustomColorNames.colorJerseyHighlight, Color.gray);
        //colors.SetCustomColor(GameCustomColorNames.colorPants, Color.black);
                
        foreach(GameCustomColorPropertiesItem item in colorsSetCustomPlayers) {
            colors.SetCustomColor(item.colorKey, item.color);
        }

        GameCustomController.SetCustomColorsEnemy(go, colors);
    }
    
    public virtual void setCustomColorsEnemy(GameObject go, GameProfileCustomItem colors) {
        
        GameCustomEnemy enemy = go.GetComponent<GameCustomEnemy>();
        
        if (enemy == null) {
            GameCustomEnemy[] enemies = go.GetComponentsInChildren<GameCustomEnemy>(true);
            
            foreach (GameCustomEnemy enemyTo in enemies) {
                enemy = enemyTo;
                break;
            }
        }
        
        if (enemy != null) {         
            enemy.SetMaterialColors(colors);            
        }
    }

    public virtual void setCustomColorItemPlayer(string colorKey, Color color) {
        if(colorsSetCustomPlayers == null) {
            colorsSetCustomPlayers = new List<GameCustomColorPropertiesItem>();
        }

        bool found = false;

        for(int i = 0; i < colorsSetCustomPlayers.Count; i++) {
            if(colorsSetCustomPlayers[i].colorKey.ToLower() == colorKey.ToLower()) {
                found = true;
                colorsSetCustomPlayers[i].colorKey = colorKey;
                colorsSetCustomPlayers[i].color = color;
                break;
            }
        }

        if(!found) {            
            colorsSetCustomPlayers.Add(new GameCustomColorPropertiesItem(colorKey, color));
        }
    }
    
    public virtual void fillSetCustomColorsAll() {
        GameCustomController.FillSetCustomColorsPlayers();
        GameCustomController.FillSetCustomColorsBoosts();
        GameCustomController.FillSetCustomColorsEffects();
        GameCustomController.FillSetCustomColorsWorlds();
        GameCustomController.FillSetCustomColorsLevels();
    }

    public virtual void fillSetCustomColorsPlayers() {
        // all custom colors used in players
        if(colorsSetCustomPlayers == null) {
            colorsSetCustomPlayers = new List<GameCustomColorPropertiesItem>();
        }
    }

    public virtual void fillSetCustomColorsBoosts() {
        // all custom colors used in boosts
        if(colorsSetCustomPlayers == null) {
            colorsSetCustomPlayers = new List<GameCustomColorPropertiesItem>();
        }        
    }

    public virtual void fillSetCustomColorsEffects() {
        // all custom colors used in effects
        if(colorsSetCustomEffects == null) {
            colorsSetCustomEffects = new List<GameCustomColorPropertiesItem>();
        }
    }

    public virtual void fillSetCustomColorsWorlds() {
        // all custom colors used in worlds
        if(colorsSetCustomWorlds == null) {
            colorsSetCustomWorlds = new List<GameCustomColorPropertiesItem>();
        }
        
    }

    public virtual void fillSetCustomColorsLevels() {
        // all custom colors used in levels
        if(colorsSetCustomLevels == null) {
            colorsSetCustomLevels = new List<GameCustomColorPropertiesItem>();
        }
        
    }

    public virtual void setCustomColorItem(GameProfileCustomPreset preset) {        
        colorPresets.SetPresetColor(preset);
    }
    
    public virtual void changeColorNext() {        
        GameCustomController.ChangeColorPreset(currentSelectedColorPreset + 1);
    }
    
    public virtual void changeColorPrevious() {        
        GameCustomController.ChangeColorPreset(currentSelectedColorPreset - 1);
    }
    
    public virtual void changeColorPreset(int index) {
        
        LogUtil.Log("ChangeColorPreset:", " index:" + index);
        
        //LogUtil.Log("ChangeColorPreset:", 
        //            " colorPresets.presets.Count:" + colorPresets.presets.Count);
        
        //GameCustomController.FillDefaultCustomColorsPlayer();

        if (index < 0) {
            index = colorPresets.presets.Count - 1;    
        }
        
        if (index > colorPresets.presets.Count - 1) {
            index = 0;
        }
        
        //LogUtil.Log("ChangeColorPreset:filtered:", " index:" + index);
        
        if (colorPresets.presets != null) {
            if (index > -1 && index < colorPresets.presets.Count) {
                
                currentSelectedColorPreset = index;
                
                string code = colorPresets.presets[currentSelectedColorPreset].code;
                string name = colorPresets.presets[currentSelectedColorPreset].name;

                
                LogUtil.Log("ChangeColorPreset:setting:", 
                            " code:" + code);

                
                LogUtil.Log("ChangeColorPreset:setting:", 
                            " name:" + name);

                GameCustomColorProperties properties = new GameCustomColorProperties();

                foreach(GameCustomColorPropertiesItem item in colorsSetCustomPlayers) {

                    Color color = colorPresets.presets[index].customItem.GetCustomColor(item.colorKey);
                    GameCustomColorPropertiesItem itemColor = new GameCustomColorPropertiesItem();

                    itemColor.colorKey = item.colorKey;
                    itemColor.color = color;

                    
                    LogUtil.Log("ChangeColorPreset:setting:", 
                                " itemColor.colorKey:" +  itemColor.colorKey);

                    
                    LogUtil.Log("ChangeColorPreset:setting:", 
                                " itemColor.color:" +  itemColor.color.ToString());

                    properties.items.Add(itemColor);
                }

                
                LogUtil.Log("ChangeColorPreset:setting:", 
                            " properties.items.Count:" + properties.items.Count);
                
                GameCustomController.SetColorProperties(
                    currentProfileCustomItem, properties
                );
                
                //LogUtil.Log("ChangeColorPreset:setting:", 
                //            " currentSelectedColorPreset:" + currentSelectedColorPreset);
                
                //LogUtil.Log("ChangeColorPreset:setting:", 
                //            " colorPresets.presets[index].name:" + name);

                Messenger<string, string>.Broadcast(GameCustomMessages.customColorPresetChanged,
                                                    code, name);

                GameCustomController.SetMaterialColors(currentPlayerObject, currentProfileCustomItem);
            }
        }   
    }

    public virtual void setColorPropertiesTrigger(
        GameProfileCustomItem profileCustomItem,
        Color color,
        GameCustomColorTriggers triggers) {

        if (triggers != null) {
            foreach (GameCustomColorTrigger trigger in triggers.items) {
                if (trigger.editing) {
                    profileCustomItem.SetCustomColor(trigger.colorKey, color);
                }
            }
        }  
    }

    public virtual void setColorProperties(
        GameProfileCustomItem profileCustomItem,
        GameCustomColorProperties properties) {
        
        if (properties != null) {
            foreach (GameCustomColorPropertiesItem item in properties.items) {
                
                LogUtil.Log("setColorProperties:setting:", 
                            " item.colorKey:" + item.colorKey);

                LogUtil.Log("setColorProperties:setting:", 
                            " item.color:" + item.color);

                profileCustomItem.SetCustomColor(item.colorKey, item.color);
            }
        }  
    }
        
    public virtual void setMaterialColors(GameObject go, GameProfileCustomItem profileCustomItem) {

        LogUtil.Log("setMaterialColors:go null:", go == null);
        LogUtil.Log("setMaterialColors:profileCustomItem null:", profileCustomItem == null);
        
        if (go == null) {
            return;
        }

        LogUtil.Log("setMaterialColors:", go.name);
        //LogUtil.Log("setMaterialColors:", profileCustomItem.);

        GameCustomController.SetCustomColorsPlayer(go, profileCustomItem);
        
        save = true;
    }

    public virtual void saveColors() {
        GameCustomController.SaveColors(currentProfileCustomItem);
    }
    
    public virtual void saveColors(GameProfileCustomItem profileCustomItem) {
        
        GameProfileCharacters.Current.SetCharacterCustom(profileCustomItem);

        GameState.SaveProfile();
        
        LogUtil.Log("SaveColors:profileCustomItem:", profileCustomItem);
        
        GameCustomController.BroadcastCustomColorsSync();
    }

    public virtual void loadCustomColors() {

        /*
        LogUtil.Log("LoadCustomColors:", " GameConfigColors.colorPresets.presets.Count:" + GameConfigColors.colorPresets.presets.Count);
        
        if(GameConfigColors.colorPresets.presets.Count == 0) {
            
            // load default presets           
            
            GameProfileCustomItem item = GameCustomController.Instance.FillDefaultCustomColorsPlayer();
            
            AddCustomColorItem(
                "my", "My Current Colors",
                currentProfileCustom.GetCustomColor(GameCustomColorNames.colorHelmet),
                currentProfileCustom.GetCustomColor(GameCustomColorNames.colorHelmetFacemask),
                currentProfileCustom.GetCustomColor(GameCustomColorNames.colorHelmetHighlight),
                currentProfileCustom.GetCustomColor(GameCustomColorNames.GameConfigColors.colorJersey),
                currentProfileCustom.GetCustomColor(GameCustomColorNames.GameConfigColors.colorJerseyHighlight),
                currentProfileCustom.GetCustomColor(GameCustomColorNames.GameConfigColors.colorPants));
            
            // --------------------------------------------------------------
            // TEAMS
            
            string code = "";
            string name = "";
            
            // --------------
            // cards
            // maroon: #870619
            
            
            code = "cardinals";
            name = "Phoenix Heat";
            
            GameController.AddCustomColorItem(
                code, name, 
                GameConfigColors.colorWhite,             // colorHelmet 1
                GameConfigColors.colorWhite,             // colorHelmetFacemask 1
                GameConfigColors.colorCardinalsRed,            // colorHelmetHighlight 2
                GameConfigColors.colorCardinalsRed,             // GameConfigColors.colorJersey 1
                GameConfigColors.colorWhite,            // GameConfigColors.colorJerseyHighlight 2
                GameConfigColors.colorCardinalsRed);            // GameConfigColors.colorPants 1
            
            AddCustomColorItem(
                code + "-away", name + " Away",  
                GameConfigColors.colorWhite,             // colorHelmet 1
                GameConfigColors.colorWhite,             // colorHelmetFacemask 1
                GameConfigColors.colorCardinalsRed,            // colorHelmetHighlight 2
                GameConfigColors.colorWhite,             // GameConfigColors.colorJersey 1
                GameConfigColors.colorCardinalsRed,            // GameConfigColors.colorJerseyHighlight 2
                GameConfigColors.colorCardinalsRed);            // GameConfigColors.colorPants 1
            
            AddCustomColorItem(
                code + "-dark", name + " Dark", 
                GameConfigColors.colorWhite,             // colorHelmet 1
                GameConfigColors.colorWhite,             // colorHelmetFacemask 1
                GameConfigColors.colorCardinalsRed,            // colorHelmetHighlight 2
                GameConfigColors.colorBlack,             // GameConfigColors.colorJersey 1
                GameConfigColors.colorCardinalsRed,            // GameConfigColors.colorJerseyHighlight 2
                GameConfigColors.colorBlack);            // GameConfigColors.colorPants 1
                */

    }

    public virtual void updateCurrentCustomColorsPlayer() {

        if(currentProfileCustomItem == null) {
            return;
        }

        GameProfileCustomPreset preset = new GameProfileCustomPreset();
        preset.code = "my";
        preset.name = "My Current Colors";
        preset.customItem = new GameProfileCustomItem();
        
        preset.customItem.SetCustomColor(
            GameCustomColorNames.colorHelmet,
            currentProfileCustomItem.GetCustomColor(GameCustomColorNames.colorHelmet));
        
        preset.customItem.SetCustomColor(
            GameCustomColorNames.colorHelmetFacemask,
            currentProfileCustomItem.GetCustomColor(GameCustomColorNames.colorHelmetFacemask));
        
        preset.customItem.SetCustomColor(
            GameCustomColorNames.colorHelmetHighlight,
            currentProfileCustomItem.GetCustomColor(GameCustomColorNames.colorHelmetHighlight));
        
        preset.customItem.SetCustomColor(
            GameCustomColorNames.colorJersey,
            currentProfileCustomItem.GetCustomColor(GameCustomColorNames.colorJersey));
        
        preset.customItem.SetCustomColor(
            GameCustomColorNames.colorJerseyHighlight,
            currentProfileCustomItem.GetCustomColor(GameCustomColorNames.colorJerseyHighlight));
        
        preset.customItem.SetCustomColor(
            GameCustomColorNames.colorPants,
            currentProfileCustomItem.GetCustomColor(GameCustomColorNames.colorPants));

        if(colorPresets != null) {
            if(colorPresets.presets != null) {
                if(colorPresets.presets.Count > 0) {
                    colorPresets.presets[0] = preset;
                }
            }
        }
    }

    public virtual void handlePeriodic() {
        
        if (Time.time > lastPeriodicSeconds + 3f) {
            lastPeriodicSeconds = Time.time;
            // every second         
            GameCustomController.DirectCustom();         
        }       
    }

    public virtual void handleUpdate() {
        // do on update always
        
        //currentActorCount = GameController.Instance.characterActorsCount; 
    }

    public virtual void handleSave() {
        lastSave += Time.deltaTime;
        
        if(lastSave > 1f) {
            lastSave = 0;
            
            if(save) {
                save = false;
                GameCustomController.SaveColors();
            }
        }
    }

    public virtual void Update() {
        
        if (!runDirector
            || GameDraggableEditor.isEditing) {
            return;
        }

        handleSave();
        
        if (GameController.IsGameRunning) {
            
            GameCustomController.HandlePeriodic();
            
            GameCustomController.HandleUpdate();         
            
        }
    }
}

