using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float size = 1.0f;

    [SerializeField]
    private Transform[] movementPoints;
    

    private Vector2 currentMovementPoint;

    private int currentMovementPointIndex, previousMovementPointIndex;

    [SerializeField]
    private float moveSpeed = 2f;

    private Vector3 tempScale;
    private bool chasePlayer;

    public Collider2D detectionArea;
    public Transform player;

    private void Start()
    {
        SetMovementPointTarget();
    }

    private void Update()
    {
        if (chasePlayer)
            MoveToPlayer();
        else
            MoveToTarget();

        HandleFacingDirection();
    }

    void MoveToTarget()
    {
        transform.position =
            Vector2.MoveTowards(transform.position, currentMovementPoint, Time.deltaTime * moveSpeed);

        if (Vector2.Distance(transform.position, currentMovementPoint) < 0.1f)
        {
            SetMovementPointTarget();
        }

        
    }

    void SetMovementPointTarget()
    {
        while (true)
        {

            currentMovementPointIndex = Random.Range(0, movementPoints.Length);

            if (currentMovementPointIndex != previousMovementPointIndex)
            {
                previousMovementPointIndex = currentMovementPointIndex;
                currentMovementPoint = movementPoints[currentMovementPointIndex].position;
                break;
            }
        }
    }

    void HandleFacingDirection()
    {
        tempScale = transform.localScale;

        if (transform.position.x > currentMovementPoint.x)
        {
            tempScale.x = Mathf.Abs(tempScale.x);
        }
        else if (transform.position.x < currentMovementPoint.x)
        {
            tempScale.x = -Mathf.Abs(tempScale.x);
        }

        transform.localScale = tempScale;
    }

    private void OnTriggerEnter2D(Collider2D detectionArea)
    {
        if (detectionArea.CompareTag("Player"))
        {
            moveSpeed = 2f;
            chasePlayer = true;
            currentMovementPoint = detectionArea.gameObject.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D detectionArea)
    {
        if (detectionArea.CompareTag("Player"))
        {
            chasePlayer = false;
            SetMovementPointTarget();
        }
    }

    void MoveToPlayer()
    {
        
        transform.position =
            Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * moveSpeed);
        
        
    }
}
