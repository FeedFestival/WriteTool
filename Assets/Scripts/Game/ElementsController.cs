using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ElementsController : MonoBehaviour
{
    private static ElementsController _elementsController;
    public static ElementsController Instance { get { return _elementsController; } }
    public GameObject Carret;
    private int _editableIndex;
    public List<Element> Elements;
    public List<IPrefabComponent> _elementsPool;
    public GameObject FileMainButtons;
    public GameObject FileOptionsButton;
    public GameObject AddNewButton;
    public GameObject FileOptionsSelection;
    public InLineSelection InLineSelection;

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

        FileMainButtons.SetActive(true);
        AddNewButton.SetActive(true);
        FileOptionsButton.SetActive(true);
        FileOptionsSelection.SetActive(false);
        InLineSelection.gameObject.SetActive(false);
    }

    public void Init()
    {
        InitHotkeys();
        InitInlineSelection();
    }

    public void OnAddNewElement()
    {
        HotkeyController.Instance.AppState = AppState.NewElement;

        FileMainButtons.SetActive(false);
        FileOptionsSelection.SetActive(false);
        InLineSelection.gameObject.SetActive(true);

        var options = FilterElementTypes();
        InLineSelection.Filter(options);
    }

    public void ToggleFileOptions()
    {
        HotkeyController.Instance.AppState = HotkeyController.Instance.AppState == AppState.FileOptions
            ? AppState.MainEdit : AppState.FileOptions;

        if (HotkeyController.Instance.AppState == AppState.FileOptions)
        {
            FileMainButtons.SetActive(false);
            FileOptionsSelection.SetActive(true);
        }
        else
        {
            FileMainButtons.SetActive(true);
            FileOptionsSelection.SetActive(false);
        }
        InLineSelection.gameObject.SetActive(false);
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

        var options = new List<int>();
        ElementType previousElement;
        if (isLastElement)
        {
            previousElement = Elements[Elements.Count - 1].ElementType;
        }
        else
        {
            previousElement = (ElementType)((_elementsPool[_editableIndex] as IElementComponent).TypeId);
        }

        int i = 0;
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            if (GameService.Instance.FilterNewElements(elementType, previousElement))
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
    }

    internal void EditElement()
    {
        (_elementsPool[_editableIndex] as ITextComponent).AutoSelect();
    }

    private void RecalculateIndexes()
    {
        for (var i = 0; i < _elementsPool.Count; i++)
        {
            var index = Elements.FindIndex(e => { return e.UniqueId() == _elementsPool[i].UniqueId; });
            Elements[index].Index = i;
            _elementsPool[i].UniqueId = Elements[index].UniqueId();
            _elementsPool[i].GameObject.name = GetElementName(Elements[index]);
        }
    }

    public void AddNewElement(ElementType elementType)
    {
        var currentIndex = GetCarretIndex();
        var isLastElement = currentIndex == _elementsPool.Count;

        Debug.Log("isLastElement: " + isLastElement);
        Debug.Log("currentIndex: " + currentIndex);
        Debug.Log("_editableIndex: " + _editableIndex);

        ElementType previousElementType = ElementType.Action;
        if (isLastElement)
        {
            if (Elements.Count > 0)
            {
                previousElementType = Elements[Elements.Count - 1].ElementType;
            }
        }
        else
        {
            if (_elementsPool.Count > 0)
            {
                previousElementType = (ElementType)((_elementsPool[_editableIndex] as IElementComponent).TypeId);
            }
        }

        Debug.Log("previousElementType: " + previousElementType);

        if (GameService.Instance.FilterNewElements(elementType, previousElementType) == false)
        {
            return;
        }

        FileMainButtons.SetActive(true);
        InLineSelection.gameObject.SetActive(false);

        var element = new Element()
        {
            Text = GetDefaultText(elementType),
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

            RecalculateIndexes();
        }
        else
        {
            MoveCarret(true);
        }

        GameService.Instance.InternalWait(() =>
            {
                (el as ITextComponent).AutoSelect();
                HotkeyController.Instance.AppState = AppState.Editing;

                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            });
    }

    private string GetDefaultText(ElementType elementType)
    {
        if (elementType == ElementType.SceneHeading)
            return "INT.";
        return "";
    }

    private void InitInlineSelection()
    {
        List<string> options = new List<string>();
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            var buttonName = elementType.ToString() + GetButtonHotkey(elementType);
            options.Add(buttonName);
        }
        InLineSelection.Init(options, OnElementTypeSelected);
    }

    private string GetButtonHotkey(ElementType? elementType)
    {
        if (elementType.HasValue)
        {
            switch (elementType.Value)
            {
                case ElementType.SceneHeading:
                    return " [<b><color=red>S</color></b>]";
                case ElementType.Action:
                    return " [<b><color=red>A</color></b>]";
                case ElementType.Character:
                    return " [<b><color=red>C</color></b>]";
                case ElementType.Dialog:
                    return " [<b><color=red>D</color></b>]";
                default:
                    break;
            }
        }
        return string.Empty;
    }

    public void OnElementTypeSelected(int value)
    {
        AddNewElement(((ElementType[])System.Enum.GetValues(typeof(ElementType)))[value]);
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
            // Debug.Log("Index: " + Elements[i].Index + ", i: " + i);

            AddElementInPool(Elements[i], true);

            MoveCarret(true, Elements.Count - 1);

            if (i >= Elements.Count - 1)
            {
                GameService.Instance.InternalWait(() =>
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
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
        el.GameObject.name = GetElementName(element);

        var elementComponent = (el as ITextComponent);
        elementComponent.SetText(element.Text);

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

    private string GetElementName(Element element)
    {
        return element.Index + "_[" + element.Id + "]_" + element.ElementType.ToString() + " (" + element.UniqueId() + ")";
    }

    public void SaveElements()
    {
        foreach (Element element in Elements)
        {
            element.StoryId = StoryService.Instance.Story.Id;
            var el = _elementsPool.FirstOrDefault(e => e.UniqueId == element.UniqueId());

            UsefullUtils.DumpToConsole(element);

            element.Text = (el as ITextComponent).GetText();

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
    }
}
