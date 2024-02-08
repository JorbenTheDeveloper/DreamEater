using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    public int healAmount = 3; // Adjust the amount to heal as needed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the other collider has a PlayerHealth component
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Heal the player
                playerHealth.Heal(healAmount);

                // Destroy the healing item after it's touched
                Destroy(gameObject);
            }
        }
    }
}
