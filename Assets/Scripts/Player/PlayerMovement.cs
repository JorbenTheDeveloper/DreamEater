using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float size = 1.0f;
    private Camera mainCamera;
    private Transform m_transform;
    private bool rush = false;
    private bool isRecharging = false;
    private float rechargeTimer = 0f;

    public int score = 0;
    public TMP_Text score_text;

    public float maxSpeed = 10f;
    public Collision2D body;

    public Image StaminaBar;
    public float Stamina;
    public float maxStamina;
    public float rushCost;
    public float chargeRate;

    public float knockbackForce = 5f;

    private Coroutine recharge;

    public PlayerHealth playerHealth;
    public int projectileDamage;

    void Start()
    {
        mainCamera = Camera.main;
        m_transform = this.transform;
    }

    void Update()
    {
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
                rechargeTimer = 3f;
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

        FollowMousePositionDelayed(maxSpeed);
        LAMouse();

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

        score_text.SetText(score.ToString());
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

        if (sizeScript != null && sizeScript.size <= size && rush)
        {
            // Check if the collision object has a health bar
            FloatingHealthBar enemyHealthBar = collision.gameObject.GetComponent<FloatingHealthBar>();

            if (enemyHealthBar != null)
            {
                // Destroy the enemy immediately
                Destroy(collision.gameObject);
                score += 10; // Adjust the score as needed
                return; // Don't execute the other checks if the object is eaten
            }

            Debug.Log("*Nom nom*");
            score += 10; // Adjust the score as needed
            Destroy(collision.gameObject);
            return; // Don't execute the other checks if the object is eaten
        }

        if (collision.gameObject.CompareTag("Enemy") && rush)
        {
            
            score += 5;
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

        if (collision.gameObject.CompareTag("Object"))
        {
            // Destroy the barrier GameObject
            score += 1;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("hit");
            playerHealth.TakeDamage(projectileDamage);
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
}

