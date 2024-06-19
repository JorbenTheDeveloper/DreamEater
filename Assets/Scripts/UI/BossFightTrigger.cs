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
    public TMPro.TextMeshProUGUI bossFightText;
    public GameObject PlayerArrow;

    private PlayerMovement playerMovement; // Reference to the player's movement script
    private bool hasBeenTriggered = false; // Flag to check if the trigger has already been activated
    private Animator playerAnimator;

    private bool isTimelinePlaying = false;

    private void Start()
    {
        boss.SetActive(false);
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

    void Update()
    {
        // Check if the timeline is playing and any key is pressed
        if (isTimelinePlaying && Input.anyKeyDown)
        {
            timeline.time = timeline.duration;
            timeline.Evaluate(); //Forces the timeline to update to the new time immediately
            timeline.Stop();

            isTimelinePlaying = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true;
        PlayerShoot.canShootDuringBossFight = false;

        targetTilemap.gameObject.SetActive(true);
        boss.SetActive(true);
        bossHPBar.SetActive(true);
        PlayerArrow.SetActive(false);
        AudioManager.Instance.Play("BossMusic");

        if (bossFightText != null)
        {
            bossFightText.gameObject.SetActive(true);
            bossFightText.text = "Press any key to skip the cutscene";
        }

        if (playerMovement != null)
        {
            playerMovement.StopMovement(); // Disable movement immediately
            if (playerAnimator != null) playerAnimator.enabled = false; // Disable animations
        }

        if (timeline != null)
        {
            isTimelinePlaying = true;
            timeline.Play();
        }
        else
        {
            BunnyBoss.StartAttack();
            ReEnablePlayerMovement(); // Re-enable movement and animations immediately if there's no timeline
        }

        // Set checkpoint when the player triggers the boss fight
        Player playerComponent = player.GetComponent<Player>();
        if (playerComponent != null)
        {
            playerComponent.SetCheckpoint(player.transform.position);
        }
    }

    private void OnTimelineStopped(PlayableDirector obj)
    {
        isTimelinePlaying = false;
        PlayerShoot.canShootDuringBossFight = true;
        BunnyBoss.StartAttack();
        ReEnablePlayerMovement();

        // Disable the TMP text here
        if (bossFightText != null)
        {
            bossFightText.gameObject.SetActive(false);
        }
    }

    // Utility method to re-enable player movement
    private void ReEnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.EnableMovement(true);
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

