using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{
    private static StoryController _storyController;
    public static StoryController Instance { get { return _storyController; } }

    public GameObject NewStoryPanel;

    public InputField StoryNameInputField;
    public InputField StoryDescInputField;

    // Start is called before the first frame update
    void Awake()
    {
        _storyController = this;
        NewStoryPanel.SetActive(false);

        // StoryNameInputField.OnSelect();
    }

    public void ShowNewStoryFields()
    {
        NewStoryPanel.SetActive(true);
    }

    public void HideNewStoryFields()
    {
        NewStoryPanel.SetActive(false);
    }

    public void OnInputSelect()
    {
        Debug.Log("selected");
        ProjectHotkeyEditor.Instance.ProjectViewState = ProjectViewState.Editing;
    }

    public void OnInputDeselect()
    {
        Debug.Log("de - selected");
        ProjectHotkeyEditor.Instance.ProjectViewState = ProjectViewState.NewStory;
    }

    public Story GetNewStory()
    {
        var story = new Story()
        {
            Name = StoryNameInputField.text
        };
        NewStoryPanel.SetActive(false);
        return story;
    }
}
