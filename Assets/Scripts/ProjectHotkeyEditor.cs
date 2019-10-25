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
    }

    private void NewKey()
    {
        StoryService.Instance.NewStory();
    }

    private void OpenKey()
    {
        throw new NotImplementedException();
    }

    private void EscapeKey()
    {
        throw new NotImplementedException();
    }

    private void CloseKey() {

    }
}
