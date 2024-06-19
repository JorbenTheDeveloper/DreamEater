using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkipTimeline : MonoBehaviour
{
    public PlayableDirector director; // Assign the PlayableDirector in the inspector

    void Update()
    {
        // Check if any key is pressed to skip the timeline
        if (director != null && director.state == PlayState.Playing && Input.anyKeyDown)
        {
            director.time = director.duration; // Fast forward to the end of the timeline
            director.Evaluate(); // Ensure the timeline state is updated
            director.Stop(); // Stop the timeline
        }
    }
}
