using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelWorlds : UIAppPanel {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
		
	void Awake() {
	
	}
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		LoadData();
	}
	
	public void LoadData() {
		StartCoroutine(LoadDataCo());
	}
	
	IEnumerator LoadDataCo() {
		
		Debug.Log("Load GameWorlds: LoadDataCo");
		
		
		Debug.Log("Load GameWorlds: LoadDataCo 2");
		
		if (listGridRoot != null) { 
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
			
			Debug.Log("Load GameWorlds: LoadDataCo 3");
		
			List<GameWorld> worlds = GameWorlds.Instance.GetAll();
		
	        Debug.Log("Load GameWorlds: worlds.Count: " + worlds.Count);
			
			int i = 0;
			
	        foreach(GameWorld world in worlds) {
				
	            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
	            item.name = "WorldItem" + world.sort_order;
	            item.transform.FindChild("LabelWorld").GetComponent<UILabel>().text = world.name;
	            item.transform.FindChild("LabelWorldDisplayName").GetComponent<UILabel>().text = world.display_name;
	            item.transform.FindChild("LabelWorldDescription").GetComponent<UILabel>().text = world.description;
				
				//GameObject iconObject = item.transform.FindChild("Icon").gameObject;	
				//UISprite iconSprite = iconObject.GetComponent<UISprite>();
				
				
				//bool completed = GameProfiles.Current.CheckIfAttributeExists(world.code);
				
				//if(completed) {
				//	completed = GameProfiles.Current.GetAchievementValue(world.code);
				//}
				
				//string points = "";
				
				/*
				if(completed) {
					int currentPoints = achievement.points;
					totalPoints += currentPoints;
					points = currentPoints.ToString();
					
					if(iconSprite != null) {
						iconSprite.alpha = 1f;
					}
				}	
				else {
					if(iconSprite != null) {
						iconSprite.alpha = .33f;
					}
				}
				
				item.transform.FindChild("LabelPoints").GetComponent<UILabel>().text = points;				
				*/
				// Get trophy icon
				
				i++;
	        }
			
	        //yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
			
        }
	}
	
}
