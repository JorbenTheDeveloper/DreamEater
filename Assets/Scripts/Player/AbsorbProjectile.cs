using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbProjectile : MonoBehaviour
{
    private float accumulatedGrowth = 0f;

    public float ConsumeGrowth()
    {
        float growth = accumulatedGrowth;
        accumulatedGrowth = 0; // Reset after consuming
        return growth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.IsRushing)
            {
                // Logic to absorb the projectile or make it disappear
                Destroy(gameObject); // Example of making the projectile disappear
            }
        }
        else
        {
            if (collision.gameObject.tag == "Projectile")
            {
                // Ignore collision
                return;
            }

            Eatable enemy = collision.gameObject.GetComponent<Eatable>();
            if (enemy != null)
            {


                // Increase the projectile's own size
                float sizeIncrease = enemy.growRate;
                transform.localScale += new Vector3(sizeIncrease, sizeIncrease, 0);

                // Accumulate growth rate to transfer to the player
                // Assuming the projectile itself has an Eatable component
                Eatable projectileEatable = GetComponent<Eatable>();
                if (projectileEatable != null)
                {
                    projectileEatable.growRate += enemy.growRate;
                }


                // Destroy the enemy
                Destroy(collision.gameObject);
            }
        }
        

    }
}
