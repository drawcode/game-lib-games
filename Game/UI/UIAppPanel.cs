using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

public enum UIAppPanelMode {    
    ModeMain,
    ModeList
}

public class UIAppPanelMessages {
    public static string panelShow = "ui-app-panel-show";
    public static string panelHide = "ui-app-panel-hide";
}

public class UIAppPanel : GameObjectBehavior {
    
    public UIAppPanelMode panelMode = UIAppPanelMode.ModeMain;
    public bool _isVisible = false;
    public string className = "";

    public Camera panelCamera;

    public virtual bool isVisible {
        get {
            return _isVisible; 
        }
        set {
            _isVisible = value;
        }
    }
    
    public virtual void Start() {
        Init();
    }
    
    public virtual void Init() {
        if (GameGlobal.Instance == null) {
            Context.Current.ApplicationLoadLevelByName("GameUISceneRoot");
        }
        else {
            GetClassName(this);
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
        if (obj != null) {
            obj.Show();
        }
    }
    
    public void HideObject(GameObject obj) {
        if (obj != null) {
            obj.Hide();
        }
    }
    
    public string GetClassName(object item) {
        className = item.GetType().Name;
        //LogUtil.Log("CLASS NAME:" + className);
        return className;
    }

    public void ShowCamera() {

        if(panelCamera != null) {
            panelCamera.ShowCameraFadeIn();
        }
    }

    public void HideCamera() {
        
        if(panelCamera != null) {
            panelCamera.HideCameraFadeOut();
        }
    }

}
