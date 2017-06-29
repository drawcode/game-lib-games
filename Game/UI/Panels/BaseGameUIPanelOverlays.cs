using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;
using Engine.Utility;

public class BaseGameUIPanelOverlays : GameUIPanelBase {

    public static GameUIPanelOverlays Instance;

    public GameObject containerObject;

    public GameObject overlayWhiteRadial;
    public GameObject overlayBlackSolid;
    public GameObject overlayWhiteSolid;
    public GameObject overlayWhiteSolidStatic;

    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public override void Awake() {
        base.Awake();
    }

    public override void OnEnable() {

        //Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnDisable() {

        //Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
            //
        }
    }

    public override void OnButtonClickEventHandler(string buttonName) {

    }

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();

        //LoadData();
        AnimateIn();
    }

    public virtual void LoadData() {
        StartCoroutine(LoadDataCo());
    }

    IEnumerator LoadDataCo() {
        yield break;
    }

    // OVERLAY WHITE FLASH

    public virtual void ShowOverlayWhiteFlash() {
        //LogUtil.Log("ShowWhiteFlash");

        //DeviceUtil.Vibrate();		
        ShowOverlayWhiteFlashOut();
        HideOverlayWhiteFlashOut();
    }

    public virtual void ShowOverlayWhiteFlashOut() {
        //LogUtil.Log("ShowOverlayWhiteFlashOut");		
        //DeviceUtil.Vibrate();		
        ShowOverlayWhite();
    }

    public virtual void HideOverlayWhiteFlashOut() {
        //LogUtil.Log("HideOverlayWhiteFlash");
        HideOverlayWhite(.3f, .3f);
    }

    // OVERLAY WHITE

    public virtual void ShowOverlayWhite() {
        ShowOverlayWhite(.5f, .55f);
    }

    public virtual void ShowOverlayWhite(float time, float delay) {

        TweenUtil.ShowObjectBottom(overlayWhiteSolid, TweenCoord.local, true, time, delay);
    }

    public virtual void HideOverlayWhite() {
        HideOverlayWhite(.5f, 0f);
    }

    public virtual void HideOverlayWhite(float time, float delay) {

        TweenUtil.HideObjectBottom(overlayWhiteSolid, TweenCoord.local, true, time, delay);
    }

    // OVERLAY WHITE STATIC 

    public virtual void ShowOverlayWhiteStatic() {
        ShowOverlayWhiteStatic(.5f, .55f);
    }

    public virtual void ShowOverlayWhiteStatic(float time, float delay) {

        TweenUtil.ShowObjectBottom(overlayWhiteSolidStatic, TweenCoord.local, true, time, delay);
    }

    public virtual void HideOverlayWhiteStatic() {
        HideOverlayWhiteStatic(.5f, 0f);
    }

    public virtual void HideOverlayWhiteStatic(float time, float delay) {

        TweenUtil.HideObjectBottom(overlayWhiteSolidStatic, TweenCoord.local, true, time, delay);
    }

    // OVERLAY WHITE RADIAL

    public virtual void ShowOverlayWhiteRadial() {
        ShowOverlayWhite(.5f, .55f);
    }

    public virtual void ShowOverlayWhiteRadial(float time, float delay) {

        TweenUtil.ShowObjectBottom(overlayWhiteRadial, TweenCoord.local, true, time, delay);
    }

    public virtual void HideOverlayWhiteRadial() {
        HideOverlayWhiteRadial(.5f, 0f);
    }

    public virtual void HideOverlayWhiteRadial(float time, float delay) {

        TweenUtil.HideObjectBottom(overlayWhiteRadial, TweenCoord.local, true, time, delay);
    }

    // OVERLAY BLACK 

    public virtual void ShowOverlayBlack() {
        ShowOverlayBlack(.5f, .55f);
    }

    public virtual void ShowOverlayBlack(float time, float delay) {

        TweenUtil.ShowObjectBottom(overlayBlackSolid, TweenCoord.local, true, time, delay);
    }

    public virtual void HideOverlayBlack() {
        HideOverlayBlack(.5f, 0f);
    }

    public virtual void HideOverlayBlack(float time, float delay) {

        TweenUtil.HideObjectBottom(overlayBlackSolid, TweenCoord.local, true, time, delay);
    }

    // OVERLAY HIDE ALL

    public virtual void HideAll() {
        HideOverlayWhite(0f, 0f);
        HideOverlayWhiteRadial(0f, 0f);
        HideOverlayBlack(0f, 0f);
        HideOverlayWhiteStatic();
    }

    // ANIMATE/EASING

    public override void AnimateIn() {

        base.AnimateIn();

        HideAll();
    }

    public override void AnimateOut() {

        base.AnimateOut();

        HideAll();
    }


    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }

        //var ry = 0f;
        //var rx = 0f;
        if(Context.Current.isMobile) {
            //ry =-Input.acceleration.y + Screen.height/2;	
            //rx =-Input.acceleration.x + Screen.width/2;	
        }
        else {
            //ry =-Input.mousePosition.y + Screen.height/2;	
            //rx =-Input.acceleration.x + Screen.width/2;				
        }

        if(overlayWhiteRadial != null) {
            //overlayWhiteRadial.transform.Rotate(Vector3.forward * (ry * .005f) * Time.deltaTime); 
        }

        if(Application.isEditor) {
            if(Input.GetKeyDown(KeyCode.O)) {
                ShowOverlayWhiteFlashOut();
            }

            if(Input.GetKeyDown(KeyCode.I)) {
                HideOverlayWhiteFlashOut();
            }

            if(Input.GetKeyDown(KeyCode.U)) {
                ShowOverlayWhiteFlash();
            }
        }
    }
}