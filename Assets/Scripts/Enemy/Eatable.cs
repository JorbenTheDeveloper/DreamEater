using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    public int Life = 1;
    public float Size;
    public float growRate = 0.1f;

    public void TakeDamage()
    {
        Life--;
        if (Life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
