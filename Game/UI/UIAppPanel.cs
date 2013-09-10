using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum UIAppPanelMode {	
	ModeMain,
	ModeList
}

public class UIAppPanel : MonoBehaviour {
	
	public UIAppPanelMode panelMode = UIAppPanelMode.ModeMain;
	
	public bool isVisible = false;
        
    public virtual void Start() {
        Init();
    }
    
    public virtual void Init() {
        if(GameGlobal.Instance == null) {
			Application.LoadLevel("GameUISceneRoot");
		}
    } 	
	
	public void ShowObjectDelayed(GameObject obj, float delay) {
		StartCoroutine(ShowObjectDelayedCo(obj, delay));
	}
	
	public void HideObjectDelayed(GameObject obj, float delay) {
		StartCoroutine(HideObjectDelayedCo(obj, delay));
	}
		
	IEnumerator ShowObjectDelayedCo(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		ShowObject(obj);
	}
	
	IEnumerator HideObjectDelayedCo(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		HideObject(obj);
	}
	
	public void ShowObject(GameObject obj) {
		if(obj != null) {
			obj.Show();
		}
	}
	
	public void HideObject(GameObject obj) {
		if(obj != null) {
			obj.Hide();
		}
	}	
	
	
}
