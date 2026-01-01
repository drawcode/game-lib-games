//#define BROADCAST_USE_EVERYPLAY
//#define BROADCAST_USE_TWITCH

#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#elif UNITY_IPHONE
//#define BROADCAST_USE_EVERYPLAY
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;
using Engine.Game.App.BaseApp;

public enum BroadcastNetworkType {
    Everplay,
    Twitch
}

public class BroadcastNetworksMessages {

    public static string broadcastRecordingStatusChanged = "broadcast-recording-status-changed";
    public static string broadcastRecordingStart = "broadcast-recording-start";
    public static string broadcastRecordingStop = "broadcast-recording-stop";
    public static string broadcastRecordingPlayback = "broadcast-recording-playback";
    public static string broadcastFacecamStart = "broadcast-facecam-start";
    public static string broadcastFacecamStop = "broadcast-facecam-stop";
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


#if USE_CONFIG_APP
    public static bool broadcastNetworksEnabled = AppConfigs.broadcastNetworksEnabled;
    public static bool broadcastNetworksTestingEnabled = AppConfigs.broadcastNetworksTestingEnabled;
#else 
    public static bool broadcastNetworksEnabled = false;
    public static bool broadcastNetworksTestingEnabled = false;
#endif

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
        //Everyplay.ThumbnailReadyAtFilePath += everyplayThumbnailReadyAtFilePathDelegate;

        Everyplay.ReadyForRecording += everyplayReadyForRecordingDelegate;
        Everyplay.UploadDidComplete += everyplayUploadDidCompleteDelegate;
        Everyplay.UploadDidProgress += everyplayUploadDidProgressDelegate;
        Everyplay.UploadDidStart += everyplayUploadDidStartDelegate;
        Everyplay.WasClosed += everyplayWasClosedDelegate;


#endif


    }

    void OnDisable() {


#if BROADCAST_USE_EVERYPLAY
        // ------------
        // EVERPLAY

        Everyplay.RecordingStarted -= everyplayRecordingStartedDelegate;
        Everyplay.RecordingStopped -= everyplayRecordingStoppedDelegate;
        //Everyplay.ThumbnailReadyAtFilePath -= everyplayThumbnailReadyAtFilePathDelegate;

        Everyplay.ReadyForRecording -= everyplayReadyForRecordingDelegate;
        Everyplay.UploadDidComplete -= everyplayUploadDidCompleteDelegate;
        Everyplay.UploadDidProgress -= everyplayUploadDidProgressDelegate;
        Everyplay.UploadDidStart -= everyplayUploadDidStartDelegate;
        Everyplay.WasClosed -= everyplayWasClosedDelegate;

#endif

    }

    public void Init() {

#if BROADCAST_USE_EVERYPLAY
        Invoke("everyplayInit", 1);
#endif
    }

    // MESSAGES 

    public void BroadcastRecordingStart() {
        BroadcastRecordingEvents(BroadcastNetworksMessages.broadcastRecordingStart);
    }

    public void BroadcastRecordingStop() {
        BroadcastRecordingEvents(BroadcastNetworksMessages.broadcastRecordingStop);
    }

    public void BroadcastRecordingPlayback() {
        BroadcastRecordingEvents(BroadcastNetworksMessages.broadcastRecordingPlayback);
    }

    // MESSAGE STATE STATUS

    public void BroadcastRecordingStatusChanged(string status) {
        Messenger<string>.Broadcast(BroadcastNetworksMessages.broadcastRecordingStatusChanged, status);
    }

    public void BroadcastRecordingEvents(string code) {
        Messenger.Broadcast(code);
        BroadcastRecordingStatusChanged(code);
    }

    // FACECAM MESSAGES

    public void BroadcastFacecamStart() {
        BroadcastMessage(BroadcastNetworksMessages.broadcastFacecamStart);
    }

    public void BroadcastFacecamStop() {
        BroadcastMessage(BroadcastNetworksMessages.broadcastFacecamStop);
    }

#if BROADCAST_USE_EVERYPLAY
    // ----------------------------------------------------------------------
    // EVERPLAY - https://developers.everyplay.com/doc/Everyplay-integration-to-Unity3d-game

    bool facecamPermissionGranted = false;

    public void everyplayInit() {

        if(broadcastEveryplay == null) {

            broadcastEveryplay = FindObjectOfType(typeof(Everyplay)) as Everyplay;

            if(!broadcastEveryplay) {
                var obj = new GameObject("_BroadcastEveryplay");
                broadcastEveryplay = obj.AddComponent<Everyplay>();
                DontDestroyOnLoad(broadcastEveryplay);

                if(IsSupported()) {
                    // Subscribe to the permission events
                    Everyplay.FaceCamRecordingPermission += everyplayCheckForRecordingPermission;
                }
            }
        }

        //broadcastEveryplay.clientId = AppConfigs.broadcastEveryplayClientId;
        //Everyplay.broadcastEveryplay.clientSecret = AppConfigs.broadcastEveryplayClientSecret;
        //broadcastEveryplay.redirectURI = AppConfigs.broadcastEveryplayAuthUrl;

    }

    // Method to listen for permissions

    private void everyplayCheckForRecordingPermission(bool granted) {
        if(granted) {
            facecamPermissionGranted = granted;

            Debug.Log("Microphone access was granted:" + " facecamPermissionGranted:" + facecamPermissionGranted);
            // * HERE YOU CAN START YOUR FACECAM SAFELY * //
            FacecamStart();
        }
        else {
            Debug.Log("Microphone access was DENIED");
        }
    }

    // RECORDING

    public bool everyplayIsRecordingSupported() {
        LogUtil.Log("everyplayIsRecordingSupported");
        return Everyplay.IsRecordingSupported();
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
        LogUtil.Log("Thumbnail ready: " + path);

        recordingThumbnailPath = path;
    }

    public void everyplayReadyForRecordingDelegate(bool isSupported) {
        Debug.Log("everyplayReadyForRecordingDelegate: isSupported:" + isSupported);

    }

    public void everyplayUploadDidCompleteDelegate(int videoId) {
        LogUtil.Log("everyplayUploadDidCompleteDelegate: videoId:", videoId);

    }

    public void everyplayUploadDidProgressDelegate(int videoId, float progress) {
        LogUtil.Log("everyplayUploadDidProgressDelegate: videoId:", videoId + " progress:" + progress);

    }

    public void everyplayUploadDidStartDelegate(int videoId) {
        LogUtil.Log("everyplayUploadDidStartDelegate: videoId:", videoId);

    }

    public void everyplayWasClosedDelegate() {
        LogUtil.Log("everyplayWasClosedDelegate");

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

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.Show();
#else
#endif
    }

    // OPEN MODAL

    public static void OpenSharing() {
        if (Instance != null) {
            Instance.openSharing();
        }
    }

    public void openSharing() {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.ShowSharingModal();
#else
#endif
    }


    // METADATA

    public static void SetMetadata(string key, object val) {
        if (Instance != null) {
            Instance.setMetadata(key, val);
        }
    }

    public void setMetadata(string key, object val) {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.SetMetadata(key, val);
#else
#endif
    }

    public static void SetMetadata(string key, Dictionary<string, object> values) {
        if (Instance != null) {
            Instance.setMetadata(key, values);
        }
    }

    public void setMetadata(string key, Dictionary<string, object> values) {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.SetMetadata(key, values);
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
        return everyplayIsRecordingSupported();
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

    // TOGGLE RECORDING

    public static void ToggleRecording() {
        if (Instance != null) {
            Instance.toggleRecording();
        }
    }

    public void toggleRecording() {
        if (!IsSupported() || !IsRecordingSupported()) {
            return;
        }

        if (IsRecording()) {
            StopRecording();
        }
        else {
            StartRecording();
        }
    }

    // START RECORDING

    public static void StartRecording() {
        if (Instance != null) {
            Instance.startRecording();
        }
    }

    public void startRecording() {
        if (!IsSupported() || !IsRecordingSupported()) {
            return;
        }

        if (!IsRecording()) {
            SetMaxRecordingMinutesLength(10);

            SetMetadata("about", Locos.Get(LocoKeys.social_everyplay_game_explore_message));
            SetMetadata("game", Locos.Get(LocoKeys.app_display_name));
            SetMetadata("level", Locos.Get(LocoKeys.game_type_arcade));
            SetMetadata("level_name", Locos.Get(LocoKeys.game_type_arcade_mode));

            if (FPSDisplay.isUnder25FPS) {
                SetLowMemoryDevice(true);
            }

#if BROADCAST_USE_EVERYPLAY
            Everyplay.StartRecording();
#endif

            BroadcastRecordingStart();
        }
    }

    // STOP RECORDING

    public static void StopRecording() {
        if (Instance != null) {
            Instance.stopRecording();
        }
    }

    public void stopRecording() {
        if (!IsSupported()) {
            return;
        }

        if (IsRecording()) {

#if BROADCAST_USE_EVERYPLAY
            Everyplay.StopRecording();
#endif

            BroadcastRecordingStop();
        }
    }

    // RESUME RECORDING

    public static void ResumeRecording() {
        if (Instance != null) {
            Instance.resumeRecording();
        }
    }

    public void resumeRecording() {
        if (!IsSupported()) {
            return;
        }

        if (IsRecording() && IsPaused()) {

#if BROADCAST_USE_EVERYPLAY
            Everyplay.ResumeRecording();
#endif

            BroadcastRecordingStart();
        }
    }

    // PAUSE RECORDING

    public static void PauseRecording() {
        if (Instance != null) {
            Instance.pauseRecording();
        }
    }

    public void pauseRecording() {
        if (!IsSupported()) {
            return;
        }

        if (IsRecording()) {

#if BROADCAST_USE_EVERYPLAY
            Everyplay.PauseRecording();
#endif
        }

        BroadcastRecordingStop();
    }

    // PLAY LAST RECORDING

    public static void PlayLastRecording() {
        if (Instance != null) {
            Instance.playLastRecording();
        }
    }

    public void playLastRecording() {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.PlayLastRecording();
#else
#endif

        BroadcastRecordingStop();
    }

    // TAKE THUMBNAIL

    public static void TakeThumbnail() {
        if (Instance != null) {
            Instance.takeThumbnail();
        }
    }

    public void takeThumbnail() {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.TakeThumbnail();
#else
#endif
    }

    // PERFORMANCE

    public static void SetLowMemoryDevice(bool isLowMemory) {
        if (Instance != null) {
            Instance.setLowMemoryDevice(isLowMemory);
        }
    }

    public void setLowMemoryDevice(bool isLowMemory) {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.SetLowMemoryDevice(isLowMemory);
#else
#endif
    }

    // PERFORMANCE

    public static void SetDisableSingleCoreDevices(bool isLowMemory) {
        if (Instance != null) {
            Instance.setDisableSingleCoreDevices(isLowMemory);
        }
    }

    public void setDisableSingleCoreDevices(bool isLowMemory) {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.SetDisableSingleCoreDevices(isLowMemory);
#else
#endif
    }

    // PERFORMANCE

    public static void SetMaxRecordingMinutesLength(int maxlength) {
        if (Instance != null) {
            Instance.setMaxRecordingMinutesLength(maxlength);
        }
    }

    public void setMaxRecordingMinutesLength(int maxlength) {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.SetMaxRecordingMinutesLength(maxlength);
#else
#endif
    }

    // THUMBNAIL

#if BROADCAST_USE_EVERYPLAY_2
    public static void LoadThumbnailFromFilePath(
        string path, 
        Everyplay.ThumbnailLoadReadyDelegate thumbnailLoadReadyDelegate, 
        Everyplay.ThumbnailLoadFailedDelegate thumbnailLoadFailedDelegate) {

        if (Instance != null) {
            Instance.loadThumbnailFromFilePath(
                path, 
                thumbnailLoadReadyDelegate, 
                thumbnailLoadFailedDelegate);
        }
    }
    
    public void loadThumbnailFromFilePath(
        string path, 
        Everyplay.ThumbnailLoadReadyDelegate thumbnailLoadReadyDelegate, 
        Everyplay.ThumbnailLoadFailedDelegate thumbnailLoadFailedDelegate) {
        
        if (!IsSupported()) {
            return;
        }

        Everyplay.LoadThumbnailFromFilePath(
            path, 
            thumbnailLoadReadyDelegate, 
            thumbnailLoadFailedDelegate);
    }
#else
#endif

    public static void SetThumbnailWidth(int thumbnailWidth) {

        if (Instance != null) {
            Instance.setThumbnailWidth(thumbnailWidth);
        }
    }

    public void setThumbnailWidth(int thumbnailWidth) {

        if (!IsSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY2
        Everyplay.SetThumbnailWidth(thumbnailWidth);
#else
#endif
    }

    // The filepath we're getting from the thumbnail event
    public string recordingThumbnailPath;

    /* Delegate for event (see section on getting events) */
    public void recordingThumbnailReadyAtFilePathDelegate(string filePath) {
        recordingThumbnailPath = filePath;
    }

    /* Define delegate methods */
    // Success delegate
    public void recordingThumbnailSuccess(Texture2D texture) {
        // Yay, we have a video thumbnail, now we present it to the user
    }

    // Error delegate
    public void recordingThumbnailError(string error) {
        // Oh noes, something went wrong
        Debug.Log("Thumbnail loading failed: " + error);
    }

    // Our own method that is used when the game is in a proper session to load and show the thumbnail
    public void recordingShowThumbnailToTheUserInTheUI() {
#if BROADCAST_USE_EVERYPLAY
        // Load the thumbnail, using our delegates as parameter
        //LoadThumbnailFromFilePath(recordingThumbnailPath, recordingThumbnailSuccess, recordingThumbnailError);
#endif
    }

    /*
     * 
     * Everyplay.SetMetadata("level", levelNumber);
Everyplay.SetMetadata("level_name", levelName);
Everyplay.SetMetadata("score", score)
*/

    // -------------------
    // FACECAM


    // IS FACECAM SESSION RUNNING

    public static bool IsFacecamSessionRunning() {
        if (Instance != null) {
            return Instance.isFacecamSessionRunning();
        }
        return false;
    }

    public bool isFacecamSessionRunning() {

#if BROADCAST_USE_EVERYPLAY
        return Everyplay.FaceCamIsSessionRunning();
#else
        return false;
#endif
    }

    // IS FACECAM SESSION RUNNING

    public static bool IsFacecamAudioRecordingSupported() {
        if (Instance != null) {
            return Instance.isFacecamAudioRecordingSupported();
        }
        return false;
    }

    public bool isFacecamAudioRecordingSupported() {

#if BROADCAST_USE_EVERYPLAY
        return Everyplay.FaceCamIsAudioRecordingSupported();
#else
        return false;
#endif
    }

    // IS FACECAM HEADPHONES PLUGGED IN

    public static bool IsFacecamHeadphonesPluggedIn() {
        if (Instance != null) {
            return Instance.isFacecamHeadphonesPluggedIn();
        }
        return false;
    }

    public bool isFacecamHeadphonesPluggedIn() {

#if BROADCAST_USE_EVERYPLAY
        return Everyplay.FaceCamIsHeadphonesPluggedIn();
#else
        return false;
#endif
    }

    // IS FACECAM RECORDING PERMISSION GRANTED

    public static bool IsFacecamRecordingPermissionGranted() {
        if (Instance != null) {
            return Instance.isFacecamRecordingPermissionGranted();
        }
        return false;
    }

    public bool isFacecamRecordingPermissionGranted() {

#if BROADCAST_USE_EVERYPLAY
        return Everyplay.FaceCamIsRecordingPermissionGranted();
#else
        return false;
#endif
    }

    // IS FACECAM VIDEO RECORDING SUPPORTED 

    public static bool IsFacecamVideoRecordingSupported() {
        if (Instance != null) {
            return Instance.isFacecamVideoRecordingSupported();
        }
        return false;
    }

    public bool isFacecamVideoRecordingSupported() {

#if BROADCAST_USE_EVERYPLAY
        return Everyplay.FaceCamIsVideoRecordingSupported();
#else
        return false;
#endif
    }

    // FACECAM START SESSION

    public static void FacecamToggle() {
        if (Instance != null) {
            Instance.facecamToggle();
        }
    }

    public void facecamToggle() {

        if (!IsSupported() || !IsFacecamVideoRecordingSupported()) {
            return;
        }

        if (IsFacecamSessionRunning()) {
            FacecamStop();
        }
        else {
            FacecamStart();
        }
    }

    // FACECAM PERMISSION

    public static void FacecamGetPermission() {
        if (Instance != null) {
            Instance.facecamGetPermission();
        }
    }

    public void facecamGetPermission() {
        if (!IsSupported() || !IsFacecamVideoRecordingSupported()) {
            return;
        }

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamRequestRecordingPermission();
#else

#endif
    }


    // FACECAM START SESSION

    public static void FacecamStart() {
        if (Instance != null) {
            Instance.facecamStart();
        }
    }

    public void facecamStart() {

        if (!IsFacecamRecordingPermissionGranted()) {
            FacecamGetPermission();
        }
        else {

#if BROADCAST_USE_EVERYPLAY
            Everyplay.FaceCamStartSession();
#else

#endif
        }
    }

    // FACECAM STOP SESSION

    public static void FacecamStop() {
        if (Instance != null) {
            Instance.facecamStop();
        }
    }

    public void facecamStop() {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamStopSession();
#else

#endif
    }

    // FACECAM SETTINGS

    public static void FaceCamSetAudioOnly(bool val) {
        if (Instance != null) {
            Instance.faceCamSetAudioOnly(val);
        }
    }

    public void faceCamSetAudioOnly(bool val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetAudioOnly(val);
#else

#endif
    }

    //

    public static void FaceCamSetMonitorAudioLevels(bool val) {
        if (Instance != null) {
            Instance.faceCamSetMonitorAudioLevels(val);
        }
    }

    public void faceCamSetMonitorAudioLevels(bool val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetMonitorAudioLevels(val);
#else

#endif
    }

    //

    public static void FaceCamSetPreviewBorderColor(float r, float g, float b, float a) {
        if (Instance != null) {
            Instance.faceCamSetPreviewBorderColor(r, g, b, a);
        }
    }

    public void faceCamSetPreviewBorderColor(float r, float g, float b, float a) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewBorderColor(r, g, b, a);
#else

#endif
    }

    //

    public static void FaceCamSetPreviewBorderWidth(int val) {
        if (Instance != null) {
            Instance.faceCamSetPreviewBorderWidth(val);
        }
    }

    public void faceCamSetPreviewBorderWidth(int val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewBorderWidth(val);
#else

#endif
    }

    //

#if BROADCAST_USE_EVERYPLAY
    public static void FaceCamSetPreviewOrigin(Everyplay.FaceCamPreviewOrigin val) {
        if(Instance != null) {
            Instance.faceCamSetPreviewOrigin(val);
        }
    }
#endif

#if BROADCAST_USE_EVERYPLAY
    public void faceCamSetPreviewOrigin(Everyplay.FaceCamPreviewOrigin val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewOrigin(val);
#else
#endif

    }
#endif

    //

    public static void FaceCamSetPreviewPositionX(int val) {
        if (Instance != null) {
            Instance.faceCamSetPreviewPositionX(val);
        }
    }

    public void faceCamSetPreviewPositionX(int val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewPositionX(val);
#else

#endif
    }

    //

    public static void FaceCamSetPreviewPositionY(int val) {
        if (Instance != null) {
            Instance.faceCamSetPreviewPositionY(val);
        }
    }

    public void faceCamSetPreviewPositionY(int val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewPositionY(val);
#else

#endif
    }

    //

    public static void FaceCamSetPreviewScaleRetina(bool val) {
        if (Instance != null) {
            Instance.faceCamSetPreviewScaleRetina(val);
        }
    }

    public void faceCamSetPreviewScaleRetina(bool val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewScaleRetina(val);
#else

#endif
    }

    //

    public static void FaceCamSetPreviewSideWidth(int val) {
        if (Instance != null) {
            Instance.faceCamSetPreviewSideWidth(val);
        }
    }

    public void faceCamSetPreviewSideWidth(int val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewSideWidth(val);
#else

#endif
    }

    //

    public static void FaceCamSetPreviewVisible(bool val) {
        if (Instance != null) {
            Instance.faceCamSetPreviewVisible(val);
        }
    }

    public void faceCamSetPreviewVisible(bool val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetPreviewVisible(val);
#else

#endif
    }

    //

    public static void FaceCamSetTargetTexture(Texture2D val) {
        if (Instance != null) {
            Instance.faceCamSetTargetTexture(val);
        }
    }

    public void faceCamSetTargetTexture(Texture2D val) {

#if BROADCAST_USE_EVERYPLAY
        Everyplay.FaceCamSetTargetTexture(val);
#else

#endif
    }

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

    public void Update() {
        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.B)) {


                if (Input.GetKeyDown(KeyCode.I)) {
                    // start

                    Debug.Log("BroadcastNetworks:" + " test start:");

                    BroadcastRecordingStart();
                }
                else if (Input.GetKeyDown(KeyCode.O)) {
                    // start

                    Debug.Log("BroadcastNetworks:" + " test stop:");

                    BroadcastRecordingStop();
                }

            }
        }
    }
}