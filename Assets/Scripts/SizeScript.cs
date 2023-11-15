using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeScript : MonoBehaviour
{
    public float scale;  // Use this variable for the size comparison
    private PlayerMovement playerMovement;
    public int score;

    void Start()
    {
        // Set the size based on the object's initial scale
        scale = transform.localScale.x; // Assuming the object has uniform scaling

        

        // Find the PlayerMovement script in the scene
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script not found in the scene.");
        }
    }

    // Example method to update the player's score based on size
    public void UpdatePlayerScore()
    {
        if (playerMovement != null)
        {
            if (scale <= playerMovement.Size)
            {
                playerMovement.Score += score; // Adjust the score as needed
            }
        }
    }

    // Example method to handle object destruction
    public void DestroyObject()
    {
        // Perform any other necessary actions before destruction
        UpdatePlayerScore(); // Update the player's score before destroying the object
        Destroy(gameObject); // Destroy the object
    }
}
