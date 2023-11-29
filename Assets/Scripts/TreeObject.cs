using UnityEngine;

public class TreeObject : MonoBehaviour
{
    public float health = 4;
    public float maxHealth = 4;
    public GameObject objectToSpawn;

    private bool isCreated = false;
    private bool rush = false;

    [SerializeField] FloatingHealthBar healthBar;
    public float size; // New variable to store the rock's size

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, maxHealth);
        healthBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && isCreated == false)
        {
            SpawnObjects();
            Destroy(gameObject);
            isCreated = true;
        }

        rush = Input.GetMouseButton(0);

        if (rush)
        {
            healthBar.gameObject.SetActive(true);
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Assuming the player's GameObject has a SizeScript component
        GameObject player = collision.gameObject;
        SizeScript playerSizeScript = player.GetComponent<SizeScript>();

        // Set the rock's size based on the new variable
        size = transform.localScale.x; // Assuming uniform scaling

        if (rush && playerSizeScript != null)
        {
            if (playerSizeScript.scale < size)
            {
                health -= 1;
                healthBar.UpdateHealthBar(health, maxHealth);

                if (health <= 0 && !isCreated)
                {
                    SpawnObjects();
                    Destroy(gameObject);
                    isCreated = true;
                }
            }
        }

        if (collision.gameObject.CompareTag("acid"))
        {
            // Destroy the projectile
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
