using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private const int MaxItems = 2;
    public List<Item> Items { get; private set; } = new List<Item>(2);

    public void AddItem(Item item)
    {
        if (Items.Count >= MaxItems)
            return;

        Items.Add(item);
    }
}
