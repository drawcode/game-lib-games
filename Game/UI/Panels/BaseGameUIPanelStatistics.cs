using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelStatistics : GameUIPanelBase {	
    
    public static GameUIPanelStatistics Instance;
	
    public GameObject listItemStatisticPrefab;
    
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
    }

    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {		
		
		LogUtil.Log("LoadDataCo");
		
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
			
	        yield return new WaitForEndOfFrame();
					
			loadDataStatistics();
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();				
        }
	}
	
		
    public virtual void loadDataStatistics() {
		
		LogUtil.Log("Load Statistics:");
				
		List<GameStatistic> statistics = GameStatistics.Instance.GetAll();
	
        LogUtil.Log("Load statistics: statistics.Count: " + statistics.Count);
		
		int i = 0;
		
        foreach(GameStatistic statistic in statistics) {

         double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
         string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);

            //if(statValue > .1) {

                GameObject item = NGUITools.AddChild(listGridRoot, listItemStatisticPrefab);
                item.name = "StatisticItem" + i;
                item.transform.FindChild("Container/LabelName").GetComponent<UILabel>().text = statistic.display_name;
                item.transform.FindChild("Container/LabelDescription").GetComponent<UILabel>().text = statistic.description;
    
    			
    			item.transform.FindChild("Container/LabelPoints").GetComponent<UILabel>().text = displayValue;				
    							
    			i++;
            //}
        }
	}
	
    public virtual void ClearList() {
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
		}
	}
			
	public override void AnimateIn() {
		
        base.AnimateIn();
        
        GameUIPanelHeader.ShowCharacter();

        GameUIPanelFooter.ShowButtonsAchievements();

        //AdNetworks.ShowAd(AdBannerType.SmartBannerLandscape, AdPosition.BottomCenter);
		
		loadData();
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		
		ClearList();
	}
	
	
}
