using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelLevels : UIAppPanelBaseList {
 
    
    public GameObject listItemPrefab;
    public GameObject listItemSetPrefab;
     
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
     
        LogUtil.Log("Load GameLevels: LoadDataCo");
     
     
        LogUtil.Log("Load GameLevels: LoadDataCo 2");
     
        if(listGridRoot != null) { 
            foreach(Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
         
            LogUtil.Log("Load GameLevels: LoadDataCo 3");
     
            List<GameLevel> levels = GameLevels.Instance.GetAll();//GetByWorldId("world-original");
     
            LogUtil.Log("Load GameLevels: levels.Count: " + levels.Count);
                     
            double panelCount = Math.Floor((double)(levels.Count / 20));
         
            LogUtil.Log("Load GameLevels: panelCount: " + panelCount);
         
            for(int k = 0; k < (int)panelCount; k++) {              
             
                GameObject itemSet = NGUITools.AddChild(listGridRoot, listItemSetPrefab);
                itemSet.name = "LevelSet" + k;
             
                UIGrid listSetGrid = itemSet.transform.FindChild("LevelContainer").GetComponent<UIGrid>();
                GameObject listSetGridObject = listSetGrid.gameObject;
         
                for(int y = (k * 20); y < ((20) * (k + 1)); y++) {
                 
                    GameObject item = NGUITools.AddChild(listSetGridObject, listItemPrefab);
                    item.name = "LevelItem" + y;
                    item.transform.FindChild("LabelWorld").GetComponent<UILabel>().text 
                     = (y + 1).ToString();//levels[y].name;
                    item.transform.FindChild("ButtonPlayLevel").GetComponent<UIImageButton>().name 
                     = "ButtonPlayLevel$" + (k + 1).ToString() + "-" + (y + 1).ToString(); ///levels[y].name;
                 
                    // TODO find stars/skulls
                    // TODO find play
                    // TODO find lock
                                     
                }            
             
                listSetGrid.Reposition();               
            }
         
            foreach(UIGrid grid in ObjectUtil.FindObjects<UIGrid>()) {
                //yield return new WaitForEndOfFrame();
                grid.Reposition();
                //yield return new WaitForEndOfFrame();
            }
                     
            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();
         
        }
    }
 
}
