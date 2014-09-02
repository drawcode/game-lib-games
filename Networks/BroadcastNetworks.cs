#define BROADCAST_USE_EVERYPLAY
//#define BROADCAST_USE_TWITCH

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public enum BroadcastNetworkType { 
    Everplay,
    Twitch
}

public class BroadcastNetworksMessages {
    
    public static string broadcastRecordingStart = "broadcast-recording-start";
    public static string broadcastRecordingStop = "broadcast-recording-stop";
    public static string broadcastRecordingPlayback = "broadcast-recording-playback";
}

public class BroadcastNetworks : GameObjectBehavior { 
    #if UNITY_EDITOR    
    #elif UNITY_STANDALONE_OSX
    #elif UNITY_STANDALONE_WIN
    #elif UNITY_ANDROID    
    #elif UNITY_IPHONE
    #endif

    #if BROADCAST_USE_EVERYPLAY
    public Everyplay broadcastEveryplay;
    #endif
    
    public static bool broadcastNetworksEnabled = AppConfigs.broadcastNetworksEnabled;
    public static bool broadcastNetworksTestingEnabled = AppConfigs.broadcastNetworksTestingEnabled;

    // Only one BroadcastNetworks can exist. We use a singleton pattern to enforce this.
    private static BroadcastNetworks _instance = null;
    
    public static BroadcastNetworks Instance {
        get {
            if (!_instance) {
                
                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(BroadcastNetworks)) as BroadcastNetworks;
                
                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_BroadcastNetworks");
                    _instance = obj.AddComponent<BroadcastNetworks>();
                }
            }
            
            return _instance;
        }
    }
    
    void Start() {
        Init();
    }
    
    void OnEnable() {
        
        
        #if BROADCAST_USE_EVERYPLAY
        // ------------
        // EVERPLAY
        
        Everyplay.RecordingStarted += everyplayRecordingStartedDelegate;
        Everyplay.RecordingStopped += everyplayRecordingStoppedDelegate;
        Everyplay.ThumbnailReadyAtFilePath += everyplayThumbnailReadyAtFilePathDelegate;

        #endif

        
    }
    
    void OnDisable() {
        
        
        #if BROADCAST_USE_EVERYPLAY
        // ------------
        // EVERPLAY
        
        Everyplay.ThumbnailReadyAtFilePath -= everyplayThumbnailReadyAtFilePathDelegate;
        Everyplay.RecordingStarted -= everyplayRecordingStartedDelegate;
        Everyplay.RecordingStopped -= everyplayRecordingStoppedDelegate;

        #endif

    }
    
    public void Init() {  
        
        #if BROADCAST_USE_EVERYPLAY
        Invoke("everyplayInit", 1);
        #endif       
    }
    
    #if BROADCAST_USE_EVERYPLAY
    // ----------------------------------------------------------------------
    // EVERPLAY - https://developers.everyplay.com/doc/Everyplay-integration-to-Unity3d-game
    
    public void everyplayInit() {

        if(broadcastEveryplay == null) {
                
            broadcastEveryplay = FindObjectOfType(typeof(Everyplay)) as Everyplay;
                
            if (!broadcastEveryplay) {
                var obj = new GameObject("_BroadcastEveryplay");
                broadcastEveryplay = obj.AddComponent<Everyplay>();
                DontDestroyOnLoad(broadcastEveryplay);
            }
        }

        //broadcastEveryplay.clientId = AppConfigs.broadcastEveryplayClientId;
        //Everyplay.broadcastEveryplay.clientSecret = AppConfigs.broadcastEveryplayClientSecret;
        //broadcastEveryplay.redirectURI = AppConfigs.broadcastEveryplayAuthUrl;
        
    }
    
    // RECORDING
    
    public void everyplayIsRecordingSupported() {
        LogUtil.Log("everyplayIsRecordingSupported");        
        Everyplay.IsRecordingSupported();
    }        
    
    public void everyplayRecordingStartedDelegate() {
        LogUtil.Log("Recording was started");
        /* The recording is now started, show the red "REC" in the upper hand corner */
        //MyGameEngine.ShowRecordingIndicator();
    }
    
    public void everyplayRecordingStoppedDelegate() {
        LogUtil.Log("Recording ended");
        /* Remove visual indicator from the user */
        //MyGameEngine.RemoveRecordingIndicator();
    }
    
    public void everyplayThumbnailReadyAtFilePathDelegate(string path) {
        LogUtil.Log("Thumbnail ready: "  + path);
        //this.thumbnailPath = path;
    }

    #endif
        
    
    // ----------------------------------------------------------------------
    
    // GENERIC CALLS
    
    
    // ----------------------------------------------------------------------
    
    // BROADCAST

    // OPEN
    
    public static void Open() {
        if (Instance != null) {
            Instance.open();
        }
    }
    
    public void open() {
        
        #if BROADCAST_USE_EVERYPLAY
        Everyplay.Show();
        #else
        #endif
    }

    // IS SUPPORTED

    public static bool IsSupported() {
        if (Instance != null) {
            return Instance.isSupported();
        }
        return false;
    }
    
    public bool isSupported() {
        
        #if BROADCAST_USE_EVERYPLAY
        return Everyplay.IsSupported();
        #else
        return false;
        #endif
    }

    // IS RECORDING SUPPORTED    
    
    public static bool IsRecordingSupported() {
        if (Instance != null) {
            return Instance.isRecordingSupported();
        }
        return false;
    }
    
    public bool isRecordingSupported() {
        
        #if BROADCAST_USE_EVERYPLAY
        return Everyplay.IsRecordingSupported();
        #else
        return false;
        #endif
    }
        
    // IS RECORDING    
    
    public static bool IsRecording() {
        if (Instance != null) {
            return Instance.isRecording();
        }
        return false;
    }
    
    public bool isRecording() {
        
        #if BROADCAST_USE_EVERYPLAY
        return Everyplay.IsRecording();
        #else
        return false;
        #endif
    }
    
    // IS RECORDING    
    
    public static bool IsPaused() {
        if (Instance != null) {
            return Instance.isPaused();
        }
        return false;
    }
    
    public bool isPaused() {
        
        #if BROADCAST_USE_EVERYPLAY
        return Everyplay.IsPaused();
        #else
        return false;
        #endif
    }

    // START RECORDING
    
    public static void StartRecording() {
        if (Instance != null) {
            Instance.startRecording();
        }
    }
        
    public void startRecording() {
        
        #if BROADCAST_USE_EVERYPLAY
        if(IsRecording()) {
            Everyplay.StartRecording();
        }
        #endif
    }
    
    // STOP RECORDING
    
    public static void StopRecording() {
        if (Instance != null) {
            Instance.stopRecording();
        }
    }
    
    public void stopRecording() {
        
        #if BROADCAST_USE_EVERYPLAY
        if(IsRecording()) {
            Everyplay.StopRecording();
        }
        #endif
    }
    
    // RESUME RECORDING
    
    public static void ResumeRecording() {
        if (Instance != null) {
            Instance.resumeRecording();
        }
    }
    
    public void resumeRecording() {
        
        #if BROADCAST_USE_EVERYPLAY
        if(IsRecording() && IsPaused()) {
            Everyplay.ResumeRecording();
        }
        #endif
    }
        
    // PAUSE RECORDING
    
    public static void PauseRecording() {
        if (Instance != null) {
            Instance.pauseRecording();
        }
    }
    
    public void pauseRecording() {
        
        #if BROADCAST_USE_EVERYPLAY
        if(IsRecording()) {
            Everyplay.PauseRecording();
        }
        #endif
    }

    /*
     * 
     * Everyplay.SetMetadata("level", levelNumber);
Everyplay.SetMetadata("level_name", levelName);
Everyplay.SetMetadata("score", score)
*/

    
    // ----------------------------------------------------------------------
    
    public static void HandleUpdate() {        
        /*
        #if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                #if PROMO_USE_CHARTBOOST
                if (CBBinding.onBackPressed())
                    return;
                else
                    Application.Quit();
                #else 
                Application.Quit();
                #endif
            }
        }
        #endif
        */
    }
    
}


