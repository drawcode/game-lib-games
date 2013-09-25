using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UICustomizeCharacterRPGItemMessages {
	public static string rpgItemCodeChanged = "rpg-item-code-change";
}

//Messenger<string, double>.AddListener(UIRPGItemMessages.rpgItemCodeUp, OnRPGItemCodeUp);
//Messenger<string, double>.RemoveListener(UIRPGItemMessages.rpgItemCodeUp, OnRPGItemCodeDown);
//Messenger<string, double>.Broadcast(UIRPGItemMessages.rpgItemCodeUp, rpgCode, 1);

public class UICustomizeCharacterRPG: UIAppPanelBaseList {
			
	public UILabel labelUpgradesAvailable;
	
	public UIImageButton buttonResetRPG;
	public UIImageButton buttonSaveRPG;
	
	public UICustomizeCharacterRPGItem rpgItemHealth;
	public UICustomizeCharacterRPGItem rpgItemEnergy;
	public UICustomizeCharacterRPGItem rpgItemSpeed;
	public UICustomizeCharacterRPGItem rpgItemAttack;
	public UICustomizeCharacterRPGItem rpgItemDefense;
	
	
	
	public string characterCode;
	
	void Start() {
		
	}
	
	void OnEnable() {
		Messenger<GameObject>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectHandler);
	}
	
	void OnDisable() {
		Messenger<GameObject>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectHandler);	
	}
	
	void OnButtonClickObjectHandler(GameObject go) {
		
		//if(go == buttonSaveRPG.gameObject) {
		//
		//}
		//else if(go == buttonResetRPG.gameObject) {
		//
		//}
	
	}
	
	public static void Save(string rpgCode) {
		//Messenger<string, double>.Broadcast(UIRPGItemMessages.rpgItemCodeUp, rpgCode, 1);
	}
	
	public static void Reset() {
		
	}

}
