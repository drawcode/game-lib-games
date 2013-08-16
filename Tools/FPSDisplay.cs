using UnityEngine;
using System.Collections;

public class FPSDisplay: MonoBehaviour {
	
	public  float updateInterval = 0.5F;
	 
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	
	public UILabel labelFPS;
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
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(this);
            return;
        }
		
		Instance = this;		
       // Init();
		
	}

	// Use this for initialization
	void Start () {
	    timeleft = updateInterval;  
	}
	
	public static float GetCurrentFPS() {
		if(isInst) {
			return Instance.lastFPS;
		}
		return 0f;
	}
	
	// Update is called once per frame
	void Update () {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0) {
		    // display two fractional digits (f2 format)
			float fps = accum/frames;
			lastFPS = fps;
			string format = System.String.Format("{0:F2} FPS",fps);
			
			if(labelFPS != null) {
				labelFPS.text = format;
			 
				if(fps < 27) {
					labelFPS.color = Color.Lerp(labelFPS.color, Color.yellow, Time.deltaTime);
				}
				else { 
					if(fps < 10) {
						labelFPS.color = Color.Lerp(labelFPS.color, Color.red, Time.deltaTime);
					}
					else {
						labelFPS.color = Color.Lerp(labelFPS.color, Color.green, Time.deltaTime);
						//	DebugConsole.Log(format,level);
						timeleft = updateInterval;
						accum = 0.0F;
						frames = 0;
					}
				}
		    }
		}
	}
}
