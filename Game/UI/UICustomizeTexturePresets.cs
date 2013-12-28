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

        ChangePreset(currentIndex);
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

        if (index < 0) {
            index = countPresets - 1;    
        }
        
        if (index > countPresets - 1) {
            index = 0;
        }

        currentIndex = index;
        
        Debug.Log("ChangePreset:texture:" + " index:" + index + " countPresets:" + countPresets);

        if (index > -1 && index < countPresets) {

            AppContentAssetTexturePreset preset = 
                AppContentAssetTexturePresets.Instance.GetListByType(type)[currentIndex];

            // change character to currently selected texture preset

            GameCustomController.UpdateTexturePresetObject(currentObject, preset, true);

                UIUtil.SetLabelValue(labelCurrentDisplayName, preset.display_name);
        }
    }
    
    public override void Update() {
        
    }
}
