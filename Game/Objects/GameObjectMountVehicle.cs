using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectMountVehicle : GameObjectMount {

    public GamePlayerController gamePlayerController;
    public GameVehicleDrive driver;
    public List<GameObjectMountWeaponHolder> gameObjectMountWeaponHolders;
    public List<GameObjectMountWeaponRotator> gameObjectMountWeaponRotators;

    // Separate rider and vehicle (Action Bikes, Blaster Master style enter/exit). All optional: a vehicle
    // without them (Action Bots' vehicle-car-1) behaves exactly as before.
    public Renderer riderSeat;   // the vehicle's own rider mesh: shown in the rider's skin while mounted, hidden when empty
    public Transform exitPoint;  // where the rider is put on exit (unset: the rider stays where it is)
    public Transform seatPoint;  // rider socket for open vehicles without a seat mesh

    public override void Awake() {
        base.Awake();
    }

    public override void Start() {
        base.Start();

        if (riderSeat != null && !isMounted) {
            riderSeat.enabled = false;
        }
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

        if (isMounted) {
            if (driver != null) {
                objectMounted.TrackObject(driver.gameObject);
            }
            else if (riderSeat != null || exitPoint != null) {
                // no drive component (a rider vehicle before its arcade host): ride along on the mount
                objectMounted.TrackObject(gameObject);
            }
        }
    }

    public override void Mount(GameObject go) {
        base.Mount(go);

        if (isMounted) {
            gamePlayerController = objectMounted.Get<GamePlayerController>();
        }

        MountVehicle();

        if (isMounted) {
            OnRiderMounted(objectMounted);
        }
    }

    public override void Unmount() {
        if (isMounted) {
            OnRiderUnmounted(objectMounted);
        }

        base.Unmount();

        SetMountVehicleAxis(0f, 0f);

        SetMountWeaponRotator(Vector3.zero);
    }

    public void MountVehicle() {

        driver = gameObject.Get<GameVehicleDrive>();

        if (driver == null) {
            driver = gameObject.FindTypeAboveRecursive<GameVehicleDrive>();
        }

        objectMounted.transform.position = gameObject.transform.position;
        objectMounted.transform.rotation = gameObject.transform.rotation;

        LogUtil.Log("MountVehicle:" + " vehicle:" + vehicleObject.name);

        FindWeaponHolders();

        FindWeaponRotators();
    }

    // The vehicle this mount belongs to: the drive component's object, or (a vehicle without one) the nearest
    // Rigidbody above the mount, else the mount itself.
    public GameObject vehicleObject {
        get {
            if (driver != null) {
                return driver.gameObject;
            }

            Rigidbody body = gameObject.FindTypeAboveRecursive<Rigidbody>();

            return body != null ? body.gameObject : gameObject;
        }
    }

    // Show the seat rider in the mounted rider's skin. The skin is the rider's skinned mesh with the seat's
    // name, else its first skinned mesh.
    public virtual void OnRiderMounted(GameObject rider) {
        if (riderSeat == null || rider == null) {
            return;
        }

        SkinnedMeshRenderer skin = null;

        foreach (SkinnedMeshRenderer item in rider.GetComponentsInChildren<SkinnedMeshRenderer>(true)) {
            if (skin == null || item.name == riderSeat.name) {
                skin = item;
            }
            if (item.name == riderSeat.name) {
                break;
            }
        }

        if (skin != null) {
            riderSeat.sharedMaterials = skin.sharedMaterials;
        }

        riderSeat.enabled = true;
    }

    // Hide the seat rider and put the rider down at the exit point; the vehicle stays parked.
    public virtual void OnRiderUnmounted(GameObject rider) {
        if (riderSeat != null) {
            riderSeat.enabled = false;
        }

        if (exitPoint != null && rider != null) {
            // an enabled nav agent owns the rider's position and would pull it back: warp it
            UnityEngine.AI.NavMeshAgent agent = rider.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (agent != null && agent.enabled && agent.isOnNavMesh) {
                agent.Warp(exitPoint.position);
            }
            else {
                rider.transform.position = exitPoint.position;
            }
        }
    }

    public void FindWeaponHolders() {

        LogUtil.Log("FindWeaponHolders:" + " gameObjectMountWeaponHolders:" + gameObjectMountWeaponHolders);

        if (gameObjectMountWeaponHolders == null) {
            gameObjectMountWeaponHolders = new List<GameObjectMountWeaponHolder>();
        }

        if (gameObjectMountWeaponHolders.Count == 0) {
            LogUtil.Log("FindWeaponHolders:" + " gameObjectMountWeaponHolders:" + gameObjectMountWeaponHolders);

            foreach (GameObjectMountWeaponHolder item in
                    vehicleObject.GetComponentsInChildren<GameObjectMountWeaponHolder>(true)) {

                LogUtil.Log("FindWeaponHolders:" + " item:" + item.name);

                gameObjectMountWeaponHolders.Add(item);
            }
        }
    }

    public void FindWeaponRotators() {

        LogUtil.Log("FindWeaponRotators:" + " gameObjectMountWeaponRotators:" + gameObjectMountWeaponRotators);

        if (gameObjectMountWeaponRotators == null) {
            gameObjectMountWeaponRotators = new List<GameObjectMountWeaponRotator>();
        }

        if (gameObjectMountWeaponRotators.Count == 0) {

            LogUtil.Log("FindWeaponRotators:" + " gameObjectMountWeaponRotators:" + gameObjectMountWeaponRotators);

            foreach (GameObjectMountWeaponRotator item in
                    vehicleObject.GetComponentsInChildren<GameObjectMountWeaponRotator>(true)) {

                LogUtil.Log("FindWeaponRotators:" + " item:" + item.name);

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
        foreach (GameObjectMountWeaponHolder holder in gameObjectMountWeaponHolders) {
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
        foreach (GameObjectMountWeaponRotator rotator in gameObjectMountWeaponRotators) {
            return rotator;
        }
        return null;
    }

    public void SetMountVehicleAxis(float h, float v) {
        if (driver != null) {
            driver.vehicleDriveData.inputAxisHorizontal = h;
            driver.vehicleDriveData.inputAxisVertical = v;
        }
    }

    public void SetMountWeaponRotator(Quaternion qt) {
        GameObjectMountWeaponRotator rotator = GetWeaponRotator();

        LogUtil.Log("SetMountWeaponRotator:" + " qt:" + qt);
        LogUtil.Log("SetMountWeaponRotator:" + " rotator:" + rotator);

        if (rotator != null) {
            LogUtil.Log("SetMountWeaponRotator:" + " rotatorSET:" + rotator);

            Vector3 rt = qt.eulerAngles;
            Vector3 to = rotator.transform.rotation.eulerAngles;

            to.y = rt.y;

            rotator.transform.rotation = Quaternion.Euler(to);
        }
    }

    public void SetMountWeaponRotator(Vector3 rt) {
        LogUtil.Log("SetMountWeaponRotator:" + " rt:" + rt);
        SetMountWeaponRotator(Quaternion.Euler(rt));
    }

    public void SetMountWeaponRotatorLocal(Quaternion qt) {
        GameObjectMountWeaponRotator rotator = GetWeaponRotator();

        LogUtil.Log("SetMountWeaponRotator:" + " qt:" + qt);
        LogUtil.Log("SetMountWeaponRotator:" + " rotator:" + rotator);

        if (rotator != null) {
            LogUtil.Log("SetMountWeaponRotator:" + " rotatorSET:" + rotator);

            Vector3 rt = qt.eulerAngles;
            Vector3 to = rotator.transform.localRotation.eulerAngles;

            to.y = rt.y;

            rotator.transform.localRotation = Quaternion.Euler(to);
        }
    }

    public void SetMountWeaponRotatorLocal(Vector3 rt) {
        LogUtil.Log("SetMountWeaponRotator:" + " rt:" + rt);
        SetMountWeaponRotator(Quaternion.Euler(rt));
    }
}