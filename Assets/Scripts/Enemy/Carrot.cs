using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float shootInterval = 2f;
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    public float health = 4;
    public float maxHealth = 4;
    private bool rush = false;

    // Directly assign the healthBar in the Inspector
    public FloatingHealthBar healthBar;

    private float nextShootTime;

    private void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not assigned. Please assign it in the Inspector.");
        }

        healthBar.gameObject.SetActive(false);
        healthBar.UpdateHealthBar(health, maxHealth);
    }
    void Update()
    {
        // Rotate the enemy in place
        RotateEnemy();

        // Shoot projectiles at regular intervals
        ShootProjectiles();

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
            // Instantiate a projectile at the spawn point
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            // Get the Rigidbody2D component
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            // Check if a Rigidbody2D is attached to the projectile
            if (projectileRb != null)
            {
                // Apply force to move the projectile
                projectileRb.AddForce(projectileSpawnPoint.up * projectileSpeed, ForceMode2D.Impulse);
            }

            // Update the next shoot time
            nextShootTime = Time.time + shootInterval;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        healthBar.gameObject.SetActive(true);

        if (rush == true)
        {
            health -= 1;
            healthBar.UpdateHealthBar(health, maxHealth);
        }
    }
}
