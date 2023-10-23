using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;

    [SerializeField] FloatingHealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            SceneManager.LoadScene("Retry");
            Destroy(gameObject);
        }
    }

    // Method to restore health
    public void RestoreHealth(int amount)
    {
        health += amount;

        // Ensure that the health does not exceed the maxHealth
        health = Mathf.Min(health, maxHealth);

        healthBar.UpdateHealthBar(health, maxHealth);
    }

    
}
