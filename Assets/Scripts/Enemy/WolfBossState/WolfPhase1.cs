using UnityEngine;
using UnityEngine.AI;

public class WolfPhase1 : IWolfPhase
{
    public Animator Animator { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public GameObject MyObject;
    public int StrikeDistance = 3;
    public int Speed;

    private float DistanceToPlayer => Vector3.Distance(MyObject.transform.position, Player.Instance.transform.position);

    private string WalkAnimName = "IsWalk";

    public WolfPhase1(GameObject myObject, int speed)
    {
        MyObject = myObject;
        Speed = speed;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
        Animator.SetBool(WalkAnimName, false);
    }

    public void Update()
    {
        // should Attack
        if (DistanceToPlayer <= StrikeDistance)
        {
            Animator.SetBool(WalkAnimName, false);

            // TODO: show claw animation on Wolf
            // TODO: init a areal claw animation

        }
        else
        {
            Animator.SetBool(WalkAnimName, true);
            NavMeshAgent.SetDestination(Player.Instance.transform.position);
        }
    }
}
