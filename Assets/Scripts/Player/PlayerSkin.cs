using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skin
{
    public string name;
    public Transform skinObject;
}

public class PlayerSkin : MonoBehaviour
{
    public Skin Skin => skins[Index];
    public int Index { get; private set; } = 0;

    public List<Skin> skins;

    public Transform currentSkin;
    public Transform currentSkinParent;

    public void NextSkin()
    {
        Index = (Index + 1) % skins.Count;
        
        ApplySkin();
    }

    public void PrevSkin()
    {
        Index = (Index - 1) % skins.Count;
        if (Index < 0) Index += skins.Count;

        ApplySkin();
    }

    public void SetIndex(int index)
    {
        Index = (index % skins.Count);
        if (Index < 0) Index += skins.Count;

        ApplySkin();
    }

    public void ApplySkin()
    {
        var skin = Instantiate(Skin.skinObject);
        skin.parent = currentSkinParent;
        skin.localScale = Vector3.one;
        skin.localRotation = Quaternion.identity;
        skin.localPosition = Vector3.zero;

        Destroy(currentSkin.gameObject);
        currentSkin = skin;
    }
}
