using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum GamePlayerIndicatorPlacementType {
    VIEWPORT,
    SCREEN
}

public class GamePlayerIndicatorType {
    public static string player = "player";
    public static string enemy = "enemy";
    public static string item = "item";
    public static string pickup = "pickup";
    public static string coin = "coin";
    public static string health = "health";
    public static string powerup = "powerup";
    public static string color = "color";
    public static string goal = "goal";
    public static string choice = "choice";
    public static string zombie = "zombie";
    public static string bot1 = "bot1";
    public static string bot2 = "bot2";
    public static string sidekick = "sidekick";
}

public class BaseGamePlayerIndicator : GameObjectBehavior {
 
    public Transform target;
    // Object that this label should follow
    public Vector3 offset = Vector3.up;
    // Units in world space to offset; 1 unit above object by default
    public bool clampToScreen = true;
    // If true, label will be visible even if object is off screen
    public float clampBorderSize = 0.05f;
    // How much viewport space to leave at the borders when a label is being clamped
    public Camera cameraToUse;
    public GameObject indicatorObject;
    public GamePlayerIndicatorPlacementType indicatorType = GamePlayerIndicatorPlacementType.SCREEN;
    public string gameIndicatorTypeCode = "bot1";
    public GamePlayerController gamePlayerController;
    public GamePlayerIndicatorItem gamePlayerIndicatorItem;
    public GamePlayerItem gamePlayerItem;
    public GameObject goTarget;
    public string type = "color";
    public Camera cam;
    public Transform camTransform;
    public float lastUpdate = 0f;
    public bool visible = true;
    public List<SkinnedMeshRenderer> renderers;
    public int targetNotFoundCycles = 0;
    public Color currentColor;
    public bool initialized = false;
    
    public bool alwaysVisible = false;

    public float currentDistance = 0;
    public float currentRangeMin = 2f;
    public float currentRangeMax = 120f;
    public float currentScale = 0;
    public Vector3 currentScaleVector;
    
    float currentLateTickTime = .3f;

    public virtual void Start() {
        SetCamera(Camera.main);
    }

    public virtual void SetCamera(Camera camToUse) {
        cameraToUse = camToUse;      
     
        if (cameraToUse == null) {
            cam = Camera.main;
        }
        else {
            cam = cameraToUse;
        }
        if (cam == null)
            return;

        if (cam.transform != null)
            camTransform = cam.transform;
    }

    // STATIC

    // TYPE

    public static GamePlayerIndicator AddIndicator(
        GameObject target, GamePlayerIndicatorType type) {

        return GamePlayerIndicator.AddIndicator(target, type.ToString());
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject target, GamePlayerIndicatorType type, Color colorTo) {

        return GamePlayerIndicator.AddIndicator(target, type.ToString(), colorTo);
    }

    // STRING

    public static GamePlayerIndicator AddIndicator(
        GameObject target, string gameIndicatorType) {

        return GamePlayerIndicator.AddIndicator(target, "default", gameIndicatorType);
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject target, string gameIndicatorType, Color colorTo) {

        GamePlayerIndicator indicator = GamePlayerIndicator.AddIndicator(target, "default", gameIndicatorType);
        UIUtil.SetSpriteColor(indicator.gameObject, colorTo);
        return indicator;
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject target, string type, string gameIndicatorType) {
        return GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators,
            target, type, gameIndicatorType);
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject parentObject, GameObject target, string gameIndicatorType) {
        return GamePlayerIndicator.AddIndicator(parentObject, target, "default", gameIndicatorType);
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject parentObject, GameObject target, string type, string gameIndicatorType) {
        // Spawn indicator
        // target new player
         
        string modelPath = ContentPaths.appCacheVersionSharedPrefabLevelUI + "GamePlayerIndicatorHUD";
    
        //LogUtil.Log("AddIndicator:modelPath:" + modelPath);

        GameObject prefabIndicator = Resources.Load(modelPath) as GameObject;
    
        //LogUtil.Log("AddIndicator:prefabIndicator:" + prefabIndicator.name);

        if (prefabIndicator != null) {

            GameObject indicator = GameObjectHelper.CreateGameObject(
                                       prefabIndicator, Vector3.zero, Quaternion.identity, GameConfigs.usePooledIndicators);

            indicator.transform.parent = parentObject.transform;
            indicator.ResetPosition();
    
            if (indicator != null) {
    
                //LogUtil.Log("AddIndicator:indicator:" + indicator.name);
    
                GamePlayerIndicator indicatorObj = indicator.GetComponent<GamePlayerIndicator>();
    
                if (indicatorObj != null) {
    
                    //LogUtil.Log("AddIndicator:indicatorObj:" + indicatorObj.name);
                    //LogUtil.Log("AddIndicator:gameIndicatorType:" + gameIndicatorType);
                    //LogUtil.Log("AddIndicator:target.transform.name:" + target.transform.name);
    
                    indicatorObj.HideIndicator();
                    indicatorObj.indicatorType = GamePlayerIndicatorPlacementType.SCREEN;
                    indicatorObj.SetTarget(target.transform);
                    indicatorObj.transform.localScale = Vector3.one;

                    indicatorObj.type = gameIndicatorType;//(GamePlayerIndicatorType)Enum.Parse(typeof(GamePlayerIndicatorType), gameIndicatorType);

                    indicatorObj.SetGameIndicatorType(gameIndicatorType);
                    indicatorObj.Run();
                }

                return indicatorObj;
            }
        }

        return null;
    }

    public virtual GamePlayerIndicator AddIndicatorItem(
        GameObject parentObject, GameObject target, string type, string gameIndicatorType) {
        string modelPath = ContentPaths.appCacheVersionSharedPrefabLevelUI + "GamePlayerIndicatorHUD";
    
        //LogUtil.Log("AddIndicator:modelPath:" + modelPath);

        GameObject prefabIndicator = Resources.Load(modelPath) as GameObject;
    
        //LogUtil.Log("AddIndicator:prefabIndicator:" + prefabIndicator.name);

        if (prefabIndicator != null) {

            GameObject indicator = GameObjectHelper.CreateGameObject(
                                       prefabIndicator, Vector3.zero, Quaternion.identity, GameConfigs.usePooledIndicators);

            indicator.transform.parent = parentObject.transform;
            indicator.ResetPosition();
    
            if (indicator != null) {
    
                //LogUtil.Log("AddIndicator:indicator:" + indicator.name);
    
                GamePlayerIndicator indicatorObj = indicator.GetComponent<GamePlayerIndicator>();
    
                if (indicatorObj != null) {
    
                    //LogUtil.Log("AddIndicator:indicatorObj:" + indicatorObj.name);
                    //LogUtil.Log("AddIndicator:gameIndicatorType:" + gameIndicatorType);
                    //LogUtil.Log("AddIndicator:target.transform.name:" + target.transform.name);
    
                    indicatorObj.HideIndicator();
                    indicatorObj.indicatorType = GamePlayerIndicatorPlacementType.SCREEN;
                    indicatorObj.SetTarget(target.transform);
                    indicatorObj.transform.localScale = Vector3.one;

                    indicatorObj.type = gameIndicatorType;//(GamePlayerIndicatorType)Enum.Parse(typeof(GamePlayerIndicatorType), gameIndicatorType);

                    indicatorObj.SetGameIndicatorType(gameIndicatorType);
                    indicatorObj.Run();
                }

                return indicatorObj;
            }
        }
        
        return null;
    }

    
    public static void ResetIndicators(GameObject parentObject) {
        if (parentObject != null) {
            parentObject.DestroyChildren(GameConfigs.usePooledIndicators);
        }
    }

    public virtual void SetIndicatorColorEffects(Color colorTo) {
        if (gamePlayerIndicatorItem != null) {
            gamePlayerIndicatorItem.SetColorValueEffects(colorTo);
        }
    }

    public virtual void SetIndicatorColorBackground(Color colorTo) {
        if (gamePlayerIndicatorItem != null) {
            gamePlayerIndicatorItem.SetColorValueBackground(colorTo);
        }
    }

    public virtual void SetIndicatorColorOutline(Color colorTo) {
        if (gamePlayerIndicatorItem != null) {
            gamePlayerIndicatorItem.SetColorValueOutline(colorTo);
        }
    }

    public virtual void SetGameIndicatorType(string gameIndicatorType) {

        //LogUtil.Log("GamePlayerIndicator:gameIndicatorType:" + gameIndicatorType);

        gameIndicatorTypeCode = gameIndicatorType;

        if (indicatorObject == null) {
            return;
        }

        // Check if there is already an indicator loaded
        // If not load

        if (!indicatorObject.Has<GamePlayerIndicatorItem>()) {
            string modelPath = ContentPaths.appCacheVersionSharedPrefabLevelUI + "indicator-" + gameIndicatorType;
    
            //LogUtil.Log("AddIndicator:modelPath:" + modelPath);
    
            GameObject prefabIndicatorType = Resources.Load(modelPath) as GameObject;
    
            //LogUtil.Log("AddIndicator:prefabIndicatorType:" + prefabIndicatorType.name);

            if (prefabIndicatorType != null) {

                GameObject indicator = GameObjectHelper.CreateGameObject(
                                           prefabIndicatorType, Vector3.zero, Quaternion.identity, GameConfigs.usePooledIndicators);

                indicator.transform.parent = indicatorObject.transform;
                indicator.ResetPosition();
                indicator.transform.localScale = indicator.transform.localScale * .1f;

                if (indicator != null) {

                    if (!indicator.Has<GamePlayerIndicatorItem>()) {
                        gamePlayerIndicatorItem = indicator.AddComponent<GamePlayerIndicatorItem>();
                    }
                    else {
                        gamePlayerIndicatorItem = indicator.Get<GamePlayerIndicatorItem>();
                    }

                    gamePlayerIndicatorItem.gameIndicatorTypeCode = gameIndicatorType;
                }
            }
        }
    }

    public virtual void SetTarget(Transform targetTo) {
        targetNotFoundCycles = 0;
        target = targetTo;
     
        if (target != null) {

            //if(type == "player") {
            //    gamePlayerController = target.gameObject.Get<GamePlayerController>();
            //    if(gamePlayerController != null) {
            //        //..
            //    }
            //}
            //else if(type == GamePlayerIndicatorType.item) {
            //    if(gamePlayerItem == null) {
            //        gamePlayerItem = target.gameObject.Get<GamePlayerItem>();
            //        if(gamePlayerItem != null) {
            //            //..
            //        }
            //    }
            //}
        }
    }

    public virtual void Run() {
        initialized = true;
    }

    public virtual void Stop() {
        initialized = false;
    }

    public virtual void ShowIndicator() {
        if (!visible) {
            visible = true;
            indicatorObject.Show();
            //LogUtil.Log("ShowIndicator:visible:" + visible);

        }
        ScaleIndicator(currentDistance);
    }

    public virtual void HideIndicator() {
        HideIndicator(false);
    }

    public virtual void HideIndicator(bool destroy) {
        if (visible) {
            if (!alwaysVisible) {
                visible = false;
                indicatorObject.Hide();
            }
            //LogUtil.Log("HideIndicator:visible:" + visible);
            if (destroy) {
                DestroyMe();
            }
        }
    }

    public void ScaleIndicator(float distance) {

        //if(currentScaleVector == null) {
        //    currentScaleVector = gameObject.transform.localScale;
        //}

        currentScale = gameObject.transform.localScale.y;

        //Debug.Log("ScaleIndicator:distance:" + distance);

        if (currentDistance >= currentRangeMin
            && currentDistance <= currentRangeMax) {

            float currentDistanceSnapshot = Mathf.Clamp(currentDistance, 0, currentRangeMax);

            //Debug.Log("ScaleIndicator:currentDistanceSnapshot:" + currentDistanceSnapshot);

            float scaleTo = (((currentRangeMax - currentDistanceSnapshot)) / currentRangeMax) * 2f;

            scaleTo = Mathf.Clamp(scaleTo, .6f, 4f);

            //Debug.Log("ScaleIndicator:scaleTo:" + scaleTo);

            LeanTween.scale(indicatorObject, indicatorObject.transform.localScale
                .WithX(scaleTo)
                .WithY(scaleTo), currentLateTickTime);
        }

    }

    public virtual void SetIndicatorPlacementType(GamePlayerIndicatorPlacementType indicatorTypeTo) {
        indicatorType = indicatorTypeTo;
    }

    public virtual void SetIndicatorObject(GameObject indicatorObjectTo) {
        indicatorObject = indicatorObjectTo;
    }

    public virtual void DestroyMe() {
        foreach (Transform t in indicatorObject.transform) {
            GameObjectHelper.DestroyGameObject(
                t.gameObject, GameConfigs.usePooledIndicators);
        }

        GameObjectHelper.DestroyGameObject(
            gameObject, GameConfigs.usePooledIndicators);
    }

    public virtual void UpdateIndicator(Vector3 relativePosition) {
             
        Vector3 indicateTemp = indicatorObject.transform.position;       
        //LogUtil.Log("indicateTemp1:" + indicateTemp);
     
        indicateTemp = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition + offset));
        //LogUtil.Log("indicateTemp1viewport:" + indicateTemp);
     
        if (indicatorType == GamePlayerIndicatorPlacementType.VIEWPORT) {
            indicatorObject.transform.localPosition = indicateTemp;
        }
        else { //(cam.WorldToScreenPoint(relativePosition + offset));//camTransform.TransformPoint(relativePosition + offset)) * 1f);//.WithY(0f);   
            indicateTemp.x = indicateTemp.x - .5f;
            indicateTemp.y = indicateTemp.y - .5f;
            indicateTemp = cam.ViewportToScreenPoint(indicateTemp);
            // adjust for HUD
            //indicateTemp.y = indicateTemp.y / 1000;
            //indicateTemp.z = 1f;
            //LogUtil.Log("indicateTemp2:" + indicateTemp);

            indicatorObject.transform.localPosition = 
            //    Vector3.Lerp(
            //indicatorObject.transform.localPosition, 
                indicateTemp;
            //, currentLateTickTime);
         
            //UITweenerUtil.MoveTo(indicatorObject, UITweener.Method.Linear, UITweener.Style.Once, .1f, 0f, indicateTemp);
        }
    }

    public virtual void LateUpdate() {

        if (camTransform == null) {
            return;
        }

        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (!initialized) {
            return;
        }

        // remove if not found

        if (initialized && target == null) {
            initialized = false;
            DestroyMe();
            return;
        }
        
        Vector3 relativePosition = Vector3.zero;

        if (Time.time > lastUpdate + currentLateTickTime) {
         
            lastUpdate = Time.time;

            if (type == GamePlayerIndicatorType.player) {
                if (gamePlayerController == null) {
                    HideIndicator(true);
                    return;
                }

                if (gamePlayerController != null) {
                    // Hide/show off screen indicator
                    if (!gamePlayerController.controllerData.visible) {

                        if (gamePlayerController.gameObject.activeInHierarchy) {                            
                            ShowIndicator();
                        }
                        else {
                            HideIndicator();
                            return;
                        }
                    }
                    else {
                        HideIndicator();
                        return;
                    }
                }
            }
            else if (type == GamePlayerIndicatorType.item) {
                if (gamePlayerItem == null) {
                    HideIndicator(true);
                    return;
                }

                if (gamePlayerItem != null) {
                    // Hide/show off screen indicator
                    if (!gamePlayerItem.gameObject.IsRenderersVisibleByCamera(Camera.main)) {

                        if (gamePlayerItem.gameObject.activeInHierarchy) {                            
                            ShowIndicator();
                        }
                        else {
                            HideIndicator();
                            return;
                        }
                    }
                    else {
                        HideIndicator();
                        return;
                    }
                }
            }
            else {
                if (gamePlayerController == null
                    && gamePlayerItem == null
                    && target == null) {
                    HideIndicator();
                }

                if (target != null) {
                    // Hide/show off screen indicator
                    if (target.gameObject != null && GameController.CurrentGamePlayerController != null) {

                        currentDistance = Vector3.Distance(target.position, GameController.CurrentGamePlayerController.transform.position);

                        if (target.gameObject.IsRenderersVisibleByCamera()
                            || currentDistance < currentRangeMin) {
                            HideIndicator();
                            return;
                        }
                        else {
                            
                            if (target.gameObject.activeInHierarchy) {                            
                                ShowIndicator();
                            }
                            else {
                                HideIndicator();
                                return;
                            }
                        }
                    }
                    else {
                        HideIndicator();
                        return;
                    }
                }
            }
        }

        // target check

        if (target == null) {
            targetNotFoundCycles++;

            if (targetNotFoundCycles > 100) {
                DestroyMe();
            }

            return;
        }
        else {
            targetNotFoundCycles = 0;
        }

        if (gamePlayerIndicatorItem != null) {
            gamePlayerIndicatorItem.transform.position = 
            //Vector3.Lerp(gamePlayerIndicatorItem.transform.position, 
                             gamePlayerIndicatorItem.transform.position.WithZ(
                (((transform.position.x) *
                (transform.position.y)) * .5f) - 1f);//, currentLateTickTime);
        }
     
        if (clampToScreen && target != null) {
            relativePosition = camTransform.InverseTransformPoint(target.position);
            relativePosition.z = Mathf.Max(relativePosition.z, 1.0f);    
         
            UpdateIndicator(relativePosition);           
         
            if (indicatorType == GamePlayerIndicatorPlacementType.SCREEN) {
         
                /* maybe lerp periodically...
             indicatorObject.transform.localPosition = Vector3.Lerp (
                 indicatorObject.transform.localPosition, new Vector3(
                     Mathf.Clamp(indicatorObject.transform.localPosition.x, -Screen.width/2 + clampBorderSize, Screen.width/2 - clampBorderSize),
                     Mathf.Clamp(indicatorObject.transform.localPosition.y, -Screen.height/2 + clampBorderSize, Screen.height/2 - clampBorderSize),
                     indicatorObject.transform.localPosition.z),
                 Time.deltaTime * .1f);
                */

                float clampHeight = (clampBorderSize * ScreenUtil.relativeHeight);
                float clampWidth = (clampBorderSize * ScreenUtil.relativeWidth);


                indicatorObject.transform.localPosition = 
                // Vector3.Lerp(indicatorObject.transform.localPosition, 
                    new Vector3(

                    Mathf.Clamp(
                        indicatorObject.transform.localPosition.x,
                        -Screen.width / 2 + clampWidth,
                        Screen.width / 2 - clampWidth),

                    Mathf.Clamp(indicatorObject.transform.localPosition.y,
                        -Screen.height / 2 + clampHeight,
                        Screen.height / 2 - clampHeight),

                    indicatorObject.transform.localPosition.z);
                //, currentLateTickTime);
         
            }
            else {           

                indicatorObject.transform.localPosition = 
                //Vector3.Lerp(indicatorObject.transform.localPosition, 
                    new Vector3(
                    Mathf.Clamp(indicatorObject.transform.localPosition.x, clampBorderSize, 1.0f - clampBorderSize),
                    Mathf.Clamp(indicatorObject.transform.localPosition.y, clampBorderSize, 1.0f - clampBorderSize),
                    indicatorObject.transform.localPosition.z);//, currentLateTickTime);
            }
         
        }
        else {
            relativePosition = cam.WorldToViewportPoint(target.position + offset); 
            UpdateIndicator(relativePosition); 
        }
    }
}