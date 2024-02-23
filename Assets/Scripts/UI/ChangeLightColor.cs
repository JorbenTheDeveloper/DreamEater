using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
// Required for 2D Light access

public class ChangeLightColor : MonoBehaviour
{
    public float transitionDuration = 5f; // Duration of the color change
    public Light2D globalLight; // Assign this in the Inspector for color change
    public Light2D intensityLight; // Light whose intensity will change, assign in Inspector

    private bool isChangingColor = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") && !isChangingColor) // Ensure the player tag is set correctly
        {
            if (globalLight != null && intensityLight != null) // Check if both lights have been assigned
            {
                StartCoroutine(ChangeColorAndIntensity(transitionDuration));
            }
            else
            {
                Debug.LogError("One or both of the Light2D objects are not assigned in the Inspector");
            }
        }
    }

    IEnumerator ChangeColorAndIntensity(float duration)
    {
        isChangingColor = true;
        float time = 0;
        Color startColor = globalLight.color; // Store the initial color of the global light
        float startIntensity = intensityLight.intensity; // Initially, assume the light starts at full intensity (1)

        while (time < duration)
        {
            // Smoothly transition the color of the global light from its current state to white
            globalLight.color = Color.Lerp(startColor, Color.white, time / duration);
            // Smoothly transition the intensity of the intensity light from its current intensity to 0
            intensityLight.intensity = Mathf.Lerp(startIntensity, 0, time / duration);

            time += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the final color and intensity are set correctly
        globalLight.color = Color.white;
        intensityLight.intensity = 0;

        // Additionally, turn off the intensityLight by disabling its Light2D component
        intensityLight.enabled = false;

        isChangingColor = false;
    }
}
