using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class BaseGameUIPanelResults : GameUIPanelBase {
    
    public static GameUIPanelResults Instance;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelContentStateDisplayName;
#else
    public Text labelContentStateDisplayName;
#endif

    public GameObject listItemPrefab;
	
    public GameObject containerModes;
    
    public virtual void Awake() {
        
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
	
	public static bool isInst {
		get {
			if(GameUIPanelResults.Instance != null) {
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
		
		loadData();
	}
		 
    public override void OnButtonClickEventHandler(string buttonName) {
		//LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
	}

    public virtual void ShowContentState() {

        if(containerModes != null) {

            foreach (GameObjectInactive inactive in containerModes.GetComponentsInChildren<GameObjectInactive>(true)) {
                if (inactive.type.IsEqualLowercase(BaseDataObjectKeys.app_content_state)) {
                    inactive.gameObject.Hide();
                }
            }

            foreach (GameObjectInactive inactive in containerModes.GetComponentsInChildren<GameObjectInactive>(true)) {
                if(inactive.code.ToLower() == AppContentStates.Current.code.ToLower()
                    && inactive.type.IsEqualLowercase(BaseDataObjectKeys.app_content_state)) {
                    inactive.gameObject.Show();
                }
            }
        }
    }
	
    public virtual void UpdateDisplay(GamePlayerRuntimeData runtimeData, float timeTotal) {

        ShowContentState();

        UIUtil.SetLabelValue(labelContentStateDisplayName, AppContentStates.Current.display_name);

        if(AppContentStates.Instance.isAppContentStateGameChallenge) {

#if ENABLE_FEATURE_MODE_CHALLENGE
            foreach(GameUIPanelResultsChallenge result in containerModes.GetComponentsInChildren<GameUIPanelResultsChallenge>(true)) {
                result.UpdateDisplay(runtimeData, timeTotal);
            }
#endif
        }

#if ENABLE_FEATURE_MODE_TRAINING
        else if(AppContentStates.Instance.isAppContentStateGameTrainingChoiceQuiz) {
            //foreach(UIPanelResultsChoiceQuiz result in containerModes.GetComponentsInChildren<UIPanelResultsChoiceQuiz>(true)) {
            //    result.UpdateDisplay(runtimeData, timeTotal);
            //}
        }
        else if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSafety) {
            //foreach(UIPanelResultsCollectionSafety result in containerModes.GetComponentsInChildren<UIPanelResultsCollectionSafety>(true)) {
            //    result.UpdateDisplay(runtimeData, timeTotal);
            //}
        }
        else if(AppContentStates.Instance.isAppContentStateGameTrainingCollectionSmarts) {
            //foreach(UIPanelResultsCollectionSmarts result in containerModes.GetComponentsInChildren<UIPanelResultsCollectionSmarts>(true)) {
            //    result.UpdateDisplay(runtimeData, timeTotal);
            //}
        }

#endif

        else { // if(AppContentStates.Instance.isAppContentStateGameArcade) {
            foreach(GameUIPanelResultsArcade result in containerModes.GetComponentsInChildren<GameUIPanelResultsArcade>(true)) {
                result.UpdateDisplay(runtimeData, timeTotal);
            }
        }
	}
	
	public static void LoadData() {
        if(GameUIPanelResults.Instance != null) {
            GameUIPanelResults.Instance.loadData();
		}
	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		yield return new WaitForSeconds(1f);

        ShowContentState();
	}

    public virtual void HandleItems() {

        // Handle by world

        string codeWorld = GameWorlds.Current.code;

        if (codeWorld.IsNullOrEmpty()) {
            return;
        }

        foreach (GameObjectInactive container in gameObject.GetList<GameObjectInactive>()) {

            if (container.type.IsEqualLowercase(BaseDataObjectKeys.display_items)) {

                foreach (GameObjectInactive item in container.gameObject.GetList<GameObjectInactive>()) {

                    if (item.type.IsEqualLowercase(BaseDataObjectKeys.display_item)
                        && !item.type.IsEqualLowercase(BaseDataObjectKeys.display_items)) {
                        item.gameObject.HideChildren();
                    }
                }    

                foreach (GameObjectData dataItem in container.gameObject.GetList<GameObjectData>()) {

                    Dictionary<string, object> data = dataItem.ToDictionary();

                    string val = data.Get<string>(BaseDataObjectKeys.world);

                    if (val.IsEqualLowercase(codeWorld)) {

                        dataItem.gameObject.Show();
                    }
                }
            }
        }
    }
    
    public override void HandleShow() {
        base.HandleShow();
        
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
        adDisplayState = UIPanelAdDisplayState.BannerBottom;
        characterDisplayState = UIPanelCharacterDisplayState.Character;
        buttonDisplayState = UIPanelButtonsDisplayState.GameNetworks;
    }

    public override void AnimateIn() {
        base.AnimateIn();

        HandleItems();

        loadData();        
        
        GameCommunity.ShowSharesCenter();

        Messenger.Broadcast(GameMessages.gameResultsStart);
    }
        
    public override void AnimateOut() {

        Messenger.Broadcast(GameMessages.gameResultsEnd);
                
        GameCommunity.HideSharesCenter();

        base.AnimateOut();
    }
	
}
