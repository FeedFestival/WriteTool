using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLineSelection : MonoBehaviour
{
    private List<IPrefabComponent> _itemsPool;
    public List<ItemOption> Items;
    public GameObject ItemPrefab;

    public delegate void OnSelectedItem(int value);
    private OnSelectedItem OnSelectedItemCallback;

    public void Init(List<string> options, OnSelectedItem onSelectedItemCallback)
    {
        OnSelectedItemCallback = onSelectedItemCallback;
        InitItemOptions(options);
        InitList();
    }

    private void InitItemOptions(List<string> options)
    {
        Items = new List<ItemOption>();
        int i = 0;
        foreach (string option in options)
        {
            Items.Add(new ItemOption() { Value = i, Text = option });
            i++;
        }
    }

    private void InitList()
    {
        if (_itemsPool != null)
        {
            foreach (IPrefabComponent ipc in _itemsPool)
            {
                ipc.GameObject.SetActive(false);
            }
        }

        foreach (ItemOption item in Items)
        {
            var wasNull = UsefullUtils.CheckInPool(
                (component) => {
                    return component.Id == item.Value;
                },
                ItemPrefab,
                transform,
                out IPrefabComponent el,
                ref _itemsPool
                );

            el.GameObject.name = item.Value + "_" + item.Text;
            (el as InLineItemComponent).Init(item, OnSelectedItemCallback);

            if (wasNull)
            {
                _itemsPool.Add(el);
            }
        }
    }

    internal void Filter(List<int> options)
    {
        foreach (IPrefabComponent item in _itemsPool)
        {
            if (options.Contains(item.Id))
            {
                item.GameObject.SetActive(true);
            }
            else
            {
                item.GameObject.SetActive(false);
            }
        }
    }
}
