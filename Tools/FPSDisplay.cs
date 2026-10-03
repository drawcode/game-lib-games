using System;
using System.Collections;

using UnityEngine;

public class GameFPS : FPSDisplay {

}

public class FPSDisplay : GameObjectBehavior {

    public float updateInterval = 0.1F;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelFPS;
#else
    // B10: agnostic UIRef handle (was UGUI Text), the BaseGameHUD pattern. UIRef exposes no
    // colour getter, so the tint lerps from labelFPSColor (white, then whatever was last set).
    public Engine.UI.UIRef labelFPS;
    Color labelFPSColor = Color.white;
#endif
    public float lastFPS = 0f;

    // The rate the throttle intervals are authored against (GameObjectTimer.GetFPSOffset).
    public static float targetFPS = 30f;
    public static FPSDisplay Instance;

    // An INACTIVE FPSDisplay never runs Update, so its lastFPS is frozen at whatever it last
    // measured -- found live reading a stale 165.87 in a scene where both instances were
    // inactive, which every IsTimerPerf gate and both spawn directors were dividing by.
    // A frozen reading is worse than no reading, so only a live component counts.
    public static bool isInst {
        get {
            if (Instance != null && Instance.isActiveAndEnabled) {
                return true;
            }
            return false;
        }
    }

    public void Awake() {

        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(this);
            return;
        }

        Instance = this;
        // Init();

    }

    // The on-screen readout is a development aid: Editor and development builds only. The
    // measurement keeps running everywhere (quality logic reads GetCurrentFPS), only the label
    // is hidden in a release build.
    public static bool showReadout {
        get {
            return Debug.isDebugBuild;
        }
    }

    // Use this for initialization
    void Start() {
        timeleft = updateInterval;

        if (!showReadout && labelFPS != null) {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            labelFPS.gameObject.SetActive(false);
#else
            UIUtil.HideObject(labelFPS);
#endif
        }
    }

    public static float GetCurrentFPS() {
        if (isInst) {
            return Instance.lastFPS;
        }
        // No live display: answer the target rate, so the throttle modifier comes out at 1 and
        // gates run at their authored interval rather than at a made-up 21fps penalty.
        return targetFPS;
    }

    public static bool IsFPSLessThan(float val) {
        // Answers off GetCurrentFPS so a missing display reads as the target rate, matching the
        // throttle. It used to return true with no live display, i.e. "assume the worst", which
        // silently put every isUnderNNFPS caller into its degraded path.
        return GetCurrentFPS() < val;
    }

    public static bool isUnder15FPS {
        get {
            if (isInst) {
                return IsFPSLessThan(15f);
            }
            return false;
        }
    }

    public static bool isUnder20FPS {
        get {
            if (isInst) {
                return IsFPSLessThan(20f);
            }
            return false;
        }
    }

    public static bool isUnder25FPS {
        get {
            if (isInst) {
                return IsFPSLessThan(25f);
            }
            return false;
        }
    }

    public static bool isUnder30FPS {
        get {
            if (isInst) {
                return IsFPSLessThan(30f);
            }
            return false;
        }
    }

    // Update is called once per frame
    void Update() {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            lastFPS = fps;

            if (labelFPS != null) {

                string format = System.String.Format("{0:F2} FPS", fps);

                UIUtil.SetLabelValue(labelFPS, format);

                if (fps < 27) {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                    labelFPS.color = Color.Lerp(labelFPS.color, Color.yellow, Time.deltaTime);
#else
                    labelFPSColor = Color.Lerp(labelFPSColor, Color.yellow, Time.deltaTime);
                    UIUtil.SetLabelColor(labelFPS, labelFPSColor);
#endif
                }
                else {
                    // Unreachable: this is the else of fps < 27, so fps is already >= 27.
                    if (fps < 10) {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                        labelFPS.color = Color.Lerp(labelFPS.color, Color.red, Time.deltaTime);
#else
                        labelFPSColor = Color.Lerp(labelFPSColor, Color.red, Time.deltaTime);
                        UIUtil.SetLabelColor(labelFPS, labelFPSColor);
#endif
                    }
                    else {
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                        labelFPS.color = Color.Lerp(labelFPS.color, Color.green, Time.deltaTime);
#else
                        labelFPSColor = Color.Lerp(labelFPSColor, Color.green, Time.deltaTime);
                        UIUtil.SetLabelColor(labelFPS, labelFPSColor);
#endif
                        //  DebugConsole.Log(format,level);
                    }
                }
            }

            // Close the measurement window here, always. These three used to be reset only
            // in the innermost else, which needed a label AND fps >= 27 -- so the moment the
            // framerate dipped under 27 (or the scene had no label at all) the window never
            // closed: this block ran every frame, and accum / frames became a LIFETIME
            // cumulative average rather than a current reading.
            //
            // lastFPS is now a rolling average over updateInterval again. That matters well
            // beyond the readout: GameObjectTimer.currentModifier is 30 / lastFPS and
            // multiplies EVERY IsTimerPerf gate in the game, and the spawn directors gate on
            // it too. Those now follow the CURRENT framerate -- they throttle harder while a
            // dip lasts and relax again once it clears, instead of being pinned to a
            // lifetime average that a long session can never move.
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}