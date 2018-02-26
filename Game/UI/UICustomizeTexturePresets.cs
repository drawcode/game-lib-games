using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeTexturePresets : UICustomizeSelectObject {

    public string type = "character";

    string characterModelCode;
    string characterModelCodeLast;

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
        //characterModelCode = "";
        //characterModelCodeLast = "changeme";
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
        
        GameProfileCharacterItem gameProfileCharacterItem = 
            GameProfileCharacters.Current.GetCurrentCharacter();

        GameCharacter gameCharacter = 
            GameCharacters.Instance.GetById(gameProfileCharacterItem.characterCode);

        if (gameCharacter == null) {
            return;
        }

        GameDataModel gameDataModel = gameCharacter.data.GetModel();

        if (gameDataModel == null) {
            return;
        }

        characterModelCode = gameDataModel.code;

        List<AppContentAssetTexturePreset> assetTexturePresets =
            AppContentAssetTexturePresets.Instance.GetListLike(BaseDataObjectKeys.code, characterModelCode);

        if(assetTexturePresets == null || assetTexturePresets.Count == 0) {
            return;
        }

        int countPresets = assetTexturePresets.Count;

        if(index < -1) {
            index = countPresets - 1;
        }

        if(index > countPresets - 1) {
            index = -1;
        }

        currentIndex = index;

        if(index > -2 && index < countPresets) {

            //if(characterModelCode != characterModelCodeLast) {
            //    initialProfileCustomItem = null;
            //}

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
                    assetTexturePresets[currentIndex];

                // change character to currently selected texture preset

                currentProfileCustomItem =
                    GameCustomController.UpdateTexturePresetObject(
                        currentProfileCustomItem, currentObject, preset);

                GameCustomController.SaveCustomItem(currentProfileCustomItem);

                UIUtil.SetLabelValue(labelCurrentDisplayName, preset.display_name);
            }
        }
    }

    //public override void Update() {
    //
    //}
}