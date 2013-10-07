using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class GameObjectChoiceAsset : MonoBehaviour {

    GameObjectChoice gameObjectChoice;
    GameObject gameObjectChoiceObject;

    void Start() {
        InvokeRepeating("FindChoiceCollisionParent", 1f, 10);
    }

    void FindChoiceCollisionParent() {
        if(gameObjectChoiceObject == null) {
            gameObjectChoiceObject = gameObject.FindTypeAboveObject<GameObjectChoice>();
        }
    
        if(gameObjectChoice == null
            && gameObjectChoiceObject != null) {
            gameObjectChoice = gameObjectChoiceObject.GetComponent<GameObjectChoice>();
            CancelInvoke("FindChoiceCollisionParent");
        }
    }

    void OnCollisionEnter(Collision collision) {

        if(gameObjectChoice != null
            && collision.transform != null) {
            GamePlayerController gamePlayerController
                = collision.transform.GetComponent<GamePlayerController>();
            if(gamePlayerController != null) {
                if(gamePlayerController.IsPlayerControlled) {
                    // trigger choice...
                   // gameObjectChoice.
                }
            }
        }

    }


}
