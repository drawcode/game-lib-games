using UnityEngine;
using System.Collections;

public class GamePlayerSpawnArea {
    public static string leftUpper = "left-upper";
    public static string rightUpper = "right-upper";
    public static string middleUpper = "middle-upper";

    public static string leftMiddle = "left-middle";
    public static string rightMiddle = "right-middle";
    public static string middleMiddle = "middle-middle";

    public static string leftLower = "left-lower";
    public static string rightLower = "right-lower";
    public static string middleLower = "middle-lower";
}

public class GamePlayerSpawn : GameObjectBehavior {

    public Vector3 distance;
    public GameObject spawnBouncyBaseObject;
    public GameObject spawnBouncyObject;
    public GameObject spawnAnchorObject;

    public string code = GamePlayerSpawnArea.middleMiddle;

    void Start() {
        //StartCoroutine(StartAnimating());
    }

    public void ResetBouncyObjectDelayed(float delay) {
        StartCoroutine(ResetBouncyObjectDelayedCo(delay));
    }

    IEnumerator ResetBouncyObjectDelayedCo(float delay) {
        yield return new WaitForSeconds(delay);
        if (spawnBouncyObject != null) {
            spawnBouncyObject.transform.position = gameObject.transform.position;
        }
    }

    public void ResetBouncyBaseObjectDelayed(float delay) {
        StartCoroutine(ResetBouncyBaseObjectDelayedCo(delay));
    }

    IEnumerator ResetBouncyBaseObjectDelayedCo(float delay) {
        yield return new WaitForSeconds(delay);
        if (spawnBouncyBaseObject != null) {
            spawnBouncyBaseObject.transform.position = gameObject.transform.position;
        }
    }

    public void AnimateDelayed(float delay) {
        StartCoroutine(AnimateDelayedCo(delay));
    }

    IEnumerator AnimateDelayedCo(float delay) {

        yield return new WaitForSeconds(delay);

        //if(gameObject != null) {
        //Vector3 initialPosition = spawnBouncyBaseObject.transform.position;

        /*
        iTween.MoveTo(spawnBouncyBaseObject, 
            iTween.Hash( 
                "looptype", iTween.LoopType.pingPong,
                "time", 1f,
                //"delay", 1f,
                "easetype", iTween.EaseType.easeInBounce,
                //"space", Space.Self,
                "x", initialPosition.x + distance.x,
                "y", initialPosition.y + distance.y,
                "z", initialPosition.z + distance.z
            )			
        );
*/
        //}
    }
}