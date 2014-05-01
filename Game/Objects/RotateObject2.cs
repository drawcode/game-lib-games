using UnityEngine;
using System.Collections;

public class RotateObject2 : GameObjectBehavior {
	
	public float RotateSpeedAlongX = 0.0f;
	public float RotateSpeedAlongY = 0.0f;
	public float RotateSpeedAlongZ = 0.0f;

	void Update () {
		// Slowly rotate the object around its axis at 1 degree/second * variable.
 		transform.Rotate(Vector3.up * Time.deltaTime * RotateSpeedAlongY);
   	 	transform.Rotate(Vector3.forward * Time.deltaTime * RotateSpeedAlongZ);
   		transform.Rotate(Vector3.right * Time.deltaTime * RotateSpeedAlongX);
	}
}
