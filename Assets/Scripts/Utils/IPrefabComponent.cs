using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPrefabComponent
{
    int Id { get; set; }
    GameObject GameObject { get; }
}

public interface ITextComponent
{
    void SetText(string text);
}