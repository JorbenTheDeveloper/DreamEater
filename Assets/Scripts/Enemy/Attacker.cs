using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public int DamageAmount = 5;
    public float AttackCoolDown = 1;
    private float attackCoolDownTimer = 0;

    private Player collidingPlayer = null;

    private void Start()
    {
        attackCoolDownTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
        {
            collidingPlayer = collision.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
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

        if (collidingPlayer == null) return;

        if (attackCoolDownTimer <= 0)
        {
            attackCoolDownTimer = AttackCoolDown;
            collidingPlayer.TakeDamage(DamageAmount);
        }
    }
}
