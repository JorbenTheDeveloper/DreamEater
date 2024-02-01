using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public int DamageAmount = 5;
    public float AttackCoolDown = 1;
    private float attackCoolDownTimer = 0;

    private Player collidingPlayer = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attackCoolDownTimer = 0;
            collidingPlayer = collision.gameObject.GetComponent<Player>();
            collidingPlayer.TakeDamage(DamageAmount);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collidingPlayer = null;
        }
    }

    private void Update()
    {
        if (collidingPlayer == null) return;
        attackCoolDownTimer += Time.deltaTime;

        if (attackCoolDownTimer >= AttackCoolDown)
        {
            collidingPlayer.TakeDamage(DamageAmount);
            attackCoolDownTimer = 0;
        }
    }
}
