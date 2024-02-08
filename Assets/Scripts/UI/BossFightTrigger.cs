using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossFightTrigger : MonoBehaviour
{
    public Tilemap targetTilemap;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            targetTilemap.gameObject.SetActive(true);
        }
    }
}
