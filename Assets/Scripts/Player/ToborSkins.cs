using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToborSkins : MonoBehaviour
{
    [SerializeField]
    public List<Skin> skins;

    public Skin this[int index] => skins[index];
    public int Count => skins.Count;
}
