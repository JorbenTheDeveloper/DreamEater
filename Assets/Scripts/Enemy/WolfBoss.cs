using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IWolfPhase
{
    Animator Animator { get; set; }
    NavMeshAgent NavMeshAgent { get; set; }
    WolfBoss WolfBoss { get; set; }

    void Enter();
    void Update();
    void Exit();
}

public class WolfBoss : MonoBehaviour
{
    public int SlowSpeed = 3;
    public int FastSpeed = 10;

    // phase 1
    public int StrikeDistance = 3;
    public float ClawAnimIndicatorDuration = 0.5f;

    public GameObject ClawAnim;
    public GameObject LungeIndicator;
    public bool isPhase1 = false;

    NavMeshAgent NavMeshAgent;
    Animator Animator;
    IWolfPhase Phase1;
    IWolfPhase PhaseLunge;

	// Start is called before the first frame update
	void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;

        Animator = GetComponent<Animator>();

        Phase1 = new WolfPhase1
        {
            WolfBoss = this,
            Animator = Animator,
            NavMeshAgent = NavMeshAgent
        };
        Phase1.Enter();

		PhaseLunge = new WolfPhaseLunge
		{
			WolfBoss = this,
			Animator = Animator,
			NavMeshAgent = NavMeshAgent
		};
		PhaseLunge.Enter();
	}

    // Update is called once per frame
    void Update()
    {
        if (isPhase1)
        {
            Phase1.Update();

		}else
        {
			PhaseLunge.Update();
		}
		
    }
}
