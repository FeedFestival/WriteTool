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

    public GameObject Carret;

    public List<Element> Elements;
    public List<IPrefabComponent> _elementsPool;

    public GameObject AddNewButton;
    public InLineSelection InLineSelection;

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

    private int GetCarretIndex()
    {
        return Carret.transform.GetSiblingIndex();
    }

    public void MoveCarret(bool down = true)
    {
        var currentIndex = GetCarretIndex();
        var newIndex = currentIndex + 1;
        if (down == false)
        {
            newIndex = currentIndex - 1;
        }

        Debug.Log(Elements.Count);

        if (newIndex == 0 || newIndex == Elements.Count + 1)
        {
            return;
        }
        
        Carret.transform.SetSiblingIndex(newIndex);
        Carret.name = newIndex + "_Carret";

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

        AddNewButton.SetActive(true);
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
        AddElementInPool(element);

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

        el.Id = element.Id;
        el.GameObject.name = GetElementName(element);

        var elementComponent = (el as ITextComponent);
        elementComponent.SetText(element.Text);

        if (wasNull)
        {
            _elementsPool.Add(el);
        }
    }

    private string GetElementName(Element element)
    {
        return element.Index + "_[" + element.Id + "]_" + element.ElementType.ToString();
    }

    private void SaveElements()
    {
        foreach (Element element in Elements)
        {
            if (element.ToDelete)
            {
                ElementData.Instance.DeleteElement(element.Id);
            }
            else
            {
                element.Id = ElementData.Instance.SaveElement(element);
                element.IsNew = false;
            }
        }

        Elements.RemoveAll((e) => { return e.ToDelete; });
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
    }
}
