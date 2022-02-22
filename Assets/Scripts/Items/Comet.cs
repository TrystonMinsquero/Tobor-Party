using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : Item
{
    public float time = 10f;
    private Car car;

    public override void Activate(Car car)
    {
        this.car = car;
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {

        car.UsingItem = true;
        car.cometEnabled = true;
        yield return new WaitForSeconds(time);

        car.cometEnabled = false;
        car.UsingItem = false;
        car.DiscardItem();
    }
}
