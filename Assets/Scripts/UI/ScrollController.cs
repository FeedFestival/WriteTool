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

    private float scrollTime = 1f;
    private int? _scrollAnimationId;

    public void OnScrollChange()
    {
        // Debug.Log(ScrollRect.content.sizeDelta.y);
        Debug.Log(Scrollbar.value);
    }

    public void ScrollToBottom()
    {
        Assets.Scripts.Utils.UIX.UpdateLayout(ScrollRect.transform); // This canvas contains the scroll rect

        Scroll(0f);
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

        Scroll(scrollValue);
    }

    private void Scroll(float value)
    {
        var initialSetting = ScrollRect.verticalNormalizedPosition;

        if (_scrollAnimationId.HasValue)
        {
            LeanTween.cancel(_scrollAnimationId.Value);
            _scrollAnimationId = null;
        }
        _scrollAnimationId = LeanTween.value(gameObject, initialSetting, value, scrollTime).id;
        LeanTween.descr(_scrollAnimationId.Value).setEase(LeanTweenType.easeOutCirc);
        LeanTween.descr(_scrollAnimationId.Value).setOnUpdate((float val) =>
        {
            ScrollRect.verticalNormalizedPosition = val;
        });
        LeanTween.descr(_scrollAnimationId.Value).setOnComplete(() =>
        {
            LeanTween.cancel(_scrollAnimationId.Value);
            _scrollAnimationId = null;
        });
    }
}
