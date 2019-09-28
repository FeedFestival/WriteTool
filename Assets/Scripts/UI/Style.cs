using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using MyBox;

public class Style : MonoBehaviour
{
    [Separator("Screen/Parent Options")]
    public bool UseCustomParent;
    [ConditionalField(nameof(UseCustomParent), false, true)]
    public RectTransform ParentRect;

    [Separator("Size")]

    public bool ApplyWidth;
    [SerializeField, ConditionalField(nameof(ApplyWidth), false, true)]
    public SizeUnit WidthSizeUnit;
    [ConditionalField(nameof(ApplyWidth), false, true)]
    public float Width;

    public bool ApplyHeight;
    [SerializeField, ConditionalField(nameof(ApplyHeight), false, true)]
    public SizeUnit HeightSizeUnit;
    [ConditionalField(nameof(ApplyHeight), false, true)]
    public float Height;

    public void ResetStyle()
    {
        UseCustomParent = false;
        ParentRect = null;

        ApplyWidth = false;
        WidthSizeUnit = SizeUnit.Pixels;
        Width = 0;
        ApplyHeight = false;
        HeightSizeUnit = SizeUnit.Pixels;
        Height = 0;
    }

    public void ApplyAllStyle()
    {
        ((CompileStyles)FindObjectOfType(typeof(CompileStyles))).Init();
    }

    public void ApplyStyle(bool atRunTime = false)
    {
        RectTransform thisRect = gameObject.GetComponent<RectTransform>();
        RectTransform thisParentRect;
        if (UseCustomParent)
        {
            if (ParentRect == null)
            {
                thisParentRect = gameObject.transform.parent.GetComponent<RectTransform>();
                UseCustomParent = false;
            }
            else
            {
                thisParentRect = ParentRect;
            }
        }
        else
        {
            thisParentRect = gameObject.transform.parent.GetComponent<RectTransform>();
            ParentRect = null;
        }

        var newWidth = thisRect.sizeDelta.x;
        var newHeight = thisRect.sizeDelta.y;

        if (ApplyWidth)
        {
            switch (WidthSizeUnit)
            {
                case SizeUnit.Pixels:
                    newWidth = Width;
                    break;
                case SizeUnit.Percent:
                    newWidth = UsefullUtils.GetPercent(thisParentRect.sizeDelta.x, Width);
                    break;
                case SizeUnit.ActualScreen:
                    float actualScreenSizeWidth = (float)Screen.width;
                    if (atRunTime == false)
                    {
                        actualScreenSizeWidth = ((CompileStyles)FindObjectOfType(typeof(CompileStyles))).ReferenceResolution.x;
                    }
                    newWidth = UsefullUtils.GetPercent(actualScreenSizeWidth, Width);
                    break;
                default:
                    //float screenSizeWidth = (float)Screen.width;
                    //if (atRunTime == false)
                    //{
                        float screenSizeWidth = ((CompileStyles)FindObjectOfType(typeof(CompileStyles))).ReferenceResolution.x;
                    //}
                    newWidth = UsefullUtils.GetPercent(screenSizeWidth, Width);
                    break;
            }
        }

        if (ApplyHeight)
        {
            switch (HeightSizeUnit)
            {
                case SizeUnit.Pixels:
                    newHeight = Height;
                    break;
                case SizeUnit.Percent:
                    newHeight = UsefullUtils.GetPercent(thisParentRect.sizeDelta.y, Height);
                    break;
                default:
                    float screenSizeHeight = (float)Screen.height;
                    if (atRunTime == false)
                    {
                        screenSizeHeight = ((CompileStyles)FindObjectOfType(typeof(CompileStyles))).ReferenceResolution.y;
                    }
                    newHeight = UsefullUtils.GetPercent(screenSizeHeight, Height);
                    break;
            }
        }

        thisRect.sizeDelta = new Vector2(newWidth, newHeight);
    }
}

public enum SizeUnit
{
    Pixels,
    Percent,
    ActualScreen,
    Screen
}
