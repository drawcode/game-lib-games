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
    public UILabel labelDisplayItemAnswers;
    public UILabel labelDisplayItemNote;
    public UILabel labelDisplayItemQuestion;

    public UIImageButton buttonDisplayItemAdvance;

    // RESULT ITEM

    public UILabel labelResultItemTip;
    public UILabel labelResultItemType;
    public UILabel labelResultItemStatus;

    public UILabel labelResultItemChoiceAnswer;
    public UILabel labelResultItemChoiceResult;
    public UILabel labelResultItemChoiceValue;
    public UILabel labelResultItemNextSteps;

    public UIImageButton buttonResultItemAdvance;

    // RESULTS

    public UILabel labelResultsTip;
    public UILabel labelResultsType;
    public UILabel labelResultsStatus;

    public UILabel labelResultsTitle;
    public UILabel labelResultsCoinsValue;
    public UILabel labelResultsScorePercentageValue;
    public UILabel labelResultsScoreFractionValue;

    public UIImageButton buttonResultsAdvance;

    // GLOBAL

    public AppModeTypeChoiceFlowState flowState = AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview;

    public Dictionary<string, AppContentChoice> choices;
    public AppContentChoicesData appContentChoicesData;

    int currentChoice = 0;
    public int choicesToLoad = 5;
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

    // EVENTS
	
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
        if(UIUtil.IsButtonClicked(buttonDisplayItemAdvance, buttonName)) {
            Advance();
        }
        else if(UIUtil.IsButtonClicked(buttonOverviewAdvance, buttonName)) {
            Advance();
        }
        else if(UIUtil.IsButtonClicked(buttonResultItemAdvance, buttonName)) {
            Advance();
        }
        else if(UIUtil.IsButtonClicked(buttonResultsAdvance, buttonName)) {
            Advance();
        }
    }

    // ADVANCE/NEXT

    public void Advance() {

        Debug.Log("Advance:flowState:" + flowState);

        if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview) {

            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem);
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem) {

            // Set question info and level info if not loaded

            //GameController.LoadLevelAssets("1-1");

            //ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem);
            HideStates();
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem) {

            HideStates();
            NextChoice();

        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceResults) {

            // set check game over flow

            HideStates();
        }
    }

    public void NextChoice() {
        currentChoice += 1;

        if(appContentChoicesData.choices.Count >= choices.Count) {
            // Advance to results
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResults);
        }
        //
            //GameController.LoadLevelAssets("1-1");
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
        Debug.Log("LoadChoices:countChoices:" + countChoices);

        // select total choices to try

        for(int i = 0; i < total; i++) {
            AppContentChoice choice = choicesFilter[i];
            choices.Add(choice.code, choice);
            Debug.Log("LoadChoices:choice.code:" + choice.code);
        }

        Debug.Log("LoadChoices:choices.Count:" + choices.Count);
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
        if(flowState != state) {
            flowState = state;
        }

        Debug.Log("UIPanelModeTypeChoice:ChangeState:flowState:" + flowState);

        ShowCurrentState();
    }

    // STATUS/TEXT

    public string GetStatusItemProgress() {
        return string.Format("question: {0} / {1} ", appContentChoicesData.choices.Count + 1, choices.Count);
    }

    public string GetStatusOverview() {
        return string.Format("{0} questions", choices.Count);
    }

    // DISPLAY

    public void DisplayStateOverview() {

        Debug.Log("UIPanelModeTypeChoice:DisplayStateOverview:flowState:" + flowState);

        UIUtil.SetLabelValue(labelOverviewStatus, GetStatusOverview());

        ShowOverview();
    }

    public void DisplayStateDisplayItem() {
        Debug.Log("UIPanelModeTypeChoice:DisplayStateDisplayItem:flowState:" + flowState);

        UIUtil.SetLabelValue(labelDisplayItemStatus, GetStatusItemProgress());

        ShowDisplayItem();
    }

    public void DisplayStateResultItem() {
        Debug.Log("UIPanelModeTypeChoice:DisplayStateResultItem:flowState:" + flowState);

        UIUtil.SetLabelValue(labelResultItemStatus, GetStatusItemProgress());

        ShowResultItem();
    }

    public void DisplayStateResults() {

        Debug.Log("UIPanelModeTypeChoice:DisplayStateResults:flowState:" + flowState);

        UIUtil.SetLabelValue(labelResultsStatus, "Results");

        ShowResults();
    }

    // SHOW / HIDE

    public void HideStates() {
        HideOverview();
        HideDisplayItem();
        HideResultItem();
        HideResults();
    }

    // OVERLAY

    public void ContentPause() {
        GameController.GameRunningStateContent();
    }

    public void ContentRun() {
        GameController.GameRunningStateRun();
    }

    public void ShowOverview() {
        HideStates();
        Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);
        AnimateInBottom(containerChoiceOverview);
        ContentPause();
    }

    public void HideOverview() {
        Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);
        AnimateOutBottom(containerChoiceOverview, 0f, 0f);
        ContentRun();
    }

    // DISPLAY ITEM

    public void ShowDisplayItem() {
        HideStates();
        Debug.Log("UIPanelModeTypeChoice:ShowDisplayItem:flowState:" + flowState);
        AnimateInBottom(containerChoiceDisplayItem);
        ContentPause();
    }

    public void HideDisplayItem() {
        Debug.Log("UIPanelModeTypeChoice:HideDisplayItem:flowState:" + flowState);
        AnimateOutBottom(containerChoiceDisplayItem, 0f, 0f);
        ContentRun();
    }

    // RESULT ITEM

    public void ShowResultItem() {
        HideStates();
        Debug.Log("UIPanelModeTypeChoice:ShowResultItem:flowState:" + flowState);
        AnimateInBottom(containerChoiceResultItem);
        ContentPause();
    }

    public void HideResultItem() {
        Debug.Log("UIPanelModeTypeChoice:HideResultItem:flowState:" + flowState);
        AnimateOutBottom(containerChoiceResultItem, 0f, 0f);
        ContentRun();
    }

    // RESULTS

    public void ShowResults() {
        HideStates();
        Debug.Log("UIPanelModeTypeChoice:ShowResults:flowState:" + flowState);
        AnimateInBottom(containerChoiceResults);
        ContentPause();
    }

    public void HideResults() {
        Debug.Log("UIPanelModeTypeChoice:HideResults:flowState:" + flowState);
        AnimateOutBottom(containerChoiceResults, 0f, 0f);
        ContentRun();
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

    public void Reset() {
        HideStates();
        LoadChoices(choicesToLoad);
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

        Reset();

        ShowCurrentState();
    }

    public override void AnimateIn() {
        base.AnimateIn();

        loadData();

    }

    public override void AnimateOut() {
        base.AnimateOut();
    }
	
}
