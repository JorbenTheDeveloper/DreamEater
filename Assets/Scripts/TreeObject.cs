using UnityEngine;

public class TreeObject : MonoBehaviour
{
    public float health = 8;
    public float maxHealth = 8;
    public GameObject objectToSpawn;

    private bool isCreated = false;
    private bool rush = false;

    [SerializeField] FloatingHealthBar healthBar;
    public float size;

    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, maxHealth);
        healthBar.gameObject.SetActive(false);

        UpdateSize(); // Set the tree's size based on the object's initial scale
    }

    void Update()
    {
        UpdateSize(); // Update the tree's size if it changes dynamically

        if (health <= 0 && !isCreated)
        {
            SpawnObjects();
            isCreated = true;
            Destroy(gameObject);
        }

        // Update rush based on input
        rush = Input.GetMouseButton(0);

        // Deactivate health bar when rushing is false
        if (!rush)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    void UpdateSize()
    {
        // Set the tree's size based on the object's scale
        size = transform.localScale.x; // Assuming uniform scaling
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;
        SizeScript playerSizeScript = player.GetComponent<SizeScript>();

        if (rush && playerSizeScript != null)
        {
            // Debug.Log("Player Size: " + playerSizeScript.scale); // Uncomment for debugging

            if (playerSizeScript.scale < size)
            {
                // Debug.Log("Activating Health Bar"); // Uncomment for debugging
                healthBar.gameObject.SetActive(true);

                // Debug.Log("Reducing Health"); // Uncomment for debugging
                health -= 1;
                healthBar.UpdateHealthBar(health, maxHealth);

                if (health <= 0 && !isCreated)
                {
                    SpawnObjects();
                    isCreated = true;
                    Destroy(gameObject); // Move the destroy statement here
                }
            }
        }

        if (collision.gameObject.CompareTag("acid"))
        {
            Destroy(gameObject);
        }
    }

    void SpawnObjects()
    {
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
        }
    }
}
