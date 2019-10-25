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
    private const float scrollTimeSlow = 2f;
    private const float scrollTimeFast = 0.7f;
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

        // Debug.Log("----------------------------------------------------------------------");

        var byWhatPercent = 0f;
        var topSpace = 250f;
        var percent = GetCarretPositionPercent(Mathf.Abs(toGameObject.anchoredPosition.y) + topSpace);
        // Debug.Log("percent: " + percent);
        if (percent > 50) {
            // Debug.Log("Lower part of the document");
            byWhatPercent = Mathf.Abs(100 - (percent - 50));
            // Debug.Log(percent + ", " + byWhatPercent);
            topSpace = UsefullUtils.GetPercent(550, byWhatPercent);
            // Debug.Log(topSpace);
            percent = GetCarretPositionPercent(Mathf.Abs(toGameObject.anchoredPosition.y) + topSpace);
        } else {
            byWhatPercent = Mathf.Abs(100 - (50 - percent));
            // Debug.Log(percent + ", " + byWhatPercent);
            topSpace = UsefullUtils.GetPercent(250f, byWhatPercent);
            // Debug.Log(topSpace);
            percent = GetCarretPositionPercent(Mathf.Abs(toGameObject.anchoredPosition.y) + topSpace);
            // Debug.Log("Upper part of the document");
        }
        // Debug.Log("percent: " + percent);

        var scrollValue = UsefullUtils.InvertPercent(percent) / 100;
        // Debug.Log("scrollValue: " + scrollValue);

        Scroll(scrollValue);
    }

    private float GetCarretPositionPercent(float heightOfCarretFromTopOfPage) {
        // Debug.Log("heightOfCarretFromTopOfPage: " + heightOfCarretFromTopOfPage);
        // Debug.Log("Content.sizeDelta.y: " + Content.sizeDelta.y);
        return UsefullUtils.GetValuePercent(heightOfCarretFromTopOfPage, Content.sizeDelta.y);
    }

    private void Scroll(float value)
    {
        var initialSetting = ScrollRect.verticalNormalizedPosition;

        // Debug.Log("initialSetting: " + initialSetting + ", value: " + value);
        var distance = (Mathf.Abs(Mathf.Abs(value) - Mathf.Abs(initialSetting)) * 100) + 50;
        distance = distance > 100 ? 100 : distance;
        LeanTweenType easing = LeanTweenType.linear;
        // scrollTime = (distance / 100);
        // Debug.Log("scrollTime: " + scrollTime);
        scrollTime = scrollTimeSlow;
        if (distance > 60)
        {
            easing = LeanTweenType.easeOutCirc;
            scrollTime = scrollTimeFast;
        }
        
        // Debug.Log("distance: " + distance);

        if (_scrollAnimationId.HasValue)
        {
            LeanTween.cancel(_scrollAnimationId.Value);
            _scrollAnimationId = null;
        }
        _scrollAnimationId = LeanTween.value(gameObject, initialSetting, value, scrollTime).id;
        LeanTween.descr(_scrollAnimationId.Value).setEase(easing);
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
