using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeProfileCharacters : UICustomizeSelectObject {

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

        if (UIUtil.IsButtonClicked(buttonCycleLeft, buttonName)) {
            ChangePresetNext();
        }
        else if (UIUtil.IsButtonClicked(buttonCycleRight, buttonName)) {
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
        
        GameProfileCharacterItems gameProfileCharacterItems 
            = GameProfileCharacters.Current.GetCharacters();

        int countPresets = gameProfileCharacterItems.items.Count;

        if (index < -1) {
            index = countPresets - 1;    
        }
        
        if (index > countPresets - 1) {
            index = -1;
        }
        
        currentIndex = index;

        if (index > -2 && index < countPresets) {
            
            if (initialProfileCustomItem == null) {
                initialProfileCustomItem = GameProfileCharacters.currentCustom;
            }
            
            currentProfileCustomItem = GameProfileCharacters.currentCustom;
            
            if (index == -1) {
                
                UIUtil.SetLabelValue(labelCurrentDisplayName, "My Previous Uniform");



                //GameCustomController.UpdateTexturePresetObject(
                //    initialProfileCustomItem, currentObject, type);
            }
            else {

                GameProfileCharacterItem profileCharacterItem = 
                    gameProfileCharacterItems.items[currentIndex];

                //GameCustomController.SaveCustomItem(currentProfileCustomItem);

                UIUtil.SetLabelValue(labelCurrentDisplayName, profileCharacterItem.code);
            }
        }
    }
    
    public override void Update() {
        
    }
}
