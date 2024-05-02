using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnCollision : MonoBehaviour
{
    public GameObject objectToActivate;  // The object to activate

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider that entered is the player
        if (other.gameObject.CompareTag("Player"))
        {
            // Activate the object
            objectToActivate.SetActive(true);
        }
    }
}
