#define DEV
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_GAME_LIB_GAMEVERSES
using Gameverses;
#endif
using Engine.Game.App.BaseApp;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;
using Engine.Content;
using Engine.Game.App;
using Engine.Audio;

public enum AppRunState {
    DEV,
    LIVE
}

public class BaseGameGlobal : GameObjectBehavior {
    public GameNetworks gameNetworks;
    public GameState state;
    public Contents contents;

#if DEV
    public static AppRunState appRunState = AppRunState.DEV;
#else
    public static AppRunState appRunState = AppRunState.LIVE;
#endif

    //public GameSettings gameSettings;
    //public Recorders raceRecorders;

    public AdNetworks adNetworks;
    public SocialNetworks socialNetworks;
    public ProductNetworks productPurchase;
    public GameScreenScaler gameScreenScaler;
    public string appDisplayCode;
    public string appDisplayName;
    //public GameMatchup matchup;
    //public GameSocialGame socialGame;
    //public AudioSystem audioSystem;
#if USE_GAME_LIB_GAMEVERSES && ENABLE_FEATURE_NETWORKING
    public GameNetworking networking;
    public GameversesGameObject gameversesGameObject;
#endif
    public AudioRecordObject audioRecorder;

#if USE_GAME_LIB_GAMEVERSES
    public GameCommunityController platformController;
    public GameCommunityService platformService;
    public GameCommunityUIController platformAppViewerUIController;
    public GameCommunitySocialController platformSocialController;
    public GameCommunityTrackingController platformTrackingController;

    //public Gameverses.GameversesAPI gameversesAPI;

#endif

    public bool ENABLE_PRODUCT_UNLOCKS = true;
    public string currentLevel = "Level1";
    public string masterserverPrefix = "game_";


    public double currentVolumeEffects = 1;
    public double currentVolumeMusic = 1;
    public double currentVolumeInc = .1;

    public virtual void Awake() {
        Init();

        Application.targetFrameRate = 60;
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

        /*
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
        
        PrepareData();
        */

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

        /*
        GameWorld world = new GameWorld();
        world.active = true;
        world.SetAttributeStringValue("default", "default");
        ;
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
        world.uuid = UniqueUtil.CreateUUID4();
        //world.
        */

        //LogUtil.Log(JsonMapper.ToJson(world));
    }

    public virtual void InitNetwork() {
        gameObject.AddComponent<WebRequests>();
    }

    public virtual void InitContext() {
        //gameScreenScaler = gameObject.AddComponent<GameScreenScaler>();   

    }

    public virtual IEnumerator InitContentCo() {
        /*
        LogUtil.Log("Starting Contents");
        contents = gameObject.AddComponent<Contents>();
        
        ContentsConfig.contentRootFolder = "test";
        ContentsConfig.contentAppFolder = "game-test";
        ContentsConfig.contentDefaultPackFolder = "game-test-1";
        ContentsConfig.contentVersion = "1.0";
        ContentsConfig.contentIncrement = 2;

        yield return StartCoroutine(contents.initCacheCo(true, false));       
        
        InitContentSystemPost();  
        */

        yield return new WaitForEndOfFrame();
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

    public virtual void InitAds() {
        adNetworks = gameObject.AddComponent<AdNetworks>();
    }

    public virtual void InitGameverses() {

#if ENABLE_FEATURE_NETWORKING
        gameversesGameObject = gameObject.AddComponent<GameversesGameObject>();
        networking = gameversesGameObject.gameNetworking;
#endif
    }

    public virtual void InitMatchupSettings() {
        //MatchupServerSettings.IP = "matchup.test.com";
        //MatchupServerSettings.players = 4;
        //MatchupServerSettings.port = 25010;
        //MatchupServerSettings.
        //socialGame = gameObject.AddComponent<GameSocialGame>();           
        //networking = gameObject.AddComponent<Gameverses.GameNetworking>();
    }

    public virtual void InitLocalization() {
        string appDisplayCode = Locos.GetString(LocoKeys.app_display_code);
        string appDisplayName = Locos.GetString(LocoKeys.app_display_name);
        appDisplayCode = Locos.GetString(LocoKeys.app_display_code);

        Debug.Log("InitLocalization:" + " appDisplayCode:" + appDisplayCode);
        Debug.Log("InitLocalization:" + " appDisplayName:" + appDisplayName);

    }

    public virtual void InitPurchase() {
        ProductNetworks.Init();

        productPurchase = ProductNetworks.instance;//gameObject.AddComponent<ProductPurchase>();   
        productPurchase.EnableProductUnlocks = ENABLE_PRODUCT_UNLOCKS;
    }

    public virtual void InitState() {

        try {
            state = GameState.Instance;
            LogUtil.Log("GameGlobal InitState init...");

            //GameWorlds.Instance.LoadState();

            //gameversesGameObject = gameObject.AddComponent<Gameverses.GameversesGameObject>();
            //socialGame = gameObject.AddComponent<GameSocialGame>();           
        }
        catch(Exception e) {
            LogUtil.Log("GameGlobal could not be initialized..." + e.Message + e.StackTrace);
        }
    }

    public virtual void InitPlayerProgress() {
        gameNetworks = gameObject.AddComponent<GameNetworks>();
        if(GameNetworks.gameNetworkiOSAppleGameCenterEnabled && Context.Current.isMobileiOS) {
            gameNetworks.loadNetwork(GameNetworkType.gameNetworkAppleGameCenter);
        }
        if(GameNetworks.gameNetworkAndroidGooglePlayEnabled && Context.Current.isMobile) {
            gameNetworks.loadNetwork(GameNetworkType.gameNetworkGooglePlayServices);
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

    public virtual void UpdateAudio(double volumeMusic, double volumeEffects) {

        GameProfiles.Current.SetAudioMusicVolume(volumeMusic);
        GameProfiles.Current.SetAudioEffectsVolume(volumeEffects);

        GameAudio.SetAmbienceVolume(volumeMusic);
        GameAudio.SetEffectsVolume(volumeEffects);

        AudioListener.volume = (float)volumeEffects;

        AudioSystem.Instance.SetAmbienceVolume(volumeMusic);
        AudioSystem.Instance.SetEffectsVolume(volumeEffects);

        //GameAudioController.SetVolumeGame(volumeMusic);
        //GameAudioController.SetVolumeUI(volumeMusic);

        GameState.SaveProfile();
    }

    public virtual void InitAudio() {

        currentVolumeEffects = GameProfiles.Current.GetAudioEffectsVolume();
        currentVolumeMusic = GameProfiles.Current.GetAudioMusicVolume();

        if(!Application.isEditor) {
            UpdateAudio(currentVolumeEffects, currentVolumeMusic);
        }

#if DEV
        //if (Application.isEditor) {
        //    UpdateAudio(GameGlobal.volumeEditorMusic, GameGlobal.volumeEditorEffects);
        //
        //}
#endif

        LogUtil.Log("GameGlobal InitAudio init...");
    }

    public void InitCommunity() {

#if USE_GAME_LIB_GAMEVERSES
        platformService = GameCommunityService.Instance;//gameObject.AddComponent<GameCommunityService>();
        platformController = gameObject.AddComponent<GameCommunityController>();
        platformAppViewerUIController = gameObject.AddComponent<GameCommunityUIController>();
        platformSocialController = gameObject.AddComponent<GameCommunitySocialController>();
        platformTrackingController = gameObject.AddComponent<GameCommunityTrackingController>();

        Messenger.Broadcast(GameCommunityMessages.gameCommunityReady);
#endif
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

#if USE_GAME_LIB_GAMES_UI
        if(result == AlertDialogResultMessages.DIALOG_RESULT_YES) {
            QuitGame();
        }
        else {
            if(AlertDialog.Instance.IsReady()) {
                AlertDialog.Instance.HideAlert();
            }
        }
#endif
    }

    public virtual void QuitGame() {
        Application.Quit();
    }

    int screenshotCount = 1;

    public virtual void Update() {

        AdNetworks.HandleAdUpdate();

#if !UNITY_IPHONE
        if(Input.GetKeyDown(KeyCode.Escape)) {
            QuitGame();
            //if(AlertDialog.Instance.IsReady()) {
            //  AlertDialog.Instance.ShowAlertQuitDialog();
            //}

        }
#endif

        if(Application.isEditor) {

            if((Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.RightControl))) {

                if(Input.GetKeyDown(KeyCode.P)) {

#if UNITY_EDITOR
                    // Toggle paused
                    //UnityEditor.EditorApplication.isPaused = 
                    //    !UnityEditor.EditorApplication.isPaused ? true : false;

                    UnityEditor.EditorApplication.isPaused =
                        !UnityEditor.EditorApplication.isPaused;
#endif

                }
                else if(Input.GetKeyDown(KeyCode.E)
                         && (Input.GetKeyDown(KeyCode.Plus)
                         || Input.GetKeyDown(KeyCode.KeypadPlus))) {

                    // volume up for effects
                    currentVolumeEffects += currentVolumeInc;
                    UpdateAudio(currentVolumeMusic, currentVolumeEffects);
                }
                else if(Input.GetKeyDown(KeyCode.E)
                         && (Input.GetKeyDown(KeyCode.Minus)
                         || Input.GetKeyDown(KeyCode.KeypadMinus))) {

                    // volume up for effects
                    currentVolumeEffects -= currentVolumeInc;
                    UpdateAudio(currentVolumeMusic, currentVolumeEffects);

                }
                else if(Input.GetKeyDown(KeyCode.M)
                         && (Input.GetKeyDown(KeyCode.Plus)
                         || Input.GetKeyDown(KeyCode.KeypadPlus))) {

                    // volume up for music
                    currentVolumeMusic += currentVolumeInc;
                    UpdateAudio(currentVolumeMusic, currentVolumeEffects);

                }
                else if(Input.GetKeyDown(KeyCode.M)
                         && (Input.GetKeyDown(KeyCode.Minus)
                         || Input.GetKeyDown(KeyCode.KeypadMinus))) {

                    // volume up for music
                    currentVolumeMusic -= currentVolumeInc;
                    UpdateAudio(currentVolumeMusic, currentVolumeEffects);
                }
            }

            if(Input.GetKeyDown(KeyCode.Comma)) {

                string sceneName = "Panel";

#if USE_GAME_LIB_GAMES_UI
                if(GameUIController.Instance != null) {
                    sceneName = GameUIController.Instance.currentPanel;
                }
#endif

                string filename = "../screenshots/" + sceneName +
                                  "-" + Screen.width.ToString() + "x" + Screen.height.ToString()
                                  + "-" + (screenshotCount++).ToString() + ".png";

                if(GameController.IsGameRunning) {
                    filename = "../screenshots/" + sceneName + "-gameplay-" +
                    "-" + Screen.width.ToString() + "x" + Screen.height.ToString()
                    + "-" + (screenshotCount++).ToString() + ".png";
                }
                ScreenCapture.CaptureScreenshot(filename);
            }
        }

    }
}