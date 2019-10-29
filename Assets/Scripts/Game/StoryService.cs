using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryService : MonoBehaviour
{
    private static StoryService _storyService;
    public static StoryService Instance { get { return _storyService; } }

    public bool AutoOpen;

    public Story Story;

    private void Awake()
    {
        _storyService = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameHiddenOptions.Instance.FileBrowser.Hide();

        if (AutoOpen)
        {
            Story = new Story()
            {
                Id = 2,
                Name = "Lavinia Story"
            };
            OpenAutomatically();
        }
    }

    public void CreateNewStory()
    {
        Story = StoryController.Instance.GetNewStory();
        DomainLogic.DB.SqlConn().Insert(Story);
        AutoOpen = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void Init()
    {
        GameHiddenOptions.Instance.FileBrowser.Hide();
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
