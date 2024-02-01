using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private float currentHP;
    public float MaxHP;

    public float maxGrow = 4;
    public float Size => transform.localScale.x;

    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;

    public float CurrentHP => currentHP;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShoot = GetComponent<PlayerShoot>();
        currentHP = MaxHP;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Size < 0.5f) return;

            bool isShot = playerShoot.ShootProjectile(Size, transform);
            if (isShot)
            {
                Shrink(0.1f);
            }
        }
    }

    public void TakeDamage(int damageValue)
    {
        currentHP -= damageValue;
        currentHP = Mathf.Clamp(currentHP, 0, MaxHP);
    }

    public void TryEat(Eatable eatable)
    {
        if (!playerMovement.IsRushing) return;

        if (Size >= eatable.Size)
        {
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

    private void Shrink(float value)
    {
        transform.localScale = new Vector3(transform.localScale.x - value, transform.localScale.y - value, 1);
    }
    private void Grow(float value)
    {
        transform.localScale = new Vector3(transform.localScale.x + value, transform.localScale.y + value, 1);
    }
}
