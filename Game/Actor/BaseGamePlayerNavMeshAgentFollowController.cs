using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class BaseGamePlayerNavMeshAgentFollowController : GameObjectBehavior {
	
	public NavMeshAgent agent;
	public Transform targetFollow;
	
	public float agentDistance = 4f;
	
	public GamePlayerNavMeshAgentState agentState = GamePlayerNavMeshAgentState.PURSUE;

    public GamePlayerController gamePlayerController;
	
	// Use this for initialization
	public virtual void Start() {
		agent = GetComponent<NavMeshAgent>();
		NavigateToDestination();
	}

    public virtual void FindGamePlayer() {
        if(gamePlayerController == null) {
            gamePlayerController = GetComponent<GamePlayerController>();
        }
    }

    public virtual void StopAgent() {
        if(agent != null && agentState != GamePlayerNavMeshAgentState.STOP) {
            if(agent.enabled) {
                agent.destination = gameObject.transform.position;
                agent.Stop(true);
                agentState = GamePlayerNavMeshAgentState.STOP;
            }
        }
    }
    
    public virtual void StartAgent() {
        if(agent != null && agentState != GamePlayerNavMeshAgentState.PURSUE) {
            agentState = GamePlayerNavMeshAgentState.PURSUE;
            agent.Resume();
            NavigateToDestination();
        }
    }
	
	public virtual void NavigateToDestination() {	
		
		if(agent != null) {
			if(agentState == GamePlayerNavMeshAgentState.STOP) {
				agent.destination = gameObject.transform.position;
				return;
			}		
			
			if(targetFollow != null) {
				float distance = Vector3.Distance (agent.destination, targetFollow.position);
				
				if(distance < 4) {	
					//if(agent.enabled)
						//agent.enabled = false;
				}
				else {	
					//if(!agent.enabled)
						//agent.enabled = true;
					agent.destination = targetFollow.position;
				}
			}
			else {
				GamePlayerThirdPersonController gamePlayerThirdPersonController = 
					FindObjectOfType(typeof(GamePlayerThirdPersonController)) as GamePlayerThirdPersonController;
				if(gamePlayerThirdPersonController != null) {
					targetFollow = gamePlayerThirdPersonController.gameObject.transform;
					if(agent.enabled) {
						agent.destination = targetFollow.position;
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {

        if(!GameConfigs.isGameRunning) {
            StopAgent();
            return;
        }
        else {
            StartAgent();
        }

        FindGamePlayer();
        
        if(gamePlayerController != null) {
            if(gamePlayerController.isDead) {
                StopAgent();
            }
        }
		
		if(agent != null) {
			if(agent.enabled) {
				if(agent.remainingDistance <= 50f || agent.isPathStale) {
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
