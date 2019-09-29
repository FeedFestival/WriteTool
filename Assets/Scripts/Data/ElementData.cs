using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;

public class ElementData : MonoBehaviour
{
    private static ElementData _elementData;
    public static ElementData Instance { get { return _elementData; } }

    private void Awake()
    {
        _elementData = this;
    }

    public delegate void OnElementsLoaded(List<Element> elements);
    private OnElementsLoaded _onElementsLoadedCallback;
    public delegate void OnElementLoaded(Element element);
    private OnElementLoaded _onElementLoaded;

    private int _storyId;

    public void GetElementsByStory(int storyId, OnElementsLoaded onElementsLoadedCallback)
    {
        _onElementsLoadedCallback = onElementsLoadedCallback;
        _storyId = storyId;

        var result = DomainLogic.DB.SqlConn().Table<Element>().Where(x => x.StoryId == _storyId);
        if (result != null && result.Count() > 0)
        {
            List<Element> elements = result.ToList();
            _onElementsLoadedCallback(elements);
        }
        //StartCoroutine(WaitForElements());
    }

    public void GetElement(int elementId, OnElementLoaded onElementLoaded)
    {
        _onElementLoaded = onElementLoaded;

        var element = DomainLogic.DB.SqlConn().Table<Element>().Where(p => p.Id == elementId).FirstOrDefault();
        element.Text = DataUtils.DecodeTextFromBytes(element.EncodedText);
        _onElementLoaded(element);
    }

    public void SaveElement(Element element)
    {
        element.EncodedText = DataUtils.EncodeTextInBytes(element.Text);
        if (element.IsNew == false)
        {
            DomainLogic.DB.SqlConn().Update(element);
        }
        else
        {
            DomainLogic.DB.SqlConn().Insert(element);
        }
    }

    IEnumerator WaitForElements()
    {
        List<Element> elements = DomainLogic.DB.SqlConn().Table<Element>().Where(x => x.StoryId == _storyId).ToList();

        yield return 0;

        _onElementsLoadedCallback(elements);
    }
}
