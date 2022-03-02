using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public float boostTime = 0.4f;


    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.TryGetComponent<Car>(out var car))
        {
            SFXManager.Play("Boost");
            car.Boost(boostTime);
        }
    }
}
