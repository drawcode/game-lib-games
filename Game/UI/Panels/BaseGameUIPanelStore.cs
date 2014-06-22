using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public enum GameUIPanelStoreListType {
	Clothing,
	Weapons,
	Powerups,
	Stats, 
	Achievements,
	Quests,
	All
}

public class BaseGameUIPanelStore : GameUIPanelBase {
    
    public static GameUIPanelStore Instance;    	
	
    public GameObject listItemStatisticPrefab;
    public GameObject listItemAchievementPrefab;
    public GameObject listItemQuestPrefab;
    public GameObject listItemPowerupPrefab;
    public GameObject listItemClothingPrefab;
    public GameObject listItemWeaponPrefab;
	
	public GameObject containerMain;
	public GameObject containerList;
	
	public UIImageButton buttonSatchelClothing;
	public UIImageButton buttonSatchelWeapons;
	public UIImageButton buttonSatchelPowerups;
	public UIImageButton buttonSatchelQuests;
	public UIImageButton buttonSatchelStats;
	public UIImageButton buttonSatchelTrophies;
	
	public UIImageButton buttonClose;
	
	public GameUIPanelStoreListType panelListType = GameUIPanelStoreListType.Clothing;
	
	public string productCodeUse = "character-bot-1";
	public string productTypeUse = "default";
    public string productCharacterUse = "bot";
    
    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {
        
    }
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		loadData();
		
		base.AnimateIn();	
	}

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
           //
        }
    }
		
    public virtual void OnButtonClickEventHandler(string buttonName) {
		//LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
		
		if(buttonName == buttonSatchelClothing.name) {
			changeList(GameUIPanelStoreListType.Clothing);
		}
		else if(buttonName.IndexOf("ButtonSatchelClothing$") > -1) {
			
			// Use costume
			
			productCodeUse = "";
			productTypeUse = "";
			productCharacterUse = "";
			
			string[] commandActionParams = buttonName.Replace("ButtonSatchelClothing$", "").Split('$');
			
			if(commandActionParams.Length > 0)
				productTypeUse = commandActionParams[0];
			if(commandActionParams.Length > 1)
				productCodeUse = commandActionParams[1];
			if(commandActionParams.Length > 2)
				productCharacterUse = commandActionParams[2];
			
			string weaponType = "ranged";
			if(productCharacterUse == "jaime") {
				weaponType = "melee";
			}
			
			if(!string.IsNullOrEmpty(productTypeUse)
				&& !string.IsNullOrEmpty(productCodeUse)
				&& !string.IsNullOrEmpty(productCharacterUse)) {
				
				GameProfileCharacters.Current.SetCurrentCharacterCode(productCharacterUse);				
				
				// TODO CHECK if can use or buy.. for now grant power and control 
				// and access beyond all virtual currency bounds...
				//GameProfileCharacters.Current.SetCharacterCode(productCodeUse);
				
				if(productTypeUse == "costume") {					
				
					GameCharacterSkin skin = GameCharacterSkins.Instance.GetById(productCodeUse);
					GameCharacterSkinItemRPG rpg = skin.GetGameCharacterSkinByData(productCharacterUse, weaponType);
					if(rpg != null) {						
						GameProfileCharacters.Current.SetCurrentCharacterCostumeCode(rpg.prefab);						
					}										
				}				
			}			
		}
		else if(buttonName == buttonSatchelWeapons.name) {
			changeList(GameUIPanelStoreListType.Weapons);
		}
		else if(buttonName == buttonSatchelPowerups.name) {
			changeList(GameUIPanelStoreListType.Powerups);
		}
		else if(buttonName == buttonSatchelStats.name) {
			//changeList(GameUIPanelStoreListType.Stats);
			GameUIPanelStatistics.Instance.AnimateIn();
		}
		else if(buttonName == buttonSatchelTrophies.name) {
			changeList(GameUIPanelStoreListType.Achievements);
		}
		else if(buttonName == buttonSatchelQuests.name) {
			changeList(GameUIPanelStoreListType.Quests);
		}
    }
	
    public virtual void changeList(GameUIPanelStoreListType listType) {
		panelListType = listType;
		loadData();
		AnimateInList();
	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {		
		
		LogUtil.Log("LoadDataCo");
		
		if (listGridRoot != null) {
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
			
	        yield return new WaitForEndOfFrame();
					
            if(panelListType == GameUIPanelStoreListType.Clothing) {
				loadDataClothing();
			}
            else if(panelListType == GameUIPanelStoreListType.Weapons) {
				loadDataWeapons();
			}
            else if(panelListType == GameUIPanelStoreListType.Powerups) {
				loadDataPowerups();
			}
            else if(panelListType == GameUIPanelStoreListType.Stats) {
				loadDataStatistics();
			}
            else if(panelListType == GameUIPanelStoreListType.Achievements) {
				loadDataAchievements();
			}
            else if(panelListType == GameUIPanelStoreListType.Quests) {
				loadDataQuests();
			}			
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();				
        }
	}
	
    public virtual void loadDataQuests() {
		
	}
	
    public virtual void loadDataClothing() {				
		loadDataProduct("character-skin");
	}
	
    public virtual void loadDataProduct(string type) {
		LogUtil.Log("Load loadDataProduct:" + type);
				
		List<GameProduct> products = GameProducts.Instance.GetListByType(type);
	
        LogUtil.Log("Load skins: products.Count: " + products.Count);
		
		int i = 0;
		
        foreach(GameProduct product in products) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemClothingPrefab);

            item.name = "WeaponItem" + i;

            item.transform.FindChild("Container/LabelName").GetComponent<UILabel>().text = 
                product.GetDefaultProductInfoByLocale().display_name;

            item.transform.FindChild("Container/LabelDescription").GetComponent<UILabel>().text = 
                product.GetDefaultProductInfoByLocale().description;
			
			GameObject iconObject = item.transform.FindChild("Container/Icon").gameObject;	

			UISprite iconSprite = iconObject.GetComponent<UISprite>();			

			if(iconSprite != null) {
				iconSprite.alpha = 1f;
				
				// TODO change out image...
			}
			
			// Update button action
			
			
			Transform buttonObject = item.transform.FindChild("Container/ButtonAction");
			if(buttonObject != null) {
				UIImageButton button = buttonObject.gameObject.GetComponent<UIImageButton>();
				if(button != null) {
					
					// TODO change to get from character skin
					string productType = "costume";
					string productCode = product.code;
					string productCharacter = "norah";
					
					productCode = productCode.Replace(productType + "-", "");
					
					if(productCode.IndexOf("jaime") > -1) {
						productCharacter = "jaime";
					}
										
					button.name = "ButtonSatchelClothing$" + productType + "$" + productCode + "$" + productCharacter;
				}
			}
			
			
			
			
			
			
			i++;
        }
	}
	
    public virtual void loadDataWeapons() {
				
		LogUtil.Log("Load Weapons:");
				
		List<GameWeapon> weapons = GameWeapons.Instance.GetAll();
	
        LogUtil.Log("Load weapons: weapons.Count: " + weapons.Count);
		
		int i = 0;
		
        foreach(GameWeapon weapon in weapons) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemWeaponPrefab);
            item.name = "WeaponItem" + i;
            item.transform.FindChild("Container/LabelName").GetComponent<UILabel>().text = weapon.display_name;
            item.transform.FindChild("Container/LabelDescription").GetComponent<UILabel>().text = weapon.description;
			
			GameObject iconObject = item.transform.FindChild("Container/Icon").gameObject;	
			UISprite iconSprite = iconObject.GetComponent<UISprite>();			

			if(iconSprite != null) {
				iconSprite.alpha = 1f;
				
				// TODO change out image...
			}
			
			i++;
        }
	}
	
    public virtual void loadDataPowerups() {		
		loadDataProduct("powerup");		
	}
	
    public virtual void loadDataStatistics() {
		
		LogUtil.Log("Load Statistics:");
				
		List<GameStatistic> statistics = GameStatistics.Instance.GetAll();
	
        LogUtil.Log("Load statistics: statistics.Count: " + statistics.Count);
		
		int i = 0;
		
        foreach(GameStatistic statistic in statistics) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemStatisticPrefab);
            item.name = "StatisticItem" + i;
            item.transform.FindChild("Container/LabelName").GetComponent<UILabel>().text = statistic.display_name;
            item.transform.FindChild("Container/LabelDescription").GetComponent<UILabel>().text = statistic.description;
			
			double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
			string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);
			
			item.transform.FindChild("Container/LabelPoints").GetComponent<UILabel>().text = displayValue;				
							
			i++;
        }
	}
	
    public virtual void loadDataAchievements() {
		
		LogUtil.Log("Load Achievements:");
		
		List<GameAchievement> achievements = GameAchievements.Instance.GetAll();	
		
        LogUtil.Log("Load Achievements: achievements.Count: " + achievements.Count);
				
		int i = 0;
		
		int totalPoints = 0;
		
        foreach(GameAchievement achievement in achievements) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemAchievementPrefab);
            item.name = "AchievementItem" + i;
            item.transform.FindChild("Container/LabelName").GetComponent<UILabel>().text = achievement.display_name;
            item.transform.FindChild("Container/LabelDescription").GetComponent<UILabel>().text = achievement.description;
			
			GameObject iconObject = item.transform.FindChild("Container/Icon").gameObject;	
			UISprite iconSprite = iconObject.GetComponent<UISprite>();
			
			
			bool completed = GameProfiles.Current.CheckIfAttributeExists(achievement.code);
			
			if(completed) {
				completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code);
			}
			
			if(!completed) {
				completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code + "_" + achievement.pack_code);
			}
			
			string points = "";
			
			if(completed) {
				int currentPoints = achievement.data.points;
				totalPoints += currentPoints;
				points = "+" + currentPoints.ToString();
				
				if(iconSprite != null) {
					iconSprite.alpha = 1f;
				}
			}	
			else {
				if(iconSprite != null) {
					iconSprite.alpha = .33f;
				}
			}
			item.transform.FindChild("Container/LabelPoints").GetComponent<UILabel>().text = points;				
			
			// Get trophy icon
			
			i++;
        }
		
		//if(labelPoints != null) {
		//	labelPoints.text = totalPoints.ToString("N0");
		//}
	}
	
    public virtual void ShowMain() {
		if(containerMain != null) {
			UITweenerUtil.MoveTo(containerMain, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, 0f, Vector3.zero.WithY(0));		
			
			UITweenerUtil.FadeTo(containerMain, 
				UITweener.Method.Linear, UITweener.Style.Once, .3f, 0f, 1f);
			
			panelMode = UIAppPanelMode.ModeMain;
		}
	}
	
    public virtual void HideMain() {
		if(containerMain != null) {
			UITweenerUtil.MoveTo(containerMain, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .2f, 0f, Vector3.zero.WithY(bottomClosedY));			
			
			UITweenerUtil.FadeTo(containerMain, 
				UITweener.Method.Linear, UITweener.Style.Once, .2f, 0f, 0f);
		}
	}	
	
    public virtual void ShowList() {
		if(containerList != null) {
			UITweenerUtil.MoveTo(containerList, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, 0f, Vector3.zero.WithY(0));		
			
			UITweenerUtil.FadeTo(containerList, 
				UITweener.Method.Linear, UITweener.Style.Once, .3f, 0f, 1f);
			
			panelMode = UIAppPanelMode.ModeList;
		}
	}
	
    public virtual void HideList() {
		if(containerList != null) {
			UITweenerUtil.MoveTo(containerList, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .2f, 0f, Vector3.zero.WithY(bottomClosedY));			
			
			UITweenerUtil.FadeTo(containerList, 
				UITweener.Method.Linear, UITweener.Style.Once, .2f, 0f, 0f);
		}
	}	
	
	public override void AnimateIn() {
		
		base.AnimateIn();
		
		AnimateInMain();		
	}
	
    public virtual void AnimateInMain() {
				
		HideList();
		ShowMain();
	}
	
    public virtual void AnimateInList() {
		
		HideMain();
		ShowList();
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		
		HideMain();
		HideList();
	}
	
}
