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
    public GameObject prefabLevelItem;

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
    public UILabel labelResultItemChoiceDisplayName;
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

    public UISlider sliderScore;

    public UIImageButton buttonResultsAdvance;
    public UIImageButton buttonResultsReplay;
    public UIImageButton buttonResultsModes;

    // GLOBAL

    public AppModeTypeChoiceFlowState flowState = AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview;

    public Dictionary<string, AppContentChoice> choices;
    public AppContentChoicesData appContentChoicesData;

    int currentChoice = 0;
    public int choicesToLoad = 3;
    public double choicesCorrect = 0;
    public AppContentChoice currentAppContentChoice;

    public AppContentChoice appContentChoice = null;
    public AppContentChoiceItem appContentChoiceItem = null;

    public AppContentChoiceItem currentChoiceItem = null;
    public AppContentChoiceData currentChoiceData = null;

    bool isCorrect = true;

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

        Messenger<GameObjectChoiceData>.AddListener(
            GameObjectChoiceMessages.gameChoiceDataResponse,
            OnAppContentChoiceItemHandler);

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);


        Messenger.AddListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<GameObjectChoiceData>.RemoveListener(
            GameObjectChoiceMessages.gameChoiceDataResponse,
            OnAppContentChoiceItemHandler);

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.RemoveListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }

    public void BroadcastAppContentChoiceItem(AppContentChoiceItem choiceItem) {
        //if(choiceItem == null) {
        //    return;
        //}

        //Messenger<AppContentChoiceItem>.Broadcast(AppContentChoiceMessages.appContentChoiceItem, choiceItem);
    }

    void OnAppContentChoiceItemHandler(GameObjectChoiceData data) {
        CheckChoicesData();

        isCorrect = data.choiceItemIsCorrect;

        AppContentChoiceData choiceData = new AppContentChoiceData();
        choiceData.choiceCode = data.choiceCode;

        appContentChoice = AppContentChoices.Instance.GetByCode(data.choiceCode);
        appContentChoiceItem = null;

        if(appContentChoice != null) {
            foreach(AppContentChoiceItem choiceItem in appContentChoice.choices) {
                if(choiceItem.code == data.choiceItemCode) {
                    appContentChoiceItem = choiceItem;
                }
            }
        }

        choiceData.choices.Add(appContentChoiceItem);
        choiceData.choiceData = "";

        if(appContentChoicesData != null) {
            appContentChoicesData.SetChoice(choiceData);
        }

        currentChoiceData = choiceData;
        currentChoiceItem = appContentChoiceItem;

        SaveChoiceState();

        ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem);
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
        else if(UIUtil.IsButtonClicked(buttonResultsReplay, buttonName)) {
            Reset();
            GameUIController.ShowGameModeTrainingModeChoiceQuiz();
        }
        else if(UIUtil.IsButtonClicked(buttonResultsModes, buttonName)) {
            Reset();
            GameUIController.ShowGameMode();
        }
    }

    // SAVE STATE

    public void SaveChoiceState() {
        string data = Engine.Data.Json.JsonMapper.ToJson(appContentChoicesData);
        Debug.Log("SaveChoiceState:" + data);
    }

    // ADVANCE/NEXT

    bool chosen = false;

    public void Advance() {

        Debug.Log("Advance:flowState:" + flowState);

        if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview) {

            chosen = false;

            // Load new set of questions.
            LoadChoices(choicesToLoad);

            LoadLevelAssets();

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
            ShowResults();
            HideStates();
        }
    }

    public void ResetChoices() {
        currentChoice = 0;
        choicesCorrect = 0;
        LoadChoices(choicesToLoad);
    }

    public void ResetChoiceItem() {
        chosen = false;
    }

    public void NextChoice() {
        currentChoice += 1;

        if(currentChoice > choices.Count - 1) {
            // Advance to results
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResults);
        }
        else {
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem);
            LoadLevelAssets();
        }

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
            if(choice != null) {
                if(!choices.ContainsKey(choice.code)) {
                    choices.Add(choice.code, choice);
                    Debug.Log("LoadChoices:choice.code:" + choice.code);
                }
            }
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
                if(currentChoice > choices.Count) {
                    currentChoice = choices.Count - 1;
                }

                List<string> keys = choices.Keys.ToList();
                if(keys.Count > currentChoice) {
                    return choices[keys[currentChoice]];
                }
            }
        }

        return null;
    }

    public void OnGameLevelItemsLoadedHandler() {

        Debug.Log("OnGameLevelItemsLoadedHandler");

        if(AppModeTypes.Instance.isAppModeTypeGameChoice) {
            //Messenger.Broadcast(GameDraggableEditorMessages.GameLevelItemsLoaded);

            // Match choices to level assets.

            Debug.Log("OnGameLevelItemsLoadedHandler2");

            //int i = 0;

            //AppContentChoice choice = GetCurrentChoice();

           // Debug.Log("OnGameLevelItemsLoadedHandler:-choice:" + choice.code);

            /*
            foreach(Transform t in GameController.Instance.levelItemsContainerObject.transform) {

                if(t.gameObject.Has<GameObjectChoice>()) {

                    GameObjectChoice choiceObject = t.gameObject.Get<GameObjectChoice>();

                    Color colorTo = GetColor(i);
    
                    if(choice != null) {

                        if(choice.choices != null) {
                            choice.choices.Shuffle();
                        }

                        if(choice.choices.Count > i) {
                            AppContentChoiceItem choiceItem = choice.choices[i];
                            choiceObject.LoadChoiceItem(choice, choiceItem, colorTo);
                        }
                        else {
                            choiceObject.gameObject.Hide();
                        }
                    }
                    else {
                        choiceObject.gameObject.Hide();
                    }

    
                    Debug.Log("OnGameLevelItemsLoadedHandler:choice:" + choice.code);
                    //Debug.Log("OnGameLevelItemsLoadedHandler:choice.choices[i]:" + choice.choices[i].display);
                    Debug.Log("OnGameLevelItemsLoadedHandler:colorTo:" + colorTo);
                    Debug.Log("OnGameLevelItemsLoadedHandler:i:" + i);
    
                    i++;

                }
            }
            */



            /*
            foreach(GameObjectChoice choiceObject
                in GameController.Instance.levelItemsContainerObject
                .GetList<GameObjectChoice>()) {

                Color colorTo = GetColor(i);

                if(choice != null) {
                    if(choice.choices.Count > i) {
                        AppContentChoiceItem choiceItem = choice.choices[i];
                        choiceObject.LoadChoiceItem(choice, choiceItem, colorTo);
                    }
                    else {
                        choiceObject.gameObject.Hide();
                    }
                }
                else {
                    choiceObject.gameObject.Hide();
                }


                Debug.Log("OnGameLevelItemsLoadedHandler:choice:" + choice.code);
                //Debug.Log("OnGameLevelItemsLoadedHandler:choice.choices[i]:" + choice.choices[i].display);
                Debug.Log("OnGameLevelItemsLoadedHandler:colorTo:" + colorTo);
                Debug.Log("OnGameLevelItemsLoadedHandler:i:" + i);

                i++;
            }
            */
        }
    }

    public void LoadLevelAssets() {

       // GameController.LoadLevelAssets(AppContentStates.Current.code);
        GameController.LoadLevelAssets(GameLevels.Current.code);
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
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem);
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
                choiceQuestion = choice.display_name;
            }

            UIUtil.SetLabelValue(labelDisplayItemTitle, choiceTitle);
            UIUtil.SetLabelValue(labelDisplayItemQuestion, choiceQuestion);
        }
    }

    public void DisplayStateResultItem() {
        //Debug.Log("UIPanelModeTypeChoice:DisplayStateResultItem:flowState:" + flowState);

        UpdateResultItemData();

        ShowResultItem();
    }

    public void UpdateResultItemData() {
        Debug.Log("UIPanelModeTypeChoice:UpdateResultItemData");

        UIUtil.SetLabelValue(labelResultItemStatus, GetStatusItemProgress());

        UIColors.UpdateColors();

        AppContentChoice choice = GetCurrentChoice();

        string typeValue = "CORRECT";
        string codeValue = "FALSE";

        if(isCorrect) {//currentChoiceData.CheckChoices(true)) {
            UIColors.UpdateColor(containerChoiceResultItem, UIColors.colorGreen);
            typeValue = "CORRECT!";
            GameAudioController.Instance.PlayCheer1();
            GameAudioController.Instance.PlayWhistle();
            choicesCorrect += 1;
        }
        else {
            UIColors.UpdateColor(containerChoiceResultItem, UIColors.colorRed);
            typeValue = "INCORRECT...";
            GameAudioController.Instance.PlayWhistle();
            GameAudioController.Instance.PlayOh();
        }

        foreach(AppContentChoiceItem choiceItem in choice.choices) {
            if(choiceItem != null) {
                if(choiceItem.IsTypeCorrect()) {
                    codeValue = choiceItem.display;
                }
            }
        }

        if(choice != null) {

            string choiceResultType = typeValue;
            string choiceResultValue = codeValue;
            string choiceResultDescription = choice.description;
            string choiceResultDisplayName = choice.display_name;

            if(choice != null) {
                //choiceQuestion = choice.display_name + choice.code;
            }

            UIUtil.SetLabelValue(labelResultItemChoiceDisplayName, choiceResultDisplayName);
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

        GameAudioController.Instance.PlayCheer1();

        //double timeCompleted = 30; // TODO
        double coins = 500;
        double choiceTotal = choices.Count;
        double choicesResult = choicesCorrect / choiceTotal;

        coins = choicesCorrect * 50;

        string scoreFractionValue = string.Format("{0}/{1}", choicesCorrect, choiceTotal);
        string scorePercentageValue = choicesResult.ToString("P0");
        //string scoreTitleValue = ;
        //string scoreTypeValue = "4/5";
        string scoreCoinsValue = coins.ToString("N0");

        UIUtil.SetSliderValue(sliderScore, choicesResult);

        UIUtil.SetLabelValue(labelResultsScoreFractionValue, scoreFractionValue);
        UIUtil.SetLabelValue(labelResultsScorePercentageValue, scorePercentageValue);
        //UIUtil.SetLabelValue(labelResultsTitle, choiceResultValue);
        //UIUtil.SetLabelValue(labelResultsType, choiceResultType);
        UIUtil.SetLabelValue(labelResultsCoinsValue, scoreCoinsValue);

        // REWARDS

        GameProfileRPGs.Current.AddCurrency(coins);

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(coins * 2);

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(1f);
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(1f);


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
        flowState = AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview;
        ResetChoices();
        ResetChoiceItem();
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

    public Color GetColor(int index) {
        if(index == 0) {
            return UIColors.colorGreen;//Color.red;
        }
        else if(index == 1) {
            return UIColors.colorYellow;//Color.blue;
        }
        else if(index == 2) {
            return UIColors.colorBlue;//Color.yellow;
        }
        else if(index == 3) {
            return UIColors.colorOrange;//Color.red;
        }
        else if(index == 4) {
            return UIColors.colorPurple;//Color.red;
        }

        return UIColors.colorGreen;
    }

    public void loadDataChoiceItems() {

        Debug.Log("loadDataChoiceItems");

        int i = 0;
     
        Debug.Log("loadDataChoiceItems:" + i);

        AppContentChoice choice = GetCurrentChoice();

        if(choice != null) {

            float x = -60;

            foreach(AppContentChoiceItem choiceItem in choice.choices) {

                // Add to list

                string levelName = "choice" + choice.code + "-" + choiceItem.code + "-" + i;

                if(!listGridRoot.ContainsChild(levelName)) {

                    GameObject item = NGUITools.AddChild(listGridRoot, prefabListItem);
                    item.name = levelName;

                    GameObjectChoice choiceObject = item.Get<GameObjectChoice>();

                    if(choiceObject != null) {
                        choiceObject.LoadChoiceItem(choice, choiceItem, GetColor(i));
                    }
                }

                // Add to level

                string levelNameItem = "choice-item" + choice.code + "-" + choiceItem.code + "-" + i;

                if(!GameController.Instance.levelItemsContainerObject.ContainsChild(levelNameItem)) {

                    GameObject itemLevel = NGUITools.AddChild(GameController.Instance.levelItemsContainerObject,
                        prefabLevelItem);
                    itemLevel.name = levelNameItem;
    
                    GameObjectChoice choiceObjectLevel = itemLevel.Get<GameObjectChoice>();
    
                    if(choiceObjectLevel != null) {
                        choiceObjectLevel.LoadChoiceItem(choice, choiceItem, GetColor(i));
    
                        GamePlayerIndicator indicator = GamePlayerIndicator.AddIndicator(
                            itemLevel, GamePlayerIndicatorType.choice, GetColor(i));

                        if(indicator != null) {
                            indicator.SetIndicatorColorEffects(GetColor(i));
                        }
                    }
    
                    choiceObjectLevel.transform.position =
                        Vector3.zero.WithX(x += (UnityEngine.Random.Range(20, 60)*1)).WithZ(UnityEngine.Random.Range(-30, 30));
                }

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
        else if(modifiderKey && Input.GetKeyDown(KeyCode.Backslash)) {
            AppContentChoiceItem item = null;

            foreach(KeyValuePair<string, AppContentChoice> choice in choices) {
                foreach(AppContentChoiceItem choiceItem in choice.Value.choices) {
                    item = choiceItem;
                    break;
                }
            }

            BroadcastAppContentChoiceItem(item);
        }
    }
	
}
