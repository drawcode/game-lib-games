using System;
using System.Collections;

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
	
    public static string StateEditOn = "app-stat-edit-on";
    public static string StateEditOff = "app-stat-edit-off";
}

public class GameDraggableEditor : MonoBehaviour {
	
	public static GameDraggableEditor Instance;
	
	public Transform grabbed;
	public Transform lastGrabbed;
	public float grabDistance = 10.0f;
	public int grabLayerMask;
	public Vector3 grabOffset; //delta between transform transform position and hit point
	public bool  useToggleDrag = true; // Didn't know which style you prefer. 
	
	public Vector3 currentEditorPosition;
		
	public static bool editingEnabled = false;
	public string dragTag = "drag";
	public GameDraggableCanvasType gameDraggableCanvasType = GameDraggableCanvasType.CANVAS_2D;
	public GameDraggableCanvasMode gameDraggableCanvasMode = GameDraggableCanvasMode.EDITING;
	
	public static GameDraggableEditEnum appEditState = GameDraggableEditEnum.StateNotEditing;
	
	// For tooltip on item selected.
	public string assetCodeCreating = "";	
	public GameObject assetObjectCreating;
			    
	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
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
				if(editingEnabled 
					&& appEditState == GameDraggableEditEnum.StateEditing) {
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
	        } 
			else { 
	            Grab();
	        }
	    } 
		else {
	        if(grabbed) {
	           //restore the original layermask
	           grabbed.gameObject.layer = grabLayerMask;
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
						if(levelItem != null) {
							assetCodeCreating = levelItem.gameLevelItemAsset.asset_code;
						}
					}
					if(lastGrabbed != grabbed) {
						lastGrabbed = grabbed;
						Messenger<GameObject>.Broadcast(
							GameDraggableEditorMessages.editorGrabbedObjectChanged, lastGrabbed.gameObject);
					}
		            //set the object to ignore raycasts
		            grabLayerMask = grabbed.gameObject.layer;
		            grabbed.gameObject.layer = 2;
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
			
			editingEnabled = false;
		}
		else if(appEditState == GameDraggableEditEnum.StateEditing) {
			changeStateEditing(GameDraggableEditEnum.StateNotEditing);
				
			editingEnabled = true;
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
			Messenger.Broadcast(GameDraggableEditorMessages.StateEditOff);
		}
		else if(appEditState == GameDraggableEditEnum.StateEditing) {			
			Messenger.Broadcast(GameDraggableEditorMessages.StateEditOn);
		}
	}
	
	// ----------------------------------------------------------------------
	// ----------------------------------------------------------------------
}