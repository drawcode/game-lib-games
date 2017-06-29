using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

#if ENABLE_FEATURE_CHARACTER_CUSTOMIZE

public class BaseGameUIPanelCustomizeWorlds : GameUIPanelBase {

    public static GameUIPanelCustomizeWorlds Instance;
    public Camera cameraCustomize;
    public int currentSelectedItem = 0;
    public GameObject playerObject;
    public GameObject playerContainerObject;
    public UICustomizeProfileCharacters customProfileCharacters;

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

        UpdateControls();
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

        Messenger<string, int>.AddListener(InputEvents.EVENT_ITEM_CLICK, OnInputClicked);
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

        Messenger<string, int>.RemoveListener(InputEvents.EVENT_ITEM_CLICK, OnInputClicked);
    }

    void OnInputClicked(string controlName, int data) {

        Debug.Log("OnInputClicked:" + " controlName:" + controlName + " data:" + data);

        if(customProfileCharacters == null) {
            customProfileCharacters = GetComponentInChildren<UICustomizeProfileCharacters>();
        }

        if(customProfileCharacters == null) {
            return;
        }

        if(customProfileCharacters.inputCurrentDisplayName != null
            && controlName == customProfileCharacters.inputCurrentDisplayName.name) {

            GameUIPanelHeader.CharacterLargeZoomIn();
            GameUIPanelHeader.CharacterLargeShowBack();
        }
        else if(customProfileCharacters.inputCurrentDisplayCode != null
            && controlName == customProfileCharacters.inputCurrentDisplayCode.name) {

            GameUIPanelHeader.CharacterLargeZoomIn();
            GameUIPanelHeader.CharacterLargeShowFront();
        }
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
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
    }

    public virtual void OnCheckboxChangedEventHandler(string buttonName, bool selected) {

    }

    public virtual void UpdateControls() {

    }

    public static void LoadData() {
        if(GameUIPanelCustomizeCharacter.Instance != null) {
            GameUIPanelCustomizeCharacter.Instance.loadData();
        }
    }

    public virtual void loadData() {
        StartCoroutine(loadDataCo());
    }

    IEnumerator loadDataCo() {

        LogUtil.Log("LoadDataCo");

        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();

            yield return new WaitForEndOfFrame();

            //loadDataPowerups();

            yield return new WaitForEndOfFrame();
            listGridRoot.GetComponent<UIGrid>().Reposition();
            yield return new WaitForEndOfFrame();
        }
    }

    public virtual void ClearList() {
        if(listGridRoot != null) {
            listGridRoot.DestroyChildren();
        }
    }

    public override void HandleShow() {
        base.HandleShow();

        buttonDisplayState = UIPanelButtonsDisplayState.None;
        characterDisplayState = UIPanelCharacterDisplayState.CharacterLarge;
        backgroundDisplayState = UIPanelBackgroundDisplayState.PanelBacker;
    }

    public override void HandleHide() {
        base.HandleHide();
    }

    public override void AnimateIn() {

        base.AnimateIn();

        loadData();
    }

    public override void AnimateOut() {

        base.AnimateOut();
        ClearList();
    }

    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }

        if(cameraCustomize == null) {
            return;
        }

        /*
        if(Input.GetMouseButtonDown(0)) {
            Ray screenRay = cameraCustomize.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
                
                LogUtil.Log("hit:" + hit.transform.name);
                
                if(hit.transform.gameObject == colorWheelPanel) {
                                                        
                    Texture2D tex = (Texture2D)hit.collider.gameObject.renderer.material.mainTexture;
                    Color color = tex.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y); // GetPixelBilinear oh how I love thee.
                                                    
                    GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
                                        
                    //LoadSelectedItem(0, false);
                    //SetColorProperties(color);
                    //SetMaterialColors();
                    
                    LogUtil.Log("hit.point:" + hit.point);
                    LogUtil.Log("hit.textureCoord:" + hit.textureCoord);
                    LogUtil.Log("hit.textureCoord2:" + hit.textureCoord2);
                }
            }
        }
        */
    }

    public virtual void LateUpdate() {

        //if (playerContainerObject) {
        //playerContainerObject.transform.Rotate(0f, -50 * Time.deltaTime, 0f);
        //}
    }
}
#endif