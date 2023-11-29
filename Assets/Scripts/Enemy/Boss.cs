using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float health = 4;
    public float maxHealth = 4;
    public float size = 1.0f;
    public GameObject objectToSpawn;

    private bool isCreated = false;
    private bool rush = false;

    // Directly assign the healthBar in the Inspector
    public FloatingHealthBar healthBar;

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

    public GameObject minionPrefab;  // Assign the minion prefab in the Inspector
    public int numMinionsToSpawn = 4;

    private void Start()
    {
        // Set the player variable by finding the GameObject with the "Player" tag
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player GameObject not found. Make sure the player has the 'Player' tag.");
        }

        // Ensure healthBar is assigned in the Inspector
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not assigned. Please assign it in the Inspector.");
        }

        healthBar.gameObject.SetActive(false);
        healthBar.UpdateHealthBar(health, maxHealth);

        SetMovementPointTarget();
    }

    void Update()
    {
        if (health <= 0 && !isCreated)
        {
            SpawnObjects();
        }

        // Set the rush variable based on player input
        rush = Input.GetMouseButton(0);

        if (chasePlayer)
        {
            MoveToPlayer();
        }
        else
        {
            MoveToTarget();
        }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            moveSpeed = 2f;
            chasePlayer = true;
            currentMovementPoint = other.gameObject.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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

    void SpawnMinions()
    {
        for (int i = 0; i < numMinionsToSpawn; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 2.0f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            GameObject spawnedMinion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            spawnedMinion.transform.parent = transform;  // Set the boss as the parent of the spawned minion
        }
    }

    void SpawnObjects()
    {
        // Disable the renderer to hide the BossEnemy
        GetComponent<SpriteRenderer>().enabled = false;

        // Disable the health bar
        healthBar.gameObject.SetActive(false);

        // Disable the Rigidbody and Collider
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();
        rb.isKinematic = true;  // Set isKinematic to true to disable physics simulation
        col.enabled = false;

        // Spawn minions
        SpawnMinions();

        // ... (rest of the existing code)

        // Schedule a method to switch the tag back, enable the renderer, and the health bar
        Invoke("SwitchTagBackAndEnableRenderer", 0.5f);

        // Move the Destroy call here or after the loop to avoid issues
        isCreated = true;
        Destroy(gameObject, 0.5f); // Destroy after 2 seconds
    }

    void SwitchTagBackAndEnableRenderer()
    {
        // Enable the renderer to show the BossEnemy
        GetComponent<SpriteRenderer>().enabled = true;

        // Enable the health bar
        healthBar.gameObject.SetActive(true);

        // Enable the Rigidbody and Collider
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();
        rb.isKinematic = false;  // Set isKinematic back to false to enable physics simulation
        col.enabled = true;

        // Switch the tag back
        SwitchTagBack();
    }

    void SwitchTagBack()
    {
        GameObject[] notEnemyObjects = GameObject.FindGameObjectsWithTag("NotEnemy");

        foreach (GameObject notEnemyObject in notEnemyObjects)
        {
            notEnemyObject.tag = "Enemy";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ... (existing OnCollisionEnter2D code)

        if (rush)
        {
            // If it's a rush, update health and health bar
            healthBar.gameObject.SetActive(true);
            health -= 1;
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        if (collision.gameObject.CompareTag("acid"))
        {
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}
