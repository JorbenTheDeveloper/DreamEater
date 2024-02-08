using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public List<Sprite> trailSprites; // List of sprites for the trail
    public GameObject spritePrefab; // Prefab with SpriteRenderer, no sprite needed
    public float spawnInterval = 0.1f; // Time between spawns
    public float fadeDuration = 1.0f; // Duration of fade effect

    private List<GameObject> pooledSprites = new List<GameObject>();
    private float timer = 0f;

    public GameObject spawnLocationController;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnTrailSprite();
        }
    }

    void SpawnTrailSprite()
    {
        GameObject spriteObj = GetPooledSprite();

        // Use the specified GameObject's position as the spawn location
        if (spawnLocationController != null)
        {
            spriteObj.transform.position = spawnLocationController.transform.position;
        }
        else
        {
            spriteObj.transform.position = transform.position; // Fallback to the script owner's position
        }

        Sprite selectedSprite = trailSprites[Random.Range(0, trailSprites.Count)];
        SpriteRenderer sr = spriteObj.GetComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        spriteObj.SetActive(true);

        // Optional: Adjust sorting layer and order here as needed
        sr.sortingOrder = 2;

        StartCoroutine(FadeSprite(spriteObj));
    }

    GameObject GetPooledSprite()
    {
        foreach (var obj in pooledSprites)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(spritePrefab);
        pooledSprites.Add(newObj);
        return newObj;
    }

    IEnumerator FadeSprite(GameObject spriteObj)
    {
        SpriteRenderer sr = spriteObj.GetComponent<SpriteRenderer>();
        Color initialColor = sr.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        spriteObj.SetActive(false);
    }
}
