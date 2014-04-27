using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectMountVehicle : GameObjectMount {

    public GamePlayerController gamePlayerController;
    public GameVehicleDrive driver;

    public List<GameObjectMountWeaponHolder> gameObjectMountWeaponHolders;
    public List<GameObjectMountWeaponRotator> gameObjectMountWeaponRotators;
    
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
        
        SetMountWeaponRotator(Vector3.zero);
    } 
    
    public void MountVehicle() {
            
        driver = gameObject.Get<GameVehicleDrive>();

        if(driver == null) {
            driver = gameObject.FindTypeAboveRecursive<GameVehicleDrive>();
        }
                
        Debug.Log("MountVehicle:" + " driver:" + driver.name);

        FindWeaponHolders();

        FindWeaponRotators();
    }

    public void FindWeaponHolders() {    
                
        Debug.Log("FindWeaponHolders:" + " gameObjectMountWeaponHolders:" + gameObjectMountWeaponHolders);

        if(gameObjectMountWeaponHolders == null) {
            gameObjectMountWeaponHolders = new List<GameObjectMountWeaponHolder>();
        }
         
        if(gameObjectMountWeaponHolders.Count == 0) {
            Debug.Log("FindWeaponHolders:" + " gameObjectMountWeaponHolders:" + gameObjectMountWeaponHolders);

            foreach(GameObjectMountWeaponHolder item in 
                    driver.gameObject.GetComponentsInChildren<GameObjectMountWeaponHolder>(true)) {
                
                Debug.Log("FindWeaponHolders:" + " item:" + item.name);

                gameObjectMountWeaponHolders.Add(item);
            }
        }
    }
    
    public void FindWeaponRotators() {        
        
        Debug.Log("FindWeaponRotators:" + " gameObjectMountWeaponRotators:" + gameObjectMountWeaponRotators);
        
        if(gameObjectMountWeaponRotators == null) {
            gameObjectMountWeaponRotators = new List<GameObjectMountWeaponRotator>();
        }
        
        if(gameObjectMountWeaponRotators.Count == 0) {
            
            Debug.Log("FindWeaponRotators:" + " gameObjectMountWeaponRotators:" + gameObjectMountWeaponRotators);

            foreach(GameObjectMountWeaponRotator item in 
                    driver.gameObject.GetComponentsInChildren<GameObjectMountWeaponRotator>(true)) {
                
                Debug.Log("FindWeaponRotators:" + " item:" + item.name);

                gameObjectMountWeaponRotators.Add(item);
            }
        }
    }

    public List<GameObjectMountWeaponHolder> GetWeaponHolders() {
        FindWeaponHolders();
        return gameObjectMountWeaponHolders;
    }

    public GameObjectMountWeaponHolder GetWeaponHolder() {
        FindWeaponHolders();
        foreach(GameObjectMountWeaponHolder holder in gameObjectMountWeaponHolders) {
            return holder;
        }
        return null;
    }
    
    public List<GameObjectMountWeaponRotator> GetWeaponRotators() {
        FindWeaponRotators();
        return gameObjectMountWeaponRotators;
    }
    
    public GameObjectMountWeaponRotator GetWeaponRotator() {
        FindWeaponRotators();
        foreach(GameObjectMountWeaponRotator rotator in gameObjectMountWeaponRotators) {
            return rotator;
        }
        return null;
    }
    
    public void SetMountVehicleAxis(float h, float v) {
        if(driver != null) {
            driver.vehicleDriveData.inputAxisHorizontal = h;
            driver.vehicleDriveData.inputAxisVertical = v;
        }
    }

    public void SetMountWeaponRotator(Quaternion qt) {
        GameObjectMountWeaponRotator rotator = GetWeaponRotator();
        
        Debug.Log("SetMountWeaponRotator:" + " qt:" + qt);
        Debug.Log("SetMountWeaponRotator:" + " rotator:" + rotator);

        if(rotator != null) {
            Debug.Log("SetMountWeaponRotator:" + " rotatorSET:" + rotator);

            Vector3 rt = qt.eulerAngles;
            Vector3 to = rotator.transform.rotation.eulerAngles;

            to.y = rt.y;

            rotator.transform.rotation = Quaternion.Euler(to);
        }
    }

    public void SetMountWeaponRotator(Vector3 rt) {
        Debug.Log("SetMountWeaponRotator:" + " rt:" + rt);
        SetMountWeaponRotator(Quaternion.Euler(rt));
    }

    
    public void SetMountWeaponRotatorLocal(Quaternion qt) {
        GameObjectMountWeaponRotator rotator = GetWeaponRotator();
        
        Debug.Log("SetMountWeaponRotator:" + " qt:" + qt);
        Debug.Log("SetMountWeaponRotator:" + " rotator:" + rotator);
        
        if(rotator != null) {
            Debug.Log("SetMountWeaponRotator:" + " rotatorSET:" + rotator);
            
            Vector3 rt = qt.eulerAngles;
            Vector3 to = rotator.transform.localRotation.eulerAngles;
            
            to.y = rt.y;
            
            rotator.transform.localRotation = Quaternion.Euler(to);
        }
    }
    
    public void SetMountWeaponRotatorLocal(Vector3 rt) {
        Debug.Log("SetMountWeaponRotator:" + " rt:" + rt);
        SetMountWeaponRotator(Quaternion.Euler(rt));
    }


}