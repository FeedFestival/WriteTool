using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryService : MonoBehaviour
{
    private static StoryService _storyService;
    public static StoryService Instance { get { return _storyService; } }

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
        ElementData.Instance.GetElementsByStory(1, (List<Element> elements) =>
        {
            ElementsController.Instance.Init(elements);
        });
    }
}
