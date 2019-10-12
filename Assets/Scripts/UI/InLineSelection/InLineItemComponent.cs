using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InLineSelection;

public class InLineItemComponent : MonoBehaviour, IPrefabComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    private int _uniqueId;
    public int UniqueId { get { return _uniqueId; } set { _uniqueId = value; } }
    public GameObject GameObject { get { return this.gameObject; } }

    //public delegate void OnSelectedItem(int value);
    private InLineSelection.OnSelectedItem OnSelectedItemCallback;
    public Text Text;

    public void Init(ItemOption item, OnSelectedItem onSelectedCallback)
    {
        Id = item.Value;
        Text.text = item.Text;
        OnSelectedItemCallback = onSelectedCallback;
    }

    public void OnSelected()
    {
        OnSelectedItemCallback(Id);
    }
}
