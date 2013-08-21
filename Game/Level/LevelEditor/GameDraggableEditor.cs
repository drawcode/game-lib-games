using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

using Engine.Events;

public enum GameDraggableCanvasType {
	CANVAS_2D,
	CANVAS_3D
}

public enum GameDraggableCanvasMode {
	EDITING,
	CREATING
}

public enum GameDraggableEditEnum {
    StateNotEditing = 0,
    StateEditing = 1
}

public class GameDraggableEditorMessages {
	public static string editorGrabbedObjectChanged = "editor-grabbed-object-changed";
	public static string editorAssetSelected = "editor-asset-selected";
		
	public static string GameControllerStarted = "game-controller-started";
	public static string GameControllerReset = "game-controller-reset";
	
	public static string EditPlay = "edit-play";
	public static string EditItems = "edit-items";
	public static string EditMeta = "edit-meta";
	public static string EditReload = "edit-reload";
	public static string EditSave = "edit-save";
	public static string EditState = "edit-state";
}

public class GameDraggableEditorButtons {

	public static string buttonEdit = "ButtonGameEdit";
	public static string buttonEditItems = "ButtonGameEditItems";
	public static string buttonEditMeta = "ButtonGameEditMeta";
	public static string buttonEditPlay = "ButtonGameEditPlay";
	public static string buttonEditRestart = "ButtonGameEditRestart";
	public static string buttonEditReload = "ButtonGameEditReload";
	public static string buttonEditSave = "ButtonGameEditSave";
	public static string buttonGameLevelItemObject = "ButtonGameLevelItemObject";
	//ButtonGameLevelItemObject$" + asset.code
	
	public static string buttonGameEditMetaSave = "ButtonGameEditMetaSave";
	public static string buttonGameEditMetaClose = "ButtonGameEditMetaClose";
	public static string buttonGameEditItemsSave = "ButtonGameEditItemsSave";
	public static string buttonGameEditItemsClose = "ButtonGameEditItemsClose";
	
	public static string buttonGameEditAssetTools = "ButtonGameEditAssetTools";
	public static string buttonGameEditAssetDelete = "ButtonGameEditAssetDelete";
	public static string buttonGameEditAssetDeselect = "ButtonGameEditAssetDeselect";
	public static string buttonGameEditAssetSave = "ButtonGameEditAssetSave";
	
	public static string checkboxEditAssetDestructable = "CheckboxEditAssetDestructable";
	public static string checkboxEditAssetKinematic = "CheckboxEditAssetKinematic";
	public static string inputSprite = "InputSprite";
	public static string inputSpriteEffect = "InputSpriteEffect";
}

public class GameDraggableEditor : MonoBehaviour {
	
	public static GameDraggableEditor Instance;
	
	public Transform grabbed;
	public Transform lastGrabbed;
	
	public float grabDistance = 1000.0f;
	public int grabLayerMask;
	public Vector3 grabOffset; //delta between transform transform position and hit point
	public bool  useToggleDrag = false; // Didn't know which style you prefer. 
	
	public string prefabRootPath = "";
	
	public Vector3 currentEditorPosition;	
		
	public static bool editingEnabled = false;
	public string dragTag = "drag";
	public GameDraggableCanvasType gameDraggableCanvasType = GameDraggableCanvasType.CANVAS_2D;
	public GameDraggableCanvasMode gameDraggableCanvasMode = GameDraggableCanvasMode.EDITING;
	
	public static GameDraggableEditEnum appEditState = GameDraggableEditEnum.StateNotEditing;
	
	// For tooltip on item selected.
	public string assetCodeCreating = "";	
	public GameObject assetObjectCreating;
		
	public GameObject gameEditToolsObject;
	public GameObject gameEditAssetObject;
	public GameObject gameEditAssetButtonObject;
	public GameObject gameEditButtonObject;
	public GameObject gameEditDialogMetaObject;
	public GameObject gameEditDialogItemsObject;
	
	public UnityEngine.Object prefabDraggableContainer;
	
	public static GameObject levelItemsContainerObject;
	
	public static bool deferTap = false;
	public static bool dialogActive = false;
	
	public UILabel labelButtonGameEdit;
			    
	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
	}	
	
	void Start() {
		
		prefabRootPath = ContentsConfig.contentRootFolder + "/"
			+ ContentsConfig.contentAppFolder + "/version/shared/prefabs/"; 
		
		ShowUIPanelEditButton();
	}

	public void Update () {
		if(ShouldUpdate()) {
			if (useToggleDrag) {
		        UpdateToggleDrag();
		    } 
			else {
		        UpdateHoldDrag();
		    }
		}
		
		// if editing keep current position
		
	    //RaycastHit hit;
	    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	    //if (Physics.Raycast(ray, out hit)) {  
		//	Debug.Log("hit.point:" + hit.point);
			currentEditorPosition = Input.mousePosition;
		//}
	}
	
	public static bool isInst {
		get {
			return Instance != null ? true : false;
		}
	}
	
	// ----------------------------------------------------------------------
	
	public static bool isEditing {
		get {
			if(isInst) {
				if(appEditState == GameDraggableEditEnum.StateEditing) {
					return true;
				}
			}
			return false;
		}
	}
	
	public static bool allowEditing {
		get {
			if(isEditing && editingEnabled) {
				return true;
			}
			return false;
		}
	}
			
	public bool ShouldUpdate() {
		if(editingEnabled) {
			return true;
		}
		grabbed = null;
		return false;
	}
	
	void OnEnable() {
		Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		//Messenger<string, string, bool>.AddListener(ListEvents.EVENT_ITEM_SELECT, OnListItemClickEventHandler);
		//Messenger<string, string>.AddListener(ListEvents.EVENT_ITEM_SELECT_CLICK, OnListItemSelectEventHandler);
		
		//Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);
		
		//Messenger<string, bool>.AddListener(CheckboxEvents.EVENT_ITEM_CHANGE, OnCheckboxChangeEventHandler);
		
		Messenger<GameDraggableEditEnum>.AddListener(GameDraggableEditorMessages.EditState, OnEditStateHandler);
		
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
		
		Messenger<GameDraggableEditEnum>.RemoveListener(GameDraggableEditorMessages.EditState, OnEditStateHandler);
		
		
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
	
	void OnEditStateHandler(GameDraggableEditEnum state) {
	
		if(state == GameDraggableEditEnum.StateEditing) {
		
		}
		else if(state == GameDraggableEditEnum.StateNotEditing) {
			
		}
	
	}
	
	// EVENTS
	
	void OnButtonClickEventHandler(string buttonName) {
		
		// EDITING
		
		if(buttonName == GameDraggableEditorButtons.buttonGameEditAssetTools) {
			ShowUIPanelEditAssetButton();
			ShowUIPanelEditAsset();	
		}		
		
		else if(buttonName == GameDraggableEditorButtons.buttonGameEditMetaSave) {
			HideUIPanelDialogMeta();
		}
		else if(buttonName == GameDraggableEditorButtons.buttonGameEditMetaClose) {
			HideUIPanelDialogMeta();
		}
		
		else if(buttonName == GameDraggableEditorButtons.buttonGameEditItemsSave) {
			HideUIPanelDialogItems();
		}
		else if(buttonName == GameDraggableEditorButtons.buttonGameEditItemsClose) {
			HideUIPanelDialogItems();
			ShowUIPanelEditNow();
		}
		
		// edit
		
		else if(buttonName == GameDraggableEditorButtons.buttonEditPlay) {
			EditPlay();
		}
		else if(buttonName == GameDraggableEditorButtons.buttonEditItems) {
			EditItems();			
		}
		else if(buttonName == GameDraggableEditorButtons.buttonEditMeta) {
			EditMeta();
		}
		else if(buttonName == GameDraggableEditorButtons.buttonEditReload) {
			EditReload();			
		}
		else if(buttonName == GameDraggableEditorButtons.buttonEditSave) {
			EditSave();			
		}		
		else if(buttonName == GameDraggableEditorButtons.buttonEdit) {
			EditEnable();
		}
		
	}	
	
	void OnEditorGrabbedObjectChanged(GameObject grabbedObject) {
		HideUIPanelEditAsset();
		ShowUIPanelEditAssetButton();
		
		LogUtil.Log("GameDraggableEditor:OnEditorGrabbedObjectChanged:grabbedObject:", grabbedObject.name);
	}
	
	
    private void FingerGestures_OnDragMove(DragGesture gesture) { //Vector2 fingerPos, Vector2 delta) {
		//Vector2 fingerPos = gesture.Position;
		//Vector2 delta = gesture.TotalMove;
		
        if (!IsInputAllowed()) {
            return;
        }

		//if(GetCurrentDraggableObject() != null) {
        //	DragObject(GetCurrentDraggableObject(), gesture.Position, gesture.DeltaMove);
		//}
		
		HandleFingerGesturesOnDragMove(gesture.Position, gesture.DeltaMove);
    }

    public void DragObject(GameObject go, Vector2 fingerPos, Vector2 delta) {
        if (go != null) {
			
            deferTap = true;
			
            if (go.rigidbody == null) {
                go.AddComponent<Rigidbody>();
                go.rigidbody.constraints =
                    RigidbodyConstraints.FreezePosition
                    | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationZ;
                go.rigidbody.useGravity = false;
                go.rigidbody.angularDrag = 2f;
            }
			
			if(Math.Abs(delta.x) > .8f) {
            	go.rigidbody.angularVelocity = (new Vector3(0, -delta.x/4, 0));				
			}
			else {
				go.rigidbody.angularVelocity = Vector3.zero;
			}
        }
    }
	
	

    private void FingerGestures_OnPinchMove(PinchGesture gesture) {
		//Vector2 fingerPos1 = gesture.Fingers[0].Position;
		//Vector2 fingerPos2 = gesture.
		float delta = gesture.Delta;
        
		if (!IsInputAllowed()) {
            return;
        }
        //ScaleCurrentObjects(delta);
		
		Vector2 fingerPos1 = Vector2.zero;
		Vector2 fingerPos2 = Vector2.zero;
		
		if(gesture.Fingers.Count > 0) {
			fingerPos1 = gesture.Fingers[0].Position;
		}
		
		if(gesture.Fingers.Count > 1) {
			fingerPos2 = gesture.Fingers[1].Position;
		}
		
		HandleFingerGesturesOnPinchMove(fingerPos1, fingerPos2, delta);
    }

    private void FingerGestures_OnRotationMove(TwistGesture gesture) {
		//Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
		float rotationAngleDelta = gesture.DeltaRotation;
        if (!IsInputAllowed()) {
            return;
        }
       // RotateCurrentObjects(Vector3.zero.WithY(rotationAngleDelta));
		
		Vector2 fingerPos1 = Vector2.zero;
		Vector2 fingerPos2 = Vector2.zero;
		
		if(gesture.Fingers.Count > 0) {
			fingerPos1 = gesture.Fingers[0].Position;
		}
		
		if(gesture.Fingers.Count > 1) {
			fingerPos2 = gesture.Fingers[1].Position;
		}
		
		HandleFingerGesturesOnRotationMove(fingerPos1, fingerPos2, rotationAngleDelta); 
    }

    private void FingerGestures_OnLongPress(LongPressGesture gesture) {
		Vector2 pos = gesture.Position;
        if (!IsInputAllowed()) {
            return;
        }
       
		//if(GetCurrentDraggableObject() != null) {
        //	LongPressObject(GetCurrentDraggableObject(), pos);                
        //}
		
		HandleFingerGesturesOnLongPress(pos);
    }

    public void LongPressObject(GameObject go, Vector2 pos) {
        if (go != null) {
            go.rigidbody.angularVelocity = Vector3.zero;
            deferTap = true;

            //ResetObject(go);
        }
    }

    private void FingerGestures_OnTap(TapGesture gesture) {//Vector2 fingerPos) {
		Vector2 fingerPos = gesture.Position;
		//LogUtil.Log("FingerGestures_OnTap", fingerPos);
        if (!isEditing) {
            return;
        }
		
		bool allowTap = true;
        
		//if(GetCurrentDraggableObject() != null) {
		//	TapObject(GetCurrentDraggableObject(), fingerPos, allowTap);
        //}
		
		HandleFingerGesturesOnTap(fingerPos);
    }

    public void TapObject(GameObject go, Vector2 fingerPos, bool allowTap) {
        if (go != null) {
            deferTap = !allowTap;

            //Debug.Log("Tap:" + fingerPos);
            //Debug.Log("Tap:Screen.Height:" + Screen.height);

            float heightToCheck = Screen.height - Screen.height * .85f;
           // Debug.Log("Tap:heightToCheck:" + heightToCheck);

            if (fingerPos.y < heightToCheck) {
                deferTap = true;
            }

           // Debug.Log("Tap:deferTap:" + deferTap);

            if (!deferTap) {

                //var fwd = transform.TransformDirection(Vector3.forward);
                //Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    //print("hit an object:" + hit.transform.name);

                    //if(hit.transform.name == "UILoaderContainer") {
                    //	GameObject loaderCube = hit.transform.gameObject;
                    //	if(loaderCube != null) {
                    //UILoaderContainer loaderContainer = loaderCube.GetComponent<UILoaderContainer>();
                    //if(loaderContainer != null) {
                    //	if(loaderContainer.placeholderObject != null) {
                    //		loaderContainer.placeholderObject.SetPlaceholderObject();
                    //	}
                    //}
                    //	}
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
	
	public void DoubleTapObject(GameObject go, Vector2 pos) {
        if (go != null) {
            go.rigidbody.angularVelocity = Vector3.zero;
            deferTap = true;

            //ResetObject(go);
        }
    }

    private void FingerGestures_OnDoubleTap(TapGesture gesture) {
        if (!IsInputAllowed()) {
            return;
        }		        
		
		if(gesture.Taps > 1) {
		
            if (GetCurrentDraggableObject() != null) {
                DoubleTapObject(GetCurrentDraggableObject(), gesture.Position);
            }
			
			HandleFingerGesturesOnDoubleTap(gesture.Position);
		}
		
		
        //var fwd = transform.TransformDirection(Vector3.forward);
        ////Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
        ////RaycastHit hit;
        ////if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
        ////    print("double tap hit an object:" + hit.transform.name);
        ////}

        //AppViewerAppController.Instance.ChangeActionNext();
    }
	/*
    private void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, 
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
		
		//HandleFingerGesturesOnTwoFingerSwipe(gesture
		
		//	HandleFingerGesturesOnDoubleTap(gesture.Position);
    }
    */

    private void FingerGestures_OnSwipe(SwipeGesture gesture) { 		
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
				if(allowSwipe) {
                	//AppViewerAppController.Instance.ChangeActionPrevious();
				}
            //}
        }
        else if (direction == FingerGestures.SwipeDirection.Left
            || direction == FingerGestures.SwipeDirection.Up) {
            //if (!AppViewerUIController.Instance.uiVisible) {
				if(allowSwipe) {
                	//AppViewerAppController.Instance.ChangeActionNext();
				}
            //}
        }
		
		HandleFingerGesturesOnSwipe(gesture.Position, direction, gesture.Velocity);
    }
		
	
	public void ScaleCurrentObject(float delta) {
		GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
		
		if(go != null) {
			GameObjectHelper.ScaleObject(go, delta);
		}
	}
	
	public void RotateCurrentObject(float delta) {
		GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
		
		if(go != null) {
			GameObjectHelper.RotateObjectZ(go, delta);
		}
	}
        
	public void SpinCurrentObject(Vector2 fingerPos, Vector2 delta) {
		GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
			
		if(go != null) {
		    GameObjectHelper.SpinObject(go, fingerPos, delta);		    
		    GameObjectHelper.deferTap = true;		    
		    GamePlayerProgress.Instance.ProcessProgressSpins();
		}
	}	
        
	public void ResetCurrentObject(Vector2 pos) {		
		GameObject go = GameDraggableEditor.GetCurrentSpriteObject();
		
		if(go != null) {                  
			if(go.rigidbody != null) {
				go.rigidbody.angularVelocity = Vector3.zero;
			}
			GameObjectHelper.deferTap = true;
			
			GameObjectHelper.ResetObject(go);
		}	
	}	
	
	void HandleFingerGesturesOnLongPress(Vector2 fingerPos) {
		//LogUtil.Log("HandleFingerGesturesOnLongPress: " 
		//	+ " fingerPos:" + fingerPos);	
		
		if(!IsInputAllowed()){
			return;
		}
		
		// Create asset at touch point (long press) if in game and editing		
		LongPress(fingerPos);
	}

	void HandleFingerGesturesOnTap(Vector2 fingerPos) {
		//LogUtil.Log("HandleFingerGesturesOnTap: " 
		//	+ " fingerPos:" + fingerPos);
				
		if(!IsInputAllowed()){
			return;
		}
		
		// ...	
		Tap(fingerPos);
		
	}

	void HandleFingerGesturesOnDoubleTap(Vector2 fingerPos) {
		//LogUtil.Log("HandleFingerGesturesOnDoubleTap: " 
		//	+ " fingerPos:" + fingerPos);
				
		if(!IsInputAllowed()){
			return;
		}
		
		// ...	
		DoubleTap(fingerPos);
		
	}
	
	void HandleFingerGesturesOnDragMove(Vector2 fingerPos, Vector2 delta) {
		//LogUtil.Log("HandleFingerGesturesOnDragMove: " 
		//	+ " fingerPos:" + fingerPos 
		//	+ " delta:" + delta);
				
		if(!IsInputAllowed()){
			return;
		}
		
		// scale current selected object	
		DragMove(fingerPos, delta);
		
	}

	void HandleFingerGesturesOnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {
		//LogUtil.Log("HandleFingerGesturesOnPinchMove: " 
		//	+ " fingerPos1:" + fingerPos1 
		//	+ " fingerPos2:" + fingerPos2
		//	+ " delta:" + delta);
				
		if(!IsInputAllowed()){
			return;
		}
		
		// scale current selected object	
		PinchMove(fingerPos1, fingerPos2, delta);
		
	}

	void HandleFingerGesturesOnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
		//LogUtil.Log("HandleFingerGesturesOnRotationMove: " 
		//	+ " fingerPos1:" + fingerPos1 
		//	+ " fingerPos2:" + fingerPos2
		//	+ " rotationAngleDelta:" + rotationAngleDelta);	
		
		if(!IsInputAllowed()){
			return;
		}
		
		// rotate current object if editing
		RotationMove(fingerPos1, fingerPos2, rotationAngleDelta);
		
	}
	
    void HandleFingerGesturesOnSwipe( Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity ) {
		//LogUtil.Log("HandleFingerGesturesOnSwipe: " 
		//	+ " startPos:" + startPos 
		///	+ " direction:" + direction
		//	+ " velocity:" + velocity);	
		
		if(!IsInputAllowed()){
			return;
		}
		
		// ...
		Swipe (startPos, direction, velocity);
	}
	
	
	public void HandleFingerGesturesOnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {		
		//LogUtil.Log("HandleFingerGesturesOnTwoFingerSwipe: " 
		//	+ " startPos:" + startPos 
		//	+ " direction:" + direction
		//	+ " velocity:" + velocity);	
		
		if(!IsInputAllowed()){
			return;
		}
		
		// ...
		TwoFingerSwipe(startPos, direction, velocity);
	}
		
	public void LongPress(Vector2 fingerPos) {		
		if(GameDraggableEditor.isEditing) {				
			ResetCurrentObject(fingerPos);	
		}
	}
	
	public void RotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {		
		if(GameDraggableEditor.isEditing) {
			RotateCurrentObject(rotationAngleDelta);
		}
	}
	
	public void DragMove(Vector2 fingerPos, Vector2 delta) {		
		if(GameDraggableEditor.isEditing) {			
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
	
	public void PinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {		
		if(GameDraggableEditor.isEditing) {
			ScaleCurrentObject(delta);
		}
	}
	
	public void Tap(Vector2 fingerPos) {		
		if(GameDraggableEditor.isEditing) {
			
		}
	}
	
	public void DoubleTap(Vector2 fingerPos) {		
		if(GameDraggableEditor.isEditing) {
			
			GameDraggableEditor.EditModeCreateAsset(fingerPos);
			
			//var fwd = transform.TransformDirection(Vector3.forward);
			Ray ray = Camera.mainCamera.ScreenPointToRay(Vector3.zero);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				print ("double tap hit an object:" + hit.transform.name);
			}
			
			
		}
		
		//AppController.Instance.ChangeActionNext();
		
		if(Application.isEditor) {
			GameShooter2DController.Instance.CycleCharacterTypesNext();
		}

	}
                
	public void TwoFingerSwipe( Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity ) {

		if(GameDraggableEditor.isEditing) {
		
			if(direction == FingerGestures.SwipeDirection.All) {
				// if swiped any direction
			}
			
			if(direction == FingerGestures.SwipeDirection.Right
			|| direction == FingerGestures.SwipeDirection.Down) {
				//AppController.Instance.ChangeActionPrevious();
			}
			else if(direction == FingerGestures.SwipeDirection.Left
			|| direction == FingerGestures.SwipeDirection.Up){
				//AppController.Instance.ChangeActionNext();
			}
		}
		
		if(direction == FingerGestures.SwipeDirection.Right
			|| direction == FingerGestures.SwipeDirection.Down) {
			GameShooter2DController.Instance.CycleCharacterTypesPrevious();
		}
		else if(direction == FingerGestures.SwipeDirection.Left
			|| direction == FingerGestures.SwipeDirection.Up){
			GameShooter2DController.Instance.CycleCharacterTypesNext();
		}

	}
	
	public void Swipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocitys) {		
		if(GameDraggableEditor.isEditing) {
			
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
		|| direction == FingerGestures.SwipeDirection.Up){
			//if(!UIController.Instance.uiVisible && allowSwipe) {
			    //AppController.Instance.ChangeActionNext();
			    GamePlayerProgress.Instance.ProcessProgressSwipes();
			//}
		}
	}
	public bool IsInputAllowed() {
		if(isEditing) {
			return true;
		}
		return false;
	}
	
	public void EditPlay() {
		HideAllUIEditPanels();
		
		// Save to current level data...			
		SaveCurrentLevel();
	}
	
	public void EditMeta() {
		ShowUIPanelDialogMeta();
	}
	
	public void EditItems() {
		HideAllEditDialogs();
		////HideAllAlertLayers();
		ShowUIPanelDialogItems();
	}
	
	public void EditEnable() {
		//HideHUD();
		//HideUIPanelEditButton();
		//ShowUIPanelEdit();
		
		GameDraggableEditor.ToggleStateEditing();
		
		if(GameDraggableEditor.isEditing) {
			if(labelButtonGameEdit != null) {
				labelButtonGameEdit.text = "PLAY";
			}
						
			//HideHUD();
			//HideUIPanelEditButton();
			ShowUIPanelEdit();
			GameDraggableEditor.editingEnabled = true;
			
			LogUtil.Log("EditEnable:editingEnabled:", editingEnabled);
		}
		else if(!GameDraggableEditor.isEditing) {
			if(labelButtonGameEdit != null) {
				labelButtonGameEdit.text = "EDIT";
			}
			
			GameDraggableEditor.editingEnabled = false;
			HideUIPanelEdit();
			HideAllEditDialogs();
			
			LogUtil.Log("EditEnable:editingEnabled:", editingEnabled);
		}
	}
	
	public void EditSave() {
		
		// Save to current level data...			
		SaveCurrentLevel();
	}
	
	public void EditReload() {		
		ReloadLevelItems();
	}
	
	
	public void ResetAssetPanelRemoveDeselect() {
		if(isInst) {
			Instance.resetAssetPanelRemoveDeselect();
		}
	}
	
	public void resetAssetPanelRemoveDeselect() {
		GameDraggableEditor.ResetCurrentGrabbedObject();
		HideUIPanelEditAssetButton();
		HideUIPanelEditAsset();
	}
	
	// ----------------------------------------------------------------------
	
	public static void UpdateToggleDrag() {
		if(isInst) {
			Instance.updateToggleDrag();
		}
	}
	
	// Toggles drag with mouse click
	public void updateToggleDrag () {
	    if (Input.GetMouseButtonDown(0)) { 
	        Grab();
	    } 
		else {
	        if (grabbed) {
	            Drag();
	        }
	    }
	}
	
	// ----------------------------------------------------------------------
	
	public static void UpdateHoldDrag() {
		if(isInst) {
			Instance.updateHoldDrag();
		}
	}
		
	// Drags when user holds down button
	public void updateHoldDrag() {
	    if (Input.GetMouseButton(0)) {
	        if (grabbed) {
	            Drag();
				LogUtil.Log("GameDraggableEditor:Drag:", grabbed.name);
	        } 
			else { 
	            Grab();
				LogUtil.Log("GameDraggableEditor:Grab:", true);
	        }
	    } 
		else {
	        if(grabbed) {
	           //restore the original layermask
	           //grabbed.gameObject.layer = LayerMask.NameToLayer("drag");
	        }
	        grabbed = null;
	    }
	}
	
	// ----------------------------------------------------------------------
	
	public void Grab() {
		if(isInst) {
			Instance.grab();
		}
	}
	
	public void grab () {
	    if (grabbed) { 
	       grabbed = null;
	    } 
		else {
	        RaycastHit hit;
	        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        if (Physics.Raycast(ray, out hit)) {          
	            grabbed = hit.transform;				
				if(grabbed.tag == dragTag) {
					/*
		            if(grabbed.parent){
		                grabbed = grabbed.parent.transform;
		            }
					 
					if(grabbed.parent.transform.parent){
						if(grabbed.parent.transform.parent.GetComponent<GameDraggableLevelItem>() != null) {
		                	grabbed = grabbed.parent.transform.parent.transform;
						}
		            } 
		            */
					GameObject gameDraggableLevelItem = grabbed.gameObject.FindTypeAboveObject<GameDraggableLevelItem>();
					if(gameDraggableLevelItem != null) {
						grabbed = gameDraggableLevelItem.transform;
						GameDraggableLevelItem levelItem = gameDraggableLevelItem.GetComponent<GameDraggableLevelItem>();
						if(levelItem != null && levelItem.gameLevelItemAsset != null) {
							assetCodeCreating = levelItem.gameLevelItemAsset.asset_code;
						}
					}
					if(lastGrabbed != grabbed) {
						lastGrabbed = grabbed;
						////currentDraggableGameObject = grabbed.gameObject;
						Messenger<GameObject>.Broadcast(
							GameDraggableEditorMessages.editorGrabbedObjectChanged, lastGrabbed.gameObject);
					}
		            //set the object to ignore raycasts
		            //grabLayerMask = grabbed.gameObject.layer;
		            //grabbed.gameObject.layer = LayerMask.NameToLayer("nodrag");
		            //now immediately do another raycast to calculate the offset
		            if (Physics.Raycast(ray, out hit)) {
		                grabOffset = grabbed.position - hit.point;
		            } 
					else {
		                //important - clear the garb if there is nothing
		                //behind the object to drag against
		                grabbed = null;
		            }
				}
				else {
					grabbed = null;
				}
	        }
	    }
	}
	
	// ----------------------------------------------------------------------
	
	public static void Drag() {  
		if(isInst) {
			Instance.drag();
		}
	}
		
	public void drag() {   
		
		// TODO add key to change Y on 3d and z on 2d
		
	    RaycastHit hit;
	    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
	    if (Physics.Raycast(ray, out hit)) {  
			
			if(grabbed != null) {
				
				if(gameDraggableCanvasType == GameDraggableCanvasType.CANVAS_2D) {
					float grabbedInitialZ = grabbed.position.z;
	        		grabbed.position = hit.point + grabOffset;
					grabbed.position = grabbed.position.WithZ(grabbedInitialZ);
				}
				else if(gameDraggableCanvasType == GameDraggableCanvasType.CANVAS_3D) {
					float grabbedInitialY = grabbed.position.y;
	        		grabbed.position = hit.point + grabOffset;
					grabbed.position = grabbed.position.WithY(grabbedInitialY);
				}
				///Debug.Log("pos:" + grabbed.position);
			}
	    }	
	}
	
	// ----------------------------------------------------------------------
	
	public static Transform GetCurrentGrabbedObject() {
		if(isInst) {
			return Instance.getCurrentGrabbedObject();
		}
		return null;
	}
	
	public Transform getCurrentGrabbedObject() {
		return lastGrabbed;
	}
	
	public static void ResetCurrentGrabbedObject() {
		if(isInst) {
			Instance.resetCurrentGrabbedObject();
		}
	}
	
	public void resetCurrentGrabbedObject() {
		lastGrabbed = null;
	}
	
	// ----------------------------------------------------------------------	
	
	public static void ToggleStateEditing() {
		if(isInst) {
			Instance.toggleStateEditing();
		}
	}
	
	public void toggleStateEditing() {
		if(appEditState == GameDraggableEditEnum.StateNotEditing) {
			changeStateEditing(GameDraggableEditEnum.StateEditing);
			
			editingEnabled = true;
		}
		else if(appEditState == GameDraggableEditEnum.StateEditing) {
			changeStateEditing(GameDraggableEditEnum.StateNotEditing);
				
			editingEnabled = false;
		}
	}
	
	// ----------------------------------------------------------------------
		
	public static void ChangeStateEditing(GameDraggableEditEnum appEditStateTo) {
		if(isInst) {
			Instance.changeStateEditing(appEditStateTo);
		}
	}
	
	public void changeStateEditing(GameDraggableEditEnum appEditStateTo) {
		if(appEditState != appEditStateTo) {
			appEditState = appEditStateTo;
			broadcastStateEditing();
		}
	}
	
	// ----------------------------------------------------------------------
	
	public static void BroadcastStateEditing() {
		if(isInst) {
			Instance.broadcastStateEditing();
		}
	}
	
	public void broadcastStateEditing() {		
		if(appEditState == GameDraggableEditEnum.StateNotEditing) {
			Messenger<GameDraggableEditEnum>.Broadcast(GameDraggableEditorMessages.EditState, appEditState);
		}
		else if(appEditState == GameDraggableEditEnum.StateEditing) {			
			Messenger<GameDraggableEditEnum>.Broadcast(GameDraggableEditorMessages.EditState, appEditState);
		}
	}
	
	// ----------------------------------------------------------------------
	
	public static void ToggleEditingMode() {		
		if(isInst) {
			Instance.toggleEditingMode();
		}
	}
	
	public void toggleEditingMode() {		
		EditEnable();		
	}
	
	public static void EditModeCreateAsset(Vector2 fingerPos) {
		if(isInst) {
			Instance.editModeCreateAsset(fingerPos);
		}
	}
	
	public void editModeCreateAsset(Vector2 fingerPos) {
		Debug.Log("EditModeCreateAsset");
		if(GameDraggableEditor.isEditing) {
			// add game draggable edit selected asset code if on
			
			if(!string.IsNullOrEmpty(GameDraggableEditor.Instance.assetCodeCreating)) {
				GameLevelItemAsset itemAsset = new GameLevelItemAsset();
				itemAsset.asset_code = GameDraggableEditor.Instance.assetCodeCreating;
				GameLevelItemAssetStep gameLevelItemStep = new GameLevelItemAssetStep();
				gameLevelItemStep.position.x = (Input.mousePosition.x - Screen.width)/1000;
				gameLevelItemStep.position.y = (Input.mousePosition.y - Screen.height)/1000;
				gameLevelItemStep.position.z = 0;
				itemAsset.steps.Add(gameLevelItemStep);
				GameDraggableEditor.LoadLevelItem(itemAsset);
			}
			
		}
	}
	
	// ----------------------------------------------------------------------
	
	public static string GetResourcePath(string spriteCode) {	
		if(isInst) {
			return Instance.getResourcePath(spriteCode);
		}
		return spriteCode;
	}	
	
	public string getResourcePath(string spriteCode) {	
		//string resourcePath = Contents.appShipCachePathShared.Replace(Application.dataPath, "");		
		//resourcePath = Path.Combine(resourcePath, spriteCode);
		string resourcePath = GameDraggableEditor.Instance.prefabRootPath
			+ "levelassets/" + spriteCode;
		return resourcePath;
	}
	
	// ----------------------------------------------------------------------
	
	public static GameObject LoadSprite(GameObject parent, string spriteCode, Vector3 scale) {
		if(isInst) {
			Instance.loadSprite(parent, spriteCode, scale);
		}
		return null;
	}
		
	public GameObject loadSprite(GameObject parent, string spriteCode, Vector3 scale) {
		
		GameObject go = null;
		
		// load in sprite from resources for now
		string resourcePath = GetResourcePath(spriteCode);
		
		LogUtil.Log("LoadSprite:resourcePath:", resourcePath);
		
		UnityEngine.Object prefabSprite = Resources.Load(resourcePath);
		if(prefabSprite != null) {
			go = Instantiate(prefabSprite) as GameObject;
			if(go != null) {
				go.transform.parent = parent.transform;
				if(parent != null) {
					//go.transform.position = parent.transform.position;
					go.transform.position = Vector3.zero;
				}
				else {
					go.transform.position = Vector3.zero;
				}
				go.transform.localScale = scale;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				
				go.AddComponent<GameLevelSprite>();
			}
		}
		return go;
	}
	
	// ----------------------------------------------------------------------
	
	public static GameObject LoadSpriteUI(GameObject parent, string spriteCode, Vector3 scale) {
		if(isInst) {
			Instance.loadSpriteUI(parent, spriteCode, scale);
		}
		return null;
	}
	
	public GameObject loadSpriteUI(GameObject parent, string spriteCode, Vector3 scale) {
		
		GameObject go = null;
		
		// load in sprite from resources for now
		string resourcePath = GetResourcePath(spriteCode);
		
		LogUtil.Log("LoadSpriteUI:resourcePath:", resourcePath);
		
		UnityEngine.Object prefabSprite = Resources.Load(resourcePath);
		if(prefabSprite != null) {
			go = Instantiate(prefabSprite) as GameObject;
			if(go != null) {
				go.transform.parent = parent.transform;
				if(parent != null) {
					go.transform.position = parent.transform.position;
				}
				else {
					go.transform.position = Vector3.zero;
				}
				go.transform.localScale = scale;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
			}
		}
		return go;
	}
	
	// ----------------------------------------------------------------------
	
	public static GameObject LoadSpriteEffect(GameObject parent, string spriteCode, Vector3 scale) {
		if(isInst) {
			Instance.loadSpriteEffect(parent, spriteCode, scale);
		}
		return null;
	}
	
	public GameObject loadSpriteEffect(GameObject parent, string spriteCode, Vector3 scale) {
		
		GameObject go = null;
		
		// load in sprite from resources for now
		string resourcePath = GetResourcePath(spriteCode);
		LogUtil.Log("LoadSpriteEffect:resourcePath:", resourcePath);
		
		UnityEngine.Object prefabSprite = Resources.Load(resourcePath);
		if(prefabSprite != null) {
			go = Instantiate(prefabSprite) as GameObject;
			if(go != null) {
				go.transform.parent = parent.transform;
				if(parent != null) {
					go.transform.position = parent.transform.position;
				}
				else {
					go.transform.position = Vector3.zero;
				}
				go.transform.localScale = scale;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				
				go.AddComponent<GameLevelSpriteEffect>();
			}
		}
		return go;
	}
	
	// ----------------------------------------------------------------------
	
	public static void LoadDraggableContainerObject() {	
		if(isInst) {
			Instance.loadDraggableContainerObject();
		}
	}
	
	public void loadDraggableContainerObject() {		
		string path = prefabRootPath + "ui/GameDraggableLevelItem";
		LoadDraggableContainerObjectResources(path);
	}
	
	public static void LoadDraggableContainerObjectResources(string path) {
		if(isInst) {
			Instance.loadDraggableContainerObjectResources(path);	
		}
	}
	
	public void loadDraggableContainerObjectResources(string path) {
		if(prefabDraggableContainer == null) {
			// Load from resources
			prefabDraggableContainer = Resources.Load(path);
		}
	}
	
	// ----------------------------------------------------------------------
	
	public static void ClearLevelItems(GameObject levelContainer) {
		if(isInst) {
			Instance.clearLevelItems(levelContainer);
		}
	}
	
	public void clearLevelItems(GameObject levelContainer) {		
		if(levelContainer != null) {
			levelContainer.DestroyChildren();
		}
	}
	
	public static void LoadLevelItems() {
		if(isInst) {
			Instance.loadLevelItems();
		}
	}
	
	public void loadLevelItems() {
		GameLevelItem gameLevelItem = GameLevelItems.Current;
		List<GameLevelItemAsset> gameLevelItemAssets = gameLevelItem.level_items;
		
		if(gameLevelItemAssets != null) {
			foreach(GameLevelItemAsset item in gameLevelItemAssets) {
				LoadLevelItem(item);
			}
		}
	}
	
	public static void ReloadLevelItems() {
		if(isInst) {
			Instance.reloadLevelItems();
		}
	}
	
	public void reloadLevelItems() {
		clearLevelItems(levelItemsContainerObject);
		loadLevelItems();
	}
	
	public static void LoadLevelItem(GameLevelItemAsset gameLevelItemAsset) {
		if(isInst) {
			Instance.loadLevelItem(gameLevelItemAsset);
		}
	}
	
	public void loadLevelItem(GameLevelItemAsset gameLevelItemAsset) {
		LoadLevelItem(prefabDraggableContainer, gameLevelItemAsset);
	}
	
	
	public static void LoadLevelItem(UnityEngine.Object prefabGameLevelItemContainer, GameLevelItemAsset gameLevelItemAsset) {
		if(isInst) {
			Instance.loadLevelItem(prefabGameLevelItemContainer, gameLevelItemAsset);
		}
	}
	
	public void loadLevelItem(UnityEngine.Object prefabGameLevelItemContainer, GameLevelItemAsset gameLevelItemAsset) {
		if(gameLevelItemAsset != null) {
			if(prefabGameLevelItemContainer != null) {
				GameObject goLevelItem = Instantiate(prefabGameLevelItemContainer) as GameObject;
				if(goLevelItem != null) {
					
					GameDraggableLevelItem dragLevelItem = goLevelItem.GetComponent<GameDraggableLevelItem>();
					
					if(dragLevelItem != null) {
						dragLevelItem.gameLevelItemAsset = gameLevelItemAsset;
						//dragLevelItem.gameLevelItemObject.transform.localScale = 
							// dragLevelItem.gameLevelItemObject.transform.localScale * .1f;
						dragLevelItem.LoadData();
					
						goLevelItem.transform.parent = GameShooter2DController.Instance.levelItemsContainerObject.transform;
						goLevelItem.transform.localScale = Vector3.one;
						goLevelItem.transform.position = Vector3.zero;
						goLevelItem.transform.localPosition = Vector3.zero;
						goLevelItem.transform.rotation = Quaternion.identity;
						goLevelItem.transform.localRotation = Quaternion.identity;
						
						foreach(GameLevelItemAssetStep step in gameLevelItemAsset.steps) {
							if(dragLevelItem.gameLevelItemObject != null) {
								
								goLevelItem.transform.position = 
									step.position.GetVector3();
								
								dragLevelItem.gameLevelItemObject.transform.rotation = 
									Quaternion.Euler(step.rotation.GetVector3());
								
								dragLevelItem.gameLevelItemObject.transform.localScale = 
									step.scale.GetVector3();// * .1f;
								
								goLevelItem.transform.position = 
									goLevelItem.transform.position.WithY(goLevelItem.transform.position.y);
							}
						}
					}
				}
			}
		}
	}		
	
	// ----------------------------------------------------------------------
		
	public static void SaveCurrentLevel() {
		SaveCurrentLevel(levelItemsContainerObject);
	}
	
	public static void SaveCurrentLevel(GameObject levelContainer) {
		if(isInst) {
			Instance.saveCurrentLevel(levelContainer);
		}
	}
	
	public void saveCurrentLevel(GameObject levelContainer) {
		
		// Get positions of all the current objects
		levelItemsContainerObject = levelContainer;
		
		foreach(Transform t in levelContainer.transform) {
			
			GameDraggableLevelItem gameDraggableLevelItem = t.gameObject.GetComponent<GameDraggableLevelItem>();
			
			GameLevelItem currentGameLevelItem = GameLevelItems.Current;
			
			if(gameDraggableLevelItem != null) {
				// Get the current GameLevelItem and update and readd to list
				GameLevelItemAsset gameLevelItemAsset = gameDraggableLevelItem.gameLevelItemAsset;
				foreach(GameLevelItemAssetStep step in gameLevelItemAsset.steps) {
					step.position.FromVector3(gameDraggableLevelItem.transform.position);
					step.rotation.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.rotation.eulerAngles);
					step.scale.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.localScale);
					break;
				}
				
				// Save the asset and step back to gamelevel items
				
				bool found = false;
				
				foreach(GameLevelItemAsset itemAsset in currentGameLevelItem.level_items) {
					if(itemAsset.uuid == gameLevelItemAsset.uuid) {
						// update
						found = true;
						
						foreach(GameLevelItemAssetStep step in itemAsset.steps) {
						step.position.FromVector3(gameDraggableLevelItem.transform.position);
						step.rotation.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.rotation.eulerAngles);
						step.scale.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.localScale);
							break;
						}
					}
				}
				
				if(!found) {
					// add a new level asset item
					
					GameLevelItemAsset newAsset = new GameLevelItemAsset();
					newAsset.asset_code = gameLevelItemAsset.asset_code;
					newAsset.physics_type = gameLevelItemAsset.physics_type;
					foreach(GameLevelItemAssetStep step in gameLevelItemAsset.steps) {
						step.position.FromVector3(gameDraggableLevelItem.transform.position);
						step.rotation.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.rotation.eulerAngles);
						step.scale.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.localScale);
						newAsset.steps.Add(step);
					}
					
					currentGameLevelItem.level_items.Add(newAsset);
				}
			}
		}
		
		GameLevelItems.Instance.Save();
	}
	
	public static GameLevelItemAssetStep UpdateGameLevelItemStep(
		GameDraggableLevelItem gameDraggableLevelItem, GameLevelItemAssetStep step) {
		if(isInst) {
			return Instance.updateGameLevelItemStep(gameDraggableLevelItem, step);
		}
		return null;
	}
	
	public GameLevelItemAssetStep updateGameLevelItemStep(
		GameDraggableLevelItem gameDraggableLevelItem, GameLevelItemAssetStep step) {
		step.position.FromVector3(gameDraggableLevelItem.transform.position);
		step.rotation.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.rotation.eulerAngles);
		step.scale.FromVector3(gameDraggableLevelItem.gameLevelItemObject.transform.localScale);
		return step;
	}
	
	
	// ----------------------------------------------------------------------
	
	public static bool IsLevelDestructableItemsComplete(GameObject go) {
		if(isInst) {
			Instance.isLevelDestructableItemsComplete(go);
		}
		return false;
	}
	public bool isLevelDestructableItemsComplete(GameObject go) {
		
		foreach(GameDraggableLevelItem item in
			go.GetComponentsInChildren<GameDraggableLevelItem>()) {
		    GameLevelItemAsset gameLevelItemAsset = item.gameLevelItemAsset;	
			if(gameLevelItemAsset.destructable) {
				return false;
			}
		}
		
		return true;
	}
	
	// ----------------------------------------------------------------------
	
	public static GameObject GetCurrentDraggableObject() {
		if(isInst) {
			if(Instance.grabbed != null) {
				return Instance.grabbed.gameObject;
			}
		}
		return null;
	}
	
	public static GameDraggableLevelItem GetCurrentDraggableLevelItem() {
		if(isInst) {
			Instance.getCurrentDraggableLevelItem();
		}
		return null;
	}
	
	public GameDraggableLevelItem getCurrentDraggableLevelItem() {
		Transform grabbedObject = GameDraggableEditor.GetCurrentGrabbedObject();
		if(grabbedObject != null) {
			return grabbedObject.gameObject.GetComponent<GameDraggableLevelItem>();
		}
		return null;
	}
	
	public static GameLevelItemAsset GetCurrentLevelItemAsset() {
		if(isInst) {
			Instance.getCurrentLevelItemAsset();
		}	
		return null;
	}
	
	public GameLevelItemAsset getCurrentLevelItemAsset() {
		
		GameDraggableLevelItem levelItem = GetCurrentDraggableLevelItem();
		if(levelItem != null) {
			return levelItem.gameLevelItemAsset;
		}		
		return null;	
	}
	
	public static GameObject GetCurrentSpriteObject() {
		if(isInst) {
			Instance.getCurrentSpriteObject();
		}
		return null;
	}
		
	public GameObject getCurrentSpriteObject() {
		GameDraggableLevelItem obj = GetCurrentDraggableLevelItem();
		if(obj != null) {
			return obj.gameLevelItemObject;
		}
		return null;
	}
	
	
	
	// ----------------------------------------------------------------------
	// UI
	
	
	// EDITING
	
	
	public static void HideAllEditDialogs() {
		if(isInst) {
			Instance.hideAllEditDialogs();
		}
	}
	
	public void hideAllEditDialogs() {
		HideUIPanelDialogMeta();
		HideUIPanelDialogItems();
		HideUIPanelEditAsset();
		HideUIPanelEditAssetButton();
		HideUIPanelEdit();
	}
	
	public static void ShowUIPanelEditNow() {
		if(isInst) {
			Instance.showUIPanelEditNow();
		}
	}
	
	public void showUIPanelEditNow() {
		if(gameEditToolsObject != null) {
			TweenPosition.Begin(gameEditToolsObject, 0f, Vector3.zero.WithY(0));
		}
	}
	
	public static void ShowUIPanelEdit() {
		if(isInst) {
			Instance.showUIPanelEdit();
		}
	}
	
	public void showUIPanelEdit() {
		if(gameEditToolsObject != null) {
			TweenPosition.Begin(gameEditToolsObject, .3f, Vector3.zero.WithY(0));
		}
		
		//HideHUD();
	}
	
	public static void HideUIPanelEdit() {
		if(isInst) {
			Instance.hideUIPanelEdit();
		}
	}
	
	public void hideUIPanelEdit() {
		//HideAllEditDialogs();
		if(gameEditToolsObject != null) {
			TweenPosition.Begin(gameEditToolsObject, .3f, Vector3.zero.WithY(650));
		}
		
		//ShowHUD();
	}	
	
	public static void ShowUIPanelEditAsset() {
		if(isInst) {
			Instance.showUIPanelEditAsset();
		}
	}
	
	public void showUIPanelEditAsset() {		
		UIPanelEditAsset.Instance.LoadData();
		
		if(gameEditAssetObject != null) {
			TweenPosition.Begin(gameEditAssetObject, .3f, Vector3.zero.WithY(0));
		}
		HideUIPanelEditAssetButton();
	}	
	
	
	public static void HideAllUIEditPanels() {		
		if(isInst) {
			Instance.hideAllUIEditPanels();
		}
	}
	
	public void hideAllUIEditPanels() {		
		HideUIPanelEditAsset();
		HideUIPanelEditAssetButton();
		//HideUIPanelEditButton();
	}
	
	public static void HideUIPanelEditAsset() {
		if(isInst) {
			Instance.hideUIPanelEditAsset();
		}
	}
	
	public void hideUIPanelEditAsset() {
		if(gameEditAssetObject != null) {
			TweenPosition.Begin(gameEditAssetObject, .3f, Vector3.zero.WithY(-650));
		}
		
		//ShowHUD();
	}
	
	public static void ShowUIPanelEditAssetButton() {
		if(isInst) {
			Instance.showUIPanelEditAssetButton();
		}
	}
	
	public void showUIPanelEditAssetButton() {
		//HideAllEditDialogs();
		if(gameEditAssetButtonObject != null) {
			TweenPosition.Begin(gameEditAssetButtonObject, .3f, Vector3.zero.WithY(0));
		}
		
		//HideHUD();
	}
	
	
	public static void HideUIPanelEditAssetButton() {
		if(isInst) {
			Instance.hideUIPanelEditAssetButton();
		}
	}
	
	public void hideUIPanelEditAssetButton() {
		if(gameEditAssetButtonObject != null) {
			TweenPosition.Begin(gameEditAssetButtonObject, .3f, Vector3.zero.WithY(-650));
		}
		
		//ShowHUD();
	}
	
	public static void ShowUIPanelEditButton() {
		if(isInst) {
			Instance.showUIPanelEditButton();
		}
	}
	
	public void showUIPanelEditButton() {
		if(gameEditButtonObject != null) {
			TweenPosition.Begin(gameEditButtonObject, .3f, Vector3.zero.WithY(0));
		}
		
	}
	
	
	public static void HideUIPanelEditButton() {
		if(isInst) {
			Instance.hideUIPanelEditButton();
		}
	}
	
	public void hideUIPanelEditButton() {
		if(gameEditButtonObject != null) {
			TweenPosition.Begin(gameEditButtonObject, .3f, Vector3.zero.WithY(650));
		}
		
	}	
	
	public static bool SetDialogState(bool active) {	
		if(isInst) {
			return Instance.setDialogState(active);
		}
		return false;
	}
	
	public bool setDialogState(bool active) {		
		dialogActive = active;
		GameDraggableEditor.editingEnabled = !dialogActive;
		return dialogActive;
	}
	
	public static void ShowUIPanelDialogMeta() {
		if(isInst) {
			Instance.showUIPanelDialogMeta();
		}
	}
		
	public void showUIPanelDialogMeta() {
		SetDialogState(true);
		GameDraggableEditor.HideAllEditDialogs();
		if(UIPanelDialogEditMeta.isInst) {
			UIPanelDialogEditMeta.Instance.LoadData();
		}
		//GameDraggableEditor.Ed
		//GameDraggableEditor.HideUIPanelEditAsset();
		if(gameEditDialogMetaObject != null) {
			TweenPosition.Begin(gameEditDialogMetaObject, .3f, Vector3.zero.WithX(0));
		}
	}
	
	public static void HideUIPanelDialogMeta() {
		if(isInst) {
			Instance.hideUIPanelDialogMeta();
		}
	}	
	
	public void hideUIPanelDialogMeta() {
		if(gameEditDialogMetaObject != null) {
			TweenPosition.Begin(gameEditDialogMetaObject, 0f, Vector3.zero.WithX(3000));
		}
		SetDialogState(false);
	}
	
	public void ShowUIPanelDialogItems() {
		if(isInst) {
			Instance.showUIPanelDialogItems();
		}
	}
	
	public void showUIPanelDialogItems() {
		SetDialogState(true);
		GameDraggableEditor.editingEnabled = !dialogActive;
		
		if(UIPanelEditAsset.Instance.actionState == UIPanelEditAssetActionState.NONE) {
			HideAllEditDialogs();
		}
		if(UIPanelDialogEditItems.isInst) {
			
			if(UIPanelEditAsset.Instance.actionState == UIPanelEditAssetActionState.SELECT_ITEM) {
				UIPanelDialogEditItems.Instance.filterType = UIPanelDialogEditItemsFilter.levelAssets;
			}
			else if(UIPanelEditAsset.Instance.actionState == UIPanelEditAssetActionState.SELECT_ITEM) {
				UIPanelDialogEditItems.Instance.filterType = UIPanelDialogEditItemsFilter.levelEffect;
			}
			else {		
				UIPanelDialogEditItems.Instance.filterType = UIPanelDialogEditItemsFilter.all;
			}
			
			UIPanelDialogEditItems.Instance.LoadData();
		}
		if(gameEditDialogItemsObject != null) {
			TweenPosition.Begin(gameEditDialogItemsObject, .3f, Vector3.zero.WithX(0));
		}
	
		HideUIPanelEdit();
	
	}
	
	public static void HideUIPanelDialogItems() {
		if(isInst) {
			Instance.hideUIPanelDialogItems();
		}
	}
	
	public void hideUIPanelDialogItems() {
		if(gameEditDialogItemsObject != null) {
			TweenPosition.Begin(gameEditDialogItemsObject, 0f, Vector3.zero.WithX(3000));
		}
		
		SetDialogState(false);
	}
	
	
	// ----------------------------------------------------------------------
}