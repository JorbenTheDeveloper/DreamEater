using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : MonoBehaviour
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

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    public int damage = 5;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

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

    void SetMovementPointTarget()
    {
        int index1 = Random.Range(0, movementPoints.Length);
        int index2 = Random.Range(0, movementPoints.Length);
        currentMovementPointIndex = (index1 != previousMovementPointIndex) ? index1 : index2;
        previousMovementPointIndex = currentMovementPointIndex;
        currentMovementPoint = movementPoints[currentMovementPointIndex].position;
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentMovementPoint, Time.deltaTime * moveSpeed);

        if ((Vector2.Distance(transform.position, currentMovementPoint)) < 0.01f)
        {
            SetMovementPointTarget();
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

    void SpawnObjects()
    {
        sr.enabled = false;
        healthBar.gameObject.SetActive(false);
        rb.isKinematic = true;
        col.enabled = false;

        for (int i = 0; i < 4; i++)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
            spawnedObject.tag = "NotEnemy";
        }

        Invoke("SwitchTagBackAndEnableRenderer", 0.5f);
        isCreated = true;
        Destroy(gameObject, 0.5f);
    }

    void SwitchTagBackAndEnableRenderer()
    {
        sr.enabled = true;
        healthBar.gameObject.SetActive(true);
        rb.isKinematic = false;
        col.enabled = true;
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
        Debug.Log("Collision detected with: " + collision.gameObject.tag);

        if (collision.gameObject.transform.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.transform.parent.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            else
            {
                Debug.LogError("PlayerHealth script not found on the player.");
            }
        }

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
