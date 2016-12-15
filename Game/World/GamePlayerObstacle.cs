using UnityEngine;
using System.Collections;

public class GamePlayerObstacle : GameObjectBehavior {

    public Vector3 distance;
    public GameObject obstacleBouncyBaseObject;
    public GameObject obstacleBouncyObject;
    public GameObject obstacleAnchorObject;
    public Vector3 positionPlaceholder;

    public float resetTime = 0f;

    void Start() {
        //StartCoroutine(StartAnimating());
        positionPlaceholder = transform.position;

        if (resetTime > 0f) {
            InvokeRepeating("ResetPosition", resetTime, resetTime);
        }
    }

    public void ResetPosition() {
        if (obstacleBouncyObject.transform.position != positionPlaceholder) {
            if (obstacleBouncyObject.Has<Rigidbody>()) {
                Rigidbody rigid = obstacleBouncyObject.Get<Rigidbody>();
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
            obstacleBouncyObject.transform.position = positionPlaceholder;
        }
    }

    public void ResetBouncyObjectDelayed(float delay) {
        StartCoroutine(ResetBouncyObjectDelayedCo(delay));
    }

    IEnumerator ResetBouncyObjectDelayedCo(float delay) {
        yield return new WaitForSeconds(delay);
        if (obstacleBouncyObject != null) {
            obstacleBouncyObject.transform.position = gameObject.transform.position;
        }
    }

    public void AnimateDelayed(float delay) {
        StartCoroutine(AnimateDelayedCo(delay));
    }

    IEnumerator AnimateDelayedCo(float delay) {

        yield return new WaitForSeconds(delay);

        /*
		if(gameObject != null) {					
			iTween.MoveTo(obstacleBouncyBaseObject, 
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
  */
    }
}