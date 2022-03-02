using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP : Item
{
    public float radius = 3;
    public float empTime = 2;

    // Knock out other players in a radius and play animation
    public override void Activate(Car thisCar)
    {
        SFXManager.Play("EMP Blast");
        foreach (var user in RaceManager.instance.cars)
        {
            var car = user.GetComponent<Car>();
            if (car != thisCar)
            {
                car.WipeOut(empTime);
                car.particles.PlayEMP();
            }
        }

        thisCar.DiscardItem();
    }
}
