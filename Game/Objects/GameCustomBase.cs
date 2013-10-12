using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomMessages {
	public static string customColorsChanged = "custom-colors-changed";
	public static string customColorChanged = "custom-color-changed";
	public static string customColorsPlayerChanged = "custom-colors-player-changed";
	public static string customColorsEnemyChanged = "custom-colors-enemy-changed";
}

public class GameCustomBase : MonoBehaviour {
	
	public List<Material> materialPlayerHelmets = new List<Material>();
	public List<Material> materialPlayerHelmetFacemasks = new List<Material>();
	public List<Material> materialPlayerHelmetHighlights = new List<Material>();
	public List<Material> materialPlayerJerseys = new List<Material>();
	public List<Material> materialPlayerJerseyHighlights = new List<Material>();
	public List<Material> materialPlayerPants = new List<Material>();
	
	bool freezeRotation = false;
	
	public CustomPlayerColorsRunner colors;
	
	void Start() {
		//FindMaterials();
	}
	
	public virtual void OnEnable() {
		Messenger.AddListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
	
	public virtual void OnDisable() {
		Messenger.RemoveListener(GameCustomMessages.customColorsChanged, BaseOnCustomizationColorsChangedHandler);
	}
	
	void BaseOnCustomizationColorsChangedHandler() {
		SetCustomColors();

        //Debug.Log("BaseOnCustomizationColorsChangedHandler");
	}
	
	public void SetCustomColors() {
		SetCustomColors(gameObject);
	}
	
	public void SetCustomColors(GameObject go) {
	
		colors = GameProfiles.Current.GetCustomColorsRunner();
		SetMaterialColors(colors);		
	}
	
	public void SetCustomColors(GameObject go, CustomPlayerColorsRunner colorsTo) {
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
	
	public void SetMaterialColors(CustomPlayerColorsRunner colors) {
		
		FindMaterials();
		
		if(materialPlayerHelmets != null) {
			Color color = colors.helmetColor.GetColor();
			color.a = 1.0f;
			foreach(Material m in materialPlayerHelmets) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorHelmet:" + color);
		}
		
		if(materialPlayerHelmetFacemasks != null) {
			Color color = colors.helmetFacemaskColor.GetColor();
			color.a = 1.0f;
			foreach(Material m in materialPlayerHelmetFacemasks) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorHelmetFacemask:" + color);
		}
		
		if(materialPlayerHelmetHighlights != null) {
			Color color = colors.helmetHighlightColor.GetColor();
			color.a = 1.0f;
			foreach(Material m in materialPlayerHelmetHighlights) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorHelmetHighlight:" + color);
		}
				
		if(materialPlayerJerseys != null) {
			Color color = colors.jerseyColor.GetColor();
			color.a = 1.0f;
			foreach(Material m in materialPlayerJerseys) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorJersey:" + color );
		}
		
		if(materialPlayerJerseyHighlights != null) {
			Color color = colors.jerseyHighlightColor.GetColor();
			color.a = 1.0f;
			foreach(Material m in materialPlayerJerseyHighlights) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorJerseyHighlight:" + color);
		}
		
		if(materialPlayerPants != null) {
			Color color = colors.pantsColor.GetColor();
			color.a = 1.0f;
			foreach(Material m in materialPlayerPants) {
				m.color = color;
			}
			//LogUtil.Log("SetMaterialColors colorPants:" + color);
		}
	}
		
	void Update() {
		if(freezeRotation) {
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}
}