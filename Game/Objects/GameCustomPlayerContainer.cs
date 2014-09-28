using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayerContainer : MonoBehaviour {
            
    public GameCustomCharacterData customCharacterData = new GameCustomCharacterData();
    public GameObject containerRotator;
    public GameObject containerPlayerDisplay;
    public bool allowRotator = false;
    Rigidbody rotatorRigidbody;
    RotateObject rotateObject;
    CapsuleCollider rotatorCollider;

    public void Awake() {
    }
    
    public void Start() {
        Init();
    }
    
    public void Init() {

    }

    public void LoadPlayer() {        
        LoadPlayer(customCharacterData.characterCode);
    }

    public void LoadPlayer(GameCustomCharacterData customCharacterDataTo) {
        customCharacterData = customCharacterDataTo;

        LoadPlayer(customCharacterData.characterCode);
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
        
        if (string.IsNullOrEmpty(characterCodeTo)) {
            return;
        }

        customCharacterData.characterCode = characterCodeTo;
        
        if (containerPlayerDisplay == null) {
            return;
        }
        
        GameProfileCharacterItem gameProfileCharacterItem = 
            GameProfileCharacters.Current.GetCharacter(
                customCharacterData.characterCode);
        
        if (gameProfileCharacterItem == null) {
            return;
        }
        
        GameCharacter gameCharacter = 
            GameCharacters.Instance.GetById(
                gameProfileCharacterItem.characterCode);
        
        if (gameCharacter == null) {
            return;
        }
        
        containerPlayerDisplay.DestroyChildren();
        
        GameObject go = gameCharacter.Load();
        
        if (go == null) {
            return;
        }
        
        go.transform.parent = containerPlayerDisplay.transform;
        go.transform.position = Vector3.zero;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;//.Euler(Vector3.zero.WithY(133));
        
        GameController.CurrentGamePlayerController.LoadCharacter(gameCharacter.data.GetModel().code);
        
        GameCustomController.BroadcastCustomSync();
                
        go.SetLayerRecursively(gameObject.layer);

        // LOAD UP PASSED IN VALUES

        GameCustomPlayer customPlayerObject = go.GetOrSet<GameCustomPlayer>();

        if (customPlayerObject == null) {
            customPlayerObject.Change(customCharacterData);
        }
    }
    
    public void UpdateRotator() {
        
        if (containerRotator == null) {
            return;
        }
        
        if (rotatorRigidbody == null) {
            rotatorRigidbody = containerRotator.GetOrSet<Rigidbody>();
        }
        
        if (rotateObject == null) {
            rotateObject = containerRotator.GetOrSet<RotateObject>();
        }
        
        if (rotatorCollider == null) {
            rotatorCollider = containerRotator.GetOrSet<CapsuleCollider>();
        }
        
        if (allowRotator) {   
            
            if (rotatorRigidbody != null) {
                rotatorRigidbody.constraints = RigidbodyConstraints.FreezePositionX
                    | RigidbodyConstraints.FreezePositionY
                    | RigidbodyConstraints.FreezePositionZ
                    | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationZ;
            }
            
            if (rotateObject != null) {
                rotateObject.RotateSpeedAlongY = 2;
            }
        }
        else {
            
            containerRotator.ResetObject();
            
            if (rotatorRigidbody != null) {
                rotatorRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            
            if (rotateObject != null) {
                rotateObject.RotateSpeedAlongY = 0;
            }
        }
    }
    
    void Update() {
        
        UpdateRotator();
        
    }
}