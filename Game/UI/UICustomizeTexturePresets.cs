using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeTexturePresets : UICustomizeSelectObject {

    public string type = "character";

    public override void OnEnable() {
        base.OnEnable();
    }
    
    public override void OnDisable() {
        base.OnDisable();
    }

    public override void Start() {
        Load();   
    }

    public override void Load() {
        base.Load();
    }

    public override void OnButtonClickEventHandler(string buttonName) {

        if(UIUtil.IsButtonClicked(buttonCycleLeft, buttonName)) {
            ChangePresetNext();
        }
        else if(UIUtil.IsButtonClicked(buttonCycleRight, buttonName)) {
            ChangePresetPrevious();
        }
    }

    public void ChangePresetNext() {
        ChangePreset(currentIndex + 1);
    }

    public void ChangePresetPrevious() {
        ChangePreset(currentIndex - 1);
    }

    public void ChangePreset(int index) {

        int countPresets = 
            AppContentAssetTexturePresets.Instance.GetListByType(type).Count;

        
        if (index < -1) {
            index = countPresets - 1;    
        }
        
        if (index > countPresets - 1) {
            index = -1;
        }
        
        currentIndex = index;
        
        if (index > -2 && index < countPresets) {
            
            if(initialProfileCustomItem == null) {
                initialProfileCustomItem = GameProfileCharacters.currentCustom;
            }
            
            currentProfileCustomItem = GameProfileCharacters.currentCustom;
            
            if(index == -1) {
                
                UIUtil.SetLabelValue(labelCurrentDisplayName, "My Previous Uniform");

                GameCustomController.UpdateTexturePresetObject(
                    initialProfileCustomItem, currentObject, type);
            }
            else {

                AppContentAssetTexturePreset preset = 
                    AppContentAssetTexturePresets.Instance.GetListByType(type)[currentIndex];

                AppColorPreset presetColor = 
                    AppColorPresets.Instance.GetListByType(type)[currentIndex];

                // change character to currently selected texture preset

                currentProfileCustomItem = 
                    GameCustomController.UpdateTexturePresetObject(
                        currentProfileCustomItem, currentObject, preset);

                //profileCustomItem = GameCustomController.UpdateColorPresetObject(profileCustomItem, currentObject, presetColor);

                GameCustomController.SaveCustomItem(currentProfileCustomItem);

                UIUtil.SetLabelValue(labelCurrentDisplayName, preset.display_name);
            }
        }
    }
    
    public override void Update() {
        
    }
}
