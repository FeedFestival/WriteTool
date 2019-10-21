using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    private static ScrollController _scrollController;
    public static ScrollController Instance { get { return _scrollController; } }

    void Awake()
    {
        _scrollController = this;
    }

    void Start()
    {
        if (ScrollRect == null)
        {
            ScrollRect = GetComponent<ScrollRect>();
        }
    }

    public ScrollRect ScrollRect;
    public Scrollbar Scrollbar;

    public void OnScrollChange()
    {
        // Debug.Log(ScrollRect.content.sizeDelta.y);
        Debug.Log(Scrollbar.value);
    }

    public void KeepElementInView(RectTransform toGameObject)
    {
        // Debug.Log(ScrollRect.content.sizeDelta.y);
        // Debug.Log(Scrollbar.value);

        ScrollRect.content.anchoredPosition =
                (Vector2)ScrollRect.transform.InverseTransformPoint(ScrollRect.content.position)
                - (Vector2)ScrollRect.transform.InverseTransformPoint(toGameObject.position);

        // ScrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(toGameObject);
    }

    public Vector2 GetSnapToPositionToBringChildIntoView(RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = ScrollRect.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
