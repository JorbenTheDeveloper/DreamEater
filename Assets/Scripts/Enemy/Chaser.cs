using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;

public class Chaser : MonoBehaviour
{
    private Player player => Player.Instance;
    private Eatable eatable;

    public float ChaseRange = 20f;
    public float Speed = 3f;
    public bool AlwaysChase = false;
    private float DistanceToPlayer => Vector3.Distance(player.transform.position, transform.position);
    private float Size => eatable.Size;

    [Header("Patrolling")]
    public List<GameObject> PatrollingPositions;
    public bool PatrolRandomize = false;
    private int patrolIndex = 0;
    public float PatrolWaitDuration = 0f;
    private float patrolWaitSecondPassed = 0f;

    public float ChaseWaitDuration = 0f;
    private float chaseWaitSecondPassed = 0f;

    private Light2D light2D;
    private NavMeshAgent agent;
    private Animator animator;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponent<Animator>();
        eatable = GetComponent<Eatable>();
        light2D = GetComponent<Light2D>();
    }

    private void Start()
    {
        if (PatrolRandomize)
        {
            PatrollingPositions = PatrollingPositions.OrderBy(i => System.Guid.NewGuid()).ToList();
        }
        agent.speed = Speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (DistanceToPlayer <= ChaseRange)
        {
            patrolWaitSecondPassed = 0;
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);

            if (ShouldChase())
            {
                if (light2D != null) light2D.enabled = true;
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
                if (light2D != null) light2D.enabled = false;
                var oppositeDir = (transform.position - player.transform.position).normalized;
                agent.isStopped = false;
                animator.SetBool("IsWalking", true);
                agent.SetDestination(transform.position + oppositeDir);
                TurnToDirection();
            }
        }
        else
        {
            if (light2D != null)  light2D.enabled = false;

            // Only add an early return if PatrollingPositions is empty
            if (PatrollingPositions.Count == 0)
            {
                // If there are no patrolling positions, ensure the chaser doesn't move.
                agent.isStopped = true;
                animator.SetBool("IsWalking", false);
                return; // Exit early from the Update method
            }

            patrolWaitSecondPassed += Time.deltaTime;
            chaseWaitSecondPassed = 0;
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);
            if (patrolWaitSecondPassed >= PatrolWaitDuration)
            {
                agent.isStopped = false;
                animator.SetBool("IsWalking", true);

                var nextPatrolPos = PatrollingPositions[patrolIndex].transform.position;
                agent.SetDestination(nextPatrolPos);
                TurnToDirection();

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
    }

    private bool ShouldChase() => AlwaysChase || player.IsVulnerable || Size >= player.Size;

    private void TurnToDirection()
    {
        var dir = agent.steeringTarget - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, ChaseRange);
    }
    #endif
}
