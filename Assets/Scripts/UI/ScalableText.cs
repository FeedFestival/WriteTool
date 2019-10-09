using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalableText : MonoBehaviour
{
    public InputField InputField;
    public Text Text;

    public void OnChange()
    {
        Text.text = InputField.text;
    }

    internal void SetText(string text)
    {
        if (Text == null)
            Text = GetComponent<Text>();
        Text.text = text;
        InputField.text = text;
        OnChange();
    }
}
