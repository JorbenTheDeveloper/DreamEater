using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WolfPhaseLunge : IWolfPhase
{
	public Animator Animator { get; set; }
	public NavMeshAgent NavMeshAgent { get; set; }
	public WolfBoss WolfBoss { get; set; }

	public GameObject MyObject => WolfBoss.gameObject;

	private State _state = State.None;
	private float _lungeIndicatorTimer;

	private string _walkAnimName = "IsWalk";
	private string _lungeAnimName = "IsLunge";
	private string _clawAttackAnimName = "IsClaw";
	private string _tiredAnimName = "IsTired";

	// below 75 hp
	// lunge at the player
	// - claw attack

	// lunge first - indicator
	// if there is an object that it clashes with, it destroyes the object and goes into tired state
	// if no object, and player is in the way, player get damaged
	// claw attack once. If not close enough, move fast towards the player

	float indicatorX;
	float indicatorY;
	float indicatorZ;
	Vector3 targetLungePos;

	public void Enter()
	{
		WolfBoss.LungeIndicator.SetActive(true);
		indicatorX = WolfBoss.LungeIndicator.transform.localPosition.x;
		indicatorY = WolfBoss.LungeIndicator.transform.localPosition.y;
		indicatorZ = WolfBoss.LungeIndicator.transform.localPosition.z;
	}

	public void Exit()
	{
		WolfBoss.LungeIndicator.SetActive(false);
	}


	public void Update()
	{
		switch (_state)
		{
			case State.None:
				_lungeIndicatorTimer = 3;
				Animator.SetBool(_lungeAnimName, false);

				_state = State.Lunge;
				break;
			case State.Lunge:
				WolfBoss.LungeIndicator.SetActive(true);
				TurnToDirection();

				var dis = Vector3.Distance(MyObject.transform.position, Player.Instance.transform.position);
				float scaleFactor = dis / 10;

				WolfBoss.LungeIndicator.transform.localScale = 
					new Vector3(scaleFactor, 0.35f, 1);
				WolfBoss.LungeIndicator.transform.localPosition = 
					new Vector3(scaleFactor / 2 * -1, indicatorY, indicatorZ);

				_lungeIndicatorTimer -= Time.deltaTime;
				if (_lungeIndicatorTimer < 0)
				{
					Animator.SetBool(_lungeAnimName, true);
					WolfBoss.LungeIndicator.SetActive(false);

					NavMeshAgent.isStopped = false;
					NavMeshAgent.SetDestination(Player.Instance.transform.position);

					_state = State.LungeEnd;
				}
				break;
			case State.LungeEnd:
				if (NavMeshAgent.remainingDistance < 1)
				{
					Animator.SetBool(_lungeAnimName, false);
					_state = State.Tired;
				}
				break;
			case State.Tired:
				/// stuff....
				_state = State.None;
				break;
			case State.FastWalk: break;
			case State.StartAttack: break;
			case State.CurrentAttack: break;
			case State.EndAttack: break;
		}
	}

	private void TurnToDirection()
	{
		var dir = Player.Instance.transform.position - MyObject.transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		MyObject.transform.rotation = rotation;
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
