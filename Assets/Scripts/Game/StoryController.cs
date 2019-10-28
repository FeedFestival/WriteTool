using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    private static StoryController _storyController;
    public static StoryController Instance { get { return _storyController; } }

    public GameObject NewStoryPanel;

    // Start is called before the first frame update
    void Awake()
    {
        _storyController = this;
        NewStoryPanel.SetActive(false);
    }

    public void ShowNewStoryFields()
    {
        NewStoryPanel.SetActive(true);
    }

    public void HideNewStoryFields()
    {
        NewStoryPanel.SetActive(false);
    }

    public Story GetNewStory()
    {
        var story = new Story() {

        };
        NewStoryPanel.SetActive(false);
        return story;
    }
}
