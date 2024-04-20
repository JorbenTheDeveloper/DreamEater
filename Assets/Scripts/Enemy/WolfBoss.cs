using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IWolfPhase
{
    Animator Animator { get; set; }
    NavMeshAgent NavMeshAgent { get; set; }
    void Enter();
    void Update();
    void Exit();
}

public class WolfBoss : MonoBehaviour
{
    public int Speed = 1;

    NavMeshAgent NavMeshAgent;
    Animator Animator;
    IWolfPhase Phase1;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        Phase1 = new WolfPhase1(gameObject, Speed);
        Phase1.Animator = Animator;
        Phase1.NavMeshAgent = NavMeshAgent;
        Phase1.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        Phase1.Update();
    }
}
