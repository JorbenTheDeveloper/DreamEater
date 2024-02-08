using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public int damageAmount = 1; // Adjust this value as needed

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Destroy the projectile when it hits the player
            DestroyProjectile();
        }
    }

    private void Update()
    {
        // You can add any additional behavior here if needed
    }

    private void Start()
    {
        // Start the coroutine to destroy the projectile after 3 seconds
        StartCoroutine(DestroyProjectileAfterDelay());
    }

    private void DestroyProjectile()
    {
        // Destroy the projectile
        Destroy(gameObject);
    }

    private IEnumerator DestroyProjectileAfterDelay()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Destroy the projectile after 3 seconds
        DestroyProjectile();
    }
}
