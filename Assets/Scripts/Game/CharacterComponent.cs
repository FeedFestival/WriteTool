using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterComponent : MonoBehaviour, IPrefabComponent, ITextComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    public GameObject GameObject { get { return this.gameObject; } }

    public Text Text;

    public void SetText(string text)
    {
        Text.text = text.ToUpper();
    }
}
