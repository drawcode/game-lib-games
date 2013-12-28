using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;


public class GameCustomBase : MonoBehaviour {
	
	public List<Material> materialPlayerHelmets = new List<Material>();
	public List<Material> materialPlayerHelmetFacemasks = new List<Material>();
	public List<Material> materialPlayerHelmetHighlights = new List<Material>();
	public List<Material> materialPlayerJerseys = new List<Material>();
	public List<Material> materialPlayerJerseyHighlights = new List<Material>();
	public List<Material> materialPlayerPants = new List<Material>();
	
	bool freezeRotation = false;

    GameProfileCustomItem colors;
    
    public string customColorCode = "default";
    public string lastCustomColorCode = "";
	
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
        if(customColorCode == "default") {
            //Debug.Log("OnCustomizationColorsPlayerChangedHandler");
            SetCustomColors(gameObject);
        }   

        // custom templates handled in tick
    }
	
	void BaseOnCustomizationColorsChangedHandler() {
        UpdatePlayer();

        //Debug.Log("BaseOnCustomizationColorsChangedHandler");
	}
    	
	public void SetCustomColors() {
		SetCustomColors(gameObject);
	}
	
	public void SetCustomColors(GameObject go) {
	
		colors = GameProfileCharacters.currentCustom;
		SetMaterialColors(colors);		
	}
	
	public void SetCustomColors(GameObject go, GameProfileCustomItem colorsTo) {
		colors = colorsTo;
		SetMaterialColors(colors);
	}	
	
	public void FindMaterials() {
		FindMaterials(gameObject);
	}
	
	public void FindMaterials(GameObject go) {
		if(go == null) {
			return;
		}

       // foreach(GameCustomColorMaterial material in GameCustomController.GEt
		
		if(materialPlayerHelmets.Count == 0) {
			materialPlayerHelmets = go.GetMaterials("PlayerHelmet");
		}
				
		if(materialPlayerHelmetFacemasks.Count == 0) {
			materialPlayerHelmetFacemasks = go.GetMaterials("PlayerHelmetFacemask");
		}
		
		if(materialPlayerHelmetHighlights.Count == 0) {
			materialPlayerHelmetHighlights = go.GetMaterials("PlayerHelmetHighlight");
		}
		
		if(materialPlayerJerseys.Count == 0) {
			materialPlayerJerseys = go.GetMaterials("PlayerJersey");
		}
		
		if(materialPlayerJerseyHighlights.Count == 0) {
			materialPlayerJerseyHighlights = go.GetMaterials("PlayerJerseyHighlight");
		}
		
		if(materialPlayerPants.Count == 0) {
			materialPlayerPants = go.GetMaterials("PlayerPants");
		}
		
	}
	
	public void SetMaterialColors(GameProfileCustomItem colors) {
		
		FindMaterials();

        if(colors == null) {
            return;
        }
		
		if(materialPlayerHelmets != null) {
			Color color = colors.GetCustomColor(GameCustomItemNames.helmet);
			color.a = 1.0f;
			foreach(Material m in materialPlayerHelmets) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorHelmet:" + color);
		}
		
		if(materialPlayerHelmetFacemasks != null) {
			Color color = colors.GetCustomColor(GameCustomItemNames.helmetFacemask);
			color.a = 1.0f;
			foreach(Material m in materialPlayerHelmetFacemasks) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorHelmetFacemask:" + color);
		}
		
		if(materialPlayerHelmetHighlights != null) {
			Color color = colors.GetCustomColor(GameCustomItemNames.helmetHighlight);
			color.a = 1.0f;
			foreach(Material m in materialPlayerHelmetHighlights) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorHelmetHighlight:" + color);
		}
				
		if(materialPlayerJerseys != null) {
			Color color = colors.GetCustomColor(GameCustomItemNames.jersey);
			color.a = 1.0f;
			foreach(Material m in materialPlayerJerseys) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorJersey:" + color );
		}
		
		if(materialPlayerJerseyHighlights != null) {
			Color color = colors.GetCustomColor(GameCustomItemNames.jerseyHighlight);
			color.a = 1.0f;
			foreach(Material m in materialPlayerJerseyHighlights) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorJerseyHighlight:" + color);
		}
		
		if(materialPlayerPants != null) {
			Color color = colors.GetCustomColor(GameCustomItemNames.pants);
			color.a = 1.0f;
			foreach(Material m in materialPlayerPants) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorPants:" + color);
		}
	}

    public void HandleCustomPlayerTemplate() {
        
        if(customColorCode == "default") {
            return;
        }

        if(lastCustomColorCode != customColorCode) {
            
            if(GameCustomController.CheckCustomColorPresetExists(customColorCode)) {
                
                // load from current code
                GameCustomController.UpdateColorPresetObject(gameObject, AppColorPresets.Instance.GetByCode(customColorCode), false);
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