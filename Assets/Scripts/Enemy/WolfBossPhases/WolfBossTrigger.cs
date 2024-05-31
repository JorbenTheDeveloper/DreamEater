using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Tilemaps;

public class WolfBossTrigger : MonoBehaviour
{
    public Tilemap exitBlockerTilemap;
    public GameObject wolfBoss;
    public PlayableDirector timeline;
    public GameObject player;
    public CinemachineVirtualCamera boss1Camera;
    public CinemachineVirtualCamera boss2Camera;
    public GameObject wolfBossHPBar;
    public PlayerMovement playerMovement;
    public GameObject PlayerArrow;

    private bool hasBeenTriggered = false;
    private bool isTimelinePlaying = false;

    private void Start()
    {
        wolfBoss.SetActive(false);
        wolfBossHPBar.SetActive(false);

        if (timeline != null)
        {
            timeline.playOnAwake = false;
            timeline.stopped += OnTimelineStopped;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true;
        TriggerBossFight();
    }

    private void TriggerBossFight()
    {
        exitBlockerTilemap.gameObject.SetActive(true);
        wolfBoss.SetActive(true);
        wolfBossHPBar.SetActive(true);
        boss1Camera.gameObject.SetActive(true);
        boss2Camera.gameObject.SetActive(true);
        timeline.gameObject.SetActive(true);
		PlayerArrow.SetActive(false);
		AudioManager.Instance.Play("BossMusic");

        if (playerMovement != null)
        {
            playerMovement.StopMovement(); // Ensure this method is being called correctly
        }
        PlayerShoot.canShootDuringBossFight = false;

        if (timeline != null)
        {
            timeline.Play();
        }
    }

    private void Update()
    {
        // Checks if the timeline is playing and allows skipping it by pressing a specific key
        if (isTimelinePlaying && Input.GetKeyDown(KeyCode.Space)) // Using space key as an example
        {
            timeline.Stop();
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        EndBossFight();
    }

    public void EndBossFight()
    {
        boss1Camera.gameObject.SetActive(false);
        boss2Camera.gameObject.SetActive(false);

        // Re-enable player movement and shooting
        if (playerMovement != null)
        {
            playerMovement.EnableMovement(true); // Ensure to pass true to re-enable movement
        }
        PlayerShoot.canShootDuringBossFight = true;

      
    }

    private void OnDestroy()
    {
        if (timeline != null)
        {
            timeline.stopped -= OnTimelineStopped;
        }
    }
}
