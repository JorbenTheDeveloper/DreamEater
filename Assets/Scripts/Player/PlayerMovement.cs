using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.AI;
using System.Drawing;
using static Unity.Burst.Intrinsics.X86;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Speed")]
    public float currentSpeed;
    public float BaseSpeed = 2;
    public float SpeedMultiplier = 2;
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
    public float GetExhaustedDuration() => exhaustedDuration;
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

    [Header("Input Control")]
    public bool inputEnabled = true;

    NavMeshAgent agent;


    public float Size => transform.localScale.x;

    private void Start()
    {
        mainCamera = Camera.main;
        currentStamina = maxStamina;
        currentSpeed = speed;
        animator = GetComponent<Animator>();

        agent = GetComponent <NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private float GetSpeedBySize()
    {
        return BaseSpeed + Size * SpeedMultiplier;
    }

    private void Update()
    {
        if (!inputEnabled) return;

        speed = GetSpeedBySize();
        UpdateRushing();
        HandleParticleEffectSpawn();

        // Determine the current speed for animation
        float animationSpeed = agent.velocity.magnitude / speed;
        animator.SetFloat("Speed", animationSpeed); // Assuming you have a "Speed" float parameter

        if (isInMud)
        {
            currentSpeed = mudSpeed;
        }

        agent.speed = currentSpeed;
        HandleMovementAndRotation();

        // Update Animator states
        animator.SetBool("IsWalking", animationSpeed > 0.1f); // Adjust threshold as needed
        animator.SetBool("IsRunning", isRushing && animationSpeed > 0.1f);
    }

    void HandleMovementAndRotation()
    {
        if (!inputEnabled)
        {
            return; // If input is disabled, do not process movement or rotation.
        }

        Vector2 targetPosition = GetWorldPositionFromMouse();

        if (!IsMouseOverPlayer()) // Only move if the mouse is not over the player to avoid jittering.
        {
            MoveAndRotate(targetPosition);
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
        Vector2 direction = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (agent.Raycast(targetPosition, out var hit))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(targetPosition);
        }


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
        return Vector2.Distance(mousePosition, transform.position) < 0.5f;
    }

    void HandleParticleEffectSpawn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Spawn the particle effect
            SpawnParticleEffect();
        }
    }

    void SpawnParticleEffect()
    {
        GameObject effect = Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);

        // Set the particle effect's scale to match the player's scale
        effect.transform.localScale = transform.localScale;

        Destroy(effect, 2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Mud"))
        {
            isInMud = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Mud"))
        {
            isInMud = false;
        }
    }

    public void StopMovement()
    {
        inputEnabled = false; // Disable input handling.
                              // If using a NavMeshAgent, consider stopping its current movement.
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    public void EnableMovement()
    {
        inputEnabled = true; // Re-enable input handling.

        if (agent != null)
        {
            agent.isStopped = false; // Make sure the NavMeshAgent can move again.
        }
    }


}
