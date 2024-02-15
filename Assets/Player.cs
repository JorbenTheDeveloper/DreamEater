using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private float currentHP;
    public float MaxHP;

    public float maxGrow = 4;
    public float Size => transform.localScale.x;
    public float StartingSize = 1.2f;

    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;
    public float sizeReduction = 0.1f;

    public float CurrentHP => currentHP;
    public CinemachineVirtualCamera cinemachineCamera;

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
        transform.localScale = new Vector3(StartingSize, StartingSize, 1);

        StartCoroutine(AdjustCameraSize(1));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Size < 0.5f) return;

            bool isShot = playerShoot.ShootProjectile(Size, transform);
            if (isShot)
            {
                Shrink(sizeReduction);
            }
        }
    }

    public void TakeDamage(int damageValue)
    {
        currentHP -= damageValue;
        currentHP = Mathf.Clamp(currentHP, 0, MaxHP);

        if (currentHP <= 0)
        {
            SceneManager.LoadScene("Retry");
        }
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
        StartCoroutine(AdjustCameraSize(1));
    }
    private void Grow(float value)
    {
        transform.localScale = new Vector3(transform.localScale.x + value, transform.localScale.y + value, 1);
        StartCoroutine(AdjustCameraSize(1));
    }

    IEnumerator AdjustCameraSize(float totalTime)
    {
        if (cinemachineCamera != null)
        {
            float sizeRatio = transform.localScale.x / StartingSize;
            float baseOrthographicSize = 6 * (StartingSize / 1f);
            float passedTime = 0;

            while (passedTime < totalTime)
            {
                float step = Mathf.SmoothStep(cinemachineCamera.m_Lens.OrthographicSize, baseOrthographicSize * sizeRatio, passedTime / totalTime);
                cinemachineCamera.m_Lens.OrthographicSize = step;
                passedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
