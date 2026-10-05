using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class GameVehicleDriveData {
    
    public float inputAxisVertical = 0;
    public float inputAxisHorizontal = 0;
    public bool inputUse = false;
    public bool inputBrake = false;
    public bool inputGas = true;
}
*/

public class GameVehicleDriveInput : GameObjectBehavior {

    public GameVehicleDrive vehicleDrive;

    void Awake() {
        vehicleDrive = gameObject.GetComponentInChildren<GameVehicleDrive>();
    }

    void Start() {

    }

    void Update() {

        // Awake's GetComponentInChildren can come back empty (the drive lives on a child that a
        // given vehicle prefab may not have). Without this the two lines below threw an NRE every
        // frame for the life of the object.
        if (vehicleDrive == null || vehicleDrive.vehicleDriveData == null) {
            return;
        }

        vehicleDrive.vehicleDriveData.inputAxisHorizontal = Input.GetAxis("Horizontal");
        vehicleDrive.vehicleDriveData.inputAxisVertical = Input.GetAxis("Vertical");
        vehicleDrive.vehicleDriveData.inputBrake = Input.GetKey(KeyCode.B);
        vehicleDrive.vehicleDriveData.inputGas = true;
        vehicleDrive.vehicleDriveData.inputUse = Input.GetKey(KeyCode.E);

        // Two UNCONDITIONAL Debug.Log calls ran here every frame. Debug.Log captures a managed
        // stack trace on every call -- the same cost measured at 34 ms of the profile save in
        // context-profile-save-cost -- so an active vehicle paid it twice a frame in a shipped
        // build. Behind the logging flag now, with the strings built only when wanted.
        if (LogUtil.loggingEnabled) {
            LogUtil.Log("vehicleDrive.vehicleDriveData.inputAxisVertical:"
                + vehicleDrive.vehicleDriveData.inputAxisVertical);
            LogUtil.Log("vehicleDrive.vehicleDriveData.inputAxisHorizontal:"
                + vehicleDrive.vehicleDriveData.inputAxisHorizontal);
        }
    }
}