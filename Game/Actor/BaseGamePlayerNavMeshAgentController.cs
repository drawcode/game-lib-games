using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum GamePlayerNavMeshAgentState {
	PURSUE,
	STOP
}

public class BaseGamePlayerNavMeshAgentController : GameObjectBehavior {

	public UnityEngine.AI.NavMeshAgent agent;
	public Vector3 nextDestination = Vector3.one;
	public GamePlayerNavMeshAgentState agentState = GamePlayerNavMeshAgentState.PURSUE;

	// Use this for initialization
	public virtual void Start() {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		nextDestination = transform.position;
		NavigateToDestination();
	}

	public virtual void StopAgent() {
		if (agent != null) {
			agentState = GamePlayerNavMeshAgentState.STOP;
			agent.destination = gameObject.transform.position;
			agent.StopAgent();
		}
	}

	public virtual void StartAgent() {
		if (agent != null) {
			agent.StartAgent();
			agentState = GamePlayerNavMeshAgentState.PURSUE;
			NavigateToDestination();
		}
	}

	public virtual void NavigateToDestination() {

		if (agent != null) {

			if (agentState == GamePlayerNavMeshAgentState.STOP) {
				agent.destination = gameObject.transform.position;
				return;
			}

			agent.destination = nextDestination;
		}
	}

	public virtual Vector3 GetRandomLocation() {
		return new Vector3(UnityEngine.Random.Range(-30, -30), 0, UnityEngine.Random.Range(-30, 30));
	}

	//public Vector3 GetRandomLocation() {
	//	return new Vector3(UnityEngine.Random.Range(0, 50), 0, UnityEngine.Random.Range(0, 50));
	//}

	// Update is called once per frame
	public virtual void Update() {

		if (!GameConfigs.isGameRunning) {
			StopAgent();
			return;
		}
		else {
			//StartAgent();
		}

		if (agentState != GamePlayerNavMeshAgentState.PURSUE) {
			nextDestination = gameObject.transform.position;
			NavigateToDestination();
			return;
		}

		if (agent != null && agentState == GamePlayerNavMeshAgentState.PURSUE) {
			if (agent.remainingDistance <= 50f || agent.isPathStale) {
				// get next destination
				nextDestination = GetRandomLocation();
				NavigateToDestination();
			}
		}
	}
}