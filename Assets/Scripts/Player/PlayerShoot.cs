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
            ShootProjectile(Player.Instance.Size, transform);
        }
    }

    public void ShootProjectile(float size, Transform playerTransform)
    {
        shootTimer = shootInterval;

        var currentProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        currentProjectile.transform.localScale = new Vector3(size, size, 1.0f);
        currentProjectile.GetComponent<Eatable>().Size = size;
        Rigidbody2D projectileRb = currentProjectile.GetComponent<Rigidbody2D>();

        Vector2 projectileDirection = new Vector2(Mathf.Cos(playerTransform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(playerTransform.rotation.eulerAngles.z * Mathf.Deg2Rad));
        projectileRb.velocity = projectileDirection * projectileSpeed;

        // shrink the player
        Player.Instance.Shrink(sizeReduction);
    }
}

