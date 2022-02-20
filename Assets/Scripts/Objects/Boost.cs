using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public float boostTime = 0.4f;


    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.TryGetComponent<Car>(out var car))
        {
            car.Boost(boostTime);
        }
    }
}
