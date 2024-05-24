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
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!canAttack) return;

            if (collision.gameObject.TryGetComponent(out Eatable enemy))
            {
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

            if (collision.gameObject.TryGetComponent(out BunnyBoss boss))
            {
                boss.TakeDamage(true);
            }

			if (collision.gameObject.TryGetComponent(out WolfBoss wolfBoss))
			{
				wolfBoss.TakeDamage(true);
			}
		}
    }
}
