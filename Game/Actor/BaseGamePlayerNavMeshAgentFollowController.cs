using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum GamePlayerFollowAgentType {
    AlwaysPursue,
    RangedPursue
}

public class BaseGamePlayerNavMeshAgentFollowController : GameObjectBehavior {
    
    public NavMeshAgent agent;
    public Transform targetFollow;
    public float agentDistance = 4f;
    public float targetAttractRange = 10f;
    public float targetLimitRange = 60f;
    public GamePlayerNavMeshAgentState agentState = GamePlayerNavMeshAgentState.PURSUE;
    public GamePlayerController gamePlayerController;
    public GamePlayerFollowAgentType followType = GamePlayerFollowAgentType.AlwaysPursue;

    // Use this for initialization
    public virtual void Start() {
        
        agent = GetComponent<NavMeshAgent>();
        
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
                agent.Stop();
                agentState = GamePlayerNavMeshAgentState.STOP;
            }
        }
    }
    
    public virtual void StartAgent() {
        if (agent != null && agentState != GamePlayerNavMeshAgentState.PURSUE) {
            agentState = GamePlayerNavMeshAgentState.PURSUE;
            agent.Resume();
            NavigateToDestination();
        }
    }
    
    public virtual void NavigateToDestination() {   
        
        if (agent != null) {
            if (agentState == GamePlayerNavMeshAgentState.STOP) {
                agent.destination = gameObject.transform.position;
                return;
            }

            Vector3 targetPosition = transform.position;
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

                if (followType == GamePlayerFollowAgentType.RangedPursue) {
                
                    if (distance <= targetAttractRange
                        || distance <= agentDistance) {

                        targetPosition = targetFollow.position;

                    }
                    else if (distance > targetLimitRange) {
                        targetPosition = transform.position;
                    }
                }
                else if (followType == GamePlayerFollowAgentType.AlwaysPursue) {  

                    targetPosition = targetFollow.position;
                }
                
                if (distance < agentDistance) {  
                    transform.LookAt(targetPosition);
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
