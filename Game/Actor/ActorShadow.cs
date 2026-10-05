using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Engine.Game.Actor {

    public class ActorShadow : GameObjectBehavior {

        public GameObject objectShadow;
        public GameObject objectParent;
        private Vector3 surfaceNormal;
        private Vector3 surfaceHitPoint;
        private Vector3 surfaceRightVector;
        public Vector3 surfaceForwardVector;
        float lastUpdate = 0f;
        public GameObject gamePlayerObject;

#if USE_GAME_LIB_GAMES
        public GamePlayerController gamePlayerController;
#endif
        //
        public Vector3 initialPlayerPosition = Vector3.zero.WithY(-1);
        public Vector3 currentPlayerPosition = Vector3.zero;
        public Vector3 lastPlayerPosition = Vector3.zero.WithY(-1);
        //
        public Vector3 deltaPlayerPosition = Vector3.zero;
        public Vector3 initialScale = Vector3.zero;
        //
        Quaternion currentShadowRotation = Quaternion.identity;
        Quaternion lastShadowRotation = Quaternion.Euler(Vector3.zero.WithY(-1));
        //
        Vector3 currentShadowPosition = Vector3.zero;
        Vector3 lastShadowPosition = Vector3.zero.WithY(-1);

        // Both of these were resolved EVERY frame, per actor: LayerMask.NameToLayer is a string
        // lookup into the layer table and Camera.main is a scene query. Neither answer changes
        // while the level is up, so resolve each once and keep it.
        //
        // -1 is the "not resolved yet" marker; NameToLayer itself returns -1 for a layer that does
        // not exist, in which case the shifted mask would be meaningless, so that case is held to
        // the same retry path rather than being cached as a bad value.
        static int terrainMaskCached = -1;
        Camera shadowCamera;

        static int GetTerrainMask() {

            if (terrainMaskCached == -1) {

                int layer = LayerMask.NameToLayer("GameGround");

                if (layer < 0) {
                    return 0;
                }

                terrainMaskCached = 1 << layer;
            }

            return terrainMaskCached;
        }

        private void Start() {

        }

        private void Update() {

            if (GameConfigs.isUIRunning) {
                return;
            }

            if (!GameConfigs.isGameRunning) {
                return;
            }

#if USE_GAME_LIB_GAMES
            if (gamePlayerObject == null) {

                gamePlayerObject = gameObject.FindTypeAboveObjectRecursive<GamePlayerController>();

                if (gamePlayerObject != null) {
                    if (gamePlayerController == null) {
                        gamePlayerController =
                            gamePlayerObject.Get<GamePlayerController>();
                    }
                }
            }

            if (!gamePlayerController.IsWithinRenderDistanceToGamePlayerCurrent()) {
                return;
            }

#endif

            if (objectParent != null && objectShadow != null) {

                // Adjust size

                //if(gamePlayerController.IsPlayerControlled) {

                currentPlayerPosition = gamePlayerObject.transform.localPosition;

                if (lastPlayerPosition != currentPlayerPosition) {

                    if (initialPlayerPosition == Vector3.zero.WithY(-1)) {
                        initialPlayerPosition = gamePlayerObject.transform.localPosition;
                    }

                    if (initialScale == Vector3.zero) {
                        initialScale = objectShadow.transform.localScale;
                    }

                    deltaPlayerPosition = currentPlayerPosition - initialPlayerPosition;

                    //if (deltaPlayerHeight > .1f) {

                    //Debug.Log("ActorShadow deltaPlayerHeight:" + deltaPlayerHeight);

                    Vector3 scaleChange = initialScale * ((5 - currentPlayerPosition.y) / 5);

                    if (scaleChange.x < .3f
                        || scaleChange.y < .3f
                        || scaleChange.z < .3f) {

                        scaleChange = Vector3.zero.WithX(.3f).WithY(.3f).WithZ(.3f);
                    }

                    if (scaleChange != objectShadow.transform.localScale) {
                        objectShadow.transform.localScale = scaleChange;

                        //Debug.Log("ActorShadow scaleChange:" + scaleChange);
                    }

                    if (shadowCamera == null) {
                        shadowCamera = Camera.main;
                    }

                    if (gamePlayerObject.IsRenderersVisibleByCamera(shadowCamera)) {

                        //if (gamePlayerController.IsAgentState()) {
                        //Debug.Log("ActorShadow IsRenderersVisibleByCamera:gamePlayerController.IsAgentState:" + gamePlayerController.IsAgentState());
                        //}

                        // Get location to put shadow at using parent normal and terrain mask
                        //float distance = Vector3.Distance(
                        //    objectParent.transform.position, 
                        //    objectShadow.transform.position);

                        RaycastHit hit;
                        Vector3 topPoint = objectParent.transform.position + Vector3.up * 1;
                        Vector3 bottomPoint = objectParent.transform.position - Vector3.up * 1;
                        Vector3 collisionVector = bottomPoint - topPoint;

                        int terrainMask = GetTerrainMask();

                        if (Physics.Raycast(topPoint, collisionVector, out hit, 100.0f, terrainMask)) {
                            surfaceNormal = hit.normal;

                            surfaceHitPoint = hit.point;
                            surfaceRightVector = Vector3.Cross(transform.forward, surfaceNormal);
                            surfaceForwardVector = Vector3.Cross(surfaceNormal, surfaceRightVector);

                            if (objectShadow != null) {
                                currentShadowPosition = surfaceHitPoint;
                                currentShadowPosition.y += 0.3f;

                                objectShadow.transform.position = currentShadowPosition;
                                objectShadow.transform.up = Vector3.up;//surfaceNormal;
                                objectShadow.transform.LookAt(surfaceHitPoint - transform.right);

                                lastShadowPosition = currentShadowPosition;

                                //Debug.Log("ActorShadow surfaceHitPoint:" + surfaceHitPoint);
                                //Debug.Log("ActorShadow shadowPos:" + shadowPos);

                                lastUpdate += Time.deltaTime;
                                if (lastUpdate > 10f) {
                                    lastUpdate = 0;

                                    //float alpha = (1 / (distance / 50)) - 1;
                                    //iTween.FadeTo(objectShadow, alpha, .5f); 
                                }

                                Debug.DrawLine(topPoint, bottomPoint, Color.yellow);

                                //Debug.DrawLine(hit.point, surfaceNormal, Color.green);
                                //Debug.DrawLine(hit.point, surfaceForwardVector, Color.blue);
                                //Debug.DrawLine(hit.point, surfaceRightVector, Color.red);

                                currentShadowRotation = objectShadow.transform.rotation;
                                currentShadowRotation.x = 0f;
                                currentShadowRotation.z = 0f;
                                objectShadow.transform.rotation = currentShadowRotation;

                                lastShadowRotation = currentShadowRotation;

                            }
                        }

                        lastPlayerPosition = currentPlayerPosition;
                    }

                    //}
                    //}

                }
            }
        }
    }
}