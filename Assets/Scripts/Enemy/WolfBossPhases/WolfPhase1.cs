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
    private string _clawAttackAnimName = "IsClaw";
    private string _tiredAnimName = "IsTired";

    private State _state = State.None;
    private float _slowWalkTimer = Random.Range(2f, 4f);
    private float _tiredTimer = Random.Range(2f, 4f);

    private float _clawAnimIndicatorTimer;

    public void Enter()
    {
    }

    public void Exit()
    {
		Animator.SetBool(_walkAnimName, false);
		Animator.SetBool(_clawAttackAnimName, false);
		Animator.SetBool(_tiredAnimName, false);
		NavMeshAgent.isStopped = true;

		_state = State.None;
	}

    public void Update()
    {
        switch (_state)
        {
            case State.None:
                Animator.SetBool(_walkAnimName, false);
                Animator.SetBool(_clawAttackAnimName, false);

                NavMeshAgent.isStopped = true;
                _slowWalkTimer = Random.Range(2f, 4f);
                _tiredTimer = Random.Range(2f, 4f);
                _clawAnimIndicatorTimer = WolfBoss.ClawAnimIndicatorDuration;

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

            case State.StartAttack:
				TurnToDirection();

				Animator.SetBool(_clawAttackAnimName, true);
                _state = State.CurrentAttack;
                break;

            case State.CurrentAttack:
                _clawAnimIndicatorTimer -= Time.deltaTime;
                if (_clawAnimIndicatorTimer <= 0)
                {
                    WolfBoss.ClawAnim.SetActive(true);

                    _state = State.EndAttack;
                }
                break;

            case State.EndAttack:
                if (!WolfBoss.ClawAnim.activeInHierarchy)
                {
                    Animator.SetBool(_clawAttackAnimName, false);
                    _state = State.Tired;
                }
                break;

            case State.Tired:
                _tiredTimer -= Time.deltaTime;
                Animator.SetBool(_tiredAnimName, true);
                if ( _tiredTimer <= 0)
                {
                    Animator.SetBool(_tiredAnimName, false);
                    _state = State.None;
                }
                break;
        }
    }

    public bool Stop()
    {
        return true;
    }

    public bool IsTired()
    {
        return _state == State.Tired;
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
            _state = State.StartAttack;
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
        StartAttack,
        CurrentAttack,
        EndAttack,
        Tired
    }
}
