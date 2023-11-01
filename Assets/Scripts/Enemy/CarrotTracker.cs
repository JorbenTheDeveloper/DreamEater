using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotTracker : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public Transform[] projectileSpawnPoints; // Array of projectile spawn points
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    public float health = 4;
    public float maxHealth = 4;
    private bool rush = false;

    public FloatingHealthBar healthBar;
    public Transform detectionAreaTransform;

    private Collider2D detectionAreaCollider;
    private Transform playerTransform;

    private float nextShootTime;
    public float projectileShootInterval = 2f; // New variable for projectile shoot interval
    public float size = 1.0f;

    private void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not assigned. Please assign it in the Inspector.");
        }

        healthBar.gameObject.SetActive(false);
        healthBar.UpdateHealthBar(health, maxHealth);

        if (detectionAreaTransform == null)
        {
            Debug.LogError("DetectionAreaTransform not assigned. Please assign it in the Inspector.");
            return;
        }

        detectionAreaCollider = detectionAreaTransform.GetComponent<Collider2D>();

        if (detectionAreaCollider == null)
        {
            Debug.LogError("Collider2D not found on the DetectionAreaTransform. Please make sure it has a Collider2D component.");
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
        }

        if (projectileSpawnPoints.Length == 0)
        {
            Debug.LogError("No projectile spawn points assigned. Please assign them in the Inspector.");
        }
    }

    void Update()
    {
        if (IsPlayerInDetectionArea())
        {
            RotateEnemyToPlayer();
            ShootProjectiles();
        }

        rush = Input.GetMouseButton(0);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void RotateEnemyToPlayer()
    {
        Vector3 playerDirection = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void ShootProjectiles()
    {
        if (Time.time > nextShootTime)
        {
            foreach (Transform spawnPoint in projectileSpawnPoints)
            {
                GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

                if (projectileRb != null)
                {
                    // Set the velocity using rigidbody physics
                    projectileRb.velocity = (playerTransform.position - spawnPoint.position).normalized * projectileSpeed;

                    // Add a custom script to the projectile to handle its behavior on collision
                    projectile.AddComponent<ProjectileScript>();
                }
            }

            nextShootTime = Time.time + projectileShootInterval; // Use the new interval variable
        }
    }

    IEnumerator DestroyProjectileAfterDelay(GameObject projectile)
    {
        yield return new WaitForSeconds(3f);

        // Destroy the projectile after 3 seconds
        if (projectile != null)
        {
            Destroy(projectile);
        }
    }

    bool IsPlayerInDetectionArea()
    {
        if (detectionAreaCollider == null)
        {
            Debug.LogError("Collider2D not found on the DetectionAreaTransform. Please make sure it has a Collider2D component.");
            return false;
        }

        List<Collider2D> colliders = new List<Collider2D>();
        int colliderCount = detectionAreaCollider.OverlapCollider(new ContactFilter2D(), colliders);

        for (int i = 0; i < colliderCount; i++)
        {
            if (colliders[i].CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        healthBar.gameObject.SetActive(true);

        if (rush)
        {
            health -= 1;
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D projectileRb = collision.collider.gameObject.GetComponent<Rigidbody2D>();

            if (projectileRb != null)
            {
                // Stop the projectile's movement
                projectileRb.velocity = Vector2.zero;

                // Destroy the projectile when it hits the player
                Destroy(collision.collider.gameObject);
            }
        }
    }
}
