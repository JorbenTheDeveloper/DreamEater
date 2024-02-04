using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Chaser : MonoBehaviour
{
    private Player player => Player.Instance;
    public Eatable eatable;

    public float ChaseRange = 20f;
    public float Speed = 3f;

    private float DistanceToPlayer => Vector3.Distance(player.transform.position, transform.position);
    private float Size => eatable.Size;

    public List<GameObject> PatrollingPositions;
    public bool PatrolRandomize = false;
    private int patrolIndex = 0;
    public float PatrolWaitDuration = 0f;
    private float patrolWaitSecondPassed = 0f;

    public float ChaseWaitDuration = 0f;
    private float chaseWaitSecondPassed = 0f;

    private NavMeshAgent agent;
    private Animator animator;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (PatrolRandomize)
        {
            PatrollingPositions = PatrollingPositions.OrderBy(i => System.Guid.NewGuid()).ToList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DistanceToPlayer <= ChaseRange)
        {
            patrolWaitSecondPassed = 0;
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);

            if (Size >= player.Size)
            {
                chaseWaitSecondPassed += Time.deltaTime;
                if (chaseWaitSecondPassed >= ChaseWaitDuration)
                {
                    agent.isStopped = false;
                    animator.SetBool("IsWalking", true);
                    agent.SetDestination(player.transform.position);
                    TurnToDirection();
                }
            }
            else
            {
                var oppositeDir = (transform.position - player.transform.position).normalized;
                agent.isStopped = false;
                animator.SetBool("IsWalking", true);
                agent.SetDestination(transform.position + oppositeDir);
                TurnToDirection();
            }
        }
        else
        {
            patrolWaitSecondPassed += Time.deltaTime;
            chaseWaitSecondPassed = 0;
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);
            var nextPatrolPos = PatrollingPositions[patrolIndex].transform.position;
            
            if (patrolWaitSecondPassed >= PatrolWaitDuration)
            {
                agent.isStopped = false;
                animator.SetBool("IsWalking", true);
                agent.SetDestination(nextPatrolPos);
                TurnToDirection();
            }

            if (Vector3.Distance(transform.position, nextPatrolPos) < 4)
            {
                patrolWaitSecondPassed = 0;
                patrolIndex++;
                if (patrolIndex >= PatrollingPositions.Count)
                {
                    patrolIndex = 0;
                    if (PatrolRandomize)
                    {
                        PatrollingPositions = PatrollingPositions.OrderBy(i => System.Guid.NewGuid()).ToList();
                    }
                }
            }
        }
    }

    private void TurnToDirection()
    {
        var dir = agent.steeringTarget - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, ChaseRange);
    }
}
