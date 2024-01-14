using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Size;

    public void TryEat(Eatable eatable)
    {
        if (Size >= eatable.Size)
        {
            Size++;
            Destroy(eatable.gameObject);
        }
    }
}
