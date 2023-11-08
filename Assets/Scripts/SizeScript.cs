using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeScript : MonoBehaviour
{
    public float size;

    void Start()
    {
        // Set the size based on the object's initial scale
        size = transform.localScale.x; // Assuming the object has uniform scaling
    }
}
