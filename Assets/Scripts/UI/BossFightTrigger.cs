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
    public BunnyBoss BunnyBoss;

    private PlayerMovement playerMovement; // Reference to the player's movement script
    private bool hasBeenTriggered = false; // Flag to check if the trigger has already been activated
    private Animator playerAnimator;

    private void Start()
    {
        //boss.SetActive(false);
        bossHPBar.SetActive(false);

        // Get the PlayerMovement component from the player GameObject
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAnimator = player.GetComponent<Animator>(); // Get the Animator component
        }

        // Optionally, ensure the timeline doesn't play automatically if that's not desired
        if (timeline != null)
        {
            timeline.playOnAwake = false;
            timeline.stopped += OnTimelineStopped; // Subscribe to the stopped event
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true;

        targetTilemap.gameObject.SetActive(true);
        boss.SetActive(true);
        bossHPBar.SetActive(true);

        if (playerMovement != null)
        {
            playerMovement.StopMovement(); // Disable movement immediately
            if (playerAnimator != null) playerAnimator.enabled = false; // Disable animations
        }

        if (timeline != null)
        {
            timeline.Play();
        }
        else
        {
            BunnyBoss.StartAttack();
            ReEnablePlayerMovement(); // Re-enable movement and animations immediately if there's no timeline
        }
    }

    private void OnTimelineStopped(PlayableDirector obj)
    {
        BunnyBoss.StartAttack();
        ReEnablePlayerMovement();
    }

    // Utility method to re-enable player movement
    private void ReEnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.EnableMovement();
        }
        if (playerAnimator != null) playerAnimator.enabled = true;
    }

    private void OnDestroy()
    {
        if (timeline != null)
        {
            timeline.stopped -= OnTimelineStopped;
        }
    }


}
