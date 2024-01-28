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

    private NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
            if (Size >= player.Size)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                var oppositeDir = (transform.position - player.transform.position).normalized;
                agent.SetDestination(transform.position + oppositeDir);
            }
        }
        else
        {
            var nextPatrolPos = PatrollingPositions[patrolIndex].transform.position;
            agent.SetDestination(nextPatrolPos);

            if (Vector3.Distance(transform.position, nextPatrolPos) < 4)
            {
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

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, ChaseRange);
    }
}
