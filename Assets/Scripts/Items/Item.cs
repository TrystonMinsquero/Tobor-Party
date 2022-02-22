using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public Vector2 probability; // left is first place, right is last place 
    public Sprite itemImage;

    public abstract void Activate(Car car);
}
