using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;
using Engine.UI;
using Engine.Utility;

public class BaseGameHUD : GameUIPanelBase {
        
    public float currentTimeBlock = 0.0f;
    public float actionInterval = 1.0f;
    public AsyncOperation asyncLevelLoad = null;
    public bool levelLoadInProgress = false;
    public bool lastLevelLoadInProgress = false;


#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UILabel labelScores;
    public UILabel labelScore;
    public UILabel labelCoins;
    public UILabel labelLevel;
    public UILabel labelTime;
    public UIImageButton buttonCamera;
    public UIImageButton buttonGameSafety;
    public UIImageButton buttonGameSmarts;
    public UIImageButton buttonGameTutorial;
    public UIImageButton buttonGameTips;
    public UIImageButton buttonGameOverview;
    public UISlider sliderHealth;
    public UISlider sliderEnergy;
#else
    public Text labelScores;
    public Text labelScore;
    public Text labelCoins;
    public Text labelSpecials;
    public Text labelLevel;
    public Text labelTime;
    public Button buttonCamera;
    public Button buttonGameSafety;
    public Button buttonGameSmarts;
    public Button buttonGameTutorial;
    public Button buttonGameTips;
    public Button buttonGameOverview;
    public Slider sliderHealth;
    public Slider sliderEnergy;
#endif


    public GameObject containerUseObject;
    public GameObject containerSmartsObject;
    public GameObject containerSafetyObject;
    public GameObject containerScoreObject;
    public GameObject containerScoresObject;
    public GameObject containerCoinsObject;
    public GameObject containerSpecialsObject;
    public GameObject containerCameraObject;
    public GameObject containerDevObject;
    public GameObject containerTimeObject;
    public GameObject containerOverviewObject;
    public bool initialized = false;
    public GameObject overlayFogObject;
    public GameObject overlayRedObject;
    public GameObject overlayMagicObject;
    public GameObject overlayFilterObject;
    public GameObject containerCharacters;
    public GameObject containerPause;
    public GameObject containerDisplay;
    public GameObject containerControlsLeft;
    public GameObject containerControlsRight;
    public GameObject containerInputLeft;
    public GameObject containerInputRight;
    public GameObject containerCamera;
    public GameObject containerHealth;
    public GameObject containerEnergy;
    public GameObject containerOffscreenIndicators;
    public static GameHUD Instance;
        
    public static bool isInst {
        get {
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {

    }
                
    public override void Start() {
        Init();
    }
    
    public override void Init() {

        // check platform
        HandlePlatform();

        InitEvents();
    }

    public virtual void HandlePlatform() {

        HandleInput();
    }

    public virtual void HandleInput() {
        if (Application.isWebPlayer || Application.isEditor) {
            // hide left virtual pad
            //HideInputLeftObject(.5f, 0f);
        }
        else {
            //ShowInputLeftObject(.5f, 0f);
        }
    }
    
    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string, string, object>.AddListener(GameMessages.gameActionItem, OnGameItem);
        
        Messenger<double>.AddListener(GameMessages.gameActionScore, OnGameShooterScore);
        Messenger<double>.AddListener(GameMessages.gameActionScores, OnGameShooterScores);
    }
    
    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<string, string, object>.RemoveListener(GameMessages.gameActionItem, OnGameItem);

        Messenger<double>.RemoveListener(GameMessages.gameActionScore, OnGameShooterScore);
        Messenger<double>.RemoveListener(GameMessages.gameActionScores, OnGameShooterScores);
    }

    public override void OnButtonClickEventHandler(string buttonName) {
        if (UIUtil.IsButtonClicked(buttonCamera, buttonName)) {
            LogUtil.Log("Button camera Clicked: " + buttonName);
            if (!AppModes.Instance.isAppModeGameTraining) {
                ChangeCameraMode();
            }
        }
        else if (UIUtil.IsButtonClicked(buttonGameOverview, buttonName)) {
            LogUtil.Log("Button camera Clicked: " + buttonName);
            GameController.GameContentDisplay(GameContentDisplayTypes.gameModeContentOverview);
        }
        else if (UIUtil.IsButtonClicked(buttonGameSafety, buttonName)) {
            LogUtil.Log("Button camera Clicked: " + buttonName);
            GameController.GameContentDisplay(GameContentDisplayTypes.gameHealth);
        }
        else if (UIUtil.IsButtonClicked(buttonGameSmarts, buttonName)) {
            LogUtil.Log("Button camera Clicked: " + buttonName);
            GameController.GameContentDisplay(GameContentDisplayTypes.gameEnergy);
        }
        else if (UIUtil.IsButtonClicked(buttonGameTips, buttonName)) {
            LogUtil.Log("Button camera Clicked: " + buttonName);
            GameController.GameContentDisplay(GameContentDisplayTypes.gameTips);
        }
        else if (UIUtil.IsButtonClicked(buttonGameTutorial, buttonName)) {
            LogUtil.Log("Button camera Clicked: " + buttonName);
            GameController.GameContentDisplay(GameContentDisplayTypes.gameTutorial);
        }
    }
    
    public virtual void ChangeCameraMode() {
        GameController.CycleGameCameraMode();
    }

    public virtual void OnGameItem(string code, string type, object val) {
        //if(type == GamePlayerItemType.itemCoin) {            
        //    SetCoins(GameController.CurrentGamePlayerController.runtimeData.coins);
        //}
        //else if(type == GamePlayerItemType.itemHealth) {            
        //        //SetHealth(GameController.CurrentGamePlayerController.runtimeData.health);
        //}
    }

    public virtual void OnGameShooterScore(double score) {
        SetScore(GameController.CurrentGamePlayerController.runtimeData.score);
    }

    public virtual void OnGameShooterScores(double scores) {
        SetScores(GameController.CurrentGamePlayerController.runtimeData.scores);
    }
        
    public virtual void InitEvents() {
            
        LogUtil.Log("InitEvents:");
    }
        
    public virtual void LateUpdate() {

    }

    public virtual void ResetIndicators() {
        if (containerOffscreenIndicators != null) {
            containerOffscreenIndicators.DestroyChildren();
        }

        GameZoneGoalMarker marker = GameZoneGoalMarker.GetMarker();
        marker.UpdateIndicator();
    }
    
    public virtual void Show() {
    
    }
    
    public virtual void Hide() {
    
    }
    
    public virtual void Reset() {
    
    }
    
    public virtual void SetLevelInit(GameLevel gameLevel) {
        
        if (gameLevel != null) {
            //GameController.Instance.runtimeData.ammo = gameLevel.ammo;            
            //GameController.Instance.runtimeData.score = 0;
            
            SetCoins(0);
            SetScore(0);
            SetScores(0);
            SetSpecials(0);
            SetLevel(gameLevel.code);
        }       
        
    }
    
    public virtual void SetScore(double score) {
        UIUtil.SetLabelValue(labelScore, score.ToString("N0"));
    }

    public virtual void SetScores(double scores) {
        UIUtil.SetLabelValue(labelScores, scores.ToString("N0"));
    }
    
    public virtual void SetCoins(double coins) {
        UIUtil.SetLabelValue(labelCoins, coins.ToString("N0"));
    }

    public virtual void SetSpecials(double specials) {
        UIUtil.SetLabelValue(labelSpecials, specials.ToString("N0"));
    }

    public virtual void SetLevel(string levelName) {
        UIUtil.SetLabelValue(labelLevel, levelName);
    }
    
    public virtual void SetTime(double time) {
        UIUtil.SetLabelValue(labelTime, FormatUtil.GetFormattedTimeMinutesSecondsMsSmall(time));
    }
        
    public virtual void ShowHitOne() {
        //LogUtil.Log("ShowHitOne");
        
        DeviceUtil.Vibrate();       
        
        //HideOverlayRed(.1f, 0f, 0f);
        ShowOverlayRed(.2f, .1f, 0f, .4f);
        HideOverlayRed(1, .2f, .4f, 0f);
    }
    
    public virtual void ShowHitOne(float modifier) {
        //LogUtil.Log("ShowHitOne");
        
        DeviceUtil.Vibrate();       
        
        //HideOverlayRed(.1f, 0f, 0f);
        ShowOverlayRed(.2f, .1f, 0f, .4f * modifier);
        HideOverlayRed(1, .2f, .4f * modifier, 0f);
    } 
    
    public virtual void ShowOverlayRed() {
        ///ShowOverlayRed(.3f, .1f, 1f);
    }
    
    public virtual void ShowOverlayRed(float time, float delay, float amountFrom, float amountTo) {
        
        if (overlayRedObject != null) {
            UITweenerUtil.FadeTo(overlayRedObject, 
                UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
        }
    }

    public virtual void HideOverlayRed() {
        HideOverlayRed(.1f, .2f, 0f, 0f);
    }
    
    public virtual void HideOverlayRed(float time, float delay, float amountFrom, float amountTo) {     
        if (overlayRedObject != null) {
            UITweenerUtil.FadeTo(overlayRedObject, 
                UITweener.Method.Linear, UITweener.Style.Once, time, delay, amountFrom, amountTo);
        }
    }
    
    public virtual void ShowCharacterObject(float time, float delay) {
        if (containerCharacters != null) {
            UITweenerUtil.MoveTo(containerCharacters, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(leftOpenX));          
        }
    }
    
    public virtual void HideCharacterObject(float time, float delay) {
        if (containerCharacters != null) {
            UITweenerUtil.MoveTo(containerCharacters, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(leftClosedX));            
        }
    }
    
    public virtual void ShowDisplayObject(float time, float delay) {
        if (containerDisplay != null) {
            UITweenerUtil.MoveTo(containerDisplay, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(topOpenY));           
        }
    }
    
    public virtual void HideDisplayObject(float time, float delay) {
        if (containerDisplay != null) {
            UITweenerUtil.MoveTo(containerDisplay, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(topClosedY));         
        }
    }
    
    public virtual void ShowOverviewObject(float time, float delay) {
        if (containerDisplay != null) {
            UITweenerUtil.MoveTo(containerOverviewObject, 
                                 UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(topOpenY));           
        }
    }
    
    public virtual void HideOverviewObject(float time, float delay) {
        if (containerDisplay != null) {
            UITweenerUtil.MoveTo(containerOverviewObject, 
                                 UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithY(topClosedY));         
        }
    }
    
    public virtual void ShowPauseObject(float time, float delay) {
        if (containerPause != null) {
            UITweenerUtil.MoveTo(containerPause, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightOpenX));         
        }
    }
    
    public virtual void HidePauseObject(float time, float delay) {
        if (containerPause != null) {
            UITweenerUtil.MoveTo(containerPause, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));           
        }
    }
    
    public virtual void ShowInputLeftObject(float time, float delay) {
        if (containerInputLeft != null) {
            UITweenerUtil.MoveTo(containerInputLeft, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftOpenX));          
        }
    }
    
    public virtual void HideInputLeftObject(float time, float delay) {
        if (containerInputLeft != null) {
            UITweenerUtil.MoveTo(containerInputLeft, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftClosedX));            
        }
    }
    
    public virtual void ShowInputRightObject(float time, float delay) {
        if (containerInputRight != null) {
            UITweenerUtil.MoveTo(containerInputRight, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightOpenX));         
        }
    }
        
    public virtual void HideInputRightObject(float time, float delay) {
        if (containerInputRight != null) {
            UITweenerUtil.MoveTo(containerInputRight, 
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));           
        }
    }

    public virtual void ShowControlsLeftObject(float time, float delay) {
        if (containerControlsLeft != null) {
            UITweenerUtil.MoveTo(containerControlsLeft,
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftOpenX));
        }
    }
    
    public virtual void HideControlsLeftObject(float time, float delay) {
        if (containerControlsLeft != null) {
            UITweenerUtil.MoveTo(containerControlsLeft,
             UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(leftClosedX));
        }
    }
    
    public virtual void ShowControlsRightObject(float time, float delay) {
        if (containerControlsRight != null) {
            UITweenerUtil.MoveTo(containerControlsRight,
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightOpenX));
        }
    }
    
    public virtual void HideControlsRightObject(float time, float delay) {
        if (containerControlsRight != null) {
            UITweenerUtil.MoveTo(containerControlsRight,
                UITweener.Method.EaseInOut, UITweener.Style.Once, time, delay, Vector3.zero.WithX(rightClosedX));
        }
    }
    
    public virtual void ShowEditState() {
        ShowCharacterObject(.5f, 0f);
        ShowPauseObject(.5f, 0f);
        HideDisplayObject(.5f, 0f);
        ShowControlsLeftObject(.5f, 0f);
        HideControlsRightObject(.5f, 0f);

        HandlePlatform();
    }
    
    public virtual void ShowGameState() {
        ShowCharacterObject(.5f, 0f);
        ShowPauseObject(.5f, 0f);
        ShowDisplayObject(.5f, 0f);
        ShowControlsLeftObject(.5f, 0f);
        ShowControlsRightObject(.5f, 0f);

        HandlePlatform();
    }
    
    public override void AnimateIn() {
        
        base.AnimateIn();

        HandleItems();

        if (GameDraggableEditor.isEditing) {
            ShowEditState();
        }
        else {
            ShowGameState();
        }

        if (AppModes.Instance.isAppModeGameTraining) {
            ShowObject(containerUseObject);

            HideObject(containerSmartsObject);
            HideObject(containerSafetyObject);
            HideObject(containerScoreObject);
            HideObject(containerScoresObject);
            HideObject(containerCoinsObject);
            HideObject(containerSpecialsObject);
            HideObject(containerCameraObject);
            HideObject(containerTimeObject);
            HideObject(containerOverviewObject);
        }
        else {
            HideObject(containerUseObject);

            ShowObject(containerSmartsObject);
            ShowObject(containerSafetyObject);
            ShowObject(containerScoreObject);
            ShowObject(containerScoresObject);
            ShowObject(containerCoinsObject);
            ShowObject(containerSpecialsObject);
            ShowObject(containerCameraObject);
            ShowObject(containerTimeObject);
            ShowObject(containerOverviewObject);
        }

        //HideOverlayRed();
    }

    public virtual void HandleItems() {

        // Handle by world

        string codeWorld = GameWorlds.Current.code;

        if (codeWorld.IsNullOrEmpty()) {
            return;
        }

        foreach (GameObjectInactive container in gameObject.GetList<GameObjectInactive>()) {

            if (container.type.IsEqualLowercase(BaseDataObjectKeys.display_items)) {

                foreach (GameObjectInactive item in container.gameObject.GetList<GameObjectInactive>()) {

                    if (item.type.IsEqualLowercase(BaseDataObjectKeys.display_item)) {
                        item.gameObject.HideChildren();
                    }
                }

                container.gameObject.Show();

                foreach (GameObjectData dataItem in container.gameObject.GetList<GameObjectData>()) {

                    Dictionary<string, object> data = dataItem.ToDictionary();

                    string val = data.Get<string>(BaseDataObjectKeys.world);

                    if (val.IsEqualLowercase(codeWorld)) {

                        dataItem.gameObject.Show();
                    }
                }
            }
        }
    }

    public virtual void AnimateInOverlayDamage() {
        
        base.AnimateIn();
        
        ShowOverlayRed();
    }
    
    public override void AnimateOut() {
        
        base.AnimateOut();
        
        //HideOverlayRed();
    }
    
    public virtual void Update() {
        /*
        var ry = 0f;
        //var rx = 0f;
        if(Context.Current.isMobile) {
            ry =-Input.acceleration.y + Screen.height/2;
            //rx =-Input.acceleration.x + Screen.width/2;
        }
        else {
            ry =-Input.mousePosition.y + Screen.height/2;
            //rx =-Input.acceleration.x + Screen.width/2;
        }
        
        if(overlayRedObject != null) {
            //overlayRedObject.transform.Rotate(Vector3.forward * (ry * .005f) * Time.deltaTime);
        }
        */
        
        if (GameController.IsGameRunning) {
            if (GameController.CurrentGamePlayerController != null) {
                if (GameController.Instance != null) {
                    if (GameController.Instance.runtimeData != null) {
                        SetScore(GameController.CurrentGamePlayerController.runtimeData.score);
                        SetScores(GameController.CurrentGamePlayerController.runtimeData.scores);
                        SetCoins(GameController.CurrentGamePlayerController.runtimeData.coins);
                        SetSpecials(GameController.CurrentGamePlayerController.runtimeData.specials);
                        SetTime(GameController.Instance.runtimeData.timeRemaining);
                    }
                }
            }
        }
        
        if (Application.isEditor) {
            if (Input.GetKeyDown(KeyCode.P)) {
                ShowHitOne();
            }
        }
    }
    
}

/*
 * 
 * 
 * using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum GameHUDState {
    GAME_ENTER,
    GAME_PREPARE,
    GAME_COUNTDOWN_1,
    GAME_COUNTDOWN_2,
    GAME_COUNTDOWN_3,
    GAME_START,
    GAME_FINISH,
    GAME_EXIT
}

public enum GameHudCameraState {
    CAMERA_FIXED_DEFAULT,
    CAMERA_FIXED_TIGHT,
    CAMERA_LOOK_AHEAD,
    CAMERA_FOLLOW_DEFAULT,
    CAMERA_FOLLOW_CLOSE
}

public class GameCameraMode {
    public string displayName = "Follow Cam";
    public CameraMode cameraMode = CameraMode.Follow;
}

public class GameHUD : GameObjectBehavior {
    
    public static GameHUD Instance;
    
    public bool destroyed = false;
    
    public GameHUDState hudState = GameHUDState.GAME_ENTER;
    public int countdownStepCompleted = 0;
    
    public GameObject hudContainer;
    
    public AssetBundle bundle; // level asset bundle if loaded
    
    public Vector3 panelPauseObjectInitialPosition;
    public Vector3 seriesInfoObjectInitialPosition;
    public Vector3 goObjectInitialPosition;
    public Vector3 goObjectInitialScale;
    public Vector3 countdown1ObjectInitialPosition;
    public Vector3 countdown1ObjectInitialScale;
    public Vector3 countdown2ObjectInitialPosition;
    public Vector3 countdown2ObjectInitialScale;
    public Vector3 countdown3ObjectInitialPosition;
    public Vector3 countdown3ObjectInitialScale;
    public Vector3 finishObjectInitialPosition;
    
    public Vector3 quitButtonInitialPosition;
    
    public GameObject panelPauseObject;
    public GameObject panelPauseObjectBackground;
    public GameObject overlayFadeObject;
    public GameObject overlayFadeInfoObject;
    public GameObject hudOutputObject;
    public GameObject inputsObject;
    public GameObject tipsObject;
    public GameObject seriesInfoObject;
    public GameObject seriesInfoObjectBackground;
    
    
    public GameObject panelTransitionObject;
    public GameObject countdown1Object;
    public GameObject countdown2Object;
    public GameObject countdown3Object;
    public GameObject goObject;
    public GameObject finishObject;
    
    public UIButton inputLeft;
    public UIButton inputRight;
        
    public UIButton buttonHitLeft;
    public UIButton buttonHitRight;
    
    public UIButton buttonPause;
    public UIButton buttonPauseHitArea;
        
    public UIButton buttonQuit;
    public UIButton buttonResume;
    public UIButton buttonRestart;
    
    public UIButton buttonCameraSwitcher;
    public UILabel labelCameraSwitcher;
    
    public UILabel labelTime;
    public UILabel labelLapCurrent;
    public UILabel labelLapTotal;
    public UILabel labelLapPlace;   
    
    public UILabel labelName;
    public UILabel labelSponsor;
    public UILabel labelLaps;
    public UILabel labelPlace;
    public UILabel labelTouchToSkip;
    public GameObject labelTouchToSkipObject;
    
    public UILabel labelRaceMode;
    
    public bool pauseExpanded = false;  
    
    public float currentTotalTime = 0f;
    public int currentPosition = 1;
    public int lastCurrentPosition = -1;
    
    float currentTimeBlockLocal = 0.0f;
    float actionIntervalLocal = 1.0f;   
    
    float currentTimeBlockMicro = 0.0f;
    float actionIntervalMicro = 0.3f;   
    
    GameObject legacyLeftButton;
    GameObject legacyRightButton;
    int limitOnLegacyCheck = 500;
    
    bool touchInputEnabled = true;
    bool lastTouchInputEnabled = false;
    
    bool initialized = false;
        
    float lastEventStartTime = 0f;
    float lastEventFinishTime = 0f;
    float lastEventLapTime = 0f;
    float lastEventResetTime = 0f;
    float lastEventBoostTime = 0f;
    float lastEventPassTime = 0f;
    float lastEventPassedTime = 0f;
    float lastEventCollisionTime = 0f;
    float lastEventCauseCollisionTime = 0f;
    
    float lastSoundBoost = 0f;
    float lastSoundCheer = 0f;
    float lastSoundBoo = 0f;
    float lastSoundBikeJump = 0f;
    float lastSoundBikeRev = 0f;
    float lastSoundBikeRace = 0f;
    float lastSoundCheerConstant = 0f;
    
    VehicleBehaviourScript humanVehicle;
    VehicleSounds humanVehicleSounds;
    VehicleStats humanVehicleStats;
    
    VehicleStatsData humanVehicleStatsData;
    
    
    CameraBehavior humanVehicleCameraBehavior;
    public List<GameCameraMode> cameraModes;
    public int currentSelectedCameraMode = 0;
    public GameCameraMode currentCameraMode;
    
    public bool raceActive = false;
    public bool raceQuit = false;
    
    UIButtonMeta buttonMeta;
    float currentTimeBlock = 0.0f;
    float actionInterval = 1.0f;    
    
    public AsyncOperation asyncLevelLoad = null;
    public bool levelLoadInProgress = false;
    public bool lastLevelLoadInProgress = false;
    
    float soundDelayModifier = 1f;
    
    bool hasCustomAudioCrowdCheer = false;
    bool hasCustomAudioCrowdJump = false;
    bool hasCustomAudioCrowdBoo = false;
    bool hasCustomAudioBikeJumping = false;
    bool hasCustomAudioBikeRacing = false;
    bool hasCustomAudioBikeRevving = false;
    
    void Awake() {
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(this);
            return;
        }
    
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        Init();
    }
    
    public void Init() {
        
        
        humanVehicleStatsData = new VehicleStatsData();
        
        soundDelayModifier = 1f;
#if UNITY_IPHONE        
        if(iPhone.generation == iPhoneGeneration.iPad1Gen
            || iPhone.generation == iPhoneGeneration.iPhone3GS) {
            soundDelayModifier = soundDelayModifier * 1.2f;
        }
                
#endif
        panelPauseObjectInitialPosition = panelPauseObject.transform.position;  
        seriesInfoObjectInitialPosition = seriesInfoObject.transform.position;
        goObjectInitialPosition = goObject.transform.position;
        goObjectInitialScale = goObject.transform.localScale;
        countdown1ObjectInitialPosition = countdown1Object.transform.position;
        countdown2ObjectInitialPosition = countdown2Object.transform.position;
        countdown3ObjectInitialPosition = countdown3Object.transform.position;
        
        countdown1ObjectInitialScale = countdown1Object.transform.localScale;
        countdown2ObjectInitialScale = countdown2Object.transform.localScale;
        countdown3ObjectInitialScale = countdown3Object.transform.localScale;
        
        finishObjectInitialPosition = finishObject.transform.position;
        
        quitButtonInitialPosition = buttonQuit.transform.localPosition;
        labelTouchToSkipObject = labelTouchToSkip.gameObject;
        
        InitObjects();
        
        InitEvents();       
        
        Hide();
    } 
    
    void InitObjects() {
        
        Tweens.Instance.FadeToObject(inputsObject, 0f, 0f, 0f);     
        Tweens.Instance.FadeToObject(countdown1Object, 0f, 0f, 0f);
        Tweens.Instance.FadeToObject(countdown2Object, 0f, 0f, 0f);
        Tweens.Instance.FadeToObject(countdown3Object, 0f, 0f, 0f);
        Tweens.Instance.FadeToObject(goObject, 0f, 0f, 0.0f);
        Tweens.Instance.FadeToObject(finishObject, 0f, 0f, 0.0f);
        Tweens.Instance.FadeToObject(labelTouchToSkipObject, 1f, 0f, 0f);
        Tweens.Instance.FadeToObject(seriesInfoObject, 1f, 0f, 0f);
        Tweens.Instance.FadeToObject(seriesInfoObjectBackground, .5f, 0f, .05f);
        Tweens.Instance.FadeToObject(panelPauseObject, 1f, 0f, 0f);
        Tweens.Instance.FadeToObject(panelPauseObjectBackground, .5f, 0f, 0f);
        //FadeToObject(overlayFadeObject, 0f, .5f, .1f);
        Tweens.Instance.FadeToObject(hudOutputObject, 0f, 0f, 0f);
    }
        
    void InitEvents() {
        
        buttonMeta.SetButton("buttonPause", ref buttonPause, delegate () {  
            TogglePausePanel();
        });
        
        buttonMeta.SetButton("buttonPauseHitArea", ref buttonPauseHitArea, delegate () {    
            TogglePausePanel();
        });
        
        buttonMeta.SetButton("buttonResume", ref buttonResume, delegate () {    
            TogglePausePanel();
        });
        
        buttonMeta.SetButton("buttonRestart", ref buttonRestart, delegate () {  
            TogglePausePanel();
            RestartRace();
        });
        
        buttonMeta.SetButton("buttonQuit", ref buttonQuit, delegate () {
            TogglePausePanel();     
            GameDatas.Current.lastRaceQuit = true;  
            Invoke("QuitRace", 1.1f);
        });
        
        buttonMeta.SetButton("buttonCameraSwitcher", ref buttonCameraSwitcher, delegate () {    
            NextCameraMode();
        });
        
        ShowOrHideInputs();
    }
    
    public void ResetCameraMode() {
        humanVehicleCameraBehavior = null;  
        FindCameraBehavior();
        int currentCameraModeSaved = GameProfiles.Current.GetCurrentCameraMode();
        if(currentCameraModeSaved > cameraModes.Count - 1) {
            currentCameraModeSaved = 0;
        }
        SelectCamera(currentCameraModeSaved);
    }
    
    public void Reset() {
        
        seriesInfoObject.SetActiveRecursively(true);
        panelPauseObject.SetActiveRecursively(true);
        
        ResetCameraMode();
        
        InitObjects();
                
        panelPauseObject.transform.position = panelPauseObjectInitialPosition;  
        seriesInfoObject.transform.position = seriesInfoObjectInitialPosition;  
        goObject.transform.position = goObjectInitialPosition;  
        goObject.transform.localScale = goObjectInitialScale;   
        countdown1Object.transform.position = countdown1ObjectInitialPosition;  
        countdown1Object.transform.localScale = countdown1ObjectInitialScale;
        countdown2Object.transform.position = countdown2ObjectInitialPosition;
        countdown2Object.transform.localScale = countdown2ObjectInitialScale;   
        countdown3Object.transform.position = countdown3ObjectInitialPosition;      
        countdown3Object.transform.localScale = countdown3ObjectInitialScale;
        finishObject.transform.position = finishObjectInitialPosition;  
        buttonQuit.transform.localPosition = quitButtonInitialPosition;         
    }
    
    public void Show() {    
        hudContainer.SetActiveRecursively(true);
        initialized = true;
        limitOnLegacyCheck = 500;
        ShowOrHideInputs();
        Reset();
        LoadSeriesInfo();   
        Tweens.Instance.MoveFromObject(seriesInfoObject,
                                       new Vector3(seriesInfoObject.transform.position.x,
                                              seriesInfoObject.transform.position.y - 9,
                                             seriesInfoObject.transform.position.z), .5f, 0f);
    }
    
    public void Hide() {
        initialized = false;        
        Tweens.Instance.FadeToObject(inputsObject, 0f, 0f, 0f); 
        
        if(GameLoadingObject.Instance != null) {
            GameLoadingObject.Instance.ShowAndHideLoadingHelp();
        }
        hudContainer.SetActiveRecursively(false);
    }
    
    public void PrepareCurrentLevel() {
                
        PrepareAndLoadLevel();  
                
        CheckTouchInputState();
    }
    
    public void StartLoadedLevel() {
        if(RaceManagerScript.Instance != null) {
            RaceManagerScript.Instance.raceEnabled = true;
        }
    }
        
    public void LoadSeriesInfo() {
        
        string raceModeName = "Series Event Mode";
        string place = "";
        string laps = "";
        string sponsorName = "";
        string levelName = "";
                    
        if(GameDatas.Current.IsRaceModeArcade()) {
            raceModeName = "Arcade Mode";
            place = "";
            laps = "Difficulty: " + GameDatas.Current.currentDifficultyValue.ToString("P");
            sponsorName = GamePacks.Instance.GetById(GameLevels.Current.pack[0]).display_name;
            levelName = GameLevels.Current.display_name;                
        }
        else if (GameDatas.Current.IsRaceModeEndless()) {
            raceModeName = "Endless Mode";
            place = "";
            laps = "Difficulty: " + GameDatas.Current.currentDifficultyValue.ToString("P");
            sponsorName = GamePacks.Instance.GetById(GameLevels.Current.pack[0]).display_name;
            levelName = GameLevels.Current.display_name;        
        }
        else { //(GameDatas.Current.IsRaceModeSeries()) {
            raceModeName = "Series Event Mode";
            place = "";
            laps = "";
            sponsorName = "";
            levelName = ""; 
            
            GameSeriesEvent seriesEvent = GameSeriesEvents.Instance.GetCurrentEvent();
            
            if(seriesEvent != null) {
                string stageName = "Qualifier";
                if(seriesEvent.stage == GameSeriesEventStage.SEMI_FINAL) {
                    stageName = "Semi-final";
                }
                else if(seriesEvent.stage == GameSeriesEventStage.FINAL) {
                    stageName = "Final";
                }
                raceModeName = raceModeName + " " + stageName;
                
                LogUtil.LogAlways("Environment:" + seriesEvent.environmentName);
                
                levelName = GameLevels.Instance.GetById(seriesEvent.environmentName).display_name;
                laps = GameDatas.Current.currentTotalLaps.ToString() + " Laps";
                sponsorName = seriesEvent.sponsor;
            
                int placeInt = GameSeriesEvents.Instance.GetPlaceByPoints(seriesEvent.minimumScore);
                place = GameSeriesEvents.Instance.GetPrettyPlace(placeInt) + " Required";
            }
        }
            
        if(labelName != null) {
            labelName.text = levelName;
        }

        if(labelLaps != null) {
            labelLaps.text = laps;
        }
        
        if(labelSponsor != null) {
            labelSponsor.text = sponsorName;
        }
        
        if(labelPlace != null) {
            labelPlace.text = place;
        }
            
        if(labelRaceMode) {
            labelRaceMode.text = raceModeName;
        }
    }
    
    public void ChangeState(GameHUDState stateTo) {
        hudState = stateTo;
    }   
    
    void OnEnable() {
        //LogUtil.Log("GameRaceHUD::OnEnable");
        
        Messenger.AddListener(GamePlayerMessages.EventRacePrepare, OnEventRacePrepare);
        Messenger.AddListener(GamePlayerMessages.EventRaceStart, OnEventRaceStart);
        Messenger.AddListener(GamePlayerMessages.EventRaceFinish, OnEventRaceFinish);
        Messenger.AddListener(GamePlayerMessages.EventRaceVehicleBoost, OnEventRaceVehicleBoost);
        Messenger.AddListener(GamePlayerMessages.EventRaceVehicleReset, OnEventRaceVehicleReset);
        
        Messenger.AddListener(GamePlayerMessages.EventRaceVehicleCollision, OnEventRaceVehicleCollision);
        Messenger.AddListener(GamePlayerMessages.EventRaceVehicleCauseCollision, OnEventRaceVehicleCauseCollision);
        Messenger.AddListener(GamePlayerMessages.EventRaceVehiclePass, OnEventRaceVehiclePass);
        Messenger.AddListener(GamePlayerMessages.EventRaceVehiclePassed, OnEventRaceVehiclePassed);
        
        Messenger<int>.AddListener(GamePlayerMessages.EventRaceCountdown, OnEventRaceCountdown);
        Messenger<int>.AddListener(GamePlayerMessages.EventRaceLapEvent, OnEventRaceLapEvent);
        
        // TODO add wreck/bump/mud
        
    }
    
    void OnDisable() {
        //LogUtil.Log("GameRaceHUD::onDisable");
        
        Messenger.RemoveListener(GamePlayerMessages.EventRacePrepare, OnEventRacePrepare);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceStart, OnEventRaceStart);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceFinish, OnEventRaceFinish);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceVehicleBoost, OnEventRaceVehicleBoost);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceVehicleReset, OnEventRaceVehicleReset);
        
        Messenger.RemoveListener(GamePlayerMessages.EventRaceVehicleCollision, OnEventRaceVehicleCollision);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceVehicleCauseCollision, OnEventRaceVehicleCauseCollision);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceVehiclePass, OnEventRaceVehiclePass);
        Messenger.RemoveListener(GamePlayerMessages.EventRaceVehiclePassed, OnEventRaceVehiclePassed);
        
        Messenger<int>.RemoveListener(GamePlayerMessages.EventRaceLapEvent, OnEventRaceLapEvent);
        Messenger<int>.RemoveListener(GamePlayerMessages.EventRaceCountdown, OnEventRaceCountdown);
    }
    
    public void ChangeCamera(GameCameraMode gameCameraMode) {
        FindCameraBehavior();
        
        currentCameraMode = gameCameraMode;
        
        if(gameCameraMode != null) {
            
            if(humanVehicleCameraBehavior != null) {
                humanVehicleCameraBehavior.SetCameraMode(gameCameraMode.cameraMode);
            }
            
            SetCameraDisplay();
        }
    }
    
    public void SetCameraDisplay() {
        if(labelCameraSwitcher != null 
            && currentCameraMode != null) {
            labelCameraSwitcher.text = currentCameraMode.displayName + " Cam";
        }
    }
    
    public void NextCameraMode() {
        SelectCamera(currentSelectedCameraMode + 1);        
    }
    
    void SelectCamera(int index) {
        FindCameraBehavior();
        
        if(index > cameraModes.Count - 1) {
            index = 0;
        }
        else if (index < 0) {
            index = cameraModes.Count - 1;
        }       
        
        currentSelectedCameraMode = index;      
        currentCameraMode = cameraModes[currentSelectedCameraMode];
        
        GameProfiles.Current.SetCurrentCameraMode(currentSelectedCameraMode);
        
        ChangeCamera(currentCameraMode);    
    }
    
    void FindCameraBehavior() {
        //
        
        if(cameraModes == null) {
            // Fill camera modes
            
            cameraModes = new List<GameCameraMode>();
            
            GameCameraMode cameraFollow = new GameCameraMode();
            cameraFollow.cameraMode = CameraMode.Follow;
            cameraFollow.displayName = "Fixed";
            cameraModes.Add(cameraFollow);
            
            currentCameraMode = cameraFollow;
            currentSelectedCameraMode = 0;
            
            GameCameraMode cameraFollowClose = new GameCameraMode();
            cameraFollowClose.cameraMode = CameraMode.FollowClose;
            cameraFollowClose.displayName = "Fixed Near";
            cameraModes.Add(cameraFollowClose);
            
            GameCameraMode cameraLookAhead = new GameCameraMode();
            cameraLookAhead.cameraMode = CameraMode.LookAhead;
            cameraLookAhead.displayName = "Stadium";
            cameraModes.Add(cameraLookAhead);
            
            GameCameraMode cameraLookAheadClose = new GameCameraMode();
            cameraLookAheadClose.cameraMode = CameraMode.LookAheadClose;
            cameraLookAheadClose.displayName = "Stadium Zoom";
            cameraModes.Add(cameraLookAheadClose);
            
            GameCameraMode cameraFollowBehind = new GameCameraMode();
            cameraFollowBehind.cameraMode = CameraMode.FollowBehind;
            cameraFollowBehind.displayName = "Follow Behind";
            cameraModes.Add(cameraFollowBehind);
            
            GameCameraMode cameraFollowBehindClose = new GameCameraMode();
            cameraFollowBehindClose.cameraMode = CameraMode.FollowBehindClose;
            cameraFollowBehindClose.displayName = "Follow Behind Near";
            cameraModes.Add(cameraFollowBehindClose);
            
            GameCameraMode cameraFollowBirdsEye = new GameCameraMode();
            cameraFollowBirdsEye.cameraMode = CameraMode.FollowBirdsEye;
            cameraFollowBirdsEye.displayName = "Bird's Eye";
            cameraModes.Add(cameraFollowBirdsEye);
            
            GameCameraMode cameraFollowBirdsEyeClose = new GameCameraMode();
            cameraFollowBirdsEyeClose.cameraMode = CameraMode.FollowBirdsEyeClose;
            cameraFollowBirdsEyeClose.displayName = "Bird's Eye Low";
            cameraModes.Add(cameraFollowBirdsEyeClose);

            ChangeCamera(cameraFollow);
        }
        
        FindHumanPlayer();
                
        if(humanVehicle != null 
            && humanVehicleCameraBehavior == null) {
            
            LogUtil.Log("FindCameraBehavior() Trying..." );
            
            UnityEngine.Object cameraBehavior = GameObject.FindObjectOfType(typeof(CameraBehavior));
            if(cameraBehavior != null) {
                humanVehicleCameraBehavior = cameraBehavior as CameraBehavior;
                humanVehicleCameraBehavior.initialXFollow = 11f;
                humanVehicleCameraBehavior.initialZFollow = 11f;
                humanVehicleCameraBehavior.XFollow = 11f;
                humanVehicleCameraBehavior.ZFollow = 11f;
                LogUtil.Log("FindCameraBehavior() FOUND..." + humanVehicleCameraBehavior);
            }
        }
    }
        
    IEnumerator CustomizePlayer() {
        
        hasCustomAudioCrowdCheer = false;
        hasCustomAudioCrowdJump = false;
        hasCustomAudioCrowdBoo = false;
        hasCustomAudioBikeJumping = false;
        hasCustomAudioBikeRacing = false;
        hasCustomAudioBikeRevving = false;
        
        yield return null;
        
        FindHumanPlayer();
        
        yield return new WaitForSeconds(1f);
        
        if(humanVehicle != null) {
        
            // colors
            CustomPlayerColors playerColors = GameProfiles.Current.GetCustomColors();
            
            Transform mxBike = humanVehicle.transform.FindChild("Body Lean/SSC_MX_Bike/MX_LowPoly");
            Transform mxRider = humanVehicle.transform.FindChild("Body Lean/SSC_MX_Rider_IdleTurnsImpact/MX_RiderMesh");            
                    
            if(mxBike) {
                if(mxBike.gameObject) {
                    humanVehicle.isCustomized = true;
                    mxBike.gameObject.renderer.materials[1].color = playerColors.bikeColor.GetColor();
                    LogUtil.Log("CustomizePlayer colorBike:" + playerColors.bikeColor.GetColor() );
                }
            }
            if(mxRider) {
                if(mxRider.gameObject) {
                    humanVehicle.isCustomized = true;
                    mxRider.gameObject.renderer.materials[0].color = playerColors.bootsSleevesPants.GetColor();//colorSleevesPants;
                    mxRider.gameObject.renderer.materials[1].color = playerColors.shirtColor.GetColor();//colorShirt;
                    mxRider.gameObject.renderer.materials[2].color = playerColors.skinColor.GetColor();//colorSkin;
                    mxRider.gameObject.renderer.materials[3].color = playerColors.riderColor.GetColor();//colorRider;
                    mxRider.gameObject.renderer.materials[4].color = playerColors.bootsGlovesColor.GetColor();//colorBootsGloves;
                    LogUtil.Log("CustomizePlayer colorRider:" + playerColors.riderColor.GetColor() );
                }
            }
            
            // audio
                
            CustomPlayerAudio customPlayerAudio = GameProfiles.Current.GetCustomAudio();            
        
            // Also set the bike sounds on ai if custom
            VehicleBehaviourScript[] bikes = GameObject.FindObjectsOfType(typeof(VehicleBehaviourScript)) as VehicleBehaviourScript[];//("MXBike");
            
            if(bikes != null) {
                LogUtil.Log("CustomSounds: bikes:" + bikes.Length);
                GameAudioRecorder.Instance.ClearLoadedClips();
            }
            
            foreach(VehicleBehaviourScript bike in bikes) {
                                
                if(bike != null) {
                
                    if(bike.gameObject != null) {
                        LogUtil.Log("CustomSounds: bike.gameObject.name:" + bike.gameObject.name);
                    }
                    LogUtil.Log("CustomSounds: bike.AIControlled:" + bike.AIControlled);
                    
                    VehicleSounds vehicleSounds = null;
                    vehicleSounds = bike.GetComponent<VehicleSounds>();
                    
                    LogUtil.Log("CustomSounds: vehicleSounds:" + vehicleSounds);
                    
                    if(!bike.AIControlled) {
                        humanVehicleSounds = vehicleSounds;
                    }
                    
                    if(vehicleSounds != null) { 
            
                        LogUtil.Log("CustomSounds: vehicleSounds:" + vehicleSounds);
                        
    
                        // default
                        //vehicleSounds.soundIdle.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_revs_idle));
                        vehicleSounds.soundRacing.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_medium_gear));
                        
                        vehicleSounds.soundJumping.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_jump_250_2));
                        vehicleSounds.soundJumping.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_jump_250_1));
                        vehicleSounds.soundJumping.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_jump2));
                        
                        //vehicleSounds.soundCrashing.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_jump_250_2));
                        vehicleSounds.soundRevving.Add(GameAudio.LoadLoop(GameAudioEffects.audio_effect_bike_revs_idle));
                        
                
                            
                        if(customPlayerAudio.audioItems != null) {
                            LogUtil.Log("CustomSounds: customPlayerAudio.audioItems.Count:" + customPlayerAudio.audioItems.Count);
                        }
                        
                        foreach(KeyValuePair<string, CustomPlayerAudioItem> item in customPlayerAudio.audioItems) {
                        
                            LogUtil.Log("CustomSounds: item:" + item.Key);
                            LogUtil.Log("CustomSounds: item.Value.useCustom:" + item.Value.useCustom);
                            
                            if(item.Value.useCustom) {
                                                            
                                if(item.Key.ToLower() == CustomPlayerAudioKeys.audioBikeBoosting) {

                                    var onSuccess = new Action<AudioClip, VehicleSounds>( (clip, vehicleSoundsItem)  => {
                                        LogUtil.Log("CustomSounds: soundAddedForBoosting:" + clip.name);
                                        LogUtil.Log("CustomSounds: soundAddedForBoosting:" + clip);
                                        if(clip != null && vehicleSounds != null) {                                         
                                            vehicleSoundsItem.isRunning = false;
                                            if(string.IsNullOrEmpty(clip.name)) {
                                                clip.name = CustomPlayerAudioKeys.audioBikeBoosting;
                                            }
                                            vehicleSoundsItem.soundJumping.Add(clip);
                                            //vehicleSoundsItem.soundCrashing.Add(clip);
                                            vehicleSoundsItem.soundJumpingCustomized = true;
                                            vehicleSoundsItem.isRunning = true;
                                            LogUtil.Log("CustomSounds: soundAddedForBoosting complete:" + clip.name);
                                            LogUtil.Log("CustomSounds: soundAddedForBoosting complete:" + clip);
                                        }
                                    });                             
                                    
                                    LogUtil.Log("CustomSounds: soundAddingForBoosting:" + item.Key);
                                
                                    GameAudioRecorder.Instance.LoadVehicleSounds(CustomPlayerAudioKeys.audioBikeBoosting, vehicleSounds, onSuccess);
                                    
                                    hasCustomAudioBikeJumping = true;
                                    yield return null;
                                }
                                else if(item.Key.ToLower() == CustomPlayerAudioKeys.audioBikeRacing) {
                                    var onSuccess = new Action<AudioClip, VehicleSounds>( (clip, vehicleSoundsItem) => {
                                        LogUtil.Log("CustomSounds: soundAddedForRacing:" + clip.name);
                                        LogUtil.Log("CustomSounds: soundAddedForRacing clip:" + clip);
                                        LogUtil.Log("CustomSounds: soundAddedForRacing vehicleSoundsItem:" + vehicleSoundsItem);
                                        if(clip != null && vehicleSounds != null) {
                                            vehicleSoundsItem.raceSoundRunning = false;
                                            vehicleSoundsItem.isRunning = false;
                                            if(string.IsNullOrEmpty(clip.name)) {
                                                clip.name = CustomPlayerAudioKeys.audioBikeRacing;
                                            }
                                            vehicleSoundsItem.soundRacing.Add(clip);
                                            LogUtil.Log("CustomSounds: vehicleSoundsItem.isRunning:" + vehicleSoundsItem.isRunning);
                                            LogUtil.Log("CustomSounds: vehicleSoundsItem.raceSoundRunning:" + vehicleSoundsItem.raceSoundRunning);
                                            vehicleSoundsItem.SetRacingRound(clip);
                                            vehicleSoundsItem.soundRacingCustomized = true;
                                            vehicleSoundsItem.isRunning = true;
                                            LogUtil.Log("CustomSounds: soundAddedForRacing complete:" + clip.name);
                                            LogUtil.Log("CustomSounds: soundAddedForRacing complete clip:" + clip);
                                            LogUtil.Log("CustomSounds: soundAddedForRacing complete vehicleSoundsItem:" + vehicleSoundsItem);
                                        }
                                    });     
                                        
                                    LogUtil.Log("CustomSounds: soundAddingForRacing:" + item.Key);              
                                
                                    GameAudioRecorder.Instance.LoadVehicleSounds(CustomPlayerAudioKeys.audioBikeRacing, vehicleSounds, onSuccess);
                                    
                                    hasCustomAudioBikeRacing = true;
                                    yield return new WaitForSeconds(.5f);
                                }
                                else if(item.Key.ToLower() == CustomPlayerAudioKeys.audioBikeRevving) { 

                                    var onSuccess = new Action<AudioClip, VehicleSounds>( (clip, vehicleSoundsItem) => {
                                        LogUtil.Log("CustomSounds: soundAddedForRevving:" + clip.name);
                                        
                                        if(clip != null && vehicleSounds != null) {
                                            //vehicleSoundsItem.soundIdle.Add(clip);
                                            if(string.IsNullOrEmpty(clip.name)) {
                                                clip.name = CustomPlayerAudioKeys.audioBikeRevving;
                                            }
                                            vehicleSoundsItem.soundRevving.Add(clip);
                                            vehicleSoundsItem.soundRevvingCustomized = true;
                                            vehicleSoundsItem.isRunning = true;
                                            LogUtil.Log("CustomSounds: soundAddedForRevving complete:" + clip.name);
                                        }
                                    });                             
                                
                                    LogUtil.Log("CustomSounds: soundAddingForRevving:" + item.Key);
                                    
                                    GameAudioRecorder.Instance.LoadVehicleSounds(CustomPlayerAudioKeys.audioBikeRevving, vehicleSounds, onSuccess);
                                    hasCustomAudioBikeRevving = true;
                                    yield return null;
                                }
                                else if(item.Key.ToLower() == CustomPlayerAudioKeys.audioCrowdBoo) {    
                                    hasCustomAudioCrowdBoo = true;
                                    yield return null;
                                }
                                else if(item.Key.ToLower() == CustomPlayerAudioKeys.audioCrowdCheer) {  
                                    hasCustomAudioCrowdCheer = true;
                                    yield return null;
                                }
                                else if(item.Key.ToLower() == CustomPlayerAudioKeys.audioCrowdJump) {
                                    hasCustomAudioCrowdJump = true;
                                    yield return null;
                                }
                                
                                yield return null;
                            }
                        }
                        
                        vehicleSounds.isRunning = true;
                        vehicleSounds.Revving();
                    }
                }
            }
        }
        
        yield return null;
        
        //Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    
    void FadeAndStopVehicleSounds() {
        VehicleBehaviourScript[] bikes = GameObject.FindObjectsOfType(typeof(VehicleBehaviourScript)) as VehicleBehaviourScript[];//("MXBike");

        foreach(VehicleBehaviourScript bike in bikes) {
                                
            if(bike != null) {
                VehicleSounds vehicleSounds = bike.GetComponent<VehicleSounds>();
                if(vehicleSounds != null) {
                    vehicleSounds.FadeVehicleSounds(2f);
                    vehicleSounds.isRunning = false;
                }
            }
        }
    }
        
    void AudioPlayCrowdCheer() {
        if(Time.time > lastSoundCheer + 9f * soundDelayModifier ) {
            lastSoundCheer = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdCheer, hasCustomAudioCrowdCheer);
        }
    }
    
    void AudioPlayCrowdCheerLow() {
        if(Time.time > lastSoundCheer + 15f * soundDelayModifier ) {
            lastSoundCheer = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdCheer, hasCustomAudioCrowdCheer);
        }
    }
    
    void AudioPlayCrowdBoo() {  
        if(Time.time > lastSoundBoo + 11f * soundDelayModifier ) {
            lastSoundBoo = Time.time;   
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdBoo, hasCustomAudioCrowdBoo);
        }
    }
    
    void AudioPlayCrowdBooLow() {
        if(Time.time > lastSoundBoo + 15f * soundDelayModifier ) {
            lastSoundBoo = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdBoo, hasCustomAudioCrowdBoo);
        }
    }
    
    void AudioPlayCrowdBoost() {
        if(Time.time > lastSoundBoost + 11f * soundDelayModifier ) {
            lastSoundBoost = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdJump, hasCustomAudioCrowdJump);
        }
    }
    
    void AudioPlayCrowdCheerConstant() {
        if(Time.time > lastSoundCheerConstant + 20f * soundDelayModifier ) {
            lastSoundCheerConstant = Time.time;
            GameAudio.PlayEffect(GameAudioEffects.audio_effect_crowd_cheer_1, (float)GameProfiles.Current.GetAudioEffectsVolume() * .2f);
        }
    }
    
    void AudioPlayBikeRevving() {
        if(Time.time > lastSoundBikeRev + 6f * soundDelayModifier ) {
            lastSoundBikeRev = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioBikeRevving, hasCustomAudioBikeRevving); 
        }
    }
    
    void AudioPlayBikeBoost() {
        if(Time.time > lastSoundBikeJump + 8f * soundDelayModifier ) {
            lastSoundBikeJump = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioBikeBoosting, hasCustomAudioBikeJumping);
        }
    }
    
    void AudioPlayBikeRacing() {
        if(Time.time > lastSoundBikeRace + 10f * soundDelayModifier ) {
            lastSoundBikeRace = Time.time;
            GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioBikeRacing, hasCustomAudioBikeRacing);   
        }
    }
    
    void OnEventRacePrepare() {
        //LogUtil.Log("GameRaceHUD::OnEventRacePrepare");
        
        // Find Human Player and handle customizations  
                
        GameAudio.SetVolumeForRace(true);
        GameAudio.PlayGameMainLoop(GameAudioEffects.audio_effect_crowd_cheer_constant_1,
                           (float)(GameProfiles.Current.GetAudioEffectsVolume()* .1));
        
        //GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioBikeRevving);
        
        Tweens.Instance.FadeToObject(overlayFadeObject, 0f, .5f, .1f);
        Tweens.Instance.FadeToObject(overlayFadeInfoObject, .4f, .5f, .1f);     
        
        if(UIGameAchievement.Instance) {
            UIGameAchievement.Instance.Reset();
        }
        
        ChangeState(GameHUDState.GAME_ENTER);
                
        FindCameraBehavior();
        
        StartCoroutine(CustomizePlayer());
    }
    
    void OnEventRaceStart() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceStart");
        
        if(Time.time > lastEventStartTime + 2f ) {
            lastEventStartTime = Time.time;
            
            AudioPlayCrowdCheer();
            AudioPlayCrowdCheerConstant();
            
            GameAudio.StartGameLapLoops();
            GameAudio.StartGameLoop(1);
                
            countdownStepCompleted = 0;
            Tweens.Instance.FadeToObject(countdown1Object, 0f, .5f, 0f);
            Tweens.Instance.ScaleToObject(countdown1Object, new Vector3(1f, 1f, 1f), .5f, 0f);
        
            Tweens.Instance.FadeToObject(goObject, 1f, .6f, 0f);
            Tweens.Instance.ScaleToObject(goObject, new Vector3(1.5f, 1.5f, 1.5f), .5f, .6f);
            Tweens.Instance.FadeToObject(goObject, 0f, .5f, 1.1f);
            
            
            Tweens.Instance.MoveToObject(seriesInfoObject,  
                                         new Vector3(seriesInfoObject.transform.position.x,
                                         seriesInfoObject.transform.position.y - 9,
                                         seriesInfoObject.transform.position.z),
                                         .5f, 0f);
            Tweens.Instance.FadeToObject(seriesInfoObject, .05f, .5f, 1f);
                        
            Tweens.Instance.FadeToObject(inputsObject, 1f, .5f, 0f);
            
            //Tweens.Instance.FadeToObject(tipsObject, 0f, .5f, .2f);
            //Tweens.Instance.MoveToObject(tipsObject, new Vector3(0, -20, 0), .5f, .2f);
                        
            Tweens.Instance.FadeToObject(hudOutputObject, 1f, .5f, 0f);
            //FadeToObject(overlayFadeObject, 0f, .5f, 0);
            
            SetLapCurrentLabel(1);
            SetLapTotalLabel(3);
            
            Tweens.Instance.FadeToObject(overlayFadeInfoObject, 0f, .5f, .1f);
            
            raceActive = true;
            raceQuit = false;
            
            // Delay for tap to go starts.
            GrowHitAreaForInputs();
            
            ChangeState(GameHUDState.GAME_START);
        }
    }   
    
    void OnEventRaceFinish() {
        LogUtil.Log("GameRaceHUD::OnEventRaceFinish");  
        
        GamePlayerProgress.Instance.SetStatAccumulate(GameStatistics.STAT_LAPS, 1);
        
        GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdJump);
        GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioCrowdCheer);
        GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioBikeRevving);
        GameAudio.PlayCustomOrDefaultEffect(CustomPlayerAudioKeys.audioBikeBoosting);
                            
        if(Time.time > lastEventFinishTime + 1f ) {
                    
            lastEventFinishTime = Time.time;
            
            GameDatas.Current.lastRaceQuit = false;
            FinishRace(false);
        }
    }
    
    void FinishRace(bool backToTrackSelect) {
        StartCoroutine(FinishRaceCo(backToTrackSelect));
    }
    
    IEnumerator FinishRaceCo(bool backToTrackSelect) {
        
        Tweens.Instance.FadeToObject(finishObject, 1f, .5f, 0f);
        Tweens.Instance.FadeToObject(finishObject, 0f, .5f, 3f);
        Tweens.Instance.FadeToObject(overlayFadeObject, 1f, .5f, 1.3f);
        
        yield return null;
        
        if(UIGameAchievement.Instance != null && !backToTrackSelect) {
            float raceTime = RaceManagerScript.Instance.TotalRaceTime;
            UIGameAchievement.Instance.CheckFastestRace(raceTime);
            
            yield return null;
        
            GameRPG.IncrementXP(GameRPGPoints.XP_RACE);
            
            yield return null;
        }
        
        raceActive = false;
        
        GameAudio.StartGameLoop(-1);
        
        yield return null;
                        
        ChangeState(GameHUDState.GAME_FINISH);
        
        yield return null;
                
        if(RaceManagerScript.Instance != null) {                
            GameDatas.SetRaceResultsData(RaceManagerScript.Instance.VehiclesSortedByRaceOrder,
                                         RaceManagerScript.Instance.VehicleScripts);
        }
        
        yield return null;
        
        if(!backToTrackSelect) {
            StartCoroutine(ProcessProgress(backToTrackSelect));
            yield return new WaitForSeconds(6f);
        }   
                    
        StartCoroutine(AdvanceToResults(backToTrackSelect));
    }
    
    
    IEnumerator ProcessProgress(bool backToTrackSelect) {
        yield return null;      
        GamePlayerProgress.Instance.ScoreMode(currentPosition); 
        yield return null;
        SyncHumanVehicleStats();
        GamePlayerProgress.Instance.ProcessRaceResult(humanVehicleStatsData);   
        yield return null;
    }
    
    IEnumerator AdvanceToResults(bool backToTrackSelect) {
        if(!destroyed) {
            //destroyed = true;
            Tweens.Instance.FadeToObject(overlayFadeInfoObject, .4f, .8f, .3f);
                            
            FadeAndStopVehicleSounds();
            
            yield return new WaitForSeconds(1.6f);          
            
            if(GameLoadingObject.Instance != null) {
                GameLoadingObject.Instance.ShowBlack();
                GameLoadingObject.Instance.ShowLoadingHelp();
                Hide();
            }
            
            yield return new WaitForSeconds(1.1f);
                            
            GameAudio.SetVolumeForRace(false);
            GameAudio.StartAmbience();          
        
            yield return null;      
        
            TestFlight.LogSceneExit();
            
            yield return null;      
                        
            if(gameObject != null) {
                //Destroy(gameObject);
            }
            if(backToTrackSelect) {             
                SceneLoader.Instance.LoadSceneRaceSelect();
            }
            else {
                SceneLoader.Instance.LoadSceneRaceResults();
            }                       
                        
            Invoke ("UnloadLevelBundle", 3);
            
            //StopAllCoroutines();
        }
    }

    void UnloadLevelBundle() {  
        Contents.UnloadLevelBundle();
    }
    
    void OnEventRaceLapEvent(int lapNumber) {
        LogUtil.Log("GameRaceHUD::OnEventRaceLapEvent:" + lapNumber);
        
        if(Time.time > lastEventLapTime + 10f ) {       
            int lapNumberItem = lapNumber + 1;
            SetLapCurrentLabel(lapNumberItem);      
            if(lapNumberItem <= 3) {
                GameAudio.StartGameLoop(lapNumberItem);             
                GameRPG.IncrementXP(GameRPGPoints.XP_LAP);
            }
            
            GamePlayerProgress.Instance.SetStatAccumulate(GameStatistics.STAT_LAPS, 1);
            
            if(UIGameAchievement.Instance != null) {
                int lapIndex = lapNumber - 1;
                if(humanVehicle.LapTimes.Length > lapIndex) {
                    float lapTime = humanVehicle.LapTimes[lapIndex];
                    UIGameAchievement.Instance.CheckFastestLap(lapTime);
                }
            }
            
            SyncHumanVehicleStats();
            GamePlayerProgress.Instance.HandleInGameLapAchievements(humanVehicleStatsData, lapNumberItem, false);           
        }           
    }
    
    void SetLapCurrentLabel(int lapNumber) {
        if(labelLapCurrent != null) {
            labelLapCurrent.text = lapNumber.ToString();
        }
    }
    
    void SetLapTotalLabel(int laps) {
        if(labelLapTotal != null) {
            labelLapTotal.text = laps.ToString();
        }
    }
    
    void FindHumanPlayer() {
        foreach(VehicleBehaviourScript vehicle in RaceManagerScript.Instance.VehiclesSortedByRaceOrder) {
            if(!vehicle.AIControlled) {
                humanVehicle = vehicle;
                break;
            }
        }
    }
        
    void UpdatePlayerPosition() {
        if(RaceManagerScript.Instance != null) {
            
            // get human player and their position
            
            if(humanVehicle != null) {
                currentPosition = humanVehicle.GetRacePosition();
            }
            
            if(currentPosition != lastCurrentPosition) {
                // update hud place
                lastCurrentPosition = currentPosition;              
                labelLapPlace.text = GameSeriesEvents.Instance.GetPrettyPlace(currentPosition);
            }           
        }
    }
    
    void OnEventRaceVehiclePass() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceVehicleBoost");      
        
        if(Time.time > lastEventPassTime + 1f) {            
            GameRPG.IncrementXP(GameRPGPoints.XP_PASS);         
            AudioPlayCrowdCheerLow();           
            lastEventPassTime = Time.time;
        }
    }
    
    void OnEventRaceVehiclePassed() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceVehicleBoost");      
        if(Time.time > lastEventPassedTime + 1f) {  
            AudioPlayCrowdBoo();
            GameRPG.IncrementXP(GameRPGPoints.XP_PASSED);
            lastEventPassedTime = Time.time;        
        }
    }
    
    void OnEventRaceVehicleCollision() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceVehicleBoost");
        
        if(Time.time > lastEventCollisionTime + 2f) {           
            AudioPlayCrowdBooLow();
            GameRPG.IncrementXP(GameRPGPoints.XP_COLLIDE);
            lastEventCollisionTime = Time.time;         
        }
    }
    
    void OnEventRaceVehicleCauseCollision() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceVehicleBoost");
        
        if(Time.time > lastEventCauseCollisionTime + 2f ) {
            AudioPlayCrowdCheer();
            GameRPG.IncrementXP(GameRPGPoints.XP_BUMP);
            lastEventCauseCollisionTime = Time.time;
        }
    }
    
    bool playVehicleBoost = true;
    
    void OnEventRaceVehicleBoost() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceVehicleBoost");
        
        if(Time.time > lastEventBoostTime + 3f ) {
            
            if(playVehicleBoost || humanVehicle.DistanceOffGround > 1.5f) {
                AudioPlayCrowdBoost();
            }
            
            playVehicleBoost = !playVehicleBoost;
            
            AudioPlayCrowdCheer();
            
            GameRPG.IncrementXP(GameRPGPoints.XP_BOOST);
            lastEventBoostTime = Time.time;
        }
    }
    
    void OnEventRaceVehicleReset() {
        //LogUtil.Log("GameRaceHUD::OnEventRaceVehicleReset");
        
        if(Time.time > lastEventResetTime + 3f ) {
            lastEventResetTime = Time.time;         
            GameLoadingObject.Instance.ShowAndHideBlack();
        }
    }
    
    void OnEventRaceCountdown(int countdown) {
        //LogUtil.Log("GameRaceHUD::OnEventRaceCountdown:" + countdown);        
        // Show countdown
        if (countdown == 3 
                 && countdownStepCompleted == 0) {
            countdownStepCompleted = countdown;         
            Tweens.Instance.FadeToObject(countdown3Object, 1f, .3f, 0f);
            Tweens.Instance.ScaleToObject(countdown3Object, new Vector3(1f, 1f, 1f), .8f, 0f);
            Tweens.Instance.FadeToObject(labelTouchToSkipObject, 0f, .3f, 0f);
        }
        else if (countdown == 2 
                 && countdownStepCompleted == (countdown + 1)) {
            countdownStepCompleted = countdown;         
            Tweens.Instance.FadeToObject(countdown3Object, 0f, .3f, 0f);            
            Tweens.Instance.FadeToObject(countdown2Object, 1f, .3f, 0f);
            Tweens.Instance.ScaleToObject(countdown2Object, new Vector3(1f, 1f, 1f), .8f, 0f);
        }
        else if(countdown == 1 
           && countdownStepCompleted == (countdown + 1)) {
            countdownStepCompleted = countdown;         
            Tweens.Instance.FadeToObject(countdown2Object, 0f, .3f, 0f);            
            Tweens.Instance.FadeToObject(countdown1Object, 1f, .3f, 0f);
            Tweens.Instance.ScaleToObject(countdown1Object, new Vector3(1f, 1f, 1f), .8f, 0f);
        }
    }   
    
    void GrowHitAreaForInputs() {
        // Make hit area big temp fix   
        if(legacyLeftButton) {
            legacyLeftButton.ScaleTo(new Vector3(0.025f, 0.025f, 0.025f), 4f, 5f);
        }
                    
        if(legacyRightButton) {
            legacyRightButton.ScaleTo(new Vector3(0.025f, 0.025f, 0.025f), 4f, 5f);
        }
    }
    
    void PrepareAndLoadLevel() {    
        
        Tweens.Instance.FadeToObject(overlayFadeInfoObject, .4f, .5f, 0f);
        Tweens.Instance.FadeToObject(overlayFadeObject, 1f, .5f, .5f);
        
        if(GameGlobal.Instance) {
            LogUtil.Log("LoadingLevel:" + GameLevels.Current.name);
            Invoke("LoadLevelDelay", 1);
            //LoadLevelDelay();
        }
    }
    
    void LoadLevelDelay() {
        foreach(AudioListener listener in gameObject.GetComponentsInChildren<AudioListener>()) {
            listener.enabled = false;
        }
        LogUtil.Log("GameLevels.Current.name:" + GameLevels.Current.name);
        
        GameLevel gameLevel = GameLevels.Current;       
        
        TestFlight.LogScenePlayingLevel(gameLevel.name);
        
        if(Contents.IsDownloadableContent(GameLevels.Current.pack[0])) {
            Contents.LoadSceneOrDownloadScenePackAndLoad(GameLevels.Current.pack[0]);
        }
        else {
            LoadLevelHandler();
        }       
    }
    
    public void LoadLevelHandler() {
        levelLoadInProgress = true;
        Messenger<string>.Broadcast(ContentMessages.ContentItemLoadStarted, "Content loading..." );     
        StartCoroutine(LoadLevelHandlerCo());
    }
    
    IEnumerator LoadLevelHandlerCo() {
        
        yield return new WaitForSeconds(.6f);
        
        asyncLevelLoad = Application.LoadLevelAsync(GameLevels.Current.name);
        yield return asyncLevelLoad;
        
        yield return new WaitForSeconds(.4f);
        levelLoadInProgress = false;
        
        Messenger<string>.Broadcast(ContentMessages.ContentItemLoadSuccess, "Content loaded!" );
        
        if(GameLoadingObject.Instance != null) {
            GameLoadingObject.Instance.HideLoadingHelp();
            GameLoadingObject.Instance.ShowReadyDelayed();
        }
        
        LogUtil.Log("LoadLevelItem: Complete");
        Messenger<string>.Broadcast(UIMessages.EventLevelLoaded, GameLevels.Current.name);
        StopCoroutine("LoadLevelHandlerCo");
    }
        
    public void StartLoadedLevelDelayed(int seconds) {
        Invoke("StartLoadedLevel", seconds);
    }
        
    void RestartRace() {
        GameAudio.StartGameLoop(-1);
        
        Hide();
        
        if(GameLoadingObject.Instance != null) {
            GameLoadingObject.Instance.ShowBackground();
            GameLoadingObject.Instance.ShowHelpTips();
            GameLoadingObject.Instance.ShowLoadingHelp();
            GameLoadingObject.Instance.LoadNextTip();
        }
        
        GamePlayerProgress.Instance.SetAchievement(GameAchievements.ACHIEVE_UI_RACE_RESTART, true);
        
        humanVehicleSounds.isRunning = false;
        humanVehicleSounds.raceSoundRunning = false;
                
        SceneLoader.Instance.LoadScene("UISceneRaceSetup");
                
        //Contents.UnloadLevelBundle();
    }
    
    void QuitRace() {
        if(!raceQuit) {
            raceQuit = true;
            raceActive = false;
            FinishRace(true);
            GamePlayerProgress.Instance.SetAchievement(GameAchievements.ACHIEVE_UI_RACE_QUIT, true);
        }
    }   
    
    void ShowOrHideInputs() {
                
        if(legacyLeftButton == null && limitOnLegacyCheck > 0) {
            legacyLeftButton = GameObject.Find("Left Button");
            limitOnLegacyCheck--;
            if(legacyLeftButton != null) {
                legacyLeftButton.renderer.enabled = false;
            }
        }
        if(legacyRightButton == null && limitOnLegacyCheck > 0) {
            legacyRightButton = GameObject.Find("Right Button");
            limitOnLegacyCheck--;
            if(legacyRightButton != null) {
                legacyRightButton.renderer.enabled = false;
            }
        }
                
        if(GameProfiles.Current.GetControlTouch()) {
            // show the inputs
            if(inputLeft) {
                inputLeft.gameObject.SetActiveRecursively(true);
            }
            
            if(inputRight) {
                inputRight.gameObject.SetActiveRecursively(true);
            }           
            
            if(legacyLeftButton) {
                legacyLeftButton.gameObject.SetActiveRecursively(true);
            }
                        
            if(legacyRightButton) {
                legacyRightButton.gameObject.SetActiveRecursively(true);
            }
                        
            if(legacyLeftButton) {
                legacyLeftButton.renderer.enabled = false;
            }           
            
            if(legacyRightButton) {
                legacyRightButton.renderer.enabled = false;
            }
        }
        else {
            if(inputLeft) {
                inputLeft.gameObject.SetActiveRecursively(false);
            }
            
            if(inputRight) {
                inputRight.gameObject.SetActiveRecursively(false);
            }           
            
            if(legacyLeftButton) {
                legacyLeftButton.gameObject.SetActiveRecursively(false);
            }
                        
            if(legacyRightButton) {
                legacyRightButton.gameObject.SetActiveRecursively(false);
            }
        }
    }
    
    void TogglePausePanel() {
        SetTimeScale(1f);
        if(pauseExpanded) { 
            //Tweens.Instance.FadeToObject(panelPauseObject, 0f, 0f, .0f);
            iTween.MoveTo(panelPauseObject, iTween.Hash("x", 20, "time", .3f, "delay", .11f, "easeType", "easeInOutQuad", 
                                                        "oncomplete", "StartTime", "oncompletetarget", gameObject));
            pauseExpanded = false;
        }
        else {
            iTween.MoveTo(panelPauseObject, iTween.Hash("x", -2.5, "time", .5f, "delay", .02f, "easeType", "easeInOutQuad", "oncomplete", "StopTime", "oncompletetarget", gameObject));
            //Tweens.Instance.FadeToObject(panelPauseObject, 0f, 0f, 1f);
            pauseExpanded = true;
        }
    }
    
    public void StopTime() {
        SetTimeScale(0f);
        AudioListener.pause = true;
    }
    
    public void StartTime() {
        //ResetButtonPositions();
        SetTimeScale(1f);
        AudioListener.pause = false;
    }
    
    public void SetTimeScale(float timeScale) {
        Time.timeScale = timeScale;
    }
    
    public void CheckTouchInputState() {
        if(GameSettings.Instance != null) {
            touchInputEnabled = !GameSettings.Instance.TiltControlEnabled;
            lastTouchInputEnabled = touchInputEnabled;
        }
    }
    
    public virtual void LateUpdate() {
        
        if(buttonMeta != null) {
            buttonMeta.ResetButtons();
            
            currentTimeBlock += Time.deltaTime;
            
            if(currentTimeBlock > actionInterval) {
                currentTimeBlock = 0.0f;
                
                buttonMeta.SetButtonsAlertState();
            }
        }
    }
        
    public void SyncHumanVehicleStats() {
        if(humanVehicleStats == null) {
            if(humanVehicle != null) {
                humanVehicleStats = humanVehicle.GetComponent<VehicleStats>();
            }
        }
        
        if(humanVehicleStats != null) {
            if(humanVehicleStatsData == null) {
                humanVehicleStatsData = new VehicleStatsData();
            }
            
            humanVehicleStatsData.BoostsHit = humanVehicleStats.BoostsHit;
            humanVehicleStatsData.CleanLaps = humanVehicleStats.CleanLaps;
            humanVehicleStatsData.CollisionsFromOtherBikes = humanVehicleStats.CollisionsFromOtherBikes;
            humanVehicleStatsData.CollisionsOnOtherBikes = humanVehicleStats.CollisionsOnOtherBikes;
            humanVehicleStatsData.GotHoleShot = humanVehicleStats.GotHoleShot;
            humanVehicleStatsData.LapsInFirstPlace = humanVehicleStats.LapsInFirstPlace;
            humanVehicleStatsData.MilesDriven = humanVehicleStats.MilesDriven;
            humanVehicleStatsData.MudPuddlesHit = humanVehicleStats.MudPuddlesHit;
            humanVehicleStatsData.PrematureStart = humanVehicleStats.PrematureStart;
            humanVehicleStatsData.TimeInFirstPlace = humanVehicleStats.TimeInFirstPlace;
            humanVehicleStatsData.TimesHitOnThisLap = humanVehicleStats.TimesHitOnThisLap;
            humanVehicleStatsData.TimesIPassedSomeone = humanVehicleStats.TimesIPassedSomeone;
            humanVehicleStatsData.TimesPassed = humanVehicleStats.TimesPassed;
        }
    }
    
    public void Update() {
                
        if(initialized) {
            
            FindCameraBehavior();
                
            currentTimeBlockLocal += Time.deltaTime;
            
            if(currentTimeBlockLocal > actionIntervalLocal) {
                currentTimeBlockLocal = 0.0f;
                
                if(humanVehicle) {
                    SyncHumanVehicleStats();
                    GamePlayerProgress.Instance.HandleInGameAchievements(humanVehicleStatsData, false);
                }
            }           
                        
            if(touchInputEnabled != lastTouchInputEnabled
               && raceActive) {
                
                CheckTouchInputState();
                
                lastTouchInputEnabled = touchInputEnabled;
                if(inputLeft != null)  {
                    inputLeft.gameObject.renderer.enabled = touchInputEnabled;
                }
                if(buttonHitLeft != null)  {
                    buttonHitLeft.gameObject.renderer.enabled = touchInputEnabled;
                }
                if(inputRight != null)  {
                    inputRight.gameObject.renderer.enabled = touchInputEnabled;
                }
                if(buttonHitRight != null)  {
                    buttonHitRight.gameObject.renderer.enabled = touchInputEnabled;
                }               
            }
            
            if(raceActive) {
                ShowOrHideInputs();
            }
            
            if(labelTime != null) {
                if(RaceManagerScript.Instance != null) {
                    string time = FormatUtil.GetFormattedTimeMinutesSecondsMs(RaceManagerScript.Instance.TotalRaceTime);
                    labelTime.text = time;
                }
            }
            
        }
        
        currentTimeBlockMicro += Time.deltaTime;
        
        if(currentTimeBlockMicro > actionIntervalMicro) {
            currentTimeBlockMicro = 0.0f;
            
            UpdatePlayerPosition();     
        }
        
    }
    
}

 * 
 * */

