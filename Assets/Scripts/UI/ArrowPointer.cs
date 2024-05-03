using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public GameObject arrow; // The arrow GameObject
    public Transform player; // Reference to the player's transform
    public float orbitRadius = 1f; // Orbit radius around the player
    public float orbitSpeed = 100f; // Speed at which the arrow orbits (degrees per second)
    private float currentAngle = 0f; // Current angle of the arrow around the player in degrees

    void Update()
    {
        // Update the current angle based on the orbit speed and time
        currentAngle += orbitSpeed * Time.deltaTime;
        currentAngle %= 360; // Keep the angle in the range of 0 to 360

        // Calculate the new position based on the current angle
        float radian = currentAngle * Mathf.Deg2Rad; // Convert degrees to radians
        Vector3 orbitPosition = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * orbitRadius;
        arrow.transform.position = player.position + orbitPosition;

        // Set the arrow rotation to face outward from the center (player)
        arrow.transform.right = orbitPosition.normalized;
    }
}

