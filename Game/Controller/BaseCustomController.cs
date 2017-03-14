using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

// Handles character and level customization overrides on default

// messages

public class BaseCustomMessages {

    // events for current model color/texture custom
    public static string customColorsChanged = "custom-colors-changed";
    public static string customColorChanged = "custom-color-changed";
    public static string customColorsPlayerChanged = "custom-colors-player-changed";
    public static string customColorsEnemyChanged = "custom-colors-enemy-changed";
    public static string customColorPresetChanged = "custom-color-preset-changed";
    public static string customTexturePresetChanged = "custom-texture-preset-changed";
    // events for character changing
    public static string customCharacterModelChanged = "custom-character-model-changed";
    public static string customCharacterPlayerChanged = "custom-character-player-changed";
    // character meta
    public static string customCharacterDisplayNameChanged = "custom-character-display-name-changed";
    public static string customCharacterDisplayCodeChanged = "custom-character-display-code-changed";
    public static string customCharacterDisplayChanged = "custom-character-display-changed";

}

public class BaseGameCustomItemNames {
}

public class GameCustomKeys {
    public static string profileCharacterDisplay = "profile-character-display";
    public static string characterDisplay = "character-display";
}

public class BaseGameCustomColors : DataObjectItem {
    
    public static Color colorWhite = Color.white;
    public static Color colorBlack = Color.black;
    public static Color colorGray = Color.gray;
    
    //public static Color colorCardinalsRed = ColorHelper.FromRGB(135,6,25);//.FromHex("870619");
}

public class BaseGameCustomController : GameObjectBehavior, IBaseGameCustomController { 
        
    public bool runDirector = true;
    public float currentFPS = 0f;
    public float lastPeriodicSeconds = 0f;
    public GameProfileCustomPresets colorPresets;
    public int currentSelectedColorPreset = -1;
    float lastSave = 0f;
    bool save = false;

    public virtual void Awake() {
        
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {
        colorPresets = new GameProfileCustomPresets();

        GameCustomController.LoadCustomColors();
    }
        
    public virtual void OnEnable() {
        //Messenger<Color>.AddListener(GameCustomMessages.customColorChanged, OnCustomColorChanged);
    }

    public virtual void OnDisable() {
        //Messenger<Color>.RemoveListener(GameCustomMessages.customColorChanged, OnCustomColorChanged);
    }

    // TEXTURE PRESETS

    // TODO use textures not preset...

    public virtual GameProfileCustomItem updateTexturePresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, string type) {

        string profilePreset = profileCustomItem.GetCustomTexturePreset();

        AppContentAssetTexturePreset preset = AppContentAssetTexturePresets.Instance.GetByCode(profilePreset);

        if (preset != null) {
            
            return updateTexturePresetObject(profileCustomItem, go, preset);
        }

        return profileCustomItem;
    }

    public virtual GameProfileCustomItem updateTexturePresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, string presetType, string presetCode) {
        foreach (AppContentAssetTexturePreset preset in 
                AppContentAssetTexturePresets.Instance.GetListByType(presetType)) {
            if (presetCode == preset.code) {
                return updateTexturePresetObject(profileCustomItem, go, preset);
            }
        }
        return null;
    }

    public virtual GameProfileCustomItem updateTexturePresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, AppContentAssetTexturePreset preset) {

        if (preset == null) {
            return null;    
        }

        if(profileCustomItem.current_texture_preset == preset.code) {
            //return profileCustomItem;
        }

        //if (saveProfile)
        profileCustomItem.SetCustomTexturePreset(preset.code);

        if (go != null) {
            
            string path = ContentPaths.appCacheVersionSharedMaterials;
            
            //LogUtil.Log("UpdateObject:" + " path:" + path);
            
            foreach (AppContentAssetCustomItem customItem in 
                    AppContentAssetCustomItems.Instance.GetListByType(preset.type)) {
                                                
                //LogUtil.Log("UpdateObject:" + " customItem:" + customItem.code);
                
                foreach (AppContentAssetCustomItemProperty prop in customItem.properties) {
                    
                    if (prop.IsTypeTexture()) {

                        string codeNew = prop.code + "-" + preset.code;

                        string pathMaterial = path + codeNew;

                        go.SetMaterialSwap(prop.code, pathMaterial);

                        //if (saveProfile)
                        profileCustomItem.SetCustomTexture(prop.code, codeNew);

                        //LogUtil.Log("UpdateObject:preset:" + " prop.code:" + prop.code);
                        //LogUtil.Log("UpdateObject:preset:" + " pathMaterial:" + pathMaterial);
                    }
                }
            }
        }

        return profileCustomItem;
    }

    // COLOR PRESETS

    public virtual GameProfileCustomItem updateColorPresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, string type) {
               
        Dictionary<string, Color> colors = new Dictionary<string, Color>();

        foreach (AppContentAssetCustomItem customItem in 
                AppContentAssetCustomItems.Instance.GetListByType(type)) {

            //if(customItem.customCode != customCode) {
            //    continue;
            //}
            
            foreach (AppContentAssetCustomItemProperty prop in customItem.properties) {
                                                
                Color colorTo = profileCustomItem.GetCustomColor(prop.code);
                colors.Add(prop.code, colorTo);
            }

            break;
        }

        //LogUtil.Log("updateColorPresetObject:colors.Count:" + colors.Count);

        return updateColorPresetObject(profileCustomItem, go, type, colors);
    }
        
    public virtual GameProfileCustomItem updateColorPresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, string presetType, string presetCode) {
        foreach (AppColorPreset preset in 
                AppColorPresets.Instance.GetListByType(presetType)) {
            if (presetCode == preset.code) {
                return updateColorPresetObject(profileCustomItem, go, preset);
            }
        }
        return profileCustomItem;
    }
    
    public virtual GameProfileCustomItem updateColorPresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, AppColorPreset preset) {
        
        if (preset == null) {
            return profileCustomItem;
        }
        
        Dictionary<string, Color> colors = new Dictionary<string, Color>();
        
        foreach (KeyValuePair<string,string> pair in preset.data) {
            colors.Add(pair.Key, AppColors.GetColor(pair.Value));
        }
        
        if (colors.Count > 0) {
            return updateColorPresetObject(profileCustomItem, go, preset.type, colors);
        }
        
        return profileCustomItem;
    }
    
    public virtual GameProfileCustomItem updateColorPresetObject(
        GameProfileCustomItem profileCustomItem, GameObject go, string type, Dictionary<string, Color> colors) {
        
        if (colors == null) {
            return profileCustomItem;
        }
        
        if(profileCustomItem.current_color_preset == type) {
            //return profileCustomItem;
        }
        
        profileCustomItem.SetCustomColorPreset(type);
        
        if (go != null) {
            
            foreach (AppContentAssetCustomItem customItem in 
                     AppContentAssetCustomItems.Instance.GetListByType(type)) {
                
                //LogUtil.Log("updateColorPresetObject:" + " customItem:" + customItem.code);
                
                foreach (AppContentAssetCustomItemProperty prop in customItem.properties) {
                    
                    if (prop.IsTypeColor()) {
                        
                        Color colorTo = colors[prop.code];
                        
                        profileCustomItem.SetCustomColor(prop.code, colorTo);
                        
                        go.SetMaterialColor(prop.code, colorTo);

                        UIUtil.SetTextColor(go, prop.code, colorTo);

                        //updateProfileCharacterDisplay(go);

                        //LogUtil.Log("updateColorPresetObject:preset:" + 
                        //          " prop.code:" + prop.code + 
                        //         " colorTo:" + colorTo.ToString());
                    }
                }
            }

            //GameCustomController.SetMaterialColors(go, profileCustomItem);
        }
        
        return profileCustomItem;
    }

    // profile character

    public virtual void updateProfileCharacterDisplay(GameObject go) {
        updateProfileCharacterDisplayName(go);
        updateProfileCharacterDisplayCode(go);    
    }

    public virtual void updateProfileCharacterDisplayName(GameObject go) {  

        string val = GameProfileCharacters.currentCharacter.characterDisplayName;
        
        GameCustomController.UpdateCharacterDisplayName(go, val);
    }    

    public virtual void updateProfileCharacterDisplayCode(GameObject go) {        
        
        string val = GameProfileCharacters.currentCharacter.characterDisplayCode;

        GameCustomController.UpdateCharacterDisplayCode(go, val);
    }

    // ANY CHARACTER
    
    public virtual void updateCharacterDisplay(GameObject go, string valName, string valCode) {    
        GameCustomController.UpdateCharacterDisplayName(go, valName);
        GameCustomController.UpdateCharacterDisplayCode(go, valCode);
    }

    public virtual void updateCharacterDisplayName(GameObject go, string val) {  
        
        string key = 
            GameCustomKeys.profileCharacterDisplay + "-" + BaseDataObjectKeys.name;

        if(string.IsNullOrEmpty(val)) {
            val = ProfileConfigs.defaultGameCharacterDisplayName;
        }

        UIUtil.SetTextValue(go, key, val);  
    }    
    
    public virtual void updateCharacterDisplayCode(GameObject go, string val) {        
        
        string key = 
            GameCustomKeys.profileCharacterDisplay + "-" + BaseDataObjectKeys.code;
        
        if(string.IsNullOrEmpty(val)) {
            val = UnityEngine.Random.Range(0,99).ToString();
        }
        
        UIUtil.SetTextValue(go, key, val);  
    }

    public virtual GameProfileCustomItem fillDefaultCustomColors(GameProfileCustomItem customItemTo, string type) {

        if (customItemTo == null) {
            return customItemTo;
        }

        if (customItemTo.HasData()) {
            return customItemTo;
        }

        // get a random

        List<AppColorPreset> colors = AppColorPresets.Instance.GetListByType(type);

        int randomIndex = UnityEngine.Random.Range(0, colors.Count - 1);
        AppColorPreset randomPreset = colors[randomIndex];

        customItemTo = loadColorPresetCustomItem(customItemTo, randomPreset);

        //GameCustomController.SaveColors(customItemTo);
        
        return customItemTo;
    }
    
    public virtual GameProfileCustomItem loadColorPresetCustomItem(AppColorPreset preset) { 
        
        GameProfileCustomItem customItem = new GameProfileCustomItem();
        customItem = loadColorPresetCustomItem(customItem, preset);
        return customItem;
    }

    public virtual GameProfileCustomItem loadColorPresetCustomItem(GameProfileCustomItem customItem, AppColorPreset preset) {        
        foreach (KeyValuePair<string, string> pair in preset.data) {            
            customItem.SetCustomColor(pair.Key, AppColors.GetColor(pair.Value));
        }
        return customItem;
    }

    public virtual GameProfileCustomItem checkCustomColorInit(GameProfileCustomItem customItem, string type) {
        if (customItem.attributes == null || customItem.attributes.Count == 0) {
            // Fill default colors
            customItem = fillDefaultCustomColors(customItem, type);
        }
        return customItem;
    }

    //
    
    public virtual bool checkCustomColorPresetExists(string code) {
        return AppColorPresets.Instance.CheckByCode(code);
    }

    //

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

    // MESSAGES ALL

    
    public virtual void broadcastCustomSync() {

        //Debug.Log("GameCustomController::broadcastCustomSync");

        broadcastCustomColorsSync();
        broadcastCustomCharacterDataSync();
    }

    // VALUES

    public virtual void broadcastCustomCharacterDataSync() {
        GameCustomController.BroadcastCustomCharacterDisplayChanged();
        GameCustomController.BroadcastCustomCharacterDisplayCodeChanged();
        GameCustomController.BroadcastCustomCharacterDisplayNameChanged();
        
        //LogUtil.Log("broadcastCustomColorsSync");
    }

    public virtual void broadcastCustomCharacterProfileCodeSync(
        GameProfileCharacterItem profileCharacterItem = null) {

        if(profileCharacterItem == null) {
            profileCharacterItem = 
                GameProfileCharacters.Current.GetCurrentCharacter();        
        }
        
        if(profileCharacterItem != null) {
            
            Messenger<string>.Broadcast(
                GameCustomMessages.customCharacterPlayerChanged, 
                profileCharacterItem.code);
        }
    }
    
    public virtual void broadcastCustomCharacterDisplayCodeChanged() {
        Messenger.Broadcast(GameCustomMessages.customCharacterDisplayCodeChanged);
    }
    
    public virtual void broadcastCustomCharacterDisplayNameChanged() {
        Messenger.Broadcast(GameCustomMessages.customCharacterDisplayNameChanged);
    }
    
    public virtual void broadcastCustomCharacterDisplayChanged() {
        Messenger.Broadcast(GameCustomMessages.customCharacterDisplayChanged);
    }

    // COLORS
    
    public virtual void broadcastCustomColorsSync() {
        GameCustomController.BroadcastCustomColorsChanged();
        GameCustomController.BroadcastCustomColorsPlayerChanged();

        //LogUtil.Log("broadcastCustomColorsSync");
    }
    
    public virtual void broadcastCustomColorsChanged() {
        Messenger.Broadcast(GameCustomMessages.customColorsChanged);
    }
    
    public virtual void broadcastCustomColorsPlayerChanged() {
        Messenger.Broadcast(GameCustomMessages.customColorsPlayerChanged);
    }
        
    public virtual void setCustomColorsPlayer(GameObject go) {
        
        GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;
        
        GameCustomController.SetCustomColorsPlayer(go, customItem);
    }
        
    public virtual void setCustomColorsPlayer(GameObject go, GameProfileCustomItem customItem) {
        
        GameCustomPlayer player = go.GetComponent<GameCustomPlayer>();
        
        if (player == null) {
            GameCustomPlayer[] players = go.GetComponentsInChildren<GameCustomPlayer>(true);
            
            foreach (GameCustomPlayer playerTo in players) {
                player = playerTo;
                break;
            }
        }
        
        if (player != null) {       
            GameCustomController.SetMaterialColors(go, customItem);
        }
    }
    
    public virtual Color getRandomizedColorFromContextUI() {
        Color colorTo = UIColors.colorWhite;
        
        int randomColor = UnityEngine.Random.Range(0, 9);
        
        if (randomColor == 0) {
            colorTo = UIColors.colorWhite;
        }
        else if (randomColor == 1) {
            colorTo = UIColors.colorBlue;
        }
        else if (randomColor == 2) {
            colorTo = UIColors.colorGreen;
        }
        else if (randomColor == 3) {
            colorTo = UIColors.colorLightBlue;
        }
        else if (randomColor == 4) {
            colorTo = UIColors.colorOrange;
        }
        else if (randomColor == 5) {
            colorTo = UIColors.colorLight;
        }
        else if (randomColor == 6) {
            colorTo = UIColors.colorPurple;
        }
        else if (randomColor == 7) {
            colorTo = UIColors.colorRed;
        }
        else if (randomColor == 8) {
            colorTo = UIColors.colorYellow;
        }
        else {
            colorTo = UIColors.colorLight;
        }

        return colorTo;
    }

    public virtual Color getRandomizedColorFromContextCustomized() {

        Color colorTo = UIColors.colorWhite;

        GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;
        
        int randomColor = UnityEngine.Random.Range(0, 8);
        
        /*
        if (randomColor == 1) {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.helmet);
        }
        else if (randomColor == 2) {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.helmetHighlight);
        }
        else if (randomColor == 3) {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.jersey);
        }
        else if (randomColor == 4) {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.jerseyHighlight);
        }
        else if (randomColor == 5) {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.pants);
        }
        else if (randomColor == 6) {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.helmetFacemask);
        }
        else {
            colorTo = customItem.GetCustomColor(GameCustomItemNames.helmetHighlight);
        }
        */

        return colorTo;
    }
        
    public virtual Color getRandomizedColorFromContext() {
        
        // Randomize or get customized colors

        Color colorTo = UIColors.colorWhite;
        
        int randomColor = UnityEngine.Random.Range(0, 4);
        
        if (randomColor < 2) {
            colorTo = GameCustomController.GetRandomizedColorFromContextCustomized();
        }
        else {
            colorTo = GameCustomController.GetRandomizedColorFromContextUI();        
        }
        
        return colorTo;
    }

    public virtual Color getRandomizedColorFromContextEffects() {
        
        // Randomize or get customized colors

        Color colorTo = UIColors.colorWhite;
        
        int randomColor = UnityEngine.Random.Range(0, 4);

        if (randomColor < 1) {
            colorTo = GameCustomController.GetRandomizedColorFromContextCustomized();
        }
        else {
            colorTo = GameCustomController.GetRandomizedColorFromContextUI();        
        }

        return colorTo;
    }
            
    public virtual void setMaterialColors(GameObject go, GameProfileCustomItem profileCustomItem) {

        //LogUtil.Log("setMaterialColors:go null:", go == null);
        //LogUtil.Log("setMaterialColors:profileCustomItem null:", profileCustomItem == null);
        
        if (go == null) {
            return;
        }

        //LogUtil.Log("setMaterialColors:", go.name);
        //LogUtil.Log("setMaterialColors:", profileCustomItem.);

        //GameCustomController.SetCustomColorsPlayer(go, profileCustomItem);
        
        TriggerSave();
    }

    public void TriggerSave() {
        save = true;
    }

    public virtual void saveCustomItem() {
        GameCustomController.SaveCustomItem(GameProfileCharacters.currentCustom);
    }
    
    public virtual void saveCustomItem(GameProfileCustomItem profileCustomItem) {
        
        GameProfileCharacters.Current.SetCharacterCustom(profileCustomItem);

        GameState.SaveProfile();
        
        //LogUtil.Log("saveCustomItem:profileCustomItem:" + profileCustomItem);
        //LogUtil.Log("saveCustomItem:profileCustomItem:json:" + profileCustomItem.ToJson());
        //LogUtil.Log("saveCustomItem:currentCustom:json:" + GameProfileCharacters.currentCustom.ToJson());
        
        GameCustomController.BroadcastCustomSync();
    }

    public virtual void loadCustomColors() {

        /*
        LogUtil.Log("LoadCustomColors:", " GameConfigColors.colorPresets.presets.Count:" + GameConfigColors.colorPresets.presets.Count);
        
        if(GameConfigColors.colorPresets.presets.Count == 0) {
            
            // load default presets           
            
            GameProfileCustomItem item = GameCustomController.Instance.FillDefaultCustomColorsPlayer();
            
            AddCustomColorItem(
                "my", "My Current Colors",
                currentProfileCustom.GetCustomColor(GameCustomItemNames.helmet),
                currentProfileCustom.GetCustomColor(GameCustomItemNames.helmetFacemask),
                currentProfileCustom.GetCustomColor(GameCustomItemNames.helmetHighlight),
                currentProfileCustom.GetCustomColor(GameCustomItemNames.GameConfigColors.colorJersey),
                currentProfileCustom.GetCustomColor(GameCustomItemNames.GameConfigColors.colorJerseyHighlight),
                currentProfileCustom.GetCustomColor(GameCustomItemNames.GameConfigColors.colorPants));
            
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
        
        if (lastSave > 1f) {
            lastSave = 0;
            
            if (save) {
                save = false;
                GameCustomController.SaveCustomItem();
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

