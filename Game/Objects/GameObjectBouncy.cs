using UnityEngine;
using System.Collections;

public class GameObjectBouncy : GameObjectBehavior {
    
    public Vector3 distance;
        
    void Start() {
        //StartCoroutine(StartAnimating());
    }
    
    public void Animate() {
        StartCoroutine(AnimateCo());
    }
    
    IEnumerator AnimateCo() {
    
        yield return new WaitForSeconds(.8f);
        
        if (gameObject != null) {
            Vector3 initialPosition = gameObject.transform.position;
            

            // TODO tween bouncy
            //iTween.MoveTo(gameObject, 
            //  iTween.Hash( 
            //      "looptype", iTween.LoopType.pingPong,
            //      "time", 1f,
            //      //"delay", 1f,
            //      "easetype", iTween.EaseType.easeInBounce,
            //      //"space", Space.Self,
            //      "x", initialPosition.x + distance.x,
            //      "y", initialPosition.y + distance.y,
            //      "z", initialPosition.z + distance.z
            //  )           
            //);
        }
    }
}

