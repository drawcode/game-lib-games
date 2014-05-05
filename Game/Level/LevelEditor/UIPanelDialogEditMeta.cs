using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelDialogEditMeta : UIAppPanel {
	
	
    public GameObject listItemPrefab;
	
	public UIInput inputName;
	public UIInput inputAmmo;
	
	public static UIPanelDialogEditMeta Instance;
	
	void Awake() {
		if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
		
        Instance = this;
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
		
		LoadData();
	}
	
	public void LoadData() {
		StartCoroutine(LoadDataCo());
	}
	
	IEnumerator LoadDataCo() {
		
		GameLevel currentLevel = GameLevels.Current;
		
		if(inputName != null) {
			inputName.text = currentLevel.display_name;
		}
		
		
		if(inputAmmo != null) {
			inputAmmo.text = "90";//currentLevel.display_name;
		}
		
		
		
		yield break;
		/*
		
		LogUtil.Log("Load GameWorlds: LoadDataCo");
		
		
		LogUtil.Log("Load GameWorlds: LoadDataCo 2");
		
		if (listGridRoot != null) { 
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
			
			LogUtil.Log("Load GameWorlds: LoadDataCo 3");
		
			List<GameWorld> worlds = GameWorlds.Instance.GetAll();
		
	        LogUtil.Log("Load GameWorlds: worlds.Count: " + worlds.Count);
			
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
				
				item.transform.FindChild("LabelPoints").GetComponent<UILabel>().text = points;				
			
				// Get trophy icon
				
				i++;
	        }
			
	        //yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
			
        }
        */
	}
	
}
