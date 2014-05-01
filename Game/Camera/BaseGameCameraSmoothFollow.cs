using UnityEngine;
using System.Collections;

using Engine.Events;

public class BaseGameCameraSmoothFollow : GameObjectBehavior {
	
	public Transform target;
	public float smoothTime= 0.3f;
	private Transform thisTransform;
	private Vector3 velocity;
	public Vector3 offset;
	public Vector3 offsetInitial;
	
	public bool followX = true;
	public bool followY = false;
	public bool followZ = true;
	
	private float orthographicSizeInitial = 15f;
	public float orthographicSizeMin = 15f;
	public float orthographicSizeMax = 25f;
	
	public int boundary = 5;
	public int speed = 25;
	 
	private int theScreenWidth;
	private int theScreenHeight;
	
	public bool allowZoom = false;
	public bool allowScrolling = false;
	public bool allowFollowing = true;
	
	public Camera cam;
	
	public virtual void OnEnable() {
		Messenger<string, Vector3>.AddListener("input-axis", OnInputAxisHandler);
	}
	
	public virtual void OnDisable() {
		Messenger<string, Vector3>.RemoveListener("input-axis", OnInputAxisHandler);
	}
	
	public virtual void OnInputAxisHandler(string name, Vector3 val) {
		if(val.x > .3f || val.y > .3f) {
			offset = offsetInitial;
		}
	}
	
	public virtual void  Start (){
		thisTransform = transform;
	    theScreenWidth = Screen.width;
	    theScreenHeight = Screen.height;
		offsetInitial = offset;
		
		FindCamera();
		
		if(cam != null) {
			orthographicSizeInitial = cam.orthographicSize;
		}		
	}
	
	public virtual void FindCamera() {
		if(cam == null) {
			cam = gameObject.GetComponent<Camera>();
		}		
	}
	
	public virtual void Reset() {
		FindCamera();
		offset = offsetInitial;
		if(cam != null) {
			cam.orthographicSize = orthographicSizeInitial;
		}
	}
	
	public virtual void SetZoom(float modifier) {
		
		float cur = Camera.main.orthographicSize;
		
		cur = orthographicSizeInitial + modifier;
		
		if(cam != null) {
			cam.orthographicSize = Mathf.Clamp(cur, orthographicSizeMin, orthographicSizeMax );			
		}	
	}
	
	public virtual void Update() {
					
	}
		
	public virtual void LateUpdate() {		
		
		if(allowZoom && cam != null) {
			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				cam.orthographicSize++;
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				cam.orthographicSize--;
			}
			
			cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, orthographicSizeMin, orthographicSizeMax );
		}
		
		if(allowScrolling) {
						
			if (Input.mousePosition.x > theScreenWidth - boundary) {
				offset.x = offset.x + speed * Time.deltaTime;
			}
			
			if (Input.mousePosition.x < 0 + boundary) {
				offset.x = offset.x - speed * Time.deltaTime;
			}
			
			if (Input.mousePosition.y > theScreenHeight - boundary) {
				offset.y = offset.y + speed * Time.deltaTime;
			}
			
			if (Input.mousePosition.y < 0 + boundary) {
				offset.y = offset.y - speed * Time.deltaTime;
			}
		}
		
		if(allowFollowing) {
			if(thisTransform != null && target != null) {
				Vector3 temp = target.position;
				
				if(followX) {
					temp.x = Mathf.SmoothDamp(thisTransform.position.x, 
						target.position.x + offset.x, ref velocity.x, smoothTime);
				}
				
				if(followY) {
					temp.y = Mathf.SmoothDamp( thisTransform.position.y, 
						target.position.y + offset.y, ref velocity.y, smoothTime);
				}
				
				if(followZ) {
					temp.z = Mathf.SmoothDamp( thisTransform.position.z, 
						target.position.z + offset.z, ref velocity.z, smoothTime);
				}
					
				thisTransform.position = temp;
			}
		}
	}	
}