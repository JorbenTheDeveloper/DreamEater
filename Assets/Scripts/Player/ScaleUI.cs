using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScaleUI : MonoBehaviour
{
    public Player player;
    public TextMeshProUGUI scaleText;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float scaleX = player.transform.localScale.x;
        scaleText.text = $"Size: {scaleX}";
    }
}
