using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float maxGrow = 4;
    public int Size;

    private PlayerMovement playerMovement;

    public void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void TryEat(Eatable eatable)
    {
        if (!playerMovement.IsRushing) return;

        if (Size >= eatable.Size)
        {
            Size++;
            Destroy(eatable.gameObject);

            if (IsMaxGrow() == false)
            {
                Grow(eatable.growRate);
            }
        }
    }

    private bool IsMaxGrow()
    {
        return transform.localScale.x >= maxGrow;
    }

    private void Grow(float value)
    {
        transform.localScale = new Vector3(transform.localScale.x + value, transform.localScale.y + value, 1);
    }
}
