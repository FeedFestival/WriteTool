using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ElementsController : MonoBehaviour
{
    private static ElementsController _elementsController;
    public static ElementsController Instance { get { return _elementsController; } }

    public List<Element> Elements;
    public List<IPrefabComponent> _elementsPool;

    public GameObject AddNewButton;
    public InLineSelection InLineSelection;

    private int? currentMaxId;

    // Start is called before the first frame update
    void Awake()
    {
        _elementsController = this;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        InitHotkeys();

        AddNewButton.SetActive(true);
        InLineSelection.gameObject.SetActive(false);
    }

    public void Init(List<Element> elements)
    {
        InitDropdown();
        InitElements(elements);
    }

    public void OnAddNewElement()
    {
        AddNewButton.SetActive(false);
        InLineSelection.gameObject.SetActive(true);

        var options = FilterElementTypes();
        InLineSelection.Filter(options);
    }

    private List<int> FilterElementTypes()
    {
        if (Elements == null)
        {
            Init(new List<Element>());
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

    /*
     * var currentIndex = transform.GetSiblingIndex();
        transform.SetSiblingIndex(currentIndex + 1);
     */
    public void AddNewElement(ElementType elementType)
    {
        if (Elements.Count > 0)
        {
            var lastElementType = Elements[Elements.Count - 1].ElementType;
            if (GameService.Instance.FilterNewElements(elementType, lastElementType) == false)
                return;
        }

        AddNewButton.SetActive(true);
        InLineSelection.gameObject.SetActive(false);

        DetermineCurrentMaxId();

        var element = new Element()
        {
            Id = currentMaxId.Value,
            Text = "Text",
            ElementType = elementType,
            Index = Elements.Count,
            IsNew = true
        };
        Elements.Add(element);
        AddElementInPool(element);

        GameService.Instance.InternalWait(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        });
    }

    private void DetermineCurrentMaxId()
    {
        if (currentMaxId.HasValue == false)
        {
            if (Elements.Count == 0)
            {
                currentMaxId = 0;
            }
            else
            {
                currentMaxId = Elements.Max(e => { return e.Id; });
            }
        }
        currentMaxId += 1;
    }

    private void InitDropdown()
    {
        List<string> options = new List<string>();
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            options.Add(elementType.ToString());
        }
        InLineSelection.Init(options, OnElementTypeSelected);
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

    private void AddElementInPool(Element element)
    {
        var prefab = GameHiddenOptions.Instance.GetPrefabElement(element.ElementType);
        var wasNull = UsefullUtils.CheckInPool(
            element.Id,
            prefab,
            transform,
            out IPrefabComponent el,
            ref _elementsPool
            );

        el.Id = element.Id;
        el.GameObject.name = element.Id + "_" + element.ElementType.ToString();

        var elementComponent = (el as ITextComponent);
        elementComponent.SetText(element.Text);

        if (wasNull)
        {
            _elementsPool.Add(el);
        }
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
