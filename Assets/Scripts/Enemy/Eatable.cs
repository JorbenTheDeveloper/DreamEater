using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    public int Life = 1;
    public float Size;
    public bool IgnoreSize = false;
    public float growRate = 0.1f;
    public ParticleSystem deathEffect; // Attached Particle System component for death effect

    public bool IsEaten = false;

    private Collider2D colliderEatable2D;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        colliderEatable2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Ensure the Particle System is not playing on start
        if (deathEffect != null)
        {
            deathEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void TakeDamage()
    {
        Life--;
        if (Life <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Disable collider and sprite renderer to "remove" the object from game interactions and visibility
        colliderEatable2D.enabled = false;
        spriteRenderer.enabled = false;
        IsEaten = true;
		// Trigger the death particle effect
		if (deathEffect != null)
        {
            deathEffect.Play();
            // Schedule the destruction of the GameObject after the particle effect duration
            Destroy(gameObject, deathEffect.main.duration);
        }
        else
        {
            // If no particle system is found, destroy the object immediately
            Destroy(gameObject);
        }
    }
}
