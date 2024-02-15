using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossFightTrigger : MonoBehaviour
{
    public Tilemap targetTilemap;
    public GameObject Boss;
    public GameObject BossHPBar;

    private void Start()
    {
        Boss.SetActive(false);
        BossHPBar.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            targetTilemap.gameObject.SetActive(true);
            Boss.SetActive(true);
            BossHPBar.SetActive(true);
        }
    }
}
