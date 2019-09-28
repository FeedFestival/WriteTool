using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsController : MonoBehaviour
{
    public List<Element> Elements;
    public List<IPrefabComponent> _elementsPool;

    public GameObject AddNewButton;
    public InLineSelection InLineSelection;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        AddNewButton.SetActive(true);
        InLineSelection.gameObject.SetActive(false);

        Init();
    }

    public void OnAddNewElement()
    {
        AddNewButton.SetActive(false);
        InLineSelection.gameObject.SetActive(true);
    }

    /*
     * var currentIndex = transform.GetSiblingIndex();
        transform.SetSiblingIndex(currentIndex + 1);
     */
    public void AddNewElement(ElementType elementType)
    {
        AddNewButton.SetActive(true);
        InLineSelection.gameObject.SetActive(false);

        var element = new Element()
        {
            id = Elements.Count,
            Text = "Text",
            ElementType = elementType,
            Index = Elements.Count,
            IsNew = true
        };
        Elements.Add(element);
        AddElementInPool(element);
    }

    public void Init()
    {
        InitDropdown();



        InitTestElements(new List<Element>() {
                new Element() {
                    id = 1,
                    ElementType = ElementType.SceneHeading,
                    Text = "Scene Heading"
                },
                new Element() {
                    id = 2,
                    ElementType = ElementType.Action,
                    Text = "Action"
                },
                new Element() {
                    id = 3,
                    ElementType = ElementType.Character,
                    Text = "Character"
                },
                new Element() {
                    id = 4,
                    ElementType = ElementType.Dialog,
                    Text = "Dialog"
                },
                new Element() {
                    id = 5,
                    ElementType = ElementType.Character,
                    Text = "Character 2"
                },
                new Element() {
                    id = 6,
                    ElementType = ElementType.Dialog,
                    Text = "Dialog 2"
                }
            });
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

    public void InitTestElements(List<Element> elements)
    {
        Elements = elements;

        if (_elementsPool != null)
        {
            foreach (IPrefabComponent ipc in _elementsPool)
            {
                ipc.GameObject.SetActive(false);
            }
        }

        foreach (Element element in Elements)
        {
            AddElementInPool(element);
        }
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

    // Update is called once per frame
    void Update()
    {

    }
}
