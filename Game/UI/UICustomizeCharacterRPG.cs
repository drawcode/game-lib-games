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
	public UIImageButton buttonBuyUpgrades;
	
	UICustomizeCharacterRPGItem rpgItemHealth;
	UICustomizeCharacterRPGItem rpgItemEnergy;
	UICustomizeCharacterRPGItem rpgItemSpeed;
	UICustomizeCharacterRPGItem rpgItemAttack;
	UICustomizeCharacterRPGItem rpgItemDefense;
	
	GameItemRPG gameItemRPG;
	
	public static UICustomizeCharacterRPG Instance;
	
	public GameObject listItemPrefab;
		
	public static bool isInst {
		get {
			if(Instance != null) {
				return true;
			}
			return false;
		}
	}
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();
		
		loadData();
	}
	
    void OnEnable() {
		Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
	}
		
    void OnDisable() {
		Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
	}
		
    void OnButtonClickEventHandler(string buttonName) {
		//Debug.Log("OnButtonClickEventHandler: " + buttonName);
    }
	
	public void SetUpgradesAvailable(double upgradesAvailable) {
		if(labelUpgradesAvailable != null) {
			labelUpgradesAvailable.text = upgradesAvailable.ToString("N0");
		}
	}
	
	public static void LoadData() {
		if(Instance != null) {
			Instance.loadData();
		}
	}
	
	public void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {		
		
		Debug.Log("LoadDataCo");
		
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
			
	        yield return new WaitForEndOfFrame();
					
			loadDataRPG();
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();				
        }
	}	
		
	public void loadDataRPG() {
		
		Debug.Log("Load RPGs:");
		
		int i = 0;
		
		SetUpgradesAvailable(GameProfileRPGs.Current.GetUpgrades());
		
		gameItemRPG = GameProfileCharacters.Current.GetCurrentCharacterRPG();
		
		rpgItemSpeed = new UICustomizeCharacterRPGItem();
		rpgItemHealth = new UICustomizeCharacterRPGItem();
		rpgItemEnergy = new UICustomizeCharacterRPGItem();
		rpgItemAttack = new UICustomizeCharacterRPGItem();
		//rpgItemDefense = new UICustomizeCharacterRPGItem();
		
		List<UICustomizeCharacterRPGItem> rpgItems = new List<UICustomizeCharacterRPGItem>();
		
		rpgItemSpeed.Load(GameItemRPGAttributes.speed);
		rpgItems.Add(rpgItemSpeed);		
		
		rpgItemHealth.Load(GameItemRPGAttributes.health);
		rpgItems.Add(rpgItemHealth);		
		
		rpgItemEnergy.Load(GameItemRPGAttributes.energy);
		rpgItems.Add(rpgItemEnergy);		
		
		rpgItemAttack.Load(GameItemRPGAttributes.attack);
		rpgItems.Add(rpgItemAttack);		
		
		//rpgItemDefense.Load(GameItemRPGAttributes.defense);
		//rpgItems.Add(rpgItemDefense);
		
        foreach(UICustomizeCharacterRPGItem rpgItem in rpgItems) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
            item.name = "AItem" + i;
			
            //item.transform.FindChild("Container/LabelName").GetComponent<UILabel>().text = statistic.display_name;
            //item.transform.FindChild("Container/LabelDescription").GetComponent<UILabel>().text = statistic.description;
			
			//double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
			//string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);
			
			//item.transform.FindChild("Container/LabelPoints").GetComponent<UILabel>().text = displayValue;				
							
			i++;
        }
	}
}
