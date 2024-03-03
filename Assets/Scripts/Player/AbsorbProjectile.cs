using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Eatable))]
public class AbsorbProjectile : MonoBehaviour
{
    private Eatable projectileEatable;
    private bool canAttack;
    public TrailRenderer TrailRenderer;

    private void Start()
    {
        projectileEatable = GetComponent<Eatable>();
        canAttack = true;

        TrailRenderer.startWidth = transform.localScale.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!canAttack) return;

            Eatable enemy = collision.gameObject.GetComponent<Eatable>();
            if (enemy == null) return;

            if (projectileEatable.Size >= enemy.Size)
            {
                // Increase the projectile's own size
                float sizeIncrease = enemy.growRate;
                transform.localScale += new Vector3(sizeIncrease, sizeIncrease, 0);

                // Accumulate growth rate to transfer to the player
                projectileEatable.growRate += enemy.growRate;

                enemy.TakeDamage();
            }
        }
    }
}
