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
    public GameObject panelCameraPhoto;
    public GameObject photoObject;
    public Material photoMaterial;
    public UIButton buttonPhotoLibrarySave;
    public UIButton buttonPhotoFacebook;
    public UIButton buttonPhotoTwitter;
    public UIButton buttonPhotoClose;

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
        //if(UIUtil.IsButtonClicked(buttonBuyRecharge, buttonName)) {
        //GameStoreController.Purchase("rpg-recharge-full-1", 1);
        //}
        //else if(UIUtil.IsButtonClicked(buttonEarn, buttonName)) {
        //GameController.QuitGame();
        //GameUIController.ShowGameMode();
        //}
        //else if(UIUtil.IsButtonClicked(buttonResume, buttonName)) {
        //HideAll();
        //GameController.ResumeGame();
        // }
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

        //GameController.PauseGame();
        
        GameCommunitySocialController.Instance.photoMaterial = photoMaterial;

        GameUIPanelOverlays.Instance.ShowOverlayWhiteFlash();
        
        yield return new WaitForSeconds(.8f);
        
        PhotoObjectSize();
        
        GameCommunitySocialController.TakePhoto();
        
        yield return new WaitForSeconds(.5f);
        
        yield return new WaitForSeconds(.5f);
        
        UINotificationDisplay.Instance.QueueInfo("Loading Photo", "Photo just taken is saving.");
        
        //ShowDefault();

        // Reshow UI Elements
        //AppViewerUIPanelHUD.ShowDefault();
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
