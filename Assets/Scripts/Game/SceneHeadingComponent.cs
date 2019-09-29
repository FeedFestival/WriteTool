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

    public void SetText(string text)
    {
        InputField.text = text.ToUpper();

        InputField.Select();
        InputField.ActivateInputField();

        InputField.onValueChanged = new InputField.OnChangeEvent();
        InputField.onValueChanged.AddListener(OnChange);
    }

    public void OnFocus()
    {
        GameService.Instance.Debounce(Focussed, 0.1f);
    }

    private void Focussed()
    {
        HotkeyController.Instance.RegisterForEnterKey(() =>
        {
            ElementsController.Instance.AddNewElement(ElementType.Action);
        });
        HotkeyController.Instance.RegisterForEscapeKey(() =>
        {
            InputField.DeactivateInputField();
            OnBlur();
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
