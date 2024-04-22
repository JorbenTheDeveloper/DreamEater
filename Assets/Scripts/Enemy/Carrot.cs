using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform[] shootPoints;
    public Sprite[] leafSprites; // Sprites from 0 to 4 leaves
    public Animator animator; // Animator for the grow-back animation
    public float rotationSpeed = 50f;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;

    private float nextShootTime;
    private int currentLeafCount;
    private SpriteRenderer spriteRenderer;
    private bool IsGrowing = false; // Flag to control growth state

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentLeafCount = leafSprites.Length - 1; // Assume the last sprite is the fullest leaf
        spriteRenderer.sprite = leafSprites[currentLeafCount]; // Set initial sprite
        if (animator == null) Debug.LogError("Animator not assigned.");
    }

    void Update()
    {
        RotateEnemy();
        ShootProjectiles();
    }

    void RotateEnemy()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void ShootProjectiles()
    {
        if (Time.time > nextShootTime && currentLeafCount > 0)
        {
            int index = shootPoints.Length - currentLeafCount; // Get the correct shoot point based on remaining leaves
            Transform shootPoint = shootPoints[index];
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null) rb.AddForce(shootPoint.up * projectileSpeed, ForceMode2D.Impulse);
            Destroy(projectile, 5f); // Destroy projectile after 5 seconds

            currentLeafCount--; // Decrease leaf count
            UpdateLeafSprite(); // Update sprite based on remaining leaves
            nextShootTime = Time.time + shootInterval;

            if (currentLeafCount == 0 && !IsGrowing) StartCoroutine(RegrowLeaves());
        }
    }

    void UpdateLeafSprite()
    {
        if (currentLeafCount >= 0 && currentLeafCount < leafSprites.Length)
        {
            animator.enabled = false;  // Disable the Animator when updating the sprite
            spriteRenderer.sprite = leafSprites[currentLeafCount];
        }
    }

    void EnableAnimator()
    {
        animator.enabled = true;
    }

    IEnumerator RegrowLeaves()
    {
        yield return new WaitForSeconds(2); // Cooldown period with no leaves
        IsGrowing = true;
        animator.SetBool("IsGrowing", true);  // Signal the animator to start the grow-back animation
        animator.enabled = true;  // Re-enable the Animator for the grow-back animation

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Wait for animation to complete
        currentLeafCount = leafSprites.Length - 1;  // Reset leaf count
        UpdateLeafSprite();  // Update sprite to full leaves
        animator.SetBool("IsGrowing", false);  // Reset the animation boolean
        IsGrowing = false;
    }
}
