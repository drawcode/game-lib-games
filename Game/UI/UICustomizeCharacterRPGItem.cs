using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UICustomizeCharacterRPGItem : MonoBehaviour {
	
	public string rpgCode = "energy"; // attack, defense, energy, health, skill, power
	
	public double currentValue = 0.1;
	public double profileValue = 0.1;
	
	public GameItemRPG gameItemRPG;
	
	public UISlider sliderProfileValue;
	public UISlider sliderCurrentValue;
	
	public UIImageButton buttonRPGItemUp;
	public UIImageButton buttonRPGItemDown;
	
	public UILabel labelName;
	
	void Start() {
		LoadData();
	}
	
	void OnEnable() {
		Messenger<GameObject>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectHandler);
		Messenger<string, double>.AddListener(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, OnRPGItemHandler);
	}
	
	void OnDisable() {
		Messenger<GameObject>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectHandler);
		Messenger<string, double>.RemoveListener(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, OnRPGItemHandler);	
	}
	
	void OnButtonClickObjectHandler(GameObject go) {
		
		if(go == buttonRPGItemUp.gameObject) {
			Up(rpgCode);
		}
		else if(go == buttonRPGItemDown.gameObject) {
			Down(rpgCode);		
		}	
	}	
	
	void OnRPGItemHandler(string rpgCode, double val) {
		currentValue += val;
		UIUtil.SetSliderValue(sliderCurrentValue, (float)currentValue);
	}
	
	public void LoadData() {
		// TODO select characters
		string displayName = "";
		double val = 0.1f;
		
		gameItemRPG = GameProfileCharacters.Current.GetCurrentCharacter().characterRPG;
		
		if(rpgCode.ToLower() == GameItemRPGAttributes.attack) {
			displayName = "Stiffarm";
		}
		else if(rpgCode.ToLower() == GameItemRPGAttributes.attack) {
			displayName = "Stiffarm";
		}
		
		
		UIUtil.SetSliderValue(sliderProfileValue, (float)gameItemRPG.energy);
		UIUtil.SetSliderValue(sliderCurrentValue, (float)gameItemRPG.energy);
	}
	
	public void SetName(string nameTo) {
		if(labelName != null) {
			labelName.text = nameTo;
		}
	}
	
	public void Load(string rpgCodeTo) {
		rpgCode = rpgCodeTo;
		LoadData();
	}
		
	public void Up(string rpgCode) {	
		Messenger<string, double>.Broadcast(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, rpgCode, .1);
	}
	
	public void Down(string rpgCode) {
		Messenger<string, double>.Broadcast(UICustomizeCharacterRPGItemMessages.rpgItemCodeChanged, rpgCode, -.1);	
	}

}
