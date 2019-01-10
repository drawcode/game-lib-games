#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class UIPanelModeTypeChoice : UIPanelBase {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3    
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
#else
    // OVERVIEW

    public Text labelOverviewTip;
    public Text labelOverviewType;
    public Text labelOverviewStatus;

    public Text labelOverviewTitle;
    public Text labelOverviewBlurb;
    public Text labelOverviewBlurb2;
    public Text labelOverviewNextSteps;

    public Button buttonOverviewAdvance;

    // DISPLAY ITEM

    public Text labelDisplayItemTip;
    public Text labelDisplayItemType;
    public Text labelDisplayItemStatus;

    public Text labelDisplayItemTitle;
    public Text labelDisplayItemAnswers;
    public Text labelDisplayItemNote;
    public Text labelDisplayItemQuestion;

    public Button buttonDisplayItemAdvance;

    // RESULT ITEM

    public Text labelResultItemTip;
    public Text labelResultItemType;
    public Text labelResultItemStatus;

    public Text labelResultItemChoiceDescription;
    public Text labelResultItemChoiceResultValue;
    public Text labelResultItemChoiceResultType;
    public Text labelResultItemChoiceDisplayName;
    public Text labelResultItemNextSteps;

    public Button buttonResultItemAdvance;

    // RESULTS

    public Text labelResultsTip;
    public Text labelResultsType;
    public Text labelResultsStatus;

    public Text labelResultsTitle;
    public Text labelResultsCoinsValue;
    public Text labelResultsScorePercentageValue;
    public Text labelResultsScoreFractionValue;

    public Slider sliderScore;

    public Button buttonResultsAdvance;
    public Button buttonResultsReplay;
    public Button buttonResultsModes;
#endif

    public static UIPanelModeTypeChoice Instance;

    public GameObject prefabListItem;
    public GameObject prefabLevelItem;

    public GameObject containerChoiceOverview;
    public GameObject containerChoiceDisplayItem;
    public GameObject containerChoiceResultItem;
    public GameObject containerChoiceResults;

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
    bool chosen = false;

    public override void Awake() {
        base.Awake();

        if(Instance != null && this != Instance) {
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

    public void OnAppContentChoiceItemHandler(GameObjectChoiceData data) {
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

    public override void OnButtonClickEventHandler(string buttonName) {
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
            //GameUIController.ShowGameModeTrainingModeChoiceQuiz();
        }
        else if(UIUtil.IsButtonClicked(buttonResultsModes, buttonName)) {
            Reset();
#if ENABLE_FEATURE_GAME_MODE
            GameUIController.ShowGameMode();
#endif
        }
    }

    // SAVE STATE

    public void SaveChoiceState() {
        string data = Engine.Data.Json.JsonMapper.ToJson(appContentChoicesData);
        LogUtil.Log("SaveChoiceState:" + data);
    }

    // ADVANCE/NEXT

    public void Advance() {

        LogUtil.Log("Advance:flowState:" + flowState);

        if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview) {

            chosen = false;

            // Load new set of questions.
            LoadChoices(choicesToLoad);

            LoadLevelAssets();

            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem);
        }
        else if(flowState == AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem) {

            // Set question info and level info if not loaded

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

        LogUtil.Log("LoadChoices:countChoices:" + countChoices);

        // select total choices to try

        for(int i = 0; i < total; i++) {
            AppContentChoice choice = choicesFilter[i];
            if(choice != null) {
                if(!choices.ContainsKey(choice.code)) {
                    choices.Add(choice.code, choice);
                    LogUtil.Log("LoadChoices:choice.code:" + choice.code);
                }
            }
        }

        LogUtil.Log("LoadChoices:choices.Count:" + choices.Count);
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

        //LogUtil.Log("UIPanelModeTypeChoice:ShowCurrentState:flowState:" + flowState);

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
            //LogUtil.Log("UIPanelModeTypeChoice:ChangeState:flowState:" + flowState);
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

        //LogUtil.Log("OnGameLevelItemsLoadedHandler");

        if(AppModeTypes.Instance.isAppModeTypeGameChoice) {
            //Messenger.Broadcast(GameDraggableEditorMessages.GameLevelItemsLoaded);

            // Match choices to level assets.

            //LogUtil.Log("OnGameLevelItemsLoadedHandler2");

            //int i = 0;

            //AppContentChoice choice = GetCurrentChoice();

            // LogUtil.Log("OnGameLevelItemsLoadedHandler:-choice:" + choice.code);

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

    
                    LogUtil.Log("OnGameLevelItemsLoadedHandler:choice:" + choice.code);
                    //LogUtil.Log("OnGameLevelItemsLoadedHandler:choice.choices[i]:" + choice.choices[i].display);
                    LogUtil.Log("OnGameLevelItemsLoadedHandler:colorTo:" + colorTo);
                    LogUtil.Log("OnGameLevelItemsLoadedHandler:i:" + i);
    
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


                LogUtil.Log("OnGameLevelItemsLoadedHandler:choice:" + choice.code);
                //LogUtil.Log("OnGameLevelItemsLoadedHandler:choice.choices[i]:" + choice.choices[i].display);
                LogUtil.Log("OnGameLevelItemsLoadedHandler:colorTo:" + colorTo);
                LogUtil.Log("OnGameLevelItemsLoadedHandler:i:" + i);

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

        //LogUtil.Log("UIPanelModeTypeChoice:DisplayStateOverview:flowState:" + flowState);

        UIUtil.SetLabelValue(labelOverviewStatus, GetStatusOverview());

        ShowOverview();
    }

    public void DisplayStateDisplayItem() {

        CheckChoices();
        CheckChoicesData();

        //LogUtil.Log("UIPanelModeTypeChoice:DisplayStateDisplayItem:flowState:" + flowState);

        LogUtil.Log("UIPanelModeTypeChoice:DisplayStateDisplayItem:choices.Count:" + choices.Count);

        if(chosen) {
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem);
        }
        else {
            UpdateDisplayItemData();
            ShowDisplayItem();
        }

    }

    public void UpdateDisplayItemData() {
        LogUtil.Log("UIPanelModeTypeChoice:UpdateDisplayItemData");

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
        //LogUtil.Log("UIPanelModeTypeChoice:DisplayStateResultItem:flowState:" + flowState);

        UpdateResultItemData();

        ShowResultItem();
    }

    public void UpdateResultItemData() {
        LogUtil.Log("UIPanelModeTypeChoice:UpdateResultItemData");

        UIUtil.SetLabelValue(labelResultItemStatus, GetStatusItemProgress());

        UIColors.UpdateColors();

        AppContentChoice choice = GetCurrentChoice();

        string typeValue = "CORRECT";
        string codeValue = "FALSE";

        if(isCorrect) {//currentChoiceData.CheckChoices(true)) {
            UIColors.UpdateColor(containerChoiceResultItem, UIColors.colorGreen);
            typeValue = "CORRECT!";
            GameAudioController.PlaySoundPlayerActionGood();
            choicesCorrect += 1;
        }
        else {
            UIColors.UpdateColor(containerChoiceResultItem, UIColors.colorRed);
            typeValue = "INCORRECT...";
            GameAudioController.PlaySoundPlayerActionBad();
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

        //LogUtil.Log("UIPanelModeTypeChoice:DisplayStateResults:flowState:" + flowState);

        UIUtil.SetLabelValue(labelResultsStatus, "Results");

        UpdateDisplayStateResultsData();

        ShowResults();
    }


    public void UpdateDisplayStateResultsData() {
        LogUtil.Log("UIPanelModeTypeChoice:UpdateDisplayStateResultsData");

        UIColors.UpdateColors();

        UIUtil.SetLabelValue(labelResultsStatus, "Results");

        GameAudioController.PlaySoundPlayerActionGood();

        //double timeCompleted = 30; // TODO
        double coins = 500;
        double choiceTotal = choices.Count;
        double choicesResult = choicesCorrect / choiceTotal;

        coins = choicesCorrect * 100;

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

        GamePlayerProgress.SetStatCoins(coins);
        GamePlayerProgress.SetStatCoinsEarned(coins);

        double xpEarned = coins * 2;

        GamePlayerProgress.SetStatXP(xpEarned);

        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(xpEarned);
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(1f); // refill
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(1f); // refill

        GameController.ProcessLevelStats();


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

        //LogUtil.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateInBottom(containerChoiceOverview);

        ContentPause();

        UIColors.UpdateColors();
    }

    public void HideOverview() {

        //LogUtil.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);

        AnimateOutBottom(containerChoiceOverview, 0f, 0f);

        ContentRun();
    }

    // DISPLAY ITEM

    public void ShowDisplayItem() {

        LogUtil.Log("UIPanelModeTypeChoice:ShowDisplayItem");

        HideStates();

        StartCoroutine(ShowDisplayItemCo());

        UIColors.UpdateColors();
    }

    IEnumerator ShowDisplayItemCo() {

        yield return new WaitForEndOfFrame();

        UIPanelDialogBackground.ShowDefault();

        //LogUtil.Log("UIPanelModeTypeChoice:ShowDisplayItem:flowState:" + flowState);

        AnimateInBottom(containerChoiceDisplayItem);

        loadDataChoice();

        UIColors.UpdateColors();

        ContentPause();

    }

    public void HideDisplayItem() {

        //LogUtil.Log("UIPanelModeTypeChoice:HideDisplayItem:flowState:" + flowState);

        listGridRoot.DestroyChildren();

        AnimateOutBottom(containerChoiceDisplayItem, 0f, 0f);

        ContentRun();
    }

    // RESULT ITEM

    public void ShowResultItem() {

        HideStates();

        UIPanelDialogBackground.ShowDefault();

        //LogUtil.Log("UIPanelModeTypeChoice:ShowResultItem:flowState:" + flowState);

        AnimateInBottom(containerChoiceResultItem);

        ContentPause();
    }

    public void HideResultItem() {

        //LogUtil.Log("UIPanelModeTypeChoice:HideResultItem:flowState:" + flowState);

        AnimateOutBottom(containerChoiceResultItem, 0f, 0f);

        ContentRun();
    }

    // RESULTS

    public void ShowResults() {
        HideStates();
        //LogUtil.Log("UIPanelModeTypeChoice:ShowResults:flowState:" + flowState);
        AnimateInBottom(containerChoiceResults);
        ContentPause();

        Messenger.Broadcast(GameMessages.gameResultsStart);
    }

    public void HideResults() {
        //LogUtil.Log("UIPanelModeTypeChoice:HideResults:flowState:" + flowState);
        AnimateOutBottom(containerChoiceResults, 0f, 0f);
        ContentRun();

        Messenger.Broadcast(GameMessages.gameResultsEnd);
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
        LogUtil.Log("UIPanelModeTypeChoice:loadDataChoice");
        StartCoroutine(loadDataChoiceCo());
    }

    IEnumerator loadDataChoiceCo() {

        // load list item

        if(listGridRoot != null) {
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
        LogUtil.Log("UIPanelModeTypeChoice:loadData");
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {
        yield return new WaitForSeconds(1f);

        Reset();

        ShowCurrentState();
    }

    public Color GetColor(int index) {
        if(index == 0) {
            return UIColors.colorGreen;
            //Color.red;
        }
        else if(index == 1) {
            return UIColors.colorBlue;
            //Color.blue;
        }
        else if(index == 2) {
            return UIColors.colorOrange;
            //Color.yellow;
        }
        else if(index == 3) {
            return UIColors.colorPurple;
            //Color.red;
        }
        else if(index == 4) {
            return UIColors.colorRed;
            //Color.red;
        }

        return UIColors.colorGreen;
    }

    public void loadDataChoiceItems() {

        LogUtil.Log("loadDataChoiceItems");

        int i = 0;

        LogUtil.Log("loadDataChoiceItems:" + i);

        AppContentChoice choice = GetCurrentChoice();

        if(choice != null) {

            float x = -60;

            foreach(AppContentChoiceItem choiceItem in choice.choices) {

                // Add to list

                string levelName = "choice" + choice.code + "-" + choiceItem.code + "-" + i;

                if(!listGridRoot.ContainsChild(levelName)) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject item = NGUITools.AddChild(listGridRoot, prefabListItem);
#else
                    GameObject item = GameObjectHelper.CreateGameObject(
                        prefabListItem, Vector3.zero, Quaternion.identity, false);
                    // NGUITools.AddChild(listGridRoot, listItemPrefab);
                    item.transform.parent = listGridRoot.transform;
                    item.ResetLocalPosition();
#endif

                    item.name = levelName;

                    GameObjectChoice choiceObject = item.Get<GameObjectChoice>();

                    if(choiceObject != null) {
                        choiceObject.LoadChoiceItem(choice, choiceItem, GetColor(i));
                    }
                }

                // Add to level

                string levelNameItem = "choice-item" + choice.code + "-" + choiceItem.code + "-" + i;

                if(!GameController.Instance.levelItemsContainerObject.ContainsChild(levelNameItem)) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                GameObject itemLevel = NGUITools.AddChild(GameController.Instance.levelItemsContainerObject, prefabLevelItem);
#else
                    GameObject itemLevel = GameObjectHelper.CreateGameObject(
                        prefabLevelItem, Vector3.zero, Quaternion.identity, false);
                    // NGUITools.AddChild(listGridRoot, listItemPrefab);
                    itemLevel.transform.parent = GameController.Instance.levelItemsContainerObject.transform;
                    itemLevel.ResetLocalPosition();
#endif


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
                        Vector3.zero
                        .WithX(x += (UnityEngine.Random.Range(20, 60) * 1))
                        .WithZ(UnityEngine.Random.Range(-30, 30));
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