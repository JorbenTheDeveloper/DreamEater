using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ArrowPointer : MonoBehaviour
{
    public GameObject arrow; // The arrow GameObject
    public Transform player; // Reference to the player's transform
    public float orbitRadius = 1f; // Orbit radius around the player
    public float orbitSpeed = 100f; // Speed at which the arrow orbits (degrees per second)
    private float currentAngle = 0f; // Current angle of the arrow around the player in degrees

    private List<Eatable> allEatables;
    private Eatable currentEatable;

	private void Start()
	{
        allEatables = FindObjectsOfType<Eatable>().ToList();
        FindClosestEatable();
	}

	void Update()
    {
        
        FindClosestEatable();

        if (currentEatable != null)
        {
            TurnToDirection();
		}
    }

	void TurnToDirection()
	{
		var dir = currentEatable.transform.position - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = rotation;
	}

	void FindClosestEatable()
    {
		allEatables = allEatables.Where(e => e != null && e.gameObject != null && e.gameObject.activeInHierarchy).
            OrderBy(e => Vector3.Distance(e.transform.position, Player.Instance.transform.position)).ToList();

		foreach (Eatable e in allEatables)
        {
            if (Player.Instance.CanEat(e))
            {
                currentEatable = e;
				break;
			}
        }
    }
}

