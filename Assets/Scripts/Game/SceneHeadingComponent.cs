using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHeadingComponent : MonoBehaviour, IPrefabComponent, ITextComponent, IElementComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    public GameObject GameObject { get { return this.gameObject; } }

    private int _typeId;
    public int TypeId { get { return _typeId; } set { _typeId = value; } }

    public InputField InputField;

    private int _backspaceClick;

    public void SetText(string text)
    {
        InputField.text = text.ToUpper();

        InputField.Select();
        InputField.ActivateInputField();

        InputField.onValueChanged = new InputField.OnChangeEvent();
        InputField.onValueChanged.AddListener(OnChange);
    }

    public string GetText()
    {
        return InputField.text;
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
            ElementsController.Instance.AddNewElement(ElementType.Action);
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

    public void OnBlur()
    {
        HotkeyController.Instance.RegisterForEnterKey(null);
    }

    public void OnChange(string value)
    {
        InputField.text = InputField.text.ToUpper();
    }
}
