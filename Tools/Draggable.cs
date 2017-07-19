
using UnityEngine;
using System.Collections;

using Engine.Events;

public class Draggable : GameObjectBehavior {

    public static Draggable Instance;

    public Transform grabbed;
    public float grabDistance = 10.0f;
    public int grabLayerMask;
    public Vector3 grabOffset; //delta between transform transform position and hit point
    public bool useToggleDrag = true; // Didn't know which style you prefer. 


    public void Awake() {

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void Update() {
        if(ShouldUpdate()) {

            if(useToggleDrag) {
                UpdateToggleDrag();
            }
            else {
                UpdateHoldDrag();
            }
        }
    }

    GamePlayerThirdPersonController controller = null;

    public bool ShouldUpdate() {
        if(InputSystem.Instance.lastNormalizedTouch.y == 0f
            && InputSystem.Instance.lastNormalizedTouch.x == 0f) {

            if(controller == null) {
                controller = FindObjectOfType(typeof(GamePlayerThirdPersonController)) as GamePlayerThirdPersonController;
                if(controller != null) {
                    if(controller.verticalInput == 0f
                        && controller.horizontalInput == 0f
                        && controller.moveSpeed == 0f) {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }

            return true;
        }
        grabbed = null;
        return false;
    }

    // Toggles drag with mouse click
    public void UpdateToggleDrag() {
        if(Input.GetMouseButtonDown(0)) {
            Grab();
        }
        else {
            if(grabbed) {
                Drag();
            }
        }
    }

    // Drags when user holds down button
    public void UpdateHoldDrag() {
        if(Input.GetMouseButton(0)) {
            if(grabbed) {
                Drag();
            }
            else {
                Grab();
            }
        }
        else {
            if(grabbed) {
                //restore the original layermask
                grabbed.gameObject.layer = grabLayerMask;
            }
            grabbed = null;
        }
    }

    public void Grab() {
        if(grabbed) {
            grabbed = null;
        }
        else {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit)) {
                grabbed = hit.transform;
                if(grabbed.tag == "drag" || grabbed.parent.tag == "drag") {
                    if(grabbed.parent) {
                        grabbed = grabbed.parent.transform;
                    }
                    //set the object to ignore raycasts
                    grabLayerMask = grabbed.gameObject.layer;
                    grabbed.gameObject.layer = 2;
                    //now immediately do another raycast to calculate the offset
                    if(Physics.Raycast(ray, out hit)) {
                        grabOffset = grabbed.position - hit.point;
                    }
                    else {
                        //important - clear the gab if there is nothing
                        //behind the object to drag against
                        grabbed = null;
                    }
                }
                else {
                    grabbed = null;
                }
            }
        }
    }

    public void Drag() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit)) {
            if(grabbed != null) {
                float grabbedInitialY = grabbed.position.y;
                grabbed.position = hit.point + grabOffset;
                grabbed.position = grabbed.position.WithY(grabbedInitialY);
                ///LogUtil.Log("pos:" + grabbed.position);
            }
        }

    }
}