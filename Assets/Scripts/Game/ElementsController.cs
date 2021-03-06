﻿using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Data;
using System.IO;

public class ElementsController : MonoBehaviour, ISceneStarter
{
    private static ElementsController _elementsController;
    public static ElementsController Instance { get { return _elementsController; } }
    public RectTransform Carret;
    private int _editableIndex;
    public List<Element> Elements;
    public List<IPrefabComponent> _elementsPool;

    // Start is called before the first frame update
    void Awake()
    {
        _elementsController = this;

        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "Carret")
            {
                Destroy(child.gameObject);
            }
        }

        Carret.gameObject.SetActive(true);
    }

    public void InitScene()
    {
        StoryService.Instance.Init();
    }

    public void Init()
    {
        InitHotkeys();
        InitInlineSelection();
    }

    public ElementType GetElementAtCarretPosition()
    {
        var isLastElement = GetCarretIndex() == _elementsPool.Count;
        return ElementsService.GetPreviousElementType(
            _elementsPool, Elements,
            _editableIndex, isLastElement
        );
    }

    private int GetCarretIndex()
    {
        var newIndex = Carret.transform.GetSiblingIndex();
        _editableIndex = newIndex - 1;
        return newIndex;
    }

    public void MoveCarret(bool down = true, int? atPos = null)
    {
        int currentIndex;
        if (atPos.HasValue)
        {
            currentIndex = atPos.Value;
        }
        else
        {
            currentIndex = GetCarretIndex();
        }

        var newIndex = currentIndex + 1;
        if (down == false)
        {
            newIndex = currentIndex - 1;
        }

        if (newIndex == 0 || newIndex == Elements.Count + 1)
        {
            return;
        }

        _editableIndex = (newIndex - 1);
        Carret.transform.SetSiblingIndex(newIndex);
        Carret.name = newIndex + "_Carret";

        var isLastElement = _editableIndex == (_elementsPool.Count - 1);
        if (isLastElement)
        {
            ScrollController.Instance.ScrollToBottom();
        }
        else
        {
            ScrollController.Instance.KeepElementInView(Carret);
        }
    }

    internal void EditElement()
    {
        GetCarretIndex();
        if ((_elementsPool[_editableIndex] as IElementComponent).TypeId == (int)ElementType.Picture)
        {
            (_elementsPool[_editableIndex] as IPictureComponent).AutoSelect();
        }
        else
        {
            (_elementsPool[_editableIndex] as ITextComponent).AutoSelect();
        }
    }

    public bool AddNewElement(ElementType elementType, bool autoCreate = false)
    {
        var currentIndex = GetCarretIndex();
        var isLastElement = currentIndex == _elementsPool.Count;

        if (autoCreate && isLastElement == false) {
            TextEditorHotkeyController.Instance.MainEdit();
            return false;
        }

        if ((Elements == null || Elements.Count == 0) && elementType != ElementType.SceneHeading)
        {
            return false;
        }

        ElementType previousElementType = ElementsService.GetPreviousElementType(
            _elementsPool, Elements,
            _editableIndex, isLastElement
        );

        if (ElementsService.FilterNewElements(elementType, previousElementType) == false)
        {
            return false;
        }

        if (TextEditorHotkeyController.Instance.ShowOptions)
        {
            TextEditorHotkeyController.Instance.FileMainButtons.SetActive(true);
            TextEditorHotkeyController.Instance.InLineSelection.gameObject.SetActive(false);
        }

        var element = new Element()
        {
            Text = ElementsService.GetDefaultText(elementType),
            ElementType = elementType,
            Index = currentIndex,
            IsNew = true
        };
        Elements.Add(element);
        var el = AddElementInPool(element);

        if (isLastElement == false)
        {
            var newIndex = (_editableIndex + 1);
            el.GameObject.transform.SetSiblingIndex(newIndex);
        }
        else
        {
            MoveCarret(true);
        }
        ElementsService.RecalculateIndexes(_elementsPool, Elements);

        GameService.Instance.InternalWait(() =>
            {
                if (element.ElementType == ElementType.Picture)
                {
                    (el as IPictureComponent).AutoSelect();
                }
                else
                {
                    (el as ITextComponent).AutoSelect();
                }
                TextEditorHotkeyController.Instance.AppState = AppState.Editing;

                if (isLastElement)
                {
                    ScrollController.Instance.ScrollToBottom();
                }
                else
                {
                    ScrollController.Instance.KeepElementInView(el.GameObject.GetComponent<RectTransform>());
                }
            });
        return true;
    }

    public void InitElements(List<Element> elements)
    {
        Elements = elements;

        if (_elementsPool != null)
        {
            foreach (IPrefabComponent ipc in _elementsPool)
            {
                ipc.GameObject.SetActive(false);
            }
        }
        else
        {
            _elementsPool = new List<IPrefabComponent>();
        }

        if (Elements.Count == 0)
        {
            GameService.Instance.InternalWait(() =>
            {
                AddNewElement(ElementType.SceneHeading);
            });
            return;
        }

        GameService.Instance.AsyncForEach(Elements.Count, (int i) =>
        {
            AddElementInPool(Elements[i], true);

            MoveCarret(true, Elements.Count - 1);

            if (i >= Elements.Count - 1)
            {
                GameService.Instance.InternalWait(() =>
                {
                    ScrollController.Instance.ScrollToBottom();
                });
            }
        });
    }

    private IPrefabComponent AddElementInPool(Element element, bool isInitializingElements = false)
    {
        var prefab = GameHiddenOptions.Instance.GetPrefabElement(element.ElementType);
        var wasNull = UsefullUtils.CheckInPool(
            (IPrefabComponent component) =>
            {
                var typeId = (ElementType)((component as IElementComponent).TypeId);
                if (component.GameObject.activeSelf == false && typeId == element.ElementType)
                    return true;
                return false;
            },
            prefab,
            transform,
            out IPrefabComponent el,
            ref _elementsPool
            );

        el.UniqueId = element.UniqueId();
        el.GameObject.name = ElementsService.GetElementName(element);

        if (element.ElementType != ElementType.Picture)
        {
            var elementComponent = (el as ITextComponent);
            elementComponent.SetText(element.Text);
        }
        else
        {
            if (element.Paths != null)
            {
                (el as IPictureComponent).Paths = element.Paths;
                (el as IPictureComponent).FillImages();
            }
        }

        (el as IElementComponent).TypeId = element.TypeId;

        if (wasNull)
        {
            if (isInitializingElements)
            {
                _elementsPool.Add(el);
            }
            else
            {
                _elementsPool.Insert(element.Index, el);
            }
        }

        return el;
    }

    public void SaveElements()
    {
        ElementsService.RecalculateIndexes(_elementsPool, Elements);

        foreach (Element element in Elements)
        {
            element.StoryId = StoryService.Instance.Story.Id;
            var el = _elementsPool.FirstOrDefault(e => e.UniqueId == element.UniqueId());

            // UsefullUtils.DumpToConsole(element);

            var isPicture = (el as IElementComponent).TypeId == (int)ElementType.Picture;
            if (isPicture)
            {
                element.Paths = (el as IPictureComponent).Paths;
                element.FileNames = new List<string>();
            }
            else
            {
                element.Text = (el as ITextComponent).GetText();
            }

            ElementData.Instance.SaveElement(element);
            el.UniqueId = element.UniqueId();
            element.IsNew = false;

            if (isPicture)
            {
                int index = 0;
                foreach (var path in element.Paths)
                {
                    if (string.IsNullOrWhiteSpace(path)) {
                        continue;
                    }
                    element.FileNames.Add("img_" + element.Id + "_0_" + ".jpg");
                    var newPath = StoryService.Instance.Story.GetActivePath() + element.FileNames[index];
                    if (path == newPath) {
                        continue;
                    }
                    File.Copy(path, newPath, true);
                    element.Paths[index] = newPath;
                    index++;
                }
                ElementData.Instance.SaveElement(element);
            }
        }

        TextEditorHotkeyController.Instance.ToggleFileOptions();
    }

    public void DeleteElement(int uniqueId)
    {
        int index = Elements.FindIndex(e => e.UniqueId() == uniqueId);
        if (index < 0)
        {
            Debug.LogWarning("Element you are tring to delete doesn't exists. (" + uniqueId + ")");
            return;
        }
        ElementsService.RecalculateIndexes(_elementsPool, Elements);
        if (Elements[index].IsNew == false)
        {
            ElementData.Instance.DeleteElement(Elements[index].Id);
        }

        if (Elements[index].ElementType == ElementType.Character) {
            // Debug.Log("Element is Character");
            int indexOfDialog = index + 1;
            if (indexOfDialog <= Elements.Count - 1 
                && Elements[indexOfDialog].ElementType == ElementType.Dialog) {
                // Debug.Log("Element " + Elements[indexOfDialog].ElementType + " can also be removed.");
                if (Elements[indexOfDialog].IsNew == false)
                {
                    ElementData.Instance.DeleteElement(Elements[indexOfDialog].Id);
                }
                RemoveElement(indexOfDialog, Elements[indexOfDialog].UniqueId());
            }
        } else if (Elements[index].ElementType == ElementType.Dialog) {
            int indexOfCharacter = index - 1;
            Debug.Log(Elements[indexOfCharacter].ElementType);
            if (indexOfCharacter>= 0
                && Elements[indexOfCharacter].ElementType == ElementType.Character) {
                Debug.Log("Element " + Elements[indexOfCharacter].ElementType + " can also be removed.");
                if (Elements[indexOfCharacter].IsNew == false)
                {
                    ElementData.Instance.DeleteElement(Elements[indexOfCharacter].Id);
                }
                RemoveElement(indexOfCharacter, Elements[indexOfCharacter].UniqueId());

                // we need to get the new uniqueId somehow
                ElementsService.RecalculateIndexes(_elementsPool, Elements);

                DeleteElement(uniqueId);
                return;
            }
        }

        RemoveElement(index, uniqueId);

        MoveCarret(true, index - 1);
        TextEditorHotkeyController.Instance.EscapeKey();
        
        // recalculate indexes again in case the order is screwed
        ElementsService.RecalculateIndexes(_elementsPool, Elements);
    }

    private void RemoveElement(int index, int uniqueId) {
        Elements.RemoveAt(index);
        index = _elementsPool.FindIndex(e => e.UniqueId == uniqueId);
        Destroy(_elementsPool[index].GameObject);
        _elementsPool.RemoveAt(index);
    }

    public void ExportToHtml()
    {
        SaveElements();
        ElementData.Instance.ExportToHtml(Elements);
    }

    private void InitInlineSelection()
    {
        List<string> options = new List<string>();
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            var buttonName = elementType.ToString() + ElementsService.GetButtonHotkey(elementType);
            options.Add(buttonName);
        }
        TextEditorHotkeyController.Instance.InLineSelection.Init(options, OnElementTypeSelected);
    }

    public void OnElementTypeSelected(int value)
    {
        AddNewElement(((ElementType[])System.Enum.GetValues(typeof(ElementType)))[value]);
    }

    private void InitHotkeys()
    {
        TextEditorHotkeyController.Instance.AddAsComponent("NewWrite", TextEditorHotkeyController.Instance.OnAddNewElement);
        TextEditorHotkeyController.Instance.AddAsComponent("NewWrite_SceneHeading", () =>
        {
            return AddNewElement(ElementType.SceneHeading);
        });
        TextEditorHotkeyController.Instance.AddAsComponent("NewWrite_Action", () =>
        {
            return AddNewElement(ElementType.Action);
        });
        TextEditorHotkeyController.Instance.AddAsComponent("NewWrite_Character", () =>
        {
            return AddNewElement(ElementType.Character);
        });
        TextEditorHotkeyController.Instance.AddAsComponent("NewWrite_Dialog", () =>
        {
            return AddNewElement(ElementType.Dialog);
        });
        TextEditorHotkeyController.Instance.AddAsComponent("NewWrite_Picture", () =>
        {
            return AddNewElement(ElementType.Picture);
        });
    }
}
