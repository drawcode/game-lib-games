using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelGameModeMission : GameUIPanelBase {
    
    public static GameUIPanelGameModeMission Instance;
	
    public GameObject listItemPrefab;
	    
    public UIImageButton buttonGamePlayEpisode1;
    public UIImageButton buttonGamePlayEpisode2;
    public UIImageButton buttonGamePlayEpisode3;
    
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
		//if(UIUtil.IsButtonClicked(buttonGamePlay, buttonName)) {
		
        //}
	}	

    public static void LoadData() {
        if(GameUIPanelGameModeMission.Instance != null) {
            GameUIPanelGameModeMission.Instance.loadData();
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
            
            loadDataMissions();
            
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();               
        }
    }
    
    public virtual void loadDataMissions() {
        
        LogUtil.Log("Load Achievements:");
        
        List<GameAchievement> achievements = GameAchievements.Instance.GetAll();    
        
        LogUtil.Log("Load Achievements: achievements.Count: " + achievements.Count);
        
        int i = 0;
        
        //int totalPoints = 0;
        
        foreach(AppContentCollect mission in AppContentCollects.Instance.GetMissions()) {

            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
            item.name = "MissionItem" + i;
            
            UIUtil.UpdateLabelObject(item, "Container/LabelName", mission.display_name);
            UIUtil.UpdateLabelObject(item, "Container/LabelDescription", mission.description);

            //GameObject iconObject = item.transform.FindChild("Container/Icon").gameObject;  
            //UISprite iconSprite = iconObject.GetComponent<UISprite>();            
            
            //bool completed = false;
            
            //bool hasValue = GameProfileAchievements.Current.CheckIfAttributeExists(achievement.code);
            
            //if(hasValue) {
                //completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code);
            //}
            
            //if(!hasValue) {
                //completed = GameProfileAchievements.Current.GetAchievementValue(achievement.code + "_" + achievement.pack_code);
            //}

            /*
            string points = "";
            
            if(completed) {
                int currentPoints = achievement.points;
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
            */
            
            //item.transform.FindChild("Container/LabelPoints").GetComponent<UILabel>().text = points;                
            
            // Get trophy icon
            
            i++;
        }
        
        //if(labelPoints != null) {
        //  labelPoints.text = totalPoints.ToString("N0");
        //}
    }
    
    public virtual void ClearList() {
        if (listGridRoot != null) {
            listGridRoot.DestroyChildren();
        }
    }
    
    public override void AnimateIn() {
        
        base.AnimateIn();        
        
        GameUIPanelHeader.ShowCharacter();
        
        GameUIPanelFooter.ShowButtonsCharacterCustomize();
        
        loadData();
    }
    
    public override void AnimateOut() {
        
        base.AnimateOut();
        
        ClearList();
    }
	
}
