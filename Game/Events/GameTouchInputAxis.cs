#pragma warning disable 0169
#pragma warning disable 0618
#pragma warning disable 0649
#pragma warning disable 0414
#pragma warning disable 0108
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine;
using Engine.Events;
using Engine.Utility;

public class GameTouchInputAxis : GameObjectBehavior {

    public static bool updateFingerNavigate = false;
    public GameObject objectPlacement;
    public Camera collisionCamera;
    public Transform pad;
    // = gameObject.transform.FindChild("Pad");
    public string axisName = InputSystemKeys.mainKey;
    public Vector3 axisInput;
    public Vector3 padPos;
    public bool controlsVisible = true;
    public bool controlsMoveable = false;
    public bool hideOnDesktopWeb = false;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UIAnchor anchor;
#endif

    Vector3 originalPlacement = Vector3.zero;
    GameObject hitObject;
    GameTouchInputAxis axisObject;
    GameTouchInputAxisPad axisPadObject;
    public float scaleModifier = 0.003125f;
    public Vector2 inputSize = Vector2.zero;
    public bool inUse = false;
    public Vector3 anchorPoint = Vector3.zero;
    public Vector3 stretchPoint = Vector3.zero;

    // How far the floating pad may travel from where it was authored, in the placement
    // object's parent-local units. <= 0 derives it from the placement collider's own half
    // extents -- the zone the pad is meant to live inside.
    //
    // Without a limit the pad walks: `objectPlacement.transform.position = worldPoint` had no
    // bound at all, so dragging the left stick could carry it across the screen and park its
    // collider on top of the right-hand ButtonInput* buttons, where it wins the raycast and
    // they stop responding. It stayed there for as long as ANY finger was down, because the
    // only restore was in the "nothing at all is pressed" branch.
    public float placementTravelLimit = 0f;

    void Awake() {

    }

    void Start() {

        if (objectPlacement != null) {
            originalPlacement = objectPlacement.transform.localPosition;
        }
    }

    void FindPad() {
        if (pad == null) {
            pad = gameObject.transform.Find("Pad");
        }

        if (hideOnDesktopWeb) {
            //HandleInputRenderWebDesktop();
        }
    }

    public virtual void ShowInputObject(float time, float delay) {

        TweenUtil.MoveToObject(gameObject, Vector3.zero.WithY(0), time, delay);

        //UITweenerUtil.MoveTo(
        //    gameObject,
        //    UITweener.Method.EaseInOut,
        //    UITweener.Style.Once, time, delay, Vector3.zero.WithY(0));

        controlsVisible = true;
    }

    public virtual void HideInputObject(float time, float delay) {

        TweenUtil.MoveToObject(gameObject, Vector3.zero.WithY(3000), time, delay);

        //UITweenerUtil.MoveTo(
        //    gameObject,
        //    UITweener.Method.EaseInOut,
        //    UITweener.Style.Once, time, delay, Vector3.zero.WithY(3000));

        controlsVisible = false;
    }

    public void HandleInputRenderWebDesktop() {
        if (Context.Current.isWebGL || Application.isEditor) {
            if (controlsVisible) {
                HideInputObject(.5f, 0f);
            }
        }
        else {
            if (!controlsVisible) {
                ShowInputObject(.5f, 0f);
            }
        }
    }

    public void HandleCollision(Collision collision) {

    }

    public bool PointHitTest(Vector3 point) {

        bool hitPad = false;
        bool hitPlacement = false;

        if (collisionCamera != null) {

            Ray screenRay = collisionCamera.ScreenPointToRay(point);
            RaycastHit hit;

            // Mask to what this camera actually draws. Unmasked, the nearest collider on ANY
            // layer answers -- a level collider in front of the HUD plane could make the pad
            // believe the finger had left it.
            if (Physics.Raycast(screenRay, out hit, Mathf.Infinity, collisionCamera.cullingMask)
                && hit.transform != null) {

                //Debug.Log("hit:" + hit.transform.gameObject.name);

                hitObject = hit.transform.gameObject;

                if (hitObject != null) {
                    axisPadObject = hitObject.Get<GameTouchInputAxisPad>();
                    if (axisPadObject != null) {
                        //if(hit.transform.gameObject == gameObject) {
                        if (axisPadObject.gameTouchInputAxis.axisName == axisName) {
                            hitPad = true;

                            //Debug.Log("PointHitTest:" + " hitPad:" + hitPad.ToString() + " axisPadName:" + axisName);
                            // }
                        }
                    }
                    else if (hitObject.name.Contains("AxisInputPlacement-" + axisName)) {
                        hitPlacement = true;
                    }
                }
            }

            if (!hitPad) {
                inUse = false;
                anchorPoint = Vector3.zero;
            }

            if (controlsMoveable) {

                if (objectPlacement != null) {

                    if (hitObject != null) {
                        //LogUtil.Log("hitObject:" + " hitObject:" + hitObject.name);
                        if (hitObject.name.Contains("AxisInputPlacement-" + axisName)) {
                            hitPlacement = true;
                        }
                    }

                    Vector3 viewportPoint = collisionCamera.ScreenToViewportPoint(point);

                    //Debug.Log("viewportPoint:" + " viewportPoint:" + viewportPoint);

                    Vector3 worldPoint = collisionCamera.ViewportToWorldPoint(viewportPoint);

                    //Debug.Log("worldPoint:" + " worldPoint:" + worldPoint);

                    viewportPoint.z = -10;
                    worldPoint.z = -10;

                    //Debug.Log("hitPlacement:" + " hitPlacement:" + hitPlacement);

                    if (hitPad) {

                        // MOVE PAD with movement

                        if (pad) {
                            pad.transform.position = worldPoint;
                        }

                        Vector3 deltaPos = pad.transform.position - objectPlacement.transform.position;

                        deltaPos *= 10;

                        axisInput.x = deltaPos.x;
                        axisInput.y = deltaPos.y;

                        //Debug.Log("axisInput:" + " axisInput:" + axisInput);

                        //if(axisInput.x != 0 || axisInput.y != 0) {
                        //Debug.Log("axisInput:" + " axisInput:" + axisInput);
                        //}

                        GameController.SendInputAxisMessage(axisName, axisInput);

                    }
                    else if (hitPlacement && !hitPad) {

                        // MOVE IT -- but only within the authored zone.

                        ResetPad();

                        ////Vector3 viewPos = collisionCamera.WorldToViewportPoint(point);  

                        MovePlacement(worldPoint);

                        anchorPoint = objectPlacement.transform.position;
                    }
                }
            }
        }

        return hitPad;
    }

    // The placement may only travel inside its own zone, so the pad cannot end up sitting on
    // another control.
    Vector2 GetPlacementTravelLimit() {

        if (placementTravelLimit > 0f) {
            return new Vector2(placementTravelLimit, placementTravelLimit);
        }

        BoxCollider box = objectPlacement.GetComponent<BoxCollider>();

        if (box != null) {
            Vector3 scale = objectPlacement.transform.localScale;
            return new Vector2(
                Mathf.Abs(box.size.x * scale.x) * .5f,
                Mathf.Abs(box.size.y * scale.y) * .5f);
        }

        return Vector2.zero;
    }

    void MovePlacement(Vector3 worldPoint) {

        if (objectPlacement == null) {
            return;
        }

        Transform t = objectPlacement.transform;

        Vector3 local = t.parent != null
            ? t.parent.InverseTransformPoint(worldPoint)
            : worldPoint;

        Vector2 limit = GetPlacementTravelLimit();

        if (limit == Vector2.zero) {
            // Nothing to clamp against -- stay put rather than wander.
            return;
        }

        local.x = Mathf.Clamp(local.x, originalPlacement.x - limit.x, originalPlacement.x + limit.x);
        local.y = Mathf.Clamp(local.y, originalPlacement.y - limit.y, originalPlacement.y + limit.y);
        local.z = originalPlacement.z;

        t.localPosition = local;
    }

    void RestorePlacement() {

        if (objectPlacement != null) {
            objectPlacement.transform.localPosition = originalPlacement;
        }
    }

    void ResetPad() {

        if (!GameController.touchHandled
            && axisName.Contains(InputSystemKeys.moveKey)
            || !axisName.Contains(InputSystemKeys.moveKey)) {
            axisInput.x = 0f;
            axisInput.y = 0f;

            GameController.SendInputAxisMessage(axisName, axisInput);
        }

        if (pad != null) {
            Vector3 padPos = pad.localPosition;
            padPos.x = 0;
            padPos.y = 0;
            padPos.z = 0;
            pad.localPosition = padPos;
        }
    }

    // True while the not-running release has already been applied, so it runs once per stop.
    bool releasedForNotRunning = false;

    void Update() {

        if (!GameConfigs.isGameRunning) {

            // The pad keeps whatever deflection it had when the round ended -- both the art and
            // the cached axisInput -- because this whole method is behind the gate. Let it go
            // once so the next round does not open with a stick already pushed over.
            if (!releasedForNotRunning) {
                releasedForNotRunning = true;

                inUse = false;
                anchorPoint = Vector3.zero;

                if (objectPlacement != null) {
                    objectPlacement.transform.localPosition = originalPlacement;
                }

                // Not ResetPad(): its move-pad branch is gated on the static
                // GameController.touchHandled, which is itself frozen at whatever the last
                // running frame left. Release unconditionally here.
                axisInput.x = 0f;
                axisInput.y = 0f;

                GameController.SendInputAxisMessage(axisName, axisInput);

                if (pad != null) {
                    pad.localPosition = Vector3.zero;
                }
            }

            return;
        }

        releasedForNotRunning = false;

        bool mousePressed = InputSystem.isMousePressed;
        bool touchPressed = InputSystem.isTouchPressed;

        bool leftPressed = InputSystem.isLeftPress;
        bool rightPressed = InputSystem.isRightPress;
        bool upPressed = InputSystem.isUpPress;
        bool downPressed = InputSystem.isDownPress;

        if (axisName.IsEqualLowercase(InputSystemKeys.moveKey)) {
            //Debug.Log("keysDown:" + " leftPressed:" + leftPressed.ToString()
            // + " rightPressed:" + rightPressed.ToString()
            // + " upPressed:" + upPressed.ToString()
            //          + " downPressed:" + downPressed.ToString()
            //          + " touchPressed:" + touchPressed.ToString()
            //          + " mousePressed:" + mousePressed.ToString());
        }

        bool handled = false;

        if (touchPressed) {// && controlsVisible) {
            foreach (Touch touch in Input.touches) {
                handled = PointHitTest(touch.position);
                if (handled)
                    break;
            }
        }
        else if (mousePressed) {//  && hideOnDesktopWeb) {
            handled = PointHitTest(Input.mousePosition);
        }
        else {
            RestorePlacement();
        }

        if (!handled
            && ((leftPressed
            || rightPressed
            || upPressed
            || downPressed)
            && (axisName.IsEqualLowercase(InputSystemKeys.mainKey)
            || axisName.IsEqualLowercase(InputSystemKeys.moveKey)))) {

            Vector3 axisInput = Vector3.zero;

            if (upPressed) {
                axisInput.y = 0.99f;
            }

            if (leftPressed) {
                axisInput.x = -0.99f;
            }

            if (downPressed) {
                axisInput.y = -0.99f;
            }

            if (rightPressed) {
                axisInput.x = 0.99f;
            }

            if (pad != null) {
                Vector3 padPos = pad.localPosition;
                padPos.x = -axisInput.x;
                padPos.y = -axisInput.y;
                padPos.z = -axisInput.y;
                pad.localPosition = padPos;
            }

            GameController.SendInputAxisMessage(axisName, axisInput);

            handled = true;
        }

        if (axisName.IsEqualLowercase(InputSystemKeys.moveKey)) {
            //LogUtil.Log("handled:" + " handled:" + handled.ToString());
        }

        if (!handled) {
            ResetPad();

            // Not just when NOTHING is pressed: a finger on the OTHER pad kept touchPressed
            // true, so a pad that had been dragged stayed where it was dragged to -- over
            // whatever it had been carried on top of.
            RestorePlacement();
        }
    }
}