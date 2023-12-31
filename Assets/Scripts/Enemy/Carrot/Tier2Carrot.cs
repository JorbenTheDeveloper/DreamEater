using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tier2Carrot : MonoBehaviour
{
    public float size = 1.0f;
    public float rotationSpeed = 50f;
    public float shootInterval = 2f;
    public Transform[] projectileSpawnPoints; // Array to hold multiple spawn points
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    public float health = 4;
    public float maxHealth = 4;
    private bool rush = false;

    // Directly assign the healthBar in the Inspector
    public FloatingHealthBar healthBar;

    // New variable for the detection area transform
    public Transform detectionAreaTransform;

    private Collider2D detectionAreaCollider;

    private float nextShootTime;

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

        // Find the Collider2D component in the child (detection area)
        detectionAreaCollider = detectionAreaTransform.GetComponent<Collider2D>();

        if (detectionAreaCollider == null)
        {
            Debug.LogError("Collider2D not found on the DetectionAreaTransform. Please make sure it has a Collider2D component.");
        }
    }

    void Update()
    {
        // Check if the player is within the detection area
        if (IsPlayerInDetectionArea())
        {
            // Rotate the enemy in place
            RotateEnemy();

            // Shoot projectiles at regular intervals from multiple spawn points
            ShootProjectiles();
        }

        rush = Input.GetMouseButton(0);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void RotateEnemy()
    {
        // Use Rotate to rotate the enemy around its own Z-axis (for 2D)
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void ShootProjectiles()
    {
        if (Time.time > nextShootTime)
        {
            foreach (Transform spawnPoint in projectileSpawnPoints)
            {
                // Instantiate a projectile at each spawn point
                GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

                // Get the Rigidbody2D component
                Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

                // Check if a Rigidbody2D is attached to the projectile
                if (projectileRb != null)
                {
                    // Apply force to move the projectile
                    projectileRb.AddForce(spawnPoint.up * projectileSpeed, ForceMode2D.Impulse);
                }

                // Destroy the projectile after 5 seconds
                Destroy(projectile, 5f);
            }

            // Update the next shoot time
            nextShootTime = Time.time + shootInterval;
        }
    }

    bool IsPlayerInDetectionArea()
    {
        // Check if there's a Collider2D assigned for detection
        if (detectionAreaCollider == null)
        {
            Debug.LogError("Collider2D not found on the DetectionAreaTransform. Please make sure it has a Collider2D component.");
            return false;
        }

        // Use a list to collect overlapping colliders
        List<Collider2D> colliders = new List<Collider2D>();

        // Use Physics2D.OverlapCollider to find colliders in the detection area
        int colliderCount = detectionAreaCollider.OverlapCollider(new ContactFilter2D(), colliders);

        // Check if any of the overlapping colliders have the "Player" tag
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
        // Assuming the player's GameObject has a SizeScript component
        GameObject player = collision.gameObject;
        SizeScript playerSizeScript = player.GetComponent<SizeScript>();

        if (playerSizeScript != null)
        {
            Debug.Log($"Player Size: {playerSizeScript.scale}, Carrot Size: {size}");
        }

        if (playerSizeScript != null && playerSizeScript.scale >= size && rush)
        {
            // The player is the same size or larger, destroy the CarrotEnemy
            Debug.Log("Destroyed Carrot");
            Destroy(gameObject);
        }
        else if (rush)
        {
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
