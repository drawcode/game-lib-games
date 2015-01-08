using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using Engine.Animation;
using Engine.Events;

public class BaseGameLevelSprite : MonoBehaviour {
    
    BoxCollider boxCollider;
    public GameDraggableLevelItem gameDraggableLevelItem;
    public int hitsAllowed = 5;
    //public bool shakOnHit = true;
    public bool playEffectOnDestroy = true;
    //public bool enablePhysicsHits = false;
    public bool exploding = false;
    public bool impactCollisionsCount = true;
    public bool physicsActive = false;
    
    public virtual void OnEnable() {        
        //Messenger.AddListener(GameAppControllerMessages.StateEditOn, OnStateEditOnHandler);
        //Messenger.AddListener(GameAppControllerMessages.StateEditOff, OnStateEditOffHandler);
    }
    
    public virtual void OnDisable() {       
        //Messenger.RemoveListener(GameAppControllerMessages.StateEditOn, OnStateEditOnHandler);
        //Messenger.RemoveListener(GameAppControllerMessages.StateEditOff, OnStateEditOffHandler);
    }
    
    public virtual void OnStateEditOnHandler() {
        //ShowAllGameLevelItems();
    }
    
    public virtual void OnStateEditOffHandler() {
        //HideAllGameLevelItems();
    }
    
    public virtual void Start() {
        
        Init();
    }
    
    public virtual void Init() {
        HandleColliderInit();
        if (gameDraggableLevelItem.gameLevelItemAsset.physics_type != GameLevelItemAssetPhysicsType.physicsOnStart) {
            HandlePhysicsInit();
        }
    }
    
    public virtual bool isReady {
        get {
            if (gameDraggableLevelItem == null) {
                return false;
            }
            
            if (gameDraggableLevelItem.gameLevelItemAsset == null) {
                return false;
            }
            
            return true;
        }
    }
    
    public virtual void HandleColliderInit() {
        
        if (!isReady) {
            return;
        }
        
        if (collider == null) {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;//gameDraggableLevelItem.gameLevelItemAsset.destructable;
            boxCollider.size = boxCollider.size.WithX(4.2f).WithY(4.2f).WithZ(4.2f);            
            boxCollider.center = boxCollider.size.WithY(2.1f); 
            boxCollider.material = MaterialUtil.LoadPhysicMaterialFromResources("materials/ArcadeBounce");
        }
    }
    
    public virtual void HandlePhysicsInit() {
        
        if (!isReady) {
            return;
        }
        
        if (physicsActive) {
            return; 
        }
        
        if (gameDraggableLevelItem.gameLevelItemAsset.physics_type != GameLevelItemAssetPhysicsType.physicsStatic) {
            if (rigidbody == null) {
                gameObject.AddComponent<Rigidbody>();
                //rigidbody.freezeRotation = true;
                rigidbody.isKinematic = gameDraggableLevelItem.gameLevelItemAsset.kinematic;
                rigidbody.useGravity = gameDraggableLevelItem.gameLevelItemAsset.gravity;
                physicsActive = true;
            }
        }
        
    }
    
    public virtual void OnCollisionEnter(Collision collision) {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (ShouldCountHit(collider)) {
            Hit();
        }
    }
    
    public virtual void OnCollisionStay(Collision collision) {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (ShouldCountHit(collider)) {
            Hit();
        }
    }
    
    public virtual void OnCollisionExit(Collision collision) {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (ShouldCountHit(collider)) {
            Hit();
        }
    }
    
    public virtual void OnTriggerEnter(Collider collider) {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (ShouldCountHit(collider)) {
            Hit();
        }
    }
    
    public virtual void OnTriggerStay(Collider collider) {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (ShouldCountHit(collider)) {
            Hit();
        }
    }
    
    public virtual void OnTriggerExit(Collider collider) {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (ShouldCountHit(collider)) {
            Hit();
        }
    }
    
    public virtual bool ShouldCountHit(Collider collider) {
        if (collider != null) {
            string objName = collider.transform.name;
            if (!objName.Contains("DragColliderObject")
                && (objName.Contains("GamePlayerObject")
                || objName.Contains("GamePlayerEnemy"))) {
                GamePlayerController player = collider.transform.gameObject.GetComponent<GamePlayerController>();
                if (player != null) {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    public virtual void Hit() {
        
        if (!isReady) {
            return;
        }
        
        if (!exploding) {
            
            if (gameDraggableLevelItem.gameLevelItemAsset.reactive) {                   
                Shake();
            }
            
            if (GameDraggableEditor.appEditState != GameDraggableEditEnum.StateEditing
                && gameDraggableLevelItem.gameLevelItemAsset.destructable) {
                
                hitsAllowed--;
                
                if (hitsAllowed == 0) { 
                    Messenger<double>.Broadcast(GameMessages.score, 10);
                    exploding = true;
                    gameDraggableLevelItem.DestroyGameLevelItemSprite();
                    Invoke("DestroyMe", 2f);
                    
                    if (gameDraggableLevelItem.gameLevelItemAsset.physics_type 
                        == GameLevelItemAssetPhysicsType.physicsOnCollide) {
                        HandlePhysicsInit();
                    }
                }
            }
        }
    }
    
    public virtual void Shake() {       
        //iTween.ShakePosition(gameObject, iTween.Hash("x", .02f, "y", .02f, "time", 1f, "easetype", "easeOutCubic"));
    }
    
    public virtual void DestroyMe() {   
        //Destroy(gameObject);
        gameObject.Hide();
        
        if (isReady) {
            gameDraggableLevelItem.gameLevelItemAsset.destroyed = true;
        }
        
        Debug.Log("GameLevelSprite:destroying..." + name);
    }
    
    public virtual void Update() {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (gameDraggableLevelItem.gameLevelItemAsset.rotation_speed.GetVector3() != Vector3.zero) {
            transform.Rotate(gameDraggableLevelItem.gameLevelItemAsset.rotation_speed.GetVector3() * Time.deltaTime);
        }
    }
    
}
