using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class StateController : MonoBehaviour {

	public State currentState;
	public EnemyStats enemyStats;
	public Transform eyes;
	public State remainState;
	public GameObject m_RedScreen;

	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public TankShooting tankShooting;
	[HideInInspector] public List<Transform> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public float stateTimeElapsed;

	private bool aiActive;


	void Awake () 
	{
		tankShooting = GetComponent<TankShooting> ();
		navMeshAgent = GetComponent<NavMeshAgent> ();
		m_RedScreen = GameObject.Find("Canvas/Red");
	}

	public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
	{
		wayPointList = wayPointsFromTankManager;
		aiActive = aiActivationFromTankManager;
		if (aiActive) 
		{
			navMeshAgent.enabled = true;
		} else 
		{
			navMeshAgent.enabled = false;
		}
	}

	public void TransitionToState(State nextState)
	{
		if (nextState == remainState) return;
		currentState = nextState;
		
		if(currentState.name == "ChaseScanner" || currentState.name == "ChaseChaser") {
			var color = m_RedScreen.GetComponent<Image>().color;
			color.a = 0.8f;
			m_RedScreen.GetComponent<Image>().color = color;
		}

		OnExitState();
	}

	public bool CheckIfCountDownElapsed(float duration)
	{
		stateTimeElapsed += Time.deltaTime;
		return stateTimeElapsed >= duration;
	}

	void Update()
	{
		if (!aiActive) return;

		currentState.UpdateState(this);

		if (m_RedScreen != null) {
			if (m_RedScreen.GetComponent<Image>().color.a > 0) {
				var color = m_RedScreen.GetComponent<Image>().color;
				color.a -= 0.01f;
				m_RedScreen.GetComponent<Image>().color = color;
			}
		}
	}

	void OnExitState()
	{
		stateTimeElapsed = 0;
	}

	void OnDrawGizmos()
	{
		if (currentState != null && eyes != null)
		{
			Gizmos.color = currentState.sceneGizmoColor;
			Gizmos.DrawWireSphere(eyes.position, enemyStats.lookSphereCastRadius);
		}
	}

}