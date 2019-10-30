using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Windows;

public class StoryService : MonoBehaviour
{
    private static StoryService _storyService;
    public static StoryService Instance { get { return _storyService; } }

    public Story Story;

    private void Awake()
    {
        _storyService = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CreateNewStory()
    {
        Story = StoryController.Instance.GetNewStory();
        DomainLogic.DB.SqlConn().Insert(Story);
        Story.Path += StoryService.Instance.Story.GetStoryNamePath();
        Directory.CreateDirectory(Story.Path);
        DomainLogic.DB.SqlConn().Update(Story);
        UnityEngine.SceneManagement.SceneManager.LoadScene("TextView");
    }

    public void Init()
    {
        if (Story == null)
        {
            Story = new Story()
            {
                Id = 2,
                Name = "Lavinia Story"
            };
            Story.Path = UsefullUtils.GetPathToStreamingAssetsFile("") + Story.GetStoryNamePath();
        }
        OpenAutomatically();
    }

    public void OpenAutomatically()
    {
        ElementsController.Instance.Init();

        ElementData.Instance.GetElementsByStory(Story.Id, (List<Element> elements) =>
        {
            ElementsController.Instance.InitElements(elements);
        });
    }
}
