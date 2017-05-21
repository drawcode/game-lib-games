using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelAchievements : GameUIPanelBase {
		
    public GameObject listItemAchievementPrefab;
    
    public static GameUIPanelAchievements Instance; 
    
    public virtual void Awake() {

    }
    
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
		
    public override void OnButtonClickEventHandler(string buttonName) {
		//LogUtil.Log("OnButtonClickEventHandler: " + buttonName);

    }
	
	public static void LoadData() {
		if(GameUIPanelAchievements.Instance != null) {
            GameUIPanelAchievements.Instance.loadData();
		}
	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {		
		
		LogUtil.Log("LoadDataCo");
		
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
			
	        yield return new WaitForEndOfFrame();
					
			loadDataAchievements();
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();				
        }
	}
	
    public virtual void loadDataAchievements() {
		
		LogUtil.Log("Load Achievements:");
		
		List<GameAchievement> achievements = GameAchievements.Instance.GetAll();	
		
        LogUtil.Log("Load Achievements: achievements.Count: " + achievements.Count);
				
		int i = 0;
		
		double totalPoints = 0;
		
        foreach(GameAchievement achievement in achievements) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemAchievementPrefab);
            item.name = "AchievementItem" + i;
            item.transform.Find("Container/LabelName").GetComponent<UILabel>().text = achievement.display_name;
            item.transform.Find("Container/LabelDescription").GetComponent<UILabel>().text = achievement.description;
			
			GameObject iconObject = item.transform.Find("Container/Icon").gameObject;	
			UISprite iconSprite = iconObject.GetComponent<UISprite>();
			
			
			bool completed = false;

            bool hasValue = GameProfileAchievements.Current.CheckIfAttributeExists(achievement.code);
			
			if(hasValue) {
				completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code);
			}
			
			if(!hasValue) {
				completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code + "_" + achievement.pack_code);
			}
			
			string points = "";
			
			if(completed) {
                double currentPoints = achievement.data.points;
				totalPoints += currentPoints;				
                
                if(GameConfigs.useCoinRewardsForAchievements) {
                    currentPoints *= (int)GameConfigs.coinRewardAchievementPoint;  
                }

                points = "+" + currentPoints.ToString();
				
				if(iconSprite != null) {
					iconSprite.alpha = 1f;
                }
                //item.transform.FindChild("Container/ContainerComplete").gameObject.Show(); 
			}	
			else {
				if(iconSprite != null) {
					iconSprite.alpha = .33f;
                }
                //item.transform.FindChild("Container/ContainerComplete").gameObject.Hide(); 
            }

            item.transform.Find("Container/LabelPoints").GetComponent<UILabel>().text = points;				
			
			// Get trophy icon
			
			i++;
        }
		
		//if(labelPoints != null) {
		//	labelPoints.text = totalPoints.ToString("N0");
		//}
	}
	
    public virtual void ClearList() {
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
		}
	}

    public override void HandleShow() {
        base.HandleShow();
        
        buttonDisplayState = UIPanelButtonsDisplayState.GameNetworks;
        characterDisplayState = UIPanelCharacterDisplayState.Character;
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
    }
		
	public override void AnimateIn() {
		
		base.AnimateIn();
        
        UIPanelCommunityBroadcast.HideBroadcastRecordPlayShare();

		loadData();
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		
		ClearList();
	}
}
