#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelModeTypeChoice : UIPanelBase {

	public static UIPanelModeTypeChoice Instance;

    public GameObject prefabListItem;

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

    public UILabel labelResultItemChoiceDescription;
    public UILabel labelResultItemChoiceResultValue;
    public UILabel labelResultItemChoiceResultType;
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

		//loadData();
	}	
	
	public override void Start() {
		Init();
	}

    // EVENTS
	
    public override void OnEnable() {
        base.OnEnable();
        Messenger<AppContentChoiceItem>.AddListener(AppContentChoiceMessages.appContentChoiceItem, OnAppContentChoiceItemHandler);
        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    public override void OnDisable() {
        base.OnDisable();
        Messenger<AppContentChoiceItem>.RemoveListener(AppContentChoiceMessages.appContentChoiceItem, OnAppContentChoiceItemHandler);
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    void OnAppContentChoiceItemHandler(AppContentChoiceItem choiceItem) {
        CheckChoicesData();

        AppContentChoiceData choiceData = new AppContentChoiceData();
        choiceData.choiceCode = choiceItem.code;
        choiceData.choices.Add(choiceItem);
        choiceData.choiceData = "";

        if(appContentChoicesData != null) {
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

    bool chosen = false;

    public void Advance() {

        Debug.Log("Advance:flowState:" + flowState);

        if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview) {

            chosen = false;

            // Load new set of questions.
            LoadChoices(choicesToLoad);

            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem);
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem) {

            // Set question info and level info if not loaded
            //GameController.LoadLevelAssets("1-1");

            if(chosen) {
                ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem);
            }
            else {
                HideStates();
            }
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem) {

            HideStates();
            ResetChoiceItem();
            NextChoice();

        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceResults) {

            // set check game over flow

            HideStates();
        }
    }

    public void ResetChoices() {
        currentChoice = 0;
        LoadChoices(choicesToLoad);
    }

    public void ResetChoiceItem() {
        chosen = false;
    }

    public void NextChoice() {
        currentChoice += 1;

        if(currentChoice > choices.Count) {
            // Advance to results
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResults);
        }
        else {
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem);
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

        ListClear(listGridRoot);

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

        //bool correctOnly = true;

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

        StartCoroutine(ShowCurrentStateCo());
    }

    IEnumerator ShowCurrentStateCo() {

        yield return new WaitForEndOfFrame();

        //Debug.Log("UIPanelModeTypeChoice:ShowCurrentState:flowState:" + flowState);

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
            //Debug.Log("UIPanelModeTypeChoice:ChangeState:flowState:" + flowState);
            ShowCurrentState();
        }
    }

    // STATUS/TEXT

    public string GetStatusItemProgress() {
        CheckChoices();
        CheckChoicesData();

        int countUser = 1;

        if(appContentChoicesData != null) {
            if(appContentChoicesData.choices != null) {
                //countUser = appContentChoicesData.choices.Count + 1;
            }
        }

        countUser = currentChoice + 1;

        int count = 1;

        if(choices != null) {
            count = choices.Count;
        }

        return string.Format("question: {0} / {1} ", countUser, count);
    }

    public string GetStatusOverview() {
        CheckChoices();

        int count = 1;

        if(choices != null) {
            count = choices.Count;
        }

        return string.Format("{0} questions", count);
    }

    // CHOICES DATA

    public AppContentChoice GetCurrentChoice() {
        CheckChoices();
        CheckChoicesData();

        if(choices != null) {
            if(choices.Count > 0) {
                return choices[choices.Keys.ToList()[currentChoice]];
            }
        }

        return null;
    }

    // DISPLAY

    public void DisplayStateOverview() {

        //Debug.Log("UIPanelModeTypeChoice:DisplayStateOverview:flowState:" + flowState);

        UIUtil.SetLabelValue(labelOverviewStatus, GetStatusOverview());

        ShowOverview();
    }

    public void DisplayStateDisplayItem() {

        CheckChoices();
        CheckChoicesData();

        //Debug.Log("UIPanelModeTypeChoice:DisplayStateDisplayItem:flowState:" + flowState);

        Debug.Log("UIPanelModeTypeChoice:DisplayStateDisplayItem:choices.Count:" + choices.Count);

        if(chosen) {
            
            ShowResultItem();

        }
        else {

            UpdateDisplayItemData();

            ShowDisplayItem();
        }

    }

    public void UpdateDisplayItemData() {
        Debug.Log("UIPanelModeTypeChoice:UpdateDisplayItemData");

        UIColors.UpdateColors();

        UIUtil.SetLabelValue(labelDisplayItemStatus, GetStatusItemProgress());

        AppContentChoice choice = GetCurrentChoice();

        if(choice != null) {

            string choiceTitle = "Loading...";
            string choiceQuestion = "Loading...";
    
            if(choice != null) {
                choiceTitle = "Question";
                choiceQuestion = choice.display_name + choice.code;
            }

            UIUtil.SetLabelValue(labelDisplayItemTitle, choiceTitle);
            UIUtil.SetLabelValue(labelDisplayItemQuestion, choiceQuestion);
        }
    }

    public void DisplayStateResultItem() {
        //Debug.Log("UIPanelModeTypeChoice:DisplayStateResultItem:flowState:" + flowState);

        UIUtil.SetLabelValue(labelResultItemStatus, GetStatusItemProgress());

        UpdateResultItemData();

        ShowResultItem();
    }

    public void UpdateResultItemData() {
        Debug.Log("UIPanelModeTypeChoice:UpdateResultItemData");

        UIUtil.SetLabelValue(labelResultItemStatus, GetStatusItemProgress());

        UIColors.UpdateColors();

        AppContentChoice choice = GetCurrentChoice();

        bool choiceCorrect = true;

        if(choiceCorrect) {
            UIColors.UpdateColor(containerChoiceResultItem, UIColors.colorGreen);
        }
        else {
            UIColors.UpdateColor(containerChoiceResultItem, UIColors.colorRed);
        }

        if(choice != null) {

            string choiceResultType = "Loading...";
            string choiceResultValue = "Loading...";
            string choiceResultDescription = "Loading...";

            if(choice != null) {
                //choiceQuestion = choice.display_name + choice.code;
            }

            UIUtil.SetLabelValue(labelResultItemChoiceDescription, choiceResultDescription);
            UIUtil.SetLabelValue(labelResultItemChoiceResultValue, choiceResultValue);
            UIUtil.SetLabelValue(labelResultItemChoiceResultType, choiceResultType);
        }
    }

    public void DisplayStateResults() {

        //Debug.Log("UIPanelModeTypeChoice:DisplayStateResults:flowState:" + flowState);

        UIUtil.SetLabelValue(labelResultsStatus, "Results");

        UpdateDisplayStateResultsData();

        ShowResults();
    }


    public void UpdateDisplayStateResultsData() {
        Debug.Log("UIPanelModeTypeChoice:UpdateDisplayStateResultsData");

        UIColors.UpdateColors();

        UIUtil.SetLabelValue(labelResultsStatus, "Results");

        /*
        AppContentChoice choice = GetCurrentChoice();

        if(choice != null) {

            string choiceTitle = "Loading...";
            string choiceQuestion = "Loading...";
    
            if(choice != null) {
                choiceTitle = "Question";
                choiceQuestion = choice.display_name + choice.code;
            }

            UIUtil.SetLabelValue(labelDisplayItemTitle, choiceTitle);
            UIUtil.SetLabelValue(labelDisplayItemQuestion, choiceQuestion);
        }
        */
    }

    // SHOW / HIDE

    public void HideStates() {

        UIPanelDialogBackground.HideAll();

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
        //HideStates();
    }

    public void ShowOverview() {

        HideStates();

        UIPanelDialogBackground.ShowDefault();

        //Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerChoiceOverview);

        ContentPause();

        UIColors.UpdateColors();
    }

    public void HideOverview() {

        //Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateOutBottom(containerChoiceOverview, 0f, 0f);

        ContentRun();
    }

    // DISPLAY ITEM

    public void ShowDisplayItem() {

        Debug.Log("UIPanelModeTypeChoice:ShowDisplayItem");

        HideStates();

        StartCoroutine(ShowDisplayItemCo());

        UIColors.UpdateColors();
    }

    IEnumerator ShowDisplayItemCo() {

        yield return new WaitForEndOfFrame();

        UIPanelDialogBackground.ShowDefault();

        //Debug.Log("UIPanelModeTypeChoice:ShowDisplayItem:flowState:" + flowState);

        AnimateInBottom(containerChoiceDisplayItem);

        loadDataChoice();

        UIColors.UpdateColors();

        ContentPause();

    }

    public void HideDisplayItem() {

        //Debug.Log("UIPanelModeTypeChoice:HideDisplayItem:flowState:" + flowState);

        listGridRoot.DestroyChildren();

        AnimateOutBottom(containerChoiceDisplayItem, 0f, 0f);

        ContentRun();
    }

    // RESULT ITEM

    public void ShowResultItem() {

        HideStates();

        UIPanelDialogBackground.ShowDefault();

        //Debug.Log("UIPanelModeTypeChoice:ShowResultItem:flowState:" + flowState);

        AnimateInBottom(containerChoiceResultItem);

        ContentPause();
    }

    public void HideResultItem() {

        //Debug.Log("UIPanelModeTypeChoice:HideResultItem:flowState:" + flowState);

        AnimateOutBottom(containerChoiceResultItem, 0f, 0f);

        ContentRun();
    }

    // RESULTS

    public void ShowResults() {
        HideStates();
        //Debug.Log("UIPanelModeTypeChoice:ShowResults:flowState:" + flowState);
        AnimateInBottom(containerChoiceResults);
        ContentPause();
    }

    public void HideResults() {
        //Debug.Log("UIPanelModeTypeChoice:HideResults:flowState:" + flowState);
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
        ResetChoices();
    }

	public static void LoadData() {
		if(Instance != null) {
			Instance.loadData();
		}
	}

    public void loadDataChoice() {
        Debug.Log("UIPanelModeTypeChoice:loadDataChoice");
        StartCoroutine(loadDataChoiceCo());
    }

    IEnumerator loadDataChoiceCo() {

        // load list item

        if (listGridRoot != null) {
            yield return new WaitForEndOfFrame();

            listGridRoot.DestroyChildren();
            //ListClear(listGridRoot);

            yield return new WaitForEndOfFrame();

            if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem) {
                loadDataChoiceItems();
            }

            yield return new WaitForEndOfFrame();
            ListReposition(listGrid, listGridRoot);
            yield return new WaitForEndOfFrame();
        }
    }

    public void loadData() {
        Debug.Log("UIPanelModeTypeChoice:loadData");
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
        yield return new WaitForSeconds(1f);

        Reset();

        ShowCurrentState();
    }

    public void loadDataChoiceItems() {

        Debug.Log("loadDataChoiceItems");

        int i = 0;
     
        Debug.Log("loadDataChoiceItems:" + i);

        AppContentChoice choice = GetCurrentChoice();

        if(choice != null) {

            foreach(AppContentChoiceItem choiceItem in choice.choices) {

                GameObject item = NGUITools.AddChild(listGridRoot, prefabListItem);
                item.name = "AItem" + i;

                GameObjectChoice choiceObject = item.Get<GameObjectChoice>();

                if(choiceObject != null) {
    
                    if(i == 0) {
                        choiceObject.startColor = UIColors.colorGreen;//Color.red;
                    }
                    else if(i == 1) {
                        choiceObject.startColor = UIColors.colorYellow;//Color.blue;
                    }
                    else if(i == 2) {
                        choiceObject.startColor = UIColors.colorBlue;//Color.yellow;
                    }
                    else if(i == 3) {
                        choiceObject.startColor = UIColors.colorOrange;//Color.red;
                    }
    
                    choiceObject.LoadChoiceItem(choice, choiceItem);
                }

                // LOAD INTO LEVEL



               i++;

            }
        }

    }

    public override void AnimateIn() {
        base.AnimateIn();

        ShowCurrentState();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideStates();
    }

    public void Update() {

        bool modifiderKey = Input.GetKey(KeyCode.RightControl);

        if(modifiderKey && Input.GetKeyDown(KeyCode.LeftBracket)) {
            chosen = false;
        }
        else if(modifiderKey && Input.GetKeyDown(KeyCode.RightBracket)) {
            chosen = true;
        }
    }
	
}
