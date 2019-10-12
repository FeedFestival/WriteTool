using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionComponent : MonoBehaviour, IPrefabComponent, ITextComponent, IElementComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    private int _uniqueId;
    public int UniqueId { get { return _uniqueId; } set { _uniqueId = value; } }
    public GameObject GameObject { get { return this.gameObject; } }

    private int _typeId;
    public int TypeId { get { return _typeId; } set { _typeId = value; } }

    public ScalableText ScalableText;

    private string _text;
    private int _backspaceClick = 0;

    public void SetText(string text)
    {
        ScalableText.SetText(text);
    }

    public string GetText()
    {
        return ScalableText.InputField.text;
    }

    public void AutoSelect()
    {
        ScalableText.InputField.Select();
        ScalableText.InputField.ActivateInputField();
        OnFocus();
    }

    public void OnFocus()
    {
        GameService.Instance.Debounce(Focussed, 0.1f);
    }

    private void Focussed()
    {
        _backspaceClick = 0;

        HotkeyController.Instance.RegisterForForcedEnterKey(() =>
        {
            ElementsController.Instance.OnAddNewElement();
            ScalableText.InputField.DeactivateInputField();
            SetText(_text.TrimEnd());
            OnBlur();
        });
        HotkeyController.Instance.RegisterForEscapeKey(() =>
        {
            ScalableText.InputField.DeactivateInputField();
            OnBlur();
        });
        HotkeyController.Instance.RegisterBackspaceKey(() =>
        {
            if (string.IsNullOrEmpty(ScalableText.InputField.text))
            {
                _backspaceClick++;
                if (_backspaceClick > 1)
                {
                    ScalableText.InputField.DeactivateInputField();
                    OnBlur();
                    ElementsController.Instance.DeleteElement(UniqueId);
                }
            }
        });
    }

    public void OnEditing(string value)
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            _text = ScalableText.InputField.text;
        }
    }

    public void OnBlur()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ScalableText.InputField.text = _text;
        }
        GameService.Instance.Debounce(Blurred, 0.1f);
    }

    private void Blurred()
    {
        HotkeyController.Instance.RegisterForForcedEnterKey(null);
    }
}
