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
    public GameObject containerChoiceAnswer;
    public GameObject containerChoiceResults;

    // GLOBAL CHOICE
    public UILabel labelOverviewTip;
    public UILabel labelOverviewType;
    public UILabel labelOverviewStatus;

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
    }
    
    void OnDisable() {
        Messenger<AppContentChoiceItem>.RemoveListener(AppContentChoiceMessages.appContentChoiceItem, OnAppContentChoiceItemHandler);
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

    public void CheckAppContentChoicesData() {
        if(appContentChoicesData == null) {
            appContentChoicesData = new AppContentChoicesData();
        }
    }

    public void ProcessNextItem() {

        CheckAppContentChoicesData();

        bool correctOnly = true;

        if(appContentChoicesData.choices.Count < choices.Count) {
            // Process next question
        }
        else if(appContentChoicesData.choices.Count == choices.Count) {
           // && appContentChoiceData.(correctOnly)) {
            ChangeState(AppModeTypeChoiceFlowState.AppModeTypeChoiceResults);
        }
    }

    public void ChangeState(AppModeTypeChoiceFlowState state) {

        if(state == AppModeTypeChoiceFlowState.AppModeTypeChoiceOverview) {
            DisplayStateOverview();
        }
        else if(state == AppModeTypeChoiceFlowState.AppModeTypeChoiceDisplayItem) {
            DisplayStateDisplayItem();
        }
        else if(state == AppModeTypeChoiceFlowState.AppModeTypeChoiceResultItem) {
            DisplayStateResultItem();
        }
        else if(state == AppModeTypeChoiceFlowState.AppModeTypeChoiceResults) {
            DisplayStateResults();
        }
    }

    public string GetStatusItemProgress() {
        return string.Format("question: {0} / {1} ", appContentChoicesData.choices.Count, choices.Count);
    }

    public string GetStatusOverview() {
        return string.Format("{0} questions", choices.Count);
    }

    public void DisplayStateOverview() {
        SetStatus(GetStatusOverview());
    }

    public void DisplayStateDisplayItem() {
        SetStatus(GetStatusItemProgress());
    }

    public void DisplayStateResultItem() {
        SetStatus(GetStatusItemProgress());
    }

    public void DisplayStateResults() {
        SetStatus("Results");
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
    }
	
}
