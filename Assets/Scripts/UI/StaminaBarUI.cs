using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Image StaminaBar;

    // Update is called once per frame
    void Update()
    {
        UpdateStaminaBar(playerMovement.currentStamina, playerMovement.maxStamina);
    }

    void UpdateStaminaBar(float currentValue, float maxValue)
    {
        StaminaBar.fillAmount = currentValue / maxValue;
    }
}
