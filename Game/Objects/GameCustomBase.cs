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
}

public class GameCustomInfo {

    public string type = GameCustomTypes.defaultType;
    
    public string actorType = GameCustomActorTypes.heroType;

    public string presetType = "character";

    public string presetColorCodeDefault = "game-nfl-cardinals";
    public string presetColorCode = "default";

    public string presetTextureCodeDefault = "fiestabowl";
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
}

public class GameCustomBase : MonoBehaviour {

    public string presetColorCodeDefault = "game-nfl-cardinals";        

    public string customColorCode = GameCustomTypes.defaultType;
    string lastCustomColorCode = "";
    
    public string customTextureCode = GameCustomTypes.defaultType;
    string lastCustomTextureCode = "";

    public string customActorType = GameCustomActorTypes.heroType;

    public GameCustomInfo customInfo;
    
    bool freezeRotation = false;

    bool initialized = false;

    public virtual void Start() {

       Invoke("Init", 1);
	}

    public virtual void Init() {

        customInfo = new GameCustomInfo();
        customInfo.actorType = customActorType;
        
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
	
	public virtual void OnEnable() {
		Messenger.AddListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
	
	public virtual void OnDisable() {
		Messenger.RemoveListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}

    public virtual void Load(string typeTo) {
        Load(typeTo, customActorType, typeTo, typeTo);
    }
    
    public virtual void Load(string typeTo, string actorType, string presetColorCodeTo, string presetTextureCodeTo) {
        GameCustomInfo customInfoTo = new GameCustomInfo();
        customInfoTo.type = typeTo;
        customInfoTo.presetColorCode = presetColorCodeTo;
        customInfoTo.presetTextureCode = presetTextureCodeTo;
        customInfo.actorType = actorType;
        Load(customInfoTo);
    }

    public virtual void Load(GameCustomInfo customInfoTo) {
        if(customInfoTo.presetColorCode != customInfo.presetColorCode) {

        }
        Change(customInfo);
    }

    public virtual void Change(GameCustomInfo customInfoTo) {

        customInfo = customInfoTo;

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

        if(customInfo.isCustomType) {
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
                  

        if(customInfo.isCustomType) {
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
        
        if(customInfo.isCustomType) {
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
        else if(lastCustomTextureCode != customTextureCode) {
            
            //if(AppColorPresets.Instance.CheckByCode(customTextureCode)) {
                
            AppContentAssetTexturePreset preset = 
                AppContentAssetTexturePresets.Instance.GetByCode(customInfo.presetTextureCode);
            if(preset != null) {
                // load from current code
                GameCustomController.UpdateTexturePresetObject(
                    gameObject, preset);
            }
                
            lastCustomTextureCode = customTextureCode;
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
        else if(lastCustomColorCode != customColorCode) {
            
            //if(AppColorPresets.Instance.CheckByCode(customColorCode)) {
                
                // load from current code
                GameCustomController.UpdateColorPresetObject(
                    gameObject, AppColorPresets.Instance.GetByCode(customColorCode));
                lastCustomColorCode = customColorCode;
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