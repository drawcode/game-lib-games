using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIGamePlayerCustomContainer : MonoBehaviour {
    
    public GameObject containerPlayerDisplay;
    public UnityEngine.Object prefabPlayerDisplay;
    
    public void Awake() {
    }
    
    public void Start() {
        Init();
    }
    
    public void Init() {

    }
    
    void OnEnable() {
        //Messenger.AddListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }
    
    void OnDisable() {
        //Messenger.RemoveListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }

    void LoadPlayerDisplay() {
        if(prefabPlayerDisplay != null) {
            
        }
    }
}
