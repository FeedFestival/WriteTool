using UnityEngine;
using System.Collections;

public class Element
{
    public int id;
    public int TypeId;
    public ElementType ElementType
    {
        set
        {
            TypeId = (int)value;
        }
        get
        {
            return (ElementType)TypeId;
        }
    }
    public string Text;
    public byte base64Text;

    public int Index;
    public bool IsNew;
}

public enum ElementType
{
    SceneHeading,
    Action,
    Character,
    Dialog
}
