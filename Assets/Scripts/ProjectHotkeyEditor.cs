using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectHotkeyEditor : MonoBehaviour
{
    public static ProjectHotkeyEditor _hotkeyController;
    public static ProjectHotkeyEditor Instance { get { return _hotkeyController; } }

    [SerializeField]
    public ProjectViewState ProjectViewState;

    public GameObject MainMenuButtons;
    public GameObject NewStoryButtons;

    void Awake()
    {
        _hotkeyController = this;
    }
    void Start()
    {
        ShowMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            EscapeKey();
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            OpenKey();
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            NewKey();
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            CloseKey();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveKey();
        }
    }

    private void NewKey()
    {
        if (ProjectViewState == ProjectViewState.MainMenu)
        {
            StoryController.Instance.ShowNewStoryFields();
            ShowNewStoryMenu();
        }
    }

    private void OpenKey()
    {
        if (ProjectViewState == ProjectViewState.MainMenu)
        {
            
        }
    }

    private void EscapeKey()
    {
        if (ProjectViewState == ProjectViewState.NewStory)
        {
            StoryController.Instance.HideNewStoryFields();
            ShowMainMenu();
        }
    }

    public void SaveKey()
    {
        if (ProjectViewState == ProjectViewState.NewStory)
        {
            StoryService.Instance.CreateNewStory();
            ShowMainMenu();
        }
    }

    private void CloseKey()
    {
        if (ProjectViewState == ProjectViewState.NewStory)
        {
            EscapeKey();
        }
        else if (ProjectViewState == ProjectViewState.MainMenu)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }

    private void ShowMainMenu()
    {
        ProjectViewState = ProjectViewState.MainMenu;
        MainMenuButtons.SetActive(true);
        NewStoryButtons.SetActive(false);
    }

    private void ShowNewStoryMenu()
    {
        ProjectViewState = ProjectViewState.NewStory;
        NewStoryButtons.SetActive(true);
        MainMenuButtons.SetActive(false);
    }
}
