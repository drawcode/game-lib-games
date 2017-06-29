using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;
using Engine.Utility;

public class BaseGameUIPanelHeader : GameUIPanelBase {

    public static GameUIPanelHeader Instance;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIImageButton buttonCoins;
    public UIImageButton buttonBack;
    public UILabel labelSection;
#else
    public Button buttonCoins;
    public Button buttonBack;
    public Text labelSection;
#endif


    /*
    easeInQuad
    easeOutQuad
    easeInOutQuad
    easeInCubic
    easeOutCubic
    easeInOutCubic
    easeInQuart
    easeOutQuart
    easeInOutQuart
    easeInQuint
    easeOutQuint
    easeInOutQuint
    easeInSine
    easeOutSine
    easeInOutSine
    easeInExpo
    easeOutExpo
    easeInOutExpo
    easeInCirc
    easeOutCirc
    easeInOutCirc
    linear
    spring
    easeInBounce
    easeOutBounce
    easeInOutBounce
    easeInBack
    easeOutBack
    easeInOutBack
    easeInElastic
    easeOutElastic
    easeInOutElastic
    
    */

    public GameObject coinObject;
    public GameObject backObject;
    public GameObject backerObject;
    public GameObject titleObject;
    public GameObject containerCharacters;
    public GameObject containerCharacter;
    public GameObject containerCharacterLarge;
    public GameCustomPlayerContainer containerCustomCharacterSmall;
    public GameCustomPlayerContainer containerCustomCharacterLarge;

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

    public override void Start() {
        Init();
    }

    public override void Init() {
        base.Init();
        loadData();

        InitCharacters();

        //base.AnimateIn();
        AnimateIn();
    }

    public virtual void InitCharacters() {

        if(containerCustomCharacterSmall == null) {
            containerCustomCharacterSmall = containerCharacter.Get<GameCustomPlayerContainer>();
        }

        if(containerCustomCharacterLarge == null) {
            containerCustomCharacterLarge = containerCharacterLarge.Get<GameCustomPlayerContainer>();
        }

        characterLargeShowFront();
        characterLargeZoomOut();
    }

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

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

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

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

        HideCharacters();

        if(className == classNameTo) {
            //

            if(code.Contains("-internal")) {
                AnimateInInternal();
            }
        }
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);

#if ENABLE_FEATURE_PRODUCT_CURRENCY
        if(buttonCoins != null) {

            if(buttonName == buttonCoins.name) {
                GameCommunity.HideGameCommunity();
                GameUIController.ShowProductCurrency();
            }
        }
#endif
    }

    public override void AnimateIn() {

        backgroundDisplayState = UIPanelBackgroundDisplayState.None;

        base.AnimateIn();
    }

    public virtual void AnimateInMain() {

        AnimateIn();

        showMain();
    }

    public virtual void AnimateInInternal() {

        AnimateIn();

        showFull();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideBackButtonObject();
        HideBackerObject();
        HideCoinsObject();
        HideTitleObject();

        HideCharacter();
    }

    //

    public static void CharacterLargeShowPose() {
        if(Instance != null) {
            Instance.characterLargeShowPose();
        }
    }

    public void characterLargeShowPose() {
        characterLargeRotation(.89);
    }

    public static void CharacterLargeShowFront() {
        if(Instance != null) {
            Instance.characterLargeShowFront();
        }
    }

    public void characterLargeShowFront() {
        characterLargeRotation(0);
    }

    public static void CharacterLargeShowBack() {
        if(Instance != null) {
            Instance.characterLargeShowBack();
        }
    }

    public void characterLargeShowBack() {
        characterLargeRotation(.5);
    }

    //

    public static void CharacterLargeZoomOut() {
        if(Instance != null) {
            Instance.characterLargeZoomOut();
        }
    }

    public void characterLargeZoomOut() {
        characterLargeZoom(1.0);
    }

    public static void CharacterLargeZoomIn() {
        if(Instance != null) {
            Instance.characterLargeZoomIn();
        }
    }

    public void characterLargeZoomIn() {
        characterLargeZoom(2.0);
    }

    public static void CharacterLargeZoom(double scaleTo) {
        if(Instance != null) {
            Instance.characterLargeZoom(scaleTo);
        }
    }

    public void characterLargeZoom(double scaleTo) {
        characterLargeScale(scaleTo);
    }

    //

    public static void CharacterLargeRotation(double valEnd) {
        if(Instance != null) {
            Instance.characterLargeRotation(valEnd);
        }
    }

    public void characterLargeRotation(double rotationTo) {

        if(containerCustomCharacterLarge == null) {
            return;
        }

        containerCustomCharacterLarge.HandleContainerRotation(rotationTo);
    }

    //

    public static void CharacterLargeScale(double valEnd) {
        if(Instance != null) {
            Instance.characterLargeScale(valEnd);
        }
    }

    public void characterLargeScale(double scaleTo) {

        if(containerCustomCharacterLarge == null) {
            return;
        }

        containerCustomCharacterLarge.HandleContainerScale(scaleTo);
    }

    //

    public static void CharacterSmallScale(double scaleTo) {
        if(Instance != null) {
            Instance.characterSmallScale(scaleTo);
        }
    }

    public void characterSmallScale(double scaleTo) {

        if(containerCustomCharacterSmall == null) {
            return;
        }

        containerCustomCharacterSmall.HandleContainerScale(scaleTo);
    }

    //

    public static void HideTitle() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.hideTitle();
        }
    }

    public virtual void hideTitle() {
        UIUtil.HideLabel(labelSection);
    }

    public static void ShowTitle(string title) {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showTitle(title);
        }
    }

    public virtual void showTitle(string title) {
        UIUtil.ShowLabel(labelSection);
        UIUtil.SetLabelValue(labelSection, title);
    }

    public static void ShowFull() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showFull();
        }
    }

    public virtual void showFull() {
        ShowCoinsObject();
        ShowBackerObject();
        ShowBackButtonObject();
        ShowTitleObject();
    }

    public static void ShowMain() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showMain();
        }
    }

    public virtual void showMain() {
        ShowCoinsObject();
        HideBackerObject();
        HideBackButtonObject();
        HideTitleObject();
    }

    public static void ShowNone() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showNone();
        }
    }

    public virtual void showNone() {
        AnimateOut();
    }

    // characters

    public static void HideCharacters() {
        HideCharacter();
        HideCharacterLarge();
    }

    // characters 

    public static void ShowCharacter() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showCharacter();
        }
    }

    public virtual void showCharacter() {
        StartCoroutine(showCharacterCo());
    }

    public IEnumerator showCharacterCo() {
        yield return new WaitForSeconds(.55f);
        TweenUtil.ShowObjectTop(containerCharacter);

        if(containerCharacter != null) {
            containerCharacter.ResetRigidBodiesVelocity();
        }

        if(containerCustomCharacterSmall != null) {
            containerCustomCharacterSmall.HandleContainerScale(1);
            containerCustomCharacterSmall.HandleContainerRotation(.91);

            InputSystem.Instance.currentDraggableUIGameObject =
                containerCustomCharacterSmall.containerRotator;
        }
    }

    public static void HideCharacter() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.hideCharacter();
        }
    }

    public virtual void hideCharacter() {
        TweenUtil.HideObjectTop(containerCharacter);

        InputSystem.Instance.currentDraggableUIGameObject =
            null;
    }

    // large

    public static void ShowCharacterLarge() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showCharacterLarge();
        }
    }

    public virtual void showCharacterLarge() {
        StartCoroutine(showCharacterLargeCo());
    }

    public IEnumerator showCharacterLargeCo() {
        yield return new WaitForSeconds(.55f);
        TweenUtil.ShowObjectTop(containerCharacterLarge);

        if(containerCharacterLarge != null) {
            containerCharacterLarge.ResetRigidBodiesVelocity();

            InputSystem.Instance.currentDraggableUIGameObject =
                containerCustomCharacterLarge.containerRotator;
        }

        characterLargeShowPose();
        characterLargeZoomOut();
    }

    public static void HideCharacterLarge() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.hideCharacterLarge();
        }
    }

    public virtual void hideCharacterLarge() {
        TweenUtil.HideObjectTop(containerCharacterLarge);

        InputSystem.Instance.currentDraggableUIGameObject = null;
    }

    public virtual void ShowBackButtonObject() {

        if(backObject != null) {

            backerObject.Show();

            TweenUtil.ShowObjectLeft(backObject);

            //UITweenerUtil.MoveTo(backObject,
            //    UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, Vector3.zero);

            //UITweenerUtil.FadeTo(backObject,
            //    UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);

            foreach(Transform t in backObject.transform) {

                TweenUtil.FadeToObject(t.gameObject, 1f, 1f);

                //UITweenerUtil.FadeTo(t.gameObject,
                //    UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);
            }
        }
    }

    public virtual void HideBackButtonObject() {

        if(backObject != null) {

            TweenUtil.HideObjectLeft(backObject);

            //UITweenerUtil.MoveTo(backObject,
            //    UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, Vector3.zero.WithX(-3000));

            //UITweenerUtil.FadeTo(backObject,
            //    UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);

            foreach(Transform t in backObject.transform) {

                TweenUtil.FadeToObject(t.gameObject, 0f, .3f);

                //UITweenerUtil.FadeTo(t.gameObject,
                //UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);
            }
        }
    }

    public virtual void ShowBackerObject() {
        TweenUtil.FadeToObject(backerObject, 1f);
    }

    public virtual void HideBackerObject() {
        TweenUtil.FadeToObject(backerObject, 0f);
    }

    public virtual void ShowTitleObject() {
        TweenUtil.FadeToObject(titleObject, 1f);
    }

    public virtual void HideTitleObject() {
        TweenUtil.FadeToObject(titleObject, 0f);
    }

    public virtual void ShowCoinsObject() {
        TweenUtil.FadeToObject(coinObject, 1f);
    }

    public virtual void HideCoinsObject() {
        TweenUtil.FadeToObject(coinObject, 0f);
    }

    public static void LoadData() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.loadData();
        }
    }

    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        yield return new WaitForSeconds(1f);
    }

    public virtual void Update() {

        if(Input.GetKey(KeyCode.LeftControl)) {
            if(Input.GetKey(KeyCode.LeftAlt)) {

                if(Input.GetKey(KeyCode.N)) {

                    CharacterLargeScale(2.0f);
                }

                if(Input.GetKey(KeyCode.M)) {

                    CharacterLargeScale(1.0f);
                }
            }
        }
    }
}