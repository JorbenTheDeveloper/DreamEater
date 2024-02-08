using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingShadow : MonoBehaviour
{
    private float StartingScale;
    public float TargetScale = 4;
    public float TimeToFill = 5;

    public SpriteRenderer FrontSpriteRenderer;

    private float CurrentScale => FrontSpriteRenderer.transform.localScale.x;

    private void Start()
    {
        StartingScale = FrontSpriteRenderer.transform.localScale.x;
    }

    public void StartFilling(float time, Vector3 pos)
    {
        FrontSpriteRenderer.transform.localScale = new Vector3(StartingScale, StartingScale, 1);
        transform.position = pos;
        StartCoroutine(Fill());
    }

    IEnumerator Fill()
    {
        while (CurrentScale < TargetScale)
        {
            FrontSpriteRenderer.transform.localScale = new Vector3(CurrentScale + 0.1f, CurrentScale + 0.1f, 1);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
