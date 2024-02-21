using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public int projectileDamage;

    public float shootInterval = 3f;
    private bool canShoot = true;
    private float shootTimer = 0;

    public Image abilityImage;
    public Sprite canShootSprite;
    public Sprite cannotShootSprite;

    public TextMeshProUGUI shootIntervalText;
    private GameObject currentProjectile = null;

    public float sizeReduction = 0.1f;

    void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            canShoot = true;
            abilityImage.sprite = canShootSprite;
            shootIntervalText.text = "";
        }
        else
        {
            canShoot = false;
            abilityImage.sprite = cannotShootSprite;
            shootIntervalText.text = shootTimer.ToString("F1");
        }

        // Check for shooting input within the Update method
        if (Input.GetKeyDown(KeyCode.Space) && canShoot && Player.Instance.Size > 0.5f)
        {
            if (ShootProjectile(Player.Instance.Size, transform))
            {
                Player.Instance.Shrink(sizeReduction); // Call Shrink method on Player
            }
        }
    }

    public bool ShootProjectile(float size, Transform playerTransform)
    {
        if (!canShoot || Player.Instance.Size < 0.5f) return false; // Ensure the player's size is considered

        shootTimer = shootInterval;

        currentProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        currentProjectile.transform.localScale = new Vector3(size, size, 1.0f);
        Rigidbody2D projectileRb = currentProjectile.GetComponent<Rigidbody2D>();

        Vector2 projectileDirection = new Vector2(Mathf.Cos(playerTransform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(playerTransform.rotation.eulerAngles.z * Mathf.Deg2Rad));
        projectileRb.velocity = projectileDirection * projectileSpeed;

        // Optionally, clear the current projectile when it's destroyed.
        Destroy(currentProjectile, 5f); // Adjust time as needed
        currentProjectile = null; // Reset current projectile

        return true;
    }
}

