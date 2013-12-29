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

    public string presetTextureCodeDefault = "game-nfl-cardinals";
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

    public virtual void Start() {

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
            return;
        }

        Debug.Log("UpdatePlayer"  
                  + " type:" + customInfo.type
                  + " presetType:" + customInfo.presetType
                  + " presetColorCode:" + customInfo.presetColorCode
                  + " presetTextureCode:" + customInfo.presetTextureCode
                  + " isCustomType:" + customInfo.isCustomType
                  + " isDefaultType:" + customInfo.isDefaultType
                  + " isExplicitType:" + customInfo.isExplicitType);

        if(customInfo.isCustomType) {
            return;
        }
        else if(customInfo.isDefaultType) {
            SetCustomColors();
        }
    }
	
	void BaseOnCustomizationColorsChangedHandler() {
        UpdatePlayer();

        //Debug.Log("BaseOnCustomizationColorsChangedHandler");
	}
    	
	public void SetCustomColors() {
        
        if(customInfo == null) {
            return;
        }
        
        Debug.Log("SetCustomColors"  
                  + " presetType:" + customInfo.presetType
                  + " presetColorCode:" + customInfo.presetColorCode
                  + " presetTextureCode:" + customInfo.presetTextureCode);

        if(customInfo.isCustomType) {
            return;
        }        
        else if(customInfo.isDefaultType) {

            GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;

            if(customItem != null) {

                string type = "character";

                customItem = GameCustomController.FillDefaultCustomColors(customItem, type);

                GameCustomController.UpdateColorPresetObject(customItem, gameObject, type);
            }
            else {                
                
                GameCustomController.UpdateColorPresetObject(
                    gameObject, AppColorPresets.Instance.GetByCode(customInfo.presetColorCodeDefault));
            }//GameCustomController.BroadcastCustomColorsChanged
        }
        else {
            HandleCustomPlayer();
        }
	}
    
    public void HandleCustomPlayer() {
        HandleCustomPlayerTexture();
        HandleCustomPlayerColor();
    }
    
    public void HandleCustomPlayerTexture() {

        if(customInfo.isCustomType || customInfo.isDefaultType) {
            return;
        }
        else if(lastCustomTextureCode != customTextureCode) {
            
            if(AppColorPresets.Instance.CheckByCode(customTextureCode)) {
                
                // load from current code
                //GameCustomController.UpdateTexturePresetObject(
                //    gameObject, AppTexturePresets.Instance.GetByCode(customTextureCode));
                lastCustomTextureCode = customTextureCode;
            }
        }
    }		

    public void HandleCustomPlayerColor() {
        
        if(customInfo.isCustomType || customInfo.isDefaultType) {
            return;
        }
        else if(lastCustomColorCode != customColorCode) {
            
            if(AppColorPresets.Instance.CheckByCode(customColorCode)) {
                
                // load from current code
                GameCustomController.UpdateColorPresetObject(
                    gameObject, AppColorPresets.Instance.GetByCode(customColorCode));
                lastCustomColorCode = customColorCode;
            }
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