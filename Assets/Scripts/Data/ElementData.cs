using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElementData : MonoBehaviour
{
    private static ElementData _elementData;
    public static ElementData Instance { get { return _elementData; } }

    private void Awake()
    {
        _elementData = this;
    }

    public delegate void OnElementsLoaded(List<Element> elements);
    private OnElementsLoaded OnElementsLoadedCallback;

    public void GetElementsByStory(int storyId, OnElementsLoaded onElementsLoadedCallback)
    {
        OnElementsLoadedCallback = onElementsLoadedCallback;
        StartCoroutine(WaitForElements());
    }

    IEnumerator WaitForElements()
    {
        yield return new WaitForSeconds(0.2f);

        OnElementsLoadedCallback(new List<Element>() {
                new Element() {
                    id = 1,
                    Index = 0,
                    ElementType = ElementType.SceneHeading,
                    Text = "Scene Heading"
                },
                new Element() {
                    id = 2,
                    Index = 1,
                    ElementType = ElementType.Action,
                    Text = "Action"
                },
                new Element() {
                    id = 3,
                    Index = 2,
                    ElementType = ElementType.Character,
                    Text = "Character"
                },
                new Element() {
                    id = 4,
                    Index = 3,
                    ElementType = ElementType.Dialog,
                    Text = "Dialog"
                },
                new Element() {
                    id = 5,
                    Index = 4,
                    ElementType = ElementType.Character,
                    Text = "Character 2"
                },
                new Element() {
                    id = 6,
                    Index = 5,
                    ElementType = ElementType.Dialog,
                    Text = "Dialog 2"
                }
            });
    }
}
