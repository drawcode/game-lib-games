using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayerContainer : MonoBehaviour {

    public GameObject gameCharacterPlayerContainer;
    public string characterCode = ProfileConfigs.defaultGameCharacterCode;
    
    public void Awake() {
    }
    
    public void Start() {
        Init();
    }
    
    public void Init() {
        LoadPlayer(characterCode);
    }
    
    public void OnEnable() {
        Messenger<string>.AddListener(
            GameCustomMessages.customCharacterPlayerChanged, 
            OnCustomCharacterPlayerChangedHandler);
    }
    
    public void OnDisable() {

        Messenger<string>.RemoveListener(
            GameCustomMessages.customCharacterPlayerChanged, 
            OnCustomCharacterPlayerChangedHandler);
    }
    
    public void OnCustomCharacterPlayerChangedHandler(string characterCode) {
        LoadPlayer(characterCode);
    }
    
    public void LoadPlayer(string characterCodeTo) {
        
        if (string.IsNullOrEmpty(characterCode)) {
            return;
        }
        
        if (gameCharacterPlayerContainer == null) {
            return;
        }
        
        GameProfileCharacterItem gameProfileCharacterItem = 
            GameProfileCharacters.Current.GetCharacter(characterCode);
        
        if (gameProfileCharacterItem == null) {
            return;
        }
        
        GameCharacter gameCharacter = 
            GameCharacters.Instance.GetById(
                gameProfileCharacterItem.characterCode);
        
        if (gameCharacter == null) {
            return;
        }
        
        gameCharacterPlayerContainer.DestroyChildren();
        
        GameObject go = gameCharacter.Load();
        
        if (go == null) {
            return;
        }
        
        go.transform.parent = gameCharacterPlayerContainer.transform;
        go.transform.position = Vector3.zero;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero.WithY(133));
        
        GameController.CurrentGamePlayerController.LoadCharacter(gameCharacter.data.GetModel().code);
        
        GameCustomController.BroadcastCustomColorsSync();
        
        go.SetLayerRecursively("UIDialog");
    }
}