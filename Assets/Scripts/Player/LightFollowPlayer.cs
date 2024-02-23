using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFollowPlayer : MonoBehaviour
{
    public Transform player;
    public float lightIntensity = 1.0f;
    public float baseVisibilityRange = 2.5f; // Base visibility range at the starting scale
    public float scaleToRangeRatio = 0.5f; // Increase in visibility range per 0.1 scale increase
    public float rangeTransitionSpeed = 5f; // Speed at which the visibility range adjusts

    private Light2D light2D;
    private float targetVisibilityRange;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        // Initialize the target visibility range based on the initial player scale
        UpdateTargetVisibilityRange();
    }

    void Update()
    {
        if (player != null && light2D != null)
        {
            // Update the light's position to follow the player
            transform.position = player.position;

            // Optionally adjust light properties
            light2D.intensity = lightIntensity;

            // Update the target visibility range based on the current player scale
            UpdateTargetVisibilityRange();

            // Smoothly transition the visibility range towards the target value
            light2D.pointLightOuterRadius = Mathf.Lerp(light2D.pointLightOuterRadius, targetVisibilityRange, Time.deltaTime * rangeTransitionSpeed);
        }
    }

    void UpdateTargetVisibilityRange()
    {
        // Assuming uniform scaling for simplicity
        float playerScale = player.localScale.x;
        float scaleMultiplier = (playerScale - 0.5f) * 10; // Calculate how many 0.1 increments fit into the scale increase from base scale 0.5
        targetVisibilityRange = baseVisibilityRange + (scaleMultiplier * scaleToRangeRatio);
    }
}
