using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public enum UIPanelEditAssetActionState {
	NONE,
	SELECT_ITEM,
	SELECT_EFFECT,
	SELECT_AUDIO
}

public class UIPanelEditAsset : UIAppPanel {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
	
	public static UIPanelEditAsset Instance;
	
	public UIPanelEditAssetActionState actionState = UIPanelEditAssetActionState.NONE;
	
	public UIImageButton buttonGameEditAssetDelete;
	public UIImageButton buttonGameEditAssetDeselect;
	public UIImageButton buttonGameEditAssetSave;
	public UIImageButton buttonGameEditAssetSprite;
	public UIImageButton buttonGameEditAssetSpriteEffect;
	
	public UICheckbox checkboxEditAssetDestructable;
	public UICheckbox checkboxEditAssetKinematic;
	public UICheckbox checkboxEditAssetReactive;
	public UICheckbox checkboxEditAssetGravity;
	
	public UIInput inputSprite;
	public UIInput inputSpriteEffect;
	
	public UILabel labelAssetEdit;
	public UILabel labelGameEditAssetSprite;
	public UILabel labelGameEditAssetSpriteEffect;
		
	// Tools
	
	// Rotation
	public UIInput inputRotationSpeed;
	public UISlider sliderRotationSpeed;
	public UIImageButton buttonGameEditAssetRotationReset;
	
	float MIN_ROTATION_SPEED = -1000f;
	float MAX_ROTATION_SPEED = 1000f;
	
	public GameLevelItemAsset itemAsset;	
	
	void Awake() {
		if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
	}
	
	void OnEnable() {
    	Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);		
		
    	Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		
    	Messenger<string,int>.AddListener(InputEvents.EVENT_ITEM_CLICK, OnInputClickEventHandler);
    	Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnInputChangeEventHandler);
		
        Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
	}
		
	void OnDisable() {
    	Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
		
    	Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		
    	Messenger<string,int>.RemoveListener(InputEvents.EVENT_ITEM_CLICK, OnInputClickEventHandler);
    	Messenger<string, string>.RemoveListener(InputEvents.EVENT_ITEM_CHANGE, OnInputChangeEventHandler);
		
        Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
	}
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		LoadData();
		
		UIUtil.SetSliderValue(sliderRotationSpeed, .5f);
	}
	
	public void LoadData() {
		LoadDataAsset();
	}
	
	public void LoadDataAsset() {	
		
		SyncCurrenItemAsset();
		
		if(itemAsset != null) {		
			//UIUtil.SetLabelValue(labelAssetEdit, itemAsset.asset_code);
			UIUtil.SetInputValue(inputSprite, itemAsset.asset_code);	
			UIUtil.SetLabelValue(labelGameEditAssetSprite, GetItemAssetDisplayName(itemAsset.asset_code));		
			UIUtil.SetInputValue(inputSpriteEffect, itemAsset.destroy_effect_code);
			UIUtil.SetLabelValue(labelGameEditAssetSpriteEffect, GetItemAssetDisplayName(itemAsset.destroy_effect_code));	
						
			UIUtil.SetCheckboxValue(checkboxEditAssetDestructable, itemAsset.destructable);
			UIUtil.SetCheckboxValue(checkboxEditAssetKinematic, itemAsset.kinematic);
			UIUtil.SetCheckboxValue(checkboxEditAssetReactive, itemAsset.reactive);
			UIUtil.SetCheckboxValue(checkboxEditAssetGravity, itemAsset.gravity);
			
			UpdateRotation((float)itemAsset.rotation_speed.z);
		}
		
		UpdateDisplay();
	}
	
	void UpdateDisplay() {
		if(itemAsset != null) {
			if(itemAsset.destructable) {
				UIUtil.ShowInput(inputSpriteEffect);
				UIUtil.ShowLabel(labelGameEditAssetSpriteEffect);
				UIUtil.ShowButton(buttonGameEditAssetSpriteEffect);
			}
			else {
				UIUtil.HideInput(inputSpriteEffect);	
				UIUtil.HideLabel(labelGameEditAssetSpriteEffect);	
				UIUtil.HideButton(buttonGameEditAssetSpriteEffect);
			}
		}
	}
	
	public void UpdateRotation(float val) {
		UpdateRotation(val, false, false);
	}
	
	public void UpdateRotation(float val, bool deferSlider, bool deferInput) {
		if(itemAsset != null) {
			
			val = Mathf.Clamp(val, MIN_ROTATION_SPEED, MAX_ROTATION_SPEED);
			float sliderVal = NormalizeRotationSlider(val);
			
			if(!deferInput) {
				UIUtil.SetInputValue(inputRotationSpeed, val.ToString());
			}
			if(!deferSlider) {
				UIUtil.SetSliderValue(sliderRotationSpeed, sliderVal);
			}
			
			itemAsset.rotation_speed.FromVector3(Vector3.zero.WithZ(-val));
		}
	}
	
	public float NormalizeRotation(float actualRotation) {
		return actualRotation / MAX_ROTATION_SPEED;
	}
	
	public float DenormalizeRotation(float normalizedRotation) {
		return normalizedRotation * MAX_ROTATION_SPEED;
	}
	
	public float NormalizeRotationSlider(float actualRotation) {
		float adjusted = NormalizeRotation(actualRotation);
		
		// when 	1000/1000 	= 		1 	= 	1
		// when 	0/1000 		= 		0 	= 	.5
		// when 	-1000/1000 	= 		-1 	= 	0
		// 1/-1 = .5/x
		
		adjusted = adjusted * .5f + .5f;
		
		return adjusted;
	}
	
	public float DenormalizeRotationSlider(float normalizedRotation) {
		float adjusted = (normalizedRotation - .5f) / .5f;
		adjusted = DenormalizeRotation(adjusted);		
		return adjusted;
	}
	
	public void UpdateSprite(string assetCode) {
		if(itemAsset != null) {
			itemAsset.asset_code = assetCode;
			UIUtil.SetInputValue(inputSprite, itemAsset.asset_code);	
			UIUtil.SetLabelValue(labelAssetEdit, itemAsset.asset_code);
			UIUtil.SetLabelValue(labelGameEditAssetSprite, GetItemAssetDisplayName(itemAsset.asset_code));
			
			GameDraggableLevelItem levelItem = GameShooterController.Instance.GetCurrentDraggableLevelItem();
			if(levelItem != null) {
				levelItem.LoadSprite(itemAsset.asset_code);
			}
		}
	}
	
	public void UpdateSpriteEffect(string assetCode) {
		if(itemAsset != null) {
			itemAsset.destroy_effect_code = assetCode;
			UIUtil.SetInputValue(inputSpriteEffect, itemAsset.destroy_effect_code);	
			UIUtil.SetLabelValue(labelGameEditAssetSpriteEffect, GetItemAssetDisplayName(itemAsset.destroy_effect_code));	
		}
	}
	
	public void SyncCurrenItemAsset() {
		if(GameShooterController.Instance != null) {
			itemAsset = GameShooterController.Instance.GetCurrentLevelItemAsset();
		}
	}
	
	public void SaveDataAsset() {
		
		SyncCurrenItemAsset();
		
		if(itemAsset != null) {
			itemAsset.destroy_effect_code = UIUtil.GetInputValue(inputSpriteEffect);
			itemAsset.asset_code = UIUtil.GetInputValue(inputSprite);
			itemAsset.rotation_speed.FromVector3(Vector3.zero.WithZ(-float.Parse(UIUtil.GetInputValue(inputRotationSpeed))));
			itemAsset.destructable = UIUtil.GetCheckboxValue(checkboxEditAssetDestructable);
			itemAsset.kinematic = UIUtil.GetCheckboxValue(checkboxEditAssetKinematic);
			itemAsset.reactive = UIUtil.GetCheckboxValue(checkboxEditAssetReactive);
			itemAsset.gravity = UIUtil.GetCheckboxValue(checkboxEditAssetGravity);
		}
	}
	
	public string GetItemAssetDisplayName(string code) {
		AppContentAsset asset = AppContentAssets.Instance.GetById(code);
		if(asset != null) {
			return asset.display_name;
		}
		return code;
	}
	
	
	/*
	IEnumerator LoadDataCo() {
		
		yield return new WaitForSeconds(.1f);
		
		if (listGridRoot != null) {
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
		
			List<AppContentAsset> assets = AppContentAssets.Instance.GetAll();
		
	        Debug.Log("Load AppContentAsset: assets.Count: " + assets.Count);
			
			int i = 0;
			
			int totalPoints = 0;
			
	        foreach(AppContentAsset asset in assets) {
				
	            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
	            item.name = "AssetItem" + i;
	            item.transform.FindChild("LabelName").GetComponent<UILabel>().text = asset.display_name;
	            //item.transform.FindChild("LabelDescription").GetComponent<UILabel>().text = achievement.description;
				
				GameObject gameLevelItemObject = item.transform.FindChild("GameLevelItemObject").gameObject;	
				
				// clear current items
				
				foreach(Transform t in gameLevelItemObject.transform) {
					Destroy(t.gameObject);
				}
				
				GameObject go = GameShooterController.Instance.LoadSpriteUI(
					gameLevelItemObject, asset.code, Vector3.one);
				gameLevelItemObject.ChangeLayersRecursively("UILayer");
				
				float maxSize = 3;
				
				if(go != null) {
					PackedSprite sprite = go.GetComponent<PackedSprite>();
					if(sprite != null) {
						
						float adjust = 1;
						
						if(sprite.height > sprite.width) {
							if(sprite.height > maxSize) {
								adjust = maxSize/sprite.height;
							}
						}
						else {
							if(sprite.width > maxSize) {
								adjust = maxSize/sprite.width;
							}
						}
						
						go.transform.localScale = go.transform.localScale.WithX(adjust).WithY(adjust).WithZ(adjust);
					}
				}
				
				item.transform.FindChild("ButtonGameLevelItemObject").GetComponent<UIButton>().name 
						= "ButtonGameLevelItemObject$" + asset.code; ///levels[y].name;
				
				i++;
	        }
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
			
        }
	}
	*/
	
	void OnInputClickEventHandler(string inputName,int cam) {
		LogUtil.Log("OnInputClickEventHandler: inputName:" + inputName);
		
		if(inputName == inputSprite.name) {
			actionState = UIPanelEditAssetActionState.SELECT_ITEM;
			GameShooterUIController.Instance.ShowUIPanelDialogItems();
		}
		else if(inputName == inputSpriteEffect.name) {
			actionState = UIPanelEditAssetActionState.SELECT_EFFECT;
			GameShooterUIController.Instance.ShowUIPanelDialogItems();
		}
	}
	
	void OnInputChangeEventHandler(string inputName, string val) {
		LogUtil.Log("OnInputChangeEventHandler: val:" + val);
		
		if(itemAsset != null) {
			if(inputName == inputRotationSpeed.name) {
				
				float rotationValue = 0f;
				string inputValue = UIUtil.GetInputValue(inputRotationSpeed);
				if(!string.IsNullOrEmpty(inputValue)) {
					bool converted = float.TryParse(inputValue, out rotationValue);
					if(!converted) {
						rotationValue = 0f;
					}
				}
				
				if(itemAsset != null) {
					UpdateRotation(rotationValue, false, false);
				}
			}
		}
	}
	
    void OnSliderChangeEventHandler(string sliderName, float val) {
    	Debug.Log("SliderEvents:OnSliderChange: sliderName: " + sliderName + " changeValue:" + val);
		
		if(itemAsset != null) {
			if(sliderName == sliderRotationSpeed.name) {
				UpdateRotation(DenormalizeRotationSlider(val), true, false);
			}
		}
    }
		
	void OnCheckboxChangeEventHandler(string checkboxName, bool selected) {
        Debug.Log("OnCheckboxChangeEventHandler: checkboxName:" + checkboxName + " selected:" + selected );
		
		if(itemAsset != null) {
			if(checkboxName == checkboxEditAssetDestructable.name) {
				if(itemAsset != null) {
	        		itemAsset.destructable = selected;
					UpdateDisplay();
				}
	        }
			else if(checkboxName == checkboxEditAssetKinematic.name) {
	        	if(itemAsset != null) {
					itemAsset.kinematic = selected;
				}
	        }		
			else if(checkboxName == checkboxEditAssetReactive.name) {
	        	if(itemAsset != null) {
					itemAsset.reactive = selected;
				}
	        }
			else if(checkboxName == checkboxEditAssetGravity.name) {
	        	if(itemAsset != null) {
					itemAsset.gravity = selected;
				}
	        }
		}
	}
	
	void OnButtonClickEventHandler(string buttonName) {
		
		if(itemAsset != null) {
			if(buttonName == buttonGameEditAssetSave.name) {
				SaveDataAsset();
				GameShooterUIController.Instance.ResetAssetPanelRemoveDeselect();
				actionState = UIPanelEditAssetActionState.NONE;
			}
			else if(buttonName == buttonGameEditAssetDelete.name) {
				
				GameDraggableLevelItem levelItem = GameShooterController.Instance.GetCurrentDraggableLevelItem();
				if(levelItem != null) {
					levelItem.DestroyMeAnimated();
				}
				
				GameShooterUIController.Instance.ResetAssetPanelRemoveDeselect();
				actionState = UIPanelEditAssetActionState.NONE;
			}		
			else if(buttonName == buttonGameEditAssetDeselect.name) {
				GameShooterUIController.Instance.ResetAssetPanelRemoveDeselect();
				actionState = UIPanelEditAssetActionState.NONE;
			}		
			else if(buttonName == buttonGameEditAssetSprite.name) {
				actionState = UIPanelEditAssetActionState.SELECT_ITEM;
				GameShooterUIController.Instance.ShowUIPanelDialogItems();
			}		
			else if(buttonName == buttonGameEditAssetSpriteEffect.name) {
				actionState = UIPanelEditAssetActionState.SELECT_EFFECT;
				GameShooterUIController.Instance.ShowUIPanelDialogItems();
			}
					
			else if(buttonName == buttonGameEditAssetRotationReset.name) {
				UpdateRotation(0f, false, false);
			}
		}
	}
	
}
