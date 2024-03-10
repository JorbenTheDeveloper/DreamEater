using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CinemachineCameraShake : MonoBehaviour
{
    public CinemachineCameraOffset CinemachineCameraOffset;
    public float ShakeDuration = 1;

    public Vector2 randomX = new Vector2(-0.5f, 0.5f);
    public Vector2 randomY = new Vector2(-0.5f, 0.5f);

    Vector3 startingPos = Vector3.zero;
    public void Start()
    {
        startingPos = CinemachineCameraOffset.m_Offset;
    }

    public void Shake()
    {
        StartCoroutine(_Shake());
    }

    private IEnumerator _Shake()
    {
        float timer = 0;
        while (timer < ShakeDuration)
        {
            float x = Random.Range(randomX.x, randomX.y);
            float y = Random.Range(randomY.x, randomY.y);
            CinemachineCameraOffset.m_Offset = new Vector3(x, y, 0);
            yield return new WaitForNextFrameUnit();
            timer += Time.deltaTime;
        }
        CinemachineCameraOffset.m_Offset = startingPos;
    }
}
