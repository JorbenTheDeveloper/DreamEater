using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouth : MonoBehaviour
{
    public Player Player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Eatable eatable))
        {
            Player.TryEat(eatable);
        }
    }

    // TODO: if game slows down, improve this function
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Eatable eatable))
        {
            Player.TryEat(eatable);
        }
    }
}
