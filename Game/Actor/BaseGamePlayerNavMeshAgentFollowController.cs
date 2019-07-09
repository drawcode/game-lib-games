using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum GamePlayerFollowAgentType {
    AlwaysPursue,
    RangedPursue,
    RangedThenAlwaysPursue,
}

public class BaseGamePlayerNavMeshAgentFollowController : GameObjectBehavior {

    public UnityEngine.AI.NavMeshAgent agent;
    public Transform targetFollow;
    public float agentDistance = 4f;
    public float targetAttractRange = 10f;
    public float targetLimitRange = 60f;
    public GamePlayerNavMeshAgentState agentState = GamePlayerNavMeshAgentState.PURSUE;
    public GamePlayerController gamePlayerController;
    public GamePlayerFollowAgentType followType = GamePlayerFollowAgentType.AlwaysPursue;


    Vector3 targetPosition = Vector3.zero;
    Vector3 targetPositionFiltered = Vector3.zero;

    //currentControllerData.navMeshAgentFollowController.agentDistance = 10;
    //currentControllerData.navMeshAgentFollowController.targetAttractRange = 20;
    //currentControllerData.navMeshAgentFollowController.targetLimitRange = 40;
    //currentControllerData.navMeshAgentFollowController.targetFollow =
    //GameController.CurrentGamePlayerController.gamePlayerSidekickTarget.transform;

    // Use this for initialization
    public virtual void Start() {

        if (agent == null) {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        NavigateToDestination();
    }

    public virtual void FindGamePlayer() {
        if (gamePlayerController == null) {
            gamePlayerController = GetComponent<GamePlayerController>();
        }
    }

    public virtual void StopAgent() {
        if (agent != null && agentState != GamePlayerNavMeshAgentState.STOP) {
            if (agent.enabled) {
                agent.destination = gameObject.transform.position;
                agent.StopAgent();
                agentState = GamePlayerNavMeshAgentState.STOP;
            }
        }
    }

    public virtual void StartAgent() {
        if (agent != null && agentState != GamePlayerNavMeshAgentState.PURSUE) {
            agentState = GamePlayerNavMeshAgentState.PURSUE;
            agent.StartAgent();
            NavigateToDestination();
        }
    }

    public virtual void RunType(GamePlayerFollowAgentType gamePlayerFollowAgentType) {
        followType = gamePlayerFollowAgentType;
        ResetTargetPositions();
        NavigateToDestination();
    }

    public virtual void ResetTargetPositions() {
        if (agent.enabled) {
            agent.destination = transform.position;
        }
    }


    public virtual void NavigateToDestination() {

        if (agent == null) {
            return;
        }
        else {

            if (!agent.isActiveAndEnabled) {
                return;
            }

            if (agentState == GamePlayerNavMeshAgentState.STOP) {
                agent.destination = gameObject.transform.position;
                return;
            }

            targetPosition = transform.position;
            targetPositionFiltered = targetPosition;

            float distance = 0f;

            if (targetFollow == null) {
                // look for main player types                
                GamePlayerThirdPersonController gamePlayerThirdPersonController =
                    FindObjectOfType(typeof(GamePlayerThirdPersonController)) as GamePlayerThirdPersonController;
                if (gamePlayerThirdPersonController != null) {
                    targetFollow = gamePlayerThirdPersonController.gameObject.transform;
                }
            }

            if (targetFollow != null) {

                distance = Vector3.Distance(agent.destination, targetFollow.position);

                if (followType == GamePlayerFollowAgentType.RangedPursue
                    || followType == GamePlayerFollowAgentType.RangedThenAlwaysPursue) {

                    if (distance <= targetAttractRange
                        || distance <= agentDistance) {

                        targetPosition = targetFollow.position;

                        if (followType == GamePlayerFollowAgentType.RangedThenAlwaysPursue) {
                            // switch to always when in range after first time
                            RunType(GamePlayerFollowAgentType.AlwaysPursue);
                        }

                    }
                    else if (distance > targetLimitRange) {
                        targetPosition = transform.position;
                    }
                }
                else if (followType == GamePlayerFollowAgentType.AlwaysPursue) {

                    targetPosition = targetFollow.position;
                }

                if (distance < agentDistance) {

                    targetPositionFiltered = targetPosition;

                    // Only rotate on y
                    targetPositionFiltered.y = gameObject.transform.position.y;

                    transform.LookAt(targetPositionFiltered);
                }
            }

            if (agent.enabled) {
                agent.destination = targetPosition;
            }
        }
    }

    // Update is called once per frame
    public virtual void Update() {

        if (!GameConfigs.isGameRunning) {
            StopAgent();
            return;
        }
        else {
            StartAgent();
        }

        FindGamePlayer();

        if (gamePlayerController != null) {
            if (gamePlayerController.isDead) {
                StopAgent();
                return;
            }
        }

        if (agent != null) {
            if (agent.enabled) {
                if (agent.remainingDistance <= 50f || agent.isPathStale) {
                    NavigateToDestination();
                }
            }
            else {
                agent.enabled = true;
                NavigateToDestination();
            }
        }
    }
}