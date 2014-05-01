using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomActorTypes {
    public static string heroType = "hero"; // used for customizer, defualt to profile, then allow changes.
    public static string enemyType = "enemy"; // use profile
    public static string sidekickType = "sidekick"; // call out a preset set
}

public class GameCustomTypes {
    public static string customType = "custom"; // used for customizer, defualt to profile, then allow changes.
    public static string defaultType = "default"; // use profile
    public static string explicitType = "explicit"; // call out a preset set
    public static string teamType = "team"; // call out a preset set
}

public class GameCustomInfo {

    public string type = GameCustomTypes.defaultType;
        
    public string actorType = GameCustomActorTypes.heroType;
    
    public string teamCode = "game-default";

    public string presetType = "character";

    public string presetColorCodeDefault = "game-default";
    public string presetColorCode = "default";

    public string presetTextureCodeDefault = "default";
    public string presetTextureCode = "default";

    public bool isCustomType {
        get {
            return type == GameCustomTypes.customType;
        }
    }
    
    public bool isDefaultType {
        get {
            return type == GameCustomTypes.defaultType;
        }
    }
    
    public bool isExplicitType {
        get {
            return type == GameCustomTypes.explicitType;
        }
    }
    
    public bool isTeamType {
        get {
            return type == GameCustomTypes.teamType;
        }
    }
}

public class BaseGameCustom : GameObjectBehavior {
    
    public string teamCode = "default";

    public string presetColorCodeDefault = "game-default";        

    public string customColorCode = GameCustomTypes.defaultType;
    string lastCustomColorCode = "--";
    
    public string customTextureCode = GameCustomTypes.defaultType;
    string lastCustomTextureCode = "--";

    public string customActorType = GameCustomActorTypes.heroType;

    public GameCustomInfo customInfo;
    
    bool freezeRotation = false;

    public virtual void Start() {

        Init();
	}

    public virtual void Init() {

        if(customInfo == null) {

            customInfo = new GameCustomInfo();
            customInfo.actorType = customActorType;

            customInfo.teamCode = teamCode;

            if(customColorCode == GameCustomTypes.customType) {
                customInfo.type = GameCustomTypes.customType;
            }
            else if(customColorCode == GameCustomTypes.defaultType) {
                customInfo.type = GameCustomTypes.defaultType;
            }
            else  {
                customInfo.type = GameCustomTypes.explicitType;
                customInfo.presetColorCode = customColorCode;
                customInfo.presetTextureCode = customTextureCode;
            }
            
            Load(customInfo);
        }
    }
	
	public virtual void OnEnable() {
		Messenger.AddListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
	
	public virtual void OnDisable() {
		Messenger.RemoveListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
    
    /*
    public virtual void Load(string typeTo) {
        Load(typeTo, customActorType, typeTo, typeTo);
    }

    public virtual void Load(string typeTo, string actorType, string presetColorCodeTo, string presetTextureCodeTo) {

        GameCustomInfo customInfoTo = new GameCustomInfo();
        customInfoTo.type = typeTo;
        customInfo.actorType = actorType;
        customInfoTo.presetColorCode = presetColorCodeTo;
        customInfoTo.presetTextureCode = presetTextureCodeTo;

        Load(customInfoTo);
    }
    */

    public virtual void Load(GameCustomInfo customInfoTo) {
        Change(customInfoTo);
    }

    public virtual void Change(GameCustomInfo customInfoTo) {

        customInfo = customInfoTo;      

        //Debug.Log("GameCustomBase:Change:customInfo:" + customInfo.teamCode);

        if(customInfo != null) {
            customColorCode = customInfo.presetColorCode;
            customTextureCode = customInfo.presetTextureCode;
            
            //Debug.Log("GameCustomBase:Change:customColorCode:" + customColorCode);
            //Debug.Log("GameCustomBase:Change:customTextureCode:" + customTextureCode);

            if(!string.IsNullOrEmpty(customInfo.teamCode)
               && customInfo.teamCode != "default") {
                
                //Debug.Log("Loading TEAM Custom Type:customInfo.teamCode:" + customInfo.teamCode);

                GameTeam team = GameTeams.Instance.GetById(customInfo.teamCode);

                if(team != null) {

                    if(team.data != null) {
                        
                        teamCode = team.code;
                        customInfo.type = GameCustomTypes.teamType;
                        
                        //Debug.Log("Loading TEAM EXISTS Type:teamCode:" + teamCode);                        
                        
                        GameDataTexturePreset itemTexture = team.data.GetTexturePreset();
                        
                        if(itemTexture != null) {  
                            customInfo.presetTextureCode = itemTexture.code;   
                            lastCustomColorCode = "--";
                            /*
                            AppContentAssetTexturePreset preset = 
                                AppContentAssetTexturePresets.Instance.GetByCode(customInfo.presetTextureCode);
                            if(preset != null) {
                                // load from current code
                                GameCustomController.UpdateTexturePresetObject(
                                    gameObject, preset);
                                */
                                //Debug.Log("Loading TEAM EXISTS TEXTURE:preset:" + preset.code);
                            //}
                        }

                        GameDataColorPreset itemColor = team.data.GetColorPreset();

                        if(itemColor != null) { 
                            customInfo.presetColorCode = itemColor.code;    
                        lastCustomTextureCode = "--";
                            /*
                            AppColorPreset preset = 
                                AppColorPresets.Instance.GetByCode(customInfo.presetColorCode);
                            if(preset != null) {
                                // load from current code
                                GameCustomController.UpdateColorPresetObject(
                                    gameObject, preset);
                                */
                                //Debug.Log("Loading TEAM EXISTS COLOR:preset:" + preset.code);
                            //}
                        }

                    } 
                }
            }
        }

        UpdatePlayer();
        /*
        GameCustomController.UpdateColorPresetObject(
            gameObject, AppColorPresets.Instance.GetByCode(presetColorCodeDefault));
        
        UpdatePlayer();
        */
    }
    
    public virtual void UpdatePlayer() {

        if(customInfo == null) {
            Init();
        }
        /*
        Debug.Log("UpdatePlayer"  
                  + " type:" + customInfo.type
                  + " presetType:" + customInfo.presetType
                  + " presetColorCode:" + customInfo.presetColorCode
                  + " presetTextureCode:" + customInfo.presetTextureCode
                  + " isCustomType:" + customInfo.isCustomType
                  + " isDefaultType:" + customInfo.isDefaultType
                  + " isExplicitType:" + customInfo.isExplicitType);
                  */

        if(customInfo.isCustomType || customInfo.isTeamType) {
            return;
        }
        else if(customInfo.isDefaultType) {
            SetCustom();
        }
    }
	
	void BaseOnCustomizationColorsChangedHandler() {
        UpdatePlayer();

        //Debug.Log("BaseOnCustomizationColorsChangedHandler");
	}

    public void SetCustom() {

        if(customInfo == null) {
            Init();
        }

        SetCustomTextures();

        SetCustomColors();
    }
    	
	public void SetCustomColors() {
        
        if(customInfo == null) {
            return;
        }

        //Debug.Log("SetCustomColors"  
        //          + " type:" + customInfo.type
        //          + " presetType:" + customInfo.presetType
        //          + " presetColorCode:" + customInfo.presetColorCode
        //          + " presetTextureCode:" + customInfo.presetTextureCode);
                  

        if(customInfo.isCustomType || customInfo.isTeamType || customInfo.isExplicitType) {
            return;
        }   
        else if(customInfo.isDefaultType) {

            if(customActorType == GameCustomActorTypes.heroType) {

                GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;
                
                //Debug.Log("SetCustomColors"  
                 //         + " customItem:" + customItem.ToJson());

                if(customItem != null) {

                    if(!customItem.HasData()) {
                        
                        GameCustomController.UpdateColorPresetObject(
                            gameObject, AppColorPresets.Instance.GetByCode(customInfo.presetColorCodeDefault));
                    }
                    else {

                        //customItem = GameCustomController.FillDefaultCustomColors(customItem, type);

                        GameCustomController.UpdateColorPresetObject(customItem, gameObject, customInfo.presetType);
                    }
                }
                else {                
                    
                    GameCustomController.UpdateColorPresetObject(
                        gameObject, AppColorPresets.Instance.GetByCode(customInfo.presetColorCodeDefault));
                }//GameCustomController.BroadcastCustomColorsChanged
            }
            
            else {    
                
                GameCustomController.UpdateColorPresetObject(
                    gameObject, AppColorPresets.Instance.GetByCode(customInfo.presetColorCodeDefault));

            }//GameCustomController.BroadcastCustomColorsChanged
        }
	}

    
    public void SetCustomTextures() {
        
        if(customInfo == null) {
            return;
        }
        
        /*
        Debug.Log("SetCustomTextures"  
                  + " presetType:" + customInfo.presetType
                  + " presetColorCode:" + customInfo.presetColorCode
                  + " presetTextureCode:" + customInfo.presetTextureCode);
                  */
        
        if(customInfo.isCustomType || customInfo.isTeamType || customInfo.isExplicitType) {
            return;
        }
        else if(customInfo.isDefaultType) {
            
            if(customActorType == GameCustomActorTypes.heroType) {
                
                GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;
                
                if(customItem != null) {
                 
                    GameCustomController.UpdateTexturePresetObject(customItem, gameObject, customInfo.presetType);
                }
                else {                
                    
                    GameCustomController.UpdateTexturePresetObject(
                        gameObject, AppContentAssetTexturePresets.Instance.GetByCode(customInfo.presetTextureCodeDefault));
                }//GameCustomController.BroadcastCustomColorsChanged
            }
            else {                
                GameCustomController.UpdateTexturePresetObject(
                    gameObject, AppContentAssetTexturePresets.Instance.GetByCode(customInfo.presetTextureCodeDefault));
            }
        }
    }
    
    public void HandleCustomPlayer() {
        
        if(customInfo == null) {
            Init();
        }

        HandleCustomPlayerTexture();
        HandleCustomPlayerColor();
    }
    
    public void HandleCustomPlayerTexture() {

        if(customInfo == null) {
            Init();
        }

        if(customInfo.isCustomType || customInfo.isDefaultType) {
            return;
        }
        else if(lastCustomTextureCode != customInfo.presetTextureCode) {
            
            //if(AppColorPresets.Instance.CheckByCode(customTextureCode)) {
            
            //Debug.Log("HandleCustomPlayerColor:changing:" + 
            //          " lastCustomColorCode:" + lastCustomTextureCode + 
            //          " customInfo.presetColorCode:" + customInfo.presetTextureCode);
                
            AppContentAssetTexturePreset preset = 
                AppContentAssetTexturePresets.Instance.GetByCode(customInfo.presetTextureCode);
            if(preset != null) {
                // load from current code
                GameCustomController.UpdateTexturePresetObject(
                    gameObject, preset);
            }
                
            lastCustomTextureCode = customInfo.presetTextureCode;
            //}
        }
    }		

    public void HandleCustomPlayerColor() {
        
        if(customInfo == null) {
            Init();
        }
        
        if(customInfo.isCustomType || customInfo.isDefaultType) {
            return;
        }
        else if(lastCustomColorCode != customInfo.presetColorCode) {
            
            //if(AppColorPresets.Instance.CheckByCode(customColorCode)) {

            //Debug.Log("HandleCustomPlayerColor:changing:" + 
            //          " lastCustomColorCode:" + lastCustomColorCode + 
             //         " customInfo.presetColorCode:" + customInfo.presetColorCode);

                // load from current code
                GameCustomController.UpdateColorPresetObject(
                gameObject, AppColorPresets.Instance.GetByCode(customInfo.presetColorCode));
            lastCustomColorCode = customInfo.presetColorCode;


            //}
        }
    }
		
	void Update() {

        HandleCustomPlayer();

		if(freezeRotation) {
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}
}