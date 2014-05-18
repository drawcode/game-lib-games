using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Engine.Animation;
using Engine.Events;
using Engine.UI;
using Engine.Utility;

public class GameUI : GameObjectBehavior {
    
    public static GameUI Instance;
    
    void Awake() {
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
        
    public UIButton buttonUIToggle;
    
    /*
    public UIButton buttonUICharacter;
    public UIButton buttonUIMap;
    public UIButton buttonUIInventory;
    public UIButton buttonUIStore;
    public UIButton buttonUIOptions;
    public UIButton buttonUIInfo;
    public UIButton buttonUIDev;
    */
    
    public UIButtonMeta buttonMeta;
    public GameObject containerUIScaled;
    public GameObject containerUI;
    public bool gameUIExpanded = true;
    bool gameLoopsStarted = false;
    
    void Start() {
        Init();
    }
    
    void Init() {
        buttonMeta = new UIButtonMeta();
        InitEvents();
        ShowUI();
    }
    
    void InitEvents() {
        LogUtil.Log("InitEvents:");
        
        buttonMeta.SetButton("buttonUIToggle", ref buttonUIToggle, delegate () {
            ToggleGameUI();
            //Application.LoadLevel("GameSceneDynamic");
            LogUtil.Log("buttonUIToggle:");
        });
        
        /*
        buttonMeta.SetButton("buttonUICharacter", ref buttonUICharacter, delegate () {
            LogUtil.Log("buttonUICharacter:");
        });
        
        buttonMeta.SetButton("buttonUIMap", ref buttonUIMap, delegate () {
            LogUtil.Log("buttonUIMap:");
        });
        
        buttonMeta.SetButton("buttonUIInventory", ref buttonUIInventory, delegate () {
            LogUtil.Log("buttonUIInventory:");
        });
        
        buttonMeta.SetButton("buttonUIStore", ref buttonUIStore, delegate () {
            LogUtil.Log("buttonUIStore:");
        });
        
        buttonMeta.SetButton("buttonUIOptions", ref buttonUIOptions, delegate () {
            LogUtil.Log("buttonUIOptions:");
        });
        
        buttonMeta.SetButton("buttonUIInfo", ref buttonUIInfo, delegate () {
            LogUtil.Log("buttonUIInfo:");
        });
        
        buttonMeta.SetButton("buttonUIDev", ref buttonUIDev, delegate () {
            LogUtil.Log("buttonUIDev:");
        });
        */
        
        //buttonUICharacter.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
        
    }
    
    public void PlayGameAudio() {
        
        if (!gameLoopsStarted) {            
            GameAudio.StartGameLoops();
        }
        
        //GameAudio.StopAmbience();
        //GameAudio.StartGameLoop(GameUIPanel.Instance.currentLevelNumber);
    }
    
    public void PlayUIAudio() {
        
        //GameAudio.StartGameLoop(-1);
        //GameAudio.StartAmbience();
    }
    
    public void ShowUI() {
        Vector3 temp = containerUI.transform.position;
        temp.x = 0f;
        //Tweens.Instance.MoveToObject(containerUI, temp, 0f, 0f);
        //Tweens.Instance.MoveToObject(containerUIScaled, temp, 0f, 0f);
        
        LogUtil.Log("containerUI2:" + containerUI);
        //PlayUIAudio();
        gameUIExpanded = true;
    }
    
    public void HideUI() {
        Vector3 temp = containerUI.transform.position;
        temp.x = 150f;
        //Tweens.Instance.MoveToObject(containerUI, temp, 0f, 0f);
        //Tweens.Instance.MoveToObject(containerUIScaled, temp, 0f, 0f);
        
        //HandleInGameAudio();
        LogUtil.Log("containerUI:" + containerUI);
        gameUIExpanded = false;
    }

    public void ToggleGameUI() {
                    
        gameUIExpanded = gameUIExpanded ? false : true;
            
        LogUtil.Log("toggling:" + gameUIExpanded);
        
        if (gameUIExpanded) {
            HideUI();
        }
        else {
            ShowUI();
        }
    }
    
    void LateUpdate() {
        if (buttonMeta != null) {
            buttonMeta.ResetButtons();
        }
    }
    
}

