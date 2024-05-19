using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int DamageAmount = 10;

    public float lifetime = 5f; // Time in seconds before the projectile is automatically destroyed

    private void Start()
    {
        Destroy(gameObject, lifetime); // Schedule destruction of this projectile after 'lifetime' seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Carrot")) return;

        if (collision.CompareTag("Player") || collision.CompareTag("Mouth") || collision.CompareTag("PlayerBody"))
        {
            Player.Instance.TakeDamage(DamageAmount);
        }
		Destroy(gameObject);
	}
}
