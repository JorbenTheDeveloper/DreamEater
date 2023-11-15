using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smallrock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("acid"))
        {

            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}
