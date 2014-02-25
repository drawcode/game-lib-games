using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectMountVehicle : GameObjectMount {

    public GamePlayerController gamePlayerController;
    public GameVehicleDrive driver;
    
    public override void Awake() { 
        base.Awake();   
    }
    
    public override void Start() {        
        base.Start();  
    }
    
    public override void Init() {
        base.Init();
        interactiveType = GameObjectInteractiveType.mount;
    }
    
    public override void FixedUpdate() {
        base.FixedUpdate();
    }
    
    public override void AttractForce<T>() {
        base.AttractForce<T>();
    }    
    
    public override void Boost(GameObject go) {
        base.Boost(go);
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
    
    public override void Update() {
        base.Update();

        if(isMounted) {
            if(driver != null) {
                objectMounted.TrackObject(driver.gameObject);
            }
        }
    }

    public override void Mount(GameObject go) {
        base.Mount(go);

        if(isMounted) {
            gamePlayerController = objectMounted.Get<GamePlayerController>();
        }

        MountVehicle();
    } 

    public override void Unmount() {
        base.Unmount();

        SetMountVehicleAxis(0f, 0f);
    } 
    
    public void MountVehicle() {
            
        driver = gameObject.Get<GameVehicleDrive>();

        if(driver == null) {
            driver = gameObject.FindTypeAboveRecursive<GameVehicleDrive>();
        }
    }
    
    public void SetMountVehicleAxis(float h, float v) {
        if(driver != null) {
            driver.vehicleDriveData.inputAxisHorizontal = h;
            driver.vehicleDriveData.inputAxisVertical = v;
        }
    }
}