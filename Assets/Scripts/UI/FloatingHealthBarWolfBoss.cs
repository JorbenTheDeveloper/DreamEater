using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBarWolfBoss : MonoBehaviour
{
	public WolfBoss boss;
	private Image slider;

	// Start is called before the first frame update
	void Start()
	{
		slider = GetComponent<Image>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateHealthBar(boss.curHP, boss.MaxHP);
	}

	void UpdateHealthBar(float currentValue, float maxValue)
	{
		slider.fillAmount = currentValue / maxValue;
	}
}
