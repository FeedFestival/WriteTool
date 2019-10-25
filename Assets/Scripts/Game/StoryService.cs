using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        //EventSystemManager.currentSystem.currentInputModule.DeactivateModule();
        Init();
    }

    public void Init()
    {
        GameHiddenOptions.Instance.FileBrowser.Hide();

        Story = new Story()
        {
            Id = 2,
            Name = "Lavinia Story"
        };

        ElementsController.Instance.Init();

        ElementData.Instance.GetElementsByStory(Story.Id, (List<Element> elements) =>
        {
            ElementsController.Instance.InitElements(elements);
        });
    }
}
