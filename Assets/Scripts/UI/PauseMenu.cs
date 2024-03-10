using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1; // Resume game time
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0; // Pause game time
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        // If running in the Unity editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); // Quit the game
        #endif
    }
}
