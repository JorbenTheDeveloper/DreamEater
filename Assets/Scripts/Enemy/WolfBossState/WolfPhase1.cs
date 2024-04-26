using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class WolfPhase1 : IWolfPhase
{
    public Animator Animator { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public GameObject MyObject => WolfBoss.gameObject;

    public WolfBoss WolfBoss { get; set; }

    private float DistanceToPlayer => Vector3.Distance(MyObject.transform.position, Player.Instance.transform.position);

    private string _walkAnimName = "IsWalk"; 

    private State _state = State.None;
    private float _slowWalkTimer = Random.Range(2f, 4f);

    private float _tiredTimer = Random.Range(2f, 4f);

    // walks slow first for random of 2-4 seconds
    // then walks faster untill close enought to player
    // does claw attack
    // tired for random of 2-4 seconds

    public void Enter()
    {
    }

    public void Exit()
    {
        Animator.SetBool(_walkAnimName, false);
    }

    public void Update()
    {
        switch (_state)
        {
            case State.None:
                Animator.SetBool(_walkAnimName, false);
                NavMeshAgent.isStopped = true;
                _slowWalkTimer = Random.Range(2f, 4f);
                _tiredTimer = Random.Range(2f, 4f);

                _state = State.SlowWalk;
                break;
            case State.SlowWalk:
                NavMeshAgent.speed = WolfBoss.SlowSpeed;
                WalkTowardPlayer();

                _slowWalkTimer -= Time.deltaTime;
                if (_slowWalkTimer <= 0)
                {
                    _state = State.FastWalk;
                }
                break;
            case State.FastWalk:
                NavMeshAgent.speed = WolfBoss.FastSpeed;
                WalkTowardPlayer();
                break;

            case State.Attack:
                
                _state = State.Tired;
                break;

            case State.Tired:
                _tiredTimer -= Time.deltaTime;
                if ( _tiredTimer <= 0)
                {
                    _state = State.None;
                }
                break;

        }
    }

    private void WalkTowardPlayer()
    {
        TurnToDirection();
        Animator.SetBool(_walkAnimName, true);
        NavMeshAgent.isStopped = false;
        NavMeshAgent.SetDestination(Player.Instance.transform.position);

        // if player close enough, start the attack state
        if (DistanceToPlayer <= WolfBoss.StrikeDistance)
        {
            NavMeshAgent.isStopped = true;
            Animator.SetBool(_walkAnimName, false);
            _state = State.Attack;
        }
    }

    private void TurnToDirection()
    {
        var dir = NavMeshAgent.steeringTarget - MyObject.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        MyObject.transform.rotation = rotation;
    }

    public enum State
    {
        None,
        SlowWalk,
        FastWalk,
        Attack,
        Tired
    }
}
