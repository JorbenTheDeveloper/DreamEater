using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;

    [SerializeField] FloatingHealthBarOld healthBar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any additional update logic here if needed
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        health += amount;

        // Ensure health doesn't exceed the maximum
        health = Mathf.Min(health, maxHealth);

        healthBar.UpdateHealthBar(health, maxHealth);
    }

    void Die()
    {
        SceneManager.LoadScene("Retry");
        Destroy(gameObject);
    }
}
