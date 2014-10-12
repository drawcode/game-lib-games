#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelCommunityShare : UIPanelCommunityBase {

    public static UIPanelCommunityShare Instance;

    public GameObject containerShares;    
    public GameObject containerActionTools;
    public GameObject containerActionAppRate;

    public override void Awake() {

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

        HideAllItems();
                
        Invoke("InitPlatform", 1);
    }

    public override void Start() {
        Init();
    }

    // EVENTS
 
    public override void OnEnable() {

        base.OnEnable();

    }
    
    public override void OnDisable() {

        base.OnDisable();
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        base.OnButtonClickEventHandler(buttonName);
    }

    //

    void InitPlatform() {
        ShowActionTools();
    }
    
    public void HideAllItems() {
        
        HideAllShares();
        HideActionTools();
        HideActionAppRate();
    }

    //

    public virtual void ShowShare(string code) {
        
        foreach (GameObjectShowItem item in 
                containerShares.GetComponentsInChildren<GameObjectShowItem>(true)) {
            
            if (item.code == code) {
                HideAllShares();
                ShowPanelBottom(item.gameObject);
                item.gameObject.ShowObjectDelayed(.7f);
            }
        }
    }
    
    public virtual void HideAllShares() {        
        
        foreach (GameObjectShowItem item in 
                containerShares.GetComponentsInChildren<GameObjectShowItem>(true)) {
            HidePanelBottom(item.gameObject);
            item.gameObject.HideObjectDelayed(.5f);
        }
    }
    
    //
    
    public static void ShowSharesCenter() {
        if (isInst) {
            Instance.showSharesCenter();
        }
    }
    
    public virtual void showSharesCenter() {   
        
        if (Context.Current.isWeb) {
            return;
        }
        
        ShowShare(GameCommunityUIShares.shareCenter);
    }
    
    public static void HideSharesCenter() {
        if (isInst) {
            Instance.hideSharesCenter();
        }
    }
    
    public virtual void hideSharesCenter() {   
        HideAllShares();
    }

    //
    
    public static void ShowActionTools() {
        if (isInst) {
            Instance.showActionTools();
        }
    }
    
    public virtual void showActionTools() {   
        
        if (Context.Current.isWeb) {
            return;
        }

        ShowPanelBottom(containerActionTools);
    }
    
    public static void HideActionTools() {
        if (isInst) {
            Instance.hideActionTools();
        }
    }
    
    public virtual void hideActionTools() {   

        HidePanelBottom(containerActionTools);
    }

    //
    
    public static void ShowActionAppRate() {
        if (isInst) {
            Instance.showActionAppRate();
        }
    }
    
    public virtual void showActionAppRate() {   
        
        if (Context.Current.isWeb) {
            return;
        }
        
        ShowPanelRight(containerActionAppRate);
    }
    
    public static void HideActionAppRate() {
        if (isInst) {
            Instance.hideActionAppRate();
        }
    }
    
    public virtual void hideActionAppRate() {   
        
        HidePanelRight(containerActionAppRate);
    }

    //

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

    public override void Update() {
        base.Update();
    }
 
}
