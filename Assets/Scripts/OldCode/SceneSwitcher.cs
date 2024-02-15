using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchToTutorialScene()
    {
        // Change "Testing" to the name of your testing scene
        SceneManager.LoadScene("Tutorial");
    }
    public void LoadLevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }
}
