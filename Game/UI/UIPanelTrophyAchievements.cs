using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelTrophyAchievements : UIAppPanelBaseList {
	
	
    public GameObject listItemPrefab;
	public UILabel labelPoints;

	public static UIPanelTrophyAchievements Instance;		

	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
	}
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		loadData();
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
		
		yield return new WaitForSeconds(.5f);
		
		if (listGridRoot != null) {
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
		
			List<GameAchievement> achievements = GameAchievements.Instance.GetAll();
		
	        LogUtil.Log("Load Achievements: achievements.Count: " + achievements.Count);
			
			int i = 0;
			
			double totalPoints = 0;
			
	        foreach(GameAchievement achievement in achievements) {
				
	            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
	            item.name = "AchievementItem" + i;
	            item.transform.Find("LabelName").GetComponent<UILabel>().text = achievement.display_name;
	            item.transform.Find("LabelDescription").GetComponent<UILabel>().text = achievement.description;
				
				GameObject iconObject = item.transform.Find("Icon").gameObject;	
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
                    double currentPoints = achievement.data.points;
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
				item.transform.Find("LabelPoints").GetComponent<UILabel>().text = points;				
				
				// Get trophy icon
				
				i++;
	        }
			
			if(labelPoints != null) {
				labelPoints.text = totalPoints.ToString("N0");
			}
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
			
        }
	}
	
}
