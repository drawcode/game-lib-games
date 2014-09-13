#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityCamera : UIPanelCommunityBase {

    public static UIPanelCommunityCamera Instance;
    public GameObject panelCameraButton;
    public GameObject panelCameraPhoto;
    public GameObject photoObject;
    public Material photoMaterial;

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
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }

    public override void Init() {
        base.Init();

        //loadData();

        ShowCameraButton();
    }

    public override void Start() {
        Init();
    }

    // EVENTS
 
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
    }

    public void OnButtonClickEventHandler(string buttonName) {

    }
    
    public static void TakePhoto() {
        if (isInst) {
            Instance.takePhoto();
        }
    }
    
    public void takePhoto() {   
        if (photoMaterial == null) {
            Debug.LogWarning("No photoMaterial found");
            return;
        }
        
        StartCoroutine(takePhotoCo());
    }
    
    IEnumerator takePhotoCo() { 

        ShowNone();

        GameUIPanelOverlays.Instance.ShowOverlayWhiteFlash();
        
        yield return new WaitForSeconds(.7f);

        PhotoObjectSize();

        GameCommunitySocialController.TakePhoto(photoMaterial);
                
        UINotificationDisplay.QueueInfo("Loading Photo", "Photo just taken is saving.");

        GameController.GameRunningStateContent();

        yield return new WaitForSeconds(.5f);

        showDialog();
    }
    
    public void PhotoObjectSize() {
        // current size 250x250
        
        if(photoObject != null) {
            photoObject.ResizePreservingAspectToScreen(640f, 420f);
        }
        
    }

    // SHOW/LOAD

    public void HidePanels() {

        HideCameraPhoto();
        HideCameraButton();
    }

    //
        
    public static void ShowCameraButton() {
        if (isInst) {
            Instance.showCameraButton();
        }
    }

    public void showCameraButton() { 
        HidePanels();
        AnimateInRight(panelCameraButton);
    }
    
    public static void HideCameraButton() {
        if (isInst) {
            Instance.hideCameraButton();
        }
    }

    public void hideCameraButton() {
        AnimateInRight(panelCameraButton);
    }    
    
    //
    
    public static void ShowCameraPhoto() {
        if (isInst) {
            Instance.showCameraPhoto();
        }
    }

    public void showCameraPhoto() {
        HidePanels();
        AnimateInBottom(panelCameraPhoto);
                
        UIPanelCommunityBackground.ShowBackground();
    }
        
    public static void HideCameraPhoto() {
        if (isInst) {
            Instance.hideCameraPhoto();
        }
    }
    
    public void hideCameraPhoto() {
        AnimateOutBottom(panelCameraPhoto);
    }

    public static void ShowDefault() {
        if (isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if (isInst) {
            Instance.AnimateOut();
        }
    }

    public static void LoadData() {
        if (Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {

    }
    
    public static void ShowButton() {
        if (isInst) {
            Instance.showButton();
        }
    }

    public void showButton() {
        ShowCameraButton();
        HideCameraPhoto();
    }

    // DIALOG
    
    public static void ShowDialog() {
        if (isInst) {
            Instance.showDialog();
        }
    }
    
    public override void showDialog() {
        base.showDialog();

        ShowCameraPhoto();
    }
    
    public static void HideDialog() {
        if (isInst) {
            Instance.hideDialog();
        }
    }
    
    public override void hideDialog() {
        base.hideDialog();        
        
        ShowCameraButton();  
    }
        
    public static void ShowNone() {
        if (isInst) {
            Instance.showNone();
        }
    }
    
    public void showNone() {
        HideCameraButton();
        HideCameraPhoto();
    }

    public override void AnimateIn() {
        base.AnimateIn();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        hideDialog();
    }

    public void Update() {
        //base.Update();
    } 
}
