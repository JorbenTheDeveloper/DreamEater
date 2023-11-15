using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2 currentMovementPoint;
    private float moveSpeed = 2f;
    private float changeDirectionTimer = 0f;
    private float timeBetweenDirectionChanges = 0.5f;

    private float lungeSpeedMultiplier = 3f; // Adjust the speed multiplier as needed
    private float lungeCooldown = 10f; // Cooldown for the lunge
    private float lungeDuration = 2f; // Adjust the duration of the lunge
    private float lungeCooldownTimer = 0f;
    private bool isLunging = false;

    private Vector3 tempScale;
    private bool chasePlayer;
    public bool useMovementPoints = true; // Added variable to switch between random and movement points

    public Collider2D detectionArea;
    public Collider2D directCollisionArea; // New collider for direct collisions with the player
    public Transform player;
    public Transform[] movementPoints;

    private void Start()
    {
        if (useMovementPoints)
            SetMovementPointTarget();
        else
            SetRandomMovementTarget();
    }

    private void Update()
    {
        if (chasePlayer)
        {
            if (!isLunging)
            {
                MoveToPlayer();

                // Check if the player is within lunge range
                float lungeRange = 2.0f; // Adjust the range as needed
                if (Vector2.Distance(transform.position, player.position) < lungeRange)
                {
                    if (Time.time >= lungeCooldownTimer)
                    {
                        StartCoroutine(Lunge());
                    }
                }
            }
        }
        else
        {
            if (useMovementPoints)
                MoveToTarget();
            else
                MoveRandomly();
        }

        HandleFacingDirection();
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentMovementPoint, Time.deltaTime * moveSpeed);

        if (Vector2.Distance(transform.position, currentMovementPoint) < 0.1f)
        {
            SetMovementPointTarget();
        }
    }

    void MoveRandomly()
    {
        if (Time.time >= changeDirectionTimer)
        {
            SetRandomMovementTarget();
            changeDirectionTimer = Time.time + timeBetweenDirectionChanges;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentMovementPoint, Time.deltaTime * moveSpeed);
    }

    void SetMovementPointTarget()
    {
        currentMovementPoint = movementPoints[Random.Range(0, movementPoints.Length)].position;
    }

    void SetRandomMovementTarget()
    {
        currentMovementPoint = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            moveSpeed = 2f;
            chasePlayer = true;
            currentMovementPoint = other.gameObject.transform.position;
            player = other.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chasePlayer = false;

            if (useMovementPoints)
                SetMovementPointTarget();
            else
                SetRandomMovementTarget();

            player = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            else
            {
                Debug.LogError("PlayerHealth script not found on the player.");
            }
        }

        if (collision.gameObject.CompareTag("acid"))
        {
            Destroy(gameObject);
        }
    }

    void MoveToPlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * moveSpeed);
        }
    }

    IEnumerator Lunge()
    {
        isLunging = true;

        // Store the original move speed before the lunge
        float originalMoveSpeed = moveSpeed;

        // Speed up for the lunge
        moveSpeed *= lungeSpeedMultiplier;

        // Wait for the lunge duration
        yield return new WaitForSeconds(lungeDuration);

        // Slow back down to the original speed
        moveSpeed = originalMoveSpeed;

        // Reset lunge cooldown timer
        lungeCooldownTimer = Time.time + lungeCooldown;

        isLunging = false;
    }
}
