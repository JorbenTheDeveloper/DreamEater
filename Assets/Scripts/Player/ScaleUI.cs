using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScaleUI : MonoBehaviour
{
    public Player player => Player.Instance;
    public TextMeshProUGUI scaleText;

    // Update is called once per frame
    void Update()
    {
        float scaleX = player.transform.localScale.x;
        scaleText.text = $"Size: {scaleX.ToString("0.0")}";
    }
}
