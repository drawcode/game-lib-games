using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeProfileCharacters : UICustomizeSelectObject {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIInput inputCurrentDisplayCode;
    public UIImageButton buttonSave;
#else
    public InputField inputCurrentDisplayCode;
    public Button buttonSave;
#endif

    public string type = "character";

    GameProfileCharacterItem profileCharacterItem;

    public override void OnEnable() {
        base.OnEnable();
        
        Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CHANGE, OnInputChanged);
        //Messenger<string, string>.AddListener(InputEvents.EVENT_ITEM_CLICK, OnInputClicked);
    }
    
    public override void OnDisable() {
        base.OnDisable();
        
        Messenger<string, string>.RemoveListener(InputEvents.EVENT_ITEM_CHANGE, OnInputChanged);
        //Messenger<string, string>.RemoveListener(InputEvents.EVENT_ITEM_CLICK, OnInputClicked);
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

    void OnInputClicked(string controlName, string data) {
        
        //Debug.Log("OnInputClicked:" + " controlName:" + controlName + " data:" + data);
        
        
        //if(inputCurrentDisplayName != null 
        //   && controlName == inputCurrentDisplayName.name) {
        //
        //}
        //else if(inputCurrentDisplayCode != null 
        //       && controlName == inputCurrentDisplayCode.name) {
        //
        //}
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
        else if(UIUtil.IsButtonClicked(buttonSave, buttonName)) {
            SaveInputs();
        }
    }

    public virtual void SaveInputs() {
        SaveCharacterDisplayNameInput();
        SaveCharacterDisplayCodeInput();

        GameCustomController.BroadcastCustomCharacterDisplayChanged();
    }

    public virtual void SaveCharacterDisplayNameInput() {
        
        if(inputCurrentDisplayName == null) {
            return;
        } 

        ChangeCharacterDisplayName(inputCurrentDisplayName.text);
    }

    public virtual void SaveCharacterDisplayCodeInput() {        
        
        if(inputCurrentDisplayCode == null) {
            return;
        } 
        
        ChangeCharacterDisplayCode(inputCurrentDisplayCode.text);
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

                GameProfileCharacters.Current.SetCurrentCharacterProfileCode(profileCharacterItem.code);
                                
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
