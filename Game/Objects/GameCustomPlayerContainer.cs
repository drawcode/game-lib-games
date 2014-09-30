using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Events;

public class GameCustomPlayerContainer : MonoBehaviour {
            
    public GameCustomCharacterData customCharacterData = new GameCustomCharacterData();
    public GameObject containerRotator;
    public GameObject containerPlayerDisplay;
    public bool allowRotator = false;
    public bool isProfileCharacterCode = false;
    Rigidbody rotatorRigidbody;
    RotateObject rotateObject;
    CapsuleCollider rotatorCollider;
    GameCustomPlayer customPlayerObject;
    
    //float lastUpdateScale = 0;
    //float factorUpdateScale = 0.0;
    
    public double currentContainerScale = 1.0;
    public double currentContainerStart = 1.0;
    public double currentContainerEnd = 2.0;
    public string uuid = System.Guid.NewGuid().ToString();

    public void Awake() {
    }
    
    public void Start() {
        //Init();
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

        string gameCharacterCode = customCharacterData.characterCode;

        if (isProfileCharacterCode) {
        
            GameProfileCharacterItem gameProfileCharacterItem = 
                GameProfileCharacters.Current.GetCharacter(
                    customCharacterData.characterCode);
            
            if (gameProfileCharacterItem == null) {
                return;
            }

            gameCharacterCode = gameProfileCharacterItem.characterCode;
        }
        
        GameCharacter gameCharacter = 
            GameCharacters.Instance.GetById(gameCharacterCode);
        
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

        customPlayerObject = go.GetOrSet<GameCustomPlayer>();

        if (customPlayerObject != null) {
            customPlayerObject.Change(customCharacterData);
        }
    }

    public void UpdateScale() {
        
        if (containerPlayerDisplay == null) {
            return;
        }
        
        if (containerPlayerDisplay.transform.childCount == 0) {
            return;
        }
        
        if (containerRotator == null) {
            return;
        }
        
        currentContainerScale = AnimationEasing.EaseGetValue(uuid, 1.0f);
        
        Debug.Log("UpdateScale:" + " currentContainerScale:" + currentContainerScale);

        float scaleTo = (float)currentContainerScale;

        containerPlayerDisplay.transform.localScale = 
            Vector3.zero
                .WithX(scaleTo)
                .WithY(scaleTo)
                .WithZ(scaleTo);

        //lastUpdateScale += Time.deltaTime;

        //if (Time.time - lastUpdateScale <= duration) {
        //    factorUpdateScale = (float)AnimationEasing.QuadEaseInOut(
        //        Time.time - lastUpdateScale, 0, 1, duration);
        //}
    }

    public void HandleContainerScale(double valStart, double valEnd) {
    
        
        if (containerPlayerDisplay == null) {
            return;
        }
        
        if (containerPlayerDisplay.transform.childCount == 0) {
            return;
        }
        
        if (containerRotator == null) {
            return;
        }

        Debug.Log("HandleContainerScale:" + " valStart:" + valStart + " valEnd:" + valEnd);

        AnimationEasing.EaseAdd(uuid, AnimationEasing.Equations.QuadEaseInOut, currentContainerScale, valStart, valEnd, .5, .1);
            
       // containerPlayerDisplay.transform.localScale = currentContainerScale;
    }
    
    public void UpdateRotator() {

        if (containerPlayerDisplay == null) {
            return;
        }

        if (containerPlayerDisplay.transform.childCount == 0) {
            return;
        }

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
        UpdateScale();
        
    }
}