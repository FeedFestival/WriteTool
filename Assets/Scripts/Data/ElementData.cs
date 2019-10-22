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

    public int SaveElement(Element element)
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
            return DomainLogic.DB.SqlConn().Update(element);
        }
        else
        {
            return DomainLogic.DB.SqlConn().Insert(element);
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
        var mainPath = UsefullUtils.GetPathToStreamingAssetsFile("");
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
                    var fileNames = new List<string>();

                    fileNames.Add("img_" + element.Id + "_0_" + ".jpg");
                    var newPath = mainPath + fileNames[0];
                    File.Copy(element.Paths[0], newPath, true);

                    if (string.IsNullOrEmpty(element.Paths[1]) == false)
                    {
                        fileNames.Add("img_" + element.Id + "_1_" + ".jpg");
                        newPath = mainPath + fileNames[1];
                        File.Copy(element.Paths[1], newPath, true);
                    }

                    html += HtmlExportUtils.Picture(element, fileNames);
                    break;
            }
        }

        html += HtmlExportUtils.DivTagEnd();
        html += HtmlExportUtils.GetBaseEnd();

        var path = mainPath + "test.html";

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                w.Write(html);
            }
        }
    }
}
