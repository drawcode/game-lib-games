using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;


public class GameCustomBase : MonoBehaviour {

    public string presetColorCodeDefault = "game-nfl-cardinals";
        
    public string customColorCode = "default";
    public string lastCustomColorCode = "";
    
    bool freezeRotation = false;

	void Start() {
        UpdatePlayer();
	}
	
	public virtual void OnEnable() {
		Messenger.AddListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
	
	public virtual void OnDisable() {
		Messenger.RemoveListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
    
    public virtual void UpdatePlayer() {

        if(customColorCode == "custom") {
            return;
        }
        else if(customColorCode == "default") {
            //Debug.Log("OnCustomizationColorsPlayerChangedHandler");
            SetCustomColors();
        }   
        else {
            
        }

        // custom templates handled in tick
    }
	
	void BaseOnCustomizationColorsChangedHandler() {
        UpdatePlayer();

        //Debug.Log("BaseOnCustomizationColorsChangedHandler");
	}
    	
	public void SetCustomColors() {
        
        if(customColorCode == "custom") {
            return;
        }        
        else if(customColorCode == "default") {
            
            GameCustomController.UpdateColorPresetObject(
                gameObject, AppColorPresets.Instance.GetByCode(presetColorCodeDefault));

            //GameCustomController.SetCustomColorsPlayer(gameObject);
        }
        else {
            HandleCustomPlayerTemplate();
        }
	}
		

    public void HandleCustomPlayerTemplate() {
        
        if(customColorCode == "default") {
            return;
        }
        else if(customColorCode == "custom") {
            return;
        }
        else if(lastCustomColorCode != customColorCode) {
            
            if(GameCustomController.CheckCustomColorPresetExists(customColorCode)) {
                
                // load from current code
                GameCustomController.UpdateColorPresetObject(
                    gameObject, AppColorPresets.Instance.GetByCode(customColorCode));
                lastCustomColorCode = customColorCode;
            }
        }
    }
		
	void Update() {

        HandleCustomPlayerTemplate();

		if(freezeRotation) {
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}
}