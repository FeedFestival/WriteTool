using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{
    private static StoryController _storyController;
    public static StoryController Instance { get { return _storyController; } }

    public GameObject NewStoryPanel;
    public GameObject OpenStoryPanel;

    public InputField StoryNameInputField;
    public InputField StoryPathInputField;

    private List<Story> _stories;
    private List<IPrefabComponent> _storiesPool;
    private int _currentSelectedIndex;

    // Start is called before the first frame update
    void Awake()
    {
        _storyController = this;
        NewStoryPanel.SetActive(false);
        OpenStoryPanel.SetActive(true);
        // StoryNameInputField.OnSelect();
    }

    void Start()
    {
        _stories = StoryService.Instance.GetStories();

        if (_stories == null && _stories.Count == 0)
        {
            ShowNewStoryFields();
            return;
        }
        UsefullUtils.DumpToConsole(_stories);
        foreach (Story story in _stories)
        {
            var prefab = GameHiddenOptions.Instance.StoryComponentPrefab;
            var wasNull = UsefullUtils.CheckInPool(
                (IPrefabComponent component) =>
                {
                    return false;
                },
                prefab,
                OpenStoryPanel.transform,
                out IPrefabComponent s,
                ref _storiesPool
                );


            s.UniqueId = s.Id = story.Id;
            (s as StoryComponent).StoryId.text = story.Id.ToString();
            (s as StoryComponent).StoryName.text = story.Name;
            (s as StoryComponent).StoryPath.text = story.Path;

            if (wasNull)
            {
                _storiesPool.Add(s);
            }
        }

        SelectStory(ArrowDirection.Down, 0);
    }

    public void OpenStory(int storyId)
    {
        StoryService.Instance.OpenStory(_stories.Find(s => s.Id == storyId));
    }

    public void SelectStory(ArrowDirection arrowDirection, int? index = null)
    {
        if (index.HasValue)
        {
            _currentSelectedIndex = index.Value;
        }
        else
        {
            (_storiesPool[_currentSelectedIndex] as StoryComponent).Deselect();
            if (arrowDirection == ArrowDirection.Up)
            {
                _currentSelectedIndex--;
                if (_currentSelectedIndex < 0)
                {
                    _currentSelectedIndex = 0;
                }
            }
            else
            {
                _currentSelectedIndex++;
                if (_currentSelectedIndex > _stories.Count - 1)
                {
                    _currentSelectedIndex = _stories.Count - 1;
                }
            }
        }
        (_storiesPool[_currentSelectedIndex] as StoryComponent).Select();
    }

    public void ShowNewStoryFields()
    {
        NewStoryPanel.SetActive(true);
        OpenStoryPanel.SetActive(false);
        StoryNameInputField.text = string.Empty;
        StoryPathInputField.text = Application.streamingAssetsPath;
        // StoryPathInputField.text = Assets.Scripts.Utils.UsefullUtils.GetPathToStreamingAssetsFile("");
    }

    public void HideNewStoryFields()
    {
        NewStoryPanel.SetActive(false);
        OpenStoryPanel.SetActive(true);
    }

    public void OnInputSelect()
    {
        ProjectHotkeyEditor.Instance.ProjectViewState = ProjectViewState.Editing;
    }

    public void OnInputDeselect()
    {
        ProjectHotkeyEditor.Instance.ProjectViewState = ProjectViewState.NewStory;
    }

    public Story GetNewStory()
    {
        var story = new Story()
        {
            Name = StoryNameInputField.text,
            Path = StoryPathInputField.text
        };
        NewStoryPanel.SetActive(false);
        return story;
    }
}
