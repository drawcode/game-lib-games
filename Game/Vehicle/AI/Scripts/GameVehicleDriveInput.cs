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

        vehicleDrive.vehicleDriveData.inputAxisHorizontal = Input.GetAxis("Horizontal");
        vehicleDrive.vehicleDriveData.inputAxisVertical = Input.GetAxis("Vertical");
        vehicleDrive.vehicleDriveData.inputBrake = Input.GetKey(KeyCode.B);
        vehicleDrive.vehicleDriveData.inputGas = true;
        vehicleDrive.vehicleDriveData.inputUse = Input.GetKey(KeyCode.E);

        Debug.Log("vehicleDrive.vehicleDriveData.inputAxisVertical:" + vehicleDrive.vehicleDriveData.inputAxisVertical);
        Debug.Log("vehicleDrive.vehicleDriveData.inputAxisHorizontal:" + vehicleDrive.vehicleDriveData.inputAxisHorizontal);
    }
}