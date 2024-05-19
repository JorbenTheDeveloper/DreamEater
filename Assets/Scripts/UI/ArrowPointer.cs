using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ArrowPointer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform

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
		allEatables = allEatables.Where(e => e != null && e.gameObject != null && e.gameObject.activeInHierarchy && !e.IsEaten).
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

