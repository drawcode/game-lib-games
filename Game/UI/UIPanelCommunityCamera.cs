#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityCamera : UIPanelBase {

    public static UIPanelCommunityCamera Instance;
    public GameObject panelCameraButton;
    public GameObject panelCameraBackground;
    public GameObject panelCameraPhoto;
    public GameObject photoObject;
    public Material photoMaterial;
    public UIImageButton buttonPhotoLibrarySave;
    public UIImageButton buttonPhotoFacebook;
    public UIImageButton buttonPhotoTwitter;
    public UIImageButton buttonPhotoClose;
    
    public UIImageButton buttonPhotoTake;

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

        if(UIUtil.IsButtonClicked(buttonPhotoTake, buttonName)) {
            TakePhoto();
        }
        else if(UIUtil.IsButtonClicked(buttonPhotoClose, buttonName)) {
            ShowCameraButton();
            
            GameController.GameRunningStateRun();
        }
        else if(UIUtil.IsButtonClicked(buttonPhotoFacebook, buttonName)) {
            GameCommunitySocialController.UploadCurrentPhotoToFacebook();
        }
        else if(UIUtil.IsButtonClicked(buttonPhotoTwitter, buttonName)) {
            GameCommunitySocialController.UploadCurrentPhotoToTwitter();
        }
    }

    
    public static void TakePhoto() {
        if(isInst) {
            Instance.takePhoto();
        }
    }
    
    public void takePhoto() {   
        if(photoMaterial == null) {
            Debug.LogWarning("No photoMaterial found");
            return;
        }
        
        StartCoroutine(takePhotoCo());
    }
    
    IEnumerator takePhotoCo() { 

        ShowNone();

        GameUIPanelOverlays.Instance.ShowOverlayWhiteFlash();
        
        yield return new WaitForSeconds(.8f);
        
        PhotoObjectSize();
        
        GameCommunitySocialController.TakePhoto(photoMaterial);
        
        GameController.GameRunningStateContent();
        
        yield return new WaitForSeconds(.5f);
        
        yield return new WaitForSeconds(.5f);
        
        UINotificationDisplay.QueueInfo("Loading Photo", "Photo just taken is saving.");

        ShowCameraPhoto();
    }
    
    
    public void PhotoObjectSize() {
        // current size 250x250
        
        float currentWidth = Screen.width;
        float currentHeight = Screen.height;
        
        float photoWidth = 640f;
        float photoHeight = 420f;
        
        float currentRatioWidth = photoWidth/currentWidth;
        float currentRatioHeight = photoHeight/currentHeight;
        
        if(currentRatioHeight < currentRatioWidth) {
            currentWidth *= currentRatioHeight;
            currentHeight *= currentRatioHeight;
        }
        else if(currentRatioWidth < currentRatioHeight) {
            currentWidth *= currentRatioWidth;
            currentHeight *= currentRatioWidth;
        }
        
        photoObject.transform.localScale 
            = photoObject.transform.localScale
                .WithX(currentWidth).WithY(currentHeight);  
        
    }


    // SHOW/LOAD

    public void HidePanels() {
        HideCameraPhoto();
        HideCameraButton();
        HideCameraBackground();
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
    
    public static void ShowCameraBackground() {
        if (isInst) {
            Instance.showCameraBackground();
        }
    }
    
    public void showCameraBackground() { 
        AnimateInRight(panelCameraBackground);
    }
    
    public static void HideCameraBackground() {
        if (isInst) {
            Instance.hideCameraBackground();
        }
    }
    
    public void hideCameraBackground() {
        AnimateInRight(panelCameraBackground);
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
        HideCameraBackground();
    }

    public static void ShowDialog() {
        if (isInst) {
            Instance.showDialog();
        }
    }
    
    public void showDialog() {
        HideCameraButton();
        ShowCameraPhoto();
        ShowCameraBackground();
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

    }

    public void Update() {
        //base.Update();
    }
 
}
