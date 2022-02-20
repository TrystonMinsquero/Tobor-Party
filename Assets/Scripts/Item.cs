using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public AnimationCurve positionProbability; // from 0 to 1. 
    public Sprite itemImage;

    public abstract void Activate(Car car);
}
