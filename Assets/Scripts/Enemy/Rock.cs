using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float health = 4;
    public float maxHealth = 4;
    public GameObject objectToSpawn;

    private bool isCreated = false;
    private bool rush = false;

    [SerializeField] FloatingHealthBar healthBar;



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
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            isCreated = true;
            Destroy(gameObject);
        }

        if (Input.GetMouseButton(0))
        {
            rush = true;
        }
        else
        {
            rush = false;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        healthBar.gameObject.SetActive(true);


        if (rush == true)
        {
            health -= 1;
            healthBar.UpdateHealthBar(health, maxHealth);
            print(health);
        }
    }
}
