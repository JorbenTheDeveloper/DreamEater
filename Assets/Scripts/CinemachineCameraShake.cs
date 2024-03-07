using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CinemachineCameraShake : MonoBehaviour
{
    public CinemachineCameraOffset CinemachineCameraOffset;
    Vector3 startingPos;
    public void Start()
    {
        startingPos = CinemachineCameraOffset.m_Offset;
    }

    public static void Shake()
    {
        
    }

    /*private IEnumerator _Shake()
    {

    }*/
}
