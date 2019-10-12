using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPrefabComponent
{
    int Id { get; set; }
    int UniqueId { get; set; }
    GameObject GameObject { get; }
}

public interface ITextComponent
{
    void SetText(string text);
    void AutoSelect();
    string GetText();
}

public interface IElementComponent
{
    int TypeId { get; set; }
    void OnFocus();
    void OnEditing(string value);
    void OnBlur();
}