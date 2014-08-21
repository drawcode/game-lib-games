using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeProfileCharacters : UICustomizeSelectObject {

    public string type = "character";
    public UIInput inputCurrentDisplayCode;

    GameProfileCharacterItem profileCharacterItem;

    public override void OnEnable() {
        base.OnEnable();
        
        Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnInputChanged);
    }
    
    public override void OnDisable() {
        base.OnDisable();
        
        Messenger<string, string>.RemoveListener(InputEvents.EVENT_ITEM_CHANGE, OnInputChanged);
    }
    
    void OnInputChanged(string controlName, string data) {

        Debug.Log("OnInputChanged:" + " controlName:" + controlName + " data:" + data);

        
        if(inputCurrentDisplayName != null 
           && controlName == inputCurrentDisplayName.name) {

            ChangeCharacterDisplayName(data);
        }
        else if(inputCurrentDisplayCode != null 
                && controlName == inputCurrentDisplayCode.name) {
            
            ChangeCharacterDisplayCode(data);
        }
    }

    public override void Start() {
        Load();   
    }

    public override void Load() {
        base.Load();

        ShowCurrentProfileCharacter();
    }

    public void ShowCurrentProfileCharacter() {
        
        GameProfileCharacterItems gameProfileCharacterItems =
            GameProfileCharacters.Current.GetCharacters();
        
        int countPresets = gameProfileCharacterItems.items.Count;
        int index = 0;
        
        string currentCharacterCode = 
            GameProfileCharacters.Current.GetCurrentCharacterProfileCode();
        
        foreach(GameProfileCharacterItem gameProfileCharacterItem 
                in gameProfileCharacterItems.items) {
            if(gameProfileCharacterItem.code == currentCharacterCode) {
                ChangePreset(index);
                break;
            }
            index++;
        }

        if(index == countPresets - 1) {
            ChangePreset(0);
        }

    }

    public override void OnButtonClickEventHandler(string buttonName) {

        if (UIUtil.IsButtonClicked(buttonCycleLeft, buttonName)) {
            ChangePresetPrevious();
        }
        else if (UIUtil.IsButtonClicked(buttonCycleRight, buttonName)) {
            ChangePresetNext();
        }
    }

    public virtual void ChangeCharacterDisplayName(string val) {

        if(inputCurrentDisplayName == null) {
            return;
        }        
        
        if(profileCharacterItem == null) {
            return;
        }        

        if(string.IsNullOrEmpty(val)) {
            return;
        }

        Debug.Log("ChangeCharacterDisplayName:" + " val:" + val);

        UIUtil.SetInputValue(inputCurrentDisplayName, val);

        profileCharacterItem.characterDisplayName = val;
        GameProfileCharacters.Current.SetCharacter(profileCharacterItem);

        GameState.SaveProfile();            
    }
    
    public virtual void ChangeCharacterDisplayCode(string val) {
        
        if(inputCurrentDisplayCode == null) {
            return;
        }        

        if(profileCharacterItem == null) {
            return;
        }        

        if(string.IsNullOrEmpty(val)) {
            return;
        }

        Debug.Log("ChangeCharacterDisplayCode:" + " val:" + val);

        UIUtil.SetInputValue(inputCurrentDisplayCode, val);

        profileCharacterItem.characterDisplayCode = val;
        GameProfileCharacters.Current.SetCharacter(profileCharacterItem);
                
        GameState.SaveProfile();
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

        if (index < 0) {
            index = countPresets - 1;    
        }
        
        if (index > countPresets - 1) {
            index = 0;
        }
        
        currentIndex = index;

        if (index > -1 && index < countPresets) {
            
            if (initialProfileCustomItem == null) {
                initialProfileCustomItem = GameProfileCharacters.currentCustom;
            }
            
            currentProfileCustomItem = GameProfileCharacters.currentCustom;
            
            if (index == -1) {
                
                UIUtil.SetLabelValue(labelCurrentDisplayName, "Previous");
                UIUtil.SetLabelValue(labelCurrentType, "");


                //GameCustomController.UpdateTexturePresetObject(
                //    initialProfileCustomItem, currentObject, type);
            }
            else {

                profileCharacterItem = 
                    gameProfileCharacterItems.items[currentIndex];

                //GameCustomController.SaveCustomItem(currentProfileCustomItem);
                                
                Messenger<string>.Broadcast(
                    GameCustomMessages.customCharacterPlayerChanged, profileCharacterItem.code);

                string characterType = "";
                GameCharacter gameCharacter = GameCharacters.Instance.GetById(profileCharacterItem.characterCode);
                if(gameCharacter != null) {
                    characterType = gameCharacter.display_name;
                    characterType = "- TYPE: " + characterType + " -";
                }
                
                UIUtil.SetInputValue(inputCurrentDisplayName, profileCharacterItem.characterDisplayName);
                UIUtil.SetLabelValue(labelCurrentDisplayName, profileCharacterItem.characterDisplayName);
                UIUtil.SetLabelValue(labelCurrentType, characterType);
                
                UIUtil.SetInputValue(inputCurrentDisplayCode, profileCharacterItem.characterDisplayCode);

                UIUtil.SetLabelValue(labelCurrentStatus, string.Format("{0}/{1}", index + 1, countPresets));
            }
        }
    }
    
    public override void Update() {
        
    }
}
