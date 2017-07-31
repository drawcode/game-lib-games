#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelDialogBackground : UIPanelBase {

    public static UIPanelDialogBackground Instance;

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

        loadData();
    }

    public override void Start() {
        Init();
    }

    public override void OnEnable() {

    }

    public override void OnDisable() {

    }

    public override void OnButtonClickEventHandler(string buttonName) {

    }

    public static void ShowDefault() {
        if(isInst) {
            Instance.showDefault();
        }
    }
    public void showDefault() {

        ShowCamera();

        AnimateIn();
        loadData();
    }
    
    public static void HideAll() {
        if(isInst) {
            Instance.hideAll();
        }
    }

    public void hideAll() {

        AnimateOut();

        HideCamera();
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

    /*
    public override void AnimateIn() {
        ShowCamera(.45f);
        base.AnimateIn();
    }

    public override void AnimateOut() {
        base.AnimateOut();
        HideCamera();
    }
    */
}