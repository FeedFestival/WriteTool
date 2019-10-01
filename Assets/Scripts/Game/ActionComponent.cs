using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionComponent : MonoBehaviour, IPrefabComponent, ITextComponent, IElementComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    public GameObject GameObject { get { return this.gameObject; } }

    private int _typeId;
    public int TypeId { get { return _typeId; } set { _typeId = value; } }

    public ScalableText ScalableText;

    private int _backspaceClick = 0;

    public void SetText(string text)
    {
        ScalableText.SetText(text);
    }
    public string GetText()
    {
        return ScalableText.InputField.text;
    }

    public void OnFocus()
    {
        GameService.Instance.Debounce(Focussed, 0.1f);
    }

    private void Focussed()
    {
        _backspaceClick = 0;
        
        HotkeyController.Instance.RegisterForEnterKey(() =>
        {
            ElementsController.Instance.OnAddNewElement();
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
                    ElementsController.Instance.DeleteElement(Id);
                }
            }
        });
    }

    public void OnBlur()
    {
        HotkeyController.Instance.RegisterForEnterKey(null);
    }
}
