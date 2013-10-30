using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum GamePlayerNavMeshAgentState {
	PURSUE,
	STOP
}

public class BaseGamePlayerNavMeshAgentController : MonoBehaviour {
	
	public NavMeshAgent agent;
	public Vector3 nextDestination = Vector3.one;	
	public GamePlayerNavMeshAgentState agentState = GamePlayerNavMeshAgentState.PURSUE;
	
	// Use this for initialization
	public void Start() {
		agent = GetComponent<NavMeshAgent>();
		nextDestination = transform.position;
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
			
			agent.destination = nextDestination;
		}
	}
		
	public Vector3 GetRandomLocation() {
		return new Vector3(UnityEngine.Random.Range(-30, -30), 0, UnityEngine.Random.Range(-30, 30));
	}
	
	//public Vector3 GetRandomLocation() {
	//	return new Vector3(UnityEngine.Random.Range(0, 50), 0, UnityEngine.Random.Range(0, 50));
	//}
	
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
			if(agent.remainingDistance <= 50f || agent.isPathStale) {
				// get next destination
				nextDestination = GetRandomLocation();
				NavigateToDestination();
			}
		}
	}
}
