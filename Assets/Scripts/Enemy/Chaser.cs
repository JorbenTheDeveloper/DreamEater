using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    private Player player => Player.Instance;
    public Eatable eatable;

    public float ChaseRange = 20f;
    public float Speed = 3f;

    private float DistanceToPlayer => Vector3.Distance(player.transform.position, transform.position);
    private float Size => eatable.Size;

    // Update is called once per frame
    void Update()
    {
        if (DistanceToPlayer <= ChaseRange && Size >= player.Size)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * Speed);
        }
        else
        {

        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, ChaseRange);
    }
}
