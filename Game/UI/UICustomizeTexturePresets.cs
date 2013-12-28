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

    public void UpdateObject(AppContentAssetTexturePreset preset) {
    
        if(currentObject != null) {

            string path = ContentPaths.appCacheVersionSharedMaterials;
            
            //Debug.Log("UpdateObject:" + " path:" + path);

            foreach(AppContentAssetCustomItem customItem in 
                    AppContentAssetCustomItems.Instance.GetListByType(type)) {
                
                //Debug.Log("UpdateObject:" + " customItem:" + customItem.code);

                foreach(AppContentAssetCustomItemProperty prop in customItem.properties) {

                    if(prop.IsTypeTexture()) {
                        string pathMaterial = path + prop.code + "-" + preset.code;
                        currentObject.SetMaterialSwap(prop.code, pathMaterial);
                        
                        //Debug.Log("UpdateObject:preset:" + " prop.code:" + prop.code);
                        //Debug.Log("UpdateObject:preset:" + " pathMaterial:" + pathMaterial);
                    }
                }
            }
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


            UpdateObject(preset);

            UIUtil.SetLabelValue(labelCurrentDisplayName, preset.display_name);

        }
    }
    
    public override void Update() {
        
    }
}
