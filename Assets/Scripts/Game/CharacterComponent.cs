using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterComponent : MonoBehaviour, IPrefabComponent, ITextComponent, IElementComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    public GameObject GameObject { get { return this.gameObject; } }

    private int _typeId;
    public int TypeId { get { return _typeId; } set { _typeId = value; } }

    public InputField InputField;

    private string _text;
    private int _backspaceClick;

    public void SetText(string text)
    {
        _text = text;
        InputField.text = _text.ToUpper();

        InputField.onValueChanged = new InputField.OnChangeEvent();
        InputField.onValueChanged.AddListener(OnEditing);
    }
    public string GetText()
    {
        return InputField.text;
    }

    public void AutoSelect()
    {
        InputField.Select();
        InputField.ActivateInputField();
        OnFocus();
    }

    public void OnFocus()
    {
        _backspaceClick = 0;
        HotkeyController.Instance.RegisterForEnterKey(() =>
        {
            ElementsController.Instance.AddNewElement(ElementType.Dialog);
        });
        HotkeyController.Instance.RegisterForEscapeKey(() =>
        {
            InputField.DeactivateInputField();
            OnBlur();
        });
        HotkeyController.Instance.RegisterBackspaceKey(() =>
        {
            if (string.IsNullOrEmpty(InputField.text))
            {
                _backspaceClick++;
                if (_backspaceClick > 1)
                {
                    InputField.DeactivateInputField();
                    OnBlur();
                    ElementsController.Instance.DeleteElement(Id);
                }
            }
        });
    }

    public void OnEditing(string value)
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            _text = InputField.text;
        }
        InputField.text = InputField.text.ToUpper();
    }

    public void OnBlur()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InputField.text = _text;
        }
        GameService.Instance.Debounce(Blurred, 0.1f);
    }

    private void Blurred()
    {
        HotkeyController.Instance.RegisterForEnterKey(null);
    }
}
