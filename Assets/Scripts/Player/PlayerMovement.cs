using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float currentSpeed;
    public float speed = 10f;
    private Camera mainCamera;

    private float currentStamina;
    public float maxStamina = 100;
    public float staminaDropFactor = 10f;

    private void Start()
    {
        mainCamera = Camera.main;
        currentStamina = maxStamina;
        currentSpeed = speed;
    }

    private void Update()
    {
        if (IsMouseOverPlayer()) return;

        if (Input.GetMouseButton(0))
        {
            if (currentStamina > 0)
            {
                currentSpeed = speed * 2;
                currentStamina -= Time.deltaTime * staminaDropFactor;
            }
            else
            {
                currentSpeed = speed;
            }
        } 
        else
        {
            currentStamina += Time.deltaTime * staminaDropFactor;
            currentSpeed = speed;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

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
