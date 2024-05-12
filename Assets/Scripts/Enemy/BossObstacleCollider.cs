using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossObstacleCollider : MonoBehaviour
{
	private WolfBoss collidedBoss;
	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collidedBoss == null)
		{
			collidedBoss = collision.gameObject.GetComponent<WolfBoss>();

		}
		else
		{
			if (collidedBoss.Stop())
			{
				Destroy(gameObject);
			}
		}
	}
}
