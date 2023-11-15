using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement and Control")]
    // Movement and Control Variables
    public float size = 1.0f;
    private Camera mainCamera;
    private Transform m_transform;
    private bool rush = false;
    private bool isRecharging = false;
    private float rechargeTimer = 0f;
    public float maxSpeed = 10f;
    public Collision2D body;

    [Header("Score")]
    // Score Variables
    public int score = 0;
    public TMP_Text score_text;

    [Header("Stamina")]
    // Stamina Variables
    public Image StaminaBar;
    public float Stamina;
    public float maxStamina;
    public float rushCost;
    public float chargeRate;

    [Header("Knockback")]
    // Knockback Variables
    public float knockbackForce = 5f;

    [Header("Coroutine variable")]
    // Coroutine Variables
    private Coroutine recharge;
    public TMP_Text rechargeTimerText;

    [Header("External References")]
    // External References
    public PlayerHealth playerHealth;
    public int projectileDamage;

    [Header("Scale")]
    // Scale Increase Variables
    public float scaleIncreaseAmount = 0.1f; // Adjust this value as needed
    public int pointsToIncreaseScale = 50; // Adjust this value as needed
    public TMP_Text sizeText;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private float initialCameraSize = 5f;
    public SizeScript sizeScript;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public float projectileSizeReduction = 0.1f;

    void Start()
    {
        mainCamera = Camera.main;
        m_transform = transform;

        sizeText.text = "Size: " + sizeScript.scale.ToString("F1");

        if (cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.m_Lens.OrthographicSize = initialCameraSize;
        }
    }

    void Update()
    {
        rechargeTimerText.text = rechargeTimer.ToString("F1");

        // Stamina Logic
        if (!isRecharging)
        {
            if (Stamina > 0)
            {
                Stamina += chargeRate * Time.deltaTime;

                if (Stamina > maxStamina)
                {
                    Stamina = maxStamina;
                    StaminaBar.fillAmount = Stamina / maxStamina;

                }
                StaminaBar.fillAmount = Stamina / maxStamina;
            }
            else
            {
                isRecharging = true;
                rechargeTimer = 2f;
            }
        }
        else
        {
            rechargeTimer -= Time.deltaTime;

            if (rechargeTimer <= 0f)
            {
                isRecharging = false;
                Stamina = maxStamina;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ShootProjectile();
        }

        // Movement and Control Logic
        FollowMousePositionDelayed(maxSpeed);
        LAMouse();

        // Rush Logic
        if (!isRecharging && Input.GetMouseButton(0) && Stamina > 0)
        {
            maxSpeed = 10f;
            rush = true;

            Stamina -= rushCost;
            StaminaBar.fillAmount = Stamina / maxStamina;

            if (Stamina < 0)
            {
                Stamina = 0;
                rush = false;
            }
        }
        else
        {
            maxSpeed = 5f;
            rush = false;
        }

        // Score Display
        score_text.SetText(score.ToString());

        if (score >= pointsToIncreaseScale)
        {
            int growthCount = score / pointsToIncreaseScale; // Calculate how many times to grow
            for (int i = 0; i < growthCount; i++)
            {
                IncreaseScale();
            }

            score %= pointsToIncreaseScale; // Update the score without deducting for the growth
        }

        // Update the size label
        sizeText.text = "Size: " + sizeScript.scale.ToString("F1");

        if (cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.m_Lens.OrthographicSize = initialCameraSize + size;
        }

        sizeScript.scale = size;

        // Update the score text as a fraction of 50
        score_text.SetText(string.Format("{0}/{1}", score, pointsToIncreaseScale));
    }

    private void FollowMousePositionDelayed(float maxSpeed)
    {
        if (!IsMouseOverPlayer())
        {
            transform.position = Vector2.MoveTowards(transform.position, GetWorldPositionFromMouse(),
                maxSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private void LAMouse()
    {
        Vector2 direction = GetWorldPositionFromMouse() - m_transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        m_transform.rotation = rotation;
    }

    private bool IsMouseOverPlayer()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        float distance = Vector2.Distance(transform.position, new Vector2(mouseWorldPosition.x, mouseWorldPosition.y));

        float mouseOverThreshold = 1.0f;

        return distance < mouseOverThreshold;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SizeScript sizeScript = collision.gameObject.GetComponent<SizeScript>();

        if (rush && sizeScript != null && sizeScript.scale <= size)
        {
            // Check if the collision object has a health bar
            FloatingHealthBar enemyHealthBar = collision.gameObject.GetComponent<FloatingHealthBar>();

            if (enemyHealthBar != null)
            {
                // Destroy the enemy immediately
                Destroy(collision.gameObject);
                sizeScript.DestroyObject(); // Update player's score and destroy the object
                return; // Don't execute the other checks if the object is eaten
            }

            Debug.Log("*Nom nom*");
            sizeScript.DestroyObject(); // Update player's score and destroy the object
            return; // Don't execute the other checks if the object is eaten
        }

        if (collision.gameObject.CompareTag("Enemy") && rush)
        {
            //score += 5;
            Knockback(collision.transform);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Rock") && rush)
        {
            score += 1;
            Knockback(collision.transform);
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene("Win");
        }

        if (collision.gameObject.CompareTag("Barrier"))
        {
            // Destroy the barrier GameObject
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Object") && rush)
        {
            // Destroy the barrier GameObject
            //score += 1;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("hit");
            playerHealth.TakeDamage(projectileDamage);
        }

        if (collision.gameObject.CompareTag("Score"))
        {
            score += 25;
        }
    }

    private void Knockback(Transform targetTransform)
    {
        Vector2 knockbackDirection = (targetTransform.position - transform.position).normalized;
        Rigidbody2D targetRigidbody = targetTransform.GetComponent<Rigidbody2D>();

        // Ensure that the target has a Rigidbody2D component
        if (targetRigidbody != null)
        {
            StartCoroutine(KnockbackCoroutine(targetRigidbody, knockbackDirection));
        }
    }

    private IEnumerator KnockbackCoroutine(Rigidbody2D targetRigidbody, Vector2 knockbackDirection)
    {
        float knockbackDuration = 0.5f; // Adjust this duration as needed
        float slowdownFactor = 0.4f; // Adjust this factor to control slowdown

        float timer = 0f;

        while (timer < knockbackDuration && targetRigidbody != null) // Add null check here
        {
            timer += Time.fixedDeltaTime;

            if (targetRigidbody != null) // Double-check for safety
            {
                targetRigidbody.velocity = knockbackDirection * knockbackForce;
            }

            yield return new WaitForFixedUpdate();
        }

        // Double-check for safety
        if (targetRigidbody != null)
        {
            // Slow down before stopping
            while (targetRigidbody.velocity.magnitude > 0.01f)
            {
                targetRigidbody.velocity *= slowdownFactor;
                yield return new WaitForFixedUpdate();
            }

            targetRigidbody.velocity = Vector2.zero;
        }
    }

    private void IncreaseScale()
    {
        size += scaleIncreaseAmount;
        transform.localScale = new Vector3(size, size, 1.0f);
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public float Size
    {
        get { return size; }
    }

    private void ShootProjectile()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Set the size of the projectile
        projectile.transform.localScale = new Vector3(size, size, 1.0f);

        // Get the rigidbody of the projectile
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        // Set the velocity of the projectile based on the player's rotation
        Vector2 projectileDirection = new Vector2(Mathf.Cos(m_transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(m_transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
        projectileRb.velocity = projectileDirection * projectileSpeed;

        // Destroy the projectile after 5 seconds
        Destroy(projectile, 5f);

        // Reduce the player's size
        size -= projectileSizeReduction;
        transform.localScale = new Vector3(size, size, 1.0f);
    }

}

