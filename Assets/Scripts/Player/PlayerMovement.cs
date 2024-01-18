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
    public float staminaDropFactor = 10f; // per second
    public float staminaRecharce = 20f; // per second

    [Header("Exhaust")]
    public float exhaustedSpeed = 3f;
    private bool hasExhausted = false;
    public float exhaustedDuration = 3f;
    private float exhaustedTimer = 0;

    public CinemachineVirtualCamera cinemachineVirtualCamera;

    private Animator Animator;

    public bool IsRushing => isRushing;

    private void Start()
    {
        mainCamera = Camera.main;
        currentStamina = maxStamina;
        currentSpeed = speed;

        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateRushing();

        if (IsMouseOverPlayer())
        {
            Animator.SetBool("IsRunning", false);
            Animator.SetBool("IsWalking", false);
            return;
        }
        Animator.SetBool("IsWalking", true);
        MoveAndRotate();
    }

    private void UpdateRushing()
    {
        if (Input.GetMouseButton(0))
        {
            if (currentStamina > 0)
            {
                currentSpeed = speed * 2;

                currentStamina -= Time.deltaTime * staminaDropFactor;
                Animator.SetBool("IsRunning", true);
                Animator.SetBool("IsWalking", false);
                isRushing = true;
            }
            else
            {
                hasExhausted = true;
                currentSpeed = exhaustedSpeed;
                Animator.SetBool("IsRunning", false);
                isRushing = false;
            }
        }
        else if (!hasExhausted)
        {
            currentStamina += Time.deltaTime * staminaRecharce;
            currentSpeed = speed;
            Animator.SetBool("IsRunning", false);
            isRushing = false;
        }

        if (hasExhausted)
        {
            exhaustedTimer += Time.deltaTime;

            if (exhaustedTimer > exhaustedDuration)
            {
                hasExhausted = false;
                exhaustedTimer = 0;
                currentStamina = maxStamina;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

    }

    private void MoveAndRotate()
    {
        transform.position = Vector2.MoveTowards(transform.position, GetWorldPositionFromMouse(),
                currentSpeed * Time.deltaTime);
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        Vector2 direction = GetWorldPositionFromMouse() - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    private bool IsMouseOverPlayer()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        // mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        float distance = Vector2.Distance(transform.position, new Vector2(mouseWorldPosition.x, mouseWorldPosition.y));

        float mouseOverThreshold = 1.0f;

        return distance < mouseOverThreshold;
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        //mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
