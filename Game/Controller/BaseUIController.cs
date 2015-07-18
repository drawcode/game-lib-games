using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Events;

public enum BaseUIStates {
    NoState,
    InUI,
    InGame,
    InEdit
}

public class BaseUIControllerMessages {
    public static string uiPanelAnimateIn = "ui-panel-animate-in";
    public static string uiPanelAnimateOut = "ui-panel-animate-out";
    public static string uiPanelAnimateInType = "ui-panel-animate-in-type";
    public static string uiPanelAnimateOutType = "ui-panel-animate-out-type";
    public static string uiPanelAnimateInClassType = "ui-panel-animate-in-class-type";
    public static string uiPanelAnimateOutClassType = "ui-panel-animate-out-class-type";
    public static string uiPanelAnimateType = "ui-panel-animate-type";
    public static string uiUpdateTouchLaunch = "ui-update-touch-launch";

    public static string uiShow = "ui-show";
    public static string uiHide = "ui-hide";
}

public class UIControllerAnimateTypes {
    public static string uiPanelAnimateTypeIn = "ui-panel-animate-type-in";
    public static string uiPanelAnimateTypeOut = "ui-panel-animate-type-out";
    public static string uiPanelAnimateTypeInBetween = "ui-panel-animate-type-in-between";
    public static string uiPanelAnimateTypeInternal = "ui-panel-animate-type-internal";
}
 
public class BaseUIButtonNames { 

    public static string buttonBack = "ButtonBack";
    public static string buttonBlank = "ButtonBlank";
    public static string buttonMenu = "ButtonMenu";
    public static string buttonInGame = "ButtonInGame";
    public static string buttonMain = "ButtonMain";
    public static string buttonPlayGame = "ButtonPlayGame";
    public static string buttonPlayWorld = "ButtonPlayWorld";
    public static string buttonPlayLevel = "ButtonPlayLevel";
    public static string buttonPlayLevels = "ButtonPlayLevels";
    public static string buttonSettings = "ButtonSettings";
    public static string buttonGameSettings = "ButtonGameSettings";
    public static string buttonGameEquipment = "ButtonGameEquipment";
    public static string buttonAppRate = "ButtonAppRate";
    public static string buttonTrophy = "ButtonTrophy";
    public static string buttonTrophyStatistics = "ButtonTrophyStatistics";
    public static string buttonTrophyAchievements = "ButtonTrophyAchievements";
    public static string buttonGameStatistics = "ButtonGameStatistics";
    public static string buttonGameAchievements = "ButtonGameAchievements";
    public static string buttonGameLeaderboards = "ButtonGameLeaderboards";
    public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";
    public static string buttonGameCenterAchievements = "ButtonGameCenterAchievements";
    public static string buttonGamePlayServicesLeaderboards = "ButtonGamePlayServicesLeaderboards";
    public static string buttonGamePlayServicesAchievements = "ButtonGamePlayServicesAchievements";
    public static string buttonSocial = "ButtonSocial";
    public static string buttonCredits = "ButtonCredits";
    public static string buttonWorlds = "ButtonWorlds";
    public static string buttonLevels = "ButtonLevels";
    public static string buttonLevel = "ButtonLevel";
    public static string buttonCoin = "ButtonCoin";
    public static string buttonProductCurrency = "ButtonProductCurrency";
    public static string buttonGameSettingsAudio = "ButtonGameSettingsAudio";
    public static string buttonGamePause = "ButtonGamePause";
    public static string buttonGameResume = "ButtonGameResume";
    public static string buttonGameRestart = "ButtonGameRestart";
    public static string buttonGameEquipmentRoom = "ButtonGameEquipmentRoom";
    public static string buttonGameQuit = "ButtonGameQuit";
    public static string buttonGameWorlds = "ButtonGameWorlds";
    public static string buttonGameContinue = "ButtonGameContinue";
    public static string buttonGameLevelItemObject = "ButtonGameLevelItemObject";
    public static string buttonGameProductCurrency = "ButtonGameProductCurrency";
    public static string buttonGameProducts = "ButtonGameProducts";
    public static string buttonGameProductsCharacter = "ButtonGameProductsCharacter";
    public static string buttonGameProductsCharacterSkin = "ButtonGameProductsCharacterSkin";
    public static string buttonGameProductsWeapon = "ButtonGameProductsWeapon";
    public static string buttonGameProductsCurrency = "ButtonGameProductsCurrency";
    public static string buttonGameProductsFeature = "ButtonGameProductsFeature";
    public static string buttonGameProductsPickup = "ButtonGameProductsPickup";
    public static string buttonGameProductsPowerup = "ButtonGameProductsPowerup";
    public static string buttonGameProductsRPGUpgrade = "ButtonGameProductsRPGUpgrade";
    public static string buttonGamePlayerPresets = "ButtonGamePlayerPresets";
    public static string buttonGameActionItemBuyUse = "ButtonGameActionItemBuyUse";
    public static string buttonGameCustomize = "ButtonGameCustomize";
    public static string buttonGameCustomizeCharacter = "ButtonGameCustomizeCharacter";
    public static string buttonGameCustomizeCharacterColors = "ButtonGameCustomizeCharacterColors";
    public static string buttonGameCustomizeCharacterRPG = "ButtonGameCustomizeCharacterRPG";   

    //ButtonCustomizeCharacterRPGBuyUpgrades

    // 
        
    public static string buttonGameModeArcade = "ButtonGameModeArcade";
    public static string buttonGameModeChallenge = "ButtonGameModeChallenge";
    public static string buttonGameModeCoop = "ButtonGameModeCoop";
    public static string buttonGameModeMultiplayerCoop = "ButtonGameModeMultiplayerCoop";
    public static string buttonGameModeMultiplayerMatchup = "ButtonGameModeMultiplayerMatchup";
    public static string buttonGameModeMultiplayer = "ButtonGameModeMultiplayer";
    public static string buttonGameModeMissions = "ButtonGameModeMissions";
    public static string buttonGameModeTraining = "ButtonGameModeTraining";
    public static string buttonGameModeTutorial = "ButtonGameModeTutorial";
    public static string buttonGamePlay = "ButtonGamePlay";
    public static string buttonGameModePlay = "ButtonGameModePlay";
    public static string buttonGameNetworkStopGame = "ButtonGameNetworkStopGame";
    public static string buttonGameNetworkStartGame = "ButtonGameNetworkStartGame";
    public static string buttonGameNetworkJoinGame = "ButtonGameNetworkJoinGame";
    public static string buttonGameInitFinish = "ButtonGameInitFinish";
    //ButtonGameInitFinish

    public static string buttonGameCustomizeCharacterFront = "ButtonGameCustomizeCharacterFront";
    public static string buttonGameCustomizeCharacterBack = "ButtonGameCustomizeCharacterBack";
    public static string buttonGameCustomizeCharacterZoomIn = "ButtonGameCustomizeCharacterZoomIn";
    public static string buttonGameCustomizeCharacterZoomOut = "ButtonGameCustomizeCharacterZoomOut";

    // COMMUNITY
        
    public static string buttonGameCommunityClose = "ButtonGameCommunityClose";
    public static string buttonGameCommunityCameraSaveTwitter = "ButtonGameCommunityCameraSaveTwitter";
    public static string buttonGameCommunityCameraSaveFacebook = "ButtonGameCommunityCameraSaveFacebook";
    public static string buttonGameCommunityCameraSaveLibrary = "ButtonGameCommunityCameraSaveLibrary";
    public static string buttonGameCommunityCameraTakePhoto = "ButtonGameCommunityCameraTakePhoto";
    public static string buttonGameCommunityShareResultFacebook = "ButtonGameCommunityShareResultFacebook";
    public static string buttonGameCommunityShareResultTwitter = "ButtonGameCommunityShareResultTwitter";
    public static string buttonGameCommunityBroadcastNetworkOpen = "ButtonGameCommunityBroadcastNetworkOpen";
    public static string buttonGameCommunityBroadcastOpen = "ButtonGameCommunityBroadcastOpen";
    public static string buttonGameCommunityBroadcastOpenShare = "ButtonGameCommunityBroadcastOpenShare";
    public static string buttonGameCommunityBroadcastReplayShare = "ButtonGameCommunityBroadcastReplayShare";
    public static string buttonGameCommunityBroadcastReplayWatch = "ButtonGameCommunityBroadcastReplayWatch";
    public static string buttonGameCommunityBroadcastClose = "ButtonGameCommunityBroadcastClose";
    public static string buttonGameCommunityBroadcastPlayLast = "ButtonGameCommunityBroadcastPlayLast";
    public static string buttonGameCommunityBroadcastShareLast = "ButtonGameCommunityBroadcastShareLast";
    public static string buttonGameCommunityBroadcastRecordStart = "ButtonGameCommunityBroadcastRecordStart";
    public static string buttonGameCommunityBroadcastRecordStop = "ButtonGameCommunityBroadcastRecordStop";
    public static string buttonGameCommunityBroadcastRecordToggle = "ButtonGameCommunityBroadcastRecordToggle";
    public static string buttonGameCommunityBroadcastFacecamStart = "ButtonGameCommunityBroadcastFacecamStart";
    public static string buttonGameCommunityBroadcastFacecamStop = "ButtonGameCommunityBroadcastFacecamStop";
    public static string buttonGameCommunityBroadcastFacecamToggle = "ButtonGameCommunityBroadcastFacecamToggle";

    // broadcast
    
    public static string buttonGameBroadcastStart = "ButtonGameBroadcastStart";
    public static string buttonGameBroadcastStop = "ButtonGameBroadcastStop";
    public static string buttonGameBroadcastShow = "ButtonGameBroadcastShow";

    //public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";
    //public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";
    //public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";
    //public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";
    //public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";


}

public class BaseHUDButtonNames {
    public static string buttonInputAttack = "ButtonInputAttack";
    public static string buttonInputAttackRight = "ButtonInputAttackRight";
    public static string buttonInputAttackLeft = "ButtonInputAttackLeft";
    public static string buttonInputAttackAlt = "ButtonInputAttackAlt";
    public static string buttonInputDefend = "ButtonInputDefend";
    public static string buttonInputDefendRight = "ButtonInputDefendRight";
    public static string buttonInputDefendLeft = "ButtonInputDefendLeft";
    public static string buttonInputDefendAlt = "ButtonInputDefendAlt";
    public static string buttonInputSkill = "ButtonInputSkill";
    public static string buttonInputMagic = "ButtonInputMagic";
    public static string buttonInputJump = "ButtonInputJump";
    public static string buttonInputUse = "ButtonInputUse";
    public static string buttonInputMount = "ButtonInputMount";
    public static string buttonInputInventoryWeapon = "ButtonInputInventoryWeapon";
    public static string buttonInputInventoryWeaponNext = "ButtonInputInventoryWeaponNext";
    public static string buttonInputInventoryWeaponPrev = "ButtonInputInventoryWeaponPrev";
    public static string buttonInputInventoryItem = "ButtonInputInventoryItem";
    public static string buttonInputInventoryItemNext = "ButtonInputInventoryItemNext";
    public static string buttonInputInventoryItemPrev = "ButtonInputInventoryItemPrev";
}

public class BaseUIPanel {
    public static string panelBlank = "PanelBlank";
    public static string panelInGame = "PanelInGame";
    public static string panelHeader = "PanelHeader";
    public static string panelNavigation = "PanelNavigation";
    public static string panelMenu = "PanelMenu";
    public static string panelMain = "PanelMain";
    public static string panelWorlds = "PanelWorlds";
    public static string panelLevels = "PanelLevels";
    public static string panelLevel = "PanelLevel";
    public static string panelGame = "PanelGame";
    public static string panelHUD = "PanelHUD";
    public static string panelSettings = "PanelSettings";
    public static string panelSettingsAudio = "PanelSettingsAudio";
    public static string panelSettingsControls = "PanelSettingsControls";
    public static string panelSettingsProfile = "PanelSettingsProfile";
    public static string panelSettingsHelp = "PanelSettingsHelp";
    public static string panelSettingsCredits = "PanelSettingsCredits";
    public static string panelGameMode = "PanelGameMode";
    public static string panelGameModeCoop = "PanelGameModeCoop";
    public static string panelGameModeMultiplayer = "PanelGameModeMultiplayer";
    public static string panelGameModeMultiplayerCoop = "PanelGameModeMultiplayerCoop";
    public static string panelGameModeMultiplayerMatchup = "PanelGameModeMultiplayerMatchup";
    public static string panelGameModeMissions = "PanelGameModeMissions";
    public static string panelGameModeArcade = "PanelGameModeArcade";
    public static string panelGameModeCareer = "PanelGameModeCareer";
    public static string panelGameModeChallenge = "PanelGameModeChallenge";
    public static string panelGameModeTraining = "PanelGameModeTraining";
    public static string panelGameModeCustomize = "PanelGameModeCustomize";
    public static string panelGameModeTrainingMode = "PanelGameModeTrainingMode";
    public static string panelGameModeTrainingModeChoice = "PanelGameModeTrainingModeChoice";
    public static string panelGameModeTrainingModeCollection = "PanelGameModeTrainingModeChoice";
    public static string panelGameModeTrainingModeContent = "PanelGameModeTrainingModeContent";
    public static string panelGameModeTrainingModeRPGHealth = "PanelGameModeTrainingModeRPGHealth";
    public static string panelGameModeTrainingModeRPGEnergy = "PanelGameModeTrainingModeRPGEnergy";
    public static string panelStore = "PanelStore";
    public static string panelCredits = "PanelCredits";
    public static string panelSocial = "PanelSocial";
    public static string panelTrophy = "PanelTrophy";
    public static string panelResults = "PanelResults";
    public static string panelTrophyStatistics = "PanelTrophyStatistics";
    public static string panelTrophyAchievements = "PanelTrophyAchievements";
    public static string panelEquipment = "PanelEquipment";
    public static string panelStatistics = "PanelStatistics";
    public static string panelAchievements = "PanelAchievements";
    public static string panelProducts = "PanelProducts";
    public static string panelProductCurrency = "PanelProductCurrency";
    public static string panelProductCurrencyEarn = "PanelProductCurrencyEarn";
    public static string panelCustomize = "PanelCustomize";
    public static string panelCustomizeLevels = "PanelCustomizeLevels";
    public static string panelCustomizeWorlds = "PanelCustomizeWorlds";
    public static string panelCustomizeCharacter = "PanelCustomizeCharacter";
    public static string panelCustomizeCharacterColors = "PanelCustomizeCharacterColors";
    public static string panelCustomizeCharacterRPG = "PanelCustomizeCharacterRPG";
    public static string panelCustomizeAudio = "PanelCustomizeAudio";
    public static string panelCustomSafety = "PanelCustomSafety";
    public static string panelCustomSmarts = "panelCustomSmarts";
    public static string panelCommunityCamera = "PanelCommunityCamera";
    public static string panelCommunityComment = "PanelCommunityComment";

}

public class BaseUIController : GameObjectBehavior { 
 
    public static BaseUIController BaseInstance;
    public bool uiVisible = true;
    public bool hasBeenClicked = false;
    public bool inModalAction = true;
    public bool dialogActive = false;
    public bool hudVisible = false;
    public bool deferTap = false;
    public GameObject gameContainerObject;
    public GameObject gamePauseDialogObject;
    public GameObject gamePauseButtonObject;
    public GameObject gameBackgroundAlertObject;
    public string currentPanel = "";
    public bool gameUIExpanded = true;
    public bool gameLoopsStarted = false;
    public bool inUIAudioPlaying = false;
    public GameObject currentDraggableGameObject = null;
    public GameObject currentDraggableUIGameObject = null;
    public Vector3 positionStart;
    public Vector3 positionEnd;
    public Vector3 positionLastLaunch;
    public float powerDistance;
    public Vector3 positionLastLaunchedNormalized;
    public GameObject pointStartObject;
    public GameObject pointEndObject;
    public UnityEngine.Object prefabPointStart;
    public UnityEngine.Object prefabPointEnd;
    public bool isCreatingStart = false;
    public bool isCreatingEnd = false;
    public Camera camHud = null;
    public Camera camUI = null;
    public Camera camDialog = null;
    public Camera camOverlay = null;
    float updateTouchStartTime = 0f;
    float updateTouchMaxTime = 2f;
    bool inputGestureDown = false;
    bool inputGestureUp = false;
    bool showPoints = false;
    public bool allowedTouch = true;
    public bool inputButtonDown = false;
    public bool inputAxisDown = false;
    public bool shouldTouch = false;
    float lastPressAttack = 0;
    float lastPressAttackAlt = 0;
    float lastPressAttackRight = 0;
    float lastPressAttackLeft = 0;
    float lastPressDefend = 0;
    float lastPressDefendAlt = 0;
    float lastPressDefendRight = 0;
    float lastPressDefendLeft = 0;
    float lastPressSkill = 0;
    float lastPressMagic = 0;
    float lastPressUse = 0;
    float lastPressMount = 0;
    //float lastPressJump = 0;
                 
    public virtual void Awake() {
     
    }

    public virtual void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<GameObject>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectEventHandler);

        //Messenger<string, string, bool>.AddListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
        //Messenger<string, string>.AddListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);
     
        //Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
     
        //Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
             
        //Messenger<GameObject>.AddListener(
        // GameDraggableEditorMessages.editorGrabbedObjectChanged, OnEditorGrabbedObjectChanged);
          
        Messenger<TapGesture>.AddListener(FingerGesturesMessages.OnTap, 
         FingerGestures_OnTap);
     
        Messenger<DragGesture>.AddListener(FingerGesturesMessages.OnDrag, 
         FingerGestures_OnDragMove);
     
        Messenger<SwipeGesture>.AddListener(FingerGesturesMessages.OnSwipe, 
         FingerGestures_OnSwipe);
     
        Messenger<PinchGesture>.AddListener(FingerGesturesMessages.OnPinch, 
         FingerGestures_OnPinchMove);
     
        Messenger<TwistGesture>.AddListener(FingerGesturesMessages.OnTwist, 
         FingerGestures_OnRotationMove);
     
        Messenger<LongPressGesture>.AddListener(FingerGesturesMessages.OnLongPress, 
         FingerGestures_OnLongPress);
     
        Messenger<TapGesture>.AddListener(FingerGesturesMessages.OnDoubleTap, 
         FingerGestures_OnDoubleTap);
    }
    
    public virtual void OnDisable() {
    
        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
        
        Messenger<GameObject>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectEventHandler);

        //Messenger<string, string, bool>.RemoveListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
        //Messenger<string, string>.RemoveListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);     
        //Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);     
        //Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
     
        //Messenger<GameObject>.RemoveListener(
        // GameDraggableEditorMessages.editorGrabbedObjectChanged, OnEditorGrabbedObjectChanged);
             
        Messenger<TapGesture>.RemoveListener(FingerGesturesMessages.OnTap, 
         FingerGestures_OnTap);
     
        Messenger<DragGesture>.RemoveListener(FingerGesturesMessages.OnDrag, 
         FingerGestures_OnDragMove);
     
        Messenger<SwipeGesture>.RemoveListener(FingerGesturesMessages.OnSwipe, 
         FingerGestures_OnSwipe);
     
        Messenger<PinchGesture>.RemoveListener(FingerGesturesMessages.OnPinch, 
         FingerGestures_OnPinchMove);
     
        Messenger<TwistGesture>.RemoveListener(FingerGesturesMessages.OnTwist, 
         FingerGestures_OnRotationMove);
     
        Messenger<LongPressGesture>.RemoveListener(FingerGesturesMessages.OnLongPress, 
         FingerGestures_OnLongPress);
     
        Messenger<TapGesture>.RemoveListener(FingerGesturesMessages.OnDoubleTap, 
         FingerGestures_OnDoubleTap);
    }
        
    public virtual void Start() {
        LoadData();
        //ShowContainerByName(BaseUIButtonNames.buttonContent);
        //Invoke("ShowMainMenuDelayed", 10);
        HideAllPanels();
        showMain();
    }

    public void broadcastUIMessageAnimateIn(string objName) {
        Messenger<string>.Broadcast(UIControllerMessages.uiPanelAnimateIn, objName);
    }

    public void broadcastUIMessageAnimateOut(string objName) {
        Messenger<string>.Broadcast(UIControllerMessages.uiPanelAnimateOut, objName);
    }

    public void broadcastUIMessageAnimateType(string objName, string code) {
        Messenger<string, string>.Broadcast(UIControllerMessages.uiPanelAnimateType, objName, code);
    }

    public static bool IsUIPanel(string panelCodeCheck) {

        if (GameUIController.isInst) {
            return GameUIController.Instance.isUIPanel(panelCodeCheck);
        }

        return false;
    }

    public bool isUIPanel(string panelCodeCheck) {
        if (currentPanel == panelCodeCheck) {
            return true;
        }

        return false;
    }

    public static bool IsUIPanelLike(string panelCodeCheck) {
        
        if (GameUIController.isInst) {
            return GameUIController.Instance.isUIPanelLike(panelCodeCheck);
        }
        
        return false;
    }

    public bool isUIPanelLike(string panelCodeCheck) {
        if (currentPanel.Contains(panelCodeCheck)) {
            return true;
        }
        
        return false;
    }

    public void showUIPanel(object obj, string panelCode, string title) {
        Type objType = obj.GetType();
        string objName = objType.Name;
        showUIPanel(objName, panelCode, title);
    }

    public void showUIPanel(Type objType, string panelCode, string title) {
        showUIPanel(objType.Name, panelCode, title);
    }

    public void showUIPanel(string objName, string panelCode, string title) {

        currentPanel = panelCode;

        AnalyticsNetworks.LogEventSceneChange(panelCode, title);

        HideAllPanelsNow();

        if (panelCode != BaseUIPanel.panelMain) {
            broadcastUIMessageAnimateType(
                "GameUIPanelBackgrounds",
                UIControllerAnimateTypes.uiPanelAnimateTypeInBetween); // starry
        }

        //GameUIPanelBackgrounds.Instance.AnimateInStarry();

        broadcastUIMessageAnimateType(
            "GameUIPanelHeader",
            UIControllerAnimateTypes.uiPanelAnimateTypeInternal); // starry

        //GameUIPanelHeader.Instance.AnimateInInternal();

        GameUIPanelHeader.ShowTitle(title);

        broadcastUIMessageAnimateIn(objName); // animate in

        // TODO base
        GameCustomController.BroadcastCustomSync();

        UIColors.UpdateColors();

        //GameUIPanelGameModeTrainingModeChoiceQuiz.Instance.AnimateIn();
    }

    public void hideUIPanel(object obj) {
        Type objType = obj.GetType();
        string objName = objType.Name;
        hideUIPanel(objName); // animate out
    }

    public void hideUIPanel(Type objType) {
        string objName = objType.Name;
        hideUIPanel(objName); // animate out
    }

    public void hideUIPanel(string objName) {
        broadcastUIMessageAnimateOut(objName); // animate out
    }
 
    public virtual void HideAllPanels() {
        foreach (GameUIPanelBase baseItem in FindObjectsOfType(typeof(GameUIPanelBase))) {
            baseItem.AnimateOut();
        }
     
        foreach (UIPanelBase baseItem in FindObjectsOfType(typeof(UIPanelBase))) {
            baseItem.AnimateOut();
        }
    }
 
    public virtual void HideAllPanelsNow() {
        foreach (GameUIPanelBase baseItem in FindObjectsOfType(typeof(GameUIPanelBase))) {
            baseItem.AnimateOutNow();
        }
     
        foreach (UIPanelBase baseItem in FindObjectsOfType(typeof(UIPanelBase))) {
            baseItem.AnimateOutNow();
        }
    }
 
    public virtual void LoadData() {
     
    }
    
    public virtual void OnListItemClickEventHandler(
        string listName, string listIndex, bool selected) {
        LogUtil.Log("OnListItemClickEventHandler: listName:" + 
            listName + " listIndex:" + listIndex.ToString() + " selected:" + selected.ToString());
    }
    
    public virtual void OnListItemSelectEventHandler(
        string listName, string selectName) {
        LogUtil.Log("OnListItemSelectEventHandler: listName:" + 
            listName + " selectName:" + selectName);
    }
    
    public virtual void OnSliderChangeEventHandler(
        string sliderName, float sliderValue) {
        LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + 
            sliderName + " sliderValue:" + sliderValue);
    }
    
    public virtual void OnCheckboxChangeEventHandler(
        string checkboxName, bool selected) {
        LogUtil.Log("OnCheckboxChangeEventHandler: checkboxName:" + 
            checkboxName + " selected:" + selected);
    }
    
    public virtual void OnApplicationQuit() {
        GameState.SaveProfile();
    }

    // NOTIFICATIONS


    public virtual void showUITip(string title, string description) {
        UINotificationDisplayTip.Instance.QueueTip(title, description);            
    }
    
    public virtual void ShowMainMenuDelayed() {
        if (!hasBeenClicked) {
            showMain();
        }
    }
    
    public virtual void Update() {

        if (Application.isEditor) {

            if (Input.GetKey(KeyCode.LeftControl)) {
                
                if (Input.GetKey(KeyCode.Space)) {

                    if (Input.GetKeyDown(KeyCode.Period)) {
                        GameController.GamePlayerUse();
                    }
                    
                    if (Input.GetKeyDown(KeyCode.C)) {
                        //UIPanelCommunityCamera.CaptureCameraPhotoState();
                    }
                }
            }
        }

        //DetectSwipe();
        GameUIController.UpdateTouchLaunch();
    }

    public virtual void FingerGestures_OnDragMove(DragGesture gesture) { //Vector2 fingerPos, Vector2 delta) {
        Vector2 fingerPos = gesture.Position;
        Vector2 delta = gesture.TotalMove;
     
        if (!IsInputAllowed()) {
            return;
        }

        if (currentDraggableGameObject != null) {
            //DragObject(currentDraggableGameObject, fingerPos, delta);
        }

        if (currentDraggableUIGameObject != null) {
            DragObject(currentDraggableUIGameObject, fingerPos, delta);
        }

        HandleRotators(camUI, fingerPos, delta);

    }

    public virtual void HandleRotators(Camera cam, Vector2 fingerPos, Vector2 delta) {
        
        GameObject goRotator = GameObjectHelper.HitObject(
            cam, 
            Vector3.zero
            .WithX(fingerPos.x)
            .WithY(fingerPos.y), 
            "rotator");
        
        //Debug.Log("goRotator:" + goRotator);
        
        if (goRotator != null) {            
            //Debug.Log("goRotator:FOUND:" + goRotator);
            
            DragObject(goRotator, fingerPos, delta);        
        }
        else {            
            //Debug.Log("goRotator:NOTFOUND:" + goRotator);
        }
        
        Messenger<Vector2,Vector2>.Broadcast(InputSystem.EVENT_INPUT_DRAG_MOVE, fingerPos, delta);
    }

    public virtual void DragObject(GameObject go, Vector2 fingerPos, Vector2 delta) {
        if (go != null) {
         
            deferTap = true;

            Rigidbody rb = go.GetComponent<Rigidbody>();
         
            if (rb == null) {
                go.AddComponent<Rigidbody>();
                rb = go.GetComponent<Rigidbody>();
                rb.constraints =
                    RigidbodyConstraints.FreezePosition
                    | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationZ;
                rb.useGravity = false;
                rb.angularDrag = 3f;
            }
         
            go.transform.localRotation = 
                Quaternion.Euler(go.transform.localRotation.eulerAngles.WithY(-delta.x));

            if (Math.Abs(delta.x) > .05f) {
                rb.angularVelocity = (new Vector3(0, -delta.x / 50, 0));                
            }
            else {
                rb.angularVelocity = Vector3.zero;
            }

            //GamePlayerProgress.Instance.ProcessProgressSpins();
        }
    }

    public virtual void FingerGestures_OnPinchMove(PinchGesture gesture) {
        //Vector2 fingerPos1 = gesture.Fingers[0].Position;
        //Vector2 fingerPos2 = gesture.
        //float delta = gesture.Delta;
        
        if (!IsInputAllowed()) {
            return;
        }
        //ScaleCurrentObjects(delta);
    }

    public virtual void FingerGestures_OnRotationMove(TwistGesture gesture) {
        //Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
        //float rotationAngleDelta = gesture.DeltaRotation;
        if (!IsInputAllowed()) {
            return;
        }
        // RotateCurrentObjects(Vector3.zero.WithY(rotationAngleDelta));
    }

    public virtual void FingerGestures_OnLongPress(LongPressGesture gesture) {
        Vector2 pos = gesture.Position;
        if (!IsInputAllowed()) {
            return;
        }
       
        if (currentDraggableGameObject != null) {
            LongPressObject(currentDraggableGameObject, pos);                
        }
    }

    public virtual void LongPressObject(GameObject go, Vector2 pos) {
        if (go != null) {

            Rigidbody rb = go.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.angularVelocity = Vector3.zero;
            }
            deferTap = true;

            //ResetObject(go);
        }
    }

    public virtual void FingerGestures_OnTap(TapGesture gesture) {//Vector2 fingerPos) {
        //Vector2 fingerPos = gesture.Position;
        //LogUtil.Log("FingerGestures_OnTap", fingerPos);
        if (!IsInputAllowed()) {
            return;
        }
     
        //bool allowTap = true;
        
        if (currentDraggableGameObject != null) {
            //TapObject(currentDraggableGameObject, fingerPos, allowTap);
        }
    }

    public virtual void TapObject(GameObject go, Vector2 fingerPos, bool allowTap) {
        if (go != null) {
            deferTap = !allowTap;

            //LogUtil.Log("Tap:" + fingerPos);
            //LogUtil.Log("Tap:Screen.Height:" + Screen.height);

            float heightToCheck = Screen.height - Screen.height * .85f;
            // LogUtil.Log("Tap:heightToCheck:" + heightToCheck);

            if (fingerPos.y < heightToCheck) {
                deferTap = true;
            }

            // LogUtil.Log("Tap:deferTap:" + deferTap);

            if (!deferTap) {

                //var fwd = transform.TransformDirection(Vector3.forward);
                //Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                //print("hit an object:" + hit.transform.name);

                //if(hit.transform.name == "UILoaderContainer") {
                //   GameObject loaderCube = hit.transform.gameObject;
                //   if(loaderCube != null) {
                //UILoaderContainer loaderContainer = loaderCube.GetComponent<UILoaderContainer>();
                //if(loaderContainer != null) {
                //   if(loaderContainer.placeholderObject != null) {
                //       loaderContainer.placeholderObject.SetPlaceholderObject();
                //   }
                //}
                //   }
                //}
                //}

                //if (!AppViewerUIController.Instance.uiVisible) {
                //AppViewerAppController.Instance.ChangeActionNext();
                // GamePlayerProgress.Instance.ProcessProgressTaps();
                //}
            }
            else {
                deferTap = false;
            }
        }
    }
 
    public virtual void DoubleTapObject(GameObject go, Vector2 pos) {
        if (go != null) {
            
            Rigidbody rb = go.GetComponent<Rigidbody>();
            
            if (rb != null) {
                rb.angularVelocity = Vector3.zero;
                deferTap = true;
            }

            //ResetObject(go);
        }
    }

    public virtual void FingerGestures_OnDoubleTap(TapGesture gesture) {
        if (!IsInputAllowed()) {
            return;
        }                
     
        if (gesture.Taps == 2) {
     
            if (currentDraggableGameObject != null) {
                DoubleTapObject(currentDraggableGameObject, gesture.Position);
            }
        }
     
        //var fwd = transform.TransformDirection(Vector3.forward);
        ////Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
        ////RaycastHit hit;
        ////if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
        ////    print("double tap hit an object:" + hit.transform.name);
        ////}

        //AppViewerAppController.Instance.ChangeActionNext();
    }

    public virtual void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, 
     FingerGestures.SwipeDirection direction, float velocity) {
        if (!IsInputAllowed()) {
            return;
        }

        if (direction == FingerGestures.SwipeDirection.All) {

            // if swiped any direction
        }

        if (direction == FingerGestures.SwipeDirection.Right
            || direction == FingerGestures.SwipeDirection.Down) {

            //AppViewerAppController.Instance.ChangeActionPrevious();
        }
        else if (direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {

            //AppViewerAppController.Instance.ChangeActionNext();
        }
    }

    public virtual void FingerGestures_OnSwipe(SwipeGesture gesture) {

        //Vector2 startPos = gesture.StartPosition;
        FingerGestures.SwipeDirection direction = gesture.Direction;
        //float velocity = gesture.Velocity;
     
        if (!IsInputAllowed()) {
            return;
        }

        bool allowSwipe = true;//AppViewerAppController.Instance.AllowCurrentActionAdvanceSwipe();

        if (direction == FingerGestures.SwipeDirection.Right
            || direction == FingerGestures.SwipeDirection.Down) {
            //if (!AppViewerUIController.Instance.uiVisible) {
            if (allowSwipe) {
                //AppViewerAppController.Instance.ChangeActionPrevious();
            }
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
        }
        else if (direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {
            //if (!AppViewerUIController.Instance.uiVisible) {
            if (allowSwipe) {
                //AppViewerAppController.Instance.ChangeActionNext();
            }
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
        }

        /*
        Vector2 swipeDirectionValue = gesture.Move;

        Vector3 dir = GameController.CurrentGamePlayerController.thirdPersonController.movementDirection;//GameController.CurrentGamePlayerController.transform.position;//swipeDirectionValue;//GameController.CurrentGamePlayerController.thirdPersonController.aimingDirection;

        Vector3 dirMovement =
            Vector3.zero
                .WithX(GameController.CurrentGamePlayerController.thirdPersonController.horizontalInput)
                .WithY(GameController.CurrentGamePlayerController.thirdPersonController.verticalInput);

        float angle =  Vector3.Angle(
            swipeDirectionValue,
            dirMovement);
        float force = 100f;

        if(angle > 320 && angle < 45) { // forwardish
            LogUtil.Log("swipe controller: FORWARD :angle:" + angle);
            GameController.CurrentGamePlayerController.Boost(force);
        }
        else if(angle < 225 && angle > 135) { // backish
            LogUtil.Log("swipe controller: BACK :angle:" + angle);
            GameController.CurrentGamePlayerController.Spin(force);
        }
        else if(angle > 45 && angle < 135) { // leftish
            LogUtil.Log("swipe controller: LEFT :angle:" + angle);
            GameController.CurrentGamePlayerController.StrafeLeft(force);
        }
        else if(angle > 320 && angle < 225) { // rightish
            LogUtil.Log("swipe controller: RIGHT :angle:" + angle);
            GameController.CurrentGamePlayerController.StrafeRight(force);
        }
        */
    }

    /*
    public Vector3 swipeCurrentStartPoint;
    public Vector3 swipeCurrentEndPoint;
    public Vector3 swipePositionLastTouch;
    public Vector3 swipePositionStart;
    public Vector3 swipePositionRelease;

    public void UpdateProjectile() {
            
        if(gameState == GameGameState.GamePause
                    || GameDraggableEditor.isEditing) {
            return;
        }

        if(gameState == GameGameState.GameStarted
                    && runtimeData.timeRemaining > 0) {
                            
            // SHOOT
                    
            //CheckForGameOver();
                    
            bool touchDown = false;
            bool touchUp = false;
                    
            if(Application.isEditor) {
                touchDown = Input.GetMouseButtonDown(0);
                touchUp = Input.GetMouseButtonUp(0);
            }
            else {
                touchDown = Input.touches.Length > 0 ? true : false;
                touchUp = !touchDown;
            }
                    
            if(swipePositionStart != Vector3.zero
                            && swipePositionRelease == Vector3.zero) {
                            
                swipeCurrentStartPoint = Input.mousePosition;
                swipeCurrentEndPoint = swipeCurrentStartPoint;
            }
            else {                          
                //launcherObject.transform.localScale = Vector3.one;
                            
                //if(launcherObject != null && launchAimerObject != null) {
                //      swipeCurrentStartPoint = launcherObject.transform.position;
                //      swipeCurrentEndPoint = launchAimerObject.transform.position;
                swipeCurrentStartPoint.z = 0f;
                swipeCurrentEndPoint.z = 0f;
                //}
            }

            Vector3 angles = CardinalAngles(swipeCurrentStartPoint, swipeCurrentEndPoint);//Vector3.zero.WithZ (
            //Vector3.Angle(swipeCurrentEndPoint, swipeCurrentStartPoint));//CardinalAngles(swipeCurrentStartPoint, swipeCurrentEndPoint);
            //LogUtil.Log("cardinalAngles:" + cardinalAngles);                        
                    
            var dist = Vector3.Distance(swipeCurrentStartPoint, swipeCurrentEndPoint);
                    
            Quaternion rotationTo = Quaternion.Euler(0, 0, angles.z);

            if(touchDown) {
                            
                swipePositionLastTouch = Input.mousePosition;
                if(swipePositionStart == Vector3.zero) {
                    swipePositionStart = swipePositionLastTouch;
                    LogUtil.Log("swipePositionStart:" + swipePositionStart);
                    swipePositionRelease = Vector3.zero;
                }                               
            }
            else if(touchUp) {
                if(swipePositionStart != Vector3.zero) {
                    if(swipePositionRelease == Vector3.zero) {
                        swipePositionRelease = Input.mousePosition;
                        LogUtil.Log("swipePositionRelease:" + swipePositionRelease);
                        // Shoot
                                            
                        Quaternion rotationProjectile = Quaternion.Euler(90, 0, 0);
                            
                        GameObject projectileObject = Instantiate(
                                                    prefabProjectile, //Resources.Load("Prefabs/GameProjectile"), 
                                                    currentGamePlayerController.gamePlayerModelHolderModel.transform.position, 
                                                    Quaternion.Euler(90, 0, 0)
                                            ) as GameObject;
                                            
                                            
                        LogUtil.Log("rotationProjectile:" + rotationProjectile);
                                            
                        projectileObject.transform.rotation = rotationProjectile;
                                            
                        //LogUtil.Log("launcherObject.transform.position:" + launcherObject.transform.position);
                        //LogUtil.Log("launchAimerObject.transform.position:" + launchAimerObject.transform.position);
                                            
                        swipeCurrentStartPoint = swipePositionRelease;
                        swipeCurrentEndPoint = swipePositionStart;
                        swipeCurrentStartPoint.z = 0f;
                        swipeCurrentEndPoint.z = 0f;
                                            
                        Vector3 crossProduct = Vector3.Cross(swipeCurrentStartPoint, swipeCurrentEndPoint);         
                                            
                        var angle = Vector3.Angle(swipeCurrentStartPoint, swipeCurrentEndPoint);
                        LogUtil.Log("Angle to other: " + angle);  
                                            
                        //var forward = transform.forward;
                        if(crossProduct.y < 0) {
                            //Do left stuff
                            //launcherObject.transform.Rotate(0, 0, -angle);
                        }
                        else {
                            //Do right stuff
                            //launcherObject.transform.Rotate(0, 0, angle);
                        }
                                            
                        //gameProjectile.direction = crossProduct;
                        LogUtil.Log("crossProduct to other: " + crossProduct);    
                                            
                        var distLaunch = Vector3.Distance(swipeCurrentStartPoint, swipeCurrentEndPoint);
                        print("Distance to other: " + distLaunch);
                        //distLaunch = 1;
                                            
                        LogUtil.Log("Rotation:" + projectileObject.transform.rotation);
                        LogUtil.Log("angle:" + angle);
                                            
                        var shootVector = swipeCurrentStartPoint - swipeCurrentEndPoint;                  
                        var multiplier = .001f;//.05f;
                        float forceAdd = distLaunch * multiplier;
                        LogUtil.Log("forceAdd:" + forceAdd);
                        forceAdd = Mathf.Clamp(forceAdd, .01f, .9f);
                        projectileObject.rigidbody.AddForce(-shootVector * forceAdd);//, ForceMode.Impulse);
                                            
                        swipePositionStart = Vector3.zero;
                        swipePositionRelease = Vector3.zero;
                                            
                        ShootOne();
                    }
                }
            }
        }
    }
    */

    public virtual void handleTouchLaunch(Vector2 move) {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        float force = 20f;
        //LogUtil.Log("SWIPE:move:" + move);
        float angleGesture = move.CrossAngle();
        float anglePlayer = GameController.CurrentGamePlayerController.transform.rotation.eulerAngles.y;
        float distance = Vector2.Distance(Vector2.zero, move);

        force = distance;

        force = Mathf.Clamp(force, 0f, 60f);

        //angleGesture = Mathf.Abs(angleGesture - anglePlayer);
        //anglePlayer = 0;

        float angleDiff = angleGesture - anglePlayer;

        //LogUtil.Log("SWIPE:angleGesture:" + angleGesture);

        //LogUtil.Log("SWIPE:anglePlayer:" + anglePlayer);
        //LogUtil.Log("SWIPE:angleDiff:" + angleDiff);

        if (angleDiff < 0) {
            angleDiff = angleDiff + 360;
            angleDiff = Mathf.Abs(angleDiff);
        }
        //if(angleDiff > 180) {
        //    angleDiff = 360 - angleDiff;
        //}

        //LogUtil.Log("SWIPE:angleDiff2:" + angleDiff);

        var forceVector = Quaternion.AngleAxis(angleDiff, transform.up) *
            GameController.CurrentGamePlayerController.transform.forward;

        //forceVector.y = 0f;

        if (angleDiff > 320 || angleDiff <= 45) { // forwardish
            LogUtil.Log("swipe controller: FORWARD :angleDiff:" + angleDiff);
            GameController.CurrentGamePlayerController.Boost(forceVector, force * 1.2f);
        }
        else if (angleDiff < 225 && angleDiff >= 135) { // backish
            LogUtil.Log("swipe controller: BACK :angleDiff:" + angleDiff);
            GameController.CurrentGamePlayerController.Spin(forceVector, force * 1.8f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.spins, 1f);
        }
        else if (angleDiff > 45 && angleDiff < 135) { // rightish
            LogUtil.Log("swipe controller: RIGHT :angleDiff:" + angleDiff);
            GameController.CurrentGamePlayerController.StrafeRight(forceVector, force * 2f);
        }
        else if (angleDiff <= 320 && angleDiff >= 225) { // leftish
            LogUtil.Log("swipe controller: LEFT :angleDiff:" + angleDiff);
            GameController.CurrentGamePlayerController.StrafeLeft(forceVector, force * 2f);
        }
    }

    public virtual void updateTouchLaunch() {

        if (!GameConfigs.isGameRunning) {
            return;
        }

        shouldTouch = true;
        inputButtonDown = false;
        inputAxisDown = false;
        inputGestureDown = false;
        inputGestureUp = false;
        showPoints = false;

        //bool inHitArea = false;

        if ((Input.mousePosition.x > Screen.width / 3
            && Input.mousePosition.x < Screen.width - Screen.width / 3)
            && (Input.mousePosition.y > Screen.height / 4
            && Input.mousePosition.y < Screen.height - Screen.height / 3)) {
            //inHitArea = true;
        }

        if (camHud == null) {
            foreach (Camera camItem in Camera.allCameras) {
                if (camItem.cullingMask == LayerMask.NameToLayer("UIHUDScaled")) {
                    camHud = camItem;
                }
            }
            if (camHud == null) {
                camHud = Camera.main;
            }
        }

        bool hasTouches = false;
        //bool hasTouchesDown = false;
        //bool hasTouchesUp = false;
        bool hasTouchesDownAllowed = false;
        bool hasTouchesUpAllowed = false;

        if (Input.touches.Length > 0) {
            hasTouches = true;
        }

        lastDownAllowedPosition = Vector3.zero;
        lastUpAllowedPosition = Vector3.zero;

        //hasTouchesDown = checkIfTouchesDown();
        //hasTouchesUp = checkIfTouchesUp();
        hasTouchesDownAllowed = checkIfTouchesDownAllowed();
        hasTouchesUpAllowed = checkIfTouchesUpAllowed();
////

        if (hasTouches) {
            //LogUtil.Log("hasTouches: " + hasTouches);
        }
        if (hasTouchesDownAllowed) {
            //LogUtil.Log("hasTouchesDownAllowed: " + hasTouchesDownAllowed);
        }
        if (hasTouchesUpAllowed) {
            //LogUtil.Log("hasTouchesUpAllowed: " + hasTouchesUpAllowed);
        }

        if (!hasTouches) {
            lastDownAllowedPosition = Input.mousePosition;
            checkIfAllowedTouch(lastDownAllowedPosition);
        }

        if (!shouldTouch) {
            //return;
        }
                
        if (((Input.GetMouseButtonDown(0) && !hasTouchesDownAllowed) || hasTouchesDownAllowed)
            && !inputAxisDown
            && !inputButtonDown) {
            inputGestureDown = true;
        }

        if (((Input.GetMouseButtonUp(0) && !hasTouchesUpAllowed) || hasTouchesUpAllowed)
            && !inputAxisDown
            && !inputButtonDown) {
            inputGestureUp = true;
        }
                
        if (inputGestureDown
                        //&& (Input.mousePosition.x > Screen.width / 4 
                        //|| Input.mousePosition.y > Screen.height / 4)
                        //&& (Input.mousePosition.x < Screen.width - (Screen.width / 5) 
                        //&& Input.mousePosition.y < Screen.height - (Screen.height / 5) )
                        ) {
            if (positionStart == Vector3.zero) {
                positionEnd = Vector3.zero;
                positionStart = lastDownAllowedPosition;
                showPoints = true;
                updateTouchStartTime = Time.time;
                //LogUtil.Log("GetMouseButtonDown:positionStart:" + positionStart);
                //LogUtil.Log("GetMouseButtonDown:positionEnd:" + positionEnd);
                //LogUtil.Log("GetMouseButtonDown:positionLastLaunch:" + positionLastLaunch);
            }

                
            if (GameController.CurrentGamePlayerController != null) {
                if (GameController.CurrentGamePlayerController.IsPlayerControlled) {
                    //Vector3 dir = positionStart - Input.mousePosition;
                    //Vector3 posNormalized = dir.normalized;
                    //gamePlayerController.UpdateAim(-posNormalized.x, -posNormalized.y);
                    showPoints = true;
                }
            }
        }
        else if (inputGestureUp) {
            if (positionEnd == Vector3.zero
                && positionStart != Vector3.zero) {

                if (hasTouches) {
                    positionEnd = lastUpAllowedPosition;
                }
                else {
                    positionEnd = Input.mousePosition;
                }

                // launch
                powerDistance = Vector3.Distance(positionStart, positionEnd);
                positionLastLaunch = positionStart - positionEnd;
                //LogUtil.Log("GetMouseButtonUp:positionEnd:" + positionEnd);
                //LogUtil.Log("GetMouseButtonUp:positionStart:" + positionStart);
                //LogUtil.Log("GetMouseButtonUp:positionLastLaunch:" + positionLastLaunch);
                                
                positionLastLaunchedNormalized = positionLastLaunch.normalized;
                                
                //LogUtil.Log("GetMouseButtonUp:posNormalized:" + posNormalized);

                bool doAction = true;

                if (Time.time > updateTouchStartTime + updateTouchMaxTime) {
                    updateTouchStartTime = Time.time;
                    doAction = false;
                    showPoints = false;
                }

                if (!doAction) {
                    positionStart = Vector3.zero;
                    return;
                }

                if (GameController.CurrentGamePlayerController != null) {
                    if (GameController.CurrentGamePlayerController.IsPlayerControlled) {
                        //Attack();
                        //gamePlayerController.gamePlayerModelHolderModel.
                        //gamePlayerController.UpdateAim(-positionLastLaunchNormalized.x, -positionLastLaunchNormalized.y);
                        //Attack();
                                                
                        //PhysicsUtil.PlotTrajectory(transform.position, positionLastLaunchNormalized, .1f, 4f);


                        Messenger<Vector3>.Broadcast(UIControllerMessages.uiUpdateTouchLaunch, positionLastLaunchedNormalized);
                        //LogUtil.Log("positionLastLaunchedNormalized:" + positionLastLaunchedNormalized);
                        //LogUtil.Log("positionLastLaunch:" + positionLastLaunch);
                        //LogUtil.Log("powerDistance:" + powerDistance);

                        Vector2 touchLaunch = Vector2.zero.WithX(-positionLastLaunchedNormalized.x).WithY(-positionLastLaunchedNormalized.y);

                        GameUIController.HandleTouchLaunch(touchLaunch);

                        //ResetAimDelayed(.8f);
                    }
                }
                showPoints = true;
                positionStart = Vector3.zero;
                positionEnd = Vector3.zero;
            }
        }
        else {
                                
        }
                
        if (showPoints) {                
            if (positionStart != Vector3.zero) {
                //showStartPoint(positionStart);
            }
                                        
            if (positionEnd != Vector3.zero) {
                //showEndPoint(positionEnd);
            }
        }
        else {
            //hidePoints();
        }
    }
        
    public virtual void hidePoints() {
        hideStartPoint();
        hideEndPoint();
    }
        
    public virtual void hideStartPoint() {
        if (pointStartObject != null) {
            pointStartObject.transform.position = Vector3.zero.WithY(3000);
        }
    }
        
    public virtual void hideEndPoint() {
        if (pointEndObject != null) {
            pointEndObject.transform.position = Vector3.zero.WithY(3000);
        }
    }
        
    public virtual void showStartPoint(Vector3 pos) {
        //
                
        if (pointStartObject == null) {
                
            if (!isCreatingStart) {
                isCreatingStart = true;
                if (prefabPointStart == null) {
                    prefabPointStart = Resources.Load(
                                                ContentPaths.appCacheVersionSharedPrefabWeapons + "GamePlayerWeaponCharacterLaunchPoint") as UnityEngine.Object;
                }
                pointStartObject = Instantiate(prefabPointStart) as GameObject;         
            }
        }
                
        if (pointStartObject != null) {
            pointStartObject.transform.position = Camera.main.ScreenToWorldPoint(pos);
        }
    }
        
    public virtual void showEndPoint(Vector3 pos) {
                
        if (pointEndObject == null) {
            if (!isCreatingEnd) {
                isCreatingEnd = true;
                if (prefabPointEnd == null) {
                    prefabPointEnd = Resources.Load(
                        ContentPaths.appCacheVersionSharedPrefabWeapons +
                        "GamePlayerWeaponCharacterLaunchPoint") as UnityEngine.Object;
                }
                pointEndObject = Instantiate(prefabPointEnd) as GameObject;     
            }
        }
                
        if (pointEndObject != null) {
            pointEndObject.transform.position = Camera.main.ScreenToWorldPoint(pos);
        }
    }

    public virtual bool checkIfAllowedTouch(Vector3 pos) {

        if (!GameConfigs.isGameRunning) {
            return false;
        }

        Ray screenRay = camHud.ScreenPointToRay(pos);

        RaycastHit hit;

        allowedTouch = true;
        inputButtonDown = false;
        inputAxisDown = false;
        shouldTouch = false;

        if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {

            if (hit.transform.name.Contains("ButtonInput")
                || hit.transform.name.Contains("ButtonInput")
                || hit.transform.name.Contains("ButtonInput")
                || hit.transform.name.Contains("Axis")
                || hit.transform.name.Contains("Ignore")
                || hit.transform.name.Contains("Pad")) {
                inputButtonDown = true;
                shouldTouch = false;
                allowedTouch = false;
            }

            if (allowedTouch) {
                if (hit.transform.gameObject.Has<GameTouchInputAxis>()) {
                    // not over axis controller
                    inputAxisDown = true;
                    shouldTouch = false;
                    allowedTouch = false;
                }
    
                if (hit.transform.gameObject.Has<GameTouchInputAxis>()) {
                    // not over axis controller
                    inputAxisDown = true;
                    shouldTouch = false;
                    allowedTouch = false;
                }
    
                if (hit.transform.gameObject.Has<UIButton>()) {
                    // not over button
                    inputButtonDown = true;
                    shouldTouch = false;
                    allowedTouch = false;
                }
    
                if (hit.transform.gameObject.Has<UIImageButton>()) {
                    // not over button
                    inputButtonDown = true;
                    shouldTouch = false;
                    allowedTouch = false;
                }
            }

            //LogUtil.Log("hit:" + hit);
            //LogUtil.Log("hit.transform.name:" + hit.transform.name);
        }

        return allowedTouch;
    }

    public virtual bool checkIfTouchesDownAllowed() {
        foreach (Touch t in Input.touches) {
            if (t.phase == TouchPhase.Began) {
                if (checkIfAllowedTouch(t.position)) {
                    lastDownAllowedPosition = t.position;
                    return true;
                }
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            if (checkIfAllowedTouch(Input.mousePosition)) {
                lastDownAllowedPosition = Input.mousePosition;
                return true;
            }
        }
        return false;
    }

    Vector3 lastDownAllowedPosition = Vector3.zero;
    Vector3 lastUpAllowedPosition = Vector3.zero;

    public virtual bool checkIfTouchesUpAllowed() {
        foreach (Touch t in Input.touches) {
            if (t.phase == TouchPhase.Ended) {
                if (checkIfAllowedTouch(t.position)) {
                    lastUpAllowedPosition = t.position;
                    return true;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (checkIfAllowedTouch(Input.mousePosition)) {
                lastUpAllowedPosition = Input.mousePosition;
                return true;
            }
        }
        return false;
    }

    public virtual bool checkIfTouchesDown() {
        foreach (Touch t in Input.touches) {
            if (t.phase == TouchPhase.Began) {
                return true;
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            return true;
        }
        return false;
    }
        
    public virtual bool checkIfTouchesUp() {
        foreach (Touch t in Input.touches) {
            if (t.phase == TouchPhase.Ended) {
                return true;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            return true;
        }
        return false;
    }
    /*
    public virtual void DetectSwipe() {
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {

            var startPos = Vector2.zero;
            var startTime = 0f;
            var touch = Input.touches[0];
            bool couldBeSwipe = false;
            float comfortZone = 500f;
            float minSwipeTime = .2f;
            float minSwipeDist = .2f;
            float maxSwipeTime = 1.7f;

            switch(touch.phase) {
            case TouchPhase.Began:
                couldBeSwipe = true;
                startPos = touch.position;
                startTime = Time.time;
                break;

            case TouchPhase.Moved:
                if(Mathf.Abs(touch.position.y - startPos.y) > comfortZone) {
                    couldBeSwipe = false;
                }
                break;
            case TouchPhase.Stationary:
                couldBeSwipe = false;
                break;
            case TouchPhase.Ended:
                var swipeTime = Time.time - startTime;
                var swipeDist = (touch.position - startPos).magnitude;
                if(couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDist > minSwipeDist)) {
                    // It's a swiiiiiiiiiiiipe!
                    var swipeDirection = Mathf.Sign(touch.position.y - startPos.y);
                    // Do something here in reaction to the swipe.

                    LogUtil.Log("swipeDirection:" + swipeDirection);
                    LogUtil.Log("swipeTime:" + swipeTime);
                    LogUtil.Log("swipeDist:" + swipeDist);
                }
                break;
            }
        }
    }
    */

    public virtual void HandleFingerGesturesOnLongPress(Vector2 fingerPos) {
        //LogUtil.Log("HandleFingerGesturesOnLongPress: " 
        //   + " fingerPos:" + fingerPos);   
     
        if (!IsInputAllowed()) {
            return;
        }
     
        // Create asset at touch point (long press) if in game and editing       
        LongPress(fingerPos);
    }

    public virtual void HandleFingerGesturesOnTap(Vector2 fingerPos) {
        //LogUtil.Log("HandleFingerGesturesOnTap: " 
        //   + " fingerPos:" + fingerPos);
             
        if (!IsInputAllowed()) {
            return;
        }
     
        // ...   
        Tap(fingerPos);
     
    }

    public virtual void HandleFingerGesturesOnDoubleTap(Vector2 fingerPos) {
        //LogUtil.Log("HandleFingerGesturesOnDoubleTap: " 
        //   + " fingerPos:" + fingerPos);
             
        if (!IsInputAllowed()) {
            return;
        }
     
        // ...   
        DoubleTap(fingerPos);
     
    }
 
    public virtual void HandleFingerGesturesOnDragMove(Vector2 fingerPos, Vector2 delta) {
        //LogUtil.Log("HandleFingerGesturesOnDragMove: " 
        //   + " fingerPos:" + fingerPos 
        //   + " delta:" + delta);
             
        if (!IsInputAllowed()) {
            return;
        }
     
        // scale current selected object 
        DragMove(fingerPos, delta);
     
    }

    public virtual void HandleFingerGesturesOnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {
        //LogUtil.Log("HandleFingerGesturesOnPinchMove: " 
        //   + " fingerPos1:" + fingerPos1 
        //   + " fingerPos2:" + fingerPos2
        //   + " delta:" + delta);
             
        if (!IsInputAllowed()) {
            return;
        }
     
        // scale current selected object 
        PinchMove(fingerPos1, fingerPos2, delta);
     
    }

    public virtual void HandleFingerGesturesOnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
        //LogUtil.Log("HandleFingerGesturesOnRotationMove: " 
        //   + " fingerPos1:" + fingerPos1 
        //   + " fingerPos2:" + fingerPos2
        //   + " rotationAngleDelta:" + rotationAngleDelta); 
     
        if (!IsInputAllowed()) {
            return;
        }
     
        // rotate current object if editing
        RotationMove(fingerPos1, fingerPos2, rotationAngleDelta);
     
    }
 
    public virtual void HandleFingerGesturesOnSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {
        //LogUtil.Log("HandleFingerGesturesOnSwipe: " 
        //   + " startPos:" + startPos 
        ///  + " direction:" + direction
        //   + " velocity:" + velocity); 
     
        if (!IsInputAllowed()) {
            return;
        }
     
        // ...
        Swipe(startPos, direction, velocity);
    }
 
    public virtual void HandleFingerGesturesOnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {        
        //LogUtil.Log("HandleFingerGesturesOnTwoFingerSwipe: " 
        //   + " startPos:" + startPos 
        //   + " direction:" + direction
        //   + " velocity:" + velocity); 
     
        if (!IsInputAllowed()) {
            return;
        }
     
        // ...
        TwoFingerSwipe(startPos, direction, velocity);
    }
     
    public virtual void LongPress(Vector2 fingerPos) {       
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
             
            ResetCurrentObject(fingerPos);   
        }
    }
 
    public virtual void RotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {     
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            RotateCurrentObject(rotationAngleDelta);
        }
    }
 
    public virtual void DragMove(Vector2 fingerPos, Vector2 delta) {     
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {         
            //SpinCurrentObject(fingerPos, delta);
         
            bool doScale = false;
            bool doRotation = false;
         
            if (Input.GetKey(KeyCode.S)) {
                doScale = true;
            }
         
            if (Input.GetKey(KeyCode.R)) {
                doRotation = true;
            }            
         
            if (doRotation) {
                RotateCurrentObject(delta.x);
            }
         
            if (doScale) {                
                ScaleCurrentObject(delta.y);
            }
         
        }
    }
 
    public virtual void PinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {     
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            ScaleCurrentObject(delta);
        }
    }
 
    public virtual void Tap(Vector2 fingerPos) {     
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
         
        }
    }
 
    public virtual void DoubleTap(Vector2 fingerPos) {       
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
         
            GameDraggableEditor.EditModeCreateAsset(fingerPos);
         
            //var fwd = transform.TransformDirection(Vector3.forward);
            Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                print("double tap hit an object:" + hit.transform.name);
            }
        }
     
        //AppController.Instance.ChangeActionNext();
     
        if (Application.isEditor) {
            GameController.CycleCharacterTypesNext();
        }

    }
                
    public virtual void TwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {

        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
     
            if (direction == FingerGestures.SwipeDirection.All) {
                // if swiped any direction
            }
         
            if (direction == FingerGestures.SwipeDirection.Right
                || direction == FingerGestures.SwipeDirection.Down) {
                //AppController.Instance.ChangeActionPrevious();
            }
            else if (direction == FingerGestures.SwipeDirection.Left
                || direction == FingerGestures.SwipeDirection.Up) {
                //AppController.Instance.ChangeActionNext();
            }
        }
     
        if (direction == FingerGestures.SwipeDirection.Right
            || direction == FingerGestures.SwipeDirection.Down) {
            GameController.CycleCharacterTypesPrevious();
        }
        else if (direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {
            GameController.CycleCharacterTypesNext();
        }
    }
 
    public virtual void Swipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocitys) {      
        if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
         
        }
     
        //bool allowSwipe = true;
         
        if (direction == FingerGestures.SwipeDirection.Right
            || direction == FingerGestures.SwipeDirection.Down) {
            //if(!UIController.Instance.uiVisible && allowSwipe) {
            // AppController.Instance.ChangeActionPrevious();
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
         
         
        }
        else if (direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {
            //if(!UIController.Instance.uiVisible && allowSwipe) {
            //AppController.Instance.ChangeActionNext();
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
        }
    }
 
    public virtual bool IsInputAllowed() {
        return !dialogActive;
    }
 
    public virtual void ScaleCurrentObject(float delta) {
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
     
        if (go != null) {
            GameObjectHelper.ScaleObject(go, delta);
        }
    }
 
    public virtual void RotateCurrentObject(float delta) {
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
     
        if (go != null) {
            GameObjectHelper.RotateObjectZ(go, delta);
        }
    }
        
    public virtual void SpinCurrentObject(Vector2 fingerPos, Vector2 delta) {
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
         
        if (go != null) {
            GameObjectHelper.SpinObject(go, fingerPos, delta);           
            GameObjectHelper.deferTap = true;            
            GamePlayerProgress.Instance.ProcessProgressSpins();
        }
    }
        
    public virtual void ResetCurrentObject(Vector2 pos) {        
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
     
        if (go != null) {         
            Rigidbody rb = go.GetComponent<Rigidbody>();
            
            if (rb != null) {         
                if (rb != null) {
                    rb.angularVelocity = Vector3.zero;
                }
            }
            GameObjectHelper.deferTap = true;
         
            GameObjectHelper.ResetObject(go);
        }    
    }
        
    public virtual void FingerGestures_OnTap(Vector2 fingerPos) {    
        if (!IsInputAllowed()) {
            return;
        }
     
        TapObject(GameDraggableEditor.GetCurrentSpriteObject(), fingerPos, true);
    }
 
    /*
 public virtual void TapObject(GameObject go, Vector2 fingerPos, bool allowTap) {
 
     if(go != null) {
     
         GameObjectHelper.deferTap = !allowTap;
         
         if(!GameObjectHelper.deferTap) {
             //var fwd = transform.TransformDirection(Vector3.forward);
             Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
             RaycastHit hit;
             if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
                 print ("hit an object:" + hit.transform.name);
                 
                     //if(hit.transform.name == "UILoaderContainer") {
                     //      GameObject loaderCube = hit.transform.gameObject;
                     //      if(loaderCube != null) {
                                     //UILoaderContainer loaderContainer = loaderCube.GetComponent<UILoaderContainer>();
                                     //if(loaderContainer != null) {
                                     //      if(loaderContainer.placeholderObject != null) {
                                     //              loaderContainer.placeholderObject.SetPlaceholderObject();
                                     //      }
                                     //}
                     //      }
                     //}
             }
             
             //if(!UIController.Instance.uiVisible) {
                 //AppController.Instance.ChangeActionNext();
                 GamePlayerProgress.Instance.ProcessProgressTaps();
             //}
         
         }
         else {
             GameObjectHelper.deferTap = false;
         }
     }
 }*/
 
            
    public virtual void ToggleUI() {
                    
        LogUtil.Log("ToggleUI uiVisible: " + uiVisible);
                        
        if (uiVisible) {
            LogUtil.Log("call HideUI");
            hideUI(false);
        }
        else {
            LogUtil.Log("call ShowUI");
            showUI();
        }            
    }
 
    public virtual bool NavigateBack(string buttonName) {    
 
        bool handled = false;

        if (buttonName == BaseUIButtonNames.buttonBack) {
            if (!GameUIController.Instance.uiVisible) {
                HideAllPanels();
                GameUIPanelHeader.Instance.AnimateOut();
                GameUIPanelBackgrounds.Instance.AnimateOut();
                GameHUD.Instance.AnimateIn();
                handled = true;
            }
            else {
                if (isUIPanel(GameUIPanel.panelSettingsAudio)
                    || isUIPanel(GameUIPanel.panelSettingsControls)
                    || isUIPanel(GameUIPanel.panelSettingsHelp)
                    || isUIPanel(GameUIPanel.panelSettingsCredits)
                    || isUIPanel(GameUIPanel.panelSettingsProfile)) {
                    
                    GameUIController.ShowSettings(); 
                    handled = true;       
                    
                }
                else if (isUIPanel(GameUIPanel.panelAchievements)
                    || isUIPanel(GameUIPanel.panelCustomize)
                    || isUIPanel(GameUIPanel.panelProducts)
                    || isUIPanel(GameUIPanel.panelStatistics)) {
                    
                    GameUIController.ShowEquipment();       
                    handled = true;
                    
                }
                else if (isUIPanel(GameUIPanel.panelCustomizeCharacterRPG)
                    || isUIPanel(GameUIPanel.panelCustomizeCharacterColors)
                         || isUIPanel(GameUIPanel.panelCustomizeCharacter)
                         || isUIPanel(GameUIPanel.panelCustomizeLevels)
                         || isUIPanel(GameUIPanel.panelCustomizeWorlds)
                    || isUIPanel(GameUIPanel.panelCustomizeAudio)) {
                    
                    GameUIController.ShowCustomize();
                    handled = true;
                    
                }
                else if (isUIPanel(GameUIPanel.panelGameModeArcade)
                    || isUIPanel(GameUIPanel.panelGameModeCustomize)
                    || isUIPanel(GameUIPanel.panelGameModeCareer)
                    || isUIPanel(GameUIPanel.panelGameModeChallenge)
                    || isUIPanel(GameUIPanel.panelGameModeCoop)
                    || isUIPanel(GameUIPanel.panelGameModeMissions)
                    || isUIPanel(GameUIPanel.panelGameModeMultiplayer)
                    || isUIPanel(GameUIPanel.panelGameModeMultiplayerCoop)
                    || isUIPanel(GameUIPanel.panelGameModeMultiplayerMatchup)
                    || isUIPanel(GameUIPanel.panelGameModeTraining)
                    || isUIPanel(GameUIPanel.panelGameModeTrainingMode)) {
                    
                    GameUIController.ShowGameMode();
                    handled = true;
                    
                }
                else if (isUIPanel(GameUIPanel.panelGameModeTrainingModeContent)
                    || isUIPanelLike(GameUIPanel.panelGameModeTrainingModeChoice)
                    || isUIPanelLike(GameUIPanel.panelGameModeTrainingModeCollection)) {
                    
                    //GameUIController.ShowGameModeTrainingMode();
                    GameUIController.ShowGameMode();
                    handled = true;
                    
                }
                else {
                    GameUIController.ShowMain(); 
                    handled = true;                   
                }
            }
        }   
                
        return handled;

        /*
     if(buttonName == BaseUIButtonNames.buttonBack) {
         if(!GameUIController.Instance.uiVisible) {
             HideAllPanels();
             GameUIPanelHeader.Instance.AnimateOut();
             GameUIPanelBackgrounds.Instance.AnimateOut();
             GameHUD.Instance.AnimateIn();
         }
         else {
             if(GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelSettingsAudio
                 || GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelSettingsControls
                 || GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelSettingsProfile) {
                     
                 GameUIController.ShowSettings();        
                 
             }
             else 
             if(GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelAchievements
                 || GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelCustomize
                 || GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelProducts
                 || GameUIController.Instance.currentPanel 
                 == BaseUIPanel.panelStatistics) {
                     
                 GameUIController.ShowEquipment();       
                 
             }
             else {
                 GameUIController.ShowMain();                    
             }
         }
     }   
     */
    }    
 
    // CURRENT
    
    public virtual void HandleInGameAudio() {
        HandleInGameAudio(true);
    }
 
    public virtual void HandleInGameAudio(bool playAudio) {
     
        if (!gameLoopsStarted) {          
            GameAudio.StartGameLoops();
        }
     
        GameAudio.StopAmbience();

        if (playAudio) {
            int loopToPlay = UnityEngine.Random.Range(1, 4);

            LogUtil.Log("HandleInGameAudio:", " loopToPlay:" + loopToPlay.ToString());

            GameAudio.StartGameLoop(loopToPlay);
        }
        inUIAudioPlaying = false;
    }
 
    public virtual void HandleInUIAudio() {
     
        if (!inUIAudioPlaying) {
            GameAudio.StartGameLoop(-1);
            GameAudio.StartAmbience();
            inUIAudioPlaying = true;
        }
    }

    public virtual void ToggleGameUI() {
                 
        gameUIExpanded = gameUIExpanded ? false : true;
         
        LogUtil.Log("toggling:" + gameUIExpanded);
     
        if (gameUIExpanded) {
            //Vector3 temp = panelCover.transform.position;
            //temp.x = 55f;
            //Tweens.Instance.MoveToObject(panelCover, temp, 0f, 0f);
         
            HandleInGameAudio();
        }
        else {           
            //Vector3 temp = panelCover.transform.position;
            //temp.x = 0f;
            //Tweens.Instance.MoveToObject(panelCover, temp, 0f, 0f);
         
            HandleInUIAudio();
        }
     
    }
 
    // ------------------------------------------------------------
    // BACKGROUNDS
     
    //public static virtual void ShowBackgrounds() {
    //   if(isInst) {
    //       Instance.showBackgrounds();
    //   }
    //}
 
    public virtual void showBackgrounds() {
     
    }
 
    // ------------------------------------------------------------
    // UI
 
    //public static virtual void HideUI() {
    //   if(isInst) {
    //       Instance.hideUI(false);
    //   }
    //}
 
    //public static virtual void HideUI(bool now) {
    //   if(isInst) {
    //       Instance.hideUI(now);   
    //   }
    //}
 
    public virtual void hideUI(bool now) {   
     
        //LogUtil.Log("HideUI");  
     
        uiVisible = false;

        showGameCanvas();

        if (now) {
            HideAllPanelsNow();
        }
        else {
            HideAllPanels();
        }
     
        HandleInGameAudio();
        
        GameController.HandleCamerasInGame();
        
        Messenger.Broadcast(UIControllerMessages.uiHide);
            
    }
    
    //public static virtual void ShowUI() {
    //   if(isInst) {
    ////     Instance.showUI();
    //   }
    //}
 
    public virtual void showUI() {
        //LogUtil.Log("ShowUI");
        uiVisible = true;        
        hideGameCanvas();        
        HandleInUIAudio();
        
        GameController.HandleCamerasInUI();
        
        GameUIPanelBackgrounds.Instance.AnimateIn(); 

        Messenger.Broadcast(UIControllerMessages.uiShow);
    }   
     
    // ------------------------------------------------------------
    // MAIN
 
    //public static virtual void ShowMain() {
    //   if(isInst) {
    //       Instance.showMain();
    //   }
    //}
 
    public virtual void showMain() {
     
        showUI();

        showUIPanel(
            typeof(GameUIPanelMain),
            BaseUIPanel.panelMain,
            "PLAY GAMEMODE");

        GameUIPanelBackgrounds.Instance.AnimateIn();        
        
        GameUIPanelHeader.Instance.AnimateInMain();
        
        GameUIPanelFooter.Instance.AnimateInMain();
          
        GameUIPanelMain.Instance.AnimateIn();

    }
    
    //public static virtual void HideMain() {
    //   if(isInst) {
    //       Instance.hideMain();
    //   }
    //}
 
    public virtual void hideMain() {
        hideUIPanel(
            typeof(GameUIPanelMain));
    }
 
    // ------------------------------------------------------------
    // GAME MODES
 
    //public static virtual void ShowGameMode() {
    //   if(isInst) {
    //       Instance.showGameMode();
    //   }
    //}
 
    public virtual void showGameMode() {

        showUI();

        showUIPanel(
            typeof(GameUIPanelGameMode),
            BaseUIPanel.panelGameMode,
            "PLAY GAMEMODE");
    } 
 
    //public static virtual void HideGameMode() {
    //   if(isInst) {
    //       Instance.hideGameMode();
    //   }
    //}
    
    public virtual void hideGameMode() {
        hideUIPanel(
            typeof(GameUIPanelGameMode));
    }

    
    // ------------------------------------------------------------
    // GAME MODE - COOP
    
    //public static virtual void ShowGameModeCoop() {
    //   if(isInst) {
    //        Instance.showGameModeCoop();
    //   }
    //}
    
    public virtual void showGameModeCoop() {
        showUIPanel(
            typeof(GameUIPanelGameModeCoop),
            GameUIPanel.panelGameModeCoop,
            "PLAY COOP");
    } 
    
    //public static virtual void HideGameModeCoop() {
    //   if(isInst) {
    //        Instance.hideGameModeCoop();
    //   }
    //}
    
    public virtual void hideGameModeCoop() {
        hideUIPanel(
            typeof(GameUIPanelGameModeCoop));
    }
    
    // ------------------------------------------------------------
    // GAME MODE MULTIPLAYER

    public virtual void showGameModeMultiplayer() {
        
        showUIPanel(
            typeof(GameUIPanelGameModeMultiplayer),
            BaseUIPanel.panelGameModeMultiplayer,
            "PLAY MULTIPLAYER");
    }
    
    public virtual void hideGameModeMultiplayer() {
        hideUIPanel(
            typeof(GameUIPanelGameModeMultiplayer));
    }

    // COOP

    public virtual void showGameModeMultiplayerCoop() {
        
        showUIPanel(
            typeof(GameUIPanelGameModeMultiplayerCoop),
            BaseUIPanel.panelGameModeMultiplayerCoop,
            "PLAY MULTIPLAYER CO-OP");
    }
    
    public virtual void hideGameModeMultiplayerCoop() {
        hideUIPanel(
            typeof(GameUIPanelGameModeMultiplayerCoop));
    }

    // MATCHUP
    
    public virtual void showGameModeMultiplayerMatchup() {
        
        showUIPanel(
            typeof(GameUIPanelGameModeMultiplayerMatchup),
            BaseUIPanel.panelGameModeMultiplayerMatchup,
            "PLAY MULTIPLAYER MATCHUP");
    }
    
    public virtual void hideGameModeMultiplayerMatchup() {
        hideUIPanel(
            typeof(GameUIPanelGameModeMultiplayerMatchup));
    }
    
    // ------------------------------------------------------------
    // GAME MODE - MISSION
    
    public virtual void showGameModeMission() {
        
        showUIPanel(
            typeof(GameUIPanelGameModeMission),
            BaseUIPanel.panelGameModeMissions,
            "PLAY MISSION");
    }
    
    public virtual void hideGameModeMission() {
        hideUIPanel(
            typeof(GameUIPanelGameModeMission));
    }

    // ------------------------------------------------------------
    // GAME WORLDS
    
    //public static virtual void ShowGameWorlds() {
    //   if(isInst) {
    //       Instance.showGameWorlds();
    //   }
    //}
    
    public virtual void showGameWorlds() {
        
        showUIPanel(
            typeof(GameUIPanelWorlds),
            BaseUIPanel.panelWorlds,
            "WORLDS");
    } 
    
    //public static virtual void HideGameWorlds() {
    //   if(isInst) {
    //       Instance.hideGameWorlds();
    //   }
    //}
    
    public virtual void hideGameWorlds() {
        hideUIPanel(
            typeof(GameUIPanelWorlds));
    }    
    
    // ------------------------------------------------------------
    // GAME LEVELS
    
    //public static virtual void ShowGameLevels() {
    //   if(isInst) {
    //       Instance.showGameLevels();
    //   }
    //}
    
    public virtual void showGameLevels() {
        
        showUIPanel(
            typeof(GameUIPanelLevels),
            BaseUIPanel.panelLevels,
            "LEVELS");
    } 
    
    //public static virtual void HideGameLevels() {
    //   if(isInst) {
    //       Instance.hideGameLevels();
    //   }
    //}
    
    public virtual void hideGameLevels() {
        hideUIPanel(
            typeof(GameUIPanelLevels));
    }

    // ------------------------------------------------------------
    // GAME MODE - TRAINING
 
    //public static virtual void ShowGameModeTraining() {
    //   if(isInst) {
    //       Instance.showGameModeTraining();
    //   }
    //}
 
    public virtual void showGameModeTraining() {
        showUIPanel(
            typeof(GameUIPanelGameModeTraining),
            BaseUIPanel.panelGameModeTraining,
            "PLAY TRAINING");
    } 
 
    //public static virtual void HideGameModeTraining() {
    //   if(isInst) {
    //       Instance.hideGameModeTraining();
    //   }
    //}

    public virtual void hideGameModeTraining() {
        hideUIPanel(
            typeof(GameUIPanelGameModeTraining));
    }

    // ------------------------------------------------------------
    // GAME MODE - TRAINING MODE
    
    public virtual void showGameModeTrainingMode() {
        showUIPanel(
            typeof(GameUIPanelGameModeTrainingMode),
            BaseUIPanel.panelGameModeTrainingMode,
            "CHOOSE TRAINING MODE");
    }

    public virtual void hideGameModeTrainingMode() {
        hideUIPanel(
            typeof(GameUIPanelGameModeTrainingMode));
    }

    /*
    // ------------------------------------------------------------
    // GAME MODE - TRAINING MODE CHOICE/QUIZ

    public virtual void showGameModeTrainingModeChoice() {

        currentPanel = BaseUIPanel.panelGameModeTrainingModeChoice;

        HideAllPanelsNow();

        GameUIPanelBackgrounds.Instance.AnimateInStarry();

        GameUIPanelHeader.Instance.AnimateInInternal();
        GameUIPanelHeader.ShowTitle("TRAINING MODE: QUESTION + ANSWER");

        GameUIPanelGameModeTrainingModeChoice.Instance.AnimateIn();
    }

    public virtual void hideGameModeTrainingModeChoice() {
        GameUIPanelGameModeTrainingModeChoice.Instance.AnimateOut();
    }

    // ------------------------------------------------------------
    // GAME MODE - TRAINING MODE RPG HEALTH

    public virtual void showGameModeTrainingModeRPGHealth() {

        currentPanel = BaseUIPanel.panelGameModeTrainingModeRPGHealth;

        HideAllPanelsNow();

        GameUIPanelBackgrounds.Instance.AnimateInStarry();

        GameUIPanelHeader.Instance.AnimateInInternal();
        GameUIPanelHeader.ShowTitle("TRAINING MODE: PLAY SMART");

        GameUIPanelGameModeTrainingModeRPGHealth.Instance.AnimateIn();
    }

    public virtual void hideGameModeTrainingModeRPGHealth() {
        GameUIPanelGameModeTrainingModeRPGHealth.Instance.AnimateOut();
    }

    // ------------------------------------------------------------
    // GAME MODE - TRAINING MODE RPG ENERGY

    public virtual void showGameModeTrainingModeRPGEnergy() {

        currentPanel = BaseUIPanel.panelGameModeTrainingModeRPGEnergy;

        HideAllPanelsNow();

        GameUIPanelBackgrounds.Instance.AnimateInStarry();

        GameUIPanelHeader.Instance.AnimateInInternal();
        GameUIPanelHeader.ShowTitle("TRAINING MODE: PLAY SAFELY");

        GameUIPanelGameModeTrainingModeRPGEnergy.Instance.AnimateIn();
    }

    public virtual void hideGameModeTrainingModeRPGEnergy() {
        GameUIPanelGameModeTrainingModeRPGEnergy.Instance.AnimateOut();
    }

    */
 
    // ------------------------------------------------------------
    // GAME MODE - CHALLENGE
 
    //public static virtual void ShowGameModeChallenge() {
    //   if(isInst) {
    //       Instance.showGameModeChallenge();
    //   }
    //}
 
    public virtual void showGameModeChallenge() {
        showUIPanel(
            typeof(GameUIPanelGameModeChallenge),
            GameUIPanel.panelGameModeChallenge,
            "PLAY CHALLENGE");
    } 
 
    //public static virtual void HideGameModeChallenge() {
    //   if(isInst) {
    //       Instance.hideGameModeChallenge();
    //   }
    //}
    
    public virtual void hideGameModeChallenge() {
        hideUIPanel(
            typeof(GameUIPanelGameModeChallenge));
    }
 
    // ------------------------------------------------------------
    // GAME MODE - ARCADE
 
    //public static virtual void ShowGameModeArcade() {
    //   if(isInst) {
    //       Instance.showGameModeArcade();
    //   }
    //}
 
    public virtual void showGameModeArcade() {
        showUIPanel(
            typeof(GameUIPanelGameModeArcade),
            GameUIPanel.panelGameModeArcade,
            "PLAY ARCADE");
    } 
 
    //public static virtual void HideGameModeArcade() {
    //   if(isInst) {
    //       Instance.hideGameModeArcade();
    //   }
    //}
    
    public virtual void hideGameModeArcade() {
        hideUIPanel(
            typeof(GameUIPanelGameModeArcade));
    }

    
    // ------------------------------------------------------------
    // GAME MODE - CUSTOMIZE
    
    //public static virtual void ShowGameModeCustomize() {
    //   if(isInst) {
    //       Instance.showGameModeCustomize();
    //   }
    //}
    
    public virtual void showGameModeCustomize() {
        showUIPanel(
            typeof(GameUIPanelGameModeCustomize),
            GameUIPanel.panelGameModeCustomize,
            "CUSTOMIZE");
    } 
    
    //public static virtual void HideGameModeCustomize() {
    //   if(isInst) {
    //       Instance.hideGameModeCustomize();
    //   }
    //}
    
    public virtual void hideGameModeCustomize() {
        hideUIPanel(
            typeof(GameUIPanelGameModeCustomize));
    }
     
    /*
 // ------------------------------------------------------------
 // EQUIPMENT - MAIN
 
    public static virtual void ShowEquipment() {
     if(isInst) {
         Instance.showEquipment();
     }
 }
 
    public virtual void showEquipment() {    
     
     currentPanel = BaseUIPanel.panelEquipment;      
     
     HideAllPanelsNow();
     
     GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
     GameUIPanelHeader.Instance.AnimateInInternal(); 
     GameUIPanelHeader.ShowTitle("EQUIPMENT ROOM");
     
     GameUIPanelEquipment.Instance.AnimateIn();      
    } 
 
 public static virtual void HideEquipment() {
     if(isInst) {
         Instance.hideEquipment();
     }
 }
    
    public virtual void hideEquipment() {
     GameUIPanelEquipment.Instance.AnimateOut();
    }
 
 
 // ------------------------------------------------------------
 // EQUIPMENT - STATISTICS
 
    public static virtual void ShowStatistics() {
     if(isInst) {
         Instance.showStatistics();
     }
 }
 
    public virtual void showStatistics() {   
     
     currentPanel = BaseUIPanel.panelStatistics;     
     
     HideAllPanelsNow();
     
     GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
     GameUIPanelHeader.Instance.AnimateInInternal(); 
     GameUIPanelHeader.ShowTitle("STATISTICS");
     
     GameUIPanelStatistics.Instance.AnimateIn();     
    } 
 
 public static virtual void HideStatistics() {
     if(isInst) {
         Instance.hideStatistics();
     }
 }
    
    public virtual void hideStatistics() {
     GameUIPanelStatistics.Instance.AnimateOut();
    }
 
     
 // ------------------------------------------------------------
 // EQUIPMENT - ACHIEVEMENTS
 
    public static virtual void ShowAchievements() {
     if(isInst) {
         Instance.showAchievements();
     }
 }
 
    public virtual void showAchievements() { 
     
     currentPanel = BaseUIPanel.panelAchievements;       
     
     HideAllPanelsNow();
     
     GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
     GameUIPanelHeader.Instance.AnimateInInternal(); 
     GameUIPanelHeader.ShowTitle("ACHIEVEMENTS");
     
     GameUIPanelAchievements.Instance.AnimateIn();       
    } 
 
 public static virtual void HideAchievements() {
     if(isInst) {
         Instance.hideAchievements();
     }
 }
    
    public virtual void hideAchievements() {
     GameUIPanelAchievements.Instance.AnimateOut();
    }
 
     
     
 // ------------------------------------------------------------
 // EQUIPMENT - ACHIEVEMENTS
 
    public static virtual void ShowProducts() {
     if(isInst) {
         Instance.showProducts();
     }
 }
 
    public virtual void showProducts() { 
     
     currentPanel = BaseUIPanel.panelProducts;       
     
     HideAllPanelsNow();
     
     GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
     GameUIPanelHeader.Instance.AnimateInInternal(); 
     GameUIPanelHeader.ShowTitle("POWERUPS");
     
     GameUIPanelProducts.Instance.AnimateIn();       
    } 
 
 public static virtual void HideProducts() {
     if(isInst) {
         Instance.hideProducts();
     }
 }
    
    public virtual void hideProducts() {
     GameUIPanelProducts.Instance.AnimateOut();
    }
 
         
 // ------------------------------------------------------------
 // EQUIPMENT - ACHIEVEMENTS
 
    public static virtual void ShowCustomize() {
     if(isInst) {
         Instance.showCustomize();
     }
 }
 
    public virtual void showCustomize() {    
     
     currentPanel = BaseUIPanel.panelCustomize;      
     
     HideAllPanelsNow();
     
     GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
     GameUIPanelHeader.Instance.AnimateInInternal(); 
     GameUIPanelHeader.ShowTitle("CUSTOMIZE");
     
     GameUIPanelCustomize.Instance.AnimateIn();      
    } 
 
 public static virtual void HideCustomize() {
     if(isInst) {
         Instance.hideCustomize();
     }
 }
    
    public virtual void hideCustomize() {
     GameUIPanelCustomize.Instance.AnimateOut();
    }
 */
 
    // ------------------------------------------------------------
    // SETTINGS
 
    //public static virtual void ShowSettings() {
    //   if(isInst) {
    //       Instance.showSettings();
    //   }
    //}
 
    public virtual void showSettings() {
        showUIPanel(
            typeof(GameUIPanelSettings),
            GameUIPanel.panelSettings,
            "SETTINGS");
    } 
 
    //public static virtual void HideSettings() {
    //   if(isInst) {
    //       Instance.hideSettings();
    //   }
    //}
    
    public virtual void hideSettings() {
        hideUIPanel(
            typeof(GameUIPanelSettings));
    }
 
    // ------------------------------------------------------------
    // SETTINGS - AUDIO
 
    //public static virtual void ShowSettingsAudio() {
    //   if(isInst) {
    //       Instance.showSettingsAudio();
    //   }
    //}
 
    public virtual void showSettingsAudio() {
        showUIPanel(
            typeof(GameUIPanelSettingsAudio),
            GameUIPanel.panelSettingsAudio,
            "SETTINGS: AUDIO");
    } 
 
    //public static virtual void HideSettingsAudio() {
    //   if(isInst) {
    //       Instance.hideSettingsAudio();
    //   }
    //}
    
    public virtual void hideSettingsAudio() {
        hideUIPanel(
            typeof(GameUIPanelSettingsAudio));
    }
     
    // ------------------------------------------------------------
    // SETTINGS - CONTROLS
 
    //public static virtual void ShowSettingsControls() {
    //   if(isInst) {
    //       Instance.showSettingsControls();
    //   }
    //}
 
    public virtual void showSettingsControls() {
        showUIPanel(
            typeof(GameUIPanelSettingsControls),
            GameUIPanel.panelSettingsControls,
            "SETTINGS: CONTROLS");
    } 
 
    //public static virtual void HideSettingsControls() {
    //   if(isInst) {
    //       Instance.hideSettingsControls();
    //   }
    //}
    
    public virtual void hideSettingsControls() {
        hideUIPanel(
            typeof(GameUIPanelSettingsControls));
    }
 
    // ------------------------------------------------------------
    // SETTINGS - PROFILES
 
    //public static virtual void ShowSettingsProfile() {
    //   if(isInst) {
    //       Instance.showSettingsProfile();
    //   }
    //}
 
    public virtual void showSettingsProfile() {
        showUIPanel(
            typeof(GameUIPanelSettingsProfile),
            GameUIPanel.panelSettingsProfile,
            "SETTINGS: PROFILES");
    } 
 
    //public static virtual void HideSettingsProfile() {
    //   if(isInst) {
    //       Instance.hideSettingsProfile();
    //   }
    //}
    
    public virtual void hideSettingsProfile() {
        hideUIPanel(
            typeof(GameUIPanelSettingsProfile));
    }


    // ------------------------------------------------------------
    // SETTINGS - HELP

    public virtual void showSettingsHelp() {
        showUIPanel(
            typeof(GameUIPanelSettingsHelp),
            GameUIPanel.panelSettingsHelp,
            "SETTINGS: HELP");
    }

    public virtual void hideSettingsHelp() {
        hideUIPanel(
            typeof(GameUIPanelSettingsHelp));
    }

    // ------------------------------------------------------------
    // SETTINGS - CREDITS

    public virtual void showSettingsCredits() {
        showUIPanel(
            typeof(GameUIPanelSettingsCredits),
            GameUIPanel.panelSettingsCredits,
            "SETTINGS: CREDITS");
    }

    public virtual void hideSettingsCredits() {
        hideUIPanel(
            typeof(GameUIPanelSettingsCredits));
    }

    // ------------------------------------------------------------
    // RESULTS

    //
 
    //public static virtual void ShowUIPanelDialogResults() {
    //   if(isInst) {
    //       Instance.showResults();
    //   }
    //}
 
    //public static virtual void ShowResults() {
    //   if(isInst) {
    //       Instance.showResults();
    //   }
    //}
 
    public virtual void showResults() {  
     
        showUI();

        showUIPanel(
            typeof(GameUIPanelResults),
            GameUIPanel.panelResults,
            "RESULTS");

        //GameUIPanelFooter.ShowButtonCustomize();
     
        StartCoroutine(HideOverlay());
    }
 
    //public static virtual void HideResults() {
    //   if(isInst) {
    //       Instance.hideResults();
    //   }
    //}
    
    public virtual void hideResults() {
        hideUIPanel(
            typeof(GameUIPanelGameModeTrainingMode));
    }
 
 
    // ------------------------------------------------------------
    // EQUIPMENT - MAIN
 
    //public static void ShowEquipment() {
    //   if(isInst) {
    //       Instance.showEquipment();
    //   }
    //}
 
    public virtual void showEquipment() {
        showUIPanel(
            typeof(GameUIPanelEquipment),
            GameUIPanel.panelEquipment,
            "PLAYER CUSTOMIZE + PROGRESS");
    } 
 
    //public static void HideEquipment() {
    //   if(isInst) {
    //       Instance.hideEquipment();
    //   }
    //}
    
    public virtual void hideEquipment() {
        hideUIPanel(
            typeof(GameUIPanelEquipment));
    }
 
 
    // ------------------------------------------------------------
    // EQUIPMENT - STATISTICS
 
    //public static void ShowStatistics() {
    //   if(isInst) {
    //       Instance.showStatistics();
    //   }
    //}
 
    public virtual void showStatistics() {
        showUIPanel(
            typeof(GameUIPanelStatistics),
            GameUIPanel.panelStatistics,
            "STATISTICS");
    } 
 
    //public static void HideStatistics() {
    //   if(isInst) {
    //       Instance.hideStatistics();
    //   }
    //}
    
    public virtual void hideStatistics() {
        hideUIPanel(
            typeof(GameUIPanelStatistics));
    }

    // ------------------------------------------------------------
    // PRODUCTS - CURRENCY

    public virtual void showProductCurrency() {
        showUIPanel(
            typeof(GameUIPanelProductCurrency),
            GameUIPanel.panelProductCurrency,
            "COINS");
    }
    
    public virtual void hideProductCurrency() {
        hideUIPanel(
            typeof(GameUIPanelProductCurrency));
    }
        
    // ------------------------------------------------------------
    // PRODUCTS - CURRENCY EARN
    
    public virtual void showProductCurrencyEarn() {
        showUIPanel(
            typeof(GameUIPanelProductCurrencyEarn),
            GameUIPanel.panelProductCurrencyEarn,
            "COINS");
    }
    
    public virtual void hideProductCurrencyEarn() {
        hideUIPanel(
            typeof(GameUIPanelProductCurrencyEarn));
    }
     
    // ------------------------------------------------------------
    // EQUIPMENT - ACHIEVEMENTS
 
    //public static void ShowAchievements() {
    //   if(isInst) {
    //       Instance.showAchievements();
    //   }
    //}
 
    public virtual void showAchievements() {
        showUIPanel(
            typeof(GameUIPanelAchievements),
            GameUIPanel.panelAchievements,
            "ACHIEVEMENTS");
    } 
 
    //public static void HideAchievements() {
    //   if(isInst) {
    //       Instance.hideAchievements();
    //   }
    //}
    
    public virtual void hideAchievements() {
        hideUIPanel(
            typeof(GameUIPanelAchievements));
    }
     
    // ------------------------------------------------------------
    // EQUIPMENT - PRODUCTS
 
    //public static void ShowProducts() {
    //   if(isInst) {
    //       Instance.showProducts();
    //   }
    //}


    public virtual void showProducts() {
        GameUIController.ShowProducts("");
    }

    public virtual void showProducts(string productType) {

        string title = "PRODUCTS";

        showUIPanel(
            typeof(GameUIPanelProducts),
            GameUIPanel.panelProducts,
            title);
    } 
 
    //public static void HideProducts() {
    //   if(isInst) {
    //       Instance.hideProducts();
    //   }
    //}
    
    public virtual void hideProducts() {
        hideUIPanel(
            typeof(GameUIPanelProducts));
    }    
         
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE
 
    //public static void ShowCustomize() {
    //   if(isInst) {
    //       Instance.showCustomize();
    //   }
    //}
 
    public virtual void showCustomize() {
        showUIPanel(
            typeof(GameUIPanelCustomize),
            GameUIPanel.panelCustomize,
            "CUSTOMIZE");
    }
 
    //public static void HideCustomize() {
    //   if(isInst) {
    //       Instance.hideCustomize();
    //   }
    //}
 
    public virtual void hideCustomize() {
        hideUIPanel(
            typeof(GameUIPanelCustomize));
    }
 
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE AUDIO
 
    //public static void ShowCustomizeAudio() {
    //   if(isInst) {
    //       Instance.showCustomizeAudio();
    //   }
    //}
 
    /*
    public virtual void showCustomizeAudio() {   
     
     currentPanel = BaseUIPanel.panelCustomizeAudio;     
     
     HideAllPanelsNow();
     
     GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
     GameUIPanelHeader.Instance.AnimateInInternal(); 
     GameUIPanelHeader.ShowTitle("CUSTOMIZE");
     
     GameUIPanelCustomizeAudio.Instance.AnimateIn();     
    } 
    */
 
    //public static void HideCustomizeAudio() {
    //   if(isInst) {
    //       Instance.hideCustomizeAudio();
    //   }
    //}
 
    //public virtual void hideCustomizeAudio() {
    //   GameUIPanelCustomizeAudio.Instance.AnimateOut();
    //}
    
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE CHARACTER
    
    //public static void ShowCustomizeCharacter() {
    //   if(isInst) {
    //       Instance.showCustomizeCharacter();
    //   }
    //}
    
    public virtual void showCustomizeCharacter() {
        showUIPanel(
            typeof(GameUIPanelCustomizeCharacter),
            GameUIPanel.panelCustomizeCharacter,
            "CUSTOMIZE CHARACTER");
    }
    
    //public static void HideCustomizeCharacter() {
    //   if(isInst) {
    //       Instance.hideCustomizeCharacter();
    //   }
    //}
    
    public virtual void hideCustomizeCharacter() {
        hideUIPanel(
            typeof(GameUIPanelCustomizeCharacter));
    }
     
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE CHARACTER COLORS    
 
    //public static void ShowCustomizeCharacterColors() {
    //   if(isInst) {
    //       Instance.showCustomizeCharacterColors();
    //   }
    //}
 
    public virtual void showCustomizeCharacterColors() { 
        showUIPanel(
            typeof(GameUIPanelCustomizeCharacterColors),
            GameUIPanel.panelCustomizeCharacterColors,
            "CUSTOMIZE: PLAYER COLORS");
    } 
 
    //public static void HideCustomizeCharacterColors() {
    //   if(isInst) {
    //       Instance.hideCustomizeCharacterColors();
    //   }
    //}
    
    public virtual void hideCustomizeCharacterColors() {
        hideUIPanel(
            typeof(GameUIPanelCustomizeCharacterColors));
    }
 
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE CHARACTER RPG   
 
    //public static void ShowCustomizeCharacterRPG() {
    //   if(isInst) {
    //       Instance.showCustomizeCharacterRPG();
    //   }
    //}
 
    public virtual void showCustomizeCharacterRPG() {
        showUIPanel(
            typeof(GameUIPanelCustomizeCharacterRPG),
            GameUIPanel.panelCustomizeCharacterRPG,
            "CUSTOMIZE: PLAYER SKILLS");
    } 
 
    //public static void HideCustomizeCharacterRPG() {
    //   if(isInst) {
    //       Instance.hideCustomizeCharacterRPG();
    //   }
    //}
    
    public virtual void hideCustomizeCharacterRPG() {
        hideUIPanel(
            typeof(GameUIPanelCustomizeCharacterRPG));
    }
 
    IEnumerator HideOverlay() {
        yield return new WaitForSeconds(0.4f);
     
        GameUIPanelOverlays.Instance.HideOverlayWhiteStatic();
    }

    //   
 
    //public static virtual void ShowHeader() {
    //   if(isInst) {
    //       Instance.showHeader();
    //   }
    //}
 
    public virtual void showHeader() {
        GameUIPanelHeader.Instance.AnimateIn();
    } 
 
    //public static virtual void HideHeader() {
    //   if(isInst) {
    //       Instance.hideHeader();
    //   }
    //}
    
    public virtual void hideHeader() {
        hideUIPanel(
            typeof(GameUIPanelGameModeTrainingMode));
    }    
             
    // HERE
    
    public virtual void CloseAllActions() {  
     
    }
     
    //public static virtual void HideHUD() {
    //   if(isInst) {
    //       Instance.hideHUD();
    //   }
    //}
 
    public virtual void hideHUD() {
        //LogUtil.Log("HideHUD");
     
        hudVisible = false;    
     
        hideUIPauseButton();
        GameHUD.Instance.AnimateOut();
    }
 
    //public static virtual void ShowHUD() {
    //   if(isInst) {
    //       Instance.showHUD();
    //   }
    //}
 
    public virtual void showHUD() {
        //LogUtil.Log("ShowHUD");
     
        hudVisible = true;    
             
        showUIPauseButton();
        GameHUD.Instance.AnimateIn();
    }        
 
    //public static virtual void ShowUIPauseButton() {
    //   if(isInst) {
    //       Instance.showUIPauseButton();
    //   }
    //}
 
    public virtual void showUIPauseButton() {
        if (gamePauseButtonObject != null) {
            TweenPosition.Begin(gamePauseButtonObject, .3f, Vector3.zero.WithY(0));
        }
    }    
 
    //public virtual void HideUIPauseButton() {
    //   if(isInst) {
    ////     Instance.hideUIPauseButton();
    //   }
    //}
         
    public virtual void hideUIPauseButton() {
        if (gamePauseButtonObject != null) {
            TweenPosition.Begin(gamePauseButtonObject, .3f, Vector3.zero.WithY(650));
        }
    }
     
    //public static virtual bool SetDialogState(bool active) {   
    //   if(isInst) {
    //       return Instance.setDialogState(active);
    //   }
    //   return false;
    //}
 
    public bool setDialogState(bool active) {        
        dialogActive = active;
        GameDraggableEditor.editingEnabled = !dialogActive;
        return dialogActive;
    }
     
    // ------------------------------------------------------------------------
    // ALERT LAYERS
 
    //public static virtual void HideAllAlertLayers() {
    //   if(isInst) {
    //       Instance.hideAllAlertLayers();
    //   }
    //}
 
    public virtual void hideAllAlertLayers() {
        hideUIPanelAlertBackground();
        hideUIPanelPause();
        //hideUIPanelDialogResults();
    }
 
    //public static virtual void ShowUIPanelAlertBackground() {
    //   if(isInst) {
    //       Instance.showUIPanelAlertBackground();
    //   }
    //}
 
    public virtual void showUIPanelAlertBackground() {
        if (gamePauseDialogObject != null) {
            gameBackgroundAlertObject.Show();    
        }
    }
 
    //public static virtual void HideUIPanelAlertBackground() {
    //   if(isInst) {
    //       Instance.hideUIPanelAlertBackground();
    //   }
    //}
 
    public virtual void hideUIPanelAlertBackground() {
        if (gamePauseDialogObject != null) {
            gameBackgroundAlertObject.Hide();    
        }
    }
 
    //public static virtual void ShowUIPanelDialogPause() {
    //   if(isInst) {
    //       Instance.showUIPanelDialogPause();
    //   }
    //}
 
    public virtual void showUIPanelPause() {
        //HideAllAlertLayers();
        showUIPanelAlertBackground();
        UIPanelPause.Instance.AnimateIn();

        //if(gamePauseDialogObject != null) {
        //    TweenPosition.Begin(gamePauseDialogObject, .3f, Vector3.zero.WithY(0));
        //}
    }
 
    //public static virtual void HideUIPanelDialogPause() {
    //   if(isInst) {
    //       Instance.hideUIPanelDialogPause();
    //   }       
    //}
 
    public virtual void hideUIPanelPause() {
        hideUIPanelAlertBackground();
        UIPanelPause.Instance.AnimateOut();

        //if(gamePauseDialogObject != null) {
        //    TweenPosition.Begin(gamePauseDialogObject, .3f, Vector3.zero.WithY(5000));
        // }
    }
 
    /*
 public static virtual void ShowUIPanelDialogResults() {
     if(isInst) {
         Instance.showResults();
     }
 }
     
 public virtual void showUIPanelDialogResults() {
     hideAllAlertLayers();
     showUIPanelAlertBackground();
     if(gameResultsDialogObject!= null) {
         TweenPosition.Begin(gameResultsDialogObject, .3f, Vector3.zero.WithY(0));   
     }
 }
 */
 
    //public virtual void HideUIPanelDialogResults() {
    //   if(isInst) {
    //       Instance.hideResults();
    //   }
    //}
 
    // ------------------------------------------------------------------------
    // LEVEL/GAME UI LAYER
 
    //public static virtual void ShowGameCanvas() {
    //   if(isInst) {
    //       Instance.showGameCanvas();
    //   }
    //}
 
    public virtual void showGameCanvas() {
        if (gameContainerObject != null) {
            TweenPosition.Begin(gameContainerObject, 0f, Vector3.zero.WithY(0));
        }

        //gameContainerObject.Show();
        //TweenPosition.Begin(gameNavigationObject, .3f, Vector3.zero.WithX(0));
    }
 
    //public static virtual void HideGameCanvas() {
    //   if(isInst) {
    //       Instance.hideGameCanvas();
    //   }
    //}
 
    public virtual void hideGameCanvas() {

        if (gameContainerObject != null) {
            TweenPosition.Begin(gameContainerObject, 0f, Vector3.zero.WithY(0));
        }

        //gameContainerObject.Hide();

        //GameController.QuitGameRunning();

        //TweenPosition.Begin(gameNavigationObject, .3f, Vector3.zero.WithX(-970));  
    }

    // ------------------------------------------------------------------------

    // EVENTS

    public virtual void OnButtonClickObjectEventHandler(
        GameObject buttonObject) {
    
        Dictionary<string,object> data = new Dictionary<string, object>();

        if (buttonObject.Has<GameObjectData>()) {
            data = buttonObject.Get<GameObjectData>().ToDictionary();            

            Debug.Log("OnButtonClickObjectEventHandler:" + " data:" + data.ToJson());
            
            OnButtonClickDataEventHandler(buttonObject.name, data);
        }
        else {
            Messenger<string>.Broadcast(ButtonEvents.EVENT_BUTTON_CLICK, buttonObject.name);
        }
    }
    
    public virtual void OnButtonClickEventHandler(
        string buttonName) {        
        OnButtonClickDataEventHandler(buttonName, null);
    }
            
    public virtual void OnButtonClickDataEventHandler(
        string buttonName, 
        Dictionary<string,object> data = null) {

        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);

        if (data == null) {
            data = new Dictionary<string, object>();
        }
     
        hasBeenClicked = true;

        // GAME

        if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameQuit, buttonName)) {
            GameQuit();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePause, buttonName)) {
            GamePause();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameResume, buttonName)) {
            GameResume();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameRestart, buttonName)) {
            GameRestart();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameContinue, buttonName)) {
            GameContinue();
        }

        // UI / MODES
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameAchievements, buttonName)) {
            GameUIController.ShowAchievements();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameStatistics, buttonName)) {
            GameUIController.ShowStatistics();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameEquipmentRoom, buttonName)) {
            GameUIController.ShowEquipment();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameEquipment, buttonName)) {
            GameUIController.ShowEquipment();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameSettingsAudio, buttonName)) {
            GameUIController.ShowSettingsAudio();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameSettings, buttonName)) {
            GameUIController.ShowSettings();
        }

        // rating/community
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonAppRate, buttonName)) {
            Platforms.ShowReviewPage();
        }

        // Game networks

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCenterAchievements, buttonName)) {
            GameNetworks.ShowAchievementsOrLogin(GameNetworkType.gameNetworkAppleGameCenter);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCenterLeaderboards, buttonName)) {
            GameNetworks.ShowLeaderboardsOrLogin(GameNetworkType.gameNetworkAppleGameCenter);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePlayServicesAchievements, buttonName)) {
            GameNetworks.ShowAchievementsOrLogin(GameNetworkType.gameNetworkGooglePlayServices);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePlayServicesLeaderboards, buttonName)) {
            GameNetworks.ShowLeaderboardsOrLogin(GameNetworkType.gameNetworkGooglePlayServices);
        }

        /*
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.button) {
            GameUIController.ShowEquipment();
        }
        else if(buttonName == buttonOptions.name) {
            
            GameUIController.ShowSettings();
        }
        else if(buttonName == buttonOptionsAudio.name) {
            GameUIController.ShowSettings();
        }   
        else if(buttonName == buttonOptionsCredits.name) {
            GameUIController.ShowSettings();
        }   
        else if(buttonName == buttonOptionsRate.name) {
            GameUIController.ShowSettings();
        }   
        else if(buttonName == buttonOptionsSocial.name) {
            GameUIController.ShowSettings();
        }   
        */
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductCurrency, buttonName)) {    
            GameUIController.ShowProductCurrency();
        }   

        // Game Modes

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeArcade, buttonName)) {                        
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameArcade);
            //GameUIController.ShowGameModeArcade();
            GameUIController.ShowGameWorlds();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeChallenge, buttonName)) {            
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameChallenge);
            GameUIController.ShowGameModeChallenge();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMultiplayerCoop, buttonName)) {            
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMultiplayerCoop);
            GameUIController.ShowGameModeMultiplayerCoop();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMultiplayerMatchup, buttonName)) {            
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMultiplayerMatchup);
            GameUIController.ShowGameModeMultiplayerMatchup();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMultiplayer, buttonName)) {            
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMultiplayer);
            GameUIController.ShowGameModeMultiplayer();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeCoop, buttonName)) {            
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameCoop);
            GameUIController.ShowGameModeCoop(); // non multiplayer coop with co bots
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMissions, buttonName)) {            
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMissions);
            GameUIController.ShowGameWorlds();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeTraining, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameTraining);
            GameUIController.ShowGameModeTrainingMode();
        }

        // PRODUCTS
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsWeapon, buttonName)) {
            GameUIController.ShowProducts(GameProductType.weapon);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsCharacterSkin, buttonName)) {
            GameUIController.ShowProducts(GameProductType.characterSkin);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsCharacter, buttonName)) {
            GameUIController.ShowProducts(GameProductType.character);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsCurrency, buttonName)) {
            GameUIController.ShowProducts(GameProductType.currency);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsFeature, buttonName)) {
            GameUIController.ShowProducts(GameProductType.feature);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsPickup, buttonName)) {
            GameUIController.ShowProducts(GameProductType.pickup);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsPowerup, buttonName)) {
            GameUIController.ShowProducts(GameProductType.powerup);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsRPGUpgrade, buttonName)) {
            GameUIController.ShowProducts(GameProductType.rpgUpgrade);
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProducts, buttonName)) {
            GameUIController.ShowProducts();
        }

        // ACTION ITEMS / USE

        else if (buttonName.IndexOf(BaseUIButtonNames.buttonGameActionItemBuyUse + "$") > -1) {
            
            string productCodeUse = "";
            string productTypeUse = "";
            string productCharacterUse = "";
            
            string[] commandActionParams = buttonName.Replace(BaseUIButtonNames.buttonGameActionItemBuyUse + "$", "").Split('$');
            
            if (commandActionParams.Length > 0)
                productTypeUse = commandActionParams[0];
            if (commandActionParams.Length > 1)
                productCodeUse = commandActionParams[1];
            if (commandActionParams.Length > 2)
                productCharacterUse = commandActionParams[2];
            
            if (!string.IsNullOrEmpty(productTypeUse)
                && !string.IsNullOrEmpty(productCodeUse)
                && !string.IsNullOrEmpty(productCharacterUse)) {
                
                GameStoreController.Purchase(productCodeUse, 1);

                GameUIPanelProducts.LoadData();
            }
        }

        // CUSTOMIZE
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterColors, buttonName)) {
            GameUIController.ShowCustomizeCharacterColors();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterRPG, buttonName)) {
            GameUIController.ShowCustomizeCharacterRPG();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacter, buttonName)) {
            GameUIController.ShowCustomizeCharacter();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomize, buttonName)) {
            GameUIController.ShowCustomize();
        }

        // COMMUNITY
                
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityClose, buttonName)) {
            GameCommunity.HideGameCommunity();
        }       

        // COMMUNITY - BROADCAST
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastRecordStart, buttonName)) {
            BroadcastNetworks.StartRecording();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastRecordStop, buttonName)) {
            BroadcastNetworks.StopRecording();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastRecordToggle, buttonName)) {
            BroadcastNetworks.ToggleRecording();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastOpen, buttonName)) {
            UIPanelCommunityBroadcast.ShowDialog();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastNetworkOpen, buttonName)) {
            BroadcastNetworks.Open();
        }

        // 

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastReplayShare, buttonName)) {
            BroadcastNetworks.OpenSharing();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastReplayWatch, buttonName)) {
            BroadcastNetworks.PlayLastRecording();
        }

        //else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastNetworkOpenSharing, buttonName)) {
        //    BroadcastNetworks.OpenSharing();
        //}

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastFacecamStart, buttonName)) {
            BroadcastNetworks.FacecamStart();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastFacecamStop, buttonName)) {
            BroadcastNetworks.FacecamStop();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastFacecamToggle, buttonName)) {
            BroadcastNetworks.FacecamToggle();
        }

        // COMMUNITY - CAMERA

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraTakePhoto, buttonName)) {
            UIPanelCommunityCamera.TakePhoto();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraSaveFacebook, buttonName)) {
            GameCommunitySocialController.StartPhotoUploadToFacebook();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraSaveTwitter, buttonName)) {
            GameCommunitySocialController.StartPhotoUploadToTwitter();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraSaveLibrary, buttonName)) {
            GameCommunitySocialController.SaveImageToLibraryDefault();
        }

        // COMMUNITY - RESULTS / SHARE

        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityShareResultFacebook, buttonName)) {
            //GameCommunitySocialController.PostGameResultsFacebook();
            UIPanelCommunityCamera.TakePhotoGameState();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityShareResultTwitter, buttonName)) {
            //GameCommunitySocialController.PostGameResultsTwitter();
            UIPanelCommunityCamera.TakePhotoGameState();
        }

        // CUSTOMIZE 

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterZoomIn, buttonName)) {
            GameUIPanelHeader.CharacterLargeZoomIn();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterZoomOut, buttonName)) {
            GameUIPanelHeader.CharacterLargeZoomOut();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterFront, buttonName)) {
            GameUIPanelHeader.CharacterLargeShowFront();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterBack, buttonName)) {
            GameUIPanelHeader.CharacterLargeShowBack();
        }

        // GAME INIT
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameInitFinish, buttonName)) {

            GameCommunity.HideGameCommunity();

            UIPanelOverlayPrepare.HideAll();

            GameController.InitLevelFinish();
        }  
        
        // GAME PRESET CHARACTER COLORS
        
        else if (UIUtil.IsButtonClickedLike(UIButtonNames.buttonGamePlayerPresets, buttonName)) {
            
            GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;
            
            string[] arrButtonName = buttonName.Split('$');
            
            string presetTexture = "";
            string presetColor = "";
            
            string panelNext = "";
            
            string markerTexture = "preset-texture--";
            string markerColor = "preset-color--";
            string markerNext = "panel-next--";
            
            if (arrButtonName != null) {
                
                foreach (string s in arrButtonName) {
                    if (s.Contains(markerTexture)) {
                        presetTexture = s.Replace(markerTexture, "");
                    }
                    if (s.Contains(markerColor)) {
                        presetColor = s.Replace(markerColor, "");
                    }
                    if (s.Contains(markerNext)) {
                        panelNext = s.Replace(markerNext, "");
                    }
                }
            }
            
            bool loadCharacter = false;
            
            if (!string.IsNullOrEmpty(presetTexture)) {
                // SET CUSTOM VALUES FOR THIS PLAYER
                
                customItem = GameCustomController.UpdateTexturePresetObject(
                    customItem, GameController.CurrentGamePlayerController.gameObject,  
                    AppContentAssetTexturePresets.Instance.GetByCode(presetTexture));
                
                loadCharacter = true;
            }
            
            
            if (!string.IsNullOrEmpty(presetColor)) {
                customItem = GameCustomController.UpdateColorPresetObject(
                    customItem, GameController.CurrentGamePlayerController.gameObject,   
                    AppColorPresets.Instance.GetByCode(presetColor));
                
                loadCharacter = true;
            }
            
            if (loadCharacter) {                
                GameCustomController.SaveCustomItem(customItem); 
                GameController.LoadCurrentProfileCharacter();
            }
            
            if (!string.IsNullOrEmpty(panelNext)) {
                if (panelNext == BaseUIPanel.panelGameMode) {                    
                    GameUIController.ShowGameMode(); 
                }
            }
        }

        
        // COIN / CURRENCY
        
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonProductCurrency, buttonName)) {
            
            GameCommunity.HideGameCommunity();
            
            showProductCurrency();
        }  

        // LAST 

        else if (UIUtil.IsButtonClickedLike(BaseUIButtonNames.buttonGamePlay, buttonName)
            || UIUtil.IsButtonClickedLike(BaseUIButtonNames.buttonGameModePlay, buttonName)) {  
            
            object dataType = null;
            object dataCode = null;
            
            if (data.ContainsKey(BaseDataObjectKeys.type)) {
                dataType = data.Get(BaseDataObjectKeys.type);
            }
            if (data.ContainsKey(BaseDataObjectKeys.code)) {
                dataCode = data.Get(BaseDataObjectKeys.code);
            }
            
            if (dataType != null) {

                // COLLECTION LOAD - MISSION
                
                if (dataType.ToString() == BaseDataObjectKeys.mission) {
                    
                    string code = dataCode.ToString();
                    
                    if (!string.IsNullOrEmpty(code)) {

                        AppContentCollect appContentCollect = AppContentCollects.Instance.GetById(code);

                        if(appContentCollect != null) {
                            
                            AppContentCollects.ChangeCurrent(code);
                            
                            Debug.Log("ACTION:" + " mission:" + code);

                            if(appContentCollect.data != null) {
                                string levelTo = "2-1";
                                int worldTo = GameWorlds.Current.data.world_num;
                                levelTo = string.Format("{0}-{1}", worldTo, appContentCollect.GetLevelSuffixRandom());
                                GameLevels.Instance.ChangeCurrent(levelTo);
                            }
                        }
                    }
                }
            }
            
            GameCommunity.HideGameCommunity();
            GameCommunity.HideBroadcastRecordPlayShare();

            GameController.PlayGame();
        }

        // BACK BUTTON

        else {
            if (buttonName == BaseUIButtonNames.buttonBack) {
                
                GameCommunity.HideGameCommunity();

                NavigateBack(buttonName);
            }
        }

        GameUIController.HandleNetworkedButtons(buttonName);

        GameUIController.HandleHUDButtons(buttonName);
     
        /*
        if(buttonName.IndexOf("FreeDownloadButton") > -1) {
        
        }       
        else if(buttonName.IndexOf("ButtonCloseAction") > -1) {
        CloseAllActions();
        gameButtonObject.Show();
        }
        
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonPlayLevel) > -1) {
        
        string[] arrLevelId = buttonName.Split('$');
        
        if(arrLevelId.Length > 0) {
        string levelId = arrLevelId[1];
        
        LogUtil.Log("ButtonPlayLevel: levelId:" + levelId);
        
        GameAppController.Instance.LoadLevel(levelId);
        }
        
        }
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameLevelItemObject) > -1) {
        
        string[] arrLevelId = buttonName.Split('$');
        
        if(arrLevelId.Length > 0) {
        string assetCodeCreating =  arrLevelId[1];
        GameDraggableEditor.assetCodeCreating = assetCodeCreating;
        LogUtil.Log("GameDraggableEditor.assetCodeCreating:" + GameDraggableEditor.assetCodeCreating);
        
        if(UIPanelEditAsset.Instance.actionState != UIPanelEditAssetActionState.NONE) {
         
         if(UIPanelEditAsset.Instance.actionState == UIPanelEditAssetActionState.SELECT_ITEM) {
             UIPanelEditAsset.Instance.UpdateSprite(assetCodeCreating);
         }       
         else if(UIPanelEditAsset.Instance.actionState == UIPanelEditAssetActionState.SELECT_EFFECT) {
             UIPanelEditAsset.Instance.UpdateSpriteEffect(assetCodeCreating);
         }       
         
         UIPanelEditAsset.Instance.actionState = UIPanelEditAssetActionState.NONE;
        }
        
        HideUIPanelDialogItems();
        ShowUIPanelEdit();
        }
        
        }
        
        // UI       
        
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonLevels) > -1) {
            SetSectionLabelDelayed("Levels", .3f);
            ShowPanelByName(BaseUIPanel.panelLevels);
        // -[worldcode]-[page]
        }    
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonLevel) > -1) {
            SetSectionLabelDelayed("Level", .3f);
            ShowPanelByName(BaseUIPanel.panelLevel);
        }    
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonSettings) > -1) {
            SetSectionLabelDelayed("Settings", .3f);
            ShowPanelByName(BaseUIPanel.panelSettings);
        }      
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonCredits) > -1) {
            SetSectionLabelDelayed("Credits", .3f);
            ShowPanelByName(BaseUIPanel.panelCredits);
        }     
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonSocial) > -1) {
            SetSectionLabelDelayed("Social", .3f);
            ShowPanelByName(BaseUIPanel.panelSocial);
        }   
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonTrophyStatistics) > -1) {
            SetSectionLabelDelayed("Statistics", .3f);
            ShowPanelByName(BaseUIPanel.panelTrophyStatistics);
            UIPanelTrophyStatistics.LoadData();
        }      
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonTrophyAchievements) > -1) {
            SetSectionLabelDelayed("Achievements", .3f);
            ShowPanelByName(BaseUIPanel.panelTrophyAchievements);
            UIPanelTrophyAchievements.LoadData();
        }    
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonWorlds) > -1) {
            SetSectionLabelDelayed("Worlds", .3f);
            ShowPanelByName(BaseUIPanel.panelWorlds);
        }
        
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameCenterAchievements) > -1) {
            GameNetworks.Instance.ShowAchievementsOrLogin();
        }  
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameCenterLeaderboards) > -1) {
            GameNetworks.Instance.ShowLeaderboardsOrLogin();
        }

        else if(buttonName.IndexOf(BaseUIButtonNames.buttonMain) > -1) {
            SetSectionLabelDelayed("Main", .3f);
    
            HideUIPanelEditButton();
            HideAllUIEditPanels();
            HideAllAlertLayers();
            HideGameCanvas();
            HideHUD();
            HideAllEditDialogs();
    
            GameController.Instance.QuitGame();
    
            ShowPanelByName(BaseUIPanel.panelMain);
        }
     */
    }

    public bool AllowPress(float lastTime) {

        if (lastTime + .1f < Time.time) {
            lastTime = Time.time;
            return true;
        }

        return true;
    }
    
    public virtual void handleNetworkedButtons(string buttonName) {
        // handle network state by buttons and areas
        if (AppConfigs.featureEnableNetworking) {
            if (UIUtil.IsButtonClickedLike("GameMode", buttonName)) { 
                if (UIUtil.IsButtonClicked(UIButtonNames.buttonGameModeCoop, buttonName)) { 
                    //Gameverses.GameNetworking.Connect();
                }
                else {
                    //Gameverses.GameNetworking.Disconnect();
                }
            }  
        }
    }
 
    public virtual void handleHUDButtons(string buttonName) {
        if (AllowPress(lastPressAttack)
            && buttonName == BaseHUDButtonNames.buttonInputAttack) {
            GameController.GamePlayerAttack();
        }
        else if (AllowPress(lastPressAttackAlt)
            && buttonName == BaseHUDButtonNames.buttonInputAttackAlt) {
            GameController.GamePlayerAttackAlt();
        }
        else if (AllowPress(lastPressAttackRight)
            && buttonName == BaseHUDButtonNames.buttonInputAttackRight) {
            GameController.GamePlayerAttackRight();
        }
        else if (AllowPress(lastPressAttackLeft)
            && buttonName == BaseHUDButtonNames.buttonInputAttackLeft) {
            GameController.GamePlayerAttackLeft();
        }
        else if (AllowPress(lastPressDefend)
            && buttonName == BaseHUDButtonNames.buttonInputDefend) {
            GameController.GamePlayerDefend();
        }
        else if (AllowPress(lastPressDefendAlt)
            && buttonName == BaseHUDButtonNames.buttonInputDefendAlt) {
            GameController.GamePlayerDefendAlt();
        }
        else if (AllowPress(lastPressDefendRight)
            && buttonName == BaseHUDButtonNames.buttonInputDefendRight) {
            GameController.GamePlayerDefendRight();
        }
        else if (AllowPress(lastPressDefendLeft)
            && buttonName == BaseHUDButtonNames.buttonInputDefendLeft) {
            GameController.GamePlayerDefendLeft();
        }
        else if (AllowPress(lastPressSkill)
            && buttonName == BaseHUDButtonNames.buttonInputSkill) {
            GameController.GamePlayerSkill();
        }
        else if (AllowPress(lastPressMagic)
            && buttonName == BaseHUDButtonNames.buttonInputMagic) {
            GameController.GamePlayerMagic();
        }
        else if (AllowPress(lastPressUse)
            && buttonName == BaseHUDButtonNames.buttonInputUse) {
            GameController.GamePlayerUse();
        }
        else if (AllowPress(lastPressMount)
            && buttonName == BaseHUDButtonNames.buttonInputMount) {
            GameController.GamePlayerMount();
        }
        else if (buttonName == BaseHUDButtonNames.buttonInputJump) {
            GameController.GamePlayerJump();
        }
        else if (buttonName == BaseHUDButtonNames.buttonInputInventoryWeapon) {
            //GameController.GamePlayerJump();
        }
        else if (buttonName == BaseHUDButtonNames.buttonInputInventoryWeaponNext) {
            GameController.CurrentGamePlayerController.LoadWeaponNext();
        }          
    }
 
    public virtual void GameContinue() {     
        GameController.QuitGame();
    }
  
    public virtual void GameRestart() {
        GameController.RestartGame();
    }
 
    public virtual void GameResume() {
        GameController.ResumeGame();
    }
 
    public virtual void GamePause() {
        GameController.TogglePauseGame();
    }
 
    public virtual void GameQuit() {
        GameController.QuitGame();
        GameUIController.ShowMain();
    }
}
