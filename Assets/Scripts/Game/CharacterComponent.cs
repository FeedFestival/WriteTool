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

    public void SetText(string text)
    {
        InputField.text = text.ToUpper();
    }
    public string GetText()
    {
        return InputField.text;
    }
}
