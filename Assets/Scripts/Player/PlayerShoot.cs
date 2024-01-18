using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public int projectileDamage;

    public float shootInterval = 3f;
    private bool canShoot = true;
    private float shootTimer = 0;

    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }

    public bool ShootProjectile(float size, Transform playerTransform)
    {
        if (!canShoot) return false;
        shootTimer = shootInterval;

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Set the size of the projectile
        projectile.transform.localScale = new Vector3(size, size, 1.0f);

        // Get the rigidbody of the projectile
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        // Set the velocity of the projectile based on the player's rotation
        Vector2 projectileDirection = new Vector2(Mathf.Cos(playerTransform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(playerTransform.rotation.eulerAngles.z * Mathf.Deg2Rad));
        projectileRb.velocity = projectileDirection * projectileSpeed;

        // Destroy the projectile after 5 seconds
        Destroy(projectile, 5f);

        return true;
    }
}
