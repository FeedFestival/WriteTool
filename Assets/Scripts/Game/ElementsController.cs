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
        HotkeyController.Instance.CanUseTools = true;

        FileMainButtons.SetActive(false);
        FileOptionsSelection.SetActive(false);
        InLineSelection.gameObject.SetActive(true);

        var options = FilterElementTypes();
        InLineSelection.Filter(options);
    }

    private void ToggleFileOptions()
    {
        HotkeyController.Instance.ShowFileOptions = !HotkeyController.Instance.ShowFileOptions;

        if (HotkeyController.Instance.ShowFileOptions)
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

        var lastElementType = Elements[Elements.Count - 1].ElementType;
        var options = new List<int>();
        int i = 0;
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            if (GameService.Instance.FilterNewElements(elementType, lastElementType))
            {
                options.Add(i);
            }
            i++;
        }
        return options;
    }

    internal void ShowCarret(bool canUseHotkeys)
    {
        if (canUseHotkeys)
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
        return Carret.transform.GetSiblingIndex();
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

        //Debug.Log(Elements.Count);

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
        var currentIndex = Carret.transform.GetSiblingIndex();
        var newIndex = currentIndex + 1;
        for (var i = 0; i < Elements.Count; i++)
        {
            if (Elements[i].Index >= newIndex)
            {
                Elements[i].Index = i;
                _elementsPool[i].GameObject.name = GetElementName(Elements[i]);
            }
        }
    }

    public void AddNewElement(ElementType elementType)
    {
        if (Elements.Count > 0)
        {
            var lastElementType = Elements[Elements.Count - 1].ElementType;
            if (GameService.Instance.FilterNewElements(elementType, lastElementType) == false)
                return;
        }

        FileMainButtons.SetActive(true);
        InLineSelection.gameObject.SetActive(false);

        var currentIndex = GetCarretIndex();
        var weNeedToShiftElements = Elements.Count != currentIndex;

        var element = new Element()
        {
            Text = GetDefaultText(elementType),
            ElementType = elementType,
            Index = currentIndex,
            IsNew = true
        };
        Elements.Add(element);
        var el = AddElementInPool(element);
        (el as ITextComponent).AutoSelect();

        //if (weNeedToShiftElements)
        MoveCarret();

        GameService.Instance.InternalWait(() =>
        {
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
            AddElementInPool(Elements[i]);

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

    private IPrefabComponent AddElementInPool(Element element)
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

        // need to determine index here
        el.UniqueId = element.UniqueId();
        el.GameObject.name = element.Index + "_[" + element.Id + "]_" + element.ElementType.ToString();

        var elementComponent = (el as ITextComponent);
        elementComponent.SetText(element.Text);

        if (wasNull)
        {
            _elementsPool.Add(el);
        }

        return el;
    }

    private string GetElementName(Element element)
    {
        return element.Index + "_[" + element.Id + "]_" + element.ElementType.ToString();
    }

    private void SaveElements()
    {
        foreach (var element in Elements)
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

        HotkeyController.Instance.AddAsComponent("Save", SaveElements);
        HotkeyController.Instance.AddAsComponent("ShowFileOptons", ToggleFileOptions);
    }
}
