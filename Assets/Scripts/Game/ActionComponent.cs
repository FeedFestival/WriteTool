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

    public void SetText(string text)
    {
        ScalableText.SetText(text);
    }
}
