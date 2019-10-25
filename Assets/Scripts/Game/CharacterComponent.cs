using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterComponent : MonoBehaviour, IPrefabComponent, ITextComponent, IElementComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    private int _uniqueId;
    public int UniqueId { get { return _uniqueId; } set { _uniqueId = value; } }
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
        TextEditorHotkeyController.Instance.RegisterForEnterKey(() =>
        {
            ElementsController.Instance.AddNewElement(ElementType.Dialog);
            Blurred();
        });
        TextEditorHotkeyController.Instance.RegisterForEscapeKey(() =>
        {
            InputField.DeactivateInputField();
            OnBlur();
        });
        TextEditorHotkeyController.Instance.RegisterBackspaceKey(() =>
        {
            if (string.IsNullOrEmpty(InputField.text))
            {
                _backspaceClick++;
                if (_backspaceClick > 1)
                {
                    InputField.DeactivateInputField();
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
        if (Input.GetMouseButtonDown(0))
        {
            InputField.DeactivateInputField();
            TextEditorHotkeyController.Instance.MainEdit();
        }
        GameService.Instance.Debounce(Blurred, 0.1f);
    }

    private void Blurred()
    {
        if (string.IsNullOrWhiteSpace(InputField.text)) {
            InputField.text = _text = "UNKNOWN";
        }
        TextEditorHotkeyController.Instance.RegisterForEnterKey(null);
    }
}
