using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : MonoBehaviour
{
    private bool rush = false;
    public PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButton(0))
        {
            rush = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") & rush == true)
        {
            
            player.score += 5;

            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Object") & rush == true)
        {
            print("Object");
            player.score += 1;
            Destroy(collision.gameObject);
        }

    }
}
