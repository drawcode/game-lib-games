using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIGamePlayerDisplay : MonoBehaviour {
    
    public GameObject containerPlayer;
    
    public string characterCode = ProfileConfigs.defaultGameCharacterCode;
    
    public void Awake() {
    }
    
    public void Start() {
        Init();
    }
    
    public void Init() {
        LoadPlayer(characterCode);
    }
    
    void OnEnable() {
        //Messenger.AddListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }
    
    void OnDisable() {
        //Messenger.RemoveListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }
    
    public void LoadPlayer(string characterCodeTo) {
        
        characterCode = characterCodeTo;
        
        if(containerPlayer != null) {
            
            GameObject go = GameCharacters.Load(characterCode);

            if(go != null) {
                containerPlayer.DestroyChildren();

                go.transform.parent = containerPlayer.transform;
                go.ResetObject();
            }
        }
    }
}