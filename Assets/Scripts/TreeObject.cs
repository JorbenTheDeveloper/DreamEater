using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : MonoBehaviour
{
    public float health = 8; // Set initial health to 8
    public float maxHealth = 8;
    public GameObject objectToSpawn;

    private bool isCreated = false;
    private bool rush = false;

    [SerializeField] FloatingHealthBar healthBar;
    public float size; // New variable to store the tree's size

    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, maxHealth);

        healthBar.gameObject.SetActive(false);

        // Set the tree's size based on the object's initial scale
        size = transform.localScale.x; // Assuming uniform scaling

        // Additional setup code if needed
    }

    void Update()
    {
        if (health <= 0 && !isCreated)
        {
            SpawnObjects();
            isCreated = true;
            Destroy(gameObject);
        }

        // Update rush based on input
        rush = Input.GetMouseButton(0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Assuming the player's GameObject has a SizeScript component
        GameObject player = collision.gameObject;
        SizeScript playerSizeScript = player.GetComponent<SizeScript>();

        // Set the tree's size based on the new variable
        size = transform.localScale.x; // Assuming uniform scaling

        if (rush && playerSizeScript != null)
        {
            Debug.Log("Player Size: " + playerSizeScript.scale);

            if (playerSizeScript.scale < size)
            {
                Debug.Log("Activating Health Bar");
                healthBar.gameObject.SetActive(true);
                Debug.Log("Reducing Health");
                health -= 1;
                healthBar.UpdateHealthBar(health, maxHealth);

                if (health <= 0 && !isCreated)
                {
                    SpawnObjects();
                    isCreated = true;
                    Destroy(gameObject);
                }
            }
        }

        if (collision.gameObject.CompareTag("acid"))
        {
            Destroy(gameObject);
        }
    }

    // Method to spawn objects when health reaches 0
    void SpawnObjects()
    {
        Instantiate(objectToSpawn, transform.position, transform.rotation);
        Instantiate(objectToSpawn, transform.position, transform.rotation);
        Instantiate(objectToSpawn, transform.position, transform.rotation);
        Instantiate(objectToSpawn, transform.position, transform.rotation);
    }
}
