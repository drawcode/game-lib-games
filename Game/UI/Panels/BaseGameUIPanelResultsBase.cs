using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class BaseGameUIPanelResultsBase : MonoBehaviour {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel totalScore;
	public UILabel totalScoreSmarts;
	public UILabel totalScores;
	public UILabel totalCoins;
    public UILabel totalTime;
    public UILabel totalKills;
#else
    public Text totalScore;
    public Text totalScoreSmarts;
    public Text totalScores;
    public Text totalCoins;
    public Text totalTime;
    public Text totalKills;
#endif

    public virtual void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    public virtual void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

	public virtual void Start() {
		Init();
	}
	
	public virtual void Init() {

		loadData();
	}
		 
    public virtual void OnButtonClickEventHandler(string buttonName) {
		//Debug.Log("OnButtonClickEventHandler: " + buttonName);
	}

	public virtual void UpdateDisplay(GamePlayerRuntimeData runtimeData, float timeTotal) {

		double totalScoreValue = runtimeData.totalScoreValue;
		UIUtil.SetLabelValue(totalTime, FormatUtil.GetFormattedTimeHoursMinutesSecondsMs((double)timeTotal));
		UIUtil.SetLabelValue(totalCoins, runtimeData.coins.ToString("N0"));
		UIUtil.SetLabelValue(totalScores, runtimeData.scores.ToString("N0"));
		UIUtil.SetLabelValue(totalScoreSmarts, runtimeData.score.ToString("N0"));
        UIUtil.SetLabelValue(totalScore, totalScoreValue.ToString("N0"));
        UIUtil.SetLabelValue(totalKills, runtimeData.kills.ToString("N0"));
	}
	
	public virtual void loadData() {
		//StartCoroutine(loadDataCo());
	}
	
	public virtual IEnumerator loadDataCo() {
		
		yield return new WaitForSeconds(1f);
	}
	
}
