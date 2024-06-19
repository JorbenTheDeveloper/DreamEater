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

    void Update()
    {
        // Check if any key is pressed to skip the timeline
        if (director != null && director.state == PlayState.Playing && Input.anyKeyDown)
        {
            director.time = director.duration; // Fast forward to the end of the timeline
            director.Evaluate(); // Ensure the timeline state is updated
            director.Stop(); // Stop the timeline, which will trigger OnPlayableDirectorStopped
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
