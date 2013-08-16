using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelDialogEditItemsFilter {
	public static string all = "all";
	public static string levelAssets = "level-assets";
	public static string levelEnvironment = "level-environments";
	public static string levelEffect = "level-effects";
}

public class UIPanelDialogEditItems : UIAppPanel {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
	
	public static UIPanelDialogEditItems Instance;
	
	public string filterType = UIPanelDialogEditItemsFilter.all;
	
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
	
	public void LoadData(string levelAssetKey) {
		filterType = levelAssetKey;
		LoadData();
	}
	
	public void LoadData() {
		StartCoroutine(LoadDataCo());
	}
	
	IEnumerator LoadDataCo() {
		
		yield return new WaitForSeconds(.1f);
		
		if (listGridRoot != null) {
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
		
			List<AppContentAsset> assets = AppContentAssets.Instance.GetAll();
		
	        Debug.Log("Load AppContentAsset: assets.Count: " + assets.Count);
			
			int i = 0;
			
			//int totalPoints = 0;
			
	        foreach(AppContentAsset asset in assets) {
				
				if(filterType != UIPanelDialogEditItemsFilter.all) {
					if(asset.key != filterType) {
						continue;
					}
				}
				
	            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
	            item.name = "AssetItem" + i;
	            item.transform.FindChild("LabelName").GetComponent<UILabel>().text = asset.display_name;
	            //item.transform.FindChild("LabelDescription").GetComponent<UILabel>().text = achievement.description;
				
				GameObject gameLevelItemObject = item.transform.FindChild("GameLevelItemObject").gameObject;	
				
				// clear current items
				
				foreach(Transform t in gameLevelItemObject.transform) {
					Destroy(t.gameObject);
				}
				
				if(GameShooterController.Instance == null) {
					yield break;
				}
				
				
				GameObject go = GameShooterController.Instance.LoadSpriteUI(
					gameLevelItemObject, asset.code, Vector3.one);
				gameLevelItemObject.ChangeLayersRecursively("UILayer");
				
				float maxSize = 3;
				
				if(go != null) {
					PackedSprite sprite = go.GetComponent<PackedSprite>();
					if(sprite != null) {
						
						float adjust = 1;
						
						if(sprite.height > sprite.width) {
							if(sprite.height > maxSize) {
								adjust = maxSize/sprite.height;
							}
						}
						else {
							if(sprite.width > maxSize) {
								adjust = maxSize/sprite.width;
							}
						}
						
						go.transform.localScale = go.transform.localScale.WithX(adjust).WithY(adjust).WithZ(adjust);
					}
				}
				
				item.transform.FindChild("ButtonGameLevelItemObject").GetComponent<UIButton>().name 
						= "ButtonGameLevelItemObject$" + asset.code; ///levels[y].name;
				
				if(filterType == UIPanelDialogEditItemsFilter.all) {
					
				}
				
				i++;
	        }
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
			
        }
	}
	
}
