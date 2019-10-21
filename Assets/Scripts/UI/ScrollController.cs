using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
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

    public RectTransform Content;
    public RectTransform ElementsPanel;

    public void OnScrollChange()
    {
        // Debug.Log(ScrollRect.content.sizeDelta.y);
        Debug.Log(Scrollbar.value);
    }

    public void ScrollToBottom()
    {
        Assets.Scripts.Utils.UIX.UpdateLayout(ScrollRect.transform); // This canvas contains the scroll rect
        ScrollRect.verticalNormalizedPosition = 0f;
    }

    public void KeepElementInView(RectTransform toGameObject)
    {
        Assets.Scripts.Utils.UIX.UpdateLayout(ScrollRect.transform);

        var topSpace = 150f;
        var height = Mathf.Abs(toGameObject.anchoredPosition.y) + topSpace;

        // Debug.Log("height: " + height);
        // Debug.Log("Content.sizeDelta: " + Content.sizeDelta);

        var percent = UsefullUtils.GetValuePercent(height, Content.sizeDelta.y);
        // var diff = UsefullUtils.GetPercent((100 - percent), 50);
        // Debug.Log("diff: " + diff);
        var scrollValue = ((100 - percent)) / 100;
        // Debug.Log("percent: " + percent);
        // Debug.Log("scrollValue: " + scrollValue);

        ScrollRect.verticalNormalizedPosition = scrollValue;
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
