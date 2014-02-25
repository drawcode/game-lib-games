using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameObjectMountType {
    typeAuto,
    typeTriggered
}

public class GameObjectMount : BaseGameObjectInteractive {

    public GameObject objectMounted;
    public int allowedMountCount = 1;
        
    public override void Awake() { 
        base.Awake();   
    }
    
    public override void Start() {        
        base.Start();  
    }
    
    public override void Init() {
        base.Init();
        interactiveType = GameObjectInteractiveType.mount;
        attractGamePlayers = true;
        attractRange = 5f;
    }
    
    public override void FixedUpdate() {
        base.FixedUpdate();
    }
    
    public override void AttractForce<T>() {
        base.AttractForce<T>();
    }    

    public override void AddForce(GameObject target, float force) {
        base.AddForce(target, force);
    }
    
    public override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
    }
    
    public override void OnTriggerEnter(Collider collider) {
        base.OnTriggerEnter(collider);
    }
    
    public virtual bool isMounted {
        get {
            if(objectMounted == null) {
                return false;
            }
            else {
                return true;
            }
        }
    }
        
    public virtual void ToggleMount(GameObject go) {
        if(isMounted) {            
            Unmount();
        }
        else {
            Mount(go);        
        }
    }

    public virtual void Mount(GameObject go) {
        if(!isMounted) {
            objectMounted = go;

            Debug.Log("Mount:" + " current:" + transform.name + " mount:" + go.name);
        }
    }
        
    public virtual void Unmount() {
        if(isMounted) {
            Debug.Log("Unmount:" + " current:" + transform.name + " mount:" + objectMounted.name);

            objectMounted = null;
        }
    }
    
    public override void Update() {
        base.Update();
        
        if(Input.GetKeyDown(KeyCode.E)) {
            HandleMount();
        }
    }
    
    public virtual void HandleMount() {

        // check mounting from mount itself for game player mounts as it is 
        // more efficient
        
        // check if any game players are around
        // if action pressed, mount vehicle
        if (GameController
            .CurrentGamePlayerController != null
            && !GameController
            .CurrentGamePlayerController.controllerData.dying) {

            if(GameController
               .CurrentGamePlayerController.IsPlayerControlled) {    
            
                if (GameController
                    .CurrentGamePlayerController
                    .controllerData
                    .distanceToPlayerControlledGamePlayer <= attractRange) {

                    GameController
                        .CurrentGamePlayerController.Mount(gameObject);
                }
                else {
                    GameController
                        .CurrentGamePlayerController.Unmount();
                }

                //ToggleMount(gamePlayerControllerHit.gameObject);

            }
        }
        
    }
}