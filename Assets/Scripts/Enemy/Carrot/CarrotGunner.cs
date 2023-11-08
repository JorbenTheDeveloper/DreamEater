using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotGunner : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public Transform projectileSpawnPoint;
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
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            if (projectileRb != null)
            {
                Vector3 playerDirection = (playerTransform.position - projectileSpawnPoint.position).normalized;
                projectileRb.AddForce(playerDirection * projectileSpeed, ForceMode2D.Impulse);
            }

            Destroy(projectile, 5f);
            nextShootTime = Time.time + projectileShootInterval; // Use the new interval variable
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
    }
}
