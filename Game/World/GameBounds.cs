using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;
using Engine.Utility;

public class GameBounds : MonoBehaviour {
	
	public GameObject boundaryTopLeft;
	public GameObject boundaryTopRight;
	public GameObject boundaryBottomLeft;
	public GameObject boundaryBottomRight;
	public GameObject boundaryBottomCeiling;
	public GameObject boundaryBottomAbyss;
	
	float lastBoundsCheck = 0f;

	void Start() {
		
	}
	
	public bool CheckBounds(Vector3 point) {
		
		if(point.x < boundaryTopRight.transform.position.x
			&& point.x > boundaryTopLeft.transform.position.x 
			&& point.y < boundaryBottomCeiling.transform.position.y
			&& point.y > boundaryBottomAbyss.transform.position.y
			&& point.z < boundaryTopLeft.transform.position.z
			&& point.z > boundaryBottomLeft.transform.position.z) {
			return true;
		}
		
		return false;
	}
	
	public Vector3 FilterBounds(Vector3 point) {
				
		if(!CheckBounds(point)) {
			
			point.x = Mathf.Clamp(point.x, 
				boundaryTopLeft.transform.position.x + .5f, 
				boundaryTopRight.transform.position.x - .5f);
			point.y = Mathf.Clamp(point.y, 
				boundaryBottomAbyss.transform.position.y + .5f, 
				boundaryBottomCeiling.transform.position.y - .5f);
			point.z = Mathf.Clamp(point.z, 
				boundaryBottomLeft.transform.position.z + .5f, 
				boundaryTopLeft.transform.position.z - .5f);
		}
		
		return point;
	}
	
	
	public bool ShouldUpdateBounds() {
		
		lastBoundsCheck += Time.deltaTime;
		
		if(lastBoundsCheck > 1f) {
			lastBoundsCheck = 0f;
			
			return true;
		}
		
		return false;
	}
	
	
	/*
	public void UpdateBounds() {
		
		if(lastBoundsChecked + 1f < Time.time) {
			lastBoundsChecked = Time.time;
			
			UpdateBounds();
		}
	}
	*/
}
	