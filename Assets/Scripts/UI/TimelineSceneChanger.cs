using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimelineSceneChanger : MonoBehaviour
{
    public PlayableDirector director; // Assign this in the inspector
    public string sceneToChangeTo = "YourNextSceneName"; // The name of the scene to change to

    void Start()
    {
        if (director != null)
        {
            director.stopped += OnPlayableDirectorStopped; // Subscribe to the 'stopped' event
        }
    }

    private void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnPlayableDirectorStopped; // Clean up, unsubscribe from the event
        }
    }

    // This method is called when the Timeline finishes playing
    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            SceneManager.LoadScene(sceneToChangeTo); // Load the new scene
        }
    }
}
