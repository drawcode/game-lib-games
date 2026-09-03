using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Content;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;
using Engine.Utility;
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
    // Margin to leave at the screen edge when an indicator is clamped, in the HUD's own
    // design units (the shipped GamePlayerIndicatorHUD prefab authors 90) -- NOT the
    // viewport fraction the 0.05 default implies. See UpdateIndicator.
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

        GamePlayerIndicator indicator =
            GamePlayerIndicator.AddIndicator(target, "default", gameIndicatorType);

        UIUtil.SetSpriteColor(indicator.gameObject, colorTo);

        return indicator;
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject target, string type, string gameIndicatorType) {

        GameObject parentObject = null;

#if USE_GAME_LIB_GAMES_UI
        parentObject = GameHUD.Instance.containerOffscreenIndicators;
#endif
        return GamePlayerIndicator.AddIndicator(
            parentObject,
            target, type, gameIndicatorType);
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject parentObject, GameObject target, string gameIndicatorType) {

        return GamePlayerIndicator.AddIndicator(
            parentObject, target, "default", gameIndicatorType);
    }

    public static GamePlayerIndicator AddIndicator(
        GameObject parentObject, GameObject target, string type, string gameIndicatorType) {
        // Spawn indicator
        // target new player

        string modelPath =
            ContentPaths.appCacheVersionSharedPrefabLevelUI + "GamePlayerIndicatorHUD";

        //LogUtil.Log("AddIndicator:modelPath:" + modelPath);

        GameObject prefabIndicator = Resources.Load(modelPath) as GameObject;

        //LogUtil.Log("AddIndicator:prefabIndicator:" + prefabIndicator.name);

        if (prefabIndicator != null) {

            GameObject indicator =
                GameObjectHelper.CreateGameObject(
                    prefabIndicator, Vector3.zero,
                    Quaternion.identity, GameConfigs.usePooledIndicators);

            indicator.transform.parent = parentObject.transform;
            indicator.ResetPosition();

            if (indicator != null) {

                //LogUtil.Log("AddIndicator:indicator:" + indicator.name);

                GamePlayerIndicator indicatorObj =
                    indicator.GetComponent<GamePlayerIndicator>();

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

            GameObject indicator =
                GameObjectHelper.CreateGameObject(
                    prefabIndicator, Vector3.zero,
                    Quaternion.identity, GameConfigs.usePooledIndicators);

            indicator.transform.parent = parentObject.transform;
            indicator.ResetPosition();

            if (indicator != null) {

                //LogUtil.Log("AddIndicator:indicator:" + indicator.name);

                GamePlayerIndicator indicatorObj =
                    indicator.GetComponent<GamePlayerIndicator>();

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

        // Nothing to show for an untyped target. Creating one anyway resolves
        // `indicator-none`, which is not a prefab, and leaves a live indicator carrying no
        // visual at all -- indistinguishable from a missing one.
        if (string.IsNullOrEmpty(gameIndicatorType)
            || gameIndicatorType == BaseDataObjectKeys.none) {
            return;
        }

        // REPLACE, do not merely fill. The type can arrive -- or change -- after the
        // indicator exists: a placeholder action zone is authored `action-none` and is only
        // given its real code by loadLevelActions, about a second after the level items
        // load, which is AFTER gameInitLevelStart has already built its indicator. Guarding
        // only on "does an item exist" froze that first, wrong type in place forever.
        if (gamePlayerIndicatorItem != null) {

            if (gamePlayerIndicatorItem.gameIndicatorTypeCode == gameIndicatorType) {
                return;
            }

            // Detach BEFORE destroying. `usePooledIndicators` is false, so this is a plain
            // GameObject.Destroy and the corpse survives until the end of the frame -- and
            // the pooled path only deactivates. The Has<> guard below searches children
            // INCLUDING inactive ones, so a still-parented corpse makes it report "there is
            // already one", the method falls off the end, and the zone is left with NO
            // indicator at all -- worse than the stale one it replaced.
            GameObject staleIndicatorObject = gamePlayerIndicatorItem.gameObject;

            staleIndicatorObject.transform.parent = null;

            GameObjectHelper.DestroyGameObject(
                staleIndicatorObject, GameConfigs.usePooledIndicators);

            gamePlayerIndicatorItem = null;
        }

        if (!indicatorObject.Has<GamePlayerIndicatorItem>()) {

            string modelPath =
                ContentPaths.appCacheVersionSharedPrefabLevelUI +
                "indicator-" + gameIndicatorType;

            //LogUtil.Log("AddIndicator:modelPath:" + modelPath);

            GameObject prefabIndicatorType = Resources.Load(modelPath) as GameObject;

            //LogUtil.Log("AddIndicator:prefabIndicatorType:" + prefabIndicatorType.name);

            if (prefabIndicatorType != null) {

                GameObject indicator =
                    GameObjectHelper.CreateGameObject(
                        prefabIndicatorType, Vector3.zero, Quaternion.identity,
                        GameConfigs.usePooledIndicators);

                indicator.transform.parent = indicatorObject.transform;
                indicator.ResetPosition();
                indicator.transform.localScale = indicator.transform.localScale * .1f;

                if (indicator != null) {

                    if (!indicator.Has<GamePlayerIndicatorItem>()) {
                        gamePlayerIndicatorItem =
                            indicator.AddComponent<GamePlayerIndicatorItem>();
                    }
                    else {
                        gamePlayerIndicatorItem =
                            indicator.Get<GamePlayerIndicatorItem>();
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

        // Two things were wrong with the gate this replaces.
        //
        // The `distance` parameter was ignored -- every caller's value was thrown away
        // and the currentDistance field read in its place -- and the upper bound meant a
        // target further away than currentRangeMax fell out of the test entirely, so its
        // indicator kept whatever scale the previous life or the previous target had left
        // on it instead of settling at the far size. Clamp into the range and always
        // scale; the distance is still ignored when nobody has measured one (the player
        // and item paths never set it, and 0 there means "unknown", not "touching").

        if (distance > 0f) {

            float currentDistanceSnapshot =
                Mathf.Clamp(distance, 0, currentRangeMax);

            //Debug.Log("ScaleIndicator:currentDistanceSnapshot:" + currentDistanceSnapshot);

            float scaleTo =
                (((currentRangeMax - currentDistanceSnapshot)) / currentRangeMax) * 2f;

            scaleTo = Mathf.Clamp(scaleTo, .6f, 4f);

            // Player-facing size dial, on top of the distance-derived size. Defaults to
            // GameIndicatorConfigs.scale (.9 -- the 10% shrink asked for on device) and is
            // overridden by the Settings: Controls slider. Applied AFTER the clamp so the
            // slider can take the dots below the .6 far-size floor, which is the whole point
            // of a "smaller" setting; the profile read is clamped to [.5, 1.5] at its own end.
            //
            // Read straight off the config rather than the profile: this runs for every
            // indicator on every late tick, and the profile lookup walks an attribute
            // dictionary. Settings: Controls pushes the player's value into the config, which
            // also keeps this lib from having to know the app's GameProfiles exists.
            scaleTo = scaleTo * GameIndicatorConfigs.scale;

            //Debug.Log("ScaleIndicator:scaleTo:" + scaleTo);

            // Re-issued every late tick: the keyed internal backend replaces the
            // prior scale tween on this target, matching LeanTween's stacking here.
            TweenMeta scaleMeta = TweenUtil.GetMetaDefault(
                TweenLib.internalEasing, indicatorObject,
                currentLateTickTime, 0f, false,
                TweenCoord.local, TweenEaseType.linear, TweenLoopType.once);

            TweenUtil.ScaleToObject(scaleMeta, indicatorObject.transform.localScale
                .WithX(scaleTo)
                .WithY(scaleTo));
        }

    }

    public virtual void SetIndicatorPlacementType(
        GamePlayerIndicatorPlacementType indicatorTypeTo) {

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

    // The camera that actually renders this indicator, cached. It is NOT the gameplay
    // camera -- the indicator lives under the HUD, in the HUD camera's space.
    public Camera indicatorUICamera;

    /// <summary>
    /// The camera that draws the indicator's layer, i.e. the one whose space the
    /// indicator's local coordinates mean something in. Cached: resolving it walks
    /// every camera in the scene.
    /// </summary>
    public virtual Camera GetIndicatorUICamera() {

        if (indicatorUICamera != null && indicatorUICamera.isActiveAndEnabled) {
            return indicatorUICamera;
        }

        if (indicatorObject == null) {
            return null;
        }

        int layerMask = 1 << indicatorObject.layer;
        Camera found = null;

        foreach (Camera candidate in Camera.allCameras) {

            if ((candidate.cullingMask & layerMask) == 0) {
                continue;
            }

            // Topmost wins, the same way the HUD is composited.
            if (found == null || candidate.depth > found.depth) {
                found = candidate;
            }
        }

        indicatorUICamera = found;

        return found;
    }

    // The visible screen area expressed in the indicator container's OWN local units,
    // cached: it only changes when the screen does.
    public Rect indicatorScreenRect;
    public int indicatorScreenRectWidth;
    public int indicatorScreenRectHeight;

    /// <summary>
    /// Measure the screen, in the units the indicator's localPosition is written in, by
    /// projecting the viewport corners through the camera that draws it.
    ///
    /// Measured against the container the indicator hangs off -- NOT against the
    /// indicator itself. Reading the plane distance off the object we are about to move
    /// feeds its own last position back in, and with the HUD camera's transform sitting
    /// inside the scaled UI hierarchy that runs away by orders of magnitude.
    /// </summary>
    public virtual bool TryGetIndicatorScreenRect(out Rect rect) {

        rect = indicatorScreenRect;

        if (indicatorObject == null) {
            return false;
        }

        Transform space = indicatorObject.transform.parent;

        if (space == null) {
            return false;
        }

        if (indicatorScreenRectWidth == Screen.width
            && indicatorScreenRectHeight == Screen.height
            && indicatorScreenRect.width != 0f) {

            return true;
        }

        Camera uiCamera = GetIndicatorUICamera();

        if (uiCamera == null) {
            return false;
        }

        float planeDistance = Mathf.Abs(
            uiCamera.transform.InverseTransformPoint(space.position).z);

        Vector3 bottomLeft = space.InverseTransformPoint(
            uiCamera.ViewportToWorldPoint(new Vector3(0f, 0f, planeDistance)));

        Vector3 topRight = space.InverseTransformPoint(
            uiCamera.ViewportToWorldPoint(new Vector3(1f, 1f, planeDistance)));

        indicatorScreenRect = new Rect(
            bottomLeft.x, bottomLeft.y,
            topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

        indicatorScreenRectWidth = Screen.width;
        indicatorScreenRectHeight = Screen.height;

        rect = indicatorScreenRect;

        return rect.width != 0f && rect.height != 0f;
    }

    public virtual void UpdateIndicator(Vector3 relativePosition) {

        // Where the target is on screen, as a 0..1 viewport point.
        Vector3 indicateTemp = cam.WorldToViewportPoint(
            camTransform.TransformPoint(relativePosition + offset));

        if (indicatorType == GamePlayerIndicatorPlacementType.VIEWPORT) {

            indicatorObject.transform.localPosition = indicateTemp;

            return;
        }

        Rect uiRect;

        if (!TryGetIndicatorScreenRect(out uiRect)) {

            // Nothing draws this layer. Fall back to the project's own screen
            // convention: ScreenUtil is referenced to a 640-unit design height, so
            // dividing BOTH axes by relativeHeight keeps the aspect instead of
            // squashing it onto a fixed 960x640.

            float unitsPerPixel = ScreenUtil.relativeHeight;

            if (unitsPerPixel <= 0f) {
                unitsPerPixel = 1f;
            }

            float fallbackWidth = Screen.width / unitsPerPixel;
            float fallbackHeight = Screen.height / unitsPerPixel;

            uiRect = new Rect(
                -fallbackWidth * .5f, -fallbackHeight * .5f,
                fallbackWidth, fallbackHeight);
        }

        // uiRect is the visible screen measured in the indicator container's OWN local
        // units, so the viewport point maps straight onto it with no unit conversion.
        //
        // What this replaces: the position was written as raw device PIXELS
        // (ViewportToScreenPoint) and clamped against Screen.width/2 and Screen.height/2,
        // into a container whose space is the HUD root's design units. Measured live on a
        // 2137x1357 screen the visible area is +/-692.5 x +/-320 of those units while the
        // clamp bounded to +/-1068.5 x +/-678.5, so every indicator was placed about twice
        // as far out as the screen edge and the whole set sat off screen. The error scaled
        // with the display, which is why it could look right at one size and vanish at
        // another.

        float placedX = uiRect.x + (indicateTemp.x * uiRect.width);
        float placedY = uiRect.y + (indicateTemp.y * uiRect.height);

        // clampBorderSize is in the SAME units as uiRect -- the shipped prefab authors it
        // as 90, a design-unit margin, not the 0.05 viewport fraction this field's default
        // and its comment suggest. Clamping it against a 0..1 viewport instead inverts the
        // bounds, and Mathf.Clamp does not complain when min > max: it just returns min,
        // which pins every indicator to the same far-off point.
        //
        // So bound the border to half the screen. A margin wider than the thing it is
        // insetting has no sane reading, and silently returning min is how this hid.

        // edgeBorderScale multiplies the AUTHORED margin rather than replacing it, so the
        // prefabs stay the one place the margin is written down. It ships at .5, halving the
        // authored 90 to 45: the visible area is +/-692.5 x +/-320 container units, so 90 was
        // holding the top and bottom dots 28% of the half-height in from the edge while the
        // side dots sat at 13% -- which is why only some of them read as far off the edge.
        float border = clampBorderSize * GameIndicatorConfigs.edgeBorderScale;

        float borderX = Mathf.Clamp(border, 0f, (uiRect.width * .5f) - 1f);
        float borderY = Mathf.Clamp(border, 0f, (uiRect.height * .5f) - 1f);

        indicatorObject.transform.localPosition = new Vector3(
            Mathf.Clamp(placedX, uiRect.xMin + borderX, uiRect.xMax - borderX),
            Mathf.Clamp(placedY, uiRect.yMin + borderY, uiRect.yMax - borderY),
            indicatorObject.transform.localPosition.z);
    }

    public virtual void LateUpdate() {

        if (camTransform == null) {
            return;
        }

        if (!initialized) {
            return;
        }

        // remove if not found
        //
        // ABOVE the isGameRunning gate deliberately. isGameRunning is
        // `GameController.IsGameRunning && !isUIRunning`, so it goes FALSE for any panel opened
        // during a round, not just at the end of one. With this check below the gate, an indicator
        // whose target died while a panel was up kept pointing at a corpse for as long as the
        // panel stayed open. Reclaiming a dead target is cleanup, not gameplay, and is safe to run
        // whether or not the round is live.
        if (initialized && target == null) {
            initialized = false;
            DestroyMe();
            return;
        }

        // Everything below MOVES the indicator, and that is the part that has to hold still while
        // the game is not running.
        if (!GameConfigs.isGameRunning) {
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
                    if (target.gameObject != null
                        && GameController.CurrentGamePlayerController != null) {

                        currentDistance = Vector3.Distance(
                            target.position,
                            GameController.CurrentGamePlayerController.transform.position);

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

            // Clamping now happens inside UpdateIndicator, in viewport space, before the
            // point is projected into the UI camera. It used to happen here instead,
            // against raw Screen.width/2 and Screen.height/2 bounds -- device pixels
            // compared against a position expressed in the HUD root's own units, which are
            // not pixels. That is what put every indicator off screen.

        }
        else {

            relativePosition = cam.WorldToViewportPoint(target.position + offset);

            UpdateIndicator(relativePosition);
        }
    }
}