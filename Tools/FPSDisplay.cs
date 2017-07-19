using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

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
    public Text labelFPS;
#endif
    public float lastFPS = 0f;
    public static FPSDisplay Instance;

    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }

    public void Awake() {

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(this);
            return;
        }

        Instance = this;
        // Init();

    }

    // Use this for initialization
    void Start() {
        timeleft = updateInterval;
    }

    public static float GetCurrentFPS() {
        if(isInst) {
            return Instance.lastFPS;
        }
        return 21f;
    }

    public static bool IsFPSLessThan(float val) {
        if(isInst) {
            return Instance.lastFPS < val;
        }
        return true;
    }

    public static bool isUnder15FPS {
        get {
            if(isInst) {
                return IsFPSLessThan(15f);
            }
            return false;
        }
    }

    public static bool isUnder20FPS {
        get {
            if(isInst) {
                return IsFPSLessThan(20f);
            }
            return false;
        }
    }

    public static bool isUnder25FPS {
        get {
            if(isInst) {
                return IsFPSLessThan(25f);
            }
            return false;
        }
    }

    public static bool isUnder30FPS {
        get {
            if(isInst) {
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
        if(timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            lastFPS = fps;

            if(labelFPS != null) {

                string format = System.String.Format("{0:F2} FPS", fps);

                UIUtil.SetLabelValue(labelFPS, format);

                if(fps < 27) {
                    labelFPS.color = Color.Lerp(labelFPS.color, Color.yellow, Time.deltaTime);
                }
                else {
                    if(fps < 10) {
                        labelFPS.color = Color.Lerp(labelFPS.color, Color.red, Time.deltaTime);
                    }
                    else {
                        labelFPS.color = Color.Lerp(labelFPS.color, Color.green, Time.deltaTime);
                        //  DebugConsole.Log(format,level);
                        timeleft = updateInterval;
                        accum = 0.0F;
                        frames = 0;
                    }
                }
            }
        }
    }
}