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

        AddNewButton.SetActive(true);
        InLineSelection.gameObject.SetActive(false);
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
        var currentIndex = Elements.Count;
        var lastElementType = Elements[currentIndex - 1].ElementType;

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
        AddNewButton.SetActive(true);
        InLineSelection.gameObject.SetActive(false);

        if (currentMaxId.HasValue == false)
        {
            currentMaxId = Elements.Max(e => { return e.id; });
        }
        currentMaxId += 1;

        var element = new Element()
        {
            id = currentMaxId.Value,
            Text = "Text",
            ElementType = elementType,
            Index = Elements.Count,
            IsNew = true
        };
        Elements.Add(element);
        AddElementInPool(element);
    }

    public void Init(List<Element> elements)
    {
        InitDropdown();



        InitElements(elements);
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

        //foreach (Element element in Elements)
        //{
        //    AddElementInPool(element);
        //}

        GameService.Instance.AsyncForEach(0.05f, Elements.Count, (int i) =>
        {
            AddElementInPool(Elements[i]);  
        });

        //GameService.Instance.InternalWait(1f, () =>
        //{
        //    Canvas.ForceUpdateCanvases();
        //});
    }

    private void AddElementInPool(Element element)
    {
        var prefab = GameHiddenOptions.Instance.GetPrefabElement(element.ElementType);
        var wasNull = UsefullUtils.CheckInPool(
            element.id,
            prefab,
            transform,
            out IPrefabComponent el,
            ref _elementsPool
            );

        el.Id = element.id;
        el.GameObject.name = element.id + "_" + element.ElementType.ToString();

        var elementComponent = (el as ITextComponent);
        elementComponent.SetText(element.Text);

        if (wasNull)
        {
            _elementsPool.Add(el);
        }
    }
}
