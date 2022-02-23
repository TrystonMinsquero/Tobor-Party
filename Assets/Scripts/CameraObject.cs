using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : MonoBehaviour
{
    public float fieldOfView
    {
        get => camera.fieldOfView;
        set => camera.fieldOfView = value;
    }

    public Car car;

    public new Camera camera;

    void Update()
    {
        gameObject.SetActive(car.gameObject.activeSelf);
    }
}
