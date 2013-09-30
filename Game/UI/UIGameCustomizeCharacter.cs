using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;


public class UIGameCustomizeCharacter : MonoBehaviour {
	
	public string playerName = "default";
	
	//int currentSelectedItem = 0;
	public CustomPlayerColors initialProfileColors;
	CustomPlayerColors currentPlayerColors;
	CustomPlayerColors currentProfileColors;
		
	public GameObject bikeObject;
	public GameObject riderObject;
	
	public GameObject colorWheelPanel;
	
	/*
	// Bike Colors
	public UIRadioBtn radioColorBike;
	public UIRadioBtn radioColorRider;
	public UIRadioBtn radioColorShirt;
	public UIRadioBtn radioColorSkin;
	public UIRadioBtn radioColorBootsGloves;
	public UIRadioBtn radioColorSleevesPants;
	
	public UIButton buttonColorWheelBike;
	
	public Color colorBike;
	public Color colorRider;
	
	public Color colorShirt;
	public Color colorSkin;
	
	public Color colorBootsGloves;
	public Color colorSleevesPants;
	
	public UIButton buttonColorSave;
	public UIButton buttonColorRevert;	
	public UIButton buttonColorDefault;	
	public UIButton buttonColorCycleLeft;
	public UIButton buttonColorCycleRight;
	*/
	public UILabel labelCurrentCustomColors;
	
	/*
	private bool editingBike = false; // flipped since we have to trick ezgui radio buttons into checkboxes, correct after init.
	private bool editingRider = false;
	private bool editingShirt = true;
	private bool editingSkin = true;
	private bool editingBootsGloves = true;
	private bool editingSleevesPants = true;
				
	GameObject mxBike;
	GameObject mxRider;
	
	public List<CustomPlayerColors> presetColors = new List<CustomPlayerColors>();
	*/
	
	/*
	 * ## SSC DEFAULT
	 bikeColor: 
	 	ColorItem name:Default 
	 		r:0.333333343267441 
	 		g:0.764705896377563 
	 		b:0.18823529779911 
	 		a:1
	 riderColor: 
	 	ColorItem name:Default 
	 		r:0.333333343267441 
	 		g:0.764705896377563 
	 		b:0.18823529779911 
	 		a:1
	 shirtColor: 
	 	ColorItem name:Default 
	 		r:0.423529446125031 
	 		g:0.447058856487274 
	 		b:0.450980424880981 
	 		a:1
	 skinColor: 
	 	ColorItem name:Default 
	 		r:0.803921639919281 
	 		g:0.588235318660736 
	 		b:0.474509835243225 
	 		a:1
	 bootsGlovesColor: 
	 	ColorItem name:Default 
	 		r:0.866666734218597 
	 		g:0.866666734218597 
	 		b:0.866666734218597 
	 		a:1
	 bootsSleevesPants: 
	 	ColorItem name:Default 
	 		r:0.184313729405403 
	 		g:0.184313729405403 
	 		b:0.184313729405403 
	 		a:1
	 */
	
	void Start() {
		
		//mxBike = GameObject.Find("MX_LowPoly");
		//mxRider = GameObject.Find("MX_RiderMesh");
		
		//InitColors();		
		
		//InitEvents();
		
		//Reset();
		
		//LoadData();
	}
	
	/*
	void InitColors() {
		
		if(GameProfiles.Current.CheckIfAttributeExists(GameProfileAttributes.ATT_CUSTOM_COLORS)) {
			initialProfileColors = GameProfiles.Current.GetCustomColors();
			currentProfileColors = GameProfiles.Current.GetCustomColors();
			currentPlayerColors = GameProfiles.Current.GetCustomColors();
			
		}
		else {			
			initialProfileColors = GetCurrentModelColors();;
			currentProfileColors = GetCurrentModelColors();
			currentPlayerColors = GetCurrentModelColors();
		}
				
		colorBike = currentProfileColors.bikeColor.GetColor();
		colorRider = currentProfileColors.riderColor.GetColor();
		colorShirt = currentProfileColors.shirtColor.GetColor();
		colorSkin = currentProfileColors.skinColor.GetColor();
		colorBootsGloves = currentProfileColors.bootsGlovesColor.GetColor();
		colorSleevesPants = currentProfileColors.bootsSleevesPants.GetColor();

		presetColors.Clear();
		
		CustomPlayerColors itemDefault = currentProfileColors;
		itemDefault.colorCode = "custom";
		itemDefault.colorDisplayName = "My Colors";
		presetColors.Add(itemDefault);
		
		CustomPlayerColors itemDefault2 = GetCurrentModelColors();
		itemDefault2.colorDisplayName = "SupaSupa Default";
		presetColors.Add(itemDefault2);		
		
		CustomPlayerColors red = CustomColors.RedSet;
		red.colorDisplayName = "Red";
		presetColors.Add(red);
		
		CustomPlayerColors grey = CustomColors.GreySet;
		grey.colorDisplayName = "Grey";
		presetColors.Add(grey);
		
		CustomPlayerColors itemDefault3 = CustomColors.GoldSet;
		itemDefault3.colorDisplayName = "Bling";
		presetColors.Add(itemDefault3);
				
		CustomPlayerColors greenBright = CustomColors.GreenBrightSet;
		greenBright.colorDisplayName = "Green Bright";
		presetColors.Add(greenBright);
		
		foreach(GameCustomization customization in GameCustomizations.Instance.GetAll()) {
			if(customization.colors != null) {
				foreach(CustomPlayerColors colorSet in customization.colors) {
					colorSet.colorCode = "preset";
					presetColors.Add(colorSet);
					LogUtil.Log ("custom Color: " + colorSet.ToString());
				}
			}
		}
		
		LoadSelectedItem(999, true);
		
	}
	
	void InitPlayerColors() {
		currentPlayerColors = GetCurrentModelColors();
	}
	
	void LoadSelectedItem(int selectedIndex, bool clear) {
		
		if(selectedIndex < 0) {
			currentSelectedItem = presetColors.Count - 1;
		}
		else if(selectedIndex > presetColors.Count - 1) {
			currentSelectedItem = 0;
		}
		else {
			currentSelectedItem = selectedIndex;
		}		
		
		LogUtil.Log("LoadSelectedItem:" + currentSelectedItem);
		
		if(currentSelectedItem > -1 
			&& currentSelectedItem < presetColors.Count) {
			
			currentPlayerColors = presetColors[currentSelectedItem];
			
			LogUtil.Log("LoadSelectedItem2:" + currentSelectedItem);
			
			if(labelCurrentCustomColors != null) {
				labelCurrentCustomColors.text = currentPlayerColors.colorDisplayName;
				
				if(clear) {
					// Load up the colors if not editing
					
					if(currentPlayerColors.colorCode == "custom" 
						|| currentSelectedItem == 0) {
						// Load from profile
						
						LogUtil.Log("LoadSelectedItem: Loading item:" + currentPlayerColors.ToString());
						
						if(GameProfiles.Current.CheckIfAttributeExists(GameProfileAttributes.ATT_CUSTOM_COLORS)) {
							currentProfileColors = GameProfiles.Current.GetCustomColors();
							currentProfileColors.colorCode = "custom";
						}
						else {
							// Set to default since there is no data
							currentProfileColors = GetCurrentModelColors();
							currentProfileColors.colorCode = "custom";
							GameProfiles.Current.SetCustomColors(currentProfileColors);				
						}
							
						LogUtil.Log("LoadSelectedItem: Loading Profile:" + currentProfileColors.ToString());
						
						currentPlayerColors = currentProfileColors;		
						
					}
					
					colorBike = currentPlayerColors.bikeColor.GetColor();
					colorRider = currentPlayerColors.riderColor.GetColor();
					colorShirt = currentPlayerColors.shirtColor.GetColor();
					colorSkin = currentPlayerColors.skinColor.GetColor();
					colorBootsGloves = currentPlayerColors.bootsGlovesColor.GetColor();
					colorSleevesPants = currentPlayerColors.bootsSleevesPants.GetColor();
					
					SetMaterialColors();
				}
			}
		}
	}
	
	void LoadSelectedItem(int selectedIndex) {
		LoadSelectedItem(selectedIndex, true);
	}
	
	void LoadNextCustom() {
		LoadSelectedItem(currentSelectedItem + 1);
	}
	
	void LoadPreviousCustom() {		
		LoadSelectedItem(currentSelectedItem - 1);
	}
		
	void Reset() {
	
	}
	
	public void LoadData() {
	}
	
	public void InitEvents() {
		
		
		if(buttonColorSave != null) {
			buttonColorSave.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					GamePlayerProgress.Instance.SetAchievement(GameAchievements.ACHIEVE_UI_CUSTOM_BIKE, true);
					
					currentPlayerColors.colorCode = "custom";
					currentPlayerColors.bikeColor.FromColor(colorBike);
					currentPlayerColors.riderColor.FromColor(colorRider);
					currentPlayerColors.shirtColor.FromColor(colorShirt);
					currentPlayerColors.skinColor.FromColor(colorSkin);
					currentPlayerColors.bootsGlovesColor.FromColor(colorBootsGloves);
					currentPlayerColors.bootsSleevesPants.FromColor(colorSleevesPants);
					GameProfiles.Current.SetCustomColors(currentPlayerColors);
					GameState.SaveProfile();
				}
			});
		}
		
		if(buttonColorRevert != null) {
			buttonColorRevert.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					currentPlayerColors = currentProfileColors;
					LoadSelectedItem(0, false);
					GameProfiles.Current.SetCustomColors(currentPlayerColors);
					SetMaterialColors();
				}
			});
		}
		
		if(buttonColorDefault != null) {
			buttonColorDefault.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					currentPlayerColors = GetCurrentModelColors();
					LoadSelectedItem(1, true);
					SetMaterialColors();
				}
			});
		}
				
		if(buttonColorCycleLeft != null) {
			buttonColorCycleLeft.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					LoadPreviousCustom();
				}
			});
		}
		
		if(buttonColorCycleRight != null) {
			buttonColorCycleRight.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					LoadNextCustom();
				}
			});
		}			
		
		if(buttonColorWheelBike != null) {
			buttonColorWheelBike.SetInputDelegate(delegate (ref POINTER_INFO info) {
				if(info.evt == POINTER_INFO.INPUT_EVENT.TAP) {	
					
					Vector2 pickpos = new Vector2(info.devicePos.x, info.devicePos.y);
					Vector3 pickposInv = buttonColorWheelBike.transform.TransformPoint(pickpos);
					Texture2D texture2D = buttonColorWheelBike.gameObject.renderer.material.mainTexture as Texture2D;
					//texture2D.GetPixelBilinear(
		            int colorX = Convert.ToInt32((pickpos.x * 2) - (texture2D.width * GameScreenScaler.Instance.scaledViewportRect.y)) - 20;
		            int colorY = Convert.ToInt32(pickpos.y * 2);
		            Color color = texture2D.GetPixel(colorX,colorY);
					LoadSelectedItem(0 , false);
					SetColorProperties(color);
					LogUtil.Log("info.ray.origin:" + info.ray.origin);
					LogUtil.Log("colorX:" + colorX);
					LogUtil.Log("colorY:" + colorY);
					LogUtil.Log("pickpos:" + pickpos);
					LogUtil.Log("pickposInv:" + pickposInv);
					//POINTER_INFO
					SetMaterialColors();
					
				}
				if(info.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF) {
					// revert to colors
				}
				
				if(info.evt == POINTER_INFO.INPUT_EVENT.MOVE) {	
					
					////Vector2 buttonPosition = buttonColorWheelBike.transform.position;
					//Vector2 pickpos = new Vector2(info.devicePos.x, info.devicePos.y);
					
					//Texture2D texture2D = buttonColorWheelBike.gameObject.renderer.material.mainTexture as Texture2D;
					//float adjust = ((texture2D.width / 2) / 2);
		            //int colorX = Convert.ToInt32((pickpos.x) - adjust);
		            //int colorY = Convert.ToInt32(pickpos.y);
		            //Color color = texture2D.GetPixel(colorX,colorY);
	 				//SetColorProperties(color);
					//SetMaterialColors();
				}
			});
		}
		
		
		if(radioColorBike != null) {
			radioColorBike.SetInputDelegate(InputDelegate);
			radioColorBike.SetValueChangedDelegate(ValueChangedDelegate);
			HandleCheckedStates(radioColorBike);
		}		
		
		if(radioColorRider != null) {
			radioColorRider.SetInputDelegate(InputDelegate);
			radioColorRider.SetValueChangedDelegate(ValueChangedDelegate);
			HandleCheckedStates(radioColorRider);
		}
		
		if(radioColorShirt != null) {
			radioColorShirt.SetInputDelegate(InputDelegate);
			radioColorShirt.SetValueChangedDelegate(ValueChangedDelegate);
			HandleCheckedStates(radioColorShirt);
		}
		
		if(radioColorSkin != null) {
			radioColorSkin.SetInputDelegate(InputDelegate);
			radioColorSkin.SetValueChangedDelegate(ValueChangedDelegate);
			HandleCheckedStates(radioColorSkin);
		}
		
		if(radioColorBootsGloves != null) {
			radioColorBootsGloves.SetInputDelegate(InputDelegate);
			radioColorBootsGloves.SetValueChangedDelegate(ValueChangedDelegate);
			HandleCheckedStates(radioColorBootsGloves);
		}
		
		if(radioColorSleevesPants != null) {
			radioColorSleevesPants.SetInputDelegate(InputDelegate);
			radioColorSleevesPants.SetValueChangedDelegate(ValueChangedDelegate);
			HandleCheckedStates(radioColorSleevesPants);
		}
		
	}
	
	void HandleCheckedStates(UIRadioBtn radio) {
		if(radio == radioColorBike) {
			editingBike = !editingBike;
			LogUtil.Log("radioColorBike:" + editingBike);
		}
		else if(radio == radioColorRider) {
			editingRider = !editingRider;
			LogUtil.Log("radioColorRider:" + editingRider);
		}
		else if(radio == radioColorShirt) {
			editingShirt = !editingShirt;
			LogUtil.Log("radioColorShirt:" + editingShirt);
		}
		else if(radio == radioColorSkin) {
			editingSkin = !editingSkin;
			LogUtil.Log("radioColorSkin:" + editingSkin);
		}
		else if(radio == radioColorBootsGloves) {
			editingBootsGloves = !editingBootsGloves;
			LogUtil.Log("radioColorBootsGloves:" + editingBootsGloves);
		}
		else if(radio == radioColorSleevesPants) {
			editingSleevesPants = !editingSleevesPants;
			LogUtil.Log("radioColorSleevesPants:" + editingSleevesPants);
		}
			
		SetCheckedStates(radio);
	}
	
	void SetCheckedStates(UIRadioBtn radio) {
		if(radio == radioColorBike) {
			radio.SetState(Convert.ToInt32(!editingBike)); // flip to trick ezgui radio to checkbox
		}
		else if(radio == radioColorRider) {
			radio.SetState(Convert.ToInt32(!editingRider));
		}
		else if(radio == radioColorShirt) {
			radio.SetState(Convert.ToInt32(!editingShirt));
		}
		else if(radio == radioColorSkin) {
			radio.SetState(Convert.ToInt32(!editingSkin));
		}
		else if(radio == radioColorBootsGloves) {
			radio.SetState(Convert.ToInt32(!editingBootsGloves));
		}
		else if(radio == radioColorSleevesPants) {
			radio.SetState(Convert.ToInt32(!editingSleevesPants));
		}
	}
	
	void ValueChangedDelegate(IUIObject obj) {
		if(obj.GetType() == typeof(UIRadioBtn)) {
			//UIRadioBtn radio = (UIRadioBtn)obj;
			
			//HandleCheckedStates(radio);
		}
	}		
	
	void InputDelegate(ref POINTER_INFO ptr) {
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP) {
			if(ptr.targetObj.GetType() == typeof(UIRadioBtn)) {	
				
				GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
				
				UIRadioBtn radio = (UIRadioBtn)ptr.targetObj;				
				
				HandleCheckedStates(radio);
										
			}
		}
	}	
	
	public void SetMaterialColors() {
		
		if(mxBike) {
			mxBike.renderer.materials[1].color = colorBike;
			LogUtil.Log("SetMaterialColors colorBike:" + colorBike );
		}
		if(mxRider) {
			mxRider.renderer.materials[0].color = colorSleevesPants;
			mxRider.renderer.materials[1].color = colorShirt;
			mxRider.renderer.materials[2].color = colorSkin;
			mxRider.renderer.materials[3].color = colorRider;
			mxRider.renderer.materials[4].color = colorBootsGloves;
			LogUtil.Log("SetMaterialColors colorRider:" + colorRider );
		}
	}
	
	public CustomPlayerColors GetCurrentModelColors() {
	
		CustomPlayerColors defaultColors = new CustomPlayerColors();
		
		if(mxBike) {
			colorBike = mxBike.renderer.materials[1].color;
		}
		if(mxRider) {			
			colorSleevesPants = mxRider.renderer.materials[0].color;
			colorShirt = mxRider.renderer.materials[1].color;
			colorSkin = mxRider.renderer.materials[2].color;
			colorRider = mxRider.renderer.materials[3].color;
			colorBootsGloves = mxRider.renderer.materials[4].color;
		}
		
		defaultColors.bikeColor.FromColor(colorBike);
		defaultColors.bootsGlovesColor.FromColor(colorBootsGloves);
		defaultColors.bootsSleevesPants.FromColor(colorSleevesPants);
		defaultColors.riderColor.FromColor(colorRider);
		defaultColors.shirtColor.FromColor(colorShirt);
		defaultColors.skinColor.FromColor(colorSkin);
		
		LogUtil.Log("defaultColors:" + defaultColors.ToString());
		
		return defaultColors;
	}
	
	public void SetColorProperties(Color color) {
		if(editingBike) {
			colorBike = color;
		}
		if(editingRider) {
			colorRider = color;
		}
		if(editingShirt) {
			colorShirt = color;
		}
		if(editingSkin) {
			colorSkin = color;
		}
		if(editingBootsGloves) {
			colorBootsGloves = color;
		}
		if(editingSleevesPants) {
			colorSleevesPants = color;
		}
	}
	
	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
				if(hit.transform.gameObject == colorWheelPanel) {
					Texture2D text = (Texture2D)hit.collider.gameObject.renderer.material.mainTexture;
					Color color = text.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y); // GetPixelBilinear oh how I love thee.
					
					GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
					
					LoadSelectedItem(0, false);
					SetColorProperties(color);
					SetMaterialColors();
				}
			}
		}
	}
	
	public void LateUpdate() {
		
		if(bikeObject) {
        	bikeObject.transform.Rotate(0f, -50* Time.deltaTime, 0f);
		}
		
		SetCheckedStates(radioColorBike);
		SetCheckedStates(radioColorRider);
		SetCheckedStates(radioColorShirt);
		SetCheckedStates(radioColorSkin);
		SetCheckedStates(radioColorBootsGloves);
		SetCheckedStates(radioColorSleevesPants);
	}	
	*/
	
}

