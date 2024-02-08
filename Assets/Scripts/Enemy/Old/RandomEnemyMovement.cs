using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyMovement : MonoBehaviour
{
    private Vector2 currentMovementPoint;
    private float moveSpeed = 2f;
    private float changeDirectionTimer = 0f;
    private float timeBetweenDirectionChanges = 0.5f;
    private Vector3 tempScale;
    private bool chasePlayer;

    public Collider2D detectionArea;
    public Collider2D directCollisionArea; // New collider for direct collisions with the player
    public Transform player;

    private void Start()
    {
        SetRandomMovementTarget();
    }

    private void Update()
    {
        if (chasePlayer)
            MoveToPlayer();
        else
            MoveRandomly();

        HandleFacingDirection();
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

            // Assign the Player GameObject to the player variable
            player = other.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chasePlayer = false;
            SetRandomMovementTarget();
            player = null;
        }
    }

    // Handle direct collisions with the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Get the PlayerHealth script from the Player GameObject
            PlayerHealth playerHealth = collision.transform.parent.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Access the PlayerHealth script as needed
                // For example, you can call a method to damage the player:
                playerHealth.TakeDamage(1);
            }
            else
            {
                Debug.LogError("PlayerHealth script not found on the player.");
            }
        }

        if (collision.gameObject.CompareTag("acid"))
        {

            // Destroy the projectile
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
}
