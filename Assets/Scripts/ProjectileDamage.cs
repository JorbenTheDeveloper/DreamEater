using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int DamageAmount = 10;
    public float AttackCoolDown = 1f;
    private float attackCoolDownTimer = 0f;

    private PlayerHealth collidingPlayer = null;

    public float lifetime = 5f; // Time in seconds before the projectile is automatically destroyed

    private void Start()
    {
        attackCoolDownTimer = 0;
        Destroy(gameObject, lifetime); // Schedule destruction of this projectile after 'lifetime' seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collidingPlayer = collision.GetComponent<PlayerHealth>();
            if (collidingPlayer != null)
            {
                // Optionally, you might want to apply damage immediately on contact
                ApplyDamage();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collidingPlayer == null)
                collidingPlayer = collision.GetComponent<PlayerHealth>();

            ApplyDamage();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collidingPlayer = null;
        }
    }

    private void Update()
    {
        if (attackCoolDownTimer > 0)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }
    }

    private void ApplyDamage()
    {
        if (attackCoolDownTimer <= 0 && collidingPlayer != null)
        {
            attackCoolDownTimer = AttackCoolDown;
            collidingPlayer.TakeDamage(DamageAmount);
        }
    }
}
