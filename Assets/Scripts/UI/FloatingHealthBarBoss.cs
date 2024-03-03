using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBarBoss : MonoBehaviour
{
    public BunnyBoss boss;
    private Image slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar(boss.CurrentHP, boss.MaxHP);
    }

    void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.fillAmount = currentValue / maxValue;
    }
}
