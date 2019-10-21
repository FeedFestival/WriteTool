using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Data;

public class ElementsController : MonoBehaviour
{
    private static ElementsController _elementsController;
    public static ElementsController Instance { get { return _elementsController; } }
    public RectTransform Carret;
    private int _editableIndex;
    public List<Element> Elements;
    public List<IPrefabComponent> _elementsPool;
    public Image CarretImage;

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

    public void Init()
    {
        InitHotkeys();
        InitInlineSelection();
    }

    public void OnAddNewElement()
    {
        HotkeyController.Instance.AppState = AppState.NewElement;

        if (HotkeyController.Instance.ShowOptions)
        {
            HotkeyController.Instance.FileMainButtons.SetActive(false);
            HotkeyController.Instance.FileOptionsSelection.SetActive(false);
            HotkeyController.Instance.InLineSelection.gameObject.SetActive(true);

            var options = FilterElementTypes();
            HotkeyController.Instance.InLineSelection.Filter(options);
        }
    }

    public void ToggleFileOptions()
    {
        HotkeyController.Instance.AppState = HotkeyController.Instance.AppState == AppState.FileOptions
            ? AppState.MainEdit : AppState.FileOptions;

        if (HotkeyController.Instance.ShowOptions)
        {
            if (HotkeyController.Instance.AppState == AppState.FileOptions)
            {
                HotkeyController.Instance.FileMainButtons.SetActive(false);
                HotkeyController.Instance.FileOptionsSelection.SetActive(true);
            }
            else
            {
                HotkeyController.Instance.FileMainButtons.SetActive(true);
                HotkeyController.Instance.FileOptionsSelection.SetActive(false);
            }
            HotkeyController.Instance.InLineSelection.gameObject.SetActive(false);
        }
    }

    private List<int> FilterElementTypes()
    {
        if (Elements == null)
        {
            InitElements(new List<Element>());
        }
        if (Elements.Count == 0)
        {
            return new List<int>() { 0 };
        }

        var isLastElement = GetCarretIndex() == _elementsPool.Count;
        var previousElementType = ElementsService.GetPreviousElementType(
            _elementsPool, Elements,
            _editableIndex, isLastElement
        );

        var options = new List<int>();
        int i = 0;
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            if (ElementsService.FilterNewElements(elementType, previousElementType))
            {
                options.Add(i);
            }
            i++;
        }
        return options;
    }

    internal void ShowCarret()
    {
        if (HotkeyController.Instance.AppState == AppState.MainEdit
            || HotkeyController.Instance.AppState == AppState.FileOptions
            || HotkeyController.Instance.AppState == AppState.NewElement)
        {
            CarretImage.color = GameHiddenOptions.Instance.CarretColor;
        }
        else
        {
            CarretImage.color = GameHiddenOptions.Instance.TransparentColor;
        }
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

    public void AddNewElement(ElementType elementType)
    {
        var currentIndex = GetCarretIndex();
        var isLastElement = currentIndex == _elementsPool.Count;
        var previousElementType = ElementsService.GetPreviousElementType(
            _elementsPool, Elements,
            _editableIndex, isLastElement
        );

        if (ElementsService.FilterNewElements(elementType, previousElementType) == false)
        {
            return;
        }

        if (HotkeyController.Instance.ShowOptions)
        {
            HotkeyController.Instance.FileMainButtons.SetActive(true);
            HotkeyController.Instance.InLineSelection.gameObject.SetActive(false);
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
                HotkeyController.Instance.AppState = AppState.Editing;

                if (isLastElement)
                {
                    ScrollController.Instance.ScrollToBottom();
                }
                else
                {
                    ScrollController.Instance.KeepElementInView(el.GameObject.GetComponent<RectTransform>());
                }
            });
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

        if (Elements.Count == 0)
            return;

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
        foreach (Element element in Elements)
        {
            element.StoryId = StoryService.Instance.Story.Id;
            var el = _elementsPool.FirstOrDefault(e => e.UniqueId == element.UniqueId());

            UsefullUtils.DumpToConsole(element);

            if ((el as IElementComponent).TypeId == (int)ElementType.Picture)
            {
                element.Paths = (el as IPictureComponent).Paths;
            }
            else
            {
                element.Text = (el as ITextComponent).GetText();
            }

            element.Id = ElementData.Instance.SaveElement(element);
            el.UniqueId = element.UniqueId();

            element.IsNew = false;
        }

        ToggleFileOptions();
    }

    public void DeleteElement(int uniqueId)
    {
        int index = Elements.FindIndex(e => e.UniqueId() == uniqueId);
        if (Elements[index].IsNew == false)
        {
            ElementData.Instance.DeleteElement(Elements[index].Id);
        }
        Elements.RemoveAt(index);
        index = _elementsPool.FindIndex(e => e.UniqueId == uniqueId);
        Destroy(_elementsPool[index].GameObject);
        _elementsPool.RemoveAt(index);

        MoveCarret(true, index - 1);
        HotkeyController.Instance.EscapeKey();
        // ShowCarret();
    }

    private void InitInlineSelection()
    {
        List<string> options = new List<string>();
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            var buttonName = elementType.ToString() + ElementsService.GetButtonHotkey(elementType);
            options.Add(buttonName);
        }
        HotkeyController.Instance.InLineSelection.Init(options, OnElementTypeSelected);
    }

    public void OnElementTypeSelected(int value)
    {
        AddNewElement(((ElementType[])System.Enum.GetValues(typeof(ElementType)))[value]);
    }

    private void InitHotkeys()
    {
        HotkeyController.Instance.AddAsComponent("NewWrite", OnAddNewElement);
        HotkeyController.Instance.AddAsComponent("NewWrite_SceneHeading", () =>
        {
            AddNewElement(ElementType.SceneHeading);
        });
        HotkeyController.Instance.AddAsComponent("NewWrite_Action", () =>
        {
            AddNewElement(ElementType.Action);
        });
        HotkeyController.Instance.AddAsComponent("NewWrite_Character", () =>
        {
            AddNewElement(ElementType.Character);
        });
        HotkeyController.Instance.AddAsComponent("NewWrite_Dialog", () =>
        {
            AddNewElement(ElementType.Dialog);
        });
        HotkeyController.Instance.AddAsComponent("NewWrite_Picture", () =>
        {
            AddNewElement(ElementType.Picture);
        });
    }
}
