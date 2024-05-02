using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public GameObject arrow; // The arrow GameObject
    public float orbitRadius = 1f; // Distance from the player
    public float transitionSpeed = 2f; // Speed of transition
    public float checkInterval = 0.5f; // Time interval to update the arrow's direction

    private float timeSinceLastCheck = 0;
    private Vector3 targetPosition; // Target position for the arrow
    private float targetAngle = 0; // Target angle for the arrow's rotation

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;

        // Check for the nearest eligible "Eatable" periodically
        if (timeSinceLastCheck >= checkInterval)
        {
            Eatable nearestEatable = FindEligibleNearestEatable();
            if (nearestEatable != null)
            {
                SetTargetPositionAndRotation(nearestEatable);
            }
            timeSinceLastCheck = 0;
        }

        // Smoothly transition the arrow to its new position and rotation
        arrow.transform.localPosition = Vector3.Lerp(arrow.transform.localPosition, targetPosition, transitionSpeed * Time.deltaTime);
        arrow.transform.rotation = Quaternion.Lerp(arrow.transform.rotation, Quaternion.Euler(new Vector3(0, 0, targetAngle)), transitionSpeed * Time.deltaTime);
    }

    Eatable FindEligibleNearestEatable()
    {
        Eatable[] eatables = FindObjectsOfType<Eatable>(); // Find all "Eatable" objects in the scene
        if (eatables.Length == 0) return null;

        float minDistance = float.MaxValue;
        Eatable nearestEatable = null;

        Player player = Player.Instance; // Assuming Player is a singleton
        float playerSize = player.Size; // Player's current size

        foreach (Eatable eatable in eatables)
        {
            // Check eligibility
            if (eatable.Size <= playerSize || eatable.IgnoreSize)
            {
                float distance = Vector3.Distance(player.transform.position, eatable.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEatable = eatable;
                }
            }
        }

        return nearestEatable;
    }

    void SetTargetPositionAndRotation(Eatable eatable)
    {
        // Direction from the player to the eatable
        Vector3 direction = eatable.transform.position - Player.Instance.transform.position;

        // Convert to angle in radians
        float angleRad = Mathf.Atan2(direction.y, direction.x);

        // Position on the circular path
        targetPosition = new Vector3(
            orbitRadius * Mathf.Cos(angleRad),
            orbitRadius * Mathf.Sin(angleRad),
            0 // For a 2D game, z should stay constant
        );

        // Set target angle for rotation
        targetAngle = angleRad * Mathf.Rad2Deg;
    }
}

