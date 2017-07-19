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

    public virtual void Awake() {
        GetClassName(this);
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {

        if(GameGlobal.Instance == null) {

            Context.Current.ApplicationLoadLevelByName("GameUISceneRoot");
        }
        else {

            GetClassName(this);

            FindCameraAbove();
        }
    }

    public Camera FindCameraAbove() {

        panelCamera = gameObject.FindTypeAboveRecursive<Camera>();

        return panelCamera;
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

    public void HideCamera(float delay) {
        StartCoroutine(HideCameraCo(delay));
    }

    IEnumerator HideCameraCo(float delay) {

        yield return new WaitForSeconds(delay);

        HideCamera();
    }

    public void HideCamera() {

        if(panelCamera != null) {
            panelCamera.HideCameraFadeOut();
        }
    }
}