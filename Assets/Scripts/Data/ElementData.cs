using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using System;
using System.IO;

public class ElementData : MonoBehaviour
{
    private static ElementData _elementData;
    public static ElementData Instance { get { return _elementData; } }

    private void Awake()
    {
        _elementData = this;
        DontDestroyOnLoad(gameObject);
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

        var result = DomainLogic.DB.SqlConn().Table<Element>().Where(x => x.StoryId == _storyId).OrderBy(e => e.Index);
        if (result != null && result.Count() > 0)
        {
            List<Element> elements = result.ToList();
            elements.ForEach((el) => { SetupElement(ref el); });
            _onElementsLoadedCallback(elements);
        } else {
            _onElementsLoadedCallback(new List<Element>());
        }
    }

    public void GetElement(int elementId, OnElementLoaded onElementLoaded)
    {
        _onElementLoaded = onElementLoaded;

        var element = DomainLogic.DB.SqlConn().Table<Element>().Where(p => p.Id == elementId).FirstOrDefault();
        SetupElement(ref element);

        _onElementLoaded(element);
    }

    private void SetupElement(ref Element element)
    {
        if (element.ElementType == ElementType.Picture)
        {
            element.Paths = DataUtils.GetPathsFromText(element.Text);
        }
        else
        {
            element.Text = DataUtils.DecodeTextFromBytes(element.EncodedText);
        }
    }

    public void SaveElement(Element element)
    {
        if (element.ElementType == ElementType.Picture)
        {
            element.Text = DataUtils.AddPathsToText(element.Paths);
        }
        else
        {
            element.EncodedText = DataUtils.EncodeTextInBytes(element.Text);
        }

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

    internal void DeleteElement(int elementId)
    {
        DomainLogic.DB.SqlConn().Delete<Element>(elementId);
    }

    internal void ExportToHtml(List<Element> elements)
    {
        var mainPath = StoryService.Instance.Story.GetActivePath();

        string html = HtmlExportUtils.GetBaseStart();

        html += HtmlExportUtils.PageStart();

        foreach (var element in elements)
        {
            switch (element.ElementType)
            {
                case ElementType.SceneHeading:
                case ElementType.Action:
                case ElementType.Character:
                case ElementType.Dialog:
                    html += HtmlExportUtils.Element(element);
                    break;
                default:
                    html += HtmlExportUtils.Picture(element);
                    break;
            }
        }

        html += HtmlExportUtils.DivTagEnd();
        html += HtmlExportUtils.PageScript();
        html += HtmlExportUtils.GetBaseEnd();


        var path = mainPath + "/_" + StoryService.Instance.Story.Name.Trim() + ".html";
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                w.Write(html);
            }
        }
        var copy_path = mainPath + "/copy_" + DateTime.Now.ToString("MM-yy-dd-hh-mm") + ".html";
        using (FileStream fs = new FileStream(copy_path, FileMode.OpenOrCreate))
        {
            using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                w.Write(html);
            }
        }
    }
}
