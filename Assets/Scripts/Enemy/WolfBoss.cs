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

    NavMeshAgent NavMeshAgent;
    Animator Animator;
    IWolfPhase Phase1;

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
    }

    // Update is called once per frame
    void Update()
    {
        Phase1.Update();
    }
}
