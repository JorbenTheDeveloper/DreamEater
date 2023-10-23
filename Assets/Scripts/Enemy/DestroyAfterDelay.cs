using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float DelayInSeconds = 2f;

    void Start()
    {
        // Schedule the object for destruction after the specified delay
        Destroy(gameObject, DelayInSeconds);
    }
}
