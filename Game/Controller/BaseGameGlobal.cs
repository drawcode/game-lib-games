#define DEV

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;


public class BaseGameGlobal : MonoBehaviour
{
    public static BaseGameGlobal BaseInstance;
    
    public GameNetworks gameNetworks;
    public GameState state;
    public Contents contents;
    
    //public GameSettings gameSettings;
    //public Recorders raceRecorders;
    
    public SocialNetworks socialNetworks;
    public ProductPurchase productPurchase;
    
    public GameScreenScaler gameScreenScaler;
    //public GameMatchup matchup;
    public GameSocialGame socialGame;
    public AudioSystem audioSystem; 
    
    public Gameverses.GameNetworking networking;
    public Gameverses.GameversesGameObject gameversesGameObject;
    
    public AudioRecordObject audioRecorder; 
    
    //public Gameverses.GameversesAPI gameversesAPI;
    
    public bool ENABLE_PRODUCT_UNLOCKS = true;
    
    public string currentLevel = "Level1";
    
    public string masterserverPrefix = "drawlabs_";
    
    public virtual void Awake() {
        
        if (BaseInstance != null && this != BaseInstance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
        
        BaseInstance = this;
        
        DontDestroyOnLoad(gameObject);
        
        Init();     
    }
    
    public static bool isReady {
        get { return GameGlobal.Instance != null ? true : false; }
    }
    
    public virtual void OnEnable() {
        //Messenger<object>.AddListener(ContentMessages.ContentSyncShipContentSuccess, OnContentSyncShipContentSuccess);
    }
    
    public virtual void OnDisable() {
        //Messenger<object>.RemoveListener(ContentMessages.ContentSyncShipContentSuccess, OnContentSyncShipContentSuccess);
    }
    
    public virtual void OnContentSyncShipContentSuccess(object obj) {
        //InitContentSystemPost();
    }
    
    public virtual void Init() {        
        gameObject.AddComponent<InputSystem>();
    }
    
    public virtual void InitContentSystemPost() {
        
        InitAudio();            
        
        InitNetwork();
        
        InitTracking();
        
        InitContext();
        
        InitUIFlow();
        
        InitState();
        
        InitRecorders();
        
        InitMatchupSettings();
        
        InitSocial();   
        
        // InitAds();              
        
        InitMovie();
        
        InitPurchase();         
        
        InitPlayerProgress();
        
        InitRaceSettings();
        
        PrepareData();
        
        /*
        foreach(GameLoaderMeta meta in GameLoaderMetas.Instance.GetAll()) {
        //meta.tracker.stats.Add();
        
                string json = JsonMapper.ToJson(meta);//<GameLoaderMeta>
                
                LogUtil.Log("------>>>  GameLoaderMeta json:" + json);
                
        }*/
        //LoadLevel();
    }
    
    public virtual void Start() {
        
    }   
    
    public virtual void PrepareData() {
        
        GameWorld world = new GameWorld();
        world.active = true;
        world.SetAttributeStringValue("default", "default");;
        world.code = "";
        world.description = "";
        world.display_name = "";
        world.game_id = "";
        world.key = "";
        world.name = "";
        world.order_by = "";
        //world.pack.Add("default");
        world.sort_order = 0;
        world.sort_order_type = 0;
        world.status = "";
        world.type = "default";
        world.uuid = UniqueUtil.Instance.CreateUUID4();
        //world.
        
        Debug.Log(JsonMapper.ToJson(world));
    }
    
    public virtual void InitNetwork() { 
        gameObject.AddComponent<WebRequests>(); 
    }
    
    public virtual void InitContext() { 
        //gameScreenScaler = gameObject.AddComponent<GameScreenScaler>();   
        
    }   
    
    public virtual IEnumerator InitContentCo() {
        Debug.Log("Starting Contents");
        contents = gameObject.AddComponent<Contents>();
        
        ContentsConfig.contentRootFolder = "drawlabs";
        ContentsConfig.contentAppFolder = "game-drawlabs-brainball";
        ContentsConfig.contentDefaultPackFolder = "game-drawlabs-brainball-1";
        ContentsConfig.contentVersion = "1.0";
        ContentsConfig.contentIncrement = 2;
        yield return StartCoroutine(contents.initCacheCo(true, false));       
        
        InitContentSystemPost();  
    }
    
    public virtual void InitTracking() {
        //TestFlight.TakeOff("e5f209d3e74acce80d1a2907cafcbc41_NDgyNDIyMDExLTEyLTE2IDE4OjI4OjUzLjY4NTc5Nw");
    }
    
    public virtual void InitUIFlow() {      
        //uiFlow = gameObject.AddComponent<GameUIFlow>();   
        //uiFlow.InitUIFlow();
    }
    
    public virtual void InitRecorders() {       
        // TODO wire up to turn off and on on PC and mobile
        //raceRecorders = Recorders.Instance;
        audioRecorder = gameObject.AddComponent<AudioRecordObject>();   
        
    }
    
    public virtual void InitMatchupSettings() { 
        MatchupServerSettings.IP = "matchup.drawlabs.com";
        MatchupServerSettings.players = 4;
        MatchupServerSettings.port = 25010;
        //MatchupServerSettings.
        //socialGame = gameObject.AddComponent<GameSocialGame>();           
        //networking = gameObject.AddComponent<Gameverses.GameNetworking>();
    }
    
    public virtual void InitPurchase() {    
        productPurchase = gameObject.AddComponent<ProductPurchase>();   
        productPurchase.EnableProductUnlocks = ENABLE_PRODUCT_UNLOCKS;
    }
    
    public virtual void InitRaceSettings() {
        //raceSettings = ScriptableObject.CreateInstance<RaceSettings>();           
        LogUtil.Log("GameGlobal InitRaceSettings init...");
    }
    
    public virtual void InitState() {
        
        try {
            state = GameState.Instance;
            LogUtil.Log("GameGlobal InitState init...");
            
            //gameversesGameObject = gameObject.AddComponent<Gameverses.GameversesGameObject>();
            //socialGame = gameObject.AddComponent<GameSocialGame>();           
        }
        catch (Exception e) {
            LogUtil.Log("GameGlobal could not be initialized..." + e.Message + e.StackTrace);
        }
    }
    
    public virtual void InitPlayerProgress() {
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled) {
            #if UNITY_IPHONE            
            gameNetworks = gameObject.AddComponent<GameNetworks>();         
            gameNetworks.loadNetwork(GameNetworkType.IOS_GAME_CENTER);
            #endif
        }
    }
    
    public virtual void InitMovie() {
        #if UNITY_EDITOR        
        #elif UNITY_IPHONE
        //GameVideo.Play();
        #elif UNITY_ANDROID
        //GameVideo.Play();
        #else
        #endif
    }

    public static float volumeEditor = 1f;
    
    public virtual void InitAudio() {    

        /*
        GameAudioEffects.audio_loop_intro_1 = "ui-music-intro";
        GameAudioEffects.audio_loop_main_1 = "ui-music-loop";
        GameAudioEffects.audio_loop_game_1 = "music-level-1";
        GameAudioEffects.audio_loop_game_2 = "music-level-2";
        GameAudioEffects.audio_loop_game_3 = "music-level-3";
        GameAudioEffects.audio_loop_game_4 = "music-quiz";
        */
        audioSystem = gameObject.AddComponent<AudioSystem>();   
        
        audioSystem.PrepareIntroFileFromResources(GameAudioEffects.audio_loop_intro_1, false, (float)GameProfiles.Current.GetAudioMusicVolume());
        audioSystem.PrepareLoopFileFromResources(GameAudioEffects.audio_loop_main_1, true, (float)GameProfiles.Current.GetAudioMusicVolume());
        
        audioSystem.PrepareGameLapLoopFileFromResources(0, GameAudioEffects.audio_loop_game_1, true, (float)GameProfiles.Current.GetAudioMusicVolume());
        audioSystem.PrepareGameLapLoopFileFromResources(1, GameAudioEffects.audio_loop_game_2, true, (float)GameProfiles.Current.GetAudioMusicVolume());
        audioSystem.PrepareGameLapLoopFileFromResources(2, GameAudioEffects.audio_loop_game_3, true, (float)GameProfiles.Current.GetAudioMusicVolume());
        audioSystem.PrepareGameLapLoopFileFromResources(3, GameAudioEffects.audio_loop_game_4, true, (float)GameProfiles.Current.GetAudioMusicVolume());
        
        audioSystem.SetAmbienceVolume(GameProfiles.Current.GetAudioMusicVolume());
        audioSystem.SetEffectsVolume(GameProfiles.Current.GetAudioEffectsVolume());
        
        
        //audioSystem.StartAmbience();
        #if DEV2
        if(Application.isEditor) {

            GameProfiles.Current.SetAudioMusicVolume(volumeEditor);
            GameProfiles.Current.SetAudioEffectsVolume(volumeEditor);          
            audioSystem.SetAmbienceVolume(volumeEditor);
            audioSystem.SetEffectsVolume(volumeEditor);
            GameAudio.SetEffectsVolume(volumeEditor);
            GameAudio.SetAmbienceVolume(volumeEditor);
        }
        #endif
        
        LogUtil.Log("GameGlobal InitAudio init...");
    }
    
    public virtual void InitSocial() {
        socialNetworks = gameObject.AddComponent<SocialNetworks>();
        socialNetworks.loadSocialLibs();
        
        LogUtil.Log("GameGlobal InitSocial init...");
    }
    /*
    public IEnumerator InitGeolocation() 
    {
#if UNITY_IPHONE
        bool currentlyPollingLocation = true;
        // Start service before querying location
        TouchScreenKeyboard.StartLocationServiceUpdates();

        // Wait until service initializes 
        int maxWait = 20;
        while(iPhoneSettings.locationServiceStatus ==
            LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if(maxWait < 1) {
            print("Timed out");
            currentlyPollingLocation = false;
        }

        // User denied access to device location 
        if(iPhoneSettings.locationServiceStatus == LocationServiceStatus.Failed) {
            print("User denied access to device location");
            currentlyPollingLocation = false;
        }
        // Access granted and location value could be retrieved
        else {
            if(currentlyPollingLocation) {
                print("Location: " + Input.location.lastData.latitude + " " +
                        Input.location.lastData.longitude + " " +
                        Input.location.lastData.altitude + " " +
                        Input.location.lastData.horizontalAccuracy + " " +
                        Input.location.lastData.timestamp);

                if(Input.location.lastData.latitude != 0) {

                    //clientApi.adClient.locationData.latitude = iPhoneInput.lastLocation.latitude;
                    //clientApi.adClient.locationData.longitude = iPhoneInput.lastLocation.longitude;
                    //clientApi.adClient.locationData.altitude = iPhoneInput.lastLocation.altitude;
                    //clientApi.adClient.locationData.horizontalAccuracy = iPhoneInput.lastLocation.horizontalAccuracy;
                    //clientApi.adClient.locationData.verticalAccuracy = iPhoneInput.lastLocation.verticalAccuracy;
                    //clientApi.adClient.locationData.timestamp = iPhoneInput.lastLocation.timestamp;

                    currentlyPollingLocation = false;
                }
            }
        }

        // Stop service if there is no need to query location updates continuously
        iPhoneSettings.StopLocationServiceUpdates();    
#else
        yield break;
#endif
    }
    */
    
    //void OnEnable() {
    //Messenger<string>.AddListener(AlertDialogMessages.DIALOG_QUIT, OnQuitDialog);
    //}
    
    //void OnDisable() {
    //Messenger<string>.RemoveListener(AlertDialogMessages.DIALOG_QUIT, OnQuitDialog);
    //}
    
    public virtual void OnQuitDialog(string result) {
        if(result == AlertDialogResultMessages.DIALOG_RESULT_YES) {
            QuitGame(); 
        }
        else { 
            if(AlertDialog.Instance.IsReady()) {
                AlertDialog.Instance.HideAlert();
            }           
        }
    }
    
    public virtual void QuitGame() {
        Application.Quit(); 
    }
    
    int screenshotCount = 1;
    
    public virtual void Update() {
        #if !UNITY_IPHONE       
        if (Input.GetKeyDown(KeyCode.Escape)) { 
            QuitGame();
            //if(AlertDialog.Instance.IsReady()) {
            //  AlertDialog.Instance.ShowAlertQuitDialog();
            //}
        }
        #endif
        
        if(Application.isEditor) {
            if(Input.GetKeyDown(KeyCode.Comma)) {
                string filename = "../screenshots/" + GameUIController.Instance.currentPanel +
                    "-" + Screen.width.ToString() + "x" + Screen.height.ToString()
                        + "-" + (screenshotCount++).ToString() + ".png";
                
                if(GameController.IsGameRunning) {
                    filename = "../screenshots/" + GameUIController.Instance.currentPanel + "-gameplay-" +
                        "-" + Screen.width.ToString() + "x" + Screen.height.ToString()
                            + "-" + (screenshotCount++).ToString() + ".png";
                }
                Application.CaptureScreenshot(filename);
            }
        }
    }
}