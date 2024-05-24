using UnityEngine;
using UnityEngine.AI;

public class WolfPhaseLunge : IWolfPhase
{
	public Animator Animator { get; set; }
	public NavMeshAgent NavMeshAgent { get; set; }
	public WolfBoss WolfBoss { get; set; }
	public bool IsRunning { get; set; }

	public float StrikeDistance = 10;
	public float ClawAnimIndicatorDuration = 1;
	public float SlowSpeed = 3;
	public float FastSpeed = 8;

	public float LungeIndicatorTimer = 3f;
	public int LungeSpeed = 35;
	public int LungeDamage = 10;

	public GameObject MyObject => WolfBoss.gameObject;
	private float DistanceToPlayer => Vector3.Distance(MyObject.transform.position, Player.Instance.transform.position);

	private State _state = State.None;
	private float _lungeIndicatorTimer;
	private float _clawAnimIndicatorTimer;
	private float _tiredTimer;

	private string _idleAnimName = "IsIdleBloody1";
	private string _walkAnimName = "IsWalkBloody1";
	private string _lungeAnimName = "IsLungeBloody1";
	private string _clawAttackAnimName = "IsClawBloody1";
	private string _tiredAnimName = "IsDazedBloody1";

	float indicatorY;
	float indicatorZ;

	public virtual void Enter()
	{
		WolfBoss.LungeIndicator.SetActive(true);
		indicatorY = WolfBoss.LungeIndicator.transform.localPosition.y;
		indicatorZ = WolfBoss.LungeIndicator.transform.localPosition.z;
		IsRunning = true;
	}

	public void Exit()
	{
		WolfBoss.LungeIndicator.SetActive(false);

		Animator.SetBool(_lungeAnimName, false);
		Animator.SetBool(_walkAnimName, false);
		Animator.SetBool(_clawAttackAnimName, false);
		Animator.SetBool(_tiredAnimName, false);

		IsRunning = false;
	}

	public void Update()
	{
		switch (_state)
		{
			case State.None:
				_lungeIndicatorTimer = LungeIndicatorTimer;
				_clawAnimIndicatorTimer = ClawAnimIndicatorDuration;
				_tiredTimer = Random.Range(2f, 4f);
				WolfBoss.LungeIndicator.SetActive(false);

				Animator.SetBool(_lungeAnimName, false);
				Animator.SetBool(_walkAnimName, false);
				Animator.SetBool(_clawAttackAnimName, false);
				Animator.SetBool(_tiredAnimName, false);

				Animator.SetBool(_idleAnimName, true);

				_state = State.Lunge;
				break;

			case State.Lunge:
				WolfBoss.LungeIndicator.SetActive(true);
				TurnToDirection();

				float scaleFactor = DistanceToPlayer / 10;

				WolfBoss.LungeIndicator.transform.localScale = 
					new Vector3(scaleFactor, 0.35f, 1);
				WolfBoss.LungeIndicator.transform.localPosition = 
					new Vector3(scaleFactor / 2 * -1, indicatorY, indicatorZ);

				_lungeIndicatorTimer -= Time.deltaTime;
				if (_lungeIndicatorTimer < 0)
				{
					Animator.SetBool(_lungeAnimName, true);
					Animator.SetBool(_idleAnimName, false);
					WolfBoss.LungeIndicator.SetActive(false);

					NavMeshAgent.isStopped = false;
					NavMeshAgent.speed = LungeSpeed;
					NavMeshAgent.SetDestination(Player.Instance.transform.position);

					_state = State.LungeEnd;
				}
				break;

			case State.LungeEnd:
				// boss reched to its lunge destination
				if (NavMeshAgent.remainingDistance < 1)
				{
					Animator.SetBool(_lungeAnimName, false);

					if (DistanceToPlayer <= StrikeDistance)
					{
						_state = State.StartAttack;
					} 
					else
					{
						_state = State.FastWalk;
					}
				}
				break;

			case State.Tired:
				_tiredTimer -= Time.deltaTime;
				if (_tiredTimer <= 0)
				{
					Animator.SetBool(_tiredAnimName, false);
					_state = State.None;
				}
				break;

			case State.FastWalk:
				NavMeshAgent.speed = FastSpeed;
				WalkTowardPlayer();
				break;

			case State.StartAttack:
				NavMeshAgent.isStopped = true;
				Animator.SetBool(_walkAnimName, false);
				TurnToDirection();

				Animator.SetBool(_clawAttackAnimName, true);
                AudioManager.Instance.Play("WolfClaw");
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
					_state = State.None;
				}
				break;
		}
	}

	public bool Stop()
	{
		if (_state != State.LungeEnd) return false;

        NavMeshAgent.isStopped = true;
		Animator.SetBool(_tiredAnimName, true);
        Animator.SetBool(_lungeAnimName, false);
		WolfBoss.LungeIndicator.SetActive(false);
		AudioManager.Instance.Play("Slam");

        _state = State.Tired;
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
		if (DistanceToPlayer <= StrikeDistance)
		{
			_state = State.StartAttack;
		}
	}
	private void TurnToDirection()
	{
		var dir = Player.Instance.transform.position - MyObject.transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		MyObject.transform.rotation = rotation;
	}

	public void AttackPlayer()
	{
		if (_state != State.LungeEnd) return;
		NavMeshAgent.isStopped = true;
		Animator.SetBool(_lungeAnimName, false);

		Player.Instance.TakeDamage(LungeDamage);
		_state = State.FastWalk;
	}

	public enum State
	{
		None,
		Lunge,
		LungeEnd,
		Tired,
		FastWalk,
		StartAttack,
		CurrentAttack,
		EndAttack
	}
}
