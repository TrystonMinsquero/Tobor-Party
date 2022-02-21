using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Item Item { get; private set; }

    public void Equip(Item item)
    {
        Item = item;
    }
}
