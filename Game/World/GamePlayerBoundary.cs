using UnityEngine;
using System.Collections;

public class GamePlayerBoundary : GameObjectBehavior {
	
	public Vector3 distance;
	public GameObject boundaryBouncyBaseObject;
	public GameObject boundaryBouncyObject;
	public GameObject boundaryAnchorObject;
	public Vector3 positionPlaceholder;
		
	void Start () {
		//StartCoroutine(StartAnimating());
		positionPlaceholder = transform.position;
		
		Invoke("ResetPosition", 5f);
	}	
	
	public void ResetPosition() {
		if(boundaryBouncyObject.transform.position != positionPlaceholder) {
			boundaryBouncyObject.transform.position = positionPlaceholder;
		}
	}
	
	public void ResetBouncyObjectDelayed(float delay) {
		StartCoroutine(ResetBouncyObjectDelayedCo(delay));
	}
	
	IEnumerator ResetBouncyObjectDelayedCo(float delay) {
		yield return new WaitForSeconds(delay);
		if(boundaryBouncyObject != null) {
			boundaryBouncyObject.transform.position = gameObject.transform.position;
		}
	}
	
	public void AnimateDelayed(float delay) {
		StartCoroutine(AnimateDelayedCo(delay));
	}
	
	IEnumerator AnimateDelayedCo(float delay) {
	
		yield return new WaitForSeconds(delay);
		
		if(gameObject != null) {					
			iTween.MoveTo(boundaryBouncyBaseObject, 
				iTween.Hash( 
					"looptype", iTween.LoopType.pingPong,
					"time", 1f,
					//"delay", 1f,
					"easetype", iTween.EaseType.easeInBounce,
					//"space", Space.Self,
					"x", positionPlaceholder.x + distance.x,
					"y", positionPlaceholder.y + distance.y,
					"z", positionPlaceholder.z + distance.z
				)			
			);
		}
	}
}
