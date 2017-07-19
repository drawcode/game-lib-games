using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;
using Engine.Utility;

public enum UIPanelBackgroundDisplayState {
    None,
    PanelBacker
}

public enum UIPanelCharacterDisplayState {
    None,
    Character,
    CharacterLarge
}

public enum UIPanelButtonsDisplayState {
    None,
    Character,
    CharacterLarge,
    CharacterTools,
    CharacterCustomize,
    Statistics,
    Achievements,
    GameNetworks,
    ProductsSections
}

public enum UIPanelAdDisplayState {
    None,
    BannerTop,
    BannerBottom,
    Video,
    VideoIncentivized,
    Interstitial
}

public class UIPanelBaseTypes {
    public static string typeDefault = "type-default";
    public static string typeDialogHUD = "type-dialog-hud";
    public static string typeModalHUD = "type-modal-hud";
    public static string typeDialogUI = "type-dialog-ui";
    public static string typeModalUI = "type-modal-ui";
    public static string typeDialogDialog = "type-dialog-dialog";
    public static string typeModalDialog = "type-modal-dialog";
    public static string typeDialogOverlay = "type-dialog-overlay";
    public static string typeModalOverlay = "type-modal-overlay";
}

public class UIPanelBase : UIAppPanel {

    public UIPanelCharacterDisplayState characterDisplayState = UIPanelCharacterDisplayState.None;
    public UIPanelButtonsDisplayState buttonDisplayState = UIPanelButtonsDisplayState.None;
    public UIPanelBackgroundDisplayState backgroundDisplayState = UIPanelBackgroundDisplayState.None;
    public UIPanelAdDisplayState adDisplayState = UIPanelAdDisplayState.None;
    public GameObject listGridRoot;
    public UIGrid listGrid;
    public UIPanel panelClipped;
    public UIDraggablePanel draggablePanel;
    public UIScrollBar draggablePanelScrollbar;

    public GameObject panelLeftObject;
    public GameObject panelLeftBottomObject;
    public GameObject panelLeftTopObject;
    public GameObject panelRightObject;
    public GameObject panelRightBottomObject;
    public GameObject panelRightTopObject;
    public GameObject panelTopObject;
    public GameObject panelBottomObject;
    public GameObject panelCenterObject;
    public GameObject panelContainer;

    [NonSerialized]
    public float
        durationShow = .45f;
    [NonSerialized]
    public float
        durationHide = .45f;
    [NonSerialized]
    public float
        durationDelayShow = .5f;
    [NonSerialized]
    public float
        durationDelayHide = 0f;
    [NonSerialized]
    public float
        leftOpenX = 0f;
    [NonSerialized]
    public float
        leftClosedX = -4500f;
    [NonSerialized]
    public float
        rightOpenX = 0f;
    [NonSerialized]
    public float
        rightClosedX = 4500f;
    [NonSerialized]
    public float
        bottomOpenY = 0f;
    [NonSerialized]
    public float
        bottomClosedY = -4500f;
    [NonSerialized]
    public float
        topOpenY = 0f;
    [NonSerialized]
    public float
        topClosedY = 4500f;
    public int increment = 0;
    public List<string> panelTypes = new List<string>();//UIPanelBaseTypes.typeDefault;

    public override bool isVisible {
        get {

            if(panelContainer != null) {
                if(_isVisible) {
                    //if (!panelContainer.GetActive()) {
                    //_isVisible = false;
                    //}
                }
                else {
                    //if (panelContainer.GetActive()) {
                    //_isVisible = true;
                    //}
                }
            }
            return _isVisible;
        }
        set {
            _isVisible = value;
        }
    }

    public virtual void OnEnable() {

        panelTypes.Add(UIPanelBaseTypes.typeDefault);

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, Dictionary<string, object>>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_DATA, OnButtonClickEventDataHandler);

        Messenger<string>.AddListener(UIControllerMessages.uiPanelAnimateIn, OnUIControllerPanelAnimateIn);
        Messenger<string>.AddListener(UIControllerMessages.uiPanelAnimateOut, OnUIControllerPanelAnimateOut);

        Messenger<string>.AddListener(UIControllerMessages.uiPanelAnimateInType, OnUIControllerPanelAnimateInType);
        Messenger<string>.AddListener(UIControllerMessages.uiPanelAnimateOutType, OnUIControllerPanelAnimateOutType);

        Messenger<string, string>.AddListener(UIControllerMessages.uiPanelAnimateInClassType, OnUIControllerPanelAnimateInClassType);
        Messenger<string, string>.AddListener(UIControllerMessages.uiPanelAnimateOutClassType, OnUIControllerPanelAnimateOutClassType);

        Messenger<string, string>.AddListener(UIControllerMessages.uiPanelAnimateType, OnUIControllerPanelAnimateType);
    }

    public virtual void OnDisable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        Messenger<string, Dictionary<string, object>>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_DATA, OnButtonClickEventDataHandler);

        Messenger<string>.RemoveListener(UIControllerMessages.uiPanelAnimateIn, OnUIControllerPanelAnimateIn);
        Messenger<string>.RemoveListener(UIControllerMessages.uiPanelAnimateOut, OnUIControllerPanelAnimateOut);

        Messenger<string>.RemoveListener(UIControllerMessages.uiPanelAnimateInType, OnUIControllerPanelAnimateInType);
        Messenger<string>.RemoveListener(UIControllerMessages.uiPanelAnimateOutType, OnUIControllerPanelAnimateOutType);

        Messenger<string, string>.RemoveListener(UIControllerMessages.uiPanelAnimateInClassType, OnUIControllerPanelAnimateInClassType);
        Messenger<string, string>.RemoveListener(UIControllerMessages.uiPanelAnimateOutClassType, OnUIControllerPanelAnimateOutClassType);

        Messenger<string, string>.RemoveListener(UIControllerMessages.uiPanelAnimateType, OnUIControllerPanelAnimateType);
    }

    public virtual void OnButtonClickEventHandler(
        string buttonName) {

    }

    public virtual void OnButtonClickEventDataHandler(
        string buttonName, Dictionary<string, object> data) {

    }

    public virtual void OnUIControllerPanelAnimateIn(string classNameTo) {

        if(className == classNameTo) {

            HandleUniquePanelTypes();

            AnimateIn();
        }
    }

    public virtual void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public virtual void OnUIControllerPanelAnimateInType(string panelTypeTo) {
        if(panelTypes.Contains(panelTypeTo)) {
            AnimateIn();
        }
    }

    public virtual void OnUIControllerPanelAnimateOutType(string panelTypeTo) {
        if(panelTypes.Contains(panelTypeTo)) {
            AnimateOut();
        }
    }

    public virtual void OnUIControllerPanelAnimateInClassType(string classNameTo, string panelTypeTo) {
        if(className != classNameTo && panelTypes.Contains(panelTypeTo)) {
            AnimateIn();
        }
    }

    public virtual void OnUIControllerPanelAnimateOutClassType(string classNameTo, string panelTypeTo) {
        if(className != classNameTo && panelTypes.Contains(panelTypeTo)) {
            AnimateOut();
        }
    }

    public virtual void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
            //
        }
    }

    public virtual void HandleUniquePanelTypes() {

        if(panelTypes.Count > 1) {
            // if this is a special panel, hide the others like it such as dialogs...modals
            foreach(string panelType in panelTypes) {

                if(panelType == UIPanelBaseTypes.typeDefault) {
                    continue;
                }

                Messenger<string, string>.Broadcast(UIControllerMessages.uiPanelAnimateOutClassType, className, panelType);
                LogUtil.Log("OnUIControllerPanelAnimateIn:", " className:" + className + " panelType:" + panelType);
            }
        }
    }

    public override void Start() {
        base.Start();

        AnimateOut();
    }

    public void HideAllPanels() {
        foreach(UIAppPanelBase baseItem in Resources.FindObjectsOfTypeAll(typeof(UIAppPanelBase))) {
            baseItem.AnimateOut();
        }
    }

    public void HideAllPanelsNow() {
        foreach(UIAppPanelBase baseItem in Resources.FindObjectsOfTypeAll(typeof(UIAppPanelBase))) {
            baseItem.AnimateOut(); // handle per panel actions
            baseItem.AnimateOutNow(); // but animate it out now it now
        }
    }

    // CENTER

    public virtual void AnimateInCenter(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInCenter(panelCenterObject, time, delay, fade);
    }

    public virtual void AnimateInCenter(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        TweenUtil.ShowObjectBottom(go, TweenCoord.local, fade, time, delay);
    }

    public virtual void AnimateOutCenter(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutCenter(panelCenterObject, time, delay, fade);
    }

    public virtual void AnimateOutCenter(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        TweenUtil.HideObjectBottom(go, TweenCoord.local, fade, time, delay);
    }

    // LEFT

    public virtual void AnimateInLeft(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInLeft(panelLeftObject, time, delay, fade);
    }

    public virtual void AnimateInLeft(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        TweenUtil.ShowObjectLeft(go, TweenCoord.local, fade, time, delay);
    }

    public virtual void AnimateOutLeft(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutLeft(panelLeftObject, time, delay, fade);
    }

    public virtual void AnimateOutLeft(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        TweenUtil.HideObjectLeft(go, TweenCoord.local, fade, time, delay);
    }

    // LEFT BOTTOM

    public virtual void AnimateInLeftBottom(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInLeftBottom(panelLeftBottomObject, time, delay, fade);
    }

    public virtual void AnimateInLeftBottom(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInLeft(go, time, delay, fade);
    }

    public virtual void AnimateOutLeftBottom(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutLeftBottom(panelLeftBottomObject, time, delay, fade);
    }

    public virtual void AnimateOutLeftBottom(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutLeft(go, time, delay, fade);
    }

    // LEFT TOP

    public virtual void AnimateInLeftTop(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInLeftTop(panelLeftTopObject, time, delay, fade);
    }

    public virtual void AnimateInLeftTop(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInLeft(go, time, delay, fade);
    }

    public virtual void AnimateOutLeftTop(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutLeftTop(panelLeftTopObject, time, delay, fade);
    }

    public virtual void AnimateOutLeftTop(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutLeft(go, time, delay, fade);
    }

    // RIGHT

    public virtual void AnimateInRight(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInRight(panelRightObject, time, delay, fade);
    }

    public virtual void AnimateInRight(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        TweenUtil.ShowObjectRight(go, TweenCoord.local, fade, time, delay);
    }

    public virtual void AnimateOutRight(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutRight(panelRightObject, time, delay, fade);
    }

    public virtual void AnimateOutRight(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        TweenUtil.HideObjectRight(go, TweenCoord.local, fade, time, delay);
    }

    // BOTTOM RIGHT

    public virtual void AnimateInRightBottom(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInRightBottom(panelRightBottomObject, time, delay, fade);
    }

    public virtual void AnimateInRightBottom(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInRight(go, time, delay, fade);
    }

    public virtual void AnimateOutRightBottom(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutRightBottom(panelRightBottomObject, time, delay, fade);
    }

    public virtual void AnimateOutRightBottom(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutRight(go, time, delay, fade);
    }

    // TOP RIGHT

    public virtual void AnimateInRightTop(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInRightTop(panelRightTopObject, time, delay, fade);
    }

    public virtual void AnimateInRightTop(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInRight(go, time, delay, fade);
    }

    public virtual void AnimateOutRightTop(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutRightTop(panelRightTopObject, time, delay, fade);
    }

    public virtual void AnimateOutRightTop(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutRight(go, time, delay, fade);
    }

    // TOP

    public virtual void AnimateInTop(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInTop(panelTopObject, time, delay, fade);
    }

    public virtual void AnimateInTop(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        TweenUtil.ShowObjectTop(go, TweenCoord.local, fade, time, delay);
    }

    public virtual void AnimateOutTop(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutTop(panelTopObject, time, delay, fade);
    }

    public virtual void AnimateOutTop(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        TweenUtil.HideObjectTop(go, TweenCoord.local, fade, time, delay);
    }

    // BOTTOM

    public virtual void AnimateInBottom(float time = .5f, float delay = .5f, bool fade = true) {
        AnimateInBottom(panelBottomObject, time, delay, fade);
    }

    public virtual void AnimateInBottom(GameObject go, float time = .5f, float delay = .5f, bool fade = true) {
        TweenUtil.ShowObjectBottom(go, TweenCoord.local, fade, time, delay);
    }

    public virtual void AnimateOutBottom(float time = .3f, float delay = 0f, bool fade = true) {
        AnimateOutBottom(panelBottomObject, time, delay, fade);
    }

    public virtual void AnimateOutBottom(GameObject go, float time = .3f, float delay = 0f, bool fade = true) {
        TweenUtil.HideObjectBottom(go, TweenCoord.local, fade, time, delay);
    }

    // ANIMATE

    public virtual void AnimateIn() {

        //AnimateOut(0f, 0f);

        HandleUniquePanelTypes();

        ShowPanel();

        float time = durationShow;
        float delay = durationDelayShow;

        //AnimateCancelEasing(delay);

        AnimateIn(time, delay);
    }

    public virtual void AnimateCancelEasing(float delay) {

        StartCoroutine(AnimateCancelEasingCo(delay));
    }

    IEnumerator AnimateCancelEasingCo(float delay) {

        yield return new WaitForSeconds(delay);

        LeanTween.cancelAll();
    }

    public virtual void AnimateIn(float time = .5f, float delay = .5f) {

        if(isVisible) {
            return;
        }

        //ShowCamera();

        HandleShow();

        HandleCharacterDisplay();

        HandleAdDisplay();

        HandleButtonDisplay();

        HandleBackgroundDisplay();

        AnimateInCenter(time, delay);
        AnimateInLeft(time, delay);
        AnimateInLeftBottom(time, delay);
        AnimateInLeftTop(time, delay);
        AnimateInRight(time, delay);
        AnimateInRightBottom(time, delay);
        AnimateInRightTop(time, delay);
        AnimateInTop(time, delay);
        AnimateInBottom(time, delay);

        isVisible = true;
    }

    public virtual void AnimateOut() {

        float time = durationHide;
        float delay = durationDelayHide;

        AnimateOut(time, delay);
    }

    public virtual void AnimateOutNow() {

        float time = 0f;
        float delay = 0f;

        AnimateOut(time, delay);
    }

    public virtual void AnimateOut(float time, float delay) {

        if(!isVisible) {
            //return;
        }

        //HideCamera();

        HandleHide();

        AdNetworks.HideAd();

        AnimateOutCenter(time, delay);
        AnimateOutLeft(time, delay);
        AnimateOutLeftBottom(time, delay);
        AnimateOutLeftTop(time, delay);
        AnimateOutRight(time, delay);
        AnimateOutRightBottom(time, delay);
        AnimateOutRightTop(time, delay);
        AnimateOutTop(time, delay);
        AnimateOutBottom(time, delay);

        ListClear();

        if(panelContainer != null) {
            if(!panelContainer.activeSelf || !panelContainer.activeInHierarchy) {
                panelContainer.Hide();
            }
            else {
                StartCoroutine(HidePanelCo(delay + .5f));
            }
        }

        isVisible = false;
    }

    public IEnumerator HidePanelCo(float delay) {
        yield return new WaitForSeconds(delay);

        HidePanel();
    }

    public virtual void HidePanel() {

        if(!isVisible) {
            if(panelContainer != null) {
                //isVisible = false;
                panelContainer.Hide();
            }
        }
    }

    public virtual void ShowPanel() {

        if(isVisible) {
            return;
        }

        if(panelContainer != null) {
            //isVisible = true;
            panelContainer.Show();
        }
    }

    void Update() {

    }

    public void ListContainerScale(GameObject listObject, float scaleTo) {
        if(listObject != null) {
            Vector3 currentScale = listObject.transform.localScale;

            float screenWidth = 640;
            float screenHeight = 960;

            scaleTo = Mathf.Clamp(scaleTo / (screenWidth / screenHeight), .5f, 2f);

            currentScale = currentScale.WithX(scaleTo).WithY(scaleTo).WithZ(scaleTo);

            listObject.transform.localScale = currentScale;
        }
    }

    public void ListScale(GameObject listObject, float scaleTo) {
        if(listObject != null) {
            Vector3 currentScale = listObject.transform.localScale;

            float screenWidth = 640;
            float screenHeight = 960;

            scaleTo = Mathf.Clamp(scaleTo / (screenWidth / screenHeight), .5f, 2f);

            currentScale = currentScale.WithX(scaleTo).WithY(scaleTo).WithZ(scaleTo);

            listObject.transform.localScale = currentScale;
        }
    }

    public void PanelScale(UIPanel panel) {
        if(panelClipped != null) {
            Vector4 range = panelClipped.clipRange;
            range.x = 0f;
            //range.y = 0f;
            range.z = 2500f;
            //range.y = 2500f;
            //range.w = 380f;
            panelClipped.clipRange = range;
        }
    }

    public void ListScale(float scaleTo) {
        if(listGridRoot != null) {
            ListScale(listGridRoot, scaleTo);
        }

        PanelScale(panelClipped);

        ListReposition();
    }

    public void ListClear() {
        if(listGridRoot != null && isVisible) {
            ListClear(listGridRoot);
        }
    }

    public void ListClear(GameObject listObject) {
        if(listObject != null && isVisible) {
            listObject.DestroyChildren();
        }
    }

    public void ListReposition() {
        increment = 0;
        if(listGrid != null) {
            RepositionList(listGrid, listGridRoot);
        }
    }

    public void ListReposition(UIGrid grid, GameObject gridObject) {
        increment = 0;
        if(grid != null) {
            RepositionList(grid, gridObject);
        }
    }

    public void RepositionList(UIGrid grid, GameObject gridObject) {
        if(grid != null) {
            grid.Reposition();
            if(gridObject.transform.parent != null) {

                UIDraggablePanel[] dragPanels =
                    gridObject.transform.parent.gameObject.GetComponentsInChildren<UIDraggablePanel>();

                if(dragPanels != null) {
                    foreach(UIDraggablePanel panel
                     in dragPanels) {
                        panel.ResetPosition();
                        break;
                    }
                }
            }
        }
    }

    public void RepositionListScroll(float scrollValue) {
        if(draggablePanelScrollbar != null) {
            draggablePanelScrollbar.scrollValue = 0;
        }
        else if(draggablePanel != null) {
            draggablePanel.ResetPosition();
        }
    }


    // LOADING

    public virtual GameObject LoadObject(GameObject prefabObject, string itemName) {
        return LoadObject(listGridRoot, prefabObject, itemName);
    }

    public virtual GameObject LoadObject(GameObject prefabObject, string itemName,
                                         string title, string description, string note, string type) {
        return LoadObject(listGridRoot, prefabObject, itemName, title, description, note, type);
    }

    public virtual GameObject LoadObject(GameObject listObject, GameObject prefabObject, string itemName) {
        if(listObject == null) {
            return null;
        }
        if(prefabObject == null) {
            return null;
        }
        GameObject item = NGUITools.AddChild(listObject, prefabObject);
        item.name = "_" + increment++ + "_" + itemName;
        return item;
    }

    public virtual GameObject LoadObject(GameObject listObject, GameObject prefabObject, string itemName,
                                         string title, string description, string note, string type) {

        if(listObject == null) {
            return null;
        }

        if(prefabObject == null) {
            return null;
        }

        GameObject item = LoadObject(listObject, prefabObject, itemName);
        SetItemLabel(item, "LabelName", title);
        SetItemLabel(item, "LabelDescription", description);
        SetItemLabel(item, "LabelNote", note);

        // show type icon

        Transform typeObjects = item.transform.Find("types");

        if(typeObjects != null) {
            foreach(Transform t in typeObjects.gameObject.transform) {
                t.gameObject.Hide(); // hide all 
            }

            Transform typeObject = typeObjects.Find(type);
            if(typeObject != null) {
                // show current
                typeObject.gameObject.Show();
            }
        }

        return item;
    }

    public void SetItemLabel(GameObject item, string labelName, string val) {
        if(item == null) {
            return;
        }

        UILabel label = GetItemLabel(item, labelName);
        if(label != null) {
            label.text = val;
        }
    }

    public UILabel GetItemLabel(GameObject item, string labelName) {
        if(item == null) {
            return null;
        }

        Transform t = item.transform.Find(labelName);
        if(t != null) {
            UILabel label = t.GetComponent<UILabel>();
            if(label != null) {
                return label;
            }
        }
        return null;
    }

    // PANEL SECTIONS STATES

    public void HandleCharacterDisplay() {

        // handle character display

        if(characterDisplayState ==
            UIPanelCharacterDisplayState.Character) {

            GameUIPanelHeader.ShowCharacter();
        }
        else if(characterDisplayState ==
            UIPanelCharacterDisplayState.CharacterLarge) {

            GameUIPanelHeader.ShowCharacterLarge();
        }
    }

    public void HandleAdDisplay() {

        // handle character display

        if(adDisplayState ==
            UIPanelAdDisplayState.BannerBottom) {

            AdNetworks.ShowAd(
                AdDisplayType.Banner, AdPosition.BottomCenter);
        }
        else if(adDisplayState ==
            UIPanelAdDisplayState.BannerTop) {

            AdNetworks.ShowAd(
                AdDisplayType.Banner, AdPosition.TopCenter);
        }
        else if(adDisplayState ==
            UIPanelAdDisplayState.Video) {

            AdNetworks.ShowAd(
                AdDisplayType.Video, AdPosition.Full);
        }
        else if(adDisplayState ==
            UIPanelAdDisplayState.Video) {

            AdNetworks.ShowAd(
                AdDisplayType.VideoIncentivized, AdPosition.Full);
        }
        else {

            AdNetworks.HideAd();
        }
    }

    public void HandleButtonDisplay() {

        // handle buttons

        if(buttonDisplayState ==
            UIPanelButtonsDisplayState.CharacterCustomize) {

            GameUIPanelFooter.ShowButtonsCharacterCustomize();
        }
        else if(buttonDisplayState ==
            UIPanelButtonsDisplayState.Character) {

            GameUIPanelFooter.ShowButtonsCharacter();
        }
        else if(buttonDisplayState ==
            UIPanelButtonsDisplayState.CharacterLarge) {

            GameUIPanelFooter.ShowButtonsCharacterLarge();
        }
        else if(buttonDisplayState ==
            UIPanelButtonsDisplayState.CharacterTools) {

            GameUIPanelFooter.ShowButtonsCharacterTools();
        }
        else if(buttonDisplayState == UIPanelButtonsDisplayState.Statistics) {

            GameUIPanelFooter.ShowButtonsStatistics();
        }
        else if(buttonDisplayState == UIPanelButtonsDisplayState.Achievements) {

            GameUIPanelFooter.ShowButtonsAchievements();
        }
        else if(buttonDisplayState == UIPanelButtonsDisplayState.GameNetworks) {

            GameUIPanelFooter.ShowButtonGameNetworks();
        }
        else if(buttonDisplayState == UIPanelButtonsDisplayState.ProductsSections) {

            GameUIPanelFooter.ShowButtonsProductsSections();
        }
    }

    public void HandleBackgroundDisplay() {

        // handle character display

        if(backgroundDisplayState ==
            UIPanelBackgroundDisplayState.PanelBacker) {
            GameUIPanelBackgrounds.ShowUI();
        }
        else if(backgroundDisplayState ==
            UIPanelBackgroundDisplayState.None) {
            GameUIPanelBackgrounds.HideUI();
        }
    }

    //

    public virtual void HandleShow() {
        buttonDisplayState = UIPanelButtonsDisplayState.None;
        characterDisplayState = UIPanelCharacterDisplayState.None;
        backgroundDisplayState = UIPanelBackgroundDisplayState.None;
        adDisplayState = UIPanelAdDisplayState.None;

        bool showAd = false;

        if(!GameUIController.IsUIPanel(GameUIPanel.panelMain)) {
            // show around every third screen

            if(UnityEngine.Random.Range(0, 3) == 0) {
                showAd = true;
            }
        }

        if(showAd) {
            adDisplayState = UIPanelAdDisplayState.BannerBottom;
        }
    }

    public virtual void HandleHide() {
        GameCommunity.HideActionAppRate();
        GameCommunity.HideSharesCenter();

        AdNetworks.HideAd();
    }
}