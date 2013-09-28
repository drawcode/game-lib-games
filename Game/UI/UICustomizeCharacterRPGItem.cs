using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UICustomizeCharacterRPGItem : MonoBehaviour {
 
    public string rpgCode = "energy"; // attack, defense, energy, health, skill, power
    public string characterCode = "default";
    public double currentValue = 0.1;
    public double profileValue = 0.1;
    public GameItemRPG gameItemRPG;
    public UISlider sliderProfileValue;
    public UISlider sliderCurrentValue;
    public UIImageButton buttonRPGItemUp;
    public UIImageButton buttonRPGItemDown;
    public UILabel labelName;
    public UILabel labelValue;

    public string displayName = "";
    public string displayValue = "";

    void Start() {
        LoadData();
    }

    void OnEnable() {
        Messenger<GameObject>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectHandler);
        Messenger<string, string, double>.AddListener(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, OnRPGItemHandler);
    }
 
    void OnDisable() {
        Messenger<GameObject>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectHandler);
        Messenger<string, string, double>.RemoveListener(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, OnRPGItemHandler);
    }
 
    void OnButtonClickObjectHandler(GameObject go) {

        if(go == buttonRPGItemUp.gameObject) {
            Up();
        }
        else if(go == buttonRPGItemDown.gameObject) {
            Down();      
        }    
    }

    void OnRPGItemHandler(string rpgCodeFrom, string characterCodeFrom, double valFrom) {

        if(rpgCode == rpgCodeFrom && characterCode == characterCodeFrom
            && UICustomizeCharacterRPG.Instance.upgradesAvailable > 0) {

            double val = currentValue + valFrom;

            if(val < profileValue
                || val > 1.0) {
                return;
            }

            currentValue += valFrom;

            UpdateControls();
        }
    }

    public void LoadData() {
        StartCoroutine(loadDataCo());
    }
 
    public IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);

        currentValue = 0.1f;

        gameItemRPG = GameProfileCharacters.Current.GetCurrentCharacterRPG();    

        if(rpgCode.ToLower() == GameItemRPGAttributes.attack) {
            displayName = RPGConfigs.displayNameAttack;
            currentValue = gameItemRPG.attack;
        }
        else if(rpgCode.ToLower() == GameItemRPGAttributes.defense) {
            displayName = RPGConfigs.displayNameDefense;
            currentValue = gameItemRPG.defense;
        }
        else if(rpgCode.ToLower() == GameItemRPGAttributes.defense) {
            displayName = RPGConfigs.displayNameDefense;
            currentValue = gameItemRPG.defense;
        }
        else if(rpgCode.ToLower() == GameItemRPGAttributes.energy) {
            displayName = RPGConfigs.displayNameEnergy;
            currentValue = gameItemRPG.energy;
        }
        else if(rpgCode.ToLower() == GameItemRPGAttributes.health) {
            displayName = RPGConfigs.displayNameHealth;
            currentValue = gameItemRPG.health;
        }
        else if(rpgCode.ToLower() == GameItemRPGAttributes.jump) {
            displayName = RPGConfigs.displayNameJump;
            currentValue = gameItemRPG.jump;
        }
        else if(rpgCode.ToLower() == GameItemRPGAttributes.speed) {
            displayName = RPGConfigs.displayNameSpeed;
            currentValue = gameItemRPG.speed;
        }

        UpdateControls();
        SetProfileValue(currentValue);
    }

    public void UpdateControls() {
        SetDisplayName(displayName);
        SetDisplayValue();
        SetValues();
    }

    public void SetValues() {
      SetValues(currentValue);
    }
 
    public void SetValues(double val) {
        SetCurrentValue(val);
        SetProfileValue(profileValue);
    }

    public void SetCurrentValue(double val) {
        currentValue = Math.Round(val, 1);
        UIUtil.SetSliderValue(sliderCurrentValue, currentValue);
    }

    public void SetProfileValue(double val) {
        profileValue = Math.Round(val, 1);
        UIUtil.SetSliderValue(sliderProfileValue, profileValue);
    }

    public void SetDisplayName(string nameTo) {
        UIUtil.SetLabelValue(labelName, nameTo);
    }

    public void SetDisplayValue() {
        double modifier = 10;
        double currentSliderValue = currentValue * modifier;
        double currentSliderMaxValue = 1 * modifier;

        displayValue = string.Format("{0}/{1}",
            currentSliderValue.ToString("N0"),
            currentSliderMaxValue.ToString("N0"));

        UIUtil.SetLabelValue(labelValue, displayValue);
    }
 
    public void Load(string rpgCodeTo) {
        Load(rpgCodeTo, GameProfileCharacters.Current.GetCurrentCharacterCode());
    }
 
    public void Load(string rpgCodeTo, string characterCodeTo) {
        rpgCode = rpgCodeTo;
        characterCode = characterCodeTo;
     
        LoadData();
    }
     
    public void Up() {   
        Messenger<string, string, double>.Broadcast(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, rpgCode, characterCode, .1);
    }
 
    public void Down() {
        Messenger<string, string, double>.Broadcast(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, rpgCode, characterCode, -.1);
    }

}
