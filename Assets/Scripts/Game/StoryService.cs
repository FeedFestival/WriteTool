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
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        Story = new Story()
        {
            Id = 1,
            Name = "Test Story"
        };

        ElementData.Instance.GetElementsByStory(Story.Id, (List<Element> elements) =>
        {
            ElementsController.Instance.Init(elements);
        });
    }
}
