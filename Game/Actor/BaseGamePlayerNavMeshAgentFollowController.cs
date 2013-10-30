using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class BaseGamePlayerNavMeshAgentFollowController : MonoBehaviour {
	
	public NavMeshAgent agent;
	public Transform targetFollow;
	
	public float agentDistance = 4f;
	
	public GamePlayerNavMeshAgentState agentState = GamePlayerNavMeshAgentState.PURSUE;
	
	// Use this for initialization
	public void Start() {
		agent = GetComponent<NavMeshAgent>();
		NavigateToDestination();
	}

    public void StopAgent() {
        if(agent != null) {
            agent.Stop(true);
            agentState = GamePlayerNavMeshAgentState.STOP;
            agent.destination = gameObject.transform.position;
        }
    }
    
    public void StartAgent() {
        if(agent != null) {
            agent.Resume();
            agentState = GamePlayerNavMeshAgentState.PURSUE;
            NavigateToDestination();
        }
    }
	
	public void NavigateToDestination() {	
		
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
	public void Update () {

        if(!GameConfigs.isGameRunning) {
            StopAgent();
            return;
        }
        else {
            //StartAgent();
        }
		
		if(agentState != GamePlayerNavMeshAgentState.PURSUE) {
			return;
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
