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
    public static string buttonTrophy = "ButtonTrophy";
    public static string buttonTrophyStatistics = "ButtonTrophyStatistics";
    public static string buttonTrophyAchievements = "ButtonTrophyAchievements";
    public static string buttonGameCenterLeaderboards = "ButtonGameCenterLeaderboards";
    public static string buttonGameCenterAchievements = "ButtonGameCenterAchievements";
    public static string buttonSocial = "ButtonSocial";
    public static string buttonCredits = "ButtonCredits";
    public static string buttonWorlds = "ButtonWorlds";
    public static string buttonLevels = "ButtonLevels";
    public static string buttonLevel = "ButtonLevel";
    public static string buttonGamePause = "ButtonGamePause";
    public static string buttonGameResume = "ButtonGameResume";
    public static string buttonGameRestart = "ButtonGameRestart";
    public static string buttonGameQuit = "ButtonGameQuit";
    public static string buttonGameWorlds = "ButtonGameWorlds";
    public static string buttonGameContinue = "ButtonGameContinue";
    public static string buttonGameLevelItemObject = "ButtonGameLevelItemObject";
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
    public static string panelGameMode = "PanelGameMode";
    public static string panelGameModeArcade = "PanelGameModeArcade";
    public static string panelGameModeChallenge = "PanelGameModeChallenge";
    public static string panelGameModeTraining = "PanelGameModeTraining";
    public static string panelGameModeTrainingMode = "PanelGameModeTrainingMode";
    public static string panelGameModeTrainingModeChoice = "PanelGameModeTrainingModeChoice";
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
    public static string panelCustomize = "PanelCustomize";
    public static string panelCustomizeCharacterColors = "PanelCustomizeCharacterColors";
    public static string panelCustomizeCharacterRPG = "PanelCustomizeCharacterRPG";
    public static string panelCustomizeAudio = "PanelCustomizeAudio";
    public static string panelCustomSafety = "PanelCustomSafety";
    public static string panelCustomSmarts = "panelCustomSmarts";
}

public class BaseUIController : MonoBehaviour { 
 
    public static BaseUIController Instance;
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
                 
    public virtual void Awake() {
     
    }    
 
    /*
    void OnEnable() {
     Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
     //Messenger<string, string, bool>.AddListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
     //Messenger<string, string>.AddListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);
     
     //Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
     
     //Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
             
     Messenger<GameObject>.AddListener(
         GameDraggableEditorMessages.editorGrabbedObjectChanged, OnEditorGrabbedObjectChanged);
     
     
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
    
    void OnDisable() {
     Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
     //Messenger<string, string, bool>.RemoveListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
     //Messenger<string, string>.RemoveListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);
     
     //Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
     
     //Messenger<string, bool>.RemoveListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
     
     Messenger<GameObject>.RemoveListener(
         GameDraggableEditorMessages.editorGrabbedObjectChanged, OnEditorGrabbedObjectChanged);

     
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
    */
    
    public virtual void Start() {
        LoadData();
        //ShowContainerByName(BaseUIButtonNames.buttonContent);
        //Invoke("ShowMainMenuDelayed", 10);
        HideAllPanels();
        showMain();
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
    
    public virtual void OnListItemClickEventHandler(string listName, string listIndex, bool selected) {
        Debug.Log("OnListItemClickEventHandler: listName:" + listName + " listIndex:" + listIndex.ToString() + " selected:" + selected.ToString());
    }
    
    public virtual void OnListItemSelectEventHandler(string listName, string selectName) {
        Debug.Log("OnListItemSelectEventHandler: listName:" + listName + " selectName:" + selectName);
    }
    
    public virtual void OnSliderChangeEventHandler(string sliderName, float sliderValue) {
        Debug.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue);
    }
    
    public virtual void OnCheckboxChangeEventHandler(string checkboxName, bool selected) {
        Debug.Log("OnCheckboxChangeEventHandler: checkboxName:" + checkboxName + " selected:" + selected);
    }
    
    public virtual void OnApplicationQuit() {
        GameState.SaveProfile();
    }
    
    public virtual void ShowMainMenuDelayed() {
        if(!hasBeenClicked) {
            showMain();
        }
    }
    
    public virtual void Update() {
            
    }
 
    public virtual void FingerGestures_OnDragMove(DragGesture gesture) { //Vector2 fingerPos, Vector2 delta) {
        //Vector2 fingerPos = gesture.Position;
        //Vector2 delta = gesture.TotalMove;
     
        if(!IsInputAllowed()) {
            return;
        }

        if(currentDraggableGameObject != null) {
            //DragObject(currentDraggableGameObject, fingerPos, delta);
        }
    }

    public virtual void DragObject(GameObject go, Vector2 fingerPos, Vector2 delta) {
        if(go != null) {
         
            deferTap = true;
         
            if(go.rigidbody == null) {
                go.AddComponent<Rigidbody>();
                go.rigidbody.constraints =
                    RigidbodyConstraints.FreezePosition
                    | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationZ;
                go.rigidbody.useGravity = false;
                go.rigidbody.angularDrag = 2f;
            }
         
            if(Math.Abs(delta.x) > .8f) {
                go.rigidbody.angularVelocity = (new Vector3(0, -delta.x / 4, 0));                
            }
            else {
                go.rigidbody.angularVelocity = Vector3.zero;
            }

            GamePlayerProgress.Instance.ProcessProgressSpins();
        }
    }

    public virtual void FingerGestures_OnPinchMove(PinchGesture gesture) {
        //Vector2 fingerPos1 = gesture.Fingers[0].Position;
        //Vector2 fingerPos2 = gesture.
        float delta = gesture.Delta;
        
        if(!IsInputAllowed()) {
            return;
        }
        //ScaleCurrentObjects(delta);
    }

    public virtual void FingerGestures_OnRotationMove(TwistGesture gesture) {
        //Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
        float rotationAngleDelta = gesture.DeltaRotation;
        if(!IsInputAllowed()) {
            return;
        }
        // RotateCurrentObjects(Vector3.zero.WithY(rotationAngleDelta));
    }

    public virtual void FingerGestures_OnLongPress(LongPressGesture gesture) {
        Vector2 pos = gesture.Position;
        if(!IsInputAllowed()) {
            return;
        }
       
        if(currentDraggableGameObject != null) {
            LongPressObject(currentDraggableGameObject, pos);                
        }
    }

    public virtual void LongPressObject(GameObject go, Vector2 pos) {
        if(go != null) {
            go.rigidbody.angularVelocity = Vector3.zero;
            deferTap = true;

            //ResetObject(go);
        }
    }

    public virtual void FingerGestures_OnTap(TapGesture gesture) {//Vector2 fingerPos) {
        Vector2 fingerPos = gesture.Position;
        //LogUtil.Log("FingerGestures_OnTap", fingerPos);
        if(!IsInputAllowed()) {
            return;
        }
     
        bool allowTap = true;
        
        if(currentDraggableGameObject != null) {
            //TapObject(currentDraggableGameObject, fingerPos, allowTap);
        }
    }

    public virtual void TapObject(GameObject go, Vector2 fingerPos, bool allowTap) {
        if(go != null) {
            deferTap = !allowTap;

            //Debug.Log("Tap:" + fingerPos);
            //Debug.Log("Tap:Screen.Height:" + Screen.height);

            float heightToCheck = Screen.height - Screen.height * .85f;
            // Debug.Log("Tap:heightToCheck:" + heightToCheck);

            if(fingerPos.y < heightToCheck) {
                deferTap = true;
            }

            // Debug.Log("Tap:deferTap:" + deferTap);

            if(!deferTap) {

                //var fwd = transform.TransformDirection(Vector3.forward);
                //Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
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
        if(go != null) {
            go.rigidbody.angularVelocity = Vector3.zero;
            deferTap = true;

            //ResetObject(go);
        }
    }

    public virtual void FingerGestures_OnDoubleTap(TapGesture gesture) {
        if(!IsInputAllowed()) {
            return;
        }                
     
        if(gesture.Taps == 2) {
     
            if(currentDraggableGameObject != null) {
                DoubleTapObject(currentDraggableGameObject, gesture.Position);
            }
        }
     
        //var fwd = transform.TransformDirection(Vector3.forward);
        ////Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
        ////RaycastHit hit;
        ////if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
        ////    print("double tap hit an object:" + hit.transform.name);
        ////}

        //AppViewerAppController.Instance.ChangeActionNext();
    }

    public virtual void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, 
     FingerGestures.SwipeDirection direction, float velocity) {
        if(!IsInputAllowed()) {
            return;
        }

        if(direction == FingerGestures.SwipeDirection.All) {

            // if swiped any direction
        }

        if(direction == FingerGestures.SwipeDirection.Right
            || direction == FingerGestures.SwipeDirection.Down) {

            //AppViewerAppController.Instance.ChangeActionPrevious();
        }
        else if(direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {

            //AppViewerAppController.Instance.ChangeActionNext();
        }
    }

    public virtual void FingerGestures_OnSwipe(SwipeGesture gesture) {       
        //Vector2 startPos = gesture.StartPosition;
        FingerGestures.SwipeDirection direction = gesture.Direction;
        //float velocity = gesture.Velocity;
     
        if(!IsInputAllowed()) {
            return;
        }

        bool allowSwipe = true;//AppViewerAppController.Instance.AllowCurrentActionAdvanceSwipe();

        if(direction == FingerGestures.SwipeDirection.Right
            || direction == FingerGestures.SwipeDirection.Down) {
            //if (!AppViewerUIController.Instance.uiVisible) {
            if(allowSwipe) {
                //AppViewerAppController.Instance.ChangeActionPrevious();
            }
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
        }
        else if(direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {
            //if (!AppViewerUIController.Instance.uiVisible) {
            if(allowSwipe) {
                //AppViewerAppController.Instance.ChangeActionNext();
            }
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
        }
    }

    public virtual void HandleFingerGesturesOnLongPress(Vector2 fingerPos) {
        //LogUtil.Log("HandleFingerGesturesOnLongPress: " 
        //   + " fingerPos:" + fingerPos);   
     
        if(!IsInputAllowed()) {
            return;
        }
     
        // Create asset at touch point (long press) if in game and editing       
        LongPress(fingerPos);
    }

    public virtual void HandleFingerGesturesOnTap(Vector2 fingerPos) {
        //LogUtil.Log("HandleFingerGesturesOnTap: " 
        //   + " fingerPos:" + fingerPos);
             
        if(!IsInputAllowed()) {
            return;
        }
     
        // ...   
        Tap(fingerPos);
     
    }

    public virtual void HandleFingerGesturesOnDoubleTap(Vector2 fingerPos) {
        //LogUtil.Log("HandleFingerGesturesOnDoubleTap: " 
        //   + " fingerPos:" + fingerPos);
             
        if(!IsInputAllowed()) {
            return;
        }
     
        // ...   
        DoubleTap(fingerPos);
     
    }
 
    public virtual void HandleFingerGesturesOnDragMove(Vector2 fingerPos, Vector2 delta) {
        //LogUtil.Log("HandleFingerGesturesOnDragMove: " 
        //   + " fingerPos:" + fingerPos 
        //   + " delta:" + delta);
             
        if(!IsInputAllowed()) {
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
             
        if(!IsInputAllowed()) {
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
     
        if(!IsInputAllowed()) {
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
     
        if(!IsInputAllowed()) {
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
     
        if(!IsInputAllowed()) {
            return;
        }
     
        // ...
        TwoFingerSwipe(startPos, direction, velocity);
    }
     
    public virtual void LongPress(Vector2 fingerPos) {       
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
             
            ResetCurrentObject(fingerPos);   
        }
    }
 
    public virtual void RotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {     
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            RotateCurrentObject(rotationAngleDelta);
        }
    }
 
    public virtual void DragMove(Vector2 fingerPos, Vector2 delta) {     
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {         
            //SpinCurrentObject(fingerPos, delta);
         
            bool doScale = false;
            bool doRotation = false;
         
            if(Input.GetKey(KeyCode.S)) {
                doScale = true;
            }
         
            if(Input.GetKey(KeyCode.R)) {
                doRotation = true;
            }            
         
            if(doRotation) {
                RotateCurrentObject(delta.x);
            }
         
            if(doScale) {                
                ScaleCurrentObject(delta.y);
            }
         
        }
    }
 
    public virtual void PinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {     
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
            ScaleCurrentObject(delta);
        }
    }
 
    public virtual void Tap(Vector2 fingerPos) {     
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
         
        }
    }
 
    public virtual void DoubleTap(Vector2 fingerPos) {       
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
         
            GameDraggableEditor.EditModeCreateAsset(fingerPos);
         
            //var fwd = transform.TransformDirection(Vector3.forward);
            Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                print("double tap hit an object:" + hit.transform.name);
            }
        }
     
        //AppController.Instance.ChangeActionNext();
     
        if(Application.isEditor) {
            GameController.Instance.CycleCharacterTypesNext();
        }

    }
                
    public virtual void TwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {

        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
     
            if(direction == FingerGestures.SwipeDirection.All) {
                // if swiped any direction
            }
         
            if(direction == FingerGestures.SwipeDirection.Right
         || direction == FingerGestures.SwipeDirection.Down) {
                //AppController.Instance.ChangeActionPrevious();
            }
            else if(direction == FingerGestures.SwipeDirection.Left
         || direction == FingerGestures.SwipeDirection.Up) {
                //AppController.Instance.ChangeActionNext();
            }
        }
     
        if(direction == FingerGestures.SwipeDirection.Right
         || direction == FingerGestures.SwipeDirection.Down) {
            GameController.Instance.CycleCharacterTypesPrevious();
        }
        else if(direction == FingerGestures.SwipeDirection.Left
         || direction == FingerGestures.SwipeDirection.Up) {
            GameController.Instance.CycleCharacterTypesNext();
        }
    }
 
    public virtual void Swipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocitys) {      
        if(GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
         
        }
     
        //bool allowSwipe = true;
         
        if(direction == FingerGestures.SwipeDirection.Right
     || direction == FingerGestures.SwipeDirection.Down) {
            //if(!UIController.Instance.uiVisible && allowSwipe) {
            // AppController.Instance.ChangeActionPrevious();
            GamePlayerProgress.Instance.ProcessProgressSwipes();
            //}
         
         
        }
        else if(direction == FingerGestures.SwipeDirection.Left
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
     
        if(go != null) {
            GameObjectHelper.ScaleObject(go, delta);
        }
    }
 
    public virtual void RotateCurrentObject(float delta) {
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
     
        if(go != null) {
            GameObjectHelper.RotateObjectZ(go, delta);
        }
    }
        
    public virtual void SpinCurrentObject(Vector2 fingerPos, Vector2 delta) {
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
         
        if(go != null) {
            GameObjectHelper.SpinObject(go, fingerPos, delta);           
            GameObjectHelper.deferTap = true;            
            GamePlayerProgress.Instance.ProcessProgressSpins();
        }
    }
        
    public virtual void ResetCurrentObject(Vector2 pos) {        
        GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
     
        if(go != null) {                  
            if(go.rigidbody != null) {
                go.rigidbody.angularVelocity = Vector3.zero;
            }
            GameObjectHelper.deferTap = true;
         
            GameObjectHelper.ResetObject(go);
        }    
    }
        
    public virtual void FingerGestures_OnTap(Vector2 fingerPos) {    
        if(!IsInputAllowed()) {
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
             Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
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
                    
        Debug.Log("ToggleUI uiVisible: " + uiVisible);
                        
        if(uiVisible) {
            Debug.Log("call HideUI");
            hideUI(false);
        }
        else {
            Debug.Log("call ShowUI");
            showUI();
        }            
    }
 
    public virtual void NavigateBack(string buttonName) {    
 
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
     
        if(!gameLoopsStarted) {          
            GameAudio.StartGameLoops();
        }
     
        GameAudio.StopAmbience();
        GameAudio.StartGameLoopForLap(UnityEngine.Random.Range(1, 3));
        inUIAudioPlaying = false;
    }
 
    public virtual void HandleInUIAudio() {
     
        if(!inUIAudioPlaying) {
            GameAudio.StartGameLoopForLap(-1);
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
     
        Debug.Log("HideUI");  
     
        uiVisible = false;

        showGameCanvas();

        if(now) {
            HideAllPanelsNow();
        }
        else {
            HideAllPanels();
        }
     
        HandleInGameAudio();
            
    }
    
    //public static virtual void ShowUI() {
    //   if(isInst) {
    ////     Instance.showUI();
    //   }
    //}
 
    public virtual void showUI() {
        Debug.Log("ShowUI");
        uiVisible = true;        
        hideGameCanvas();        
        HandleInUIAudio();
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
     
        currentPanel = BaseUIPanel.panelMain;
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInScary();
     
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
        GameUIPanelMain.Instance.AnimateOut();
    }
 
    // ------------------------------------------------------------
    // GAME MODES
 
    //public static virtual void ShowGameMode() {
    //   if(isInst) {
    //       Instance.showGameMode();
    //   }
    //}
 
    public virtual void showGameMode() { 
     
        currentPanel = BaseUIPanel.panelGameMode;        
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("PLAY GAMEMODE");
     
        GameUIPanelGameMode.Instance.AnimateIn();        
    } 
 
    //public static virtual void HideGameMode() {
    //   if(isInst) {
    //       Instance.hideGameMode();
    //   }
    //}
    
    public virtual void hideGameMode() {
        GameUIPanelGameMode.Instance.AnimateOut();
    }

    // ------------------------------------------------------------
    // GAME MODE - TRAINING
 
    //public static virtual void ShowGameModeTraining() {
    //   if(isInst) {
    //       Instance.showGameModeTraining();
    //   }
    //}
 
    public virtual void showGameModeTraining() { 
     
        currentPanel = BaseUIPanel.panelGameModeTraining;
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("PLAY TRAINING");
     
        GameUIPanelGameModeTraining.Instance.AnimateIn();        
    } 
 
    //public static virtual void HideGameModeTraining() {
    //   if(isInst) {
    //       Instance.hideGameModeTraining();
    //   }
    //}

    public virtual void hideGameModeTraining() {
        GameUIPanelGameModeTraining.Instance.AnimateOut();
    }

    // ------------------------------------------------------------
    // GAME MODE - TRAINING MODE

    public virtual void showGameModeTrainingMode() {

        currentPanel = BaseUIPanel.panelGameModeTrainingMode;

        HideAllPanelsNow();

        GameUIPanelBackgrounds.Instance.AnimateInStarry();

        GameUIPanelHeader.Instance.AnimateInInternal();
        GameUIPanelHeader.ShowTitle("PLAY TRAINING MODE");

        GameUIPanelGameModeTrainingMode.Instance.AnimateIn();
    }

    public virtual void hideGameModeTrainingMode() {
        GameUIPanelGameModeTrainingMode.Instance.AnimateOut();
    }

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
 
    // ------------------------------------------------------------
    // GAME MODE - CHALLENGE
 
    //public static virtual void ShowGameModeChallenge() {
    //   if(isInst) {
    //       Instance.showGameModeChallenge();
    //   }
    //}
 
    public virtual void showGameModeChallenge() {    
     
        currentPanel = BaseUIPanel.panelGameModeChallenge;       
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("PLAY CHALLENGE");
     
        GameUIPanelGameModeChallenge.Instance.AnimateIn();       
    } 
 
    //public static virtual void HideGameModeChallenge() {
    //   if(isInst) {
    //       Instance.hideGameModeChallenge();
    //   }
    //}
    
    public virtual void hideGameModeChallenge() {
        GameUIPanelGameModeChallenge.Instance.AnimateOut();
    }
 
    // ------------------------------------------------------------
    // GAME MODE - ARCADE
 
    //public static virtual void ShowGameModeArcade() {
    //   if(isInst) {
    //       Instance.showGameModeArcade();
    //   }
    //}
 
    public virtual void showGameModeArcade() {   
     
        currentPanel = BaseUIPanel.panelGameModeArcade;      
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("PLAY ARCADE");
     
        GameUIPanelGameModeArcade.Instance.AnimateIn();      
    } 
 
    //public static virtual void HideGameModeArcade() {
    //   if(isInst) {
    //       Instance.hideGameModeArcade();
    //   }
    //}
    
    public virtual void hideGameModeArcade() {
        GameUIPanelGameModeArcade.Instance.AnimateOut();
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
     
        currentPanel = BaseUIPanel.panelSettings;
     
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("SETTINGS");
     
        GameUIPanelSettings.Instance.AnimateIn();        
    } 
 
    //public static virtual void HideSettings() {
    //   if(isInst) {
    //       Instance.hideSettings();
    //   }
    //}
    
    public virtual void hideSettings() {
        GameUIPanelSettings.Instance.AnimateOut();
    }
 
    // ------------------------------------------------------------
    // SETTINGS - AUDIO
 
    //public static virtual void ShowSettingsAudio() {
    //   if(isInst) {
    //       Instance.showSettingsAudio();
    //   }
    //}
 
    public virtual void showSettingsAudio() {    
     
        currentPanel = BaseUIPanel.panelSettingsAudio;       
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("SETTINGS: AUDIO");
     
        UIPanelSettingsAudio.Instance.AnimateIn();       
    } 
 
    //public static virtual void HideSettingsAudio() {
    //   if(isInst) {
    //       Instance.hideSettingsAudio();
    //   }
    //}
    
    public virtual void hideSettingsAudio() {
        UIPanelSettingsAudio.Instance.AnimateOut();
    }
     
    // ------------------------------------------------------------
    // SETTINGS - CONTROLS
 
    //public static virtual void ShowSettingsControls() {
    //   if(isInst) {
    //       Instance.showSettingsControls();
    //   }
    //}
 
    public virtual void showSettingsControls() { 
     
        currentPanel = BaseUIPanel.panelSettingsControls;        
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("SETTINGS: CONTROLS");
     
        GameUIPanelSettingsControls.Instance.AnimateIn();        
    } 
 
    //public static virtual void HideSettingsControls() {
    //   if(isInst) {
    //       Instance.hideSettingsControls();
    //   }
    //}
    
    public virtual void hideSettingsControls() {
        GameUIPanelSettingsControls.Instance.AnimateOut();
    }
 
    // ------------------------------------------------------------
    // SETTINGS - PROFILES
 
    //public static virtual void ShowSettingsProfile() {
    //   if(isInst) {
    //       Instance.showSettingsProfile();
    //   }
    //}
 
    public virtual void showSettingsProfile() {  
     
        currentPanel = BaseUIPanel.panelSettingsProfile;     
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("SETTINGS: PROFILES");
     
        GameUIPanelSettingsProfile.Instance.AnimateIn();     
    } 
 
    //public static virtual void HideSettingsProfile() {
    //   if(isInst) {
    //       Instance.hideSettingsProfile();
    //   }
    //}
    
    public virtual void hideSettingsProfile() {
        GameUIPanelSettingsProfile.Instance.AnimateOut();
    }
     
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
     
        currentPanel = BaseUIPanel.panelResults;     
     
        HideAllPanelsNow();
             
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("RESULTS");
     
        GameUIPanelResults.Instance.AnimateIn(); 
     
        GameUIPanelFooter.Instance.ShowStoreButtonObject();
     
        StartCoroutine(HideOverlay());
     
    } 
 
 
    // ------------------------------------------------------------
    // EQUIPMENT - MAIN
 
    //public static void ShowEquipment() {
    //   if(isInst) {
    //       Instance.showEquipment();
    //   }
    //}
 
    public virtual void showEquipment() {    
     
        currentPanel = BaseUIPanel.panelEquipment;       
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("EQUIPMENT, PROGRESS + CUSTOMIZATION");
     
        GameUIPanelEquipment.Instance.AnimateIn();       
    } 
 
    //public static void HideEquipment() {
    //   if(isInst) {
    //       Instance.hideEquipment();
    //   }
    //}
    
    public virtual void hideEquipment() {
        GameUIPanelEquipment.Instance.AnimateOut();
    }
 
 
    // ------------------------------------------------------------
    // EQUIPMENT - STATISTICS
 
    //public static void ShowStatistics() {
    //   if(isInst) {
    //       Instance.showStatistics();
    //   }
    //}
 
    public virtual void showStatistics() {   
     
        currentPanel = BaseUIPanel.panelStatistics;      
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("STATISTICS");
     
        GameUIPanelStatistics.Instance.AnimateIn();      
    } 
 
    //public static void HideStatistics() {
    //   if(isInst) {
    //       Instance.hideStatistics();
    //   }
    //}
    
    public virtual void hideStatistics() {
        GameUIPanelStatistics.Instance.AnimateOut();
    }
 
     
    // ------------------------------------------------------------
    // EQUIPMENT - ACHIEVEMENTS
 
    //public static void ShowAchievements() {
    //   if(isInst) {
    //       Instance.showAchievements();
    //   }
    //}
 
    public virtual void showAchievements() { 
     
        currentPanel = BaseUIPanel.panelAchievements;        
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("ACHIEVEMENTS");
     
        GameUIPanelAchievements.Instance.AnimateIn();        
    } 
 
    //public static void HideAchievements() {
    //   if(isInst) {
    //       Instance.hideAchievements();
    //   }
    //}
    
    public virtual void hideAchievements() {
        GameUIPanelAchievements.Instance.AnimateOut();
    }
     
    // ------------------------------------------------------------
    // EQUIPMENT - PRODUCTS
 
    //public static void ShowProducts() {
    //   if(isInst) {
    //       Instance.showProducts();
    //   }
    //}
 
    public virtual void showProducts() { 
     
        currentPanel = BaseUIPanel.panelProducts;        
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("POWERUPS");
     
        GameUIPanelProducts.Instance.AnimateIn();        
    } 
 
    //public static void HideProducts() {
    //   if(isInst) {
    //       Instance.hideProducts();
    //   }
    //}
    
    public virtual void hideProducts() {
        GameUIPanelProducts.Instance.AnimateOut();
    }    
         
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE
 
    //public static void ShowCustomize() {
    //   if(isInst) {
    //       Instance.showCustomize();
    //   }
    //}
 
    public virtual void showCustomize() {    
     
        currentPanel = BaseUIPanel.panelCustomize;       
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("CUSTOMIZE");
     
        GameUIPanelCustomize.Instance.AnimateIn();       
    } 
 
    //public static void HideCustomize() {
    //   if(isInst) {
    //       Instance.hideCustomize();
    //   }
    //}
 
    public virtual void hideCustomize() {
        GameUIPanelCustomize.Instance.AnimateOut();
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
    // EQUIPMENT - CUSTOMIZE CHARACTER COLORS    
 
    //public static void ShowCustomizeCharacterColors() {
    //   if(isInst) {
    //       Instance.showCustomizeCharacterColors();
    //   }
    //}
 
    public virtual void showCustomizeCharacterColors() { 
     
        currentPanel = BaseUIPanel.panelCustomizeCharacterColors;        
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("CUSTOMIZE: PLAYER COLORS");
     
        GameUIPanelCustomizeCharacterColors.Instance.AnimateIn();        
    } 
 
    //public static void HideCustomizeCharacterColors() {
    //   if(isInst) {
    //       Instance.hideCustomizeCharacterColors();
    //   }
    //}
    
    public virtual void hideCustomizeCharacterColors() {
        GameUIPanelCustomizeCharacterColors.Instance.AnimateOut();
    }
 
    // ------------------------------------------------------------
    // EQUIPMENT - CUSTOMIZE CHARACTER RPG   
 
    //public static void ShowCustomizeCharacterRPG() {
    //   if(isInst) {
    //       Instance.showCustomizeCharacterRPG();
    //   }
    //}
 
    public virtual void showCustomizeCharacterRPG() {    
     
        currentPanel = BaseUIPanel.panelCustomizeCharacterRPG;       
     
        HideAllPanelsNow();
     
        GameUIPanelBackgrounds.Instance.AnimateInStarry();
     
        GameUIPanelHeader.Instance.AnimateInInternal();  
        GameUIPanelHeader.ShowTitle("CUSTOMIZE: PLAYER SKILLS");
     
        GameUIPanelCustomizeCharacterRPG.Instance.AnimateIn();       
    } 
 
    //public static void HideCustomizeCharacterRPG() {
    //   if(isInst) {
    //       Instance.hideCustomizeCharacterRPG();
    //   }
    //}
    
    public virtual void hideCustomizeCharacterRPG() {
        GameUIPanelCustomizeCharacterRPG.Instance.AnimateOut();
    }
 
    IEnumerator HideOverlay() {
        yield return new WaitForSeconds(0.4f);
     
        GameUIPanelOverlays.Instance.HideOverlayWhiteStatic();
    }
 
 
    //public static virtual void HideResults() {
    //   if(isInst) {
    //       Instance.hideResults();
    //   }
    //}
    
    public virtual void hideResults() {
        GameUIPanelResults.Instance.AnimateOut();
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
        GameUIPanelHeader.Instance.AnimateOut();
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
        Debug.Log("HideHUD");
     
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
        Debug.Log("ShowHUD");
     
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
        hideUIPanelDialogPause();
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
 
    public virtual void showUIPanelDialogPause() {
        //HideAllAlertLayers();
        showUIPanelAlertBackground();
        if(gamePauseDialogObject != null) {
            TweenPosition.Begin(gamePauseDialogObject, .3f, Vector3.zero.WithY(0));  
        }
    }
 
    //public static virtual void HideUIPanelDialogPause() {
    //   if(isInst) {
    //       Instance.hideUIPanelDialogPause();
    //   }       
    //}
 
    public virtual void hideUIPanelDialogPause() {
        hideUIPanelAlertBackground();
        if(gamePauseDialogObject != null) {
            TweenPosition.Begin(gamePauseDialogObject, .3f, Vector3.zero.WithY(5000));
        }
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
        //TweenPosition.Begin(gameNavigationObject, .3f, Vector3.zero.WithX(-970));  
    }
            
    public virtual void OnButtonClickEventHandler(string buttonName) {
        Debug.Log("OnButtonClickEventHandler: " + buttonName);
     
        hasBeenClicked = true;
     
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
             
             Debug.Log("ButtonPlayLevel: levelId:" + levelId);
             
             GameAppController.Instance.LoadLevel(levelId);
         }
         
     }
     else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameLevelItemObject) > -1) {
         
         string[] arrLevelId = buttonName.Split('$');

         if(arrLevelId.Length > 0) {
             string assetCodeCreating =  arrLevelId[1];
             GameDraggableEditor.assetCodeCreating = assetCodeCreating;
             Debug.Log("GameDraggableEditor.assetCodeCreating:" + GameDraggableEditor.assetCodeCreating);
             
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
             
        // GAME
         
        if(buttonName.IndexOf(BaseUIButtonNames.buttonGameQuit) > -1) {
            GameQuit();
        }
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGamePause) > -1) {
            GamePause();
        }
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameResume) > -1) {
            GameResume();
        }
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameRestart) > -1) {
            GameRestart();
        }
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameWorlds) > -1) { 
            GameWorlds();
        }
        else if(buttonName.IndexOf(BaseUIButtonNames.buttonGameContinue) > -1) {
            GameContinue();
        }
        else {
            if(buttonName == BaseUIButtonNames.buttonBack) {
                NavigateBack(buttonName);
            }
        }
     
        HandleHUDButtons(buttonName);
    }
 
    public virtual void HandleHUDButtons(string buttonName) {
        if(buttonName == "ButtonInputAttack") {
            GameController.GamePlayerAttack();
        }
        else if(buttonName == "ButtonInputAttackAlt") {
            //GameController.GamePlayerAttackAlt();
        }
        else if(buttonName == "ButtonInputAttackRight") {
            //GameController.GamePlayerAttackRight();
        }
        else if(buttonName == "ButtonInputSkill") {
            //GameController.GamePlayerSkill();
        }
        else if(buttonName == "ButtonInputUse") {
            GameController.GamePlayerUse();
        }
        else if(buttonName == "ButtonINputJump") {
            GameController.GamePlayerJump();
        }            
    }
 
    public virtual void GameContinue() {     
        GameController.Instance.QuitGame();
    }
 
    public virtual void GameWorlds() {       
        GameController.Instance.QuitGame();
    }
 
    public virtual void GameRestart() {
        GameController.Instance.RestartGame();
    }
 
    public virtual void GameResume() {
        GameController.Instance.ResumeGame();
    }
 
    public virtual void GamePause() {
        GameController.Instance.TogglePauseGame();
    }
 
    public virtual void GameQuit() {
        GameController.Instance.QuitGame();
        GameUIController.ShowMain();
    }
}
