using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UIPanelTrophyStatistics : UIAppPanel {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
	
	public static UIPanelTrophyStatistics Instance;		

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
		Debug.Log("Load GameStatistics: Init");
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
		
		yield return new WaitForSeconds(1f);
		
		Debug.Log("Load GameStatistics: LoadDataCo");
		
		if (listGridRoot != null) {
            foreach (Transform item in listGridRoot.transform) {
                Destroy(item.gameObject);
            }
		
			Debug.Log("Load GameStatistics: LoadDataCo 3");
			
			List<GameStatistic> statistics = GameStatistics.Instance.GetAll();
		
	        Debug.Log("Load statistics: statistics.Count: " + statistics.Count);
			
			int i = 0;
			
			//int totalPoints = 0;
			
	        foreach(GameStatistic statistic in statistics) {
				
	            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
	            item.name = "StatisticItem" + i;
	            item.transform.FindChild("LabelName").GetComponent<UILabel>().text = statistic.display_name;
	            item.transform.FindChild("LabelDescription").GetComponent<UILabel>().text = statistic.description;
				
				double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
				string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);
				
				item.transform.FindChild("LabelPoints").GetComponent<UILabel>().text = displayValue;				
								
				i++;
	        }
			
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
			
        }
	}
	
}
