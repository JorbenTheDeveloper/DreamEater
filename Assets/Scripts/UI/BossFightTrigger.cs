using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Tilemaps;

public class BossFightTrigger : MonoBehaviour
{
    public Tilemap targetTilemap;
    public GameObject boss;
    public GameObject bossHPBar;
    public PlayableDirector timeline; // Reference to the Timeline's PlayableDirector
    public GameObject player; // Reference to the player GameObject

    private PlayerMovement playerMovement; // Reference to the player's movement script
    private bool hasBeenTriggered = false; // Flag to check if the trigger has already been activated

    private void Start()
    {
        boss.SetActive(false);
        bossHPBar.SetActive(false);

        // Get the PlayerMovement component from the player GameObject
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        if (timeline != null)
        {
            timeline.playOnAwake = false;
            timeline.stopped += OnTimelineStopped; // Subscribe to the stopped event
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Exit if the trigger has already been activated
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true; // Set the flag to true to prevent reactivation

        targetTilemap.gameObject.SetActive(true);
        boss.SetActive(true);
        bossHPBar.SetActive(true);

        // Disable player controls
        if (playerMovement != null)
        {
            playerMovement.StopMovement(); // Disable movement immediately
        }

        // Play the Timeline
        if (timeline != null)
        {
            timeline.Play();
        }
        else
        {
            // If there's no timeline, re-enable movement immediately (or after a delay if preferred)
            Invoke(nameof(ReEnablePlayerMovement), 1f); // 1 second delay as an example
        }
    }

    private void OnTimelineStopped(PlayableDirector obj)
    {
        ReEnablePlayerMovement();
    }

    // Utility method to re-enable player movement
    private void ReEnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.EnableMovement();
        }
    }

    private void OnDestroy()
    {
        if (timeline != null)
        {
            timeline.stopped -= OnTimelineStopped;
        }
    }


}
