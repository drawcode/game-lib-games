using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine;
using Engine.Events;
using Engine.Utility;

public class GameTouchInputTexturedMessages {
    public static string inputAxis = "input-axis";
}

public class GameTouchInputAxisTextured : GameObjectBehavior {
    
    public static string inputAxisMove = "input-axis-move";
    public static string inputAxisAttack = "input-axis-attack";
    public static string inputAxisMoveHorizontal = "input-axis-move-horizontal";
    public static string inputAxisMoveVertical = "input-axis-move-vertical";
    public static string inputAxisAttack2DSide = "input-axis-attack-2d-side";
    public static string inputAxisAttack2DSide2 = "input-axis-attack-2d-side-2";
    public static bool updateFingerNavigate = false;
    public GameObject objectPlacement;
    public Camera collisionCamera;
    public Transform pad;// = gameObject.transform.FindChild("Pad");
    public string axisName = "main";
    public Vector3 axisInput;
    public Vector3 padPos;
    public bool controlsVisible = true;
    public bool controlsMoveable = false;
    public bool hideOnDesktopWeb = false;
    public UIAnchor anchor;
    Vector3 originalPlacement = Vector3.zero;
    GameObject hitObject;
    GameTouchInputAxisTextured axisObject;
    
    void Awake() {
        
    }
    
    void Start() {
        
        if (objectPlacement != null) {
            originalPlacement = objectPlacement.transform.localPosition;
        }
    }
    
    void FindPad() {
        if (pad == null) {
            pad = gameObject.transform.FindChild("Pad");
        }
        
        if (hideOnDesktopWeb) {         
            //HandleInputRenderWebDesktop();
        }
    }
    
    public virtual void ShowInputObject(float time, float delay) {
        UITweenerUtil.MoveTo(
            gameObject, 
            UITweener.Method.EaseInOut, 
            UITweener.Style.Once, time, delay, Vector3.zero.WithY(0)); 
        
        controlsVisible = true;
    }
    
    public virtual void HideInputObject(float time, float delay) {
        UITweenerUtil.MoveTo(
            gameObject, 
            UITweener.Method.EaseInOut, 
            UITweener.Style.Once, time, delay, Vector3.zero.WithY(3000));  
        
        controlsVisible = false;          
    }
    
    public void HandleInputRenderWebDesktop() {   
        if (Application.isWebPlayer || Application.isEditor) {
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
    
    public bool PointHitTest(Vector3 point) {
        
        bool hitThis = false;
        
        if (collisionCamera != null) {
            
            Ray screenRay = collisionCamera.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {   
                
                LogUtil.Log("hit:" + hit.transform.gameObject.name);
                
                hitObject = hit.transform.gameObject;
                
                if (hitObject != null) {
                    axisObject = hitObject.Get<GameTouchInputAxisTextured>();
                    if (axisObject != null) {
                        //if(hit.transform.gameObject == gameObject) {
                        if (axisObject.axisName == axisName) {
                            hitThis = true;
                            
                            LogUtil.Log("PointHitTest:" + " hitThis:" + hitThis.ToString() + " axisName:" + axisName);
                            // }
                        }
                    }
                }
            }
            
            
            
            if (hitThis) {
                
                if (axisName == "move") { 
                    //&& GameController.isFingerNavigating) {
                    //hitThis = false;
                    //return hitThis;
                }
                
                axisInput.x = (hit.textureCoord.x - .5f) * 2;
                axisInput.y = (hit.textureCoord.y - .5f) * 2;
                
                GameController.SendInputAxisMessage(axisName, axisInput);
                
                if (pad != null) {
                    padPos = pad.localPosition;
                    padPos.x = -Mathf.Clamp(axisInput.x * 1.5f, -1.2f, 1.2f);
                    padPos.z = -Mathf.Clamp(axisInput.y * 1.5f, -1.2f, 1.2f);
                    padPos.y = 0f;
                    pad.localPosition = padPos;
                }
            }
            else {
                //ResetPad();
                
                if (controlsMoveable) {
                    
                    if (objectPlacement != null) {
                        
                        if (axisName == "move") {
                            
                            bool hitPlacement = false;
                            
                            if (hitObject != null) {
                                LogUtil.Log("hitObject:" + " hitObject:" + hitObject.name);
                                if (hitObject.name.Contains("AxisInputPlacement-" + axisName)) {
                                    hitPlacement = true;
                                }
                            }
                            
                            LogUtil.Log("hitPlacement:" + " hitPlacement:" + hitPlacement);
                            
                            if (hitPlacement) {
                                
                                //}
                                //if((point.x > 10 && point.x < Screen.width / 2)) {
                                
                                LogUtil.Log("hitThis:" + " objectPlacement.transform.position:" + objectPlacement.transform.position);
                                LogUtil.Log("hitThis:" + " point:" + point);
                                
                                Vector3 viewPos = collisionCamera.WorldToViewportPoint(point);
                                
                                if (anchor != null) {
                                    Vector3 offset = anchor.transform.position;
                                    //offset.y += 1000;
                                    
                                    LogUtil.Log("hitThis:" + " offset:" + offset);
                                    
                                    Vector3 movedPos = viewPos;
                                    movedPos.x += offset.x - (originalPlacement.x) - 20;
                                    movedPos.y += ((1000 - (Screen.height - (offset.y))) + originalPlacement.y) + 180;
                                    movedPos.x *= 2; // hald pixel offset
                                    movedPos.y *= 2; // hald pixel offset
                                    //movedPos.y += 1000;
                                    LogUtil.Log("hitThis:" + " movedPos:" + movedPos);
                                    
                                    objectPlacement.transform.localPosition = movedPos;
                                }
                                else {                          
                                    LogUtil.Log("hitThis:" + " viewPos:" + viewPos);          
                                    objectPlacement.transform.localPosition = viewPos;                             
                                }
                            }
                        }                    
                    }                
                }
            }
        }
        
        return hitThis;
    }
    
    void ResetPad() {
        
        if (!GameController.touchHandled && axisName.Contains("move")
            || !axisName.Contains("move")) {
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
    
    void Update() {
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        bool mousePressed = InputSystem.isMousePressed;
        bool touchPressed = InputSystem.isTouchPressed;
        
        bool leftPressed = InputSystem.isLeftPressed;
        bool rightPressed = InputSystem.isRightPressed;
        bool upPressed = InputSystem.isUpPressed;
        bool downPressed = InputSystem.isDownPressed;
        
        if (axisName == "move") {
            //LogUtil.Log("keysDown:" + " leftPressed:" + leftPressed.ToString()
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
            if (objectPlacement != null) {
                objectPlacement.transform.localPosition = originalPlacement;
            }
        }
        
        if (!handled 
            && ((leftPressed
            || rightPressed
            || upPressed
            || downPressed)
            && (axisName == "main"
            || axisName == "move"))) {
            
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
        
        if (axisName == "move") {
            //LogUtil.Log("handled:" + " handled:" + handled.ToString());
        }
        
        if (!handled) {
            ResetPad();
        }
    }
}

