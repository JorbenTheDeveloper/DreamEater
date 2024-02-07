using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Speed")]
    public float currentSpeed;
    public float speed = 10f;

    [Header("Rushing")]
    public float currentStamina;
    private bool isRushing = false;
    public float maxStamina = 100;
    public float staminaDropFactor = 10f; // Stamina consumption per second while rushing
    public float staminaRecharge = 20f; // Stamina recharge rate per second
    public bool IsRushing => isRushing;

    [Header("Exhaust")]
    public float exhaustedSpeed = 3f;
    private bool hasExhausted = false;
    public float exhaustedDuration = 3f;
    private float exhaustedTimer = 0;
    public float GetExhaustedTimer() => exhaustedTimer;
    public bool HasExhausted() => hasExhausted;

    public CinemachineVirtualCamera cinemachineVirtualCamera;

    private Animator animator;

    [Header("Particle Effects")]
    public GameObject particleEffectPrefab; // Assign your particle prefab in the inspector
    public float particleSpawnTimer = 0f;
    public float particleSpawnInterval = 2f; // Interval between particle spawns

    [Header("Slow Layer")]
    public LayerMask mudLayer; // Assign this in the Inspector
    private bool isInMud = false;
    public float mudSpeed = 5f;

    private void Start()
    {
        mainCamera = Camera.main;
        currentStamina = maxStamina;
        currentSpeed = speed;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleMovementAndRotation();
        UpdateRushing();
        HandleParticleEffectSpawn();
        if (isInMud)
        {
            currentSpeed = mudSpeed;
        }

    }

    void HandleMovementAndRotation()
    {
        Vector2 targetPosition = GetWorldPositionFromMouse();
        if (!IsMouseOverPlayer())
        {
            MoveAndRotate(targetPosition);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }

    void UpdateRushing()
    {
        isRushing = Input.GetMouseButton(0) && currentStamina > 0 && !hasExhausted;

        if (isRushing)
        {
            currentSpeed = speed * 2;
            currentStamina -= Time.deltaTime * staminaDropFactor;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            currentSpeed = speed;
            animator.SetBool("IsRunning", false);
            if (!hasExhausted) currentStamina += Time.deltaTime * staminaRecharge;
        }

        if (currentStamina <= 0)
        {
            hasExhausted = true;
            isRushing = false;
            currentSpeed = exhaustedSpeed; // Apply exhausted speed
        }

        if (hasExhausted)
        {
            exhaustedTimer += Time.deltaTime;
            if (exhaustedTimer >= exhaustedDuration)
            {
                hasExhausted = false;
                exhaustedTimer = 0;
                currentStamina = maxStamina; // Reset stamina after exhaustion period
                currentSpeed = speed; // Reset speed after exhaustion ends
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void MoveAndRotate(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        Vector2 direction = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    Vector2 GetWorldPositionFromMouse()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    bool IsMouseOverPlayer()
    {
        Vector2 mousePosition = GetWorldPositionFromMouse();
        return Vector2.Distance(mousePosition, transform.position) < 1.0f;
    }

    void HandleParticleEffectSpawn()
    {
        // Spawn particles only when the player is rushing
        if (isRushing && particleSpawnTimer >= particleSpawnInterval)
        {
            SpawnParticleEffect();
            particleSpawnTimer = 0f; // Reset the timer after spawning
        }
        else
        {
            
            particleSpawnTimer += Time.deltaTime;
        }
    }

    void SpawnParticleEffect()
    {
        GameObject effect = Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 2f); // Automatically destroy the spawned effect after 2 seconds
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Mud"))
        {
            isInMud = true;
            currentSpeed = mudSpeed;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Mud"))
        {
            isInMud = false;
            currentSpeed = speed;
        }
    }
}
