using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Image StaminaBar;
    public TextMeshProUGUI exhaustedDurationText;

    private float exhaustedDuration;
    private void Start()
    {
        exhaustedDuration = playerMovement.GetExhaustedDuration();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateStaminaBar(playerMovement.currentStamina, playerMovement.maxStamina);

        if (playerMovement.HasExhausted())
        {
            float exhaustedTimer = playerMovement.GetExhaustedTimer();
            exhaustedDurationText.text = $"{exhaustedTimer:F1}/{exhaustedDuration}";
        }
        else
        {
            exhaustedDurationText.text = "";
        }
    }

    void UpdateStaminaBar(float currentValue, float maxValue)
    {
        StaminaBar.fillAmount = currentValue / maxValue;
    }
}
