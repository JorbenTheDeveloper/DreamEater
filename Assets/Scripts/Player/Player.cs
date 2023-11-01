using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Camera mainCamera;
    private Transform m_transform;
    private bool rush = false;
    private bool isRecharging = false; // Added flag to track if recharging is in progress
    private float rechargeTimer = 0f;

    private bool isAttaching = false; // Flag to track if attaching to enemy is in progress
    private float attachTimer = 0f;
    private GameObject attachedEnemy; // Reference to the attached enemy

    public int score = 0;
    public TMP_Text score_text;

    public float maxSpeed = 10f;
    public float health;
    public Collision2D body;

    public Image StaminaBar;
    public float Stamina;
    public float maxStamina;
    public float rushCost;
    public float chargeRate;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        m_transform = this.transform;
    }

    // Update is called once per frame
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
                // Start recharging if Stamina is 0
                isRecharging = true;
                rechargeTimer = 3f; // Set the recharge time
            }
        }
        else
        {
            // Countdown the recharge timer
            rechargeTimer -= Time.deltaTime;

            if (rechargeTimer <= 0f)
            {
                isRecharging = false;

                // Reset stamina to avoid immediate recharging
                Stamina = maxStamina; // Reset to maxStamina so that it starts regenerating
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

        // Check if the player presses "E" and is not currently attaching or rushing
        if (Input.GetKeyDown(KeyCode.E) && !isAttaching && !rush)
        {
            TryAttachToEnemy();
        }

        // If attaching, start draining health every second
        if (isAttaching)
        {
            attachTimer += Time.deltaTime;

            // Check if the attachment duration has reached 3 seconds
            if (attachTimer >= 3f)
            {
                DetachFromEnemy();
            }
            else
            {
                DrainEnemyHealth();
            }
        }

        score_text.SetText(score.ToString());
    }

    private void FollowMousePositionDelayed(float maxSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, GetWorldPositionFromMouse(),
            maxSpeed * Time.deltaTime);
    }

    private Vector2 GetWorldPositionFromMouse()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void LAMouse()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        m_transform.rotation = rotation;
    }

    private void TryAttachToEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, m_transform.up, 1f, LayerMask.GetMask("Enemy"));

        if (hit.collider != null)
        {
            // Attach to the enemy
            isAttaching = true;
            attachedEnemy = hit.collider.gameObject;
        }
    }

    private void DrainEnemyHealth()
    {
        // Check if there's an attached enemy
        if (attachedEnemy != null)
        {
            // Get the enemy's health script
            EnemyHealth enemyHealth = attachedEnemy.GetComponent<EnemyHealth>();

            // Drain the enemy's health every second
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
            }
        }
    }

    private void DetachFromEnemy()
    {
        // Reset the attaching-related variables
        isAttaching = false;
        attachTimer = 0f;
        attachedEnemy = null;
    }
}
    

