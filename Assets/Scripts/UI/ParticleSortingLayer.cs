using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSortingLayer : MonoBehaviour
{
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    void Start()
    {
        // Get the Renderer component from the Particle System
        var renderer = GetComponent<ParticleSystemRenderer>();

        // Set the sorting layer and order
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = sortingOrder;
    }
}
