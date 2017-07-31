#pragma warning disable 0169
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
    public static string buttonGameNetworkAchievements = "ButtonGameNetworkAchievements";
    public static string buttonGameNetworkLeaderboards = "ButtonGameNetworkLeaderboards";
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
    public static string buttonGameProductsAccess = "ButtonGameProductsAccess";
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
    public static string buttonProfileGameversesSync = "ButtonProfileGameversesSync";

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
    
    public static string buttonAR = "ButtonAR";
    public static string buttonARSettings = "ButtonARSettings";
    public static string buttonARSettingsSelect = "ButtonARSettingsSelect";
    //
    public static string buttonVR = "ButtonVR";
    public static string buttonVRSettings = "ButtonVRSettings";

    // 
    public static string buttonActionUrl = "ButtonActionUrl";

    public static string buttonStoreRestorePurchases = "ButtonStoreRestorePurchases";
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
    //
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
    //
    public static string panelSettings = "PanelSettings";
    public static string panelSettingsAudio = "PanelSettingsAudio";
    public static string panelSettingsControls = "PanelSettingsControls";
    public static string panelSettingsProfile = "PanelSettingsProfile";
    public static string panelSettingsHelp = "PanelSettingsHelp";
    public static string panelSettingsCredits = "PanelSettingsCredits";
    //
    public static string panelGameMode = "PanelGameMode";
    public static string panelGameModeCoop = "PanelGameModeCoop";
    public static string panelGameModeMultiplayer = "PanelGameModeMultiplayer";
    public static string panelGameModeMultiplayerCoop = "PanelGameModeMultiplayerCoop";
    public static string panelGameModeMultiplayerMatchup = "PanelGameModeMultiplayerMatchup";
    public static string panelGameModeMissions = "PanelGameModeMissions";
    public static string panelGameModeArcade = "PanelGameModeArcade";
    public static string panelGameModeCareer = "PanelGameModeCareer";
    public static string panelGameModeChallenge = "PanelGameModeChallenge";
    //
    public static string panelGameModeCustomize = "PanelGameModeCustomize";
    //
    public static string panelGameModeTraining = "PanelGameModeTraining";
    public static string panelGameModeTrainingMode = "PanelGameModeTrainingMode";
    public static string panelGameModeTrainingModeChoice = "PanelGameModeTrainingModeChoice";
    public static string panelGameModeTrainingModeCollection = "PanelGameModeTrainingModeChoice";
    public static string panelGameModeTrainingModeContent = "PanelGameModeTrainingModeContent";
    public static string panelGameModeTrainingModeRPGHealth = "PanelGameModeTrainingModeRPGHealth";
    public static string panelGameModeTrainingModeRPGEnergy = "PanelGameModeTrainingModeRPGEnergy";
    //
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
    //
    public static string panelProductCurrency = "PanelProductCurrency";
    public static string panelProductCurrencyEarn = "PanelProductCurrencyEarn";
    //
    public static string panelCustomize = "PanelCustomize";
    public static string panelCustomizeLevels = "PanelCustomizeLevels";
    public static string panelCustomizeWorlds = "PanelCustomizeWorlds";
    public static string panelCustomizeCharacter = "PanelCustomizeCharacter";
    public static string panelCustomizeCharacterColors = "PanelCustomizeCharacterColors";
    public static string panelCustomizeCharacterRPG = "PanelCustomizeCharacterRPG";
    public static string panelCustomizeAudio = "PanelCustomizeAudio";
    public static string panelCustomSafety = "PanelCustomSafety";
    public static string panelCustomSmarts = "panelCustomSmarts";
    //
    public static string panelCommunityCamera = "PanelCommunityCamera";
    public static string panelCommunityComment = "PanelCommunityComment";
    //
    //
    public static string panelAR = "PanelAR";
    public static string panelARSettings = "PanelARSettings";
    public static string panelVR = "PanelVR";
    public static string panelVRSettings = "PanelVRSettings";
    //
    //public static string panelVR = "PanelVR";
    //public static string panelVRSettings = "PanelVRSettings";
    //public static string panelGameModeVR = "PanelGameModeVR";
    //public static string panelGameModeVRSettings = "PanelGameModeVRSettings";
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

    public Camera camHud = null;
    public Camera camUI = null;
    public Camera camDialog = null;
    public Camera camOverlay = null;

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

        Messenger<string, Dictionary<string, object>>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_DATA, OnButtonClickDataEventHandler);

        Messenger<GameObject>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectEventHandler);

        //Messenger<string, string, bool>.AddListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
        //Messenger<string, string>.AddListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);

        //Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);

        //Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);

        //Messenger<GameObject>.AddListener(
        // GameDraggableEditorMessages.editorGrabbedObjectChanged, OnEditorGrabbedObjectChanged);
    }

    public virtual void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string, Dictionary<string, object>>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK_DATA, OnButtonClickDataEventHandler);

        Messenger<GameObject>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK_OBJECT, OnButtonClickObjectEventHandler);

        //Messenger<string, string, bool>.RemoveListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
        //Messenger<string, string>.RemoveListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);     
        //Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);     
        //Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);

        //Messenger<GameObject>.RemoveListener(
        // GameDraggableEditorMessages.editorGrabbedObjectChanged, OnEditorGrabbedObjectChanged);
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

        if(GameUIController.isInst) {
            return GameUIController.Instance.isUIPanel(panelCodeCheck);
        }

        return false;
    }

    public bool isUIPanel(string panelCodeCheck) {
        if(currentPanel == panelCodeCheck) {
            return true;
        }

        return false;
    }

    public static bool IsUIPanelLike(string panelCodeCheck) {

        if(GameUIController.isInst) {
            return GameUIController.Instance.isUIPanelLike(panelCodeCheck);
        }

        return false;
    }

    public bool isUIPanelLike(string panelCodeCheck) {
        if(currentPanel.Contains(panelCodeCheck)) {
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

        //if (currentPanel == panelCode) {
        // Don't reload
        //return;
        //}

        //if (currentPanel != panelCode) {
        //
        //}


        //Debug.Log("<color=#ffa500ff>GameUIController:showUIPanel():</color> objName:" + objName + " panelCode:" + panelCode);

        currentPanel = panelCode;

        AnalyticsNetworks.LogEventSceneChange(panelCode, title);

        //HideAllPanelsNow();
        HideAllPanels();

        //if(panelCode != BaseUIPanel.panelMain) {
            broadcastUIMessageAnimateType(
                "GameUIPanelBackgrounds",
                UIControllerAnimateTypes.uiPanelAnimateTypeInBetween); // starry
        //}

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
        foreach(GameUIPanelBase baseItem in FindObjectsOfType(typeof(GameUIPanelBase))) {
            baseItem.AnimateOut();
        }

        foreach(UIPanelBase baseItem in FindObjectsOfType(typeof(UIPanelBase))) {
            baseItem.AnimateOut();
        }
    }

    public virtual void HideAllPanelsNow() {
        foreach(GameUIPanelBase baseItem in FindObjectsOfType(typeof(GameUIPanelBase))) {
            baseItem.AnimateOutNow();
        }

        foreach(UIPanelBase baseItem in FindObjectsOfType(typeof(UIPanelBase))) {
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
        if(!hasBeenClicked) {
            showMain();
        }
    }

    public virtual void Update() {

        if(Application.isEditor) {

            if(Input.GetKey(KeyCode.LeftControl)) {

                if(Input.GetKey(KeyCode.Space)) {

                    if(Input.GetKeyDown(KeyCode.Period)) {
                        GameController.GamePlayerUse();
                    }

                    if(Input.GetKeyDown(KeyCode.C)) {
                        //UIPanelCommunityCamera.CaptureCameraPhotoState();
                    }
                }
            }
        }
    }

    public virtual void ToggleUI() {

        LogUtil.Log("ToggleUI uiVisible: " + uiVisible);

        if(uiVisible) {
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

        if(buttonName == BaseUIButtonNames.buttonBack) {
            if(!GameUIController.Instance.uiVisible) {
                HideAllPanels();
                GameUIPanelHeader.Instance.AnimateOut();
                GameUIPanelBackgrounds.Instance.AnimateOut();
                GameHUD.Instance.AnimateIn();
                handled = true;
            }
            else {
                if(isUIPanel(GameUIPanel.panelSettingsAudio)
                    || isUIPanel(GameUIPanel.panelSettingsControls)
                    || isUIPanel(GameUIPanel.panelSettingsHelp)
                    || isUIPanel(GameUIPanel.panelSettingsCredits)
                    || isUIPanel(GameUIPanel.panelSettingsProfile)) {

#if ENABLE_FEATURE_SETTINGS
                    GameUIController.ShowSettings();
                    handled = true;
#endif
                }
                else if(isUIPanel(GameUIPanel.panelAchievements)
                    || isUIPanel(GameUIPanel.panelCustomize)
                    || isUIPanel(GameUIPanel.panelProducts)
                    || isUIPanel(GameUIPanel.panelStatistics)) {

                    GameUIController.ShowEquipment();
                    handled = true;

                }

#if ENABLE_FEATURE_CHARACTER_CUSTOMIZE
                else if(isUIPanel(GameUIPanel.panelCustomizeCharacterRPG)
                    || isUIPanel(GameUIPanel.panelCustomizeCharacterColors)
                         || isUIPanel(GameUIPanel.panelCustomizeCharacter)
                         || isUIPanel(GameUIPanel.panelCustomizeLevels)
                         || isUIPanel(GameUIPanel.panelCustomizeWorlds)
                    || isUIPanel(GameUIPanel.panelCustomizeAudio)) {

                    GameUIController.ShowCustomize();
                    handled = true;

                }
#endif
                else if(isUIPanel(GameUIPanel.panelGameModeArcade)
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

#if ENABLE_FEATURE_GAME_MODE
                    GameUIController.ShowGameMode();
                    handled = true;
#endif

                }

#if ENABLE_FEATURE_GAME_MODE
                else if(isUIPanel(GameUIPanel.panelGameModeTrainingModeContent)
                    || isUIPanelLike(GameUIPanel.panelGameModeTrainingModeChoice)
                    || isUIPanelLike(GameUIPanel.panelGameModeTrainingModeCollection)) {

                    //GameUIController.ShowGameModeTrainingMode();
                    GameUIController.ShowGameMode();
                    handled = true;

                }
#endif
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

        if(!gameLoopsStarted) {
            GameAudio.StartGameLoops();
        }

        GameAudio.StopAmbience();

        if(playAudio) {
            int loopToPlay = UnityEngine.Random.Range(1, 4);

            LogUtil.Log("HandleInGameAudio:", " loopToPlay:" + loopToPlay.ToString());

            GameAudio.StartGameLoop(loopToPlay);
        }
        inUIAudioPlaying = false;
    }

    public virtual void HandleInUIAudio() {

        if(!inUIAudioPlaying) {
            GameAudio.StartGameLoop(-1);
            GameAudio.StartAmbience();
            inUIAudioPlaying = true;
        }
    }

    public virtual void ToggleGameUI() {

        gameUIExpanded = gameUIExpanded ? false : true;

        LogUtil.Log("toggling:" + gameUIExpanded);

        if(gameUIExpanded) {
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

        if(now) {
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


#if ENABLE_FEATURE_GAME_MODE

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

#endif


#if ENABLE_FEATURE_GAME_MODE_COOP

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

#endif

#if ENABLE_FEATURE_NETWORKING

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
#endif

#if ENABLE_FEATURE_GAME_MODE_MISSIONS
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

#endif

#if ENABLE_FEATURE_WORLDS
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

#endif


#if ENABLE_FEATURE_AR

    // ------------------------------------------------------------
    // AR

    //public static virtual void ShowARSettings() {
    //   if(isInst) {
    //       Instance.showARSettings();
    //   }
    //}

    public virtual void showARSettings() {

        showUIPanel(
            typeof(GameUIPanelARSettings),
            BaseUIPanel.panelARSettings,
            "AR");
    }

    //public static virtual void HideARSettings() {
    //   if(isInst) {
    //       Instance.hideARSettings();
    //   }
    //}

    public virtual void hideARSettings() {
        hideUIPanel(
            typeof(GameUIPanelARSettings));
    }

    //public static virtual void ShowAR() {
    //   if(isInst) {
    //       Instance.showAR();
    //   }
    //}

    public virtual void showAR() {

        showUIPanel(
            typeof(GameUIPanelAR),
            BaseUIPanel.panelAR,
            "AR");
    }

    //public static virtual void HideAR() {
    //   if(isInst) {
    //       Instance.hideAR();
    //   }
    //}

    public virtual void hideAR() {
        hideUIPanel(
            typeof(GameUIPanelAR));
    }

    // ------------------------------------------------------------
    // VR

    //public static virtual void ShowVRSettings() {
    //   if(isInst) {
    //       Instance.showVRSettings();
    //   }
    //}

    public virtual void showVRSettings() {

        showUIPanel(
            typeof(GameUIPanelVRSettings),
            BaseUIPanel.panelVRSettings,
            "VR");
    }

    //public static virtual void HideVRSettings() {
    //   if(isInst) {
    //       Instance.hideVRSettings();
    //   }
    //}

    public virtual void hideVRSettings() {
        hideUIPanel(
            typeof(GameUIPanelVRSettings));
    }

    //public static virtual void ShowVR() {
    //   if(isInst) {
    //       Instance.showVR();
    //   }
    //}

    public virtual void showVR() {

        showUIPanel(
            typeof(GameUIPanelVR),
            BaseUIPanel.panelVR,
            "VR");
    }

    //public static virtual void HideVR() {
    //   if(isInst) {
    //       Instance.hideVR();
    //   }
    //}

    public virtual void hideVR() {
        hideUIPanel(
            typeof(GameUIPanelVR));
    }

#endif


#if ENABLE_FEATURE_TRAINING

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

#endif


#if ENABLE_FEATURE_GAME_MODE_CHALLENGE
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

#endif

#if ENABLE_FEATURE_GAME_MODE_ARCADE

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

#endif

#if ENABLE_FEATURE_GAME_MODE_CUSTOMIZE
    
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

#endif

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

#if ENABLE_FEATURE_SETTINGS
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

#endif

#if ENABLE_FEATURE_SETTINGS_AUDIO

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

#endif

#if ENABLE_FEATURE_SETTINGS_CONTROLS
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

#endif

#if ENABLE_FEATURE_SETTINGS_PROFILE

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

#endif

#if ENABLE_FEATURE_SETTINGS_HELP

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

#endif

#if ENABLE_FEATURE_SETTINGS_CREDITS

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

#endif

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
            typeof(GameUIPanelResults));
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

#if ENABLE_FEATURE_PRODUCT_CURRENCY

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

#endif

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


#if ENABLE_FEATURE_PRODUCTS

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

#endif

#if ENABLE_FEATURE_CHARACTER_CUSTOMIZE

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

#endif

    // ------------------------------------------------------------
    IEnumerator HideOverlay() {
        yield return new WaitForSeconds(0.4f);

        GameUIPanelOverlays.Instance.HideOverlayWhiteStatic();
    }

    // ------------------------------------------------------------
    //   

    //public static virtual void ShowHeader() {
    //   if(isInst) {
    //       Instance.showHeader();
    //   }
    //}

    public virtual void showHeader() {
        GameUIPanelHeader.Instance.AnimateIn();
    }

    // ------------------------------------------------------------
    //public static virtual void HideHeader() {
    //   if(isInst) {
    //       Instance.hideHeader();
    //   }
    //}

    public virtual void hideHeader() {
        hideUIPanel(
            typeof(GameUIPanelHeader));
    }

    // ------------------------------------------------------------
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

    // ------------------------------------------------------------
    //public static virtual void ShowUIPauseButton() {
    //   if(isInst) {
    //       Instance.showUIPauseButton();
    //   }
    //}

    public virtual void showUIPauseButton() {
        if(gamePauseButtonObject != null) {
            TweenPosition.Begin(gamePauseButtonObject, .3f, Vector3.zero.WithY(0));
        }
    }

    //public virtual void HideUIPauseButton() {
    //   if(isInst) {
    ////     Instance.hideUIPauseButton();
    //   }
    //}

    public virtual void hideUIPauseButton() {
        if(gamePauseButtonObject != null) {
            TweenPosition.Begin(gamePauseButtonObject, .3f, Vector3.zero.WithY(650));
        }
    }

    // ------------------------------------------------------------
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
        if(gamePauseDialogObject != null) {
            gameBackgroundAlertObject.Show();
        }
    }

    //public static virtual void HideUIPanelAlertBackground() {
    //   if(isInst) {
    //       Instance.hideUIPanelAlertBackground();
    //   }
    //}

    public virtual void hideUIPanelAlertBackground() {
        if(gamePauseDialogObject != null) {
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
        if(gameContainerObject != null) {
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

        if(gameContainerObject != null) {
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

        Debug.Log("OnButtonClickObjectEventHandler:" + buttonObject.name);

        Dictionary<string, object> data = new Dictionary<string, object>();

        if(buttonObject.Has<GameObjectData>()) {
            data = buttonObject.Get<GameObjectData>().ToDictionary();

            Debug.Log("OnButtonClickObjectEventHandler:" + " data:" + data.ToJson());

            //OnButtonClickDataEventHandler(buttonObject.name, data);

            Messenger<string, Dictionary<string, object>>.Broadcast(
                ButtonEvents.EVENT_BUTTON_CLICK_DATA, buttonObject.name, data);
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
        Dictionary<string, object> data = null
        ) {

        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);

        if(data == null) {
            data = new Dictionary<string, object>();
        }

        hasBeenClicked = true;

        // GAME

        if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameQuit, buttonName)) {
            GameQuit();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePause, buttonName)) {
            GamePause();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameResume, buttonName)) {
            GameResume();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameRestart, buttonName)) {
            GameRestart();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameContinue, buttonName)) {
            GameContinue();
        }

        // UI / MODES

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameAchievements, buttonName)) {
            GameUIController.ShowAchievements();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameStatistics, buttonName)) {
            GameUIController.ShowStatistics();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameEquipmentRoom, buttonName)) {
            GameUIController.ShowEquipment();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameEquipment, buttonName)) {
            GameUIController.ShowEquipment();
        }
#if ENABLE_FEATURE_SETTINGS_AUDIO
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameSettingsAudio, buttonName)) {
            GameUIController.ShowSettingsAudio();
        }
#endif

#if ENABLE_FEATURE_SETTINGS

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameSettings, buttonName)) {
            GameUIController.ShowSettings();
        }

#endif
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonMain, buttonName)) {
            GameUIController.ShowMain();
        }

        // PROFILE SYNC


        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonProfileGameversesSync, buttonName)) {
            GameState.SyncProfile();
        }

        // rating/community

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonAppRate, buttonName)) {
            Platforms.ShowReviewPage();
        }

        // ACTION URL
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonActionUrl, buttonName)) {

            if(data != null) {

                if(data.Has(BaseDataObjectKeys.url)) {
                    string url = data.Get<string>(BaseDataObjectKeys.url);
                    string title = data.Get<string>(BaseDataObjectKeys.title);

                    if(title.IsNullOrEmpty()) {
                        title = AppConfigs.appGameDisplayName;
                    }

                    if(!url.IsNullOrEmpty()) {
                        Platforms.ShowWebView(title, url);
                    }
                }
            }
        }

        // Game networks

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCenterAchievements, buttonName)) {
            GameNetworks.ShowAchievementsOrLogin(GameNetworkType.gameNetworkAppleGameCenter);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCenterLeaderboards, buttonName)) {
            GameNetworks.ShowLeaderboardsOrLogin(GameNetworkType.gameNetworkAppleGameCenter);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePlayServicesAchievements, buttonName)) {
            GameNetworks.ShowAchievementsOrLogin(GameNetworkType.gameNetworkGooglePlayServices);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePlayServicesLeaderboards, buttonName)) {
            GameNetworks.ShowLeaderboardsOrLogin(GameNetworkType.gameNetworkGooglePlayServices);
        }

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameNetworkAchievements, buttonName)) {
            GameNetworks.ShowAchievementsOrLogin();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameNetworkLeaderboards, buttonName)) {
            GameNetworks.ShowLeaderboardsOrLogin();
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
#if ENABLE_FEATURE_PRODUCT_CURRENCY
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductCurrency, buttonName)) {
            GameUIController.ShowProductCurrency();
        }

#endif

        // Game Modes

#if ENABLE_FEATURE_GAME_MODE_ARCADE
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeArcade, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameArcade);
            //GameUIController.ShowGameModeArcade();
            GameUIController.ShowGameWorlds();
        }
#endif

#if ENABLE_FEATURE_GAME_MODE_CHALLENGE
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeChallenge, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameChallenge);
            GameUIController.ShowGameModeChallenge();
        }

#endif


#if ENABLE_FEATURE_NETWORKING

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMultiplayerCoop, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMultiplayerCoop);
            GameUIController.ShowGameModeMultiplayerCoop();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMultiplayerMatchup, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMultiplayerMatchup);
            GameUIController.ShowGameModeMultiplayerMatchup();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMultiplayer, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMultiplayer);
            GameUIController.ShowGameModeMultiplayer();
        }

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeCoop, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameCoop);
            GameUIController.ShowGameModeCoop(); // non multiplayer coop with co bots
        }
#endif

#if ENABLE_FEATURE_WORLDS
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeMissions, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameMissions);
            GameUIController.ShowGameWorlds();
        }
#endif

#if ENABLE_FEATURE_TRAINING
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameModeTraining, buttonName)) {
            GameController.ChangeGameStates(AppContentStateMeta.appContentStateGameTraining);
            GameUIController.ShowGameModeTrainingMode();
        }
#endif


#if ENABLE_FEATURE_PRODUCTS

        // PRODUCTS

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsWeapon, buttonName)) {
            GameUIController.ShowProducts(GameProductType.weapon);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsCharacterSkin, buttonName)) {
            GameUIController.ShowProducts(GameProductType.characterSkin);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsCharacter, buttonName)) {
            GameUIController.ShowProducts(GameProductType.character);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsCurrency, buttonName)) {
            GameUIController.ShowProducts(GameProductType.currency);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsAccess, buttonName)) {
            GameUIController.ShowProducts(GameProductType.access);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsFeature, buttonName)) {
            GameUIController.ShowProducts(GameProductType.feature);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsPickup, buttonName)) {
            GameUIController.ShowProducts(GameProductType.pickup);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsPowerup, buttonName)) {
            GameUIController.ShowProducts(GameProductType.powerup);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProductsRPGUpgrade, buttonName)) {
            GameUIController.ShowProducts(GameProductType.rpgUpgrade);
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameProducts, buttonName)) {
            GameUIController.ShowProducts();
        }

#endif

        // ACTION ITEMS / USE

        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameActionItemBuyUse + "$") > -1) {

            string productCodeUse = "";
            string productTypeUse = "";
            string productCharacterUse = "";

            string[] commandActionParams = buttonName.Replace(BaseUIButtonNames.buttonGameActionItemBuyUse + "$", "").Split('$');

            if(commandActionParams.Length > 0)
                productTypeUse = commandActionParams[0];
            if(commandActionParams.Length > 1)
                productCodeUse = commandActionParams[1];
            if(commandActionParams.Length > 2)
                productCharacterUse = commandActionParams[2];

            if(!string.IsNullOrEmpty(productTypeUse)
                && !string.IsNullOrEmpty(productCodeUse)
                && !string.IsNullOrEmpty(productCharacterUse)) {

                GameStoreController.Purchase(productCodeUse, 1);

                GameUIPanelProducts.LoadData();
            }
        }

        else if(buttonName.IsEqualLowercase(BaseUIButtonNames.buttonGameActionItemBuyUse)) {

            if(data != null) {

                string productCodeUse = data.Get<string>(BaseDataObjectKeys.code);

                if(!string.IsNullOrEmpty(productCodeUse)) {

                    GameStoreController.Purchase(productCodeUse, 1);
                }
            }
        }

#if ENABLE_FEATURE_CHARACTER_CUSTOMIZE

        // CUSTOMIZE

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterColors, buttonName)) {
            GameUIController.ShowCustomizeCharacterColors();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterRPG, buttonName)) {
            GameUIController.ShowCustomizeCharacterRPG();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacter, buttonName)) {
            GameUIController.ShowCustomizeCharacter();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomize, buttonName)) {
            GameUIController.ShowCustomize();
        }
#endif

        // COMMUNITY

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityClose, buttonName)) {
            GameCommunity.HideGameCommunity();
        }

        // COMMUNITY - BROADCAST

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastRecordStart, buttonName)) {
            BroadcastNetworks.StartRecording();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastRecordStop, buttonName)) {
            BroadcastNetworks.StopRecording();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastRecordToggle, buttonName)) {
            BroadcastNetworks.ToggleRecording();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastOpen, buttonName)) {
            UIPanelCommunityBroadcast.ShowDialog();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastNetworkOpen, buttonName)) {
            BroadcastNetworks.Open();
        }

        // 

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastReplayShare, buttonName)) {
            BroadcastNetworks.OpenSharing();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastReplayWatch, buttonName)) {
            BroadcastNetworks.PlayLastRecording();
        }

        //else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastNetworkOpenSharing, buttonName)) {
        //    BroadcastNetworks.OpenSharing();
        //}

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastFacecamStart, buttonName)) {
            BroadcastNetworks.FacecamStart();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastFacecamStop, buttonName)) {
            BroadcastNetworks.FacecamStop();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityBroadcastFacecamToggle, buttonName)) {
            BroadcastNetworks.FacecamToggle();
        }

        // COMMUNITY - CAMERA

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraTakePhoto, buttonName)) {
            UIPanelCommunityCamera.TakePhoto();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraSaveFacebook, buttonName)) {
            GameCommunitySocialController.StartPhotoUploadToFacebook();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraSaveTwitter, buttonName)) {
            GameCommunitySocialController.StartPhotoUploadToTwitter();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityCameraSaveLibrary, buttonName)) {
            GameCommunitySocialController.SaveImageToLibraryDefault();
        }

        // COMMUNITY - RESULTS / SHARE


        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityShareResultFacebook, buttonName)) {
            //GameCommunitySocialController.PostGameResultsFacebook();
            UIPanelCommunityCamera.TakePhotoGameState();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCommunityShareResultTwitter, buttonName)) {
            //GameCommunitySocialController.PostGameResultsTwitter();
            UIPanelCommunityCamera.TakePhotoGameState();
        }

        // CUSTOMIZE 

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterZoomIn, buttonName)) {
            GameUIPanelHeader.CharacterLargeZoomIn();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterZoomOut, buttonName)) {
            GameUIPanelHeader.CharacterLargeZoomOut();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterFront, buttonName)) {
            GameUIPanelHeader.CharacterLargeShowFront();
        }
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameCustomizeCharacterBack, buttonName)) {
            GameUIPanelHeader.CharacterLargeShowBack();
        }


#if ENABLE_FEATURE_AR

        // AR
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonARSettings, buttonName)) {
            showARSettings();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonAR, buttonName)) {
            showAR();
        }

#endif

#if ENABLE_FEATURE_VR
        // VR

        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonVRSettings, buttonName)) {
            showVRSettings();
        }
        else if (UIUtil.IsButtonClicked(BaseUIButtonNames.buttonVR, buttonName)) {
            showVR();
        }
#endif

        // GAME INIT

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGameInitFinish, buttonName)) {

            GameCommunity.HideGameCommunity();

            // TODO MOVE
            UIPanelOverlayPrepare.HideAll();

            GameController.InitLevelFinish();
        }

        // GAME PRESET CHARACTER COLORS

        else if(UIUtil.IsButtonClickedLike(UIButtonNames.buttonGamePlayerPresets, buttonName)) {

            GameProfileCustomItem customItem = GameProfileCharacters.currentCustom;

            string[] arrButtonName = buttonName.Split('$');

            string presetTexture = "";
            string presetColor = "";

            string panelNext = "";

            string markerTexture = "preset-texture--";
            string markerColor = "preset-color--";
            string markerNext = "panel-next--";

            if(arrButtonName != null) {

                foreach(string s in arrButtonName) {
                    if(s.Contains(markerTexture)) {
                        presetTexture = s.Replace(markerTexture, "");
                    }
                    if(s.Contains(markerColor)) {
                        presetColor = s.Replace(markerColor, "");
                    }
                    if(s.Contains(markerNext)) {
                        panelNext = s.Replace(markerNext, "");
                    }
                }

                Debug.Log("panelNext:" + panelNext);
            }

            bool loadCharacter = false;

            if(!string.IsNullOrEmpty(presetTexture)) {
                // SET CUSTOM VALUES FOR THIS PLAYER

                customItem = GameCustomController.UpdateTexturePresetObject(
                    customItem, GameController.CurrentGamePlayerController.gameObject,
                    AppContentAssetTexturePresets.Instance.GetByCode(presetTexture));

                loadCharacter = true;
            }


            if(!string.IsNullOrEmpty(presetColor)) {
                customItem = GameCustomController.UpdateColorPresetObject(
                    customItem, GameController.CurrentGamePlayerController.gameObject,
                    AppColorPresets.Instance.GetByCode(presetColor));

                loadCharacter = true;
            }

            if(loadCharacter) {
                GameCustomController.SaveCustomItem(customItem);
                GameController.LoadCurrentProfileCharacter();
            }

#if ENABLE_FEATURE_GAME_MODE
            if(!string.IsNullOrEmpty(panelNext)) {
                if(panelNext == BaseUIPanel.panelGameMode) {
                    GameUIController.ShowGameMode();
                }
            }
#endif
        }
#if ENABLE_FEATURE_PRODUCT_CURRENCY

        // COIN / CURRENCY

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonProductCurrency, buttonName)) {

            GameCommunity.HideGameCommunity();

            showProductCurrency();
        }

#endif
        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonStoreRestorePurchases, buttonName)) {

            GameCommunity.HideGameCommunity();

            GameStoreController.PurchasesRestore();
        }
        
        // LAST 

        else if(UIUtil.IsButtonClicked(BaseUIButtonNames.buttonGamePlay, buttonName)
            || UIUtil.IsButtonClickedLike(BaseUIButtonNames.buttonGamePlay + "$", buttonName)
            || UIUtil.IsButtonClickedLike(BaseUIButtonNames.buttonGameModePlay, buttonName)) {

            object dataType = null;
            object dataCode = null;
            string dataAppContentState = null;

            if(data.ContainsKey(BaseDataObjectKeys.type)) {
                dataType = data.Get(BaseDataObjectKeys.type);
            }

            if(data.ContainsKey(BaseDataObjectKeys.code)) {
                dataCode = data.Get(BaseDataObjectKeys.code);
            }

            if(data.ContainsKey(BaseDataObjectKeys.app_content_state)) {

                dataAppContentState = data.Get<string>(BaseDataObjectKeys.app_content_state);

                // Check content states/modes from button

                if(dataAppContentState != null) {
                    // TODO check content state validity
                    GameController.ChangeGameStates(dataAppContentState);
                }
            }

            if(dataType != null) {

                // COLLECTION LOAD - MISSION

                if(dataType.ToString() == BaseDataObjectKeys.mission) {

                    string code = dataCode.ToString();

                    if(!string.IsNullOrEmpty(code)) {

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
            if(buttonName == BaseUIButtonNames.buttonBack) {

                GameCommunity.HideGameCommunity();

                NavigateBack(buttonName);
            }
        }

#if ENABLE_FEATURE_NETWORKING
        GameUIController.HandleNetworkedButtons(buttonName);
#endif
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

        if(lastTime + .1f < Time.time) {
            lastTime = Time.time;
            return true;
        }

        return true;
    }

    public virtual void handleNetworkedButtons(string buttonName) {
        // handle network state by buttons and areas
        if(AppConfigs.featureEnableNetworking) {
            if(UIUtil.IsButtonClickedLike("GameMode", buttonName)) {
                if(UIUtil.IsButtonClicked(UIButtonNames.buttonGameModeCoop, buttonName)) {
                    //Gameverses.GameNetworking.Connect();
                }
                else {
                    //Gameverses.GameNetworking.Disconnect();
                }
            }
        }
    }

    public virtual void handleHUDButtons(string buttonName) {
        if(AllowPress(lastPressAttack)
            && buttonName == BaseHUDButtonNames.buttonInputAttack) {
            GameController.GamePlayerAttack();
        }
        else if(AllowPress(lastPressAttackAlt)
            && buttonName == BaseHUDButtonNames.buttonInputAttackAlt) {
            GameController.GamePlayerAttackAlt();
        }
        else if(AllowPress(lastPressAttackRight)
            && buttonName == BaseHUDButtonNames.buttonInputAttackRight) {
            GameController.GamePlayerAttackRight();
        }
        else if(AllowPress(lastPressAttackLeft)
            && buttonName == BaseHUDButtonNames.buttonInputAttackLeft) {
            GameController.GamePlayerAttackLeft();
        }
        else if(AllowPress(lastPressDefend)
            && buttonName == BaseHUDButtonNames.buttonInputDefend) {
            GameController.GamePlayerDefend();
        }
        else if(AllowPress(lastPressDefendAlt)
            && buttonName == BaseHUDButtonNames.buttonInputDefendAlt) {
            GameController.GamePlayerDefendAlt();
        }
        else if(AllowPress(lastPressDefendRight)
            && buttonName == BaseHUDButtonNames.buttonInputDefendRight) {
            GameController.GamePlayerDefendRight();
        }
        else if(AllowPress(lastPressDefendLeft)
            && buttonName == BaseHUDButtonNames.buttonInputDefendLeft) {
            GameController.GamePlayerDefendLeft();
        }
        else if(AllowPress(lastPressSkill)
            && buttonName == BaseHUDButtonNames.buttonInputSkill) {
            GameController.GamePlayerSkill();
        }
        else if(AllowPress(lastPressMagic)
            && buttonName == BaseHUDButtonNames.buttonInputMagic) {
            GameController.GamePlayerMagic();
        }
        else if(AllowPress(lastPressUse)
            && buttonName == BaseHUDButtonNames.buttonInputUse) {
            GameController.GamePlayerUse();
        }
        else if(AllowPress(lastPressMount)
            && buttonName == BaseHUDButtonNames.buttonInputMount) {
            GameController.GamePlayerMount();
        }
        else if(buttonName == BaseHUDButtonNames.buttonInputJump) {
            GameController.GamePlayerJump();
        }
        else if(buttonName == BaseHUDButtonNames.buttonInputInventoryWeapon) {
            //GameController.GamePlayerJump();
        }
        else if(buttonName == BaseHUDButtonNames.buttonInputInventoryWeaponNext) {
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