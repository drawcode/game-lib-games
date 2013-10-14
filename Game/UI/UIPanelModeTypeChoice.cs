#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelModeTypeChoice : UIPanelBase {

	public static UIPanelModeTypeChoice Instance;

    public GameObject containerChoiceOverview;
    public GameObject containerChoiceDisplayItem;
    public GameObject containerChoiceResultItem;
    public GameObject containerChoiceResults;

    // GLOBAL CHOICE

    // OVERVIEW

    public UILabel labelOverviewTip;
    public UILabel labelOverviewType;
    public UILabel labelOverviewStatus;

    public UILabel labelOverviewTitle;
    public UILabel labelOverviewBlurb;
    public UILabel labelOverviewBlurb2;
    public UILabel labelOverviewNextSteps;

    public UIImageButton buttonOverviewAdvance;

    // DISPLAY ITEM

    public UILabel labelDisplayItemTip;
    public UILabel labelDisplayItemType;
    public UILabel labelDisplayItemStatus;

    public UILabel labelDisplayItemTitle;
    public UILabel labelDisplayItemBlurb;
    public UILabel labelDisplayItemBlurb2;
    public UILabel labelDisplayItemNextSteps;

    public UIImageButton buttonDisplayItemAdvance;

    // RESULT ITEM

    public UILabel labelResultItemTip;
    public UILabel labelResultItemType;
    public UILabel labelResultItemStatus;

    public UILabel labelResultItemTitle;
    public UILabel labelResultItemBlurb;
    public UILabel labelResultItemBlurb2;
    public UILabel labelResultItemNextSteps;

    public UIImageButton buttonResultItemAdvance;

    // RESULTS

    public UILabel labelResultsTip;
    public UILabel labelResultsType;
    public UILabel labelResultsStatus;

    public UILabel labelResultsTitle;
    public UILabel labelResultsBlurb;
    public UILabel labelResultsBlurb2;
    public UILabel labelResultsNextSteps;

    public UIImageButton buttonResultsAdvance;


    public AppModeTypeChoiceFlowState flowState = AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview;

    public Dictionary<string, AppContentChoice> choices;
    public AppContentChoicesData appContentChoicesData;

    int currentChoice = 0;
    public AppContentChoice currentAppContentChoice;

	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
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
	
	public override void Init() {
		base.Init();

		loadData();
	}	
	
	public override void Start() {
		Init();
	}
	
    void OnEnable() {
        Messenger<AppContentChoiceItem>.AddListener(AppContentChoiceMessages.appContentChoiceItem, OnAppContentChoiceItemHandler);
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    void OnDisable() {
        Messenger<AppContentChoiceItem>.RemoveListener(AppContentChoiceMessages.appContentChoiceItem, OnAppContentChoiceItemHandler);
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    void OnAppContentChoiceItemHandler(AppContentChoiceItem choiceItem) {
        CheckChoicesData();

        AppContentChoiceData choiceData = new AppContentChoiceData();
        choiceData.choiceCode = choiceItem.code;
        choiceData.choices.Add(choiceItem);
        choiceData.choiceData = "";

        if(currentChoice != null) {
            appContentChoicesData.SetChoice(choiceData);
        }
    }

    void OnButtonClickEventHandler(string buttonName) {
        //if(UIUtil.IsButtonClicked(button, buttonName)) {
           // HideAll();

            //GameController.LoadLevelAssets("1-1");
        //}
    }

    // SET CONTENT

    public void CheckChoices() {
        if(choices == null) {
            choices = new Dictionary<string, AppContentChoice>();
        }
    }

    public void CheckChoicesData() {
        if(choices == null) {
            appContentChoicesData = new AppContentChoicesData();
        }
    }

    public void LoadChoices(int total) {

        CheckChoices();
        CheckChoicesData();

        choices.Clear();
        appContentChoicesData = new AppContentChoicesData();

        List<AppContentChoice> choicesFilter = AppContentChoices.Instance.GetAll();

        // Randomize
        choicesFilter.Shuffle();

        int countChoices = choicesFilter.Count;

        // select total choices to try

        for(int i = 0; i < total; i++) {
            AppContentChoice choice = choicesFilter[i];
            choices.Add(choice.code, choice);
        }
    }

    // GLOBAL

    public void SetStatus(string val) {
        UIUtil.SetLabelValue(labelOverviewStatus, val);
    }

    public void SetTip(string val) {
        UIUtil.SetLabelValue(labelOverviewTip, val);
    }

    public void SetType(string val) {
        UIUtil.SetLabelValue(labelOverviewType, val);
    }

    // STATES

    public void ProcessNextItem() {

        CheckChoices();
        CheckChoicesData();

        bool correctOnly = true;

        if(appContentChoicesData.choices.Count < choices.Count) {
            // Process next question
        }
        else if(appContentChoicesData.choices.Count == choices.Count) {
           // && appContentChoiceData.(correctOnly)) {
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResults);
        }
    }

    public void ShowCurrentState() {
        CheckChoices();
        CheckChoicesData();
        HideStates();

        Debug.Log("UIPanelModeTypeChoice:ShowCurrentState:flowState:" + flowState);

        if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview) {
            DisplayStateOverview();
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem) {
            DisplayStateDisplayItem();
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem) {
            DisplayStateResultItem();
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceResults) {
            DisplayStateResults();
        }
    }

    public void ChangeState(AppModeTypeChoiceFlowState state) {

        flowState = state;

        ShowCurrentState();
    }

    public string GetStatusItemProgress() {
        return string.Format("question: {0} / {1} ", appContentChoicesData.choices.Count, choices.Count);
    }

    public string GetStatusOverview() {
        return string.Format("{0} questions", choices.Count);
    }

    public void DisplayStateOverview() {

        Debug.Log("UIPanelModeTypeChoice:DisplayStateOverview:flowState:" + flowState);

        SetStatus(GetStatusOverview());

        ShowOverview();
    }

    // SHOW / HIDE

    public void HideStates() {
        HideOverview();
        HideDisplayItem();
        HideResultItem();
        HideResults();
    }

    public void ShowOverview() {
        AnimateInBottom(containerChoiceOverview);
    }

    public void HideOverview() {
        AnimateOutTop(containerChoiceOverview);
    }

    public void ShowDisplayItem() {
        AnimateInBottom(containerChoiceDisplayItem);
    }

    public void HideDisplayItem() {
        AnimateOutBottom(containerChoiceDisplayItem);
    }

    public void ShowResultItem() {
        AnimateInBottom(containerChoiceResultItem);
    }

    public void HideResultItem() {
        AnimateOutBottom(containerChoiceResultItem);
    }

    public void ShowResults() {
        AnimateInBottom(containerChoiceResults);
    }

    public void HideResults() {
        AnimateOutBottom(containerChoiceResults);
    }

    public void DisplayStateDisplayItem() {
        Debug.Log("UIPanelModeTypeChoice:DisplayStateDisplayItem:flowState:" + flowState);

        SetStatus(GetStatusItemProgress());

        ShowDisplayItem();
    }

    public void DisplayStateResultItem() {
        Debug.Log("UIPanelModeTypeChoice:DisplayStateResultItem:flowState:" + flowState);

        SetStatus(GetStatusItemProgress());

        ShowResultItem();
    }

    public void DisplayStateResults() {

        Debug.Log("UIPanelModeTypeChoice:DisplayStateResults:flowState:" + flowState);

        SetStatus("Results");

        ShowResults();
    }


    // SHOW/LOAD

    public static void ShowDefault() {
        if(isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if(isInst) {
            Instance.AnimateOut();
        }
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

        ShowOverview();
    }

    public override void AnimateIn() {
        base.AnimateIn();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideStates();
    }
	
}
