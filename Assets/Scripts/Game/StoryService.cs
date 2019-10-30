using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;
using System.Linq;

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

    public void Init()
    {
        if (Story == null)
        {
            Story = new Story()
            {
                Id = 2,
                Name = "Lavinia Story"
            };
            Story.Path = Application.streamingAssetsPath + Story.GetStoryNamePath();
        }
        OpenAutomatically();
    }

    public void CreateNewStory()
    {
        Story = StoryController.Instance.GetNewStory();
        DomainLogic.DB.SqlConn().Insert(Story);
        Story.Path += StoryService.Instance.Story.GetStoryNamePath();
        System.IO.Directory.CreateDirectory(Story.Path);
        DomainLogic.DB.SqlConn().Update(Story);
        
        OpenStory();
    }

    public void OpenStory(Story story = null)
    {
        if (story != null)
        {
            Story = story;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("TextView");
    }

    public List<Story> GetStories()
    {
        var result = DomainLogic.DB.SqlConn().Table<Story>().Where(s => true).OrderByDescending(s => s.Id);
        if (result != null && result.Count() > 0)
        {
            List<Story> stories = result.ToList();
            return stories;
        }
        else
        {
            return new List<Story>();
        }
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
